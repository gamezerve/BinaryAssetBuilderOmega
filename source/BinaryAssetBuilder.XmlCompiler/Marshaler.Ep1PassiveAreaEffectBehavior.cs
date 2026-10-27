using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize all official EP1 passive-effect fields with schema defaults and source-sized modifier references. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PassiveAreaEffectBehaviorModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("EffectRadius","0"),&value->EffectRadius,state);
        Marshal(node.GetAttributeValue("MaxBeneficiaries","0"),&value->MaxBeneficiaries,state);
        Marshal(node.GetAttributeValue("HealingPercentPerSecond","0"),&value->HealingPercentPerSecond,state);
        Marshal(node.GetAttributeValue("HealingPointsPerSecond","0"),&value->HealingPointsPerSecond,state);
        Marshal(node.GetAttributeValue("SurveyDelayFrames","20"),&value->SurveyDelayFrames,state);
        Marshal(node.GetAttributeValue("UpgradeRequired",null),&value->UpgradeRequired,state);
        Ep1ToggleEnum(node.GetAttributeValue("AntiCategoryMask",null),(uint*)&value->AntiCategoryMask,state,typeof(AttributeModifierCategoryType),true);
        Marshal(node.GetAttributeValue("AntiFX",null),&value->AntiFX,state);
        Marshal(node.GetAttributeValue("HealFX",null),&value->HealFX,state);
        Marshal(node.GetAttributeValue("AttachModuleId",null),&value->AttachModuleId,state);
        Marshal(node.GetAttributeValue("DisabledTypesToProcess",null),&value->DisabledTypesToProcess,state);
        Marshal(node.GetChildNodes("Modifier"),&value->Modifier,state);
        Marshal(node.GetChildNode("AllowFilter",null),&value->AllowFilter,state);
        Marshal(node.GetChildNode("ContainerFilter",null),&value->ContainerFilter,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&value->ObjectStatusValidation,state);
        Marshal(node.GetAttributeValue("NonStackable","false"),&value->NonStackable,state);
        Marshal(node.GetAttributeValue("AffectAttached","false"),&value->AffectAttached,state);
        Marshal(node.GetAttributeValue("AffectWoundedOnly","false"),&value->AffectWoundedOnly,state);
        Marshal(node.GetAttributeValue("AffectUnderAttack","false"),&value->AffectUnderAttack,state);
        Marshal(node.GetAttributeValue("AffectContained","false"),&value->AffectContained,state);
        Marshal(node.GetAttributeValue("ShowDebugCircle","false"),&value->ShowDebugCircle,state);
        Marshal(node,(UpdateModuleData*)value,state);
    }
}
