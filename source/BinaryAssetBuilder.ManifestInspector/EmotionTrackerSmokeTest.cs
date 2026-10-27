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

// Reborn: cover original emotion records, schema defaults, filters and optional enum pointers and full object composition without editing user sources.
internal static class EmotionTrackerSmokeTest
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
            var grammar=EmotionTrackerContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            // Reborn: three original Engineer records and fully authored/default/expanded lists exercise EP1 rather than KW semantics.
            var originals=new[]{"Soviet/Units/SovietEngineer.xml","Allied/Units/AlliedEngineer.xml","Japan/Units/JapanEngineer.xml"}.SelectMany(path=>GameObjectExtendedRootContract.Original(path,"EmotionTrackerUpdate")).ToArray();
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(originals[Math.Min(variant,2)],true);behaviors.AppendChild(owner);
                // Reborn: authored trackers own every scalar and all four filters; reordered emotion targets remain strongly typed imports.
                if(variant==3){owner.SetAttribute("id","RebornEmotions");owner.SetAttribute("TauntAndPointDistance","150");owner.SetAttribute("HeroScanDistance","200");owner.SetAttribute("TauntAndPointUpdateDelay","0.25s");owner.SetAttribute("FearScanDistance","350");owner.SetAttribute("ImmuneToFearLevel","7");owner.SetAttribute("IgnoreVeterancy","true");owner.InnerXml="<TauntAndPointExcluded xmlns='uri:ea.com:eala:asset' Rule='NONE' Include='INFANTRY'><IncludeThing>SovietEngineer</IncludeThing></TauntAndPointExcluded><AfraidOf xmlns='uri:ea.com:eala:asset' Rule='ALL' Exclude='STRUCTURE'/><AlwaysAfraidOf xmlns='uri:ea.com:eala:asset' Rule='NONE'><ExcludeThing>SovietEngineer</ExcludeThing></AlwaysAfraidOf><PointAt xmlns='uri:ea.com:eala:asset' Rule='ALL'/><AddEmotion xmlns='uri:ea.com:eala:asset'>CheerBusy_Base</AddEmotion><AddEmotion xmlns='uri:ea.com:eala:asset'>CheerIdle_Base</AddEmotion>";}
                // Reborn: the official schema allows all filters and emotions to be absent; no fixed two-emotion sample quota remains.
                if(variant==4){owner.RemoveAll();owner.SetAttribute("id","RebornEmotionDefaults");}
                if(variant==5){owner.SetAttribute("id","RebornManyEmotions");owner.InnerXml=string.Concat(Enumerable.Repeat("<AddEmotion xmlns='uri:ea.com:eala:asset'>CheerIdle_Base</AddEmotion>",8));}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("EmotionTrackerUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,EmotionTrackerContract.TypeId);var element=resolved.GetElementsByTagName("EmotionTrackerUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1EmotionTrackerUpdateModuleData),EmotionTrackerContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,EmotionTrackerContract.Wire,typeof(Ep1EmotionTrackerUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1EmotionTrackerUpdateModuleData),proof.Records);checks++;
                // Reborn: authored emotion variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 3){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted update delay must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1EmotionTrackerUpdateModuleData>(nameof(Ep1EmotionTrackerUpdateModuleData.TauntAndPointUpdateDelay)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1EmotionTrackerUpdateModuleData),EmotionTrackerContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt emotion payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: invalid schema values remain errors after replacing the former unsupported-family gate.
            foreach(string invalid in new[]{"IgnoreVeterancy='unknown'","QuarrelProbability='0.5'"}){
                var source=(XmlDocument)baseline.CloneNode(true);source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().InnerXml="<EmotionTrackerUpdate xmlns='uri:ea.com:eala:asset' id='Invalid' "+invalid+"/>";
                try{_=SchemaNativeContract.Resolve(source,schemas,catalog);throw new InvalidOperationException("Invalid emotion source accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: EmotionTrackerUpdate: PASS ({checks} three-stock/all-scalars/four-filters/empty-and-eight-emotions/strong-import/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
