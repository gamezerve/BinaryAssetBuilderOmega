using System;
using System.Buffers.Binary;
using System.IO;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: exact eight-byte SMC helper and its five inherited marker tables; no arbitrary empty-class or legacy writer fallback.
public static class Ep1SmcHelperTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate a stock-corroborated empty helper inheritance fragment from the real native class/id words. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native==null || native.InstanceBuffer==null || native.InstanceBuffer.Length!=8 || BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer)!=0x7F72F242 || native.RelocationBuffer==null || native.RelocationBuffer.Length!=0 || native.ImportsBuffer==null || native.ImportsBuffer.Length!=0)throw new InvalidDataException("Exact dependency-free EP1 SMC helper required.");
        var output=new Chunk {InstanceBuffer=new byte[176],RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};native.InstanceBuffer.CopyTo(output.InstanceBuffer,168);
        Token(0,2,0x7F72F242,12);string[] bases={"ObjectHelperModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"};
        for(int i=0;i<bases.Length;i++)Token(12+i*24,1,FastHash.GetHashCode(bases[i]),(uint)(36+i*24));
        Token(132,FastHash.GetHashCode("TypeId"),0x23E650B8,168);Token(144,FastHash.GetHashCode("id"),0xBDF07965,172);return output;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: inherited metadata is not native relocation storage and contains no stock-copied bytes. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+8,4),target);}
    }
}
