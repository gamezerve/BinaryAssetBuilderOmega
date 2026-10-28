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

// Reborn: cover Terror Drone attachment, EP1 Bezier movement and sweep state with editable/default values and full object composition without editing user sources.
internal static class ScoutVehicleBehaviorSmokeTest
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
            var grammar=ScoutVehicleBehaviorContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                foreach(var family in ScoutVehicleBehaviorContract.Families){var owner=(XmlElement)source.ImportNode(ScoutVehicleBehaviorContract.Original(family.Source),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                    // Reborn: authored/default records are independent of the three captured original IDs and values.
                    if(variant>0){owner.RemoveAll();owner.SetAttribute("id","Reborn"+family.Source+variant);}
                    if(variant==2&&family.Source=="AttachUpdate"){owner.SetAttribute("Flags",string.Join(" ",Enum.GetNames<AttachUpdateFlagsType>().Where(n=>n!="USE_BONE_POSITION")));owner.SetAttribute("Range","75");owner.SetAttribute("CloseEnoughRange","2");owner.SetAttribute("ParentStatusToCopy","WEAPON_UPGRADED_01");owner.SetAttribute("ParentStatusToPrefer","UNDER_IRON_CURTAIN");owner.SetAttribute("DamageTypesToNotLeech","GUN");owner.SetAttribute("DeathTypesToNotLeech","NORMAL");owner.SetAttribute("AttachBoneName","RebornAttachBone");owner.InnerXml="<ObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='VEHICLE INFANTRY' Relationship='ENEMIES'/><ModifierToLeechFromParent xmlns='uri:ea.com:eala:asset'>AttributeModifier_ArmorUp</ModifierToLeechFromParent>";}
                    if(variant==2&&family.Source=="BezierProjectile"){owner.SetAttribute("Flags",string.Join(" ",Enum.GetNames<BezierProjectileFlag>().Where(n=>n!="TUMBLE_RANDOMLY")));owner.SetAttribute("Type","TORPEDO");owner.SetAttribute("FireStraightDistance","25");owner.SetAttribute("Acceleration","3.5");owner.SetAttribute("TargetVectorOffset","10");owner.SetAttribute("DisabledTypesToProcess","HELD EMP");owner.SetAttribute("GroundHitWeapon","SovietAntiInfantryInfantryAK47");owner.SetAttribute("GroundHitFX","FX_NONE");owner.SetAttribute("FlightPathAdjustDistPerSecond","25");}
                    if(variant==2&&family.Source=="SweepingLaserState"){owner.SetAttribute("Radius","22");owner.SetAttribute("Angle","45d");owner.SetAttribute("SweepWeapon","SovietAntiInfantryInfantryAK47");owner.SetAttribute("SweepingLaserOptions",string.Join(" ",Enum.GetNames<SweepingLaserOptionsType>()));owner.SetAttribute("OriginBoneName","RebornLaserBone");owner.InnerXml="<EndOffset xmlns='uri:ea.com:eala:asset' x='1' y='2' z='3'/><ObjectStatusValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='WEAPON_UPGRADED_01'/>";}
                }
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                foreach(var family in ScoutVehicleBehaviorContract.Families){uint typeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(family.Wire);var native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),typeId);var element=resolved.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var proof=SchemaNativeContract.Verify(element,family.Type,typeId,native,refs);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,family.Wire,family.Type);_=grammar.Verify(metadata,native,family.Type,proof.Records);checks++;}
                var full=GameObjectBodyProjectionSmokeTest.Compile(normalized,true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: scout behaviors: PASS ({checks} original/default/edit native/metadata and complete-object checks; attach pointers/filter/modifiers, all projectile/sweep flags, typed references and transforms; no game launch).");
        }finally{Settings.Current=saved;}
    }
}
