using System.Runtime.InteropServices;
using System.Xml;
using System.Text;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: these additional original behaviors are admitted only after complete independent retail/source/native proof.
internal static class AdditionalStockBehaviorContract
{
    internal sealed record Family(string Source,string File,string Asset,Type Type,uint TypeId,int Size,string? ModuleId=null,bool WorldBuilder=false,bool LocalDefines=false,string? SourceOwner=null,bool RuntimeAbi=false);
    internal static readonly Family[] Families={
        // Reborn: this unused source module has no emitted owner; original PC field ABI extends an independently native-qualified Capture base.
        new("DistributeEnergyToAlliesBehavior","Neutral/Structures/NeutralPowerPlant.xml","NeutralPowerPlant",typeof(DistributeEnergyToAlliesBehaviorModuleData),0x0EF819D2u,24,RuntimeAbi:true),
        // Reborn: all remaining native candidates require an original owner, complete field proof and metadata chain.
        new("MoneyCrateCollide","Neutral/Crates/MoneyCrate.xml","MoneyCrate",typeof(Ep1MoneyCrateCollideModuleData),0xEF293C44u,Marshal.SizeOf<Ep1MoneyCrateCollideModuleData>(),SourceOwner:"MoneyCrate"),
        new("UnitCrateCollide","Neutral/Crates/UnitCrate.xml","UnitCrate",typeof(Ep1UnitCrateCollideModuleData),0xEE961694u,Marshal.SizeOf<Ep1UnitCrateCollideModuleData>(),SourceOwner:"UnitCrate"),
        new("VeterancyCrateCollide","Neutral/Crates/VeterancyCrate.xml","VeterancyCrate",typeof(Ep1VeterancyCrateCollideModuleData),0x573DC203u,Marshal.SizeOf<Ep1VeterancyCrateCollideModuleData>(),SourceOwner:"VeterancyCrate"),
        new("SalvageCrateCollide","Neutral/Crates/SalvageCrate.xml","SalvageCrate",typeof(Ep1SalvageCrateCollideModuleData),0x1E205942u,Marshal.SizeOf<Ep1SalvageCrateCollideModuleData>(),SourceOwner:"SalvageCrate"),
        new("RandomCrateCollide","Neutral/Crates/RandomCrate.xml","RandomCrate",typeof(Ep1RandomCrateCollideModuleData),0x1CC014E0u,Marshal.SizeOf<Ep1RandomCrateCollideModuleData>(),SourceOwner:"RandomCrate"),
        new("SpawnCrateUpdate","Neutral/Units/GenericCrateSpawner.xml","GenericCrateSpawner",typeof(Ep1SpawnCrateUpdateModuleData),0x4A32BF8Eu,Marshal.SizeOf<Ep1SpawnCrateUpdateModuleData>(),SourceOwner:"GenericCrateSpawner"),
        new("EnergyDisablingBehavior","Neutral/Props/EnergyDisabler.xml","EnergyDisabler",typeof(Ep1EnergyDisablingBehaviorModuleData),0x6451F8D7u,Marshal.SizeOf<Ep1EnergyDisablingBehaviorModuleData>(),SourceOwner:"EnergyDisabler"),
        new("UnitVisionBehavior","Neutral/Props/UnitVisionObserver.xml","UnitVisionObserver",typeof(Ep1UnitVisionBehaviorModuleData),0xF29A36D0u,Marshal.SizeOf<Ep1UnitVisionBehaviorModuleData>(),SourceOwner:"UnitVisionObserver"),
        new("SqueezeFlingUpdate","Soviet/Props/MagneticSingularityEffect.xml","MagneticSingularityEffect",typeof(Ep1SqueezeFlingUpdateModuleData),0x6E77AA59u,Marshal.SizeOf<Ep1SqueezeFlingUpdateModuleData>(),SourceOwner:"MagneticSingularityEffect"),
        new("RenderStoredThingTemplate","Soviet/Props/OrbitalRefuseSatteliteDebris.xml","OrbitalRefuseRandomDebris",typeof(Ep1ReplaceDrawModuleBehaviorModuleData),0xB4C271D2u,Marshal.SizeOf<Ep1ReplaceDrawModuleBehaviorModuleData>(),SourceOwner:"OrbitalRefuseRandomDebris"),
        new("GatherEjectUpdate","Soviet/Props/VacuumBombEffect.xml","VacuumBombEffect",typeof(Ep1GatherEjectUpdateModuleData),0xE611C8EEu,Marshal.SizeOf<Ep1GatherEjectUpdateModuleData>(),SourceOwner:"VacuumBombEffect"),
        new("CrateCollide","Neutral/Crates/BombCrate.xml","BombCrate",typeof(Ep1CrateCollideModuleData),0x2027962Au,Marshal.SizeOf<Ep1CrateCollideModuleData>(),SourceOwner:"BombCrate"),

        // Reborn: original bombardment and veterancy capture have independent full native/metadata witnesses.
        new("BombardAreaUpdate","Japan/Props/BalloonBombEffect.xml","BaseBalloonBombEffect",typeof(BombardAreaUpdateModuleData),0x4950FD10u,44,"ModuleTag_BalloonBombs",SourceOwner:"BaseBalloonBombEffect"),
        new("CaptureAndGiveUpgradeBehavior","Neutral/Structures/VeterancyTechStructure.xml","VeterancyTechStructure",typeof(CaptureAndGiveUpgradeBehaviorModuleData),0x1E95EE57u,32,WorldBuilder:true),
        // Reborn: the original balloon bomb proves EP1's 20-byte height-death update and complete metadata chain.
        new("HeightDieUpdate","Japan/Props/JapanBalloonBombProjectile.xml","JapanBalloonBomb",typeof(Ep1HeightDieUpdateModuleData),0x7517FE52u,20),
        // Reborn: spawned slaves retain the full original SlavedUpdate ABI under their own concrete EP1 class.
        new("SpawnedSlaveUpdate","EP1/Soviet/Props/SovietDesolatorInfantryObjects.xml","DesolatorSprayDamageTrail",typeof(SpawnedSlaveUpdateModuleData),0x004CA983u,112),
        // Reborn: each newly admitted EP1 family has a real populated original source/native/metadata witness.
        new("EncloseBehavior","Japan/Props/NanoSwarmHiveEffect.xml","NanoSwarmHiveEffect",typeof(EncloseBehaviorModuleData),0xAAB64617u,148),
        new("ArmorUpgrade","EP1/Japan/Units/JapanYurikoTech2.xml","JapanYurikoTech2",typeof(ArmorUpgradeModuleData),0xA1FE37ACu,44),
        new("LineSegmentDamageUpdate","EP1/Soviet/Props/SovietDesolatorInfantryObjects.xml","DesolatorSprayDamageTrail",typeof(LineSegmentDamageUpdateModuleData),0x00DDBB83u,80,LocalDefines:true),
        // Reborn: Future Tank's populated sphere proves the complete inherited filter, bone string, weapon and condition/status masks.
        new("DamageSphereUpdate","EP1/Allied/Props/AlliedFutureTankNeutronScramblerNode.xml","AlliedFutureTankNeutronScramblerNode",typeof(DamageSphereUpdateModuleData),0x878CEA1Fu,372),
        // Reborn: both secret-file collision modules have populated independent original WorldBuilder witnesses.
        new("AttributeCrateCollide","Civilian/Shiro_Sanitarium_SS/Buildings/SS_SecretFile1.xml","SS_SecretFile1",typeof(AttributeCrateCollideModuleData),0x4843B2E7u,124,WorldBuilder:true),
        new("HealCrateCollide","Civilian/Shiro_Sanitarium_SS/Buildings/SS_SecretFile1.xml","SS_SecretFile1",typeof(Ep1HealCrateCollideModuleData),0x839AE83Cu,124,WorldBuilder:true),
        // Reborn: the original WorldBuilder blast door anchors both geometry-name lists and the gate state/timing record.
        new("GateOpenAndCloseBehavior","Civilian/Romania_RM/Destructible_Props/RM_AlliedBlastDoor.xml","RM_AlliedBlastDoor",typeof(GateOpenAndCloseBehaviorModuleData),0xCE47CA96u,68,WorldBuilder:true),
        new("SetLocomotorSpeedUpdate","BaseObjects/BaseCivilianAuto.xml","CUAuto01",typeof(SetLocomotorSpeedUpdateModuleData),0x2F4536A8,40,WorldBuilder:true),
        new("InvalidTargetsDeathUpdate","Soviet/Props/SovietBombingRunBeacon.xml","SovietBombingRunBeacon",typeof(InvalidTargetsDeathUpdateModuleData),0x5717D9DC,12),
        new("SelectiveVisibilityBehavior","Allied/Props/ChronoTargetMarker.xml","ChronoTargetMarker",typeof(SelectiveVisibilityBehaviorModuleData),0x6154A1A4,16),
        new("LaserScatterUpdate","Allied/Props/AlliedAntiVehicleVehicleTech3CannonNode.xml","AlliedAntiVehicleVehicleTech3CannonNodeEnd",typeof(LaserScatterUpdateModuleData),0xFA3F1C6A,16),
        new("WeaponScatterEffectUpdate","Allied/Props/AlliedAntiVehicleVehicleTech3CannonNode.xml","AlliedAntiVehicleVehicleTech3CannonNodeRegular",typeof(WeaponScatterEffectUpdateModuleData),0x3EB8B5D6,144,"Regular_ScatterEffect"),
        new("SubStructureBehavior","Allied/Structures/AlliedPowerPlantTurbine.xml","AlliedPowerPlantTurbine",typeof(SubStructureBehaviorModuleData),0x331E6DB8,8),
        new("ParabolicCurveUpdate","Allied/Units/AlliedSupersonicBomber.xml","AlliedSupersonicBomber",typeof(ParabolicCurveUpdateModuleData),0x4727001D,8),
        new("DestroyObjectsUpdate","Allied/Props/AlliedAntiStructureVehicleCannonEffect.xml","AlliedAntiStructureVehicleCannonEffect",typeof(DestroyObjectsUpdateModuleData),0x7C27AD44,132)};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the actual original module, including nested filters and inherited update fields. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(Family family,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf(family.Type)!=family.Size)throw new InvalidDataException("Original behavior layout differs: "+family.Source);
        // Reborn: these original WorldBuilder records retain the same EP1 ABI and have their own physical native catalog.
        manifest=family.WorldBuilder?WorldBuilderAssetResolver.CatalogPath():manifest;
        // Reborn: do not claim an emitted stock owner for the runtime-only extension; reuse only its real native Capture base grammar.
        if(family.RuntimeAbi)
        {
            var runtimeGrammar=Qualify(Families.Single(f=>f.Source=="CaptureAndGiveUpgradeBehavior"),manifest,baseline,schemas,catalog);
            runtimeGrammar.QualifyEnergySharingRuntimeAbi(schemas.Schemas);return runtimeGrammar;
        }
        string identity="GameObject:"+family.Asset;
        var stock=GameObjectStockReview.Capture(manifest,identity,allowNullArmorReferences:true);
        var module=GameObjectModuleReview.Inspect(manifest,identity,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==family.TypeId);
        var source=(XmlDocument)baseline.CloneNode(true);
        var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
        var original=GameObjectExtendedRootContract.Original(family.File,family.Source).Single(e=>(family.ModuleId==null||e.GetAttribute("id")==family.ModuleId)&&
            // Reborn: repeated module IDs in separate derived objects must select the actual original source owner.
            (family.SourceOwner==null||e.ParentNode?.ParentNode is XmlElement sourceOwner&&sourceOwner.GetAttribute("id")==family.SourceOwner));
        // Reborn: this original line-damage witness uses exact local literals declared in the same immutable source document.
        if(family.LocalDefines)original=ResolveLocalWitness(original);
        var owner=(XmlElement)source.ImportNode(original,true);
        ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
        var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
        SchemaNativeContract.VerifyStock(resolved,family.Type,family.TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray(),required:true);
        var grammar=new StockMetadataGrammar();grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);return grammar;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve only the captured original module's own local literal definitions through the existing pre-inheritance evaluator. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement ResolveLocalWitness(XmlElement original)
    {
        var declaration=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<AssetDeclaration xmlns=\""+SchemaSet.XmlNamespace+"\"><GameObject id=\"RebornLocalModuleWitness\"><Behaviors/></GameObject></AssetDeclaration>"));
        var definitions=original.OwnerDocument!.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Defines"&&e.NamespaceURI==SchemaSet.XmlNamespace);
        declaration.DocumentElement!.PrependChild(declaration.ImportNode(definitions,true));
        declaration.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!.AppendChild(declaration.ImportNode(original,true));
        var result=SdkLocalDefineProfile.ApplyBeforeInheritance(Encoding.UTF8.GetBytes(declaration.OuterXml));
        if(result.Bytes==null)throw new InvalidDataException("Original module local definitions rejected: "+string.Join("; ",result.Evidence.Diagnostics));
        return SdkXmlDiagnostics.Parse(result.Bytes).GetElementsByTagName(original.LocalName,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
    }

}
