using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunk=Relo.Chunk;
namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: EquivalentTo owns source-sized inline weak-GameObject IDs at root slots 452/456, with no strong import occurrences.
public static class Ep1GameObjectEquivalentTo
{
    public const int CountSlot=452,ItemsSlot=456;
    public sealed record Layout(int Count,int Items);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the real four-byte item extent and the list's single relocation before metadata composition. */
    //-------------------------------------------------------------------------------------------------
    public static Layout Inspect(byte[] bytes,IReadOnlyCollection<uint> relos,bool requireRelocations=true,int root=0)
    {
        uint count=Word(bytes,root+CountSlot),items=Word(bytes,root+ItemsSlot);
        if(count==0){if(items!=0||requireRelocations&&relos.Contains((uint)(root+ItemsSlot)))throw new InvalidDataException("Empty EquivalentTo retains storage.");return new(0,0);}
        if(count>bytes.Length/4||items<root+600||items%4!=0||items>bytes.Length-count*4||requireRelocations&&!relos.Contains((uint)(root+ItemsSlot)))throw new InvalidDataException("EquivalentTo native list bounds/relocation differ.");return new((int)count,(int)items);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: hide only the header from older composers; weak item bytes remain in their original owned native allocation. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Detach(Chunk native,Layout layout)
    {
        if(layout.Count==0)return native;var bytes=native.InstanceBuffer.ToArray();Put(bytes,CountSlot,0);Put(bytes,ItemsSlot,0);
        return new Chunk{InstanceBuffer=bytes,RelocationBuffer=Pack(Ep1GameObjectBrowser.Slots(native.RelocationBuffer).Where(s=>s!=ItemsSlot)),ImportsBuffer=native.ImportsBuffer.ToArray()};
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore the exact source count and one rebased native pointer without altering weak hash values or adding imports. */
    //-------------------------------------------------------------------------------------------------
    public static void Restore(Chunk output,int prefix,Layout layout)
    {
        if(layout.Count==0)return;if(Word(output.InstanceBuffer,prefix+CountSlot)!=0||Word(output.InstanceBuffer,prefix+ItemsSlot)!=0)throw new InvalidDataException("EquivalentTo detached header was overwritten.");
        Put(output.InstanceBuffer,prefix+CountSlot,(uint)layout.Count);Put(output.InstanceBuffer,prefix+ItemsSlot,(uint)(prefix+layout.Items));
        var slots=Ep1GameObjectBrowser.Slots(output.RelocationBuffer).ToList();uint slot=(uint)(prefix+ItemsSlot);if(slots.Contains(slot))throw new InvalidDataException("EquivalentTo relocation is already owned.");int index=slots.FindIndex(s=>s>slot);if(index<0)slots.Add(slot);else slots.Insert(index,slot);output.RelocationBuffer=Pack(slots);
    }
    // Reborn: auxiliary arrays retain the engine sentinel and omit it for an empty list.
    private static byte[] Pack(IEnumerable<uint> source){var slots=source.ToArray();if(slots.Length==0)return Array.Empty<byte>();var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    // Reborn: all metadata/native reads are bounded to their captured or owned asset slice.
    private static uint Word(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("EquivalentTo word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    // Reborn: writes only target newly owned detached/composed buffers.
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
