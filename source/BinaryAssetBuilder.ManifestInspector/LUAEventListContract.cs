using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 LUAEventList — a 16-byte BehaviorModuleData record carrying one owned ASCII EventListName string. The SDK only
// serializes the name; it does not resolve or execute LUA callbacks. Layout and metadata are qualified against stock SovietRefinery.
internal static class LUAEventListContract
{
    private const uint TypeId=0xFA6AC8B6u;
    internal const int HeaderSize=16;
    internal const int MetadataSize=108;
    private const int Text=8;

    private static string Name(XmlElement owner)
    {
        if(!owner.HasAttribute("EventListName"))throw SdkXmlDiagnostics.At(new InvalidDataException("LUAEventList requires EventListName."),owner);
        string value=owner.GetAttribute("EventListName");var node=owner.GetAttributeNode("EventListName");
        if(value.Length==0)throw SdkXmlDiagnostics.At(new InvalidDataException("LUAEventList.EventListName must not be empty."),node);
        if(value.Any(c=>c>127||c<32))throw SdkXmlDiagnostics.At(new InvalidDataException("LUAEventList.EventListName must be printable ASCII."),node);
        return value;
    }

    internal static void Validate(XmlElement owner)
    {
        _=Name(owner);
        var child=owner.ChildNodes.OfType<XmlElement>().FirstOrDefault();if(child!=null)throw SdkXmlDiagnostics.At(new InvalidDataException("LUAEventList has no child elements."),child);
    }

    internal static void Mask(XmlElement actual,XmlElement model){actual.RemoveAttribute("EventListName");if(model.HasAttribute("EventListName"))actual.SetAttribute("EventListName",model.GetAttribute("EventListName"));}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify ABI, inheritance, the stock "BuildingPowerFunctions" payload and metadata (LUAEventList is the last stock Behaviors module). */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet schemas)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="LUAEventListBehaviorModuleData");int index=Array.IndexOf(modules,module);
        if(Marshal.SizeOf<LUAEventListBehaviorModuleData>()!=HeaderSize||module.TypeId!=TypeId||!module.ClassChain.SequenceEqual(new[]{"LUAEventListBehaviorModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 LUAEventList stock ABI/inheritance differs.");
        uint length=Word(captured.Bytes,module.Native+Text);int size=Align(HeaderSize+(int)length+1);
        if(index+1<modules.Length&&(modules[index+1].Wrapper-module.Wrapper!=MetadataSize||modules[index+1].Native-module.Native!=size))throw new InvalidDataException("EP1 LUAEventList stock module extent differs.");
        for(int at=MetadataSize-12;at<MetadataSize;at++)if(captured.Bytes[module.Wrapper+at]!=0)throw new InvalidDataException("EP1 LUAEventList stock metadata terminator differs.");
        var b=captured.Bytes.AsSpan(module.Native,size).ToArray();uint pointer=Word(b,Text+4);
        if(pointer<module.Native||pointer>=module.Native+size)throw new InvalidDataException("LUAEventList stock pointer escapes its module.");Put(b,Text+4,pointer-(uint)module.Native);
        var native=new Chunk{InstanceBuffer=b,RelocationBuffer=Pack(new uint[]{Text+4}),ImportsBuffer=Array.Empty<byte>()};
        var sample=new XmlDocument();sample.LoadXml("<LUAEventList id='ModuleTag_LUAEventList' EventListName='BuildingPowerFunctions'/>");
        var generated=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,schemas,"LUAEventListBehaviorModuleData",typeof(LUAEventListBehaviorModuleData));
        Verify(sample.DocumentElement!,native,generated);
        for(int at=0;at<MetadataSize;at+=12){uint target=Word(captured.Bytes,module.Wrapper+at+8);if(target!=0)target=target>=module.Native?target-(uint)module.Native+MetadataSize:target-(uint)module.Wrapper;
            if(Word(generated.InstanceBuffer,at)!=Word(captured.Bytes,module.Wrapper+at)||Word(generated.InstanceBuffer,at+4)!=Word(captured.Bytes,module.Wrapper+at+4)||Word(generated.InstanceBuffer,at+8)!=target)throw new InvalidDataException("LUAEventList stock field/table target differs at metadata +"+at);}
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove identity, the owned name string and its terminator, allocation coverage, the single relocation and metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata)
    {
        Validate(owner);var b=native.InstanceBuffer;var covered=new bool[b.Length];
        if(b.Length<HeaderSize||Word(b,0)!=TypeId||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("EP1 LUAEventList identity/header differs.");
        Claim(0,HeaderSize);byte[] text=Encoding.ASCII.GetBytes(Name(owner));uint length=Word(b,Text),pointer=Word(b,Text+4);
        if(length!=text.Length||pointer<HeaderSize||pointer>b.Length-text.Length-1)throw new InvalidDataException("EP1 LUAEventList EventListName header differs.");
        Claim((int)pointer,text.Length+1);if(!b.AsSpan((int)pointer,text.Length).SequenceEqual(text)||b[pointer+text.Length]!=0)throw new InvalidDataException("EP1 LUAEventList EventListName text differs.");
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("EP1 LUAEventList unowned native allocation at +"+i);}
        if(!Slots(native.RelocationBuffer).SequenceEqual(new uint[]{Text+4})||native.ImportsBuffer.Length!=0)throw new InvalidDataException("EP1 LUAEventList relocation/import inventory differs.");
        byte[] expected=Metadata();if(metadata.InstanceBuffer.Length-b.Length!=MetadataSize||!metadata.InstanceBuffer.AsSpan(0,MetadataSize).SequenceEqual(expected))throw new InvalidDataException("EP1 LUAEventList generated/literal metadata differ.");
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("EP1 LUAEventList allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    private static byte[] Metadata()
    {
        const uint P=MetadataSize;var b=new byte[MetadataSize];
        Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("BehaviorModuleData"),48);Row(24,FastHash.GetHashCode("EventListName"),0x1A2933AAu,P+Text);
        Row(48,1,FastHash.GetHashCode("ModuleData"),72);Row(72,FastHash.GetHashCode("TypeId"),0x23E650B8u,P);Row(84,FastHash.GetHashCode("id"),0xBDF07965u,P+4);
        return b;
        void Row(int at,uint name,uint type,uint target){Put(b,at,name);Put(b,at+4,type);Put(b,at+8,target);}
    }

    private static int Align(int value)=>(value+3)&~3;
    private static byte[] Pack(IEnumerable<uint> slots){var list=slots.ToList();if(list.Count==0)return Array.Empty<byte>();var b=new byte[list.Count*4+4];for(int i=0;i<list.Count;i++)Put(b,i*4,list[i]);Put(b,list.Count*4,uint.MaxValue);return b;}
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("EP1 LUAEventList auxiliary terminator differs.");return Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();}
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
