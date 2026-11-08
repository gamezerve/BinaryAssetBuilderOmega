using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: official bombard delay bounds store seconds in two native Time fields.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CoverageTimeRange { public Time Low; public Time High; }

// Reborn: original balloon bombardment retains its optional status pointer, strong weapon and inline ranges before the byte flag.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct BombardAreaUpdateModuleData
{
    public UpdateModuleData Base;
    public Angle IncomingAngle;
    public AssetReference<WeaponTemplate> WeaponName;
    public int MaxShots;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public Ep1CoverageTimeRange TimeDelayRange;
    public RealRange RadiusSpreadRange;
    public SageBool UsePlayerStore;
}

// Reborn: unified capture keeps its complete inherited money/slave fields before its alliance mode.
public enum UnifiedControlType { SAME_PLAYER, ALLIED_PLAYERS }
[StructLayout(LayoutKind.Sequential)]
public struct UnifiedControlBehaviorModuleData
{
    public CaptureBehaviorModuleData Base;
    public UnifiedControlType UnificationType;
}

// Reborn: each ownership threshold imports its concrete upgrade without hashing or skipping the reference.
[StructLayout(LayoutKind.Sequential)]
public struct UpgradeMap { public int OwnershipAmount; public AssetReference<UpgradeTemplate> ProvidedUpgrade; }

// Reborn: the original veterancy structure proves this source-sized upgrade map on the WorldBuilder stream.
[StructLayout(LayoutKind.Sequential)]
public struct CaptureAndGiveUpgradeBehaviorModuleData
{
    public UnifiedControlBehaviorModuleData Base;
    public List<UpgradeMap> UpgradeMap;
}
