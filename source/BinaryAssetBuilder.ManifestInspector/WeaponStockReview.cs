using System.Buffers.Binary;
using System.Security.Cryptography;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: inspect one real EP1 weapon's bounded token/native slice; this grants no compiler or package authority.
internal static class WeaponStockReview
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: recover root scalar token offsets from the stock AK47 and verify the known 340-byte native root relationship. */
    //-------------------------------------------------------------------------------------------------
    internal static object Inspect(string path)
    {
        path = Path.GetFullPath(path);
        var metadata = File.ReadAllBytes(path);
        var document = ManifestReader.Read(metadata);
        // Reborn: version alone does not distinguish valid target metadata; require the known EP1 aggregate type hash.
        TypeRegistryAudit.ValidateTarget(document.Header.Version, document.Header.AllTypesHash);
        if (document.Validate().Count != 0 || document.Header.Version != 7 || !document.Header.IsLinked)
            throw new InvalidDataException("Expected valid linked EP1 version-7 manifest.");
        var asset = document.Assets.Single(item => item.Name == "WeaponTemplate:SovietAntiInfantryInfantryAK47");
        if (asset.TypeId != 0x94D4D96Eu || asset.TypeHash != 0x27E8D984u || asset.Tokenized != 1
            || asset.InstanceDataSize < 340 || asset.InstanceDataSize > 16384)
            throw new InvalidDataException("Weapon stock fingerprint or bounded slice differs.");
        long offset = document.Header.ContainerPrefixSize + sizeof(uint);
        foreach (var item in document.Assets) { if (ReferenceEquals(item, asset)) break; offset = checked(offset + item.InstanceDataSize); }
        var bytes = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, offset, asset.InstanceDataSize);
        // Reborn: bind the bounded payload read to this linked manifest's stream checksum before decoding root tokens.
        var header = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, 0, 8);
        if (Word(header, 0) != 0xBABB0000u || Word(header, 4) != document.Header.StreamChecksum)
            throw new InvalidDataException("Weapon BIN stream header differs from manifest.");
        var root = DecodeRoot(bytes);
        return new {
            Asset = asset.Name, TypeId = $"{asset.TypeId:X8}", TypeHash = $"{asset.TypeHash:X8}", asset.Tokenized,
            ManifestSha256 = Convert.ToHexString(SHA256.HashData(metadata)), SliceOffset = offset,
            asset.InstanceDataSize, asset.RelocationDataSize, asset.ImportsDataSize,
            SliceSha256 = Convert.ToHexString(SHA256.HashData(bytes)), RootTokenCount = root.Count,
            NativeRootOffset = root.Native, NativeRootSize = 340, AttackRangeOffset = root.Range, AttackRange = 150,
            WeaponSpeedOffset = root.Speed, WeaponSpeed = 999999,
            Dependencies = asset.References.Select(item => new { TypeId = $"{item.TypeId:X8}", InstanceId = $"{item.InstanceId:X8}" }).ToArray(),
            StockRootScalarsVerified = true, CompletePayloadVerified = false, WeaponCompilerEnabled = false,
            ModPackageBuilt = false, GameExecuted = false
        };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode only the explicitly proven root scalars; malformed token tables do not become evidence. */
    //-------------------------------------------------------------------------------------------------
    private static (int Count, int Native, int Range, int Speed) DecodeRoot(byte[] bytes)
    {
        var tokens = new Dictionary<uint, (uint Type, int Offset)>();
        bool terminated = false;
        for (int at = 0; at + 12 <= bytes.Length; at += 12)
        {
            uint name = Word(bytes, at), type = Word(bytes, at + 4), value = Word(bytes, at + 8);
            if (name == 0 && type == 0 && value == 0) { terminated = true; break; }
            if (!tokens.TryAdd(name, (type, checked((int)value)))) throw new InvalidDataException("Duplicate root token.");
        }
        if (!terminated) throw new InvalidDataException("Root token terminator absent.");
        int range = Field(tokens, "AttackRange", 0x645864F3u, bytes.Length, 4);
        int speed = Field(tokens, "WeaponSpeed", 0x645864F3u, bytes.Length, 4);
        int native = range - 4;
        if (native < 0 || native + 340 > bytes.Length || speed != native + 44
            || BitConverter.Int32BitsToSingle(unchecked((int)Word(bytes, range))) != 150f
            || BitConverter.Int32BitsToSingle(unchecked((int)Word(bytes, speed))) != 999999f)
            throw new InvalidDataException("Known stock AK47 root scalar/native relationship differs.");
        return (tokens.Count, native, range, speed);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use owned in-memory tokens for deterministic root proof and five corruption refusals, without stock writes. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        var bytes = new byte[376];
        Put(bytes, 0, FastHash.GetHashCode("AttackRange")); Put(bytes, 4, 0x645864F3u); Put(bytes, 8, 40);
        Put(bytes, 12, FastHash.GetHashCode("WeaponSpeed")); Put(bytes, 16, 0x645864F3u); Put(bytes, 20, 80);
        Put(bytes, 40, unchecked((uint)BitConverter.SingleToInt32Bits(150f)));
        Put(bytes, 80, unchecked((uint)BitConverter.SingleToInt32Bits(999999f)));
        if (DecodeRoot(bytes) != (2, 36, 40, 80)) throw new InvalidDataException("Synthetic weapon root differs.");
        foreach (var fault in new[] { (4, 0u), (8, 400u), (12, FastHash.GetHashCode("AttackRange")), (40, 0u), (20, 84u) })
        {
            var changed = (byte[])bytes.Clone(); Put(changed, fault.Item1, fault.Item2);
            try { DecodeRoot(changed); }
            catch (InvalidDataException) { continue; }
            throw new InvalidDataException("Weapon root corruption accepted.");
        }
        Console.WriteLine("Weapon stock root self-test: PASS (synthetic offsets, five corruption refusals; no compiler/package/game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only owned synthetic token fixture words for detached rejection tests. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require an exact root field/tag and in-slice value range before reading any recovered scalar. */
    //-------------------------------------------------------------------------------------------------
    private static int Field(Dictionary<uint, (uint Type, int Offset)> tokens, string name, uint tag, int length, int size)
    {
        if (!tokens.TryGetValue(FastHash.GetHashCode(name), out var token) || token.Type != tag
            || token.Offset < 0 || token.Offset > length - size) throw new InvalidDataException("Root field differs: " + name);
        return token.Offset;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read stock token words in their explicit little-endian representation. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4));
}
