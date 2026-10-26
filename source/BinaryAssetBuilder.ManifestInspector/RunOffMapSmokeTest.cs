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

// Reborn: cover run-off-map stock imports, authored waypoint strings, defaults and full object composition without editing user sources.
internal static class RunOffMapSmokeTest
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
            var grammar=RunOffMapContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/BaseSovietSupportBomber.xml","RunOffMapBehavior").Single(),true);behaviors.AppendChild(owner);
                // Reborn: compile the exact campaign replacement, edited waypoint/flags, omitted defaults and explicit empty text.
                if(variant==1){behaviors.RemoveAll();owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Allied/Units_Campaign/CAMP_AlliedBomberAircraft_Infantry_2.xml","RunOffMapBehavior").Single(),true);behaviors.AppendChild(owner);foreach(var attribute in owner.Attributes.Cast<XmlAttribute>().Where(a=>a.LocalName=="joinAction").ToArray())owner.RemoveAttributeNode(attribute);}
                if(variant==2){owner.SetAttribute("id","RebornRunOffMap");owner.SetAttribute("RequiresSpecificTrigger","false");owner.SetAttribute("DieOnMap","true");owner.SetAttribute("FlyingOffMap","false");owner.SetAttribute("RunOffMapWaypointName","Reborn_ExitWaypoint");}
                if(variant>=3){owner.RemoveAll();owner.SetAttribute("id","RebornRunOffDefaults");if(variant==4)owner.SetAttribute("RunOffMapWaypointName","");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("RunOffMapBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,RunOffMapContract.TypeId);var element=resolved.GetElementsByTagName("RunOffMapBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(RunOffMapBehaviorModuleData),RunOffMapContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,RunOffMapContract.Wire,typeof(RunOffMapBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(RunOffMapBehaviorModuleData),proof.Records);checks++;
                // Reborn: authored run-off-map variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted trigger flag must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<RunOffMapBehaviorModuleData>(nameof(RunOffMapBehaviorModuleData.RequiresSpecificTrigger)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(RunOffMapBehaviorModuleData),RunOffMapContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt run-off-map payload accepted.");}catch(InvalidDataException){checks++;}
                if(variant==2){
                    // Reborn: metadata class corruption must fail even with intact native waypoint/flag bytes.
                    var badTokens=new Chunk{InstanceBuffer=metadata.InstanceBuffer.ToArray(),RelocationBuffer=metadata.RelocationBuffer,ImportsBuffer=metadata.ImportsBuffer};badTokens.InstanceBuffer[4]^=1;
                    try{grammar.Verify(badTokens,native,typeof(RunOffMapBehaviorModuleData),proof.Records);throw new InvalidOperationException("Corrupt run-off-map metadata accepted.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: RunOffMapBehavior: PASS ({checks} retail/campaign/waypoint/all-flags/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
