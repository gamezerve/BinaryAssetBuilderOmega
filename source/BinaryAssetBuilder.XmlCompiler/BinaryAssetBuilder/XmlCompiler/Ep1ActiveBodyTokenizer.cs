using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: bounded standalone EP1 body fragment writer; no registry, general body dispatch or GameObject publication is enabled here.
public static class Ep1ActiveBodyTokenizer
{
    // Reborn: ImmortalBody is a distinct retail class over the 76-byte EP1 ActiveBody record.
    // Reborn: the retained Highlander class is a fieldless ActiveBody extension with explicit legacy warning at qualification.
    public const uint HighlanderBodyTypeId=0x6F0F9F52u;
    public const uint ImmortalBodyTypeId = 0x88E94F7Au;
    // Reborn: ManagerBody shares the retail 76-byte EP1 ActiveBody record.
    public const uint ManagerBodyTypeId = 0x71982EACu;
    // Reborn: the center bridge segment uses ManagedActiveBody with one fieldless metadata level.
    public const uint ManagedActiveBodyTypeId = 0xEE0D6E31u;
    // Reborn: bridge segments keep their own concrete wire class.
    public const uint ManagedImmortalBodyTypeId = 0xE48BF32Cu;

    // Reborn: own-field native offsets/types are independently audited against selected stock ActiveBody tables.
    public static IReadOnlyList<Ep1GameObjectScalarTokens.Field> Fields {get;}=Array.AsReadOnly(new Ep1GameObjectScalarTokens.Field[]{
        new("MaxHealth",8,0x645864F3,4),new("MaxHealthDamagedPercentage",12,0x645864F3,4),new("MaxHealthReallyDamagedPercentage",16,0x645864F3,4),
        new("InitialHealth",20,0x645864F3,4),new("DodgePercent",24,0x645864F3,4),new("EnteringDamagedTransitionTime",28,0x998669E8,4),
        new("EnteringReallyDamagedTransitionTime",32,0x998669E8,4),new("GrabObject",36,0x805D25F4,8),new("HealingBuffFX",44,0x0D1C8E83,4),
        new("DamagedAttributeModifier",48,0x09DDCFF3,4),new("ReallyDamagedAttributeModifier",52,0x09DDCFF3,4),new("CheerRadius",56,0x645864F3,4),
        new("RemoveUpgradesOnDeath",72,0x77E3F4F7,1),new("BurningDeathBehavior",73,0x77E3F4F7,1),new("BurningDeathFX",60,0x0D1C8E83,4)
    });
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate inherited body and ordered child tokens from canonical 76-byte-root native output, preserving import indices. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native==null || native.InstanceBuffer==null || native.InstanceBuffer.Length<76 || native.InstanceBuffer.Length>844)throw new InvalidDataException("Bounded EP1 ActiveBody chunk required.");
        var b=native.InstanceBuffer;uint count=Word(b,64);
        uint concreteType=Word(b,0);
        // Reborn: the derived bodies differ in the intermediate class row and share the ActiveBody fields.
        string intermediateClass=concreteType switch
        {
            ManagerBodyTypeId => "ImmortalBodyModuleData",
            ManagedImmortalBodyTypeId => "ManagedActiveBodyModuleData",
            _ => null
        };
        // Reborn: ImmortalBody and ManagedActiveBody each inherit directly from ActiveBody.
        bool directDerived=concreteType is ImmortalBodyTypeId or ManagedActiveBodyTypeId or HighlanderBodyTypeId;
        if((concreteType!=0x5A0E3D9B && !directDerived && intermediateClass==null) || count>64 || b.Length!=76+count*12 || Word(b,68)!=(count==0?0:76) || Word(b,36)!=0 || Word(b,40)!=0 || b[72]>1 || b[73]>1 || b[74]!=0 || b[75]!=0)
            throw new InvalidDataException("Unsupported EP1 ActiveBody class/list/string/boolean layout.");
        var imports=new List<uint>();
        foreach(uint slot in new uint[]{44,48,52,60})if(Word(b,(int)slot)!=0)imports.Add(slot);
        for(int i=0;i<count;i++)if(Word(b,76+i*12+8)!=0)imports.Add((uint)(76+i*12+8));
        var relos=count==0?Array.Empty<uint>():new uint[]{68};
        CheckSlots(native.RelocationBuffer,relos);CheckSlots(native.ImportsBuffer,imports);
        // Reborn: each fieldless inheritance level adds a base row and terminator while retaining the same native body record.
        int inherited=intermediateClass!=null?48:directDerived?24:0;
        int prefix=inherited+324+(int)count*60;
        var result=new Chunk {InstanceBuffer=new byte[prefix+b.Length]};
        b.CopyTo(result.InstanceBuffer,prefix);
        Token(0,2,concreteType,12);
        if(intermediateClass!=null)
            Token(12,1,FastHash.GetHashCode(intermediateClass),36);
        if(inherited!=0)
            Token(inherited-12,1,FastHash.GetHashCode("ActiveBodyModuleData"),(uint)(inherited+12));
        BodyToken(12,1,FastHash.GetHashCode("BodyModuleData"),228);
        for(int i=0;i<Fields.Count;i++){var f=Fields[i];BodyToken(24+i*12,FastHash.GetHashCode(f.Name),f.Type,(uint)(prefix+f.Offset));}
        BodyToken(204,FastHash.GetHashCode("DamageRangeModifier"),0x751527B1,312);
        BodyToken(228,1,FastHash.GetHashCode("BehaviorModuleData"),252);BodyToken(252,1,FastHash.GetHashCode("ModuleData"),276);
        BodyToken(276,FastHash.GetHashCode("TypeId"),0x23E650B8,(uint)prefix);BodyToken(288,FastHash.GetHashCode("id"),0xBDF07965,(uint)(prefix+4));
        BodyToken(312,0x6C827073,0x5D448FD8,(uint)(prefix+64));
        for(int i=0;i<count;i++)
        {
            int table=324+(int)count*12+i*48,child=prefix+76+i*12;
            BodyToken(324+i*12,0xB151A0EB,0,(uint)table);
            BodyToken(table,FastHash.GetHashCode("Min"),0x645864F3,(uint)child);
            BodyToken(table+12,FastHash.GetHashCode("Max"),0x645864F3,(uint)(child+4));
            BodyToken(table+24,FastHash.GetHashCode("AttributeModifier"),0x09DDCFF3,(uint)(child+8));
        }
        if(count!=0)Put(result.InstanceBuffer,prefix+68,(uint)(prefix+76));
        result.RelocationBuffer=Pack(relos.Select(s=>s+(uint)prefix));result.ImportsBuffer=Pack(imports.Select(s=>s+(uint)prefix));return result;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: token metadata never enters native relocation/import streams. */
        //-------------------------------------------------------------------------------------------------
        void BodyToken(int at,uint name,uint fieldType,uint target) =>
            Token(at+inherited,name,fieldType,target<324+(int)count*60 && target!=0 ? target+(uint)inherited : target);

        void Token(int at,uint name,uint type,uint target){Put(result.InstanceBuffer,at,name);Put(result.InstanceBuffer,at+4,type);Put(result.InstanceBuffer,at+8,target);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact auxiliary membership forbids unreviewed storage owners, duplicate slots and missing sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSlots(byte[] b,IEnumerable<uint> expected)
    {
        var a=expected.ToArray();
        if(b==null || b.Length!=(a.Length==0?0:(a.Length+1)*4) || (a.Length!=0 && (Word(b,b.Length-4)!=uint.MaxValue || !Enumerable.Range(0,a.Length).Select(i=>Word(b,i*4)).OrderBy(s=>s).SequenceEqual(a.OrderBy(s=>s)))))throw new InvalidDataException("EP1 body auxiliary ownership differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent streams stay empty; each present stream has one final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: chunk reads stay within the supplied buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Body fragment bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: body native and metadata coordinates use little-endian Win32 words. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
