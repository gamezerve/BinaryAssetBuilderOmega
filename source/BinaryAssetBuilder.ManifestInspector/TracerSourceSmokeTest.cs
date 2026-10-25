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

// Reborn: exercise arbitrary tracer source values and optional status records in complete private GameObject graphs.
internal static class TracerSourceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original Sentry Gun tracers and edited colors, references and masks through production dispatch and metadata composition. */
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
            // Reborn: the original building has no ObjectStatusValidation; its complete stock graph anchors that optional-absence case independently.
            var original=GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBaseDefenseGround.xml","TracerModelDraw").Single();var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietBaseDefenseGround");var stockModule=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Single(m=>m.TypeId==0x54018E60u);
            // Reborn: schema validation uses the parent GameObject declaration, retaining the official tracer type rather than adding a source-only alias.
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var draws=source.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();var owner=(XmlElement)draws.AppendChild(source.ImportNode(original,true))!;
                if(variant==1){owner.SetAttribute("id","RebornTracerEdited");owner.SetAttribute("Width","12");owner.SetAttribute("FrameLifeTime","123");owner.SetAttribute("TracerHitFx","FX_NONE");owner.SetAttribute("TracerEmitFx","FX_NONE");owner.SetAttribute("WeaponSlotID","3");owner.SetAttribute("UseAdditiveBlending","false");var status=source.CreateElement("ObjectStatusValidation",SchemaSet.XmlNamespace);status.SetAttribute("RequiredStatus","WEAPON_UPGRADED_01");status.SetAttribute("ForbiddenStatus","GENERIC_TOGGLE_STATE");owner.AppendChild(status);owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="TailColor").SetAttribute("a","0.5");}
                // Reborn: multiple tracers with different local identities preserve their own optional pointer and dependency occurrences.
                if(variant==2){var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornTracerSecond");second.SetAttribute("Texture","FXTracerHeroic");second.SetAttribute("SweepSpeed","0.25");draws.AppendChild(second);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var element=resolved.GetElementsByTagName("TracerModelDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().First();if(variant==0)SchemaNativeContract.VerifyStock(element,typeof(W3DTracerDrawModuleData),0x54018E60u,captured.Bytes,stockModule.Native,captured.Asset.References.ToArray());
                var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: source-driven TracerModelDraw: PASS ({checks} complete original/edit/multiple-tracer graphs, original no-status stock bytes, optional status presence, FX/texture imports, colors and metadata composition; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
