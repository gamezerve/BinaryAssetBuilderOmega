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

// Reborn: cover power-manager stock identities and authored power/EVA references and local module IDs and full object composition without editing user sources.
internal static class PlayerPowerManagerSmokeTest
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
            var grammar=PlayerPowerManagerContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","PlayerPowerManager").First(),true);behaviors.AppendChild(owner);
                // Reborn: edit the local power-manager identity through the official inherited module contract.
                if(variant==1){owner.SetAttribute("id","RebornPowerManager");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerAlliedFreeTrade");owner.SetAttribute("OnAvailableEvaEvent","FreeTradeUpradeAuthorized");}
                if(variant==2){owner.SetAttribute("id","RebornPowerManagerDefaults");owner.RemoveAttribute("OnAvailableEvaEvent");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("PlayerPowerManager",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,PlayerPowerManagerContract.TypeId);var element=resolved.GetElementsByTagName("PlayerPowerManager",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1PlayerPowerManagerBehaviorModuleData),PlayerPowerManagerContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,PlayerPowerManagerContract.Wire,typeof(Ep1PlayerPowerManagerBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(Ep1PlayerPowerManagerBehaviorModuleData),proof.Records);checks++;
                // Reborn: one authored power-manager variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted power import must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[8]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1PlayerPowerManagerBehaviorModuleData),PlayerPowerManagerContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt power-manager payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: PlayerPowerManager: PASS ({checks} original/retarget/EVA/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
