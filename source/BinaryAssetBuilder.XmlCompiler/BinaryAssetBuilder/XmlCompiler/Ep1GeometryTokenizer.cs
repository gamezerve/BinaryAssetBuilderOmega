using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: generate reviewed geometry/list/shape/contact metadata from a bounded owned EP1 native graph, with no stock prefix copying.
public static class Ep1GeometryTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit exact native ownership before generating tables or rebasing pointers; unsupported shapes/aliases/auxiliary slots fail closed. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native?.InstanceBuffer==null || native.InstanceBuffer.Length<76 || native.InstanceBuffer.Length>1048576 || native.ImportsBuffer==null || native.ImportsBuffer.Length!=0)throw new InvalidDataException("Geometry requires a bounded EP1 native graph without imports.");
        var b=native.InstanceBuffer;var relos=Slots(native.RelocationBuffer);var expected=new HashSet<uint>();var owners=new List<(int Start,int End,bool Text)>();
        Own(0,28,false);int shapes=(int)Word(b,8),contacts=(int)Word(b,16);
        if(shapes<1 || shapes>32 || contacts<0 || contacts>128 || b[24]>1 || b.AsSpan(25,3).ToArray().Any(v=>v!=0))throw new InvalidDataException("Geometry count/flag/padding exceeds the reviewed profile.");
        int rotation=Pointer(0,8),placement=Pointer(4,48),shapeItems=Pointer(12,checked(shapes*48)),contactItems=Pointer(20,checked(contacts*20));
        if(shapeItems==0 || (contacts==0)!=(contactItems==0))throw new InvalidDataException("Geometry count/items presence differs.");
        if(rotation!=0)Reals(rotation,2);if(placement!=0)Shape(placement);
        for(int i=0;i<shapes;i++)Shape(shapeItems+i*48);
        for(int i=0;i<contacts;i++){int member=contactItems+i*20;Text(member);Reals(member+8,3);}
        if(!expected.OrderBy(s=>s).SequenceEqual(relos.OrderBy(s=>s)))throw new InvalidDataException("Geometry relocation ownership differs.");
        // Reborn: only alignment padding may remain outside recognized allocations; hidden native tails or orphan buffers are refused.
        int end=0;foreach(var owner in owners.OrderBy(o=>o.Start)){if(owner.Start<end)throw new InvalidDataException("Geometry allocations overlap.");Padding(end,owner.Start);end=owner.End;}Padding(end,b.Length);
        int placementTable=placement==0?0:72,shapeList=72+(placement==0?0:144),contactList=checked(shapeList+12+shapes*156),prefix=checked(contactList+12+contacts*48);
        var output=new Chunk{InstanceBuffer=new byte[checked(prefix+b.Length)],ImportsBuffer=Array.Empty<byte>()};b.CopyTo(output.InstanceBuffer,prefix);
        Token(0,"IsSmall",0x77E3F4F7u,(uint)(prefix+24));
        Token(12,"RotationAnchorOffset",0x4AE31F24u,rotation==0?0u:(uint)(prefix+rotation));
        Token(24,"PlacementShape",0xE7210555u,(uint)placementTable);
        Token(36,"Shape",0xE7210555u,(uint)shapeList);
        Token(48,"ContactPoint",0x6B8EE8EFu,(uint)contactList);
        if(placement!=0)ShapeTable(placementTable,placement);
        Row(shapeList,0x6C827073u,0x5D448FD8u,(uint)(prefix+8));
        for(int i=0;i<shapes;i++){int table=shapeList+12+shapes*12+i*144;Row(shapeList+12+i*12,0xB151A0EBu,0,(uint)table);ShapeTable(table,shapeItems+i*48);}
        Row(contactList,0x6C827073u,0x5D448FD8u,(uint)(prefix+16));
        for(int i=0;i<contacts;i++){int table=contactList+12+contacts*12+i*36,member=prefix+contactItems+i*20;Row(contactList+12+i*12,0xB151A0EBu,0,(uint)table);Token(table,"Label",0x1A2933AAu,(uint)member);Token(table+12,"Pos",0x29368077u,(uint)(member+8));}
        foreach(uint slot in relos)Put(output.InstanceBuffer,prefix+(int)slot,checked(Word(b,(int)slot)+(uint)prefix));output.RelocationBuffer=Pack(relos.Select(s=>checked(s+(uint)prefix)));return output;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: explicit primitive padding is zero and no larger than three bytes under the reviewed x86 allocator. */
        //-------------------------------------------------------------------------------------------------
        void Padding(int start,int finish){if(finish<start || finish-start>3 || b.AsSpan(start,finish-start).ToArray().Any(v=>v!=0))throw new InvalidDataException("Unowned geometry bytes or nonzero padding.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only exactly identical string allocations may alias; records/vectors never share or overlap owners. */
        //-------------------------------------------------------------------------------------------------
        void Own(int start,int length,bool text)
        {
            if(start<0 || length<=0 || start>b.Length-length)throw new InvalidDataException("Geometry allocation bounds exceeded.");int end=checked(start+length);
            foreach(var old in owners.Where(o=>start<o.End && end>o.Start)){if(text && old.Text && start==old.Start && end==old.End)return;throw new InvalidDataException("Geometry allocation aliases another owner.");}owners.Add((start,end,text));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: nullable vector/record/list pointers have exact relocation membership and four-byte aligned allocated targets. */
        //-------------------------------------------------------------------------------------------------
        int Pointer(int slot,int length){uint target=Word(b,slot);if(target==0)return 0;if(target%4!=0 || target<28 || target>int.MaxValue || length==0)throw new InvalidDataException("Geometry pointer alignment/presence differs.");Own((int)target,length,false);expected.Add((uint)slot);return (int)target;}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: geometry strings currently admit bounded non-NUL ASCII, including allocated empty strings, and own only their data/terminator. */
        //-------------------------------------------------------------------------------------------------
        void Text(int at)
        {
            uint length=Word(b,at),pointer=Word(b,at+4);if(pointer==0){if(length!=0)throw new InvalidDataException("Absent geometry string has length.");return;}
            if(length>4096 || pointer<28 || pointer>b.Length-1 || length>b.Length-pointer-1 || b[(int)(pointer+length)]!=0)throw new InvalidDataException("Geometry string bounds/terminator differ.");
            for(uint i=0;i<length;i++)if(b[(int)(pointer+i)]==0 || b[(int)(pointer+i)]>127)throw new InvalidDataException("Unsupported geometry string encoding.");Own((int)pointer,(int)length+1,true);expected.Add((uint)(at+4));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: float fields and fixed vectors cannot silently retain NaN/infinity payloads. */
        //-------------------------------------------------------------------------------------------------
        void Reals(int at,int count){for(int i=0;i<count;i++){float value=BitConverter.Int32BitsToSingle(unchecked((int)Word(b,at+i*4)));if(float.IsNaN(value) || float.IsInfinity(value))throw new InvalidDataException("Nonfinite geometry scalar/vector.");}}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: exact shape enum/flag widths, scalar fields, strings and optional vector ownership are checked independently of source validation. */
        //-------------------------------------------------------------------------------------------------
        void Shape(int at){if(Word(b,at)>3 || Word(b,at+4)>4 || b[at+44]>1 || b[at+45]>1 || b[at+46]!=0 || b[at+47]!=0)throw new InvalidDataException("Unsupported geometry shape enum/flag/padding.");Text(at+8);Text(at+24);Reals(at+16,2);Reals(at+32,2);int offset=Pointer(at+40,12);if(offset!=0)Reals(offset,3);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: generated shape tables preserve official field ordering while fixed vectors target native payloads directly. */
        //-------------------------------------------------------------------------------------------------
        void ShapeTable(int table,int at)
        {
            var fields=new[]{("Type",0xB0D49164u,0),("ContactPointGeneration",0x465508CEu,4),("Name",0x1A2933AAu,8),("Active",0x77E3F4F7u,44),("UsedForHealthBox",0x77E3F4F7u,45),("MajorRadius",0x645864F3u,16),("MinorRadius",0x645864F3u,20),("Other",0x1A2933AAu,24),("Height",0x645864F3u,32),("FrontAngle",0x4E16EA4Du,36)};
            for(int i=0;i<fields.Length;i++)Token(table+i*12,fields[i].Item1,fields[i].Item2,(uint)(prefix+at+fields[i].Item3));uint offset=Word(b,at+40);Token(table+120,"Offset",0x29368077u,offset==0?0:checked(offset+(uint)prefix));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: type/name words are metadata and never enter native relocation lists. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at,string name,uint type,uint target)=>Row(at,FastHash.GetHashCode(name),type,target);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: every generated metadata row is an owned little-endian triplet. */
        //-------------------------------------------------------------------------------------------------
        void Row(int at,uint name,uint type,uint target){Put(output.InstanceBuffer,at,name);Put(output.InstanceBuffer,at+4,type);Put(output.InstanceBuffer,at+8,target);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native relocation streams require unique aligned in-range slots and exactly one sentinel; imports are separately forbidden. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)
    {
        if(b==null || b.Length%4!=0)throw new InvalidDataException("Malformed geometry relocation stream.");if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Geometry relocation sentinel differs.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Any(s=>s%4!=0 || s>1048572) || slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Geometry relocation slot differs.");return slots;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: rebased relocation order and empty streams remain identical to the native allocator's contract. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed public input cannot turn a token read into an unchecked native memory access. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Geometry word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: geometry native pointers and token coordinates are little-endian Win32 words. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
