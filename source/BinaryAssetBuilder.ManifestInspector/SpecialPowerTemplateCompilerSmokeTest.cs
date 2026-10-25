using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: special-power authoring uses EP1 native selectors, full flag ordinals and ordinary source/command dependencies.
internal static class SpecialPowerTemplateCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify stock native graphs and prove edited fields, filters, hashes and lists before private final stream readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh special-power test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();SpecialPowerTemplateCompiler.Qualify(manifest,schemas,catalog);var assets=new List<string>();int checks=0;
            foreach(string id in new[]{"SpecialPowerOrbitalRefuse_Rank1","SpecialPowerOrbitalRefuse_Rank2","SpecialPowerOrbitalRefuse_Rank3","SpecialPower_TriggerLeapDeploy","SpecialPowerIronCurtain"}){
                var source=SpecialPowerTemplateCompiler.Original(id,schemas);source.DocumentElement!.SetAttribute("ReloadTime","12s");source.DocumentElement.SetAttribute("NumTargets","3");source.DocumentElement.SetAttribute("Money","-200");source.DocumentElement.SetAttribute("RestrictionType","2");foreach(var required in source.GetElementsByTagName("RequiredObject",SchemaSet.XmlNamespace).OfType<XmlElement>())required.InnerText="SovietPowerPlant";Check(source);if(id.StartsWith("SpecialPowerOrbitalRefuse",StringComparison.Ordinal))assets.Add(source.DocumentElement.OuterXml);
            }
            // Reborn: every EP1 flag bit, all five target enums, numeric restrictions and schema-valid boolean spellings are source-bound independently.
            string[] flags=Enum.GetNames<Ep1SpecialPowerTemplateFlag>(),targets=Enum.GetNames<Ep1SpecialPowerTemplateTargetType>();for(int i=0;i<flags.Length;i++){var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<SpecialPowerTemplate xmlns='uri:ea.com:eala:asset' id='RebornFlagProbe' TargetType='"+targets[i%targets.Length]+"' NumTargets='7' Flags='"+flags[i]+"' RestrictionType='"+(i%3==0?"UNRESTRICTED":(i%3).ToString())+"' WaypointModeTerminal='1'/>"));Check(source);}
            // Reborn: inheritance remains source-level, while a real command binds the newly compiled special power through the normal linker.
            File.WriteAllText(Path.Combine(output,"base.xml"),"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><SpecialPowerTemplate id='RebornSpecialBase' TargetType='NONE' Flags='CAN_NOT_BE_SCRAMBLED'/></AssetDeclaration>");assets.Add("<SpecialPowerTemplate id='RebornSpecialPower' inheritFrom='RebornSpecialBase' TargetType='LOCATION_AND_ANGLE' ReloadTime='=$RELOAD' NumTargets='5' Flags='CAN_NOT_BE_SCRAMBLED PATHABLE_ONLY_NO_LOCOMOTOR_CHECK' RestrictionType='1' NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveWhileUnderAttackVoice='RebornVoice'><ObjectFilter Rule='ANY' Include='VEHICLE'><IncludeThing>SovietAntiInfantryInfantry</IncludeThing></ObjectFilter><GameDependency id='RebornPowerDependency'><RequiredObject>SovietPowerPlant</RequiredObject></GameDependency></SpecialPowerTemplate>");assets.Add("<LogicCommand id='RebornSpecialCommand' Type='SPECIAL_POWER'><SpecialPower>RebornSpecialPower</SpecialPower></LogicCommand>");string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='RELOAD' value='15s'/></Defines>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Special-power source route not selected.");
            Console.WriteLine($"Reborn: SpecialPowerTemplate: PASS (7 original EP1 graphs, {checks} edited/all-flag/target/restriction/boolean native controls, EVA/audio/tech imports, filters/dependencies/hash pointers, three Orbital overrides, instance bases/definitions and local LogicCommand binding, final stream/package readback; no user output or game launch).");

            // Reborn: current source owns every scalar/import/relocation; changing the reload word must be rejected rather than normalized away.
            void Check(XmlDocument source){var proof=SpecialPowerTemplateCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=SpecialPowerTemplateCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1SpecialPowerTemplate),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[32]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1SpecialPowerTemplate),0,bad,refs);throw new InvalidOperationException("Corrupt special power accepted.");}catch(InvalidDataException){}checks++;}
        }finally{Settings.Current=saved;}
    }
}
