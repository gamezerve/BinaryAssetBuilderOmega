
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class AptUIDocumentTest
{
    public static void Run(string xmlPath, string schemaPath)
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
                QualifiedName =
                    "BinaryAssetBuilder.XmlCompiler.Plugin, BinaryAssetBuilder.XmlCompiler"
            };

            var plugins = new PluginRegistry(
                new[] { descriptor },
                TargetPlatform.Win32);

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

            var options = new DocumentProcessor.ProcessOptions
            {
                GenerateOutput = false,
                UsePrecompiled = true
            };

            foreach (var type in new[]
            {
                "AptAptData",
                "AptConstData",
                "AptDatData",
                "AptGeometryData"
            })
            {
                uint id = BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(type);
                var info = plugins.GetExtendedTypeInformation(id);
                Console.WriteLine($"{type}: 0x{id:X8}, Hash=0x{info.TypeHash:X8}");
            }

            Console.WriteLine("Processing AptUI XML through DocumentProcessor.");

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
                options);

            if (document.SelfInstances.Count != 46)
                throw new InvalidDataException(
                    $"Expected 46 AptUI instances, found {document.SelfInstances.Count}.");

            Console.WriteLine($"Instances: {document.SelfInstances.Count}");

            foreach (var instance in document.SelfInstances)
            {
                Console.WriteLine(
                    $"{instance.Handle.Name} TypeId=0x{instance.Handle.TypeId:X8}");
            }

            string aptuiDirectory = Path.GetDirectoryName(assetPath)!;

            string aptBigPath = @"D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\Apt.big";
            using var archive = BigArchive.Open(aptBigPath);

            string manifestPath = @"data\aptui\tacticalhudbuildqueuepage.manifest";
            var manifestEntry = archive.Entries.Single(e => e.Name.Equals(manifestPath, StringComparison.OrdinalIgnoreCase));
            var originalManifest = ManifestReader.Read(archive.ReadEntry(manifestEntry));

            var originalStreams = new Dictionary<string, byte[]>();

            foreach (string extension in new[] { ".bin", ".relo", ".imp" })
            {
                string entryName = @"data\aptui\tacticalhudbuildqueuepage" + extension;
                var entry = archive.Entries.Single(e => e.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase));
                originalStreams.Add("diagnostic" + extension, archive.ReadEntry(entry));
            }

            originalStreams.Add("diagnostic.manifest", archive.ReadEntry(manifestEntry));

            foreach (string extension in new[] { ".bin", ".relo", ".imp" })
            {
                byte[] data = originalStreams["diagnostic" + extension];

                Console.WriteLine(
                    $"{extension}: Length={data.Length}, Header64={Convert.ToHexString(data.AsSpan(0, Math.Min(64, data.Length)))}");
            }

            var originalBin = RefPack.Decompress(originalStreams["diagnostic.bin"], 16 * 1024 * 1024);
            var originalRelo = originalStreams["diagnostic.relo"];
            var originalImp = originalStreams["diagnostic.imp"];

            int binOffset = 8;
            int reloOffset = 8;
            int impOffset = 8;

            var originalAssets = new Dictionary<string, WorldBuilderAssetResolver.Native>(StringComparer.OrdinalIgnoreCase);

            foreach (var asset in originalManifest.Assets)
            {
                var bin = originalBin.AsSpan(binOffset, asset.InstanceDataSize).ToArray();
                var relo = originalRelo.AsSpan(reloOffset, asset.RelocationDataSize).ToArray();
                var imp = originalImp.AsSpan(impOffset, asset.ImportsDataSize).ToArray();

                originalAssets.Add(asset.Name, new WorldBuilderAssetResolver.Native(asset, bin, relo, imp));

                binOffset += asset.InstanceDataSize;
                reloOffset += asset.RelocationDataSize;
                impOffset += asset.ImportsDataSize;
            }

            if (binOffset != originalBin.Length || reloOffset != originalRelo.Length || impOffset != originalImp.Length)
                throw new InvalidDataException("Original AptUI stream lengths do not match manifest asset sizes.");

            int matchedAssets = 0;

            int serializedCount = 0;
            long serializedBytes = 0;

            foreach (var instance in document.SelfInstances)
            {
                string file = instance.XmlNode.Attributes?["File"]?.Value
                    ?? throw new InvalidDataException($"Missing File attribute: {instance.Handle.Name}");

                string payloadPath = Path.GetFullPath(Path.Combine(aptuiDirectory, file));

                if (!File.Exists(payloadPath))
                    throw new FileNotFoundException($"AptUI payload missing: {instance.Handle.Name}", payloadPath);

                AssetBuffer buffer;

                try
                {
                    buffer = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Serialization failed: {instance.Handle.Name}, File={file}", ex);
                }

                if (buffer is null || buffer.InstanceData is null || buffer.InstanceData.Length == 0)
                    throw new InvalidDataException($"Empty serialized output: {instance.Handle.Name}");

                byte[] payload = File.ReadAllBytes(payloadPath);

                if (payload.Length == 0)
                    throw new InvalidDataException($"Empty source payload: {instance.Handle.Name}");

                int payloadOffset = buffer.InstanceData.AsSpan().IndexOf(payload);

                if (payloadOffset < 0)
                    throw new InvalidDataException(
                        $"Source payload not found in serialized output: {instance.Handle.Name}, File={file}");

                Console.WriteLine(
                    $"Payload verified: {instance.Handle.Name}, Bytes={payload.Length}, Offset={payloadOffset}");

                int expectedOffset = instance.Handle.TypeName == "AptGeometryData" ? 16 : 12;

                if (payloadOffset != expectedOffset)
                    throw new InvalidDataException(
                        $"Unexpected payload offset: {instance.Handle.Name}, Expected={expectedOffset}, Actual={payloadOffset}");

                Console.WriteLine(
                    $"Binary layout: {instance.Handle.Name}, Instance={buffer.InstanceData.Length}, Relocations={buffer.RelocationData.Length}, Imports={buffer.ImportsData.Length}");

                if (buffer.RelocationData.Length != 8)
                    throw new InvalidDataException(
                        $"Unexpected relocation size: {instance.Handle.Name}");

                if (buffer.ImportsData.Length != 0)
                    throw new InvalidDataException(
                        $"Unexpected imports: {instance.Handle.Name}");

                Console.WriteLine(
                    $"Relocation bytes: {instance.Handle.Name}, Hex={Convert.ToHexString(buffer.RelocationData)}");

                uint relocationOffset = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buffer.RelocationData.AsSpan(0, 4));
                uint relocationTerminator = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buffer.RelocationData.AsSpan(4, 4));
                uint pointerValue = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(4, 4));
                uint serializedSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buffer.InstanceData.AsSpan(8, 4));

                if (relocationOffset != 4 || relocationTerminator != uint.MaxValue)
                    throw new InvalidDataException($"Unexpected relocation table: {instance.Handle.Name}");

                if (pointerValue != (uint)payloadOffset)
                    throw new InvalidDataException(
                        $"Unexpected DataBlob pointer: {instance.Handle.Name}, Expected={payloadOffset}, Actual={pointerValue}");

                if (serializedSize != payload.Length)
                    throw new InvalidDataException(
                        $"Unexpected DataBlob size: {instance.Handle.Name}, Expected={payload.Length}, Actual={serializedSize}");

                Console.WriteLine($"Relocation verified: {instance.Handle.Name}, Pointer={pointerValue}, Size={serializedSize}");

                if (!originalAssets.TryGetValue(instance.Handle.Name, out var original))
                    throw new InvalidDataException($"Asset missing from original Apt.big: {instance.Handle.Name}");

                bool binMatches = buffer.InstanceData.SequenceEqual(original.Bin);
                bool reloMatches = buffer.RelocationData.SequenceEqual(original.Relo);
                bool impMatches = buffer.ImportsData.SequenceEqual(original.Imp);

                Console.WriteLine(
                    $"Original comparison: {instance.Handle.Name}, BIN={binMatches}, RELO={reloMatches}, IMP={impMatches}");

                if (!binMatches || !reloMatches || !impMatches)
                {
                    int firstDifference = -1;
                    int length = Math.Min(buffer.InstanceData.Length, original.Bin.Length);

                    for (int i = 0; i < length; i++)
                    {
                        if (buffer.InstanceData[i] != original.Bin[i])
                        {
                            firstDifference = i;
                            break;
                        }
                    }

                    throw new InvalidDataException(
                        $"Original AptUI binary mismatch: {instance.Handle.Name}, FirstDifference={firstDifference}, GeneratedSize={buffer.InstanceData.Length}, OriginalSize={original.Bin.Length}");
                }

                matchedAssets++;

                serializedCount++;
                serializedBytes += buffer.InstanceData.Length;

                Console.WriteLine(
                    $"{instance.Handle.Name}: Input={new FileInfo(payloadPath).Length}, Output={buffer.InstanceData.Length}");
            }

            if (matchedAssets != 46)
                throw new InvalidDataException($"Expected 46 matching AptUI assets, found {matchedAssets}.");

            Console.WriteLine($"Original AptUI binary matches: {matchedAssets}/46");

            if (serializedCount != 46)
                throw new InvalidDataException($"Expected 46 serialized assets, found {serializedCount}.");

            Console.WriteLine($"Serialized assets: {serializedCount}");
            Console.WriteLine($"Serialized bytes: {serializedBytes}");


            string originalInclude = Path.Combine(
                Path.GetDirectoryName(xmlPath)!,
                Path.GetFileNameWithoutExtension(xmlPath),
                Path.GetFileName(xmlPath));

            string texturePath = Path.Combine(
                Path.GetDirectoryName(originalInclude)!,
                "art",
                "Textures",
                "apt_TacticalHUDBuildQueuePage_1.tga");

            string originalXml = Path.Combine(
                Directory.GetCurrentDirectory(),
                "alpha",
                "mods",
                "AAA",
                "Data",
                "AptUI",
                "TacticalHUDBuildQueuePage",
                "TacticalHUDBuildQueuePage.xml");

            var textureDocument = new System.Xml.XmlDocument();
            textureDocument.Load(originalXml);

            var textureNamespace = new System.Xml.XmlNamespaceManager(textureDocument.NameTable);
            textureNamespace.AddNamespace("a", "uri:ea.com:eala:asset");

            var textureElement = textureDocument.SelectSingleNode(
                "/a:AssetDeclaration/a:Texture",
                textureNamespace) as System.Xml.XmlElement
                ?? throw new InvalidDataException("Original Texture declaration not found.");

            byte[] textureSource = File.ReadAllBytes(texturePath);
            var textureBuffer = Ep1TextureCompiler.Compile(textureElement, textureSource, texturePath);

            byte[] textureBinary = textureBuffer.InstanceBuffer;
            byte[] textureRelocations = textureBuffer.RelocationBuffer;
            byte[] textureImports = textureBuffer.ImportsBuffer;

            uint texturePointer = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(textureBinary.AsSpan(4, 4));
            uint textureSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(textureBinary.AsSpan(8, 4));

            if (texturePointer != 12 || textureSize != textureBinary.Length - 12)
                throw new InvalidDataException("Invalid compiled Texture root.");

            if (!textureRelocations.SequenceEqual(new byte[] { 4, 0, 0, 0, 255, 255, 255, 255 }))
                throw new InvalidDataException("Invalid Texture relocations.");

            if (textureImports.Length != 0)
                throw new InvalidDataException("Unexpected Texture imports.");

            if (!textureBinary.AsSpan(12, 4).SequenceEqual(new byte[] { 0x44, 0x44, 0x53, 0x20 }))
                throw new InvalidDataException("Compiled Texture does not contain DDS data.");

            string textureAssetName = "Texture:apt_TacticalHUDBuildQueuePage_1";

            if (!originalAssets.TryGetValue(textureAssetName, out var originalTexture))
                throw new InvalidDataException($"Original Texture not found: {textureAssetName}");

            bool textureBinMatches = textureBinary.SequenceEqual(originalTexture.Bin);
            bool textureReloMatches = textureRelocations.SequenceEqual(originalTexture.Relo);
            bool textureImpMatches = textureImports.SequenceEqual(originalTexture.Imp);

            Console.WriteLine(
                $"Original Texture comparison: BIN={textureBinMatches}, RELO={textureReloMatches}, IMP={textureImpMatches}");

            if (!textureBinMatches || !textureReloMatches || !textureImpMatches)
            {
                int firstDifference = -1;
                int compareLength = Math.Min(textureBinary.Length, originalTexture.Bin.Length);

                for (int i = 0; i < compareLength; i++)
                {
                    if (textureBinary[i] != originalTexture.Bin[i])
                    {
                        firstDifference = i;
                        break;
                    }
                }

                throw new InvalidDataException(
                    $"Original Texture mismatch: FirstDifference={firstDifference}, GeneratedSize={textureBinary.Length}, OriginalSize={originalTexture.Bin.Length}, RELO={textureReloMatches}, IMP={textureImpMatches}");
            }

            Console.WriteLine("Original AptUI binary matches: 47/47");

            Console.WriteLine($"Texture verified: {Ep1TextureCompiler.Describe(textureBuffer)}");
            Console.WriteLine($"Texture binary bytes: {textureBinary.Length}");
            Console.WriteLine($"Total verified assets: {serializedCount + 1}");


            Console.WriteLine("AptUI document processing passed.");
        }
        finally
        {
            Settings.Current = saved;
        }
    }
}
