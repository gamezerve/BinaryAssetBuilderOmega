using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: retain the original XSD enumeration order in the native music record.
public enum Ep1MusicStateSetCategory { STANDARD, SCRIPTED }

// Reborn: retain the original XSD enumeration order in the native music record.
public enum Ep1MusicStateChange { ENTER, EXIT }

// Reborn: retain the original XSD enumeration order in the native music record.
public enum Ep1MusicComparison { LESS_THAN, LESS_THAN_OR_EQUAL, EQUAL, GREATER_THAN_OR_EQUAL, GREATER_THAN }

// Reborn: retain the original XSD enumeration order in the native music record.
public enum Ep1MusicScreenStatus { ON_SCREEN, OFF_SCREEN, ON_OR_OFF_SCREEN }

// Reborn: serialize the complete original EP1 music root, including its typed imports and weak identities.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1PathMusicGameDynamicTransition
{
    public BaseAssetType Base;
    public TypedAssetId<BaseAssetType> DestinationState;
    public AssetReference<MusicScriptConditionNuggetBase> Condition;
}

// Reborn: serialize the complete original EP1 music root, including its typed imports and weak identities.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1PathMusicGameDynamicStateSet
{
    public BaseAssetType Base;
    public Ep1MusicStateSetCategory Category;
    public List<AssetReference<BaseAssetType>> State;
}

// Reborn: serialize the complete original EP1 music root, including its typed imports and weak identities.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MusicScriptConditionNugget_PathMusicStateLastActive
{
    public MusicScriptConditionNuggetBase Base;
    public TypedAssetId<BaseAssetType> TrackedState;
    public Ep1MusicStateChange TrackedStateChange;
    public Time TimeSinceLastActive;
    public Ep1MusicComparison TimeSicneLastActiveOp;
}

// Reborn: serialize the complete original EP1 music root, including its typed imports and weak identities.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MusicScriptConditionNugget_PathMusicStateCurrentlyActive
{
    public MusicScriptConditionNuggetBase Base;
    public TypedAssetId<BaseAssetType> TrackedState;
}

// Reborn: serialize the complete original EP1 music root, including its typed imports and weak identities.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MusicScriptConditionNugget_EngagementCount
{
    public MusicScriptConditionExpensiveNuggetBase Base;
    public int Count;
    public Time Time;
    public Ep1MusicScreenStatus RequiredScreenStatus;
    public AssetReference<ObjectFilterAsset>* TargetFilter;
    // Reborn: original EP1 places the two local flags after the optional filter cell.
    public SageBool CountOnlyEngagementsAgainstTheLocalPlayer;
    public SageBool CountOnlyEngagementsByTheLocalPlayer;
}

// Reborn: serialize the complete original EP1 music root, including its typed imports and weak identities.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MusicScriptConditionNugget_LocalPlayerKillRatio
{
    public MusicScriptConditionExpensiveNuggetBase Base;
    public Time Time;
    public float Ratio;
    public Ep1MusicComparison RatioOp;
}

// Reborn: serialize the complete original EP1 music root, including its typed imports and weak identities.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MusicScriptConditionNugget_HitBySuperWeapon
{
    public MusicScriptConditionNuggetBase Base;
    public TypedAssetId<WeaponTemplate> TrackedWeapon;
    public Time Time;
    public int Count;
    public AssetReference<ObjectFilterAsset>* TargetFilter;
    // Reborn: original EP1 stores the hit flag after the optional filter.
    public SageBool CountOnlyHitsAgainstTheLocalPlayer;
}

