using System.Buffers.Binary;
using System.Reflection;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: write only a fixed two-family diagnostic graph using existing serializers; never bypass experimental production output policies.
internal static class ModifierShaderStreamSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile resolved declarations, serialize two assets and ordered identities, then independently read every native slice and reject corruption. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-ModifierShaderStream-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, SchemaPath = Path.Combine(fixtures, "ModifierShaderPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            Ra3Ep1AttributeModifierPlugin modifierPlugin = new(true); modifierPlugin.Initialize(TargetPlatform.Win32);
            Ra3Ep1ShaderOverridePlugin shaderPlugin = new(); shaderPlugin.Initialize(TargetPlatform.Win32);
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            plugins.AddPlugin(0xC5E07887u, modifierPlugin); plugins.AddPlugin(0xBCC23F6Cu, shaderPlugin);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            XmlDocument sourceShaders = new(); sourceShaders.Load(Path.Combine(fixtures, "ShaderOverrideProbe.xml"));
            XmlElement iron = sourceShaders.DocumentElement!.ChildNodes.OfType<XmlElement>()
                .Single(element => element.GetAttribute("id") == "ShaderOverride_ObjectsIronCurtain");
            File.WriteAllText(Path.Combine(directory, "shader.xml"), "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + iron.OuterXml + "</AssetDeclaration>");
            string parent = Path.Combine(directory, "parent.xml");
            File.WriteAllText(parent, """
                <AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="instance" source="shader.xml" /></Includes>
                <AttributeModifier id="RebornStreamModifier" StartFX="RebornStreamFX" Shader="ShaderOverride_ObjectsIronCurtain" />
                </AssetDeclaration>
                """);
            InstanceHandle external = new("FXList", "RebornStreamFX");
            string externalPath = Path.Combine(directory, "external.manifest");
            ExternalLinkSmokeTest.WriteFixture(externalPath, external, new ReferencedFileBuffer());
            Settings.Current.ProcessedExternalManifests = new[] { externalPath };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(parent, parent, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true });
            InstanceDeclaration modifier = document.SelfInstances.Single();
            DependencyResolutionSmokeTest.Prepare(document, modifier);
            InstanceDeclaration shader = document.FindInstance(new InstanceHandle("ShaderOverride", iron.GetAttribute("id")), FindLocation.None, out _);
            Require(shader != null && modifier.ValidatedReferencedInstances!.Count == 2 && modifier.AllDependentInstances!.Single() == shader.Handle,
                "Diagnostic writer input is not the resolved local/external graph.");
            // Reborn: this explicit two-node order is diagnostic only, not a replacement for the production stable sorter.
            InstanceDeclaration[] ordered = new[] { shader!, modifier };
            AssetBuffer[] chunks = ordered.Select(instance => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance)).ToArray();
            Require(chunks[0].InstanceData.Length == 180 && chunks[1].InstanceData.Length == 60,
                "Diagnostic compiler roots differ from the bounded fixture.");
            MethodInfo checksumMethod = typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum", BindingFlags.NonPublic | BindingFlags.Static)!;
            uint checksum = (uint)checksumMethod.Invoke(null, new object[] { ordered })!;
            string path = Write(Path.Combine(directory, "first"), ordered, chunks, checksum);
            Verify(path, ordered, chunks, external, checksum);
            CheckLegacyOffsets(path);
            string repeated = Write(Path.Combine(directory, "repeat"), ordered, chunks, checksum);
            Verify(repeated, ordered, chunks, external, checksum);
            foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
                Require(File.ReadAllBytes(Path.ChangeExtension(path, extension)).SequenceEqual(File.ReadAllBytes(Path.ChangeExtension(repeated, extension))),
                    "Repeated two-family serialization differs.");
            // Reborn: mutate only diagnostic-owned bytes, restoring them after each failed readback check.
            string binPath = Path.ChangeExtension(path, ".bin"), impPath = Path.ChangeExtension(path, ".imp");
            byte[] originalBin = File.ReadAllBytes(binPath), originalImp = File.ReadAllBytes(impPath);
            byte[] badHeader = (byte[])originalBin.Clone(); badHeader[4] ^= 1;
            CheckCorruption(path, binPath, badHeader, originalBin, ordered, chunks, external, checksum);
            byte[] badSelector = (byte[])originalBin.Clone(); BinaryPrimitives.WriteUInt32LittleEndian(badSelector.AsSpan(8 + 180 + 56), 3);
            CheckCorruption(path, binPath, badSelector, originalBin, ordered, chunks, external, checksum);
            CheckCorruption(path, impPath, originalImp[..^1], originalImp, ordered, chunks, external, checksum);
            // Reborn: the identity checksum excludes dependency identities; swapped manifest targets must still fail explicit readback checks.
            byte[] originalManifest = File.ReadAllBytes(path), swapped = (byte[])originalManifest.Clone();
            Array.Copy(originalManifest, 156, swapped, 148, 8); Array.Copy(originalManifest, 148, swapped, 156, 8);
            CheckCorruption(path, path, swapped, originalManifest, ordered, chunks, external, checksum);
            Verify(path, ordered, chunks, external, checksum);
            ExpectPolicy(plugins.ValidateProductionOutput);
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!, new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Console.WriteLine($"Modifier/shader stream self-test: OK (two resolved compiler entries, manifest and utility readback, native offsets/selectors, exact deterministic slices, checksum/header/length corruption, production gate); diagnostic={path}");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize only the test's explicit two-root order and dependencies with production utility/header writers, never OutputManager.CommitManifest. */
    //-------------------------------------------------------------------------------------------------
    private static string Write(string directory, InstanceDeclaration[] ordered, AssetBuffer[] chunks, uint checksum)
    {
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "modifier-shader.manifest");
        using MemoryStream names = new(), sources = new(), references = new(), entries = new();
        using BinaryWriter referenceWriter = new(references, Encoding.UTF8, true);
        for (int index = 0; index < ordered.Length; index++)
        {
            InstanceDeclaration instance = ordered[index]; AssetBuffer chunk = chunks[index];
            InstanceHandle[] dependencies = instance.ValidatedReferencedInstances!.ToArray();
            using AssetEntry entry = new() { TypeId = instance.Handle.TypeId, TypeHash = instance.Handle.TypeHash,
                InstanceId = instance.Handle.InstanceId, InstanceHash = instance.Handle.InstanceHash, Tokenized = false,
                NameOffset = checked((int)names.Length), SourceFileNameOffset = checked((int)sources.Length),
                AssetReferenceOffset = checked((int)references.Length), AssetReferenceCount = dependencies.Length,
                InstanceDataSize = chunk.InstanceData.Length, RelocationDataSize = chunk.RelocationData.Length, ImportsDataSize = chunk.ImportsData.Length };
            entry.SaveToStream(entries, false);
            names.Write(Encoding.UTF8.GetBytes(instance.Handle.Name + '\0'));
            sources.Write(Encoding.UTF8.GetBytes("Tests/ModifierShaderStream/" + (index == 0 ? "shader.xml" : "parent.xml") + '\0'));
            foreach (InstanceHandle dependency in dependencies) { referenceWriter.Write(dependency.TypeId); referenceWriter.Write(dependency.InstanceId); }
        }
        referenceWriter.Flush();
        ReferencedFileBuffer runtime = new(); runtime.AddReference("external.manifest", false);
        using MemoryStream manifest = new();
        using (BinaryAssetBuilder.Utility.ManifestHeader header = new() { IsLinked = true, AllTypesHash = 0x5454A8E9u,
            StreamChecksum = checksum, AssetCount = (uint)ordered.Length, TotalInstanceDataSize = (uint)chunks.Sum(chunk => chunk.InstanceData.Length),
            MaxInstanceChunkSize = (uint)chunks.Max(chunk => chunk.InstanceData.Length), MaxRelocationChunkSize = (uint)chunks.Max(chunk => chunk.RelocationData.Length),
            MaxImportsChunkSize = (uint)chunks.Max(chunk => chunk.ImportsData.Length), AssetReferenceBufferSize = (uint)references.Length,
            ReferenceManifestNameBufferSize = (uint)runtime.Length, AssetNameBufferSize = (uint)names.Length, SourceFileNameBufferSize = (uint)sources.Length })
            header.SaveToStream(manifest, false);
        entries.WriteTo(manifest); references.WriteTo(manifest); runtime.SaveToStream(manifest); names.WriteTo(manifest); sources.WriteTo(manifest);
        File.WriteAllBytes(path, manifest.ToArray());
        MethodInfo writeHeader = typeof(OutputManager).GetMethod("WriteLinkedStreamHeader", BindingFlags.NonPublic | BindingFlags.Static)!;
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u, Parts: chunks.Select(chunk => chunk.InstanceData)),
            (Extension: ".relo", Magic: 0xBABE0000u, Parts: chunks.Select(chunk => chunk.RelocationData)),
            (Extension: ".imp", Magic: 0xBAB10000u, Parts: chunks.Select(chunk => chunk.ImportsData)) })
        {
            using MemoryStream payload = new(); using BinaryWriter writer = new(payload, Encoding.UTF8, true);
            writeHeader.Invoke(null, new object[] { writer, checksum, stream.Magic });
            foreach (byte[] part in stream.Parts) writer.Write(part);
            writer.Flush(); File.WriteAllBytes(Path.ChangeExtension(path, stream.Extension), payload.ToArray());
        }
        File.WriteAllText(Path.Combine(directory, "DIAGNOSTIC_ONLY.txt"),
            "Two-family synthetic document/compiler/serializer readback only. Not a playable Uprising mod.\n"
            + "Production registry and cache gates remain closed. No OutputManager commit/link, packaging or in-game loading proof.\n"
            + "Checksum uses the existing identity algorithm, not a payload digest. External FX is metadata-only and not compiled.\n"
            + "external.manifest is a serialized runtime-name fixture, not a packaged dependency stream.\n");
        return path;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently parse metadata, verify utility reader offsets, exact concatenated slices and decoded local/external import identities. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(string path, InstanceDeclaration[] ordered, AssetBuffer[] chunks, InstanceHandle external, uint checksum)
    {
        ManifestDocument parsed = ManifestReader.Read(File.ReadAllBytes(path));
        Require(parsed.Validate().Count == 0 && parsed.Header.Version == 7 && parsed.Header.ContainerPrefixSize == 4
            && parsed.Header.IsLinked && parsed.Header.AllTypesHash == 0x5454A8E9u && parsed.Header.StreamChecksum == checksum
            && parsed.Header.TotalInstanceDataSize == 240 && parsed.Header.MaxInstanceChunkSize == 180
            && parsed.Header.MaxRelocationChunkSize == 52 && parsed.Header.MaxImportsChunkSize == 12
            && parsed.Header.AssetReferenceBufferSize == 16 && parsed.Assets.Count == 2
            && parsed.ReferencedManifests.Single() == new ReferencedManifest("external.manifest", false), "Two-family manifest metadata differs.");
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u, Size: chunks.Sum(chunk => chunk.InstanceData.Length)),
            (Extension: ".relo", Magic: 0xBABE0000u, Size: chunks.Sum(chunk => chunk.RelocationData.Length)),
            (Extension: ".imp", Magic: 0xBAB10000u, Size: chunks.Sum(chunk => chunk.ImportsData.Length)) })
        {
            byte[] data = File.ReadAllBytes(Path.ChangeExtension(path, stream.Extension));
            Require(data.Length == 8 + stream.Size && Read(data, 0) == stream.Magic && Read(data, 4) == checksum,
                "Diagnostic stream header/checksum/exact length differs.");
        }
        using BinaryAssetBuilder.Utility.Manifest utility = new();
        Require(utility.Load(path, false) && utility.AssetCount == 2 && utility.StreamChecksum == checksum,
            "Utility reader rejected diagnostic manifest.");
        int bin = 8, relo = 8, imp = 8;
        for (int index = 0; index < ordered.Length; index++)
        {
            ManifestAsset asset = parsed.Assets[index]; InstanceDeclaration instance = ordered[index]; AssetBuffer chunk = chunks[index];
            var utilityAsset = utility.Assets[index];
            Require(utilityAsset.LinkedInstanceOffset == bin && utilityAsset.LinkedRelocationOffset == relo && utilityAsset.LinkedImportsOffset == imp,
                $"Utility linked offsets differ for {asset.Name}: got {utilityAsset.LinkedInstanceOffset}/{utilityAsset.LinkedRelocationOffset}/{utilityAsset.LinkedImportsOffset}, expected {bin}/{relo}/{imp}.");
            Require(asset.Name == instance.Handle.Name && asset.TypeId == instance.Handle.TypeId && asset.InstanceId == instance.Handle.InstanceId
                && asset.TypeHash == instance.Handle.TypeHash && asset.InstanceHash == instance.Handle.InstanceHash && asset.Tokenized == 0
                && asset.References.SequenceEqual(instance.ValidatedReferencedInstances!.Select(handle => new AssetId(handle.TypeId, handle.InstanceId)))
                && asset.InstanceDataSize == chunk.InstanceData.Length && asset.RelocationDataSize == chunk.RelocationData.Length && asset.ImportsDataSize == chunk.ImportsData.Length
                && utilityAsset.ExternalReferences.Select(handle => new AssetId(handle.TypeId, handle.InstanceId)).SequenceEqual(asset.References),
                "Diagnostic entry identities, references or utility stream offsets differ.");
            byte[] actualBin = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, chunk.InstanceData.Length);
            Require(actualBin.SequenceEqual(chunk.InstanceData)
                && AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, chunk.RelocationData.Length).SequenceEqual(chunk.RelocationData)
                && AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, chunk.ImportsData.Length).SequenceEqual(chunk.ImportsData),
                "Diagnostic native slice differs on readback.");
            if (index == 1)
            {
                uint fx = Read(actualBin, 12), shader = Read(actualBin, (int)Read(actualBin, 40));
                Require(fx > 0 && fx <= asset.References.Count && shader > 0 && shader <= asset.References.Count
                    && asset.References[(int)fx - 1] == new AssetId(external.TypeId, external.InstanceId)
                    && asset.References[(int)shader - 1] == new AssetId(parsed.Assets[0].TypeId, parsed.Assets[0].InstanceId),
                    "Serialized imports select the wrong local/external identities.");
            }
            bin += chunk.InstanceData.Length; relo += chunk.RelocationData.Length; imp += chunk.ImportsData.Length;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: corrupt only an owned fixture, require readback failure and restore original bytes before further checks. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckCorruption(string path, string file, byte[] bad, byte[] original, InstanceDeclaration[] ordered, AssetBuffer[] chunks, InstanceHandle external, uint checksum)
    {
        File.WriteAllBytes(file, bad);
        try
        {
            try { Verify(path, ordered, chunks, external, checksum); }
            catch (InvalidDataException) { return; }
            throw new InvalidOperationException("Corrupt diagnostic stream passed readback.");
        }
        finally { File.WriteAllBytes(file, original); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain unprefixed-v7 offsets and the compiled reader's v6 rejection without claiming legacy native payload compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckLegacyOffsets(string path)
    {
        byte[] unprefixed = File.ReadAllBytes(path)[4..];
        foreach (bool legacy in new[] { false, true })
        {
            byte[] metadata = (byte[])unprefixed.Clone();
            if (legacy) { metadata[0] = 0; metadata[1] = 1; metadata[2] = 6; metadata[3] = 0; }
            string probe = Path.Combine(Path.GetDirectoryName(path)!, legacy ? "legacy-offset-only.manifest" : "unprefixed-offset-only.manifest");
            File.WriteAllBytes(probe, metadata);
            using BinaryAssetBuilder.Utility.Manifest reader = new();
            if (legacy)
            {
                bool rejected = false;
                try { reader.Load(probe, false); }
                catch (ArgumentException error) when (error.Message.Contains("Unsupported file version", StringComparison.Ordinal)) { rejected = true; }
                Require(rejected, "VERSION7 utility reader unexpectedly accepted a legacy v6 manifest.");
                continue;
            }
            Require(reader.Load(probe, false) && reader.Version == 7
                && reader.Assets[0].LinkedInstanceOffset == 4 && reader.Assets[0].LinkedRelocationOffset == 4 && reader.Assets[0].LinkedImportsOffset == 4
                && reader.Assets[1].LinkedInstanceOffset == 184 && reader.Assets[1].LinkedRelocationOffset == 56 && reader.Assets[1].LinkedImportsOffset == 4,
                "EP1 prefix fix changed legacy/unprefixed utility offsets.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: diagnostic serializers must not relax registered experimental production policies. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("Experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Diagnostic serialization enabled production output.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode bounded little-endian diagnostic words independently of host native pointers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail the diagnostic writer proof on its first compiler, identity, stream or policy mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
