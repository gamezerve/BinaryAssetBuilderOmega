using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover original toggle records, schema defaults, filters and optional enum pointers and full object composition without editing user sources.
internal static class ToggleStatusCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete source/native/relocation/metadata checks and a corrupt-output refusal through the existing production EP1 dispatcher. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            baseline.DocumentElement!.SetAttribute("BuildTime","4");
            var grammar=ToggleStatusContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            // Reborn: exercise all six stock families and source graphs that no longer resemble the Conscript's two-state sample.
            var originals=ToggleStatusContract.Witnesses.SelectMany(w=>ToggleStatusContract.Originals(w.Item2)).ToArray();
            // Reborn: validate every standalone leaf before running the more expensive whole-object integration checks.
            var fullInputs=new List<(XmlDocument Source,InstanceDeclaration Instance,ConscriptVariableBehaviorSource.Plan Plan)>();
            foreach(int variant in Enumerable.Range(0,9)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(originals[variant<6?variant:1],true);behaviors.AppendChild(owner);
                // Reborn: the authored Flak Trooper variant changes state audio, appends a third state and edits nested weapon AI and scalar defaults.
                if(variant==6){owner.SetAttribute("id","RebornToggleCustom");owner.SetAttribute("ReloadAllWeaponsOnToggle","false");owner.SetAttribute("ForceIdleAfterToggle","true");var states=owner.GetElementsByTagName("ToggleState",SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray();states[0].SetAttribute("EnterStateSound","SOV_FlakTrooper_VoiceArmCannon");owner.AppendChild(states[1].CloneNode(true));foreach(var ai in owner.GetElementsByTagName("SkirmishAiInfo",SchemaSet.XmlNamespace).OfType<XmlElement>()){ai.SetAttribute("NeverUseWhenHealthIsBelow","500");ai.SetAttribute("UseOnlyWhenPowerAdvantageIsAbove","1.2");}foreach(var weapon in owner.GetElementsByTagName("StateWeapon",SchemaSet.XmlNamespace).OfType<XmlElement>())weapon.SetAttribute("ToggleShortRangeTargetForcesTarget","false");}
                // Reborn: the schema permits one or arbitrarily many states, with required AI and source-owned Module/geometry lists.
                if(variant>=7){owner.RemoveAll();owner.SetAttribute("id","RebornToggleDefaults");owner.InnerXml="<ToggleState xmlns='uri:ea.com:eala:asset'><SkirmishAiInfo ToggleHint='TOGGLE_ALWAYS'/></ToggleState>";if(variant==8){var state=(XmlElement)owner.FirstChild!;state.SetAttribute("AIUpdateModuleId","ModuleTag_AI");state.SetAttribute("ArmorSet","HERO");state.SetAttribute("LocomotorSet","SPECIALABILITY_ACTIVE");state.SetAttribute("GenericLuaEventParameter","reborn test payload");state.InnerXml="<SkirmishAiInfo xmlns='uri:ea.com:eala:asset' ToggleHint='TOGGLE_ALWAYS'/><Module xmlns='uri:ea.com:eala:asset' ModuleId='RebornToggleDefaults' TriggerType='TARGET_OBJECT'/><ActiveGeometry xmlns='uri:ea.com:eala:asset'>ship_geom</ActiveGeometry><ActiveGeometry xmlns='uri:ea.com:eala:asset'>air_geom</ActiveGeometry>";}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("ToggleStatusSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,ToggleStatusContract.TypeId);var element=resolved.GetElementsByTagName("ToggleStatusSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(ToggleStatusSpecialAbilityUpdateModuleData),ToggleStatusContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,ToggleStatusContract.Wire,typeof(ToggleStatusSpecialAbilityUpdateModuleData));_=grammar.Verify(metadata,native,typeof(ToggleStatusSpecialAbilityUpdateModuleData),proof.Records);checks++;
                // Reborn: authored toggle variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant is 1 or 6 or 8)fullInputs.Add((source,instance,plan));
                // Reborn: changing the emitted ReloadAllWeaponsOnToggle value must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<ToggleStatusSpecialAbilityUpdateModuleData>(nameof(ToggleStatusSpecialAbilityUpdateModuleData.ReloadAllWeaponsOnToggle)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(ToggleStatusSpecialAbilityUpdateModuleData),ToggleStatusContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt toggle payload accepted.");}catch(InvalidDataException){checks++;}
            }
            foreach(string invalid in new[]{"<ToggleStatusSpecialAbilityUpdate xmlns='uri:ea.com:eala:asset' id='Invalid'/>","<ToggleStatusSpecialAbilityUpdate xmlns='uri:ea.com:eala:asset' id='Invalid'><ToggleState><SkirmishAiInfo ToggleHint='UNKNOWN'/></ToggleState></ToggleStatusSpecialAbilityUpdate>"}){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.InnerXml=invalid;
                try{_=SchemaNativeContract.Resolve(source,schemas,catalog);throw new InvalidOperationException("Invalid toggle source accepted.");}catch(InvalidDataException){checks++;}
            }
            foreach(var input in fullInputs){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(input.Instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(input.Source,full,input.Instance,input.Plan.Origins);checks++;}
            Console.WriteLine($"Reborn: ToggleStatusSpecialAbilityUpdate: PASS ({checks} six-stock/source-sized-states/AI/weapon/module/geometry/audio/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
