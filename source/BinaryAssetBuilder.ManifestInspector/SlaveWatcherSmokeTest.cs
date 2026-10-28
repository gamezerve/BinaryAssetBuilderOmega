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

// Reborn: cover all original slave-watcher records, schema defaults, authored upgrades/flags/status and full object composition without editing user sources.
internal static class SlaveWatcherSmokeTest
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
            var grammar=SlaveWatcherContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            var upgrades=catalog.Where(a=>a.Name.StartsWith("UpgradeTemplate:",StringComparison.Ordinal)).Select(a=>a.Name.Split(':')[1]).Take(2).ToArray();if(upgrades.Length!=2)throw new InvalidDataException("Two catalogued upgrades are required.");
            for(int variant=0;variant<6;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var witness=SlaveWatcherContract.Witnesses[Math.Min(variant,3)];var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"SlaveWatcherBehavior").Single(),true);behaviors.AppendChild(owner);
                // Reborn: 4 keeps only the id (all schema defaults); 5 edits every field and adds a second watcher.
                if(variant==4){owner.RemoveAll();owner.SetAttribute("id","RebornSlaveWatcherDefaults");}
                if(variant==5){owner.RemoveAll();owner.SetAttribute("id","RebornSlaveWatcher");owner.SetAttribute("GrantUpgrade",upgrades[0]);owner.SetAttribute("RemoveUpgrade",upgrades[1]);foreach(string flag in new[]{"ShareUpgrades","LetSlaveLive","WatchingSpawnedSlave","ForceSameTeam"})owner.SetAttribute(flag,"true");var validation=source.CreateElement("ObjectStatusValidation",SchemaSet.XmlNamespace);validation.SetAttribute("ForbiddenStatus","GENERIC_TOGGLE_STATE");validation.SetAttribute("RequiredStatus","DESOLATION_ACTIVE");owner.AppendChild(validation);var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornSecondSlaveWatcher");second.RemoveAttribute("GrantUpgrade");second.SetAttribute("ShareUpgrades","false");behaviors.AppendChild(second);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("SlaveWatcherBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                var native=ConscriptBehaviorSurvey.Compile(normalized,SlaveWatcherContract.TypeId);var element=resolved.GetElementsByTagName("SlaveWatcherBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().First();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(SlaveWatcherBehaviorModuleData),SlaveWatcherContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,SlaveWatcherContract.Wire,typeof(SlaveWatcherBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(SlaveWatcherBehaviorModuleData),proof.Records);checks++;
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
                // Reborn: a changed output flag byte must fail independent source/native proof.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<SlaveWatcherBehaviorModuleData>(nameof(SlaveWatcherBehaviorModuleData.ForceSameTeam)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(SlaveWatcherBehaviorModuleData),SlaveWatcherContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt SlaveWatcherBehavior payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: SlaveWatcherBehavior: PASS ({checks} four-original/default/edited-upgrade-flag-status/multiple-module/composition/corruption checks; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
