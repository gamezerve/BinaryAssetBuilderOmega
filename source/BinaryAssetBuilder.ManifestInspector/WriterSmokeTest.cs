using System.Text;
using System.Reflection;
using BinaryAssetBuilder.Core;
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

        TestLinkedStreamHeaders();

        Console.WriteLine($"Writer round trip OK: {Path.GetFullPath(outputPath)}");
        Console.WriteLine($"  Bytes={bytes.Length} Header={parsed.Header.ContainerPrefixSize + 48} Entry=48 Version={parsed.Header.Version}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Lock the EP1 BIN/RELO/IMP marker and checksum bytes used by OutputManager.LinkStream. */
    //-------------------------------------------------------------------------------------------------
    private static void TestLinkedStreamHeaders()
    {
        MethodInfo write = typeof(OutputManager).GetMethod(
            "WriteLinkedStreamHeader", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new MissingMethodException(nameof(OutputManager), "WriteLinkedStreamHeader");
        MethodInfo matches = typeof(OutputManager).GetMethod(
            "LinkedStreamHeaderMatches", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new MissingMethodException(nameof(OutputManager), "LinkedStreamHeaderMatches");
        const uint checksum = 0x12345678u;
        foreach (uint magic in new[] { 0xBABB0000u, 0xBABE0000u, 0xBAB10000u })
        {
            using MemoryStream linkedStream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(linkedStream, Encoding.UTF8, true);
            write.Invoke(null, new object[] { writer, checksum, magic });
            writer.Flush();
            byte[] header = linkedStream.ToArray();
            if (header.Length != 8
                || BitConverter.ToUInt32(header, 0) != magic
                || BitConverter.ToUInt32(header, 4) != checksum)
            {
                throw new InvalidDataException($"EP1 linked-stream header 0x{magic:X8} was not emitted correctly.");
            }

            linkedStream.Position = 0;
            if (matches.Invoke(null, new object[] { linkedStream, checksum, magic }) is not true)
            {
                throw new InvalidDataException($"EP1 linked-stream header 0x{magic:X8} was not recognized.");
            }
        }

        // Reborn: A legacy checksum-only stream must be rebuilt instead of being mistaken for current EP1 output.
        using MemoryStream legacyStream = new MemoryStream(BitConverter.GetBytes(checksum));
        if (matches.Invoke(null, new object[] { legacyStream, checksum, 0xBABB0000u }) is not false)
        {
            throw new InvalidDataException("A legacy checksum-only stream passed the EP1 linked-stream header gate.");
        }
    }
}
