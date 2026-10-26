using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover weapon-fire ability stock identities and authored weapon-fire fields, inherited references and local module IDs and full object composition without editing user sources.
internal static class WeaponFireAbilitySmokeTest
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
            var grammar=WeaponFireAbilityContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var originals=GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","WeaponFireSpecialAbilityUpdate");var owner=(XmlElement)source.ImportNode(originals[variant<4?variant:0],true);behaviors.AppendChild(owner);
                // Reborn: edit all twelve EP1 attributes together, including both newly restored fields and inherited ability timing.
                if(variant==4){owner.SetAttribute("id","RebornWeaponFireAbility");string[] fields={"SpecialWeapon","WhichSpecialWeapon","SkipContinue","CanTriggerWhileDead","BusyForDuration","NeedLivingTargets","PlayWeaponPreFireFX","OffsetTargetLocationFromObject","WeaponSlotID","WeaponSlotType","DestealthClosestObjectMatchingFilter","ModelConditionsOnUnpack"};string[] values={"PlayerPowerProductionKickbacksWeapon","2","true","true","1.75s","true","true","true","4","SECONDARY_WEAPON","true","DAMAGED"};for(int i=0;i<fields.Length;i++)owner.SetAttribute(fields[i],values[i]);owner.SetAttribute("SpecialPowerTemplate","SpecialPower_ProductionKickbacks");owner.SetAttribute("UnpackTime","0.5s");owner.SetAttribute("StartAbilityRange","250");}
                if(variant==5){foreach(string field in new[]{"SpecialWeapon","WhichSpecialWeapon","SkipContinue","CanTriggerWhileDead","BusyForDuration","NeedLivingTargets","PlayWeaponPreFireFX","OffsetTargetLocationFromObject","WeaponSlotID","WeaponSlotType","DestealthClosestObjectMatchingFilter","ModelConditionsOnUnpack"})owner.RemoveAttribute(field);owner.SetAttribute("id","RebornWeaponFireDefaults");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("WeaponFireSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,WeaponFireAbilityContract.TypeId);var element=resolved.GetElementsByTagName("WeaponFireSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1WeaponFireSpecialAbilityUpdateModuleData),WeaponFireAbilityContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,WeaponFireAbilityContract.Wire,typeof(Ep1WeaponFireSpecialAbilityUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1WeaponFireSpecialAbilityUpdateModuleData),proof.Records);checks++;
                // Reborn: one authored weapon-fire ability variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: reject independent corruption of the weapon import and both EP1-only fields, not just a repeated producer snapshot.
                foreach(int offset in new[]{252,272,277}){
                    var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[offset]^=1;
                    try{SchemaNativeContract.Verify(element,typeof(Ep1WeaponFireSpecialAbilityUpdateModuleData),WeaponFireAbilityContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt weapon-fire ability payload accepted.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: WeaponFireSpecialAbilityUpdate: PASS ({checks} four originals/all twelve attributes/default/inherited/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
