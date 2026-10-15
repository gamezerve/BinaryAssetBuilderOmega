using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: inline AI records occupy pre-behavior storage and must be partitioned from root strings, arrays and pointed children before fragment composition.
public static class Ep1GameObjectAiRoot
{
    // Reborn: each reviewed AI keeps its independent native and metadata coordinate origins; the enclosing root owns only the count/items list.
    public sealed record Module(int Start,int End,Chunk Native,Chunk Tokens)
    {
        public int Prefix=>Tokens.InstanceBuffer.Length-Native.InstanceBuffer.Length;
    }
    // Reborn: an empty native list has neither items storage nor emitted module fragments in the selected partial-root profile.
    public sealed record Layout(int Items,IReadOnlyList<Module> Modules);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind the inline count/items/ordered modules to disjoint bounded native owners and recheck every extracted AI through its standalone tokenizer. */
    //-------------------------------------------------------------------------------------------------
    internal static Layout Inspect(byte[] data,uint list,uint[] relocations,uint[] imports,uint armor,uint armorCount,uint locomotor,uint locomotorCount,IReadOnlyList<Ep1GameObjectPointerRoots.Field> pointerRoots,IReadOnlyList<(uint Start,uint End)> strings,List<uint> expectedRelos,List<uint> expectedImports)
    {
        uint count=Word(480),items=Word(484);
        if(count>2 || (count==0?items!=0:items<600 || items%4!=0 || items>list || count*4>list-items))throw new InvalidDataException("Selected inline AI count/items ownership differs.");
        if(count==0)return new(0,Array.Empty<Module>());
        var outer=strings.Select(r=>((int)r.Start,(int)r.End)).ToList();
        if(armorCount!=0)outer.Add(((int)armor,checked((int)(armor+armorCount*12))));if(locomotorCount!=0)outer.Add(((int)locomotor,checked((int)(locomotor+locomotorCount*12))));
        foreach(var field in pointerRoots)
        {
            outer.Add((field.Target,field.Target+field.Size));
            if(field.Name is "DisplayName" or "DisplayNameTransformed"){uint text=Word(field.Target+4),length=Word(field.Target);if(text!=0)outer.Add(((int)text,checked((int)(text+length+1))));}
        }
        int array=(int)items,arrayEnd=checked(array+(int)count*4);if(outer.Any(r=>array<r.Item2 && arrayEnd>r.Item1))throw new InvalidDataException("Inline AI items alias another root allocation.");outer.Add((array,arrayEnd));
        var starts=new int[count];for(int i=0;i<count;i++){uint target=Word(array+i*4);if(target<arrayEnd || target%4!=0 || target>list || 76>list-target || (i!=0 && target<starts[i-1]+76))throw new InvalidDataException("Inline AI module bounds/order differ.");starts[i]=(int)target;}
        var modules=new List<Module>();expectedRelos.Add(484);
        for(int i=0;i<starts.Length;i++)
        {
            int start=starts[i],end=i+1<starts.Length?starts[i+1]:(int)list;
            // Reborn: later camera/vision/name allocations can follow the last AI; they must not be consumed as AI tail storage.
            foreach(var range in outer)if(range.Item1>start && range.Item1<end)end=range.Item1;
            if(end-start<76 || outer.Any(r=>start<r.Item2 && end>r.Item1))throw new InvalidDataException("Inline AI module aliases root storage.");
            var localRelos=relocations.Where(s=>s>=start && s<end).ToArray();var localImports=imports.Where(s=>s>=start && s<end).ToArray();
            // Reborn: an auxiliary word may not straddle a fragment boundary even when its first byte falls inside that owner.
            if(localRelos.Concat(localImports).Any(s=>s%4!=0 || s>end-4))throw new InvalidDataException("Inline AI auxiliary slot alignment/bounds differ.");
            var native=new Chunk{InstanceBuffer=data.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(localRelos.Select(s=>s-(uint)start)),ImportsBuffer=Pack(localImports.Select(s=>s-(uint)start))};
            foreach(uint slot in localRelos){uint target=Word((int)slot);if(target<start || target>=end)throw new InvalidDataException("Inline AI pointer crosses fragment owners.");BinaryPrimitives.WriteUInt32LittleEndian(native.InstanceBuffer.AsSpan((int)slot-start,4),target-(uint)start);}
            var tokens=Ep1AIUpdateTokenizer.Tokenize(native);modules.Add(new(start,end,native,tokens));expectedRelos.Add((uint)(array+i*4));expectedRelos.AddRange(localRelos);expectedImports.AddRange(localImports);outer.Add((start,end));
        }
        return new(array,Array.AsReadOnly(modules.ToArray()));
        //-------------------------------------------------------------------------------------------------
        /** Reborn: native list/module/pointed-child reads remain inside the captured GameObject bytes. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at){if(at<0 || at>data.Length-4)throw new InvalidDataException("Inline AI root read bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(at,4));}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: locally extracted auxiliary slots keep occurrence order and explicit empty/sentinel framing. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.ToArray();if(values.Length==0)return Array.Empty<byte>();var bytes=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(i*4,4),values[i]);BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(values.Length*4,4),uint.MaxValue);return bytes;}
}
