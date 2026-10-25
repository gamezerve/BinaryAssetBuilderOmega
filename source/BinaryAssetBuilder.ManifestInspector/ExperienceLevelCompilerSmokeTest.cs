using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise source-authored experience targets, awards, ranks and real local/stock prerequisite imports.
internal static class ExperienceLevelCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare original EP1 bytes, edited values and pointer/list ownership before normal linked stream and package readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh experience test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();ExperienceLevelCompiler.Qualify(manifest,schemas,catalog);
            foreach(string id in new[]{"ExperienceLevel_Rank1","ExperienceLevel_Rank2","ExperienceLevel_StructuresLarge","AlliedAntiAirShipExperienceLevel_2"}){
                var source=ExperienceLevelCompiler.Original(id,schemas);source.DocumentElement!.SetAttribute("ExperienceAward","9876");source.DocumentElement.SetAttribute("RequiredExperience","99");source.DocumentElement.SetAttribute("ShowLevelUpTint","true");var proof=ExperienceLevelCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=ExperienceLevelCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1ExperienceLevelTemplate),0,native,refs);
                // Reborn: the required-experience word is independently tied to current source, not a captured stock constant.
                var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[8]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1ExperienceLevelTemplate),0,bad,refs);throw new InvalidOperationException("Corrupt experience payload accepted.");}catch(InvalidDataException){}
            }
            // Reborn: source-only bases and definitions feed multiple emitted levels with real local prerequisite/FX dependencies.
            File.WriteAllText(Path.Combine(output,"base.xml"),"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><ExperienceLevelTemplate id='RebornExperienceBase' Rank='1'><LevelUpTintColor r='1' g='1' b='1'/></ExperienceLevelTemplate></AssetDeclaration>");string entry=Path.Combine(output,"mod.xml");
            File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='AWARD' value='4500'/></Defines><ExperienceLevelTemplate id='RebornExperience1' inheritFrom='RebornExperienceBase' ExperienceAward='=$AWARD'>"+string.Concat(Enumerable.Repeat("<Target>SovietPowerPlant</Target>",37))+"</ExperienceLevelTemplate><ExperienceLevelTemplate id='RebornExperience2' inheritFrom='RebornExperienceBase' Rank='2' RequiredExperience='100' Prerequisite='RebornExperience1' ModelConditionState='SPECIAL_CHEERING' LevelUpOCL='"+catalog.First(a=>a.TypeName=="ObjectCreationList").Name+"'><Upgrade>Upgrade_Veterancy_VETERAN</Upgrade><LevelUpFxList FxList='RebornExperienceFX' BoneName='RebornBone'/><Target>SovietAntiInfantryInfantry</Target><AttributeModifier>Unit_Veteran</AttributeModifier></ExperienceLevelTemplate><FXList id='RebornExperienceFX'><NuggetList/></FXList></AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Experience-only source route was not selected.");
            // Reborn: a real missing source base fails instead of silently borrowing an unrelated retail native payload.
            string missing=Path.Combine(output,"missing.xml");File.WriteAllText(missing,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><ExperienceLevelTemplate id='MissingDerived' inheritFrom='MissingExperienceBase'/></AssetDeclaration>");bool rejected=false;try{ConscriptSourcePreparation.Load(missing);}catch(InvalidDataException){rejected=true;}if(!rejected)throw new InvalidDataException("Missing experience source base accepted.");
            Console.WriteLine("Reborn: ExperienceLevelTemplate: PASS (four original native graphs, edited awards/requirements/tint, optional condition/prerequisite pointers, weak targets/upgrades, strong FX/modifier/OCL imports, 37 targets, source-only instance bases/definitions, real local prerequisites and final stream/package readback; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
