using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: protocol purchase bindings retain retail import pairs and support multiple protocols granting one player upgrade.
internal static class PlayerTechUpgradeBindingCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original faction binding records, reject corrupted import lists and link a local shared-upgrade power dependency. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh protocol binding test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();PlayerTechUpgradeBindingCompiler.Qualify(global,schemas,catalog);int checks=0;
            foreach(string id in new[]{"PlayerTechUpgradeBinding_Allied","PlayerTechUpgradeBinding_Soviet","PlayerTechUpgradeBinding_Japan"})Check(PlayerTechUpgradeBindingCompiler.Original(id,schemas));
            // Reborn: three alternate purchased protocols can grant one shared player upgrade without duplicating power modules.
            string bindings=string.Concat(new[]{"PlayerTech_Allied_AirPower","PlayerTech_Allied_SatelliteSweep","PlayerTech_Allied_HighTechnology"}.Select(t=>"<Binding TechDependency='"+t+"' UpgradeToGrant='RebornProtocolThreshold'/>"));
            string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<!-- Reborn: private shared purchase-threshold source and native readback. --><AssetDeclaration xmlns='uri:ea.com:eala:asset'><PlayerTechUpgradeBinding id='RebornProtocolBindings'>"+bindings+"</PlayerTechUpgradeBinding><UpgradeTemplate id='RebornProtocolThreshold' Type='PLAYER'/><SpecialPowerTemplate id='RebornProtocolPower' TargetType='LOCATION'><GameDependency><RequiredObject>AlliedConstructionYard</RequiredObject><NeededUpgrade>RebornProtocolThreshold</NeededUpgrade></GameDependency></SpecialPowerTemplate></AssetDeclaration>");
            string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Protocol binding route not selected.");
            Console.WriteLine($"Reborn: PlayerTechUpgradeBinding: PASS (3 original EP1 native graphs, {checks} import-corruption controls, shared upgrade grants, local power dependency and stream readback; gameplay purchase behavior requires runtime testing).");

            // Reborn: current source owns every import and relocation; a damaged list count must be rejected.
            void Check(XmlDocument source){var proof=PlayerTechUpgradeBindingCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=PlayerTechUpgradeBindingCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PlayerTechUpgradeBinding),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[4]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PlayerTechUpgradeBinding),0,bad,refs);throw new InvalidOperationException("Corrupt binding list accepted.");}catch(InvalidDataException){}checks++;}
        }finally{Settings.Current=saved;}
    }
}
