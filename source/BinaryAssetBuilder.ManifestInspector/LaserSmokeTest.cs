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

// Reborn: cover original laser draw/state records, schema defaults, authored edits and full object composition without editing user sources.
internal static class LaserSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete source/native/relocation/metadata checks and a corrupt-output refusal through the existing production EP1 dispatcher. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            baseline.DocumentElement!.SetAttribute("BuildTime","4");
            _=LaserDrawContract.Qualify(manifest,baseline,schemas,catalog);_=LaserStateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            var ns=SchemaSet.XmlNamespace;var draws0=GameObjectExtendedRootContract.Original("Soviet/Units/SovietCommandoTech1.xml","LaserDraw");var states0=GameObjectExtendedRootContract.Original("Soviet/Units/SovietCommandoTech1.xml","LaserState");
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var draws=source.GetElementsByTagName("Draws",ns).OfType<XmlElement>().Single();draws.RemoveAll();var behaviors=source.GetElementsByTagName("Behaviors",ns).OfType<XmlElement>().Single();
                // Reborn: 0 = both original Natasha lasers; 1 = every EP1 field edited; 2 = schema defaults with only the required shader.
                if(variant==0){foreach(var d in draws0)draws.AppendChild(source.ImportNode(d,true));foreach(var st in states0)behaviors.AppendChild(source.ImportNode(st,true));}
                if(variant==1){var d=(XmlElement)draws.AppendChild(source.ImportNode(draws0[0],true))!;d.SetAttribute("id","RebornLaserEdited");d.SetAttribute("FadeInTime","0.25s");d.SetAttribute("FadeOutTime","0.5s");d.SetAttribute("WeaponSlotID","1");d.SetAttribute("LaserStateID","7");d.SetAttribute("LaserWidth","2.5");d.SetAttribute("UseDistortionShader","true");
                    var v=source.CreateElement("ObjectStatusValidation",ns);v.SetAttribute("RequiredStatus","DESOLATION_ACTIVE");v.SetAttribute("ForbiddenStatus","GENERIC_TOGGLE_STATE");d.AppendChild(v);
                    var st=source.CreateElement("LaserState",ns);st.SetAttribute("id","RebornLaserStateEdited");st.SetAttribute("LaserId","7");st.SetAttribute("OriginBoneName","FX_Laser");st.SetAttribute("RequiresWeapon","false");
                    st.InnerXml="<LaserEndParticleSystem xmlns='"+ns+"'>Allied_Target_LaserEnd</LaserEndParticleSystem><LaserStartParticleSystem xmlns='"+ns+"'>Allied_Target_LaserEnd</LaserStartParticleSystem><EndOffset xmlns='"+ns+"' x='1' y='-2' z='3.5'/><ObjectStatusValidation xmlns='"+ns+"' RequiredStatus='DESOLATION_ACTIVE'/>";behaviors.AppendChild(st);}
                if(variant==2){var d=source.CreateElement("LaserDraw",ns);d.SetAttribute("id","RebornLaserDefaults");d.InnerXml="<FXShader xmlns='"+ns+"' ShaderName='Laser.fx'/>";draws.AppendChild(d);var st=source.CreateElement("LaserState",ns);st.SetAttribute("id","RebornLaserStateDefaults");behaviors.AppendChild(st);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: Laser: PASS ({checks} complete original/edited/default graphs; LaserDraw fades, shader constants, status validation; LaserState bones, particle lists, end offsets, status validation; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
