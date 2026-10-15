using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: six inline ASCII string headers and their variable-sized allocations are reviewed projection fields, not arbitrary Unicode/localization support.
public static class Ep1GameObjectStringTokens
{
    // Reborn: native string headers remain eight-byte length/pointer pairs even when absent or explicitly empty.
    public static IReadOnlyList<Ep1GameObjectScalarTokens.Field> Fields { get; } = Array.AsReadOnly(new Ep1GameObjectScalarTokens.Field[] {
        new("Description",52,0x1A2933AAu,8), new("DescriptionTransformed",60,0x1A2933AAu,8),
        new("TypeDescription",68,0x1A2933AAu,8), new("TypeDescriptionTransformed",76,0x1A2933AAu,8),
        new("EditorName",124,0x1A2933AAu,8), new("ExperienceScalarTable",284,0x1A2933AAu,8)
    });
    // Reborn: behavior allocation addresses are decoded from native pointers, never inferred from root string lengths or fixed at 600/608/612.
    public readonly record struct Layout(int List, int Items, int Module, int Body, int Armor, int ArmorCount, IReadOnlyList<int> Modules, int Locomotor, int LocomotorCount);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate dynamic behavior ownership, bounded ASCII string storage and exact root relocation membership before splitting the module graph. */
    //-------------------------------------------------------------------------------------------------
    public static Layout Inspect(byte[] b, uint[] relos, uint[] imports)
    {
        uint list = Word(476);
        if (list < 600 || list % 4 != 0 || list > b.Length - 8)throw new InvalidDataException("Projection requires a bounded behavior envelope after root allocations.");
        uint count=Word((int)list);
        if(count<1 || count>3 || list>b.Length-8-count*4 || Word((int)list+4)!=list+8)throw new InvalidDataException("Projection admits one to three canonical behavior pointers.");
        var modules=new int[count];var owners=new List<uint>{476,list+4};
        for(int i=0;i<count;i++)
        {
            uint slot=list+8+(uint)i*4,target=Word((int)slot);
            if(target%4!=0 || target>b.Length-8 || (i==0 ? target!=list+8+count*4 : target<modules[i-1]+8))throw new InvalidDataException("Behavior native owners alias or escape their ordered envelope.");
            modules[i]=(int)target;owners.Add(slot);
        }
        int module=modules[0];
        // Reborn: the explicit EP1 body path owns a separate tail after destruction; no alias into the root/list/module is admissible.
        uint body = Word(488);
        if(body != 0)
        {
            if(body % 4 != 0 || body < modules[^1] + 8 || body > b.Length - 76)throw new InvalidDataException("Body tail ownership/bounds differ.");
            owners.Add(488);
        }
        var ranges = new List<(uint Start, uint End)>();
        // Reborn: inline ArmorSet count/items own a disjoint pre-behavior allocation; absent lists cannot retain a dangling pointer.
        uint armorCount=Word(420),armor=Word(424);
        if(armorCount>32 || (armorCount==0 ? armor!=0 : armor<600 || armor%4!=0 || armor>list || armorCount*12>list-armor))
            throw new InvalidDataException("ArmorSet count/allocation ownership differs.");
        if(armorCount!=0)owners.Add(424);
        // Reborn: movement records own a separate bounded pre-behavior allocation and cannot alias armor or root strings.
        uint locomotorCount=Word(428),locomotor=Word(432);
        if(locomotorCount>32 || (locomotorCount==0 ? locomotor!=0 : locomotor<600 || locomotor%4!=0 || locomotor>list || locomotorCount*12>list-locomotor))
            throw new InvalidDataException("LocomotorSet count/allocation ownership differs.");
        if(locomotorCount!=0)
        {
            owners.Add(432);
            if(armorCount!=0 && locomotor<armor+armorCount*12 && locomotor+locomotorCount*12>armor)throw new InvalidDataException("LocomotorSet aliases ArmorSet storage.");
        }
        foreach (var field in Fields)
        {
            uint length = Word(field.Offset), pointer = Word(field.Offset + 4);
            if (pointer == 0)
            { if (length != 0) throw new InvalidDataException("Absent root string has nonzero length: " + field.Name); continue; }
            if (pointer < 600 || pointer >= list || length > list - pointer - 1 || b[(int)(pointer + length)] != 0)
                throw new InvalidDataException("Root string allocation bounds/terminator differ: " + field.Name);
            uint end = pointer + length + 1;
            // Reborn: even identical byte spans cannot alias an ArmorSet record as a string allocation.
            if(armorCount!=0 && pointer<armor+armorCount*12 && end>armor)throw new InvalidDataException("Root string aliases ArmorSet storage.");
            // Reborn: even an identical string span is not a movement record owner.
            if(locomotorCount!=0 && pointer<locomotor+locomotorCount*12 && end>locomotor)throw new InvalidDataException("Root string aliases LocomotorSet storage.");
            if (ranges.Any(r => pointer < r.End && end > r.Start && (pointer != r.Start || end != r.End)))
                throw new InvalidDataException("Root string allocations partially overlap.");
            for (uint at = pointer; at < pointer + length; at++)
                if (b[(int)at] == 0 || b[(int)at] > 127) throw new InvalidDataException("Projection root strings currently require non-NUL ASCII content.");
            ranges.Add((pointer,end)); owners.Add((uint)(field.Offset + 4));
        }
        // Reborn: present root references require exactly one import slot each; zero is absence and no unreviewed root slot is admitted.
        var rootImports = Ep1GameObjectStrongTokens.Fields.Where(f => Word(f.Offset) != 0).Select(f => (uint)f.Offset).ToList();
        // Reborn: armor and optional DamageFX fields share the same occurrence table, but no other slot in the twelve-byte records is an import.
        for(int i=0;i<armorCount;i++)
        {
            int member=(int)armor+i*12;
            if((Word(member+8)>>23)!=0)throw new InvalidDataException("Unsupported ArmorSet condition bits.");
            foreach(int offset in new[]{0,4})if(Word(member+offset)!=0)rootImports.Add((uint)(member+offset));
        }
        // Reborn: finite speed and scoped engine conditions are literal data; only the final strong-reference word is an import.
        for(int i=0;i<locomotorCount;i++)
        {
            int member=(int)locomotor+i*12,condition=unchecked((int)Word(member+4));
            if(float.IsNaN(BitConverter.Int32BitsToSingle(unchecked((int)Word(member)))) || float.IsInfinity(BitConverter.Int32BitsToSingle(unchecked((int)Word(member)))) || condition < -1 || condition > 18)
                throw new InvalidDataException("Unsupported LocomotorSet speed/condition.");
            if(Word(member+8)!=0)rootImports.Add((uint)(member+8));
        }
        if (!relos.Where(s => s < module).OrderBy(s => s).SequenceEqual(owners.OrderBy(s => s)) || !imports.Where(s => s < module).OrderBy(s => s).SequenceEqual(rootImports.OrderBy(s => s)))
            throw new InvalidDataException("Unsupported root allocation/import ownership in string projection.");
        return new((int)list,(int)list + 8,module,(int)body,(int)armor,(int)armorCount,Array.AsReadOnly(modules),(int)locomotor,(int)locomotorCount);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: all dynamic headers and pointers stay inside the supplied native chunk. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("String projection bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4)); }
    }
}
