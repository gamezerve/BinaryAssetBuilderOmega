using System.Security.Cryptography;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compare only the four rejected identity-mapped audio records against their original BIG entries, without extraction or codec calls.
internal static class AudioArchiveComparisonProbe
{
    private static readonly string[] Names = { "A08_KirovEntry","WAGreat_Outdoors_quad","WANight_Transylvania_quad","WAShima_Compound1_quad" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require identical manifests, bounded RefPack decoding and independent framing before interpreting unpacked mismatches. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifestPath,string archivePath) => ReadCorrections(manifestPath,archivePath);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return only verified fixed-identity original records for an explicitly requested read-only reconciliation overlay. */
    //-------------------------------------------------------------------------------------------------
    internal static IReadOnlyDictionary<string,byte[]> ReadCorrections(string manifestPath,string archivePath)
    {
        AudioFileRuntimeProbe.Run(manifestPath);
        using BigArchive archive = BigArchive.Open(archivePath);
        byte[] archivedManifest = Decode(archive,Find(archive,"data/audio.manifest"));
        byte[] localManifest = File.ReadAllBytes(manifestPath);
        RequireSameManifest(archivedManifest,localManifest);
        ManifestDocument metadata = ManifestReader.Read(localManifest);
        using FileStream bin = File.OpenRead(Path.ChangeExtension(manifestPath,".bin"));
        long offset = 8; int selected = 0, mismatches = 0;
        Dictionary<string,byte[]> corrections = new(StringComparer.Ordinal);
        foreach (ManifestAsset asset in metadata.Assets)
        {
            if (Names.Any(name => asset.Name.Equals("AudioFile:"+name,StringComparison.Ordinal)))
            {
                selected++;
                byte[] prefix = new byte[32]; bin.Position = offset; bin.ReadExactly(prefix);
                AudioFileRuntimeProbe.Header header = AudioFileRuntimeProbe.Parse(prefix,asset.InstanceDataSize);
                if (header.HeaderSize is not (0 or 8)) throw new InvalidDataException("Original audio inline-header bound exceeded.");
                byte[] inline = new byte[header.HeaderSize]; if (inline.Length != 0) { bin.Position = offset+header.HeaderPointer; bin.ReadExactly(inline); }
                string relative = $"data/audio/cdata/{asset.TypeId:x8}.{asset.TypeHash:x8}.{asset.InstanceId:x8}.{asset.InstanceHash:x8}.cdata";
                BigEntry entry = Find(archive,relative); byte[] decoded = Decode(archive,entry);
                using MemoryStream original = new(decoded,false);
                AudioCustomDataProbe.Result framing = AudioCustomDataProbe.Inspect(original,header,inline);
                string localPath = Path.Combine(Path.GetDirectoryName(manifestPath)!,"audio","cdata",Path.GetFileName(relative));
                using FileStream local = File.OpenRead(localPath);
                long difference = FirstDifference(local,decoded);
                if (difference != -1) mismatches++;
                corrections.Add(asset.Name,decoded);
                Console.WriteLine($"  archive {asset.Name}: stored={entry.StoredSize}, decoded={decoded.Length}, local={local.Length}, blocks={framing.Blocks}, first-difference={difference}, decoded-sha256={Convert.ToHexString(SHA256.HashData(decoded))}");
            }
            offset = checked(offset+asset.InstanceDataSize);
        }
        if (selected != Names.Length) throw new InvalidDataException("Expected exactly four original audio identities.");
        Console.WriteLine($"Audio archive comparison: OK; manifest identical; original framing passed={selected}, unpacked mismatches={mismatches}; no files written or codec decoding.");
        return corrections;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require an exact unique BIG entry after only separator normalization. */
    //-------------------------------------------------------------------------------------------------
    private static BigEntry Find(BigArchive archive,string name)
    {
        BigEntry[] entries = archive.Entries.Where(entry => entry.Name.Replace('\\','/').Equals(name,StringComparison.OrdinalIgnoreCase)).ToArray();
        if (entries.Length != 1) throw new InvalidDataException("Missing or ambiguous original audio entry: "+name);
        return entries[0];
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode only explicitly selected small archive entries; this unwraps RefPack, never an audio codec. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Decode(BigArchive archive,BigEntry entry)
    {
        byte[] stored = archive.ReadEntry(entry,16*1024*1024);
        return RefPack.IsCompressed(stored) ? RefPack.Decompress(stored,16*1024*1024) : stored;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: disallow overlays across differing manifest content even when asset names or metadata fingerprints match. */
    //-------------------------------------------------------------------------------------------------
    internal static void RequireSameManifest(ReadOnlySpan<byte> original,ReadOnlySpan<byte> local)
    {
        if (!original.SequenceEqual(local)) throw new InvalidDataException("Original/archive manifest differs; identity comparison is not admitted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare unpacked data in capped chunks and report the first differing byte or length boundary. */
    //-------------------------------------------------------------------------------------------------
    internal static long FirstDifference(Stream local,byte[] original)
    {
        byte[] buffer = new byte[4096]; long offset = 0,common = Math.Min(local.Length,original.Length); local.Position = 0;
        while (offset < common)
        {
            int count = (int)Math.Min(buffer.Length,common-offset); local.ReadExactly(buffer.AsSpan(0,count));
            for (int index = 0; index < count; index++) if (buffer[index] != original[checked((int)offset+index)]) return offset+index;
            offset += count;
        }
        return local.Length == original.Length ? -1 : common;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin chunked comparison across equal, shorter, longer, early and later mismatched unpacked records. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        byte[] original = new byte[9000]; original[8192] = 42;
        foreach ((byte[] bytes,long expected) in new[] { (original,-1L),(original[..8192],8192L),(original.Concat(new byte[1]).ToArray(),9000L) })
        { using MemoryStream stream = new(bytes); if (FirstDifference(stream,original) != expected) throw new InvalidDataException("Archive comparison boundary differs."); }
        foreach (int index in new[] { 0,4095,4096,8192,8999 })
        { byte[] bytes = (byte[])original.Clone(); bytes[index] ^= 1; using MemoryStream stream = new(bytes); if (FirstDifference(stream,original) != index) throw new InvalidDataException("Archive comparison mismatch offset differs."); }
        // Reborn: matching manifests admit the comparison, while content or length differences reject before any overlay is returned.
        RequireSameManifest(original,original);
        foreach (byte[] mismatch in new[] { original[..8192],original.Concat(new byte[1]).ToArray(),new byte[9000] })
        {
            bool rejected = false;
            try { RequireSameManifest(original,mismatch); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Differing original manifest admitted an overlay.");
        }
        Console.WriteLine("Audio archive comparison self-test: OK (equal/length mismatch, early/chunk-boundary/later differences; no extraction)");
    }
}
