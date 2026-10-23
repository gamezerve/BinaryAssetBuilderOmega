using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: CreateObjectDie and DestroyDie share complete authored death-condition storage, while creation owns explicit OCL/upgrade imports.
internal static class DeathBehaviorContract
{
    // Reborn: only these real DieModuleData families use this contract; similarly named fields gain no implicit admission.
    internal static bool Supports(Type type)=>type==typeof(Ep1CreateObjectDieModuleData)||type==typeof(DestroyDieModuleData);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check nested scalar/status/veterancy source before allocation and retain exact node diagnostics for invalid values. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        var die=owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="DieMuxData")??throw SdkXmlDiagnostics.At(new InvalidDataException(owner.LocalName+" requires DieMuxData."),owner);
        foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))_=ConscriptModuleScalarContract.Bytes(die,field);
        foreach(string name in new[]{"ExemptStatus","RequiredStatus"})if(die.HasAttribute(name))_=Status(die,name);
        if(Words(die.GetAttribute("VeterancyLevels")).Any(s=>s.Any(c=>c>127||c=='\0')))throw SdkXmlDiagnostics.At(new InvalidDataException("Death veterancy labels must be ASCII without NUL."),die.GetAttributeNode("VeterancyLevels"));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison-only masks do not replace actual OCL names, list rows or death conditions passed to native compilation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        actual.RemoveAttribute("CreationList");if(model.HasAttribute("CreationList"))actual.SetAttribute("CreationList",model.GetAttribute("CreationList"));
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>())actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently recompile every original creation/destroy witness and compare complete stock native graphs and metadata before flexible composition. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet official,SchemaSet schemas,XmlDocument baseline,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1CreateObjectDieModuleData>()!=72||Marshal.SizeOf<DestroyDieModuleData>()!=52||Marshal.SizeOf<DieMuxDataType>()!=44)throw new InvalidDataException("Death behavior ABI differs.");
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var source=new XmlDocument();source.LoadXml(baseline.OuterXml);source.Schemas.Add(official);source.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));ConscriptBehaviorSurvey.ResolveStrong(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source.OuterXml);
        var originals=ConscriptModuleScalarContract.Owners(baseline).Where(o=>Supports(o.NativeType)).ToArray();if(originals.Length!=3)throw new InvalidDataException("Death stock witness inventory differs.");
        foreach(var owner in originals){
            var original=owner.Element;int index=Array.FindIndex(modules,m=>Word(stock.Bytes,m.Native+4)==FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant()));if(index<0)throw new InvalidDataException("Death stock module identity missing.");var module=modules[index];
            var compiled=instance.XmlNode.SelectSingleNode("*[local-name()='Behaviors']/*[@id='"+original.GetAttribute("id")+"']")??throw new InvalidDataException("Normalized death witness missing.");var native=ConscriptBehaviorSurvey.Compile(compiled,module.TypeId);
            var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),native,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true);comparer.Compare(owner.NativeType,module.Native);
            var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,module.Type,owner.NativeType);BehaviorMetadataStockProof.Compare(metadata,native,stock.Bytes,module.Wrapper,modules[index+1].Wrapper,owner.NativeType,comparer.NativeRecords);
            Verify(original,native,metadata,instance.ReferencedInstances.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToArray(),catalog);
        }
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source controls all death words, optional masks/strings, strong creation target and ordered strong upgrade arrays; every allocation is owned. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata,AssetId[] references,ManifestAsset[] catalog)
    {
        Validate(owner);bool create=owner.LocalName=="CreateObjectDie";int header=create?72:52;string wire=create?"CreateObjectDieModuleData":"DestroyDieModuleData";var b=native.InstanceBuffer;
        if(b.Length<header||Word(b,0)!=FastHash.GetHashCode(wire)||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("Death behavior identity/header differs.");
        var covered=new bool[b.Length];var relos=new HashSet<uint>();var imports=new HashSet<uint>();Claim(0,header);
        var die=owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="DieMuxData");foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))if(!b.AsSpan(8+field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(die,field)))throw new InvalidDataException("Death authored field differs: "+field.Name);
        foreach(var field in new[]{("ExemptStatus",16),("RequiredStatus",20)}){bool present=die.HasAttribute(field.Item1);int at=Pointer(field.Item2,present,32);if(present&&!b.AsSpan(at,32).SequenceEqual(Status(die,field.Item1)))throw new InvalidDataException("Death status mask differs.");}
        var labels=Words(die.GetAttribute("VeterancyLevels"));int array=List(8,labels.Length,8);
        for(int i=0;i<labels.Length;i++){byte[] text=Encoding.ASCII.GetBytes(labels[i]);int at=array+i*8;if(Word(b,at)!=text.Length)throw new InvalidDataException("Death veterancy length differs.");int target=Pointer(at+4,true,text.Length+1);if(!b.AsSpan(target,text.Length).SequenceEqual(text)||b[target+text.Length]!=0)throw new InvalidDataException("Death veterancy text differs.");}
        if(create){
            Reference(owner.GetAttribute("CreationList"),"ObjectCreationList",52,(XmlNode?)owner.GetAttributeNode("CreationList")??owner);
            foreach(var field in new[]{("UpgradeRequired",56),("UpgradeForbidden",64)}){var items=owner.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==field.Item1).ToArray();int at=List(field.Item2,items.Length,4);for(int i=0;i<items.Length;i++)Reference(items[i].InnerText.Trim(),"UpgradeTemplate",at+i*4,items[i]);}
        }
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("Death unowned native storage.");}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||!Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order()))throw new InvalidDataException("Death auxiliary ownership differs.");var literal=Metadata(b,create);if(metadata.InstanceBuffer.Length-b.Length!=literal.Length||!metadata.InstanceBuffer.AsSpan(0,literal.Length).SequenceEqual(literal))throw new InvalidDataException("Death generic/literal metadata differs.");
        // Reborn: import selectors must identify a unique compatible source target, not merely land inside the Core occurrence table.
        void Reference(string value,string typeName,int at,XmlNode location){uint selector=Word(b,at);if(value.Length==0){if(selector!=0)throw new InvalidDataException("Absent death reference has import.");return;}int colon=value.IndexOf(':');if(colon>=0){if(value[..colon]!=typeName)throw SdkXmlDiagnostics.At(new InvalidDataException("Expected "+typeName+" death reference."),location);value=value[(colon+1)..];}uint id=InstanceHandle.GetInstanceId(value),type=FastHash.GetHashCode(typeName);if(catalog.Count(a=>a.TypeId==type&&a.InstanceId==id)!=1||selector==0||selector>references.Length||references[(int)selector-1]!=new AssetId(type,id))throw SdkXmlDiagnostics.At(new InvalidDataException("Missing/ambiguous "+typeName+" or incorrect death import: "+value),location);imports.Add((uint)at);}
        // Reborn: optional allocations have exact source presence and disjoint bounds; strings need not be word-aligned.
        int Pointer(int at,bool present,int size){uint target=Word(b,at);if(!present){if(target!=0)throw new InvalidDataException("Absent death field has storage.");return 0;}if(target<header||target>int.MaxValue)throw new InvalidDataException("Death pointer differs.");Claim((int)target,size);relos.Add((uint)at);return (int)target;}
        // Reborn: source cardinality controls checked allocation size, not a captured sample count.
        int List(int at,int count,int stride){if(Word(b,at)!=count)throw new InvalidDataException("Death list count differs.");return Pointer(at+4,count!=0,checked(count*stride));}
        // Reborn: no overlapping records, hidden native bytes or orphan arrays are accepted.
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("Death allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent literal metadata covers both concrete families, full DieMux and source-sized strong upgrade/veterancy lists. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(byte[] b,bool create)
    {
        var rows=new List<uint>();var targets=new List<(int At,int Target)>();int wrapper=Allocate(1),owner=Allocate(create?5:2),die=Allocate(3),behavior=Allocate(2),module=Allocate(3),mux=Allocate(9);
        Row(wrapper,2,FastHash.GetHashCode(create?"CreateObjectDieModuleData":"DestroyDieModuleData"),(uint)owner);Row(owner,1,FastHash.GetHashCode("DieModuleData"),(uint)die);Row(die,1,FastHash.GetHashCode("BehaviorModuleData"),(uint)behavior);Row(die+12,FastHash.GetHashCode("DieMuxData"),0x0084001B,(uint)mux);Row(behavior,1,FastHash.GetHashCode("ModuleData"),(uint)module);Field(module,"TypeId",0x23E650B8,0);Field(module+12,"id",0xBDF07965,4);
        int count=checked((int)Word(b,8)),list=Allocate(1+count);Row(mux,FastHash.GetHashCode("VeterancyLevels"),0x6D66AE18,(uint)list);ListHeader(list,8,count);for(int i=0;i<count;i++)Native(list+20+i*12,checked((int)Word(b,12))+i*8);
        foreach(var field in new[]{("ExemptStatus",16,12),("RequiredStatus",20,24)}){Row(mux+field.Item3,FastHash.GetHashCode(field.Item1),0xA7BD811E,0);uint target=Word(b,field.Item2);if(target!=0)Native(mux+field.Item3+8,(int)target);}
        string[] names={"DamageAmountRequired","MinKillerAngle","MaxKillerAngle","DeathTypes","DeathTypesForbidden"};uint[] types={0x645864F3,0x4E16EA4D,0x4E16EA4D,0x034DF1A1,0x034DF1A1};int[] offsets={24,28,32,36,44};for(int i=0;i<names.Length;i++)Field(mux+36+i*12,names[i],types[i],offsets[i]);
        if(create){Field(owner+12,"CreationList",0x42E4E9D1,52);foreach(var field in new[]{("UpgradeRequired",56,24),("UpgradeForbidden",64,36)}){int length=checked((int)Word(b,field.Item2)),table=Allocate(1+length);Row(owner+field.Item3,FastHash.GetHashCode(field.Item1),0x4ABA1F29,(uint)table);ListHeader(table,field.Item2,length);for(int i=0;i<length;i++)Native(table+20+i*12,checked((int)Word(b,field.Item2+4))+i*4);}}
        var result=new byte[rows.Count*4];for(int i=0;i<rows.Count;i++)BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(i*4),rows[i]);foreach(var target in targets)BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(target.At),checked((uint)(result.Length+target.Target)));return result;
        // Reborn: all own/base tables reserve explicit zero terminators.
        int Allocate(int count){int at=rows.Count*4;for(int i=0;i<count*3;i++)rows.Add(0);return at;}
        // Reborn: metadata-table addresses stay in prefix coordinates.
        void Row(int at,uint name,uint type,uint target){rows[at/4]=name;rows[at/4+1]=type;rows[at/4+2]=target;}
        // Reborn: native field/header addresses shift exactly once after prefix allocation completes.
        void Native(int at,int target)=>targets.Add((at,target));
        // Reborn: primitive/import fields target native slots even when their values are zero.
        void Field(int at,string name,uint type,int target){Row(at,FastHash.GetHashCode(name),type,0);Native(at+8,target);}
        // Reborn: list markers bind native row slots; count headers bind their native count words.
        void ListHeader(int at,int slot,int length){Row(at,0x6C827073,0x5D448FD8,0);Native(at+8,slot);for(int i=0;i<length;i++)Row(at+12+i*12,0xB151A0EB,0,0);}
    }
    // Reborn: optional death masks use the shared engine bit-ordinal codec and retain absence versus authored empty storage.
    private static byte[] Status(XmlElement owner,string name)=>ConscriptModuleScalarContract.Bytes(owner,new(name,0,32,"Flags:SageBinaryData.ObjectStatusType",null,0xA7BD811E));
    // Reborn: XML list whitespace separates veterancy labels without storing indentation as character data.
    private static string[] Words(string text)=>text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);
    // Reborn: exact auxiliary lists have canonical sentinels and unique source slots.
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8||b.Length%4!=0||Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Death auxiliary sentinel differs.");var values=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(values.Distinct().Count()!=values.Length)throw new InvalidDataException("Duplicate death auxiliary slot.");return values;}
    // Reborn: callers bound native records before reading words.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
