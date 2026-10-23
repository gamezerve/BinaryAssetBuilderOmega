using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 repair record checked against stock SovietRefinery ABI/metadata; inline percent records are authored, self-repair audio stays empty.
internal static class RepairUpdateContract
{
    internal const int NativeSize=44;
    internal const int MetadataSize=312;
    private const uint TypeId=0x0A2B8743u;
    private static readonly string[] Percents={"Pristine","Damaged","ReallyDamaged","Rubble"};
    private static readonly string[] Audio={"SelfRepairFromDamageLoop","SelfRepairFromRubbleLoop"};

    // Reborn: schema Percentage default 50 encodes 0.5; offsets are the stock inline record coordinates.
    private static ConscriptModuleScalarContract.Binding PercentBinding(int index)=>new(Percents[index],16+index*4,4,"Percentage","50",0x645864F3u);
    private static XmlElement Child(XmlElement owner,string name)=>owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==name)??throw SdkXmlDiagnostics.At(new InvalidDataException("RepairUpdate requires "+name),owner);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: percent values must be representable; populated self-repair audio needs its own import contract and is refused. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        var percent=Child(owner,"PercentOfBuildCostToRebuildInfo");for(int i=0;i<Percents.Length;i++)_=ConscriptModuleScalarContract.Bytes(percent,PercentBinding(i));
        var audio=Child(owner,"SelfAudio");foreach(string name in Audio){var value=audio.GetAttributeNode(name);if(value!=null&&value.Value.Trim().Length!=0)throw SdkXmlDiagnostics.At(new InvalidDataException("RepairUpdate SelfAudio."+name+" is not implemented yet; leave it empty."),value);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison copies mask only nested percent/audio attributes; native output is checked against the authored values. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var (record,names) in new[]{("PercentOfBuildCostToRebuildInfo",Percents),("SelfAudio",Audio)})
        {
            var element=actual.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==record);var basis=model.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==record);if(element==null||basis==null)continue;
            foreach(string name in names){element.RemoveAttribute(name);if(basis.HasAttribute(name))element.SetAttribute(name,basis.GetAttribute(name));}
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact stock ABI, inheritance, extents, literal metadata (including anonymous Owner::Element record tags) and default values. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="RepairUpdateModuleData");int index=Array.IndexOf(modules,module);
        if(Marshal.SizeOf<Ep1RepairUpdateModuleData>()!=NativeSize||module.TypeId!=TypeId||!module.ClassChain.SequenceEqual(new[]{"RepairUpdateModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 repair stock ABI/inheritance differs.");
        if(index+1<modules.Length&&(modules[index+1].Native-module.Native!=NativeSize||modules[index+1].Wrapper-module.Wrapper!=MetadataSize))throw new InvalidDataException("EP1 repair stock native/metadata extent differs.");
        var b=captured.Bytes;var actual=b.AsSpan(module.Wrapper,MetadataSize).ToArray();
        for(int at=8;at<MetadataSize;at+=12){uint target=Word(actual,at);if(target!=0)BinaryPrimitives.WriteUInt32LittleEndian(actual.AsSpan(at,4),target>=module.Native?target-(uint)module.Native+MetadataSize:target-(uint)module.Wrapper);}
        if(!actual.SequenceEqual(Metadata()))throw new InvalidDataException("EP1 repair literal metadata/native field anchors differ from stock.");
        var sample=new XmlDocument();sample.LoadXml("<RepairUpdate id='ModuleTag_RepairUpdate'><PercentOfBuildCostToRebuildInfo/><SelfAudio/></RepairUpdate>");
        CheckValues(sample.DocumentElement!,b.AsSpan(module.Native,NativeSize).ToArray());return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: all 44 native and 312 metadata bytes are owned; the admitted form has no imports or allocations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata)
    {
        Validate(owner);var b=native.InstanceBuffer;
        if(b.Length!=NativeSize||Word(b,0)!=TypeId||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant())||native.RelocationBuffer.Length!=0||native.ImportsBuffer.Length!=0)throw new InvalidDataException("EP1 repair identity/native allocation/auxiliary framing differs.");
        CheckValues(owner,b);if(metadata.InstanceBuffer.Length-b.Length!=MetadataSize||!metadata.InstanceBuffer.AsSpan(0,MetadataSize).SequenceEqual(Metadata()))throw new InvalidDataException("EP1 repair generated/literal metadata differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own scalars use the shared module codecs, inline percents their own bindings, and audio/padding must stay zero. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckValues(XmlElement owner,byte[] b)
    {
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(Ep1RepairUpdateModuleData)))if(!b.AsSpan(binding.Offset,binding.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(owner,binding)))throw new InvalidDataException("EP1 repair scalar differs: "+binding.Name);
        var percent=Child(owner,"PercentOfBuildCostToRebuildInfo");for(int i=0;i<Percents.Length;i++){var binding=PercentBinding(i);if(!b.AsSpan(binding.Offset,4).SequenceEqual(ConscriptModuleScalarContract.Bytes(percent,binding)))throw new InvalidDataException("EP1 repair percent differs: "+binding.Name);}
        if(Word(b,32)!=0||Word(b,36)!=0||b[42]!=0||b[43]!=0)throw new InvalidDataException("EP1 repair audio/padding storage is populated.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal own/base metadata plus the two anonymous inline record tables after the base chain. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata()
    {
        const int prefix=MetadataSize;var b=new byte[MetadataSize];
        Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("UpdateModuleData"),108);
        Row(24,FastHash.GetHashCode("Toggleable"),0x77E3F4F7u,prefix+40);Row(36,FastHash.GetHashCode("RepairableWhenDead"),0x77E3F4F7u,prefix+41);
        Row(48,FastHash.GetHashCode("RepopThreshold"),0x645864F3u,prefix+8);Row(60,FastHash.GetHashCode("HealAmountPerSecond"),0x763177A4u,prefix+12);
        Row(72,FastHash.GetHashCode("PercentOfBuildCostToRebuildInfo"),FastHash.GetHashCode("RepairUpdateModuleData::PercentOfBuildCostToRebuildInfo"),216);
        Row(84,FastHash.GetHashCode("SelfAudio"),FastHash.GetHashCode("RepairUpdateModuleData::SelfAudio"),276);
        Row(108,1,FastHash.GetHashCode("ContainModuleData"),132);Row(132,1,FastHash.GetHashCode("BehaviorModuleData"),156);Row(156,1,FastHash.GetHashCode("ModuleData"),180);
        Row(180,FastHash.GetHashCode("TypeId"),0x23E650B8u,prefix);Row(192,FastHash.GetHashCode("id"),0xBDF07965u,prefix+4);
        for(int i=0;i<Percents.Length;i++)Row(216+i*12,FastHash.GetHashCode(Percents[i]),0x645864F3u,(uint)(prefix+16+i*4));
        Row(276,FastHash.GetHashCode(Audio[0]),0x63D7B468u,prefix+32);Row(288,FastHash.GetHashCode(Audio[1]),0x63D7B468u,prefix+36);return b;
        // Reborn: metadata-internal and native targets keep distinct coordinate spaces.
        void Row(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+8,4),target);}
    }
    // Reborn: callers prove fixed bounds before reading words.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
