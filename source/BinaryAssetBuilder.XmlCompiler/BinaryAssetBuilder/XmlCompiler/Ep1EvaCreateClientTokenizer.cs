using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: literal create-announcement metadata is independent of source normalization and is checked against original retail field tables.
public static class Ep1EvaCreateClientTokenizer
{
    internal const uint TypeId=0xD0EEFDDDu;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the exact client inheritance and eight stock-typed fields, preserving all strong import slots and the native tail. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native?.InstanceBuffer==null||native.InstanceBuffer.Length!=28||native.RelocationBuffer==null||native.RelocationBuffer.Length!=0||native.ImportsBuffer==null)throw new InvalidDataException("Complete 28-byte EP1 EVA-create client required.");
        var b=native.InstanceBuffer;
        if(W(0)!=TypeId||W(4)==0||b[24]>1||b[25]>1||b[26]>1||b[27]>1)throw new InvalidDataException("EVA-create client identity/flags differ.");
        var expected=new[]{8u,12u,16u}.Where(at=>W((int)at)!=0).ToArray();
        if(native.ImportsBuffer.Length!=(expected.Length==0?0:(expected.Length+1)*4))throw new InvalidDataException("EVA-create client import count differs.");
        var imports=expected.Length==0?Array.Empty<uint>():Enumerable.Range(0,expected.Length).Select(i=>BinaryPrimitives.ReadUInt32LittleEndian(native.ImportsBuffer.AsSpan(i*4,4))).ToArray();
        if(!imports.OrderBy(i=>i).SequenceEqual(expected)||imports.Distinct().Count()!=imports.Length||expected.Length!=0&&BinaryPrimitives.ReadUInt32LittleEndian(native.ImportsBuffer.AsSpan(expected.Length*4,4))!=uint.MaxValue)throw new InvalidDataException("EVA-create client import inventory differs.");
        string[] names={"AnnouncementEventEnemy","AnnouncementEventAlly","AnnouncementEventOwner","DelayBeforeAnnouncementLogicFrames","OnlyIfVisible","CountAsFirstSightedAnnouncement","UseObjectsPosition","CreateFakeRadarEvent"};
        // Reborn: the delay is a raw logic-frame uint, matching its captured retail metadata signature.
        uint[] tags={0xC4EC43B1u,0xC4EC43B1u,0xC4EC43B1u,0x763177A4u,0x77E3F4F7u,0x77E3F4F7u,0x77E3F4F7u,0x77E3F4F7u};int[] offsets={8,12,16,20,24,25,26,27};
        // Reborn: class row, own field table (rows + terminator), then the three inherited tables.
        int own=24,client=own+(names.Length+1)*12,behavior=client+24,module=behavior+24,prefix=module+36;
        var output=new Chunk{InstanceBuffer=new byte[prefix+28],RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=native.ImportsBuffer.ToArray()};b.CopyTo(output.InstanceBuffer,prefix);
        for(int i=0;i<imports.Length;i++)BinaryPrimitives.WriteUInt32LittleEndian(output.ImportsBuffer.AsSpan(i*4,4),imports[i]+(uint)prefix);
        Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("ClientUpdateModuleData"),(uint)client);
        for(int i=0;i<names.Length;i++)Row(own+i*12,FastHash.GetHashCode(names[i]),tags[i],(uint)(prefix+offsets[i]));
        Row(client,1,FastHash.GetHashCode("BehaviorModuleData"),(uint)behavior);Row(behavior,1,FastHash.GetHashCode("ModuleData"),(uint)module);Row(module,FastHash.GetHashCode("TypeId"),0x23E650B8u,(uint)prefix);Row(module+12,FastHash.GetHashCode("id"),0xBDF07965u,(uint)prefix+4);return output;
        // Reborn: all native reads remain inside the already bounded fixed record.
        uint W(int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
        // Reborn: generated rows target only this metadata prefix or its preserved native tail.
        void Row(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+8,4),target);}
    }
}
