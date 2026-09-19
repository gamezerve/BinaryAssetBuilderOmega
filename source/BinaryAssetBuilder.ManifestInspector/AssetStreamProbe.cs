namespace BinaryAssetBuilder.ManifestInspector;

internal static class AssetStreamProbe
{
    public static void Print(
        string manifestPath,
        string binPath,
        string typeName,
        string? manifestEntryName,
        string? binEntryName,
        string? assetName,
        uint? findUInt32,
        int? rangeOffset,
        int? rangeCount)
    {
        ManifestDocument manifest = ReadManifest(manifestPath, manifestEntryName);
        long instanceOffset = 0;
        int matches = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (asset.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase)
                && (assetName is null || asset.Name.Equals(assetName, StringComparison.OrdinalIgnoreCase)))
            {
                byte[] bytes = ReadRange(binPath, binEntryName, instanceOffset, asset.InstanceDataSize);
                Console.WriteLine(
                    $"{asset.Name} TypeId=0x{asset.TypeId:X8} InstanceId=0x{asset.InstanceId:X8} " +
                    $"TypeHash=0x{asset.TypeHash:X8} Offset={instanceOffset:N0} Size={asset.InstanceDataSize:N0}");
                int displayed = Math.Min(bytes.Length, 256);
                Console.WriteLine(Convert.ToHexString(bytes.AsSpan(0, displayed)));
                if (displayed != bytes.Length)
                {
                    Console.WriteLine($"(showing first {displayed:N0} of {bytes.Length:N0} bytes)");
                }
                if (findUInt32.HasValue)
                {
                    PrintUInt32Matches(bytes, findUInt32.Value);
                }
                if (rangeOffset.HasValue || rangeCount.HasValue)
                {
                    PrintRange(bytes, rangeOffset ?? 0, rangeCount ?? 256);
                }
                matches++;
            }
            instanceOffset = checked(instanceOffset + asset.InstanceDataSize);
        }

        if (matches == 0)
        {
            string[] suggestions = assetName is null
                ? Array.Empty<string>()
                : manifest.Assets
                    .Where(asset => asset.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase)
                        && asset.Name.Contains(assetName, StringComparison.OrdinalIgnoreCase))
                    .Select(asset => asset.Name)
                    .Take(10)
                    .ToArray();
            string suffix = suggestions.Length == 0
                ? string.Empty
                : $" Similar names: {string.Join(", ", suggestions)}.";
            throw new InvalidDataException(
                assetName is null
                    ? $"Manifest contains no assets of type '{typeName}'."
                    : $"Manifest contains no asset '{assetName}' of type '{typeName}'.{suffix}");
        }
    }

    private static void PrintRange(byte[] bytes, int offset, int count)
    {
        const int maximumRangeLength = 16 * 1024;
        if (offset < 0 || offset > bytes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset),
                $"Asset range offset 0x{offset:X} is outside the {bytes.Length:N0}-byte asset chunk.");
        }
        if (count < 0 || count > maximumRangeLength)
        {
            throw new ArgumentOutOfRangeException(nameof(count),
                $"Asset range count must be between 0 and {maximumRangeLength:N0} bytes.");
        }

        int displayed = Math.Min(count, bytes.Length - offset);
        Console.WriteLine($"  range +0x{offset:X} ({offset:N0}), {displayed:N0} byte(s):");
        Console.WriteLine(Convert.ToHexString(bytes.AsSpan(offset, displayed)));
        if (displayed != count)
        {
            Console.WriteLine($"  (range ended at the end of the asset chunk; requested {count:N0} bytes)");
        }
    }

    private static void PrintUInt32Matches(byte[] bytes, uint value)
    {
        const int maximumMatches = 64;
        int matches = 0;
        for (int offset = 0; offset <= bytes.Length - sizeof(uint); offset++)
        {
            uint candidate = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset));
            if (candidate != value)
            {
                continue;
            }

            int windowStart = Math.Max(0, offset - 16);
            int windowLength = Math.Min(bytes.Length - windowStart, 96);
            Console.WriteLine(
                $"  u32 0x{value:X8} at +0x{offset:X} ({offset:N0}): " +
                Convert.ToHexString(bytes.AsSpan(windowStart, windowLength)));
            matches++;
            if (matches == maximumMatches)
            {
                Console.WriteLine($"  (stopped after {maximumMatches} matches)");
                break;
            }
        }

        if (matches == 0)
        {
            Console.WriteLine($"  u32 0x{value:X8}: no little-endian matches in this asset chunk");
        }
    }

    private static ManifestDocument ReadManifest(string path, string? entryName)
    {
        if (!path.EndsWith(".big", StringComparison.OrdinalIgnoreCase))
        {
            return ManifestReader.Read(File.ReadAllBytes(path));
        }

        using BigArchive archive = BigArchive.Open(path);
        BigEntry entry = FindEntry(archive, entryName, ".manifest");
        return ManifestReader.Read(archive.ReadEntry(entry));
    }

    private static byte[] ReadRange(string path, string? entryName, long offset, int count)
    {
        if (!path.EndsWith(".big", StringComparison.OrdinalIgnoreCase))
        {
            using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (offset > stream.Length - count)
            {
                throw new EndOfStreamException($"Requested instance range exceeds '{path}'.");
            }
            stream.Position = offset;
            byte[] bytes = new byte[count];
            stream.ReadExactly(bytes);
            return bytes;
        }

        using BigArchive archive = BigArchive.Open(path);
        BigEntry entry = FindEntry(archive, entryName, ".bin");
        byte[] signature = archive.ReadEntryRange(entry, 0, Math.Min(2, checked((int)entry.StoredSize)));
        if (RefPack.IsCompressed(signature))
        {
            throw new NotSupportedException(
                $"BIG entry '{entry.Name}' is RefPack-compressed and cannot be randomly probed without decoding the full stream.");
        }
        return archive.ReadEntryRange(entry, offset, count);
    }

    private static BigEntry FindEntry(BigArchive archive, string? requested, string extension)
    {
        BigEntry[] matches = archive.Entries.Where(entry =>
            requested is null
                ? entry.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
                : entry.Name.Equals(requested, StringComparison.OrdinalIgnoreCase)).ToArray();
        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new InvalidDataException(
                requested is null ? $"BIG contains no '{extension}' entry." : $"BIG contains no entry '{requested}'."),
            _ => throw new InvalidDataException($"BIG contains multiple '{extension}' entries; select one explicitly.")
        };
    }
}
