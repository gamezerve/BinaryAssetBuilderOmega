using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Uprising aura uses a 112-byte upgrade header with disabled flags and eight trailing booleans.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AttributeModifierAuraUpdateModuleData
{
    public UpgradeModuleData Base;
    public AssetReference<AttributeModifier> AttributeModifierName;
    public Time RefreshDelay;
    public float Range;
    public AttributeModifierAuraUpdateRequiredBitFlags RequiredConditions;
    public AttributeModifierCategoryBitFlags AntiCategory;
    public AssetReference<FXList> AntiFX;
    public int MaxActiveRank;
    public ObjectStatusBitFlags RequiredObjectStatusFlags;
    public DisabledBitFlags DisabledTypesToProcess;
    public unsafe ObjectFilter* ObjectFilter;
    public SageBool AllowPowerWhenAttacking;
    public SageBool InitiallyActive;
    public SageBool RunWhileDead;
    public SageBool AllowSelf;
    public SageBool AffectContainedOnly;
    public SageBool AffectHordeMembersOnly;
    public SageBool AffectAttacheeOnly;
    public SageBool ShowDebugCircle;
}
