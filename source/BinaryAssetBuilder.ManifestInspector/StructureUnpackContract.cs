using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently qualify the EP1 unpack inheritance, source-bound time/height and optional concrete audio import.
internal static class StructureUnpackContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: optional sound presence/identity is checked against catalog and Core indices, not frozen to the refinery's sound. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        actual.RemoveAttribute("UnpackCompleteSound");if(model.HasAttribute("UnpackCompleteSound"))actual.SetAttribute("UnpackCompleteSound",model.GetAttribute("UnpackCompleteSound"));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock supplies the full inheritance chain, nondefault delay, optional audio allocation and all 204 literal metadata bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="StructureUnpackUpdateModuleData");var next=modules[Array.IndexOf(modules,module)+1];
        if(Marshal.SizeOf<Ep1StructureUnpackUpdateModuleData>()!=20||module.TypeId!=0x977F8391u||next.Native-module.Native!=24||next.Wrapper-module.Wrapper!=204||!module.ClassChain.SequenceEqual(new[]{"StructureUnpackUpdateModuleData","GenericUnpackUpdateModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 StructureUnpack stock ABI differs.");
        var b=captured.Bytes.AsSpan(module.Native,24).ToArray();Put(b,12,Word(b,12)-(uint)module.Native);
        var native=new Chunk{InstanceBuffer=b,RelocationBuffer=Pack(12),ImportsBuffer=Pack(20)};
        var sample=new XmlDocument();sample.LoadXml("<StructureUnpackUpdate id='ModuleTag_StructureUnpackUpdate' UnpackTime='20s' UnpackCompleteSound='SOV_OreCollector_VoiceCreateFromRefinery'/>");
        CheckValues(sample.DocumentElement!,native,captured.Asset.References.ToArray(),schemas,catalog);
        byte[] expected=Metadata(true);for(int at=0;at<204;at+=12){uint target=Word(captured.Bytes,module.Wrapper+at+8);if(target!=0)target=target>=module.Native?target-(uint)module.Native+204:target-(uint)module.Wrapper;
            if(Word(expected,at)!=Word(captured.Bytes,module.Wrapper+at)||Word(expected,at+4)!=Word(captured.Bytes,module.Wrapper+at+4)||Word(expected,at+8)!=target)throw new InvalidDataException("Unpack stock metadata field/target differs at +"+at);
        }
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove exact native and metadata ownership for both sound-present and sound-omitted authored modules. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata,AssetId[] references,XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        CheckValues(owner,native,references,schemas,catalog);byte[] expected=Metadata(owner.HasAttribute("UnpackCompleteSound"));
        if(metadata.InstanceBuffer.Length-native.InstanceBuffer.Length!=204||!metadata.InstanceBuffer.AsSpan(0,204).SequenceEqual(expected))throw new InvalidDataException("Unpack generated/literal metadata differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent source codecs and concrete audio identity validate every byte without copying stock values into authored output. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckValues(XmlElement owner,Chunk native,AssetId[] references,XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        var b=native.InstanceBuffer;bool sound=owner.HasAttribute("UnpackCompleteSound");if(b.Length!=(sound?24:20)||Word(b,0)!=0x977F8391u||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant())||Word(b,12)!=(sound?20u:0u))throw new InvalidDataException("Unpack native identity/optional audio framing differs.");
        foreach(var field in ConscriptModuleScalarContract.Bindings(typeof(Ep1StructureUnpackUpdateModuleData)))if(!b.AsSpan(field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(owner,field)))throw new InvalidDataException("Unpack authored field differs: "+field.Name);
        if(sound){
            string name=owner.GetAttribute("UnpackCompleteSound");int colon=name.IndexOf(':');string? explicitType=colon<0?null:name[..colon];string identity=colon<0?name:name[(colon+1)..];uint id=InstanceHandle.GetInstanceId(identity);
            var required=schemas.GlobalTypes[new XmlQualifiedName("BaseAudioEventInfo",SchemaSet.XmlNamespace)] as XmlSchemaType;
            var candidates=catalog.Where(a=>a.InstanceId==id).Where(a=>{
                string type=a.Name.Split(':')[0];var concrete=schemas.GlobalTypes[new XmlQualifiedName(type,SchemaSet.XmlNamespace)] as XmlSchemaType;
                return (explicitType==null||explicitType==type)&&required!=null&&concrete!=null&&XmlSchemaType.IsDerivedFrom(concrete,required,XmlSchemaDerivationMethod.None);
            }).ToArray();
            uint selector=Word(b,20);if(candidates.Length!=1||selector==0||selector>references.Length||references[(int)selector-1].TypeId!=candidates[0].TypeId||references[(int)selector-1].InstanceId!=id)throw SdkXmlDiagnostics.At(new InvalidDataException("Unpack audio target is missing/ambiguous/incompatible or has a different Core index: "+name),owner.GetAttributeNode("UnpackCompleteSound"));
        }
        if(!native.RelocationBuffer.SequenceEqual(sound?Pack(12):Array.Empty<byte>())||!native.ImportsBuffer.SequenceEqual(sound?Pack(20):Array.Empty<byte>()))throw new InvalidDataException("Unpack exact relocation/import inventory differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently authored literal metadata preserves the Generic/Update/Contain base chain and optional pointee target. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(bool sound)
    {
        var b=new byte[204];Row(0,2,0x977F8391,12);Row(12,1,FastHash.GetHashCode("GenericUnpackUpdateModuleData"),36);
        Row(36,1,FastHash.GetHashCode("UpdateModuleData"),96);Row(48,FastHash.GetHashCode("UnpackTime"),0x998669E8,212);Row(60,FastHash.GetHashCode("UnpackCompleteSound"),0x63D7B468,sound?224u:0u);Row(72,FastHash.GetHashCode("OffsetHeightAboveWater"),0x645864F3,220);
        Row(96,1,FastHash.GetHashCode("ContainModuleData"),120);Row(120,1,FastHash.GetHashCode("BehaviorModuleData"),144);Row(144,1,FastHash.GetHashCode("ModuleData"),168);Row(168,FastHash.GetHashCode("TypeId"),0x23E650B8,204);Row(180,FastHash.GetHashCode("id"),0xBDF07965,208);return b;
        // Reborn: zero terminators are retained between each independent own/base table.
        void Row(int at,uint name,uint type,uint target){Put(b,at,name);Put(b,at+4,type);Put(b,at+8,target);}
    }
    // Reborn: one owned pointer/import source uses the canonical relocation sentinel, not a synthetic empty record.
    private static byte[] Pack(uint slot){var b=new byte[8];Put(b,0,slot);Put(b,4,uint.MaxValue);return b;}
    // Reborn: callers validate the fixed header extent before native word reads.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    // Reborn: writes affect only literal metadata or an isolated copied stock slice.
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
