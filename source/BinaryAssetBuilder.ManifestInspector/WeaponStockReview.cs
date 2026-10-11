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
    internal static object Inspect(string path, string? referenceManifest = null)
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
        // Reborn: optional global metadata resolves direct stock names only, not recursive import closure or recompilation.
        var documents = new List<ManifestDocument> { document };
        string? referenceHash = null;
        if (referenceManifest != null)
        {
            var referenceBytes = File.ReadAllBytes(Path.GetFullPath(referenceManifest));
            var reference = ManifestReader.Read(referenceBytes);
            TypeRegistryAudit.ValidateTarget(reference.Header.Version, reference.Header.AllTypesHash);
            if (reference.Validate().Count != 0) throw new InvalidDataException("Invalid reference metadata.");
            documents.Add(reference); referenceHash = Convert.ToHexString(SHA256.HashData(referenceBytes));
        }
        var dependencies = asset.References.Select(item => new {
            TypeId = $"{item.TypeId:X8}", InstanceId = $"{item.InstanceId:X8}",
            Names = documents.SelectMany(value => value.Assets).Where(value => value.TypeId == item.TypeId
                && value.InstanceId == item.InstanceId).Select(value => value.Name).Distinct().ToArray()
        }).ToArray();
        if (referenceManifest != null && (dependencies.Length != 3
            || dependencies[0].Names.SingleOrDefault() != "Multisound:SOV_Conscript_GunFireMS"
            || dependencies[1].Names.SingleOrDefault() != "FXList:FX_MuzzleSoviet"
            || dependencies[2].Names.SingleOrDefault() != "FXList:FX_MuzzleSoviet"))
            throw new InvalidDataException("Expected stock weapon direct references not resolved.");
        // Reborn: follow the polymorphic list rather than finding a coincidental float value anywhere in the slice.
        var damage = DecodeDamage(bytes, root.Native);
        foreach (var fault in new[] { (damage.Damage, 0u), (root.Native + 264, 2u), (damage.Native, 0u) })
        {
            var changed = (byte[])bytes.Clone(); Put(changed, fault.Item1, fault.Item2);
            try { DecodeDamage(changed, root.Native); }
            catch (InvalidDataException) { continue; }
            throw new InvalidDataException("Stock damage subtree corruption accepted.");
        }
        return new {
            Asset = asset.Name, TypeId = $"{asset.TypeId:X8}", TypeHash = $"{asset.TypeHash:X8}", asset.Tokenized,
            ManifestSha256 = Convert.ToHexString(SHA256.HashData(metadata)), SliceOffset = offset,
            asset.InstanceDataSize, asset.RelocationDataSize, asset.ImportsDataSize,
            SliceSha256 = Convert.ToHexString(SHA256.HashData(bytes)), RootTokenCount = root.Count,
            NativeRootOffset = root.Native, NativeRootSize = 340, AttackRangeOffset = root.Range, AttackRange = 150,
            WeaponSpeedOffset = root.Speed, WeaponSpeed = 999999,
            DamageNuggetTokenTableOffset = damage.Table, DamageNuggetNativeOffset = damage.Native,
            DamageValueOffset = damage.Damage, DamageValue = 5, DamageTypeValueOffset = damage.DamageType,
            DamageTypeValue = 21, SelectedDamageScalarChainVerified = true, DetachedDamageControlsPassed = true,
            Dependencies = dependencies, ReferenceManifestSha256 = referenceHash, DirectReferenceNamesVerified = referenceManifest != null,
            StockRootScalarsVerified = true, CompletePayloadVerified = false, WeaponCompilerEnabled = false,
            RecursiveDependencyClosureVerified = false, ModPackageBuilt = false, GameExecuted = false
        };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode only the explicitly proven root scalars; malformed token tables do not become evidence. */
    //-------------------------------------------------------------------------------------------------
    private static (int Count, int Native, int Range, int Speed) DecodeRoot(byte[] bytes)
    {
        var tokens = Tokens(bytes, 0);
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
    /** Reborn: require the observed single DamageNugget class/base chain, native list pointers and GUN damage value. */
    //-------------------------------------------------------------------------------------------------
    private static (int Table, int Native, int Damage, int DamageType) DecodeDamage(byte[] bytes, int root)
    {
        var tokens = Tokens(bytes, 0);
        int list = Field(tokens, "Nuggets", 0xA5C43252u, bytes.Length, 48);
        if (Word(bytes, list) != 0x6C827073u || Word(bytes, list + 4) != 0x5D448FD8u
            || Word(bytes, list + 8) != root + 264 || Word(bytes, root + 264) != 1
            || Word(bytes, list + 12) != 0xB151A0EBu || Word(bytes, list + 16) != 0)
            throw new InvalidDataException("Weapon nugget list/count differs.");
        int classes = Pointer(bytes, list + 20, 24);
        if (Word(bytes, classes) != 2 || Word(bytes, classes + 4) != 0xC0F53E99u)
            throw new InvalidDataException("Expected DamageNuggetType token class.");
        int table = Pointer(bytes, classes + 8, 12);
        if (Word(bytes, table) != 1 || Word(bytes, table + 4) != 0xC7C2CC04u)
            throw new InvalidDataException("Expected WeaponEffectNugget base table.");
        var fields = Tokens(bytes, table);
        int damage = Field(fields, "Damage", 0x645864F3u, bytes.Length, 4);
        int damageType = Field(fields, "DamageType", 0x46486FF1u, bytes.Length, 4);
        int native = damage - 40;
        int item = Pointer(bytes, root + 268, 4);
        if (native < 0 || native > bytes.Length - 156 || Word(bytes, item) != native
            || Word(bytes, native) != 0xC0F53E99u || damageType != native + 76
            || Word(bytes, damage) != unchecked((uint)BitConverter.SingleToInt32Bits(5f))
            || Word(bytes, damageType) != 21)
            throw new InvalidDataException("Stock GUN DamageNugget payload relationship differs.");
        return (table, native, damage, damageType);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode an in-slice absolute pointer with an explicit required target size, never trust arbitrary token offsets. */
    //-------------------------------------------------------------------------------------------------
    private static int Pointer(byte[] bytes, int at, int size)
    {
        int value = checked((int)Word(bytes, at));
        if (value < 0 || value > bytes.Length - size) throw new InvalidDataException("Weapon table pointer out of bounds.");
        return value;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse bounded twelve-byte token records and require an explicit zero terminator and unique names. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<uint, (uint Type, int Offset)> Tokens(byte[] bytes, int start)
    {
        var tokens = new Dictionary<uint, (uint Type, int Offset)>();
        bool terminated = false;
        for (int at = start; at + 12 <= bytes.Length; at += 12)
        {
            uint name = Word(bytes, at), type = Word(bytes, at + 4), value = Word(bytes, at + 8);
            if (name == 0 && type == 0 && value == 0) { terminated = true; break; }
            if (!tokens.TryAdd(name, (type, checked((int)value)))) throw new InvalidDataException("Duplicate root token.");
        }
        if (!terminated) throw new InvalidDataException("Root token terminator absent.");
        return tokens;
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
