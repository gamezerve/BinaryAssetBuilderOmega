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

// Reborn: verify scripted model-state turret lists privately with edited strings, angles, IDs and defaults.
internal static class ScriptedTurretSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: complete source/native/metadata composition must preserve original, multiple and default turret records. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));baseline.DocumentElement!.SetAttribute("BuildTime","4");
            var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);int checks=0;
            var original=GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBaseDefenseGround.xml","Turret").Single();
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var draws=source.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var draw=draws.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="ScriptedModelDraw");foreach(var other in draws.ChildNodes.OfType<XmlElement>().Where(e=>e!=draw).ToArray())draws.RemoveChild(other);
                // Reborn: isolate this draw feature from unrelated behavior qualification while retaining the complete GameObject allocator and linker.
                var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();foreach(var other in behaviors.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName!="StancesBehavior").ToArray())behaviors.RemoveChild(other);
                var state=draw.ChildNodes.OfType<XmlElement>().First(e=>e.LocalName=="ModelConditionState");var turret=(XmlElement)state.AppendChild(source.ImportNode(original,true))!;
                if(variant==1){turret.SetAttribute("TurretNameKey","RebornTurret");turret.SetAttribute("TurretPitch","RebornPitch");turret.SetAttribute("TurretArtAngle","35d");turret.SetAttribute("TurretArtPitch","-12d");turret.SetAttribute("TurretID","2");var second=(XmlElement)turret.CloneNode(true);second.SetAttribute("TurretNameKey","SecondTurret");second.SetAttribute("TurretID","3");state.AppendChild(second);}
                if(variant==2)turret.RemoveAll();
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: scripted draw Turret: PASS ({checks} complete original/edit/multiple/default graphs, original Sentry Gun native and metadata witness, string/angle/ID ownership; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
