using System.Buffers.Binary;
using System.Reflection;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify variable complete-object behavior composition without weakening the fixed alpha publication oracle.
internal static class ConscriptModuleGraphSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile removed/reordered/cloned behavior graphs on one Core/Tracker and independently verify every native owner against stock. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest,string output,string? caseFilter=null)
    {
        output=SdkEnvironmentPreflight.Absolute(output);SdkEnvironmentPreflight.CheckPath(output);
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh module-graph output required.");
        SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(output)!);Directory.CreateDirectory(output);
        var saved=Settings.Current;
        try
        {
            var official=ConscriptSourceInput.Schemas();string fixtureRoot=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtureRoot,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,globalManifest}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name);
            var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtureRoot,"live-alpha","ConscriptBuildTime1","data","mod.xml")));baseline.DocumentElement!.SetAttribute("BuildTime","4");
            var templates=ConscriptModuleScalarContract.Owners(baseline).ToDictionary(o=>o.Element.GetAttribute("id"),o=>topology.Containers.Single(c=>c.Name==o.Container).Modules[o.Index],StringComparer.Ordinal);
            var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            string snapshot=baseline.OuterXml;var results=new List<object>();int faults=0,metadataFaults=0;
            // Reborn: string-based SageUnsignedInt schema syntax can admit ten decimal digits; native overflow must still reject at the real source attribute.
            var overflow=(XmlDocument)baseline.CloneNode(true);var overflowOwner=(XmlElement)overflow.GetElementsByTagName("ModelConditionUpgrade",SchemaSet.XmlNamespace)[0]!;var overflowAnimation=overflow.CreateElement("CustomAnimAndDuration",SchemaSet.XmlNamespace);overflowAnimation.SetAttribute("Frames","4294967296");overflowOwner.PrependChild(overflowAnimation);bool overflowRejected=false;
            try{ConscriptVariableBehaviorSource.Admit(overflow,baseline,official);}catch(InvalidDataException error){overflowRejected=SdkXmlDiagnostics.Node(error)?.LocalName=="Frames";}
            if(!overflowRejected)throw new InvalidDataException("Nested unsigned overflow/source coordinate control failed.");
            // Reborn: allow a focused proof run when extending one capability without replaying unrelated expensive native matrices.
            string[] modes={"original","reverse","remove-half","clone-twelve","combined","new-id-combined","retarget-combined","beyond-former-budget","upgrade-lists-expanded","upgrade-lists-empty","upgrade-animation","upgrade-animation-defaults","upgrade-list-large","upgrade-animation-lexical","root-text-edited","root-text-empty","root-text-omitted","combat-combined","combat-omitted","combat-single","shared-parity","shared-defaults","nested-values","nested-defaults","movement-expanded","movement-omitted","movement-defaults"};
            // Reborn: resource presence, empty lists, explicit zero, real uint32 boundaries and multiple rows are separate schema-valid states.
            modes=modes.Concat(new[]{"resource-omitted","resource-empty","resource-zero","resource-expanded","resource-uint32"}).ToArray();
            // Reborn: one new complete scalar module is proven with explicit edits, schema defaults and repeated reordered instances.
            modes=modes.Concat(new[]{"sell-edited","sell-defaults","sell-multiple"}).ToArray();
            // Reborn: destruction composition covers real FX imports, absent optional fields and a source list beyond the former quota.
            modes=modes.Concat(new[]{"destruction-edited","destruction-defaults","destruction-expanded"}).ToArray();
            // Reborn: docking exercises every authored string, schema-empty defaults and two independently owned reordered modules.
            modes=modes.Concat(new[]{"dock-edited","dock-defaults","dock-multiple"}).ToArray();
            // Reborn: queue-exit proves real nonzero time units, packed booleans, both inline vectors and source-driven repeated modules.
            modes=modes.Concat(new[]{"exit-edited","exit-defaults","exit-multiple"}).ToArray();
            // Reborn: exercise populated/default/duplicate Spawn graphs as one combined module-family batch.
            modes=modes.Concat(new[]{"spawn-edited","spawn-defaults","spawn-multiple"}).ToArray();
            // Reborn: unpack tests combine authored timing/height, optional sound presence and repeated independently named instances.
            modes=modes.Concat(new[]{"unpack-edited","unpack-defaults","unpack-multiple","unpack-empty"}).ToArray();
            // Reborn: production proves every EP1 scalar with edited values, schema defaults and two independently named instances.
            modes=modes.Concat(new[]{"production-edited","production-defaults","production-multiple","production-quantity","production-modifier"}).ToArray();
            // Reborn: repair proves own scalars and inline percent records with edits, defaults and two instances.
            modes=modes.Concat(new[]{"repair-edited","repair-defaults","repair-multiple","repair-audio"}).ToArray();
            // Reborn: engineer proves voices, retargeted FX/EVA imports, status masks and source-sized filters, plus defaults and two instances.
            modes=modes.Concat(new[]{"engineer-edited","engineer-defaults","engineer-multiple"}).ToArray();
            // Reborn: assisted placement proves hint masks, snapping distance and an optional source-sized filter.
            modes=modes.Concat(new[]{"placement-edited","placement-defaults","placement-multiple"}).ToArray();
            // Reborn: exercise defaults and explicit values for the reviewed dock, ownership and gate classes together.
            modes = modes.Append("reviewed-ep1-behaviors").ToArray();
            // Reborn: infiltrator proves edited values/references, schema-default imports and two instances.
            modes=modes.Concat(new[]{"infiltrator-edited","infiltrator-defaults","infiltrator-multiple"}).ToArray();
            // Reborn: structure placement proves every Shape field, optional strings/Offset, defaults and two instances.
            modes=modes.Concat(new[]{"structure-edited","structure-defaults","structure-multiple"}).ToArray();
            // Reborn: LUAEventList proves the owned name string at padded/unpadded lengths and two instances.
            modes=modes.Concat(new[]{"lua-edited","lua-short","lua-multiple"}).ToArray();
            // Reborn: legacy PowerUpgrade proves flags, source-sized trigger lists and optional animation, schema defaults and two instances.
            modes=modes.Concat(new[]{"power-edited","power-defaults","power-multiple"}).ToArray();
            // Reborn: schema-proved scripted draws: the six real SovietRefinery draws and an edited Conscript draw with new nested states.
            modes=modes.Concat(new[]{"draws-refinery","draws-edited","draws-clients","draws-material","draws-material-empty","draws-material-defaults"}).ToArray();
            // Reborn: FX proof covers the real building death shape, every EP1 event kind and source-sized optional/list storage together.
            modes=modes.Concat(new[]{"fx-building","fx-expanded","fx-multiple"}).ToArray();
            // Reborn: tracker tests combine the exact building shape, schema-empty/default fields and multiple expanded effect lists.
            modes=modes.Concat(new[]{"tracker-building","tracker-empty","tracker-multiple"}).ToArray();
            // Reborn: one batch qualifies normal/water creation plus destroy, populated strong upgrade/death storage and schema defaults.
            modes=modes.Concat(new[]{"death-refinery","death-expanded","death-defaults"}).ToArray();
            // Reborn: reviewed draw/client counts and ordering are source-driven, with omitted and allocated-empty ownership tested separately.
            modes=modes.Concat(new[]{"containers-reverse","containers-expanded","containers-omitted","containers-empty","containers-large"}).ToArray();
            // Reborn: real building geometry, source defaults and lists beyond old diagnostic quotas share complete-object stream/BIG proof.
            modes=modes.Concat(new[]{"geometry-refinery","geometry-defaults","geometry-expanded"}).ToArray();
            // Reborn: building prerequisites exercise authored empty, original refinery, expanded weak/mask and nested filter storage together.
            modes=modes.Concat(new[]{"dependency-empty","dependency-refinery","dependency-expanded","dependency-filter"}).ToArray();
            // Reborn: one batch exercises the building's actual optional roots plus expanded audio/shadow/projected source graphs.
            modes=modes.Concat(new[]{"roots-refinery","roots-defaults","roots-expanded"}).ToArray();
            // Reborn: AI root coverage includes the real refinery criteria, defaults, omission and expanded optional masks/filters.
            modes=modes.Concat(new[]{"skirmish-refinery","skirmish-defaults","skirmish-omitted","skirmish-expanded"}).ToArray();
            // Reborn: one AI batch proves the original building's absent chooser, authored defaults and independently edited chooser/native imports.
            modes=modes.Concat(new[]{"chooser-refinery","chooser-defaults","chooser-expanded"}).ToArray();
            // Reborn: root voice/EVA authoring preserves omission, allocated-empty and expanded strong-reference occurrences.
            modes=modes.Concat(new[]{"audio-refinery","audio-empty","audio-omitted","audio-expanded"}).ToArray();
            // Reborn: building crusher levels, omission/defaults and every existing scalar/weapon field compile together through the real complete-object route.
            modes=modes.Concat(new[]{"crusher-building","crusher-defaults","crusher-omitted","crusher-edited"}).ToArray();
            // Reborn: real Power Plant window modules, absent optional strings and all four authored strings share complete-object package proof.
            modes=modes.Concat(new[]{"windows-stock","windows-defaults","windows-edited"}).ToArray();
            // Reborn: one focused batch exercises original defaults, edited SpecialPower values/list storage, and repeated assign-slaves modules.
            modes=modes.Concat(new[]{"assign-defaults","assign-edited","assign-multiple"}).ToArray();
            // Reborn: run each SquishCollide flag in isolation through the normal EP1 source pipeline.
            modes = modes.Concat(new[] { "squish-vehicles", "squish-allies" }).ToArray();
            if (caseFilter == "squish")
            {
                modes = modes.Where(mode => mode.StartsWith("squish-", StringComparison.Ordinal)).ToArray();
                caseFilter = null;
            }
            // Reborn: exercise both offsets and native defaults in a complete named environment module.
            modes = modes.Append("destructible-environment").ToArray();
            // Reborn: exercise the two named EP1 behaviors independently through ordinary GameObject builds.
            modes = modes.Concat(new[] { "locomotor-ability", "replace-self-upgrade", "substructure-build", "delayed-weapon-fire", "leech-targeting", "transport-ai", "wander-ai", "emp-update", "weapon-contained", "weapon-scatter", "give-restore-upgrade", "activation-ability", "naval-modifier", "inactive-body", "shield-body", "shield-body-default", "create-enter", "status-ability", "special-power-update", "damage-field", "horde-ai", "supply-truck-ai", "bezier-spline", "sphere-update", "steal-unit", "booby-trap" }).ToArray();
            if(caseFilter=="assign"){modes=modes.Where(m=>m.StartsWith("assign-",StringComparison.Ordinal)).ToArray();caseFilter=null;}
            if(caseFilter=="skirmish"){
                var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");int rootTable=GameObjectStockReview.Tokens(captured.Bytes,0)[FastHash.GetHashCode("SkirmishAIInformation")].Offset;
                var criteria=GameObjectStockReview.Tokens(captured.Bytes,rootTable)[FastHash.GetHashCode("ResourceNodeBaseClosenessTestSelectionCriteria")];int criteriaNative=(int)Word(captured.Bytes,captured.Projection.NativeRoot+468+0);criteriaNative=(int)Word(captured.Bytes,criteriaNative+76);
                foreach(var p in GameObjectStockReview.Tokens(captured.Bytes,criteria.Offset))Console.WriteLine($"AI criteria stock: field={p.Key:X8}, type={p.Value.Type:X8}, offset={p.Value.Offset-criteriaNative}.");
            }
            if(caseFilter=="roots"){
                // Reborn: show only independently captured type signatures needed by the new root writer, never full asset buffers.
                var audioStock=GameObjectStockReview.Capture(manifest,"GameObject:SovietPowerPlant");int audioTable=GameObjectStockReview.Tokens(audioStock.Bytes,0)[FastHash.GetHashCode("AudioArraySound")].Offset;var audioRows=GameObjectStockReview.Tokens(audioStock.Bytes,audioTable);var entry=audioRows[FastHash.GetHashCode("AudioEntry")];int itemTable=(int)Word(audioStock.Bytes,entry.Offset+20);var itemRows=GameObjectStockReview.Tokens(audioStock.Bytes,itemTable);
                var projectedStock=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");int projectedTable=GameObjectStockReview.Tokens(projectedStock.Bytes,0)[FastHash.GetHashCode("ProjectedBuildabilityInfo")].Offset;var projectedRows=GameObjectStockReview.Tokens(projectedStock.Bytes,(int)Word(projectedStock.Bytes,projectedTable+20));
                Console.WriteLine($"Root stock signatures: AudioSoundEntry[]={entry.Type:X8}; ThingTemplateSoundType={itemRows[FastHash.GetHashCode("AudioType")].Type:X8}; BuildPlacementTypeBitFlags={projectedRows[FastHash.GetHashCode("BuildPlacementTypes")].Type:X8}.");
            }
            if(caseFilter!=null&&caseFilter is not ("combat" or "movement" or "resource" or "sell" or "destruction" or "dock" or "exit" or "spawn" or "unpack" or "production" or "repair" or "engineer" or "placement" or "infiltrator" or "structure" or "lua" or "fx" or "tracker" or "death" or "containers" or "power" or "draws" or "geometry" or "dependency" or "roots" or "skirmish" or "chooser" or "audio" or "crusher" or "windows")&&!modes.Contains(caseFilter,StringComparer.Ordinal))throw new InvalidDataException("Unknown module graph proof case.");
            if(caseFilter=="death"){
                // Reborn: malformed death conditions fail before native allocation while optional data remains genuinely optional.
                foreach(string fragment in new[]{"", "<DieMuxData DeathTypes='NOT_A_DEATH'/>","<DieMuxData RequiredStatus='NOT_A_STATUS'/>","<DieMuxData MinKillerAngle='bad'/>"}){var bad=Copy(baseline);var module=bad.CreateElement("CreateObjectDie",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornDeathBad");module.InnerXml=fragment;((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});}
                Console.WriteLine("Death source controls: PASS (missing DieMuxData and invalid death/status/angle rejected).");
            }
            if(caseFilter=="tracker"){
                // Reborn: malformed nested values fail before allocation; valid omissions remain enabled.
                foreach(string child in new[]{"<EffectInfo/>","<EffectInfo Fraction='bad'/>","<EffectInfo Fraction='1' Options='NOT_AN_OPTION'/>","<Color R='1' G='1'/>","<ObjectStatusValidation RequiredStatus='NOT_A_STATUS'/>"}){
                    var bad=Copy(baseline);var module=bad.CreateElement("SecondaryDamageTracker",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornTrackerBad");module.InnerXml=child;((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("Tracker source controls: PASS (missing/invalid Fraction, invalid option/status and incomplete RGB rejected).");
            }
            if(caseFilter=="resource")
            {
                foreach(string name in new[]{"Account","Amount"}){var bad=Copy(baseline);GameObjectResourceContract.Entries(bad)[0].RemoveAttribute(name);Refuse(()=>ConscriptVariableBehaviorSource.Admit(bad,baseline,official));}
                foreach(string value in new[]{"-1","4294967296","not-a-number"}){var bad=Copy(baseline);GameObjectResourceContract.Entries(bad)[0].SetAttribute("Amount",value);Refuse(()=>ConscriptVariableBehaviorSource.Admit(bad,baseline,official));}
                var duplicate=Copy(baseline);duplicate.DocumentElement!.AppendChild(duplicate.ImportNode(GameObjectResourceContract.Owner(baseline)!,true));Refuse(()=>ConscriptVariableBehaviorSource.Admit(duplicate,baseline,official));
                Console.WriteLine("Resource schema controls: PASS (Account/Amount required, unsigned overflow/negative/invalid value and duplicate owner rejected).");
            }
            if(caseFilter=="production")
            {
                // Reborn: invalid production scalars and malformed lists must be refused at their own source node.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>{var modifier=d.CreateElement("ProductionModifier",SchemaSet.XmlNamespace);m.AppendChild(modifier);},
                    (d,m)=>m.SetAttribute("MaxQueueEntries","4294967296"),
                    (d,m)=>m.SetAttribute("Type","SPECIALPOWERS"),
                    (d,m)=>m.SetAttribute("NewObjectFadeInTime","not-a-time"),
                    // Reborn: a present upgrade wrapper requires at least one translation under the official XSD.
                    (d,m)=>m.AppendChild(d.CreateElement("GiveUpgradeToProduced",SchemaSet.XmlNamespace))})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("ProductionUpdate",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornProductionBad");((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("Production source controls: PASS (missing ModifierFilter, uint overflow, legacy SPECIALPOWERS and invalid Time rejected).");
            }
            if(caseFilter=="draws")
            {
                // Reborn: qualify the generic schema proof on the real stock SovietRefinery draws compiled by EA from the SDK source.
                var refinery=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var refineryModules=GameObjectModuleReview.Inspect(manifest,refinery.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==0x4ECF2A4Bu).ToArray();
                var probe=Copy(baseline);var probeDraws=ConscriptVariableContainers.Find(probe,"Draws")!;foreach(var child in probeDraws.ChildNodes.OfType<XmlElement>().ToArray())probeDraws.RemoveChild(child);foreach(var draw in RefineryDraws(fixtureRoot))probeDraws.AppendChild(probe.ImportNode(draw,true));
                var resolvedProbe=SchemaNativeContract.Resolve(probe,schemas,catalog);var probeElements=ConscriptVariableContainers.Find(resolvedProbe,"Draws")!.ChildNodes.OfType<XmlElement>().ToArray();
                if(refineryModules.Length==0||refineryModules.Length!=probeElements.Length)throw new InvalidDataException("SovietRefinery scripted draw inventory differs from the SDK source.");
                foreach(var module in refineryModules){var element=probeElements.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==Word(refinery.Bytes,module.Native+4));SchemaNativeContract.VerifyStock(element,typeof(W3DScriptedModelDrawModuleData),0x4ECF2A4Bu,refinery.Bytes,module.Native,refinery.Asset.References.ToArray());}
                Console.WriteLine("Schema draw stock qualification: PASS ("+refineryModules.Length+" SovietRefinery scripted draws derived from the SDK source match stock native bytes).");
                // Reborn: the inherited BaseStructure client behaviors qualify the client families against the same stock refinery.
                var clientModules=GameObjectModuleReview.Inspect(manifest,refinery.Asset.Name).Containers.Single(c=>c.Name=="ClientBehaviors").Modules;
                var clientProbe=Copy(baseline);var clientList=ConscriptVariableContainers.Find(clientProbe,"ClientBehaviors")!;foreach(var child in clientList.ChildNodes.OfType<XmlElement>().ToArray())clientList.RemoveChild(child);foreach(var client in SdkLeaves(fixtureRoot,"BaseObjects/BaseStructure.xml","ClientBehaviors").Concat(SdkLeaves(fixtureRoot,"Soviet/Structures/SovietRefinery.xml","ClientBehaviors")))clientList.AppendChild(clientProbe.ImportNode(client,true));
                var clientElements=ConscriptVariableContainers.Find(SchemaNativeContract.Resolve(clientProbe,schemas,catalog),"ClientBehaviors")!.ChildNodes.OfType<XmlElement>().ToArray();int clientQualified=0;
                foreach(var module in clientModules){var family=SchemaNativeContract.Families.SingleOrDefault(f=>f.Container=="ClientBehaviors"&&f.TypeId==module.TypeId);if(family==null)continue;var element=clientElements.SingleOrDefault(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==Word(refinery.Bytes,module.Native+4));if(element==null)continue;SchemaNativeContract.VerifyStock(element,family.Type,family.TypeId,refinery.Bytes,module.Native,refinery.Asset.References.ToArray());clientQualified++;}
                if(clientQualified==0)throw new InvalidDataException("No stock SovietRefinery client behavior qualified the schema proof.");
                Console.WriteLine("Schema client stock qualification: PASS ("+clientQualified+" SovietRefinery client behaviors derived from the SDK BaseStructure/SovietRefinery sources match stock native bytes).");
                // Reborn: an invalid enum and an unresolvable model are refused before compilation.
                foreach(var mutate in new Action<XmlElement>[]{d=>d.SelectNodes("*[local-name()='AnimationState']/*[local-name()='Animation']")!.OfType<XmlElement>().First().SetAttribute("AnimationMode","NOT_A_MODE"),d=>d.SelectNodes("*[local-name()='ModelConditionState']/*[local-name()='Model']")!.OfType<XmlElement>().First().SetAttribute("Name","RebornMissingModel")})
                {
                    var bad=Copy(baseline);var draws=ConscriptVariableContainers.Find(bad,"Draws")!;var draw=(XmlElement)bad.ImportNode(RefineryDraws(fixtureRoot)[0],true);draw.SetAttribute("id","RebornBadDraw");draws.AppendChild(draw);mutate(draw);
                    Refuse(()=>{ConscriptVariableBehaviorSource.Admit(bad,baseline,official);SchemaNativeContract.Resolve(bad,schemas,catalog);});
                }
                Console.WriteLine("Schema draw source controls: PASS (invalid AnimationMode and unresolvable model rejected).");
            }
            if(caseFilter=="power")
            {
                // Reborn: missing IDs, invalid flags and undeclared children remain errors; missing weak targets are not malformed XML.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>m.RemoveAttribute("id"),
                    (d,m)=>m.SetAttribute("Permanent","maybe"),
                    (d,m)=>m.AppendChild(d.CreateElement("Offset",SchemaSet.XmlNamespace))})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("PowerUpgrade",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornPowerUpgradeBad");((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);foreach(var owner in ConscriptModuleScalarContract.Owners(bad))_=ConscriptUpgradeStructure.Read(owner,catalog);});
                }
                Console.WriteLine("PowerUpgrade source controls: PASS (missing id, invalid flag and undeclared child rejected).");
            }
            if(caseFilter=="lua")
            {
                // Reborn: a missing, empty or non-ASCII name and any child element are refused at their own source node.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>m.RemoveAttribute("EventListName"),
                    (d,m)=>m.SetAttribute("EventListName",""),
                    (d,m)=>m.SetAttribute("EventListName","Reborn\u00E7Events"),
                    (d,m)=>m.AppendChild(d.CreateElement("Offset",SchemaSet.XmlNamespace))})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("LUAEventList",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornLUAEventListBad");module.SetAttribute("EventListName","RebornEvents");((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("LUAEventList source controls: PASS (missing, empty and non-ASCII EventListName and child elements rejected).");
            }
            if(caseFilter=="structure")
            {
                // Reborn: invalid geometry/contact enums, non-numeric radius/axis and a missing shape are refused.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>((XmlElement)m.FirstChild!).SetAttribute("Type","NOT_A_SHAPE"),
                    (d,m)=>((XmlElement)m.FirstChild!).SetAttribute("MajorRadius","abc"),
                    (d,m)=>{var o=d.CreateElement("Offset",SchemaSet.XmlNamespace);o.SetAttribute("x","abc");m.FirstChild!.AppendChild(o);},
                    (d,m)=>m.RemoveChild(m.FirstChild!)})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("StructurePlacementBehavior",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornStructureBad");module.AppendChild(bad.CreateElement("RestrictedAreaShape",SchemaSet.XmlNamespace));((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("Structure placement source controls: PASS (invalid shape type/radius/axis and missing shape rejected).");
            }
            if(caseFilter=="infiltrator")
            {
                // Reborn: invalid status, effect and time values are refused at their source attribute.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>m.SetAttribute("InfiltratedStatusFilter","NOT_A_STATUS"),
                    (d,m)=>m.SetAttribute("Effect","NOT_AN_EFFECT"),
                    (d,m)=>m.SetAttribute("Duration","abc")})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("InfiltratorContain",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornInfiltratorBad");((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("Infiltrator source controls: PASS (invalid status/effect/time rejected).");
            }
            if(caseFilter=="placement")
            {
                // Reborn: invalid hint/KindOf names, a non-finite distance and an authored filter id are refused.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>m.SetAttribute("PlacementHint","NOT_A_HINT"),
                    (d,m)=>m.SetAttribute("SnappingDistance","abc"),
                    (d,m)=>{var f=d.CreateElement("RequirementObjectFilter",SchemaSet.XmlNamespace);f.SetAttribute("Include","NOT_A_KINDOF");m.AppendChild(f);},
                    (d,m)=>{var f=d.CreateElement("RequirementObjectFilter",SchemaSet.XmlNamespace);f.SetAttribute("id","RebornFilter");m.AppendChild(f);}})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("AssistedPlacementBehavior",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornPlacementBad");((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("Placement source controls: PASS (invalid hint/KindOf/distance and authored filter id rejected).");
            }
            if(caseFilter=="engineer")
            {
                // Reborn: missing/incompatible references, invalid flags, authored filter id and empty filter names are refused.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>m.SetAttribute("ObjectStatusToSet","NOT_A_STATUS"),
                    (d,m)=>{var f=d.CreateElement("CanEnterFilter",SchemaSet.XmlNamespace);f.SetAttribute("Include","NOT_A_KINDOF");m.AppendChild(f);},
                    (d,m)=>{var f=d.CreateElement("CanEnterFilter",SchemaSet.XmlNamespace);f.SetAttribute("id","RebornFilter");m.AppendChild(f);}})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("EngineerContain",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornEngineerBad");((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("Engineer source controls: PASS (invalid status/KindOf and authored filter id rejected).");
            }
            if(caseFilter=="repair")
            {
                // Reborn: invalid percent, uint overflow and a missing required record are refused.
                foreach(var mutate in new Action<XmlDocument,XmlElement>[]{
                    (d,m)=>((XmlElement)m.FirstChild!).SetAttribute("Pristine","abc"),
                    (d,m)=>m.SetAttribute("HealAmountPerSecond","4294967296"),
                    (d,m)=>m.RemoveChild(m.LastChild!)})
                {
                    var bad=Copy(baseline);var module=bad.CreateElement("RepairUpdate",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornRepairBad");module.AppendChild(bad.CreateElement("PercentOfBuildCostToRebuildInfo",SchemaSet.XmlNamespace));module.AppendChild(bad.CreateElement("SelfAudio",SchemaSet.XmlNamespace));((XmlElement)bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).PrependChild(module);mutate(bad,module);
                    Refuse(()=>{ConscriptModuleScalarContract.Validate(bad);ConscriptVariableBehaviorSource.Admit(bad,baseline,official);});
                }
                Console.WriteLine("Repair source controls: PASS (invalid percent, uint overflow and missing SelfAudio rejected).");
            }
            foreach(string mode in caseFilter==null?modes:caseFilter is "combat" or "movement" or "resource" or "sell" or "destruction" or "dock" or "exit" or "spawn" or "unpack" or "production" or "repair" or "engineer" or "placement" or "infiltrator" or "structure" or "lua" or "fx" or "tracker" or "death" or "containers" or "power" or "draws" or "geometry" or "dependency" or "roots" or "skirmish" or "chooser" or "audio" or "crusher" or "windows"?modes.Where(m=>m.StartsWith(caseFilter+"-",StringComparison.Ordinal)):new[]{caseFilter})
            {
                var source=(XmlDocument)baseline.CloneNode(true);var origins=templates.Keys.ToDictionary(id=>id,id=>id,StringComparer.Ordinal);var behaviors=(XmlElement)source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!;var original=behaviors.ChildNodes.OfType<XmlElement>().ToArray();
                // Reborn: cover nondefault selectors, ordered references, and a second module with omitted optional fields.
                if (mode is "locomotor-ability" or "replace-self-upgrade")
                {
                    var fragment = source.CreateDocumentFragment();
                    fragment.InnerXml = mode == "locomotor-ability" ? """
                        <SwitchLocomotorsSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornSwitchAuthored"
                            UseUpgradedLocomotor="true" TriggersLanding="false" TriggersFlight="true"
                            BusyForDuration="2.5s" LandingRange="47.5" LocomotorToSet="FREEFALL"
                            SpecialPowerTemplate="SpecialPowerParticleCannon" HordeMembersSpecialPowerTemplate="SpecialPowerAlliedFreeTrade"/>
                        <SwitchLocomotorsSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornSwitchDefault" UseUpgradedLocomotor="false"/>
                        """ : """
                        <ReplaceSelfUpgrade xmlns="uri:ea.com:eala:asset" id="RebornReplaceAuthored" NewObjectUnpackTime="3.5s"
                            Options="COPY_TARGET_OBJECT COPY_UPGRADES CHECK_BUILD_ASSISTANT">
                            <TriggeredBy>Upgrade_AlliesFaction</TriggeredBy>
                            <ReplacementTemplate>SovietMiner</ReplacementTemplate>
                            <ReplacementTemplate>SovietAntiInfantryInfantry</ReplacementTemplate>
                        </ReplaceSelfUpgrade>
                        <ReplaceSelfUpgrade xmlns="uri:ea.com:eala:asset" id="RebornReplaceDefault"/>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: both weak pairs and both signed vectors must survive the named EP1 route in list order.
                if (mode == "substructure-build")
                {
                    var fragment = source.CreateDocumentFragment();
                    fragment.InnerXml = """
                        <SubStructureBuild xmlns="uri:ea.com:eala:asset" id="RebornSubStructure">
                            <AvailableSubStructure><UpgradeTrigger>Upgrade_AlliesFaction</UpgradeTrigger><ObjectTemplate>AlliedCrane</ObjectTemplate></AvailableSubStructure>
                            <AvailableSubStructure><UpgradeTrigger>Upgrade_JapanFaction</UpgradeTrigger><ObjectTemplate>SovietMiner</ObjectTemplate></AvailableSubStructure>
                            <StructureOffset ModelCondition="USER_6"><Offset x="4.56" y="-62.211" z="0"/></StructureOffset>
                            <StructureOffset ModelCondition="USER_7"><Offset x="30.095" y="-12" z="3"/></StructureOffset>
                        </SubStructureBuild>
                        <SubStructureBuild xmlns="uri:ea.com:eala:asset" id="RebornSubStructureDefault"/>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: exercise the real inherited weapon import and both explicit and default fire delays.
                if (mode == "delayed-weapon-fire")
                {
                    var fragment = source.CreateDocumentFragment();
                    fragment.InnerXml = """
                        <DelayedWeaponFireSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornDelayedWeapon"
                            SpecialWeapon="PlayerPowerIrradiateTargetWeapon" FireDelay="4.25s" WhichSpecialWeapon="2"/>
                        <DelayedWeaponFireSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornDefaultDelay"/>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: populate the three same-size base fields whose order differs from the legacy Attach model.
                if (mode == "leech-targeting")
                {
                    var fragment = source.CreateDocumentFragment();
                    fragment.InnerXml = """
                        <LeechTargetingAttachUpdate xmlns="uri:ea.com:eala:asset" id="RebornLeech" AttachBoneName="BONE_LEECH" Range="10">
                            <ObjectFilter Rule="ANY" Include="VEHICLE" Relationship="ENEMIES"/>
                            <ModifierToLeechFromParent>AttributeModifier_Garrisoned</ModifierToLeechFromParent>
                        </LeechTargetingAttachUpdate>
                        <LeechTargetingAttachUpdate xmlns="uri:ea.com:eala:asset" id="RebornLeechDefault"/>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: keep the original populated target chooser while switching its enclosing AI class.
                if (mode is "transport-ai" or "wander-ai")
                {
                    var previous = ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="AI").Element;
                    bool wander = mode == "wander-ai";
                    var derived = source.CreateElement(wander?"WanderAIUpdate":"TransportAIUpdate",SchemaSet.XmlNamespace);
                    foreach(XmlAttribute attribute in previous.Attributes)
                        derived.SetAttribute(attribute.Name,attribute.Value);
                    foreach(XmlNode child in previous.ChildNodes)
                        derived.AppendChild(child.CloneNode(true));
                    derived.SetAttribute("FadeOnPortals","true");
                    derived.SetAttribute("DefaultLocomotorSet",wander?"WANDER":"FREEFALL");
                    derived.SetAttribute("SpawnOffsetRadius","17.5");
                    if(wander)
                    {
                        derived.SetAttribute("EntryCondition","USER_6 USER_7");
                        derived.SetAttribute("WanderDistance","125");
                        derived.SetAttribute("AttackAll","true");
                        derived.SetAttribute("Selectable","false");
                    }
                    previous.ParentNode!.ReplaceChild(derived,previous);
                }
                // Reborn: cover independent RGB allocations, a missing start color and two missing colors.
                if (mode == "emp-update")
                {
                    var fragment = source.CreateDocumentFragment();
                    fragment.InnerXml = """
                        <EMPUpdate xmlns="uri:ea.com:eala:asset" id="RebornEmpBoth" Lifetime="2.5s" StartFadeDelay="1s" DisabledDuration="4s">
                            <StartColor r="0.1" g="0.2" b="0.3"/>
                            <EndColor r="0.4" g="0.5" b="0.6"/>
                        </EMPUpdate>
                        <EMPUpdate xmlns="uri:ea.com:eala:asset" id="RebornEmpEnd"><EndColor r="0.75" g="0.5" b="0.25"/></EMPUpdate>
                        <EMPUpdate xmlns="uri:ea.com:eala:asset" id="RebornEmpDefault"/>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: exercise an ordinary mixed hardpoint/contained set and the EA signed scatter boundaries.
                if(mode is "weapon-contained" or "weapon-scatter")
                {
                    var fragment=source.CreateDocumentFragment();
                    fragment.InnerXml=mode=="weapon-contained"?"""
                        <WeaponSlotContained xmlns="uri:ea.com:eala:asset" ID="31" AutoChooseWeapon="0">
                            <Weapon Ordering="SECONDARY_WEAPON" Template="SovietAntiInfantryInfantryAK47"/>
                            <Weapon Ordering="PRIMARY_WEAPON" Template="SovietAntiInfantryInfantryAK47"/>
                        </WeaponSlotContained>
                        <WeaponSlotContained xmlns="uri:ea.com:eala:asset" ID="33" AllowInterleavedFiring="true"/>
                        """:"""
                        <WeaponScatterEffectUpdate xmlns="uri:ea.com:eala:asset" id="RebornScatterNegative"><ScatterSettings ScatterAmount="-1"/><Filter/></WeaponScatterEffectUpdate>
                        <WeaponScatterEffectUpdate xmlns="uri:ea.com:eala:asset" id="RebornScatterMax"><ScatterSettings ScatterAmount="255"/><Filter/></WeaponScatterEffectUpdate>
                        <WeaponScatterEffectUpdate xmlns="uri:ea.com:eala:asset" id="RebornScatterOverflow"><ScatterSettings ScatterAmount="256"/><Filter/></WeaponScatterEffectUpdate>
                        """;
                    if(mode=="weapon-contained")original.Single(m=>m.LocalName=="WeaponSetUpdate").AppendChild(fragment);
                    else behaviors.AppendChild(fragment);
                }
                // Reborn: cover the EP1 command string and upgrade import independently of their omitted defaults.
                if(mode=="give-restore-upgrade")
                {
                    var fragment=source.CreateDocumentFragment();
                    fragment.InnerXml="""
                        <GiveOrRestoreUpgradeSpecialPower xmlns="uri:ea.com:eala:asset" id="RebornGiveUpgrade"
                            CommandButton="Command_RebornGrant" UpgradeToGive="Upgrade_AlliesFaction"/>
                        <GiveOrRestoreUpgradeSpecialPower xmlns="uri:ea.com:eala:asset" id="RebornGiveUpgradeDefault"/>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: use forward/backward local activation targets and an independently resolvable naval modifier.
                if(mode is "activation-ability" or "naval-modifier")
                {
                    var fragment=source.CreateDocumentFragment();
                    fragment.InnerXml=mode=="activation-ability"?"""
                        <GiveOrRestoreUpgradeSpecialPower xmlns="uri:ea.com:eala:asset" id="RebornFirstRecipient"/>
                        <ActivateModuleSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornActivation">
                            <SpecialPowerModules ModuleId="RebornFirstRecipient" TriggerType="SELF_POSITION" TriggerAtTargetPosition="false"/>
                            <SpecialPowerModules ModuleId="RebornSecondRecipient" TriggerType="TARGET_OBJECT"/>
                        </ActivateModuleSpecialAbilityUpdate>
                        <GiveOrRestoreUpgradeSpecialPower xmlns="uri:ea.com:eala:asset" id="RebornSecondRecipient"/>
                        <ActivateModuleSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornEmptyActivation"/>
                        """:"""
                        <AttributeModifierNaval xmlns="uri:ea.com:eala:asset" id="RebornNaval" AttributeModifier="AttributeModifier_Garrisoned"/>
                        <AttributeModifierNaval xmlns="uri:ea.com:eala:asset" id="RebornNavalDefault"/>
                        """;
                    behaviors.AppendChild(fragment);
                    if(mode=="activation-ability")
                    {
                        var missing=Copy(source);
                        ((XmlElement)missing.GetElementsByTagName("SpecialPowerModules",SchemaSet.XmlNamespace)[0]!).SetAttribute("ModuleId","MissingRebornRecipient");
                        Refuse(()=>ConscriptVariableBehaviorSource.Admit(missing,baseline,official));faults++;
                    }
                }
                // Reborn: exercise the named Body root with and without the shield's independently allocated armor import.
                if(mode is "inactive-body" or "shield-body" or "shield-body-default")
                {
                    var body=ConscriptVariableContainers.Find(source,"Body")!;
                    body.InnerXml=mode switch
                    {
                        "inactive-body" => """<InactiveBody xmlns="uri:ea.com:eala:asset" id="RebornInactive"/>""",
                        "shield-body" => """
                            <ShieldBody xmlns="uri:ea.com:eala:asset" id="RebornShield" MaxHealth="750" ShieldAmount="225.5"
                                ShieldRechargeIdleTime="2.75s" ShieldActiveModelCondition="USER_6" ShieldArmor="SovietAntiInfantryInfantryArmor"
                                ShieldEnabledFX="FX_SOV_LargeBearBloodImpact">
                                <DamageRangeModifier Min="10" Max="50" AttributeModifier="AttributeModifier_Garrisoned"/>
                            </ShieldBody>
                            """,
                        _ => """<ShieldBody xmlns="uri:ea.com:eala:asset" id="RebornShieldDefault" MaxHealth="750"/>"""
                    };
                }
                // Reborn: cover authored OCL references and FX position along with independent status masks.
                if(mode is "create-enter" or "status-ability")
                {
                    var fragment=source.CreateDocumentFragment();
                    fragment.InnerXml=mode=="create-enter"?"""
                        <CreateAndEnterObjectSpecialPower xmlns="uri:ea.com:eala:asset" id="RebornCreateEnter"
                            SpecialPowerTemplate="SpecialPowerChronoRiftTeleportLvl1" CreateLocation="CREATE_AT_OFFSET_FROM_TARGET"
                            CreateLocationOffset="17.5">
                            <UpgradeOCL OCL="OCL_Generic5SecondBeacon"/>
                            <FXOffset x="1.25" y="-2.5" z="3.75"/>
                        </CreateAndEnterObjectSpecialPower>
                        <CreateAndEnterObjectSpecialPower xmlns="uri:ea.com:eala:asset" id="RebornCreateEnterDefault"
                            SpecialPowerTemplate="SpecialPowerChronoRiftTeleportLvl1"/>
                        """:"""
                        <ObjectStatusSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornStatusAbility"
                            Status1="IMMOBILE" Status2="UNSELECTABLE"/>
                        <ObjectStatusSpecialAbilityUpdate xmlns="uri:ea.com:eala:asset" id="RebornStatusAbilityDefault"/>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: exercise both optional-record presence and original defaults in the normal GameObject source route.
                if(mode is "special-power-update" or "damage-field")
                {
                    var fragment=source.CreateDocumentFragment();
                    fragment.InnerXml=mode=="special-power-update"?"""
                        <SpecialPowerUpdate xmlns="uri:ea.com:eala:asset" id="RebornPowerDefault" SpecialPowerTemplate="SpecialPowerChronoRiftTeleportLvl1"/>
                        <SpecialPowerUpdate xmlns="uri:ea.com:eala:asset" id="RebornPowerEnabled" SpecialPowerTemplate="SpecialPowerChronoRiftTeleportLvl1"
                            DisabledTypesToIgnore="HELD PARALYZED" StartsPaused="true">
                            <InitiateSound Sound="SOV_Conscript_VoiceSelectMS" EvaEvent="BuildingRepaired"/>
                        </SpecialPowerUpdate>
                        """:"""
                        <DamageFieldUpdate xmlns="uri:ea.com:eala:asset" id="RebornDamageDefault"/>
                        <DamageFieldUpdate xmlns="uri:ea.com:eala:asset" id="RebornDamageField"
                            FireInterval="1.5s" AliveOnly="true" Radius="173" RequiredUpgrade="Upgrade_SovietFaction">
                            <FireWeaponNugget WeaponName="SovietHeavyAntiVehicleInfantryShortCircuit" FireDelay="2s" OneShot="true">
                                <Offset x="1.25" y="-2.5" z="3.75"/>
                            </FireWeaponNugget>
                            <ObjectStatusValidation ForbiddenStatus="UNSELECTABLE" RequiredStatus="IMMOBILE"/>
                            <ObjectFilter Rule="ANY" Include="INFANTRY" Exclude="STRUCTURE"/>
                        </DamageFieldUpdate>
                        """;
                    behaviors.AppendChild(fragment);
                }
                // Reborn: keep a real inherited chooser, locomotor selector and import on both authored AI classes.
                if(mode is "horde-ai" or "supply-truck-ai")
                {
                    var previous=ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="AI").Element;
                    var ai=source.CreateElement(mode=="horde-ai"?"HordeAIUpdate":"SupplyTruckAIUpdate",SchemaSet.XmlNamespace);
                    foreach(XmlAttribute attr in previous.Attributes)ai.SetAttribute(attr.Name,attr.Value);
                    foreach(XmlNode child in previous.ChildNodes)ai.AppendChild(child.CloneNode(true));
                    ai.SetAttribute("FadeOnPortals","true");
                    ai.SetAttribute("StateMachine","TransportHelicopterAIStateMachine");
                    ai.SetAttribute("DefaultLocomotorSet","FREEFALL");
                    if(mode=="supply-truck-ai")
                    {
                        ai.SetAttribute("MaxBoxes","17");
                        ai.SetAttribute("SupplyCenterActionDelay","1.25s");
                        ai.SetAttribute("SupplyWarehouseActionDelay","2.5s");
                        ai.SetAttribute("SupplyWarehouseScanDistance","39");
                        ai.SetAttribute("HarvestActivationRange","420");
                        ai.SetAttribute("HarvestPreparationTime","3.75s");
                        ai.SetAttribute("HarvestActionTime","4.5s");
                        ai.SetAttribute("HarvestTrees","true");
                        ai.SetAttribute("HarvestTiberium","false");
                        ai.SetAttribute("HarvestsFromRearEnd","true");
                        ai.SetAttribute("CanPathThroughUnitsWhileDocking","false");
                    }
                    previous.ParentNode!.ReplaceChild(ai,previous);
                }
                // Reborn: exercise all four named shapes with populated fields that distinguish the EP1 ABI.
                if(mode is "bezier-spline" or "sphere-update" or "steal-unit" or "booby-trap")
                {
                    var fragment=source.CreateDocumentFragment();
                    fragment.InnerXml=mode switch
                    {
                        "bezier-spline" => """
                            <ProjectileUpdate xmlns="uri:ea.com:eala:asset" id="RebornBezier" StateMachine="BezierSplineProjectile" Flags="TRACK_TARGET">
                                <Movement><BezierSpline Velocity="22.5" Range="137.5" Gravity="2.25" FlightPathAdjustMaxDistancePerFrame="3.75">
                                    <Path>ProjectilePath_Foo</Path><Path>ProjectilePath_Foo</Path>
                                </BezierSpline></Movement>
                            </ProjectileUpdate>
                            <ProjectileUpdate xmlns="uri:ea.com:eala:asset" id="RebornBezierDefault" StateMachine="BezierSplineProjectile">
                                <Movement><BezierSpline Velocity="1" Range="2"><Path>ProjectilePath_Foo</Path></BezierSpline></Movement>
                            </ProjectileUpdate>
                            """,
                        "sphere-update" => """
                            <SphereModuleUpdate xmlns="uri:ea.com:eala:asset" id="RebornSphere" RadiusMax="54" RadiusMin="12" ScanFrequency="0.75s"
                                Duration="8.25s" SphereBoneName="Sphere_Anchor" SphereSizeMultiplier="1.5" InitiallyActive="true" DrawDebugCircle="false">
                                <PositionOffset x="1.25" y="-2.5" z="3.75"/>
                                <ObjectFilter Rule="ANY" Include="INFANTRY"/>
                                <IgnoreInsideToInsideCheck Rule="ANY" Include="VEHICLE"/>
                            </SphereModuleUpdate>
                            """,
                        "steal-unit" => """
                            <StealUnitUpdate xmlns="uri:ea.com:eala:asset" id="RebornSteal" AttachBoneName="EP1_BONE" Range="14">
                                <ObjectFilter Rule="ANY" Include="VEHICLE"/>
                                <ModifierToLeechFromParent>AttributeModifier_Garrisoned</ModifierToLeechFromParent>
                            </StealUnitUpdate>
                            """,
                        _ => """
                            <BoobyTrapUpdate xmlns="uri:ea.com:eala:asset" id="RebornTrap" AttachBoneName="EP1_BONE" Range="14"
                                FireWeaponConditionsOnParent="IMMOBILE" PreventEnteringSetStatusOnParent="UNSELECTABLE"
                                SpecialWeapon="SovietHeavyAntiVehicleInfantryShortCircuit" AllowTriggerOnAllies="true">
                                <ObjectFilter Rule="ANY" Include="VEHICLE"/>
                                <ModifierToLeechFromParent>AttributeModifier_Garrisoned</ModifierToLeechFromParent>
                                <PreventEnteringFilter Rule="ANY" Include="INFANTRY"/>
                            </BoobyTrapUpdate>
                            """
                    };
                    behaviors.AppendChild(fragment);
                }
                // Reborn: authored XML omits TypeId and sets exactly one of the retail byte fields.
                if (mode is "squish-vehicles" or "squish-allies")
                {
                    var squish = original.Single(module => module.LocalName == "SquishCollide");
                    squish.SetAttribute("CrushAnimTime", "2.5s");
                    squish.SetAttribute("UseVehicleStyleCrush", mode == "squish-vehicles" ? "true" : "false");
                    squish.SetAttribute("AllowAllyCrush", mode == "squish-allies" ? "true" : "false");
                }
                // Reborn: use independently authored rows with nondefault and default vehicle settings.
                if (mode == "destructible-environment")
                {
                    for (int variant = 0; variant < 2; variant++)
                    {
                        var environment = source.CreateElement("DestructibleEnvironmentUpdate", SchemaSet.XmlNamespace);
                        environment.SetAttribute("id", "RebornEnvironment" + variant);
                        if (variant == 0)
                        {
                            environment.SetAttribute("Enabled", "true");
                            environment.SetAttribute("Size", "HEAVY");
                        }
                        environment.InnerXml = "<LeftOffset xmlns='uri:ea.com:eala:asset' x='1' y='2' z='3'/><RightOffset xmlns='uri:ea.com:eala:asset' x='4' y='5' z='6'/>";
                        behaviors.AppendChild(environment);
                    }
                }
                // Reborn: preserve absent/empty/root-list states while exposing real sound imports and optional projected/shadow fields.
                // Reborn: BuildingBehavior values are optional subobject strings; no global asset with the name LIGHT_01 is generated or required.
                if(mode.StartsWith("windows-",StringComparison.Ordinal)){
                    if(mode=="windows-stock")foreach(var module in GameObjectExtendedRootContract.Original("Soviet/Structures/SovietPowerPlant.xml","BuildingBehavior")){var added=(XmlElement)source.ImportNode(module,true);behaviors.PrependChild(added);origins.Add(added.GetAttribute("id"),"RebornBuildingTemplate");}
                    else for(int i=0;i<(mode=="windows-edited"?2:1);i++){var module=source.CreateElement("BuildingBehavior",SchemaSet.XmlNamespace);string id="RebornWindows"+i;module.SetAttribute("id",id);if(mode=="windows-edited"){module.SetAttribute("NightWindowObject","Reborn_LIGHT_"+i);if(i==0)module.SetAttribute("FireWindowObject","");module.SetAttribute("GlowWindowObject","GLOW_"+i);module.SetAttribute("NightFireWindowObject","NIGHT_FIRE_"+i);}behaviors.PrependChild(module);origins.Add(id,"RebornBuildingTemplate");}
                }
                // Reborn: the pipeline-only id may change without creating a runtime hash field; signed level bounds and all scalar/import values remain source-bound.
                if(mode.StartsWith("crusher-",StringComparison.Ordinal)){
                    var previous=GameObjectExtendedRootContract.Owners(source,"CrusherInfo").Single();if(mode=="crusher-omitted")previous.ParentNode!.RemoveChild(previous);
                    else{var crusher=mode=="crusher-building"?(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("BaseObjects/BaseSovietStructure.xml","CrusherInfo").Single(),true):source.CreateElement("CrusherInfo",SchemaSet.XmlNamespace);previous.ParentNode!.ReplaceChild(crusher,previous);
                        if(mode=="crusher-edited")foreach(var edit in new[]{("id","RebornCrusherPipelineId"),("CrusherLevel","-128"),("CrushableLevel","127"),("CrushKnockback","-12"),("CrushZFactor","1.75"),("MinCrushVelocityPercent","25%"),("CrushDecelerationPercent","50%"),("CrushWeapon","SovietAntiInfantryInfantryAK47"),("DefaultCrushKillDelay","1250ms"),("CrushEqualLevelProps","true"),("UseCrushAttack","false"),("CrushOnlyWhileCharging","true"),("CrushAllies","true"),("CrushAircraftWhileStationary","true"),("UseDirectionCheck","false"),("ShouldSquishOnCollide","false"),("DecelerateForAllCollides","true"),("CannotCrushTarget","true")})crusher.SetAttribute(edit.Item1,edit.Item2);
                    }
                }
                // Reborn: root audio fixtures exercise real inherited capture EVA entries, changed voice cardinality and a separately source-sized root EVA list.
                if(mode.StartsWith("audio-",StringComparison.Ordinal)){
                    // Reborn: retain real sibling anchors; the source may omit optional sound/shadow/vision owners entirely.
                    XmlNode? voiceAnchor=GameObjectExtendedRootContract.Owners(source,"AudioArrayVoice").Single().NextSibling,evaAnchor=GameObjectExtendedRootContract.Owners(source,"EvaEventArray").Single().NextSibling;while(voiceAnchor!=null&&(voiceAnchor is not XmlElement||voiceAnchor.LocalName=="EvaEventArray"))voiceAnchor=voiceAnchor.NextSibling;while(evaAnchor!=null&&evaAnchor is not XmlElement)evaAnchor=evaAnchor.NextSibling;
                    foreach(string name in new[]{"AudioArrayVoice","EvaEventArray"})foreach(var previous in GameObjectExtendedRootContract.Owners(source,name))source.DocumentElement!.RemoveChild(previous);
                    if(mode!="audio-omitted"){
                        var voice=source.CreateElement("AudioArrayVoice",SchemaSet.XmlNamespace);source.DocumentElement!.InsertBefore(voice,voiceAnchor);var eva=source.CreateElement("EvaEventArray",SchemaSet.XmlNamespace);source.DocumentElement.InsertBefore(eva,evaAnchor);
                        if(mode=="audio-refinery"){
                            foreach(var row in GameObjectExtendedRootContract.Original("Soviet/Structures/SovietRefinery.xml","AudioArrayVoice").Single().ChildNodes.OfType<XmlElement>())voice.AppendChild(source.ImportNode(row,true));
                            foreach(var owner in GameObjectExtendedRootContract.Original("BaseObjects/BaseStructure.xml","AudioArrayVoice").Where(e=>e.ParentNode is XmlElement parent&&parent.LocalName=="GameObject"))foreach(var row in owner.ChildNodes.OfType<XmlElement>())voice.AppendChild(source.ImportNode(row,true));
                            foreach(var row in GameObjectExtendedRootContract.Owners(baseline,"EvaEventArray").Single().ChildNodes.OfType<XmlElement>())eva.AppendChild(source.ImportNode(row,true));
                        }
                        if(mode=="audio-expanded"){
                            for(int i=0;i<18;i++){var row=source.CreateElement("AudioEntry",SchemaSet.XmlNamespace);row.SetAttribute("AudioType",i%3==0?"voiceSelect":i%3==1?"voiceDefectingAway":"voiceDefectingTowards");if(i%3==0)row.SetAttribute("Sound","SOV_Refinery_Select");else row.SetAttribute("EvaEvent",i%3==1?"BuildingStolen":"EnemyBuildingCaptured");voice.AppendChild(row);}
                            var model=GameObjectExtendedRootContract.Owners(baseline,"EvaEventArray").Single().ChildNodes.OfType<XmlElement>().First();for(int i=0;i<20;i++){var row=(XmlElement)source.ImportNode(model,true);row.SetAttribute("EvaEvent",i%2==0?"BuildingStolen":"EnemyBuildingCaptured");eva.AppendChild(row);}
                            // Reborn: the same typed EVA layout must work inside existing client selectors, not only through the newly opened root path.
                            var selector=source.CreateElement("ModelConditionSoundSelectorClientBehavior",SchemaSet.XmlNamespace);selector.SetAttribute("id","RebornCaptureVoices");selector.InnerXml="<Override xmlns='uri:ea.com:eala:asset' RequiredFlags='FROZEN'><AudioArrayVoice><AudioEntry EvaEvent='BuildingStolen' AudioType='voiceDefectingAway'/></AudioArrayVoice></Override>";ConscriptVariableContainers.Find(source,"ClientBehaviors")!.AppendChild(selector);
                        }
                    }
                }
                // Reborn: keep one source AI module while authoring absent/default/edited chooser records on the normal production allocation path.
                if(mode.StartsWith("chooser-",StringComparison.Ordinal)){
                    var update=ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="AI").Element;
                    if(mode=="chooser-refinery"){var ai=GameObjectExtendedRootContract.Original("Soviet/Structures/SovietRefinery.xml","AI").Single();update.ParentNode!.ReplaceChild(source.ImportNode(ai.ChildNodes.OfType<XmlElement>().Single(),true),update);}
                    else{
                        string id=update.GetAttribute("id");update.RemoveAll();update.SetAttribute("id",id);var chooser=source.CreateElement("UnitAITargetChooserData",SchemaSet.XmlNamespace);update.AppendChild(chooser);
                        if(mode=="chooser-expanded"){
                            foreach(var edit in new[]{("AttackPriority","RebornAITest"),("AutoAcquireEnemiesWhenIdle","NO ATTACK_BUILDINGS"),("StateMachine","StructureAIStateMachine"),("DefaultLocomotorSet","NORMAL"),("StopChaseDistance","23.5"),("BurningDeathTime","2s"),("MinCowerTime","3s"),("MaxCowerTime","7s"),("StandGround","true"),("FadeOnPortals","true"),("MaxCombineChildren","8")})update.SetAttribute(edit.Item1,edit.Item2);
                            foreach(var edit in new[]{("IdleScanDelay","250ms"),("ReacquireDelay","3s"),("StartupDelay","2s"),("SympathyRange","51.25"),("CanPickDynamicTargetType","ATTACK_MOVE AGGRESSIVE_STANCE"),("CheckVisionRange","true"),("CanPickDynamicTargets","false"),("RotateToTargetWhenAiming","true"),("CanAutoAcquireNonAutoAcquirable","true"),("CanChaseAttack","false"),("CanOnlyAttackTargetsInFront","true"),("TargetingCompareList","DefaultTargetingCompareList")})chooser.SetAttribute(edit.Item1,edit.Item2);
                        }
                    }
                }
                // Reborn: author AI roots in source schema order without changing user Reborn files.
                if(mode.StartsWith("skirmish-",StringComparison.Ordinal)){
                    // Reborn: already implemented AIUpdate mask/state encodings must compose with source-owned building AI roots.
                    if(mode is "skirmish-refinery" or "skirmish-expanded"){var update=ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="AI").Element;update.SetAttribute("AutoAcquireEnemiesWhenIdle",mode=="skirmish-refinery"?"NO":"YES ATTACK_BUILDINGS");update.SetAttribute("StateMachine","StructureAIStateMachine");}
                    foreach(var previous in GameObjectExtendedRootContract.Owners(source,"SkirmishAIInformation"))source.DocumentElement!.RemoveChild(previous);
                    if(mode!="skirmish-omitted"){
                        var ai=mode=="skirmish-refinery"?(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietRefinery.xml","SkirmishAIInformation").Single(),true):source.CreateElement("SkirmishAIInformation",SchemaSet.XmlNamespace);
                        source.DocumentElement!.InsertBefore(ai,source.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="Draws"));
                        if(mode=="skirmish-expanded"){
                            foreach(var edit in new[]{("BaseBuildingLocation","NEAR_RESOURCE_NODE"),("AdditionalRotation","90d"),("BaseBuildingMaxNudges","7"),("ConquerMetricsOverrideDPS","99.5"),("ConquerMetricsOverrideDefendingAntiMask","ANTI_GROUND ANTI_WATER"),("NearResourceNodeType","ENERGY"),("OverrideInnerDistanceToTarget","12.5"),("OverrideOuterDistanceToTarget","120"),("DodgingRequiredObjectStatus","CAN_ATTACK"),("CanFireUpCliffs","true")})ai.SetAttribute(edit.Item1,edit.Item2);
                            foreach(string child in new[]{"ConstructionBaseSelectionCriteria","ResourceNodeBaseClosenessTestSelectionCriteria"}){var row=source.CreateElement(child,SchemaSet.XmlNamespace);row.SetAttribute("RequiredBaseTypes","MAIN_BASE LAND_CENTERED");row.SetAttribute("ExcludedBaseTypes","NO_BUILD_RADIUS");row.SetAttribute("PreferredBaseTypes","EXPANSION OLDEST_OF_TYPE");row.SetAttribute("PreferredNotBaseTypes","PRIMARY_TYPE_MAX UNDER_ATTACK");row.SetAttribute("SortOrder","PREFER_CLOSEST");ai.AppendChild(row);}
                            var filter=source.CreateElement("DodgeWithUnits",SchemaSet.XmlNamespace);filter.SetAttribute("Rule","ANY");filter.SetAttribute("Include","INFANTRY");ai.AppendChild(filter);
                        }
                    }
                }
                if(mode.StartsWith("roots-",StringComparison.Ordinal)){
                    foreach(string name in GameObjectExtendedRootContract.Names)foreach(var owner in GameObjectExtendedRootContract.Owners(source,name))source.DocumentElement!.RemoveChild(owner);
                    var sound=source.CreateElement("AudioArraySound",SchemaSet.XmlNamespace);source.DocumentElement!.InsertBefore(sound,source.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="EvaEventArray"));
                    if(mode=="roots-expanded")foreach(var tuple in new[]{("SOV_Reactor_AmbientLoop","soundAmbient"),("BuildingUnderConstructionLoop","soundUnderConstruction")}){var entry=source.CreateElement("AudioEntry",SchemaSet.XmlNamespace);entry.SetAttribute("Sound",tuple.Item1);entry.SetAttribute("AudioType",tuple.Item2);sound.AppendChild(entry);}
                    if(mode!="roots-refinery"){var shadow=source.CreateElement("ShadowInfo",SchemaSet.XmlNamespace);source.DocumentElement.InsertBefore(shadow,source.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="VisionInfo"));if(mode=="roots-expanded"){shadow.SetAttribute("Type","DECAL");shadow.SetAttribute("Texture","ShadowI");shadow.SetAttribute("AdditionalTexture","ShadowI");shadow.SetAttribute("SizeX","99.25");shadow.SetAttribute("OpacityFadeInTime","2s");shadow.SetAttribute("IsRotatingWithObject","false");}}
                    if(mode=="roots-refinery")foreach(var row in GameObjectExtendedRootContract.Original("Soviet/Structures/SovietRefinery.xml","ProjectedBuildabilityInfo"))source.DocumentElement.AppendChild(source.ImportNode(row,true));
                    else for(int i=0;i<(mode=="roots-expanded"?3:1);i++){var row=source.CreateElement("ProjectedBuildabilityInfo",SchemaSet.XmlNamespace);if(mode=="roots-expanded"){row.SetAttribute("Radius",(200+i).ToString(System.Globalization.CultureInfo.InvariantCulture));if(i!=1)row.SetAttribute("RadiusY","75");row.SetAttribute("BuildPlacementTypes","MAIN_STRUCTURE OTHER_STRUCTURE");row.SetAttribute("StatusToReject","UNDER_CONSTRUCTION");row.SetAttribute("ModelConditionsToReject","UNPACKING");row.SetAttribute("AllowedBuildabilityHeightVariation","17.5");row.SetAttribute("PrimaryBuidability","false");if(i==2){var filter=source.CreateElement("AllowedObjectFilter",SchemaSet.XmlNamespace);filter.SetAttribute("Rule","ANY");filter.SetAttribute("Include","STRUCTURE");row.AppendChild(filter);}}source.DocumentElement.AppendChild(row);}
                }
                // Reborn: preserve optional dependency presence independently of source-sized weak lists and optional masks/filter pointers.
                if(mode.StartsWith("dependency-",StringComparison.Ordinal)){
                    var dependency=source.CreateElement("GameDependency",SchemaSet.XmlNamespace);source.DocumentElement!.InsertBefore(dependency,source.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="ObjectResourceInfo"));
                    if(mode!="dependency-empty")foreach(string name in mode=="dependency-expanded"?new[]{"SovietPowerPlant","SovietRefinery","SovietMiner"}:new[]{"SovietPowerPlant"}){var row=source.CreateElement("RequiredObject",SchemaSet.XmlNamespace);row.InnerText=name;dependency.AppendChild(row);}
                    if(mode=="dependency-expanded"){
                        dependency.SetAttribute("RequiredModelConditionsAny","STRUCTURE_UNPACKING UNPACKING");dependency.SetAttribute("ForbiddenModelConditions","DAMAGED");dependency.SetAttribute("RequiredObjectStatusAny","UNDER_CONSTRUCTION");
                        foreach(string name in new[]{"ForbiddenUpgrade","NeededUpgrade"})foreach(string id in new[]{"Upgrade_AlliesFaction","Upgrade_JapanFaction"}){var row=source.CreateElement(name,SchemaSet.XmlNamespace);row.InnerText=id;dependency.AppendChild(row);}
                    }
                    if(mode=="dependency-filter"){var filter=source.CreateElement("ObjectFilter",SchemaSet.XmlNamespace);filter.SetAttribute("Rule","NONE");filter.SetAttribute("Include","INFANTRY");foreach(string name in new[]{"IncludeThing","ExcludeThing"}){var row=source.CreateElement(name,SchemaSet.XmlNamespace);row.InnerText=name=="IncludeThing"?"SovietEngineer":"AlliedEngineer";filter.AppendChild(row);}dependency.AppendChild(filter);}
                }
                // Reborn: Geometry is authored independently of the infantry's one-cylinder shape and its empty placement/contact records.
                if(mode.StartsWith("geometry-",StringComparison.Ordinal)){
                    var geometry=GameObjectGeometryContract.Owner(source)!;geometry.RemoveAll();
                    if(mode=="geometry-refinery")foreach(var child in SdkLeaves(fixtureRoot,"Soviet/Structures/SovietRefinery.xml","Geometry"))geometry.AppendChild(source.ImportNode(child,true));
                    else{
                        if(mode=="geometry-expanded"){
                            var rotation=source.CreateElement("RotationAnchorOffset",SchemaSet.XmlNamespace);rotation.SetAttribute("x","17.5");rotation.SetAttribute("y","-3");geometry.AppendChild(rotation);
                            var placement=source.CreateElement("PlacementShape",SchemaSet.XmlNamespace);placement.SetAttribute("Type","BOX");placement.SetAttribute("Name","RebornPlacement");placement.SetAttribute("Other","RebornOther");placement.SetAttribute("FrontAngle","90d");geometry.AppendChild(placement);
                        }
                        for(int i=0;i<(mode=="geometry-expanded"?33:1);i++){
                            var shape=source.CreateElement("Shape",SchemaSet.XmlNamespace);if(mode=="geometry-expanded"){shape.SetAttribute("Type","RING");shape.SetAttribute("Name","RebornShape"+i);shape.SetAttribute("MajorRadius",(i+2).ToString(System.Globalization.CultureInfo.InvariantCulture));shape.SetAttribute("Active","false");var offset=source.CreateElement("Offset",SchemaSet.XmlNamespace);offset.SetAttribute("x","-17.5");offset.SetAttribute("y","20");offset.SetAttribute("z","3");shape.AppendChild(offset);}geometry.AppendChild(shape);
                        }
                        if(mode=="geometry-expanded")for(int i=0;i<129;i++){var contact=source.CreateElement("ContactPoint",SchemaSet.XmlNamespace);contact.SetAttribute("Label","RebornContact"+i);var pos=source.CreateElement("Pos",SchemaSet.XmlNamespace);pos.SetAttribute("x","1");pos.SetAttribute("y","2");pos.SetAttribute("z","3");contact.AppendChild(pos);geometry.AppendChild(contact);}
                    }
                    // Reborn: independently prove the original refinery geometry against its real stock bytes before using it in a different object.
                    if(mode=="geometry-refinery"){var refinery=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var resolvedGeometry=GameObjectGeometryContract.Owner(SchemaNativeContract.Resolve(source,schemas,catalog))!;SchemaNativeContract.VerifyStock(resolvedGeometry,typeof(Ep1Geometry),0,refinery.Bytes,(int)Word(refinery.Bytes,refinery.Projection.NativeRoot+500),refinery.Asset.References.ToArray());}
                }
                // Reborn: each source list retains complete reviewed leaves; count/order changes never select a stock module by new ordinal.
                if(mode.StartsWith("containers-",StringComparison.Ordinal))foreach(string name in ConscriptVariableContainers.Names){
                    var container=ConscriptVariableContainers.Find(source,name)!;var children=container.ChildNodes.OfType<XmlElement>().ToArray();
                    if(mode=="containers-omitted"){container.ParentNode!.RemoveChild(container);continue;}
                    foreach(var child in children)container.RemoveChild(child);
                    if(mode=="containers-empty")continue;
                    foreach(var child in children.Reverse())container.AppendChild(child);
                    if(mode=="containers-expanded")for(int i=0;i<2;i++){var added=(XmlElement)children[0].CloneNode(true);added.SetAttribute("id","Reborn"+name+i);container.AppendChild(added);}
                    // Reborn: 129 complete update leaves cross both historical 64-tokenizer and 128-address quotas without weakening allocation checks.
                    if(mode=="containers-large"&&name=="ClientUpdates")for(int i=0;i<128;i++){var added=(XmlElement)children[0].CloneNode(true);added.SetAttribute("id","RebornManyUpdates"+i);container.AppendChild(added);}
                }
                // Reborn: both real refinery OCL variants and destroy use new local identities, with expanded optional/list data in the same graph.
                if(mode.StartsWith("death-",StringComparison.Ordinal))for(int i=0;i<3;i++){
                    bool creating=i<2;var module=source.CreateElement(creating?"CreateObjectDie":"DestroyDie",SchemaSet.XmlNamespace);string id="RebornDeath"+i;module.SetAttribute("id",id);var die=source.CreateElement("DieMuxData",SchemaSet.XmlNamespace);module.AppendChild(die);
                    if(mode!="death-defaults"){
                        die.SetAttribute("DeathTypes","ALL");die.SetAttribute("DeathTypesForbidden","SUICIDED");if(i==1)die.SetAttribute("RequiredStatus","OVER_WATER");if(creating)module.SetAttribute("CreationList",i==0?"SBRefinery_Die_OCL":"ObjectCreationList:SBRefineryPlatform_Die_OCL");
                        if(mode=="death-expanded"){
                            die.SetAttribute("VeterancyLevels","REGULAR VETERAN HEROIC");die.SetAttribute("ExemptStatus","UNDER_CONSTRUCTION");die.SetAttribute("DamageAmountRequired","17.5");die.SetAttribute("MinKillerAngle","2d");die.SetAttribute("MaxKillerAngle","1r");
                            if(creating){foreach(string upgrade in new[]{"Upgrade_AlliesFaction","Upgrade_JapanFaction","Upgrade_Veterancy_HEROIC"}){var row=source.CreateElement("UpgradeRequired",SchemaSet.XmlNamespace);row.InnerText=upgrade;module.AppendChild(row);}var forbidden=source.CreateElement("UpgradeForbidden",SchemaSet.XmlNamespace);forbidden.InnerText="UpgradeTemplate:Upgrade_AlliesFaction";module.AppendChild(forbidden);}
                        }
                    }
                    behaviors.PrependChild(module);origins.Add(id,creating?"ModuleTag_CreateFrozenObjectDie":"ModuleTag_FrozenDie");
                }
                // Reborn: replace the old tracker with actual building/default/expanded authoring rather than retaining a fixed reference count.
                if(mode.StartsWith("tracker-",StringComparison.Ordinal)){
                    var old=behaviors.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="SecondaryDamageTracker");string origin=old.GetAttribute("id");behaviors.RemoveChild(old);origins.Remove(origin);
                    for(int i=0;i<(mode=="tracker-multiple"?2:1);i++){
                        var module=(XmlElement)old.CloneNode(true);string id="RebornTracker"+i;module.SetAttribute("id",id);
                        if(mode=="tracker-building"){foreach(var item in module.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="EffectInfo"))item.RemoveAttribute("Options");var status=source.CreateElement("ObjectStatusValidation",SchemaSet.XmlNamespace);status.SetAttribute("ForbiddenStatus","UNDER_IRON_CURTAIN");module.AppendChild(status);}
                        else{
                            foreach(var child in module.ChildNodes.OfType<XmlElement>().ToArray())module.RemoveChild(child);
                            foreach(string name in new[]{"UpdateDuration","DissipateAmount","MaxDamageClampScaler"})module.RemoveAttribute(name);
                            if(mode!="tracker-empty"){
                                module.SetAttribute("UpdateDuration","1250ms");module.SetAttribute("DissipateAmount","17");module.SetAttribute("MaxDamageClampScaler","175%");
                                if(i==0){var color=source.CreateElement("Color",SchemaSet.XmlNamespace);color.SetAttribute("R","0.25");color.SetAttribute("G","2.5");color.SetAttribute("B","7.25");module.AppendChild(color);}
                                for(int row=0;row<(i==0?9:1);row++){var item=source.CreateElement("EffectInfo",SchemaSet.XmlNamespace);item.SetAttribute("Fraction",(row*0.125f).ToString(System.Globalization.CultureInfo.InvariantCulture));if(row%3!=0)item.SetAttribute("AttributeModifier",row%2==0?"AttributeModifer_CryoBeam_02":"AttributeModifier:AttributeModifer_CryoBeam_04");item.SetAttribute("Options",row%2==0?"DIE_ON_ENTER CRASH_IF_AIRBORNE":"");item.SetAttribute("OnEnterDisable","1");item.SetAttribute("OnEnterCancelSpecialAbilities","0");item.SetAttribute("SetUpgradedArmor","true");module.AppendChild(item);}
                                var status=source.CreateElement("ObjectStatusValidation",SchemaSet.XmlNamespace);status.SetAttribute("ForbiddenStatus","UNDER_IRON_CURTAIN");status.SetAttribute("RequiredStatus","CAN_ATTACK");module.AppendChild(status);
                            }
                        }
                        behaviors.PrependChild(module);origins.Add(id,origin);
                    }
                }
                // Reborn: new IDs, original repair-power defaults, compatible retargets and source-sized inherited lists are authorable.
                if(mode.StartsWith("assign-",StringComparison.Ordinal))for(int i=0;i<(mode=="assign-multiple"?2:1);i++){
                    var module=source.CreateElement("AssignSlavesTargetObjectSpecialPower",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornAssignSlaves"+i);module.SetAttribute("SpecialPowerTemplate",mode=="assign-defaults"?"SpecialPower_TargetedRepairVehicle":"SpecialPower_MolotovCocktail");
                    if(mode!="assign-defaults"){module.SetAttribute("UpdateModuleStartsAttack","true");module.SetAttribute("StartsPaused","true");module.SetAttribute("AttributeModifierRange","37.5");module.SetAttribute("GiveLevels","3");module.SetAttribute("SetModelConditionTime","2s");module.SetAttribute("WeatherDuration","1200");module.SetAttribute("RequirementsFilterMPSkirmish","RebornFirst RebornSecond");module.SetAttribute("AffectAllies","false");module.SetAttribute("AvailableAtStart","false");}
                    behaviors.AppendChild(module);origins.Add(module.GetAttribute("id"),SchemaNativeContract.Origin);
                }
                // Reborn: add the six real refinery draws (new local ids), or an edited Conscript draw with a new nested animation state.
                if(mode.StartsWith("draws-",StringComparison.Ordinal)){
                    var draws=ConscriptVariableContainers.Find(source,"Draws")!;
                    if(mode=="draws-clients"){var clients=ConscriptVariableContainers.Find(source,"ClientBehaviors")!;foreach(var client in SdkLeaves(fixtureRoot,"BaseObjects/BaseStructure.xml","ClientBehaviors").Concat(SdkLeaves(fixtureRoot,"Soviet/Structures/SovietRefinery.xml","ClientBehaviors"))){var added=(XmlElement)source.ImportNode(client,true);added.SetAttribute("id","Reborn_"+added.GetAttribute("id"));clients.AppendChild(added);}}
                    else if(mode=="draws-refinery")foreach(var draw in RefineryDraws(fixtureRoot)){var added=(XmlElement)source.ImportNode(draw,true);added.SetAttribute("id","Reborn_"+added.GetAttribute("id"));draws.AppendChild(added);}
                    else{
                        var edited=(XmlElement)draws.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="ScriptedModelDraw").CloneNode(true);edited.SetAttribute("id","RebornEditedDraw");edited.SetAttribute("OkToChangeModelColor","true");edited.SetAttribute("ExtraPublicBone","RebornBoneA RebornBoneB");
                        // Reborn: exercise source-owned optional materials, all four constant kinds, vector cardinality, custom names, ordering and schema defaults.
                        if(mode.StartsWith("draws-material",StringComparison.Ordinal)){
                            var modelState=edited.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="ModelConditionState");
                            var material=source.CreateElement("Material",SchemaSet.XmlNamespace);material.SetAttribute("ShaderName","ObjectsFrozen.fx");
                            if(mode!="draws-material-defaults")material.SetAttribute("TechniqueName","Default");
                            if(mode=="draws-material")material.SetAttribute("TechniqueIndex","7");
                            if(mode!="draws-material-defaults"){
                                var constants=source.CreateElement("Constants",SchemaSet.XmlNamespace);material.AppendChild(constants);
                                if(mode=="draws-material")foreach(var entry in new[]{("Bool","RebornEnabled",new[]{"true"}),("Int","RebornMode",new[]{"-7"}),("Float","RebornVector",new[]{"-0.5","2","3.25","4"}),("Texture","RebornDiffuse",new[]{"SBBarracks"}),("Float","RebornScalar",new[]{"0.125"})}){
                                    var constant=source.CreateElement(entry.Item1,SchemaSet.XmlNamespace);constant.SetAttribute("Name",entry.Item2);constants.AppendChild(constant);
                                    foreach(string text in entry.Item3){var value=source.CreateElement("Value",SchemaSet.XmlNamespace);value.InnerText=text;constant.AppendChild(value);}
                                }
                            }
                            modelState.InsertBefore(material,modelState.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName!="Model"));
                        }
                        var state=source.CreateElement("AnimationState",SchemaSet.XmlNamespace);state.SetAttribute("ParseCondStateType","PARSE_NORMAL");state.SetAttribute("ConditionsYes","SOLD");state.SetAttribute("StateName","RebornState");state.SetAttribute("Flags","START_FRAME_LAST");
                        var animation=source.CreateElement("Animation",SchemaSet.XmlNamespace);animation.SetAttribute("AnimationName","SBRefinery_BLD");animation.SetAttribute("AnimationMode","ONCE_BACKWARDS");animation.SetAttribute("AnimationBlendTime","0");animation.SetAttribute("AnimationAbsoluteTime","2s");state.AppendChild(animation);
                        var script=source.CreateElement("Script",SchemaSet.XmlNamespace);script.InnerText="CurDrawableShowSubObjectPermanently(\"Table\")";state.AppendChild(script);
                        var bone=source.CreateElement("ParticleSysBone",SchemaSet.XmlNamespace);bone.SetAttribute("BoneName","NONE");bone.SetAttribute("FXParticleSystemTemplate","Con_Med");bone.SetAttribute("FollowBone","true");state.AppendChild(bone);
                        var anchor=edited.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName is "AttachModel" or "EmbedPortal");edited.InsertBefore(state,anchor);draws.AppendChild(edited);
                    }
                }
                // Reborn: author every PowerUpgrade flag, both trigger lists and the optional animation; defaults mode authors only the required id.
                if(mode.StartsWith("power-",StringComparison.Ordinal))for(int i=0;i<(mode=="power-multiple"?2:1);i++){
                    var module=source.CreateElement("PowerUpgrade",SchemaSet.XmlNamespace);string id="RebornPowerUpgrade"+i;module.SetAttribute("id",id);
                    if(mode!="power-defaults"){
                        if(i==0){
                            module.SetAttribute("Permanent","true");module.SetAttribute("RequiresAllTriggers","1");module.SetAttribute("RequiresAllConflictingTriggers","true");module.SetAttribute("WaypointMode","true");module.SetAttribute("WaypointModeTerminal","false");
                            var animation=source.CreateElement("CustomAnimAndDuration",SchemaSet.XmlNamespace);animation.SetAttribute("AnimState","USER_8");animation.SetAttribute("Frames","12");animation.SetAttribute("Trigger","3");module.AppendChild(animation);
                        }
                        // Reborn: prove an absent upgrade target remains an exact source-owned weak hash through native/metadata/package generation.
                        foreach(var entry in i==0?new[]{("TriggeredBy","Upgrade_JapanFaction"),("TriggeredBy","Upgrade_AlliesFaction"),("TriggeredBy","RebornMissingUpgrade"),("ConflictsWith","Upgrade_Veterancy_HEROIC")}:new[]{("TriggeredBy","Upgrade_AlliesFaction")}){var child=source.CreateElement(entry.Item1,SchemaSet.XmlNamespace);child.InnerText=entry.Item2;module.AppendChild(child);}
                    }
                    behaviors.PrependChild(module);origins.Add(id,"RebornPowerUpgradeTemplate");
                }
                // Reborn: author event-list names covering 4-byte aligned and padded string allocations.
                // Reborn: author new FX modules using all schema event ordinals and both empty/populated death-condition storage.
                if(mode.StartsWith("fx-",StringComparison.Ordinal))for(int i=0;i<(mode=="fx-multiple"?2:1);i++){
                    var module=source.CreateElement("FXListBehavior",SchemaSet.XmlNamespace);string id="RebornFx"+i;module.SetAttribute("id",id);var die=source.CreateElement("DieMuxData",SchemaSet.XmlNamespace);die.SetAttribute("DeathTypes","ALL");module.AppendChild(die);
                    bool expanded=mode!="fx-building"&&i==0;
                    if(expanded){die.SetAttribute("VeterancyLevels","REGULAR VETERAN");die.SetAttribute("ExemptStatus","UNDER_CONSTRUCTION");die.SetAttribute("RequiredStatus","CAN_ATTACK");die.SetAttribute("DeathTypesForbidden","SUICIDED");die.SetAttribute("DamageAmountRequired","17.5");die.SetAttribute("MinKillerAngle","2d");die.SetAttribute("MaxKillerAngle","1r");}
                    string[] kinds={"onDeath","onDamaged","onCreate","onTransitionToDamaged","onTransitionToReallyDamaged","onTransitionToRubble","onBecomingTemporarySlave","onBecomingTemporaryOwner","onEndingTemporarySlave","onEndingTemporaryOwner","onTerrainCollision","onShieldSphereCollision","onSurfacing","onSubmerging"};
                    for(int row=0;row<(expanded?kinds.Length:1);row++){
                        var item=source.CreateElement("Event",SchemaSet.XmlNamespace);item.SetAttribute("Index",kinds[row]);if(mode=="fx-building")item.SetAttribute("FX","FX_MediumBuildingDieExplosion");
                        else if(expanded&&row%3!=0)item.SetAttribute("FX",row%2==0?"FX_InfantrySquished":"FXList:FX_JapanInfantryPsyonicDeath");
                        if(expanded){item.SetAttribute("MinThreshold","0.25");item.SetAttribute("MaxThreshold","7.5");item.SetAttribute("OrientToObject",row%2==0?"1":"0");item.SetAttribute("ForceUseDoFXObj",row%2==0?"true":"false");item.SetAttribute("Direction",new[]{"Front","Rear","Side","Top","Default"}[row%5]);}module.AppendChild(item);
                    }
                    behaviors.PrependChild(module);origins.Add(id,"SquishedFXPlayer");
                }
                if(mode.StartsWith("lua-",StringComparison.Ordinal))for(int i=0;i<(mode=="lua-multiple"?2:1);i++){
                    var module=source.CreateElement("LUAEventList",SchemaSet.XmlNamespace);string id="RebornLUAEventList"+i;module.SetAttribute("id",id);
                    module.SetAttribute("EventListName",mode=="lua-short"?"A":i==0?"BuildingPowerFunctions":"RebornEvents1");behaviors.PrependChild(module);origins.Add(id,"RebornLUAEventListTemplate");
                }
                // Reborn: author every restricted-area Shape field; defaults mode leaves the required shape empty.
                if(mode.StartsWith("structure-",StringComparison.Ordinal))for(int i=0;i<(mode=="structure-multiple"?2:1);i++){
                    var module=source.CreateElement("StructurePlacementBehavior",SchemaSet.XmlNamespace);string id="RebornStructurePlacement"+i;module.SetAttribute("id",id);var shape=source.CreateElement("RestrictedAreaShape",SchemaSet.XmlNamespace);
                    if(mode!="structure-defaults"){
                        shape.SetAttribute("Type",i==0?"CYLINDER":"RING");shape.SetAttribute("MajorRadius",i==0?"12.5":"3");shape.SetAttribute("Height","7.25");
                        if(i==0){shape.SetAttribute("ContactPointGeneration","STRUCTURE");shape.SetAttribute("Name","RebornShape");shape.SetAttribute("Other","RebornOther");shape.SetAttribute("MinorRadius","4.5");shape.SetAttribute("FrontAngle","45d");shape.SetAttribute("Active","false");shape.SetAttribute("UsedForHealthBox","0");}
                        var offset=source.CreateElement("Offset",SchemaSet.XmlNamespace);offset.SetAttribute("x",i==0?"-10":"0.5");offset.SetAttribute("y","2.5");if(i==0)offset.SetAttribute("z","1");shape.AppendChild(offset);
                    }
                    module.AppendChild(shape);behaviors.PrependChild(module);origins.Add(id,"RebornStructurePlacementTemplate");
                }
                // Reborn: author infiltrator values and references from stock-existing assets; defaults mode relies on schema-default names.
                if(mode.StartsWith("infiltrator-",StringComparison.Ordinal))for(int i=0;i<(mode=="infiltrator-multiple"?2:1);i++){
                    var module=source.CreateElement("InfiltratorContain",SchemaSet.XmlNamespace);string id="RebornInfiltrator"+i;module.SetAttribute("id",id);
                    if(mode!="infiltrator-defaults"){
                        module.SetAttribute("BlockedDuration",i==0?"12s":"500ms");module.SetAttribute("Effect",i==0?"VISION":"STEAL_MONEY");module.SetAttribute("Amount",i==0?"1500":"-2.5");module.SetAttribute("Duration","3s");module.SetAttribute("UnitDuration","250ms");
                        if(i==0){module.SetAttribute("InfiltratedStatusFilter","SOLD");module.SetAttribute("ImmediatelyEnabled","true");module.SetAttribute("NameOfVoiceToUseForHostileEnter","RebornHostileVoice");module.SetAttribute("EvaEventForInfiltratingPlayer","BuildingRepaired");module.SetAttribute("EvaEventForInfiltratedPlayer","");
                            module.SetAttribute("FXForInfiltrate","FX_Building_Repaired");module.SetAttribute("ObjectRef","SovietMiner");module.SetAttribute("StructureFilter","InfiltrationCanEnterObjectFilter");module.SetAttribute("ObjectStatusToSet","UNSELECTABLE CAN_ATTACK");}
                    }
                    behaviors.PrependChild(module);origins.Add(id,"RebornInfiltratorTemplate");
                }
                // Reborn: use named EP1 XML, with a second variant to prove omissions and independent pointers.
                if (mode == "reviewed-ep1-behaviors")
                {
                    foreach (var (name, plain, authored) in new[]
                    {
                        ("SupplyCenterDockUpdate", "ValueMultiplier='1.25' DistributedDeposit='false'/>", "ValueMultiplier='1.25' DistributedDeposit='true'/>"),
                        ("TemporaryOwnerSpecialPower", "SpecialPowerTemplate='SpecialPowerAlliedFreeTrade'/>", "SpecialPowerTemplate='SpecialPowerAlliedFreeTrade' SlaveStatus='UNSELECTABLE CANNOT_BE_SOLD'/>"),
                        ("GateProxyBehavior", "/>", "OpenByDefault='true' ResetFrames='19' PercentOpenForPathing='75' ProxyUnitsName='GateProxy'><GeometriesForOpen>GateOpen</GeometriesForOpen></GateProxyBehavior>"),
                        ("RepairDockUpdate", "/>", "FramesForFullHeal='92.5' NumberApproachPositions='3'><ForVoiceRetreatThisIsASafeHarborToObjectFilter Rule='ANY' Include='INFANTRY'/></RepairDockUpdate>"),
                        ("RailedTransportDockUpdate", "/>", "PullInsideDurationInFrames='17' PushOutsideDurationInFrames='41'><ForVoiceRetreatThisIsASafeHarborToObjectFilter Rule='ANY' Include='STRUCTURE'/></RailedTransportDockUpdate>"),
                        ("SupplyCenterProductionExitUpdate", "/>", "><UnitCreatePoint x='-17.5' y='2' z='3'/><NaturalRallyPoint x='75' y='4' z='5'/></SupplyCenterProductionExitUpdate>")
                    })
                    {
                        for (int variant = 0; variant < 2; variant++)
                        {
                            string id = "RebornReview" + name + variant;
                            string end = variant == 0 ? plain : authored;
                            var fixture = new XmlDocument();
                            fixture.LoadXml($"<{name} xmlns='{SchemaSet.XmlNamespace}' id='{id}' {end}");
                            behaviors.PrependChild(source.ImportNode(fixture.DocumentElement!, true));
                            origins.Add(id, SchemaNativeContract.Origin);
                        }
                    }
                }
                // Reborn: author placement hints/distances and a filter with include/exclude rows; nothing is copied from stock.
                if(mode.StartsWith("placement-",StringComparison.Ordinal))for(int i=0;i<(mode=="placement-multiple"?2:1);i++){
                    var module=source.CreateElement("AssistedPlacementBehavior",SchemaSet.XmlNamespace);string id="RebornPlacement"+i;module.SetAttribute("id",id);
                    if(mode!="placement-defaults"){
                        module.SetAttribute("PlacementHint",i==0?"FACING_AWAY ALLOW_SHROUDED":"FACING_TOWARDS");module.SetAttribute("SnappingDistance",i==0?"75.5":"-3");
                        if(i==0){var filter=source.CreateElement("RequirementObjectFilter",SchemaSet.XmlNamespace);filter.SetAttribute("Rule","ALL");filter.SetAttribute("Relationship","NEUTRAL");filter.SetAttribute("Include","INFANTRY");filter.SetAttribute("StatusBitFlagsExclude","UNSELECTABLE");
                            foreach(var thing in new[]{("IncludeThing","SovietMiner"),("ExcludeThing","SovietEngineer"),("ExcludeThing","AlliedEngineer")}){var e=source.CreateElement(thing.Item1,SchemaSet.XmlNamespace);e.InnerText=thing.Item2;filter.AppendChild(e);}module.AppendChild(filter);}
                    }
                    // Reborn: test explicit NONE and the stock nonzero hint independently in the same object.
                    if (mode == "placement-multiple")
                        module.SetAttribute("PlacementHint", i == 0 ? "NONE" : "FACING_TOWARDS ALLOW_SHROUDED");
                    behaviors.PrependChild(module);origins.Add(id,"RebornPlacementTemplate");
                }
                // Reborn: author every engineer field from stock-existing assets; nothing is copied from the refinery payload.
                if(mode.StartsWith("engineer-",StringComparison.Ordinal))for(int i=0;i<(mode=="engineer-multiple"?2:1);i++){
                    var module=source.CreateElement("EngineerContain",SchemaSet.XmlNamespace);string id="RebornEngineer"+i;module.SetAttribute("id",id);
                    if(mode!="engineer-defaults"){
                        module.SetAttribute("ImmediatelyEnabled",i==0?"true":"1");module.SetAttribute("NameOfVoiceToUseForFriendlyEnter",i==0?"RebornFriendlyVoice":"VoiceEngineerRepair");if(i==0)module.SetAttribute("NameOfVoiceToUseForHostileEnter","VoiceCaptureBuilding");
                        module.SetAttribute("ObjectStatusToSet",i==0?"UNSELECTABLE CAN_ATTACK":"NO_REFUND");module.SetAttribute("FXForRepair","FX_Building_Captured");if(i==0){module.SetAttribute("FXForReplace","FX_Building_Repaired");module.SetAttribute("EvaEventForCaptureAndReplace","BuildingRepaired");}module.SetAttribute("EvaEventForRepair",i==0?"":"BuildingRepaired");
                        if(i==0){var filter=source.CreateElement("CanEnterFilter",SchemaSet.XmlNamespace);filter.SetAttribute("Rule","ANY");filter.SetAttribute("Relationship","ALLIES ENEMIES");filter.SetAttribute("Alignment","GOOD");filter.SetAttribute("Include","INFANTRY");filter.SetAttribute("Exclude","STRUCTURE");filter.SetAttribute("StatusBitFlags","CAN_ATTACK");
                            foreach(var thing in new[]{("IncludeThing","SovietEngineer"),("IncludeThing","AlliedEngineer"),("ExcludeThing","JapanEngineer")}){var e=source.CreateElement(thing.Item1,SchemaSet.XmlNamespace);e.InnerText=thing.Item2;filter.AppendChild(e);}module.AppendChild(filter);}
                    }
                    behaviors.PrependChild(module);origins.Add(id,"RebornEngineerTemplate");
                }
                // Reborn: exercise inline percentages, two distinct repair sound imports, and absent sound defaults.
                if(mode.StartsWith("repair-",StringComparison.Ordinal))for(int i=0;i<(mode is "repair-multiple" or "repair-audio"?2:1);i++){
                    var module=source.CreateElement("RepairUpdate",SchemaSet.XmlNamespace);string id="RebornRepair"+i;module.SetAttribute("id",id);
                    var percent=source.CreateElement("PercentOfBuildCostToRebuildInfo",SchemaSet.XmlNamespace);var audio=source.CreateElement("SelfAudio",SchemaSet.XmlNamespace);
                    if(mode!="repair-defaults"){module.SetAttribute("Toggleable",i==0?"false":"1");module.SetAttribute("RepairableWhenDead","true");module.SetAttribute("RepopThreshold",i==0?"0.75":"0.125");module.SetAttribute("HealAmountPerSecond",i==0?"37":"4294967295");percent.SetAttribute("Pristine","25");percent.SetAttribute("Damaged","75%");percent.SetAttribute("ReallyDamaged",i==0?"100":"10");percent.SetAttribute("Rubble","0");}
                    if(mode=="repair-audio"&&i==0){
                        audio.SetAttribute("SelfRepairFromDamageLoop","SOV_ConstructionYard_Select");
                        audio.SetAttribute("SelfRepairFromRubbleLoop","SOV_ConstructionYard_Die");
                    }
                    module.AppendChild(percent);module.AppendChild(audio);behaviors.PrependChild(module);origins.Add(id,"RebornRepairTemplate");
                }
                // Reborn: author every production scalar, including EP1 WATERCRAFT and NewObjectFadeInTime; nothing is copied from the stock refinery payload.
                if(mode.StartsWith("production-",StringComparison.Ordinal))for(int i=0;i<(mode=="production-multiple"?2:1);i++){
                    var module=source.CreateElement("ProductionUpdate",SchemaSet.XmlNamespace);string id="RebornProduction"+i;module.SetAttribute("id",id);
                    // Reborn: exercise one authored weak bonus ID while the default case omits it.
                    if (mode == "production-edited")
                        module.SetAttribute("BonusForType", "SovietMiner");
                    // Reborn: exercise weak-template identity, signed counts and order through complete named EP1 source.
                    if (mode == "production-quantity")
                    {
                        foreach (var (name, amount) in new[] { ("SovietMiner", "-3"), ("SovietAntiInfantryInfantry", "7") })
                        {
                            var modifier = source.CreateElement("QuantityModifier", SchemaSet.XmlNamespace);
                            modifier.SetAttribute("Name", name);
                            modifier.SetAttribute("Count", amount);
                            module.AppendChild(modifier);
                        }
                    }
                    // Reborn: prove two external upgrade imports, a populated native filter and omitted scalar defaults.
                    if (mode == "production-modifier")
                    {
                        var populated = source.CreateElement("ProductionModifier", SchemaSet.XmlNamespace);
                        populated.SetAttribute("RequiredUpgrade", "Upgrade_ProductionVeterancy_VETERAN");
                        populated.SetAttribute("CostMultiplier", "0.75");
                        populated.SetAttribute("TimeMultiplier", "1.25");
                        populated.InnerXml = "<ModifierFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='INFANTRY' Exclude='STRUCTURE' StatusBitFlags='IMMOBILE'><IncludeThing>SovietMiner</IncludeThing><ExcludeThing>SovietAntiInfantryInfantry</ExcludeThing></ModifierFilter>";
                        module.AppendChild(populated);
                        var defaults = source.CreateElement("ProductionModifier", SchemaSet.XmlNamespace);
                        defaults.SetAttribute("RequiredUpgrade", "Upgrade_ProductionVeterancy_ELITE");
                        defaults.InnerXml = "<ModifierFilter xmlns='uri:ea.com:eala:asset'/>";
                        module.AppendChild(defaults);
                    }
                    if(mode!="production-defaults"){
                        module.SetAttribute("MaxQueueEntries",i==0?"7":"4294967295");module.SetAttribute("NumDoorAnimations",i==0?"2":"-3");module.SetAttribute("DoorOpeningTime",i==0?"1250ms":"0.25s");module.SetAttribute("DoorWaitOpenTime","3.5s");module.SetAttribute("DoorCloseTime","750ms");
                        module.SetAttribute("ConstructionCompleteDuration","33");module.SetAttribute("UnitInvulnerableTime","500");module.SetAttribute("SpecialPrepModelconditionTime","250");module.SetAttribute("Type",i==0?"WATERCRAFT":"INFANTRY");
                        module.SetAttribute("ProductionTimeDelayScalar","0.5s");module.SetAttribute("NewObjectFadeInTime",i==0?"1.75s":"0s");
                        foreach(string name in new[]{"GiveNoXP","VeteranUnitsFromVeteranFactory","SecondaryQueue","IgnorePreReqs"})module.SetAttribute(name,i==0?"true":"1");
                        // Reborn: edited pairs, a source count beyond the stock three rows and a missing optional RequiredUpgrade share one full package batch.
                        var give=source.CreateElement("GiveUpgradeToProduced",SchemaSet.XmlNamespace);string[] ranks={"VETERAN","ELITE","HEROIC"};int translations=mode=="production-multiple"?(i==0?5:1):3;
                        for(int row=0;row<translations;row++){var translation=source.CreateElement("UpgradeTranslate",SchemaSet.XmlNamespace);if(i==0)translation.SetAttribute("RequiredUpgrade","Upgrade_ProductionVeterancy_"+ranks[row%3]);translation.SetAttribute("ProvidedUpgrade","Upgrade_CreationVeterancy_"+ranks[(row+1)%3]);give.AppendChild(translation);}module.AppendChild(give);
                    }
                    behaviors.PrependChild(module);origins.Add(id,"RebornProductionTemplate");
                }
                // Reborn: retarget audio independently of the original refinery and mix omitted audio with a second edited unpack module.
                if(mode.StartsWith("unpack-",StringComparison.Ordinal))for(int i=0;i<(mode=="unpack-multiple"?2:1);i++){
                    var module=source.CreateElement("StructureUnpackUpdate",SchemaSet.XmlNamespace);string id="RebornUnpack"+i;module.SetAttribute("id",id);
                    if(mode!="unpack-defaults"){module.SetAttribute("UnpackTime",i==0?"1250ms":"0.25s");module.SetAttribute("OffsetHeightAboveWater",i==0?"17.5":"-2.25");if(i==0)module.SetAttribute("UnpackCompleteSound","SOV_Conscript_VoiceAttack");}
                    // Reborn: an explicit empty sound is an allocated null reference, not a missing audio asset.
                    if(mode=="unpack-empty")module.SetAttribute("UnpackCompleteSound","");
                    // Reborn: the official reference pattern permits empty text but rejects whitespace-only identities.
                    if(mode=="unpack-empty"){var whitespace=Copy(source);var bad=whitespace.CreateElement("StructureUnpackUpdate",SchemaSet.XmlNamespace);bad.SetAttribute("id","RebornWhitespaceSound");bad.SetAttribute("UnpackCompleteSound","   ");((XmlElement)whitespace.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!).AppendChild(bad);Refuse(()=>ConscriptVariableBehaviorSource.Admit(whitespace,baseline,official));faults++;}
                    behaviors.PrependChild(module);origins.Add(id,"RebornUnpackTemplate");
                }
                // Reborn: populate all Spawn storage families together, including the three EP1 flags and nested status/Die/upgrade records.
                if(mode.StartsWith("spawn-",StringComparison.Ordinal))for(int i=0;i<(mode=="spawn-multiple"?2:1);i++){
                    var module=source.CreateElement("SpawnBehavior",SchemaSet.XmlNamespace);string id="RebornSpawn"+i;module.SetAttribute("id",id);
                    if(mode!="spawn-defaults"){
                        module.SetAttribute("SpawnNumberData","3");module.SetAttribute("InitialBurst","2");module.SetAttribute("SpawnReplaceDelayData","1250ms");module.SetAttribute("FadeInTime","0.25s");module.SetAttribute("InitialDelay","2s");module.SetAttribute("DamageTypesToPropagateToSlaves","GUN CANNON");
                        foreach(string name in new[]{"IsOneShotData","CanReclaimOrphans","AggregateHealth","ExitByBudding","SpawnedRequireSpawner","RespectCommandLimit","KillSpawnBasedOnModelConditionState","ShareUpgrades","SpawnInsideBuilding","CombineOnCreate","KillSpawnsOnDisabled","PassExperienceToSpawned","KillSpawnsOnCaptured","KillSpawnsWhenObjectInvalid","SpawnOnUnpack","SetPurchasePrice"})module.SetAttribute(name,i==0?"true":"false");
                        var animation=source.CreateElement("CustomAnimAndDuration",SchemaSet.XmlNamespace);animation.SetAttribute("AnimState","DAMAGED");animation.SetAttribute("Frames","7");animation.SetAttribute("Trigger","2");module.AppendChild(animation);
                        string upgrade=baseline.GetElementsByTagName("TriggeredBy",SchemaSet.XmlNamespace).OfType<XmlElement>().First().InnerText;
                        foreach(string name in new[]{"TriggeredBy","ConflictsWith"}){var trigger=source.CreateElement(name,SchemaSet.XmlNamespace);trigger.InnerText=upgrade;module.AppendChild(trigger);}
                        var die=source.CreateElement("Die",SchemaSet.XmlNamespace);die.SetAttribute("VeterancyLevels","REGULAR VETERAN");die.SetAttribute("ExemptStatus","UNDER_CONSTRUCTION");die.SetAttribute("RequiredStatus","CAN_ATTACK");die.SetAttribute("DeathTypes","ALL");die.SetAttribute("DamageAmountRequired","17.5");module.AppendChild(die);
                        foreach(string value in new[]{"SovietMiner","SovietAntiInfantryInfantry"}){var template=source.CreateElement("SpawnTemplate",SchemaSet.XmlNamespace);template.InnerText=value;module.AppendChild(template);}
                        var status=source.CreateElement("ObjectStatusValidation",SchemaSet.XmlNamespace);status.SetAttribute("ForbiddenStatus","UNSELECTABLE");status.SetAttribute("RequiredStatus","CAN_ATTACK");module.AppendChild(status);
                    }
                    behaviors.PrependChild(module);origins.Add(id,"RebornSpawnTemplate");
                }
                // Reborn: author every exit value from schema names; neither coordinates nor timing are borrowed from the stock payload.
                if(mode.StartsWith("exit-",StringComparison.Ordinal))for(int i=0;i<(mode=="exit-multiple"?2:1);i++){
                    var module=source.CreateElement("QueueProductionExitUpdate",SchemaSet.XmlNamespace);string id="RebornExit"+i;module.SetAttribute("id",id);
                    if(mode!="exit-defaults"){module.SetAttribute("PlacementViewAngle",i==0?"90d":"1r");module.SetAttribute("ExitDelaySeconds",i==0?"1250ms":"0.25s");module.SetAttribute("InitialBurst","17");module.SetAttribute("AllowAirborneCreation","1");module.SetAttribute("NoExitPath","true");module.SetAttribute("CanRallyToSlaughter","true");module.SetAttribute("ClearAlliesFromDestination","false");}
                    foreach(string name in new[]{"UnitCreatePoint","NaturalRallyPoint"}){var vector=source.CreateElement(name,SchemaSet.XmlNamespace);if(mode!="exit-defaults"){vector.SetAttribute("x",name=="UnitCreatePoint"?"-17.5":"75");vector.SetAttribute("y","20.25");vector.SetAttribute("z","1.5");}module.AppendChild(vector);}behaviors.PrependChild(module);origins.Add(id,"RebornQueueExitTemplate");
                }
                // Reborn: no docking bone name or animation is pinned to stock, and omitted names retain their actual schema-empty storage.
                if(mode.StartsWith("dock-",StringComparison.Ordinal))for(int i=0;i<(mode=="dock-multiple"?2:1);i++){
                    var module=source.CreateElement("QueueDockBehavior",SchemaSet.XmlNamespace);string id="RebornDock"+i;module.SetAttribute("id",id);if(mode!="dock-defaults"){foreach(string name in QueueDockContract.Names)module.SetAttribute(name,i==0?"Reborn_"+name:"");module.SetAttribute("DockingAnimation",i==0?"DAMAGED":"DOCKING_ACTIVE");}behaviors.PrependChild(module);origins.Add(id,"RebornQueueDockTemplate");
                }
                // Reborn: every destruction field remains authored, and repeated item rows are never cloned from stock native bytes.
                if(mode.StartsWith("destruction-",StringComparison.Ordinal)){
                    var module=source.CreateElement("BuildingDestructionBehavior",SchemaSet.XmlNamespace);module.SetAttribute("id","RebornDestruction");var die=source.CreateElement("DieMuxData",SchemaSet.XmlNamespace);die.SetAttribute("DeathTypes","ALL");die.SetAttribute("DeathTypesForbidden","SUICIDED");module.AppendChild(die);
                    if(mode=="destruction-edited"){module.SetAttribute("AssociatedModule",original.First(e=>e.LocalName=="Physics").GetAttribute("id"));module.SetAttribute("FillerMesh","AuthoredMesh");module.SetAttribute("MaxDebrisCloudsPerSecond","3.25");die.SetAttribute("DamageAmountRequired","9");die.SetAttribute("MinKillerAngle","2d");}
                    for(int i=0;i<(mode=="destruction-expanded"?257:1);i++){var item=source.CreateElement("Item",SchemaSet.XmlNamespace);if(mode!="destruction-defaults"){item.SetAttribute("BonePrefix",i%2==0?"BONE_REBORN_":"");item.SetAttribute("FlingVelocity",i%2==0?"75.5":"2");item.SetAttribute("CreationObject","SovietBarracks_Debris");if(i%2==0)item.SetAttribute("FX","SovietRefineryHit");}module.AppendChild(item);}behaviors.PrependChild(module);origins.Add("RebornDestruction","RebornDestructionTemplate");
                }
                // Reborn: neither source order nor module IDs require the new family to have existed on the stock Conscript.
                if(mode.StartsWith("sell-",StringComparison.Ordinal)){
                    for(int i=0;i<(mode=="sell-multiple"?2:1);i++){var module=source.CreateElement("StructureSellUpdate",SchemaSet.XmlNamespace);string id="RebornSell"+i;module.SetAttribute("id",id);if(mode!="sell-defaults"){module.SetAttribute("SellTime",i==0?"4.5s":"1s");module.SetAttribute("ActiveModelCondition",i==0?"DAMAGED":"SOLD");}behaviors.PrependChild(module);origins.Add(id,"RebornStockStructureSell");}
                }
                // Reborn: preserve authored presence/cardinality and exact uint32 values instead of synthesizing a single zero transaction.
                if(mode.StartsWith("resource-",StringComparison.Ordinal))
                {
                    var resource=GameObjectResourceContract.Owner(source)!;resource.RemoveAll();
                    if(mode=="resource-omitted")source.DocumentElement!.RemoveChild(resource);
                    else for(int i=0;i<(mode=="resource-empty"?0:mode=="resource-expanded"?40:1);i++){var row=source.CreateElement("BuildCost",SchemaSet.XmlNamespace);row.SetAttribute("Account",mode=="resource-uint32"?"0xA":i.ToString(System.Globalization.CultureInfo.InvariantCulture));row.SetAttribute("Amount",mode=="resource-uint32"?"0xFFFFFFFF":mode=="resource-zero"?"0":(i+100).ToString(System.Globalization.CultureInfo.InvariantCulture));resource.AppendChild(row);}
                }
                // Reborn: zero, default and forty-row movement graphs exercise native bounds instead of the historical one/32 row limits.
                if(mode.StartsWith("movement-",StringComparison.Ordinal))
                {
                    foreach(var row in ConscriptLocomotorSetContract.Entries(source))source.DocumentElement!.RemoveChild(row);
                    var anchor=source.DocumentElement!.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="SkirmishAIInformation");
                    for(int i=0;i<(mode=="movement-omitted"?0:mode=="movement-expanded"?40:1);i++)
                    {
                        var row=source.CreateElement("LocomotorSet",SchemaSet.XmlNamespace);
                        if(mode!="movement-defaults"){row.SetAttribute("Speed",(30+i).ToString(System.Globalization.CultureInfo.InvariantCulture));row.SetAttribute("Condition",i%2==0?"NORMAL":"SPECIALABILITY_ACTIVE");if(i%3!=1)row.SetAttribute("Locomotor","TestReactiveLocomotorHUMAN");}
                        source.DocumentElement.InsertBefore(row,anchor);
                    }
                }
                // Reborn: exercise all implemented plain-record fields together, including packed geometry bool and vision percentage/default semantics.
                if(mode.StartsWith("nested-",StringComparison.Ordinal))
                {
                    var vision=(XmlElement)source.GetElementsByTagName("VisionInfo",SchemaSet.XmlNamespace)[0]!;
                    foreach(var value in new[]{("VisionSide","75%"),("VisionRear","25%"),("VisionBonusTestRadius","123.5"),("VisionBonusTestSegments","12"),("VisionBonusPercentPerFoot","2%"),("MaxVisionBonusPercent","80%"),("MinVisionBonusPercent","-20%")})vision.SetAttribute(value.Item1,value.Item2);
                    ((XmlElement)source.GetElementsByTagName("BuildCost",SchemaSet.XmlNamespace)[0]!).SetAttribute("Account","7");
                    ((XmlElement)source.GetElementsByTagName("Geometry",SchemaSet.XmlNamespace)[0]!).SetAttribute("IsSmall","false");
                    // Reborn: geometry dimensions and packed shape flags must be editable without cloning stock geometry payloads.
                    foreach(XmlElement shape in source.GetElementsByTagName("Geometry",SchemaSet.XmlNamespace)[0]!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Shape")){shape.SetAttribute("MajorRadius","17.5");shape.SetAttribute("Height","23.25");shape.SetAttribute("Active","false");shape.SetAttribute("UsedForHealthBox","false");}
                    if(mode=="nested-defaults")foreach(XmlAttribute attribute in vision.Attributes.Cast<XmlAttribute>().Where(a=>a.LocalName!="VisionRange").ToArray())vision.RemoveAttribute(attribute.Name);
                }
                // Reborn: every already-supported fixed primitive category must compose with variable modules and changed root-reference presence in one package.
                if(mode.StartsWith("shared-",StringComparison.Ordinal)){
                    GameObjectScalarContract.AuthorProbe(source);GameObjectRootAttributeContract.AuthorProbe(source);GameObjectRootStringContract.AuthorProbe(source,baseline);ConscriptModuleScalarContract.AuthorProbe(source);
                    foreach(var field in ConscriptEditableScalars.Fields.Skip(1))ConscriptEditableScalars.Element(source,field).SetAttribute(field.Attribute,field.Attribute switch{"Amount"=>"111","MaxHealth"=>"543.25","VisionRange"=>"777.5","ShroudClearingRange"=>"654.25",_=>"88.5"});
                    source.DocumentElement!.RemoveAttribute("UnitTypeIcon");source.DocumentElement.RemoveAttribute("UnitIntro");
                    if(mode=="shared-defaults"){var icon=catalog.First(a=>a.TypeId==FastHash.GetHashCode("UnitTypeIcon"));source.DocumentElement.SetAttribute("UnitTypeIcon",icon.Name[(icon.Name.IndexOf(':')+1)..]);}
                    if(mode=="shared-defaults")foreach(var field in ConscriptEditableScalars.Fields.Where(f=>f.Attribute is "MaxHealth" or "VisionRange" or "ShroudClearingRange" or "Speed"))ConscriptEditableScalars.Element(source,field).RemoveAttribute(field.Attribute);
                    if(!ConscriptSourceInput.FixedAccepted(source,baseline,official))throw new InvalidDataException("Shared supported values were not accepted by the fixed source route.");
                    // Reborn: after proving the unchanged topology accepts the same values, force genuinely different behavior topology through the shared native verifier.
                    var clone=(XmlElement)original.First(e=>e.LocalName=="Physics").CloneNode(true);string id="RebornParityPhysics";origins.Add(id,clone.GetAttribute("id"));clone.SetAttribute("id",id);behaviors.AppendChild(clone);foreach(var child in behaviors.ChildNodes.OfType<XmlElement>().Reverse().ToArray())behaviors.AppendChild(child);
                }
                // Reborn: one batch exercises real source-sized armor/hardpoint/weapon lists, optional imports, defaults, flags and official weapon sorting.
                if(mode.StartsWith("combat-",StringComparison.Ordinal)){
                    foreach(var entry in ConscriptArmorSetContract.Entries(source))source.DocumentElement!.RemoveChild(entry);
                    var anchor=source.DocumentElement!.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="LocomotorSet");
                    for(int i=0;i<(mode=="combat-combined"?40:mode=="combat-single"?1:0);i++){var armor=source.CreateElement("ArmorSet",SchemaSet.XmlNamespace);armor.SetAttribute("Armor",i%2==0?"AlliedConYardArmor":"SovietAntiInfantryInfantryArmor");if(i%2==0)armor.SetAttribute("DamageFX","InfantryDamageFX");if(i%3==0)armor.SetAttribute("Conditions","VETERAN SHRINK_EFFECT");source.DocumentElement.InsertBefore(armor,anchor);}
                    var module=(XmlElement)source.GetElementsByTagName("WeaponSetUpdate",SchemaSet.XmlNamespace)[0]!;foreach(var child in module.ChildNodes.OfType<XmlElement>().ToArray())module.RemoveChild(child);
                    for(int i=0;i<(mode=="combat-combined"?10:mode=="combat-single"?1:0);i++){var hp=source.CreateElement("WeaponSlotHardpoint",SchemaSet.XmlNamespace);hp.SetAttribute("ID",(i+1).ToString(System.Globalization.CultureInfo.InvariantCulture));hp.SetAttribute("AutoChooseWeapon","1");hp.SetAttribute("AllowInterleavedFiring","true");for(int j=0;j<(mode=="combat-combined"?17:1);j++){var row=source.CreateElement("Weapon",SchemaSet.XmlNamespace);row.SetAttribute("Ordering",j%2==0?"SECONDARY_WEAPON":"PRIMARY_WEAPON");if(j%4!=1)row.SetAttribute("Template",j%2==0?"JapanArcherInfantryEnergyBow":"SovietAntiInfantryInfantryAK47");if(j%3==0)row.SetAttribute("Upgrade","Upgrade_Veterancy_HEROIC");row.SetAttribute("ObjectStatus",j%2==0?"GENERIC_TOGGLE_STATE":"");row.SetAttribute("IsPlayerUpgradePermanent","1");hp.AppendChild(row);}module.AppendChild(hp);}
                    source.GetElementsByTagName("DisplayName",SchemaSet.XmlNamespace)[0]!.InnerText="Name:RebornCombatLists";
                }
                // Reborn: display keys change independently of stock text, including absent/empty/transformed forms and combined description edits.
                if(mode.StartsWith("root-text-",StringComparison.Ordinal)){
                    foreach(string name in GameObjectRootTextContract.Pointed){var child=source.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==name);if(child!=null)source.DocumentElement.RemoveChild(child);if(mode=="root-text-omitted")continue;child=source.CreateElement(name,SchemaSet.XmlNamespace);child.InnerText=mode=="root-text-empty"?string.Empty:"Name:Reborn & <Custom> "+name+new string('R',300);source.DocumentElement.InsertBefore(child,source.DocumentElement.ChildNodes.OfType<XmlElement>().First(e=>!GameObjectRootTextContract.Pointed.Contains(e.LocalName)));}
                    GameObjectRootStringContract.AuthorProbe(source,baseline);
                }
                if(mode is "remove-half" or "combined" or "new-id-combined" or "retarget-combined")foreach(var node in original.Where((_,i)=>i%2==0))behaviors.RemoveChild(node);
                if(mode is "clone-twelve" or "combined" or "new-id-combined" or "retarget-combined")foreach(var node in original.Take(12)){var copy=(XmlElement)node.CloneNode(true);string id="RebornGraphClone_"+origins.Count;origins.Add(id,node.GetAttribute("id"));copy.SetAttribute("id",id);behaviors.AppendChild(copy);}
                if(mode is "reverse" or "combined" or "new-id-combined" or "retarget-combined")foreach(var node in behaviors.ChildNodes.OfType<XmlElement>().Reverse().ToArray())behaviors.AppendChild(node);
                // Reborn: combined composition must preserve authored compatible targets instead of comparing them to stock asset names.
                if(mode=="retarget-combined"){
                    int edits=0;foreach(XmlElement weapon in source.GetElementsByTagName("Weapon",SchemaSet.XmlNamespace))if(weapon.GetAttribute("Template").Contains("SovietAntiInfantryInfantryAK47",StringComparison.Ordinal)){weapon.SetAttribute("Template","JapanArcherInfantryEnergyBow");edits++;}
                    if(edits==0)throw new InvalidDataException("Combined reference control did not edit a real weapon slot.");
                    // Reborn: typed weak upgrade triggers must also remain editable together with structural composition and strong references.
                    int weakEdits=0;foreach(XmlElement trigger in source.GetElementsByTagName("TriggeredBy",SchemaSet.XmlNamespace))if(trigger.InnerText.Trim() is "Upgrade_JapanFaction" or "Upgrade_AlliesFaction"){trigger.InnerText=trigger.InnerText.Trim()=="Upgrade_JapanFaction"?"Upgrade_AlliesFaction":"Upgrade_JapanFaction";weakEdits++;}
                    if(weakEdits==0)throw new InvalidDataException("Combined upgrade control did not edit a real weak-reference slot.");
                }
                if(mode=="beyond-former-budget"){var physics=original.Single(e=>e.LocalName=="Physics");while(behaviors.ChildNodes.OfType<XmlElement>().Count()<65){var copy=(XmlElement)physics.CloneNode(true);string id="RebornUncapped_"+origins.Count;origins.Add(id,physics.GetAttribute("id"));copy.SetAttribute("id",id);behaviors.AppendChild(copy);}}
                // Reborn: upgrade nesting is authored independently of stock list cardinality and optional-pointer presence.
                if(mode.StartsWith("upgrade-",StringComparison.Ordinal)){
                    var module=behaviors.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="ModelConditionUpgrade");
                    if(mode is "upgrade-lists-expanded" or "upgrade-lists-empty")foreach(var child in module.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "TriggeredBy" or "ConflictsWith").ToArray())module.RemoveChild(child);
                    if(mode!="upgrade-lists-empty"){
                        var animation=source.CreateElement("CustomAnimAndDuration",SchemaSet.XmlNamespace);if(mode!="upgrade-animation-defaults"){animation.SetAttribute("AnimState","USER_8");animation.SetAttribute("Frames","12");animation.SetAttribute("Trigger","3");}module.PrependChild(animation);
                        if(mode=="upgrade-animation-lexical"){animation.SetAttribute("Frames","0xC");animation.SetAttribute("Trigger","0.0");}
                    }
                    if(mode=="upgrade-lists-expanded")foreach(var entry in new[]{("TriggeredBy","Upgrade_JapanFaction"),("TriggeredBy","Upgrade_AlliesFaction"),("TriggeredBy","Upgrade_Veterancy_HEROIC"),("ConflictsWith","Upgrade_AlliesFaction"),("ConflictsWith","Upgrade_JapanFaction")}){var child=source.CreateElement(entry.Item1,SchemaSet.XmlNamespace);child.InnerText=entry.Item2;module.AppendChild(child);}
                    // Reborn: a real 5000-entry source list must use allocation bounds instead of the old tokenizer quota.
                    if(mode=="upgrade-list-large")for(int i=1;i<5000;i++){var child=source.CreateElement("TriggeredBy",SchemaSet.XmlNamespace);child.InnerText="Upgrade_AlliesFaction";module.AppendChild(child);}
                    module.RemoveAttribute("RequiresAllTriggers");module.RemoveAttribute("RequiresAllConflictingTriggers");
                }
                var admitted=ConscriptVariableBehaviorSource.Admit(source,baseline,official);
                // Reborn: the real admission plan supplies leaf templates for new/reordered draw and client identities.
                // Reborn: use the schema-proved origins for newly authored environment leaves.
                if(mode is "destructible-environment" or "locomotor-ability" or "replace-self-upgrade" or "substructure-build" or "delayed-weapon-fire" or "leech-targeting" or "transport-ai" or "wander-ai" or "emp-update" or "weapon-contained" or "weapon-scatter" or "give-restore-upgrade" or "activation-ability" or "naval-modifier" or "inactive-body" or "shield-body" or "shield-body-default" or "create-enter" or "status-ability" or "special-power-update" or "damage-field" or "horde-ai" or "supply-truck-ai" or "bezier-spline" or "sphere-update" or "steal-unit" or "booby-trap"||mode.StartsWith("containers-",StringComparison.Ordinal)||mode.StartsWith("draws-",StringComparison.Ordinal)||mode.StartsWith("audio-",StringComparison.Ordinal))origins=admitted.Origins;
                // Reborn: a new object identity receives a command only from an explicit authored declaration, never from its ID alone.
                if(mode=="new-id-combined"){if(catalog.Any(a=>a.TypeId==0x942FFF2D&&a.InstanceId==InstanceHandle.GetInstanceId("RebornConscriptTrial")))throw new InvalidDataException("Variable trial ID collides with stock.");source.DocumentElement!.SetAttribute("id","RebornConscriptTrial");}
                var pair=Build(source);var tokens=Verify(source,pair.Native,pair.Instance,origins);var repeat=Build(source);
                if(!Equal(pair.Native,repeat.Native)||!Equal(tokens,Verify(source,repeat.Native,repeat.Instance,origins)))throw new InvalidDataException("Variable graph repeated build differs.");
                int prefix=tokens.InstanceBuffer.Length-pair.Native.InstanceBuffer.Length;
                // Reborn: corrupt every assign-slaves header byte plus its concrete/base metadata; accepted authoring is still independently source-verified.
                if(mode.StartsWith("assign-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.NativeType==typeof(AssignSlavesTargetObjectSpecialPowerModuleData))){
                    int nativeAt=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);for(int offset=0;offset<System.Runtime.InteropServices.Marshal.SizeOf<AssignSlavesTargetObjectSpecialPowerModuleData>();offset++){var bad=Clone(pair.Native);bad.InstanceBuffer[nativeAt+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int listAt=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset,wrapper=(int)Word(tokens.InstanceBuffer,listAt+12+owner.Index*12+8);foreach(int offset in new[]{0,4,8,12,16,20,24,28,32,36,40,44,48,52,56}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: window string lengths/pointers/text and class/inheritance metadata must fail independently when corrupted.
                if(mode.StartsWith("windows-",StringComparison.Ordinal))foreach(var window in ConscriptModuleScalarContract.Owners(source).Where(o=>o.NativeType==typeof(BuildingBehaviorModuleData))){
                    int nativeAt=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,window);var positions=new HashSet<int>(Enumerable.Range(0,40).Select(i=>nativeAt+i));foreach(int o in new[]{12,20,28,36}){int target=(int)Word(pair.Native.InstanceBuffer,nativeAt+o);if(target!=0)positions.Add(target);}foreach(int at in positions){var bad=Clone(pair.Native);
                        // Reborn: an empty string may legally point at adjacent NUL padding; use an invalid target instead of mistaking equivalent placement for corruption.
                        int relative=at-nativeAt;if(new[]{12,20,28,36}.Contains(relative)&&Word(pair.Native.InstanceBuffer,nativeAt+relative-4)==0&&Word(pair.Native.InstanceBuffer,at)!=0)BinaryPrimitives.WriteUInt32LittleEndian(bad.InstanceBuffer.AsSpan(at),uint.MaxValue);else bad.InstanceBuffer[at]^=1;
                        try{Refuse(()=>Verify(source,bad,pair.Instance,origins));}catch(InvalidDataException error){throw new InvalidDataException("Window corruption control failed: "+window.Element.GetAttribute("id")+" native offset "+relative+" ("+pair.Native.InstanceBuffer[at]+" -> "+bad.InstanceBuffer[at]+").",error);}faults++;}
                    int listAt=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset,wrapper=(int)Word(tokens.InstanceBuffer,listAt+12+window.Index*12+8);foreach(int offset in new[]{4,8,20,32,44,56,68,80}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: every crusher record byte, including signed levels/booleans/imports and padding, must reject corruption after the source proof passes.
                if(mode.StartsWith("crusher-",StringComparison.Ordinal)){
                    int crusherAt=(int)Word(pair.Native.InstanceBuffer,564);var rootRows=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0);int crusherTable=rootRows[FastHash.GetHashCode("CrusherInfo")].Offset;if(crusherAt!=0){for(int i=0;i<52;i++){var bad=Clone(pair.Native);bad.InstanceBuffer[crusherAt+i]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}foreach(int offset in new[]{0,4,8,20,32,44,56,68}){var bad=Clone(tokens);bad.InstanceBuffer[crusherTable+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}}
                    var badPointer=Clone(pair.Native);badPointer.InstanceBuffer[564]^=1;Refuse(()=>Verify(source,badPointer,pair.Instance,origins));faults++;
                }
                // Reborn: every AI header word and representative chooser/import/metadata targets are independently corrupted after a successful proof.
                if(mode.StartsWith("chooser-",StringComparison.Ordinal)){
                    int aiStart=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="AI"));var positions=new HashSet<int>(Enumerable.Range(0,19).Select(i=>aiStart+4*i));int chooserAt=(int)Word(pair.Native.InstanceBuffer,aiStart+68);if(chooserAt!=0)foreach(int o in Enumerable.Range(0,10).Select(i=>4*i))positions.Add(chooserAt+o);
                    foreach(int at in positions){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int aiTable=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("AI")].Offset;foreach(int offset in new[]{0,4,8,12,16,20,28,32,40,44,52,56,64,68,76,80}){var bad=Clone(tokens);bad.InstanceBuffer[aiTable+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: optional pointer/list headers, complete root native bytes and root metadata targets have independent corruption controls.
                if(mode.StartsWith("roots-",StringComparison.Ordinal)||mode.StartsWith("skirmish-",StringComparison.Ordinal)||mode.StartsWith("audio-",StringComparison.Ordinal))foreach(var field in Ep1GameObjectRootOwnersProjection.Fields.Where(f=>mode.StartsWith("skirmish-",StringComparison.Ordinal)?f.Name=="SkirmishAIInformation":mode.StartsWith("audio-",StringComparison.Ordinal)?f.Name is "AudioArrayVoice" or "EvaEventArray":GameObjectExtendedRootContract.Names.Contains(f.Name,StringComparer.Ordinal))){
                    foreach(int slot in field.InlineList?new[]{field.Slot,field.PointerSlot}:new[]{field.Slot}){var bad=Clone(pair.Native);bad.InstanceBuffer[slot]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int start=Ep1GameObjectRootOwnersProjection.NativeStart(pair.Native.InstanceBuffer,field);if(start!=0){int index=Ep1GameObjectRootOwnersProjection.Fields.ToList().IndexOf(field),end=Ep1GameObjectRootOwnersProjection.Fields.Skip(index+1).Select(f=>Ep1GameObjectRootOwnersProjection.NativeStart(pair.Native.InstanceBuffer,f)).FirstOrDefault(at=>at!=0);if(end==0)end=pair.Native.InstanceBuffer.Length;foreach(int at in new[]{start,start+4,end-1}){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}}
                    int row=Enumerable.Range(0,prefix/12).Select(i=>i*12).First(at=>Word(tokens.InstanceBuffer,at)==FastHash.GetHashCode(field.Name));foreach(int offset in new[]{4,8}){var bad=Clone(tokens);bad.InstanceBuffer[row+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: AI corruption controls cover every header word and each live optional pointee independently of source admission.
                if(mode.StartsWith("skirmish-",StringComparison.Ordinal)){
                    int aiAt=(int)Word(pair.Native.InstanceBuffer,468);if(aiAt!=0){var offsets=new HashSet<int>(Enumerable.Range(0,23).Select(i=>aiAt+i*4));foreach(int slot in new[]{20,36,52,56,60,72,76,80}){int target=(int)Word(pair.Native.InstanceBuffer,aiAt+slot);if(target!=0)offsets.Add(target);}foreach(int at in offsets){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                        int tableAt=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("SkirmishAIInformation")].Offset;foreach(int offset in new[]{4,8,20,32,44,56,68,80,92}){var bad=Clone(tokens);bad.InstanceBuffer[tableAt+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}}
                }
                // Reborn: corrupt optional dependency headers, weak IDs and native pointees as well as independent enclosing metadata targets.
                if(mode.StartsWith("dependency-",StringComparison.Ordinal)){
                    int dependencyAt=(int)Word(pair.Native.InstanceBuffer,412);var offsets=new HashSet<int>(Enumerable.Range(0,40).Select(i=>dependencyAt+i));
                    foreach(uint slot in Slots(pair.Native.RelocationBuffer).Where(s=>s>=dependencyAt&&s<(uint)Word(pair.Native.InstanceBuffer,416))){uint target=Word(pair.Native.InstanceBuffer,(int)slot);offsets.Add((int)target);}
                    foreach(int at in offsets){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int dependencyTable=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("GameDependency")].Offset;foreach(int offset in new[]{0,4,8,20,32,44,56,68,80,84,92}){var bad=Clone(tokens);bad.InstanceBuffer[dependencyTable+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: shape/contact counts, scalar/pointer data and enclosing metadata must be rejected independently when corrupted.
                if(mode.StartsWith("geometry-",StringComparison.Ordinal)){
                    int geometryAt=(int)Word(pair.Native.InstanceBuffer,500),shapeAt=(int)Word(pair.Native.InstanceBuffer,geometryAt+12);foreach(int at in new[]{geometryAt,geometryAt+4,geometryAt+8,geometryAt+12,geometryAt+16,geometryAt+20,geometryAt+24,shapeAt,shapeAt+16,shapeAt+40,shapeAt+44}){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int geometryTable=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Geometry")].Offset;foreach(int offset in new[]{0,4,8,20,32,44,56,60,68,72,80}){var bad=Clone(tokens);bad.InstanceBuffer[geometryTable+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: corrupt list counts, wrapper bindings and native identities independently after successful source/native proof.
                if(mode.StartsWith("containers-",StringComparison.Ordinal))foreach(string name in ConscriptVariableContainers.Names){int slot=name=="Draws"?472:name=="ClientUpdates"?492:496;var brokenRoot=Clone(pair.Native);brokenRoot.InstanceBuffer[slot]^=4;Refuse(()=>Verify(source,brokenRoot,pair.Instance,origins));faults++;int header=(int)Word(pair.Native.InstanceBuffer,slot);if(header==0)continue;foreach(int offset in new[]{0,4}){var broken=Clone(pair.Native);broken.InstanceBuffer[header+offset]^=1;Refuse(()=>Verify(source,broken,pair.Instance,origins));faults++;}int tableAt=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode(name)].Offset;foreach(int offset in new[]{0,4,8}){var broken=Clone(tokens);broken.InstanceBuffer[tableAt+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,broken));metadataFaults++;}}
                // Reborn: serialize/package only after complete independent ownership proof; later stages are bound to these exact graph bytes.
                var qualification=new ConscriptGraphQualification(pair.Instance,tokens,prefix);
                var streams=GameObjectOriginalStreamSmokeTest.Run(tokens,pair.Instance,manifest,globalManifest,true,qualification);
                if(mode=="new-id-combined")
                {
                    ConscriptRecruitmentSmokeTest.Qualify(schemas,manifest);
                    var command=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes("<!-- Reborn: explicit existing Barracks command override for the variable graph trial. --><LogicCommand xmlns=\"uri:ea.com:eala:asset\" id=\"Command_ConstructSovietAntiInfantryInfantry\" Type=\"UNIT_BUILD\"><Object>RebornConscriptTrial</Object></LogicCommand>"));
                    streams=ConscriptRecruitmentSmokeTest.Compose(streams,pair.Instance,schemas,manifest,globalManifest,command,qualification);
                    File.WriteAllText(Path.Combine(output,"new-id-command.xml"),command.OuterXml);
                }
                GameObjectConscriptTrialSmokeTest.Package(Path.Combine(output,mode+"-package"),source,streams,source.DocumentElement!.GetAttribute("id"),prefix);
                // Reborn: corruption controls target independently owned module identity, container count and auxiliary tables, not just a repeated producer.
                int address=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,ConscriptModuleScalarContract.Owners(source).First(o=>o.Container=="Behaviors"));
                // Reborn: check the stock class and both independently authored flag bytes.
                if (mode is "squish-vehicles" or "squish-allies")
                {
                    var owner = ConscriptModuleScalarContract.Owners(source).Single(module => module.Element.LocalName == "SquishCollide");
                    int offset = ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer, 0, owner);
                    byte vehicle = (byte)(mode == "squish-vehicles" ? 1 : 0);
                    byte ally = (byte)(mode == "squish-allies" ? 1 : 0);
                    if (Word(pair.Native.InstanceBuffer, offset) != 0xB5EB41EDu ||
                        Word(pair.Native.InstanceBuffer, offset + 8) != BitConverter.SingleToUInt32Bits(2.5f) ||
                        pair.Native.InstanceBuffer[offset + 12] != vehicle ||
                        pair.Native.InstanceBuffer[offset + 13] != ally)
                        throw new InvalidDataException("Named EP1 SquishCollide fields differ from the retail layout.");
                }
                // Reborn: verify literal EP1 destinations, import identities, replacement order and default values.
                if (mode is "locomotor-ability" or "replace-self-upgrade")
                {
                    bool locomotor = mode == "locomotor-ability";
                    var owners = ConscriptModuleScalarContract.Owners(source).Where(module =>
                        module.Element.LocalName == (locomotor ? "SwitchLocomotorsSpecialAbilityUpdate" : "ReplaceSelfUpgrade")).ToArray();
                    if (owners.Length != 2)
                        throw new InvalidDataException("Two named EP1 behavior records are required.");
                    var bytes = pair.Native.InstanceBuffer;
                    int authored = ConscriptModuleScalarContract.Address(bytes, 0, owners[0]);
                    int defaults = ConscriptModuleScalarContract.Address(bytes, 0, owners[1]);
                    uint wire = locomotor ? 0x14E031A2u : 0x53FBA9B5u;
                    if (Word(bytes, authored) != wire || Word(bytes, defaults) != wire)
                        throw new InvalidDataException("EP1 behavior class differs.");
                    int changedAt;
                    if (locomotor)
                    {
                        int import = authored + 0x104;
                        if (Word(bytes, authored + 0xfc) != BitConverter.SingleToUInt32Bits(2.5f) ||
                            Word(bytes, authored + 0x100) != BitConverter.SingleToUInt32Bits(47.5f) ||
                            !Slots(pair.Native.ImportsBuffer).Contains((uint)import) ||
                            pair.Instance.ReferencedInstances[(int)Word(bytes, import) - 1].InstanceId != InstanceHandle.GetInstanceId("SpecialPowerAlliedFreeTrade") ||
                            Word(bytes, authored + 0x108) != 2u || Word(bytes, authored + 0x10c) != 0x00010001u ||
                            Enumerable.Range(0, 5).Any(word => Word(bytes, defaults + 0xfc + 4 * word) != 0))
                            throw new InvalidDataException("EP1 locomotor tail, strong import or default differs.");
                        changedAt = authored + 0x108;
                    }
                    else
                    {
                        int list = (int)Word(bytes, authored + 0x30);
                        if (Word(bytes, authored + 0x24) != BitConverter.SingleToUInt32Bits(3.5f) ||
                            Word(bytes, authored + 0x28) != 0x15u || Word(bytes, authored + 0x2c) != 2u ||
                            !Slots(pair.Native.RelocationBuffer).Contains((uint)(authored + 0x30)) ||
                            Word(bytes, list) != InstanceHandle.GetInstanceId("SovietMiner") ||
                            Word(bytes, list + 4) != InstanceHandle.GetInstanceId("SovietAntiInfantryInfantry") ||
                            Word(bytes, defaults + 0x24) != 0 || Word(bytes, defaults + 0x28) != 8u ||
                            Word(bytes, defaults + 0x2c) != 0 || Word(bytes, defaults + 0x30) != 0)
                            throw new InvalidDataException("EP1 replacement options, weak list or default differs.");
                        changedAt = authored + 0x28;
                    }
                    var bad = Clone(pair.Native);
                    bad.InstanceBuffer[changedAt] ^= 1;
                    Refuse(() => Verify(source, bad, pair.Instance, origins));
                    faults++;
                    int metadataList = GameObjectStockReview.Tokens(tokens.InstanceBuffer, 0)[FastHash.GetHashCode("Behaviors")].Offset;
                    int wrapper = (int)Word(tokens.InstanceBuffer, metadataList + 12 + owners[0].Index * 12 + 8);
                    var badMetadata = Clone(tokens);
                    badMetadata.InstanceBuffer[wrapper + 4] ^= 1;
                    Refuse(() => Verify(source, pair.Native, pair.Instance, origins, badMetadata));
                    metadataFaults++;
                }
                // Reborn: check the explicit reader offsets, native array strides and bare-list defaults.
                if (mode == "substructure-build")
                {
                    var owners = ConscriptModuleScalarContract.Owners(source).Where(o => o.Element.LocalName == "SubStructureBuild").ToArray();
                    var bytes = pair.Native.InstanceBuffer;
                    int at = ConscriptModuleScalarContract.Address(bytes, 0, owners[0]);
                    int defaults = ConscriptModuleScalarContract.Address(bytes, 0, owners[1]);
                    int pairs = (int)Word(bytes, at + 0x28);
                    int offsets = (int)Word(bytes, at + 0x30);
                    if (Word(bytes, at) != 0x3176B098u || Word(bytes, at + 0x24) != 2 || Word(bytes, at + 0x2c) != 2 ||
                        Word(bytes, pairs) != InstanceHandle.GetInstanceId("Upgrade_AlliesFaction") ||
                        Word(bytes, pairs + 4) != InstanceHandle.GetInstanceId("AlliedCrane") ||
                        Word(bytes, pairs + 8) != InstanceHandle.GetInstanceId("Upgrade_JapanFaction") ||
                        Word(bytes, pairs + 12) != InstanceHandle.GetInstanceId("SovietMiner") ||
                        Word(bytes, offsets + 64) != BitConverter.SingleToUInt32Bits(-62.211f) ||
                        Word(bytes, offsets + 72 + 68) != BitConverter.SingleToUInt32Bits(3f) ||
                        Enumerable.Range(0, 4).Any(i => Word(bytes, defaults + 0x24 + i * 4) != 0))
                        throw new InvalidDataException("EP1 substructure count, weak pair or vector destinations differ.");
                    var bad = Clone(pair.Native);
                    bad.InstanceBuffer[offsets + 72 + 68] ^= 1;
                    Refuse(() => Verify(source, bad, pair.Instance, origins));
                    faults++;
                }
                // Reborn: check the original concrete class, inherited weapon slot and scheduler's delay source.
                if (mode == "delayed-weapon-fire")
                {
                    var owners = ConscriptModuleScalarContract.Owners(source).Where(o => o.Element.LocalName == "DelayedWeaponFireSpecialAbilityUpdate").ToArray();
                    var bytes = pair.Native.InstanceBuffer;
                    int at = ConscriptModuleScalarContract.Address(bytes, 0, owners[0]);
                    int defaults = ConscriptModuleScalarContract.Address(bytes, 0, owners[1]);
                    int weapon = at + 0xfc;
                    if (Word(bytes, at) != 0x368F2215u || Word(bytes, at + 0x100) != 2u ||
                        Word(bytes, at + 0x11c) != BitConverter.SingleToUInt32Bits(4.25f) || Word(bytes, defaults + 0x11c) != 0 ||
                        !Slots(pair.Native.ImportsBuffer).Contains((uint)weapon) ||
                        pair.Instance.ReferencedInstances[(int)Word(bytes, weapon) - 1].InstanceId != InstanceHandle.GetInstanceId("PlayerPowerIrradiateTargetWeapon"))
                        throw new InvalidDataException("EP1 delayed WeaponFire import or time differs.");
                    var bad = Clone(pair.Native);
                    bad.InstanceBuffer[at + 0x11c] ^= 1;
                    Refuse(() => Verify(source, bad, pair.Instance, origins));
                    faults++;
                }
                // Reborn: prove EP1 attachment string, filter relocation, modifier import and empty defaults independently.
                if (mode == "leech-targeting")
                {
                    var owners = ConscriptModuleScalarContract.Owners(source).Where(o => o.Element.LocalName == "LeechTargetingAttachUpdate").ToArray();
                    var bytes = pair.Native.InstanceBuffer;
                    int at = ConscriptModuleScalarContract.Address(bytes, 0, owners[0]);
                    int defaults = ConscriptModuleScalarContract.Address(bytes, 0, owners[1]);
                    var relocations = Slots(pair.Native.RelocationBuffer);
                    int bone = (int)Word(bytes, at + 352);
                    int modifier = (int)Word(bytes, at + 364);
                    if (Word(bytes, at) != 0xCA6038A6u || Word(bytes, at + 348) != 10u ||
                        System.Text.Encoding.ASCII.GetString(bytes, bone, 10) != "BONE_LEECH" ||
                        !relocations.Contains((uint)(at + 352)) || !relocations.Contains((uint)(at + 356)) ||
                        !relocations.Contains((uint)(at + 364)) || Word(bytes, at + 360) != 1u ||
                        !Slots(pair.Native.ImportsBuffer).Contains((uint)modifier) ||
                        pair.Instance.ReferencedInstances[(int)Word(bytes, modifier) - 1].InstanceId != InstanceHandle.GetInstanceId("AttributeModifier_Garrisoned") ||
                        Word(bytes, defaults + 348) != 0u || Word(bytes, defaults + 356) != 0u || Word(bytes, defaults + 360) != 0u)
                        throw new InvalidDataException("EP1 Leech Attach header, filter, modifier or defaults differ.");
                    var bad = Clone(pair.Native);
                    bad.InstanceBuffer[bone] ^= 1;
                    Refuse(() => Verify(source, bad, pair.Instance, origins));
                    faults++;
                }
                // Reborn: pin the concrete AI selector, shared field destinations and optional chooser relocation.
                if (mode is "transport-ai" or "wander-ai")
                {
                    var owner = ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="AI");
                    var bytes = pair.Native.InstanceBuffer;
                    int at = ConscriptModuleScalarContract.Address(bytes,0,owner);
                    bool wander = mode == "wander-ai";
                    if(Word(bytes,at)!=(wander?0xD813AF2Au:0xBE9CD6FEu) ||
                        Word(bytes,at+56)!=BitConverter.SingleToUInt32Bits(17.5f) ||
                        Word(bytes,at+64)!=(wander?3u:2u) || bytes[at+0x49]!=1 ||
                        !Slots(pair.Native.RelocationBuffer).Contains((uint)(at+68)) || Word(bytes,at+68)==0 ||
                        wander && (Word(bytes,at+0x88)!=125 || Word(bytes,at+0x8c)!=1 ||
                            bytes.AsSpan(at+0x4c,60).IndexOfAnyExcept((byte)0)<0))
                        throw new InvalidDataException("Derived EP1 AI header, mask or chooser differs.");
                    var bad=Clone(pair.Native);
                    bad.InstanceBuffer[at+(wander?0x88:0x49)]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                    var metadata=Clone(tokens);
                    int ai=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("AI")].Offset;
                    int wrapper=(int)Word(tokens.InstanceBuffer,ai+20);
                    metadata.InstanceBuffer[wrapper+16]^=1;
                    Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,metadata));metadataFaults++;
                }
                // Reborn: independently verify the color relocations and their six component destinations.
                if (mode == "emp-update")
                {
                    var owners=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="EMPUpdate").ToArray();
                    var bytes=pair.Native.InstanceBuffer;
                    int both=ConscriptModuleScalarContract.Address(bytes,0,owners[0]);
                    int endOnly=ConscriptModuleScalarContract.Address(bytes,0,owners[1]);
                    int defaults=ConscriptModuleScalarContract.Address(bytes,0,owners[2]);
                    int start=(int)Word(bytes,both+0x20), end=(int)Word(bytes,both+0x24);
                    var relocations=Slots(pair.Native.RelocationBuffer);
                    if(Word(bytes,both)!=0xD3302D7Eu || Word(bytes,both+8)!=BitConverter.SingleToUInt32Bits(2.5f) ||
                        !relocations.Contains((uint)(both+0x20)) || !relocations.Contains((uint)(both+0x24)) ||
                        start==end || Word(bytes,endOnly+0x20)!=0 || Word(bytes,endOnly+0x24)==0 ||
                        Word(bytes,defaults+0x20)!=0 || Word(bytes,defaults+0x24)!=0)
                        throw new InvalidDataException("EP1 EMP color pointer layout differs.");
                    foreach(var (position, expected) in new[]{(start,new[]{0.1f,0.2f,0.3f}),(end,new[]{0.4f,0.5f,0.6f})})
                    for(int i=0;i<3;i++)
                        if(Word(bytes,position+i*4)!=BitConverter.SingleToUInt32Bits(expected[i]))
                            throw new InvalidDataException("EP1 EMP color differs.");
                    var bad=Clone(pair.Native);bad.InstanceBuffer[start+4]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                }
                // Reborn: confirm contained slot ordering, stride, and the nested weapon reference.
                if(mode=="weapon-contained")
                {
                    var owner=ConscriptModuleScalarContract.Owners(source).Single(o=>o.Element.LocalName=="WeaponSetUpdate");
                    var bytes=pair.Native.InstanceBuffer;
                    int at=ConscriptModuleScalarContract.Address(bytes,0,owner);
                    int slots=(int)Word(bytes,at+0x28), weapon=(int)Word(bytes,slots+20), import=weapon+4;
                    if(Word(bytes,at)!=0x431722A9u || Word(bytes,at+12)==0 || Word(bytes,at+0x24)!=2 ||
                        Word(bytes,slots)!=0 || Word(bytes,slots+4)!=31 || Word(bytes,slots+28+4)!=33 ||
                        bytes[slots+28+24]!=1 || bytes[slots+25]!=0 || Word(bytes,slots+16)!=2 ||
                        Word(bytes,weapon)!=0 || Word(bytes,weapon+80)!=1 ||
                        !Slots(pair.Native.ImportsBuffer).Contains((uint)import) ||
                        !Slots(pair.Native.ImportsBuffer).Contains((uint)(import+80)) ||
                        pair.Instance.ReferencedInstances[(int)Word(bytes,import)-1].InstanceId!=InstanceHandle.GetInstanceId("SovietAntiInfantryInfantryAK47"))
                        throw new InvalidDataException("Contained weapon stride or import differs.");
                    var bad=Clone(pair.Native);bad.InstanceBuffer[slots+28+4]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                }
                // Reborn: preserve all signed bits, independently checking the retail consumer's low-byte result.
                if(mode=="weapon-scatter")
                {
                    var actual=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="WeaponScatterEffectUpdate")
                        .Select(o=>Word(pair.Native.InstanceBuffer,ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,o)+0x0c)).ToArray();
                    if(!actual.SequenceEqual(new[]{0xFFFFFFFFu,255u,256u}) ||
                        !actual.Select(v=>(byte)v).SequenceEqual(new byte[]{255,255,0}))
                        throw new InvalidDataException("EP1 signed ScatterAmount differs from the original serializer.");
                }
                // Reborn: pin the command text and import to the original EP1 field coordinates.
                if(mode=="give-restore-upgrade")
                {
                    var owners=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="GiveOrRestoreUpgradeSpecialPower").ToArray();
                    var bytes=pair.Native.InstanceBuffer;
                    int at=ConscriptModuleScalarContract.Address(bytes,0,owners[0]);
                    int defaults=ConscriptModuleScalarContract.Address(bytes,0,owners[1]);
                    int command=(int)Word(bytes,at+0x100), upgrade=(int)Word(bytes,at+0x104);
                    if(Word(bytes,at)!=0x0453E648u || Word(bytes,at+0xfc)!=19 ||
                        System.Text.Encoding.ASCII.GetString(bytes,command,19)!="Command_RebornGrant" ||
                        !Slots(pair.Native.RelocationBuffer).Contains((uint)(at+0x100)) ||
                        !Slots(pair.Native.ImportsBuffer).Contains((uint)(at+0x104)) ||
                        pair.Instance.ReferencedInstances[upgrade-1].InstanceId!=InstanceHandle.GetInstanceId("Upgrade_AlliesFaction") ||
                        Word(bytes,defaults+0xfc)!=0 || Word(bytes,defaults+0x100)!=0 || Word(bytes,defaults+0x104)!=0)
                        throw new InvalidDataException("EP1 give/restore command string or upgrade import differs.");
                    var bad=Clone(pair.Native);bad.InstanceBuffer[command]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                }
                // Reborn: check the concrete class and distinguish local activation identities from global imports.
                if(mode is "activation-ability" or "naval-modifier")
                {
                    bool activation=mode=="activation-ability";
                    var owners=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName==
                        (activation?"ActivateModuleSpecialAbilityUpdate":"AttributeModifierNaval")).ToArray();
                    var bytes=pair.Native.InstanceBuffer;
                    int at=ConscriptModuleScalarContract.Address(bytes,0,owners[0]);
                    int empty=ConscriptModuleScalarContract.Address(bytes,0,owners[1]);
                    int corrupt;
                    if(activation)
                    {
                        int list=(int)Word(bytes,at+0x100);
                        if(Word(bytes,at)!=0xCD33BCEEu || Word(bytes,at+0xfc)!=2 ||
                            !Slots(pair.Native.RelocationBuffer).Contains((uint)(at+0x100)) ||
                            Word(bytes,list)!=FastHash.GetHashCode("rebornfirstrecipient") ||
                            Word(bytes,list+4)!=0 || bytes[list+8]!=0 ||
                            Word(bytes,list+12)!=FastHash.GetHashCode("rebornsecondrecipient") ||
                            Word(bytes,list+16)!=2 || bytes[list+20]!=1 ||
                            Word(bytes,empty+0xfc)!=0 || Word(bytes,empty+0x100)!=0)
                            throw new InvalidDataException("EP1 activation list identity, trigger or defaults differ.");
                        corrupt=list+12;
                    }
                    else
                    {
                        int import=(int)Word(bytes,at+8);
                        if(Word(bytes,at)!=0xA34CED5Cu || Word(bytes,empty+8)!=0 ||
                            !Slots(pair.Native.ImportsBuffer).Contains((uint)(at+8)) ||
                            pair.Instance.ReferencedInstances[import-1].InstanceId!=InstanceHandle.GetInstanceId("AttributeModifier_Garrisoned"))
                            throw new InvalidDataException("EP1 naval modifier reference differs.");
                        corrupt=at+8;
                    }
                    var bad=Clone(pair.Native);bad.InstanceBuffer[corrupt]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                }
                // Reborn: compare the complete EP1 headers and typed reference targets against distinct authored values.
                if(mode is "special-power-update" or "damage-field" or "horde-ai" or "supply-truck-ai")
                {
                    var owners=ConscriptModuleScalarContract.Owners(source);
                    var bytes=pair.Native.InstanceBuffer;
                    var relos=Slots(pair.Native.RelocationBuffer);
                    var imports=Slots(pair.Native.ImportsBuffer);
                    int corrupt;
                    if(mode is "special-power-update" or "damage-field")
                    {
                        bool power=mode=="special-power-update";
                        var modules=owners.Where(o=>o.Element.LocalName==(power?"SpecialPowerUpdate":"DamageFieldUpdate")).ToArray();
                        int empty=ConscriptModuleScalarContract.Address(bytes,0,modules[0]);
                        int at=ConscriptModuleScalarContract.Address(bytes,0,modules[1]);
                        if(power)
                        {
                            int sound=(int)Word(bytes,at+0x10);
                            if(Word(bytes,at)!=0xB9B2A9B3 || Word(bytes,at+0x0c)!=0x18 || bytes[at+0x14]!=1 ||
                                Word(bytes,empty+0x0c)!=8 || Word(bytes,empty+0x10)!=0 || bytes[empty+0x14]!=0 ||
                                sound==0 || !relos.Contains((uint)(at+0x10)) || !relos.Contains((uint)sound) ||
                                !NamesImport(at+8,"SpecialPowerChronoRiftTeleportLvl1") ||
                                !NamesImport((int)Word(bytes,sound),"SOV_Conscript_VoiceSelectMS") ||
                                !NamesImport(sound+4,"BuildingRepaired"))
                                throw new InvalidDataException("EP1 power mask, paused flag or InitiateSound imports differ.");
                            corrupt=at+0x0c;
                        }
                        else
                        {
                            int nugget=(int)Word(bytes,at+0x14);
                            if(Word(bytes,at)!=0xF3B802D5 || Word(bytes,at+0x08)!=BitConverter.SingleToUInt32Bits(1.5f) ||
                                Word(bytes,at+0x10)!=1 || bytes[at+0x1c]!=1 || Word(bytes,at+0x20)!=173 ||
                                Word(bytes,empty+0x10)!=0 || Word(bytes,empty+0x28)!=0 ||
                                !relos.Contains((uint)(at+0x14)) || !relos.Contains((uint)(at+0x18)) ||
                                !relos.Contains((uint)(at+0x28)) || !NamesImport(at+0x24,"Upgrade_SovietFaction") ||
                                !NamesImport(nugget,"SovietHeavyAntiVehicleInfantryShortCircuit"))
                                throw new InvalidDataException("EP1 damage field interval, nugget, status, upgrade or filter differs.");
                            corrupt=at+0x20;
                        }
                    }
                    else
                    {
                        bool truck=mode=="supply-truck-ai";
                        int at=ConscriptModuleScalarContract.Address(bytes,0,owners.Single(o=>o.Container=="AI"));
                        int chooser=(int)Word(bytes,at+0x44);
                        if(Word(bytes,at)!=(truck?0x9AB8094Du:0x55D77703u) || Word(bytes,at+0x3c)!=13 ||
                            Word(bytes,at+0x40)!=2 || bytes[at+0x49]!=1 || chooser==0 ||
                            !relos.Contains((uint)(at+0x44)) || !NamesImport(chooser+0x24,"DefaultTargetingCompareList") ||
                            truck && (Word(bytes,at+0x4c)!=17 ||
                                Word(bytes,at+0x50)!=BitConverter.SingleToUInt32Bits(1.25f) ||
                                Word(bytes,at+0x54)!=BitConverter.SingleToUInt32Bits(2.5f) ||
                                Word(bytes,at+0x5c)!=BitConverter.SingleToUInt32Bits(2000f) ||
                                !bytes.AsSpan(at+0x6c,5).SequenceEqual(new byte[]{1,0,1,0,1})))
                            throw new InvalidDataException("EP1 Horde/truck inherited data, chooser or truck tail differs.");
                        corrupt=at+(truck?0x6f:0x49);
                    }
                    var bad=Clone(pair.Native);
                    bad.InstanceBuffer[corrupt]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;

                    // Reborn: inspect the occurrence table as well as the native import word.
                    bool NamesImport(int slot,string id)=>imports.Contains((uint)slot) &&
                        Word(bytes,slot)>0 && pair.Instance.ReferencedInstances[(int)Word(bytes,slot)-1].InstanceId==InstanceHandle.GetInstanceId(id);
                }
                // Reborn: check the Body pointer, shield-specific native offsets, and optional import.
                if(mode is "inactive-body" or "shield-body" or "shield-body-default")
                {
                    var owner=ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="Body");
                    var bytes=pair.Native.InstanceBuffer;
                    int at=ConscriptModuleScalarContract.Address(bytes,0,owner);
                    bool inactive=mode=="inactive-body",shield=mode=="shield-body";
                    if(!Slots(pair.Native.RelocationBuffer).Contains(488u) ||
                        Word(bytes,at)!=(inactive?0x29234E6Eu:0xD3565857u))
                        throw new InvalidDataException("Concrete EP1 Body identity differs.");
                    if(!inactive)
                    {
                        int armor=(int)Word(bytes,at+0x60);
                        if(Word(bytes,at+8)!=BitConverter.SingleToUInt32Bits(750f) ||
                            Word(bytes,at+0x5c)!=(shield?BitConverter.SingleToUInt32Bits(225.5f):0u) ||
                            Word(bytes,at+0x64)!=(shield?BitConverter.SingleToUInt32Bits(2.75f):0u) ||
                            Word(bytes,at+0x68)!=(shield?0x120u:uint.MaxValue) ||
                            shield&&(armor==0||!Slots(pair.Native.RelocationBuffer).Contains((uint)(at+0x60))||
                                !Slots(pair.Native.ImportsBuffer).Contains((uint)(at+0x4c))||
                                pair.Instance.ReferencedInstances[(int)Word(bytes,at+0x4c)-1].InstanceId!=InstanceHandle.GetInstanceId("FX_SOV_LargeBearBloodImpact")||
                                !Slots(pair.Native.ImportsBuffer).Contains((uint)armor)||
                                pair.Instance.ReferencedInstances[(int)Word(bytes,armor)-1].InstanceId!=InstanceHandle.GetInstanceId("SovietAntiInfantryInfantryArmor")) ||
                            !shield&&armor!=0)
                            throw new InvalidDataException("EP1 ShieldBody field or armor import differs.");
                    }
                    var bad=Clone(pair.Native);bad.InstanceBuffer[at+(inactive?4:0x5c)]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                }
                // Reborn: pin the original derived tail coordinates and their independent linked storage.
                if(mode is "create-enter" or "status-ability")
                {
                    bool create=mode=="create-enter";
                    var owners=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName==
                        (create?"CreateAndEnterObjectSpecialPower":"ObjectStatusSpecialAbilityUpdate")).ToArray();
                    var bytes=pair.Native.InstanceBuffer;
                    int at=ConscriptModuleScalarContract.Address(bytes,0,owners[0]);
                    int empty=ConscriptModuleScalarContract.Address(bytes,0,owners[1]);
                    int corrupt;
                    if(create)
                    {
                        int vector=(int)Word(bytes,at+0x20c);
                        int upgradeCount=System.Runtime.InteropServices.Marshal.OffsetOf<Ep1OCLSpecialPowerModuleData>(nameof(Ep1OCLSpecialPowerModuleData.UpgradeOCL)).ToInt32();
                        int import=(int)Word(bytes,at+upgradeCount+4);
                        if(Word(bytes,at)!=0x38C496D8u || Word(bytes,empty+0x20c)!=0 ||
                            !Slots(pair.Native.RelocationBuffer).Contains((uint)(at+0x20c)) ||
                            Word(bytes,vector)!=BitConverter.SingleToUInt32Bits(1.25f) ||
                            Word(bytes,vector+4)!=BitConverter.SingleToUInt32Bits(-2.5f) ||
                            Word(bytes,vector+8)!=BitConverter.SingleToUInt32Bits(3.75f) ||
                            Word(bytes,at+upgradeCount)!=1 || !Slots(pair.Native.ImportsBuffer).Contains((uint)import) ||
                            pair.Instance.ReferencedInstances[(int)Word(bytes,import)-1].InstanceId!=InstanceHandle.GetInstanceId("OCL_Generic5SecondBeacon"))
                            throw new InvalidDataException("EP1 CreateAndEnter FX or upgrade OCL differs.");
                        corrupt=vector+4;
                    }
                    else
                    {
                        var first=bytes.AsSpan(at+0xfc,32);
                        var second=bytes.AsSpan(at+0x11c,32);
                        if(Word(bytes,at)!=0x079B6EADu || first.SequenceEqual(second) ||
                            first.IndexOfAnyExcept((byte)0)<0 || second.IndexOfAnyExcept((byte)0)<0 ||
                            bytes.AsSpan(empty+0xfc,64).IndexOfAnyExcept((byte)0)>=0)
                            throw new InvalidDataException("EP1 ability status masks differ.");
                        corrupt=at+0x11c;
                    }
                    var bad=Clone(pair.Native);bad.InstanceBuffer[corrupt]^=1;
                    Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                }
                // Reborn: pin the retail native coordinates and reject a deliberate corruption through the same package verifier.
                if(mode is "bezier-spline" or "sphere-update" or "steal-unit" or "booby-trap")
                {
                    uint wire=mode switch
                    {
                        "bezier-spline"=>0xB38F4984u,
                        "sphere-update"=>0x9D74F07Au,
                        "steal-unit"=>0x21068FF6u,
                        _=>0xE502295Eu
                    };
                    var bytes=pair.Native.InstanceBuffer;
                    var owners=ConscriptModuleScalarContract.Owners(source);
                    string id=mode switch
                    {
                        "bezier-spline"=>"RebornBezier",
                        "sphere-update"=>"RebornSphere",
                        "steal-unit"=>"RebornSteal",
                        _=>"RebornTrap"
                    };
                    int at=ConscriptModuleScalarContract.Address(bytes,0,owners.Single(o=>o.Element.GetAttribute("id")==id));
                    int nativeAt=mode=="bezier-spline"?(int)Word(bytes,at+0x18):at;
                    var relos=Slots(pair.Native.RelocationBuffer);
                    var imports=Slots(pair.Native.ImportsBuffer);
                    if(Word(bytes,nativeAt)!=wire)
                        throw new InvalidDataException("Named EP1 concrete class differs.");
                    if(mode=="bezier-spline")
                    {
                        int list=(int)Word(bytes,nativeAt+0x18);
                        int empty=ConscriptModuleScalarContract.Address(bytes,0,owners.Single(o=>o.Element.GetAttribute("id")=="RebornBezierDefault"));
                        empty=(int)Word(bytes,empty+0x18);
                        if(Word(bytes,at+8)!=3 || Word(bytes,nativeAt+0x10)!=BitConverter.SingleToUInt32Bits(3.75f) ||
                            Word(bytes,nativeAt+0x14)!=2 || !relos.Contains((uint)(nativeAt+0x18)) ||
                            !imports.Contains((uint)list) || !imports.Contains((uint)(list+4)) ||
                            Word(bytes,list)==Word(bytes,list+4) || Word(bytes,empty+0x0c)!=0x3f800000 ||
                            Word(bytes,empty+0x10)!=0 || Word(bytes,empty+0x14)!=1)
                            throw new InvalidDataException("EP1 Bezier adjustment, defaults or path list differs.");
                    }
                    else if(mode=="sphere-update")
                    {
                        if(Word(bytes,at+0x1c)!=BitConverter.SingleToUInt32Bits(8.25f) ||
                            !relos.Contains((uint)(at+0x24)) || !relos.Contains((uint)(at+0x2c)) ||
                            !relos.Contains((uint)(at+0xa8)) || bytes[at+0xac]!=1 || bytes[at+0xad]!=0)
                            throw new InvalidDataException("Direct Sphere header or relocations differ.");
                    }
                    else
                    {
                        if(!relos.Contains((uint)(at+0x160)) || Word(bytes,at+0x15c)!=8 ||
                            mode=="booby-trap" && (!imports.Contains((uint)(at+0x1b0)) ||
                                !relos.Contains((uint)(at+0x1b4)) || bytes[at+0x1b8]!=1))
                            throw new InvalidDataException("Attachment EP1 bone or derived trap tail differs.");
                    }
                    var corrupt=Clone(pair.Native);
                    corrupt.InstanceBuffer[nativeAt]^=1;
                    Refuse(()=>Verify(source,corrupt,pair.Instance,origins));faults++;
                }
                // Reborn: confirm concrete identity, exact vector destinations and EP1 enum/default encoding.
                if (mode == "destructible-environment")
                {
                    var owners = ConscriptModuleScalarContract.Owners(source)
                        .Where(module => module.Element.LocalName == "DestructibleEnvironmentUpdate").ToArray();
                    if (owners.Length != 2)
                        throw new InvalidDataException("Two authored environment records are required.");

                    for (int index = 0; index < owners.Length; index++)
                    {
                        int offset = ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer, 0, owners[index]);
                        var bytes = pair.Native.InstanceBuffer;
                        if (Word(bytes, offset) != 0x75FE8D90u ||
                            Word(bytes, offset + 8) != (index == 0 ? 2u : 0u) ||
                            bytes[offset + 0x24] != (index == 0 ? 1 : 0))
                            throw new InvalidDataException("Environment class or native enum/default differs.");
                        for (int component = 0; component < 6; component++)
                            if (Word(bytes, offset + 0x0c + component * 4) != BitConverter.SingleToUInt32Bits(component + 1))
                                throw new InvalidDataException("Environment inline offsets differ.");
                    }
                }
                // Reborn: sample all death header words plus optional masks/list references and literal inheritance/field targets.
                if(mode.StartsWith("death-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>DeathBehaviorContract.Supports(o.NativeType)&&o.Element.GetAttribute("id").StartsWith("RebornDeath",StringComparison.Ordinal))){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner),header=owner.Element.LocalName=="CreateObjectDie"?72:52;var offsets=Enumerable.Range(0,header/4).Select(i=>i*4).ToList();
                    foreach(int slot in header==72?new[]{12,16,20,60,68}:new[]{12,16,20}){uint target=Word(pair.Native.InstanceBuffer,at+slot);if(target!=0)offsets.Add((int)target-at);}
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{8,20,32,44,56,68,80,92,104,116,128,140,152,164,176,188,200,212,224,236}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: sample tracker pointer/count/scalar storage, both ends of effect lists and representative independent metadata targets.
                if(mode.StartsWith("tracker-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="SecondaryDamageTracker")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,36).ToList();foreach(int slot in new[]{20,28,32}){uint pointer=Word(pair.Native.InstanceBuffer,at+slot);if(pointer!=0)offsets.Add((int)pointer-at);}
                    int trackerCount=(int)Word(pair.Native.InstanceBuffer,at+24);if(trackerCount>0){int array=(int)Word(pair.Native.InstanceBuffer,at+28)-at;foreach(int row in new[]{0,trackerCount-1}.Distinct())foreach(int field in new[]{0,4,8,12,13,14,15})offsets.Add(array+row*16+field);}
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{8,20,32,44,56,68,80,92,104,116,140,164,188,212,224}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: corrupt each unpack field, optional pointer/import index and base metadata target after a complete package build.
                if(mode.StartsWith("unpack-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="StructureUnpackUpdate")){
                    // Reborn: accepting an intentional empty sound must not allow a nonexistent named sound to pass.
                    var missing=Copy(source);var missingOwner=ConscriptModuleScalarContract.Owners(missing).Single(o=>o.Element.GetAttribute("id")==owner.Element.GetAttribute("id")).Element;missingOwner.SetAttribute("UnpackCompleteSound","RebornMissingUnpackSound");Refuse(()=>Verify(missing,pair.Native,pair.Instance,origins));faults++;
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);
                    foreach(int offset in owner.Element.HasAttribute("UnpackCompleteSound")?new[]{0,4,8,12,16,20}:new[]{0,4,8,12,16}){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{8,16,20,40,56,64,68,80,104,128,152,176,188}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every header byte of the first schema-proved draw and representative words of its spliced metadata.
                if(mode.StartsWith("draws-",StringComparison.Ordinal)){
                    var proved=ConscriptModuleScalarContract.Owners(source).Where(o=>ConscriptVariableContainers.Supports(o.Container)&&origins[o.Element.GetAttribute("id")]==SchemaNativeContract.Origin).ToArray();if(proved.Length==0)throw new InvalidDataException("No schema-proved leaf was exercised.");
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,proved[0]);
                    for(int offset=0;offset<SchemaNativeContract.Size(proved[0].NativeType);offset++){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode(proved[0].Container)].Offset;int leafTable=(int)Word(tokens.InstanceBuffer,list+12+proved[0].Index*12+8);
                    for(int row=0;row<48;row++){var bad=Clone(tokens);bad.InstanceBuffer[leafTable+row*12+(row%3)*4]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                    // Reborn: material tests corrupt the actual nested allocation and its metadata, not only the enclosing draw header.
                    if(mode.StartsWith("draws-material",StringComparison.Ordinal)){
                        var owner=proved.Single(o=>o.Element.GetAttribute("id")=="RebornEditedDraw");int draw=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);int states=(int)Word(pair.Native.InstanceBuffer,draw+168),material=(int)Word(pair.Native.InstanceBuffer,states+112);var points=Enumerable.Range(material,28).ToList();int shaderCount=(int)Word(pair.Native.InstanceBuffer,material+16),items=(int)Word(pair.Native.InstanceBuffer,material+20);
                        for(int i=0;i<shaderCount;i++){points.Add(items+i*4);int constant=(int)Word(pair.Native.InstanceBuffer,items+i*4);points.AddRange(Enumerable.Range(constant,16));if(Word(pair.Native.InstanceBuffer,constant)==0xDF08DF25){points.Add(constant+16);int values=(int)Word(pair.Native.InstanceBuffer,constant+16);for(int j=0;j<Word(pair.Native.InstanceBuffer,constant+12);j++)points.Add(values+j*4);}}
                        foreach(int point in points.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[point]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                        int drawWrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8),drawTable=(int)Word(tokens.InstanceBuffer,drawWrapper+8),stateList=GameObjectStockReview.Tokens(tokens.InstanceBuffer,drawTable)[FastHash.GetHashCode("ModelConditionState")].Offset,stateTable=(int)Word(tokens.InstanceBuffer,stateList+20),materialTable=GameObjectStockReview.Tokens(tokens.InstanceBuffer,stateTable)[FastHash.GetHashCode("Material")].Offset;
                        for(int row=0;row<5;row++)foreach(int word in new[]{0,4,8}){var bad=Clone(tokens);bad.InstanceBuffer[materialTable+row*12+word]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                    }
                }
                // Reborn: mutate every PowerUpgrade header/padding byte, owned animation and trigger words, and the class/base metadata rows.
                if(mode.StartsWith("power-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="PowerUpgrade")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,PowerUpgradeContract.HeaderSize).ToList();
                    uint animation=Word(pair.Native.InstanceBuffer,at+8);if(animation!=0)offsets.AddRange(Enumerable.Range((int)animation-at,12));
                    foreach(int slot in new[]{12,20}){uint triggerCount=Word(pair.Native.InstanceBuffer,at+slot),triggerItems=Word(pair.Native.InstanceBuffer,at+slot+4);if(triggerCount>0)offsets.AddRange(Enumerable.Range((int)triggerItems-at,(int)triggerCount*4));}
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);int own=(int)Word(tokens.InstanceBuffer,wrapper+8),inherited=(int)Word(tokens.InstanceBuffer,own+8);
                    foreach(int offset in new[]{wrapper+4,wrapper+8,own+4,own+8,inherited,inherited+4,inherited+8}){var bad=Clone(tokens);bad.InstanceBuffer[offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every LUAEventList header byte, every owned name byte (including NUL) and every metadata row word.
                // Reborn: mutate representative FX header, optional heap, first/last event, imported selector and literal metadata table fields.
                if(mode.StartsWith("fx-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="FXListBehavior"&&o.Element.GetAttribute("id").StartsWith("RebornFx",StringComparison.Ordinal))){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=new List<int>{0,4,8,12,16,20,24,28,32,36,44,52,56};
                    foreach(int slot in new[]{12,16,20}){uint pointer=Word(pair.Native.InstanceBuffer,at+slot);if(pointer!=0)offsets.Add((int)pointer-at);}
                    int array=(int)Word(pair.Native.InstanceBuffer,at+56)-at,fxEventCount=(int)Word(pair.Native.InstanceBuffer,at+52);foreach(int row in new[]{0,fxEventCount-1}.Distinct())foreach(int field in new[]{0,4,8,12,16,20,21,22})offsets.Add(array+row*24+field);
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{8,20,32,56,68,92,116,140,152,164,176,200,212,224,248}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                if(mode.StartsWith("lua-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="LUAEventList")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,LUAEventListContract.HeaderSize).ToList();
                    int text=(int)Word(pair.Native.InstanceBuffer,at+12)-at;offsets.AddRange(Enumerable.Range(text,owner.Element.GetAttribute("EventListName").Length+1));
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);
                    foreach(int offset in new[]{4,8,16,20,24,28,32,52,56,76,80,88,92}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every structure placement header byte, owned string/Offset words and representative metadata rows.
                if(mode.StartsWith("structure-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="StructurePlacementBehavior")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,StructurePlacementContract.HeaderSize).ToList();
                    foreach(int slot in new[]{20,36}){uint target=Word(pair.Native.InstanceBuffer,at+slot);if(target!=0)offsets.Add((int)target-at);}
                    uint vector=Word(pair.Native.InstanceBuffer,at+48);if(vector!=0)foreach(int o in new[]{0,4,8})offsets.Add((int)vector-at+o);
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);
                    foreach(int offset in new[]{4,8,20,28,32,56,80,92,112,116,128,140,152,176,188,212,224,236}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every infiltrator byte, the optional filter pointee and representative metadata rows.
                if(mode.StartsWith("infiltrator-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="InfiltratorContain")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,InfiltratorContainContract.HeaderSize).ToList();
                    foreach(int slot in new[]{92,96}){uint target=Word(pair.Native.InstanceBuffer,at+slot);if(target!=0)offsets.Add((int)target-at);}
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);
                    foreach(int offset in new[]{4,8,20,28,32,44,56,80,104,140,176,188,212,224,236,256,280,308,320}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every placement header byte, filter value/list word and representative metadata rows.
                if(mode.StartsWith("placement-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="AssistedPlacementBehavior")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,AssistedPlacementContract.HeaderSize).ToList();
                    // Reborn: assert retail hint words independently of the shared enum/flag codec.
                    uint expectedHint = owner.Element.GetAttribute("PlacementHint") switch
                    {
                        "" or "NONE" => 0u,
                        "FACING_TOWARDS ALLOW_SHROUDED" => 0xAu,
                        "FACING_AWAY ALLOW_SHROUDED" => 0xCu,
                        _ => throw new InvalidDataException("Unexpected placement fixture hint.")
                    };
                    if (Word(pair.Native.InstanceBuffer, at + 8) != expectedHint)
                        throw new InvalidDataException("EP1 placement hint does not match the retail mask.");
                    uint filterAt=Word(pair.Native.InstanceBuffer,at+16);if(filterAt!=0)foreach(int o in new[]{0,4,8,12,16,56,96,100,104,112}){offsets.Add((int)filterAt-at+o);}
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);
                    foreach(int offset in new[]{4,8,20,28,32,44,56,80,104,116}.Concat(filterAt!=0?new[]{140,152,188,212,236,248,272,284}:Array.Empty<int>())){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every engineer header byte, every owned pointee word and representative metadata rows.
                if(mode.StartsWith("engineer-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="EngineerContain")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,EngineerContainContract.HeaderSize).ToList();
                    foreach(int slot in new[]{12,16,52,56,60,64,84}){uint target=Word(pair.Native.InstanceBuffer,at+slot);if(target!=0)offsets.Add((int)target-at);}
                    uint filterAt=Word(pair.Native.InstanceBuffer,at+84);if(filterAt!=0)foreach(int o in new[]{4,8,12,16,56,104,112}){offsets.Add((int)filterAt-at+o);}
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);
                    foreach(int offset in new[]{4,8,20,28,32,64,92,104,140,188,236,256,268}.Concat(filterAt!=0?new[]{296,308,320,392,404,416,428}:Array.Empty<int>())){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every repair byte, including padding, and representative own/base/inline metadata rows.
                // Reborn: the two source sound identities must occupy their own live import slots in the package.
                if (mode == "repair-audio")
                {
                    var owner = ConscriptModuleScalarContract.Owners(source).Single(o => o.Element.GetAttribute("id") == "RebornRepair0");
                    var bytes = pair.Native.InstanceBuffer;
                    int at = ConscriptModuleScalarContract.Address(bytes, 0, owner);
                    var imports = Slots(pair.Native.ImportsBuffer);
                    foreach (var (offset, sound) in new[] { (32, "SOV_ConstructionYard_Select"), (36, "SOV_ConstructionYard_Die") })
                    {
                        int selector = (int)Word(bytes, at + offset);
                        if (!imports.Contains((uint)(at + offset)) || selector == 0 ||
                            pair.Instance.ReferencedInstances[selector - 1].InstanceId != InstanceHandle.GetInstanceId(sound))
                            throw new InvalidDataException("EP1 repair sound import differs: " + sound);
                    }
                    // Reborn: a populated but unresolved sound must still fail the independent reference contract.
                    var missing = Copy(source);
                    var missingOwner = ConscriptModuleScalarContract.Owners(missing).Single(o => o.Element.GetAttribute("id") == "RebornRepair0").Element;
                    ((XmlElement)missingOwner.LastChild!).SetAttribute("SelfRepairFromDamageLoop", "RebornMissingRepairCue");
                    Refuse(() => Verify(missing, pair.Native, pair.Instance, origins));
                    faults++;
                }
                if(mode.StartsWith("repair-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="RepairUpdate")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);for(int offset=0;offset<RepairUpdateContract.NativeSize;offset++){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{4,8,20,28,32,56,64,76,80,92,116,188,200,224,236,260,284,296}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: corrupt the entire production header, optional list storage and every actual translation import independently.
                if(mode.StartsWith("production-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="ProductionUpdate")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);for(int offset=0;offset<ProductionUpdateContract.NativeSize;offset++){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    // Reborn: use the installed SovietMiner ID and the EA witness's inline weak-ID encoding.
                    uint bonusSlot = checked((uint)(at + 0x2c));
                    uint bonus = mode == "production-edited" ? 0xF6DAC2A4u : 0u;
                    if (Word(pair.Native.InstanceBuffer, (int)bonusSlot) != bonus ||
                        Slots(pair.Native.ImportsBuffer).Contains(bonusSlot) || Slots(pair.Native.RelocationBuffer).Contains(bonusSlot))
                        throw new InvalidDataException("EP1 ProductionUpdate bonus ID or auxiliary slots differ from the EA witness.");
                    // Reborn: check the independent EP1 row stride, filter words, nullable defaults and exact IMP slots.
                    if (mode == "production-modifier")
                    {
                        var bytes = pair.Native.InstanceBuffer;
                        int rows = checked((int)Word(bytes, at + 0x48));
                        uint second = checked((uint)(rows + 0x84));
                        var imports = Slots(pair.Native.ImportsBuffer);
                        var relocations = Slots(pair.Native.RelocationBuffer);
                        if (Word(bytes, at + 0x44) != 2 || !relocations.Contains((uint)(at + 0x48)) ||
                            !imports.Contains((uint)rows) || !imports.Contains(second) ||
                            Word(bytes, rows + 4) != BitConverter.SingleToUInt32Bits(0.75f) ||
                            Word(bytes, rows + 8) != BitConverter.SingleToUInt32Bits(1.25f) ||
                            Word(bytes, rows + 0x10) != 2 ||
                            Word(bytes, (int)second + 4) != BitConverter.SingleToUInt32Bits(1f) ||
                            Word(bytes, (int)second + 8) != BitConverter.SingleToUInt32Bits(1f) ||
                            Word(bytes, (int)second + 0x10) != 3)
                            throw new InvalidDataException("ProductionModifier external imports, native row stride or filter defaults differ.");
                        foreach (int slot in new[] { rows, rows + 4, rows + 8, rows + 0x10, rows + 0x6c, rows + 0x78, (int)second })
                        {
                            var bad = Clone(pair.Native);
                            bad.InstanceBuffer[slot] ^= 1;
                            Refuse(() => Verify(source, bad, pair.Instance, origins));
                            faults++;
                        }
                    }
                    int give=(int)Word(pair.Native.InstanceBuffer,at+76);if(give!=0){int translationCount=(int)Word(pair.Native.InstanceBuffer,give),items=(int)Word(pair.Native.InstanceBuffer,give+4);foreach(int slot in new[]{give,give+4}.Concat(Enumerable.Range(0,translationCount*2).Select(i=>items+i*4))){var bad=Clone(pair.Native);BinaryPrimitives.WriteUInt32LittleEndian(bad.InstanceBuffer.AsSpan(slot),uint.MaxValue);Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{4,8,20,28,32,112,116,128,188,224,236,248,260,280,356,392,404}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: corrupt every Spawn header word/packed flag and every optional-record/list payload, not only its type identity.
                if(mode.StartsWith("spawn-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="SpawnBehavior")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=Enumerable.Range(0,20).Select(i=>i*4).Concat(Enumerable.Range(80,16)).ToList();
                    foreach(int slot in new[]{8,16,24,64,72,76}){uint target=Word(pair.Native.InstanceBuffer,at+slot);if(target!=0)offsets.Add((int)target-at);}
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{8,28,40,64,100,220,244,280,292,304,316}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: mutate every byte of this wholly inline module, including all packed flags, not merely its first ID word.
                if(mode.StartsWith("exit-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="QueueProductionExitUpdate")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);for(int offset=0;offset<48;offset++){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{8,28,40,44,112,116,128,148,220,236}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: all docking headers, string bytes, enum values and literal metadata remain source-bound after serialization.
                if(mode.StartsWith("dock-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="QueueDockBehavior")){
                    // Reborn: null pointers are genuinely invalid even for empty strings; shifting a zero terminator into equivalent padding is not a semantic fault.
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);var offsets=new List<int>{0,4,48};for(int i=0;i<QueueDockContract.Names.Length;i++){offsets.Add(8+i*8);offsets.Add(12+i*8);uint text=Word(pair.Native.InstanceBuffer,at+12+i*8);if(text!=0)offsets.Add((int)text-at);}foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);if(offset>=12&&offset<=44&&(offset-12)%8==0)Put(bad.InstanceBuffer,at+offset,0);else bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{8,28,32,88,92,112,160,176}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: independently refuse changed headers, DieMux values, source item data, FX selectors and final metadata rows.
                if(mode.StartsWith("destruction-",StringComparison.Ordinal)){
                    var owner=ConscriptModuleScalarContract.Owners(source).Single(o=>o.Element.LocalName=="BuildingDestructionBehavior");int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);int destructionCount=BuildingDestructionContract.Items(owner.Element).Length;var offsets=new List<int>{0,4,8,24,28,36,44,52,56,60,64,68,72+8,72+12,72+16,72+(destructionCount-1)*20+16};uint bone=Word(pair.Native.InstanceBuffer,at+76);if(bone!=0)offsets.Add((int)bone-at);
                    foreach(int offset in offsets.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at+offset]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);foreach(int offset in new[]{4,8,300,304,308,312+destructionCount*12+4}){var bad=Clone(tokens);bad.InstanceBuffer[wrapper+offset]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: source/type/id/time/condition and its composed metadata are independently checked after packaging.
                if(mode.StartsWith("sell-",StringComparison.Ordinal))foreach(var owner in ConscriptModuleScalarContract.Owners(source).Where(o=>o.Element.LocalName=="StructureSellUpdate")){
                    int at=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);foreach(int field in new[]{0,4,8,12}){var bad=Clone(pair.Native);bad.InstanceBuffer[at+field]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int list=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;int wrapper=(int)Word(tokens.InstanceBuffer,list+12+owner.Index*12+8);var broken=Clone(tokens);broken.InstanceBuffer[wrapper+8]^=4;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,broken));metadataFaults++;
                }
                // Reborn: pointer/presence/count, first/last row values and exact metadata grammar are independently rejected when corrupted.
                if(mode.StartsWith("resource-",StringComparison.Ordinal))
                {
                    int start=(int)Word(pair.Native.InstanceBuffer,416);var points=new List<int>{416};if(start!=0){points.Add(start);points.Add(start+4);if(GameObjectResourceContract.Entries(source).Length>0){int items=(int)Word(pair.Native.InstanceBuffer,start+4);points.Add(items);points.Add(items+4);points.Add(items+(GameObjectResourceContract.Entries(source).Length-1)*8+4);}}
                    foreach(int at in points.Distinct()){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int resourceTable=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("ObjectResourceInfo")].Offset;
                    if(resourceTable!=0)foreach(int at in new[]{resourceTable,resourceTable+4,resourceTable+8,resourceTable+24,resourceTable+28,resourceTable+32}){var bad=Clone(tokens);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                    var receipt=JsonDocument.Parse(File.ReadAllBytes(Path.Combine(output,mode+"-package","trial-plan.json")));if(receipt.RootElement.GetProperty("ResourceInfoPresent").GetBoolean()!=(mode!="resource-omitted")||receipt.RootElement.GetProperty("BuildCosts").GetArrayLength()!=GameObjectResourceContract.Entries(source).Length)throw new InvalidDataException("Resource receipt lost authored presence/cardinality.");
                    if(mode=="resource-uint32"&&receipt.RootElement.GetProperty("BuildCosts")[0].GetProperty("Amount").GetUInt32()!=uint.MaxValue)throw new InvalidDataException("Resource receipt rounded uint32 Amount through float32.");
                }
                // Reborn: counts, source values, import selectors and metadata row types/targets remain checked on every movement cardinality.
                if(mode.StartsWith("movement-",StringComparison.Ordinal))
                {
                    var points=new List<int>{428,432};int start=(int)Word(pair.Native.InstanceBuffer,432);if(start!=0)points.AddRange(new[]{start,start+4,start+8});
                    foreach(int at in points){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int movementTable=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("LocomotorSet")].Offset;
                    foreach(int at in new[]{movementTable,movementTable+4,movementTable+8}){var bad=Clone(tokens);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                    // Reborn: a late-row corruption proves the verifier consumes the entire expanded list rather than its first sample row.
                    if(mode=="movement-expanded"){var bad=Clone(pair.Native);bad.InstanceBuffer[start+39*12+4]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                }
                // Reborn: independently corrupt each newly editable nested field; valid-range output changes must still be refused.
                if(mode.StartsWith("nested-",StringComparison.Ordinal))foreach(int at in GameObjectNestedValueContract.Expected(source,pair.Native.InstanceBuffer).Keys){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                // Reborn: permitting value edits never means accepting changed output bytes that disagree with the source, including optional root-import selectors.
                if(mode.StartsWith("shared-",StringComparison.Ordinal))foreach(int at in ConscriptEditableScalars.Offsets(pair.Native.InstanceBuffer).Concat(GameObjectStrongReferenceContract.Fields.Select(f=>f.Offset))){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                foreach(int at in new[]{address,address+4,(int)Word(pair.Native.InstanceBuffer,476)}){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                foreach(bool relocations in new[]{true,false}){var bad=Clone(pair.Native);if(relocations)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                // Reborn: valid-range content changes and count/metadata corruption must still disagree with their actual authored combat data.
                if(mode.StartsWith("combat-",StringComparison.Ordinal)){
                    foreach(int at in new[]{420,424}){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int armor=(int)Word(pair.Native.InstanceBuffer,424);if(armor!=0){var bad=Clone(pair.Native);bad.InstanceBuffer[armor+8]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    var owner=ConscriptModuleScalarContract.Owners(source).First(o=>o.Element.LocalName=="WeaponSetUpdate");int start=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner);foreach(int at in new[]{start+12,start+16}){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}
                    int tableAt=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("ArmorSet")].Offset;foreach(int at in new[]{tableAt,tableAt+4,tableAt+8}){var bad=Clone(tokens);bad.InstanceBuffer[at]^=4;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                }
                // Reborn: independent source-bound string checks reject both content/length corruption and wrong root metadata targets.
                if(mode.StartsWith("root-text-",StringComparison.Ordinal))foreach(int slot in new[]{404,408}){
                    int header=(int)Word(pair.Native.InstanceBuffer,slot);if(header!=0){foreach(int at in new[]{header,header+4,(int)Word(pair.Native.InstanceBuffer,header+4)}){var bad=Clone(pair.Native);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;}}
                    string name=slot==404?"DisplayName":"DisplayNameTransformed";int row=Enumerable.Range(0,prefix/12).Select(i=>i*12).Single(at=>Word(tokens.InstanceBuffer,at)==FastHash.GetHashCode(name));var metadata=Clone(tokens);metadata.InstanceBuffer[row+8]^=4;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,metadata));metadataFaults++;
                }
                // Reborn: valid-range count/hash/nested-scalar corruption must fail source-aware checking, not merely native bounds validation.
                if(mode.StartsWith("upgrade-",StringComparison.Ordinal)){
                    var owner=ConscriptModuleScalarContract.Owners(source).First(o=>o.Element.LocalName=="ModelConditionUpgrade");int module=ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,owner),baseOffset=0;Type current=owner.NativeType;
                    while(current!=typeof(UpgradeModuleData)){baseOffset+=System.Runtime.InteropServices.Marshal.OffsetOf(current,"Base").ToInt32();current=current.GetField("Base")!.FieldType;}
                    int list=module+baseOffset+System.Runtime.InteropServices.Marshal.OffsetOf(typeof(UpgradeModuleData),"TriggeredBy").ToInt32();var badCount=Clone(pair.Native);Put(badCount.InstanceBuffer,list,Word(badCount.InstanceBuffer,list)+1);Refuse(()=>Verify(source,badCount,pair.Instance,origins));faults++;
                    if(Word(pair.Native.InstanceBuffer,list)>0){int item=(int)Word(pair.Native.InstanceBuffer,list+4);var badHash=Clone(pair.Native);Put(badHash.InstanceBuffer,item,InstanceHandle.GetInstanceId("RebornMissingUpgrade"));Refuse(()=>Verify(source,badHash,pair.Instance,origins));faults++;}
                    int pointer=module+baseOffset+System.Runtime.InteropServices.Marshal.OffsetOf(typeof(UpgradeModuleData),"CustomAnimAndDuration").ToInt32();if(Word(pair.Native.InstanceBuffer,pointer)!=0){var badScalar=Clone(pair.Native);badScalar.InstanceBuffer[(int)Word(badScalar.InstanceBuffer,pointer)+4]^=1;Refuse(()=>Verify(source,badScalar,pair.Instance,origins));faults++;}
                    // Reborn: directly corrupt the independent flexible metadata proof, not only the enclosing serializer's splice comparison.
                    var siblings=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Container=="Behaviors").ToArray();int stop=owner.Index+1<siblings.Length?ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,siblings[owner.Index+1]):ConscriptModuleScalarContract.Address(pair.Native.InstanceBuffer,0,ConscriptModuleScalarContract.Owners(source).Single(o=>o.Container=="Body"));
                    var leaf=Extract(pair.Native,module,stop);var model=templates[owner.Element.GetAttribute("id")];var plan=ConscriptUpgradeStructure.Read(owner,catalog)!;
                    var comparator=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,pair.Instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true,upgrade:plan);comparator.Compare(owner.NativeType,model.Native);
                    var standalone=Ep1BehaviorMetadataTokenizer.Tokenize(leaf,official,model.Type,owner.NativeType);int ownPrefix=standalone.InstanceBuffer.Length-leaf.InstanceBuffer.Length;var models=topology.Containers.Single(c=>c.Name=="Behaviors").Modules;int position=Array.IndexOf(models,model),stockEnd=position+1<models.Length?models[position+1].Wrapper:GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("AI")].Offset;
                    UpgradeStructureMetadataProof.Compare(standalone,leaf,stock.Bytes,model.Wrapper,stockEnd,owner.NativeType,comparator.NativeRecords,plan);
                    var fields=Enumerable.Range(0,ownPrefix/12).Select(i=>i*12).ToArray();int triggerRow=fields.Single(i=>Word(standalone.InstanceBuffer,i)==FastHash.GetHashCode("TriggeredBy")),triggerTable=(int)Word(standalone.InstanceBuffer,triggerRow+8),animationRow=fields.Single(i=>Word(standalone.InstanceBuffer,i)==FastHash.GetHashCode("CustomAnimAndDuration"));
                    var mutations=new List<int>{triggerRow+4,triggerTable,triggerTable+4,triggerTable+8,animationRow+4,animationRow+8};
                    if(Word(leaf.InstanceBuffer,baseOffset+System.Runtime.InteropServices.Marshal.OffsetOf(typeof(UpgradeModuleData),"TriggeredBy").ToInt32())>0)mutations.AddRange(new[]{triggerTable+12,triggerTable+16,triggerTable+20});
                    int animationTable=(int)Word(standalone.InstanceBuffer,animationRow+8);if(animationTable!=0)mutations.AddRange(new[]{animationTable,animationTable+4,animationTable+8,animationTable+12,animationTable+16,animationTable+20,animationTable+36});
                    foreach(int at in mutations){var bad=Clone(standalone);bad.InstanceBuffer[at]^=1;Refuse(()=>UpgradeStructureMetadataProof.Compare(bad,leaf,stock.Bytes,model.Wrapper,stockEnd,owner.NativeType,comparator.NativeRecords,plan));metadataFaults++;}
                }
                // Reborn: a valid but wrong same-type target must still fail after retarget support is enabled.
                if(mode=="retarget-combined"){
                    uint target=InstanceHandle.GetInstanceId("JapanArcherInfantryEnergyBow");
                    uint slot=Slots(pair.Native.ImportsBuffer).First(s=>pair.Instance.ReferencedInstances[(int)Word(pair.Native.InstanceBuffer,(int)s)-1].InstanceId==target);
                    var expected=pair.Instance.ReferencedInstances[(int)Word(pair.Native.InstanceBuffer,(int)slot)-1];
                    int wrong=Enumerable.Range(0,pair.Instance.ReferencedInstances.Count).First(i=>pair.Instance.ReferencedInstances[i].TypeId==expected.TypeId&&pair.Instance.ReferencedInstances[i].InstanceId!=expected.InstanceId);
                    var bad=Clone(pair.Native);Put(bad.InstanceBuffer,(int)slot,(uint)(wrong+1));Refuse(()=>Verify(source,bad,pair.Instance,origins));faults++;
                }
                // Reborn: changed root links, list markers, class tables and native-tail words must fail independent metadata validation.
                int table=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[FastHash.GetHashCode("Behaviors")].Offset;
                int firstModule=(int)Word(tokens.InstanceBuffer,table+20);
                foreach(int at in new[]{table,table+4,table+8,table+12,table+16,table+20,firstModule,firstModule+4,firstModule+8,prefix+address+4}){var bad=Clone(tokens);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(source,pair.Native,pair.Instance,origins,bad));metadataFaults++;}
                File.WriteAllText(Path.Combine(output,mode+".xml"),source.OuterXml);File.WriteAllBytes(Path.Combine(output,mode+".bin"),tokens.InstanceBuffer);
                int count=behaviors.ChildNodes.OfType<XmlElement>().Count();results.Add(new{Mode=mode,BehaviorModules=count,TotalModules=ConscriptModuleScalarContract.Owners(source).Length,MetadataPrefix=prefix,NativeBytes=pair.Native.InstanceBuffer.Length,References=pair.Instance.ReferencedInstances.Count});
                Console.WriteLine($"Variable Conscript graph: {mode}: behaviors={count}, prefix={prefix}, native={pair.Native.InstanceBuffer.Length}, references={pair.Instance.ReferencedInstances.Count}: PASS");
            }
            if(baseline.OuterXml!=snapshot)throw new InvalidDataException("Module graph mutated its baseline.");
            File.WriteAllText(Path.Combine(output,"module-graph-report.json"),JsonSerializer.Serialize(new{Passed=true,Cases=results,NativeCorruptionsRejected=faults,MetadataCorruptionsRejected=metadataFaults,CompleteNativeOwnership=true,BehaviorMetadataStockVerified=true,FullMetadataStockVerified=true,VariableStreamPackagesBuilt=true,AlphaPublicationEnabled=false,GameExecuted=false},new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine($"Variable Conscript module graphs: PASS ({results.Count} complete-object stream/package cases; {faults} native/{metadataFaults} metadata faults rejected; complete native + source-composed stock metadata; no alpha publication/game).");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: normalize all references once and compile through the real reviewed complete-object allocation path. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Build(XmlDocument source)
            {
                var raw=(XmlDocument)source.CloneNode(true);raw.Schemas.Add(official);raw.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));Ep1SelectedModuleIdentity.ValidateSource(raw.DocumentElement!,true);Ep1GameObjectEmptyRootProjection.ValidateSource(raw.DocumentElement!);ConscriptBehaviorSurvey.ResolveStrong(raw,schemas,catalog);
                var instance=ReferencePipelineSmokeTest.Normalize(schemas,raw.OuterXml);var native=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);return(native,instance);
            }

            // Reborn: diagnostics and Studio consume the same independent native/metadata proof.
            Chunk Verify(XmlDocument source,Chunk native,InstanceDeclaration instance,Dictionary<string,string> origins,Chunk? supplied=null)
                =>verifier.Verify(source,native,instance,origins,supplied);
        }
        finally{Settings.Current=saved;}
    }

    // Reborn: isolated negative controls preserve source values/defaults and never mutate the captured baseline.
    private static XmlDocument Copy(XmlDocument source)=>SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));

    // Reborn: rebase a combined metadata owner and its native tail into a standalone stock-proof input without changing any occurrence index.
    private static Chunk Detach(Chunk output,int prefix,int table,int length,int start,int end){var leaf=Extract(output,prefix+start,prefix+end);var result=new Chunk{InstanceBuffer=new byte[length+leaf.InstanceBuffer.Length],RelocationBuffer=Pack(Slots(leaf.RelocationBuffer).Select(s=>s+(uint)length)),ImportsBuffer=Pack(Slots(leaf.ImportsBuffer).Select(s=>s+(uint)length))};output.InstanceBuffer.AsSpan(table,length).CopyTo(result.InstanceBuffer);leaf.InstanceBuffer.CopyTo(result.InstanceBuffer,length);foreach(uint slot in Slots(leaf.RelocationBuffer))Put(result.InstanceBuffer,length+(int)slot,Word(leaf.InstanceBuffer,(int)slot)+(uint)length);for(int at=8;at<length;at+=12){uint target=Word(result.InstanceBuffer,at);if(target!=0)Put(result.InstanceBuffer,at,target>=table&&target<table+length?target-(uint)table:target>=prefix+start&&target<prefix+end?target-(uint)(prefix+start)+(uint)length:throw new InvalidDataException("Variable metadata detachment crosses owners."));}return result;}

    // Reborn: extraction retains common Core indices but rejects pointers escaping their complete owner allocation.
    private static Chunk Extract(Chunk source,int start,int end){if(start<0||end<start||end>source.InstanceBuffer.Length)throw new InvalidDataException("Module extraction bounds.");var leaf=new Chunk{InstanceBuffer=source.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start&&s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start&&s<end).Select(s=>s-(uint)start))};foreach(uint s in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)s);if(target<start||target>=end)throw new InvalidDataException("Cross-module native pointer.");Put(leaf.InstanceBuffer,(int)s,target-(uint)start);}return leaf;}
    // Reborn: duplicate auxiliary ownership must fail rather than silently collapse in a set.
    private static void Add(HashSet<uint> slots,uint value){if(!slots.Add(value))throw new InvalidDataException("Duplicate variable graph auxiliary owner.");}
    // Reborn: diagnostics retain caller-owned snapshots and independent copies for mutation controls.
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    // Reborn: repeated output equality includes all three native streams.
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer)&&a.RelocationBuffer.SequenceEqual(b.RelocationBuffer)&&a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    // Reborn: explicit framing and unique aligned slots precede native partitioning.
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Variable graph auxiliary framing.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Any(s=>s%4!=0)||slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Duplicate/unaligned graph auxiliary slot.");return slots;}
    // Reborn: ordered nonempty auxiliary streams have one terminating sentinel.
    private static byte[] Pack(IEnumerable<uint> source){var slots=source.ToArray();if(slots.Length==0)return Array.Empty<byte>();var bytes=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(bytes,i*4,slots[i]);Put(bytes,slots.Length*4,uint.MaxValue);return bytes;}
    // Reborn: all diagnostic word reads stay within their supplied graph.

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read the scripted draws of the official SDK SovietRefinery source (read-only reference) next to the repository fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement[] SdkLeaves(string fixtureRoot,string relative,string container)
    {
        for(var directory=new DirectoryInfo(fixtureRoot);directory!=null;directory=directory.Parent)
        {
            string path=Path.Combine(new[]{directory.FullName,"alpha","SageXML"}.Concat(relative.Split('/')).ToArray());if(!File.Exists(path))continue;
            var document=SdkXmlDiagnostics.Parse(File.ReadAllBytes(path));return document.GetElementsByTagName(container,SchemaSet.XmlNamespace).OfType<XmlElement>().Single().ChildNodes.OfType<XmlElement>().ToArray();
        }
        throw new InvalidDataException("SDK source not found for schema qualification: "+relative);
    }

    private static XmlElement[] RefineryDraws(string fixtureRoot)
    {
        for(var directory=new DirectoryInfo(fixtureRoot);directory!=null;directory=directory.Parent)
        {
            string path=Path.Combine(directory.FullName,"alpha","SageXML","Soviet","Structures","SovietRefinery.xml");if(!File.Exists(path))continue;
            var document=SdkXmlDiagnostics.Parse(File.ReadAllBytes(path));var draws=document.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
            return draws.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ScriptedModelDraw").ToArray();
        }
        throw new InvalidDataException("SDK SovietRefinery source not found for schema draw qualification.");
    }

    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Variable graph word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
    // Reborn: only newly owned comparison arrays receive identity/coordinate patches.
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
    // Reborn: each deliberately corrupt graph must be refused, not merely reported as incomplete.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Variable module corruption accepted.");}
}
