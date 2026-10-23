using System.Buffers.Binary;
using System.Xml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: optional ObjectResourceInfo and source-sized BuildCost implement the official zero/one/many grammar without inventing a default transaction.
internal static class GameObjectResourceContract
{
    internal static XmlElement? Owner(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName=="ObjectResourceInfo");
    internal static XmlElement[] Entries(XmlDocument source)=>Owner(source)?.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="BuildCost").ToArray()??Array.Empty<XmlElement>();

    // Reborn: an existing transaction must carry both required unsigned fields; schema syntax and native uint32 representability remain distinct checks.
    internal static void Validate(XmlDocument source)
    {
        var owners=source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ObjectResourceInfo").ToArray();
        if(owners.Length>1)throw SdkXmlDiagnostics.At(new InvalidDataException("GameObject permits at most one ObjectResourceInfo."),owners[1]);
        foreach(var row in Entries(source))foreach(string name in new[]{"Account","Amount"})
        {
            if(!row.HasAttribute(name))throw SdkXmlDiagnostics.At(new InvalidDataException("BuildCost requires @"+name+" when the transaction is present."),row);
            _=ConscriptModuleScalarContract.Bytes(row,ConscriptModuleScalarContract.RecordBindings(typeof(MoneyTransaction)).Single(b=>b.Name==name));
        }
    }

    // Reborn: comparison copies use the baseline owner only; real source preserves omitted, empty, explicit-zero and multiple-transaction states.
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        var actual=Owner(source);var model=Owner(baseline);
        if(actual!=null){if(model==null)source.DocumentElement!.RemoveChild(actual);else actual.ParentNode!.ReplaceChild(source.ImportNode(model,true),actual);return;}
        if(model==null)return;
        var following=baseline.DocumentElement!.ChildNodes.OfType<XmlElement>().SkipWhile(e=>e!=model).Skip(1).Select(e=>e.LocalName).ToHashSet(StringComparer.Ordinal);
        var before=source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>following.Contains(e.LocalName));
        source.DocumentElement!.InsertBefore(source.ImportNode(model,true),before);
    }

    // Reborn: verify the actual optional owner, list header, full allocation and exact relocation ownership; transaction values are independently checked by the nested scalar contract.
    internal static ConscriptArmorSetContract.Allocation VerifyNative(XmlDocument source,byte[] native,int end)
    {
        int start=checked((int)Word(native,416));bool present=Owner(source)!=null;
        if(!present){if(start!=0)throw new InvalidDataException("Absent ObjectResourceInfo unexpectedly has native storage.");return new(0,0,Array.Empty<uint>(),Array.Empty<uint>());}
        var rows=Entries(source);int size=checked(8+rows.Length*8);
        if(start<600||start%4!=0||start>native.Length-size||end!=start+size)throw new InvalidDataException("ObjectResourceInfo source/native allocation differs.");
        int count=checked((int)Word(native,start)),items=checked((int)Word(native,start+4));
        if(count!=rows.Length||items!=(count==0?0:start+8))throw new InvalidDataException("BuildCost source/native count or pointer differs.");
        return new(start,size,count==0?new[]{416u}:new[]{416u,(uint)(start+4)},Array.Empty<uint>());
    }

    // Reborn: verify the independently captured MoneyTransaction metadata grammar at source-sized extents, retaining a null root target for an absent owner.
    internal static int VerifyMetadata(XmlDocument source,byte[] native,byte[] tokens,int prefix,int table)
    {
        if(Owner(source)==null){if(table!=0)throw new InvalidDataException("Absent ObjectResourceInfo retains a metadata target.");return 0;}
        int count=Entries(source).Length,start=checked((int)Word(native,416)),items=checked((int)Word(native,start+4)),list=checked(table+24);
        Row(table,BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("BuildCost"),0xB706334D,(uint)list);Row(table+12,0,0,0);
        Row(list,0x6C827073,0x5D448FD8,(uint)(prefix+start));
        for(int i=0;i<count;i++)
        {
            int record=checked(list+12+count*12+i*36),member=checked(prefix+items+i*8);
            Row(list+12+i*12,0xB151A0EB,0,(uint)record);
            Row(record,BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Account"),0x763177A4,(uint)member);
            Row(record+12,BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Amount"),0x763177A4,(uint)(member+4));Row(record+24,0,0,0);
        }
        return checked(36+count*48);
        // Reborn: exact tags, types, sentinels and targets come from the independent stock grammar, never the metadata producer being checked.
        void Row(int at,uint name,uint type,uint target){if(at<0||at>prefix-12||Word(tokens,at)!=name||Word(tokens,at+4)!=type||Word(tokens,at+8)!=target)throw new InvalidDataException("ObjectResourceInfo/BuildCost metadata differs.");}
    }

    // Reborn: receipts retain exact uint32 transaction values instead of converting money through float32.
    internal static uint Value(XmlElement row,string name)=>BinaryPrimitives.ReadUInt32LittleEndian(ConscriptModuleScalarContract.Bytes(row,ConscriptModuleScalarContract.RecordBindings(typeof(MoneyTransaction)).Single(b=>b.Name==name)));

    // Reborn: native pointer and metadata reads stay within their supplied buffers.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("ObjectResourceInfo word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
