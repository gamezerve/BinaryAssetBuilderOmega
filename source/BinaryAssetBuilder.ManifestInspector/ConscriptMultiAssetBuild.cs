// Reborn: authored special-power templates participate in shared source, inheritance and native stream publication.
// Reborn: experience templates share ordinary source preparation, inheritance and explicit asset emission.
// Reborn: particle templates participate in the shared source, inheritance and native asset pipeline.
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
    internal static bool TryRun(string entry,string manifest,string global,string output,string? streamContext=null,bool earlyGlobal=false)
    {
        // Reborn: an explicit global dependency context selects the mixed linker even for a single GameObject consumer.
        var external=SdkGlobalStreams.Load(streamContext);
        string additional=Path.Combine(Path.GetDirectoryName(entry)!,"additional.xml");var source=ReadPrepared(entry);bool hasGame=source.DocumentElement!.LocalName=="GameObject";if(!File.Exists(additional)&&hasGame&&external.Length==0&&!earlyGlobal)return false;
        var saved=Settings.Current;
        try{
            var official=ConscriptSourceInput.Schemas();string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalogs=new[]{manifest,global}.Select(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576))).ToArray();foreach(var d in catalogs){TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked||d.Validate().Count!=0)throw new InvalidDataException("Mixed build requires valid EP1 stock catalogs.");}
            var stock=catalogs.SelectMany(d=>d.Assets).ToArray();var sources=new List<XmlDocument>{source};
            // Reborn: global mod definitions override retail identities for name lookup, while their payloads remain in their own streams.
            foreach(var stream in external){var assets=ManifestReader.Read(SdkEnvironmentPreflight.Read(stream.Manifest,64*1048576)).Assets;var ids=assets.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToHashSet();stock=stock.Where(a=>!ids.Contains(new(a.TypeId,a.InstanceId))).Concat(assets).ToArray();}
            if(File.Exists(additional))sources.AddRange(ReadPrepared(additional).DocumentElement!.ChildNodes.OfType<XmlElement>().Select(e=>Xml(e.OuterXml)));
            string command=Path.Combine(Path.GetDirectoryName(entry)!,"command.xml");if(File.Exists(command))sources.Add(ReadPrepared(command));
            // Reborn: retail AudioFile metadata stays SDK-local; authored events reference the loaded audio stream without repacking its samples.
            string? audioManifest=null;if(sources.Any(s=>s.DocumentElement!.LocalName=="AudioEvent")){audioManifest=AuthoredAmbientCompiler.CatalogPath();var audio=ManifestReader.Read(SdkEnvironmentPreflight.Read(audioManifest,64*1048576));TypeRegistryAudit.ValidateTarget(audio.Header.Version,audio.Header.AllTypesHash);if(!audio.Header.IsLinked||audio.Validate().Count!=0)throw new InvalidDataException("Valid linked EP1 audio catalog required.");var known=stock.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToHashSet();stock=stock.Concat(audio.Assets.Where(a=>a.TypeName=="AudioFile"&&!known.Contains(new(a.TypeId,a.InstanceId)))).ToArray();}
            var fingerprints=new Dictionary<uint,(uint Hash,bool Tokenized)>();var locals=new Dictionary<AssetId,XmlDocument>();
            foreach(var xml in sources){string type=xml.DocumentElement!.LocalName,id=xml.DocumentElement.GetAttribute("id");if(type is not ("AudioEvent" or "GameObject" or "ArmorTemplate" or "WeaponTemplate" or "LogicCommand" or "LogicCommandSet" or "UpgradeTemplate" or "ObjectCreationList" or "PurchasePlayerTechButtonTemplate" or "UnitAbilityButtonTemplate" or "PlayerTech" or "ButtonSingleStateData" or "JoypadCommandBarButtonTemplate" or "PlayerPowerButtonTemplate" or "SpecialPowerTemplate" or "AttributeModifier" or "ExperienceLevelTemplate" or "FXParticleSystemTemplate" or "FXList" or "Texture" or "PackedTextureImage" or "W3DContainer" or "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh")||!ConscriptSourceInput.Identity(type,id))throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported mixed asset type or invalid identity: "+type+":"+id),xml.DocumentElement);
                uint typeId=FastHash.GetHashCode(type);var examples=stock.Where(a=>a.TypeId==typeId).Select(a=>(a.TypeHash,Tokenized:a.Tokenized==1)).Distinct().ToArray();if(examples.Length!=1)throw new InvalidDataException("EP1 asset fingerprint is absent/ambiguous: "+type);fingerprints[typeId]=examples[0];if(!locals.TryAdd(new(typeId,InstanceHandle.GetInstanceId(id)),xml))throw SdkXmlDiagnostics.At(new InvalidDataException("DUPLICATEASSET ' "+type+":"+id+" ' Duplicate local asset/hash identity."),xml.DocumentElement);
            }
            // Reborn: source-only catalog entries resolve local names before native data exists; payloads are freshly compiled below, never copied from stock.
            var localEntries=locals.Select(p=>new ManifestAsset(p.Key.TypeId,p.Key.InstanceId,fingerprints[p.Key.TypeId].Hash,0,0,0,0,0,0,0,0,fingerprints[p.Key.TypeId].Tokenized?1u:0u,p.Value.DocumentElement!.LocalName+":"+p.Value.DocumentElement.GetAttribute("id"),"authored.xml",Array.Empty<AssetId>())).ToArray();
            // Reborn: map/WorldBuilder-only models (e.g. VL_RussianPortBuilding) are referenced, not compiled into the mod; the original WorldBuilder manifest only supplies their concrete render type.
            var worldBuilderRender=WorldBuilderRenderAssets(manifest,schemas,stock.Concat(localEntries).ToArray());
            // Reborn: validation owns WorldBuilder diagnostics; compilation retains the same catalog-backed native resolution without repeating warnings.
            var available=stock.Where(a=>!locals.ContainsKey(new(a.TypeId,a.InstanceId))).Concat(localEntries).Concat(worldBuilderRender).ToArray();
            var baseline=Read(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml"));baseline.DocumentElement!.SetAttribute("BuildTime","4");
            // Reborn: each emitted object owns its source admission/template map; never reuse the primary object's modules for another object.
            var plans=sources.Where(s=>s.DocumentElement!.LocalName=="GameObject").ToDictionary(s=>s,s=>ConscriptVariableBehaviorSource.Admit(s,baseline,official));
            // Reborn: immutable stock grammar and source-keyed reference tables belong to this build/catalog generation; every object still owns its own source plan and native verification.
            var graphVerifier=plans.Count==0?null:new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,available);
            // Reborn: optional developer mode compares original FX payloads; current-source native ownership remains mandatory in both modes.
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="FXList"))FXListCompiler.Qualify(manifest,schemas,stock);
            // Reborn: developer qualification reads selected original particle payloads only; actual authored payload proof stays enabled in both modes.
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="FXParticleSystemTemplate"))FXParticleSystemCompiler.Qualify(manifest,schemas,stock);
            var built=new Dictionary<AssetId,Built>();int prefix=0;var compileLog=ConscriptCompileLog.ForPrepared(entry);
            // Reborn: validate the isolated EP1 upgrade ABI against original stock before emitting any authored upgrades.
            // Reborn: developer mode independently checks the actual EP1 experience layout; source/native proof is mandatory in both build modes.
            // Reborn: original special powers independently qualify EP1 target/flag ordinals and EVA/tech imports in developer mode.
            // Reborn: developer mode qualifies original button native graphs; fast mode retains current source/native validation.
            // Reborn: only developer mode repeats original joypad native qualification.
            // Reborn: developer builds qualify original standalone button states without slowing fast builds.
            // Reborn: original technology assets reside in global, while the catalog includes both retail streams.
            // Reborn: developer mode qualifies represented original unit-button data variants from static.
            // Reborn: developer mode qualifies original purchase-button imports from the retail static stream.
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="PurchasePlayerTechButtonTemplate"))PurchaseTechButtonCompiler.Qualify(manifest,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="UnitAbilityButtonTemplate"))UnitAbilityButtonCompiler.Qualify(manifest,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="PlayerTech"))PlayerTechCompiler.Qualify(global,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="ButtonSingleStateData"))ButtonStateCompiler.Qualify(manifest,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="JoypadCommandBarButtonTemplate"))JoypadButtonCompiler.Qualify(manifest,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="PlayerPowerButtonTemplate"))PlayerPowerButtonCompiler.Qualify(manifest,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="SpecialPowerTemplate"))SpecialPowerTemplateCompiler.Qualify(manifest,schemas,stock);
            // Reborn: developer mode compares original modifier roots; authored native ownership remains checked in fast builds too.
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="AttributeModifier"))AttributeModifierCompiler.Qualify(manifest,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="ExperienceLevelTemplate"))ExperienceLevelCompiler.Qualify(manifest,schemas,stock);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="UpgradeTemplate"))UpgradeTemplateCompiler.Qualify(global,schemas,stock);
            // Reborn: ObjectCreationList families/fields are enabled only after original EP1 OCLs exhibiting them pass the same proof.
            var lists=sources.Where(s=>s.DocumentElement!.LocalName=="ObjectCreationList").ToArray();
            // Reborn: qualification retains its actual authored nugget node; enrich failures before temporary prepared-input fallback can replace the source location.
            if(lists.Length!=0)try{ObjectCreationListCompiler.Qualify(manifest,global,schemas,stock,lists);}catch(InvalidDataException error){compileLog.Enrich(error);throw;}
            // Reborn: authored textures/packed images are emitted only after the writers are proved on original EP1 static data.
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="Texture"))Ep1TextureCompiler.Qualify(manifest);
            // Reborn: use the existing W3DContainer marshaler only after its untokenized EP1 stock layout matches.
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="W3DContainer"))W3DContainerCompiler.Qualify(manifest,schemas,available);
            if(SdkBuildTests.Enabled&&sources.Any(s=>s.DocumentElement!.LocalName=="PackedTextureImage"))PackedTextureImageCompiler.Qualify(manifest,schemas,stock);
            // Reborn: ambient/control extensions are qualified on original native data before any authored audio enters a package.
            if(SdkBuildTests.Enabled&&audioManifest!=null)AuthoredAmbientCompiler.Qualify(global,Normalize);
            foreach(var xml in sources){
                string type=xml.DocumentElement!.LocalName;InstanceDeclaration instance;AssetBuffer buffer;
                // Reborn: announce and time the asset around its actual processor/verifier, as EA BuildMod does per instance.
                var progress=compileLog.Begin(type,xml.DocumentElement.GetAttribute("id"));
                if(type=="GameObject"){
                    // Reborn: native errors without an attached XML node still identify their actual authored GameObject, not temporary game.xml.
                    try{instance=Normalize(xml);var native=GameObjectBodyProjectionSmokeTest.Compile(Xml(instance.XmlNode.OuterXml),true,true,true);var tokens=graphVerifier!.Verify(xml,native,instance,plans[xml].Origins);if(ReferenceEquals(xml,source))prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;buffer=Buffer(tokens);}
                    catch(Exception error)when(error is InvalidDataException or FormatException or OverflowException){compileLog.NativeError(error,xml);throw;}
                    // Reborn: weak CommandSet availability is reported consistently by the common mod/stock audit, without pretending it is a native import.
                }else if(type=="AudioEvent"){
                    instance=Normalize(xml);buffer=AuthoredAmbientCompiler.Compile(instance);
                }else if(type=="ArmorTemplate"){
                    instance=Normalize(xml);var plugin=new Ra3Ep1ArmorPlugin();plugin.Initialize(TargetPlatform.Win32);buffer=plugin.ProcessInstance(instance);
                }else if(type=="WeaponTemplate"){
                    AdmitWeapon(xml,official,fixtures);instance=Normalize(xml);var copy=Xml(instance.XmlNode.OuterXml);foreach(XmlElement nugget in copy.GetElementsByTagName("DamageNugget",SchemaSet.XmlNamespace))nugget.SetAttribute("TypeId",FastHash.GetHashCode("DamageNuggetType").ToString(System.Globalization.CultureInfo.InvariantCulture));
                    var native=WeaponNativeProbe.Compile(copy);VerifyWeaponReferences(xml,native,instance);buffer=Buffer(WeaponTokenReproduction.EncodeAuthored(native));
                }else if(type=="PurchasePlayerTechButtonTemplate"){
                    // Reborn: local/stock technology and state imports use the same typed resolution as other UI assets.
                    instance=Normalize(xml);var native=PurchaseTechButtonCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(PurchaseTechButtonCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1PurchasePlayerTechButtonTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="UnitAbilityButtonTemplate"){
                    // Reborn: each unit button owns its command import and independently proved concrete EP1 Data payload.
                    instance=Normalize(xml);var native=UnitAbilityButtonCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(UnitAbilityButtonCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1UnitAbilityButtonTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="PlayerTech"){
                    // Reborn: technology nodes retain their typed dependency imports and weak grant-FX identities in either supported stream.
                    instance=Normalize(xml);var native=PlayerTechCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(PlayerTechCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1PlayerTech),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="ButtonSingleStateData"){
                    // Reborn: button-state images retain real imports and all three localized-key string descriptors are authored normally.
                    instance=Normalize(xml);var native=ButtonStateCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(ButtonStateCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1ButtonSingleStateData),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="JoypadCommandBarButtonTemplate"){
                    // Reborn: joypad buttons use standalone untokenized EP1 records with schema-selected concrete data and real imports.
                    instance=Normalize(xml);var native=JoypadButtonCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(JoypadButtonCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1JoypadCommandBarButtonTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="PlayerPowerButtonTemplate"){
                    // Reborn: standalone EP1 power-button roots retain their concrete Data variant and typed imports.
                    instance=Normalize(xml);var native=PlayerPowerButtonCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(PlayerPowerButtonCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1PlayerPowerButtonTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="SpecialPowerTemplate"){
                    // Reborn: fresh EP1 special-power native data carries source-owned filters, dependency arrays and Core import selectors.
                    instance=Normalize(xml);var native=SpecialPowerTemplateCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(SpecialPowerTemplateCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1SpecialPowerTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="AttributeModifier"){
                    // Reborn: modifiers use the existing native EP1 writer, with source-sized lists/masks and real local/stock FX and shader imports.
                    instance=Normalize(xml);var native=AttributeModifierCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(AttributeModifierCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(AttributeModifier),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="ExperienceLevelTemplate"){
                    // Reborn: experience targets/upgrades are weak handles; prerequisite, FX, modifiers and OCL retain real Core strong imports.
                    instance=Normalize(xml);var native=ExperienceLevelCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(ExperienceLevelCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1ExperienceLevelTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="UpgradeTemplate"){
                    // Reborn: stock upgrades use native-only streams; Core still owns real strong-reference occurrence indices.
                    instance=Normalize(xml);var native=UpgradeTemplateCompiler.Compile(instance.XmlNode);
                    SchemaNativeContract.Verify(UpgradeTemplateCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1UpgradeTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="ObjectCreationList"){
                    // Reborn: untokenized EP1 OCL native stream; Core still owns strong-reference occurrence indices.
                    instance=Normalize(xml);var native=ObjectCreationListCompiler.Compile(instance.XmlNode);
                    SchemaNativeContract.Verify(ObjectCreationListCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1ObjectCreationList),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="FXParticleSystemTemplate"){
                    // Reborn: compile particle templates with real texture/FX/audio imports and schema-selected native modules.
                    instance=Normalize(xml);var native=FXParticleSystemCompiler.Compile(instance.XmlNode,official);SchemaNativeContract.Verify(FXParticleSystemCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(FXParticleSystemTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="FXList"){
                    // Reborn: general untokenized FXList compilation preserves real Core reference indices and source-sized polymorphic nugget order.
                    instance=Normalize(xml);var native=FXListCompiler.Compile(instance.XmlNode);SchemaNativeContract.Verify(FXListCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(FXList),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="Texture"){
                    // Reborn: PC texture = 12-byte root + DDS; the TGA is the content-addressed copy captured during preparation.
                    instance=Normalize(xml);string resource=TextureResource(entry,xml.DocumentElement.GetAttribute("File"));var payload=SdkEnvironmentPreflight.Read(resource,64*1048576);
                    if(!string.Equals(Path.GetFileNameWithoutExtension(resource),Ep1TextureCompiler.Sha256(payload),StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Captured texture payload changed: "+resource);
                    var native=Ep1TextureCompiler.Compile(xml.DocumentElement,payload,resource);Console.WriteLine("Texture "+xml.DocumentElement.GetAttribute("id")+": "+Ep1TextureCompiler.Describe(native)+".");buffer=Buffer(native);
                }else if(type is "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh"){
                    // Reborn: mesh shader/texture references retain normal Core indices; EP1 hierarchy pivots use Poid hashes rather than KW strings.
                    instance=Normalize(xml);var native=W3DGeometryCompiler.Compile(instance,schemas);buffer=Buffer(native);
                }else if(type=="W3DContainer"){
                    // Reborn: hierarchy/mesh/collision imports are normalized by Core; authoring a container never copies their stock payloads.
                    instance=Normalize(xml);var native=W3DContainerCompiler.Compile(instance.XmlNode);W3DContainerCompiler.Verify(instance,native);buffer=Buffer(native);
                }else if(type=="PackedTextureImage"){
                    // Reborn: untokenized 36-byte record; Core owns the Texture reference occurrence index.
                    instance=Normalize(xml);var native=PackedTextureImageCompiler.Compile(instance.XmlNode);
                    SchemaNativeContract.Verify(PackedTextureImageCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(PackedTextureImage),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else if(type=="LogicCommand"&&LogicCommandCompiler.IsRecruitment(xml)){
                    var result=ConscriptRecruitmentSmokeTest.CompileXml(schemas,xml.OuterXml);instance=Attach(result.Instance,xml.DocumentElement.GetAttribute("id")+".xml");buffer=result.Chunk;
                }else if(type=="LogicCommand"){
                    // Reborn: every other EP1 command shape uses the general route, proved first on the original static commands.
                    LogicCommandCompiler.Admit(xml);if(SdkBuildTests.Enabled)LogicCommandCompiler.Qualify(manifest,schemas,stock);instance=Normalize(xml);var native=LogicCommandCompiler.Compile(instance.XmlNode);
                    SchemaNativeContract.Verify(LogicCommandCompiler.ProofSource(xml,schemas,available).DocumentElement!,typeof(Ep1LogicCommand),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());buffer=Buffer(native);
                }else{
                    if(SdkBuildTests.Enabled)QualifySet(stock,manifest,schemas);instance=Normalize(xml);buffer=CompileSet(instance);
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
            // Reborn: emitted AudioEvents import original AudioFiles from the runtime audio stream, in addition to existing static/global references.
            // Reborn: restore the pre-regression reference spelling; native prefix observations alone did not prove safe runtime reader/cache ownership.
            var physical=new[]{Path.GetFullPath(manifest),Path.GetFullPath(global)}.Concat(audioManifest==null?Array.Empty<string>():new[]{audioManifest}).ToArray();var runtimeNames=new[]{"data\\static.manifest","data\\global.manifest"}.Concat(audioManifest==null?Array.Empty<string>():new[]{"data\\audio.manifest"}).ToArray();
            // Reborn: publish actual global manifest references rather than merging early-load assets into mod.manifest.
            physical=physical.Concat(external.Select(s=>s.Manifest)).ToArray();runtimeNames=runtimeNames.Concat(external.Select(s=>s.Runtime.ToLowerInvariant())).ToArray();
            // Reborn: early globals must not pull the entire static stream into startup just because the compiler used its lookup catalog.
            if(earlyGlobal){
                var needed=instances.SelectMany(i=>i.ValidatedReferencedInstances!).Select(r=>new AssetId(r.TypeId,r.InstanceId)).Where(id=>!locals.ContainsKey(id)).ToHashSet();
                var providers=physical.Select(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToHashSet()).ToArray();var used=new HashSet<int>();
                foreach(var id in needed){int provider=Array.FindLastIndex(providers,p=>p.Contains(id));if(provider<0)throw new InvalidDataException("Global import has no runtime manifest provider: "+id);if(runtimeNames[provider]=="data\\static.manifest")throw new InvalidDataException("Global source imports a static-only asset before static subsystems are loaded: "+available.Single(a=>a.TypeId==id.TypeId&&a.InstanceId==id.InstanceId).Name);used.Add(provider);}
                runtimeNames=runtimeNames.Where((_,i)=>used.Contains(i)).ToArray();physical=physical.Where((_,i)=>used.Contains(i)).ToArray();
            }
            var runtime=new ReferencedFileBuffer();typeof(OutputManager).GetMethod("AddExternalManifestReferences",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{runtime,physical,string.Join(";",runtimeNames)});
            Console.WriteLine("Generating stream: "+instances.Length+" assets, output checksum "+checksum.ToString("x8")+".");
            // Reborn: one serialization/readback is mandatory; only developer builds repeat it to test determinism.
            var streams=BoundedDiagnosticBuild.SerializeAssetGraph(instances,chunks,checksum,runtime,flags,fingerprints);if(SdkBuildTests.Enabled){var repeated=BoundedDiagnosticBuild.SerializeAssetGraph(instances,chunks,checksum,runtime,flags,fingerprints);if(streams.Any(p=>!p.Value.SequenceEqual(repeated[p.Key])))throw new InvalidDataException("Mixed stream serialization is not deterministic.");}
            Directory.CreateDirectory(output);string verification=Path.Combine(output,"stream-readback");ModifierFXStreamSmokeTest.WriteNew(verification,streams);BoundedDiagnosticBuild.Verify(verification,instances,chunks,streams,checksum,runtimeNames,flags);
            // Reborn: final static/global usage auditing also needs weak handles, which are intentionally absent from native import tables.
            SdkUnusedAssetAudit.WriteUsage(output,built.Values.Select(b=>(b.Instance,b.Source)),schemas,available);
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
    //-------------------------------------------------------------------------------------------------
    /** Reborn: render assets (BaseRenderAssetType family) present only in the original EP1 WorldBuilder stream, for type resolution of authored model references.
        Entries whose instance already resolves as a render asset in stock/local data are excluded, so stock and mod targets always win and nothing becomes ambiguous. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestAsset[] WorldBuilderRenderAssets(string staticManifest,SchemaSet schemas,ManifestAsset[] known)
    {
        // Reborn: use the same SDK-local catalog as the resolver, so type lookup and copied native payloads cannot select different WorldBuilder generations.
        string path=WorldBuilderAssetResolver.CatalogPath();
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);if(!document.Header.IsLinked||document.Validate().Count!=0)throw new InvalidDataException("Valid linked EP1 WorldBuilder stream required: "+path);
        var required=schemas.GetXmlType("BaseRenderAssetType")??throw new InvalidDataException("BaseRenderAssetType schema missing.");
        var render=schemas.Schemas.GlobalTypes.Values.Cast<System.Xml.Schema.XmlSchemaType>().Where(t=>System.Xml.Schema.XmlSchemaType.IsDerivedFrom(t,required,System.Xml.Schema.XmlSchemaDerivationMethod.None)).Select(t=>FastHash.GetHashCode(t.Name!)).ToHashSet();
        var resolved=known.Where(a=>render.Contains(a.TypeId)).Select(a=>a.InstanceId).ToHashSet();
        // Reborn: hierarchy/texture/shader dependencies use complete typed identities, so a container sharing the hierarchy's name does not hide it.
        var knownIds=known.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToHashSet();
        return document.Assets.Where(a=>render.Contains(a.TypeId)&&!resolved.Contains(a.InstanceId)).GroupBy(a=>a.InstanceId).Where(g=>g.Count()==1).Select(g=>g.Single())
            .Concat(document.Assets.Where(a=>!render.Contains(a.TypeId)&&!knownIds.Contains(new(a.TypeId,a.InstanceId)))).ToArray();
    }

    private static void PackageSupport(string output,XmlDocument source,Dictionary<string,byte[]> streams,InstanceDeclaration[] instances,ManifestAsset[] stock)
    {
        string directory=Path.Combine(output,"support");Directory.CreateDirectory(directory);string[] extensions={".manifest",".bin",".relo",".imp"};var payloads=extensions.Select(e=>streams["diagnostic"+e]).ToArray();byte[] bytes=ArmorDiagnosticPackage.Serialize(payloads,true,true);string path=Path.Combine(directory,"RebornConscriptProbe.big");File.WriteAllBytes(path,bytes);
        // Reborn: texture/model support streams can exceed the old command-set fixture quota; readback is bounded by each exact generated payload.
        using(var archive=BigArchive.Open(path,15,4096)){if(archive.Entries.Count!=15)throw new InvalidDataException("Support package entry count differs.");foreach(string suffix in new[]{"","_l","_m"})foreach(string extension in extensions.Concat(new[]{".version"})){var entry=archive.Entries.Single(e=>e.Name=="data\\mod"+suffix+extension);byte[] expected=extension==".version"?Encoding.ASCII.GetBytes("\r\n"):streams["diagnostic"+extension];if(!archive.ReadEntry(entry,expected.Length).SequenceEqual(expected))throw new InvalidDataException("Support package readback differs.");}}
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
    // Reborn: prepared Texture/@File values are exactly content-addressed TGA/DDS files beside the entry; retain the captured format's extension.
    private static string TextureResource(string entry,string file)
    {
        var match=System.Text.RegularExpressions.Regex.Match(file,"^resources/([0-9A-F]{64})\\.(?:tga|dds)$");if(!match.Success)throw new InvalidDataException("Texture File was not captured by source preparation: "+file);
        return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(entry))!,file.Replace('/',Path.DirectorySeparatorChar));
    }

    private static AssetBuffer Buffer(Chunk chunk)=>new(){InstanceData=chunk.InstanceBuffer,RelocationData=chunk.RelocationBuffer,ImportsData=chunk.ImportsBuffer};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generated merged XML scales with its captured assets rather than the per-source four-MiB quota; DTD/entity resolution stays disabled. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ReadPrepared(string path)=>SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(path,int.MaxValue),path);
    // Reborn: authored fixture reads retain their individual source quota.
    private static XmlDocument Read(string path)=>SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(path,4*1048576),path);
    // Reborn: comparison and native inputs are independent detached XML copies.
    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
    // Reborn: typed native reads remain bounded to the actual emitted asset chunk.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Mixed asset word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
