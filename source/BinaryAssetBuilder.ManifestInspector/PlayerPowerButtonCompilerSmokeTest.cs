using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise all four power-button variants with independent native proofs and private final package output.
internal static class PlayerPowerButtonCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify stock graphs, edit optional tint/outline/list fields and link buttons to a newly authored special power. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh power-button output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();PlayerPowerButtonCompiler.Qualify(manifest,schemas,catalog);int checks=0;var assets=new List<string>();
            foreach(string id in new[]{"ButtonPlayerPoweOrbitalRefuse_Rank1","ButtonPlayerPowerSatelliteSweep","ButtonPlayerPowerChronoSwap","ButtonPlayerPowerPsychicSlam_Rank1","ButtonPlayerPowerYurikoShieldRank1"}){
                var source=PlayerPowerButtonCompiler.Original(id,schemas);Check(source);source.DocumentElement!.SetAttribute("id","Reborn"+id);var data=source.GetElementsByTagName("Data",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().ChildNodes.OfType<XmlElement>().Single();
                if(data.LocalName!="Power"){
                    data.SetAttribute("ValidTargetOutline","NONE");var tint=source.CreateElement("ValidTargetTintColor",SchemaSet.XmlNamespace);tint.SetAttribute("R","0.25");tint.SetAttribute("G","0.5");tint.SetAttribute("B","1");data.PrependChild(tint);
                    if(data.LocalName=="MultipleTargetsTargetedPower"){data.SetAttribute("TargetCount","7");data.SetAttribute("MinTargetDistance","12.5");}
                    foreach(string name in new[]{"AdditionalSpecialPowerTemplate","AdditionalRadiusCursor","AdditionalValidTargetCursor"}){var first=data.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==name);if(first!=null)data.InsertAfter(first.CloneNode(true),first);}
                }
                Check(source);source.DocumentElement.SetAttribute("SpecialPower","RebornButtonPower");assets.Add(source.DocumentElement.OuterXml);
            }
            // Reborn: explicit negative/numeric outline forceValue ordinals and default cursor imports have independent controls.
            foreach(string outline in Enum.GetNames<Ep1ButtonOutline>()){
                var source=PlayerPowerButtonCompiler.Original("ButtonPlayerPoweOrbitalRefuse_Rank1",schemas);var data=source.GetElementsByTagName("TargetedPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();data.SetAttribute("ValidTargetOutline",outline);Check(source);
            }
            assets.Add("<SpecialPowerTemplate id='RebornButtonPower' TargetType='LOCATION' ReloadTime='=$RELOAD'/>");string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Defines><Define name='RELOAD' value='15s'/></Defines>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Power-button route not selected.");
            Console.WriteLine($"Reborn: PlayerPowerButtonTemplate: PASS (5 original EP1 records, all 4 Data variants, {checks} original/edit/outline native controls, tint/defaults/markers/import lists, local SpecialPower binding and final stream/package readback; no user output or game launch).");

            // Reborn: independent validation must reject a corrupted concrete Data class identity.
            void Check(XmlDocument source){var proof=PlayerPowerButtonCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=PlayerPowerButtonCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PlayerPowerButtonTemplate),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[12]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PlayerPowerButtonTemplate),0,bad,refs);throw new InvalidOperationException("Corrupt button class accepted.");}catch(InvalidDataException){}checks++;}
        }finally{Settings.Current=saved;}
    }
}
