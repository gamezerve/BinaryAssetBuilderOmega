using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: generate the common EP1 AI inheritance/chooser metadata only after exact native ownership; nonempty contact/vision lists remain a separate token gate.
public static class Ep1AIUpdateTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit bounded ASCII/chooser owners and exact auxiliary slots before emitting any AI fragment or rebasing pointers. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if(native?.InstanceBuffer==null || native.InstanceBuffer.Length<76 || native.InstanceBuffer.Length>1048576 || native.RelocationBuffer==null || native.ImportsBuffer==null)throw new InvalidDataException("Bounded EP1 AI native graph required.");
        var b=native.InstanceBuffer;var relos=Slots(native.RelocationBuffer);var imports=Slots(native.ImportsBuffer);var expectedRelos=new List<uint>();var expectedImports=new List<uint>();var owners=new List<(int Start,int End)>();Own(0,76);
        if(Word(0)!=0x08DBC00E || Word(4)==0 || Word(16)>>5!=0 || Word(60)>16 || unchecked((int)Word(64)) is <-1 or >18 || b[72]>1 || b[73]>1 || b[74]!=0 || b[75]!=0)throw new InvalidDataException("AI concrete identity/enum/mask/flag/padding differs.");
        // Reborn: empty string lists have a reviewed count anchor; nonempty element-token encoding is not inferred from native-only tests.
        if(Word(40)!=0 || Word(44)!=0)throw new InvalidDataException("Nonempty AI SpecialContactPoints tokenization is not yet admitted.");
        foreach(int at in new[]{20,24,28,32,36,48,56})Real(at);
        int chooser=(int)Word(68);
        if(chooser!=0)
        {
            if(chooser<76 || chooser%4!=0)throw new InvalidDataException("AI chooser alignment differs.");Own(chooser,40);expectedRelos.Add(68);
            foreach(int at in new[]{0,4,8,12})Real(chooser+at);
            if(Word(chooser+16)>>4!=0 || Word(chooser+20)!=0 || Word(chooser+24)!=0 || b.AsSpan(chooser+28,6).ToArray().Any(v=>v>1) || b[chooser+34]!=0 || b[chooser+35]!=0 || Word(chooser+36)==0)throw new InvalidDataException("AI chooser flags/empty vision-list/strong reference differs.");expectedImports.Add((uint)(chooser+36));
        }
        int length=(int)Word(8),text=(int)Word(12);
        if(text==0){if(length!=0)throw new InvalidDataException("AI absent text has length.");}
        else
        {
            if(length<0 || length>4096 || text<76 || text>b.Length-length-1 || b[text+length]!=0)throw new InvalidDataException("AI text bounds/terminator differ.");Own(text,length+1);for(int i=0;i<length;i++)if(b[text+i]==0 || b[text+i]>127)throw new InvalidDataException("AI token text currently requires non-NUL ASCII.");expectedRelos.Add(12);
        }
        if(!relos.Order().SequenceEqual(expectedRelos.Order()) || !imports.Order().SequenceEqual(expectedImports.Order()))throw new InvalidDataException("AI native auxiliary ownership differs.");
        int end=0;foreach(var owner in owners.OrderBy(o=>o.Start)){Padding(end,owner.Start);end=owner.End;}Padding(end,b.Length);
        int prefix=chooser==0?348:552;var output=new Chunk{InstanceBuffer=new byte[checked(prefix+b.Length)]};b.CopyTo(output.InstanceBuffer,prefix);
        Token(0,2,0x08DBC00E,12);Token(12,1,0xF1002505,228);
        var fields=new[]{("AttackPriority",8,0x1A2933AAu),("AutoAcquireEnemiesWhenIdle",16,0x8B0FB9DAu),("StopChaseDistance",20,0x645864F3u),("StandGround",72,0x77E3F4F7u),("MinCowerTime",24,0x998669E8u),("MaxCowerTime",28,0x998669E8u),("BurningDeathTime",32,0x998669E8u),("ComboLocoAttackDistance",36,0x645864F3u),("SpecialContactPoints",40,0x6D66AE18u),("FadeOnPortals",73,0x77E3F4F7u),("IdleTargetTime",48,0x998669E8u),("MaxCombineChildren",52,0x763177A4u),("SpawnOffsetRadius",56,0x645864F3u),("StateMachine",60,0xAC6E35E0u),("DefaultLocomotorSet",64,0x845D12B2u),("UnitAITargetChooserData",68,0x243BEF01u)};
        for(int i=0;i<fields.Length;i++){var f=fields[i];uint target=f.Item1=="SpecialContactPoints"?336u:f.Item1=="UnitAITargetChooserData"?(chooser==0?0u:348u):(uint)(prefix+f.Item2);Token(24+i*12,FastHash.GetHashCode(f.Item1),f.Item3,target);}
        // Reborn: three empty inherited tables lead to the terminal ModuleData identity fields, not guessed native base offsets.
        string[] bases={"ContainModuleData","BehaviorModuleData","ModuleData"};for(int i=0;i<3;i++)Token(228+i*24,1,FastHash.GetHashCode(bases[i]),(uint)(252+i*24));Token(300,FastHash.GetHashCode("TypeId"),0x23E650B8,(uint)prefix);Token(312,FastHash.GetHashCode("id"),0xBDF07965,(uint)(prefix+4));Token(336,0x6C827073,0x5D448FD8,(uint)(prefix+40));
        if(chooser!=0)
        {
            Token(348,1,0xAA42EB9C,384);Token(360,FastHash.GetHashCode("TargetingCompareList"),0x1B85B469,(uint)(prefix+chooser+36));
            var child=new[]{("IdleScanDelay",0,0x998669E8u),("ReacquireDelay",4,0x998669E8u),("StartupDelay",8,0x998669E8u),("SympathyRange",12,0x645864F3u),("CheckVisionRange",28,0x77E3F4F7u),("CanPickDynamicTargets",29,0x77E3F4F7u),("CanPickDynamicTargetType",16,0x344E0F38u),("RotateToTargetWhenAiming",30,0x77E3F4F7u),("CanAutoAcquireNonAutoAcquirable",31,0x77E3F4F7u),("CanChaseAttack",32,0x77E3F4F7u),("CanOnlyAttackTargetsInFront",33,0x77E3F4F7u),("ObjectVisionRange",20,0x2E11FC54u)};
            for(int i=0;i<child.Length;i++){var f=child[i];Token(384+i*12,FastHash.GetHashCode(f.Item1),f.Item3,f.Item1=="ObjectVisionRange"?540u:(uint)(prefix+chooser+f.Item2));}Token(540,0x6C827073,0x5D448FD8,(uint)(prefix+chooser+20));
        }
        foreach(uint slot in relos){uint target=Word((int)slot);if(target>=b.Length)throw new InvalidDataException("AI pointer target exceeds native owner.");BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(prefix+(int)slot,4),checked(target+(uint)prefix));}
        output.RelocationBuffer=Pack(relos.Select(s=>checked(s+(uint)prefix)));output.ImportsBuffer=Pack(imports.Select(s=>checked(s+(uint)prefix)));return output;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: no record/string may overlap another owner or point outside the captured native fragment. */
        //-------------------------------------------------------------------------------------------------
        void Own(int at,int size){if(at<0 || size<0 || at>b.Length-size || owners.Any(o=>at<o.End && at+size>o.Start))throw new InvalidDataException("AI native allocations overlap or exceed bounds.");owners.Add((at,at+size));}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: unowned storage is limited to at most three zero alignment bytes between/following recognized allocations. */
        //-------------------------------------------------------------------------------------------------
        void Padding(int from,int to){if(to<from || to-from>3 || b.AsSpan(from,to-from).ToArray().Any(v=>v!=0))throw new InvalidDataException("AI native orphan/padding bytes differ.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all reviewed real/time words must be finite, independently of permissive source lexical patterns. */
        //-------------------------------------------------------------------------------------------------
        void Real(int at){if(!float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(at)))))throw new InvalidDataException("Nonfinite native AI value.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: every native read is a checked little-endian offset, never an unsafe captured pointer. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("AI native word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: metadata offsets belong to the generated prefix; they are not native relocation slots. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(output.InstanceBuffer.AsSpan(at+8,4),target);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: input auxiliary buffers are empty or uniquely sentinel-framed and cannot hide duplicate/sentinel-only slots. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8 || bytes.Length%4!=0 || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(bytes.Length-4,4))!=uint.MaxValue)throw new InvalidDataException("AI auxiliary framing differs.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i*4,4))).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Duplicate AI auxiliary slots.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: rebased auxiliary slots preserve occurrence order and exactly one final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.ToArray();if(values.Length==0)return Array.Empty<byte>();var bytes=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(i*4,4),values[i]);BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(values.Length*4,4),uint.MaxValue);return bytes;}
}
