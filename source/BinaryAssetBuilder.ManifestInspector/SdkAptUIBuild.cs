
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



    internal static void Package(string inputBig, string aptuiDirectory, string outputBig)
    {
        inputBig = Path.GetFullPath(inputBig);
        aptuiDirectory = Path.GetFullPath(aptuiDirectory);
        outputBig = Path.GetFullPath(outputBig);

        using var archive = BigArchive.Open(inputBig);

        var files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in archive.Entries)
            files.Add(entry.Name, archive.ReadEntry(entry));

        var manifests = Directory.GetFiles(aptuiDirectory, "*.manifest", SearchOption.AllDirectories);

        if (manifests.Length == 0)
            throw new InvalidDataException("No compiled AptUI manifests found.");

        foreach (string manifestPath in manifests)
        {
            var manifest = ManifestReader.Read(File.ReadAllBytes(manifestPath));

            if (!manifest.Header.IsLinked || manifest.Validate().Count != 0)
                throw new InvalidDataException("Invalid AptUI manifest: " + manifestPath);

            string name = Path.GetFileNameWithoutExtension(manifestPath).ToLowerInvariant();

            foreach (string suffix in new[] { "", "_l", "_m" })
            {
                foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
                {
                    byte[] bytes = extension == ".version"
                        ? System.Text.Encoding.ASCII.GetBytes("\r\n")
                        : File.ReadAllBytes(Path.ChangeExtension(manifestPath, extension));

                    string runtime = @"data\aptui\" + name + suffix + extension;

                    if (!files.TryAdd(runtime, bytes))
                        throw new InvalidDataException("Duplicate AptUI BIG entry: " + runtime);
                }
            }
        }

        byte[] package = WorldBuilderAssetResolver.Pack(files);
        File.WriteAllBytes(outputBig, package);

        using var verified = BigArchive.Open(outputBig);

        if (verified.Entries.Count != files.Count)
            throw new InvalidDataException("AptUI BIG entry count mismatch.");

        foreach (var entry in verified.Entries)
        {
            if (!verified.ReadEntry(entry).SequenceEqual(files[entry.Name]))
                throw new InvalidDataException("AptUI BIG verification failed: " + entry.Name);
        }

        Console.WriteLine($"AptUI BIG verified: {manifests.Length} page(s), {verified.Entries.Count} entries.");
    }



}
