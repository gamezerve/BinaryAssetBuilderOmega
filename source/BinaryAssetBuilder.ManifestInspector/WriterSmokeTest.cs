using System.Text;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class WriterSmokeTest
{
    private const uint UprisingAllTypesHash = 0x5454A8E9;

    public static void Run(string outputPath)
    {
        byte[] assetName = Encoding.UTF8.GetBytes("GameObject:Ep1WriterSmoke\0");
        byte[] sourceName = Encoding.UTF8.GetBytes("Tests/WriterSmoke.xml\0");

        using MemoryStream stream = new MemoryStream();
        using (BinaryAssetBuilder.Utility.ManifestHeader header = new BinaryAssetBuilder.Utility.ManifestHeader
        {
            IsLinked = true,
            StreamChecksum = 0x12345678,
            AllTypesHash = UprisingAllTypesHash,
            AssetCount = 1,
            AssetNameBufferSize = (uint)assetName.Length,
            SourceFileNameBufferSize = (uint)sourceName.Length
        })
        {
            header.SaveToStream(stream, false);
            if (header.SerializedSize != 52)
            {
                throw new InvalidDataException($"Expected a 52-byte EP1 header, got {header.SerializedSize}.");
            }
        }

        using (BinaryAssetBuilder.Utility.AssetEntry entry = new BinaryAssetBuilder.Utility.AssetEntry
        {
            TypeId = 0x942FFF2D,
            InstanceId = 0x11223344,
            TypeHash = 0xCEB9FE36,
            InstanceHash = 0x55667788,
            Tokenized = true
        })
        {
            entry.SaveToStream(stream, false);
        }

        stream.Write(assetName);
        stream.Write(sourceName);
        byte[] bytes = stream.ToArray();
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
        File.WriteAllBytes(outputPath, bytes);

        stream.Position = 0;
        using (BinaryAssetBuilder.Utility.ManifestHeader loadedHeader = new BinaryAssetBuilder.Utility.ManifestHeader())
        {
            loadedHeader.LoadFromStream(stream, false);
            if (loadedHeader.Version != 7
                || loadedHeader.SerializedSize != 52
                || loadedHeader.AllTypesHash != UprisingAllTypesHash
                || !loadedHeader.IsLinked)
            {
                throw new InvalidDataException("Utility manifest-header v7 read/write round trip failed.");
            }
        }

        using (BinaryAssetBuilder.Utility.Manifest utilityManifest = new BinaryAssetBuilder.Utility.Manifest())
        {
            if (!utilityManifest.Load(outputPath, false)
                || utilityManifest.Version != 7
                || utilityManifest.AssetCount != 1
                || !utilityManifest.Assets.Single().Tokenized)
            {
                throw new InvalidDataException("Utility manifest v7 reader failed to consume writer output.");
            }
        }

        ManifestDocument parsed = ManifestReader.Read(bytes);
        IReadOnlyList<string> errors = parsed.Validate();
        ManifestAsset parsedAsset = parsed.Assets.Single();
        if (errors.Count != 0
            || parsed.Header.Version != 7
            || parsed.Header.ContainerPrefixSize != 4
            || parsed.Header.AllTypesHash != UprisingAllTypesHash
            || parsedAsset.Tokenized != 1
            || parsedAsset.Name != "GameObject:Ep1WriterSmoke")
        {
            throw new InvalidDataException(
                $"EP1 writer round trip failed: {string.Join("; ", errors.DefaultIfEmpty("header/entry mismatch"))}");
        }

        Console.WriteLine($"Writer round trip OK: {Path.GetFullPath(outputPath)}");
        Console.WriteLine($"  Bytes={bytes.Length} Header={parsed.Header.ContainerPrefixSize + 48} Entry=48 Version={parsed.Header.Version}");
    }
}
