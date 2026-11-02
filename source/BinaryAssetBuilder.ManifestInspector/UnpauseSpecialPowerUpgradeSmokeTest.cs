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

// Reborn: cover unpause-upgrade stock imports, authored power references, defaults and full object composition without editing user sources.
internal static class UnpauseSpecialPowerUpgradeSmokeTest
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
            var grammar=UnpauseSpecialPowerUpgradeContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=source.CreateElement("UnpauseSpecialPowerUpgrade",SchemaSet.XmlNamespace);owner.SetAttribute("id","RebornUnpause");owner.SetAttribute("SpecialPowerTemplate",catalog.First(a=>a.TypeName=="SpecialPowerTemplate").Name.Split(':')[1]);var trigger=source.CreateElement("TriggeredBy",SchemaSet.XmlNamespace);trigger.InnerText="Upgrade_AlliedAirPower";owner.AppendChild(trigger);behaviors.AppendChild(owner);
                // Reborn: cover arbitrary existing power identities, all inherited booleans, repeated trigger/conflict lists and optional animation/default shapes.
                if(variant==1){owner.SetAttribute("id","RebornModifierUpgrade");owner.SetAttribute("SpecialPowerTemplate",catalog.Last(a=>a.TypeName=="SpecialPowerTemplate").Name.Split(':')[1]);owner.SetAttribute("ObeyRechageOnTrigger","true");foreach(string name in new[]{"RequiresAllTriggers","RequiresAllConflictingTriggers","Permanent","WaypointMode"})owner.SetAttribute(name,"true");owner.SetAttribute("WaypointModeTerminal","false");owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_3' Frames='12' Trigger='2'/><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedAirPower</TriggeredBy><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech2</TriggeredBy><ConflictsWith xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</ConflictsWith>";}
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornModifierDefaults");if(variant==3)owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset'/>";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("UnpauseSpecialPowerUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,UnpauseSpecialPowerUpgradeContract.TypeId);var element=resolved.GetElementsByTagName("UnpauseSpecialPowerUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(UnpauseSpecialPowerUpgradeModuleData),UnpauseSpecialPowerUpgradeContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,UnpauseSpecialPowerUpgradeContract.Wire,typeof(UnpauseSpecialPowerUpgradeModuleData));_=grammar.Verify(metadata,native,typeof(UnpauseSpecialPowerUpgradeModuleData),proof.Records);checks++;
                // Reborn: authored unpause-upgrade variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted power reference must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<UnpauseSpecialPowerUpgradeModuleData>(nameof(UnpauseSpecialPowerUpgradeModuleData.SpecialPowerTemplate)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(UnpauseSpecialPowerUpgradeModuleData),UnpauseSpecialPowerUpgradeContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt unpause-upgrade payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: UnpauseSpecialPowerUpgrade: PASS ({checks} original/power/all-upgrade-fields/animation/default/composition/corruption checks; retail Upgrade-base/schema own-field composition; no populated EP1 Unpause witness claimed; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
