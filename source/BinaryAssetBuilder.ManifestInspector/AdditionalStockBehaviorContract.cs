using System.Runtime.InteropServices;
using System.Xml;
using System.Text;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: these additional original behaviors are admitted only after complete independent retail/source/native proof.
internal static class AdditionalStockBehaviorContract
{
    internal sealed record Family(string Source,string File,string Asset,Type Type,uint TypeId,int Size,string? ModuleId=null,bool WorldBuilder=false,bool LocalDefines=false);
    internal static readonly Family[] Families={
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
        string identity="GameObject:"+family.Asset;
        var stock=GameObjectStockReview.Capture(manifest,identity,allowNullArmorReferences:true);
        var module=GameObjectModuleReview.Inspect(manifest,identity,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==family.TypeId);
        var source=(XmlDocument)baseline.CloneNode(true);
        var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
        var original=GameObjectExtendedRootContract.Original(family.File,family.Source).Single(e=>family.ModuleId==null||e.GetAttribute("id")==family.ModuleId);
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
