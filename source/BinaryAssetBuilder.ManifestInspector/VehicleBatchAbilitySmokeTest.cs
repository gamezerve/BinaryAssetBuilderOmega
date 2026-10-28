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

// Reborn: cover vehicle abilities, grappling, storage and EP1 tank draws with editable/default values and full object composition without editing user sources.
internal static class VehicleBatchAbilitySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete original/default/edited source/native/relocation/metadata and whole-object checks through the production EP1 dispatcher. */
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
            var grammar=VehicleBatchAbilityContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                foreach(var family in VehicleBatchAbilityContract.Families){var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(family.Path,family.Source).First(),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                    // Reborn: authored/default records are independent of the captured original IDs and values.
                    if(variant>0){owner.RemoveAll();owner.SetAttribute("id","Reborn"+family.Source+variant);}
                    // Reborn: PullRate is the only required grappling attribute even in a defaults probe.
                    if(variant>0&&family.Source=="GrapplingHookUpdate")owner.SetAttribute("PullRate",variant==2?"75":"10");
                    if(variant==2&&family.Source=="GrapplingHookUpdate"){
                        owner.SetAttribute("MinBounceDelay","0.2s");owner.SetAttribute("MaxBounceDelay","0.9s");owner.SetAttribute("BouncePercentage","35%");owner.SetAttribute("CollidingPullRateFactor","60%");
                        foreach(string field in new[]{"SingleFireWeapon","ContinuousFireWeapon","ContinuousFireWeaponBlocked"})owner.SetAttribute(field,"SovietAntiInfantryInfantryAK47");owner.SetAttribute("ContinuousFireWeaponDelay","0.3s");
                        owner.InnerXml="<ObjectStatusValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='WEAPON_UPGRADED_01'/><KeepAliveObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='VEHICLE INFANTRY'/>";
                    }
                    if(variant==2&&family.Source=="SpecialAbilityUpdate"){owner.SetAttribute("PreparationTime","1s");owner.SetAttribute("PackTime","0.5s");owner.SetAttribute("StartAbilityRange","125");owner.SetAttribute("Options","RECONSTITUTE_STORED_COMMAND BUSY_DURING_PACK");owner.SetAttribute("SetObjectStatusOnTrigger","UNSELECTABLE");}

                }
                // Reborn: the same whole-object checks include original, minimal and source-sized articulated EP1 tank draws.
                var draws=source.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();
                var tank=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(TankDrawContract.Path,"TankDraw").Single(),true);draws.AppendChild(tank);ConscriptVariableBehaviorSource.DropLeafFormatting(tank);
                if(variant>0){tank.RemoveAll();tank.SetAttribute("id","RebornTank"+variant);}
                if(variant==2)tank.InnerXml="<TreadTerrainParticles xmlns='uri:ea.com:eala:asset'>DefaultTankTerrainParticleMapping</TreadTerrainParticles><ArticulationBone xmlns='uri:ea.com:eala:asset' ArticulationBoneName='RebornWheel' ArticulationHelperBoneName='RebornHelper'/><ArticulationBone xmlns='uri:ea.com:eala:asset'/>";
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                foreach(var family in VehicleBatchAbilityContract.Families){uint typeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(family.Wire);var native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),typeId);var element=resolved.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var proof=SchemaNativeContract.Verify(element,family.Type,typeId,native,refs);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,family.Wire,family.Type);_=grammar.Verify(metadata,native,family.Type,proof.Records);checks++;}
                var full=GameObjectBodyProjectionSmokeTest.Compile(normalized,true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: vehicle batch abilities/TankDraw: PASS ({checks} original/default/edit native/metadata and complete-object checks; grappling defaults/filters/weapons, ability fields, storage and articulated/terrain draws; no game launch).");
        }finally{Settings.Current=saved;}
    }
}
