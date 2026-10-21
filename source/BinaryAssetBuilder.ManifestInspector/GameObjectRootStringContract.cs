using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 root ASCII strings own ordered aligned allocations; this contract independently resizes them and rebases every later native pointer/auxiliary slot.
internal static class GameObjectRootStringContract
{
    private const int NativeRootSize=600;
    internal static IReadOnlyList<Ep1GameObjectScalarTokens.Field> Fields=>Ep1GameObjectStringTokens.Fields;
    internal static int AllocationSize(XmlDocument source)=>Values(source).Sum(v=>v.Allocation);
    internal static int PresentCount(XmlDocument source)=>Values(source).Count(v=>v.Present);

    private sealed record Value(Ep1GameObjectScalarTokens.Field Field,bool Present,byte[] Bytes)
    {
        internal int Allocation=>Present?(Bytes.Length+4)&~3:0;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept non-NUL ASCII independently of the old allocation extent; checked buffer/source limits replace per-key trial quotas. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source,XmlDocument baseline){_=Values(source);_=Values(baseline);}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admitted root strings are semantic inputs; their dynamic storage is checked and rebased rather than hidden by structural comparison. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var field in Fields){candidate.DocumentElement!.RemoveAttribute(field.Name);if(model.DocumentElement!.HasAttribute(field.Name))candidate.DocumentElement.SetAttribute(field.Name,model.DocumentElement.GetAttribute(field.Name));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: vary all existing strings and populate all absent string slots in one grouped allocation-growth probe. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source,XmlDocument baseline)
    {
        foreach(var field in Fields)
        {
            string value=baseline.DocumentElement!.GetAttribute(field.Name);if(field.Name=="TypeDescription"){source.DocumentElement!.RemoveAttribute(field.Name);continue;}source.DocumentElement!.SetAttribute(field.Name,field.Name=="EditorName"?string.Empty:value.Length==0?"Reborn"+field.Name+"Probe":value+"_RebornProbe");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: canonicalize authored allocation growth/shrinkage back to the baseline graph before the whole-stock ownership oracle runs. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Normalize(XmlDocument source,XmlDocument baseline,Chunk actual,Chunk baselineChunk)
    {
        var baselineValues=Values(baseline);var baselineSlots=Slots(baselineChunk.RelocationBuffer);var baselineStringSlots=baselineValues.Where(v=>v.Present).Select(v=>(uint)(v.Field.Offset+4)).ToHashSet();int insertion=Array.FindIndex(baselineSlots,baselineStringSlots.Contains);if(insertion<0)throw new InvalidDataException("Baseline root string relocation group absent.");var normalized=Rewrite(source,baseline,actual,0,insertion);if(normalized.InstanceBuffer.Length!=baselineChunk.InstanceBuffer.Length || normalized.RelocationBuffer.Length!=baselineChunk.RelocationBuffer.Length || normalized.ImportsBuffer.Length!=actual.ImportsBuffer.Length)throw new InvalidDataException("Normalized root string graph extent differs.");return normalized;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently derive expected metadata/native-tail bytes from the stock token graph and authored dynamic string layout. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Expected(XmlDocument baseline,XmlDocument source,Chunk tokens,int prefix)=>Rewrite(baseline,source,tokens,prefix);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: package publication rechecks the complete ordered allocation run, exact content, terminators, padding and header pointers. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,int prefix)=>Inspect(Values(source),native,prefix);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: replace only the contiguous root-string block run and transform every subsequent relocation/import coordinate exactly once. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Rewrite(XmlDocument currentSource,XmlDocument targetSource,Chunk input,int prefix,int? emptyCurrentInsertion=null)
    {
        if(input?.InstanceBuffer==null || input.RelocationBuffer==null || input.ImportsBuffer==null || prefix<0 || prefix>input.InstanceBuffer.Length-NativeRootSize)throw new InvalidDataException("Bounded root string graph required.");
        var current=Values(currentSource);var target=Values(targetSource);int start=checked(prefix+NativeRootSize),currentEnd=Inspect(current,input.InstanceBuffer,prefix),targetEnd=checked(start+target.Sum(v=>v.Allocation)),delta=targetEnd-currentEnd;
        var output=new Chunk{InstanceBuffer=new byte[checked(input.InstanceBuffer.Length+delta)]};input.InstanceBuffer.AsSpan(0,start).CopyTo(output.InstanceBuffer);input.InstanceBuffer.AsSpan(currentEnd).CopyTo(output.InstanceBuffer.AsSpan(targetEnd));
        int next=start;
        foreach(var value in target)
        {
            int header=prefix+value.Field.Offset;Put(output.InstanceBuffer,header,(uint)value.Bytes.Length);Put(output.InstanceBuffer,header+4,value.Present?(uint)next:0);if(!value.Present)continue;value.Bytes.CopyTo(output.InstanceBuffer.AsSpan(next));next=checked(next+value.Allocation);
        }
        if(next!=targetEnd)throw new InvalidDataException("Target root string extent differs.");
        // Reborn: tokenized metadata consists of aligned 12-byte rows whose third word targets metadata or native storage but is not itself listed in RELO.
        for(int at=8;at<prefix;at+=12){uint targetWord=Word(input.InstanceBuffer,at);if(targetWord>=(uint)currentEnd && targetWord<(uint)input.InstanceBuffer.Length)Put(output.InstanceBuffer,at,Map(targetWord));}
        var currentRelos=Slots(input.RelocationBuffer);var currentStringSlots=current.Where(v=>v.Present).Select(v=>(uint)(prefix+v.Field.Offset+4)).ToHashSet();
        foreach(uint slot in currentStringSlots)if(!currentRelos.Contains(slot))throw new InvalidDataException("Root string relocation slot absent.");
        int stringInsertion=currentStringSlots.Count==0?emptyCurrentInsertion??-1:Array.FindIndex(currentRelos,currentStringSlots.Contains);if(target.Any(v=>v.Present) && (stringInsertion<0 || stringInsertion>currentRelos.Length))throw new InvalidDataException("Root string relocation insertion point absent.");var relos=new List<uint>();
        for(int index=0;index<currentRelos.Length;index++)
        {
            if(index==stringInsertion)relos.AddRange(target.Where(v=>v.Present).Select(v=>(uint)(prefix+v.Field.Offset+4)));uint slot=currentRelos[index];if(currentStringSlots.Contains(slot))continue;
            uint mapped=Map(slot),oldTarget=Word(input.InstanceBuffer,(int)slot),mappedTarget=Map(oldTarget);Put(output.InstanceBuffer,(int)mapped,mappedTarget);relos.Add(mapped);
        }
        if(stringInsertion==currentRelos.Length)relos.AddRange(target.Where(v=>v.Present).Select(v=>(uint)(prefix+v.Field.Offset+4)));
        output.RelocationBuffer=Pack(relos);
        var imports=Slots(input.ImportsBuffer).Select(Map).ToArray();output.ImportsBuffer=Pack(imports);
        if(relos.Distinct().Count()!=relos.Count || imports.Distinct().Count()!=imports.Length)throw new InvalidDataException("Rebased root string auxiliary slot alias.");
        return output;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: coordinates outside the replaced allocation run retain order; no pointer may target the removed string interiors. */
        //-------------------------------------------------------------------------------------------------
        uint Map(uint coordinate)
        {
            if(coordinate<(uint)start)return coordinate;if(coordinate>=(uint)currentEnd)return checked((uint)(coordinate+delta));throw new InvalidDataException("Non-string coordinate entered the replaced root string allocation run.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: root string blocks are the first post-root Tracker allocations, ordered by the concrete GameObject marshaler and padded to four bytes. */
    //-------------------------------------------------------------------------------------------------
    private static int Inspect(IReadOnlyList<Value> values,byte[] native,int prefix)
    {
        int next=checked(prefix+NativeRootSize);
        foreach(var value in values)
        {
            int header=prefix+value.Field.Offset;uint length=Word(native,header),pointer=Word(native,header+4);
            if(length!=value.Bytes.Length || (value.Present?pointer!=next:pointer!=0))throw new InvalidDataException("GameObject root string header/allocation differs: "+value.Field.Name);
            if(!value.Present)continue;if(next>native.Length-value.Allocation || !native.AsSpan(next,value.Bytes.Length).SequenceEqual(value.Bytes) || native[next+value.Bytes.Length]!=0 || native.AsSpan(next+value.Bytes.Length+1,value.Allocation-value.Bytes.Length-1).ToArray().Any(b=>b!=0))throw new InvalidDataException("GameObject root string content/padding differs: "+value.Field.Name);next=checked(next+value.Allocation);
        }
        return next;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source presence distinguishes a missing pointer from an explicitly empty allocated string. */
    //-------------------------------------------------------------------------------------------------
    private static Value[] Values(XmlDocument source)=>Fields.Select(field=>
    {
        // Reborn: root keys have no stock-example length ceiling; existing checked graph/source allocation bounds still apply.
        bool present=source.DocumentElement!.HasAttribute(field.Name);string text=present?source.DocumentElement.GetAttribute(field.Name):string.Empty;if(text.Any(c=>c==0 || c>127))throw new InvalidDataException("Root string currently requires non-NUL ASCII: "+field.Name);return new Value(field,present,Encoding.ASCII.GetBytes(text));
    }).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary tables preserve EA sentinel framing and reject duplicate source slots. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8 || bytes.Length%4!=0 || Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Root string auxiliary framing differs.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Root string auxiliary slot duplicate.");return slots;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: transformed auxiliary tables are emitted in deterministic native source order. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){uint[] slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var bytes=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(bytes,i*4,slots[i]);Put(bytes,slots.Length*4,uint.MaxValue);return bytes;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: all root string graph reads and writes remain inside explicitly owned arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int offset){if(offset<0 || offset>bytes.Length-4)throw new InvalidDataException("Root string graph word is out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));}
    private static void Put(byte[] bytes,int offset,uint value){if(offset<0 || offset>bytes.Length-4)throw new InvalidDataException("Root string graph write is out of bounds.");BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(offset,4),value);}
}
