using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: joypad enum ordinals retain explicit original schema values.
public enum Ep1JoypadOutlineDrawType { UNKNOWN=-1, NONE=0, SELECTED=1, HOVER_SELECT=2, HOVER_ATTACK=3 }

// Reborn: joypad enum ordinals retain explicit original schema values.
public enum Ep1JoypadProductionQueueType { INVALID=-1, MAIN_STRUCTURE=0, OTHER_STRUCTURE=1, INFANTRY=2, VEHICLE=3, AIRCRAFT=4, UPGRADE=5, WATERCRAFT=6 }

// Reborn: joypad enum ordinals retain explicit original schema values.
public enum Ep1JoypadStance { GUARD=0, AGGRESSIVE=1, HOLD_POSITION=2, HOLD_FIRE=3 }

// Reborn: joypad enum ordinals retain explicit original schema values.
public enum Ep1JoypadAIStrategicDirective { Invalid=-1, Normal=0, Assault=1, HoldPosition=2, HoldObject=3, Help=4 }

// Reborn: joypad enum ordinals retain explicit original schema values.
public enum Ep1JoypadCommandMode { Sell=0, TogglePower=1, ToggleRepair=2 }

// Reborn: EP1 JoypadCommandBarButtonTemplateData retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateData : IPolymorphic { public uint TypeId; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataAIStrategicDirective retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataAIStrategicDirective { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; public Ep1JoypadAIStrategicDirective Directive; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataActivateHotSpot retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataActivateHotSpot { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataAssembleStructure retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataAssembleStructure { public Ep1JoypadCommandBarButtonTemplateDataProduceStructureBase Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataAutoControlGroup retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataAutoControlGroup { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; public ObjectFilter ObjectFilter; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataBaseSpecialPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataBaseSpecialPower { public Ep1JoypadCommandBarButtonTemplateDataUnitAbility Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataEnterCommandMode retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataEnterCommandMode { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; public Ep1JoypadCommandMode CommandMode; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataEvacuate retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataEvacuate { public Ep1JoypadCommandBarButtonTemplateDataUnitAbility Base; public AssetReference<BaseAssetType> StateData; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataMultiplePowersTargetedSpecialPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataMultiplePowersTargetedSpecialPower { public Ep1JoypadCommandBarButtonTemplateDataTargetedSpecialPower Base; public List<AssetReference<BaseAssetType>> AdditionalSpecialPowerTemplate; public List<AssetReference<BaseAssetType>> AdditionalRadiusCursor; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataObjectUpgrade retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataObjectUpgrade { public Ep1JoypadCommandBarButtonTemplateDataUpgrade Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenControlGroups retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenControlGroups { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenCoopCommunicator retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenCoopCommunicator { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenPlayerPowers retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenPlayerPowers { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenPlayerTechStore retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenPlayerTechStore { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenStances retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenStances { public Ep1JoypadCommandBarButtonTemplateData Base; public AssetReference<BaseAssetType> MixedStanceStateData; public List<Ep1JoypadCommandBarButtonTemplateDataOpenStancesStanceSettings> StanceSettings; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenStancesStanceSettings retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenStancesStanceSettings { public Ep1JoypadStance Stance; public AssetReference<BaseAssetType> StateData; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenUnitAbilities retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenUnitAbilities { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataOpenUnitsMenu retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataOpenUnitsMenu { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataPlaceReadyStructure retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataPlaceReadyStructure { public Ep1JoypadCommandBarButtonTemplateData Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataPlayerControlGroup retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataPlayerControlGroup { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; public uint GroupShortcut; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataPlayerMultiplePowersTargetedPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataPlayerMultiplePowersTargetedPower { public Ep1JoypadCommandBarButtonTemplateDataPlayerTargetedPower Base; public List<AssetReference<BaseAssetType>> AdditionalSpecialPowerTemplate; public List<AssetReference<BaseAssetType>> AdditionalRadiusCursor; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataPlayerMultipleTargetsTargetedPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataPlayerMultipleTargetsTargetedPower { public Ep1JoypadCommandBarButtonTemplateDataPlayerTargetedPower Base; public TypedAssetId<GameObject> TargetMarkerTemplate; public TypedAssetId<FXList> TargetMarkerFXList; public uint TargetCount; public float MinTargetDistance; public List<AssetReference<BaseAssetType>> AdditionalRadiusCursor; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataPlayerPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataPlayerPower { public Ep1JoypadCommandBarButtonTemplateData Base; public AssetReference<BaseAssetType> SpecialPower; public AssetReference<BaseAssetType> StateData; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataPlayerTargetedPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataPlayerTargetedPower { public Ep1JoypadCommandBarButtonTemplateDataPlayerPower Base; public AssetReference<BaseAssetType> RadiusCursor; public Ep1JoypadOutlineDrawType ValidTargetOutline; public RGBColor* ValidTargetTintColor; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataPlayerUpgrade retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataPlayerUpgrade { public Ep1JoypadCommandBarButtonTemplateDataUpgrade Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataProduceStructureBase retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataProduceStructureBase { public Ep1JoypadCommandBarButtonTemplateDataSpecialPower Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataProductionQueueType retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataProductionQueueType { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; public Ep1JoypadProductionQueueType QueueType; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataRedAlert retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataRedAlert { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataSetStance retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataSetStance { public Ep1JoypadCommandBarButtonTemplateDataSingleState Base; public Ep1JoypadStance Stance; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataSingleState retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataSingleState { public Ep1JoypadCommandBarButtonTemplateData Base; public AssetReference<BaseAssetType> StateData; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataSpecialPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataSpecialPower { public Ep1JoypadCommandBarButtonTemplateDataBaseSpecialPower Base; public AssetReference<BaseAssetType> StateData; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataTargetedSpecialPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataTargetedSpecialPower { public Ep1JoypadCommandBarButtonTemplateDataSpecialPower Base; public AssetReference<BaseAssetType> RadiusCursor; public Ep1JoypadOutlineDrawType ValidTargetOutline; public RGBColor* ValidTargetTintColor; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataToggleSpecialPower retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataToggleSpecialPower { public Ep1JoypadCommandBarButtonTemplateDataBaseSpecialPower Base; public List<AssetReference<BaseAssetType>> StateData; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataUnitAbility retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataUnitAbility { public Ep1JoypadCommandBarButtonTemplateData Base; public AssetReference<BaseAssetType> LogicCommand; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataUnpack retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataUnpack { public Ep1JoypadCommandBarButtonTemplateDataProduceStructureBase Base; }

// Reborn: EP1 JoypadCommandBarButtonTemplateDataUpgrade retains inherited fields, attribute order and source-sized children.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplateDataUpgrade { public Ep1JoypadCommandBarButtonTemplateDataUnitAbility Base; public AssetReference<BaseAssetType> StateData; }

// Reborn: each joypad button asset owns a single schema-selected concrete Data record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1JoypadCommandBarButtonTemplate { public BaseAssetType Base; public Ep1JoypadCommandBarButtonTemplateData* Data; }
