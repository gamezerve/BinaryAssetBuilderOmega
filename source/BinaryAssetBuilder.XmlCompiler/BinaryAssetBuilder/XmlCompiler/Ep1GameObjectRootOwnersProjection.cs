using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: explicitly join six complete reviewed native owners to the selected root; this remains a partial GameObject projection, not general asset publication.
public static class Ep1GameObjectRootOwnersProjection
{
    // Reborn: fixed reviewed root slots and native types are independent of per-asset stock addresses.
    public readonly record struct Field(string Name,int Slot,Type NativeType);
    public static IReadOnlyList<Field> Fields { get; }=Array.AsReadOnly(new[]{
        new Field("ObjectResourceInfo",416,typeof(ObjectResourceInfo)),new Field("SkirmishAIInformation",468,typeof(Ep1SkirmishAIInformation)),
        new Field("AudioArrayVoice",532,typeof(Ep1AudioArrayVoice)),new Field("EvaEventArray",540,typeof(Ep1EvaEventArray)),
        new Field("ShadowInfo",552,typeof(ShadowInfo)),new Field("CrusherInfo",564,typeof(Ep1CrusherInfo))});

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compose metadata only; every native byte and common reference index comes from one caller-owned graph and shifts globally exactly once. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Compose(Chunk native,XmlSchemaSet schemas)
    {
        if(native?.InstanceBuffer==null || native.RelocationBuffer==null || native.ImportsBuffer==null || schemas==null || native.InstanceBuffer.Length<1100 || native.InstanceBuffer.Length>1048576)throw new InvalidDataException("Bounded six-owner root/schema required.");
        byte[] b=native.InstanceBuffer;uint[] relos=Slots(native.RelocationBuffer),imports=Slots(native.ImportsBuffer);int[] starts=Fields.Select(f=>checked((int)Word(b,f.Slot))).ToArray();int cut=starts[0];
        if(cut<620 || starts.Any(s=>s%4!=0 || s<cut || s>b.Length-4) || !starts.SequenceEqual(starts.Distinct().Order()))throw new InvalidDataException("Root owners must have distinct ordered bounded native tails.");
        if(relos.Intersect(imports).Any() || relos.Concat(imports).Any(s=>s>b.Length-4))throw new InvalidDataException("Root owner auxiliary overlap/bounds.");
        foreach(var f in Fields)if(!relos.Contains((uint)f.Slot) || imports.Contains((uint)f.Slot))throw new InvalidDataException("Root owner pointer relocation missing.");
        var baseBytes=b.AsSpan(0,cut).ToArray();foreach(var f in Fields)Put(baseBytes,f.Slot,0);
        uint[] baseRelos=relos.Where(s=>s<cut && !Fields.Any(f=>f.Slot==s)).ToArray(),baseImports=imports.Where(s=>s<cut).ToArray();
        foreach(uint slot in baseRelos)if(Word(b,(int)slot)>=cut)throw new InvalidDataException("Base graph crosses remaining owner boundary.");
        var basis=new Chunk{InstanceBuffer=baseBytes,RelocationBuffer=Pack(baseRelos),ImportsBuffer=Pack(baseImports)};
        var old=Ep1GameObjectDestructionProjection.Compose(basis,schemas);int oldPrefix=old.InstanceBuffer.Length-cut,insert=0;
        for(;insert<oldPrefix;insert+=12)if(Word(old.InstanceBuffer,insert)==0 && Word(old.InstanceBuffer,insert+4)==0 && Word(old.InstanceBuffer,insert+8)==0)break;
        if(insert>=oldPrefix)throw new InvalidDataException("Selected root sentinel missing.");
        var pieces=new List<(Chunk Tokens,int Prefix,int Start)>();
        for(int i=0;i<Fields.Count;i++)
        {
            int start=starts[i],end=i+1<starts.Length?starts[i+1]:b.Length;var leaf=Extract(native,start,end);var tokens=Ep1BehaviorMetadataTokenizer.TokenizeRootRecord(leaf,schemas,Fields[i].Name);pieces.Add((tokens,tokens.InstanceBuffer.Length-leaf.InstanceBuffer.Length,start));
        }
        int extra=Fields.Count*12,prefix=checked(oldPrefix+extra+pieces.Sum(p=>p.Prefix));var output=new Chunk{InstanceBuffer=new byte[checked(prefix+b.Length)]};
        old.InstanceBuffer.AsSpan(0,insert).CopyTo(output.InstanceBuffer);old.InstanceBuffer.AsSpan(insert,oldPrefix-insert).CopyTo(output.InstanceBuffer.AsSpan(insert+extra));
        // Reborn: existing metadata table origins move by inserted root rows; existing native targets instead move to the single enlarged native origin.
        for(int at=0;at<oldPrefix;at+=12){int destination=at<insert?at:at+extra;uint target=Word(old.InstanceBuffer,at+8);if(target!=0)Put(output.InstanceBuffer,destination+8,target<oldPrefix?target+(uint)(target>=insert?extra:0):checked(target-(uint)oldPrefix+(uint)prefix));}
        int next=oldPrefix+extra;
        for(int i=0;i<Fields.Count;i++)
        {
            var field=Fields[i];var piece=pieces[i];int row=insert+i*12;Put(output.InstanceBuffer,row,FastHash.GetHashCode(field.Name));Put(output.InstanceBuffer,row+4,FastHash.GetHashCode(field.Name));Put(output.InstanceBuffer,row+8,(uint)next);
            piece.Tokens.InstanceBuffer.AsSpan(0,piece.Prefix).CopyTo(output.InstanceBuffer.AsSpan(next));for(int at=8;at<piece.Prefix;at+=12){uint target=Word(piece.Tokens.InstanceBuffer,at);if(target!=0)Put(output.InstanceBuffer,next+at,target<piece.Prefix?target+(uint)next:checked(target-(uint)piece.Prefix+(uint)prefix+(uint)piece.Start));}next+=piece.Prefix;
        }
        b.CopyTo(output.InstanceBuffer,prefix);foreach(uint slot in relos){uint target=Word(b,(int)slot);if(target>=b.Length)throw new InvalidDataException("Root owner relocation target bounds.");Put(output.InstanceBuffer,prefix+(int)slot,checked(target+(uint)prefix));}
        output.RelocationBuffer=Pack(relos.Select(s=>s+(uint)prefix));output.ImportsBuffer=Pack(imports.Select(s=>s+(uint)prefix));return output;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every extracted owner retains shared Core occurrences and refuses pointers escaping its independent native range. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk native,int start,int end)
    {
        var chunk=new Chunk{InstanceBuffer=native.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(native.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(native.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};
        foreach(uint slot in Slots(chunk.RelocationBuffer)){uint target=Word(chunk.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Root owner pointer crosses another native owner.");Put(chunk.InstanceBuffer,(int)slot,target-(uint)start);}return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact auxiliary framing and unique aligned source slots precede any ownership partitioning. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b==null || b.Length%4!=0 || b.Length==4 || b.Length!=0 && Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Root owners auxiliary framing.");if(b.Length==0)return Array.Empty<uint>();var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Any(s=>s%4!=0) || slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Root owners duplicate/unaligned slots.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve slot order and the single nonempty sentinel when changing coordinate origins. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: supplied native and generated metadata words cannot escape their owning buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Root owners word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: patch only newly owned little-endian native/metadata arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
