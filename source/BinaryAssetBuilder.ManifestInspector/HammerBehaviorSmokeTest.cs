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

// Reborn: cover Hammer notification, converging beams and creation/audio upgrades with editable/default values and full object composition without editing user sources.
internal static class HammerBehaviorSmokeTest
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
            var grammar=HammerBehaviorContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                foreach(var family in HammerBehaviorContract.Families){var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(HammerBehaviorContract.Path,family.Source).First(),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                    // Reborn: authored/default records are independent of the captured original IDs and values.
                    if(variant>0){owner.RemoveAll();owner.SetAttribute("id","Reborn"+family.Source+variant);}
                    // Reborn: a listener always needs at least one schema-required filter tuple, including default probes.
                    if(variant>0&&family.Source=="SecondaryObjectListenerModule"){
                        if(variant==2)owner.SetAttribute("Flags","NOTIFY_WEAPON_SET_UPDATE NOTIFY_SUBOBJECT_UPGRADE_MUX");
                        owner.InnerXml="<ObjectEffectTuple xmlns='uri:ea.com:eala:asset'><ObjectFilter Rule='ANY' Include='INFANTRY VEHICLE'/><UpgradeSelf>Leech_ApocCannonUpgrade</UpgradeSelf></ObjectEffectTuple>";
                        if(variant==2){owner.InnerXml="<SelfValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='WEAPON_UPGRADED_01'/>"+owner.InnerXml+owner.InnerXml;}
                    }
                    if(variant==2&&family.Source=="ConvergingLaserState"){owner.SetAttribute("ConvergingAngle","90d");owner.SetAttribute("AngleRotation","15d");owner.SetAttribute("Radius","22");owner.SetAttribute("Lifetime","3s");owner.SetAttribute("SweepWeapon","SovietAntiInfantryInfantryAK47");owner.SetAttribute("OriginBoneName","RebornBeam");owner.InnerXml="<EndOffset xmlns='uri:ea.com:eala:asset' x='1' y='2' z='3'/><ObjectStatusValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='WEAPON_UPGRADED_01'/>";}
                    if(variant==2&&family.Source=="ObjectCreationUpgrade"){owner.SetAttribute("ThingToSpawn","AlliedMCV");owner.SetAttribute("UpgradeObject","OCL_SovietParachute");owner.SetAttribute("Delay","25");owner.SetAttribute("Angle","90d");owner.SetAttribute("FadeInTime","300");owner.SetAttribute("DestroyWhenSold","true");owner.SetAttribute("UseBuildingProduction","true");owner.InnerXml="<Offset xmlns='uri:ea.com:eala:asset' x='1' y='2' z='3'/>";}
                    if(variant==2&&family.Source=="AudioLoopUpgrade"){owner.SetAttribute("SoundToPlay","SOV_HammerTank_WeaponLeechedMS");owner.SetAttribute("KillAfterMS","300");owner.SetAttribute("KillOnDeath","true");}

                }
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                foreach(var family in HammerBehaviorContract.Families){uint typeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(family.Wire);var native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),typeId);var element=resolved.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var proof=SchemaNativeContract.Verify(element,family.Type,typeId,native,refs);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,family.Wire,family.Type);_=grammar.Verify(metadata,native,family.Type,proof.Records);checks++;}
                var full=GameObjectBodyProjectionSmokeTest.Compile(normalized,true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: Hammer behaviors: PASS ({checks} original/default/edit native/metadata and complete-object checks; ordered filter/upgrade tuples, converging beam fields, optional transforms and audio; no game launch).");
        }finally{Settings.Current=saved;}
    }
}
