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

// Reborn: cover slaughter-container refunds, filters, inherited optional records and defaults without editing user sources.
internal static class SlaughterContainSmokeTest
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
            var grammar=SlaughterContainContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietCrane.xml","SlaughterHordeContain").Single(),true);behaviors.AppendChild(owner);
                if(variant==1){owner.SetAttribute("id","RebornSlaughterEdited");owner.SetAttribute("CashBackPercent","25%");owner.SetAttribute("CanAlwaysEnterStatus","TARGETED_FOR_REPAIR");owner.SetAttribute("ContainMax","12");owner.SetAttribute("ExitDelay","7");owner.SetAttribute("MobileGarrison","true");
                    // Reborn: inherited vectors, roster and an expanded inline filter remain authored source rather than frozen Crane values.
                    foreach(string name in new[]{"EntryOffset","EntryPosition","ExitOffset"}){var vector=source.CreateElement(name,SchemaSet.XmlNamespace);vector.SetAttribute("x","12.5");vector.SetAttribute("y","-3");owner.InsertBefore(vector,owner.GetElementsByTagName("CanAlwaysEnterObjectFilter",SchemaSet.XmlNamespace)[0]);}
                    var roster=source.CreateElement("InitialRoster",SchemaSet.XmlNamespace);roster.SetAttribute("TemplateId","SovietAntiInfantryInfantry");roster.SetAttribute("Count","3");owner.InsertBefore(roster,owner.FirstChild);
                    var filter=owner.GetElementsByTagName("CanAlwaysEnterObjectFilter",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();filter.SetAttribute("StatusBitFlags","TARGETED_FOR_REPAIR");var excluded=source.CreateElement("ExcludeThing",SchemaSet.XmlNamespace);excluded.InnerText="SovietAntiInfantryInfantry";filter.AppendChild(excluded);}
                if(variant==2){while(owner.Attributes.Count>0)owner.RemoveAttributeNode(owner.Attributes[0]!);owner.SetAttribute("id","RebornSlaughterDefaults");while(owner.HasChildNodes)owner.RemoveChild(owner.FirstChild!);owner.AppendChild(source.CreateElement("CanAlwaysEnterObjectFilter",SchemaSet.XmlNamespace));}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("SlaughterHordeContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,SlaughterContainContract.TypeId);var element=resolved.GetElementsByTagName("SlaughterHordeContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(SlaughterHordeContainModuleData),SlaughterContainContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,SlaughterContainContract.Wire,typeof(SlaughterHordeContainModuleData));_=grammar.Verify(metadata,native,typeof(SlaughterHordeContainModuleData),proof.Records);checks++;
                // Reborn: one authored expanded-container variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted refund percentage must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<SlaughterHordeContainModuleData>(nameof(SlaughterHordeContainModuleData.CashBackPercent)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(SlaughterHordeContainModuleData),SlaughterContainContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt slaughter-container payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: SlaughterHordeContain: PASS ({checks} original/edit/default/filter/inherited/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
