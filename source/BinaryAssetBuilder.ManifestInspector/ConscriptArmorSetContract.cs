using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: ArmorSet cardinality, optional imports and flags come from source, not the three captured Conscript entries.
internal static class ConscriptArmorSetContract
{
    internal sealed record Allocation(int Start,int Length,uint[] Relocations,uint[] Imports);
    internal static XmlElement[] Entries(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ArmorSet").ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only comparison/reference-template copies receive stock entries; real source still supplies every compiled armor row. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        foreach(var node in Entries(source))source.DocumentElement!.RemoveChild(node);
        var model=Entries(baseline);if(model.Length==0)return;
        int ordinal=baseline.DocumentElement!.ChildNodes.OfType<XmlElement>().TakeWhile(e=>e!=model[0]).Count();
        var before=source.DocumentElement!.ChildNodes.OfType<XmlElement>().ElementAtOrDefault(ordinal);
        foreach(var node in model)source.DocumentElement.InsertBefore(source.ImportNode(node,true),before);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify source-sized native rows and exact typed import identities, including omitted Armor or DamageFX slots. */
    //-------------------------------------------------------------------------------------------------
    internal static Allocation VerifyNative(XmlDocument source,byte[] native,InstanceDeclaration instance)
    {
        var rows=Entries(source);int count=checked((int)Word(native,420)),start=checked((int)Word(native,424));
        if(count!=rows.Length||(count==0)!=(start==0)||count>native.Length/12||start%4!=0||start<0||start>native.Length-count*12||(count>0&&start<600))throw new InvalidDataException("ArmorSet source/native list layout differs.");
        var imports=new List<uint>();
        for(int i=0;i<count;i++){
            int member=start+i*12;Reference(rows[i],"Armor","ArmorTemplate",member);Reference(rows[i],"DamageFX","DamageFX",member+4);
            if(Word(native,member+8)!=Flags(rows[i].GetAttribute("Conditions")))throw SdkXmlDiagnostics.At(new InvalidDataException("ArmorSet Conditions differ from authored source."),(XmlNode?)rows[i].GetAttributeNode("Conditions")??rows[i]);
        }
        return new(start,count*12,count==0?Array.Empty<uint>():new[]{424u},imports.ToArray());

        // Reborn: normalized Core indices are checked against named source identities rather than copied stock occurrence ordinals.
        void Reference(XmlElement row,string attribute,string type,int at)
        {
            string name=row.GetAttribute(attribute).Trim();uint index=Word(native,at);
            if(name.Length==0){if(index!=0)throw new InvalidDataException("Omitted ArmorSet reference still has an import.");return;}
            int colon=name.IndexOf(':');if(colon>=0){if(name[..colon]!=type)throw SdkXmlDiagnostics.At(new InvalidDataException("ArmorSet reference type differs: "+name),row.GetAttributeNode(attribute));name=name[(colon+1)..];}
            if(index==0||index>instance.ReferencedInstances.Count)throw new InvalidDataException("ArmorSet reference index exceeds Core occurrences.");
            var actual=instance.ReferencedInstances[(int)index-1];if(actual.TypeId!=FastHash.GetHashCode(type)||actual.InstanceId!=InstanceHandle.GetInstanceId(name))throw new InvalidDataException("ArmorSet import differs from authored target.");imports.Add((uint)at);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: count/item/field tags follow the independently captured ArmorSet grammar; only authored cardinality and native addresses vary. */
    //-------------------------------------------------------------------------------------------------
    internal static int VerifyMetadata(XmlDocument source,byte[] native,byte[] tokens,int prefix,int table)
    {
        int count=Entries(source).Length,items=checked((int)Word(native,424));
        Row(table,0x6C827073,0x5D448FD8,(uint)(prefix+420));
        for(int i=0;i<count;i++){
            int record=checked(table+12+count*12+i*48),member=checked(prefix+items+i*12);
            Row(table+12+i*12,0xB151A0EB,0,(uint)record);Row(record,FastHash.GetHashCode("Armor"),0x34E9127B,(uint)member);Row(record+12,FastHash.GetHashCode("DamageFX"),0x9A95EDFF,(uint)(member+4));Row(record+24,FastHash.GetHashCode("Conditions"),0xED8BC8E9,(uint)(member+8));Row(record+36,0,0,0);
        }
        return checked(12+count*60);
        // Reborn: literal expected field tags do not come from the metadata producer under test.
        void Row(int at,uint name,uint type,uint target){if(at<0||at>prefix-12||Word(tokens,at)!=name||Word(tokens,at+4)!=type||Word(tokens,at+8)!=target)throw new InvalidDataException("ArmorSet metadata field/tag/target differs.");}
    }

    // Reborn: accept any combination of official ArmorSetType values, not only conditions present in the captured unit.
    private static uint Flags(string text){uint mask=0;foreach(string token in text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries)){int bit=(int)Enum.Parse<ArmorSetType>(token,false);if(bit<0||bit>=ArmorSetBitFlags.Count)throw new InvalidDataException("ArmorSet condition has no native bit.");mask|=1u<<bit;}return mask;}
    // Reborn: independent native/metadata reads never escape their supplied buffer.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("ArmorSet word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
