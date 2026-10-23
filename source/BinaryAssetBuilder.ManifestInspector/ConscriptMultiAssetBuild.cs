using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile explicitly declared support assets and the complete object into one dependency-sorted EP1 stream, with local targets overriding stock lookup.
internal static class ConscriptMultiAssetBuild
{
    private sealed record Built(XmlDocument Source,InstanceDeclaration Instance,AssetBuffer Buffer,bool Tokenized);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: existing single-object routes remain unchanged; additional.xml selects the actual multi-asset frontend output. */
    //-------------------------------------------------------------------------------------------------
    internal static bool TryRun(string entry,string manifest,string global,string output)
    {
        string additional=Path.Combine(Path.GetDirectoryName(entry)!,"additional.xml");var source=Read(entry);bool hasGame=source.DocumentElement!.LocalName=="GameObject";if(!File.Exists(additional)&&hasGame)return false;
        var saved=Settings.Current;
        try{
            var official=ConscriptSourceInput.Schemas();string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalogs=new[]{manifest,global}.Select(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576))).ToArray();foreach(var d in catalogs){TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked||d.Validate().Count!=0)throw new InvalidDataException("Mixed build requires valid EP1 stock catalogs.");}
            var stock=catalogs.SelectMany(d=>d.Assets).ToArray();var sources=new List<XmlDocument>{source};
            if(File.Exists(additional))sources.AddRange(Read(additional).DocumentElement!.ChildNodes.OfType<XmlElement>().Select(e=>Xml(e.OuterXml)));
            string command=Path.Combine(Path.GetDirectoryName(entry)!,"command.xml");if(File.Exists(command))sources.Add(Read(command));
            var fingerprints=new Dictionary<uint,(uint Hash,bool Tokenized)>();var locals=new Dictionary<AssetId,XmlDocument>();
            foreach(var xml in sources){string type=xml.DocumentElement!.LocalName,id=xml.DocumentElement.GetAttribute("id");if(type is not ("GameObject" or "ArmorTemplate" or "WeaponTemplate" or "LogicCommand" or "LogicCommandSet" or "UpgradeTemplate")||!ConscriptSourceInput.Identity(id))throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported mixed asset type or invalid identity: "+type+":"+id),xml.DocumentElement);
                uint typeId=FastHash.GetHashCode(type);var examples=stock.Where(a=>a.TypeId==typeId).Select(a=>(a.TypeHash,Tokenized:a.Tokenized==1)).Distinct().ToArray();if(examples.Length!=1)throw new InvalidDataException("EP1 asset fingerprint is absent/ambiguous: "+type);fingerprints[typeId]=examples[0];if(!locals.TryAdd(new(typeId,InstanceHandle.GetInstanceId(id)),xml))throw SdkXmlDiagnostics.At(new InvalidDataException("Duplicate local asset/hash identity: "+type+":"+id),xml.DocumentElement);
            }
            // Reborn: source-only catalog entries resolve local names before native data exists; payloads are freshly compiled below, never copied from stock.
            var localEntries=locals.Select(p=>new ManifestAsset(p.Key.TypeId,p.Key.InstanceId,fingerprints[p.Key.TypeId].Hash,0,0,0,0,0,0,0,0,fingerprints[p.Key.TypeId].Tokenized?1u:0u,p.Value.DocumentElement!.LocalName+":"+p.Value.DocumentElement.GetAttribute("id"),"authored.xml",Array.Empty<AssetId>())).ToArray();
            var available=stock.Where(a=>!locals.ContainsKey(new(a.TypeId,a.InstanceId))).Concat(localEntries).ToArray();
            var baseline=Read(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml"));baseline.DocumentElement!.SetAttribute("BuildTime","4");
            // Reborn: each emitted object owns its source admission/template map; never reuse the primary object's modules for another object.
            var plans=sources.Where(s=>s.DocumentElement!.LocalName=="GameObject").ToDictionary(s=>s,s=>ConscriptVariableBehaviorSource.Admit(s,baseline,official));
            var built=new Dictionary<AssetId,Built>();int prefix=0;var compileLog=ConscriptCompileLog.ForPrepared(entry);
            // Reborn: validate the isolated EP1 upgrade ABI against original stock before emitting any authored upgrades.
            if(sources.Any(s=>s.DocumentElement!.LocalName=="UpgradeTemplate"))UpgradeTemplateCompiler.Qualify(global,schemas,stock);
            foreach(var xml in sources){
                string type=xml.DocumentElement!.LocalName;InstanceDeclaration instance;AssetBuffer buffer;
                // Reborn: announce and time the asset around its actual processor/verifier, as EA BuildMod does per instance.
                var progress=compileLog.Begin(type,xml.DocumentElement.GetAttribute("id"));
                if(type=="GameObject"){
                    instance=Normalize(xml);var native=GameObjectBodyProjectionSmokeTest.Compile(Xml(instance.XmlNode.OuterXml),true,true,true);var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,available);var tokens=verifier.Verify(xml,native,instance,plans[xml].Origins);if(ReferenceEquals(xml,source))prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;buffer=Buffer(tokens);
                    // Reborn: weak CommandSet availability is reported consistently by the common mod/stock audit, without pretending it is a native import.
                }else if(type=="ArmorTemplate"){
                    instance=Normalize(xml);var plugin=new Ra3Ep1ArmorPlugin();plugin.Initialize(TargetPlatform.Win32);buffer=plugin.ProcessInstance(instance);
                }else if(type=="WeaponTemplate"){
                    AdmitWeapon(xml,official,fixtures);instance=Normalize(xml);var copy=Xml(instance.XmlNode.OuterXml);foreach(XmlElement nugget in copy.GetElementsByTagName("DamageNugget",SchemaSet.XmlNamespace))nugget.SetAttribute("TypeId",FastHash.GetHashCode("DamageNuggetType").ToString(System.Globalization.CultureInfo.InvariantCulture));
                    var native=WeaponNativeProbe.Compile(copy);VerifyWeaponReferences(xml,native,instance);buffer=Buffer(WeaponTokenReproduction.EncodeAuthored(native));
                }else if(type=="UpgradeTemplate"){
                    // Reborn: stock upgrades use native-only streams; Core still owns real strong-reference occurrence indices.
                    instance=Normalize(xml);var native=UpgradeTemplateCompiler.Compile(instance.XmlNode);
                    SchemaNativeContract.Verify(UpgradeTemplateCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1UpgradeTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="LogicCommand"){
                    var result=ConscriptRecruitmentSmokeTest.CompileXml(schemas,xml.OuterXml);instance=Attach(result.Instance,xml.DocumentElement.GetAttribute("id")+".xml");buffer=result.Chunk;
                }else{
                    QualifySet(stock,manifest,schemas);instance=Normalize(xml);buffer=CompileSet(instance);
                }
                var id=new AssetId(instance.Handle.TypeId,instance.Handle.InstanceId);instance.Handle.TypeHash=fingerprints[id.TypeId].Hash;instance.Handle.InstanceHash=FastHash.GetHashCode(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));instance.ValidatedReferencedInstances=new(instance.ReferencedInstances);
                foreach(var reference in instance.ReferencedInstances)if(available.Count(a=>a.TypeId==reference.TypeId&&a.InstanceId==reference.InstanceId)!=1)throw SdkXmlDiagnostics.At(new InvalidDataException("Missing or ambiguous asset: "+reference.Name),xml.DocumentElement);
                built.Add(id,new(xml,instance,buffer,fingerprints[id.TypeId].Tokenized));compileLog.End(progress);
            }
            // Reborn: stable dependency order is determined by actual local strong edges, not declaration order; stock references stay external.
            var ordered=new List<Built>();var active=new HashSet<AssetId>();var done=new HashSet<AssetId>();foreach(var id in built.Keys)Visit(id);
            int localReferences=ordered.Sum(b=>b.Instance.ReferencedInstances.Count(r=>built.ContainsKey(new(r.TypeId,r.InstanceId))));int stockReferences=ordered.Sum(b=>b.Instance.ReferencedInstances.Count)-localReferences;
            Console.WriteLine("Resolving references: "+ordered.Count+" assets dependency-sorted; "+localReferences+" local and "+stockReferences+" stock reference occurrences resolved.");
            var instances=ordered.Select(b=>b.Instance).ToArray();var chunks=ordered.Select(b=>b.Buffer).ToArray();var flags=ordered.Select(b=>b.Tokenized).ToArray();uint checksum=(uint)typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{instances})!;
            var runtime=new ReferencedFileBuffer();typeof(OutputManager).GetMethod("AddExternalManifestReferences",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{runtime,new[]{Path.GetFullPath(manifest),Path.GetFullPath(global)},"data\\static.manifest;data\\global.manifest"});
            Console.WriteLine("Generating stream: "+instances.Length+" assets, output checksum "+checksum.ToString("x8")+".");
            var streams=BoundedDiagnosticBuild.SerializeAssetGraph(instances,chunks,checksum,runtime,flags,fingerprints);var repeated=BoundedDiagnosticBuild.SerializeAssetGraph(instances,chunks,checksum,runtime,flags,fingerprints);if(streams.Any(p=>!p.Value.SequenceEqual(repeated[p.Key])))throw new InvalidDataException("Mixed stream serialization is not deterministic.");
            Directory.CreateDirectory(output);string verification=Path.Combine(output,"stream-readback");ModifierFXStreamSmokeTest.WriteNew(verification,streams);BoundedDiagnosticBuild.Verify(verification,instances,chunks,streams,checksum,new[]{"data\\static.manifest","data\\global.manifest"},flags);
            // Reborn: reported only after deterministic re-serialization and binary/relocation/import readback both passed.
            Console.WriteLine("Linking binary, relocation and import data verified by readback; mod Stream complete");compileLog.Statistics();
            // Reborn: pure command-set graphs publish through the same verified stream/archive contract without a dummy unit.
            if(!hasGame){PackageSupport(output,source,streams,instances,stock);return true;}
            // Reborn: stock overrides are not limited to the Conscript sample, and receipts must not label an existing building as a new identity.
            string gameId=source.DocumentElement!.GetAttribute("id");bool stockOverride=stock.Any(a=>a.TypeId==0x942FFF2D&&a.InstanceId==InstanceHandle.GetInstanceId(gameId)&&string.Equals(a.Name,"GameObject:"+gameId,StringComparison.OrdinalIgnoreCase));string package=Path.Combine(output,stockOverride?"override":"new-id");GameObjectConscriptTrialSmokeTest.Package(package,source,streams,gameId,prefix,stockOverride:stockOverride);
            // Reborn: the legacy package receipt keeps its primary-object contract, while explicitly reporting the complete mixed declaration graph.
            string receiptPath=Path.Combine(package,"trial-plan.json");var receipt=JsonNode.Parse(File.ReadAllText(receiptPath))!;receipt["ReferenceOccurrences"]=built[new(FastHash.GetHashCode("GameObject"),InstanceHandle.GetInstanceId(gameId))].Instance.ReferencedInstances.Count;receipt["CommandOverride"]=ordered.FirstOrDefault(b=>b.Source.DocumentElement!.LocalName=="LogicCommand")?.Instance.Handle.Name;receipt["MultiAsset"]=true;receipt["Assets"]=JsonSerializer.SerializeToNode(instances.Select(i=>i.Handle.Name));receipt["LocalEdges"]=JsonSerializer.SerializeToNode(ordered.SelectMany(b=>b.Instance.ReferencedInstances.Where(r=>locals.ContainsKey(new(r.TypeId,r.InstanceId))).Select(r=>new{Owner=b.Instance.Handle.Name,Target=r.Name})));
            // Reborn: report only explicitly declared Barracks recruitment edges; offline linkage is not gameplay confirmation.
            receipt["GameObjects"]=JsonSerializer.SerializeToNode(ordered.Where(b=>b.Source.DocumentElement!.LocalName=="GameObject").Select(b=>new{Asset=b.Instance.Handle.Name,ResourceInfoPresent=GameObjectResourceContract.Owner(b.Source)!=null,BuildCostCount=GameObjectResourceContract.Entries(b.Source).Length,LocomotorSetCount=ConscriptLocomotorSetContract.Entries(b.Source).Length,ReferenceOccurrences=b.Instance.ReferencedInstances.Count}));
            var commands=ordered.Where(b=>b.Source.DocumentElement!.LocalName=="LogicCommand"&&b.Instance.ReferencedInstances.Any(r=>r.TypeId==FastHash.GetHashCode("GameObject")&&r.InstanceId==InstanceHandle.GetInstanceId(gameId))).ToArray();
            bool barracks=ordered.Any(b=>b.Instance.Handle.Name=="LogicCommandSet:SovietBarracksCommandSet"&&b.Instance.ReferencedInstances.Any(r=>commands.Any(c=>c.Instance.Handle.TypeId==r.TypeId&&c.Instance.Handle.InstanceId==r.InstanceId)));
            receipt["DeclaredBarracksRecruitmentChain"]=barracks;receipt["RecruitableNewIdentity"]=!stockOverride&&barracks;receipt["NextGate"]="Controlled gameplay validation of the authored assets; offline linkage is not runtime proof.";
            File.WriteAllText(receiptPath,receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine($"Reborn: mixed asset graph compiled: {instances.Length} explicit assets; dependency-sorted local Armor/Weapon/Upgrade/GameObject/LogicCommand/LogicCommandSet edges; no generated commands or game launch.");return true;

            // Reborn: prepare current official references against mod-first identity lookup, then use actual Core occurrence normalization.
            InstanceDeclaration Normalize(XmlDocument xml){var copy=Xml(xml.DocumentElement!.OuterXml);copy.Schemas.Add(official);copy.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));ConscriptBehaviorSurvey.ResolveStrong(copy,schemas,available);return Attach(ReferencePipelineSmokeTest.Normalize(schemas,copy.OuterXml),xml.DocumentElement.GetAttribute("id")+".xml");}
            // Reborn: cycles in strong local dependencies cannot be silently emitted in an arbitrary guessed order.
            void Visit(AssetId id){if(done.Contains(id))return;if(!active.Add(id))throw new InvalidDataException("Cyclic local strong asset dependency: "+built[id].Instance.Handle.Name);foreach(var r in built[id].Instance.ReferencedInstances){var target=new AssetId(r.TypeId,r.InstanceId);if(built.ContainsKey(target))Visit(target);}active.Remove(id);done.Add(id);ordered.Add(built[id]);}
        }finally{Settings.Current=saved;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: package an explicit command-set primary and its authored dependencies, with exact BIG entry readback and no runtime claims. */
    //-------------------------------------------------------------------------------------------------
    private static void PackageSupport(string output,XmlDocument source,Dictionary<string,byte[]> streams,InstanceDeclaration[] instances,ManifestAsset[] stock)
    {
        string directory=Path.Combine(output,"support");Directory.CreateDirectory(directory);string[] extensions={".manifest",".bin",".relo",".imp"};var payloads=extensions.Select(e=>streams["diagnostic"+e]).ToArray();byte[] bytes=ArmorDiagnosticPackage.Serialize(payloads,true,true);string path=Path.Combine(directory,"RebornConscriptProbe.big");File.WriteAllBytes(path,bytes);
        using(var archive=BigArchive.Open(path,15,4096)){if(archive.Entries.Count!=15)throw new InvalidDataException("Command-set package entry count differs.");foreach(string suffix in new[]{"","_l","_m"})foreach(string extension in extensions.Concat(new[]{".version"})){var entry=archive.Entries.Single(e=>e.Name=="data\\mod"+suffix+extension);byte[] expected=extension==".version"?Encoding.ASCII.GetBytes("\r\n"):streams["diagnostic"+extension];if(!archive.ReadEntry(entry,1048576).SequenceEqual(expected))throw new InvalidDataException("Command-set package readback differs.");}}
        string name=source.DocumentElement!.LocalName+":"+source.DocumentElement.GetAttribute("id");var primary=instances.Single(i=>i.Handle.Name==name);
        var receipt=new{Asset=name,AssetCount=instances.Length,Assets=instances.Select(i=>i.Handle.Name),MultiAsset=true,ReferenceOccurrences=primary.ReferencedInstances.Count,AutomaticCommandGenerated=false,ArchiveEntryCount=15,ArchiveSha256=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)),SourceSha256=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(source.DocumentElement.OuterXml))),OfflineValidated=true,NewIdentity=!stock.Any(a=>a.Name==name),GameExecuted=false,InstalledFilesModified=false,RuntimeReady=false,InstanceHashPolicy="experimental-normalized-source"};
        File.WriteAllText(Path.Combine(directory,"trial-plan.json"),JsonSerializer.Serialize(receipt,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine("Reborn: support asset graph compiled: "+instances.Length+" explicit asset(s); no placeholder GameObject, generated commands or game launch.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the current weapon metadata covers one existing DamageNugget topology; supported fixed fields are values, not an AK47 identity/value whitelist. */
    //-------------------------------------------------------------------------------------------------
    internal static void AdmitWeapon(XmlDocument source,System.Xml.Schema.XmlSchemaSet schemas,string fixtures)
    {
        var model=Read(Path.Combine(fixtures,"WeaponAuthoringProbe.xml"));var actual=Xml(source.DocumentElement!.OuterXml);foreach(var d in new[]{actual,model}){d.Schemas.Add(schemas);d.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));}
        // Reborn: discriminator IDs belong to the processor, never silently replace an authored incompatible value.
        foreach(XmlAttribute attribute in source.SelectNodes("//@TypeId")!)throw SdkXmlDiagnostics.At(new InvalidDataException("TypeId is compiler-generated; omit the authored discriminator."),attribute);
        // Reborn: reject nonfinite XSD float values while the original authored node can still supply file/line diagnostics.
        foreach(XmlAttribute attribute in actual.SelectNodes("//@*")!)if(attribute.SchemaInfo.SchemaType?.Datatype?.TypeCode==System.Xml.Schema.XmlTypeCode.Float&&!float.IsFinite(XmlConvert.ToSingle(attribute.Value)))throw SdkXmlDiagnostics.At(new InvalidDataException("WeaponTemplate requires a finite value for "+attribute.LocalName+"."),SdkXmlDiagnostics.Unmasked(attribute,source));
        // Reborn: materialize schema defaults before masking; removing a defaulted attribute can otherwise resurrect it with a different presence shape.
        actual=Xml(actual.OuterXml);model=Xml(model.OuterXml);
        Mask(actual.DocumentElement!,model.DocumentElement!);if(ConscriptSourceInput.Signature(actual.DocumentElement!)!=ConscriptSourceInput.Signature(model.DocumentElement!)){string difference=ConscriptSourceInput.Difference(actual.DocumentElement!,model.DocumentElement!,"WeaponTemplate",out var node);throw SdkXmlDiagnostics.At(new InvalidDataException("WeaponTemplate contains an unimplemented single-DamageNugget processor layout/field: "+difference),SdkXmlDiagnostics.Unmasked(node,source));}
        // Reborn: only metadata-bound attributes may vary; absent structural pointers and extra nuggets remain real unsupported layouts.
        void Mask(XmlElement a,XmlElement b){if(b.HasAttribute("id"))a.SetAttribute("id",b.GetAttribute("id"));foreach(var name in a.Attributes.OfType<XmlAttribute>().Concat(b.Attributes.OfType<XmlAttribute>()).Where(x=>x.NamespaceURI.Length==0).Select(x=>x.LocalName).Distinct().ToArray())if(WeaponTokenReproduction.HasNativeField(name)){if(name is "FireFX" or "FireVeteranFX" or "FireSoundPerClip"&&(a.GetAttribute(name).Length==0||b.GetAttribute(name).Length==0))continue;a.RemoveAttribute(name);if(b.HasAttribute(name))a.SetAttribute(name,b.GetAttribute(name));}var left=a.ChildNodes.OfType<XmlElement>().ToArray();var right=b.ChildNodes.OfType<XmlElement>().ToArray();if(left.Length==right.Length)for(int i=0;i<left.Length;i++)if(left[i].LocalName==right[i].LocalName)Mask(left[i],right[i]);}
    }

    // Reborn: the three observed weak/encoded weapon dependency fields keep their actual normalized target identities even though this tokenized profile has no IMP bytes.
    private static void VerifyWeaponReferences(XmlDocument source,Chunk native,InstanceDeclaration instance){if(instance.ReferencedInstances.Count!=3)throw new InvalidDataException("Current EP1 weapon metadata requires its three populated sound/FX slots.");foreach(var field in new[]{("FireSoundPerClip",340),("FireFX",104),("FireVeteranFX",108)}){uint index=Word(native.InstanceBuffer,field.Item2);if(index==0||index>instance.ReferencedInstances.Count)throw new InvalidDataException("Weapon encoded reference index bounds.");string name=source.DocumentElement!.GetAttribute(field.Item1);int colon=name.IndexOf(':');if(colon>=0)name=name[(colon+1)..];if(instance.ReferencedInstances[(int)index-1].InstanceId!=InstanceHandle.GetInstanceId(name))throw new InvalidDataException("Weapon encoded reference differs from authored target.");}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the existing native LogicCommandSet list without a new sample-size quota. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe AssetBuffer CompileSet(InstanceDeclaration instance){var xml=Xml(instance.XmlNode.OuterXml);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);LogicCommandSet* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(LogicCommandSet),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("LogicCommandSet native allocation failed.");return Buffer(chunk);}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin the reused command-set ABI to the actual stock Barracks set using targeted slices, not a guessed KW layout. */
    //-------------------------------------------------------------------------------------------------
    private static void QualifySet(ManifestAsset[] stock,string manifest,SchemaSet schemas){var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));var asset=document.Assets.Single(a=>a.Name=="LogicCommandSet:SovietBarracksCommandSet");var xml=Xml("<LogicCommandSet xmlns='uri:ea.com:eala:asset' id='RebornSetLayoutProbe'/>");foreach(var r in asset.References){var child=xml.CreateElement("Cmd",SchemaSet.XmlNamespace);child.InnerText=stock.Single(a=>a.TypeId==r.TypeId&&a.InstanceId==r.InstanceId).Name;xml.DocumentElement!.AppendChild(child);}var built=CompileSet(ReferencePipelineSmokeTest.Normalize(schemas,xml.OuterXml));foreach(var part in new[]{(".bin",built.InstanceData),(".relo",built.RelocationData),(".imp",built.ImportsData)}){long offset=8;foreach(var preceding in document.Assets.TakeWhile(a=>a!=asset))offset+=part.Item1==".bin"?preceding.InstanceDataSize:part.Item1==".relo"?preceding.RelocationDataSize:preceding.ImportsDataSize;int size=part.Item1==".bin"?asset.InstanceDataSize:part.Item1==".relo"?asset.RelocationDataSize:asset.ImportsDataSize;if(size!=part.Item2.Length||!AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,part.Item1),null,offset,size).SequenceEqual(part.Item2))throw new InvalidDataException("EP1 LogicCommandSet stock/native ABI differs.");}}

    // Reborn: standalone normalization documents acquire a real source name for the shared manifest writer without mutating caller instances.
    private static InstanceDeclaration Attach(InstanceDeclaration source,string path){var document=new AssetDeclarationDocument();var type=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic)!;var state=Activator.CreateInstance(type,true)!;type.GetField("SourcePath")!.SetValue(state,path);typeof(AssetDeclarationDocument).GetField("_current",BindingFlags.NonPublic|BindingFlags.Instance)!.SetValue(document,state);var instance=new InstanceDeclaration(document){XmlNode=Xml(source.XmlNode.OuterXml).DocumentElement!};instance.ReferencedInstances.AddRange(source.ReferencedInstances);instance.ValidatedReferencedInstances=new(instance.ReferencedInstances);return instance;}
    // Reborn: stream buffers own exact freshly compiled native/tokenized arrays.
    private static AssetBuffer Buffer(Chunk chunk)=>new(){InstanceData=chunk.InstanceBuffer,RelocationData=chunk.RelocationBuffer,ImportsData=chunk.ImportsBuffer};
    // Reborn: all source reads retain existing bounded XML parsing and no entity/network resolution.
    private static XmlDocument Read(string path)=>SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(path,1048576),path);
    // Reborn: comparison and native inputs are independent detached XML copies.
    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
    // Reborn: typed native reads remain bounded to the actual emitted asset chunk.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Mixed asset word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
