using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: enum ordinals follow the official EP1 TelefragType schema.
public enum TelefragType { NONE, HEALTH_SUBTRACTION, INSTANT_DEATH_TELEPORTER_ONLY, SWAP }

// Reborn: retail teleport-update metadata anchors this sixty-byte header, optional shader import and independently allocated object filters.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TeleportStoredObjectsUpdateModuleData
{
    public UpdateModuleData Base;
    public AssetReference<FXList> TriggerFX;
    public AssetReference<FXList> TargetFX;
    public Time FadeInTime;
    public Time InitialDelayTime;
    public Time DelayTime;
    public float ScaleFadeInTimeByDistance;
    public TelefragType Telefrag;
    public AssetReference<SpecialPowerTemplate> SpecialPowerTemplate;
    public AssetReference<ShaderOverride>* Shader;
    public DisabledBitFlags DisabledTypesToProcess;
    public ObjectFilter* InstaKillObjectFilter;
    public ObjectFilter* CollidesWithOnTeleport;
    public SageBool GetObjectsCurrentlyAtStorePosition;
}
