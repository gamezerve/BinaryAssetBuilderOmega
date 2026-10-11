using System.Reflection;
using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise the real document pipeline without authorizing experimental production stream writes.
internal static class Ep1ArmorDocumentSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile one admitted caller snapshot through real Core output without enabling general production or cache reuse. */
    //-------------------------------------------------------------------------------------------------
    internal static string BuildAuthored(string directory, string authored, int gunPercent, string stockManifest)
    {
        Settings saved = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "ArmorDocumentPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            PluginDescriptor descriptor = new() { IsEnabled = true, AssetTypes = "#all", UseBuildCache = false,
                QualifiedName = "BinaryAssetBuilder.XmlCompiler.Ra3Ep1ArmorPlugin, BinaryAssetBuilder.XmlCompiler" };
            PluginRegistry plugins = new(new[] { descriptor }, TargetPlatform.Win32);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            return WriteConYardOverride(directory, fixtures, processor, plugins,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true }, stockManifest, authored, gunPercent);
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate official schema defaults through document loading/validation/hash stages and force stale-session reload. */
    //-------------------------------------------------------------------------------------------------
    public static void Run(string directory, string? stockManifest = null)
    {
        directory = Path.GetFullPath(directory);
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "ArmorDocumentPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            PluginDescriptor descriptor = new() { IsEnabled = true, AssetTypes = "#all", UseBuildCache = false,
                QualifiedName = "BinaryAssetBuilder.XmlCompiler.Ra3Ep1ArmorPlugin, BinaryAssetBuilder.XmlCompiler" };
            PluginRegistry plugins = new(new[] { descriptor }, TargetPlatform.Win32);
            SessionCache cache = new();
            cache.InitializeCache(new System.Collections.Generic.List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            Require(!plugins.CanReuseCompiledDocuments, "Experimental document reuse was enabled.");
            string source = Path.Combine(directory, "armor-document.xml");
            File.WriteAllText(source, """<AssetDeclaration xmlns="uri:ea.com:eala:asset"><ArmorTemplate id="DocumentArmor" /></AssetDeclaration>""");
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!, options);
            InstanceDeclaration instance = document.SelfInstances.Single();
            Require(instance.XmlNode.Attributes!["Default"]!.Value == "100" && instance.Handle.TypeId == 0x3A6C5E8Eu
                && instance.ProcessingHash == plugins.GetExtendedTypeInformation(instance.Handle.TypeId).ProcessingHash,
                "Full document pipeline did not insert defaults or bind EP1 identity/processor metadata.");
            AssetBuffer first = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance);
            Require(first.InstanceData.Length == 128, "Document-to-profile compilation did not produce empty-list armor tokens.");
            // Reborn: write the actual Core identity and padded checksum, retaining all experimental production restrictions.
            var emptyStreams = WriteCoreStream(Path.Combine(directory, "core-empty"), instance, first, plugins);

            // Reborn: poison only the in-memory cached declaration; an unchanged source file must override this stale state.
            instance.XmlNode.Attributes["Default"]!.Value = "5";
            AssetDeclarationDocument repeated = processor.ProcessDocumentInternal(source, source, null!, options);
            InstanceDeclaration fresh = repeated.SelfInstances.Single();
            AssetBuffer second = plugins.GetPlugin(fresh.Handle.TypeId).ProcessInstance(fresh);
            Require(fresh.XmlNode.Attributes!["Default"]!.Value == "100" && first.InstanceData.SequenceEqual(second.InstanceData)
                && options.UsePrecompiled, "Experimental document reused stale state or mutated caller options.");
            var repeatedStreams = WriteCoreStream(Path.Combine(directory, "core-repeat"), fresh, second, plugins);
            Require(emptyStreams.All(pair => pair.Value.SequenceEqual(repeatedStreams[pair.Key])), "Repeated Core armor streams changed.");
            // Reborn: edit only this generated source, retaining the same asset ID; restore it after proving identity/checksum/payload invalidation.
            string originalSource = File.ReadAllText(source);
            try
            {
                File.WriteAllText(source, originalSource.Replace("id=\"DocumentArmor\"", "id=\"DocumentArmor\" Default=\"50\""));
                var edited = processor.ProcessDocumentInternal(source, source, null!, options).SelfInstances.Single();
                var editedBuffer = plugins.GetPlugin(edited.Handle.TypeId).ProcessInstance(edited);
                var editedStreams = WriteCoreStream(Path.Combine(directory, "core-edited"), edited, editedBuffer, plugins);
                Require(edited.Handle.InstanceId == fresh.Handle.InstanceId && edited.Handle.InstanceHash != fresh.Handle.InstanceHash
                    && !editedBuffer.InstanceData.SequenceEqual(second.InstanceData)
                    && !editedStreams["diagnostic.manifest"].SequenceEqual(repeatedStreams["diagnostic.manifest"])
                    && !editedStreams["diagnostic.bin"].AsSpan(4, 4).SequenceEqual(repeatedStreams["diagnostic.bin"].AsSpan(4, 4)),
                    "Armor source edit did not invalidate Core identity/checksum/payload.");
            }
            finally { File.WriteAllText(source, originalSource); }

            // Reborn: populated documents also receive core-generated TypeId attributes on nested Armor records.
            XmlDocument populatedXml = new();
            populatedXml.Load(Path.Combine(fixtures, "ArmorTokenProbe.xml"));
            string populatedSource = Path.Combine(directory, "armor-populated-document.xml");
            File.WriteAllText(populatedSource, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + populatedXml.DocumentElement!.OuterXml + "</AssetDeclaration>");
            AssetDeclarationDocument populated = processor.ProcessDocumentInternal(populatedSource, populatedSource, null!, options);
            InstanceDeclaration populatedInstance = populated.SelfInstances.Single();
            AssetBuffer populatedBuffer = plugins.GetPlugin(populatedInstance.Handle.TypeId).ProcessInstance(populatedInstance);
            var golden = ArmorTokenSmokeTest.Compile(ArmorTokenSmokeTest.LoadSchemas(fixtures), populatedXml.DocumentElement.OuterXml);
            Require(populatedBuffer.InstanceData.SequenceEqual(golden.InstanceBuffer)
                && populatedBuffer.RelocationData.SequenceEqual(golden.RelocationBuffer), "Full populated document differs from golden-tested compiler output.");
            WriteCoreStream(Path.Combine(directory, "core-populated"), populatedInstance, populatedBuffer, plugins);
            // Reborn: exercise the actual intermediate commit, output selection, manifest writer and linker for one bounded document.
            WriteConYardOverride(directory, fixtures, processor, plugins, options, stockManifest);
            populatedInstance.XmlNode.Attributes!["TypeId"]!.Value = "0";
            bool wrongIdentityRejected = false;
            try { plugins.GetPlugin(populatedInstance.Handle.TypeId).ProcessInstance(populatedInstance); }
            catch (NotSupportedException) { wrongIdentityRejected = true; }
            Require(wrongIdentityRejected, "Profile accepted a tampered core-injected TypeId.");

            // Reborn: replacing cached documents must retain Windows case-insensitive circular-inclusion detection.
            var stack = (System.Collections.Generic.List<string>)typeof(DocumentProcessor)
                .GetField("_documentStack", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(processor)!;
            Require(stack.Count == 0, "Successful document processing left an inclusion stack entry.");
            stack.Add(source.ToUpperInvariant());
            bool cycleRejected = false;
            try { processor.ProcessDocumentInternal(source, source, null!, options); }
            catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.CircularDependency) { cycleRejected = true; }
            finally { stack.RemoveAt(0); }
            Require(cycleRejected, "Fresh experimental documents bypassed circular-inclusion detection.");

            // Reborn: production requests must reject before cache/file access, including the stream-hint fast path.
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = true, UsePrecompiled = true }));
            MethodInfo precompiled = typeof(DocumentProcessor).GetMethod("LoadPrecompiledReference", BindingFlags.Instance | BindingFlags.NonPublic)!;
            Require(precompiled.Invoke(processor, new object?[] { document, "missing", Array.Empty<string>() }) is false,
                "Experimental precompiled reference lookup was not disabled before file access.");
            Require(!File.Exists(Path.ChangeExtension(source, ".manifest")), "Document test unexpectedly emitted a production manifest.");
            Console.WriteLine("EP1 armor document self-test: OK (official schema/Core hashes, padded checksum, tokenized streams/two-reader readback/repeat/corruption, forced session reload, production/precompiled guards)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: publish diagnostic armor streams with real Core identity rows and independently packed checksum, not zero hashes or a fixed checksum marker. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string, byte[]> WriteCoreStream(string directory, InstanceDeclaration instance, AssetBuffer buffer, PluginRegistry plugins)
    {
        var metadata = plugins.GetExtendedTypeInformation(instance.Handle.TypeId);
        Require(metadata.Tokenized && metadata.TypeHash == instance.Handle.TypeHash && instance.Handle.InstanceHash != 0,
            "Core armor identity or tokenized profile differs.");
        DependencyResolutionSmokeTest.Prepare(instance.Document, instance);
        Require(instance.ValidatedReferencedInstances!.Count == 0 && instance.ReferencedInstances.Count == 0, "Standalone armor has dependencies.");
        InstanceDeclaration[] ordered = { instance }; AssetBuffer[] chunks = { buffer }; bool[] flags = { metadata.Tokenized };
        uint checksum = (uint)typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, new object[] { ordered })!;
        // Reborn: independently reproduce one 20-byte identity row in the reviewed 256-byte capacity; never hash just the logical payload.
        byte[] padded = new byte[256];
        uint[] fields = { instance.Handle.TypeId, instance.Handle.TypeHash, instance.Handle.InstanceId, instance.Handle.InstanceHash, 0 };
        for (int index = 0; index < fields.Length; index++) BinaryPrimitives.WriteUInt32LittleEndian(padded.AsSpan(index * 4), fields[index]);
        Require(checksum == BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(padded), "Armor checksum differs from independent padded row.");
        var payloads = BoundedDiagnosticBuild.Serialize(ordered, chunks, checksum, new ReferencedFileBuffer(), flags);
        // Reborn: refuse a tokenized flag count mismatch or a non-armor fingerprint without changing any production registration.
        bool refusedFlags = false;
        try { BoundedDiagnosticBuild.Serialize(ordered, chunks, checksum, new ReferencedFileBuffer(), Array.Empty<bool>()); }
        catch (InvalidDataException) { refusedFlags = true; }
        Require(refusedFlags, "Tokenized flag count mismatch accepted.");
        uint savedTypeHash = instance.Handle.TypeHash;
        try
        {
            instance.Handle.TypeHash ^= 1; bool refusedFingerprint = false;
            try { BoundedDiagnosticBuild.Serialize(ordered, chunks, checksum, new ReferencedFileBuffer(), flags); }
            catch (InvalidDataException) { refusedFingerprint = true; }
            Require(refusedFingerprint, "Unvalidated armor fingerprint relabeled tokenized.");
        }
        finally { instance.Handle.TypeHash = savedTypeHash; }
        payloads["DIAGNOSTIC_ONLY.txt"] = Encoding.UTF8.GetBytes("Reborn: actual Core armor identities/checksum; experimental local ProcessingHash, NOT a playable mod. Production/cache guards remain closed.\n");
        Require(!Directory.Exists(directory) && !File.Exists(directory), "Core armor output already exists.");
        Directory.CreateDirectory(directory);
        foreach (var pair in payloads) File.WriteAllBytes(Path.Combine(directory, pair.Key), pair.Value);
        BoundedDiagnosticBuild.Verify(directory, ordered, chunks, payloads, checksum, Array.Empty<string>(), flags);
        bool refusedRelabel = false;
        try { BoundedDiagnosticBuild.Verify(directory, ordered, chunks, payloads, checksum, Array.Empty<string>(), new[] { false }); }
        catch (InvalidDataException) { refusedRelabel = true; }
        Require(refusedRelabel, "Tokenized armor metadata verified as non-tokenized.");
        // Reborn: verify independent mismatch detection, then restore the known generated file even when the refusal assertion fails.
        string binPath = Path.Combine(directory, "diagnostic.bin");
        byte[] damaged = (byte[])payloads["diagnostic.bin"].Clone(); damaged[4] ^= 1;
        try
        {
            File.WriteAllBytes(binPath, damaged); bool refused = false;
            // Reborn: also change the expected snapshot so rejection must reach independent header/checksum validation, not byte equality alone.
            var matchingDamage = new Dictionary<string, byte[]>(payloads) { ["diagnostic.bin"] = damaged };
            try { BoundedDiagnosticBuild.Verify(directory, ordered, chunks, matchingDamage, checksum, Array.Empty<string>(), flags); }
            catch (InvalidDataException) { refused = true; }
            Require(refused, "Corrupt Core armor stream accepted.");
        }
        finally { File.WriteAllBytes(binPath, payloads["diagnostic.bin"]); }
        BoundedDiagnosticBuild.Verify(directory, ordered, chunks, payloads, checksum, Array.Empty<string>(), flags);
        Console.WriteLine($"  Core armor stream: {instance.Handle.Name}; InstanceHash={instance.Handle.InstanceHash:X8}; checksum={checksum:X8}; BIN={buffer.InstanceData.Length}; diagnostic={directory}");
        return payloads;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the real ConYard identity and non-GUN values; preserve the fixed recipe or compile a separately admitted coefficient. */
    //-------------------------------------------------------------------------------------------------
    private static string WriteConYardOverride(string directory, string fixtures, DocumentProcessor processor, PluginRegistry plugins,
        DocumentProcessor.ProcessOptions options, string? stockManifest, string? admittedXml = null, int gunPercent = 5000)
    {
        XmlDocument probe = new() { XmlResolver = null };
        // Reborn: preserve the legacy fixture recipe while allowing the separately admitted canonical caller snapshot.
        if (admittedXml == null) probe.Load(Path.Combine(fixtures, "AlliedConYardArmorProbe.xml"));
        else probe.LoadXml(admittedXml);
        var root = probe.DocumentElement!;
        var entries = root.ChildNodes.OfType<XmlElement>().ToArray();
        var gun = entries.Single(item => item.GetAttribute("Damage") == "GUN");
        Require(root.GetAttribute("id") == "AlliedConYardArmor" && !root.HasAttribute("inheritFrom")
            && entries.Length == 13 && gun.GetAttribute("Percent") == gunPercent.ToString(System.Globalization.CultureInfo.InvariantCulture), "ConYard probe identity/value differs.");
        string authored = root.OuterXml;
        gun.SetAttribute("Percent", "100");
        var baseline = ArmorTokenSmokeTest.Compile(ArmorTokenSmokeTest.LoadSchemas(fixtures), root.OuterXml);
        if (stockManifest != null) ArmorTokenSmokeTest.CompareGolden(new Dictionary<string, Relo.Chunk>
            { ["ArmorTemplate:AlliedConYardArmor"] = baseline }, stockManifest);
        string source = Path.Combine(directory, "allied-conyard-override.xml");
        File.WriteAllText(source, $"<!-- Reborn: owned test source; only AlliedConYardArmor GUN changes to {gunPercent}. -->\n"
            + "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + authored + "</AssetDeclaration>");
        var document = processor.ProcessDocumentInternal(source, source, null!, options);
        var instance = document.SelfInstances.Single();
        var buffer = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance);
        var expected = new InstanceHandle("ArmorTemplate", "AlliedConYardArmor");
        int gunOffset = 96 + entries.Length * 48 + 32 + Array.IndexOf(entries, gun) * 8 + 4;
        Require(instance.Handle.TypeId == expected.TypeId && instance.Handle.InstanceId == expected.InstanceId
            && instance.Handle.Name == "ArmorTemplate:AlliedConYardArmor" && buffer.InstanceData.Length == 856
            && BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(baseline.InstanceBuffer.AsSpan(gunOffset))) == 1f
            && BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(buffer.InstanceData.AsSpan(gunOffset))) == gunPercent / 100f
            && buffer.RelocationData.SequenceEqual(baseline.RelocationBuffer) && buffer.ImportsData.Length == 0
            && buffer.InstanceData.AsSpan(0, gunOffset).SequenceEqual(baseline.InstanceBuffer.AsSpan(0, gunOffset))
            && buffer.InstanceData.AsSpan(gunOffset + 4).SequenceEqual(baseline.InstanceBuffer.AsSpan(gunOffset + 4)),
            "ConYard override altered identity or bytes outside the single GUN float.");
        DependencyResolutionSmokeTest.Prepare(document, instance);
        string package = WriteManagedStream(directory, processor, document, instance, buffer);
        Console.WriteLine($"  ConYard override: InstanceId={instance.Handle.InstanceId:X8}; GUN 100 -> {gunPercent}; only GUN value slot may differ; stockCompared={stockManifest != null}");
        return package;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate real OutputManager armor commit/link and fail-closed mutations without opening production output. */
    //-------------------------------------------------------------------------------------------------
    private static string WriteManagedStream(string parent, DocumentProcessor processor, AssetDeclarationDocument document,
        InstanceDeclaration instance, AssetBuffer buffer)
    {
        bool linked = Settings.Current.LinkedStreams;
        Settings.Current.LinkedStreams = true;
        try
        {
            using var manager = OutputManager.CreateExperimentalArmorDiagnostic(processor, document, parent);
            var asset = manager.GetBinaryAsset(instance, true);
            asset.Buffer = buffer;
            byte original = buffer.InstanceData[0];
            buffer.InstanceData[0] ^= 1;
            try { ExpectScope(() => asset.Commit()); }
            finally { buffer.InstanceData[0] = original; }
            document.PrepareOutputInstances(manager);
            string intermediate = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName);
            byte[] committed = File.ReadAllBytes(intermediate);
            byte[] damaged = (byte[])committed.Clone(); damaged[^1] ^= 1;
            try { File.WriteAllBytes(intermediate, damaged); ExpectScope(() => manager.CommitManifest(document)); }
            finally { File.WriteAllBytes(intermediate, committed); }
            Require(!File.Exists(manager.OutputDirectory + ".manifest"), "Rejected intermediate emitted manifest.");
            manager.CommitManifest(document);
            manager.LinkStream(document);
            string path = manager.OutputDirectory + ".manifest";
            var parsed = ManifestReader.Read(File.ReadAllBytes(path));
            Require(parsed.Validate().Count == 0 && parsed.Header.Version == 7 && parsed.Header.IsLinked
                && parsed.Header.AllTypesHash == 0x5454A8E9u && parsed.Header.StreamChecksum == document.OutputChecksum
                && parsed.Assets.Count == 1, "Actual OutputManager manifest header differs.");
            var entry = parsed.Assets.Single();
            Require(entry.TypeId == instance.Handle.TypeId && entry.TypeHash == instance.Handle.TypeHash
                && entry.InstanceId == instance.Handle.InstanceId && entry.InstanceHash == instance.Handle.InstanceHash
                && entry.Tokenized == 1 && entry.SourceFile == document.LogicalSourcePath && entry.References.Count == 0,
                "Actual OutputManager manifest identity differs.");
            using var utility = new BinaryAssetBuilder.Utility.Manifest();
            Require(utility.Load(path, false) && utility.AssetCount == 1 && utility.Assets[0].Tokenized
                && utility.Assets[0].InstanceHash == entry.InstanceHash && utility.StreamChecksum == document.OutputChecksum,
                "Actual OutputManager second-reader identity differs.");
            foreach (var stream in new[] { (".bin", 0xBABB0000u, buffer.InstanceData),
                (".relo", 0xBABE0000u, buffer.RelocationData), (".imp", 0xBAB10000u, buffer.ImportsData) })
            {
                byte[] bytes = File.ReadAllBytes(manager.OutputDirectory + stream.Item1);
                Require(bytes.Length == 8 + stream.Item3.Length && BinaryPrimitives.ReadUInt32LittleEndian(bytes) == stream.Item2
                    && BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)) == document.OutputChecksum
                    && bytes.AsSpan(8).SequenceEqual(stream.Item3), "Actual OutputManager linked payload differs.");
            }
            string xml = instance.XmlNode.OuterXml;
            // Reborn: build a fresh four-entry diagnostic BIG only after actual intermediate/linked payload verification.
            ArmorDiagnosticPackage.Exercise(manager.OutputDirectory, instance, buffer, document.OutputChecksum);
            string typeIdValue = instance.XmlNode.Attributes!["TypeId"]!.Value;
            try { instance.XmlNode.Attributes["TypeId"]!.Value = "0"; ExpectScope(() => manager.LinkStream(document)); }
            finally { instance.XmlNode.Attributes["TypeId"]!.Value = typeIdValue; }
            Require(instance.XmlNode.OuterXml == xml, "Scope mutation restore differs.");
            // Reborn: source and settings changes cannot reuse a previously validated output capability.
            byte[] source = File.ReadAllBytes(document.SourcePath);
            try { File.AppendAllText(document.SourcePath, " "); ExpectScope(() => manager.LinkStream(document)); }
            finally { File.WriteAllBytes(document.SourcePath, source); }
            try { Settings.Current.BuildCache = true; ExpectScope(() => manager.LinkStream(document)); }
            finally { Settings.Current.BuildCache = false; }
            ExpectScope(() => manager.LinkStream(null!));
            ExpectScope(() => manager.CleanOutput());
            ExpectPolicy(() => manager.CreateVersionFile(document, "_mod"));
            Require(!processor.Plugins.CanReuseCompiledDocuments, "Diagnostic scope enabled compiled reuse.");
            Console.WriteLine($"  Actual OutputManager: committed .asset + manifest/bin/relo/imp verified; diagnostic={manager.OutputDirectory}");
            // Reborn: return only the package produced and independently verified in this exact scoped output directory.
            return Path.Combine(Path.GetDirectoryName(manager.OutputDirectory)!, "RebornArmorProbe.big");
        }
        finally { Settings.Current.LinkedStreams = linked; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative controls must fail through the scoped policy, not an unrelated I/O failure. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectScope(Action action)
    {
        try { action(); }
        catch (InvalidDataException error) when (error.Message.Contains("Experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Experimental armor scope negative control was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a production denial must identify profile policy rather than a missing cache, schema or file. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException exception) when (exception.Message.Contains("RA3EP1-Armor-Experimental-v1", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Document production request bypassed experimental profile policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject any false document/cache proof at its first failed invariant. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
