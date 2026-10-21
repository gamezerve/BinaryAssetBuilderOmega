using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: explicit bounded hardpoint-only token writer; legacy dispatch, turret/contained payloads and general GameObject publication remain closed.
public static class Ep1WeaponSetTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate the reviewed root/base/hardpoint/weapon graph from canonical native allocations, preserving strong dependency indices. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native==null || native.InstanceBuffer==null || native.InstanceBuffer.Length<44)throw new InvalidDataException("Bounded EP1 weapon set required.");
        var b=native.InstanceBuffer;uint h=Word(b,12);
        // Reborn: actual stride/buffer bounds replace the former hardpoint and fragment-size trial quotas.
        if(Word(b,0)!=0x431722A9 || h>(b.Length-44)/28 || Word(b,16)!=(h==0?0:44) || Enumerable.Range(0,6).Any(i=>Word(b,20+i*4)!=0))throw new InvalidDataException("Unsupported EP1 weapon set class/list family.");
        var relos=new List<uint>();var imports=new List<uint>();if(h!=0)relos.Add(16);
        var counts=new List<int>();int weaponsAt=44+(int)h*28,total=0;
        for(int i=0;i<h;i++)
        {
            int at=44+i*28;uint count=Word(b,at+16);
            if(Word(b,at)!=0 || Word(b,at+8)>1 || Word(b,at+12)>5 || count>(b.Length-weaponsAt)/80 || Word(b,at+20)!=(count==0?0:weaponsAt) || b[at+24]>1 || b[at+25]>1 || b[at+26]!=0 || b[at+27]!=0)throw new InvalidDataException("EP1 hardpoint layout differs.");
            if(count!=0)relos.Add((uint)(at+20));counts.Add((int)count);
            for(int j=0;j<count;j++)
            {
                int w=weaponsAt+j*80;if(w>b.Length-80 || Word(b,w)>5 || b[w+76]>1 || b[w+77]>1 || b[w+78]!=0 || b[w+79]!=0)throw new InvalidDataException("EP1 weapon row layout differs.");
                if(j!=0 && Word(b,w)<Word(b,w-80))throw new InvalidDataException("Unnormalized weapon ordering.");
                foreach(int offset in new[]{4,8})if(Word(b,w+offset)!=0)imports.Add((uint)(w+offset));
            }
            weaponsAt+=(int)count*80;total+=(int)count;
        }
        if(weaponsAt!=b.Length)throw new InvalidDataException("Unowned EP1 weapon bytes.");CheckSlots(native.RelocationBuffer,relos);CheckSlots(native.ImportsBuffer,imports);
        int prefix=252+(int)h*120+total*108;var result=new Chunk {InstanceBuffer=new byte[prefix+b.Length]};b.CopyTo(result.InstanceBuffer,prefix);
        Token(0,2,0x431722A9,12);Token(12,1,FastHash.GetHashCode("UpdateModuleData"),96);Field(24,"ActiveWhenDisabledBy",0x196570C9,8);
        Token(36,FastHash.GetHashCode("WeaponSlotHardpoint"),0xCE5853D7,204);
        Token(48,FastHash.GetHashCode("WeaponSlotTurret"),0x4F14A301,(uint)(prefix-36));Token(60,FastHash.GetHashCode("WeaponSlotHierarchicalTurret"),0xC4884051,(uint)(prefix-24));Token(72,FastHash.GetHashCode("WeaponSlotContained"),0xBA2D72E2,(uint)(prefix-12));
        Token(96,1,FastHash.GetHashCode("ContainModuleData"),120);Token(120,1,FastHash.GetHashCode("BehaviorModuleData"),144);Token(144,1,FastHash.GetHashCode("ModuleData"),168);Field(168,"TypeId",0x23E650B8,0);Field(180,"id",0xBDF07965,4);
        Count(204,12);Count(prefix-36,20);Count(prefix-24,28);Count(prefix-12,36);
        int table=216+(int)h*12,nativeWeapons=44+(int)h*28;
        for(int i=0;i<h;i++)
        {
            int hp=44+i*28,n=counts[i],list=table+96,childStart=list+12+n*12;
            Token(216+i*12,0xB151A0EB,0,(uint)table);
            Field(table,"TypeId",0x23E650B8,hp);Field(table+12,"ID",0x763177A4,hp+4);Field(table+24,"AllowInterleavedFiring",0x77E3F4F7,hp+24);
            Field(table+36,"InterleavedStyle",0xBE669829,hp+8);Field(table+48,"WeaponChoiceCriteria",0x1D110792,hp+12);Field(table+60,"AutoChooseWeapon",0x77E3F4F7,hp+25);Token(table+72,FastHash.GetHashCode("Weapon"),0x77994E4B,(uint)list);Count(list,hp+16);
            for(int j=0;j<n;j++)
            {
                int child=childStart+j*96,w=nativeWeapons+j*80;Token(list+12+j*12,0xB151A0EB,0,(uint)child);
                Field(child,"Ordering",0x1848C549,w);Field(child+12,"Template",0xD046DD22,w+4);Field(child+24,"Upgrade",0x4ABA1F29,w+8);
                Field(child+36,"ObjectStatus",0xA7BD811E,w+12);Field(child+48,"ForbiddenObjectStatus",0xA7BD811E,w+44);Field(child+60,"IsPlayerUpgradePermanent",0x77E3F4F7,w+76);Field(child+72,"ConquerMetricsAlwaysCount",0x77E3F4F7,w+77);
            }
            table=childStart+n*96;nativeWeapons+=n*80;
        }
        foreach(uint slot in relos)Put(result.InstanceBuffer,prefix+(int)slot,Word(b,(int)slot)+(uint)prefix);
        result.RelocationBuffer=Pack(relos.Select(s=>s+(uint)prefix));result.ImportsBuffer=Pack(imports.Select(s=>s+(uint)prefix));return result;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: inline native field targets are distinct from token-table targets. */
        //-------------------------------------------------------------------------------------------------
        void Field(int at,string name,uint type,int target)=>Token(at,FastHash.GetHashCode(name),type,(uint)(prefix+target));
        //-------------------------------------------------------------------------------------------------
        /** Reborn: list metadata names native counts; it is not native relocation storage. */
        //-------------------------------------------------------------------------------------------------
        void Count(int at,int target)=>Token(at,0x6C827073,0x5D448FD8,(uint)(prefix+target));
        //-------------------------------------------------------------------------------------------------
        /** Reborn: emit only prefix metadata rows in Win32 byte order. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at,uint name,uint type,uint target){Put(result.InstanceBuffer,at,name);Put(result.InstanceBuffer,at+4,type);Put(result.InstanceBuffer,at+8,target);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact auxiliary ownership rejects missing/duplicate/out-of-owner entries and malformed sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSlots(byte[] b,List<uint> expected){if(b==null || b.Length!=(expected.Count==0?0:(expected.Count+1)*4) || (expected.Count!=0 && (Word(b,b.Length-4)!=uint.MaxValue || !Enumerable.Range(0,expected.Count).Select(i=>Word(b,i*4)).OrderBy(s=>s).SequenceEqual(expected.OrderBy(s=>s)))))throw new InvalidDataException("Weapon set auxiliary ownership differs.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent streams remain empty and nonempty streams receive exactly one sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all native/token reads retain explicit bounds. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Weapon set bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pointer and metadata writes use little-endian Win32 representation. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
