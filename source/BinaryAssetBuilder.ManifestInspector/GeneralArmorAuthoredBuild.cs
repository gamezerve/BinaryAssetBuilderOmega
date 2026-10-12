using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: general single-armor candidate reuses the reviewed type processor, not a hard-coded yard identity or fixed list recipe.
internal static class GeneralArmorAuthoredBuild
{
    private const string Namespace = "uri:ea.com:eala:asset";
    private static readonly string[] Scalars = { "Default", "DamageScalar", "SideDamageScalar", "RearDamageScalar", "FlankedPenalty" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit bounded one-armor XML with user-chosen identity, unique damage entries and literal percentages; no source semantics are ignored. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Admit(byte[] bytes)
    {
        if (bytes.Length == 0 || bytes.Length > 65536) throw new InvalidDataException("General armor input must be 1..65536 bytes.");
        using var stream = new MemoryStream(bytes, false);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null, IgnoreWhitespace = true, IgnoreComments = true, MaxCharactersInDocument = 65536 });
        var xml = new XmlDocument { XmlResolver = null }; xml.Load(reader);
        if (xml.ChildNodes.Cast<XmlNode>().Any(node => node is not XmlElement && node is not XmlDeclaration))
            throw new InvalidDataException("Unsupported armor document content.");
        var root = xml.DocumentElement!;
        if (root.LocalName == "AssetDeclaration")
        {
            CheckAttributes(root, Array.Empty<string>());
            if (root.NamespaceURI != Namespace || root.ChildNodes.Count != 1 || root.FirstChild is not XmlElement)
                throw new InvalidDataException("General armor candidate requires exactly one ArmorTemplate.");
            root = (XmlElement)root.FirstChild;
        }
        if (root.LocalName != "ArmorTemplate" || root.NamespaceURI != Namespace
            || !Regex.IsMatch(root.GetAttribute("id"), "^[A-Za-z_][A-Za-z0-9_.-]{0,127}$", RegexOptions.CultureInvariant))
            throw new InvalidDataException("Expected one armor with a 1..128-character ASCII identifier.");
        CheckAttributes(root, Scalars.Append("id").ToArray());
        foreach (string scalar in Scalars) if (root.HasAttribute(scalar)) ReadPercentage(root.GetAttribute(scalar));
        if (root.ChildNodes.Count > 64) throw new InvalidDataException("At most 64 armor entries are admitted.");
        var damages = new HashSet<string>(StringComparer.Ordinal);
        foreach (XmlNode node in root.ChildNodes)
        {
            if (node is not XmlElement entry || entry.LocalName != "Armor" || entry.NamespaceURI != Namespace || entry.HasChildNodes)
                throw new InvalidDataException("Only leaf Armor entries are admitted.");
            CheckAttributes(entry, new[] { "Damage", "Percent" });
            if (!entry.HasAttribute("Damage") || !entry.HasAttribute("Percent") || entry.GetAttribute("Damage") == "ALL"
                || !damages.Add(entry.GetAttribute("Damage")))
                throw new InvalidDataException("Each armor entry requires one unique non-ALL damage kind and explicit Percent.");
            ReadPercentage(entry.GetAttribute("Percent"));
        }
        // Reborn: official type graph supplies enum validation and omitted scalar defaults, not a fixture-specific replacement XML.
        var normalized = new XmlDocument { XmlResolver = null }; normalized.LoadXml(root.OuterXml);
        var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        schemas.Add(null, Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "ArmorDocumentPipeline.xsd"));
        schemas.Compile(); normalized.Schemas = schemas;
        normalized.Validate((_, error) => throw new InvalidDataException("Armor schema: " + error.Message));
        return normalized;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allow only reviewed unqualified attributes and the EA default namespace; reject inheritance/expressions/TypeId controls. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckAttributes(XmlElement root, string[] allowed)
    {
        foreach (XmlAttribute attribute in root.Attributes)
            if (!(attribute.Name == "xmlns" && attribute.Value == Namespace)
                && (attribute.NamespaceURI.Length != 0 || !allowed.Contains(attribute.Name, StringComparer.Ordinal)))
                throw new InvalidDataException("Unsupported general armor attribute: " + attribute.Name);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: percentage suffixes follow EA literal normalization, while non-finite, signed and formula values remain refused. */
    //-------------------------------------------------------------------------------------------------
    private static float ReadPercentage(string text)
    {
        string number = text.EndsWith('%') ? text[..^1] : text;
        if (!float.TryParse(number, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float value)
            || !float.IsFinite(value) || value < 0 || value > 10000)
            throw new InvalidDataException("General armor percentages require finite unsigned literals 0..10000, optionally %.");
        return value * 0.01f;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify native values and token/list/relocation shape independently of the processor's emitted buffer. */
    //-------------------------------------------------------------------------------------------------
    private static void VerifyPayload(XmlElement root, AssetBuffer buffer)
    {
        var entries = root.ChildNodes.Cast<XmlElement>().ToArray(); int prefix = 96 + entries.Length * 48;
        if (buffer.InstanceData.Length != 128 + entries.Length * 56 || buffer.ImportsData.Length != 0
            || buffer.RelocationData.Length != (entries.Length == 0 ? 0 : 8))
            throw new InvalidDataException("General armor chunk shape differs.");
        // Reborn: walk each token record/terminator independently, including count-dependent nested tables rather than only native float slots.
        for (int index = 0; index < Scalars.Length; index++)
            Token(index * 12, BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Scalars[index]), 0x645864F3, prefix + 4 + index * 4);
        Token(60, BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Armor"), 0x24D5712F, 84);
        Token(72, 0, 0, 0); Token(84, 0x6C827073, 0x5D448FD8, prefix + 24);
        for (int index = 0; index < entries.Length; index++)
        {
            int nested = 96 + entries.Length * 12 + index * 36;
            Token(96 + index * 12, 0xB151A0EB, 0, nested);
            Token(nested, BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Damage"), 0x46486FF1, prefix + 32 + index * 8);
            Token(nested + 12, BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Percent"), 0x645864F3, prefix + 36 + index * 8);
            Token(nested + 24, 0, 0, 0);
        }
        if (BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(prefix)) != 0)
            throw new InvalidDataException("General armor native base is not empty.");
        for (int index = 0; index < Scalars.Length; index++)
            if (BinaryPrimitives.ReadInt32LittleEndian(buffer.InstanceData.AsSpan(prefix + 4 + index * 4))
                != BitConverter.SingleToInt32Bits(ReadPercentage(root.GetAttribute(Scalars[index]))))
                throw new InvalidDataException("General armor scalar normalization differs.");
        if (BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(prefix + 24)) != entries.Length
            || BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(prefix + 28)) != (entries.Length == 0 ? 0 : prefix + 32))
            throw new InvalidDataException("General armor list count/pointer differs.");
        for (int index = 0; index < entries.Length; index++)
        {
            if (!Enum.TryParse<SageBinaryData.DamageType>(entries[index].GetAttribute("Damage"), out var damage)
                || BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(prefix + 32 + index * 8)) != (uint)damage
                || BinaryPrimitives.ReadInt32LittleEndian(buffer.InstanceData.AsSpan(prefix + 36 + index * 8))
                    != BitConverter.SingleToInt32Bits(ReadPercentage(entries[index].GetAttribute("Percent"))))
                throw new InvalidDataException("General armor entry enum/percentage differs.");
        }
        if (entries.Length != 0 && (BinaryPrimitives.ReadUInt32LittleEndian(buffer.RelocationData) != prefix + 28
            || BinaryPrimitives.ReadUInt32LittleEndian(buffer.RelocationData.AsSpan(4)) != uint.MaxValue))
            throw new InvalidDataException("General armor relocation differs.");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require exact name/type/value words for every reviewed token and zero terminator. */
        //-------------------------------------------------------------------------------------------------
        void Token(int offset, uint name, uint type, int value)
        {
            if (BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(offset)) != name
                || BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(offset + 4)) != type
                || BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(offset + 8)) != value)
                throw new InvalidDataException("General armor token record differs.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: actual Core documents publish one frozen single-armor candidate using the existing experimental capability, never ordinary production. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string source, string directory)
    {
        using var input = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (input.Length == 0 || input.Length > 65536) throw new InvalidDataException("General armor source exceeds bound.");
        byte[] bytes = new byte[(int)input.Length]; input.ReadExactly(bytes); var admitted = Admit(bytes);
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory) || !Directory.Exists(Path.GetDirectoryName(directory)))
            throw new InvalidDataException("General armor output must be a fresh child directory.");
        for (var parent = new DirectoryInfo(Path.GetDirectoryName(directory)!); parent != null; parent = parent.Parent)
            if ((parent.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Redirected armor output refused.");
        Directory.CreateDirectory(directory);
        string authored = Path.Combine(directory, "armor.xml");
        File.WriteAllText(authored, "<AssetDeclaration xmlns=\"" + Namespace + "\">" + admitted.DocumentElement!.OuterXml + "</AssetDeclaration>");
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "ArmorDocumentPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                LinkedStreams = true, CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var descriptor = new PluginDescriptor { IsEnabled = true, AssetTypes = "#all", UseBuildCache = false,
                QualifiedName = "BinaryAssetBuilder.XmlCompiler.Ra3Ep1ArmorPlugin, BinaryAssetBuilder.XmlCompiler" };
            var plugins = new PluginRegistry(new[] { descriptor }, TargetPlatform.Win32);
            var cache = new SessionCache(); cache.InitializeCache(new List<string>());
            var processor = new DocumentProcessor(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            var document = processor.ProcessDocumentInternal(authored, authored, null!, new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
            var instance = document.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(document, instance);
            var buffer = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance); VerifyPayload(admitted.DocumentElement!, buffer);
            // Reborn: no ordinary output authority is introduced; existing scope freezes source, identity and compiler bytes for this manager.
            using var manager = OutputManager.CreateExperimentalArmorDiagnostic(processor, document, directory);
            var asset = manager.GetBinaryAsset(instance, true); asset.Buffer = buffer;
            document.PrepareOutputInstances(manager);
            // Reborn: the generalized identity/list path retains source, normalized XML and committed-buffer immutability controls.
            byte[] snapshot = File.ReadAllBytes(authored);
            try { File.AppendAllText(authored, " "); ExpectScope(() => manager.CommitManifest(document)); }
            finally { File.WriteAllBytes(authored, snapshot); }
            var instanceRoot = (XmlElement)instance.XmlNode; string originalId = instanceRoot.GetAttribute("id");
            try { instanceRoot.SetAttribute("id", "RebornTamperedIdentity"); ExpectScope(() => manager.CommitManifest(document)); }
            finally { instanceRoot.SetAttribute("id", originalId); }
            string intermediate = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName); byte[] committed = File.ReadAllBytes(intermediate);
            try { var changed = committed.ToArray(); changed[^1] ^= 1; File.WriteAllBytes(intermediate, changed); ExpectScope(() => manager.CommitManifest(document)); }
            finally { File.WriteAllBytes(intermediate, committed); }
            manager.CommitManifest(document); manager.LinkStream(document);
            string manifestPath = manager.OutputDirectory + ".manifest";
            var manifest = ManifestReader.Read(File.ReadAllBytes(manifestPath)); var entry = manifest.Assets.Single();
            if (manifest.Validate().Count != 0 || entry.Name != instance.Handle.Name || entry.InstanceId != instance.Handle.InstanceId
                || entry.InstanceHash != instance.Handle.InstanceHash || entry.TypeId != 0x3A6C5E8E || entry.TypeHash != 0xA0E237D8
                || entry.Tokenized != 1 || entry.References.Count != 0 || manifest.ReferencedManifests.Count != 0)
                throw new InvalidDataException("General armor manifest differs.");
            using var utility = new BinaryAssetBuilder.Utility.Manifest();
            if (!utility.Load(manifestPath, false) || utility.AssetCount != 1 || utility.Assets[0].InstanceId != entry.InstanceId
                || utility.Assets[0].InstanceHash != entry.InstanceHash || !utility.Assets[0].Tokenized)
                throw new InvalidDataException("General armor second reader differs.");
            string archive = ArmorDiagnosticPackage.Exercise(manager.OutputDirectory, instance, buffer, document.OutputChecksum);
            // Reborn: only already verified actual streams enter the default-reader family; the diagnostic archive remains separate.
            string candidate = CreateCandidate(directory, manager.OutputDirectory);
            var receipt = new { ArmorId = instance.Handle.InstanceName, EntryCount = admitted.DocumentElement.ChildNodes.Count,
                SourceSha256 = Convert.ToHexString(SHA256.HashData(bytes)), ActualCoreOutputUsed = true,
                AuthoredPayloadVerified = true, ManifestPath = manifestPath, PackagePath = archive,
                PackageSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(archive))),
                InstanceHash = instance.Handle.InstanceHash, DefaultReaderPackage = true, DefaultReaderPackagePath = candidate,
                DefaultReaderPackageSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(candidate))),
                ConfigPath = Path.Combine(Path.GetDirectoryName(candidate)!, "general_1.0.cfg"), AlphaCapabilitiesExpanded = false,
                ScopeCorruptionControlsPassed = 3, CandidateCorruptionControlsPassed = 3,
                GameExecuted = false, RuntimeOverrideProven = false };
            string json = JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(directory, "general-armor-build.json"), json); Console.WriteLine(json);

            //-------------------------------------------------------------------------------------------------
            /** Reborn: a changed bound source/XML/intermediate must be deliberately rejected before publication. */
            //-------------------------------------------------------------------------------------------------
            static void ExpectScope(Action action)
            {
                try { action(); } catch (InvalidDataException) { return; }
                throw new InvalidDataException("General armor scoped corruption was accepted.");
            }
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare/read back fifteen exact default-reader entries and config fragments from verified single-armor streams, without launch authority. */
    //-------------------------------------------------------------------------------------------------
    private static string CreateCandidate(string directory, string prefix)
    {
        string package = Path.Combine(directory, "package"); string config = Path.Combine(package, "general_1.0.cfg");
        if (config.Length > 240 || config.Any(character => character < 32 || character > 126))
            throw new InvalidDataException("Choose a shorter printable ASCII general armor output path.");
        Directory.CreateDirectory(package);
        string[] extensions = { ".manifest", ".bin", ".relo", ".imp", ".version" };
        byte[][] payloads = extensions.Take(4).Select(extension => File.ReadAllBytes(prefix + extension)).ToArray();
        string archivePath = Path.Combine(package, "RebornGeneralArmor.big");
        byte[] bytes = ArmorDiagnosticPackage.Serialize(payloads, true, true);
        using (var output = File.Open(archivePath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) output.Write(bytes);
        string fragment = "add-big RebornGeneralArmor.big\n";
        File.WriteAllText(config, fragment, Encoding.ASCII);
        string sku = Path.Combine(package, "general_1.0.skudef"); File.WriteAllText(sku, fragment, Encoding.ASCII);
        Verify();
        // Reborn: equal-length payload corruption must fail full entry readback; restore only this owned candidate afterward.
        try { var damaged = bytes.ToArray(); damaged[^1] ^= 1; File.WriteAllBytes(archivePath, damaged); Refuse(Verify); }
        finally { File.WriteAllBytes(archivePath, bytes); }
        foreach (string path in new[] { config, sku })
        {
            try { File.WriteAllText(path, "add-big Wrong.big\n", Encoding.ASCII); Refuse(Verify); }
            finally { File.WriteAllText(path, fragment, Encoding.ASCII); }
        }
        Verify(); return archivePath;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: verify exact framing/ranges/names/bytes plus blank CRLF version markers and both configuration fragments. */
        //-------------------------------------------------------------------------------------------------
        void Verify()
        {
            byte[] actual = File.ReadAllBytes(archivePath);
            if (!actual.SequenceEqual(bytes) || File.ReadAllText(config, Encoding.ASCII) != fragment || File.ReadAllText(sku, Encoding.ASCII) != fragment)
                throw new InvalidDataException("General armor candidate bytes/config differ.");
            using var archive = BigArchive.Open(archivePath, 15, 4096);
            if (archive.Entries.Count != 15) throw new InvalidDataException("General armor candidate entry count differs.");
            long offset = BinaryPrimitives.ReadUInt32BigEndian(actual.AsSpan(12)); int index = 0;
            foreach (string variant in new[] { "", "_l", "_m" })
                for (int chunk = 0; chunk < extensions.Length; chunk++)
                {
                    var entry = archive.Entries[index++]; byte[] expected = chunk == 4 ? Encoding.ASCII.GetBytes("\r\n") : payloads[chunk];
                    if (entry.Name != "data\\mod" + variant + extensions[chunk] || entry.Offset != offset || entry.StoredSize != expected.Length
                        || !archive.ReadEntry(entry, 1024 * 1024).SequenceEqual(expected))
                        throw new InvalidDataException("General armor candidate directory/payload differs.");
                    offset += expected.Length;
                }
            if (offset != actual.Length) throw new InvalidDataException("General armor candidate has unexpected trailing bytes.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: candidate corruption controls must produce an intentional refusal, not a silently accepted package. */
        //-------------------------------------------------------------------------------------------------
        static void Refuse(Action action)
        {
            try { action(); } catch (InvalidDataException) { return; }
            throw new InvalidDataException("General armor candidate corruption was accepted.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test identities, list sizes, defaults and literal formats as one authoring batch, preserving old fixed-ID admission. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string directory)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("General armor test output exists.");
        Directory.CreateDirectory(directory);
        string[] contents = {
            "<ArmorTemplate id=\"RebornNewArmor\" />",
            "<ArmorTemplate id=\"RebornNewArmor\" Default=\"75.5%\"><Armor Damage=\"GUN\" Percent=\"250.5\" /></ArmorTemplate>",
            "<ArmorTemplate id=\"OtherNewArmor\" DamageScalar=\"0\"><Armor Damage=\"CANNON\" Percent=\"25%\"/><Armor Damage=\"GUN\" Percent=\"10000\"/></ArmorTemplate>",
            "<ArmorTemplate id=\"RebornEmptySiblingArmor\" />"
        };
        // Reborn: exercise a thirteen-entry list under a new identity through the same path, not the fixed-yard admission helper.
        var longList = new XmlDocument { XmlResolver = null };
        longList.Load(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "AlliedConYardArmorProbe.xml"));
        longList.DocumentElement!.SetAttribute("id", "RebornThirteenEntryArmor");
        contents = contents.Append(longList.DocumentElement.OuterXml).ToArray();
        var packages = new List<byte[]>();
        for (int index = 0; index < contents.Length; index++)
        {
            string xml = contents[index].Contains("xmlns=", StringComparison.Ordinal) ? contents[index]
                : contents[index].Replace("<ArmorTemplate ", "<ArmorTemplate xmlns=\"" + Namespace + "\" ");
            string source = Path.Combine(directory, "input-" + index + ".xml"); File.WriteAllText(source, xml);
            string output = Path.Combine(directory, "build-" + index); Run(source, output);
            using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(output, "general-armor-build.json")));
            byte[] packaged = File.ReadAllBytes(report.RootElement.GetProperty("PackagePath").GetString()!);
            if (packages.Any(previous => previous.SequenceEqual(packaged))) throw new InvalidDataException("General armor source/identity changes produced identical packages.");
            packages.Add(packaged);
            string repeat = Path.Combine(directory, "repeat-" + index); Run(source, repeat);
            using var repeated = JsonDocument.Parse(File.ReadAllText(Path.Combine(repeat, "general-armor-build.json")));
            if (!packages[index].SequenceEqual(File.ReadAllBytes(repeated.RootElement.GetProperty("PackagePath").GetString()!)) || File.ReadAllText(source) != xml)
                throw new InvalidDataException("General armor repeat/source preservation differs.");
            bool refused = false; try { Run(source, output); } catch (InvalidDataException) { refused = true; }
            if (!refused) throw new InvalidDataException("Occupied general armor output accepted.");
            if (!File.ReadAllBytes(report.RootElement.GetProperty("DefaultReaderPackagePath").GetString()!).SequenceEqual(
                File.ReadAllBytes(repeated.RootElement.GetProperty("DefaultReaderPackagePath").GetString()!)))
                throw new InvalidDataException("General armor default-reader repeat differs.");
        }
        string valid = "<ArmorTemplate xmlns=\"" + Namespace + "\" id=\"RebornNewArmor\"><Armor Damage=\"GUN\" Percent=\"5\"/></ArmorTemplate>";
        string[] faults = { valid.Replace("GUN", "UNKNOWN"), valid.Replace("GUN", "ALL"), valid.Replace("Percent=\"5\"", "Percent=\"NaN\""),
            valid.Replace("Percent=\"5\"", "Percent=\"10001\""), valid.Replace("Percent=\"5\"", "Percent=\"=$VALUE\""),
            valid.Replace("id=\"RebornNewArmor\"", "id=\"RebornNewArmor\" inheritFrom=\"Base\""),
            valid.Replace("</ArmorTemplate>", "<Armor Damage=\"GUN\" Percent=\"5\"/></ArmorTemplate>"),
            valid.Replace("Percent=\"5\"", "TypeId=\"1\" Percent=\"5\""), valid.Replace("<Armor Damage", "<Wrong Damage"),
            "<!DOCTYPE ArmorTemplate>" + valid,
            "<AssetDeclaration xmlns=\"" + Namespace + "\">" + valid + valid + "</AssetDeclaration>" };
        foreach (string fault in faults)
        {
            bool refused = false;
            try { Admit(Encoding.UTF8.GetBytes(fault)); } catch (Exception error) when (error is InvalidDataException or XmlException) { refused = true; }
            if (!refused) throw new InvalidDataException("General armor unsupported input accepted.");
        }
        Console.WriteLine("General armor authoring: PASS (new IDs, 0/1/2/13 entries, decimal/%/defaults, Core/scoped output/two readers/default-reader package, repeat and eleven refusals; no alpha/game).");
    }
}
