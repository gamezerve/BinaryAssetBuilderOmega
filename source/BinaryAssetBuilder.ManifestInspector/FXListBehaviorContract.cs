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

// Reborn: source-sized FX events and complete death conditions are independent of the original Conscript's particular nested values.
internal static class FXListBehaviorContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate all nested scalars, flags and ASCII list labels before compilation, preserving authored diagnostic positions. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        var die=owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="DieMuxData")??throw SdkXmlDiagnostics.At(new InvalidDataException("FXListBehavior requires DieMuxData."),owner);
        foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))_=ConscriptModuleScalarContract.Bytes(die,field);
        foreach(string name in new[]{"ExemptStatus","RequiredStatus"})if(die.HasAttribute(name))_=Status(die,name);
        if(Words(die.GetAttribute("VeterancyLevels")).Any(s=>s.Any(c=>c>127||c=='\0')))throw SdkXmlDiagnostics.At(new InvalidDataException("FXListBehavior veterancy labels must be ASCII without NUL."),die.GetAttributeNode("VeterancyLevels"));
        var events=Events(owner);if(events.Length==0)throw SdkXmlDiagnostics.At(new InvalidDataException("FXListBehavior requires at least one Event."),owner);
        foreach(var item in events)foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(FXListBehaviorEvent))){_=ConscriptModuleScalarContract.Bytes(item,field);if(field.Codec=="SageBool"&&item.HasAttribute(field.Name))item.SetAttribute(field.Name,item.GetAttribute(field.Name).Trim() is "true" or "1"?"true":"false");}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: erase nested differences only in comparison copies; Verify consumes every actual source value, occurrence and allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>())actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently compare all three original FX graphs and literal metadata against stock before authoring new nested shapes. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet official,SchemaSet schemas,XmlDocument baseline,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<FXListBehaviorModuleData>()!=60||Marshal.SizeOf<FXListBehaviorEvent>()!=24)throw new InvalidDataException("FX behavior native ABI differs.");
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var source=new XmlDocument();source.LoadXml(baseline.OuterXml);source.Schemas.Add(official);source.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));ConscriptBehaviorSurvey.ResolveStrong(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source.OuterXml);
        var originals=baseline.GetElementsByTagName("FXListBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray();if(originals.Length!=3)throw new InvalidDataException("FX stock witness inventory differs.");
        foreach(var original in originals){
            var compiled=instance.XmlNode.SelectSingleNode("*[local-name()='Behaviors']/*[@id='"+original.GetAttribute("id")+"']")??throw new InvalidDataException("Normalized FX stock witness missing.");
            int index=Array.FindIndex(modules,m=>m.Type=="FXListBehaviorModuleData"&&Word(stock.Bytes,m.Native+4)==FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant()));if(index<0)throw new InvalidDataException("FX stock identity missing.");var module=modules[index];
            var native=ConscriptBehaviorSurvey.Compile(compiled,module.TypeId);var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),native,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true);comparer.Compare(typeof(FXListBehaviorModuleData),module.Native);
            var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,"FXListBehaviorModuleData",typeof(FXListBehaviorModuleData));
            BehaviorMetadataStockProof.Compare(metadata,native,stock.Bytes,module.Wrapper,modules[index+1].Wrapper,typeof(FXListBehaviorModuleData),comparer.NativeRecords);
            Verify(original,native,metadata,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray(),catalog);
        }
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own every header/list/optional record/string and bind strong FX imports to independently resolved source identities. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata,AssetId[] references,ManifestAsset[] catalog)
    {
        Validate(owner);var b=native.InstanceBuffer;var events=Events(owner);if(b.Length<60||Word(b,0)!=FastHash.GetHashCode("FXListBehaviorModuleData")||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("FX behavior identity/header differs.");
        var covered=new bool[b.Length];var relos=new HashSet<uint>();var imports=new HashSet<uint>();Claim(0,60);
        var die=owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="DieMuxData");foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))Check(die,field,8);
        foreach(var field in new[]{("ExemptStatus",16),("RequiredStatus",20)}){bool present=die.HasAttribute(field.Item1);int target=Pointer(field.Item2,present,32);if(present&&!b.AsSpan(target,32).SequenceEqual(Status(die,field.Item1)))throw new InvalidDataException("FX death status differs.");}
        var labels=Words(die.GetAttribute("VeterancyLevels"));int veterans=List(8,labels.Length,8);
        for(int i=0;i<labels.Length;i++){var text=Encoding.ASCII.GetBytes(labels[i]);int at=veterans+i*8;if(Word(b,at)!=text.Length)throw new InvalidDataException("FX veterancy length differs.");int target=Pointer(at+4,true,text.Length+1);if(!b.AsSpan(target,text.Length).SequenceEqual(text)||b[target+text.Length]!=0)throw new InvalidDataException("FX veterancy text differs.");}
        int array=List(52,events.Length,24);
        for(int i=0;i<events.Length;i++){
            var item=events[i];int at=array+i*24;foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(FXListBehaviorEvent)))Check(item,field,at);if(b[at+22]!=0||b[at+23]!=0)throw new InvalidDataException("FX event padding differs.");
            string value=item.GetAttribute("FX");uint selector=Word(b,at+4);if(value.Length==0){if(selector!=0)throw new InvalidDataException("Absent event FX has an import.");}
            else{int colon=value.IndexOf(':');if(colon>=0){if(value[..colon]!="FXList")throw SdkXmlDiagnostics.At(new InvalidDataException("Event FX must reference FXList."),item.GetAttributeNode("FX"));value=value[(colon+1)..];}uint id=InstanceHandle.GetInstanceId(value),type=FastHash.GetHashCode("FXList");if(catalog.Count(a=>a.TypeId==type&&a.InstanceId==id)!=1||selector==0||selector>references.Length||references[(int)selector-1]!=new AssetId(type,id))throw SdkXmlDiagnostics.At(new InvalidDataException("Missing/ambiguous FXList or incorrect event import: "+value),item.GetAttributeNode("FX"));imports.Add((uint)(at+4));}
        }
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("FX behavior unowned allocation.");}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||!Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order()))throw new InvalidDataException("FX behavior auxiliary ownership differs.");
        var literal=Metadata(b);if(metadata.InstanceBuffer.Length-b.Length!=literal.Length||!metadata.InstanceBuffer.AsSpan(0,literal.Length).SequenceEqual(literal))throw new InvalidDataException("FX generic/literal metadata differs.");
        // Reborn: nested scalar widths/defaults are independently parsed from source, not borrowed from emitted memory.
        void Check(XmlElement element,ConscriptModuleScalarContract.Binding field,int start){if(!b.AsSpan(start+field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,field)))throw new InvalidDataException("FX authored field differs: "+element.LocalName+"."+field.Name);}
        // Reborn: optional pointer presence owns exactly its bounded native allocation and one relocation slot.
        int Pointer(int at,bool present,int size){uint target=Word(b,at);if(!present){if(target!=0)throw new InvalidDataException("Absent FX optional field has storage.");return 0;}if(target<60||target>int.MaxValue)throw new InvalidDataException("FX pointer differs.");Claim((int)target,size);relos.Add((uint)at);return (int)target;}
        // Reborn: source cardinality determines exact array storage without a sample-based count limit.
        int List(int at,int count,int stride){if(Word(b,at)!=count)throw new InvalidDataException("FX list count differs.");return Pointer(at+4,count!=0,checked(count*stride));}
        // Reborn: every native allocation is bounded and disjoint, including inline headers and string terminators.
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("FX allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal schema-order metadata independently covers variable event/veterancy counts, optional status pointers and all concrete field targets. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(byte[] native)
    {
        var rows=new List<uint>();var targets=new List<(int At,int Native)>();int wrapper=Allocate(1),module=Allocate(3),die=Allocate(3),behavior=Allocate(2),baseModule=Allocate(3),mux=Allocate(9);
        Row(wrapper,2,FastHash.GetHashCode("FXListBehaviorModuleData"),(uint)module);Row(module,1,FastHash.GetHashCode("DieModuleData"),(uint)die);Row(die,1,FastHash.GetHashCode("BehaviorModuleData"),(uint)behavior);Row(die+12,FastHash.GetHashCode("DieMuxData"),0x0084001B,(uint)mux);Row(behavior,1,FastHash.GetHashCode("ModuleData"),(uint)baseModule);
        Field(baseModule,"TypeId",0x23E650B8,0);Field(baseModule+12,"id",0xBDF07965,4);
        int labels=checked((int)Word(native,8)),strings=Allocate(1+labels);Row(mux,FastHash.GetHashCode("VeterancyLevels"),0x6D66AE18,(uint)strings);ListHeader(strings,8,labels);
        for(int i=0;i<labels;i++)Native(strings+20+i*12,checked((int)Word(native,12))+i*8);
        foreach(var f in new[]{("ExemptStatus",16,12),("RequiredStatus",20,24)}){Row(mux+f.Item3,FastHash.GetHashCode(f.Item1),0xA7BD811E,0);uint pointer=Word(native,f.Item2);if(pointer!=0)Native(mux+f.Item3+8,(int)pointer);}
        string[] scalar={"DamageAmountRequired","MinKillerAngle","MaxKillerAngle","DeathTypes","DeathTypesForbidden"};uint[] tags={0x645864F3,0x4E16EA4D,0x4E16EA4D,0x034DF1A1,0x034DF1A1};int[] offsets={24,28,32,36,44};for(int i=0;i<scalar.Length;i++)Field(mux+36+i*12,scalar[i],tags[i],offsets[i]);
        int count=checked((int)Word(native,52)),list=Allocate(1+count);Row(module+12,FastHash.GetHashCode("Event"),0x65BE8011,(uint)list);ListHeader(list,52,count);
        string[] names={"Index","FX","MinThreshold","MaxThreshold","OrientToObject","ForceUseDoFXObj","Direction"};uint[] types={0x03AF8D2F,0x0D1C8E83,0x645864F3,0x645864F3,0x77E3F4F7,0x77E3F4F7,0x5E151008};int[] fields={0,4,8,12,20,21,16};
        for(int i=0;i<count;i++){int table=Allocate(8),start=checked((int)Word(native,56))+i*24;rows[(list+20+i*12)/4]=(uint)table;for(int f=0;f<names.Length;f++)Field(table+f*12,names[f],types[f],start+fields[f]);}
        var result=new byte[rows.Count*4];for(int i=0;i<rows.Count;i++)BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(i*4),rows[i]);foreach(var target in targets)BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(target.At),checked((uint)(result.Length+target.Native)));return result;
        // Reborn: each table reserves its own explicit zero terminator.
        int Allocate(int count){int at=rows.Count*4;for(int i=0;i<count*3;i++)rows.Add(0);return at;}
        // Reborn: token-table targets stay unshifted; native addresses are patched after the final prefix length is known.
        void Row(int at,uint name,uint type,uint target){rows[at/4]=name;rows[at/4+1]=type;rows[at/4+2]=target;}
        // Reborn: native-target metadata has a separate origin from internal table offsets.
        void Native(int at,int target)=>targets.Add((at,target));
        // Reborn: primitive records target their own native slots, including zero/absent strong reference words.
        void Field(int at,string name,uint type,int target){Row(at,FastHash.GetHashCode(name),type,0);Native(at+8,target);}
        // Reborn: list markers are metadata records, whereas their header target binds the native count word.
        void ListHeader(int at,int slot,int count){Row(at,0x6C827073,0x5D448FD8,0);Native(at+8,slot);for(int i=0;i<count;i++)Row(at+12+i*12,0xB151A0EB,0,0);}
    }
    // Reborn: event order and multiplicity come directly from the authored source.
    private static XmlElement[] Events(XmlElement owner)=>owner.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Event").ToArray();
    // Reborn: optional death-status masks share the existing engine bit ordinal codec.
    private static byte[] Status(XmlElement owner,string name)=>ConscriptModuleScalarContract.Bytes(owner,new(name,0,32,"Flags:SageBinaryData.ObjectStatusType",null,0xA7BD811E));
    // Reborn: XML whitespace separates veterancy labels, not their native character contents.
    private static string[] Words(string text)=>text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);
    // Reborn: retain a canonical sentinel and reject duplicate relocation/import entries.
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8||b.Length%4!=0||Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("FX auxiliary sentinel differs.");var values=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(values.Distinct().Count()!=values.Length)throw new InvalidDataException("FX duplicate auxiliary slot.");return values;}
    // Reborn: all callers bound fixed headers and claimed records before reading words.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
