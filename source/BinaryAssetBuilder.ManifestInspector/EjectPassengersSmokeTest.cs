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

// Reborn: cover passenger-ejection stock imports, authored ejection fields, defaults and full object composition without editing user sources.
internal static class EjectPassengersSmokeTest
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
            var grammar=EjectPassengersContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,7)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var witness=EjectPassengersContract.Witnesses[Math.Min(variant,1)];var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"EjectPassengersSpecialAbilityUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: the original support-bomber XML also compiles, but its retail object has no ejection module and is not claimed as a native witness.
                if(variant==2){behaviors.RemoveAll();owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/SovietSupportBomber.xml","EjectPassengersSpecialAbilityUpdate").Single(),true);behaviors.AppendChild(owner);}
                // Reborn: use the original campaign replacement after instance directives have served their source-overlay purpose.
                if(variant==3){behaviors.RemoveAll();owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Allied/Units_Campaign/CAMP_AlliedBomberAircraft_Infantry_2.xml","EjectPassengersSpecialAbilityUpdate").Single(),true);foreach(var attribute in owner.Attributes.Cast<XmlAttribute>().Where(a=>a.LocalName=="joinAction").ToArray())owner.RemoveAttributeNode(attribute);behaviors.AppendChild(owner);}
                // Reborn: edit every own field plus inherited timing, power identity and animation; distinguish omitted versus explicit zero-vector optionals.
                if(variant==4){owner.SetAttribute("id","RebornPassengerEjection");foreach(var pair in new[]{("TimePerPassenger","0.125s"),("Speed","33.5"),("Radius","42"),("Height","175"),("ApexHeight","27"),("EjectionFX","FX_SovietManCannon"),("EjectionBoneName","REBORNEJECT"),("RunOffMapWhenDone","true"),("IgnoreTargetLocation","true"),("SpecialPowerTemplate","SpecialPower_EjectPassengers"),("StartAbilityRange","333"),("PreparationTime","0.25s"),("UnpackTime","0.5s"),("Options","RECONSTITUTE_STORED_COMMAND DO_NOT_DO_AI_SPECIAL_POWER")})owner.SetAttribute(pair.Item1,pair.Item2);owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_3' Frames='12' Trigger='2'/><EjectionBoneOffset xmlns='uri:ea.com:eala:asset' x='1.25' y='-2.5' z='3.75'/>";}
                if(variant>=5){owner.RemoveAll();owner.SetAttribute("id","RebornEjectionDefaults");if(variant==6)owner.InnerXml="<EjectionBoneOffset xmlns='uri:ea.com:eala:asset'/>";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("EjectPassengersSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,EjectPassengersContract.TypeId);var element=resolved.GetElementsByTagName("EjectPassengersSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(EjectPassengersSpecialAbilityUpdateModuleData),EjectPassengersContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,EjectPassengersContract.Wire,typeof(EjectPassengersSpecialAbilityUpdateModuleData));_=grammar.Verify(metadata,native,typeof(EjectPassengersSpecialAbilityUpdateModuleData),proof.Records);checks++;
                // Reborn: authored passenger-ejection variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>=3){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted ejection height must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<EjectPassengersSpecialAbilityUpdateModuleData>(nameof(EjectPassengersSpecialAbilityUpdateModuleData.Height)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(EjectPassengersSpecialAbilityUpdateModuleData),EjectPassengersContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt passenger-ejection payload accepted.");}catch(InvalidDataException){checks++;}
                if(variant==4){
                    // Reborn: an altered emitted class tag must fail the independent metadata grammar, even with intact native bytes.
                    var badTokens=new Chunk{InstanceBuffer=metadata.InstanceBuffer.ToArray(),RelocationBuffer=metadata.RelocationBuffer,ImportsBuffer=metadata.ImportsBuffer};badTokens.InstanceBuffer[4]^=1;
                    try{grammar.Verify(badTokens,native,typeof(EjectPassengersSpecialAbilityUpdateModuleData),proof.Records);throw new InvalidOperationException("Corrupt ejection metadata accepted.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: EjectPassengersSpecialAbilityUpdate: PASS ({checks} two-retail/support-source/campaign/all-field/vector/animation/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
