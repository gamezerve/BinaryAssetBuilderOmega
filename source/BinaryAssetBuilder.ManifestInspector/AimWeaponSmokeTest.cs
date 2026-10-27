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

// Reborn: cover original aiming records, schema defaults, authored scalar values and full object composition without editing user sources.
internal static class AimWeaponSmokeTest
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
            var grammar=AimWeaponContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/SovietAntiVehicleInfantry.xml","AimWeapon").Single(),true);behaviors.AppendChild(owner);
                // Reborn: source IDs and all four finite aiming values vary independently of the original unit identity.
                if(variant==1){owner.SetAttribute("id","RebornAimWeapon");owner.SetAttribute("AimLowThreshold","-0.75");owner.SetAttribute("AimHighThreshold","0.9");owner.SetAttribute("AimNearDistance","12.5");owner.SetAttribute("AimFarDistance","700");}
                // Reborn: omitted values use Uprising defaults, while authored zeros remain zeros instead of being replaced by defaults.
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornAimDefaults");if(variant==3)foreach(string name in new[]{"AimLowThreshold","AimHighThreshold","AimNearDistance","AimFarDistance"})owner.SetAttribute(name,"0");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("AimWeapon",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,AimWeaponContract.TypeId);var element=resolved.GetElementsByTagName("AimWeapon",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(AimWeaponBehaviorModuleData),AimWeaponContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,AimWeaponContract.Wire,typeof(AimWeaponBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(AimWeaponBehaviorModuleData),proof.Records);checks++;
                // Reborn: authored aiming variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted radius must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<AimWeaponBehaviorModuleData>(nameof(AimWeaponBehaviorModuleData.AimLowThreshold)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(AimWeaponBehaviorModuleData),AimWeaponContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt aiming payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: AimWeapon: PASS ({checks} two-stock/four-floats/renamed-ID/default/explicit-zero/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
