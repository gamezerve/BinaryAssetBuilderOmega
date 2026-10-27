using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 projectile nuggets add an optional barrel override pointer after the veterancy list (68 bytes; retail torpedo witnesses relocate +52, +60 and +64).
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ProjectileNuggetType
{
    public WeaponEffectNugget Base;
    public AssetReference<WeaponTemplate> WarheadTemplate;
    public AssetReference<GameObject> ProjectileTemplate;
    public WeaponSlotType WeaponLaunchBoneSlotOverride;
    public Coord3D* AttackOffset;
    public List<VeteranProjectile> VeterancyProjectiles;
    public int* SpecificBarrelOverride;
}

// Reborn: EP1 suppression follows the official schema order (no KW Radius copy) and adds taper/arc fields (60 bytes, retail-qualified offsets 40..56).
[StructLayout(LayoutKind.Sequential)]
public struct Ep1SuppressionNuggetType
{
    public WeaponEffectNugget Base;
    public float Suppression;
    public Time DurationSeconds;
    public float SuppressionTaperOff;
    public Angle SuppressionArc;
    public SageBool SuppressionArcInverted;
}

// Reborn: EP1 meta-impact has no KW HeroResist word; the filter pointer precedes five packed flags (96 bytes, retail-qualified offsets 40..92).
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MetaImpactNuggetType
{
    public WeaponEffectNugget Base;
    public float ShockWaveAmount;
    public float ShockWaveRadius;
    public Angle ShockWaveArc;
    public float ShockWaveTaperOff;
    public float ShockWaveSpeed;
    public float ShockWaveZMult;
    public float CyclonicFactor;
    public Time ShockwaveDelaySeconds;
    public float Suppression;
    public float ShockWaveClearWaveMult;
    public float ShockWaveClearFlingHeight;
    public ObjectFilter* KillObjectFilter;
    public SageBool ShockWaveArcInverted;
    public SageBool InvertShockWave;
    public SageBool FlipDirection;
    public SageBool OnlyWhenJustDied;
    public SageBool ShockWaveClearRadius;
}
