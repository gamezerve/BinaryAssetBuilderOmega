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
            string[] modes={"original","reverse","remove-half","clone-twelve","combined","new-id-combined","retarget-combined","beyond-former-budget","upgrade-lists-expanded","upgrade-lists-empty","upgrade-animation","upgrade-animation-defaults","upgrade-list-large","upgrade-animation-lexical","root-text-edited","root-text-empty","root-text-omitted","combat-combined","combat-omitted","combat-single","shared-parity","shared-defaults"};
            if(caseFilter!=null&&caseFilter!="combat"&&!modes.Contains(caseFilter,StringComparer.Ordinal))throw new InvalidDataException("Unknown module graph proof case.");
            foreach(string mode in caseFilter==null?modes:caseFilter=="combat"?modes.Where(m=>m.StartsWith("combat-",StringComparison.Ordinal)):new[]{caseFilter})
            {
                var source=(XmlDocument)baseline.CloneNode(true);var origins=templates.Keys.ToDictionary(id=>id,id=>id,StringComparer.Ordinal);var behaviors=(XmlElement)source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!;var original=behaviors.ChildNodes.OfType<XmlElement>().ToArray();
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
                ConscriptVariableBehaviorSource.Admit(source,baseline,official);
                // Reborn: a new object identity receives a command only from an explicit authored declaration, never from its ID alone.
                if(mode=="new-id-combined"){if(catalog.Any(a=>a.TypeId==0x942FFF2D&&a.InstanceId==InstanceHandle.GetInstanceId("RebornConscriptTrial")))throw new InvalidDataException("Variable trial ID collides with stock.");source.DocumentElement!.SetAttribute("id","RebornConscriptTrial");}
                var pair=Build(source);var tokens=Verify(source,pair.Native,pair.Instance,origins);var repeat=Build(source);
                if(!Equal(pair.Native,repeat.Native)||!Equal(tokens,Verify(source,repeat.Native,repeat.Instance,origins)))throw new InvalidDataException("Variable graph repeated build differs.");
                int prefix=tokens.InstanceBuffer.Length-pair.Native.InstanceBuffer.Length;
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
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Variable graph word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
    // Reborn: only newly owned comparison arrays receive identity/coordinate patches.
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
    // Reborn: each deliberately corrupt graph must be refused, not merely reported as incomplete.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Variable module corruption accepted.");}
}
