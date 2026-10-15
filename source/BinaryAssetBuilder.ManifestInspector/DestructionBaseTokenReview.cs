using System.Buffers.Binary;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit the selected destruction inheritance and inline DieMux token graph independently of already-equal native bytes.
internal static class DestructionBaseTokenReview
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate concrete/base identities, terminal native anchors, all selected DieMux fields and absent/list storage; return mutation slots for detached refusal controls. */
    //-------------------------------------------------------------------------------------------------
    internal static int[] Inspect(byte[] bytes, GameObjectModuleReview.Module module)
    {
        var faults = new List<int>(); var seen = new HashSet<int>();
        var root = GameObjectStockReview.Tokens(bytes, 0);
        if (!root.TryGetValue(FastHash.GetHashCode("KindOf"), out var kind) || kind.Type != 0x9AFC9193u ||
            !root.TryGetValue(FastHash.GetHashCode("BuildTime"), out var time) || time.Type != 0x645864F3u)
            throw new InvalidDataException("Destruction token-prefix root anchors missing.");
        int nativeRoot = kind.Offset - 4;
        if (nativeRoot < 0 || nativeRoot > bytes.Length - 600 || time.Offset != nativeRoot + 140 ||
            module.Native < nativeRoot + 600 || module.Native > bytes.Length - 72 || module.Type != "BuildingDestructionModuleData")
            throw new InvalidDataException("Destruction token/native root bounds differ.");
        Prefix(module.Wrapper, 12);
        if (Word(module.Wrapper) != 2 || Word(module.Wrapper + 4) != 0x0D96A2F4u || Word(module.Native) != 0x0D96A2F4u)
            throw new InvalidDataException("Destruction concrete class differs.");
        int table = checked((int)Word(module.Wrapper + 8));
        var own = ReadTable(table);
        var die = Base(ref table, own, "DieModuleData");
        if (die.Count != 2 || !die.TryGetValue(FastHash.GetHashCode("DieMuxData"), out var mux) || mux.Type != 0x0084001Bu)
            throw new InvalidDataException("Destruction DieMux parent token differs.");
        // Reborn: the DieMux record is an inline aggregate field table, not a native pointer to another allocation.
        AddFieldSlots(table, "DieMuxData");
        var behavior = Base(ref table, die, "BehaviorModuleData");
        if (behavior.Count != 1) throw new InvalidDataException("Destruction Behavior base table differs.");
        var terminal = Base(ref table, behavior, "ModuleData");
        if (terminal.Count != 2) throw new InvalidDataException("Destruction terminal ModuleData fields differ.");
        Require(terminal, "TypeId", 0x23E650B8u, module.Native); AddFieldSlots(table, "TypeId");
        Require(terminal, "id", 0xBDF07965u, module.Native + 4); AddFieldSlots(table, "id");
        var fields = ReadTable(mux.Offset);
        if (fields.Count != 8) throw new InvalidDataException("Destruction DieMux field inventory differs.");
        foreach (var f in new (string Name, uint Type, int Offset)[] {
            ("DamageAmountRequired", 0x645864F3u, 24), ("MinKillerAngle", 0x4E16EA4Du, 28),
            ("MaxKillerAngle", 0x4E16EA4Du, 32), ("DeathTypes", 0x034DF1A1u, 36), ("DeathTypesForbidden", 0x034DF1A1u, 44) })
        {
            Require(fields, f.Name, f.Type, module.Native + f.Offset); AddFieldSlots(mux.Offset, f.Name);
        }
        foreach (var f in new[] { ("ExemptStatus", 16), ("RequiredStatus", 20) })
        {
            Require(fields, f.Item1, 0xA7BD811Eu, 0); AddFieldSlots(mux.Offset, f.Item1);
            if (Word(module.Native + f.Item2) != 0) throw new InvalidDataException("Selected absent DieMux status pointer differs.");
            faults.Add(module.Native + f.Item2);
        }
        if (!fields.TryGetValue(FastHash.GetHashCode("VeterancyLevels"), out var veterancy) || veterancy.Type != 0x6D66AE18u)
            throw new InvalidDataException("Destruction veterancy list token differs.");
        AddFieldSlots(mux.Offset, "VeterancyLevels"); Prefix(veterancy.Offset, 12);
        if (Word(veterancy.Offset) != 0x6C827073u || Word(veterancy.Offset + 4) != 0x5D448FD8u ||
            Word(veterancy.Offset + 8) != module.Native + 8 || Word(module.Native + 8) != 0 || Word(module.Native + 12) != 0)
            throw new InvalidDataException("Selected empty DieMux veterancy token/native list differs.");
        faults.AddRange(new[] { veterancy.Offset, veterancy.Offset + 4, veterancy.Offset + 8, module.Native + 8, module.Native + 12 });
        return faults.Distinct().ToArray();

        //-------------------------------------------------------------------------------------------------
        /** Reborn: every base edge must identify the expected schema class and a fresh bounded table, never a native address or cycle. */
        //-------------------------------------------------------------------------------------------------
        Dictionary<uint, (uint Type, int Offset)> Base(ref int at, Dictionary<uint, (uint Type, int Offset)> current, string name)
        {
            if (Word(at) != 1 || !current.TryGetValue(1, out var parent) || parent.Type != FastHash.GetHashCode(name))
                throw new InvalidDataException("Destruction base identity differs: " + name);
            faults.AddRange(new[] { at, at + 4, at + 8 }); at = parent.Offset; return ReadTable(at);
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: tables require alignment, bounded terminators and complete containment before the independently anchored native root. */
        //-------------------------------------------------------------------------------------------------
        Dictionary<uint, (uint Type, int Offset)> ReadTable(int at)
        {
            Prefix(at, 12);
            if (!seen.Add(at)) throw new InvalidDataException("Destruction inherited/inline token cycle or alias.");
            var result = GameObjectStockReview.Tokens(bytes, at); Prefix(at, checked((result.Count + 1) * 12)); return result;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: locate type and target words by field identity rather than assuming declaration order for mutation controls. */
        //-------------------------------------------------------------------------------------------------
        void AddFieldSlots(int at, string name)
        {
            uint hash = FastHash.GetHashCode(name);
            for (int i = 0; i < 512; i++)
            {
                int slot = checked(at + i * 12); Prefix(slot, 12);
                if (Word(slot) == hash) { faults.Add(slot + 4); faults.Add(slot + 8); return; }
                if (Word(slot) == 0) break;
            }
            throw new InvalidDataException("Destruction mutation field missing: " + name);
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: token tables must not reinterpret any native bytes as structural metadata. */
        //-------------------------------------------------------------------------------------------------
        void Prefix(int at, int size) { if (at < 0 || at % 4 != 0 || size < 0 || at > nativeRoot - size) throw new InvalidDataException("Destruction token prefix bounds exceeded."); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep all scalar reads inside the captured asset slice. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at) { if (at < 0 || at > bytes.Length - 4) throw new InvalidDataException("Destruction token word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: selected field names, schema type tags and native/absent targets are independent obligations. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(Dictionary<uint, (uint Type, int Offset)> fields, string name, uint type, int target)
    {
        if (!fields.TryGetValue(FastHash.GetHashCode(name), out var field) || field.Type != type || field.Offset != target)
            throw new InvalidDataException("Destruction inherited field differs: " + name);
    }
}
