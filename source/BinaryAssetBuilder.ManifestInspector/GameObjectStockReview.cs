using System.Buffers.Binary;
using System.Security.Cryptography;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using System.Xml;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recover bounded GameObject root/ArmorSet relationships from stock tokens; no authored compiler, package or runtime authority.
internal static class GameObjectStockReview
{
    private const uint ArmorType = 0x3A6C5E8E, DamageFxType = 0xAD3568F5;
    internal sealed record ArmorBinding(int TokenTable, int NativeOffset, uint Conditions, uint ArmorReference, uint DamageFxReference);
    internal sealed record Projection(int RootTokens, int NativeRoot, float BuildTime, int ArmorList, ArmorBinding[] Armors);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only one bounded linked EP1 asset slice and resolve its armor references against independently read metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static object Inspect(string path, string name)
    {
        var captured = Capture(path, name);
        var (metadata, document, asset, offset, bytes, projection) = captured;
        var bindings = projection.Armors.Select(binding => new {
            binding.TokenTable, binding.NativeOffset, binding.Conditions, binding.ArmorReference, binding.DamageFxReference,
            Armor = Resolve(binding.ArmorReference, ArmorType), DamageFX = Resolve(binding.DamageFxReference, DamageFxType)
        }).ToArray();
        // Reborn: compile only the recovered reference-normalized ArmorSet projection; this is not a full GameObject/source processor.
        var nativeProof = NativeProjection(bytes, projection);
        return new { Asset = asset.Name, TypeId = $"{asset.TypeId:X8}", TypeHash = $"{asset.TypeHash:X8}", asset.Tokenized,
            ManifestSha256 = Convert.ToHexString(SHA256.HashData(metadata)), SliceOffset = offset, asset.InstanceDataSize,
            SliceSha256 = Convert.ToHexString(SHA256.HashData(bytes)), projection.RootTokens,
            NativeRootOffset = projection.NativeRoot, NativeRootSize = 600, projection.BuildTime,
            ArmorListOffset = projection.ArmorList, ArmorBindings = bindings,
            RootAnchorRelationshipsVerified = true, ArmorTokenNativeDependencyChainVerified = true,
            NativeArmorProjection = nativeProof,
            CompletePayloadVerified = false, RecursiveDependencyClosureVerified = false,
            GameObjectCompilerEnabled = false, ModPackageBuilt = false, GameExecuted = false };

        //-------------------------------------------------------------------------------------------------
        /** Reborn: resolve one-based native reference words to a unique stock name and verify its declared asset type. */
        //-------------------------------------------------------------------------------------------------
        string Resolve(uint reference, uint type)
        {
            var id = asset.References[checked((int)reference - 1)];
            if (id.TypeId != type) throw new InvalidDataException("ArmorSet reference type mismatch.");
            return document.Assets.Where(item => item.TypeId == id.TypeId && item.InstanceId == id.InstanceId)
                .Select(item => item.Name).Distinct().Single();
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share the bounded fingerprint/header/root capture with follow-up inspectors without sharing compiler/publication authority. */
    //-------------------------------------------------------------------------------------------------
    internal static (byte[] Metadata, ManifestDocument Document, ManifestAsset Asset, long Offset, byte[] Bytes, Projection Projection) Capture(string path, string name, bool allowNullArmorReferences=false)
    {
        path = Path.GetFullPath(path);
        byte[] metadata = SdkEnvironmentPreflight.Read(path, 64 * 1048576);
        var document = ManifestReader.Read(metadata);
        TypeRegistryAudit.ValidateTarget(document.Header.Version, document.Header.AllTypesHash);
        if (document.Validate().Count != 0 || !document.Header.IsLinked || document.Header.ContainerPrefixSize != 4)
            throw new InvalidDataException("Expected valid linked EP1 metadata with the reviewed stream prefix.");
        var asset = document.Assets.Single(item => item.Name == name);
        if (asset.TypeId != 0x942FFF2Du || asset.TypeHash != 0xCEB9FE36u || asset.Tokenized != 1
            || asset.InstanceDataSize < 600 || asset.InstanceDataSize > 1048576)
            throw new InvalidDataException("Unsupported GameObject fingerprint or slice size.");
        long offset = 8;
        foreach (var member in document.Assets) { if (ReferenceEquals(member, asset)) break; offset = checked(offset + member.InstanceDataSize); }
        string bin = Path.ChangeExtension(path, ".bin");
        byte[] header = AssetStreamProbe.ReadRange(bin, null, 0, 8);
        if (Word(header, 0) != 0xBABB0000u || Word(header, 4) != document.Header.StreamChecksum)
            throw new InvalidDataException("GameObject stream header does not match its manifest.");
        byte[] bytes = AssetStreamProbe.ReadRange(bin, null, offset, asset.InstanceDataSize);
        var projection = Decode(bytes, asset.References.ToArray(),allowNullArmorReferences);
        return (metadata, document, asset, offset, bytes, projection);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare the existing GameObject marshaler's selected armor-list bytes/import locations to independently read stock native records. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe object NativeProjection(byte[] stock, Projection projection)
    {
        var xml = new XmlDocument { XmlResolver = null };
        var rootXml = xml.CreateElement("GameObject", "uri:ea.com:eala:asset"); xml.AppendChild(rootXml);
        rootXml.SetAttribute("BuildTime", projection.BuildTime.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
        foreach (var binding in projection.Armors)
        {
            var member = xml.CreateElement("ArmorSet", "uri:ea.com:eala:asset"); rootXml.AppendChild(member);
            // Reborn: placeholders carry captured zero-based Core reference indices; real source-name resolution remains a separate gate.
            member.SetAttribute("Armor", "CapturedArmor\\" + (binding.ArmorReference - 1));
            member.SetAttribute("DamageFX", "CapturedDamageFX\\" + (binding.DamageFxReference - 1));
            member.SetAttribute("Conditions", string.Join(" ", Enumerable.Range(0, 23)
                .Where(bit => (binding.Conditions & (1u << bit)) != 0)
                .Select(bit => Enum.GetName(typeof(SageBinaryData.ArmorSetType), bit)!)));
        }
        var namespaces = new XmlNamespaceManager(xml.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        var node = new Node(xml.CreateNavigator()!.SelectSingleNode("/ea:GameObject", namespaces)!, namespaces);
        SageBinaryData.GameObject* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(SageBinaryData.GameObject), false);
        Marshaler.Marshal(node, root, tracker);
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Native GameObject projection allocation failed.");
        int count = projection.Armors.Length;
        if (chunk.InstanceBuffer.Length != 600 + count * 12 || Word(chunk.InstanceBuffer, 420) != count
            || Word(chunk.InstanceBuffer, 424) != 600 || Word(chunk.InstanceBuffer, 140) != Word(stock, projection.NativeRoot + 140))
            throw new InvalidDataException("Native GameObject armor projection root/list differs.");
        for (int index = 0; index < count; index++)
            if (!chunk.InstanceBuffer.AsSpan(600 + index * 12, 12).SequenceEqual(stock.AsSpan(projection.Armors[index].NativeOffset, 12)))
                throw new InvalidDataException("Marshaled ArmorSet member differs from stock native projection.");
        if (chunk.RelocationBuffer.Length != 8 || Word(chunk.RelocationBuffer, 0) != 424 || Word(chunk.RelocationBuffer, 4) != uint.MaxValue
            || chunk.ImportsBuffer.Length != (count * 2 + 1) * 4 || Word(chunk.ImportsBuffer, count * 8) != uint.MaxValue)
            throw new InvalidDataException("Native GameObject armor projection auxiliary streams differ.");
        for (int index = 0; index < count * 2; index++)
            if (Word(chunk.ImportsBuffer, index * 4) != 600 + index / 2 * 12 + index % 2 * 4)
                throw new InvalidDataException("Native ArmorSet import location differs.");
        return new { NativeBinBytes = chunk.InstanceBuffer.Length, NativeReloBytes = chunk.RelocationBuffer.Length,
            NativeImpBytes = chunk.ImportsBuffer.Length, ArmorMembersMatchStock = true, BuildTimeMatchesStock = true,
            ReferenceIndicesAreCapturedDiagnosticInput = true, SourceNameNormalizationVerified = false,
            FullGameObjectPayloadVerified = false, GameExecuted = false };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require three independent root anchors and ordered list-token/native/reference agreement without guessing a Damage-key join. */
    //-------------------------------------------------------------------------------------------------
    private static Projection Decode(byte[] bytes, AssetId[] references,bool allowNullArmorReferences=false)
    {
        var root = Tokens(bytes, 0);
        int native = Field(root, "KindOf", 0x9AFC9193, bytes.Length, 40) - 4;
        Range(bytes, native, 600);
        int time = Field(root, "BuildTime", 0x645864F3, bytes.Length, 4);
        float buildTime = BitConverter.Int32BitsToSingle(unchecked((int)Word(bytes, time)));
        if (time != native + 140 || !float.IsFinite(buildTime) || buildTime < 0)
            throw new InvalidDataException("GameObject BuildTime/native root anchor differs.");
        int list = Field(root, "ArmorSet", 0x5E5AF575, bytes.Length, 24);
        if (Word(bytes, list) != 0x6C827073 || Word(bytes, list + 4) != 0x5D448FD8
            || Word(bytes, list + 8) != native + 420)
            throw new InvalidDataException("GameObject ArmorSet/native list anchor differs.");
        uint rawCount = Word(bytes, native + 420);
        // Reborn: explicitly opted-in inert props legitimately have no armor; their zero count/pointer must agree before module-only review.
        if(rawCount==0&&allowNullArmorReferences){if(Word(bytes,native+424)!=0)throw new InvalidDataException("Empty prop ArmorSet has storage.");return new(root.Count,native,buildTime,list,Array.Empty<ArmorBinding>());}
        if (rawCount < 1 || rawCount > 32) throw new InvalidDataException("ArmorSet list must contain 1..32 reviewed members.");
        int count = (int)rawCount;
        int items = Pointer(bytes, native + 424, checked(count * 12));
        // Reborn: this stock list has count-delimited markers immediately followed by fixed three-field member tables, not a list zero terminator.
        Range(bytes, list + 12, checked(count * 60));
        var armors = new ArmorBinding[count];
        var tables = new HashSet<int>();
        for (int index = 0; index < count; index++)
        {
            int entry = list + 12 + index * 12;
            if (Word(bytes, entry) != 0xB151A0EB || Word(bytes, entry + 4) != 0)
                throw new InvalidDataException("ArmorSet element marker differs.");
            int table = Pointer(bytes, entry + 8, 48), member = items + index * 12;
            if (!tables.Add(table) || table != list + 12 + count * 12 + index * 48)
                throw new InvalidDataException("ArmorSet count/contiguous token tables differ.");
            var fields = Tokens(bytes, table);
            if (fields.Count != 3 || Field(fields, "Armor", 0x34E9127B, bytes.Length, 4) != member
                || Field(fields, "DamageFX", 0x9A95EDFF, bytes.Length, 4) != member + 4
                || Field(fields, "Conditions", 0xED8BC8E9, bytes.Length, 4) != member + 8)
                throw new InvalidDataException("ArmorSet token fields do not match their ordered native member.");
            uint armor = Word(bytes, member), damageFx = Word(bytes, member + 4), conditions = Word(bytes, member + 8);
            // Reborn: noncombat spellbooks can use null armor references; explicitly selected module reviews still validate every non-null reference.
            if ((!allowNullArmorReferences && (armor == 0 || damageFx == 0)) || armor > references.Length || damageFx > references.Length
                || (armor != 0 && references[checked((int)armor - 1)].TypeId != ArmorType)
                || (damageFx != 0 && references[checked((int)damageFx - 1)].TypeId != DamageFxType)
                || (conditions >> 23) != 0)
                throw new InvalidDataException("ArmorSet reference index/type or condition mask differs.");
            armors[index] = new(table, member, conditions, armor, damageFx);
        }
        return new(root.Count, native, buildTime, list, armors);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound token walks before native decoding, reject duplicate field hashes and require an explicit zero terminator. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<uint, (uint Type, int Offset)> Tokens(byte[] bytes, int start)
    {
        var result = new Dictionary<uint, (uint, int)>();
        for (int index = 0; index <= 512; index++)
        {
            int at = checked(start + index * 12); Range(bytes, at, 12);
            uint name = Word(bytes, at), type = Word(bytes, at + 4), offset = Word(bytes, at + 8);
            if (name == 0 && type == 0 && offset == 0) return result;
            if (name == 0 || index == 512 || offset > int.MaxValue || !result.TryAdd(name, (type, (int)offset)))
                throw new InvalidDataException("Malformed/bounded GameObject token table.");
        }
        throw new InvalidDataException("GameObject token terminator missing.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact field type tags and an in-slice payload range. */
    //-------------------------------------------------------------------------------------------------
    private static int Field(Dictionary<uint, (uint Type, int Offset)> fields, string name, uint type, int length, int size)
    {
        if (!fields.TryGetValue(FastHash.GetHashCode(name), out var field) || field.Type != type
            || field.Offset < 0 || field.Offset > length - size) throw new InvalidDataException("GameObject field differs: " + name);
        return field.Offset;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate signed ranges before reading untrusted payload words. */
    //-------------------------------------------------------------------------------------------------
    private static void Range(byte[] bytes, int at, int size)
    { if (at < 0 || size < 0 || at > bytes.Length - size) throw new InvalidDataException("GameObject slice range exceeded."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only explicit little-endian in-range words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at)
    { Range(bytes, at, 4); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require nonzero in-slice pointer targets with a bounded required extent. */
    //-------------------------------------------------------------------------------------------------
    private static int Pointer(byte[] bytes, int at, int size)
    { uint value = Word(bytes, at); if (value == 0 || value > int.MaxValue) throw new InvalidDataException("GameObject pointer invalid."); Range(bytes, (int)value, size); return (int)value; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached synthetic controls prove root/list/reference admission; optional stock reads never modify stream files. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string? manifest = null)
    {
        byte[] bytes = new byte[2400];
        Token(0, "KindOf", 0x9AFC9193, 1028); Token(12, "BuildTime", 0x645864F3, 1164); Token(24, "ArmorSet", 0x5E5AF575, 64);
        Put(1164, unchecked((uint)BitConverter.SingleToInt32Bits(4f)));
        Put(64, 0x6C827073); Put(68, 0x5D448FD8); Put(72, 1444); Put(1444, 3); Put(1448, 2000);
        var references = Enumerable.Range(0, 6).Select(index => new AssetId(index % 2 == 0 ? ArmorType : DamageFxType, (uint)index)).ToArray();
        for (int index = 0; index < 3; index++)
        {
            int marker = 76 + index * 12, table = 112 + index * 48, member = 2000 + index * 12;
            Put(marker, 0xB151A0EB); Put(marker + 8, (uint)table);
            Token(table, "Armor", 0x34E9127B, (uint)member); Token(table + 12, "DamageFX", 0x9A95EDFF, (uint)(member + 4));
            Token(table + 24, "Conditions", 0xED8BC8E9, (uint)(member + 8));
            Put(member, (uint)(index * 2 + 1)); Put(member + 4, (uint)(index * 2 + 2)); Put(member + 8, index == 0 ? 0x2000u : 0u);
        }
        var decoded = Decode(bytes, references);
        if (decoded.NativeRoot != 1024 || decoded.BuildTime != 4 || decoded.Armors.Length != 3 || decoded.Armors[2].ArmorReference != 5)
            throw new InvalidDataException("Synthetic GameObject projection differs.");
        var faults = new[] { (4, 0u), (20, 1168u), (72, 1448u), (1444, 33u), (1448, 2396u), (76, 0u), (96, 112u),
            (116, 0u), (120, 2012u), (2000, 0u), (2000, 2u), (2004, 99u), (2008, 0x80000000u), (1444, 2u), (12, FastHash.GetHashCode("KindOf")) };
        foreach (var fault in faults)
        {
            byte[] changed = bytes.ToArray(); BinaryPrimitives.WriteUInt32LittleEndian(changed.AsSpan(fault.Item1, 4), fault.Item2);
            try { Decode(changed, references); }
            catch (InvalidDataException) { continue; }
            throw new InvalidDataException("GameObject corruption accepted.");
        }
        if (manifest != null)
            foreach (string asset in new[] { "GameObject:SovietAntiInfantryInfantry", "GameObject:AlliedConstructionYard" })
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(Inspect(manifest, asset)));
        Console.WriteLine("GameObject stock review: PASS (root/ArmorSet/reference chain, fifteen detached corruption refusals; optional stock/native subset, no authored publication/package/game).");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: populate only owned synthetic fixture fields. */
        //-------------------------------------------------------------------------------------------------
        void Put(int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: bind synthetic field names to their real engine hash rather than hard-code expected dictionary keys. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at, string name, uint type, uint offset) { Put(at, FastHash.GetHashCode(name)); Put(at + 4, type); Put(at + 8, offset); }
    }
}
