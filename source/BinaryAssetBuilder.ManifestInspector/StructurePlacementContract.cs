using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 structure placement — the inline restricted-area Shape (enums, radii, angle, flags, optional strings and Offset vector)
// is proved from source with exact allocation/relocation ownership and a literal metadata writer qualified against stock SovietRefinery.
internal static class StructurePlacementContract
{
    private const uint TypeId=0xE9D1B7EBu;
    internal const int HeaderSize=56;
    internal const int MetadataSize=252;
    private const int Shape=8;
    private static readonly ConscriptModuleScalarContract.Binding[] ShapeFields={
        new("Type",0,4,"Enum:SageBinaryData.Ep1GeometryType","SPHERE",0xB0D49164u),
        new("ContactPointGeneration",4,4,"Enum:SageBinaryData.ContactPointGenerationType","NONE",0x465508CEu),
        new("MajorRadius",16,4,"Single","1.0",0x645864F3u),
        new("MinorRadius",20,4,"Single","1.0",0x645864F3u),
        new("Height",32,4,"Single","1.0",0x645864F3u),
        new("FrontAngle",36,4,"Angle",null,0x4E16EA4Du),
        new("Active",44,1,"SageBool","true",0x77E3F4F7u),
        new("UsedForHealthBox",45,1,"SageBool","true",0x77E3F4F7u)};
    private static readonly (string Name,int Offset)[] Texts={("Name",8),("Other",24)};
    private static readonly string[] Axes={"x","y","z"};
    private static ConscriptModuleScalarContract.Binding Axis(int i)=>new(Axes[i],i*4,4,"Single","0.0",0x645864F3u);
    private static XmlElement Area(XmlElement owner)=>owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="RestrictedAreaShape")??throw SdkXmlDiagnostics.At(new InvalidDataException("StructurePlacementBehavior requires RestrictedAreaShape"),owner);
    private static XmlElement? Offset(XmlElement area)=>area.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="Offset");

    internal static void Validate(XmlElement owner)
    {
        var area=Area(owner);foreach(var f in ShapeFields){var bytes=ConscriptModuleScalarContract.Bytes(area,f);
            // Reborn: the legacy marshaler accepts only true/false; canonicalize schema-legal 1/0 spellings exactly as module bindings do.
            if(f.Codec=="SageBool"&&area.HasAttribute(f.Name))area.SetAttribute(f.Name,bytes[0]!=0?"true":"false");}
        foreach(var t in Texts)if(area.HasAttribute(t.Name)&&area.GetAttribute(t.Name).Any(c=>c>127||c=='\0'))throw SdkXmlDiagnostics.At(new InvalidDataException("RestrictedAreaShape."+t.Name+" must be ASCII without NUL."),area.GetAttributeNode(t.Name));
        var offset=Offset(area);if(offset!=null)for(int i=0;i<3;i++)_=ConscriptModuleScalarContract.Bytes(offset,Axis(i));
    }

    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="RestrictedAreaShape").ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="RestrictedAreaShape"))actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify ABI, inheritance, the complete stock BOX payload with its Offset vector, and metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet schemas)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="StructurePlacementBehaviorModuleData");var next=modules[Array.IndexOf(modules,module)+1];
        if(Marshal.SizeOf<StructurePlacementBehaviorModuleData>()!=HeaderSize||module.TypeId!=TypeId||next.Wrapper-module.Wrapper!=MetadataSize||!module.ClassChain.SequenceEqual(new[]{"StructurePlacementBehaviorModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 structure placement stock ABI/inheritance differs.");
        var b=captured.Bytes.AsSpan(module.Native,next.Native-module.Native).ToArray();var relos=new List<uint>();
        foreach(int slot in new[]{Shape+12,Shape+28,Shape+40}){uint v=Word(b,slot);if(v==0)continue;if(v<module.Native||v>=next.Native)throw new InvalidDataException("Structure placement stock pointer escapes its module.");Put(b,slot,v-(uint)module.Native);relos.Add((uint)slot);}
        var native=new Chunk{InstanceBuffer=b,RelocationBuffer=Pack(relos),ImportsBuffer=Array.Empty<byte>()};
        var sample=new XmlDocument();sample.LoadXml("<StructurePlacementBehavior id='ModuleTag_StructurePlacementBehavior'><RestrictedAreaShape Type='BOX' MajorRadius='30.0' MinorRadius='45.0' Height='5.0'><Offset x='90.0' y='0.0' z='0.0'/></RestrictedAreaShape></StructurePlacementBehavior>");
        var generated=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,schemas,"StructurePlacementBehaviorModuleData",typeof(StructurePlacementBehaviorModuleData));
        Verify(sample.DocumentElement!,native,generated);
        for(int at=0;at<MetadataSize;at+=12){uint target=Word(captured.Bytes,module.Wrapper+at+8);if(target!=0)target=target>=module.Native?target-(uint)module.Native+MetadataSize:target-(uint)module.Wrapper;
            if(Word(generated.InstanceBuffer,at)!=Word(captured.Bytes,module.Wrapper+at)||Word(generated.InstanceBuffer,at+4)!=Word(captured.Bytes,module.Wrapper+at+4)||Word(generated.InstanceBuffer,at+8)!=target)throw new InvalidDataException("Structure placement stock field/table target differs at metadata +"+at);}
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove every header byte, both optional strings, the optional Offset pointee, coverage and relocations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata)
    {
        Validate(owner);var b=native.InstanceBuffer;var covered=new bool[b.Length];var relos=new HashSet<uint>();var area=Area(owner);
        if(b.Length<HeaderSize||Word(b,0)!=TypeId||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("EP1 structure placement identity/header differs.");
        Claim(0,HeaderSize);if(b[Shape+46]!=0||b[Shape+47]!=0)throw new InvalidDataException("EP1 structure placement padding differs.");
        foreach(var f in ShapeFields)if(!b.AsSpan(Shape+f.Offset,f.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(area,f)))throw new InvalidDataException("EP1 structure placement shape field differs: "+f.Name);
        foreach(var t in Texts){int at=Shape+t.Offset;uint length=Word(b,at),pointer=Word(b,at+4);
            if(!area.HasAttribute(t.Name)){if(length!=0||pointer!=0)throw new InvalidDataException("Absent shape "+t.Name+" has storage.");continue;}
            byte[] text=Encoding.ASCII.GetBytes(area.GetAttribute(t.Name));if(length!=text.Length||pointer<HeaderSize||pointer>b.Length-text.Length-1)throw new InvalidDataException("EP1 structure placement "+t.Name+" header differs.");
            Claim((int)pointer,text.Length+1);relos.Add((uint)(at+4));if(!b.AsSpan((int)pointer,text.Length).SequenceEqual(text)||b[pointer+text.Length]!=0)throw new InvalidDataException("EP1 structure placement "+t.Name+" text differs.");}
        var offset=Offset(area);uint vector=Word(b,Shape+40);
        if(offset==null){if(vector!=0)throw new InvalidDataException("Absent shape Offset has storage.");}
        else{if(vector<HeaderSize||vector%4!=0||vector>b.Length-12)throw new InvalidDataException("EP1 structure placement Offset pointer differs.");Claim((int)vector,12);relos.Add((uint)(Shape+40));for(int i=0;i<3;i++)if(!b.AsSpan((int)vector+i*4,4).SequenceEqual(ConscriptModuleScalarContract.Bytes(offset,Axis(i))))throw new InvalidDataException("EP1 structure placement Offset."+Axes[i]+" differs.");}
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("EP1 structure placement unowned native allocation at +"+i);}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||native.ImportsBuffer.Length!=0)throw new InvalidDataException("EP1 structure placement relocation/import inventory differs.");
        byte[] expected=Metadata(b);if(metadata.InstanceBuffer.Length-b.Length!=MetadataSize||!metadata.InstanceBuffer.AsSpan(0,MetadataSize).SequenceEqual(expected))throw new InvalidDataException("EP1 structure placement generated/literal metadata differ.");
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("EP1 structure placement allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    private static byte[] Metadata(byte[] n)
    {
        const uint P=MetadataSize;var b=new byte[MetadataSize];
        Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("BehaviorModuleData"),48);Row(24,FastHash.GetHashCode("RestrictedAreaShape"),0xE7210555u,108);
        Row(48,1,FastHash.GetHashCode("ModuleData"),72);Row(72,FastHash.GetHashCode("TypeId"),0x23E650B8u,P);Row(84,FastHash.GetHashCode("id"),0xBDF07965u,P+4);
        uint S=P+(uint)Shape;uint vector=Word(n,Shape+40);
        var rows=new (string,uint,uint)[]{("Type",0xB0D49164u,S),("ContactPointGeneration",0x465508CEu,S+4),("Name",0x1A2933AAu,S+8),("Active",0x77E3F4F7u,S+44),("UsedForHealthBox",0x77E3F4F7u,S+45),("MajorRadius",0x645864F3u,S+16),("MinorRadius",0x645864F3u,S+20),("Other",0x1A2933AAu,S+24),("Height",0x645864F3u,S+32),("FrontAngle",0x4E16EA4Du,S+36),("Offset",0x29368077u,vector==0?0:P+vector)};
        for(int i=0;i<rows.Length;i++)Row(108+i*12,FastHash.GetHashCode(rows[i].Item1),rows[i].Item2,rows[i].Item3);
        return b;
        void Row(int at,uint name,uint type,uint target){Put(b,at,name);Put(b,at+4,type);Put(b,at+8,target);}
    }

    private static byte[] Pack(IEnumerable<uint> slots){var list=slots.ToList();if(list.Count==0)return Array.Empty<byte>();var b=new byte[list.Count*4+4];for(int i=0;i<list.Count;i++)Put(b,i*4,list[i]);Put(b,list.Count*4,uint.MaxValue);return b;}
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("EP1 structure placement auxiliary terminator differs.");return Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();}
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
