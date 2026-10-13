using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: map every stock root token against the current native model; indirect graphs remain explicit unverified work, not compiler support.
internal static class GameObjectRootLayoutReview
{
    internal sealed record Field(string Name, string NativeType, int NativeOffset, uint NameHash, uint TokenType, int Target, string Relationship);
    // Reborn: retain unmapped native bytes as evidence, never assume an absent token means an unused/null native member.
    internal sealed record UnmappedSlot(string Name, int NativeOffset, string BytesHex);
    internal sealed record Proof(string Asset, string SliceSha256, int RootTokens, int NativeSize, Field[] Fields,
        string[] MissingNativeFields, UnmappedSlot[] NativeSlotsWithoutTokens, string[] UnknownStockHashes, int DirectMatches, int PointerMatches, int DirectMismatches, int IndirectTables, bool OmittedNativeFieldsZero)
    {
        // Reborn: root name/offset coverage must never imply verified values, type tags, indirect modules, tokenization or playable publication.
        public bool StockRootNameCoverageComplete => UnknownStockHashes.Length == 0 && Fields.Length == RootTokens;
        public bool NativeModelFullyTokenRepresented => MissingNativeFields.Length == 0;
        public bool DirectRootOffsetsMatch => DirectMismatches == 0;
        public bool CompletePayloadVerified => false;
        public bool IndirectGraphsVerified => false;
        public bool TokenTypeSemanticsVerified => false;
        public bool GameObjectCompilerEnabled => false;
        public bool GameExecuted => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse the bounded linked stock capture and independently reflect native offsets for every root field name. */
    //-------------------------------------------------------------------------------------------------
    internal static Proof Inspect(string manifest, string name)
    {
        var captured = GameObjectStockReview.Capture(manifest, name);
        return Decode(name, captured.Bytes, captured.Projection.NativeRoot);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: classify direct native targets separately from in-prefix token tables; unknown hashes and offset drift remain visible rather than guessed. */
    //-------------------------------------------------------------------------------------------------
    private static Proof Decode(string name, byte[] bytes, int nativeRoot)
    {
        var nativeType = typeof(SageBinaryData.GameObject);
        int size = Marshal.SizeOf(nativeType);
        if (size != 600 || nativeRoot < 0 || nativeRoot > bytes.Length - size) throw new InvalidDataException("GameObject root layout size/range drift.");
        var tokens = GameObjectStockReview.Tokens(bytes, 0);
        var model = nativeType.GetFields().Where(f => f.Name != "Base").ToArray();
        if (model.Select(f => FastHash.GetHashCode(f.Name)).Distinct().Count() != model.Length) throw new InvalidDataException("Ambiguous reflected GameObject field hashes.");
        var missing = new List<string>(); var fields = new List<Field>(); var known = new HashSet<uint>();
        foreach (var field in model)
        {
            uint hash = FastHash.GetHashCode(field.Name); known.Add(hash);
            if (!tokens.TryGetValue(hash, out var token)) { missing.Add(field.Name); continue; }
            int offset = Marshal.OffsetOf(nativeType, field.Name).ToInt32();
            // Reborn: a non-null pointer field can target its pointee directly rather than the root pointer slot; check both range and stored pointer.
            bool pointee = field.FieldType.IsPointer && token.Offset >= nativeRoot + size && token.Offset <= bytes.Length - 4
                && BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(nativeRoot + offset, 4)) == token.Offset;
            string relationship = token.Offset < nativeRoot ? "IndirectTokenTableUnverified" :
                token.Offset == nativeRoot + offset ? "DirectNativeOffsetMatch" : pointee ? "NativePointerTargetMatch" : "DirectNativeOffsetMismatch";
            fields.Add(new(field.Name, field.FieldType.ToString(), offset, hash, token.Type, token.Offset, relationship));
        }
        string[] unknown = tokens.Keys.Where(hash => !known.Contains(hash)).Order().Select(hash => hash.ToString("X8")).ToArray();
        // Reborn: this observation does not authorize removing legacy native fields or accepting missing source/schema fields.
        var omitted = model.Where(f => missing.Contains(f.Name)).Select(f => {
            int at = nativeRoot + Marshal.OffsetOf(nativeType, f.Name).ToInt32();
            int width = f.FieldType.IsPointer ? 4 : Marshal.SizeOf(f.FieldType);
            return new UnmappedSlot(f.Name, at - nativeRoot, Convert.ToHexString(bytes.AsSpan(at, width)));
        }).ToArray();
        bool omittedZero = omitted.All(f => f.BytesHex.All(c => c == '0'));
        return new(name, Convert.ToHexString(SHA256.HashData(bytes)), tokens.Count, size, fields.ToArray(), missing.ToArray(), omitted, unknown,
            fields.Count(f => f.Relationship == "DirectNativeOffsetMatch"), fields.Count(f => f.Relationship == "NativePointerTargetMatch"),
            fields.Count(f => f.Relationship == "DirectNativeOffsetMismatch"), fields.Count(f => f.Relationship == "IndirectTokenTableUnverified"), omittedZero);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare two real objects, then prove detached field-offset/name corruptions cannot masquerade as complete root coverage. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string manifest)
    {
        foreach (string name in new[] { "GameObject:SovietAntiInfantryInfantry", "GameObject:AlliedConstructionYard" })
        {
            var captured = GameObjectStockReview.Capture(manifest, name);
            var proof = Decode(name, captured.Bytes, captured.Projection.NativeRoot);
            Console.WriteLine(JsonSerializer.Serialize(proof, new JsonSerializerOptions { WriteIndented = true }));
            if (!proof.StockRootNameCoverageComplete || !proof.DirectRootOffsetsMatch || proof.DirectMatches == 0 || proof.IndirectTables == 0
                || !proof.NativeModelFullyTokenRepresented)
                throw new InvalidDataException("GameObject stock root model requires an explicit layout correction/review.");
            var field = proof.Fields.Single(f => f.Name == "BountyValue");
            int entry = Enumerable.Range(0, proof.RootTokens).Single(i => BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(i * 12, 4)) == field.NameHash) * 12;
            var damaged = captured.Bytes.ToArray();
            BinaryPrimitives.WriteUInt32LittleEndian(damaged.AsSpan(entry + 8, 4), checked((uint)(captured.Projection.NativeRoot + field.NativeOffset + 4)));
            if (Decode(name, damaged, captured.Projection.NativeRoot).DirectRootOffsetsMatch) throw new InvalidDataException("Root offset corruption accepted.");
            damaged = captured.Bytes.ToArray();
            BinaryPrimitives.WriteUInt32LittleEndian(damaged.AsSpan(entry, 4), 0xDEADBEEF);
            var unknown = Decode(name, damaged, captured.Projection.NativeRoot);
            if (unknown.StockRootNameCoverageComplete || !unknown.MissingNativeFields.Contains("BountyValue") || !unknown.UnknownStockHashes.Contains("DEADBEEF"))
                throw new InvalidDataException("Root field-name corruption hidden.");
            // Reborn: preserve the distinction between a verified pointee address and arbitrary in-slice data under a pointer-named token.
            var pointer = proof.Fields.FirstOrDefault(f => f.Relationship == "NativePointerTargetMatch");
            if (pointer != null)
            {
                damaged = captured.Bytes.ToArray();
                int pointerEntry = Enumerable.Range(0, proof.RootTokens).Single(i => BinaryPrimitives.ReadUInt32LittleEndian(damaged.AsSpan(i * 12, 4)) == pointer.NameHash) * 12;
                BinaryPrimitives.WriteUInt32LittleEndian(damaged.AsSpan(pointerEntry + 8, 4), checked((uint)(pointer.Target + 4)));
                if (Decode(name, damaged, captured.Projection.NativeRoot).DirectRootOffsetsMatch) throw new InvalidDataException("Pointer target drift hidden.");
            }
        }
        Console.WriteLine("GameObject root layout: PASS (two stock root maps, field-name/offset and non-null pointer drift controls; indirect graphs/type semantics/publication unverified).");
    }
}
