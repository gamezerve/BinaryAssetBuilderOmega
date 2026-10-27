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

// Reborn: verify source-driven EP1 stream draws in complete private GameObject graphs.
internal static class StreamDrawSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original Desolator stream draws and edited scalars, shader constants and particle references through production dispatch and metadata composition. */
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
            // Reborn: the stock stream supplies shader texture and particle imports; edits cover every EP1 scalar and all shader constant kinds.
            var original=GameObjectExtendedRootContract.Original("EP1/Soviet/Units/SovietDesolatorInfantry.xml","StreamDraw").Single();
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietDesolatorInfantry");var stockModule=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Single(m=>m.TypeId==StreamDrawContract.TypeId);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var draws=source.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();var owner=(XmlElement)draws.AppendChild(source.ImportNode(original,true))!;
                if(variant==1){owner.SetAttribute("id","RebornStreamEdited");owner.SetAttribute("Velocity","125");owner.SetAttribute("OffsetMult","0.75");owner.SetAttribute("ArcHeightFactor","0.5");owner.SetAttribute("UVWorldSize","50");owner.SetAttribute("NumTubeSides","6");owner.SetAttribute("TubeRadius","2.5");owner.SetAttribute("HitFx","FX_NONE");owner.SetAttribute("WeaponSlotID","2");owner.SetAttribute("StreamStateID","3");owner.SetAttribute("UseDistortionShader","true");
                    var shader=owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="FXShader");shader.SetAttribute("TechniqueName","RebornStream");shader.SetAttribute("TechniqueIndex","1");var constants=shader.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Constants");constants.InnerXml+="<Float xmlns='uri:ea.com:eala:asset' Name='RebornVector'><Value>1.25</Value><Value>-2</Value></Float><Int xmlns='uri:ea.com:eala:asset' Name='RebornInt'><Value>7</Value></Int><Bool xmlns='uri:ea.com:eala:asset' Name='RebornBool'><Value>true</Value></Bool>";}
                // Reborn: omitted scalars use official defaults; absent and allocated-empty optional particle headers remain distinct.
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornStreamDefaults"+variant);owner.InnerXml="<FXShader xmlns='uri:ea.com:eala:asset' ShaderName='Stream.fx'/>";if(variant==3){owner.InnerXml+="<ParticleSystems xmlns='uri:ea.com:eala:asset'/>";var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornSecondStream");draws.AppendChild(second);}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var element=resolved.GetElementsByTagName("StreamDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                if(variant==0)SchemaNativeContract.VerifyStock(element,typeof(Ep1W3DStreamDrawModuleData),StreamDrawContract.TypeId,captured.Bytes,stockModule.Native,captured.Asset.References.ToArray());
                var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: StreamDraw: PASS ({checks} complete original/edit/default/multiple-draw graphs; OffsetMult, four shader constant types, FX/texture/particle imports, absent/empty particle lists; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
