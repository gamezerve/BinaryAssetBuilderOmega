using System.Buffers.Binary;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compare complete independent stock AI prefixes and every emitted metadata/native/auxiliary word, not producer self-consistency alone.
internal static class Ep1AIUpdateTokenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: complete stock metadata targets normalize through independently located native/chooser and metadata origins. */
    //-------------------------------------------------------------------------------------------------
    internal static void Stock(Chunk native,byte[] stock,int wrapper,int original)
    {
        var output=Check(native);int prefix=Prefix(native),chooser=(int)Word(native.InstanceBuffer,68),stockChooser=(int)Word(stock,original+68);var expected=stock.AsSpan(wrapper,prefix).ToArray();
        // Reborn: retail vision item and its status-flag allocations are independent stock owners rebased onto the generated native coordinates.
        var owners=new List<(int Stock,int Native,int Size)>();if(stockChooser!=0 && Word(stock,stockChooser+20)!=0){int stockVision=(int)Word(stock,stockChooser+24),vision=(int)Word(native.InstanceBuffer,chooser+24);owners.Add((stockVision,vision,128));foreach(int at in new[]{104,108})if(Word(stock,stockVision+at)!=0)owners.Add(((int)Word(stock,stockVision+at),(int)Word(native.InstanceBuffer,vision+at),32));}
        for(int at=8;at<prefix;at+=12)
        {
            int target=(int)Word(expected,at);if(target==0)continue;uint rebased;
            if(target>=wrapper && target<wrapper+prefix)rebased=(uint)(target-wrapper);
            else if(target>=original && target<original+76)rebased=(uint)(prefix+target-original);
            else if(stockChooser!=0 && target>=stockChooser && target<stockChooser+40)rebased=(uint)(prefix+chooser+target-stockChooser);
            else if(owners.Any(o=>target>=o.Stock && target<o.Stock+o.Size)){var owner=owners.First(o=>target>=o.Stock && target<o.Stock+o.Size);rebased=(uint)(prefix+owner.Native+target-owner.Stock);}
            else throw new InvalidDataException("Independent stock AI metadata target crosses the selected fragment.");Put(expected,at,rebased);
        }
        if(!expected.AsSpan().SequenceEqual(output.InstanceBuffer.AsSpan(0,prefix)))throw new InvalidDataException("Complete independent stock AI prefix differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate once, then independently verify the result; detached controls reuse Verify without repairing metadata through another producer call. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Check(Chunk native){var output=Ep1AIUpdateTokenizer.Tokenize(native);Verify(output,native);return output;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native-only nonempty contact support must not masquerade as generated element-token support. */
    //-------------------------------------------------------------------------------------------------
    internal static void RefuseContacts(Chunk native)=>Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(native));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal stock field hashes, known metadata extents and independent base chains pin every prefix word before inverse native/auxiliary comparison. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output,Chunk native)
    {
        int prefix=Prefix(native),chooser=(int)Word(native.InstanceBuffer,68);if(output.InstanceBuffer.Length!=prefix+native.InstanceBuffer.Length)throw new InvalidDataException("AI token output extent differs.");var expected=new byte[prefix];Row(0,2,0x08DBC00E,12);Row(12,1,0xF1002505,228);
        var fields=new[]{(0xD4EF4947u,0x1A2933AAu,8),(0x6CC5B6BFu,0x8B0FB9DAu,16),(0x77A11801u,0x645864F3u,20),(0x0766B407u,0x77E3F4F7u,72),(0x027B58C4u,0x998669E8u,24),(0xE5984206u,0x998669E8u,28),(0x9BE286ABu,0x998669E8u,32),(0x294ACDB5u,0x645864F3u,36),(0xAC461227u,0x6D66AE18u,-1),(0x1D330B2Au,0x77E3F4F7u,73),(0xB9DF8CE7u,0x998669E8u,48),(0x00537EBEu,0x763177A4u,52),(0x99FB0294u,0x645864F3u,56),(0x7392FB34u,0xAC6E35E0u,60),(0xF64C9F9Bu,0x845D12B2u,64),(0x243BEF01u,0x243BEF01u,-2)};
        for(int i=0;i<fields.Length;i++){var f=fields[i];Row(24+i*12,f.Item1,f.Item2,f.Item3==-1?336u:f.Item3==-2?(chooser==0?0u:348u):(uint)(prefix+f.Item3));}
        Row(228,1,FastHash.GetHashCode("ContainModuleData"),252);Row(252,1,FastHash.GetHashCode("BehaviorModuleData"),276);Row(276,1,FastHash.GetHashCode("ModuleData"),300);Row(300,FastHash.GetHashCode("TypeId"),0x23E650B8,(uint)prefix);Row(312,FastHash.GetHashCode("id"),0xBDF07965,(uint)(prefix+4));Row(336,0x6C827073,0x5D448FD8,(uint)(prefix+40));
        if(chooser!=0)
        {
            Row(348,1,0xAA42EB9C,384);Row(360,0xBE06A9E5,0x1B85B469,(uint)(prefix+chooser+36));
            var child=new[]{(0xFA7B2809u,0x998669E8u,0),(0x75881E10u,0x998669E8u,4),(0xEE67F9FDu,0x998669E8u,8),(0x913BF59Au,0x645864F3u,12),(0xFEE97B77u,0x77E3F4F7u,28),(0x9EC5DD15u,0x77E3F4F7u,29),(0x4513F0A2u,0x344E0F38u,16),(0x57CF8D55u,0x77E3F4F7u,30),(0x87C92B38u,0x77E3F4F7u,31),(0x653CDF52u,0x77E3F4F7u,32),(0xE4022CC3u,0x77E3F4F7u,33),(0x799997EAu,0x2E11FC54u,-1)};
            for(int i=0;i<child.Length;i++){var f=child[i];Row(384+i*12,f.Item1,f.Item2,f.Item3<0?540u:(uint)(prefix+chooser+f.Item3));}Row(540,0x6C827073,0x5D448FD8,(uint)(prefix+chooser+20));
            // Reborn: one retail vision item: list item row, Range/Filter item table, inline filter table and empty thing lists, with literal stock hashes.
            if(Word(native.InstanceBuffer,chooser+20)!=0)
            {
                int vision=(int)Word(native.InstanceBuffer,chooser+24);uint Flags(int at)=>Word(native.InstanceBuffer,vision+at)==0?0u:(uint)prefix+Word(native.InstanceBuffer,vision+at);
                Row(552,0xB151A0EB,0,564);Row(564,0xCC37CEA6,0x645864F3,(uint)(prefix+vision+4));Row(576,0x8579AC26,0xE46EA3B2,600);
                var filter=new[]{(0x8CDC9898u,0x4D3B3F19u,(uint)(prefix+vision+8)),(0xC9C22AA2u,0x0027BB58u,(uint)(prefix+vision+12)),(0xA037DC14u,0x5C8981A9u,(uint)(prefix+vision+16)),(0x27AF1102u,0x9FFD43EBu,(uint)(prefix+vision+20)),(0x1AF86941u,0x9AFC9193u,(uint)(prefix+vision+24)),(0x87D26051u,0x9AFC9193u,(uint)(prefix+vision+64)),(0xF2FC1F81u,0xA7BD811Eu,Flags(104)),(0x5C5EB235u,0xA7BD811Eu,Flags(108)),(0xC8801D7Eu,0x9B19049Fu,732u),(0xD6A24C87u,0x9B19049Fu,744u)};
                for(int i=0;i<filter.Length;i++)Row(600+i*12,filter[i].Item1,filter[i].Item2,filter[i].Item3);Row(732,0x6C827073,0x5D448FD8,(uint)(prefix+vision+112));Row(744,0x6C827073,0x5D448FD8,(uint)(prefix+vision+120));
            }
        }
        if(!expected.AsSpan().SequenceEqual(output.InstanceBuffer.AsSpan(0,prefix)))throw new InvalidDataException("AI detached metadata prefix differs.");
        var recovered=output.InstanceBuffer.AsSpan(prefix).ToArray();var relocations=Slots(native.RelocationBuffer);var imports=Slots(native.ImportsBuffer);
        if(!Slots(output.RelocationBuffer).SequenceEqual(relocations.Select(s=>s+(uint)prefix)) || !Slots(output.ImportsBuffer).SequenceEqual(imports.Select(s=>s+(uint)prefix)))throw new InvalidDataException("AI rebased auxiliary slots differ.");foreach(uint slot in relocations){uint actual=Word(output.InstanceBuffer,prefix+(int)slot);if(actual!=(uint)prefix+Word(native.InstanceBuffer,(int)slot))throw new InvalidDataException("AI rebased native pointer differs.");Put(recovered,(int)slot,actual-(uint)prefix);}if(!recovered.SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("AI inverse native payload differs.");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: this independent expected table uses stock literal hashes and never writes into the producer output. */
        //-------------------------------------------------------------------------------------------------
        void Row(int at,uint name,uint type,uint target){Put(expected,at,name);Put(expected,at+4,type);Put(expected,at+8,target);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed identities/flags/enums/owners/auxiliary streams and every detached metadata word must refuse independently. */
    //-------------------------------------------------------------------------------------------------
    internal static void Controls(Chunk native)
    {
        int chooser=(int)Word(native.InstanceBuffer,68),length=native.InstanceBuffer.Length,nativeFailures=0;var changes=new[]{(0,0u),(4,0u),(16,32u),(60,17u),(64,19u),(64,0xFFFFFFFEu),(72,2u),(74,1u),(40,1u),(44,76u),(8,uint.MaxValue),(12,(uint)chooser),(68,1u),(68,(uint)length),(chooser+16,16u),(chooser+20,1u),(chooser+24,76u),(chooser+28,2u),(chooser+34,1u),(chooser+36,0u)};
        foreach(var change in changes){var bad=Clone(native);if(change.Item1 is 72 or 74 || change.Item1==chooser+28 || change.Item1==chooser+34)bad.InstanceBuffer[change.Item1]=(byte)change.Item2;else Put(bad.InstanceBuffer,change.Item1,change.Item2);Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(bad));nativeFailures++;}
        // Reborn: imported native buffers still require ASCII/non-NUL content and the exact terminator, regardless of source-side guards.
        int text=(int)Word(native.InstanceBuffer,12),textLength=(int)Word(native.InstanceBuffer,8);foreach(var mutation in new[]{(text,(byte)128),(text,(byte)0),(text+textLength,(byte)1)}){var bad=Clone(native);bad.InstanceBuffer[mutation.Item1]=mutation.Item2;Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(bad));nativeFailures++;}
        foreach(int offset in new[]{20,24,28,32,36,48,56,chooser,chooser+4,chooser+8,chooser+12})foreach(uint value in new[]{0x7FC00000u,0x7F800000u}){var bad=Clone(native);Put(bad.InstanceBuffer,offset,value);Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(bad));nativeFailures++;}
        foreach(uint slot in new[]{12u,68u}){var bad=Clone(native);bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Where(s=>s!=slot));Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(bad));nativeFailures++;}
        var extra=Clone(native);extra.RelocationBuffer=Pack(Slots(extra.RelocationBuffer).Append(4u));Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(extra));nativeFailures++;var duplicate=Clone(native);duplicate.RelocationBuffer=Pack(Slots(duplicate.RelocationBuffer).Append(12u));Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(duplicate));nativeFailures++;
        var missingImport=Clone(native);missingImport.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(missingImport));nativeFailures++;var extraImport=Clone(native);extraImport.ImportsBuffer=Pack(Slots(extraImport.ImportsBuffer).Append(4u));Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(extraImport));nativeFailures++;
        var sentinel=Clone(native);sentinel.RelocationBuffer=new byte[]{255,255,255,255};Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(sentinel));nativeFailures++;var tail=Clone(native);tail.InstanceBuffer=tail.InstanceBuffer.Concat(new byte[4]).ToArray();Refuse(()=>Ep1AIUpdateTokenizer.Tokenize(tail));nativeFailures++;
        var output=Check(native);int metadataFailures=0;for(int at=0;at<Prefix(native);at+=4){var bad=Clone(output);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^1u);Refuse(()=>Verify(bad,native));metadataFailures++;}
        var payload=Clone(output);payload.InstanceBuffer[Prefix(native)+72]^=1;Refuse(()=>Verify(payload,native));metadataFailures++;
        // Reborn: detached pointer/auxiliary output corruption cannot hide behind an otherwise matching generated metadata prefix.
        var pointer=Clone(output);Put(pointer.InstanceBuffer,Prefix(native)+12,0);Refuse(()=>Verify(pointer,native));metadataFailures++;var outputRelo=Clone(output);outputRelo.RelocationBuffer=Pack(Slots(outputRelo.RelocationBuffer).Skip(1));Refuse(()=>Verify(outputRelo,native));metadataFailures++;var outputImport=Clone(output);outputImport.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(outputImport,native));metadataFailures++;
        Console.WriteLine($"EP1 AI tokens: PASS (three complete independent stock prefixes, one with a retail vision item; 680 explicitly empty-contact source variants; {nativeFailures} native/{metadataFailures} detached metadata/payload refusals; nonempty contacts and multi-item vision lists not token-enabled, no shared GameObject publication).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent expected extents depend only on optional chooser presence and vision count, not on producer metadata offsets. */
    //-------------------------------------------------------------------------------------------------
    private static int Prefix(Chunk native){int chooser=(int)Word(native.InstanceBuffer,68);return chooser==0?348:Word(native.InstanceBuffer,chooser+20)==0?552:756;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: refusals cannot become a repaired or partially successful output. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed AI token fragment admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls never alter caller/native or stock buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: empty or one-sentinel framing is checked independently of production's auxiliary decoder. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("AI token auxiliary framing differs.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: owned mutation buffers retain standard sentinel framing, including deliberate duplicate controls. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native and metadata words are bounded little-endian offsets, not unsafe pointers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("AI token word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only private expected/mutation arrays receive word writes. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
