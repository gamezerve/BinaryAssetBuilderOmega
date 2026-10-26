using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: player-tech authoring uses EP1 native selectors, full flag ordinals and ordinary source/command dependencies.
internal static class PlayerTechCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original global technologies, edited costs/lists and local tech/power dependencies before private stream readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh player-tech test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();PlayerTechCompiler.Qualify(global,schemas,catalog);int checks=0;
            foreach(string id in new[]{"PlayerTech_Allied","PlayerTech_Allied_AirPower","PlayerTech_Generic_Rank_3","PlayerTech_Yuriko_Shield_Rank2","PlayerTech_Yuriko_Shield_Rank3"}){var source=PlayerTechCompiler.Original(id,schemas);Check(source);source.DocumentElement!.SetAttribute("TechPointsRequired","7");source.DocumentElement.SetAttribute("GrantFX","FX_YurikoPowerUp");Check(source);}
            Check(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<PlayerTech xmlns='uri:ea.com:eala:asset' id='RebornEmptyTech'/>")));Check(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<PlayerTech xmlns='uri:ea.com:eala:asset' id='RebornMaxCostTech' TechPointsRequired='4294967295'/>")));
            // Reborn: source-sized lists retain repeated import occurrences and exceed the small retail examples without truncation.
            Check(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<PlayerTech xmlns='uri:ea.com:eala:asset' id='RebornLongTech'>"+string.Concat(Enumerable.Repeat("<TechDependency>PlayerTech_Generic_Rank_3</TechDependency>",37))+"</PlayerTech>")));
            File.WriteAllText(Path.Combine(output,"base.xml"),"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><PlayerTech id='RebornTech_Rank1' TechPointsRequired='9' GrantFX='FX_YurikoPowerUp'/></AssetDeclaration>");
            // Reborn: reverse declaration order proves mod+stock dependency resolution and same-ID instance inheritance with definitions.
            var assets=new List<string>{"<PlayerTech id='RebornTech_Rank3' TechPointsRequired='3'><TechDependency>RebornTech_Rank2</TechDependency><TechDependency>PlayerTech_Generic_Rank_5</TechDependency></PlayerTech>","<PlayerTech id='RebornTech_Rank2' TechPointsRequired='2'><TechDependency>RebornTech_Rank1</TechDependency><TechDependency>PlayerTech_Generic_Rank_4</TechDependency></PlayerTech>","<PlayerTech id='RebornTech_Rank1' inheritFrom='RebornTech_Rank1' TechPointsRequired='=$COST'><TechDependency>PlayerTech_Allied_PrecisionStrike</TechDependency><TechDependency>PlayerTech_Generic_Rank_3</TechDependency></PlayerTech>"};
            string techOnly=Path.Combine(output,"tech-only.xml");File.WriteAllText(techOnly,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='COST' value='1'/></Defines>"+string.Join("",assets)+"</AssetDeclaration>");string globalPrepared=Path.Combine(output,"global-prepared");ConscriptSourceInput.Prepare(techOnly,globalPrepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(globalPrepared,"game.xml"),manifest,global,Path.Combine(output,"global-built"),earlyGlobal:true))throw new InvalidDataException("Player-tech global route not selected.");
            assets.Add("<SpecialPowerTemplate id='RebornTechPower' TargetType='LOCATION' RequiredPlayerTech='RebornTech_Rank3'><ForbiddenPlayerTech>RebornTech_Rank1</ForbiddenPlayerTech><ForbiddenPlayerTech>RebornTech_Rank2</ForbiddenPlayerTech></SpecialPowerTemplate>");string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='COST' value='1'/></Defines>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Player-tech static route not selected.");
            Console.WriteLine($"Reborn: PlayerTech: PASS (5 original global EP1 graphs, {checks} native controls, default/uint-max cost/weak FX/37-entry dependency list, reverse mod+stock tech chain, definitions and same-ID instance base, Required/ForbiddenPlayerTech consumers, global/static stream/package readback; no user output or game launch).");

            // Reborn: current source owns every scalar/import/relocation; changing the tech-point cost must be rejected.
            void Check(XmlDocument source){var proof=PlayerTechCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=PlayerTechCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PlayerTech),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[4]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PlayerTech),0,bad,refs);throw new InvalidOperationException("Corrupt player-tech cost accepted.");}catch(InvalidDataException){}checks++;}
        }finally{Settings.Current=saved;}
    }
}
