using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;
namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: preserve the retail-qualified AI metadata encoder beneath the original WorldBuilder animal class and its ten own attributes.
public static class Ep1AnimalAIUpdateTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: project the common 76-byte AI base, then map its metadata back across the actual forty-byte animal extension. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        var b=native.InstanceBuffer;
        if(b==null||b.Length<116||b.Length>2*1048576||W(b,0)!=0xCD56659Bu||b[112]>1||b[113]!=0||b[114]!=0||b[115]!=0)throw new InvalidDataException("Reviewed EP1 animal AI native record required.");
        var relos=Slots(native.RelocationBuffer);var imports=Slots(native.ImportsBuffer);
        if(relos.Any(s=>s>=76&&s<116)||imports.Any(s=>s>=76&&s<116))throw new InvalidDataException("Animal scalar extension cannot own auxiliary slots.");
        var projected=new byte[b.Length-40];b.AsSpan(0,76).CopyTo(projected);b.AsSpan(116).CopyTo(projected.AsSpan(76));Put(projected,0,0x08DBC00Eu);
        foreach(uint slot in relos)
        {
            uint target=W(b,(int)slot);if(target<116||target>=b.Length)throw new InvalidDataException("Animal base pointer escapes its native allocations.");
            Put(projected,(int)Map(slot),target-40);
        }
        var baseNative=new Chunk{InstanceBuffer=projected,RelocationBuffer=Pack(relos.Select(Map)),ImportsBuffer=Pack(imports.Select(Map))};
        var inherited=Ep1AIUpdateTokenizer.Tokenize(baseNative);int basePrefix=inherited.InstanceBuffer.Length-projected.Length,prefix=144+basePrefix;
        var output=new byte[prefix+b.Length];inherited.InstanceBuffer.AsSpan(12,basePrefix-12).CopyTo(output.AsSpan(156));
        for(int at=156;at<prefix;at+=12){uint target=W(output,at+8);if(target!=0)Put(output,at+8,target<basePrefix?target+144:checked((uint)prefix+Unmap(target-(uint)basePrefix)));}
        b.CopyTo(output,prefix);foreach(uint slot in relos)Put(output,prefix+(int)slot,checked(W(b,(int)slot)+(uint)prefix));
        Row(0,2,0xCD56659Bu,12);Row(12,1,0x08DBC00Eu,156);
        var fields=new[]{("FleeRadius",76,0x23E650B8u),("FleeDistance",80,0x23E650B8u),("WanderPercentage",84,0x23E650B8u),("MinDistance",88,0x23E650B8u),("MaxDistance",92,0x23E650B8u),("Radius",96,0x23E650B8u),("FleeUpdateTimer",100,0x998669E8u),("WanderUpdateTimer",104,0x998669E8u),("AfraidOfCastles",112,0x77E3F4F7u),("InitialFleeBlindlyRadius",108,0x645864F3u)};
        for(int i=0;i<fields.Length;i++){var field=fields[i];Row(24+i*12,FastHash.GetHashCode(field.Item1),field.Item3,(uint)(prefix+field.Item2));}
        return new Chunk{InstanceBuffer=output,RelocationBuffer=Pack(relos.Select(s=>s+(uint)prefix)),ImportsBuffer=Pack(imports.Select(s=>s+(uint)prefix))};
        // Reborn: coordinates only cross the inserted scalar extension; original inherited allocations stay intact.
        static uint Map(uint offset)=>offset<76?offset:offset-40;
        static uint Unmap(uint offset)=>offset<76?offset:offset+40;
        // Reborn: class and field metadata target their independently proved native or inherited-table coordinates.
        void Row(int at,uint name,uint tag,uint target){Put(output,at,name);Put(output,at+4,tag);Put(output,at+8,target);}
    }
    // Reborn: bound all metadata/native reads to the qualified leaf buffer.
    private static uint W(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("Animal native word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    // Reborn: write one little-endian coordinate.
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
    // Reborn: reject duplicate sources, missing terminators and sentinel-only auxiliary buffers.
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8||b.Length%4!=0||W(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Animal auxiliary framing differs.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>W(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Duplicate animal auxiliary slots.");return slots;}
    // Reborn: preserve occurrence order and exactly one trailing sentinel for nonempty auxiliary tables.
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var bytes=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(bytes,i*4,slots[i]);Put(bytes,slots.Length*4,uint.MaxValue);return bytes;}
}
