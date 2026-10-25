using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify general FXList source/native compilation and mixed-stream publication privately, without editing live mod sources.
internal static class FXListCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use real Core references, original stock witnesses, edited multi-nugget lists and final stream readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh FXList test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();FXListCompiler.Qualify(manifest,schemas,catalog);int count=0;
            var bodies=new[]{"<NuggetList/>","<NuggetList>"+string.Concat(Enumerable.Repeat("<Sound Value='RA3TinyBuildingDie' StopIfPlayed='true'><SourceObjectFilter Rule='ANY'><IncludeThing>AlliedAntiInfantryInfantry</IncludeThing></SourceObjectFilter></Sound>",4))+"<ParticleSystem Particle='Civ_Destruction_Tiny'><Offset z='7'/></ParticleSystem><LightPulse Radius='50'><Color R='1' G='0.5' B='0.25'/></LightPulse><ViewShake Type='SUBTLE'/><DynamicDecal Size='12'><Color R='255' G='128' B='64'/></DynamicDecal><RayEffect/><Laser/><CameraShakerVolume/><AttachedModel/><TerrainScorch Type='TERRAIN_MAPPED'/><TintDrawable/><FXParticleSysBone/><EvaEvent/><ForceFeedback/></NuggetList>"};var assets=new List<string>();
            foreach(string body in bodies){var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<FXList xmlns='uri:ea.com:eala:asset' id='RebornFX"+count+"'>"+body+"</FXList>"));// Reborn: nonempty EVA and ForceFeedback fields must produce real selectors, not only zero-filled default records.
                foreach(var eventNode in source.GetElementsByTagName("EvaEvent",SchemaSet.XmlNamespace).OfType<XmlElement>())eventNode.SetAttribute("EvaEventOwner",catalog.First(a=>a.TypeName=="EvaEvent").Name);
                foreach(var feedback in source.GetElementsByTagName("ForceFeedback",SchemaSet.XmlNamespace).OfType<XmlElement>())feedback.SetAttribute("Event",catalog.First(a=>a.TypeName=="ForceFeedbackEvent").Name);
                var proof=FXListCompiler.ProofSource(source,schemas,catalog);var normalized=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,normalized.OuterXml);var native=FXListCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(FXList),0,native,refs);
                // Reborn: the current source proof must reject a changed root scalar even when all import selectors remain valid.
                var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[4]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(FXList),0,bad,refs);throw new InvalidOperationException("Corrupt FXList root accepted.");}catch(InvalidDataException){}
                assets.Add(source.DocumentElement!.OuterXml);count++;
            }
            // Reborn: preserve local FX-to-FX references and emit FX-only mods through ordinary source preparation and the production mixed linker.
            assets.Add("<FXList id='RebornFX2'><NuggetList><FXListAtBonePos FX='RebornFX0' Bone='NONE'/></NuggetList></FXList>");// Reborn: a source-only instance base and local definition exercise the same path used by original inherited EA FX lists.
            File.WriteAllText(Path.Combine(output,"base.xml"),"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><FXList id='RebornFXBase'><NuggetList/></FXList></AssetDeclaration>");
            assets.Add("<FXList id='RebornFXInherited' inheritFrom='RebornFXBase' PlayEvenIfShrouded='true'><NuggetList><LightPulse Radius='=$FX_RADIUS'/></NuggetList></FXList>");
            string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='FX_RADIUS' value='50'/></Defines>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);string built=Path.Combine(output,"built");if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,built))throw new InvalidDataException("FX-only production route was not selected.");
            Console.WriteLine("Reborn: general FXList: PASS (8 original stock native records, all 15 nugget kinds, nonempty EVA/ForceFeedback imports, corruption rejection, FX-only instance/include/definition/inheritance preparation, local FX dependency and stream/package readback; no user mod publication or game launch).");
        }finally{Settings.Current=saved;}
    }
}
