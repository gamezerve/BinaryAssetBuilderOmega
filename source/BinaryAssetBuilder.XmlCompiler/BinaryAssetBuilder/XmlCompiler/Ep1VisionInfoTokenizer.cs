using System;
using System.Buffers.Binary;
using System.IO;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: generate ten direct primitive field tokens for a bounded EP1 vision record, independently matched against complete stock tables.
public static class Ep1VisionInfoTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject nonfinite reals, legacy/trailing bytes and any auxiliary ownership before emitting the 132-byte metadata prefix. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native?.InstanceBuffer==null || native.InstanceBuffer.Length!=40 || native.RelocationBuffer==null || native.ImportsBuffer==null || native.RelocationBuffer.Length!=0 || native.ImportsBuffer.Length!=0)throw new InvalidDataException("EP1 vision requires exactly forty native bytes and no auxiliary slots.");
        var names=new[]{"VisionRange","ShroudClearingRange","ShroudClearingHeight","VisionSide","VisionRear","VisionBonusTestRadius","VisionBonusTestSegments","VisionBonusPercentPerFoot","MaxVisionBonusPercent","MinVisionBonusPercent"};
        var output=new Chunk{InstanceBuffer=new byte[172],RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};native.InstanceBuffer.CopyTo(output.InstanceBuffer,132);
        for(int i=0;i<10;i++)
        {
            if(i!=6 && !float.IsFinite(BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(native.InstanceBuffer.AsSpan(i*4,4)))))throw new InvalidDataException("Nonfinite EP1 vision field.");
            BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(i*12,4),FastHash.GetHashCode(names[i]));
            BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(i*12+4,4),i==6?0x763177A4u:0x645864F3u);
            BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(i*12+8,4),(uint)(132+i*4));
        }
        return output;
    }
}
