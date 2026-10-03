using System.Buffers.Binary;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recover outer custom-audio block framing without reading or decoding compressed frame bodies.
internal static class AudioCustomDataProbe
{
    // Reborn: aggregate framing evidence, not a codec compatibility or playback result.
    internal sealed record Result(int Blocks,long Bytes,byte CodecTag);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check RAM/streamed headers, bounded block seeks, exact EOF and total declared samples. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Inspect(Stream stream,AudioFileRuntimeProbe.Header native,byte[] inline)
    {
        if (!stream.CanSeek || stream.Length is < 8 or > 268435456) throw new InvalidDataException("Custom audio size/seek bound exceeded.");
        bool streamed = native.HeaderSize != 0;
        byte[] sound = streamed ? inline : Read(stream,0,8);
        if (sound.Length != 8 || ((sound[1] >> 2)+1) != native.Channels
            || BinaryPrimitives.ReadUInt16BigEndian(sound.AsSpan(2,2)) != native.Rate
            || (BinaryPrimitives.ReadUInt32BigEndian(sound.AsSpan(4)) & 0x3FFFFFFFu) != native.Samples
            || (BinaryPrimitives.ReadUInt32BigEndian(sound.AsSpan(4)) & 0xC0000000u) != (streamed ? 0x40000000u : 0u))
            throw new InvalidDataException("Custom audio sound header contradicts native fields/play location.");
        long offset = streamed ? 0 : 8; ulong samples = 0; int blocks = 0; bool terminal = false;
        while (offset < stream.Length)
        {
            if (++blocks > 65536) throw new InvalidDataException("Custom audio block-count bound exceeded.");
            byte[] prefix = Read(stream,offset,8);
            uint word = BinaryPrimitives.ReadUInt32BigEndian(prefix);
            int length = (int)(word & 0xFFFFFFu); byte kind = (byte)(word >> 24);
            uint count = BinaryPrimitives.ReadUInt32BigEndian(prefix.AsSpan(4));
            if (kind is not (0 or 128) || length < 8 || offset > stream.Length-length || count == 0)
                throw new InvalidDataException($"Unproven/truncated custom audio block framing: offset={offset}, kind={kind:X2}, declared={length}, file={stream.Length}, samples={count}.");
            samples += count; offset += length; terminal = kind == 128;
            if (samples > native.Samples || (terminal && offset != stream.Length))
                throw new InvalidDataException("Custom audio sample overflow or premature terminal block.");
        }
        if (blocks == 0 || samples != native.Samples || (streamed && !terminal))
            throw new InvalidDataException("Custom audio total samples or streamed terminal marker differ.");
        return new Result(blocks,stream.Length,sound[0]);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only header bytes and fail before seeking outside the declared custom file. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Read(Stream stream,long offset,int count)
    {
        if (offset < 0 || offset > stream.Length-count) throw new InvalidDataException("Custom audio header is truncated.");
        byte[] bytes = new byte[count]; stream.Position = offset; stream.ReadExactly(bytes); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test frame boundaries and corruption independently of codec payload content or game installations. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        AudioFileRuntimeProbe.Header native = new(0,0,10,48000,32,8,1);
        byte[] inline = Convert.FromHexString("0600BB804000000A");
        byte[] block = Convert.FromHexString("8000000C0000000ADEADBEEF");
        using (MemoryStream stream = new(block))
            if (Inspect(stream,native,inline) != new Result(1,12,6)) throw new InvalidDataException("Synthetic streamed framing differs.");
        byte[] ram = Convert.FromHexString("0400BB800000000A0000000C0000000ADEADBEEF");
        using (MemoryStream stream = new(ram))
            if (Inspect(stream,native with { HeaderPointer = 0,HeaderSize = 0 },Array.Empty<byte>()) != new Result(1,20,4)) throw new InvalidDataException("Synthetic RAM framing differs.");
        byte[] two = Convert.FromHexString("0000000C00000004DEADBEEF8000000C00000006DEADBEEF");
        using (MemoryStream stream = new(two))
            if (Inspect(stream,native,inline).Blocks != 2) throw new InvalidDataException("Synthetic multi-block framing differs.");
        foreach (string hex in new[] { "", "8000000C0000000A", "800000070000000A", "0100000C0000000ADEADBEEF", "0000000C0000000ADEADBEEF",
            "8000000C0000000BDEADBEEF", "8000000C00000009DEADBEEF", "8000000C00000000DEADBEEF", "8000000C0000000ADEADBEEF00" })
            Reject(Convert.FromHexString(hex),native,inline);
        foreach (int slot in new[] { 1,2,4,7 })
        { byte[] bad = (byte[])inline.Clone(); bad[slot] ^= slot == 1 ? (byte)4 : (byte)1; Reject(block,native,bad); }
        Reject(ram,native with { HeaderPointer = 0,HeaderSize = 0,Samples = 11 },Array.Empty<byte>());
        Console.WriteLine("Custom audio framing self-test: OK (RAM/streamed, two blocks, truncation/unknown flags/sizes/totals/terminal/header rejection; no payload decoding)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require malformed custom framing to fail closed without codec calls. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(byte[] bytes,AudioFileRuntimeProbe.Header native,byte[] inline)
    {
        using MemoryStream stream = new(bytes);
        try { Inspect(stream,native,inline); } catch (InvalidDataException) { return; }
        throw new InvalidDataException("Malformed custom audio framing accepted.");
    }
}
