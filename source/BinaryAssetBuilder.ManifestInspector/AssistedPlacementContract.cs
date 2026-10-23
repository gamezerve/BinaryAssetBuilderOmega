using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 assisted placement — hint mask, snapping distance and the optional requirement ObjectFilter are proved from source,
// with exact allocation/relocation ownership and a literal metadata writer qualified against stock SovietRefinery.
internal static class AssistedPlacementContract
{
    private const uint TypeId=0xAE0EE9A0u;
    internal const int HeaderSize=20;
    private const int FilterSize=120;
    private static ConscriptModuleScalarContract.Binding Status(string name)=>new(name,0,32,"Flags:SageBinaryData.ObjectStatusType",null,0xA7BD811Eu);
    private static readonly ConscriptModuleScalarContract.Binding[] FilterFields={
        new("Rule",4,4,"Enum:SageBinaryData.ObjectFilterRule","NONE",0x0027BB58u),
        new("Relationship",8,4,"Flags:SageBinaryData.ObjectFilterRelationship","",0x5C8981A9u),
        new("Alignment",12,4,"Enum:SageBinaryData.ObjectFilterAlignment","NONE",0x9FFD43EBu),
        new("Include",16,40,"Flags:SageBinaryData.KindOf",null,0x9AFC9193u),
        new("Exclude",56,40,"Flags:SageBinaryData.KindOf",null,0x9AFC9193u)};
    private static XmlElement? Filter(XmlElement owner)=>owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="RequirementObjectFilter");
    private static string[] Things(XmlElement filter,string name)=>filter.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name).Select(e=>e.InnerText.Trim()).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: filter values must be representable; an authored filter id has no reviewed storage. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        var filter=Filter(owner);if(filter==null)return;
        foreach(var field in FilterFields)_=ConscriptModuleScalarContract.Bytes(filter,field);
        foreach(string name in new[]{"StatusBitFlags","StatusBitFlagsExclude"})if(filter.HasAttribute(name))_=ConscriptModuleScalarContract.Bytes(filter,Status(name));
        var id=filter.GetAttributeNode("id");if(id!=null&&id.Value.Trim().Length!=0)throw SdkXmlDiagnostics.At(new InvalidDataException("AssistedPlacementBehavior RequirementObjectFilter id is not implemented yet."),id);
        foreach(var thing in filter.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "IncludeThing" or "ExcludeThing"))if(thing.InnerText.Trim().Length==0)throw SdkXmlDiagnostics.At(new InvalidDataException("AssistedPlacementBehavior filter thing name is empty."),thing);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the whole optional filter is proved by Verify, so comparison copies take the model's filter shape. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="RequirementObjectFilter").ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="RequirementObjectFilter"))actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify ABI, inheritance, the complete stock payload and generated metadata byte-for-byte. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet schemas)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="AssistedPlacementBehaviorModuleData");var next=modules[Array.IndexOf(modules,module)+1];
        if(Marshal.SizeOf<AssistedPlacementBehaviorModuleData>()!=HeaderSize||module.TypeId!=TypeId||!module.ClassChain.SequenceEqual(new[]{"AssistedPlacementBehaviorModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 assisted placement stock ABI/inheritance differs.");
        var b=captured.Bytes.AsSpan(module.Native,next.Native-module.Native).ToArray();var relos=new List<uint>();
        void Rebase(int at){uint v=Word(b,at);if(v==0)return;if(v<module.Native||v>=next.Native)throw new InvalidDataException("Assisted placement stock pointer escapes its module.");Put(b,at,v-(uint)module.Native);relos.Add((uint)at);}
        Rebase(16);int filter=(int)Word(b,16);if(filter!=0)foreach(int at in new[]{filter+96,filter+100,filter+108,filter+116})Rebase(at);
        var native=new Chunk{InstanceBuffer=b,RelocationBuffer=Pack(relos),ImportsBuffer=Array.Empty<byte>()};
        var sample=new XmlDocument();sample.LoadXml("<AssistedPlacementBehavior id='ModuleTag_AsssitedPlacementBehavior' PlacementHint='FACING_TOWARDS ALLOW_SHROUDED' SnappingDistance='50'><RequirementObjectFilter Rule='ANY' Include='ORE_NODE'/></AssistedPlacementBehavior>");
        var generated=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,schemas,"AssistedPlacementBehaviorModuleData",typeof(AssistedPlacementBehaviorModuleData));
        Verify(sample.DocumentElement!,native,generated);
        int prefix=generated.InstanceBuffer.Length-b.Length;if(next.Wrapper-module.Wrapper!=prefix)throw new InvalidDataException("Assisted placement stock metadata extent differs.");
        for(int at=0;at<prefix;at+=12){uint target=Word(captured.Bytes,module.Wrapper+at+8);if(target!=0)target=target>=module.Native?target-(uint)module.Native+(uint)prefix:target-(uint)module.Wrapper;
            if(Word(generated.InstanceBuffer,at)!=Word(captured.Bytes,module.Wrapper+at)||Word(generated.InstanceBuffer,at+4)!=Word(captured.Bytes,module.Wrapper+at+4)||Word(generated.InstanceBuffer,at+8)!=target)throw new InvalidDataException("Assisted placement stock field/table target differs at metadata +"+at);}
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove header scalars, every filter value/list item, allocation coverage and the exact relocation inventory. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata)
    {
        Validate(owner);var b=native.InstanceBuffer;var covered=new bool[b.Length];var relos=new HashSet<uint>();
        if(b.Length<HeaderSize||Word(b,0)!=TypeId||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("EP1 assisted placement identity/header differs.");
        Claim(0,HeaderSize);
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(AssistedPlacementBehaviorModuleData)))Check(owner,binding,0);
        if(!ConscriptModuleScalarContract.Bindings(typeof(AssistedPlacementBehaviorModuleData)).Any(f=>f.Name=="PlacementHint"))throw new InvalidDataException("EP1 assisted placement hint codec is not bound.");
        var filter=Filter(owner);int f0=Pointer(16,filter!=null,FilterSize);
        if(filter!=null){
            if(Word(b,f0)!=0)throw new InvalidDataException("EP1 assisted placement filter id storage differs.");
            foreach(var field in FilterFields)Check(filter,field,f0);
            foreach(var s in new[]{("StatusBitFlags",96),("StatusBitFlagsExclude",100)}){bool present=filter.HasAttribute(s.Item1);int at=Pointer(f0+s.Item2,present,32);if(present)Check(filter,Status(s.Item1),at);}
            foreach(var l in new[]{("IncludeThing",104),("ExcludeThing",112)}){var names=Things(filter,l.Item1);if(Word(b,f0+l.Item2)!=names.Length)throw new InvalidDataException("EP1 assisted placement filter list count differs: "+l.Item1);int items=Pointer(f0+l.Item2+4,names.Length!=0,names.Length*4);for(int i=0;i<names.Length;i++)if(Word(b,items+i*4)!=FastHash.GetHashCode(names[i].ToLowerInvariant()))throw new InvalidDataException("EP1 assisted placement filter weak id differs: "+names[i]);}
        }
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("EP1 assisted placement unowned native allocation at +"+i);}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||native.ImportsBuffer.Length!=0)throw new InvalidDataException("EP1 assisted placement relocation/import inventory differs.");
        byte[] expected=Metadata(b);if(metadata.InstanceBuffer.Length-b.Length!=expected.Length||!metadata.InstanceBuffer.AsSpan(0,expected.Length).SequenceEqual(expected))throw new InvalidDataException("EP1 assisted placement generated/literal metadata differ.");

        void Check(XmlElement element,ConscriptModuleScalarContract.Binding field,int start){if(!b.AsSpan(start+field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,field)))throw new InvalidDataException("EP1 assisted placement authored field differs: "+element.LocalName+"."+field.Name);}
        int Pointer(int at,bool present,int size){uint target=Word(b,at);if(!present){if(target!=0)throw new InvalidDataException("Absent EP1 assisted placement field has storage at +"+at);return 0;}if(target<HeaderSize||target>int.MaxValue||target%4!=0)throw new InvalidDataException("EP1 assisted placement pointer differs at +"+at);Claim((int)target,size);relos.Add((uint)at);return (int)target;}
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("EP1 assisted placement allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal schema-ordered metadata (132 bytes without a filter, filter tables and list rows after the base chain). */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(byte[] n)
    {
        int f=(int)Word(n,16);int ni=f==0?0:(int)Word(n,f+104),ne=f==0?0:(int)Word(n,f+112);int size=f==0?132:264+12*(1+ni)+12*(1+ne);uint P=(uint)size;var b=new byte[size];
        uint Ptr(int slot)=>Word(n,slot)==0?0:P+Word(n,slot);
        Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("BehaviorModuleData"),72);
        Row(24,FastHash.GetHashCode("PlacementHint"),0xFA98DE4Du,P+8);Row(36,FastHash.GetHashCode("SnappingDistance"),0x645864F3u,P+12);Row(48,FastHash.GetHashCode("RequirementObjectFilter"),0xE46EA3B2u,f==0?0u:132u);
        Row(72,1,FastHash.GetHashCode("ModuleData"),96);Row(96,FastHash.GetHashCode("TypeId"),0x23E650B8u,P);Row(108,FastHash.GetHashCode("id"),0xBDF07965u,P+4);
        if(f!=0){
            uint F=(uint)f;var filter=new (string,uint,uint)[]{("id",0x4D3B3F19u,P+F),("Rule",0x0027BB58u,P+F+4),("Relationship",0x5C8981A9u,P+F+8),("Alignment",0x9FFD43EBu,P+F+12),("Include",0x9AFC9193u,P+F+16),("Exclude",0x9AFC9193u,P+F+56),("StatusBitFlags",0xA7BD811Eu,Ptr(f+96)),("StatusBitFlagsExclude",0xA7BD811Eu,Ptr(f+100)),("IncludeThing",0x9B19049Fu,264u),("ExcludeThing",0x9B19049Fu,(uint)(264+12*(1+ni)))};
            for(int i=0;i<filter.Length;i++)Row(132+i*12,FastHash.GetHashCode(filter[i].Item1),filter[i].Item2,filter[i].Item3);
            int at=264;foreach(var (count,slot) in new[]{(ni,f+104),(ne,f+112)}){Row(at,0x6C827073u,0x5D448FD8u,P+(uint)slot);uint items=Word(n,slot+4);for(int k=0;k<count;k++)Row(at+12+k*12,0xB151A0EBu,0,P+items+(uint)(k*4));at+=12*(1+count);}
        }
        return b;
        void Row(int at,uint name,uint type,uint target){Put(b,at,name);Put(b,at+4,type);Put(b,at+8,target);}
    }

    private static byte[] Pack(IEnumerable<uint> slots){var list=slots.ToList();if(list.Count==0)return Array.Empty<byte>();var b=new byte[list.Count*4+4];for(int i=0;i<list.Count;i++)Put(b,i*4,list[i]);Put(b,list.Count*4,uint.MaxValue);return b;}
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("EP1 assisted placement auxiliary terminator differs.");return Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();}
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
