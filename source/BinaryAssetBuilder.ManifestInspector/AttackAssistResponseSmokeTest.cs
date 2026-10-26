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

// Reborn: cover attack-assist response stock imports, authored status and slot fields, defaults and full object composition without editing user sources.
internal static class AttackAssistResponseSmokeTest
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
            var grammar=AttackAssistResponseContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                string path=variant switch{1=>"Allied/Structures/AlliedBaseDefenseAdvanced.xml",2=>"Soviet/Units/SovietHeavyAntiVehicleInfantry.xml",_=>"Soviet/Structures/SovietBaseDefenseAdvanced.xml"};var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(path,"AttackAssistResponseBehavior").Single(),true);behaviors.AppendChild(owner);
                // Reborn: edit the complete status/slot record and pair a renamed response with the original Tesla notification family.
                if(variant==3){owner.SetAttribute("id","RebornAssist");owner.SetAttribute("ObjectStatus","GENERIC_TOGGLE_STATE CHARGING_BASE_DEFENSES");owner.SetAttribute("WeaponSlotID","7");owner.SetAttribute("WeaponSlotType","PRIMARY_WEAPON");var notify=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBaseDefenseAdvanced.xml","NotifyWhenAttackingBehavior").Single(),true);notify.SetAttribute("AssistResponseModuleId","RebornAssist");behaviors.AppendChild(notify);}
                if(variant>=4){owner.RemoveAll();owner.SetAttribute("id","RebornAssistDefaults");if(variant==5){owner.SetAttribute("ObjectStatus","");owner.SetAttribute("WeaponSlotID","4294967295");owner.SetAttribute("WeaponSlotType","SECONDARY_WEAPON");}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("AttackAssistResponseBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,AttackAssistResponseContract.TypeId);var element=resolved.GetElementsByTagName("AttackAssistResponseBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(AttackAssistResponseBehaviorModuleData),AttackAssistResponseContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,AttackAssistResponseContract.Wire,typeof(AttackAssistResponseBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(AttackAssistResponseBehaviorModuleData),proof.Records);checks++;
                // Reborn: authored attack-assist response variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>=3){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted status mask must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<AttackAssistResponseBehaviorModuleData>(nameof(AttackAssistResponseBehaviorModuleData.ObjectStatus)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(AttackAssistResponseBehaviorModuleData),AttackAssistResponseContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt attack-assist response payload accepted.");}catch(InvalidDataException){checks++;}
                if(variant==3){
                    // Reborn: reject class-tag corruption separately from native status-byte corruption.
                    var badTokens=new Chunk{InstanceBuffer=metadata.InstanceBuffer.ToArray(),RelocationBuffer=metadata.RelocationBuffer,ImportsBuffer=metadata.ImportsBuffer};badTokens.InstanceBuffer[4]^=1;
                    try{grammar.Verify(badTokens,native,typeof(AttackAssistResponseBehaviorModuleData),proof.Records);throw new InvalidOperationException("Corrupt attack-assist metadata accepted.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: AttackAssistResponseBehavior: PASS ({checks} three-retail/status-mask/slots/default/notification-pair/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
