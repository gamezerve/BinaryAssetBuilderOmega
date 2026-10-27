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

// Reborn: cover original capture records, schema defaults, filters and optional enum pointers and full object composition without editing user sources.
internal static class CaptureAbilitySmokeTest
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
            var grammar=CaptureAbilityContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            // Reborn: original include, authored inherited values and omitted defaults use the existing production capture dispatcher.
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Includes/CaptureBehavior.xml","CaptureSpecialAbilityUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: source-owned IDs, actual power references, durations, range and options are independent of the shared retail include.
                if(variant==1){owner.SetAttribute("id","RebornCapture");owner.SetAttribute("SpecialPowerTemplate","SpecialPower_MagneticBomb");owner.SetAttribute("UnpackTime","0.5s");owner.SetAttribute("PreparationTime","3s");owner.SetAttribute("PackTime","2s");owner.SetAttribute("StartAbilityRange","75");owner.SetAttribute("Options","DO_CAPTURE_FX RECONSTITUTE_STORED_COMMAND");owner.SetAttribute("ContactPointOverride","RebornCapturePoint");owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_3' Frames='12' Trigger='2'/>";}
                // Reborn: schema-default capture abilities do not require authored own fields or a stock-named local identity.
                if(variant==2){owner.RemoveAll();owner.SetAttribute("id","RebornCaptureDefaults");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("CaptureSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,CaptureAbilityContract.TypeId);var element=resolved.GetElementsByTagName("CaptureSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(CaptureSpecialAbilityUpdateModuleData),CaptureAbilityContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,CaptureAbilityContract.Wire,typeof(CaptureSpecialAbilityUpdateModuleData));_=grammar.Verify(metadata,native,typeof(CaptureSpecialAbilityUpdateModuleData),proof.Records);checks++;
                // Reborn: authored capture variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted capture range must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<SpecialAbilityUpdateModuleData>(nameof(SpecialAbilityUpdateModuleData.StartAbilityRange)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(CaptureSpecialAbilityUpdateModuleData),CaptureAbilityContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt capture payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: invalid schema values remain errors after replacing the former unsupported-family gate.
            foreach(string invalid in new[]{"Options='UNKNOWN'","UnexpectedField='1'"}){
                var source=(XmlDocument)baseline.CloneNode(true);source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().InnerXml="<CaptureSpecialAbilityUpdate xmlns='uri:ea.com:eala:asset' id='Invalid' "+invalid+"/>";
                try{_=SchemaNativeContract.Resolve(source,schemas,catalog);throw new InvalidOperationException("Invalid capture source accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: CaptureSpecialAbilityUpdate: PASS ({checks} three-stock/shared-include/edited-power-and-inherited-values/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
