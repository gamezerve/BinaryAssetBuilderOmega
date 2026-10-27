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

// Reborn: cover Uprising stealth detector stock layouts, imports, masks, defaults and full object composition without editing user sources.
internal static class StealthDetectorSmokeTest
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
            var grammar=StealthDetectorContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/SovietScoutInfantry.xml","StealthDetectorUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: exercise every own attribute, high KindOf bits, real typed imports and all inherited upgrade list/animation fields.
                if(variant==1){
                    owner.SetAttribute("id","RebornStealthDetector");owner.SetAttribute("DetectionRate","1250ms");owner.SetAttribute("DetectionRange","123.5");
                    foreach(string name in new[]{"InitiallyDisabled","CanDetectWhileGarrisoned","CanDetectWhileContained","RequiresAllTriggers","RequiresAllConflictingTriggers","Permanent","WaypointMode"})owner.SetAttribute(name,"true");owner.SetAttribute("WaypointModeTerminal","false");
                    foreach(string name in new[]{"PingSound","LoudPingSound"})owner.SetAttribute(name,catalog.First(a=>a.TypeName=="AudioEvent").Name);
                    foreach(string name in new[]{"IRBeaconParticleSys","IRParticleSys","IRBrightParticleSys","IRGridParticleSys"})owner.SetAttribute(name,catalog.First(a=>a.TypeName=="FXParticleSystemTemplate").Name);
                    owner.SetAttribute("IRParticleSysBone","RebornDetectorBone");owner.SetAttribute("ExtraRequiredKindOf","INFANTRY DETECTOR");owner.SetAttribute("ExtraForbiddenKindOf","AIRCRAFT");owner.SetAttribute("RequiredUpgrade","Upgrade_AlliedAirPower");
                    owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_3' Frames='12' Trigger='2'/><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedAirPower</TriggeredBy><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech2</TriggeredBy><ConflictsWith xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</ConflictsWith>";
                }
                // Reborn: unauthored fields keep official defaults; explicit empty bone/masks/references retain their native null/empty representation.
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornDetectorDefaults");if(variant==3)foreach(string name in new[]{"IRParticleSysBone","ExtraRequiredKindOf","ExtraForbiddenKindOf","PingSound","LoudPingSound","IRBeaconParticleSys","IRParticleSys","IRBrightParticleSys","IRGridParticleSys","RequiredUpgrade"})owner.SetAttribute(name,"");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("StealthDetectorUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,StealthDetectorContract.TypeId);var element=resolved.GetElementsByTagName("StealthDetectorUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1StealthDetectorUpdateModuleData),StealthDetectorContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,StealthDetectorContract.Wire,typeof(Ep1StealthDetectorUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1StealthDetectorUpdateModuleData),proof.Records);checks++;
                // Reborn: authored detector variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted radius must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1StealthDetectorUpdateModuleData>(nameof(Ep1StealthDetectorUpdateModuleData.DetectionRange)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1StealthDetectorUpdateModuleData),StealthDetectorContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt detector payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: StealthDetectorUpdate: PASS ({checks} three-stock/all-detector-fields/typed-imports/KindOf/bone/upgrade/default/empty/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
