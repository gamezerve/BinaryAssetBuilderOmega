using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: root-value/string/import/container projection only; dependency closure and other indirect fields still prevent complete GameObject publication.
public static class Ep1GameObjectDestructionProjection
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compose reviewed root values/Behaviors and a generated destruction fragment while preserving the original native graph and reference indices. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Compose(Chunk native)
    {
        if (native == null || native.InstanceBuffer == null || native.InstanceBuffer.Length < 620 || native.InstanceBuffer.Length > 1048576)
            throw new InvalidDataException("GameObject destruction projection requires bounded native dispatch output.");
        byte[] b = native.InstanceBuffer;
        var relos = Slots(native.RelocationBuffer); var imports = Slots(native.ImportsBuffer);
        // Reborn: range partitioning must not hide auxiliary slots outside every native owner.
        if(relos.Any(s=>s>b.Length-4) || imports.Any(s=>s>b.Length-4))throw new InvalidDataException("Projection auxiliary slot escapes native buffer.");
        // Reborn: explicit root-string allocations may move the behavior list and module; decode and validate ownership before extracting the tail.
        var layout = Ep1GameObjectStringTokens.Inspect(b, relos, imports);
        // Reborn: selected behavior modules own ordered disjoint tails; explicit class dispatch has no legacy or unknown-module fallback.
        var fragments=new System.Collections.Generic.List<(Chunk Data,int Prefix,int Native)>();
        for(int i=0;i<layout.Modules.Count;i++)
        {
            int start=layout.Modules[i],end=i+1<layout.Modules.Count?layout.Modules[i+1]:layout.Body!=0?layout.Body:layout.Geometry!=0?layout.Geometry:b.Length;
            var module=Extract(start,end);
            var piece=Word(module.InstanceBuffer,0) switch {
                0x0D96A2F4u=>Ep1DestructionTokenizer.Tokenize(module),
                0x431722A9u=>Ep1WeaponSetTokenizer.Tokenize(module),
                0x7F72F242u=>Ep1SmcHelperTokenizer.Tokenize(module),
                _=>throw new InvalidDataException("Unreviewed behavior module in selected projection.")};
            fragments.Add((piece,piece.InstanceBuffer.Length-module.InstanceBuffer.Length,start));
        }
        var body = layout.Body == 0 ? null : Extract(layout.Body,layout.Geometry==0?b.Length:layout.Geometry);
        var bodyFragment = body == null ? null : Ep1ActiveBodyTokenizer.Tokenize(body);
        // Reborn: geometry is a non-polymorphic final owner with a separately validated generated prefix.
        var geometry=layout.Geometry==0?null:Extract(layout.Geometry,b.Length);
        var geometryFragment=geometry==null?null:Ep1GeometryTokenizer.Tokenize(geometry);
        // Reborn: root token count is descriptor-driven; nested fragment coordinates must never retain the old two-field prefix size.
        int armorFields=(layout.ArmorCount==0?0:1)+(layout.LocomotorCount==0?0:1)+(geometry==null?0:1)+layout.PointerRoots.Count;
        int container = checked((Ep1GameObjectRootValueTokens.AllFields.Count + (body == null ? 2 : 3) + armorFields) * 12), fragmentStart = checked(container + 12 + fragments.Count*12);
        int bodyStart=checked(fragmentStart+fragments.Sum(f=>f.Prefix));
        int bodyPrefix = body == null ? 0 : checked(324 + (int)Word(body.InstanceBuffer,64) * 60),armorStart=checked(bodyStart+bodyPrefix);
        int armorPrefix=layout.ArmorCount==0?0:checked(12+layout.ArmorCount*60),locomotorStart=checked(armorStart+armorPrefix);
        // Reborn: movement count/marker/member tables follow the other reviewed fragments without altering absent-list output.
        int locomotorPrefix=layout.LocomotorCount==0?0:checked(12+layout.LocomotorCount*60),geometryStart=checked(locomotorStart+locomotorPrefix);
        int geometryPrefix=geometryFragment==null?0:geometryFragment.InstanceBuffer.Length-geometry!.InstanceBuffer.Length,pointerStart=checked(geometryStart+geometryPrefix);
        // Reborn: pointed text/vector roots target native payload directly; the optional four-field Flammability table follows geometry metadata.
        int pointerPrefix=layout.PointerRoots.Any(f=>f.HasTable)?60:0,prefix=checked(pointerStart+pointerPrefix);
        var result = new Chunk { InstanceBuffer = new byte[checked(prefix + b.Length)] };
        b.CopyTo(result.InstanceBuffer, prefix);
        for (int i = 0; i < Ep1GameObjectRootValueTokens.AllFields.Count; i++)
        {
            var field = Ep1GameObjectRootValueTokens.AllFields[i];
            Token(i * 12, FastHash.GetHashCode(field.Name), field.Type, (uint)(prefix + field.Offset));
        }
        Token(Ep1GameObjectRootValueTokens.AllFields.Count * 12, FastHash.GetHashCode("Behaviors"), 0x24FCCAE2u, (uint)container);
        // Reborn: Body is an indirect concrete wrapper, not a direct token pointing at root pointer storage.
        if(body != null)Token((Ep1GameObjectRootValueTokens.AllFields.Count + 1) * 12,FastHash.GetHashCode("Body"),0xAFAD56F4u,(uint)bodyStart);
        // Reborn: ArmorSet is an inline native list with indirect count/marker/member metadata, not another pointer-based module wrapper.
        if(layout.ArmorCount!=0)Token((Ep1GameObjectRootValueTokens.AllFields.Count+1+(body==null?0:1))*12,FastHash.GetHashCode("ArmorSet"),0x5E5AF575u,(uint)armorStart);
        // Reborn: LocomotorSet metadata targets its native count, not the items pointer or a behavior wrapper.
        if(layout.LocomotorCount!=0)Token((Ep1GameObjectRootValueTokens.AllFields.Count+1+(body==null?0:1)+(layout.ArmorCount==0?0:1))*12,FastHash.GetHashCode("LocomotorSet"),0x6E6BF482u,(uint)locomotorStart);
        // Reborn: root geometry points to its generated own-field table, not native pointer storage or a concrete module wrapper.
        if(geometryFragment!=null)Token((Ep1GameObjectRootValueTokens.AllFields.Count+1+(body==null?0:1)+(layout.ArmorCount==0?0:1)+(layout.LocomotorCount==0?0:1))*12,FastHash.GetHashCode("Geometry"),FastHash.GetHashCode("Geometry"),(uint)geometryStart);
        // Reborn: absent optional root pointers emit no additional partial-projection fields, preserving older zero-pointer cases.
        int pointerRow=Ep1GameObjectRootValueTokens.AllFields.Count+1+(body==null?0:1)+(layout.ArmorCount==0?0:1)+(layout.LocomotorCount==0?0:1)+(geometryFragment==null?0:1);
        foreach(var field in layout.PointerRoots)Token(pointerRow++*12,FastHash.GetHashCode(field.Name),field.Type,(uint)(field.HasTable?pointerStart:prefix+field.Target));
        Token(container, 0x6C827073u, 0x5D448FD8u, (uint)(prefix + layout.List));
        int nextFragment=fragmentStart;
        for(int i=0;i<fragments.Count;i++)
        {
            var f=fragments[i];Token(container+12+i*12,0xB151A0EBu,0,(uint)nextFragment);
            Join(f.Data,f.Prefix,nextFragment,f.Native);nextFragment=checked(nextFragment+f.Prefix);
        }
        if(bodyFragment != null)Join(bodyFragment,bodyPrefix,bodyStart,layout.Body);
        if(layout.ArmorCount!=0)
        {
            Token(armorStart,0x6C827073u,0x5D448FD8u,(uint)(prefix+420));
            for(int i=0;i<layout.ArmorCount;i++)
            {
                int table=armorStart+12+layout.ArmorCount*12+i*48,member=prefix+layout.Armor+i*12;
                Token(armorStart+12+i*12,0xB151A0EBu,0,(uint)table);
                Token(table,FastHash.GetHashCode("Armor"),0x34E9127Bu,(uint)member);
                Token(table+12,FastHash.GetHashCode("DamageFX"),0x9A95EDFFu,(uint)(member+4));
                Token(table+24,FastHash.GetHashCode("Conditions"),0xED8BC8E9u,(uint)(member+8));
            }
        }
        // Reborn: three inline movement fields use independently recovered stock token identities and native offsets.
        if(layout.LocomotorCount!=0)
        {
            Token(locomotorStart,0x6C827073u,0x5D448FD8u,(uint)(prefix+428));
            for(int i=0;i<layout.LocomotorCount;i++)
            {
                int table=locomotorStart+12+layout.LocomotorCount*12+i*48,member=prefix+layout.Locomotor+i*12;
                Token(locomotorStart+12+i*12,0xB151A0EBu,0,(uint)table);
                Token(table,FastHash.GetHashCode("Speed"),0x645864F3u,(uint)member);
                Token(table+12,FastHash.GetHashCode("Condition"),0x845D12B2u,(uint)(member+4));
                Token(table+24,FastHash.GetHashCode("Locomotor"),0xFED20C6Cu,(uint)(member+8));
            }
        }
        if(geometryFragment!=null)Join(geometryFragment,geometryPrefix,geometryStart,layout.Geometry);
        // Reborn: Flammability is four native unsigned words, not a polymorphic class or a root pointer-token target.
        foreach(var field in layout.PointerRoots.Where(f=>f.HasTable))
        {
            var names=new[]{"Fuel","MaxBurnRate","Decay","Resistance"};for(int i=0;i<names.Length;i++)Token(pointerStart+i*12,FastHash.GetHashCode(names[i]),0x23E650B8u,(uint)(prefix+field.Target+i*4));
        }
        foreach (uint slot in relos)
        {
            if (slot > b.Length - 4 || Word(b, (int)slot) >= b.Length) throw new InvalidDataException("Root projection relocation bounds exceeded.");
            Put(result.InstanceBuffer, checked(prefix + (int)slot), checked(Word(b, (int)slot) + (uint)prefix));
        }
        result.RelocationBuffer = Pack(relos.Select(s => checked(s + (uint)prefix)));
        result.ImportsBuffer = Pack(imports.Select(s => checked(s + (uint)prefix)));
        return result;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: root projection metadata never enters native relocation/import streams. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at, uint name, uint type, uint target) { Put(result.InstanceBuffer, at, name); Put(result.InstanceBuffer, at + 4, type); Put(result.InstanceBuffer, at + 8, target); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject cross-owner pointers before local rebasing; strong dependency indices are never interpreted as addresses. */
        //-------------------------------------------------------------------------------------------------
        Chunk Extract(int start,int end)
        {
            if(start < 600 || end < start || end > b.Length)throw new InvalidDataException("Projection native owner bounds differ.");
            var chunk = new Chunk {InstanceBuffer=b.AsSpan(start,end-start).ToArray()};
            foreach(uint slot in relos.Where(s=>s>=start && s<end))
            {
                if(slot>end-4 || Word(b,(int)slot)<start || Word(b,(int)slot)>=end)throw new InvalidDataException("Projection pointer crosses native owners.");
                Put(chunk.InstanceBuffer,(int)slot-start,Word(b,(int)slot)-(uint)start);
            }
            chunk.RelocationBuffer=Pack(relos.Where(s=>s>=start && s<end).Select(s=>s-(uint)start));
            chunk.ImportsBuffer=Pack(imports.Where(s=>s>=start && s<end).Select(s=>s-(uint)start));return chunk;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: local fragment tables and local native targets have different origins in the composed global coordinate system. */
        //-------------------------------------------------------------------------------------------------
        void Join(Chunk piece,int metadataBytes,int tableStart,int nativeStart)
        {
            piece.InstanceBuffer.AsSpan(0,metadataBytes).CopyTo(result.InstanceBuffer.AsSpan(tableStart));
            for(int at=8;at<metadataBytes;at+=12)
            {
                uint target=Word(piece.InstanceBuffer,at);
                if(target!=0)Put(result.InstanceBuffer,tableStart+at,target<metadataBytes?checked(target+(uint)tableStart):checked(target-(uint)metadataBytes+(uint)prefix+(uint)nativeStart));
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary streams require an aligned, unique bounded slot list and one final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)
    {
        if (b == null || b.Length % 4 != 0) throw new InvalidDataException("Malformed projection auxiliary stream.");
        if (b.Length == 0) return Array.Empty<uint>();
        if (b.Length == 4 || Word(b, b.Length - 4) != uint.MaxValue) throw new InvalidDataException("Projection auxiliary sentinel differs.");
        var slots = Enumerable.Range(0, b.Length / 4 - 1).Select(i => Word(b, i * 4)).ToArray();
        if (slots.Any(s => s % 4 != 0 || s > 1048572) || slots.Distinct().Count() != slots.Length) throw new InvalidDataException("Projection slot alignment/uniqueness differs.");
        return slots;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve original slot order and absent streams when changing ownership coordinates. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(System.Collections.Generic.IEnumerable<uint> values)
    {
        var slots = values.ToArray(); if (slots.Length == 0) return Array.Empty<byte>();
        var b = new byte[(slots.Length + 1) * 4]; for (int i = 0; i < slots.Length; i++) Put(b, i * 4, slots[i]); Put(b, slots.Length * 4, uint.MaxValue); return b;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native input and generated metadata word reads stay within their respective buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Projection word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all generated root/container/module coordinates use little-endian Win32 words. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
