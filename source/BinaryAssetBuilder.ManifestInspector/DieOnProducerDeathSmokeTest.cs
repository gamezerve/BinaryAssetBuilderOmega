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

// Reborn: cover the original producer-death watcher, schema defaults, authored interval/flag and full object composition without editing user sources.
internal static class DieOnProducerDeathSmokeTest
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
            var grammar=DieOnProducerDeathContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var witness=DieOnProducerDeathContract.Witnesses[0];var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"DieOnProducerDeathUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: 1 edits the interval and flag and adds a second watcher; 2 keeps only the id.
                if(variant==1){owner.SetAttribute("id","RebornDieOnProducerDeath");owner.SetAttribute("SleepInterval","1.25s");owner.SetAttribute("DieOnProducerDisabled","false");var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornSecondDieOnProducerDeath");second.SetAttribute("SleepInterval","2s");second.SetAttribute("DieOnProducerDisabled","true");behaviors.AppendChild(second);}
                if(variant==2){owner.RemoveAll();owner.SetAttribute("id","RebornDieOnProducerDeathDefaults");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("DieOnProducerDeathUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                var native=ConscriptBehaviorSurvey.Compile(normalized,DieOnProducerDeathContract.TypeId);var element=resolved.GetElementsByTagName("DieOnProducerDeathUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(DieOnProducerDeathUpdateModuleData),DieOnProducerDeathContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,DieOnProducerDeathContract.Wire,typeof(DieOnProducerDeathUpdateModuleData));_=grammar.Verify(metadata,native,typeof(DieOnProducerDeathUpdateModuleData),proof.Records);checks++;
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
                // Reborn: a changed interval must fail independent source/native proof.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<DieOnProducerDeathUpdateModuleData>(nameof(DieOnProducerDeathUpdateModuleData.SleepInterval)).ToInt32()+3]^=0x10;
                try{SchemaNativeContract.Verify(element,typeof(DieOnProducerDeathUpdateModuleData),DieOnProducerDeathContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt DieOnProducerDeathUpdate payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: DieOnProducerDeathUpdate: PASS ({checks} original/edited-interval-flag/default/multiple-module/composition/corruption checks; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
