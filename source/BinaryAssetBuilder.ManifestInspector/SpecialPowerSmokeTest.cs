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

// Reborn: cover base-power stock identities and authored filters and optional records, references and local module IDs and full object composition without editing user sources.
internal static class SpecialPowerSmokeTest
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
            var grammar=SpecialPowerContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,9)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var originals=SpecialPowerContract.Originals();var owner=(XmlElement)source.ImportNode(variant<6?originals[variant]:originals.First(e=>e.GetElementsByTagName("AttributeModifierAffects",SchemaSet.XmlNamespace).Count>0),true);behaviors.AppendChild(owner);
                // Reborn: retain all six original base powers, then exercise altered filters, defaults and optional requirement lists.
                if(variant==6){owner.SetAttribute("id","RebornBasePower");owner.SetAttribute("AttributeModifierRange","173.5");var filter=owner.GetElementsByTagName("AttributeModifierAffects",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();filter.SetAttribute("Exclude","AIRCRAFT");filter.SetAttribute("StatusBitFlagsExclude","STEALTHED");}
                if(variant==7){owner.RemoveAll();owner.SetAttribute("id","RebornDefaultPower");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerJapanEmperorsResolve_L1");}
                if(variant==8){owner.SetAttribute("RequirementsFilterMPSkirmish","RebornFirst RebornSecond");foreach(string name in new[]{"RequirementsFilterMP","RequirementsFilterStrategic"}){var filter=source.CreateElement(name,SchemaSet.XmlNamespace);filter.SetAttribute("Rule","ALL");owner.AppendChild(filter);}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("SpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,SpecialPowerContract.TypeId);var element=resolved.GetElementsByTagName("SpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(SpecialPowerModuleData),SpecialPowerContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,SpecialPowerContract.Wire,typeof(SpecialPowerModuleData));_=grammar.Verify(metadata,native,typeof(SpecialPowerModuleData),proof.Records);checks++;
                // Reborn: three authored base-power variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>=6){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted radius must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[System.Runtime.InteropServices.Marshal.OffsetOf<SpecialPowerModuleData>(nameof(SpecialPowerModuleData.AttributeModifierRange)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(SpecialPowerModuleData),SpecialPowerContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt base-power payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: SpecialPower: PASS ({checks} six originals/edited filters/defaults/string-list/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
