using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct InvisibilitySpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    // Reborn: RA3/Uprising use the shared template asset and an optional relocated object filter.
    public Relo.AssetReference<BaseAssetType> InvisibilityTemplate;
    public float BroadcastRadius;
    public Time Duration;
    public ObjectFilter* ObjectFilter;
    public SageBool Permanent;
}
