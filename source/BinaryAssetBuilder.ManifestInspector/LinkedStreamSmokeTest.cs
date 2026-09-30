using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise real intermediate commits and linking with synthetic bytes, never approving an EP1 production profile.
internal static class LinkedStreamSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify deterministic commits, coordinated stream repair and direct experimental write guards. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string outputDirectory)
    {
        Settings previous = Settings.Current;
        try
        {
            // Reborn: each run owns a fresh tiny fixture directory and cannot alter existing user/game streams.
            string root = Path.Combine(Path.GetFullPath(outputDirectory), "link-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            Settings.Current = new Settings { BuildCache = false, AlwaysTouchCache = false, BigEndian = false,
                TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", LinkedStreams = true };
            // Reborn: the built-in null plugin is only a diagnostic fixture context; no compiler/type table is fabricated.
            PluginRegistry registry = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            DocumentProcessor processor = new(Settings.Current, registry, null!);
            using OutputManager manager = new(processor, null, Path.Combine(root, "synthetic"), root, null!, null!, Array.Empty<string>());
            AssetDeclarationDocument document = new();
            Type stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic)!;
            object state = Activator.CreateInstance(stateType, true)!;
            stateType.GetField("OutputChecksum")!.SetValue(state, 0x10203040u);
            stateType.GetField("SourcePathFromRoot")!.SetValue(state, "DiagnosticOnly");
            typeof(AssetDeclarationDocument).GetField("_current", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(document, state);
            List<AssetBuffer> buffers = new();
            List<BinaryAsset> assets = new();
            for (int i = 0; i < 2; i++)
            {
                XmlDocument xml = new();
                xml.LoadXml($"<ArmorTemplate xmlns=\"uri:ea.com:eala:asset\" id=\"Synthetic{i}\" />");
                InstanceDeclaration declaration = new(document) { XmlNode = xml.DocumentElement! };
                // Reborn: arbitrary nonzero hashes identify synthetic fixture assets, not EA-compatible compiled declarations.
                declaration.Handle.TypeHash = 0x11223344u;
                declaration.Handle.InstanceHash = (uint)(0x55667788 + i);
                document.OutputInstances.Add(declaration);
                AssetBuffer buffer = new() { InstanceData = new byte[] { (byte)(10 + i), 11, 12, 13 },
                    RelocationData = new byte[] { (byte)(20 + i), 21, 22, 23 },
                    ImportsData = new byte[] { (byte)(30 + i), 31, 32, 33, 34, 35, 36, 37 } };
                BinaryAsset asset = manager.GetBinaryAsset(declaration, true);
                asset.Buffer = buffer;
                asset.Commit();
                buffers.Add(buffer);
                assets.Add(asset);
                // Reborn: read the real intermediate header and verify all three payloads, including consumed memory buffers.
                using Stream input = File.OpenRead(Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName));
                BinaryAssetBuilder.Utility.AssetHeader header = new();
                header.LoadFromStream(input, false);
                using MemoryStream payload = new();
                input.CopyTo(payload);
                Require(header.TypeId == declaration.Handle.TypeId && header.InstanceId == declaration.Handle.InstanceId
                    && header.InstanceDataSize == 4 && header.RelocationDataSize == 4 && header.ImportsDataSize == 8
                    && payload.ToArray().SequenceEqual(buffer.InstanceData.Concat(buffer.RelocationData).Concat(buffer.ImportsData))
                    && asset.Buffer == null, "Intermediate commit changed header/payload or retained the memory buffer.");
            }
            // Reborn: inherited base entries have large metadata sizes but must not add any local payload or require an asset file.
            XmlDocument baseXml = new();
            baseXml.LoadXml("<ArmorTemplate xmlns=\"uri:ea.com:eala:asset\" id=\"InheritedDiagnostic\" />");
            InstanceDeclaration inherited = new(document) { XmlNode = baseXml.DocumentElement! };
            inherited.Handle.TypeHash = 0x11223344u;
            SortedDictionary<string, BinaryAssetBuilder.Utility.AssetHeader> baseAssets = new()
            {
                [inherited.Handle.FileBase] = new BinaryAssetBuilder.Utility.AssetHeader { TypeId = inherited.Handle.TypeId,
                    InstanceId = inherited.Handle.InstanceId, TypeHash = inherited.Handle.TypeHash,
                    InstanceDataSize = 1000, RelocationDataSize = 100, ImportsDataSize = 80 }
            };
            typeof(OutputManager).GetProperty(nameof(OutputManager.BasePatchStreamAssets))!.SetValue(manager, baseAssets);
            document.OutputInstances.Add(inherited);
            BinaryAsset baseAsset = manager.GetBinaryAsset(inherited, true);
            Require(baseAsset.GetLocation(AssetLocation.BasePatchStream, AssetLocationOption.None) == AssetLocation.BasePatchStream,
                "Diagnostic inherited entry was not classified as base-stream data.");
            string[] paths = new[] { ".bin", ".relo", ".imp" }.Select(suffix => manager.OutputDirectory + suffix).ToArray();
            uint[] magics = { 0xBABB0000u, 0xBABE0000u, 0xBAB10000u };
            byte[][] chunks = { buffers.SelectMany(buffer => buffer.InstanceData).ToArray(),
                buffers.SelectMany(buffer => buffer.RelocationData).ToArray(), buffers.SelectMany(buffer => buffer.ImportsData).ToArray() };
            byte[][] expected = chunks.Select((chunk, i) => BitConverter.GetBytes(magics[i])
                .Concat(BitConverter.GetBytes(document.OutputChecksum)).Concat(chunk).ToArray()).ToArray();
            manager.LinkStream(document);
            CheckFiles(paths, expected);
            // Reborn: intact generations must not be rewritten; fixed timestamps make the test independent of clock precision.
            DateTime stamp = new(2020, 1, 2, 3, 4, 6, DateTimeKind.Utc);
            foreach (string path in paths) File.SetLastWriteTimeUtc(path, stamp);
            manager.LinkStream(document);
            Require(paths.All(path => File.GetLastWriteTimeUtc(path) == stamp), "Intact linked files were needlessly rewritten.");
            // Reborn: independently damage every member while retaining the other two valid files and their checksum.
            for (int i = 0; i < paths.Length; i++)
            {
                File.Delete(paths[i]);
                manager.LinkStream(document);
                CheckFiles(paths, expected);
                File.WriteAllBytes(paths[i], expected[i].Take(8).ToArray());
                manager.LinkStream(document);
                CheckFiles(paths, expected);
                byte[] wrong = (byte[])expected[i].Clone();
                wrong[0] ^= 1;
                File.WriteAllBytes(paths[i], wrong);
                manager.LinkStream(document);
                CheckFiles(paths, expected);
                wrong = (byte[])expected[i].Clone();
                wrong[4] ^= 1;
                File.WriteAllBytes(paths[i], wrong);
                manager.LinkStream(document);
                CheckFiles(paths, expected);
                File.WriteAllBytes(paths[i], expected[i].Concat(new byte[] { 0 }).ToArray());
                manager.LinkStream(document);
                CheckFiles(paths, expected);
            }
            // Reborn: an already-created manager cannot be reused after its registry switches to an experimental profile.
            Ra3Ep1ArmorPlugin experimental = new();
            experimental.Initialize(TargetPlatform.Win32);
            registry.DefaultPlugin = experimental;
            ExpectBlocked(() => manager.LinkStream(null!));
            ExpectBlocked(() => manager.CreateVersionFile(null!, "test"));
            assets[0].Buffer = buffers[0];
            ExpectBlocked(() => assets[0].Commit());
            CheckFiles(paths, expected);
            Require(!File.Exists(manager.OutputDirectory + ".version"), "Blocked version call wrote output.");
            Require(!Directory.EnumerateFiles(root, "*.tmp", SearchOption.AllDirectories).Any(), "Blocked commit left temporary assets.");
            Console.WriteLine("Linked-stream self-test: OK (real asset commits, two-asset order, base exclusion, intact reuse, 15 repair cases, direct write guards)");
            Console.WriteLine($"  Diagnostic fixture directory: {root}");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare full tiny synthetic outputs so header-only repairs cannot pass. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckFiles(string[] paths, byte[][] expected)
    {
        for (int i = 0; i < paths.Length; i++)
            Require(File.ReadAllBytes(paths[i]).SequenceEqual(expected[i]), $"Linked payload differs: {paths[i]}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require a profile-specific rejection before any document or file operation. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectBlocked(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException exception) when (exception.Message.Contains("RA3EP1-Armor-Experimental-v1", StringComparison.Ordinal)
            && exception.Message.Contains("experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Direct write was not blocked by experimental profile policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on the first intermediate/linking contract mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
