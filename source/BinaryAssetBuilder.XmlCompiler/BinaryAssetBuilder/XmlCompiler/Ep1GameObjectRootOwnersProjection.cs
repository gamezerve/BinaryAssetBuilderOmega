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
    // Reborn: repeated root records own an inline count/items header; optional pointer records retain null versus authored-empty semantics.
    public readonly record struct Field(string Name,int Slot,Type NativeType,bool InlineList=false)
    {
        public int PointerSlot=>InlineList?Slot+4:Slot;
        public bool Optional=>InlineList||Name is "GameDependency" or "ObjectResourceInfo" or "SkirmishAIInformation" or "AudioArrayVoice" or "AudioArraySound" or "EvaEventArray" or "ShadowInfo" or "CrusherInfo";
        public uint Wire=>Name switch{"GameDependency"=>0x6D7966DEu,"AudioArraySound"=>0x59867D0Bu,"ProjectedBuildabilityInfo"=>0x52BD0121u,_=>FastHash.GetHashCode(Name)};
    }
    public static IReadOnlyList<Field> Fields { get; }=Array.AsReadOnly(new[]{
        // Reborn: optional dependency records use the same one-Tracker tail ownership as other source-sized root records.
        new Field("GameDependency",412,typeof(GameDependencyType)),new Field("ObjectResourceInfo",416,typeof(ObjectResourceInfo)),new Field("SkirmishAIInformation",468,typeof(Ep1SkirmishAIInformation)),
        new Field("AudioArrayVoice",532,typeof(Ep1AudioArrayVoice)),new Field("AudioArraySound",536,typeof(Ep1AudioArraySound)),new Field("EvaEventArray",540,typeof(Ep1EvaEventArray)),
        new Field("ShadowInfo",552,typeof(ShadowInfo)),new Field("CrusherInfo",564,typeof(Ep1CrusherInfo)),new Field("ProjectedBuildabilityInfo",568,typeof(ProjectedBuildabilityInfo),true)});

    // Reborn: an inline list's native owner starts at its item array, never at its count word.
    public static int NativeStart(byte[] bytes,Field field)=>checked((int)Word(bytes,field.PointerSlot));
    // Reborn: detach both inline header words from the base graph, while leaving actual caller-owned output unchanged.
    public static void ClearSlot(byte[] bytes,Field field){Put(bytes,field.Slot,0);if(field.InlineList)Put(bytes,field.PointerSlot,0);}

    // Reborn: the first actually allocated tail owner bounds geometry/base storage; ObjectResourceInfo may legitimately be absent.
    public static int NativeTailStart(byte[] native)
    {
        var starts=Fields.Select(f=>Word(native,f.PointerSlot)).Where(at=>at!=0).ToArray();
        // Reborn: reject a corrupt optional pointer before it can select an invalid slicing boundary for another owner.
        if(starts.Any(at=>at<600||at%4!=0||at>native.Length-4))throw new InvalidDataException("Native root owner tail pointer bounds/alignment differ.");
        return starts.Length==0?native.Length:checked((int)starts.Min());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compose metadata only; every native byte and common reference index comes from one caller-owned graph and shifts globally exactly once. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Compose(Chunk native,XmlSchemaSet schemas)
    {
        if(native?.InstanceBuffer==null || native.RelocationBuffer==null || native.ImportsBuffer==null || schemas==null || native.InstanceBuffer.Length<1100 || native.InstanceBuffer.Length>1048576)throw new InvalidDataException("Bounded six-owner root/schema required.");
        byte[] b=native.InstanceBuffer;uint[] relos=Slots(native.RelocationBuffer),imports=Slots(native.ImportsBuffer);int[] starts=Fields.Select(f=>NativeStart(b,f)).ToArray();int cut=NativeTailStart(b);var present=starts.Where(s=>s!=0).ToArray();
        // Reborn: only the implemented optional resource pointer may be null; populated owners must still occupy distinct ordered native tails.
        if(Fields.Where(f=>!f.Optional).Any(f=>NativeStart(b,f)==0)||cut<620 || present.Any(s=>s%4!=0 || s<cut || s>b.Length-4) || !present.SequenceEqual(present.Distinct().Order()))throw new InvalidDataException("Root owners must have distinct ordered bounded native tails.");
        if(relos.Intersect(imports).Any() || relos.Concat(imports).Any(s=>s>b.Length-4))throw new InvalidDataException("Root owner auxiliary overlap/bounds.");
        foreach(var f in Fields)if(relos.Contains((uint)f.PointerSlot)!=(Word(b,f.PointerSlot)!=0) || imports.Contains((uint)f.PointerSlot))throw new InvalidDataException("Root owner pointer relocation presence differs.");
        var baseBytes=b.AsSpan(0,cut).ToArray();foreach(var f in Fields)ClearSlot(baseBytes,f);
        uint[] baseRelos=relos.Where(s=>s<cut && !Fields.Any(f=>f.PointerSlot==s)).ToArray(),baseImports=imports.Where(s=>s<cut).ToArray();
        foreach(uint slot in baseRelos)if(Word(b,(int)slot)>=cut)throw new InvalidDataException("Base graph crosses remaining owner boundary.");
        var basis=new Chunk{InstanceBuffer=baseBytes,RelocationBuffer=Pack(baseRelos),ImportsBuffer=Pack(baseImports)};
        var old=Ep1GameObjectDestructionProjection.Compose(basis,schemas);int oldPrefix=old.InstanceBuffer.Length-cut,insert=0;
        for(;insert<oldPrefix;insert+=12)if(Word(old.InstanceBuffer,insert)==0 && Word(old.InstanceBuffer,insert+4)==0 && Word(old.InstanceBuffer,insert+8)==0)break;
        if(insert>=oldPrefix)throw new InvalidDataException("Selected root sentinel missing.");
        var pieces=new List<(Chunk Tokens,int Prefix,int Start)>();
        for(int i=0;i<Fields.Count;i++)
        {
            // Reborn: retain the root field declaration with a null target when its optional resource owner is absent.
            int start=starts[i];var field=Fields[i];if(start==0&&!field.InlineList){pieces.Add((new Chunk{InstanceBuffer=Array.Empty<byte>()},0,0));continue;}
            int end=starts.Skip(i+1).FirstOrDefault(s=>s!=0);if(end==0)end=b.Length;
            var leaf=field.InlineList?WrapInlineList(native,field,start,end):Extract(native,start,end);var tokens=field.InlineList?Ep1BehaviorMetadataTokenizer.TokenizeRootList(leaf,schemas,field.Name):Ep1BehaviorMetadataTokenizer.TokenizeRootRecord(leaf,schemas,field.Name);pieces.Add((tokens,tokens.InstanceBuffer.Length-leaf.InstanceBuffer.Length,start));
        }
        int extra=Fields.Count*12,prefix=checked(oldPrefix+extra+pieces.Sum(p=>p.Prefix));var output=new Chunk{InstanceBuffer=new byte[checked(prefix+b.Length)]};
        old.InstanceBuffer.AsSpan(0,insert).CopyTo(output.InstanceBuffer);old.InstanceBuffer.AsSpan(insert,oldPrefix-insert).CopyTo(output.InstanceBuffer.AsSpan(insert+extra));
        // Reborn: existing metadata table origins move by inserted root rows; existing native targets instead move to the single enlarged native origin.
        for(int at=0;at<oldPrefix;at+=12){int destination=at<insert?at:at+extra;uint target=Word(old.InstanceBuffer,at+8);if(target!=0)Put(output.InstanceBuffer,destination+8,target<oldPrefix?target+(uint)(target>=insert?extra:0):checked(target-(uint)oldPrefix+(uint)prefix));}
        int next=oldPrefix+extra;
        for(int i=0;i<Fields.Count;i++)
        {
            // Reborn: GameDependency is the root field name, but its stock wire type is GameDependencyType.
            var field=Fields[i];var piece=pieces[i];int row=insert+i*12;Put(output.InstanceBuffer,row,FastHash.GetHashCode(field.Name));Put(output.InstanceBuffer,row+4,field.Wire);Put(output.InstanceBuffer,row+8,piece.Prefix==0?0u:(uint)next);
            piece.Tokens.InstanceBuffer.AsSpan(0,piece.Prefix).CopyTo(output.InstanceBuffer.AsSpan(next));for(int at=8;at<piece.Prefix;at+=12){uint target=Word(piece.Tokens.InstanceBuffer,at);if(target!=0)Put(output.InstanceBuffer,next+at,target<piece.Prefix?target+(uint)next:field.InlineList&&target==piece.Prefix?(uint)(prefix+field.Slot):checked(target-(uint)piece.Prefix+(uint)prefix+(uint)piece.Start-(field.InlineList?8u:0u)));}next+=piece.Prefix;
        }
        b.CopyTo(output.InstanceBuffer,prefix);foreach(uint slot in relos){uint target=Word(b,(int)slot);if(target>=b.Length)throw new InvalidDataException("Root owner relocation target bounds.");Put(output.InstanceBuffer,prefix+(int)slot,checked(target+(uint)prefix));}
        output.RelocationBuffer=Pack(relos.Select(s=>s+(uint)prefix));output.ImportsBuffer=Pack(imports.Select(s=>s+(uint)prefix));return output;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: a synthetic header exposes an actual inline list to the standalone metadata writer without adding native bytes to the final object. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk WrapInlineList(Chunk native,Field field,int start,int end)
    {
        if(!field.InlineList)throw new InvalidDataException("Inline list field required.");uint count=Word(native.InstanceBuffer,field.Slot);int stride=System.Runtime.InteropServices.Marshal.SizeOf(field.NativeType);
        if((count==0)!=(start==0)||count>native.InstanceBuffer.Length/stride)throw new InvalidDataException("Root inline list count/items presence differs.");
        var tail=count==0?new Chunk{InstanceBuffer=Array.Empty<byte>(),RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()}:Extract(native,start,end);
        if(count>0&&tail.InstanceBuffer.Length<count*stride)throw new InvalidDataException("Root inline list items escape their allocation.");
        var result=new Chunk{InstanceBuffer=new byte[8+tail.InstanceBuffer.Length]};Put(result.InstanceBuffer,0,count);Put(result.InstanceBuffer,4,count==0?0u:8u);tail.InstanceBuffer.CopyTo(result.InstanceBuffer,8);
        var relos=Slots(tail.RelocationBuffer);foreach(uint slot in relos)Put(result.InstanceBuffer,8+(int)slot,checked(Word(tail.InstanceBuffer,(int)slot)+8));result.RelocationBuffer=Pack((count==0?Array.Empty<uint>():new uint[]{4}).Concat(relos.Select(s=>s+8)));result.ImportsBuffer=Pack(Slots(tail.ImportsBuffer).Select(s=>s+8));return result;
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
