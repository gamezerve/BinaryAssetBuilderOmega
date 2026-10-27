using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: sound, shadow and projected buildability are source-sized optional root capabilities, not captured infantry owner constants.
internal static class GameObjectExtendedRootContract
{
    internal static readonly string[] Names={"AudioArraySound","ShadowInfo","ProjectedBuildabilityInfo","SkirmishAIInformation","AudioArrayVoice","EvaEventArray","CrusherInfo"};
    // Reborn: repeated projected records remain separate elements, while schema admission checks singleton owners.
    internal static XmlElement[] Owners(XmlDocument source,string name)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore only comparison owners so fixed-stock reference coordinates stay independent of authored root presence/counts. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        foreach(string name in Names){foreach(var node in Owners(source,name))source.DocumentElement!.RemoveChild(node);foreach(var model in Owners(baseline,name)){XmlElement? next=null;for(var following=model.NextSibling;following!=null;following=following.NextSibling)if(following is XmlElement element&&(next=source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==element.LocalName))!=null)break;source.DocumentElement!.InsertBefore(source.ImportNode(model,true),next);}}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose original SDK root declarations read-only for qualification and developer fixtures. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement[] Original(string relative,string name)
    {
        string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        for(var folder=new DirectoryInfo(fixtures);folder!=null;folder=folder.Parent){string path=Path.Combine(new[]{folder.FullName,"alpha","SageXML"}.Concat(relative.Split('/')).ToArray());if(!File.Exists(path))continue;var xml=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(path,1048576),path);return xml.GetElementsByTagName(name,SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray();}
        throw new InvalidDataException("Original SDK source not found: "+relative);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify real shadow/buildability records and the original power-plant ambient sound entry against independently captured stock bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string name,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var grammar=new StockMetadataGrammar();string stockName=name is "ShadowInfo" or "EvaEventArray" or "CrusherInfo"?"SovietAntiInfantryInfantry":name=="AudioArraySound"?"SovietPowerPlant":"SovietRefinery";
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+stockName);var rows=GameObjectStockReview.Tokens(stock.Bytes,0);
        var probe=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(baseline.DocumentElement!.OuterXml));var field=BinaryAssetBuilder.XmlCompiler.Ep1GameObjectRootOwnersProjection.Fields.Single(f=>f.Name==name);
        if(name is not ("ShadowInfo" or "EvaEventArray" or "CrusherInfo")){foreach(var existing in Owners(probe,name))probe.DocumentElement!.RemoveChild(existing);foreach(var node in Original("Soviet/Structures/"+stockName+".xml",name))probe.DocumentElement!.InsertBefore(probe.ImportNode(node,true),name=="AudioArraySound"?probe.DocumentElement.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName=="EvaEventArray"):name=="AudioArrayVoice"?probe.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName is "AudioArraySound" or "EvaEventArray"):name=="SkirmishAIInformation"?probe.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="Draws"):null);}
        // Reborn: original refinery voiceSelect and inherited building capture announcements qualify distinct real stock entries without inventing source inheritance order.
        if(name=="AudioArrayVoice")foreach(var owner in Original("BaseObjects/BaseStructure.xml",name).Where(e=>e.ParentNode is XmlElement parent&&parent.LocalName=="GameObject"))foreach(var row in owner.ChildNodes.OfType<XmlElement>())Owners(probe,name).Single().AppendChild(probe.ImportNode(row,true));
        var resolved=SchemaNativeContract.Resolve(probe,schemas,catalog);int native=(int)Word(stock.Bytes,stock.Projection.NativeRoot+field.PointerSlot);
        if(name=="AudioArraySound"){
            // Reborn: inherited construction audio may coexist with this entry; match the real ambient ordinal, not a guessed array position.
            var element=Owners(resolved,name).Single().ChildNodes.OfType<XmlElement>().Single(e=>e.GetAttribute("AudioType")=="soundAmbient");int items=(int)Word(stock.Bytes,native+4),count=(int)Word(stock.Bytes,native);int index=Enumerable.Range(0,count).Single(i=>Word(stock.Bytes,items+i*8+4)==(uint)Ep1ThingTemplateSoundType.soundAmbient);
            SchemaNativeContract.VerifyStock(element,typeof(Ep1AudioSoundEntry),0,stock.Bytes,items+index*8,stock.Asset.References.ToArray());grammar.Learn(stock.Bytes,rows[FastHash.GetHashCode(name)].Offset,stock.Projection.NativeRoot,field.NativeType);
        }else if(name=="AudioArrayVoice"){
            int count=(int)Word(stock.Bytes,native),items=(int)Word(stock.Bytes,native+4);var entries=Owners(resolved,name).Single().ChildNodes.OfType<XmlElement>().ToArray();if(entries.Length!=count)throw new InvalidDataException("Original refinery voice witness cardinality differs.");
            foreach(var entry in entries){uint ordinal=(uint)Enum.Parse<Ep1ThingTemplateVoiceType>(entry.GetAttribute("AudioType"));var matches=Enumerable.Range(0,count).Where(i=>Word(stock.Bytes,items+i*12+8)==ordinal).ToArray();if(matches.Length!=1)throw new InvalidDataException("Voice stock ordinal differs: "+entry.GetAttribute("AudioType")+"="+ordinal+"; stock="+string.Join(",",Enumerable.Range(0,count).Select(i=>Word(stock.Bytes,items+i*12+8))));int index=matches[0];SchemaNativeContract.VerifyStock(entry,typeof(Ep1AudioVoiceEntry),0,stock.Bytes,items+index*12,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,rows[FastHash.GetHashCode(name)].Offset,stock.Projection.NativeRoot,field.NativeType);
            // Reborn: named voice entries are qualified on the original Century Bomber (three NamedEntry records with Sound pointers and Name hashes).
            {var bomber=GameObjectStockReview.Capture(manifest,"GameObject:AlliedBomberAircraft");var document=(XmlDocument)baseline.CloneNode(true);foreach(var existing in Owners(document,name))document.DocumentElement!.RemoveChild(existing);document.DocumentElement!.InsertBefore(document.ImportNode(Original("Allied/Units/AlliedBomberAircraft.xml",name).Single(),true),document.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName is "AudioArraySound" or "EvaEventArray"));
            var named=Owners(SchemaNativeContract.Resolve(document,schemas,catalog),name).Single().ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="NamedEntry").ToArray();int voice=(int)Word(bomber.Bytes,bomber.Projection.NativeRoot+field.Slot);
            if(named.Length==0||Word(bomber.Bytes,voice+8)!=0||Word(bomber.Bytes,voice+16)!=named.Length)throw new InvalidDataException("Original named-voice witness cardinality differs: AlliedBomberAircraft.");
            int namedItems=(int)Word(bomber.Bytes,voice+20);for(int i=0;i<named.Length;i++)SchemaNativeContract.VerifyStock(named[i],typeof(Ep1AudioVoiceReferentialEntry),0,bomber.Bytes,namedItems+i*12,bomber.Asset.References.ToArray());
            grammar.Learn(bomber.Bytes,GameObjectStockReview.Tokens(bomber.Bytes,0)[FastHash.GetHashCode(name)].Offset,bomber.Projection.NativeRoot,field.NativeType);}
            // Reborn: Tesla Trooper's root object-specific voice independently qualifies the EP1 sixteen-byte row, inline EVA word and target hash.
            {var trooper=GameObjectStockReview.Capture(manifest,"GameObject:SovietHeavyAntiVehicleInfantry");var document=(XmlDocument)baseline.CloneNode(true);foreach(var existing in Owners(document,name))document.DocumentElement!.RemoveChild(existing);
            var original=Original("Soviet/Units/SovietHeavyAntiVehicleInfantry.xml",name).Single(e=>e.ParentNode is XmlElement parent&&parent.LocalName=="GameObject");
            document.DocumentElement!.InsertBefore(document.ImportNode(original,true),document.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName is "AudioArraySound" or "EvaEventArray"));
            var entriesSpecific=Owners(SchemaNativeContract.Resolve(document,schemas,catalog),name).Single().ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ObjectSpecificEntry").ToArray();int voice=(int)Word(trooper.Bytes,trooper.Projection.NativeRoot+field.Slot);
            if(entriesSpecific.Length==0||Word(trooper.Bytes,voice+8)!=entriesSpecific.Length)throw new InvalidDataException("Original object-specific voice witness cardinality differs: SovietHeavyAntiVehicleInfantry.");
            int objectItems=(int)Word(trooper.Bytes,voice+12);for(int i=0;i<entriesSpecific.Length;i++)SchemaNativeContract.VerifyStock(entriesSpecific[i],typeof(Ep1AudioObjectSpecificVoiceEntry),0,trooper.Bytes,objectItems+i*16,trooper.Asset.References.ToArray());
            grammar.Learn(trooper.Bytes,GameObjectStockReview.Tokens(trooper.Bytes,0)[FastHash.GetHashCode(name)].Offset,trooper.Projection.NativeRoot,field.NativeType);}
        }else if(name=="SkirmishAIInformation"){
            SchemaNativeContract.VerifyStock(Owners(resolved,name).Single(),field.NativeType,0,stock.Bytes,native,stock.Asset.References.ToArray());grammar.Learn(stock.Bytes,rows[FastHash.GetHashCode(name)].Offset,stock.Projection.NativeRoot,field.NativeType);
            // Reborn: actual optional float/mask witnesses supplement the refinery's absent optional pointers.
            foreach(string unit in new[]{"AlliedSupportAircraft","AlliedInfiltrationInfantry"}){var witness=GameObjectStockReview.Capture(manifest,"GameObject:"+unit);var original=Original("Allied/Units/"+unit+".xml",name).Single();var document=(XmlDocument)baseline.CloneNode(true);var previous=Owners(document,name).Single();previous.ParentNode!.ReplaceChild(document.ImportNode(original,true),previous);var effective=Owners(SchemaNativeContract.Resolve(document,schemas,catalog),name).Single();int pointer=(int)Word(witness.Bytes,witness.Projection.NativeRoot+field.Slot);SchemaNativeContract.VerifyStock(effective,field.NativeType,0,witness.Bytes,pointer,witness.Asset.References.ToArray());grammar.Learn(witness.Bytes,GameObjectStockReview.Tokens(witness.Bytes,0)[FastHash.GetHashCode(name)].Offset,witness.Projection.NativeRoot,field.NativeType);}
            var engineer=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.Type=="EngineerContainModuleData");grammar.Learn(stock.Bytes,engineer.Wrapper,stock.Projection.NativeRoot,typeof(Ep1EngineerContainModuleData));
        }else if(name=="CrusherInfo"){
            SchemaNativeContract.VerifyStock(Owners(resolved,name).Single(),field.NativeType,0,stock.Bytes,native,stock.Asset.References.ToArray());grammar.Learn(stock.Bytes,rows[FastHash.GetHashCode(name)].Offset,stock.Projection.NativeRoot,field.NativeType);
            // Reborn: independently qualify the inherited Soviet building levels against the actual refinery, not a changed infantry byte constant.
            var building=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var document=(XmlDocument)baseline.CloneNode(true);var previous=Owners(document,name).Single();previous.ParentNode!.ReplaceChild(document.ImportNode(Original("BaseObjects/BaseSovietStructure.xml",name).Single(),true),previous);var effective=Owners(SchemaNativeContract.Resolve(document,schemas,catalog),name).Single();int target=(int)Word(building.Bytes,building.Projection.NativeRoot+field.Slot);SchemaNativeContract.VerifyStock(effective,field.NativeType,0,building.Bytes,target,building.Asset.References.ToArray());grammar.Learn(building.Bytes,GameObjectStockReview.Tokens(building.Bytes,0)[FastHash.GetHashCode(name)].Offset,building.Projection.NativeRoot,field.NativeType);
            // Reborn: populate both crusher list grammars from original EP1 vehicles rather than accepting only the old empty-header shape.
            CrusherChildrenContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        }else if(field.InlineList){
            int table=rows[FastHash.GetHashCode(name)].Offset;var elements=Owners(resolved,name);if(Word(stock.Bytes,stock.Projection.NativeRoot+field.Slot)!=elements.Length)throw new InvalidDataException("Original projected source/stock cardinality differs.");
            for(int i=0;i<elements.Length;i++){SchemaNativeContract.VerifyStock(elements[i],field.NativeType,0,stock.Bytes,native+i*SchemaNativeContract.Size(field.NativeType),stock.Asset.References.ToArray());grammar.Learn(stock.Bytes,(int)Word(stock.Bytes,table+12+i*12+8),stock.Projection.NativeRoot,field.NativeType);}
            // Reborn: an authored optional filter uses the same independently stock-learned ObjectFilter grammar as real engineer containment.
            var engineer=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.Type=="EngineerContainModuleData");grammar.Learn(stock.Bytes,engineer.Wrapper,stock.Projection.NativeRoot,typeof(Ep1EngineerContainModuleData));
        }else{SchemaNativeContract.VerifyStock(Owners(resolved,name).Single(),field.NativeType,0,stock.Bytes,native,stock.Asset.References.ToArray());grammar.Learn(stock.Bytes,rows[FastHash.GetHashCode(name)].Offset,stock.Projection.NativeRoot,field.NativeType);}
        return grammar;
    }
    // Reborn: stock pointer/count reads remain inside the captured asset slice.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Extended root stock word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
