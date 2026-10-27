using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise root object-specific voices through real source admission, native compilation and final GameObject metadata verification.
internal static class ObjectSpecificVoiceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: cover the retail Tesla voice, all six ordinals, repeated/empty lists and retargeting without changing editable mod sources. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();
            var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));baseline.DocumentElement!.SetAttribute("BuildTime","4");
            _=GameObjectExtendedRootContract.Qualify("AudioArrayVoice",manifest,baseline,schemas,catalog);
            var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);int checks=0;
            var original=GameObjectExtendedRootContract.Original("Soviet/Units/SovietHeavyAntiVehicleInfantry.xml","AudioArrayVoice").Single(e=>e.ParentNode is XmlElement parent&&parent.LocalName=="GameObject");
            for(int variant=0;variant<4;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var owner=GameObjectExtendedRootContract.Owners(source,"AudioArrayVoice").Single();owner.ParentNode!.ReplaceChild(source.ImportNode(original,true),owner);owner=GameObjectExtendedRootContract.Owners(source,"AudioArrayVoice").Single();
                // Reborn: source-sized lists and schema-valid weak targets must survive independently of the captured Tesla cardinality.
                if(variant==1){var entry=owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="ObjectSpecificEntry");foreach(string ordinal in Enum.GetNames<ThingTemplateObjectSpecificVoiceType>()){var row=(XmlElement)entry.CloneNode(true);row.SetAttribute("AudioType",ordinal);row.SetAttribute("TargetObject","SovietAntiInfantryInfantry");owner.AppendChild(row);}}
                if(variant==2)foreach(var entry in owner.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ObjectSpecificEntry").ToArray())owner.RemoveChild(entry);
                if(variant==3)owner.RemoveAll();
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine("EP1 object-specific root voices: PASS (retail native/metadata witness; "+checks+" full objects, all six ordinals, retargeted/repeated/empty lists; no game launch).");
        }finally{Settings.Current=saved;}
    }
}
