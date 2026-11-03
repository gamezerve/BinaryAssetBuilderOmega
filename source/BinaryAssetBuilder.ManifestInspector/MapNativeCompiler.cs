using System.Reflection;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: build map object/script graphs from source through the existing marshalers, independently comparing the EP1 retail ABI before enabling publication.
internal static class MapNativeCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: list real terrain source dependencies independently of the native map proof; missing source artwork is never replaced by synthetic test pixels. */
    //-------------------------------------------------------------------------------------------------
    internal static int AuditResources(string entry,string sdk,string output,bool warnings=false)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh map resource diagnostic directory required.");
        var search=SdkSourceSearch.ForMod(entry,sdk);var raw=File.ReadAllBytes(entry);var xml=SdkXmlDiagnostics.Parse(raw,entry);
        var resources=new System.Collections.Generic.List<object>();int missing=0;
        foreach(var atlas in xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="TerrainTextureAtlas"))
        foreach(var tile in atlas.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Tile"))
        foreach(string field in new[]{"BaseTexture","NormalTexture"}){
            string logical=tile.GetAttribute(field);
            try{var selected=search.Select(entry,logical,search.ModData);resources.Add(new{TextureID=tile.GetAttribute("TextureID"),Field=field,LogicalPath=logical,Selected=selected.PhysicalPath,Missing=false});}
            catch(SdkSourceSearch.MissingFileException error){missing++;resources.Add(new{TextureID=tile.GetAttribute("TextureID"),Field=field,LogicalPath=logical,Missing=true,Searched=error.Candidates});
                // Reborn: absent terrain is an explicit warning at its authored field; the map stage can skip that map while retaining existing Misc output.
                if(warnings){var located=SdkXmlDiagnostics.At(new InvalidDataException(error.Message),tile.GetAttributeNode(field));new SdkXmlDiagnostics.SourceMap(new[]{new SdkXmlDiagnostics.Snapshot(entry,raw)}).Enrich(located,entry,xml);Console.WriteLine("Warning: MISSINGTERRAIN ' "+logical+" ' "+SdkXmlDiagnostics.Format(located,new[]{entry}));}
            }
        }
        Directory.CreateDirectory(output);File.WriteAllText(Path.Combine(output,"terrain-resources.json"),System.Text.Json.JsonSerializer.Serialize(new{Entry=entry,Count=resources.Count,Missing=missing,Resources=resources},new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine($"Map terrain resource audit: {resources.Count} texture uses, {missing} missing; report={Path.Combine(output,"terrain-resources.json")}. No terrain pixels or mod sources changed.");
        return missing;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize map declarations after Core has normalized POIDs, script polymorphism and typed reference occurrences. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlElement source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        // Reborn: standalone normalization resolves asset references but does not inject polymorphic class tags; bind the two official script parameter classes explicitly.
        foreach(XmlElement parameter in xml.GetElementsByTagName("ParameterList",SchemaSet.XmlNamespace).Cast<XmlElement>().SelectMany(e=>e.ChildNodes.OfType<XmlElement>())){
            string concrete=parameter.LocalName switch{"ObjectType"=>"GameScriptParameterObjectType","ObjectTypeList"=>"GameScriptParameterObjectTypeList",_=>throw new InvalidDataException("Unsupported map script parameter: "+parameter.LocalName)};
            parameter.SetAttribute("TypeId",BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(concrete).ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        var node=new Node(xml.DocumentElement!.CreateNavigator()!,ns);Chunk chunk=new();
        if(source.LocalName=="GameMap"){
            Ep1GameMap* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1GameMap),false);
            Marshaler.Marshal(node,root,tracker);if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("GameMap native allocation failed.");
        }else if(source.LocalName=="GameScriptList"){
            GameScriptList* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(GameScriptList),false);
            Marshaler.Marshal(node,root,tracker);if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("GameScriptList native allocation failed.");
        }else throw new InvalidDataException("Unknown map-native declaration: "+source.LocalName);
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare selected authored map/script native graphs to their actual EP1 map counterparts without copying any stock payload into the authored output. */
    //-------------------------------------------------------------------------------------------------
    internal static void Probe(string entry,string mapManifest,string staticManifest,string globalManifest,string output)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh map native diagnostic directory required.");
        Directory.CreateDirectory(output);CompilerSmokeTest.InitializeHashProvider();
        var saved=Settings.Current;
        try{
            Settings.Current=new Settings{SchemaPath=ReferencePipelineSmokeTest.FindFixture(),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);
            // Reborn: standalone diagnostic aliases reuse the official map types without enabling map assets in the ordinary static frontend.
            var official=ConscriptSourceInput.Schemas();var aliases=new System.Xml.Schema.XmlSchema{TargetNamespace=SchemaSet.XmlNamespace,ElementFormDefault=System.Xml.Schema.XmlSchemaForm.Qualified};
            foreach(string name in new[]{"GameMap","GameScriptList"})if(!official.GlobalElements.Contains(new XmlQualifiedName(name,SchemaSet.XmlNamespace)))aliases.Items.Add(new System.Xml.Schema.XmlSchemaElement{Name=name,SchemaTypeName=new XmlQualifiedName(name,SchemaSet.XmlNamespace)});
            official.Add(aliases);official.Compile();schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var map=ManifestReader.Read(File.ReadAllBytes(mapManifest));
            var catalog=new[]{mapManifest,staticManifest,globalManifest}.SelectMany(p=>ManifestReader.Read(File.ReadAllBytes(p)).Assets).GroupBy(a=>(a.TypeId,a.InstanceId)).Select(g=>g.First()).ToArray();
            PlayerTechStoreCompiler.Qualify(staticManifest,schemas,catalog);
            var document=SdkXmlDiagnostics.Parse(File.ReadAllBytes(entry));int count=0;
            foreach(var asset in document.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "GameMap" or "GameScriptList")){
                string name=asset.LocalName+":"+asset.GetAttribute("id");
                var stock=map.Assets.Single(a=>a.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
                var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(asset.OuterXml));source.Schemas.Add(official);source.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
                ConscriptBehaviorSurvey.ResolveStrong(source,schemas,catalog);
                var instance=ReferencePipelineSmokeTest.Normalize(schemas,source.OuterXml);
                var native=Compile((XmlElement)instance.XmlNode);
                File.WriteAllBytes(Path.Combine(output,asset.LocalName+"-"+count+".bin"),native.InstanceBuffer);
                var bytes=W3DContainerCompiler.ReadStock(mapManifest,map,stock);
                Console.WriteLine($"Map native {name}: authored={native.InstanceBuffer.Length}/{native.RelocationBuffer.Length}/{native.ImportsBuffer.Length}, retail={stock.InstanceDataSize}/{stock.RelocationDataSize}/{stock.ImportsDataSize}; tokenized={stock.Tokenized}, type={stock.TypeId:X8}/{stock.TypeHash:X8}.");
                var comparer=new SelectedNativeGraphComparer(bytes,stock.References.ToArray(),native,instance,captureNativeCoordinates:false);
                comparer.Compare(asset.LocalName=="GameMap"?typeof(Ep1GameMap):typeof(GameScriptList),0);
                count++;
            }
            if(count==0)throw new InvalidDataException("No direct GameMap/GameScriptList declarations found.");
            Console.WriteLine($"Map native source/retail comparison PASS: {count} complete object/script graphs. Terrain atlas and map publication remain separate.");
            // Reborn: atlas packing is tested independently with synthetic pixels; missing original terrain artwork cannot become a false successful source build.
            foreach(var atlas in document.DocumentElement.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="TerrainTextureAtlas"))TerrainAtlasCompiler.Probe(atlas,mapManifest,output);
        }finally{Settings.Current=saved;}
    }
}
