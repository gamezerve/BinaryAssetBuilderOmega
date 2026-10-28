using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: EP1 places the bone-name header before its filter pointer and modifier list; the legacy attach model remains separate.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AttachUpdateModuleData
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
    public AnsiString AttachBoneName;
    public unsafe ObjectFilter* ObjectFilter;
    public List<AssetReference<AttributeModifier>> ModifierToLeechFromParent;
}
