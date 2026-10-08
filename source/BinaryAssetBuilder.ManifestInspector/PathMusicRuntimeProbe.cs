using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using BinaryAssetBuilder.Core;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: isolate actual tracker serialization of the observed music runtime ABI without registering a processor or emitting game streams.
internal static class PathMusicRuntimeProbe
{
    // Reborn: keep the isolated ABI outside production model/marshaller inventories until authored processor/dependency contracts are proved.
    [StructLayout(LayoutKind.Explicit,Size=16)]
    private unsafe struct Runtime
    {
        [FieldOffset(0)] public uint Base;
        [FieldOffset(4)] public uint Event;
        [FieldOffset(8)] public uint* Alternate;
        [FieldOffset(12)] public byte Cacheable;
    }
    // Reborn: detached chunks and input snapshot hashes are diagnostic evidence, not stock processing hashes.
    internal sealed record HeaderResult(string HeaderSha256,string Name,uint EventValue,Chunk Chunk);
    internal sealed record LayoutReport(int MatchedRecords,int AlternateRecords,int ZeroRecords,int BinBytes,int RelocationBytes,string SourceSha256,string ManifestSha256)
    {
        public bool ReadOnly => true;
        public bool StockValuesReplayed => true;
        public bool HeaderRecovered => false;
        public bool ProductionBuildReady => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require a unique canonical nonzero literal header definition before isolated serialization; never adopt legacy warning-and-zero fallback. */
    //-------------------------------------------------------------------------------------------------
    internal static HeaderResult FromHeader(byte[] header,string name,string? alternate = null,bool cacheable = true)
    {
        byte[] snapshot = (byte[])header.Clone(); var first = PathMusicHeaderSemantics.Scan(snapshot,name);
        if (!first.CanonicalLiteral || first.Value is null or 0 || first.UnsupportedReason != null) throw new InvalidDataException("Isolated header probe requires a canonical nonzero event literal.");
        int matches = 0,lines = 0; using StringReader reader = new(Encoding.ASCII.GetString(snapshot)); string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (++lines > 8192) throw new InvalidDataException("Full header duplicate-review line budget exceeded.");
            if (PathMusicHeaderSemantics.ReviewLine(line,name).LegacyShapeMatch) matches++;
        }
        if (matches != 1) throw new InvalidDataException("Duplicate event definitions are outside strict isolated admission.");
        Chunk chunk = EncodeObserved(first.Value.Value,alternate,cacheable);
        if (!snapshot.AsSpan().SequenceEqual(header)) throw new InvalidDataException("Caller header changed during isolated preparation.");
        return new(Hash(snapshot),name,first.Value.Value,chunk);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize only explicit observed values through the real Win32 tracker; zero is allowed here solely for stock-layout characterization. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk EncodeObserved(uint eventValue,string? alternate = null,bool cacheable = true)
    {
        if (IntPtr.Size != 4 || sizeof(Runtime) != 16 || Marshal.OffsetOf<Runtime>(nameof(Runtime.Alternate)).ToInt32() != 8
            || Marshal.OffsetOf<Runtime>(nameof(Runtime.Cacheable)).ToInt32() != 12 || eventValue > int.MaxValue)
            throw new InvalidDataException("Isolated music ABI requires Win32 and the reviewed signed literal range.");
        if (alternate != null) PathMusicHeaderSemantics.ReviewLine("",alternate);
        Runtime* root; using Tracker tracker = new((void**)&root,16,false);
        root->Base = 0; root->Event = eventValue; root->Cacheable = cacheable ? (byte)1 : (byte)0;
        if (alternate != null)
        {
            using Tracker.Context context = tracker.Push((void**)&root->Alternate,4,1);
            *root->Alternate = InstanceHandle.GetInstanceId(alternate);
        }
        Chunk chunk = new(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Isolated music relocation failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: replay observed stock words through the independent tracker layout, never claim this reconstructs a header or authored processor. */
    //-------------------------------------------------------------------------------------------------
    internal static LayoutReport CompareStock(string source,string manifest)
    {
        var observed = PathMusicStockReview.Inspect(source,manifest);
        foreach (var row in observed.Rows)
        {
            Chunk chunk = EncodeObserved(row.Word4,row.Alternate,true);
            if (Hash(chunk.InstanceBuffer) != row.SliceSha256 || Hash(chunk.RelocationBuffer) != row.RelocationSha256 || chunk.ImportsBuffer.Length != 0)
                throw new InvalidDataException("Tracker music runtime differs from selected stock record: "+row.Name);
        }
        return new(observed.Events,observed.AlternateReferences,observed.Rows.Count(row => row.Word4 == 0),observed.SelectedBinBytes,
            observed.SelectedRelocationBytes,observed.SourceSha256,observed.ManifestSha256);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fingerprint isolated bytes without changing native asset identities or production registration. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
