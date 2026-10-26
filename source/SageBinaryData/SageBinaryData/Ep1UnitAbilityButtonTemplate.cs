using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: EP1 AssembleStructureUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AssembleStructureUnitAbilityButtonTemplateData { public Ep1ProduceStructureUnitAbilityButtonBaseTemplateData Base; }

// Reborn: EP1 BaseSpecialPowerUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1BaseSpecialPowerUnitAbilityButtonTemplateData { public Ep1UnitAbilityButtonTemplateData Base; }

// Reborn: EP1 BuildUnitUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1BuildUnitUnitAbilityButtonTemplateData { public Ep1UnitAbilityButtonTemplateData Base; }

// Reborn: EP1 BuildWallUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1BuildWallUnitAbilityButtonTemplateData { public Ep1SingleStateUnitAbilityButtonTemplateData Base; }

// Reborn: EP1 EvacuateUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1EvacuateUnitAbilityButtonTemplateData { public Ep1SingleStateUnitAbilityButtonTemplateData Base; }

// Reborn: EP1 MultiplePowersTargetedSpecialPowerUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MultiplePowersTargetedSpecialPowerUnitAbilityButtonTemplateData { public Ep1TargetedSpecialPowerUnitAbilityButtonTemplateData Base; public List<AssetReference<BaseAssetType>> AdditionalSpecialPowerTemplate; public List<AssetReference<BaseAssetType>> AdditionalRadiusCursor; public List<AssetReference<BaseAssetType>> AdditionalValidTargetCursor; }

// Reborn: EP1 ObjectUpgradeUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ObjectUpgradeUnitAbilityButtonTemplateData { public Ep1SingleStateUnitAbilityButtonTemplateData Base; }

// Reborn: EP1 PlayerUpgradeUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1PlayerUpgradeUnitAbilityButtonTemplateData { public Ep1SingleStateUnitAbilityButtonTemplateData Base; }

// Reborn: EP1 ProduceStructureUnitAbilityButtonBaseTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ProduceStructureUnitAbilityButtonBaseTemplateData { public Ep1SpecialPowerUnitAbilityButtonTemplateData Base; }

// Reborn: EP1 SingleStateUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SingleStateUnitAbilityButtonTemplateData { public Ep1UnitAbilityButtonTemplateData Base; public AssetReference<BaseAssetType> StateData; }

// Reborn: EP1 SpecialPowerUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpecialPowerUnitAbilityButtonTemplateData { public Ep1BaseSpecialPowerUnitAbilityButtonTemplateData Base; public AssetReference<BaseAssetType> StateData; }

// Reborn: EP1 TargetedSpecialPowerUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1TargetedSpecialPowerUnitAbilityButtonTemplateData { public Ep1SpecialPowerUnitAbilityButtonTemplateData Base; public AssetReference<BaseAssetType> ValidTargetCursor; public AssetReference<BaseAssetType> InvalidTargetCursor; public AssetReference<BaseAssetType> RadiusCursor; public Ep1ButtonOutline ValidTargetOutline; public RGBColor* ValidTargetTintColor; }

// Reborn: EP1 ToggleSpecialPowerUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ToggleSpecialPowerUnitAbilityButtonTemplateData { public Ep1BaseSpecialPowerUnitAbilityButtonTemplateData Base; public List<AssetReference<BaseAssetType>> StateData; }

// Reborn: EP1 UnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UnitAbilityButtonTemplateData : IPolymorphic { public uint TypeId; }

// Reborn: EP1 UnpackUnitAbilityButtonTemplateData retains source-owned inherited fields and list sizes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UnpackUnitAbilityButtonTemplateData { public Ep1ProduceStructureUnitAbilityButtonBaseTemplateData Base; }

// Reborn: standalone unit-button roots hold a real command import and one concrete Data pointer.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UnitAbilityButtonTemplate { public BaseAssetType Base; public AssetReference<LogicCommand> LogicCommand; public Ep1UnitAbilityButtonTemplateData* Data; }
