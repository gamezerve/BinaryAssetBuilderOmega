
using System.Reflection;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class SdkAptUIBuild
{
    internal static Dictionary<string, byte[]> CompileXmlAssets(string xmlPath, string schemaPath)
    {
        xmlPath = Path.GetFullPath(xmlPath);
        schemaPath = Path.GetFullPath(schemaPath);

        if (!File.Exists(xmlPath))
            throw new FileNotFoundException("AptUI XML not found.", xmlPath);

        if (!File.Exists(schemaPath))
            throw new FileNotFoundException("AptUI schema not found.", schemaPath);

        var saved = Settings.Current;

        try
        {
            string sourceRoot = Path.GetDirectoryName(xmlPath)!;

            Settings.Current = new Settings
            {
                BuildCache = false,
                SchemaPath = schemaPath,
                DataRoot = sourceRoot,
                DataPaths = new[] { sourceRoot },
                TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "",
                StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>()
            };

            var descriptor = new PluginDescriptor
            {
                IsEnabled = true,
                AssetTypes = "#all",
                UseBuildCache = false,
                QualifiedName = "BinaryAssetBuilder.XmlCompiler.Plugin, BinaryAssetBuilder.XmlCompiler"
            };

            var plugins = new PluginRegistry(
                new[] { descriptor },
                TargetPlatform.Win32);

            var texturePlugin = new Ep1TexturePlugin();
            texturePlugin.Initialize(TargetPlatform.Win32);
            plugins.AddPlugin(Ep1TextureCompiler.TypeId, texturePlugin);

            var cache = new SessionCache();
            cache.InitializeCache(new List<string>());

            var processor = new DocumentProcessor(
                Settings.Current,
                plugins,
                new VerifierPluginRegistry(
                    Array.Empty<PluginDescriptor>(),
                    TargetPlatform.Win32))
            {
                Cache = cache,
                SchemaSet = new SchemaSet(false)
            };

            string assetPath = Path.Combine(
            Path.GetDirectoryName(xmlPath)!,
            Path.GetFileNameWithoutExtension(xmlPath),
            Path.GetFileName(xmlPath));

            if (!File.Exists(assetPath))
                throw new FileNotFoundException("Included AptUI XML not found.", assetPath);

            var document = processor.ProcessDocumentInternal(
                assetPath,
                assetPath,
                null!,
                new DocumentProcessor.ProcessOptions
                {
                    GenerateOutput = false,
                    UsePrecompiled = true
                });

            var ordered = document.SelfInstances.ToArray();

            Console.WriteLine($"SelfInstances={document.SelfInstances.Count}, Instances={document.Instances.Count}");

            foreach (var instance in document.Instances)
                Console.WriteLine($"Document asset: {instance.Handle.Name}, TypeId=0x{instance.Handle.TypeId:X8}");

            if (ordered.Length == 0)
                throw new InvalidDataException("AptUI document contains no assets.");

            foreach (var instance in ordered)
                DependencyResolutionSmokeTest.Prepare(document, instance);

            var chunks = ordered
                .Select(instance => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance))
                .ToArray();

            var checksumMethod = typeof(AssetDeclarationDocument).GetMethod(
                "ComputeOutputChecksum",
                BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new MissingMethodException("ComputeOutputChecksum");

            uint checksum = (uint)checksumMethod.Invoke(null, new object[] { ordered })!;

            var fingerprints = ordered
                .Select(instance => instance.Handle)
                .GroupBy(handle => handle.TypeId)
                .ToDictionary(
                    group => group.Key,
                    group => (Hash: group.First().TypeHash, Tokenized: false));

            return BoundedDiagnosticBuild.SerializeAssetGraph(
                ordered,
                chunks,
                checksum,
                new ReferencedFileBuffer(),
                new bool[ordered.Length],
                fingerprints);
        }
        finally
        {
            Settings.Current = saved;
        }
    }



    //-------------------------------------------------------------------------------------------------
    /** Reborn: merge compiled AptUI variants with static streams using bounded copying and readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Package(string inputBig, string aptuiDirectory, string outputBig)
    {
        using var archive = BigArchive.Open(Path.GetFullPath(inputBig));
        var originals = archive.Entries.ToDictionary(entry => entry.Name, StringComparer.OrdinalIgnoreCase);
        var sizes = originals.ToDictionary(pair => pair.Key, pair => pair.Value.StoredSize, StringComparer.OrdinalIgnoreCase);
        var aptuiFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var manifests = Directory.GetFiles(Path.GetFullPath(aptuiDirectory), "*.manifest", SearchOption.AllDirectories);

        if (manifests.Length == 0)
            throw new InvalidDataException("No compiled AptUI manifests found.");

        foreach (string manifestPath in manifests)
        {
            var manifest = ManifestReader.Read(File.ReadAllBytes(manifestPath));
            if (!manifest.Header.IsLinked || manifest.Validate().Count != 0)
                throw new InvalidDataException("Invalid AptUI manifest: " + manifestPath);

            string name = Path.GetFileNameWithoutExtension(manifestPath).ToLowerInvariant();
            foreach (string suffix in new[] { "", "_l", "_m" })
            foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
            {
                string source = Path.ChangeExtension(manifestPath, extension);
                string runtime = @"data\aptui\" + name + suffix + extension;
                if (!sizes.TryAdd(runtime, checked((uint)new FileInfo(source).Length)))
                    throw new InvalidDataException("Duplicate AptUI BIG entry: " + runtime);
                aptuiFiles.Add(runtime, source);
            }
        }

        var buffer = new byte[65536];
        var readback = new byte[buffer.Length];
        outputBig = Path.GetFullPath(outputBig);
        bool created = false;
        try
        {
            using (var output = new FileStream(outputBig, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                created = true;
                WorldBuilderAssetResolver.WritePack(output, sizes,
                    (name, destination) => Transfer(name, destination, null, null));
            }

            using var verified = BigArchive.Open(outputBig);
            if (verified.Entries.Count != sizes.Count)
                throw new InvalidDataException("AptUI BIG entry count mismatch.");

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in verified.Entries)
            {
                if (!names.Add(entry.Name) || !sizes.TryGetValue(entry.Name, out uint size) || entry.StoredSize != size)
                    throw new InvalidDataException("AptUI BIG entry differs: " + entry.Name);
                Transfer(entry.Name, null, verified, entry);
            }
            Console.WriteLine($"AptUI BIG verified: {manifests.Length} page(s), {verified.Entries.Count} entries.");
        }
        catch
        {
            if (created) File.Delete(outputBig);
            throw;
        }

        // Reborn: validate source lengths and compare the output without retaining full archive payloads.
        void Transfer(string name, Stream? destination, BigArchive? verification, BigEntry? entry)
        {
            using var source = aptuiFiles.TryGetValue(name, out string? path) ? File.OpenRead(path) : null;
            uint size = sizes[name];
            if (source != null && source.Length != size)
                throw new InvalidDataException("AptUI source length changed: " + path);

            for (long offset = 0; offset < size;)
            {
                int count = (int)Math.Min(buffer.Length, size - offset);
                var chunk = buffer.AsSpan(0, count);
                if (source != null) source.ReadExactly(chunk);
                else archive.ReadEntryRange(originals[name], offset, chunk);

                if (destination != null) destination.Write(chunk);
                else
                {
                    var actual = readback.AsSpan(0, count);
                    verification!.ReadEntryRange(entry!, offset, actual);
                    if (!actual.SequenceEqual(chunk))
                        throw new InvalidDataException("AptUI BIG verification failed: " + name);
                }
                offset += count;
            }
        }
    }

}
