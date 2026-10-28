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

// Reborn: cover original command-set upgrades, weak identities and inherited settings without editing user sources.
internal static class CommandSetUpgradeSmokeTest
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
            var grammar=CommandSetUpgradeContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(CommandSetUpgradeContract.Original(),true);behaviors.AppendChild(owner);
                // Reborn: default and authored weak-ID probes are independent of original command-set names and trigger lists.
                if(variant>=1){owner.RemoveAll();owner.SetAttribute("id","RebornCommandSetUpgrade"+variant);}
                if(variant==2){
                    owner.SetAttribute("CommandSet","AlliedBarracksCommandSet");owner.SetAttribute("RequiresAllTriggers","true");owner.SetAttribute("Permanent","true");
                    owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_1' Frames='12' Trigger='1'/><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech2</TriggeredBy><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</TriggeredBy><ConflictsWith xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</ConflictsWith>";
                }
                _=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("CommandSetUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,CommandSetUpgradeContract.TypeId);var element=resolved.GetElementsByTagName("CommandSetUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(CommandSetUpgradeModuleData),CommandSetUpgradeContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,CommandSetUpgradeContract.Wire,typeof(CommandSetUpgradeModuleData));_=grammar.Verify(metadata,native,typeof(CommandSetUpgradeModuleData),proof.Records);checks++;
                // Reborn: changing the emitted command-set identity must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<CommandSetUpgradeModuleData>(nameof(CommandSetUpgradeModuleData.CommandSet)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(CommandSetUpgradeModuleData),CommandSetUpgradeContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt command-set upgrade payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: CommandSetUpgrade: PASS ({checks} original/default/edit/weak-id/upgrade/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
