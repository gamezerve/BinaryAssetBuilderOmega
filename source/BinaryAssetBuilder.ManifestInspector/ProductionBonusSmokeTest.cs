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

// Reborn: cover production-bonus stock identities and authored six bonus percentages and inherited references and local module IDs and full object composition without editing user sources.
internal static class ProductionBonusSmokeTest
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
            var grammar=ProductionBonusContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var originals=GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","GiveProductionBonusSpecialPower");var owner=(XmlElement)source.ImportNode(originals[variant<3?variant:0],true);behaviors.AppendChild(owner);
                // Reborn: edit every percentage independently, including negative, positive and fractional input; preserve inherited power references.
                if(variant==3){owner.SetAttribute("id","RebornProductionBonus");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerSovietMassProduction");string[] fields={"BuildCostBonus","BuildCostBonusAllies","BuildTimeBonus","BuildTimeBonusAllies","HarvesterBonus","HarvesterBonusAllies"};string[] values={"-12.5%","-3%","-35%","-7%","45%","8%"};for(int i=0;i<fields.Length;i++)owner.SetAttribute(fields[i],values[i]);}
                if(variant==4){foreach(var field in new[]{"BuildCostBonus","BuildCostBonusAllies","BuildTimeBonus","BuildTimeBonusAllies","HarvesterBonus","HarvesterBonusAllies"})owner.RemoveAttribute(field);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("GiveProductionBonusSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,ProductionBonusContract.TypeId);var element=resolved.GetElementsByTagName("GiveProductionBonusSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(GiveProductionBonusSpecialPowerModuleData),ProductionBonusContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,ProductionBonusContract.Wire,typeof(GiveProductionBonusSpecialPowerModuleData));_=grammar.Verify(metadata,native,typeof(GiveProductionBonusSpecialPowerModuleData),proof.Records);checks++;
                // Reborn: one authored production-bonus variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==3){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing an emitted percentage must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[System.Runtime.InteropServices.Marshal.SizeOf<SpecialPowerModuleData>()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(GiveProductionBonusSpecialPowerModuleData),ProductionBonusContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt production-bonus payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: GiveProductionBonusSpecialPower: PASS ({checks} three originals/all six edits/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
