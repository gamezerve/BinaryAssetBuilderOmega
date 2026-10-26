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

// Reborn: cover transport stock imports, authored passenger lists, defaults and full object composition without editing user sources.
internal static class TransportContainSmokeTest
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
            var grammar=TransportContainContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,8)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var witness=TransportContainContract.Witnesses[Math.Min(variant,3)];var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"TransportContain").Single(),true);behaviors.AppendChild(owner);
                // Reborn: reproduce the campaign bomber's inherited transport and its three original authored payload rows in memory.
                if(variant==4){owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(TransportContainContract.Witnesses[0].Path,"TransportContain").Single(),true);behaviors.RemoveAll();behaviors.AppendChild(owner);foreach(var payload in GameObjectExtendedRootContract.Original("Allied/Units_Campaign/CAMP_AlliedBomberAircraft_Infantry_2.xml","TransportContain").Single().ChildNodes.OfType<XmlElement>())owner.AppendChild(source.ImportNode(payload,true));}
                // Reborn: cover omitted optionals, explicit empty filters and all extended transport values without changing user mod files.
                if(variant>=5){owner.RemoveAll();owner.SetAttribute("id","RebornTransport");}
                if(variant==6)owner.InnerXml="<FadeFilter xmlns='uri:ea.com:eala:asset'/>";
                if(variant==7){
                    foreach(var pair in new[]{("ScatterNearbyOnExit","false"),("OrientLikeContainerOnExit","true"),("DestroyRidersWhoAreNotFreeToExit","true"),("ExitBone","REBORNBONE"),("HealthRegenPercentPerSec","2.5"),("ExitDelay","12"),("TypeOneForWeaponState","INFANTRY"),("TypeTwoForWeaponState","VEHICLE"),("TypeThreeForWeaponState","AIRCRAFT"),("ForceOrientationContainer","false"),("ConditionForEntry","INVALID"),("ShouldThrowOutPassengers","true"),("ThrowOutPassengersDelay","25"),("FadePassengerOnEnter","true"),("FadePassengerOnExit","true"),("EnterFadeTime","0.5"),("ExitFadeTime","0.25"),("FadeReverse","true"),("ReleaseSnappyness","0.9"),("ExtendedExitContainerChecks","true")})owner.SetAttribute(pair.Item1,pair.Item2);
                    // Reborn: exercise every inherited containment scalar together with source-sized member/modifier/passenger lists.
                    foreach(var pair in new[]{("ContainMax","12"),("DamagePercentToUnits","17%"),("PassengersTestCollisionHeight","6"),("PassengersInTurret","true"),("NumberOfExitPaths","3"),("DoorOpenTime","9"),("AllowOwnPlayerInsideOverride","true"),("AllowAlliesInside","false"),("AllowEnemiesInside","false"),("AllowNeutralInside","false"),("PassDisabilityToRiders","true"),("IgnoreDisabledBitsForRiders","EMP"),("ShowPips","false"),("CollidePickup","false"),("EjectPassengersOnDeath","false"),("KillPassengersOnDeath","true"),("HasObjectStatusOfContainedEntry","false"),("Enabled","false"),("ObjectStatusOfContained","ENCLOSED UNSELECTABLE"),("ObjectStatusWhileContaining","CONTAINER_OCCUPIED NO_ATTACK"),("ModifierRequiredTime","22"),("KillIfEmptyTime","3s")})owner.SetAttribute(pair.Item1,pair.Item2);
                    owner.SetAttribute("EnterSound","VehicleEnter");owner.SetAttribute("ExitSound","ParachutingOutOfAirplane");
                    owner.SetAttribute("ThrowOutPassengersLandingWarhead",catalog.First(a=>a.Name.StartsWith("WeaponTemplate:")).Name.Split(':')[1]);
                    string upgrade=catalog.First(a=>a.Name.StartsWith("UpgradeTemplate:")).Name.Split(':')[1];
                    owner.InnerXml=string.Concat(Enumerable.Range(1,4).Select(i=>$"<InitialPayload xmlns='uri:ea.com:eala:asset' Name='SovietAntiInfantryInfantry' Count='{i}'/>"))+"<ThrowOutPassengersVelocity xmlns='uri:ea.com:eala:asset' x='1' y='2' z='3'/>"+string.Concat(Enumerable.Range(1,4).Select(i=>$"<UpgradeCreationTrigger xmlns='uri:ea.com:eala:asset' Upgrade='{upgrade}' Template='AlliedScoutInfantry' Num='{i}'/>"))+"<FadeFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='INFANTRY VEHICLE'><ExcludeThing>SovietBaseDefenseAdvanced</ExcludeThing></FadeFilter>";
                }
                if(variant==7){
                    string modifier=catalog.First(a=>a.Name.StartsWith("AttributeModifier:")).Name.Split(':')[1];
                    var inherited=source.CreateElement("Inherited",SchemaSet.XmlNamespace);inherited.InnerXml="<PassengerFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='INFANTRY VEHICLE'/><DieMuxData xmlns='uri:ea.com:eala:asset' DeathTypes='ALL'/>"+string.Concat(Enumerable.Range(1,2).Select(i=>$"<PassengerData xmlns='uri:ea.com:eala:asset' BonePrefix='BONE{i}' MaxPassengers='{i}' SlingUnderBone='true'><Filter Rule='ANY' Include='INFANTRY'/></PassengerData>"))+$"<ModifierToGiveOnExit xmlns='uri:ea.com:eala:asset'>{modifier}</ModifierToGiveOnExit><MemberTemplateStatusInfo xmlns='uri:ea.com:eala:asset' ThingTemplate='SovietAntiInfantryInfantry' ObjectStatus='ENCLOSED'/>";
                    foreach(var child in inherited.ChildNodes.OfType<XmlElement>().Reverse().ToArray())owner.PrependChild(child);
                }
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("TransportContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,TransportContainContract.TypeId);var element=resolved.GetElementsByTagName("TransportContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(TransportContainModuleData),TransportContainContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,TransportContainContract.Wire,typeof(TransportContainModuleData));_=grammar.Verify(metadata,native,typeof(TransportContainModuleData),proof.Records);checks++;
                // Reborn: authored transport variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>=4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted health regeneration must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<TransportContainModuleData>(nameof(TransportContainModuleData.HealthRegenPercentPerSec)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(TransportContainModuleData),TransportContainContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt transport payload accepted.");}catch(InvalidDataException){checks++;}
                if(variant==7){
                    // Reborn: reject corrupt metadata and preserve the official four-row payload cardinality, rather than adding a compiler-specific quota.
                    var badTokens=new Chunk{InstanceBuffer=metadata.InstanceBuffer.ToArray(),RelocationBuffer=metadata.RelocationBuffer,ImportsBuffer=metadata.ImportsBuffer};badTokens.InstanceBuffer[4]^=1;
                    try{grammar.Verify(badTokens,native,typeof(TransportContainModuleData),proof.Records);throw new InvalidOperationException("Corrupt transport metadata accepted.");}catch(InvalidDataException){checks++;}
                    var payload=owner.ChildNodes.OfType<XmlElement>().Last(e=>e.LocalName=="InitialPayload");owner.InsertAfter(payload.CloneNode(true),payload);
                    try{SchemaNativeContract.Resolve(source,schemas,catalog);throw new InvalidOperationException("Five transport payload rows accepted.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: TransportContain: PASS ({checks} four-retail/campaign-payload/all-field/upgrade/filter/default/composition/corruption checks; retail root/base witnesses, schema-derived extended items; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
