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

// Reborn: cover original stream-state records, schema defaults, authored signed identifiers and full object composition without editing user sources.
internal static class StreamStateSmokeTest
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
            var grammar=StreamStateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            var originals=GameObjectExtendedRootContract.Original("EP1/Soviet/Units/SovietDesolatorInfantry.xml","StreamState");
            for(int variant=0;variant<4;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(originals[Math.Min(variant,1)],true);behaviors.AppendChild(owner);
                // Reborn: default zero and authored signed stream IDs must not be frozen to the two captured retail values.
                if(variant==2)owner.RemoveAttribute("StreamId");
                if(variant==3){owner.SetAttribute("id","RebornStreamState");owner.SetAttribute("StreamId","27");var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornSecondStreamState");second.SetAttribute("StreamId","-1");behaviors.AppendChild(second);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("StreamState",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                var native=ConscriptBehaviorSurvey.Compile(normalized,StreamStateContract.TypeId);var element=resolved.GetElementsByTagName("StreamState",SchemaSet.XmlNamespace).OfType<XmlElement>().First();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(StreamStateModuleData),StreamStateContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,StreamStateContract.Wire,typeof(StreamStateModuleData));_=grammar.Verify(metadata,native,typeof(StreamStateModuleData),proof.Records);checks++;
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
                // Reborn: a changed output stream identifier must fail independent source/native proof.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<StreamStateModuleData>(nameof(StreamStateModuleData.StreamId)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(StreamStateModuleData),StreamStateContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt StreamState payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: StreamState: PASS ({checks} original/default/edited signed-ID/multiple-module/composition/corruption checks; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
