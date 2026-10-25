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

// Reborn: cover original deposits plus authored transaction lists, categories, flags and defaults without editing user sources.
internal static class AutoDepositSmokeTest
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
            var grammar=AutoDepositContract.Qualify(WorldBuilderAssetResolver.CatalogPath(),baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Neutral/Structures/OilDerrick.xml","AutoDepositUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: isolated source expressions use the official account definition already anchored by the stock qualification.
                foreach(var row in owner.ChildNodes.OfType<XmlElement>())row.SetAttribute("Account","0");
                if(variant==1){owner.SetAttribute("id","RebornDepositVariant");owner.SetAttribute("DepositInterval","6s");owner.SetAttribute("Flags","NONE ACTIVE_WHEN_REPAIRING");owner.SetAttribute("TransactionCategory","PILLAGE");owner.SetAttribute("OnlyWhenGarrisoned","true");owner.SetAttribute("GiveNoXP","false");while(owner.HasChildNodes)owner.RemoveChild(owner.FirstChild!);
                    foreach(string list in new[]{"InitialCaptureBonus","Deposit","ResourceCap"})foreach(int account in new[]{0,3}){var row=source.CreateElement(list,SchemaSet.XmlNamespace);row.SetAttribute("Account",account.ToString());row.SetAttribute("Amount",(500+account).ToString());owner.AppendChild(row);}}
                if(variant==2){while(owner.HasChildNodes)owner.RemoveChild(owner.FirstChild!);foreach(string field in new[]{"DepositInterval","Flags","TransactionCategory","GiveNoXP","OnlyWhenGarrisoned"})owner.RemoveAttribute(field);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("AutoDepositUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,AutoDepositContract.TypeId);var element=resolved.GetElementsByTagName("AutoDepositUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1AutoDepositUpdateModuleData),AutoDepositContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,AutoDepositContract.Wire,typeof(Ep1AutoDepositUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1AutoDepositUpdateModuleData),proof.Records);checks++;
                // Reborn: one authored multi-list variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted transaction category must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1AutoDepositUpdateModuleData>(nameof(Ep1AutoDepositUpdateModuleData.TransactionCategory)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1AutoDepositUpdateModuleData),AutoDepositContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt automatic-deposit payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: AutoDepositUpdate: PASS ({checks} original/edit/default/three-list/category/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
