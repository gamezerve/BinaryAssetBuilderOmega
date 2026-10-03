using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit four proven native families in bounded Include graphs without enabling production output.
internal static class BoundedDiagnosticBuild
{
    private static readonly string[] OutputNames = { "diagnostic.manifest", "diagnostic.bin", "diagnostic.relo", "diagnostic.imp", "DIAGNOSTIC_ONLY.txt" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate admitted inputs/mappings, compile isolated native entries and stage readback before an exclusive same-parent directory move. */
    //-------------------------------------------------------------------------------------------------
    internal static string Build(string sourcePath, string outputDirectory, string[] externalPairs, Action<string>? inspectStage = null)
    {
        sourcePath = Path.GetFullPath(sourcePath); outputDirectory = Path.TrimEndingDirectorySeparator(Path.GetFullPath(outputDirectory));
        string parent = Path.GetDirectoryName(outputDirectory) ?? throw new InvalidDataException("A new child output directory is required.");
        if (!Directory.Exists(parent) || Directory.Exists(outputDirectory) || File.Exists(outputDirectory) || Path.GetFileName(outputDirectory).Length == 0)
            throw new InvalidDataException("Output must be a new directory beneath an existing parent; existing output is never replaced.");
        RejectReparseAncestors(parent);
        DiagnosticSourceGraph sourceSnapshots = DiagnosticSourceGraph.Read(sourcePath);
        if (externalPairs.Length > 8) throw new InvalidDataException("At most eight external mappings are admitted.");
        string[] files = new string[externalPairs.Length], runtimeNames = new string[externalPairs.Length];
        List<ManifestDocument> externalMetadata = new();
        List<byte[]> externalSnapshots = new();
        for (int index = 0; index < externalPairs.Length; index++)
        {
            string[] pair = externalPairs[index].Split('=', 2);
            if (pair.Length != 2 || string.IsNullOrWhiteSpace(pair[0]) || pair[0].Contains(';'))
                throw new InvalidDataException("External mapping must be physical.manifest=relative-runtime.manifest.");
            files[index] = Path.GetFullPath(pair[0]); runtimeNames[index] = pair[1].Trim().Replace('/', '\\').ToLowerInvariant();
            if (!File.Exists(files[index]) || new FileInfo(files[index]).Length > 16 * 1024 * 1024)
                throw new InvalidDataException("External manifest is absent or exceeds the 16 MiB metadata limit.");
            byte[] bytes = File.ReadAllBytes(files[index]);
            ManifestDocument metadata = ManifestReader.Read(bytes);
            TypeRegistryAudit.ValidateTarget(metadata.Header.Version, metadata.Header.AllTypesHash);
            if (!metadata.Header.IsLinked || metadata.Validate().Count != 0 || metadata.ReferencedManifests.Any(reference => reference.IsPatch))
                throw new InvalidDataException("External input must be a valid linked EP1 manifest without patch bases.");
            externalMetadata.Add(metadata);
            externalSnapshots.Add(bytes);
        }
        if (files.Distinct(StringComparer.OrdinalIgnoreCase).Count() != files.Length
            || runtimeNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() != runtimeNames.Length)
            throw new InvalidDataException("External physical/runtime mappings must be unique.");
        ReferencedFileBuffer runtime = new();
        Invoke(typeof(OutputManager).GetMethod("AddExternalManifestReferences", BindingFlags.NonPublic | BindingFlags.Static)!,
            runtime, files, string.Join(';', runtimeNames));
        Settings saved = Settings.Current;
        string? staging = null;
        string? inputDirectory = null;
        List<string> inputNames = new();
        try
        {
            // Reborn: compile the approved XML/manifest snapshots, not files that could change between preflight and core lookup.
            inputDirectory = Path.Combine(Path.GetTempPath(), "Reborn-DiagnosticInput-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(inputDirectory);
            string snapshotPath = Path.Combine(inputDirectory, "source.xml");
            sourceSnapshots.Write(inputDirectory, inputNames);
            string[] snapshotManifests = new string[externalSnapshots.Count];
            for (int index = 0; index < externalSnapshots.Count; index++)
            {
                string name = "external-" + index + ".manifest"; inputNames.Add(name);
                snapshotManifests[index] = Path.Combine(inputDirectory, name); File.WriteAllBytes(snapshotManifests[index], externalSnapshots[index]);
            }
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, SchemaPath = Path.Combine(fixtures, "DiagnosticAssetPipeline.xsd"),
                DataRoot = inputDirectory, DataPaths = new[] { inputDirectory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", ProcessedExternalManifests = snapshotManifests, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            Ra3Ep1AttributeModifierPlugin modifiers = new(true); modifiers.Initialize(TargetPlatform.Win32);
            Ra3Ep1ShaderOverridePlugin shaders = new(); shaders.Initialize(TargetPlatform.Win32);
            // Reborn: map only the stock-proven NONE-rule weak filter profile; wider masks/status controls stay closed.
            Ra3Ep1ObjectFilterPlugin filters = new(); filters.Initialize(TargetPlatform.Win32);
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            plugins.AddPlugin(0xC5E07887u, modifiers); plugins.AddPlugin(0xBCC23F6Cu, shaders);
            plugins.AddPlugin(0x44A5973Du, filters);
            // Reborn: use only the isolated empty/two-Sound FX profile with prepared concrete external audio identities.
            Ra3Ep1FXListPlugin fx = new(); fx.Initialize(TargetPlatform.Win32); plugins.AddPlugin(0x86682E78u, fx);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(snapshotPath, snapshotPath, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
            // Reborn: shaders, weak filters and FX precede modifier consumers; admitted FX audio targets are external-only.
            // Reborn: seed self/all assets, then retain real resolution's local closure; unused instance-Include roots are not forced into output.
            Dictionary<InstanceHandle, InstanceDeclaration> selected = new();
            foreach (InstanceDeclaration seed in document.Instances)
            {
                DependencyResolutionSmokeTest.Prepare(document, seed);
                foreach (var visited in DependencyResolutionSmokeTest.Visited(document)) selected[visited.Key] = visited.Value;
            }
            InstanceDeclaration[] ordered = selected.Values.OrderBy(instance => instance.Handle.TypeId switch
                { 0xBCC23F6Cu => 0, 0x44A5973Du => 1, 0x86682E78u => 2, 0xC5E07887u => 3, _ => throw new InvalidDataException("Unadmitted diagnostic asset type.") })
                .ThenBy(instance => instance.Handle.Name, StringComparer.Ordinal).ToArray();
            if (ordered.Length == 0 || ordered.Length > 32) throw new InvalidDataException("Diagnostic input must contain 1–32 admitted roots.");
            // Reborn: share selected external identity/fingerprint checks with fixed native proofs without widening command root admission.
            ValidateExternalDependencies(ordered, externalMetadata);
            AssetBuffer[] chunks = ordered.Select(instance => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance)).ToArray();
            if (chunks.Sum(chunk => (long)chunk.InstanceData.Length + chunk.RelocationData.Length + chunk.ImportsData.Length) > 1024 * 1024)
                throw new InvalidDataException("Compiled diagnostic exceeds its 1 MiB native payload limit.");
            uint checksum = (uint)Invoke(typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum", BindingFlags.NonPublic | BindingFlags.Static)!, (object)ordered)!;
            Dictionary<string, byte[]> payloads = Serialize(ordered, chunks, checksum, runtime);
            staging = Path.Combine(parent, ".reborn-diagnostic-" + Guid.NewGuid().ToString("N"));
            // Reborn: both publication paths are resolved children of the explicitly selected parent; never move or overwrite an existing directory.
            if (Path.GetDirectoryName(staging) != parent || Path.GetDirectoryName(outputDirectory) != parent)
                throw new InvalidDataException("Diagnostic publication paths escaped their parent.");
            Directory.CreateDirectory(staging);
            foreach (var file in payloads) File.WriteAllBytes(Path.Combine(staging, file.Key), file.Value);
            // Reborn: internal fault-injection seam is available to tests only; the command never supplies a publication hook.
            inspectStage?.Invoke(staging);
            Verify(staging, ordered, chunks, payloads, checksum, runtimeNames);
            RejectReparseAncestors(staging);
            Directory.Move(staging, outputDirectory); staging = null;
            return Path.Combine(outputDirectory, "diagnostic.manifest");
        }
        finally
        {
            Settings.Current = saved;
            // Reborn: clean only the five known files of this freshly generated staging directory; unknown files prevent nonrecursive directory removal.
            if (staging != null) CleanOwnedDirectory(staging, OutputNames);
            if (inputDirectory != null) CleanOwnedDirectory(inputDirectory, inputNames);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require unique explicitly mapped external identities and observed shader/audio fingerprints for already prepared roots. */
    //-------------------------------------------------------------------------------------------------
    internal static void ValidateExternalDependencies(InstanceDeclaration[] ordered, IEnumerable<ManifestDocument> externalMetadata)
    {
        // Reborn: a failed recursive attempt must not reach stream serialization through an original-selector-only compiler entry.
        if (ordered.Any(instance => instance.ValidatedReferencedInstances == null
            || instance.ValidatedReferencedInstances.Count != instance.ReferencedInstances.Count))
            throw new InvalidDataException("Prepared diagnostic dependencies are required for every selected root.");
        foreach (InstanceHandle dependency in ordered.SelectMany(instance => instance.ValidatedReferencedInstances!))
        {
            if (ordered.Any(instance => instance.Handle.TypeId == dependency.TypeId && instance.Handle.InstanceId == dependency.InstanceId)) continue;
            ManifestAsset[] matches = externalMetadata.SelectMany(metadata => metadata.Assets)
                .Where(asset => asset.TypeId == dependency.TypeId && asset.InstanceId == dependency.InstanceId).ToArray();
            if (matches.Length != 1) throw new InvalidDataException("External dependency must resolve uniquely to an explicit mapped manifest.");
            if (dependency.TypeId == 0xBCC23F6Cu && (matches[0].TypeHash != 0x3D5B1D16u || matches[0].Tokenized != 0))
                throw new InvalidDataException("External shader fingerprint differs from the proven native EP1 type.");
            // Reborn: selected external sounds must match stock EP1 metadata; this does not validate or rebuild audio payloads.
            uint? audioHash = dependency.TypeId switch { 0x844D7B9Fu => 0x560C2E45u, 0xA3A7AF37u => 0xF79C5A89u, _ => null };
            if (audioHash.HasValue && (matches[0].TypeHash != audioHash.Value || matches[0].Tokenized != 0))
                throw new InvalidDataException("External audio fingerprint differs from the observed stock EP1 type.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize admitted root entries, ordered references and native streams into owned memory before staging publication. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string, byte[]> Serialize(InstanceDeclaration[] ordered, AssetBuffer[] chunks, uint checksum, ReferencedFileBuffer runtime)
    {
        using MemoryStream names = new(), sources = new(), references = new(), entries = new();
        using BinaryWriter refWriter = new(references, Encoding.UTF8, true);
        for (int index = 0; index < ordered.Length; index++)
        {
            var instance = ordered[index]; var chunk = chunks[index];
            // Reborn: each manifest entry retains its actual approved snapshot document identity, not the parent source name.
            string sourceName = Path.GetFileName(instance.Document.SourcePath);
            using AssetEntry entry = new() { TypeId = instance.Handle.TypeId, TypeHash = instance.Handle.TypeHash, InstanceId = instance.Handle.InstanceId,
                InstanceHash = instance.Handle.InstanceHash, Tokenized = false, NameOffset = (int)names.Length, SourceFileNameOffset = (int)sources.Length,
                AssetReferenceOffset = (int)references.Length, AssetReferenceCount = instance.ValidatedReferencedInstances!.Count,
                InstanceDataSize = chunk.InstanceData.Length, RelocationDataSize = chunk.RelocationData.Length, ImportsDataSize = chunk.ImportsData.Length };
            entry.SaveToStream(entries, false); names.Write(Encoding.UTF8.GetBytes(instance.Handle.Name + '\0')); sources.Write(Encoding.UTF8.GetBytes(sourceName + '\0'));
            foreach (var dependency in instance.ValidatedReferencedInstances) { refWriter.Write(dependency.TypeId); refWriter.Write(dependency.InstanceId); }
        }
        refWriter.Flush(); using MemoryStream metadata = new();
        using (BinaryAssetBuilder.Utility.ManifestHeader header = new() { IsLinked = true, AllTypesHash = 0x5454A8E9u, StreamChecksum = checksum,
            AssetCount = (uint)ordered.Length, TotalInstanceDataSize = (uint)chunks.Sum(chunk => chunk.InstanceData.Length),
            MaxInstanceChunkSize = (uint)chunks.Max(chunk => chunk.InstanceData.Length), MaxRelocationChunkSize = (uint)chunks.Max(chunk => chunk.RelocationData.Length),
            MaxImportsChunkSize = (uint)chunks.Max(chunk => chunk.ImportsData.Length), AssetReferenceBufferSize = (uint)references.Length,
            ReferenceManifestNameBufferSize = (uint)runtime.Length, AssetNameBufferSize = (uint)names.Length, SourceFileNameBufferSize = (uint)sources.Length }) header.SaveToStream(metadata, false);
        entries.WriteTo(metadata); references.WriteTo(metadata); runtime.SaveToStream(metadata); names.WriteTo(metadata); sources.WriteTo(metadata);
        Dictionary<string, byte[]> result = new() { ["diagnostic.manifest"] = metadata.ToArray() };
        foreach (var stream in new[] { (Name: "diagnostic.bin", Magic: 0xBABB0000u, Parts: chunks.Select(chunk => chunk.InstanceData)),
            (Name: "diagnostic.relo", Magic: 0xBABE0000u, Parts: chunks.Select(chunk => chunk.RelocationData)),
            (Name: "diagnostic.imp", Magic: 0xBAB10000u, Parts: chunks.Select(chunk => chunk.ImportsData)) })
        {
            using MemoryStream data = new(); using BinaryWriter writer = new(data, Encoding.UTF8, true);
            Invoke(typeof(OutputManager).GetMethod("WriteLinkedStreamHeader", BindingFlags.NonPublic | BindingFlags.Static)!, writer, checksum, stream.Magic);
            foreach (byte[] part in stream.Parts) writer.Write(part); writer.Flush(); result.Add(stream.Name, data.ToArray());
        }
        result.Add("DIAGNOSTIC_ONLY.txt", Encoding.UTF8.GetBytes("Bounded native Include diagnostic only. NOT a playable Uprising mod.\n"
            + "Production/cache gates remain closed; no OutputManager commit/link, packaging or game-load proof.\n"
            + "External mappings serialize runtime names but do not copy or validate native dependency streams.\n"
            + "FX supports empty roots or at most two checked Sound nuggets; concrete audio metadata does not prove native audio payload compatibility.\n"
            + "Filter GameObject weak IDs do not prove target presence and do not become strong imports.\n"));
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact staged bytes and independent metadata/native-offset readback before the new directory becomes visible. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(string directory, InstanceDeclaration[] ordered, AssetBuffer[] chunks, Dictionary<string, byte[]> expected, uint checksum, string[] runtimeNames)
    {
        foreach (var file in expected) if (!File.ReadAllBytes(Path.Combine(directory, file.Key)).SequenceEqual(file.Value)) throw new InvalidDataException("Staged diagnostic bytes differ.");
        string path = Path.Combine(directory, "diagnostic.manifest"); ManifestDocument parsed = ManifestReader.Read(File.ReadAllBytes(path));
        if (parsed.Validate().Count != 0 || parsed.Header.StreamChecksum != checksum || parsed.Assets.Count != ordered.Length
            || parsed.Header.Version != 7 || parsed.Header.ContainerPrefixSize != 4 || !parsed.Header.IsLinked || parsed.Header.AllTypesHash != 0x5454A8E9u
            || parsed.Header.TotalInstanceDataSize != chunks.Sum(chunk => chunk.InstanceData.Length)
            || parsed.Header.MaxInstanceChunkSize != chunks.Max(chunk => chunk.InstanceData.Length)
            || parsed.Header.MaxRelocationChunkSize != chunks.Max(chunk => chunk.RelocationData.Length)
            || parsed.Header.MaxImportsChunkSize != chunks.Max(chunk => chunk.ImportsData.Length)
            || parsed.Header.AssetReferenceBufferSize != ordered.Sum(instance => instance.ValidatedReferencedInstances!.Count) * 8
            || !parsed.ReferencedManifests.SequenceEqual(runtimeNames.Select(name => new ReferencedManifest(name, false))))
            throw new InvalidDataException("Staged manifest validation failed.");
        // Reborn: independently check stream headers and exact lengths, not just equality with the staged byte snapshot.
        foreach (var stream in new[] { (Name: "diagnostic.bin", Magic: 0xBABB0000u, Size: chunks.Sum(chunk => chunk.InstanceData.Length)),
            (Name: "diagnostic.relo", Magic: 0xBABE0000u, Size: chunks.Sum(chunk => chunk.RelocationData.Length)),
            (Name: "diagnostic.imp", Magic: 0xBAB10000u, Size: chunks.Sum(chunk => chunk.ImportsData.Length)) })
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(directory, stream.Name));
            if (bytes.Length != 8 + stream.Size || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(0, 4)) != stream.Magic
                || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4, 4)) != checksum)
                throw new InvalidDataException("Staged stream header/checksum/length validation failed.");
        }
        using BinaryAssetBuilder.Utility.Manifest utility = new();
        if (!utility.Load(path, false) || utility.AssetCount != ordered.Length || utility.StreamChecksum != checksum || utility.AllTypesHash != 0x5454A8E9u)
            throw new InvalidDataException("Utility readback failed.");
        int bin = 8, relo = 8, imp = 8;
        for (int index = 0; index < ordered.Length; index++)
        {
            var asset = parsed.Assets[index]; var instance = ordered[index]; var chunk = chunks[index]; var other = utility.Assets[index];
            if (asset.TypeId != instance.Handle.TypeId || asset.InstanceId != instance.Handle.InstanceId || asset.TypeHash != instance.Handle.TypeHash
                || asset.InstanceHash != instance.Handle.InstanceHash || asset.Name != instance.Handle.Name || asset.Tokenized != 0
                || asset.InstanceDataSize != chunk.InstanceData.Length || asset.RelocationDataSize != chunk.RelocationData.Length || asset.ImportsDataSize != chunk.ImportsData.Length
                || asset.SourceFile != Path.GetFileName(instance.Document.SourcePath)
                || !asset.References.SequenceEqual(instance.ValidatedReferencedInstances!.Select(handle => new AssetId(handle.TypeId, handle.InstanceId)))
                || other.QualifiedName != asset.Name || other.TypeHash != asset.TypeHash || other.InstanceHash != asset.InstanceHash
                || !other.ExternalReferences.Select(handle => new AssetId(handle.TypeId, handle.InstanceId)).SequenceEqual(asset.References)
                || other.LinkedInstanceOffset != bin || other.LinkedRelocationOffset != relo || other.LinkedImportsOffset != imp
                || !AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, chunk.InstanceData.Length).SequenceEqual(chunk.InstanceData)
                || !AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, chunk.RelocationData.Length).SequenceEqual(chunk.RelocationData)
                || !AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, chunk.ImportsData.Length).SequenceEqual(chunk.ImportsData))
                throw new InvalidDataException("Staged asset/native identity readback failed.");
            bin += chunk.InstanceData.Length; relo += chunk.RelocationData.Length; imp += chunk.ImportsData.Length;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject output ancestors redirected through filesystem reparse points rather than publishing into an unexpected location. */
    //-------------------------------------------------------------------------------------------------
    private static void RejectReparseAncestors(string path)
    {
        for (DirectoryInfo? directory = new(path); directory != null; directory = directory.Parent)
            if ((directory.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Diagnostic output parent cannot contain reparse points.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: remove only named files in a freshly owned non-reparse directory; retain unexpected state and report cleanup failure without hiding build results. */
    //-------------------------------------------------------------------------------------------------
    private static void CleanOwnedDirectory(string directory, IEnumerable<string> names)
    {
        try
        {
            if (!Directory.Exists(directory)) return;
            RejectReparseAncestors(directory);
            foreach (string name in names) if (File.Exists(Path.Combine(directory, name))) File.Delete(Path.Combine(directory, name));
            Directory.Delete(directory, false);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or InvalidDataException)
        { Console.Error.WriteLine($"Diagnostic temporary directory retained: {directory}; {error.Message}"); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve real core exception categories when invoking already-tested checksum/runtime/header seams. */
    //-------------------------------------------------------------------------------------------------
    private static object? Invoke(MethodInfo method, params object[] args)
    {
        try { return method.Invoke(null, args); }
        catch (TargetInvocationException error) when (error.InnerException != null) { ExceptionDispatchInfo.Capture(error.InnerException).Throw(); throw; }
    }
}
