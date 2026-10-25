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

// Reborn: cover original and renamed production-info markers and source-sized module lists without editing user sources.
internal static class ProductionUpdateInfoSmokeTest
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
            var grammar=ProductionUpdateInfoContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,2)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietConstructionYard.xml","ProductionUpdateInfo").Single(),true);behaviors.AppendChild(owner);
                // Reborn: authored marker identities and repeated independently named instances require no fixed-stock name or module count.
                if(variant==1){owner.SetAttribute("id","RebornProductionInfo");var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornProductionInfoSecond");behaviors.AppendChild(second);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("ProductionUpdateInfo",SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray();
                foreach(var marker in normalized){
                    var native=ConscriptBehaviorSurvey.Compile(marker,ProductionUpdateInfoContract.TypeId);var element=resolved.GetElementsByTagName("ProductionUpdateInfo",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.GetAttribute("id")==marker.GetAttribute("id"));var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                    var proof=SchemaNativeContract.Verify(element,typeof(ProductionUpdateInfoModuleData),ProductionUpdateInfoContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,ProductionUpdateInfoContract.Wire,typeof(ProductionUpdateInfoModuleData));_=grammar.Verify(metadata,native,typeof(ProductionUpdateInfoModuleData),proof.Records);checks++;
                    // Reborn: a changed emitted local ID must be rejected independently of the marker writer.
                    var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[4]^=1;
                    try{SchemaNativeContract.Verify(element,typeof(ProductionUpdateInfoModuleData),ProductionUpdateInfoContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt production-info identity accepted.");}catch(InvalidDataException){checks++;}
                }
                // Reborn: two authored markers also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
            }
            Console.WriteLine($"Reborn: ProductionUpdateInfo: PASS ({checks} original/renamed/multiple/composition/identity-corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
