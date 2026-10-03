using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.XmlCompiler
{
    // Reborn: serialize checked EP1 wire envelopes without altering the legacy KW runtime model or registering codecs/processors.
    public static class Ra3Ep1AudioFileRuntimeSerializer
    {
        // Reborn: these uint slots describe serialized Win32 relative offsets, not live process pointers.
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct NativePrefix
        {
            public uint Base,SubtitleLength,SubtitlePointer,Samples,Rate,HeaderPointer,HeaderSize;
            public byte Channels;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: emit an independent 32-byte prefix, aligned inline subtitle/SNR, exact relocations and no imports. */
        //-------------------------------------------------------------------------------------------------
        public static AssetBuffer Serialize(TargetPlatform platform,string subtitle,int samples,int rate,byte channels,ReadOnlySpan<byte> streamedHeader)
        {
            if (platform != TargetPlatform.Win32) throw new NotSupportedException("EP1 AudioFile runtime serialization is Win32 only.");
            if (subtitle == null || subtitle.Length is < 1 or > 1024) throw new ArgumentException("Bounded nonempty subtitle required.",nameof(subtitle));
            foreach (char value in subtitle) if (value is < ' ' or > '~') throw new ArgumentException("Subtitle must be printable ASCII without NUL.",nameof(subtitle));
            if (samples is < 1 or > 0x3FFFFFFF || rate is < 400 or > 96000 || channels is not (1 or 2 or 4 or 6))
                throw new ArgumentOutOfRangeException(nameof(samples),"Unproven AudioFile scalar range.");
            if (streamedHeader.Length != 0 && (streamedHeader.Length != 8 || ((streamedHeader[1] >> 2)+1) != channels
                || BinaryPrimitives.ReadUInt16BigEndian(streamedHeader.Slice(2,2)) != rate
                || BinaryPrimitives.ReadUInt32BigEndian(streamedHeader.Slice(4)) != ((uint)samples | 0x40000000u)))
                throw new ArgumentException("Streamed sound header contradicts runtime scalars/play location.",nameof(streamedHeader));
            int headerOffset = 32+((subtitle.Length+1+3)&~3);
            byte[] bin = new byte[headerOffset+streamedHeader.Length];
            Put(bin,4,(uint)subtitle.Length); Put(bin,8,32); Put(bin,12,(uint)samples); Put(bin,16,(uint)rate); bin[28] = channels;
            Encoding.ASCII.GetBytes(subtitle).CopyTo(bin,32);
            byte[] relo = streamedHeader.Length == 0 ? new byte[8] : new byte[12]; Put(relo,0,8);
            if (streamedHeader.Length != 0)
            { Put(bin,20,(uint)headerOffset); Put(bin,24,8); streamedHeader.CopyTo(bin.AsSpan(headerOffset)); Put(relo,4,20); }
            Put(relo,relo.Length-4,uint.MaxValue);
            return new AssetBuffer { InstanceData = bin,RelocationData = relo,ImportsData = Array.Empty<byte>() };
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: write native little-endian words into owned buffers with checked managed slices. */
        //-------------------------------------------------------------------------------------------------
        private static void Put(byte[] bytes,int offset,uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(offset,4),value);
    }
}
