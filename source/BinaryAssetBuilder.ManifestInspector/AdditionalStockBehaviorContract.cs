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
        // Reborn: the EA fixture and the EP1 field reader independently prove this unshipped module.
        new("DestructibleEnvironmentUpdate","","",typeof(DestructibleEnvironmentUpdateModuleData),DestructibleEnvironmentUpdateModuleData.WireTypeId,40,RuntimeAbi:true),
        // Reborn: EA's named fixtures and EP1 readers prove the concrete tails over original EP1 parent tables.
        new("SwitchLocomotorsSpecialAbilityUpdate","","",typeof(Ep1SwitchLocomotorsSpecialAbilityUpdateModuleData),Ep1SwitchLocomotorsSpecialAbilityUpdateModuleData.WireTypeId,0x110,RuntimeAbi:true),
        new("ReplaceSelfUpgrade","","",typeof(Ep1ReplaceSelfUpgradeModuleData),Ep1ReplaceSelfUpgradeModuleData.WireTypeId,0x34,RuntimeAbi:true),
        // Reborn: the original named SubStructureBuild route and EP1 list readers prove both nested records.
        new("SubStructureBuild","","",typeof(Ep1SubStructureBuildUpgradeModuleData),Ep1SubStructureBuildUpgradeModuleData.WireTypeId,0x34,RuntimeAbi:true),
        // Reborn: EA source emission and the EP1 delay scheduler prove the delayed WeaponFire extension.
        new("DelayedWeaponFireSpecialAbilityUpdate","","",typeof(Ep1DelayedWeaponFireSpecialAbilityUpdateModuleData),Ep1DelayedWeaponFireSpecialAbilityUpdateModuleData.WireTypeId,0x120,RuntimeAbi:true),
        // Reborn: EA serialized a base-only row, and the EP1 factory allocates the retail Attach record.
        new("LeechTargetingAttachUpdate","","",typeof(Ep1LeechTargetingAttachUpdateModuleData),Ep1LeechTargetingAttachUpdateModuleData.WireTypeId,0x170,RuntimeAbi:true),
        // Reborn: the EA color rows and EP1 runtime reader prove the independent EMP layout.
        new("EMPUpdate","","",typeof(Ep1EMPUpdateModuleData),Ep1EMPUpdateModuleData.Wire,0x28,RuntimeAbi:true),
        // Reborn: the named EA record and EP1 loader prove a command string and upgrade import over SpecialAbilityUpdate.
        new("GiveOrRestoreUpgradeSpecialPower","","",typeof(Ep1GiveOrRestoreUpgradeSpecialPowerModuleData),Ep1GiveOrRestoreUpgradeSpecialPowerModuleData.Wire,0x108,RuntimeAbi:true),
        // Reborn: the EA activation rows and naval import extend independently stock-qualified parents.
        new("ActivateModuleSpecialAbilityUpdate","","",typeof(Ep1ActivateModuleSpecialAbilityUpdateModuleData),Ep1ActivateModuleSpecialAbilityUpdateModuleData.Wire,0x104,RuntimeAbi:true),
        new("AttributeModifierNaval","","",typeof(Ep1AttributeModifierNavalModuleData),Ep1AttributeModifierNavalModuleData.Wire,12,RuntimeAbi:true),
        // Reborn: the original named powers extend the independently qualified OCL and SpecialAbility bases.
        new("CreateAndEnterObjectSpecialPower","","",typeof(Ep1CreateAndEnterObjectSpecialPowerModuleData),Ep1CreateAndEnterObjectSpecialPowerModuleData.Wire,0x210,RuntimeAbi:true),
        new("ObjectStatusSpecialAbilityUpdate","","",typeof(Ep1ObjectStatusSpecialAbilityUpdateModuleData),Ep1ObjectStatusSpecialAbilityUpdateModuleData.Wire,0x13c,RuntimeAbi:true),
        // Reborn: named power and damage-field layouts are captured by EA and verified against the EP1 parents.
        new("SpecialPowerUpdate","","",typeof(Ep1SpecialPowerUpdateModuleData),Ep1SpecialPowerUpdateModuleData.Wire,0x18,RuntimeAbi:true),
        new("DamageFieldUpdate","","",typeof(Ep1DamageFieldUpdateModuleData),Ep1DamageFieldUpdateModuleData.Wire,0x2c,RuntimeAbi:true),
        // Reborn: direct EP1 readers identify these concrete classes and their independently qualified base tables.
        new("SphereModuleUpdate","","",typeof(SphereModuleUpdateModuleData),0x9D74F07Au,0xb0,RuntimeAbi:true),
        new("StealUnitUpdate","","",typeof(StealUnitUpdateModuleData),0x21068FF6u,0x170,RuntimeAbi:true),
        new("BoobyTrapUpdate","","",typeof(BoobyTrapUpdateModuleData),0xE502295Eu,0x1bc,RuntimeAbi:true),
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
        // Reborn: use an actual stock UpdateModuleData table for the environment module's inherited metadata.
        if (family.Type == typeof(DestructibleEnvironmentUpdateModuleData))
        {
            var refinery = GameObjectStockReview.Capture(manifest, "GameObject:SovietRefinery");
            var production = GameObjectModuleReview.Inspect(manifest, refinery.Asset.Name).Containers
                .Single(container => container.Name == "Behaviors").Modules.Single(module => module.TypeId == 0x6EC75439u);
            var own = checked((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(refinery.Bytes.AsSpan(production.Wrapper + 8)));
            int inherited = GameObjectStockReview.Tokens(refinery.Bytes, own)[1u].Offset;
            var environmentGrammar = new StockMetadataGrammar();
            environmentGrammar.Learn(refinery.Bytes, inherited, refinery.Projection.NativeRoot, typeof(UpdateModuleData));
            environmentGrammar.QualifyDestructibleEnvironment(schemas.Schemas);
            return environmentGrammar;
        }
        // Reborn: reuse the independently reviewed retail parent metadata, omitting unrelated sibling rules.
        if (family.Type == typeof(Ep1SwitchLocomotorsSpecialAbilityUpdateModuleData) ||
            family.Type == typeof(Ep1ReplaceSelfUpgradeModuleData) ||
            family.Type == typeof(Ep1SubStructureBuildUpgradeModuleData))
        {
            bool locomotor = family.Type == typeof(Ep1SwitchLocomotorsSpecialAbilityUpdateModuleData);
            var basis = locomotor ? WeaponFireAbilityContract.Qualify(manifest, baseline, schemas, catalog)
                : AttributeModifierUpgradeContract.Qualify(manifest, baseline, schemas, catalog);
            var composedGrammar = StockMetadataGrammar.Import(basis.Export().Where(rule => rule.Class == null &&
                rule.Type != typeof(Ep1SubObjectsUpgradeNugget).FullName));
            if (family.Type == typeof(Ep1SubStructureBuildUpgradeModuleData))
                composedGrammar.QualifySubStructureBuild(schemas.Schemas);
            else
                composedGrammar.QualifyLocomotorOrReplacement(schemas.Schemas, family.Type, family.TypeId, family.Size);
            return composedGrammar;
        }
        // Reborn: the original concrete WeaponFire table becomes a plain inherited table on the delayed class.
        if (family.Type == typeof(Ep1DelayedWeaponFireSpecialAbilityUpdateModuleData))
        {
            var weaponFire = WeaponFireAbilityContract.Qualify(manifest, baseline, schemas, catalog);
            var delayed = StockMetadataGrammar.Import(weaponFire.Export().Select(rule => rule with { Class = null }));
            delayed.QualifyDelayedWeaponFire(schemas.Schemas);
            return delayed;
        }
        // Reborn: the two stock attachment witnesses jointly prove absent and populated optional base records.
        if (family.Type == typeof(Ep1LeechTargetingAttachUpdateModuleData))
        {
            var scout = ScoutVehicleBehaviorContract.Qualify(manifest, baseline, schemas, catalog);
            var attach = ScoutVehicleBehaviorContract.QualifyMoneyGain(manifest, scout);
            var rules = attach.Export().Where(rule => rule.Class == null || rule.Type == typeof(Ep1AttachUpdateModuleData).FullName)
                .Select(rule => rule with { Class = null });
            var leech = StockMetadataGrammar.Import(rules);
            leech.QualifyLeechTargeting(schemas.Schemas);
            return leech;
        }
        // Reborn: reuse the original UpdateModuleData table and add the two authored color tables.
        if (family.Type == typeof(Ep1EMPUpdateModuleData))
        {
            var environment = Qualify(Families.Single(f=>f.Type==typeof(DestructibleEnvironmentUpdateModuleData)),manifest,baseline,schemas,catalog);
            var grammarWithColors = StockMetadataGrammar.Import(environment.Export().Where(rule=>rule.Class==null));
            grammarWithColors.QualifyEMP(schemas.Schemas);
            return grammarWithColors;
        }
        // Reborn: qualify the stock SpecialAbilityUpdate parent separately from the EA-authored grant row.
        if (family.Type == typeof(Ep1GiveOrRestoreUpgradeSpecialPowerModuleData))
        {
            var parent=WeaponFireAbilityContract.Qualify(manifest,baseline,schemas,catalog);
            var grant=StockMetadataGrammar.Import(parent.Export().Where(rule=>rule.Class==null));
            grant.QualifyGiveOrRestoreUpgrade(schemas.Schemas);
            return grant;
        }
        // Reborn: compose EA-authored own rows over the independently read stock ability and create tables.
        if(family.Type==typeof(Ep1ActivateModuleSpecialAbilityUpdateModuleData)||family.Type==typeof(Ep1AttributeModifierNavalModuleData))
        {
            bool activation=family.Type==typeof(Ep1ActivateModuleSpecialAbilityUpdateModuleData);
            var parent=activation?WeaponFireAbilityContract.Qualify(manifest,baseline,schemas,catalog)
                :RadarBehaviorContract.Qualify(manifest,baseline,schemas,catalog);
            var rules=parent.Export().Where(rule=>rule.Class==null).ToList();
            rules.AddRange(ActivationAndNavalRules(family,schemas.Schemas));
            return StockMetadataGrammar.Import(rules);
        }
        // Reborn: retain the stock parent tables as inherited rows, then append the original named own fields.
        if(family.Type==typeof(Ep1CreateAndEnterObjectSpecialPowerModuleData)||family.Type==typeof(Ep1ObjectStatusSpecialAbilityUpdateModuleData))
        {
            bool create=family.Type==typeof(Ep1CreateAndEnterObjectSpecialPowerModuleData);
            var parent=create?OclSpecialPowerContract.Qualify(manifest,baseline,schemas,catalog)
                :WeaponFireAbilityContract.Qualify(manifest,baseline,schemas,catalog);
            var rules=parent.Export().Where(row=>create||row.Class==null).Select(row=>row with { Class=null });
            return StockMetadataGrammar.Import(rules.Append(DerivedPowerRule(family,schemas.Schemas)));
        }
        // Reborn: the original power and damage field each preserve their own independently identified parent.
        if(family.Type==typeof(Ep1SpecialPowerUpdateModuleData) || family.Type==typeof(Ep1DamageFieldUpdateModuleData))
        {
            bool damage=family.Type==typeof(Ep1DamageFieldUpdateModuleData);
            var parent=FireWeaponUpdateContract.Qualify(manifest,baseline,schemas,catalog);
            var rules=parent.Export().Where(row=>damage || row.Type==typeof(UpdateModuleData).FullName ||
                row.Type==typeof(ContainModuleData).FullName || row.Type==typeof(BehaviorModuleData).FullName ||
                row.Type==typeof(ModuleData).FullName).Select(row=>row with { Class=null });
            return StockMetadataGrammar.Import(rules.Concat(PowerAndFieldRules(family,schemas.Schemas)));
        }
        // Reborn: the EP1 direct reader consumes the same sphere table as the observed retail shield parents.
        if(family.Type==typeof(SphereModuleUpdateModuleData))
        {
            var rules=YurikoShieldSphereContract.Qualify(manifest).Export()
                .Where(row=>row.Type!=typeof(YurikoShieldSphereUpdateModuleData).FullName &&
                    row.Type!=typeof(ShieldSphereUpdateModuleData).FullName)
                .Select(row=>row.Type==family.Type.FullName ? row with { Class=family.TypeId } : row);
            return StockMetadataGrammar.Import(rules);
        }
        // Reborn: combine retail EP1 attachment base rows with the derived class readers' concrete tags.
        if(family.Type==typeof(StealUnitUpdateModuleData) || family.Type==typeof(BoobyTrapUpdateModuleData))
        {
            var scout=ScoutVehicleBehaviorContract.Qualify(manifest,baseline,schemas,catalog);
            var known=ScoutVehicleBehaviorContract.QualifyMoneyGain(manifest,scout);
            var rules=known.Export().Where(row=>row.Class==null || row.Type==typeof(Ep1AttachUpdateModuleData).FullName)
                .Select(row=>row with { Class=null });
            return StockMetadataGrammar.Import(rules.Append(DerivedAttachmentRule(family,schemas.Schemas)));
        }
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
    /** Reborn: retain the EA-declared row order and verify the EP1 field offsets and repeated-item stride. */
    //-------------------------------------------------------------------------------------------------
    private static IEnumerable<StockMetadataGrammar.Rule> ActivationAndNavalRules(Family family,System.Xml.Schema.XmlSchemaSet schemas)
    {
        bool activation=family.Type==typeof(Ep1ActivateModuleSpecialAbilityUpdateModuleData);
        var schema=schemas.GlobalTypes[new XmlQualifiedName(family.Source+"ModuleData",SchemaSet.XmlNamespace)]
            as System.Xml.Schema.XmlSchemaComplexType;
        string parent=activation?"SpecialAbilityUpdateModuleData":"CreateModuleData";
        if(schema?.BaseXmlSchemaType?.QualifiedName.Name!=parent)
            throw new InvalidDataException("Activation or naval schema parent differs: "+family.Source);
        if(!activation)
        {
            if(Marshal.OffsetOf(family.Type,"AttributeModifier").ToInt32()!=8)
                throw new InvalidDataException("EP1 naval modifier reference offset differs.");
            yield return new(family.Type.FullName!,family.TypeId,new StockMetadataGrammar.RuleField[]{
                new(1,0x30A36B8Du,2),new(0xC5E07887u,0x09DDCFF3u,4)});
            yield break;
        }
        if(Marshal.OffsetOf(family.Type,"SpecialPowerModules").ToInt32()!=0xfc ||
            Marshal.SizeOf<SpecialPowerModuleInfo>()!=12 ||
            Marshal.OffsetOf<SpecialPowerModuleInfo>(nameof(SpecialPowerModuleInfo.TriggerType)).ToInt32()!=4 ||
            Marshal.OffsetOf<SpecialPowerModuleInfo>(nameof(SpecialPowerModuleInfo.TriggerAtTargetPosition)).ToInt32()!=8)
            throw new InvalidDataException("EP1 activation list layout differs.");
        yield return new(family.Type.FullName!,family.TypeId,new StockMetadataGrammar.RuleField[]{
            new(1,0xAC598F82u,2),new(0x8381468Eu,0xADA54D29u,2)});
        yield return new(typeof(SpecialPowerModuleInfo).FullName!,null,new StockMetadataGrammar.RuleField[]{
            new(0xE1DF8F55u,0xBDF07965u,4),new(0xEC377D59u,0x77E3F4F7u,4),new(0x01C3A3A2u,0x01C3A3A2u,4)});
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the EA concrete rows and exact EP1 header offsets for the two power variants. */
    //-------------------------------------------------------------------------------------------------
    private static StockMetadataGrammar.Rule DerivedPowerRule(Family family,System.Xml.Schema.XmlSchemaSet schemas)
    {
        bool create=family.Type==typeof(Ep1CreateAndEnterObjectSpecialPowerModuleData);
        var shape=schemas.GlobalTypes[new XmlQualifiedName(family.Source+"ModuleData",SchemaSet.XmlNamespace)]
            as System.Xml.Schema.XmlSchemaComplexType;
        if(shape?.BaseXmlSchemaType?.QualifiedName.Name!=(create?"OCLSpecialPowerModuleData":"SpecialAbilityUpdateModuleData"))
            throw new InvalidDataException("EP1 derived power schema parent differs.");
        if(create?Marshal.OffsetOf(family.Type,"FXOffset").ToInt32()!=0x20c:
            Marshal.OffsetOf(family.Type,"Status1").ToInt32()!=0xfc ||
            Marshal.OffsetOf(family.Type,"Status2").ToInt32()!=0x11c)
            throw new InvalidDataException("EP1 derived power tail layout differs.");
        var fields=create?new StockMetadataGrammar.RuleField[]{new(1,0x8999C6A4u,2),new(0x388F3EC5u,0x29368077u,5)}
            :new StockMetadataGrammar.RuleField[]{new(1,0xAC598F82u,2),new(0x128B08D0u,0xA7BD811Eu,4),new(0x957A5821u,0xA7BD811Eu,4)};
        return new(family.Type.FullName!,family.TypeId,fields);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind the EA power and damage tails, including their optional sound/EVA and filter tables. */
    //-------------------------------------------------------------------------------------------------
    private static IEnumerable<StockMetadataGrammar.Rule> PowerAndFieldRules(Family family,System.Xml.Schema.XmlSchemaSet schemas)
    {
        bool damage=family.Type==typeof(Ep1DamageFieldUpdateModuleData);
        var schema=schemas.GlobalTypes[new XmlQualifiedName(family.Source+"ModuleData",SchemaSet.XmlNamespace)]
            as System.Xml.Schema.XmlSchemaComplexType;
        if(schema?.BaseXmlSchemaType?.QualifiedName.Name!=(damage?"FireWeaponUpdateModuleData":"UpdateModuleData"))
            throw new InvalidDataException("EP1 power/damage update source parent differs.");
        if(!damage)
        {
            if(Marshal.OffsetOf(family.Type,"DisabledTypesToIgnore").ToInt32()!=0x0c ||
                Marshal.OffsetOf(family.Type,"InitiateSound").ToInt32()!=0x10 ||
                Marshal.OffsetOf(family.Type,"StartsPaused").ToInt32()!=0x14 || Marshal.SizeOf<Ep1SoundOrEvaEvent>()!=8)
                throw new InvalidDataException("EP1 special power update layout differs.");
            yield return new(family.Type.FullName!,family.TypeId,new StockMetadataGrammar.RuleField[]
            {
                new(1,0xF1002505u,2),new(0x81D85EFAu,0xAEE83888u,4),new(0x566B716Cu,0x77E3F4F7u,4),
                new(0x59AADBE2u,0x196570C9u,4),new(0x17E786ADu,0x1AE6B6B7u,3)
            });
            yield return new(typeof(Ep1SoundOrEvaEvent).FullName!,null,new StockMetadataGrammar.RuleField[]
            {
                new(0x3006E54Du,0x63D7B468u,5),new(0x1553B19Eu,0xC4EC43B1u,4)
            });
            yield break;
        }
        if(Marshal.OffsetOf(family.Type,"Radius").ToInt32()!=0x20 ||
            Marshal.OffsetOf(family.Type,"RequiredUpgrade").ToInt32()!=0x24 ||
            Marshal.OffsetOf(family.Type,"ObjectFilter").ToInt32()!=0x28)
            throw new InvalidDataException("EP1 damage field layout differs.");
        yield return new(family.Type.FullName!,family.TypeId,new StockMetadataGrammar.RuleField[]
        {
            new(1,0xB28C3D52u,2),new(0x4F31C6C5u,0x23E650B8u,4),
            new(0x7F719729u,0x4ABA1F29u,4),new(0xE46EA3B2u,0xE46EA3B2u,3)
        });
        yield return new(typeof(ObjectFilter).FullName!,null,new StockMetadataGrammar.RuleField[]
        {
            new(0x8CDC9898u,0x4D3B3F19u,4),new(0xC9C22AA2u,0x0027BB58u,4),
            new(0xA037DC14u,0x5C8981A9u,4),new(0x27AF1102u,0x9FFD43EBu,4),
            new(0x1AF86941u,0x9AFC9193u,4),new(0x87D26051u,0x9AFC9193u,4),
            new(0xF2FC1F81u,0xA7BD811Eu,5),new(0x5C5EB235u,0xA7BD811Eu,5),
            new(0xC8801D7Eu,0x9B19049Fu,2),new(0xD6A24C87u,0x9B19049Fu,2)
        });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify the named attachment subclass and the EP1 Booby Trap offsets from reader 00493c20. */
    //-------------------------------------------------------------------------------------------------
    private static StockMetadataGrammar.Rule DerivedAttachmentRule(Family family,System.Xml.Schema.XmlSchemaSet schemas)
    {
        var shape=schemas.GlobalTypes[new XmlQualifiedName(family.Source+"ModuleData",SchemaSet.XmlNamespace)]
            as System.Xml.Schema.XmlSchemaComplexType;
        if(shape?.BaseXmlSchemaType?.QualifiedName.Name!="AttachUpdateModuleData" ||
            family.Type.GetField("Base")?.FieldType!=typeof(Ep1AttachUpdateModuleData))
            throw new InvalidDataException("EP1 attachment subclass parent differs.");
        bool trap=family.Type==typeof(BoobyTrapUpdateModuleData);
        if(trap && (Marshal.OffsetOf(family.Type,"FireWeaponConditionsOnParent").ToInt32()!=0x170 ||
            Marshal.OffsetOf(family.Type,"PreventEnteringSetStatusOnParent").ToInt32()!=0x190 ||
            Marshal.OffsetOf(family.Type,"SpecialWeapon").ToInt32()!=0x1b0 ||
            Marshal.OffsetOf(family.Type,"PreventEnteringFilter").ToInt32()!=0x1b4 ||
            Marshal.OffsetOf(family.Type,"AllowTriggerOnAllies").ToInt32()!=0x1b8))
            throw new InvalidDataException("EP1 Booby Trap tail differs.");
        var fields=trap ? new StockMetadataGrammar.RuleField[]
        {
            new(0x00000001u,0xE9FD14C9u,2), new(0x05E9630Eu,0xA7BD811Eu,4), new(0x1731FE66u,0xA7BD811Eu,4), new(0x8CFA496Du,0x77E3F4F7u,4), new(0x6524EEC6u,0xD046DD22u,4), new(0x8745BFD1u,0xE46EA3B2u,3)
        } : new StockMetadataGrammar.RuleField[] { new(1,0xE9FD14C9u,2) };
        return new(family.Type.FullName!,family.TypeId,fields);
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
