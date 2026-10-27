using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 passive effects use an 80-byte header with optional module/status pointers, two filters and six trailing flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1PassiveAreaEffectBehaviorModuleData
{
    public UpdateModuleData Base;
    public float EffectRadius;
    public uint MaxBeneficiaries;
    public Percentage HealingPercentPerSecond;
    public float HealingPointsPerSecond;
    public uint SurveyDelayFrames;
    public AssetReference<UpgradeTemplate> UpgradeRequired;
    public AttributeModifierCategoryBitFlags AntiCategoryMask;
    public AssetReference<FXList> AntiFX;
    public AssetReference<FXList> HealFX;
    public TypedAssetId<ModuleData>* AttachModuleId;
    public DisabledBitFlags DisabledTypesToProcess;
    public List<AssetReference<AttributeModifier>> Modifier;
    public ObjectFilter* AllowFilter;
    public ObjectFilter* ContainerFilter;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public SageBool NonStackable;
    public SageBool AffectAttached;
    public SageBool AffectWoundedOnly;
    public SageBool AffectUnderAttack;
    public SageBool AffectContained;
    public SageBool ShowDebugCircle;
}
