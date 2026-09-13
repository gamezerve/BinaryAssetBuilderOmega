using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData;

public enum AttributeModifierCategoryType
{
    NONE,
    LEADERSHIP,
    FORMATION,
    SPELL,
    WEAPON,
    STRUCTURE,
    LEVEL,
    BUFF,
    DEBUFF,
    STUN,
    INNATE_ARMOR,
    INNATE_DAMAGEMULT,
    INNATE_VISION,
    INNATE_AUTOHEAL,
    INNATE_HEALTH,
    SHRINK
}

public enum AttributeType
{
    NONE,
    ARMOR,
    DAMAGE_ADD,
    DAMAGE_MULT,
    RESIST_FEAR,
    RESIST_TERROR,
    EXPERIENCE,
    RANGE,
    SPEED,
    CRUSH_DECELERATE,
    RESIST_KNOCKBACK,
    SPELL_DAMAGE,
    RECHARGE_TIME,
    PRODUCTION,
    PRODUCTION_COST,
    HEALTH,
    HEALTH_MULT,
    VISION,
    BOUNTY_PERCENTAGE,
    MIN_CRUSH_VELOCITY_PERCENTAGE,
    AUTO_HEAL,
    SHROUD_CLEARING,
    RATE_OF_FIRE,
    DAMAGE_STRUCTURE_BOUNTY_ADD,
    CRUSHER_LEVEL,
    COMMAND_POINT_BONUS,
    CRUSHABLE_LEVEL,
    CRUSHED_DECELERATE,
    INVULNERABLE,
    SUPPRESSABILITY,
    RESIST_EMP,
    POWER_BOOST,
    AREA_OF_EFFECT,
    COLLISION_GEOMETRY_SIZE_MULT,
    BROADCAST_RANGE,
    SPECIAL_ABILTY_RANGE,
    RADIATION_ARMOR
}

[StructLayout(LayoutKind.Sequential)]
public struct AttributeModifierCategoryBitFlags
{
    public const int Count = 15;
    public const int BitsInSpan = 32;
    public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

    public unsafe fixed uint Value[NumSpans];
}

[StructLayout(LayoutKind.Sequential)]
public struct AttributeModifierListType
{
    public AttributeType Type;
    public Percentage Value;
}

[StructLayout(LayoutKind.Sequential)]
public struct AttributeModifier
{
    public BaseInheritableAsset Base;
    public AttributeModifierCategoryType Category;
    /// <summary>
    /// Duration of zero is infinite.
    /// </summary>
    public Time Duration;
    public AssetReference<FXList> StartFX;
    public AssetReference<FXList> EndFX;
    public unsafe ModelConditionBitFlags* ModelConditionsSet;
    public unsafe ModelConditionBitFlags* ModelConditionsClear;
    public unsafe ObjectStatusBitFlags* ObjectStatusToSet;
    public uint StackingLimit;
    public ArmorSetType ArmorSetType;
    public AssetReference<ShaderOverride> Shader;
    public List<AttributeModifierListType> Modifier;
    public SageBool ReplaceInCategoryIfLongest;
    public SageBool IgnoreIfAnticategoryActive;
}

// AttributeModifier only stores an asset-reference pointer to this runtime type.
[StructLayout(LayoutKind.Sequential)]
public struct ShaderOverride
{
    private byte _opaque;
}
