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

// Reborn: cover garrison stock imports, authored garrison fields, defaults and full object composition without editing user sources.
internal static class GarrisonContainSmokeTest
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
            var grammar=GarrisonContainContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();string path=variant==1?"Allied/Structures/AlliedBaseDefense.xml":"Soviet/Structures/SovietBunker.xml";var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(path,"GarrisonContain").Single(),true);behaviors.AppendChild(owner);
                // Reborn: edit every own flag, populate the inline roster and inherited lists/filter graphs, then distinguish omitted and explicitly empty roster forms.
                if(variant==2){owner.SetAttribute("id","RebornGarrison");owner.SetAttribute("MobileGarrison","true");owner.SetAttribute("ImmuneToClearBuildingAttacks","true");owner.SetAttribute("ResetInitialTeamOnCapture","true");owner.SetAttribute("ContainMax","12");owner.SetAttribute("PassDisabilityToRiders","false");owner.SetAttribute("IgnoreDisabledBitsForRiders","EMP");owner.SetAttribute("ObjectStatusOfContained","ENCLOSED UNSELECTABLE NO_ATTACK");string modifier=catalog.First(a=>a.Name.StartsWith("AttributeModifier:")).Name.Split(':')[1];owner.InnerXml="<PassengerFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='INFANTRY VEHICLE'><ExcludeThing>SovietScoutVehicle</ExcludeThing></PassengerFilter><DieMuxData xmlns='uri:ea.com:eala:asset' DeathTypes='ALL'/><PassengerData xmlns='uri:ea.com:eala:asset' BonePrefix='RebornBone' MaxPassengers='4' SlingUnderBone='true'><Filter Rule='ANY' Include='INFANTRY'/></PassengerData>"+$"<ModifierToGiveOnExit xmlns='uri:ea.com:eala:asset'>{modifier}</ModifierToGiveOnExit>"+"<MemberTemplateStatusInfo xmlns='uri:ea.com:eala:asset' ThingTemplate='SovietAntiInfantryInfantry' ObjectStatus='ENCLOSED'/><InitialRoster xmlns='uri:ea.com:eala:asset' TemplateId='SovietAntiInfantryInfantry' Count='3'/>";}
                if(variant>=3){owner.RemoveAll();owner.SetAttribute("id","RebornGarrisonDefaults");if(variant==4)owner.InnerXml="<InitialRoster xmlns='uri:ea.com:eala:asset'/>";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("GarrisonContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,GarrisonContainContract.TypeId);var element=resolved.GetElementsByTagName("GarrisonContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(GarrisonContainModuleData),GarrisonContainContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,GarrisonContainContract.Wire,typeof(GarrisonContainModuleData));_=grammar.Verify(metadata,native,typeof(GarrisonContainModuleData),proof.Records);checks++;
                // Reborn: authored garrison variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted mobile-garrison flag must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<GarrisonContainModuleData>(nameof(GarrisonContainModuleData.MobileGarrison)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(GarrisonContainModuleData),GarrisonContainContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt garrison payload accepted.");}catch(InvalidDataException){checks++;}
                if(variant==2){
                    // Reborn: corrupt roster metadata must fail independently of the intact native roster and inherited payload.
                    var badTokens=new Chunk{InstanceBuffer=metadata.InstanceBuffer.ToArray(),RelocationBuffer=metadata.RelocationBuffer,ImportsBuffer=metadata.ImportsBuffer};badTokens.InstanceBuffer[4]^=1;
                    try{grammar.Verify(badTokens,native,typeof(GarrisonContainModuleData),proof.Records);throw new InvalidOperationException("Corrupt garrison metadata accepted.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: GarrisonContain: PASS ({checks} two-retail/all-flags/inline-roster/filter/member/modifier/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
