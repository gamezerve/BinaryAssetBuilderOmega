using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: use the EP1 schema ordering without KW's leading NONE member.
public enum AttachUpdateFlagsType
{
    FIND_BEST_PARENT,
    UNCONTAINED_ONLY,
    SAME_PLAYER_ONLY,
    ONE_ATTACH_PER_PARENT,
    STICK_TO_PARENT,
    TELEPORT,
    USE_GEOMETRY,
    USE_PARENT_POSITION_ELEVATION,
    DETACH_WHEN_PARENT_HEALED,
    DETACH_WHEN_PARENT_OUT_OF_SLAVE_RANGE,
    PARENT_MUST_BE_FULL_HEALTH_TO_DETACH,
    SCAN_FOR_NEW_PARENT_WHEN_DETACHED,
    CAN_ATTACH_TO_HORDE_MEMBERS,
    DIE_WHEN_DETACH_ALWAYS,
    DIE_WHEN_DETACH_FROM_PARENT_HEAL,
    DIE_WHEN_PARENT_DIES_FROM_NOT_ME,
    LEECH_DAMAGE_FROM_PARENT,
    MOVE_ORDER_FORCES_DETACH,
    TELEPORT_AND_ALIGN_WITH_PARENT,
    INSTANT_TELEPORT_ON_PARENT_TELEPORT,
    ADD_BOUNCE_TO_PARENT,
    TRANSFER_ON_REPLACE_SELF,
    ABSORB_ALL_DAMAGE,
    DIE_WHEN_PARENT_HAS_FORBIDDEN_STATUS,
    DEFECT_WHEN_PARENT_DEFECTS,
    DETACH_WHEN_IDLE,
    DETACH_WHEN_MULTIPLE_ARE_ATTACHED,
    USE_BONE_POSITION
}

// Reborn: all 28 EP1 attach flags occupy one native 32-bit word.
[StructLayout(LayoutKind.Sequential)]
public struct AttachUpdateFlagsBitFlags
{
    public const int Count = 28;
    public const int BitsInSpan = 32;
    public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;
    public unsafe fixed uint Value[NumSpans];
}

// Reborn: restore official RA3 inline/pointer masks and references, then append EP1's bone string.
[StructLayout(LayoutKind.Sequential)]
public struct AttachUpdateModuleData
{
    public UpdateModuleData Base;
    public ObjectStatusBitFlags ParentStatus;
    public ObjectStatusBitFlags ParentStatusAttached;
    public ObjectStatusBitFlags AttachedObjectStatus;
    public ModelConditionBitFlags AttachedModelConditions;
    public ObjectStatusBitFlags ForbiddenParentStatus;
    public ObjectStatusBitFlags IgnoreForbiddenParentStatusStatus;
    public unsafe ObjectStatusBitFlags* ParentStatusToCopy;
    public unsafe ObjectStatusBitFlags* ParentStatusToPrefer;
    public float Range;
    public float CloseEnoughRange;
    public AssetReference<BaseAssetType> ParentOwnerAttachmentEvaEvent;
    public AssetReference<BaseAssetType> ParentAllyAttachmentEvaEvent;
    public AssetReference<BaseAssetType> ParentEnemyAttachmentEvaEvent;
    public AssetReference<FXList> AttachFXList;
    public AssetReference<FXList> DetachFXList;
    public AssetReference<BaseAssetType> ParentOwnerDiedEvaEvent;
    public AssetReference<BaseAssetType> ParentAllyDiedEvaEvent;
    public AssetReference<BaseAssetType> ParentEnemyDiedEvaEvent;
    public Time InitialAttachDelay;
    public Time IdleScanDelay;
    public AttachUpdateFlagsBitFlags Flags;
    public DeathType ParentDeathTypeToListenFor;
    public ObjectStatusBitFlags NoDieIfStatusActive;
    public float ParentPositionElevation;
    public float BounceAmount;
    public Time BounceTimeout;
    public unsafe DamageBitFlags* DamageTypesToNotLeech;
    public unsafe DeathBitFlags* DeathTypesToNotLeech;
    public DeathType ForbiddenParentStatusDieDeathType;
    public unsafe ObjectFilter* ObjectFilter;
    public List<AssetReference<AttributeModifier>> ModifierToLeechFromParent;
    public AnsiString AttachBoneName;
}
