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

// Reborn: cover timer-monitor stock imports, authored ready-condition masks, defaults and full object composition without editing user sources.
internal static class MonitorSpecialPowerTimerSmokeTest
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
            var grammar=MonitorSpecialPowerTimerContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietSuperWeapon.xml","MonitorSpecialPowerTimerUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: retarget the power and change multiple ready-condition bits, then also check fully omitted optional fields.
                if(variant==1){owner.SetAttribute("id","RebornTimerMonitor");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerParticleCannon");owner.SetAttribute("ReadyCondition","SPECIALPOWER1_READY DAMAGED");}
                if(variant==2){while(owner.Attributes.Count>0)owner.RemoveAttributeNode(owner.Attributes[0]!);owner.SetAttribute("id","RebornTimerMonitorDefaults");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("MonitorSpecialPowerTimerUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,MonitorSpecialPowerTimerContract.TypeId);var element=resolved.GetElementsByTagName("MonitorSpecialPowerTimerUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(MonitorSpecialPowerTimerUpdateModuleData),MonitorSpecialPowerTimerContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,MonitorSpecialPowerTimerContract.Wire,typeof(MonitorSpecialPowerTimerUpdateModuleData));_=grammar.Verify(metadata,native,typeof(MonitorSpecialPowerTimerUpdateModuleData),proof.Records);checks++;
                // Reborn: one authored timer-monitor variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted ready-condition mask must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<MonitorSpecialPowerTimerUpdateModuleData>(nameof(MonitorSpecialPowerTimerUpdateModuleData.ReadyCondition)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(MonitorSpecialPowerTimerUpdateModuleData),MonitorSpecialPowerTimerContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt timer-monitor payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: MonitorSpecialPowerTimerUpdate: PASS ({checks} original/retarget/flags/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
