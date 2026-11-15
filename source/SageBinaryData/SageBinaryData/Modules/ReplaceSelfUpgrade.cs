using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct ReplaceSelfUpgradeModuleData
{
    public UpgradeModuleData Base;
    public Time NewObjectUnpackTime;
    public List<TypedAssetId<GameObject>> ReplacementTemplate;
    public SageBool DisabledDuringUnpack;
    public SageBool CheckBuildAssistant;
}

// Reborn: EP1 packs the replacement options into a one-word bit mask in schema order.
public enum ReplaceSelfUpgradeOptions
{
    COPY_TARGET_OBJECT,
    IGNORE_TERRAIN_RESTRICTIONS,
    COPY_UPGRADES,
    DISABLE_DURING_UNPACK,
    CHECK_BUILD_ASSISTANT
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ReplaceSelfUpgradeOptionsBitFlags
{
    public fixed uint Value[1];
}

// Reborn: EP1 stores ordered weak object IDs after the Upgrade base and option mask.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ReplaceSelfUpgradeModuleData
{
    public const uint WireTypeId = 0x53FBA9B5u;
    public UpgradeModuleData Base;
    public Time NewObjectUnpackTime;
    public ReplaceSelfUpgradeOptionsBitFlags Options;
    public List<TypedAssetId<GameObject>> ReplacementTemplate;
}
