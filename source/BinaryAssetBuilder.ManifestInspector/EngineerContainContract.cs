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

// Reborn: EP1 engineer contract — every header word, voice hash, strong FX/EVA/OCL import, status mask and CanEnterFilter record is
// proved from source/catalog, with exact allocation, relocation and import ownership and a literal metadata writer qualified on stock.
internal static class EngineerContainContract
{
    private const uint TypeId=0x0FF760E0u;
    internal const int HeaderSize=92;
    private const int FilterSize=120;
    private static readonly (string Name,int Slot)[] Voices={("NameOfVoiceToUseForFriendlyEnter",12),("NameOfVoiceToUseForHostileEnter",16)};
    private static readonly (string Name,int Slot)[] Fx={("FXForRepair",52),("FXForCapture",56),("FXForReplace",60),("FXForCaptureAndReplace",64)};
    private static readonly (string Name,int Slot)[] Eva={("EvaEventForRepair",68),("EvaEventForCapture",72),("EvaEventForReplace",76),("EvaEventForCaptureAndReplace",80)};
    private static readonly string[] MaskedAttributes={"ReplaceWith","NameOfVoiceToUseForFriendlyEnter","NameOfVoiceToUseForHostileEnter","ObjectStatusToSet","FXForRepair","FXForCapture","FXForReplace","FXForCaptureAndReplace","EvaEventForRepair","EvaEventForCapture","EvaEventForReplace","EvaEventForCaptureAndReplace"};

    // Reborn: inline/record codecs; KindOf/status/relationship masks reuse the shared flag encoder with their own enum ordinals.
    private static ConscriptModuleScalarContract.Binding Status(string name,int offset,string? fallback)=>new(name,offset,32,"Flags:SageBinaryData.ObjectStatusType",fallback,0xA7BD811Eu);
    private static readonly ConscriptModuleScalarContract.Binding[] FilterFields={
        new("Rule",4,4,"Enum:SageBinaryData.ObjectFilterRule","NONE",0x0027BB58u),
        new("Relationship",8,4,"Flags:SageBinaryData.ObjectFilterRelationship","",0x5C8981A9u),
        new("Alignment",12,4,"Enum:SageBinaryData.ObjectFilterAlignment","NONE",0x9FFD43EBu),
        new("Include",16,40,"Flags:SageBinaryData.KindOf",null,0x9AFC9193u),
        new("Exclude",56,40,"Flags:SageBinaryData.KindOf",null,0x9AFC9193u)};

    // Reborn: blank optional names and the schema's empty EVA default are absence, matching the EP1 marshaler.
    private static string? Text(XmlElement e,string name){string value=e.GetAttribute(name).Trim();return value.Length==0?null:value;}
    private static XmlElement? Filter(XmlElement owner)=>owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="CanEnterFilter");
    private static string[] Things(XmlElement filter,string name)=>filter.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name).Select(e=>e.InnerText.Trim()).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: representability and unimplemented filter identity are rejected at their own source position. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        _=ConscriptModuleScalarContract.Bytes(owner,Status("ObjectStatusToSet",0,"NO_REFUND"));
        var filter=Filter(owner);if(filter==null)return;
        foreach(var field in FilterFields)_=ConscriptModuleScalarContract.Bytes(filter,field);
        foreach(string name in new[]{"StatusBitFlags","StatusBitFlagsExclude"})if(filter.HasAttribute(name))_=ConscriptModuleScalarContract.Bytes(filter,Status(name,0,null));
        var id=filter.GetAttributeNode("id");if(id!=null&&id.Value.Trim().Length!=0)throw SdkXmlDiagnostics.At(new InvalidDataException("EngineerContain CanEnterFilter id is not implemented yet."),id);
        foreach(var thing in filter.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "IncludeThing" or "ExcludeThing"))if(thing.InnerText.Trim().Length==0)throw SdkXmlDiagnostics.At(new InvalidDataException("EngineerContain filter thing name is empty."),thing);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison copies hide authored references/strings/status and the whole filter; Verify proves them independently. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(string name in MaskedAttributes){actual.RemoveAttribute(name);if(model.HasAttribute(name))actual.SetAttribute(name,model.GetAttribute(name));}
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="CanEnterFilter").ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="CanEnterFilter"))actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify ABI, inheritance, the complete stock payload (with real FX/EVA imports and three filter things) and metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="EngineerContainModuleData");var next=modules[Array.IndexOf(modules,module)+1];
        if(Marshal.SizeOf<Ep1EngineerContainModuleData>()!=HeaderSize||Marshal.SizeOf<ObjectFilter>()!=FilterSize||module.TypeId!=TypeId||!module.ClassChain.SequenceEqual(new[]{"EngineerContainModuleData","ContainModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 engineer stock ABI/inheritance differs.");
        var b=captured.Bytes.AsSpan(module.Native,next.Native-module.Native).ToArray();var relos=new List<uint>();var imports=new List<uint>();
        // Reborn: rebase only real pointer words in the isolated copy; inventories are rebuilt from the stock payload itself.
        void Rebase(int at){uint v=Word(b,at);if(v==0)return;if(v<module.Native||v>=next.Native)throw new InvalidDataException("Engineer stock pointer escapes its module.");Put(b,at,v-(uint)module.Native);relos.Add((uint)at);}
        foreach(var v in Voices)Rebase(v.Slot);foreach(var f in Fx)Rebase(f.Slot);Rebase(84);int filter=(int)Word(b,84);
        foreach(int at in new[]{filter+96,filter+100,filter+108,filter+116})Rebase(at);
        if(Word(b,8)!=0)imports.Add(8);foreach(var f in Fx){uint p=Word(b,f.Slot);if(p!=0)imports.Add(p);}foreach(var e in Eva)if(Word(b,e.Slot)!=0)imports.Add((uint)e.Slot);
        var native=new Chunk{InstanceBuffer=b,RelocationBuffer=Pack(relos),ImportsBuffer=Pack(imports)};
        var sample=new XmlDocument();sample.LoadXml("<EngineerContain id='ModuleTag_EngineerContain' NameOfVoiceToUseForFriendlyEnter='VoiceEngineerRepair' NameOfVoiceToUseForHostileEnter='VoiceCaptureBuilding' EvaEventForRepair='BuildingRepaired' EvaEventForCapture='' FXForRepair='FX_Building_Repaired' FXForCapture='FX_Building_Captured' FXForCaptureAndReplace='FX_Building_Captured' ImmediatelyEnabled='true'><CanEnterFilter Rule='NONE'><IncludeThing>SovietEngineer</IncludeThing><IncludeThing>JapanEngineer</IncludeThing><IncludeThing>AlliedEngineer</IncludeThing></CanEnterFilter></EngineerContain>");
        var generated=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,schemas,"EngineerContainModuleData",typeof(Ep1EngineerContainModuleData));
        Verify(sample.DocumentElement!,native,generated,captured.Asset.References.ToArray(),schemas,catalog);
        int prefix=generated.InstanceBuffer.Length-b.Length;if(next.Wrapper-module.Wrapper!=prefix)throw new InvalidDataException("Engineer stock metadata extent differs.");
        for(int at=0;at<prefix;at+=12){uint target=Word(captured.Bytes,module.Wrapper+at+8);if(target!=0)target=target>=module.Native?target-(uint)module.Native+(uint)prefix:target-(uint)module.Wrapper;
            if(Word(generated.InstanceBuffer,at)!=Word(captured.Bytes,module.Wrapper+at)||Word(generated.InstanceBuffer,at+4)!=Word(captured.Bytes,module.Wrapper+at+4)||Word(generated.InstanceBuffer,at+8)!=target)throw new InvalidDataException("Engineer stock field/table target differs at metadata +"+at);}
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the authored engineer graph byte by byte, including allocation coverage and the exact auxiliary inventories. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata,AssetId[] references,XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        Validate(owner);var b=native.InstanceBuffer;var covered=new bool[b.Length];var relos=new HashSet<uint>();var imports=new HashSet<uint>();
        if(b.Length<HeaderSize||Word(b,0)!=TypeId||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("EP1 engineer identity/header differs.");
        Claim(0,HeaderSize);if(b[89]!=0||b[90]!=0||b[91]!=0)throw new InvalidDataException("EP1 engineer padding differs.");
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(Ep1EngineerContainModuleData)))Check(owner,binding,0);
        Check(owner,new("ImmediatelyEnabled",88,1,"SageBool","false",0x77E3F4F7u),0);Check(owner,Status("ObjectStatusToSet",20,"NO_REFUND"),0);
        Strong(8,Text(owner,"ReplaceWith"),"ObjectCreationList",owner.GetAttributeNode("ReplaceWith"));
        foreach(var v in Voices){string? text=Text(owner,v.Name);int at=Pointer(v.Slot,text!=null,4);if(text!=null&&Word(b,at)!=FastHash.GetHashCode(text))throw SdkXmlDiagnostics.At(new InvalidDataException("EP1 engineer voice hash differs: "+v.Name),owner.GetAttributeNode(v.Name));}
        foreach(var f in Fx){string? text=Text(owner,f.Name);int at=Pointer(f.Slot,text!=null,4);if(text!=null)Strong(at,text,"FXList",owner.GetAttributeNode(f.Name));}
        foreach(var e in Eva)Strong(e.Slot,Text(owner,e.Name),"EvaEvent",owner.GetAttributeNode(e.Name));
        var filter=Filter(owner);int f0=Pointer(84,filter!=null,FilterSize);
        if(filter!=null){
            if(Word(b,f0)!=0)throw new InvalidDataException("EP1 engineer filter id storage differs.");
            foreach(var field in FilterFields)Check(filter,field,f0);
            foreach(var s in new[]{("StatusBitFlags",96),("StatusBitFlagsExclude",100)}){bool present=filter.HasAttribute(s.Item1);int at=Pointer(f0+s.Item2,present,32);if(present)Check(filter,Status(s.Item1,0,null),at);}
            foreach(var l in new[]{("IncludeThing",104),("ExcludeThing",112)}){var names=Things(filter,l.Item1);if(Word(b,f0+l.Item2)!=names.Length)throw new InvalidDataException("EP1 engineer filter list count differs: "+l.Item1);int items=Pointer(f0+l.Item2+4,names.Length!=0,names.Length*4);for(int i=0;i<names.Length;i++)if(Word(b,items+i*4)!=FastHash.GetHashCode(names[i].ToLowerInvariant()))throw new InvalidDataException("EP1 engineer filter weak id differs: "+names[i]);}
        }
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("EP1 engineer unowned native allocation at +"+i);}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||!Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order()))throw new InvalidDataException("EP1 engineer relocation/import inventory differs.");
        byte[] expected=Metadata(b);if(metadata.InstanceBuffer.Length-b.Length!=expected.Length||!metadata.InstanceBuffer.AsSpan(0,expected.Length).SequenceEqual(expected))throw new InvalidDataException("EP1 engineer generated/literal metadata differ.");

        // Reborn: source codecs are independent of the native marshaler.
        void Check(XmlElement element,ConscriptModuleScalarContract.Binding field,int start){if(!b.AsSpan(start+field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,field)))throw new InvalidDataException("EP1 engineer authored field differs: "+element.LocalName+"."+field.Name);}
        // Reborn: optional pointees exist exactly when authored and are disjoint, bounded allocations.
        int Pointer(int at,bool present,int size){uint target=Word(b,at);if(!present){if(target!=0)throw new InvalidDataException("Absent EP1 engineer field has storage at +"+at);return 0;}if(target<HeaderSize||target>int.MaxValue||target%4!=0)throw new InvalidDataException("EP1 engineer pointer differs at +"+at);Claim((int)target,size);relos.Add((uint)at);return (int)target;}
        // Reborn: strong selectors must name one schema-compatible catalog asset at its exact Core reference index.
        void Strong(int at,string? name,string required,XmlNode? node)
        {
            uint selector=Word(b,at);if(name==null){if(selector!=0)throw new InvalidDataException("Absent EP1 engineer reference has an import at +"+at);return;}
            int colon=name.IndexOf(':');string? explicitType=colon<0?null:name[..colon];string identity=colon<0?name:name[(colon+1)..];uint id=InstanceHandle.GetInstanceId(identity);
            var baseType=schemas.GlobalTypes[new XmlQualifiedName(required,SchemaSet.XmlNamespace)] as XmlSchemaType;
            var candidates=catalog.Where(a=>a.InstanceId==id).Where(a=>{string type=a.Name.Split(':')[0];var concrete=schemas.GlobalTypes[new XmlQualifiedName(type,SchemaSet.XmlNamespace)] as XmlSchemaType;return (explicitType==null||explicitType==type)&&baseType!=null&&concrete!=null&&(concrete==baseType||XmlSchemaType.IsDerivedFrom(concrete,baseType,XmlSchemaDerivationMethod.None));}).Select(a=>a.TypeId).Distinct().ToArray();
            if(candidates.Length!=1||selector==0||selector>references.Length||references[(int)selector-1].TypeId!=candidates[0]||references[(int)selector-1].InstanceId!=id)throw SdkXmlDiagnostics.At(new InvalidDataException("EP1 engineer "+required+" target is missing/ambiguous/incompatible or has a different Core index: "+name),node??owner);
            imports.Add((uint)at);
        }
        // Reborn: overlapping or out-of-range owners are refused before payload comparison.
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("EP1 engineer allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal schema-ordered metadata; pointee targets come from the already ownership-checked native pointers. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(byte[] n)
    {
        int f=(int)Word(n,84);int ni=f==0?0:(int)Word(n,f+104),ne=f==0?0:(int)Word(n,f+112);int size=f==0?288:420+12*(1+ni)+12*(1+ne);uint P=(uint)size;var b=new byte[size];
        uint Ptr(int slot)=>Word(n,slot)==0?0:P+Word(n,slot);
        Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("ContainModuleData"),204);
        var rows=new List<(string,uint,uint)>{("ImmediatelyEnabled",0x77E3F4F7u,P+88),("ReplaceWith",0x42E4E9D1u,P+8),(Voices[0].Name,0x58BEC727u,Ptr(12)),(Voices[1].Name,0x58BEC727u,Ptr(16)),("ObjectStatusToSet",0xA7BD811Eu,P+20)};
        foreach(var x in Fx)rows.Add((x.Name,0x0D1C8E83u,Ptr(x.Slot)));foreach(var x in Eva)rows.Add((x.Name,0xC4EC43B1u,P+(uint)x.Slot));rows.Add(("CanEnterFilter",0xE46EA3B2u,f==0?0u:288u));
        for(int i=0;i<rows.Count;i++)Row(24+i*12,FastHash.GetHashCode(rows[i].Item1),rows[i].Item2,rows[i].Item3);
        Row(204,1,FastHash.GetHashCode("BehaviorModuleData"),228);Row(228,1,FastHash.GetHashCode("ModuleData"),252);Row(252,FastHash.GetHashCode("TypeId"),0x23E650B8u,P);Row(264,FastHash.GetHashCode("id"),0xBDF07965u,P+4);
        if(f!=0){
            uint F=(uint)f;var filter=new (string,uint,uint)[]{("id",0x4D3B3F19u,P+F),("Rule",0x0027BB58u,P+F+4),("Relationship",0x5C8981A9u,P+F+8),("Alignment",0x9FFD43EBu,P+F+12),("Include",0x9AFC9193u,P+F+16),("Exclude",0x9AFC9193u,P+F+56),("StatusBitFlags",0xA7BD811Eu,Ptr(f+96)),("StatusBitFlagsExclude",0xA7BD811Eu,Ptr(f+100)),("IncludeThing",0x9B19049Fu,420u),("ExcludeThing",0x9B19049Fu,(uint)(420+12*(1+ni)))};
            for(int i=0;i<filter.Length;i++)Row(288+i*12,FastHash.GetHashCode(filter[i].Item1),filter[i].Item2,filter[i].Item3);
            int at=420;foreach(var (count,slot) in new[]{(ni,f+104),(ne,f+112)}){Row(at,0x6C827073u,0x5D448FD8u,P+(uint)slot);uint items=Word(n,slot+4);for(int k=0;k<count;k++)Row(at+12+k*12,0xB151A0EBu,0,P+items+(uint)(k*4));at+=12*(1+count);}
        }
        return b;
        void Row(int at,uint name,uint type,uint target){Put(b,at,name);Put(b,at+4,type);Put(b,at+8,target);}
    }

    // Reborn: canonical slot list with the relocation sentinel.
    private static byte[] Pack(IEnumerable<uint> slots){var list=slots.ToList();if(list.Count==0)return Array.Empty<byte>();var b=new byte[list.Count*4+4];for(int i=0;i<list.Count;i++)Put(b,i*4,list[i]);Put(b,list.Count*4,uint.MaxValue);return b;}
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("EP1 engineer auxiliary terminator differs.");return Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();}
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
