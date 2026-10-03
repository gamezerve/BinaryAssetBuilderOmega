using System.Buffers.Binary;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: inspect the recovered Win32 runtime envelope without invoking codecs or admitting AudioFile compilation.
internal static class AudioFileRuntimeProbe
{
    // Reborn: offsets describe the reference compiler's inline subtitle string, not the legacy pointer-to-string model.
    internal sealed record Header(uint SubtitleLength, uint SubtitlePointer, uint Samples, uint Rate, uint HeaderPointer, uint HeaderSize, byte Channels);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode only a fixed prefix and validate bounded native ranges; codec bytes remain opaque. */
    //-------------------------------------------------------------------------------------------------
    internal static Header Parse(ReadOnlySpan<byte> bytes, int chunkSize)
    {
        if (bytes.Length < 32 || chunkSize < 32) throw new InvalidDataException("AudioFile runtime prefix is truncated.");
        Header header = new(Word(bytes,4),Word(bytes,8),Word(bytes,12),Word(bytes,16),Word(bytes,20),Word(bytes,24),bytes[28]);
        if (Word(bytes,0) != 0 || header.Channels is not (1 or 2 or 4 or 6) || header.Rate is < 400 or > 96000
            || header.Samples == 0 || bytes[29] != 0 || bytes[30] != 0 || bytes[31] != 0)
            throw new InvalidDataException("Unproven AudioFile runtime scalar/padding values.");
        CheckRange(header.SubtitlePointer,header.SubtitleLength,chunkSize,true);
        CheckRange(header.HeaderPointer,header.HeaderSize,chunkSize,false);
        if (header.SubtitlePointer != 0 && header.HeaderPointer != 0
            && (ulong)header.SubtitlePointer < (ulong)header.HeaderPointer+header.HeaderSize
            && (ulong)header.HeaderPointer < (ulong)header.SubtitlePointer+header.SubtitleLength+1)
            throw new InvalidDataException("AudioFile subtitle and codec header overlap.");
        return header;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read native little-endian words without capturing a ref-like buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(ReadOnlySpan<byte> bytes,int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset,4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin the actual debris prefix and reject legacy, dangling and overflowing header interpretations. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        byte[] prefix = Convert.FromHexString("000000002B0000002000000080BB000080BB0000000000000000000001000000");
        Header header = Parse(prefix,76);
        if (header != new Header(43,32,48000,48000,0,0,1)) throw new InvalidDataException("Stock AudioFile prefix decode differs.");
        byte[] streamed = (byte[])prefix.Clone(); BinaryPrimitives.WriteUInt32LittleEndian(streamed.AsSpan(20),76); BinaryPrimitives.WriteUInt32LittleEndian(streamed.AsSpan(24),8);
        if (Parse(streamed,84).HeaderSize != 8) throw new InvalidDataException("Synthetic inline envelope decode differs.");
        foreach (int length in new[] { 0,28,31 }) Reject(prefix.AsSpan(0,length).ToArray(),76);
        Reject(prefix,75); Reject(streamed,83);
        foreach ((int slot,uint value) in new[] { (0,1u),(4,uint.MaxValue),(8,0u),(8,28u),(12,0u),(16,0u),(16,96001u),(20,76u),(24,8u),(28,3u),(28,0x10001u) })
        { byte[] bad = (byte[])prefix.Clone(); BinaryPrimitives.WriteUInt32LittleEndian(bad.AsSpan(slot),value); Reject(bad,76); }
        byte[] overlap = (byte[])streamed.Clone(); BinaryPrimitives.WriteUInt32LittleEndian(overlap.AsSpan(20),32); Reject(overlap,84);
        // Reborn: a second independent stock golden pins the actual streamed command-voice envelope and eight-byte embedded header.
        byte[] voice = Convert.FromHexString("000000002D0000002000000096B3050080BB00005000000008000000020000004449414C4F474556454E543A616576615F6B656570636F6D6D616E646469726563746976655375625469746C650000000604BB804005B396");
        Header voiceHeader = Parse(voice,voice.Length);
        if (voiceHeader != new Header(45,32,373654,48000,80,8,2)) throw new InvalidDataException("Stock streamed voice envelope differs.");
        ValidateInlineHeader(voice.AsSpan(80,8),voiceHeader);
        foreach (int slot in new[] { 1,2,4,7 })
        {
            byte[] bad = voice.AsSpan(80,8).ToArray(); bad[slot] ^= slot == 1 ? (byte)4 : (byte)1; bool rejected = false;
            try { ValidateInlineHeader(bad,voiceHeader); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Contradictory embedded sound header accepted.");
        }
        // Reborn: pin stock stream magic/checksum/length validation independently of real game files.
        byte[] streamPrefix = Convert.FromHexString("0000BBBAA2E35C39");
        ValidateEnvelope(streamPrefix,84,0xBABB0000u,0x395CE3A2u,76);
        foreach ((ulong length,uint magic,uint checksum,ulong total) in new[] { (83UL,0xBABB0000u,0x395CE3A2u,76UL),(84UL,0u,0x395CE3A2u,76UL),(84UL,0xBABB0000u,0u,76UL),(84UL,0xBABB0000u,0x395CE3A2u,75UL) })
        {
            bool rejected = false;
            try { ValidateEnvelope(streamPrefix,length,magic,checksum,total); } catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Wrong stock stream magic/checksum/length accepted.");
        }
        Console.WriteLine("AudioFile runtime self-test: OK (stock 32-byte prefix, synthetic inline header, legacy/truncation/range/overflow/overlap/scalar rejection; no codecs)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require malformed native envelopes to fail at the bounded header reader. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(byte[] prefix,int size)
    {
        try { Parse(prefix,size); } catch (InvalidDataException) { return; }
        throw new InvalidDataException("Malformed AudioFile runtime envelope accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject dangling, overflowing or root-overlapping pointer/count pairs before seeking. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckRange(uint pointer,uint count,int size,bool terminated)
    {
        if (pointer == 0 && count == 0) return;
        if (pointer < 32 || count == 0 || (ulong)pointer+count+(terminated ? 1UL : 0UL) > (ulong)size)
            throw new InvalidDataException("AudioFile runtime pointer/count is outside its native chunk.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: audit every selected stock envelope using bounded reads and emit only representative opaque codec prefixes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifestPath,bool auditCustom = false,IReadOnlyDictionary<string,byte[]>? corrections = null)
    {
        if (new FileInfo(manifestPath).Length > 16*1024*1024) throw new InvalidDataException("Audio manifest exceeds audit bound.");
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(manifestPath));
        if (manifest.Assets.Count > 65536 || manifest.Header.Version != 7 || !manifest.Header.IsLinked || manifest.Header.AllTypesHash != 0x5454A8E9u
            || manifest.Header.ContainerPrefixSize != 4 || manifest.Validate().Count != 0)
            throw new InvalidDataException("Expected a validated linked prefixed EP1 manifest.");
        using FileStream bin = File.OpenRead(Path.ChangeExtension(manifestPath,".bin"));
        using FileStream relo = File.OpenRead(Path.ChangeExtension(manifestPath,".relo"));
        using FileStream imp = File.OpenRead(Path.ChangeExtension(manifestPath,".imp"));
        // Reborn: stock streams carry BABB/BABE/BAB1 magic words, unlike owned diagnostic streams with a zero prefix.
        ValidateStream(bin,0xBABB0000u,manifest.Header.StreamChecksum,manifest.Header.TotalInstanceDataSize);
        ValidateStream(relo,0xBABE0000u,manifest.Header.StreamChecksum,(ulong)manifest.Assets.Sum(asset => (long)asset.RelocationDataSize));
        ValidateStream(imp,0xBAB10000u,manifest.Header.StreamChecksum,(ulong)manifest.Assets.Sum(asset => (long)asset.ImportsDataSize));
        long offset = 8, relocationOffset = 8; int count = 0, inline = 0, shown = 0; bool shownInline = false;
        Dictionary<string,int> codecs = new();
        // Reborn: optional full identity-mapped custom framing audit stays independent of native-only envelope admission.
        long customBytes = 0,customBlocks = 0; int customFailures = 0,correctionsUsed = 0; Dictionary<byte,int> customTags = new();
        if (corrections != null && (!auditCustom || corrections.Count != 4)) throw new InvalidDataException("Expected four verified archive corrections for custom audit only.");
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (asset.TypeId == 0x166B084Du)
            {
                if (asset.TypeName != "AudioFile" || asset.TypeHash != 0x53C81E47u || asset.Tokenized != 0 || asset.References.Count != 0 || asset.ImportsDataSize != 0)
                    throw new InvalidDataException("AudioFile fingerprint/reference mismatch.");
                Header header = Parse(Read(bin,offset,32),asset.InstanceDataSize);
                if (header.SubtitleLength > 1024 || header.HeaderSize > 4096) throw new InvalidDataException("AudioFile optional data exceeds audit bound.");
                byte[] subtitle = header.SubtitlePointer == 0 ? Array.Empty<byte>() : Read(bin,offset+header.SubtitlePointer,checked((int)header.SubtitleLength+1));
                if (subtitle.Length != 0 && (subtitle[^1] != 0 || subtitle.AsSpan(0,subtitle.Length-1).Contains((byte)0)))
                    throw new InvalidDataException("AudioFile subtitle length/terminator mismatch.");
                if (header.HeaderSize != 0) ValidateInlineHeader(Read(bin,offset+header.HeaderPointer,checked((int)header.HeaderSize)),header);
                uint[] expected = new[] { header.SubtitlePointer == 0 ? 0u : 8u,header.HeaderPointer == 0 ? 0u : 20u }.Where(value => value != 0).Append(uint.MaxValue).ToArray();
                if (asset.RelocationDataSize != expected.Length*4) throw new InvalidDataException("AudioFile relocation chunk size differs.");
                byte[] relocation = Read(relo,relocationOffset,expected.Length*4);
                if (asset.RelocationDataSize != relocation.Length || !expected.Select((value,index) => BinaryPrimitives.ReadUInt32LittleEndian(relocation.AsSpan(index*4,4)) == value).All(value => value))
                    throw new InvalidDataException("AudioFile relocation slots differ from recovered layout.");
                count++; if (header.HeaderSize != 0) inline++;
                if (auditCustom)
                {
                    string customPath = Path.Combine(Path.GetDirectoryName(manifestPath)!,Path.GetFileNameWithoutExtension(manifestPath),"cdata",
                        $"{asset.TypeId:x8}.{asset.TypeHash:x8}.{asset.InstanceId:x8}.{asset.InstanceHash:x8}.cdata");
                    // Reborn: retain strict per-file rejection but inventory all identities when the unpacked corpus is incomplete.
                    try
                    {
                        // Reborn: an explicit verified archive overlay affects only this read-only audit, never the unpacked files or compiler inputs.
                        bool corrected = corrections != null && corrections.TryGetValue(asset.Name,out _);
                        using Stream customStream = corrected ? new MemoryStream(corrections![asset.Name],false) : File.OpenRead(customPath);
                        if (corrected) correctionsUsed++;
                        AudioCustomDataProbe.Result framing = AudioCustomDataProbe.Inspect(customStream,header,header.HeaderSize == 0 ? Array.Empty<byte>() : Read(bin,offset+header.HeaderPointer,checked((int)header.HeaderSize)));
                        customBytes = checked(customBytes+framing.Bytes); customBlocks = checked(customBlocks+framing.Blocks);
                        customTags[framing.CodecTag] = customTags.GetValueOrDefault(framing.CodecTag)+1;
                    }
                    catch (Exception error) when (error is IOException or InvalidDataException)
                    {
                        customFailures++;
                        if (customFailures <= 8) Console.WriteLine($"  custom REJECT {asset.Name}: {error.Message}");
                    }
                }
                if (shown < 3 || (!shownInline && header.HeaderSize != 0))
                {
                    string prefix = "<absent>";
                    if (header.HeaderSize != 0) { prefix = Convert.ToHexString(Read(bin,offset+header.HeaderPointer,Math.Min(16,checked((int)header.HeaderSize)))); shownInline = true; }
                    string cdata = Path.Combine(Path.GetDirectoryName(manifestPath)!,Path.GetFileNameWithoutExtension(manifestPath),"cdata",
                        $"{asset.TypeId:x8}.{asset.TypeHash:x8}.{asset.InstanceId:x8}.{asset.InstanceHash:x8}.cdata");
                    string custom = "<not present>";
                    if (File.Exists(cdata))
                    {
                        using FileStream customStream = File.OpenRead(cdata);
                        custom = Convert.ToHexString(Read(customStream,0,(int)Math.Min(16,customStream.Length)))+$" ({customStream.Length} bytes)";
                    }
                    Console.WriteLine($"  {asset.Name}: samples={header.Samples} rate={header.Rate} channels={header.Channels} subtitle={Encoding.ASCII.GetString(subtitle).TrimEnd('\0')} inline={prefix} custom={custom}");
                    shown++;
                }
                if (header.HeaderSize != 0)
                {
                    // Reborn: group only four signature bytes; variable sample-count bytes must not create an unbounded printed dump.
                    string key = $"size={header.HeaderSize} prefix="+Convert.ToHexString(Read(bin,offset+header.HeaderPointer,Math.Min(4,checked((int)header.HeaderSize))));
                    codecs[key] = codecs.GetValueOrDefault(key)+1;
                }
            }
            offset = checked(offset+asset.InstanceDataSize); relocationOffset = checked(relocationOffset+asset.RelocationDataSize);
        }
        if (count == 0) throw new InvalidDataException("No AudioFile envelopes found.");
        Console.WriteLine($"AudioFile runtime audit: OK; records={count}, inline-header={inline}, external-only={count-inline}; no encoding or production admission.");
        foreach (var item in codecs.OrderBy(item => item.Key).Take(16)) Console.WriteLine($"  opaque inline {item.Key}: {item.Value}");
        if (codecs.Count > 16) Console.WriteLine($"  ({codecs.Count-16} further signature groups omitted)");
        if (auditCustom)
        {
            if (corrections != null && correctionsUsed != 4) throw new InvalidDataException("Verified archive corrections did not match four unique native identities.");
            Console.WriteLine($"Custom audio framing audit: {(customFailures == 0 ? "OK" : "INCOMPLETE")}; files={count}, validated={count-customFailures}, rejected={customFailures}, validated-blocks={customBlocks}, validated-bytes={customBytes}; only headers read, payload skipped.");
            if (corrections != null) Console.WriteLine($"  Explicit original archive overlay used={correctionsUsed}; unpacked files unchanged, default local-only audit still rejects its four records.");
            foreach (var tag in customTags.OrderBy(item => item.Key)) Console.WriteLine($"  opaque codec tag {tag.Key:X2}: {tag.Value}");
            if (customFailures > 0) throw new InvalidDataException($"Custom audio corpus has {customFailures} rejected/missing records; no complete framing proof.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify linked stream prefix/checksum and exact aggregate length before any asset seeks. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateStream(FileStream stream,uint magic,uint checksum,ulong total)
    {
        byte[] bytes = Read(stream,0,8);
        ValidateEnvelope(bytes,(ulong)stream.Length,magic,checksum,total);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: cross-check observed eight-byte embedded header scalars without interpreting the codec tag or compressed frames. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateInlineHeader(ReadOnlySpan<byte> bytes,Header header)
    {
        if (bytes.Length != 8 || ((bytes[1] >> 2)+1) != header.Channels
            || BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(2,2)) != header.Rate
            || (BinaryPrimitives.ReadUInt32BigEndian(bytes.Slice(4)) & 0x3FFFFFFFu) != header.Samples
            || (BinaryPrimitives.ReadUInt32BigEndian(bytes.Slice(4)) & 0xC0000000u) != 0x40000000u)
            throw new InvalidDataException("Embedded sound header contradicts AudioFile runtime scalars or streamed flag.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check the stock signature contract independently of stream IO and reject aggregate overflow. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateEnvelope(ReadOnlySpan<byte> bytes,ulong length,uint magic,uint checksum,ulong total)
    {
        if (bytes.Length != 8 || total > ulong.MaxValue-8 || length != total+8 || BinaryPrimitives.ReadUInt32LittleEndian(bytes) != magic
            || BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(4)) != checksum) throw new InvalidDataException("AudioFile linked stream header/length mismatch.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: perform exact bounded seeks without loading the entire game stream. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Read(FileStream stream,long offset,int count)
    {
        if (count is < 0 or > 4096 || offset < 0 || offset > stream.Length-count) throw new InvalidDataException("AudioFile read lies outside stream bounds.");
        byte[] bytes = new byte[count]; stream.Position = offset; stream.ReadExactly(bytes); return bytes;
    }
}
