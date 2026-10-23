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

// Reborn: the recovered 136-byte infiltrator record matches stock SovietRefinery; scalars, status masks, voice hashes, weak/strong
// references (including schema-default names) and the two optional filter-reference pointees are proved from source and catalog.
internal static class InfiltratorContainContract
{
    private const uint TypeId=0x5047272Eu;
    internal const int HeaderSize=136;
    internal const int MetadataSize=336;
    private static readonly (string Name,int Offset,string? Default,string Type)[] Strong={("CanEnterFilter",44,"InfiltrationCanEnterObjectFilter","ObjectFilterAsset"),("ReplaceWith",48,null,"ObjectCreationList"),("EvaEventForInfiltratingPlayer",60,"EnemyBuildingInfiltrated","EvaEvent"),("EvaEventForInfiltratedPlayer",64,"OurBuildingInfiltrated","EvaEvent"),("FXForInfiltrate",68,"FX_Building_Infiltrated_Generic","FXList")};
    private static readonly (string Name,int Offset,string Default)[] Voices={("NameOfVoiceToUseForHostileEnter",52,"VoiceInfiltrate"),("NameOfVoiceToUseForInfiltrationDeath",56,"VoiceInfiltrateDeath")};
    private static readonly (string Name,int Slot)[] Filters={("UnitFilter",92),("StructureFilter",96)};
    private static readonly string[] Masked={"InfiltratedStatusFilter","CanEnterFilter","ReplaceWith","NameOfVoiceToUseForHostileEnter","NameOfVoiceToUseForInfiltrationDeath","EvaEventForInfiltratingPlayer","EvaEventForInfiltratedPlayer","FXForInfiltrate","ObjectRef","UnitFilter","StructureFilter","ObjectStatusToSet"};
    private static ConscriptModuleScalarContract.Binding Status(string name,int offset,string fallback)=>new(name,offset,32,"Flags:SageBinaryData.ObjectStatusType",fallback,0xA7BD811Eu);
    private static readonly ConscriptModuleScalarContract.Binding[] Statuses={Status("InfiltratedStatusFilter",12,"DESTROYED UNDER_CONSTRUCTION SOLD UNPACKING"),Status("ObjectStatusToSet",100,"NO_REFUND")};
    // Reborn: an absent attribute uses its schema default; an explicit blank value means no reference.
    private static string? Name(XmlElement e,string attribute,string? fallback){if(!e.HasAttribute(attribute))return fallback;string value=e.GetAttribute(attribute).Trim();return value.Length==0?null:value;}

    internal static void Validate(XmlElement owner){foreach(var s in Statuses)_=ConscriptModuleScalarContract.Bytes(owner,s);}

    internal static void Mask(XmlElement actual,XmlElement model){foreach(string name in Masked){actual.RemoveAttribute(name);if(model.HasAttribute(name))actual.SetAttribute(name,model.GetAttribute(name));}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify ABI, inheritance, the full stock payload (default-named imports and voice hashes) and metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="InfiltratorContainModuleData");var next=modules[Array.IndexOf(modules,module)+1];
        if(Marshal.SizeOf<InfiltratorContainModuleData>()!=HeaderSize||module.TypeId!=TypeId||next.Wrapper-module.Wrapper!=MetadataSize||!module.ClassChain.SequenceEqual(new[]{"InfiltratorContainModuleData","ContainModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 infiltrator stock ABI/inheritance differs.");
        var b=captured.Bytes.AsSpan(module.Native,next.Native-module.Native).ToArray();var relos=new List<uint>();var imports=new List<uint>();
        foreach(var f in Filters){uint v=Word(b,f.Slot);if(v!=0){Put(b,f.Slot,v-(uint)module.Native);relos.Add((uint)f.Slot);imports.Add(v-(uint)module.Native);}}
        foreach(var s in Strong)if(Word(b,s.Offset)!=0)imports.Add((uint)s.Offset);
        var native=new Chunk{InstanceBuffer=b,RelocationBuffer=Pack(relos),ImportsBuffer=Pack(imports)};
        var sample=new XmlDocument();sample.LoadXml("<InfiltratorContain id='ModuleTag_InfiltratorContain' Effect='STEAL_MONEY' Amount='2000' BlockedDuration='30s'/>");
        var generated=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,schemas,"InfiltratorContainModuleData",typeof(InfiltratorContainModuleData));
        Verify(sample.DocumentElement!,native,generated,captured.Asset.References.ToArray(),schemas,catalog);
        for(int at=0;at<MetadataSize;at+=12){uint target=Word(captured.Bytes,module.Wrapper+at+8);if(target!=0)target=target>=module.Native?target-(uint)module.Native+MetadataSize:target-(uint)module.Wrapper;
            if(Word(generated.InstanceBuffer,at)!=Word(captured.Bytes,module.Wrapper+at)||Word(generated.InstanceBuffer,at+4)!=Word(captured.Bytes,module.Wrapper+at+4)||Word(generated.InstanceBuffer,at+8)!=target)throw new InvalidDataException("Infiltrator stock field/table target differs at metadata +"+at);}
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove every header word, optional filter pointee, allocation coverage, auxiliary inventories and literal metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata,AssetId[] references,XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        Validate(owner);var b=native.InstanceBuffer;var covered=new bool[b.Length];var relos=new HashSet<uint>();var imports=new HashSet<uint>();
        if(b.Length<HeaderSize||Word(b,0)!=TypeId||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("EP1 infiltrator identity/header differs.");
        Claim(0,HeaderSize);if(b[133]!=0||b[134]!=0||b[135]!=0)throw new InvalidDataException("EP1 infiltrator padding differs.");
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(InfiltratorContainModuleData)))Check(owner,binding,0);
        foreach(var s in Statuses)Check(owner,s,0);
        foreach(var v in Voices){string? text=Name(owner,v.Name,v.Default);if(Word(b,v.Offset)!=(text==null?0u:FastHash.GetHashCode(text)))throw SdkXmlDiagnostics.At(new InvalidDataException("EP1 infiltrator voice hash differs: "+v.Name),(XmlNode?)owner.GetAttributeNode(v.Name)??owner);}
        foreach(var s in Strong)Resolve(s.Offset,Name(owner,s.Name,s.Default),s.Type,owner.GetAttributeNode(s.Name));
        string? weak=Name(owner,"ObjectRef",null);if(Word(b,88)!=(weak==null?0u:FastHash.GetHashCode(weak.ToLowerInvariant())))throw new InvalidDataException("EP1 infiltrator ObjectRef weak id differs.");
        foreach(var f in Filters){string? name=Name(owner,f.Name,null);uint target=Word(b,f.Slot);if(name==null){if(target!=0)throw new InvalidDataException("Absent EP1 infiltrator filter has storage.");continue;}if(target<HeaderSize||target%4!=0||target>b.Length-4)throw new InvalidDataException("EP1 infiltrator filter pointer differs.");Claim((int)target,4);relos.Add((uint)f.Slot);Resolve((int)target,name,"ObjectFilterAsset",owner.GetAttributeNode(f.Name));}
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("EP1 infiltrator unowned native allocation at +"+i);}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||!Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order()))throw new InvalidDataException("EP1 infiltrator relocation/import inventory differs.");
        byte[] expected=Metadata(b);if(metadata.InstanceBuffer.Length-b.Length!=MetadataSize||!metadata.InstanceBuffer.AsSpan(0,MetadataSize).SequenceEqual(expected))throw new InvalidDataException("EP1 infiltrator generated/literal metadata differ.");

        void Check(XmlElement element,ConscriptModuleScalarContract.Binding field,int start){if(!b.AsSpan(start+field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,field)))throw new InvalidDataException("EP1 infiltrator authored field differs: "+field.Name);}
        void Resolve(int at,string? name,string required,XmlNode? node)
        {
            uint selector=Word(b,at);if(name==null){if(selector!=0)throw new InvalidDataException("Absent EP1 infiltrator reference has an import at +"+at);return;}
            int colon=name.IndexOf(':');string? explicitType=colon<0?null:name[..colon];string identity=colon<0?name:name[(colon+1)..];uint id=InstanceHandle.GetInstanceId(identity);
            var baseType=schemas.GlobalTypes[new XmlQualifiedName(required,SchemaSet.XmlNamespace)] as XmlSchemaType;
            var candidates=catalog.Where(a=>a.InstanceId==id).Where(a=>{string type=a.Name.Split(':')[0];var concrete=schemas.GlobalTypes[new XmlQualifiedName(type,SchemaSet.XmlNamespace)] as XmlSchemaType;return (explicitType==null||explicitType==type)&&baseType!=null&&concrete!=null&&(concrete==baseType||XmlSchemaType.IsDerivedFrom(concrete,baseType,XmlSchemaDerivationMethod.None));}).Select(a=>a.TypeId).Distinct().ToArray();
            if(candidates.Length!=1||selector==0||selector>references.Length||references[(int)selector-1].TypeId!=candidates[0]||references[(int)selector-1].InstanceId!=id)throw SdkXmlDiagnostics.At(new InvalidDataException("EP1 infiltrator "+required+" target is missing/ambiguous/incompatible or has a different Core index: "+name),node??owner);
            imports.Add((uint)at);
        }
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("EP1 infiltrator allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal schema-ordered metadata; only the two optional filter rows target pointees. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(byte[] n)
    {
        const uint P=MetadataSize;var b=new byte[MetadataSize];uint Ptr(int slot)=>Word(n,slot)==0?0:P+Word(n,slot);
        Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("ContainModuleData"),252);
        var rows=new (string,uint,uint)[]{("BlockedDuration",0x998669E8u,P+8),("InfiltratedStatusFilter",0xA7BD811Eu,P+12),("CanEnterFilter",0x1E592B76u,P+44),("ImmediatelyEnabled",0x77E3F4F7u,P+132),("ReplaceWith",0x42E4E9D1u,P+48),
            ("NameOfVoiceToUseForHostileEnter",0x58BEC727u,P+52),("NameOfVoiceToUseForInfiltrationDeath",0x58BEC727u,P+56),("EvaEventForInfiltratingPlayer",0xC4EC43B1u,P+60),("EvaEventForInfiltratedPlayer",0xC4EC43B1u,P+64),("FXForInfiltrate",0x0D1C8E83u,P+68),
            ("Duration",0x998669E8u,P+72),("UnitDuration",0x998669E8u,P+76),("Amount",0x645864F3u,P+80),("Effect",0xD0CE7F56u,P+84),("ObjectRef",0x148ED75Du,P+88),("UnitFilter",0x1E592B76u,Ptr(92)),("StructureFilter",0x1E592B76u,Ptr(96)),("ObjectStatusToSet",0xA7BD811Eu,P+100)};
        for(int i=0;i<rows.Length;i++)Row(24+i*12,FastHash.GetHashCode(rows[i].Item1),rows[i].Item2,rows[i].Item3);
        Row(252,1,FastHash.GetHashCode("BehaviorModuleData"),276);Row(276,1,FastHash.GetHashCode("ModuleData"),300);Row(300,FastHash.GetHashCode("TypeId"),0x23E650B8u,P);Row(312,FastHash.GetHashCode("id"),0xBDF07965u,P+4);
        return b;
        void Row(int at,uint name,uint type,uint target){Put(b,at,name);Put(b,at+4,type);Put(b,at+8,target);}
    }

    private static byte[] Pack(IEnumerable<uint> slots){var list=slots.ToList();if(list.Count==0)return Array.Empty<byte>();var b=new byte[list.Count*4+4];for(int i=0;i<list.Count;i++)Put(b,i*4,list[i]);Put(b,list.Count*4,uint.MaxValue);return b;}
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("EP1 infiltrator auxiliary terminator differs.");return Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();}
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
