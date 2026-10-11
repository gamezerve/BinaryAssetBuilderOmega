using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove real two-armor document/commit/link behavior before admitting a multi-asset alpha package.
internal static class ArmorPairDocumentSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: build both stock-matched yard identities with independent buffers, two readers and mutation refusals; no package/game authority. */
    //-------------------------------------------------------------------------------------------------
    internal static string Run(string directory, string stockManifest, string[]? admittedPair = null)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("Pair test output occupied.");
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            var schema = ArmorTokenSmokeTest.LoadSchemas(fixtures);
            string fixture = File.ReadAllText(Path.Combine(fixtures, "AlliedConYardArmorProbe.xml"));
            string[] names = { "AlliedConYardArmor", "SovietConYardArmor" };
            var authored = names.Select(name => ArmorAuthoringAdmission.Admit(System.Text.Encoding.UTF8.GetBytes(
                fixture.Replace("AlliedConYardArmor", name))).Xml).ToArray();
            // Reborn: caller values must first pass the bounded pair admission; preserve deterministic identity order.
            if (admittedPair != null) authored = ArmorPairAuthoredBuild.Admit(ArmorPairAuthoredBuild.Declaration(admittedPair));
            var expected = new Dictionary<string, Relo.Chunk>();
            for (int index = 0; index < 2; index++)
            {
                var stock = new XmlDocument { XmlResolver = null }; stock.LoadXml(fixture.Replace("AlliedConYardArmor", names[index]));
                stock.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(item => item.GetAttribute("Damage") == "GUN").SetAttribute("Percent", "100");
                var baseline = ArmorTokenSmokeTest.Compile(schema, stock.DocumentElement.OuterXml);
                string name = "ArmorTemplate:" + names[index];
                ArmorTokenSmokeTest.CompareGolden(new Dictionary<string, Relo.Chunk> { [name] = baseline }, stockManifest);
                var compiled = ArmorTokenSmokeTest.Compile(schema, authored[index]);
                ArmorAuthoringAdmission.VerifyPayload(authored[index], baseline.InstanceBuffer, compiled.InstanceBuffer);
                expected.Add(name, compiled);
            }
            Settings.Current = new Settings { BuildCache = false, LinkedStreams = true,
                SchemaPath = Path.Combine(fixtures, "ArmorDocumentPipeline.xsd"), DataRoot = directory, DataPaths = new[] { directory },
                TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            PluginDescriptor descriptor = new() { IsEnabled = true, AssetTypes = "#all", UseBuildCache = false,
                QualifiedName = "BinaryAssetBuilder.XmlCompiler.Ra3Ep1ArmorPlugin, BinaryAssetBuilder.XmlCompiler" };
            PluginRegistry plugins = new(new[] { descriptor }, TargetPlatform.Win32);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory, "armor-pair.xml");
            File.WriteAllText(source, "<!-- Reborn: owned two-armor document test. -->\n<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + string.Concat(authored) + "</AssetDeclaration>");
            var document = processor.ProcessDocumentInternal(source, source, null!, new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true });
            // Reborn: freeze output authority only after each member has passed the actual dependency stage.
            foreach (var instance in document.SelfInstances) DependencyResolutionSmokeTest.Prepare(document, instance);
            Refuse(() => { using var rejected = OutputManager.CreateExperimentalArmorDiagnostic(processor, document, directory); });
            using var manager = OutputManager.CreateExperimentalArmorPairDiagnostic(processor, document, directory);
            Refuse(() => manager.CommitManifest(document));
            foreach (var instance in document.SelfInstances)
            {
                var buffer = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance);
                var golden = expected[instance.Handle.Name];
                if (!buffer.InstanceData.SequenceEqual(golden.InstanceBuffer) || !buffer.RelocationData.SequenceEqual(golden.RelocationBuffer) || buffer.ImportsData.Length != 0)
                    throw new InvalidDataException("Pair Core buffer differs.");
                var asset = manager.GetBinaryAsset(instance, true); asset.Buffer = buffer;
                // Reborn: each member's changed buffer must fail its own frozen proof, including the second member.
                buffer.InstanceData[^1] ^= 1;
                try { Refuse(() => asset.Commit()); } finally { buffer.InstanceData[^1] ^= 1; }
            }
            document.PrepareOutputInstances(manager);
            // Reborn: equal-length corruption of either committed member must block the shared manifest before publication.
            foreach (var asset in manager.Assets.Values)
            {
                string path = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName);
                byte[] original = File.ReadAllBytes(path), damaged = original.ToArray(); damaged[^1] ^= 1;
                try { File.WriteAllBytes(path, damaged); Refuse(() => manager.CommitManifest(document)); }
                finally { File.WriteAllBytes(path, original); }
                if (File.Exists(manager.OutputDirectory + ".manifest")) throw new InvalidDataException("Corrupt pair published manifest.");
            }
            manager.CommitManifest(document); manager.LinkStream(document);
            string manifestPath = manager.OutputDirectory + ".manifest";
            var manifest = ManifestReader.Read(File.ReadAllBytes(manifestPath));
            if (manifest.Validate().Count != 0 || manifest.Assets.Count != 2 || !manifest.Header.IsLinked
                || manifest.Header.Version != 7 || manifest.ReferencedManifests.Count != 0 || manifest.Header.AllTypesHash != 0x5454A8E9u)
                throw new InvalidDataException("Pair manifest differs.");
            // Reborn: independently reconstruct the padded two-row checksum from written manifest identity fields.
            byte[] table = new byte[256]; int rowOffset = 0;
            foreach (var entry in manifest.Assets)
            {
                var instance = document.SelfInstances.Single(item => item.Handle.Name == entry.Name);
                if (entry.TypeId != 0x3A6C5E8Eu || entry.TypeHash != 0xA0E237D8u || entry.InstanceId != instance.Handle.InstanceId
                    || entry.InstanceHash != instance.Handle.InstanceHash || entry.Tokenized != 1 || entry.References.Count != 0
                    || entry.SourceFile != document.LogicalSourcePath || entry.InstanceDataSize != 856 || entry.RelocationDataSize != 8 || entry.ImportsDataSize != 0)
                    throw new InvalidDataException("Pair manifest member identity differs.");
                foreach (uint word in new[] { entry.TypeId, entry.TypeHash, entry.InstanceId, entry.InstanceHash, 0u })
                { BinaryPrimitives.WriteUInt32LittleEndian(table.AsSpan(rowOffset), word); rowOffset += 4; }
            }
            if (FastHash.GetHashCode(table) != manifest.Header.StreamChecksum) throw new InvalidDataException("Pair checksum differs.");
            using var utility = new BinaryAssetBuilder.Utility.Manifest();
            if (!utility.Load(manifestPath, false) || utility.AssetCount != 2 || utility.StreamChecksum != manifest.Header.StreamChecksum)
                throw new InvalidDataException("Pair second reader differs.");
            // Reborn: offsets are cumulative per stream, not the one-asset offset reused for both members.
            foreach (var stream in new[] { (".bin", 0xBABB0000u, 0), (".relo", 0xBABE0000u, 1), (".imp", 0xBAB10000u, 2) })
            {
                byte[] bytes = File.ReadAllBytes(manager.OutputDirectory + stream.Item1); int offset = 8;
                if (BinaryPrimitives.ReadUInt32LittleEndian(bytes) != stream.Item2 || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)) != manifest.Header.StreamChecksum)
                    throw new InvalidDataException("Pair stream header differs.");
                foreach (var entry in manifest.Assets)
                {
                    var chunk = expected[entry.Name]; byte[] payload = stream.Item3 == 0 ? chunk.InstanceBuffer : stream.Item3 == 1 ? chunk.RelocationBuffer : chunk.ImportsBuffer;
                    if (offset + payload.Length > bytes.Length || !bytes.AsSpan(offset, payload.Length).SequenceEqual(payload)) throw new InvalidDataException("Pair stream member slice differs.");
                    offset += payload.Length;
                }
                if (offset != bytes.Length) throw new InvalidDataException("Pair stream trailing bytes differ.");
            }
            // Reborn: equal-handle foreign objects must not impersonate either frozen member, even with unchanged checksum/XML.
            for (int index = 0; index < document.OutputInstances.Count; index++)
            {
                var original = document.OutputInstances[index];
                var foreign = (InstanceDeclaration)typeof(object).GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(original, null)!;
                if (!foreign.Equals(original) || ReferenceEquals(foreign, original)) throw new InvalidDataException("Pair foreign-object control differs.");
                try { document.OutputInstances[index] = foreign; Refuse(() => manager.LinkStream(document)); }
                finally { document.OutputInstances[index] = original; }
            }
            // Reborn: a completed pair cannot turn its private capability into build-cache or production permission.
            try { Settings.Current.BuildCache = true; Refuse(() => manager.LinkStream(document)); }
            finally { Settings.Current.BuildCache = false; }
            byte[] originalSource = File.ReadAllBytes(source);
            try { File.AppendAllText(source, " "); Refuse(() => manager.LinkStream(document)); }
            finally { File.WriteAllBytes(source, originalSource); }
            if (plugins.CanReuseCompiledDocuments) throw new InvalidDataException("Pair scope opened cache reuse.");
            Console.WriteLine($"Armor pair document: PASS (two stock-matched identities, actual Core/intermediate/linked streams, checksum/two readers/member corruption/source/incomplete/single-factory refusals; no BIG or game); manifest={manifestPath}");
            return manager.OutputDirectory;
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: count only intentional experimental-policy refusal as a passing negative control. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action)
    {
        try { action(); } catch (InvalidDataException error) when (error.Message.Contains("Experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Pair scope negative control accepted.");
    }
}
