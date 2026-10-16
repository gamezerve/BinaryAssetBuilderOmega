using System;
using System.Buffers.Binary;
using System.IO;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: emit the stock-reviewed group-audio client inheritance and primitive hash-key list; this does not compile audio assets or validate playback.
public static class Ep1GroupAudioClientTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: require one canonical 28-byte client record and 1..64 contiguous hash words with exactly the Key items relocation and no imports. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native?.InstanceBuffer==null || native.InstanceBuffer.Length<32 || native.InstanceBuffer.Length>284 || native.RelocationBuffer==null || native.ImportsBuffer==null || native.ImportsBuffer.Length!=0)throw new InvalidDataException("Bounded group-audio client native graph required.");
        var b=native.InstanceBuffer;uint count=Word(16);if(Word(0)!=0x9826A297 || Word(4)==0 || count<1 || count>64 || b.Length!=28+count*4 || Word(20)!=28 || b[26]!=0 || b[27]!=0)throw new InvalidDataException("Group-audio client identity/count/layout/padding differs.");
        if(native.RelocationBuffer.Length!=8 || BinaryPrimitives.ReadUInt32LittleEndian(native.RelocationBuffer)!=20 || BinaryPrimitives.ReadUInt32LittleEndian(native.RelocationBuffer.AsSpan(4,4))!=uint.MaxValue)throw new InvalidDataException("Group-audio client auxiliary ownership differs.");
        int prefix=checked(180+(int)count*12);var output=new Chunk{InstanceBuffer=new byte[prefix+b.Length],RelocationBuffer=new byte[8],ImportsBuffer=Array.Empty<byte>()};b.CopyTo(output.InstanceBuffer,prefix);
        Token(0,2,0x9826A297,12);Token(12,1,0x4CAD6EDC,84);
        Token(24,FastHash.GetHashCode("FramesBetweenUpdatesMin"),0x763177A4,(uint)(prefix+8));Token(36,FastHash.GetHashCode("FramesBetweenUpdatesVariation"),0x763177A4,(uint)(prefix+12));Token(48,FastHash.GetHashCode("UnitWeight"),0x43501092,(uint)(prefix+24));Token(60,FastHash.GetHashCode("Key"),0x58BEC727,168);
        Token(84,1,0xA50D729A,108);Token(108,1,0xD75D93A2,132);Token(132,FastHash.GetHashCode("TypeId"),0x23E650B8,(uint)prefix);Token(144,FastHash.GetHashCode("id"),0xBDF07965,(uint)(prefix+4));Token(168,0x6C827073,0x5D448FD8,(uint)(prefix+16));
        // Reborn: the observed primitive element marker targets the native hash word directly, unlike complex-list markers targeting another metadata table.
        for(int i=0;i<count;i++)Token(180+i*12,0xB151A0EB,0,(uint)(prefix+28+i*4));
        BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(prefix+20,4),(uint)(prefix+28));BinaryPrimitives.WriteUInt32LittleEndian(output.RelocationBuffer,(uint)(prefix+20));BinaryPrimitives.WriteUInt32LittleEndian(output.RelocationBuffer.AsSpan(4,4),uint.MaxValue);return output;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: native words remain bounded little-endian offsets and never become unsafe captured pointers. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Group-audio client word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: generated metadata targets keep their own origin and do not become native relocation slots. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+8,4),target);}
    }
}
