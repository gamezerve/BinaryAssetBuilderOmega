using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: movement row count, optional imports and native allocation follow authored LocomotorSet entries, not a captured stock unit.
internal static class ConscriptLocomotorSetContract
{
    internal static XmlElement[] Entries(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="LocomotorSet").ToArray();

    // Reborn: restore stock comparison coordinates only; the actual compiled source retains every authored row and target.
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        foreach(var entry in Entries(source))source.DocumentElement!.RemoveChild(entry);
        var rows=Entries(baseline);if(rows.Length==0)return;
        // Reborn: optional earlier owners change ordinals; locate the next schema-ordered family rather than inserting by a stock child index.
        var following=baseline.DocumentElement!.ChildNodes.OfType<XmlElement>().SkipWhile(e=>e!=rows[0]).Skip(1).Select(e=>e.LocalName).Where(n=>n!="LocomotorSet").ToHashSet(StringComparer.Ordinal);
        var before=source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>following.Contains(e.LocalName));
        foreach(var row in rows)source.DocumentElement.InsertBefore(source.ImportNode(row,true),before);
    }

    // Reborn: independently verify list allocation and exact authored strong target selectors; scalar/default bytes are checked by the nested-value contract.
    internal static ConscriptArmorSetContract.Allocation VerifyNative(XmlDocument source,byte[] native,InstanceDeclaration instance)
    {
        var rows=Entries(source);int count=checked((int)Word(native,428)),start=checked((int)Word(native,432));
        if(count!=rows.Length||(count==0)!=(start==0)||count>native.Length/12||start%4!=0||start<0||start>native.Length-count*12||(count>0&&start<600))throw new InvalidDataException("LocomotorSet source/native allocation differs.");
        var imports=new List<uint>();
        for(int i=0;i<count;i++)
        {
            string name=rows[i].GetAttribute("Locomotor").Trim();int at=start+i*12+8;uint index=Word(native,at);
            if(name.Length==0){if(index!=0)throw SdkXmlDiagnostics.At(new InvalidDataException("Omitted Locomotor still has an import."),rows[i]);continue;}
            int colon=name.IndexOf(':');if(colon>=0){if(name[..colon]!="LocomotorTemplate")throw SdkXmlDiagnostics.At(new InvalidDataException("Wrong Locomotor target type."),rows[i]);name=name[(colon+1)..];}
            if(index==0||index>instance.ReferencedInstances.Count)throw new InvalidDataException("Locomotor import selector exceeds Core occurrences.");
            var target=instance.ReferencedInstances[(int)index-1];
            if(target.TypeId!=FastHash.GetHashCode("LocomotorTemplate")||target.InstanceId!=InstanceHandle.GetInstanceId(name))throw SdkXmlDiagnostics.At(new InvalidDataException("Locomotor import differs from authored target."),rows[i]);
            imports.Add((uint)at);
        }
        return new(start,count*12,count==0?Array.Empty<uint>():new[]{432u},imports.ToArray());
    }

    // Reborn: independently captured list grammar retains exact field tags/types while authored cardinality selects metadata extent.
    internal static int VerifyMetadata(XmlDocument source,byte[] native,byte[] tokens,int prefix,int table)
    {
        int count=Entries(source).Length,items=checked((int)Word(native,432));Row(table,0x6C827073,0x5D448FD8,(uint)(prefix+428));
        for(int i=0;i<count;i++)
        {
            int record=checked(table+12+count*12+i*48),member=checked(prefix+items+i*12);
            Row(table+12+i*12,0xB151A0EB,0,(uint)record);
            Row(record,FastHash.GetHashCode("Speed"),0x645864F3,(uint)member);
            Row(record+12,FastHash.GetHashCode("Condition"),0x845D12B2,(uint)(member+4));
            Row(record+24,FastHash.GetHashCode("Locomotor"),0xFED20C6C,(uint)(member+8));Row(record+36,0,0,0);
        }
        return checked(12+count*60);
        // Reborn: field/tag checks never use expectations read from the token producer under test.
        void Row(int at,uint name,uint type,uint target){if(at<0||at>prefix-12||Word(tokens,at)!=name||Word(tokens,at+4)!=type||Word(tokens,at+8)!=target)throw new InvalidDataException("LocomotorSet metadata field/tag/target differs.");}
    }

    // Reborn: checked reads reject malformed allocation addresses before inspecting any list payload.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("LocomotorSet word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
