using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

public enum FormationType
{
    MAIN
}

public enum HordeMeleeType
{
    AMOEBA
}

[StructLayout(LayoutKind.Sequential)]
public struct HordeMeleeBehaviorData
{
    public HordeMeleeType Type;
}

[StructLayout(LayoutKind.Sequential)]
public struct PositionAndLeaderType
{
    public float X;
    public float Y;
    public int LeaderRank;
    public int LeaderIndex;
}

[StructLayout(LayoutKind.Sequential)]
public struct RankInfoType
{
    public int RankID;
    public TypedAssetId<GameObject> UnitType;
    public List<PositionAndLeaderType> Position;
    // Reborn: RA3/EP1 rank records end with the position list; no KW weapon-condition pointers.
}

[StructLayout(LayoutKind.Sequential)]
public struct BannerCarrierPosType
{
    public TypedAssetId<GameObject> UnitType;
    public Coord2D Pos;
}

#if KANESWRATH
[StructLayout(LayoutKind.Sequential)]
public struct OnDeathBehaviorType
{
    /// <summary>
    /// RequiredStatus does a check for ALL bitflags.
    /// </summary>
    public ObjectStatusBitFlags RequiredStatus;
    public List<AssetReference<ObjectCreationList>> OCL;
}

[StructLayout(LayoutKind.Sequential)]
public struct WiggleBehaviorType
{
    public float WiggleAmplitude;
    public float WiggleFrequency;
    public float WiggleIdleAmplitude;
    public float WiggleIdleFrequency;
}
#endif

[StructLayout(LayoutKind.Sequential)]
public struct HordeContainModuleData
{
    public TransportContainModuleData Base;
    public FormationType Formation;
    public TypedAssetId<GameObject> AlternateFormation;
    public LocomotorSetType ForcedLocomotorSet;
    public float MeleeAttackLeashDistance;
    public float GeometryFrontAngleRadians;
    // Reborn: EVA notification is a four-byte asset import; its standalone model remains unported.
    public AssetReference<BaseAssetType> EvaEventLastMemberDeath;
    public float FrontAngle;
    public float FlankedDelaySeconds;
    public float FlankedDurationSeconds;
    public uint MinimumHordeSize;
    public Time BackupMinDelayTime;
    public Time BackupMaxDelayTime;
    public float BackupMinDistance;
    public float BackupMaxDistance;
    public float BackupPercentage;
    public float RadiusCowerOverride;
    public float VisionOverrideRear;
    public float VisionOverrideSide;
    public ObjectStatusBitFlags ForbiddenCoverStatus;
    public unsafe Coord2D* RandomOffset;
    public List<RankInfoType> RankInfo;
    public List<int> RankThatStopsAdvance;
    public List<int> RankToReleaseWhenAttacking;
    public unsafe Coord2D* LeaderPosition;
    public List<BannerCarrierPosType> BannerCarrierPosition;
    public List<TypedAssetId<GameObject>> BannerCarriersAllowed;
    public List<TypedAssetId<GameObject>> LeadersAllowed;
    public List<AssetReference<AttributeModifier>> AttributeModifier;
    // Reborn: RA3/EP1 has no KW melee, on-death, wiggle or extra banner behavior fields.
    public SageBool UseSlowHordeMovement;
    public SageBool SpawnBannerCarrierImmediately;
    public SageBool BannerCarrierByUpgradeOnly;
}
