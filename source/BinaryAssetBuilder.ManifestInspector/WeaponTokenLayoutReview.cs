using System.Buffers.Binary;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: enumerate bounded stock token descriptors without treating recovered tables as a production tokenizer.
internal static class WeaponTokenLayoutReview
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: identify field hashes against native structures and follow only in-prefix table targets with bounded recursion. */
    //-------------------------------------------------------------------------------------------------
    internal static object Inspect(byte[] bytes, int prefix)
    {
        var fields = new[] { typeof(WeaponTemplate), typeof(DamageNuggetType), typeof(WeaponEffectNugget) }
            .SelectMany(type => type.GetFields().Select(field => new {
                Hash = FastHash.GetHashCode(field.Name), field.Name, Structure = type.Name,
                Offset = Marshal.OffsetOf(type, field.Name).ToInt32() })).GroupBy(field => field.Hash)
            .ToDictionary(group => group.Key, group => group.ToArray());
        var pending = new Queue<int>(); pending.Enqueue(0);
        var seen = new HashSet<int>(); var tables = new List<object>();
        while (pending.TryDequeue(out int start))
        {
            if (!seen.Add(start)) continue;
            if (seen.Count > 64 || start % 4 != 0 || start < 0 || start > prefix - 12)
                throw new InvalidDataException("Invalid token table graph.");
            var records = new List<object>(); bool terminated = false;
            // Reborn: count tables and concrete class wrappers have explicit lengths, not the field-table zero terminator.
            int end = prefix;
            bool explicitLength = false;
            if (Word(bytes, start) == 0x6C827073u && Word(bytes, start + 4) == 0x5D448FD8u)
            {
                int countAt = checked((int)Word(bytes, start + 8));
                if (countAt < prefix || countAt > bytes.Length - 4 || Word(bytes, countAt) > 16)
                    throw new InvalidDataException("Invalid native token list count.");
                end = checked(start + 12 + (int)Word(bytes, countAt) * 12); explicitLength = true;
            }
            else if (Word(bytes, start) == 2 && Word(bytes, start + 4) == 0xC0F53E99u)
            { end = start + 12; explicitLength = true; }
            if (end > prefix) throw new InvalidDataException("Token table crosses native boundary.");
            for (int at = start; at <= end - 12; at += 12)
            {
                uint name = Word(bytes, at), tag = Word(bytes, at + 4), target = Word(bytes, at + 8);
                if (name == 0 && tag == 0 && target == 0) { terminated = true; break; }
                if (target >= bytes.Length) throw new InvalidDataException("Token target outside weapon slice.");
                if (target > 0 && target < prefix) pending.Enqueue(checked((int)target));
                records.Add(new { At = at, NameHash = $"{name:X8}", Tag = $"{tag:X8}", Target = target,
                    NativeRelativeTarget = target >= prefix ? (int)target - prefix : (int?)null,
                    Fields = fields.TryGetValue(name, out var matches) ? matches.Select(field => new { field.Name, field.Structure, field.Offset }).ToArray() : null });
            }
            if (!terminated && !explicitLength) throw new InvalidDataException("Token table lacks bounded terminator at " + start);
            tables.Add(new { Start = start, Count = records.Count, Records = records });
        }
        return tables;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode only explicit in-slice little-endian words for table review. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4));
}
