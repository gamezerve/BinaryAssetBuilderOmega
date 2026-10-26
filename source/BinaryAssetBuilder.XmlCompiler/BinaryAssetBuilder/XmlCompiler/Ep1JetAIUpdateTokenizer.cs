using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;
namespace BinaryAssetBuilder.XmlCompiler;
// Reborn: preserve the independently reviewed AI metadata writer while adding the retail jet wrapper and nine own attributes.
public static class Ep1JetAIUpdateTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: project only the shared AI header/tail into its existing tokenizer, then map metadata targets back to the untouched 100-byte jet native record. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        var b=native.InstanceBuffer;if(b==null||b.Length<100||b.Length>2*1048576||W(b,0)!=0x4486CF48||b.AsSpan(96,4).ToArray().Any(v=>v>1))throw new InvalidDataException("Reviewed EP1 jet native record required.");
        var relos=Slots(native.RelocationBuffer);var imports=Slots(native.ImportsBuffer);
        if(relos.Any(s=>s>=76&&s<100)||imports.Any(s=>s>=76&&s<100&&s!=88&&s!=92))throw new InvalidDataException("Jet own auxiliary slots differ.");
        foreach(uint slot in new uint[]{88,92})if((W(b,(int)slot)!=0)!=imports.Contains(slot))throw new InvalidDataException("Jet optional import slot differs.");
        var projected=new byte[b.Length-24];b.AsSpan(0,76).CopyTo(projected);b.AsSpan(100).CopyTo(projected.AsSpan(76));Put(projected,0,0x08DBC00E);
        foreach(uint slot in relos){uint target=W(b,(int)slot);if(target<100||target>=b.Length)throw new InvalidDataException("Jet base pointer escapes its native allocations.");Put(projected,(int)Map(slot),target-24);}
        var baseNative=new Chunk{InstanceBuffer=projected,RelocationBuffer=Pack(relos.Select(Map)),ImportsBuffer=Pack(imports.Where(s=>s!=88&&s!=92).Select(Map))};
        var baseTokens=Ep1AIUpdateTokenizer.Tokenize(baseNative);int basePrefix=baseTokens.InstanceBuffer.Length-projected.Length,prefix=132+basePrefix;var output=new byte[prefix+b.Length];
        // Reborn: inherited AI metadata starts at its own table; the standalone AI class wrapper is not repeated inside a jet.
        baseTokens.InstanceBuffer.AsSpan(12,basePrefix-12).CopyTo(output.AsSpan(144));
        for(int at=144;at<prefix;at+=12){uint target=W(output,at+8);if(target!=0)Put(output,at+8,target<basePrefix?target+132:checked((uint)prefix+Unmap(target-(uint)basePrefix)));}
        b.CopyTo(output,prefix);foreach(uint slot in relos)Put(output,prefix+(int)slot,checked(W(b,(int)slot)+(uint)prefix));
        Row(0,2,0x4486CF48,12);Row(12,1,0x08DBC00E,144);
        var fields=new[]{("OutOfAmmoDamagePerSecond",76,0x645864F3u),("OutOfAmmoDamageDelay",80,0x998669E8u),("NeedsRunway",96,0x77E3F4F7u),("ReturnToBaseWhenVictimDead",97,0x77E3F4F7u),("KeepsParkingSpaceWhenAirborne",98,0x77E3F4F7u),("ParkingOffset",84,0x645864F3u),("CirclesForAttack",99,0x77E3F4F7u),("UseForOutOfAmmoCheck",88,0xD046DD22u),("OutOfAmmoEvaEvent",92,0xC4EC43B1u)};
        for(int i=0;i<fields.Length;i++){var f=fields[i];Row(24+i*12,FastHash.GetHashCode(f.Item1),f.Item3,(uint)(prefix+f.Item2));}
        return new Chunk{InstanceBuffer=output,RelocationBuffer=Pack(relos.Select(s=>s+(uint)prefix)),ImportsBuffer=Pack(imports.Select(s=>s+(uint)prefix))};
        // Reborn: coordinate translation changes only the inserted 24-byte jet extension, never base header offsets.
        static uint Map(uint offset)=>offset<76?offset:offset-24;
        static uint Unmap(uint offset)=>offset<76?offset:offset+24;
        void Row(int at,uint name,uint tag,uint target){Put(output,at,name);Put(output,at+4,tag);Put(output,at+8,target);}
    }
    // Reborn: bounded reads and exact sentinel framing reject malformed standalone auxiliary arrays.
    private static uint W(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("Jet native word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    // Reborn: write one little-endian metadata/native coordinate.
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
    // Reborn: require one terminal sentinel and distinct input slots.
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8||b.Length%4!=0||W(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Jet auxiliary framing differs.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>W(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Duplicate jet auxiliary slots.");return slots;}
    // Reborn: preserve auxiliary occurrence order during prefix rebasing.
    private static byte[] Pack(System.Collections.Generic.IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var bytes=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(bytes,i*4,slots[i]);Put(bytes,slots.Length*4,uint.MaxValue);return bytes;}
}
