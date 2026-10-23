using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: authored tracker colors, ordered effect records and optional status masks no longer require the Conscript's nested shape.
internal static class SecondaryDamageTrackerContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: required nested values, options and boolean spellings are validated at their source nodes before the existing writer allocates storage. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        foreach(var child in owner.ChildNodes.OfType<XmlElement>()){
            Type? type=child.LocalName switch{"Color"=>typeof(RGBColor),"EffectInfo"=>typeof(EffectInfoType),"ObjectStatusValidation"=>typeof(ObjectStatusValidationDataType),_=>null};if(type==null)continue;
            foreach(string required in child.LocalName=="Color"?new[]{"R","G","B"}:child.LocalName=="EffectInfo"?new[]{"Fraction"}:Array.Empty<string>())if(!child.HasAttribute(required))throw SdkXmlDiagnostics.At(new InvalidDataException(child.LocalName+" requires @"+required),child);
            foreach(var field in ConscriptModuleScalarContract.RecordBindings(type)){_=ConscriptModuleScalarContract.Bytes(child,field);if(field.Codec=="SageBool"&&child.HasAttribute(field.Name))child.SetAttribute(field.Name,child.GetAttribute(field.Name).Trim() is "true" or "1"?"true":"false");}
            if(type==typeof(EffectInfoType))_=Options(child);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only comparison copies use the baseline children; actual list length, optional presence and values remain source-authored. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>())actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify complete infantry and building variants against stock native bytes, dependencies and every metadata field/target. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,XmlSchemaSet official,SchemaSet schemas,XmlDocument baseline,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<SecondaryDamageTrackerModuleData>()!=36||Marshal.SizeOf<EffectInfoType>()!=16)throw new InvalidDataException("Tracker native ABI differs.");
        var witness=baseline.GetElementsByTagName("SecondaryDamageTracker",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
        foreach(bool building in new[]{false,true}){
            var original=(XmlElement)witness.CloneNode(true);
            if(building){foreach(var item in Effects(original))item.RemoveAttribute("Options");var status=original.OwnerDocument!.CreateElement("ObjectStatusValidation",SchemaSet.XmlNamespace);status.SetAttribute("ForbiddenStatus","UNDER_IRON_CURTAIN");original.AppendChild(status);}
            var source=new XmlDocument();source.LoadXml(baseline.OuterXml);var old=source.GetElementsByTagName("SecondaryDamageTracker",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();old.ParentNode!.ReplaceChild(source.ImportNode(original,true),old);source.Schemas.Add(official);source.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));ConscriptBehaviorSurvey.ResolveStrong(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source.OuterXml);
            var stock=GameObjectStockReview.Capture(manifest,building?"GameObject:AlliedConstructionYard":"GameObject:SovietAntiInfantryInfantry");var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;int index=Array.FindIndex(modules,m=>m.Type=="SecondaryDamageTrackerModuleData");if(index<0)throw new InvalidDataException("Tracker stock witness missing.");var module=modules[index];
            var compiled=instance.XmlNode.SelectSingleNode("*[local-name()='Behaviors']/*[@id='"+original.GetAttribute("id")+"']")??throw new InvalidDataException("Normalized tracker witness missing.");var native=ConscriptBehaviorSurvey.Compile(compiled,module.TypeId);
            var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),native,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true);comparer.Compare(typeof(SecondaryDamageTrackerModuleData),module.Native);
            var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,"SecondaryDamageTrackerModuleData",typeof(SecondaryDamageTrackerModuleData));int end=index+1<modules.Length?modules[index+1].Wrapper:GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("AI")].Offset;
            BehaviorMetadataStockProof.Compare(metadata,native,stock.Bytes,module.Wrapper,end,typeof(SecondaryDamageTrackerModuleData),comparer.NativeRecords);Verify(original,native,metadata,instance.ReferencedInstances.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToArray(),catalog);
        }
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently bind all scalar/optional/list bytes, modifier import identities and exact auxiliary ownership to authored source. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata,AssetId[] references,ManifestAsset[] catalog)
    {
        Validate(owner);var b=native.InstanceBuffer;if(b.Length<36||Word(b,0)!=FastHash.GetHashCode("SecondaryDamageTrackerModuleData")||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("Tracker identity/header differs.");
        var covered=new bool[b.Length];var relos=new HashSet<uint>();var imports=new HashSet<uint>();Claim(0,36);foreach(var field in ConscriptModuleScalarContract.Bindings(typeof(SecondaryDamageTrackerModuleData)))Check(owner,field,0);
        foreach(var record in new[]{("Color",20,typeof(RGBColor)),("ObjectStatusValidation",32,typeof(ObjectStatusValidationDataType))}){var element=owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==record.Item1);int target=Pointer(record.Item2,element!=null,Marshal.SizeOf(record.Item3));if(element!=null)foreach(var field in ConscriptModuleScalarContract.RecordBindings(record.Item3))Check(element,field,target);}
        var items=Effects(owner);if(Word(b,24)!=items.Length)throw new InvalidDataException("Tracker effect count differs.");int array=Pointer(28,items.Length!=0,checked(items.Length*16));
        for(int i=0;i<items.Length;i++){
            var item=items[i];int at=array+i*16;foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(EffectInfoType)))Check(item,field,at);if(!b.AsSpan(at+8,4).SequenceEqual(Options(item))||b[at+15]!=0)throw new InvalidDataException("Tracker options/padding differs.");
            string value=item.GetAttribute("AttributeModifier");uint selector=Word(b,at+4);if(value.Length==0){if(selector!=0)throw new InvalidDataException("Absent tracker modifier has import.");}
            else{int colon=value.IndexOf(':');if(colon>=0){if(value[..colon]!="AttributeModifier")throw SdkXmlDiagnostics.At(new InvalidDataException("EffectInfo requires AttributeModifier target."),item.GetAttributeNode("AttributeModifier"));value=value[(colon+1)..];}uint id=InstanceHandle.GetInstanceId(value),type=FastHash.GetHashCode("AttributeModifier");if(catalog.Count(a=>a.TypeId==type&&a.InstanceId==id)!=1||selector==0||selector>references.Length||references[(int)selector-1]!=new AssetId(type,id))throw SdkXmlDiagnostics.At(new InvalidDataException("Missing/ambiguous modifier or incorrect effect import: "+value),item.GetAttributeNode("AttributeModifier"));imports.Add((uint)(at+4));}
        }
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("Tracker unowned native allocation.");}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||!Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order()))throw new InvalidDataException("Tracker auxiliary ownership differs.");var literal=Metadata(b);if(metadata.InstanceBuffer.Length-b.Length!=literal.Length||!metadata.InstanceBuffer.AsSpan(0,literal.Length).SequenceEqual(literal))throw new InvalidDataException("Tracker generic/literal metadata differs.");
        // Reborn: source codecs independently check defaults, packed values and engine-unit conversions.
        void Check(XmlElement element,ConscriptModuleScalarContract.Binding field,int start){if(!b.AsSpan(start+field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,field)))throw new InvalidDataException("Tracker authored field differs: "+element.LocalName+"."+field.Name);}
        // Reborn: optional records and nonempty arrays own exactly one relocation and a disjoint complete allocation.
        int Pointer(int at,bool present,int size){uint pointer=Word(b,at);if(!present){if(pointer!=0)throw new InvalidDataException("Absent tracker field has allocation.");return 0;}if(pointer<36||pointer>int.MaxValue)throw new InvalidDataException("Tracker pointer differs.");Claim((int)pointer,size);relos.Add((uint)at);return (int)pointer;}
        // Reborn: no hidden or overlapping heap storage is accepted as a valid tracker graph.
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("Tracker allocation bounds/overlap differs.");Array.Fill(covered,true,at,size);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal metadata independently encodes source-sized effects and optional RGB/status tables, not cached stock offsets. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(byte[] b)
    {
        var rows=new List<uint>();var targets=new List<(int At,int Target)>();int wrapper=Allocate(1),owner=Allocate(8),update=Allocate(2),contain=Allocate(2),behavior=Allocate(2),module=Allocate(3);
        Row(wrapper,2,FastHash.GetHashCode("SecondaryDamageTrackerModuleData"),(uint)owner);Row(owner,1,FastHash.GetHashCode("UpdateModuleData"),(uint)update);Row(update,1,FastHash.GetHashCode("ContainModuleData"),(uint)contain);Row(contain,1,FastHash.GetHashCode("BehaviorModuleData"),(uint)behavior);Row(behavior,1,FastHash.GetHashCode("ModuleData"),(uint)module);Field(module,"TypeId",0x23E650B8,0);Field(module+12,"id",0xBDF07965,4);
        Field(owner+12,"UpdateDuration",0x998669E8,8);Field(owner+24,"DissipateAmount",0x763177A4,12);Field(owner+36,"MaxDamageClampScaler",0x645864F3,16);
        uint color=Word(b,20);Row(owner+48,FastHash.GetHashCode("Color"),0xAEE762A5,0);if(color!=0){int table=Allocate(4);rows[(owner+56)/4]=(uint)table;for(int i=0;i<3;i++)Field(table+i*12,new[]{"R","G","B"}[i],0x645864F3,(int)color+i*4);}
        int count=checked((int)Word(b,24)),list=Allocate(1+count);Row(owner+60,FastHash.GetHashCode("EffectInfo"),0x25F5379F,(uint)list);Row(list,0x6C827073,0x5D448FD8,0);Native(list+8,24);
        string[] names={"Fraction","AttributeModifier","OnEnterDisable","OnEnterCancelSpecialAbilities","SetUpgradedArmor","Options"};uint[] types={0x645864F3,0x09DDCFF3,0x77E3F4F7,0x77E3F4F7,0x77E3F4F7,0xA2F37A52};int[] offsets={0,4,12,13,14,8};
        for(int i=0;i<count;i++){int table=Allocate(7),start=checked((int)Word(b,28))+i*16;Row(list+12+i*12,0xB151A0EB,0,(uint)table);for(int f=0;f<names.Length;f++)Field(table+f*12,names[f],types[f],start+offsets[f]);}
        uint status=Word(b,32);Row(owner+72,FastHash.GetHashCode("ObjectStatusValidation"),0x3E8F938C,0);if(status!=0){int table=Allocate(3);rows[(owner+80)/4]=(uint)table;Field(table,"ForbiddenStatus",0xA7BD811E,(int)status);Field(table+12,"RequiredStatus",0xA7BD811E,(int)status+32);}
        var result=new byte[rows.Count*4];for(int i=0;i<rows.Count;i++)BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(i*4),rows[i]);foreach(var target in targets)BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(target.At),checked((uint)(result.Length+target.Target)));return result;
        // Reborn: each table/list includes its explicit zero sentinel where required.
        int Allocate(int count){int at=rows.Count*4;for(int i=0;i<count*3;i++)rows.Add(0);return at;}
        // Reborn: internal table offsets are distinct from native targets.
        void Row(int at,uint name,uint type,uint target){rows[at/4]=name;rows[at/4+1]=type;rows[at/4+2]=target;}
        // Reborn: native addresses shift once after the final metadata extent is known.
        void Native(int at,int target)=>targets.Add((at,target));
        // Reborn: primitive and reference fields retain direct native-slot targets even when their value is zero.
        void Field(int at,string name,uint type,int target){Row(at,FastHash.GetHashCode(name),type,0);Native(at+8,target);}
    }
    // Reborn: ordered repeated rows are never deduplicated or sorted by an undocumented fraction policy.
    private static XmlElement[] Effects(XmlElement owner)=>owner.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="EffectInfo").ToArray();
    // Reborn: the existing four-byte options word uses the official two enum-indexed bits.
    private static byte[] Options(XmlElement item)=>ConscriptModuleScalarContract.Bytes(item,new("Options",8,4,"Flags:SageBinaryData.EffectInfoFlagType","",0xA2F37A52));
    // Reborn: exact auxiliary inventories include one canonical sentinel and no duplicate sources.
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8||b.Length%4!=0||Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Tracker auxiliary sentinel differs.");var values=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(values.Distinct().Count()!=values.Length)throw new InvalidDataException("Tracker duplicate auxiliary slot.");return values;}
    // Reborn: callers bound header/record extents before reading native words.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
