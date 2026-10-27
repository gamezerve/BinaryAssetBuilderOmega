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

// Reborn: cover complete Uprising death phases, typed imports, literal audio lists, masks and defaults and full object composition without editing user sources.
internal static class SlowDeathSmokeTest
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
            var grammar=SlowDeathContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/SovietScoutInfantry.xml","SlowDeath").First(),true);behaviors.AppendChild(owner);
                // Reborn: exercise every own field plus all four phases, typed FX/OCL/weapon imports, multiple audio names and full death conditions.
                if(variant==1){
                    owner.SetAttribute("id","RebornSlowDeath");
                    foreach(var pair in new[]{("SinkRate","1.25"),("ProbabilityModifier","-3"),("ModifierBonusPerOverkillPercent","25%"),("SinkDelay","1s"),("SinkDelayVariance","0.2s"),("DestructionDelay","9.5s"),("DestructionDelayVariance","2s"),("DecayBeginTime","1.25s"),("FlingForce","10"),("FlingForceVariance","2"),("FlingPitch","45d"),("FlingPitchVariance","5d"),("DeathFlags","DEATH_1"),("FadeTime","0.75s"),("FadeDelay","0.5s"),("DeathTypes","DEATH_2"),("DeathObjectStatusBits","NO_BRIBE"),("ShadowWhenDead","true"),("Fade","true")})owner.SetAttribute(pair.Item1,pair.Item2);
                    string fx=catalog.First(a=>a.TypeName=="FXList").Name,ocl=catalog.First(a=>a.TypeName=="ObjectCreationList").Name,weapon=catalog.First(a=>a.TypeName=="WeaponTemplate").Name;
                    owner.InnerXml=string.Concat(Enum.GetNames<SlowDeathPhaseType>().Select(phase=>"<FX xmlns='uri:ea.com:eala:asset' Type='"+phase+"'>"+string.Concat(Enumerable.Repeat("<FX>"+fx+"</FX>",5))+"</FX>"));
                    owner.InnerXml+=string.Concat(Enum.GetNames<SlowDeathPhaseType>().Select(phase=>"<OCL xmlns='uri:ea.com:eala:asset' Type='"+phase+"'><OCL>"+ocl+"</OCL><OCL>"+ocl+"</OCL></OCL>"));
                    owner.InnerXml+=string.Concat(Enum.GetNames<SlowDeathPhaseType>().Select(phase=>"<Weapon xmlns='uri:ea.com:eala:asset' Type='"+phase+"'><Weapon>"+weapon+"</Weapon><Weapon>"+weapon+"</Weapon></Weapon>"));
                    owner.InnerXml+=string.Concat(Enum.GetNames<SlowDeathPhaseType>().Select(phase=>"<Sound xmlns='uri:ea.com:eala:asset' Type='"+phase+"' List='SOV_Bear_VoiceDieOnLand SOV_Bear_VoiceDieInWater'/>"));
                    owner.InnerXml+="<DieMuxData xmlns='uri:ea.com:eala:asset' DeathTypes='ALL' DeathTypesForbidden='ELECTROCUTED' RequiredStatus='OVER_WATER' ExemptStatus='UNDER_CONSTRUCTION'/>";
                }
                // Reborn: omitted optional fields and present empty masks/lists remain distinct native ownership states.
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornDeathDefaults");if(variant==3){foreach(string name in new[]{"DeathFlags","DeathTypes","DeathObjectStatusBits"})owner.SetAttribute(name,"");owner.InnerXml="<DieMuxData xmlns='uri:ea.com:eala:asset'/><Sound xmlns='uri:ea.com:eala:asset' Type='INITIAL' List=''/><Weapon xmlns='uri:ea.com:eala:asset' Type='FINAL'/><OCL xmlns='uri:ea.com:eala:asset' Type='MIDPOINT'/><FX xmlns='uri:ea.com:eala:asset' Type='HIT_GROUND'/>";}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("SlowDeath",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,SlowDeathContract.TypeId);var element=resolved.GetElementsByTagName("SlowDeath",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(SlowDeathBehaviorModuleData),SlowDeathContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,SlowDeathContract.Wire,typeof(SlowDeathBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(SlowDeathBehaviorModuleData),proof.Records);checks++;
                // Reborn: authored death variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted radius must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<SlowDeathBehaviorModuleData>(nameof(SlowDeathBehaviorModuleData.SinkRate)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(SlowDeathBehaviorModuleData),SlowDeathContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt death payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: all seven Bear death modules compile together without matching their IDs or conditions to Conscript templates.
            var bear=(XmlDocument)baseline.CloneNode(true);var bearBehaviors=bear.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();bearBehaviors.RemoveAll();foreach(var leaf in GameObjectExtendedRootContract.Original("Soviet/Units/SovietScoutInfantry.xml","SlowDeath"))bearBehaviors.AppendChild(bear.ImportNode(leaf,true));var bearPlan=ConscriptVariableBehaviorSource.Admit(bear,baseline,official);var bearResolved=SchemaNativeContract.Resolve(bear,schemas,catalog);var bearInstance=ReferencePipelineSmokeTest.Normalize(schemas,bearResolved.OuterXml);var bearNative=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(bearInstance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(bear,bearNative,bearInstance,bearPlan.Origins);checks++;
            // Reborn: XSD phase-family cardinality remains four, and genuine missing typed FX references still reject.
            foreach(bool excess in new[]{true,false}){var invalid=(XmlDocument)baseline.CloneNode(true);var list=invalid.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();list.RemoveAll();var leaf=invalid.CreateElement("SlowDeath",SchemaSet.XmlNamespace);leaf.SetAttribute("id","RebornBadDeath");list.AppendChild(leaf);leaf.InnerXml=excess?string.Concat(Enumerable.Repeat("<FX xmlns='uri:ea.com:eala:asset' Type='INITIAL'/>",5)):"<FX xmlns='uri:ea.com:eala:asset' Type='INITIAL'><FX>RebornMissingDeathFX</FX></FX>";try{SchemaNativeContract.Resolve(invalid,schemas,catalog);throw new InvalidOperationException("Invalid death phase/reference source accepted.");}catch(InvalidDataException){checks++;}}
            Console.WriteLine($"Reborn: SlowDeath: PASS ({checks} all-death-fields/four-phases/typed-imports/audio-lists/masks/default/empty/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
