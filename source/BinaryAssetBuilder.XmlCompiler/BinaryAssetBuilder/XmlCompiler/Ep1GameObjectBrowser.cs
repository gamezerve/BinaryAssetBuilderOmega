using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: EP1 GameObject/@Browser is an inline root string list (count +44, items +48). Like stock (WorldBuilder VL_RussianPortBuilding),
// the marshaler allocates it first after the 600-byte root: an 8-byte record per item, then each NUL-terminated ASCII text padded to 4.
// Lower root/behavior composers predate populated Browser, so it is detached before them and restored afterwards in final coordinates.
public static class Ep1GameObjectBrowser
{
    public const int CountSlot=44,ItemsSlot=48,NativeRootSize=600,MaxItems=256,MaxText=4096;
    public sealed record Layout(int Count,int Items,int End,uint[] Relocations);
    public static readonly Layout Empty=new(0,0,0,Array.Empty<uint>());

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact canonical allocation: items at +600, texts contiguous and in item order, padding zero, every pointer relocated. */
    //-------------------------------------------------------------------------------------------------
    public static Layout Inspect(byte[] b,IReadOnlyCollection<uint> relos,bool requireRelocations=true,int root=0)
    {
        uint count=Word(b,root+CountSlot),items=Word(b,root+ItemsSlot);
        if(count==0){if(items!=0||requireRelocations&&relos.Contains((uint)(root+ItemsSlot)))throw new InvalidDataException("Empty Browser list retains storage.");return Empty;}
        if(count>MaxItems||items!=root+NativeRootSize||items>b.Length-count*8)throw new InvalidDataException("Browser list allocation differs (count="+count+", items="+items+").");
        var slots=new List<uint>{(uint)(root+ItemsSlot)};int next=checked((int)items+(int)count*8);
        for(int i=0;i<count;i++)
        {
            int record=(int)items+i*8;uint length=Word(b,record),pointer=Word(b,record+4);
            if(length==0||length>MaxText||pointer!=next||pointer>b.Length-length-1)throw new InvalidDataException("Browser item text allocation differs at item "+i+".");
            for(int at=(int)pointer;at<pointer+length;at++)if(b[at]<=32||b[at]>126)throw new InvalidDataException("Browser item text must be printable ASCII without spaces.");
            int allocation=Align((int)length+1);if(b[pointer+length]!=0||pointer>b.Length-allocation||b.AsSpan((int)pointer+(int)length+1,allocation-(int)length-1).ToArray().Any(x=>x!=0))throw new InvalidDataException("Browser item terminator/padding differs.");
            slots.Add((uint)(record+4));next=checked((int)pointer+allocation);
        }
        if(requireRelocations)foreach(uint slot in slots)if(!relos.Contains(slot))throw new InvalidDataException("Browser pointer relocation absent at +"+slot+".");
        return new((int)count,(int)items,next,slots.ToArray());
    }

    // Reborn: authored list tokens use the marshaler's whitespace split.
    public static string[] Tokens(string text)=>(text??string.Empty).Split((char[])null,StringSplitOptions.RemoveEmptyEntries);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: hide the populated list from lower composers; its bytes stay in place and are restored after composition. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Detach(Chunk native,Layout layout)
    {
        if(layout.Count==0)return native;var bytes=native.InstanceBuffer.ToArray();Put(bytes,CountSlot,0);Put(bytes,ItemsSlot,0);
        var removed=new HashSet<uint>(layout.Relocations);return new Chunk{InstanceBuffer=bytes,RelocationBuffer=Pack(Slots(native.RelocationBuffer).Where(s=>!removed.Contains(s))),ImportsBuffer=native.ImportsBuffer.ToArray()};
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore header words and item pointers in final (prefix-shifted) coordinates and re-add their relocations in slot order. */
    //-------------------------------------------------------------------------------------------------
    public static void Restore(Chunk composed,int prefix,Layout layout)
    {
        if(layout.Count==0)return;var b=composed.InstanceBuffer;
        if(Word(b,prefix+CountSlot)!=0||Word(b,prefix+ItemsSlot)!=0)throw new InvalidDataException("Detached Browser header was rewritten by a lower composer.");
        Put(b,prefix+CountSlot,(uint)layout.Count);Put(b,prefix+ItemsSlot,checked((uint)(prefix+layout.Items)));
        for(int i=0;i<layout.Count;i++){int slot=prefix+layout.Items+i*8+4;Put(b,slot,checked(Word(b,slot)+(uint)prefix));}
        var existing=Slots(composed.RelocationBuffer).ToList();
        foreach(uint slot in layout.Relocations.Select(s=>checked(s+(uint)prefix)))
        {
            if(existing.Contains(slot))throw new InvalidDataException("Browser relocation already owned.");
            int index=existing.FindIndex(s=>s>slot);if(index<0)existing.Add(slot);else existing.Insert(index,slot);
        }
        composed.RelocationBuffer=Pack(existing);
    }

    private static int Align(int value)=>(value+3)&~3;
    public static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Browser auxiliary framing differs.");return Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();}
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    private static uint Word(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("Browser word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    private static void Put(byte[] b,int at,uint value){if(at<0||at>b.Length-4)throw new InvalidDataException("Browser write bounds.");BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);}
}
