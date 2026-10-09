using System.Buffers.Binary;
using System.Text;
using System.Text.Json;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: validate one caller-selected marker/stream family as package metadata, not as engine selection or load proof.
internal static class StreamVariantAudit
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse only bounded ASCII whitespace or a conservative custom suffix from a version marker. */
    //-------------------------------------------------------------------------------------------------
    internal static string ParseMarker(byte[] bytes)
    {
        if (bytes.Length > 64 || bytes.Any(value => value > 126 || value < 32 && value is not (9 or 10 or 13)))
            throw new InvalidDataException("Marker exceeds bounded ASCII policy.");
        string suffix = Encoding.ASCII.GetString(bytes).Trim(' ', '\t', '\r', '\n');
        if (suffix.Length != 0 && (suffix.Length > 32 || suffix[0] != '_' || suffix.Length == 1 ||
            suffix.Skip(1).Any(value => !char.IsAsciiLetterOrDigit(value) && value != '_')))
            throw new InvalidDataException("Marker suffix is outside conservative diagnostic policy.");
        return suffix;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit an explicit virtual stream stem without traversal, extension guessing or suffix-slot inference. */
    //-------------------------------------------------------------------------------------------------
    internal static string NormalizeStem(string stem)
    {
        string name = stem.Replace('\\', '/');
        if (name.Length is < 1 or > 240 || name.Any(value => value < 32 || value > 126 || value is ':' or '"' or ';') ||
            name.Split('/').Any(part => part.Length == 0 || part is "." or "..") || name.Contains('.'))
            throw new InvalidDataException("Provide a short relative virtual stream stem with no extension.");
        return name;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject absent or case/slash-colliding siblings instead of choosing an archive winner. */
    //-------------------------------------------------------------------------------------------------
    private static BigEntry Find(IEnumerable<BigEntry> directory, string name)
    {
        var entries = directory.Where(entry => string.Equals(entry.Name.Replace('\\', '/'), name, StringComparison.OrdinalIgnoreCase)).ToArray();
        return entries.Length == 1 ? entries[0] : throw new InvalidDataException($"Missing or ambiguous stream entry '{name}'.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require raw target-specific sidecar headers matching the selected manifest, never whole stream reads. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckHeader(byte[] prefix, ushort version, uint checksum, string kind)
    {
        int length = version == 7 ? 8 : 4;
        if (version is not (5 or 6 or 7) || kind is not ("bin" or "relo" or "imp") || prefix.Length != length || RefPack.IsCompressed(prefix))
            throw new InvalidDataException("Unsupported or compressed sidecar prefix for this narrow audit.");
        if (version == 7)
        {
            uint magic = kind switch { "bin" => 0xBABB0000, "relo" => 0xBABE0000, "imp" => 0xBAB10000, _ => throw new InvalidDataException("Unknown sidecar kind.") };
            if (BinaryPrimitives.ReadUInt32LittleEndian(prefix) != magic)
                throw new InvalidDataException("Sidecar target marker mismatch.");
        }
        if (BinaryPrimitives.ReadUInt32LittleEndian(prefix.AsSpan(length - 4)) != checksum)
            throw new InvalidDataException("Sidecar checksum header differs from manifest.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only the bounded marker, selected manifest and three header prefixes from an explicitly selected BIG. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string path, string requestedStem)
    {
        string stem = NormalizeStem(requestedStem);
        using var archive = BigArchive.Open(path, 100_000, 16 * 1024 * 1024);
        var marker = Find(archive.Entries, stem + ".version");
        byte[] markerBytes = archive.ReadEntry(marker, 64);
        string suffix = ParseMarker(markerBytes);
        string candidate = stem + suffix;
        var manifestEntry = Find(archive.Entries, candidate + ".manifest");
        byte[] manifestBytes = archive.ReadEntry(manifestEntry, 4 * 1024 * 1024);
        if (RefPack.IsCompressed(manifestBytes)) manifestBytes = RefPack.Decompress(manifestBytes, 4 * 1024 * 1024);
        var manifest = ManifestReader.Read(manifestBytes);
        var errors = manifest.Validate();
        if (errors.Count != 0 || !manifest.Header.IsLinked || manifest.Profile is null)
            throw new InvalidDataException("Selected manifest fails linked known-profile structural admission.");
        var headers = new List<object>();
        foreach (string kind in new[] { "bin", "relo", "imp" })
        {
            var entry = Find(archive.Entries, candidate + "." + kind);
            int count = manifest.Header.Version == 7 ? 8 : 4;
            byte[] prefix = archive.ReadEntryRange(entry, 0, count);
            CheckHeader(prefix, manifest.Header.Version, manifest.Header.StreamChecksum, kind);
            headers.Add(new { entry.Name, entry.StoredSize, PrefixHex = Convert.ToHexString(prefix) });
        }
        Console.WriteLine(JsonSerializer.Serialize(new {
            Archive = Path.GetFullPath(path), RequestedStem = stem, MarkerName = marker.Name,
            MarkerHex = Convert.ToHexString(markerBytes), CandidateSuffix = suffix, CandidateStem = candidate,
            Profile = manifest.Profile.Name, manifest.Header.Version, AllTypesHash = $"{manifest.Header.AllTypesHash:X8}",
            Checksum = $"{manifest.Header.StreamChecksum:X8}", manifest.Header.AssetCount,
            ReferencedManifests = manifest.ReferencedManifests, SidecarHeaders = headers,
            MarkerAndSiblingMetadataMatch = true, CDataValidated = false, PatchBaseResolved = false,
            CompletePayloadsValidated = false, RuntimeSuffixSelectionProven = false, GameExecuted = false,
            ModPackageLoaded = false, ProductionBuildReady = false
        }, new JsonSerializerOptions { WriteIndented = true }));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached marker/stem/header cases include observed KW/RA3 bytes and synthetic v7 headers only. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        foreach (var pair in new[] { ("5F6D6F640A", "_mod"), ("5F6D6F640D0A", "_mod"), ("0D0A", ""), ("200D0A", ""), ("", "") })
            if (ParseMarker(Convert.FromHexString(pair.Item1)) != pair.Item2) throw new InvalidDataException("Marker fixture mismatch.");
        foreach (byte[] fault in new[] { new byte[65], new byte[] { 0 }, new byte[] { 0xEF, 0xBB, 0xBF }, Encoding.ASCII.GetBytes("_../bad"), Encoding.ASCII.GetBytes("_mod extra"), Encoding.ASCII.GetBytes("_") })
            Refuse(() => ParseMarker(fault));
        if (NormalizeStem("data\\maps\\official\\example\\map") != "data/maps/official/example/map") throw new InvalidDataException("Stem fixture mismatch.");
        foreach (string fault in new[] { "../map", "C:/map", "data//map", "data/map.manifest", "/map", "data/map;other" })
            Refuse(() => NormalizeStem(fault));
        foreach (ushort version in new ushort[] { 5, 6, 7 })
            foreach (string kind in new[] { "bin", "relo", "imp" })
            {
                byte[] header = new byte[version == 7 ? 8 : 4];
                if (version == 7) BinaryPrimitives.WriteUInt32LittleEndian(header, kind switch { "bin" => 0xBABB0000u, "relo" => 0xBABE0000u, _ => 0xBAB10000u });
                BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(header.Length - 4), 0x12345678);
                CheckHeader(header, version, 0x12345678, kind);
                header[^1] ^= 1; Refuse(() => CheckHeader(header, version, 0x12345678, kind));
            }
        // Reborn: missing entries, namespace collisions, wrong target markers and undersized headers must not pass.
        var directory = new[] { new BigEntry("data\\map_mod.bin", 0, 4) };
        if (Find(directory, "data/map_mod.bin") != directory[0]) throw new InvalidDataException("Sibling fixture mismatch.");
        Refuse(() => Find(directory, "data/map_mod.imp"));
        Refuse(() => Find(directory.Concat(new[] { new BigEntry("DATA/map_mod.bin", 4, 4) }), "data/map_mod.bin"));
        Refuse(() => CheckHeader(new byte[8], 7, 0, "bin"));
        Refuse(() => CheckHeader(new byte[3], 6, 0, "bin"));
        Refuse(() => CheckHeader(new byte[4], 6, 0, "other"));
        Console.WriteLine("Stream variant audit: detached PASS; 16 positive cases and 26 refusals. No archive or game opened.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require policy failures explicitly instead of counting any silent acceptance as success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action)
    {
        try { action(); } catch (InvalidDataException) { return; }
        throw new InvalidDataException("Stream variant fault admitted.");
    }
}
