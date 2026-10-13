using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: official EP1 enumeration order; exercised against staged schema declarations.
public enum Ep1ToggleStatusAiHint { TOGGLE_DEFAULT, TOGGLE_ALWAYS, TOGGLE_ONE_PER_TARGET, TOGGLE_DEFENSE_PROTECT_OTHERS, TOGGLE_DEFENSE_SELF_ONLY, TOGGLE_WEAPON, TOGGLE_TRANSFORM, TOGGLE_SHORT_RANGE_WEAPON, TOGGLE_IMMOBILIZE, TOGGLE_LOCKDOWN, TOGGLE_NO_HINT, TOGGLE_AOE_ATTACK, TOGGLE_LEECH_HEALTH_AND_WEAPON, TOGGLE_STEALTH_OTHERS, TOGGLE_SHIELD, TOGGLE_HARPOON }

// Reborn: official EP1 enumeration order; exercised against staged schema declarations.
public enum Ep1MicroManagerState { ALPHA_STRIKE, RETREAT, COMBAT, CLEANUP, SIEGE, GATHER, OPEN_TARGET, HARVESTING, IDLE, DEMOLISH_WALL_OBSTACLE }

// Reborn: official EP1 enumeration order; exercised against staged schema declarations.
public enum Ep1MicroManagerUnitTask { AVAILABLE, CRUSHING, GARRISONING, CLEARING_GARRISON, REPAIRING, REVERSING, CONTROLLED_BY_SPECIAL_POWER, ACTIVATING_SPECIAL_POWER, DEPLOYING, DODGING }

// Reborn: official EP1 enumeration order; exercised against staged schema declarations.
public enum Ep1AiTargetLocation { Water, Land }

// Reborn: official EP1 enumeration order; exercised against staged schema declarations.
public enum Ep1AoeAttackPowerLocationPreference { PREFER_NEAREST, PREFER_CENTER_OF_ENEMIES, PREFER_MAXIMUM_DAMAGE }

// Reborn: official EP1 enumeration order; exercised against staged schema declarations.
public enum Ep1AoeAttackPowerEmanationLocation { EMANATE_FROM_SELF, EMANATE_FROM_TARGET }

// Reborn: EP1 toggle AI native layout; standalone diagnostics only until enclosing module/payload validation.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AISpecialPowerWeaponData
{
    public AssetReference<WeaponTemplate> Weapon;
    public int TotalValueGreaterThan;
    public uint AoeAttackPowerLocationPreference;
    public Time AoeAttackPowerBlockAreaAfterActivationTime;
    public uint AoeAttackEmanationLocation;
    public float ToggleRangeHysteresis;
    public SageBool IsDisablingAttack;
    public SageBool AffectsFriendlies;
    public SageBool AffectsSelf;
    public SageBool ToggleShortRangeTargetForcesTarget;
}

// Reborn: EP1 toggle AI native layout; standalone diagnostics only until enclosing module/payload validation.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AIToggleSpecialPowerInfo
{
    public uint ToggleHint;
    public float* UseOnlyWhenPowerAdvantageIsAbove;
    public float* UseOnlyWhenPowerAdvantageIsBelow;
    public float* UseOnlyWhenCostAdvantageIsAbove;
    public float* UseOnlyWhenCostAdvantageIsBelow;
    public float* NeverUseWhenFurtherThanNUnitsFromTarget;
    public float* NeverUseWhenCloserThanNUnitsFromTarget;
    public float* NeverUseWhenHealthIsBelow;
    public float* MaxZDifferenceFromTarget;
    public uint NeverUseInState;
    public uint NeverUseDuringTask;
    public Time* SelfDefenseDamageLookAheadTime;
    public Time* SelfDefenseDamageLookBehindTime;
    public float* SelfDefenseDamageFraction;
    public int NumOtherAttackersOnTarget;
    public AssetReference<UpgradeTemplate> LeechWeaponUpgrade;
    public float LeechWeaponTargetHealthFraction;
    public TypedAssetId<ModuleData>* LeechWeaponSOLModuleId;
    public float LeechHealthActivatePowerAdvantage;
    public float LeechHealthDeactivatePowerAdvantage;
    public TypedAssetId<ModuleData>* StealthOthersInvisibilityModuleId;
    public int ShieldMinCostProtected;
    public float ShieldMinEnemyDPS;
    public float ShieldCostAdjustPastDistanceFraction;
    public float ShieldCostAdjustMultiplierAtEdge;
    public KindOfBitFlags NeverUseWhenTargetIsAnyKindOf;
    public uint Location;
    public AISpecialPowerWeaponData* StateWeapon;
    public SageBool OnlyUseWhenHaveTarget;
    public SageBool NeverUseWhenClear;
    public SageBool DontUseIfInRange;
    public SageBool EnemyMustBeInVisionRange;
    public SageBool AlwaysUseWhenEnemiesInRange;
}
