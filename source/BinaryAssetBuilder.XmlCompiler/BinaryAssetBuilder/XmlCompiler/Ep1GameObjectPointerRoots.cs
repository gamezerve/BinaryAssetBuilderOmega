using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: selected pointed root children own pre-behavior records, unlike inline enum/color words or post-Body geometry.
public static class Ep1GameObjectPointerRoots
{
    // Reborn: native targets remain separate from root slots; Flammability and VisionInfo need distinct own-field metadata tables.
    public readonly record struct Field(string Name,int Slot,uint Type,int Target,int Size)
    {
        public bool HasTable=>MetadataSize!=0;
        public int MetadataSize=>Name=="Flammability"?60:Name=="VisionInfo"?132:0;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit bounded disjoint header/vector/flammability owners and nested ASCII storage before accepting their exact relocation slots. */
    //-------------------------------------------------------------------------------------------------
    internal static IReadOnlyList<Field> Inspect(byte[] data,uint list,uint armor,uint armorCount,uint locomotor,uint locomotorCount,List<uint> relocations,IReadOnlyList<(uint Start,uint End)> strings)
    {
        // Reborn: the enclosing root inspector supplies reviewed list/array/string owners; retain an explicit byte-envelope guard at this seam too.
        if(data==null || data.Length<600 || list<600 || list>data.Length)throw new InvalidDataException("Pointed root envelope differs.");
        var fields=new List<Field>();var records=new List<(uint Start,uint End)>();var texts=strings.ToList();
        if(armorCount!=0)records.Add((armor,armor+armorCount*12));if(locomotorCount!=0)records.Add((locomotor,locomotor+locomotorCount*12));
        foreach(var spec in new[]{("DisplayName",404,0x1A2933AAu,8),("DisplayNameTransformed",408,0x1A2933AAu,8),("Flammability",464,0x10444D6Cu,16),("LiveCameraOffset",528,0x29368077u,12),("VisionInfo",560,0x649F97D1u,40)})
        {
            uint target=Word(spec.Item2);if(target==0)continue;
            if(target<600 || target%4!=0 || target>list || spec.Item4>list-target)throw new InvalidDataException("Pointed root allocation bounds/alignment differ: "+spec.Item1);
            uint end=target+(uint)spec.Item4;
            if(records.Any(r=>target<r.End && end>r.Start) || texts.Any(r=>target<r.End && end>r.Start))throw new InvalidDataException("Pointed root record aliases existing root storage.");
            records.Add((target,end));fields.Add(new(spec.Item1,spec.Item2,spec.Item3,(int)target,spec.Item4));relocations.Add((uint)spec.Item2);
        }
        foreach(var field in fields)
        {
            if(field.Name is "DisplayName" or "DisplayNameTransformed")
            {
                uint length=Word(field.Target),pointer=Word(field.Target+4);if(pointer==0){if(length!=0)throw new InvalidDataException("Absent pointed root text has length.");continue;}
                if(pointer<600 || pointer>=list || length>list-pointer-1 || data[(int)(pointer+length)]!=0)throw new InvalidDataException("Pointed root text bounds/terminator differ.");uint end=pointer+length+1;
                if(records.Any(r=>pointer<r.End && end>r.Start) || texts.Any(r=>pointer<r.End && end>r.Start && (pointer!=r.Start || end!=r.End)))throw new InvalidDataException("Pointed root text aliases a record or partially overlaps text.");
                for(uint i=0;i<length;i++)if(data[(int)(pointer+i)]==0 || data[(int)(pointer+i)]>127)throw new InvalidDataException("Pointed root text currently requires non-NUL ASCII.");texts.Add((pointer,end));relocations.Add((uint)(field.Target+4));
            }
            else if(field.Name=="LiveCameraOffset")
                for(int i=0;i<3;i++){float value=BitConverter.Int32BitsToSingle(unchecked((int)Word(field.Target+i*4)));if(float.IsNaN(value) || float.IsInfinity(value))throw new InvalidDataException("Nonfinite root camera vector.");}
            // Reborn: vision's unsigned segment count is not a float; every other four-byte word must be finite.
            else if(field.Name=="VisionInfo")
                for(int i=0;i<10;i++)if(i!=6 && !float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(field.Target+i*4)))))throw new InvalidDataException("Nonfinite root vision field.");
        }
        return Array.AsReadOnly(fields.ToArray());
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all header/scalar reads stay inside the bounded native GameObject graph. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at){if(at<0 || at>data.Length-4)throw new InvalidDataException("Pointed root read bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(at,4));}
    }
}
