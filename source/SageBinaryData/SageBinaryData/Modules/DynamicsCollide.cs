using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct DynamicsCollideModuleData
{
    public BehaviorModuleData Base;
}

[StructLayout(LayoutKind.Sequential)]
public struct MagnitudeSoundSelectorEntry
{
    public float MinimumMagnitude;
    public AssetReference<BaseAudioEventInfo, AudioEventInfo> Sound;
}

[StructLayout(LayoutKind.Sequential)]
public struct MagnitudeSoundSelectorTable
{
    public List<MagnitudeSoundSelectorEntry> Entry;
}

[StructLayout(LayoutKind.Sequential)]
public struct AudioDynamicsCollideModuleData
{
    public DynamicsCollideModuleData Base;
    public float MinimumImpactVelocity;
    public MagnitudeSoundSelectorTable MagnitudeSoundSelector;
}

[StructLayout(LayoutKind.Sequential)]
public struct DamageDynamicsCollideModuleData
{
    public DynamicsCollideModuleData Base;
    public float MaxMagnitude;
    public List<DamageNuggetType> DamageNugget;
}
