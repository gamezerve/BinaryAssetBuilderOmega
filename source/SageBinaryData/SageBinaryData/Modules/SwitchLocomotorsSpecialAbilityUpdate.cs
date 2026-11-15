using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct SwitchLocomotorsSpecialAbilityUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public Time BusyForDuration;
    public float LandingRange;
    public AssetReference<SpecialPowerTemplate> HordeMembersSpecialPowerTemplate;
    public SageBool UseUpgradedLocomotor;
    public SageBool TriggersLanding;
    public SageBool TriggersFlight;
}

// Reborn: EP1 adds the locomotor selector before the three byte flags.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1SwitchLocomotorsSpecialAbilityUpdateModuleData
{
    public const uint WireTypeId = 0x14E031A2u;
    public SpecialAbilityUpdateModuleData Base;
    public Time BusyForDuration;
    public float LandingRange;
    public AssetReference<SpecialPowerTemplate> HordeMembersSpecialPowerTemplate;
    public Ep1RootLocomotorSetType LocomotorToSet;
    public SageBool UseUpgradedLocomotor;
    public SageBool TriggersLanding;
    public SageBool TriggersFlight;
}
