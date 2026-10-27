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

// Reborn: cover invisibility behavior stock imports, authored template imports, defaults and full object composition without editing user sources.
internal static class InvisibilityBehaviorSmokeTest
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
            var grammar=InvisibilityBehaviorContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBunker.xml","InvisibilityBehavior").Single(),true);behaviors.AppendChild(owner);
                // Reborn: preserve the retail target, permit arbitrary compatible catalog targets and qualified identities, then compose with the actual bunker garrison.
                if(variant==1){owner.SetAttribute("id","RebornInvisibility");owner.SetAttribute("InvisibilityTemplate",catalog.First(a=>a.TypeName=="InvisibilityTemplate"&&a.Name!="InvisibilityTemplate:"+owner.GetAttribute("InvisibilityTemplate")).Name.Split(':')[1]);}
                if(variant==2)owner.SetAttribute("InvisibilityTemplate","InvisibilityTemplate:"+owner.GetAttribute("InvisibilityTemplate"));
                if(variant==3)behaviors.PrependChild(source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBunker.xml","GarrisonContain").Single(),true));
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("InvisibilityBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,InvisibilityBehaviorContract.TypeId);var element=resolved.GetElementsByTagName("InvisibilityBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(InvisibilityBehaviorModuleData),InvisibilityBehaviorContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,InvisibilityBehaviorContract.Wire,typeof(InvisibilityBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(InvisibilityBehaviorModuleData),proof.Records);checks++;
                // Reborn: authored invisibility behavior variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted template import must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<InvisibilityBehaviorModuleData>(nameof(InvisibilityBehaviorModuleData.InvisibilityTemplate)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(InvisibilityBehaviorModuleData),InvisibilityBehaviorContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt invisibility behavior payload accepted.");}catch(InvalidDataException){checks++;}
                if(variant==1){
                    // Reborn: retain genuine missing-reference and required-attribute errors instead of silently accepting a null template.
                    var missing=(XmlDocument)source.CloneNode(true);var leaf=missing.GetElementsByTagName("InvisibilityBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();leaf.SetAttribute("InvisibilityTemplate","RebornMissingInvisibilityTemplate");
                    try{SchemaNativeContract.Resolve(missing,schemas,catalog);throw new InvalidOperationException("Missing invisibility-template reference accepted.");}catch(InvalidDataException){checks++;}
                    leaf.RemoveAttribute("InvisibilityTemplate");try{SchemaNativeContract.Resolve(missing,schemas,catalog);throw new InvalidOperationException("Required invisibility-template attribute omitted without error.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: InvisibilityBehavior: PASS ({checks} retail/retarget/qualified-reference/garrison-pair/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
