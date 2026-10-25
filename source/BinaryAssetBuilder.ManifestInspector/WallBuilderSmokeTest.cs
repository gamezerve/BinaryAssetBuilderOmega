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

// Reborn: cover original walls, editable segment count, flags and expanded connecting filters without editing user sources.
internal static class WallBuilderSmokeTest
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
            var grammar=WallBuilderContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietWallPiece.xml","WallBuilderBehavior").Single(),true);behaviors.AppendChild(owner);
                if(variant==1){owner.SetAttribute("id","RebornWallBuilder");owner.SetAttribute("MaxNumSegments","39");owner.SetAttribute("SecondaryModelConditions","WALL_SEGMENT DAMAGED");owner.SetAttribute("SecondaryObjectStatus","WALL_SEGMENT UNSELECTABLE");owner.SetAttribute("StartInSecondaryModel","true");
                    // Reborn: custom weak names and status-filter pointers remain source-owned rather than limited to the original two wall objects.
                    var filter=owner.GetElementsByTagName("ConnectingObjectFilter",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();filter.SetAttribute("Relationship","ALLIES");filter.SetAttribute("StatusBitFlagsExclude","AIRBORNE_TARGET");var target=source.CreateElement("IncludeThing",SchemaSet.XmlNamespace);target.InnerText="SovietTeslaWallHub";filter.AppendChild(target);}
                if(variant==2){while(owner.Attributes.Count>0)owner.RemoveAttributeNode(owner.Attributes[0]!);owner.SetAttribute("id","RebornWallDefaults");while(owner.HasChildNodes)owner.RemoveChild(owner.FirstChild!);owner.AppendChild(source.CreateElement("ConnectingObjectFilter",SchemaSet.XmlNamespace));}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("WallBuilderBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,WallBuilderContract.TypeId);var element=resolved.GetElementsByTagName("WallBuilderBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(WallBuilderBehaviorModuleData),WallBuilderContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,WallBuilderContract.Wire,typeof(WallBuilderBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(WallBuilderBehaviorModuleData),proof.Records);checks++;
                // Reborn: one authored expanded-wall variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted wall segment count must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<WallBuilderBehaviorModuleData>(nameof(WallBuilderBehaviorModuleData.MaxNumSegments)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(WallBuilderBehaviorModuleData),WallBuilderContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt wall-builder payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: WallBuilderBehavior: PASS ({checks} original/39-segments/flags/filter/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
