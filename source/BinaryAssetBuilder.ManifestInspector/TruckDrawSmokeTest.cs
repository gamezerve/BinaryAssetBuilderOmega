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

// Reborn: verify source-driven EP1 truck draws in complete private GameObject graphs.
internal static class TruckDrawSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original vehicle truck draws and edited wheels, cab fields and scrolling through production dispatch and metadata composition. */
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
            // Reborn: original wheel/terrain data and populated scrolling each exercise the complete inherited scripted-model graph.
            for(int variant=0;variant<4;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var draws=source.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();
                string path=variant==1?"Allied/Units/AlliedAntiStructureVehicle.xml":"EP1/Soviet/Units/SovietMortarcycle.xml";var owner=(XmlElement)draws.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(path,"TruckDraw").Single(),true))!;ConscriptVariableBehaviorSource.DropLeafFormatting(owner);
                // Reborn: every wheel/cab name and scalar is authored; optional terrain imports are independent of list cardinality.
                if(variant==2){owner.SetAttribute("id","RebornTruckEdited");foreach(string field in new[]{"LeftFrontTireBone","RightFrontTireBone","LeftRearTireBone","RightRearTireBone","MidLeftFrontTireBone","MidRightFrontTireBone","MidLeftRearTireBone","MidRightRearTireBone","MidLeftMidTireBone","MidRightMidTireBone","LeftFrontTireBone2","RightFrontTireBone2","LeftRearTireBone2","RightRearTireBone2","MidLeftMidTireBone2","MidRightMidTireBone2","CabBone","TrailerBone"})owner.SetAttribute(field,"Reborn_"+field);
                    owner.SetAttribute("TireRotationMultiplier","2.5");owner.SetAttribute("TireRotationMultiplierFront","-0.5");owner.SetAttribute("PowerslideRotationAddition","3");owner.SetAttribute("CabRotationMultiplier","0.4");owner.SetAttribute("TrailerRotationMultiplier","0.6");owner.SetAttribute("RotationDamping","0.8");
                    var terrain=owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="TreadTerrainParticles");owner.RemoveChild(terrain);
                    owner.InnerXml+="<TreadScrollingMap xmlns='uri:ea.com:eala:asset' LeftTrack='RebornLeft' RightTrack='RebornRight' SpeedMultiplier='-0.25' TreadPivotSpeedFraction='75%' TreadDriveSpeedFraction='40%'/><TreadScrollingMap xmlns='uri:ea.com:eala:asset'/>";}
                // Reborn: minimal truck draws use official defaults and may coexist under separate local module IDs.
                if(variant==3){owner.RemoveAll();owner.SetAttribute("id","RebornMinimalTruck");var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornSecondTruck");draws.AppendChild(second);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var element=resolved.GetElementsByTagName("TruckDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                if(variant<2){string name=variant==0?"SovietMortarcycle":"AlliedAntiStructureVehicle";var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Single(m=>m.TypeId==TruckDrawContract.TypeId);SchemaNativeContract.VerifyStock(element,typeof(Ep1W3DTruckDrawModuleData),TruckDrawContract.TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());}
                var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: TruckDraw: PASS ({checks} complete original/edit/default/multiple-draw graphs; all wheel/cab fields, terrain imports, scrolling names/percentages and inherited scripted children; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
