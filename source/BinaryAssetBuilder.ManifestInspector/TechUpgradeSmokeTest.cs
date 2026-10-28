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

// Reborn: cover original Allied transmitters, optional radii, tech-range controls and inherited upgrades without editing user sources.
internal static class TechUpgradeSmokeTest
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
            var grammar=TechUpgradeContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(TechUpgradeContract.Original(variant==1?3:2),true);behaviors.AppendChild(owner);
                // Reborn: default probes preserve omitted fields, while an explicit zero RadiusY still owns optional storage.
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornTechTransmitter"+variant);}
                if(variant==4)owner.SetAttribute("RadiusY","0");
                if(variant==3){
                    owner.SetAttribute("Radius","1000");owner.SetAttribute("RadiusY","400");owner.SetAttribute("DistanceType","CIRCULAR");owner.SetAttribute("FXLineBoneName","RebornDish");
                    foreach(string flag in new[]{"RequiresAllTriggers","RequiresAllConflictingTriggers","Permanent","WaypointMode","WaypointModeTerminal"})owner.SetAttribute(flag,"true");
                    owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_1' Frames='12' Trigger='1'/><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech2</TriggeredBy><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</TriggeredBy><ConflictsWith xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</ConflictsWith><ObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='STRUCTURE' Relationship='ALLIES'><IncludeThing>AlliedConstructionYard</IncludeThing></ObjectFilter>";
                }
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("TechUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,TechUpgradeContract.TypeId);var element=resolved.GetElementsByTagName("TechUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(TechUpgradeModuleData),TechUpgradeContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,TechUpgradeContract.Wire,typeof(TechUpgradeModuleData));_=grammar.Verify(metadata,native,typeof(TechUpgradeModuleData),proof.Records);checks++;
                // Reborn: original, default and edited transmitter variants also traverses the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 2 or 3){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted transmitter radius must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<TechUpgradeModuleData>(nameof(TechUpgradeModuleData.Radius)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(TechUpgradeModuleData),TechUpgradeContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt tech transmitter payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: TechUpgrade: PASS ({checks} original-tech2/tech3/default/edit/optional-radius/upgrade/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
