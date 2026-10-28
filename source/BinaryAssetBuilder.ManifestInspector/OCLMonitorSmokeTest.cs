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

// Reborn: cover the original OCL monitor, schema defaults, authored options/status and full object composition without editing user sources.
internal static class OCLMonitorSmokeTest
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
            var grammar=OCLMonitorContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var witness=OCLMonitorContract.Witnesses[0];var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"OCLMonitorUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: 1 sets every option and a required master status and adds a second monitor; 2 keeps only the id.
                if(variant==1){owner.SetAttribute("id","RebornOCLMonitor");owner.SetAttribute("Options","MASTER KILL_WHEN_RELEASED RELEASE_WHEN_MASTER_CHANGES_TARGET");owner.SetAttribute("RequiredMasterObjectStatus","DESOLATION_ACTIVE GENERIC_TOGGLE_STATE");var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornSecondOCLMonitor");second.SetAttribute("Options","KILL_WHEN_RELEASED");behaviors.AppendChild(second);}
                if(variant==2){owner.RemoveAll();owner.SetAttribute("id","RebornOCLMonitorDefaults");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("OCLMonitorUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                var native=ConscriptBehaviorSurvey.Compile(normalized,OCLMonitorContract.TypeId);var element=resolved.GetElementsByTagName("OCLMonitorUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(OCLMonitorUpdateModuleData),OCLMonitorContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,OCLMonitorContract.Wire,typeof(OCLMonitorUpdateModuleData));_=grammar.Verify(metadata,native,typeof(OCLMonitorUpdateModuleData),proof.Records);checks++;
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
                // Reborn: a changed option word must fail independent source/native proof.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<OCLMonitorUpdateModuleData>(nameof(OCLMonitorUpdateModuleData.Options)).ToInt32()]^=2;
                try{SchemaNativeContract.Verify(element,typeof(OCLMonitorUpdateModuleData),OCLMonitorContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt OCLMonitorUpdate payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: OCLMonitorUpdate: PASS ({checks} original/edited-options-status/default/multiple-module/composition/corruption checks; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
