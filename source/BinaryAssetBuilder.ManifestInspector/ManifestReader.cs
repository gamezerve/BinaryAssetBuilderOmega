using System.Buffers.Binary;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class ManifestReader
{
    private const int HeaderSize = 48;
    private const int Version5EntrySize = 44;
    private const int Version6Or7EntrySize = 48;

    public static ManifestDocument Read(ReadOnlySpan<byte> storedBytes)
    {
        var compressed = RefPack.IsCompressed(storedBytes);
        var payloadBytes = compressed ? RefPack.Decompress(storedBytes) : storedBytes.ToArray();
        var payload = payloadBytes.AsSpan();
        var header = ReadHeader(payload);
        if (header.IsBigEndian)
        {
            throw new NotSupportedException("Big-endian SAGE manifests are not supported yet.");
        }

        var entrySize = header.Version == 5 ? Version5EntrySize : Version6Or7EntrySize;
        var entriesSize = checked((long)header.AssetCount * entrySize);
        var assetNamesStart = checked(
            (long)header.ContainerPrefixSize + HeaderSize + entriesSize +
            header.AssetReferenceBufferSize + header.ReferenceManifestNameBufferSize);
        var sourceNamesStart = checked(assetNamesStart + header.AssetNameBufferSize);
        var expectedEnd = checked(sourceNamesStart + header.SourceFileNameBufferSize);
        if (expectedEnd > payload.Length)
        {
            throw new InvalidDataException(
                $"Manifest buffers end at {expectedEnd:N0}, payload is {payload.Length:N0} bytes.");
        }

        var assetNames = payload.Slice(checked((int)assetNamesStart), checked((int)header.AssetNameBufferSize));
        var sourceNames = payload.Slice(checked((int)sourceNamesStart), checked((int)header.SourceFileNameBufferSize));
        var referencedNamesStart = checked(
            (long)header.ContainerPrefixSize + HeaderSize + entriesSize + header.AssetReferenceBufferSize);
        var assetReferencesStart = checked((long)header.ContainerPrefixSize + HeaderSize + entriesSize);
        var assetReferences = payload.Slice(
            checked((int)assetReferencesStart),
            checked((int)header.AssetReferenceBufferSize));
        var referencedNames = payload.Slice(
            checked((int)referencedNamesStart),
            checked((int)header.ReferenceManifestNameBufferSize));

        var assets = new List<ManifestAsset>(checked((int)header.AssetCount));
        var position = header.ContainerPrefixSize + HeaderSize;
        for (var index = 0u; index < header.AssetCount; index++)
        {
            var entry = payload.Slice(position, entrySize);
            var nameOffset = ReadInt32(entry, 24);
            var sourceOffset = ReadInt32(entry, 28);
            var referenceOffset = ReadInt32(entry, 16);
            var referenceCount = ReadInt32(entry, 20);
            assets.Add(new ManifestAsset(
                ReadUInt32(entry, 0),
                ReadUInt32(entry, 4),
                ReadUInt32(entry, 8),
                ReadUInt32(entry, 12),
                referenceOffset,
                referenceCount,
                nameOffset,
                sourceOffset,
                ReadInt32(entry, 32),
                ReadInt32(entry, 36),
                ReadInt32(entry, 40),
                entrySize == Version6Or7EntrySize ? ReadUInt32(entry, 44) : null,
                ReadNullTerminatedUtf8(assetNames, nameOffset, "asset name"),
                ReadNullTerminatedUtf8(sourceNames, sourceOffset, "source filename"),
                ReadAssetReferences(assetReferences, referenceOffset, referenceCount)));
            position += entrySize;
        }

        var profile = TargetProfile.Identify(header.Version, header.AllTypesHash);
        return new ManifestDocument(
            header,
            assets,
            ReadStringTable(referencedNames),
            profile,
            payload.Length,
            compressed);
    }

    private static ManifestHeader ReadHeader(ReadOnlySpan<byte> payload)
    {
        if (payload.Length < HeaderSize)
        {
            throw new InvalidDataException("Manifest is shorter than its header.");
        }

        int prefix;
        ushort version;
        bool bigEndian;
        bool linked;

        if (IsLegacyHeader(payload))
        {
            prefix = 0;
            bigEndian = ReadBoolean(payload[0], "IsBigEndian");
            linked = ReadBoolean(payload[1], "IsLinked");
            version = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(2, 2));
        }
        else if (IsVersion7Header(payload))
        {
            prefix = payload[..4].SequenceEqual(stackalloc byte[4]) ? 4 : 0;
            var header = payload[prefix..];
            version = BinaryPrimitives.ReadUInt16LittleEndian(header[..2]);
            bigEndian = ReadBoolean(header[2], "IsBigEndian");
            linked = ReadBoolean(header[3], "IsLinked");
        }
        else
        {
            throw new InvalidDataException("Unrecognized SAGE manifest header.");
        }

        var body = payload.Slice(prefix + 4, HeaderSize - 4);
        return new ManifestHeader(
            version,
            bigEndian,
            linked,
            ReadUInt32(body, 0),
            ReadUInt32(body, 4),
            ReadUInt32(body, 8),
            ReadUInt32(body, 12),
            ReadUInt32(body, 16),
            ReadUInt32(body, 20),
            ReadUInt32(body, 24),
            ReadUInt32(body, 28),
            ReadUInt32(body, 32),
            ReadUInt32(body, 36),
            ReadUInt32(body, 40),
            prefix);
    }

    private static bool IsLegacyHeader(ReadOnlySpan<byte> payload)
    {
        var version = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(2, 2));
        return payload[0] <= 1 && payload[1] <= 1 && version is 5 or 6;
    }

    private static bool IsVersion7Header(ReadOnlySpan<byte> payload)
    {
        if (BinaryPrimitives.ReadUInt16LittleEndian(payload[..2]) == 7 && payload[2] <= 1 && payload[3] <= 1)
        {
            return true;
        }

        return payload.Length >= HeaderSize + 4 &&
               payload[..4].SequenceEqual(stackalloc byte[4]) &&
               BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(4, 2)) == 7 &&
               payload[6] <= 1 && payload[7] <= 1;
    }

    private static bool ReadBoolean(byte value, string name) => value switch
    {
        0 => false,
        1 => true,
        _ => throw new InvalidDataException($"Invalid {name} value {value}.")
    };

    private static uint ReadUInt32(ReadOnlySpan<byte> bytes, int offset) =>
        BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset, sizeof(uint)));

    private static int ReadInt32(ReadOnlySpan<byte> bytes, int offset) =>
        BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(offset, sizeof(int)));

    private static string ReadNullTerminatedUtf8(ReadOnlySpan<byte> table, int offset, string label)
    {
        if (offset < 0 || offset >= table.Length)
        {
            throw new InvalidDataException($"Invalid {label} offset {offset} for table size {table.Length}.");
        }

        var terminator = table[offset..].IndexOf((byte)0);
        if (terminator < 0)
        {
            throw new InvalidDataException($"Unterminated {label} at offset {offset}.");
        }

        return Encoding.UTF8.GetString(table.Slice(offset, terminator));
    }

    private static IReadOnlyList<AssetId> ReadAssetReferences(
        ReadOnlySpan<byte> table,
        int offset,
        int count)
    {
        if (offset < 0 || count < 0 || offset > table.Length - checked(count * 8))
        {
            throw new InvalidDataException(
                $"Invalid asset-reference range offset={offset}, count={count}, table={table.Length}.");
        }

        var references = new AssetId[count];
        for (var index = 0; index < count; index++)
        {
            var entry = table.Slice(offset + index * 8, 8);
            references[index] = new AssetId(ReadUInt32(entry, 0), ReadUInt32(entry, 4));
        }

        return references;
    }

    private static IReadOnlyList<ReferencedManifest> ReadStringTable(ReadOnlySpan<byte> table)
    {
        var manifests = new List<ReferencedManifest>();
        var position = 0;
        while (position < table.Length)
        {
            var marker = table[position++];
            if (marker is not (1 or 2))
            {
                throw new InvalidDataException($"Invalid referenced-manifest marker {marker}.");
            }

            var terminator = table[position..].IndexOf((byte)0);
            if (terminator < 0)
            {
                throw new InvalidDataException("Unterminated referenced-manifest name.");
            }

            if (terminator > 0)
            {
                manifests.Add(new ReferencedManifest(
                    Encoding.UTF8.GetString(table.Slice(position, terminator)),
                    marker == 2));
            }

            position += terminator + 1;
        }

        return manifests;
    }
}
