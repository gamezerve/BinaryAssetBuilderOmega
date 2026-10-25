using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: EP1 exposes each button as its own native asset with one concrete data pointer.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1PlayerPowerButtonTemplate { public BaseAssetType Base; public AssetReference<SpecialPowerTemplate> SpecialPower; public Ep1PlayerPowerButtonTemplateData* Data; }
// Reborn: the concrete class word precedes the shared state-data import; no inline KW ButtonState is stored.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PlayerPowerButtonTemplateData : IPolymorphic { public uint TypeId; public AssetReference<BaseAssetType> StateData; }
// Reborn: numeric outline values follow explicit schema forceValue declarations.
public enum Ep1ButtonOutline { UNKNOWN=-1, NONE=0, SELECTED, HOVER_SELECT, HOVER_ATTACK }
// Reborn: EP1 cursor fields are imports and the tint is an optional RGB allocation.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1TargetedPlayerPowerButtonTemplateData { public Ep1PlayerPowerButtonTemplateData Base; public AssetReference<BaseAssetType> ValidTargetCursor; public AssetReference<BaseAssetType> InvalidTargetCursor; public AssetReference<BaseAssetType> RadiusCursor; public Ep1ButtonOutline ValidTargetOutline; public RGBColor* ValidTargetTintColor; }
// Reborn: multi-target buttons add weak marker handles, targeting scalars and a source-sized cursor list.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1MultipleTargetsTargetedPlayerPowerButtonTemplateData { public Ep1TargetedPlayerPowerButtonTemplateData Base; public TypedAssetId<GameObject> TargetMarkerTemplate; public TypedAssetId<FXList> TargetMarkerFXList; public uint TargetCount; public float MinTargetDistance; public List<AssetReference<BaseAssetType>> AdditionalRadiusCursor; }
// Reborn: chained powers carry independent source-sized power, radius and valid-cursor import lists.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1MultiplePowersTargetedPlayerPowerButtonTemplateData { public Ep1TargetedPlayerPowerButtonTemplateData Base; public List<AssetReference<SpecialPowerTemplate>> AdditionalSpecialPowerTemplate; public List<AssetReference<BaseAssetType>> AdditionalRadiusCursor; public List<AssetReference<BaseAssetType>> AdditionalValidTargetCursor; }
