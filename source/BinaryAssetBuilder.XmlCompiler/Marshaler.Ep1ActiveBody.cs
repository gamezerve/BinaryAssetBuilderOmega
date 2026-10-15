using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit EP1-only allocation selects the reviewed body ABI and concrete wire ID without altering legacy polymorphic dispatch. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1ActiveBody(Node node,BodyModuleData** objT,Tracker state)
    {
        if(node is null || node.GetAttributeValue("TypeId",null)?.GetText() != "1510882715")throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Explicit EP1 ActiveBody class ID required.");
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1ActiveBodyModuleData),1u))Marshal(node,(Ep1ActiveBodyModuleData*)*objT,state);
        // Reborn: hashing the separate C# name would be wrong; the bound EP1 schema retains ActiveBodyModuleData's original concrete ID.
        uint type=0x5A0E3D9B;state.InplaceEndianToPlatform(&type);*(uint*)*objT=type;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve ordered EP1 damage-range percentages and the already-normalized modifier occurrence index. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActiveBodyDamageRangeModifier* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Min",null),&objT->Min,state);
        Marshal(node.GetAttributeValue("Max",null),&objT->Max,state);
        Marshal(node.GetAttributeValue("AttributeModifier",null),&objT->AttributeModifier,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: opt-in EP1 body marshalling uses official percentage/default semantics without replacing legacy KW Body dispatch. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActiveBodyModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("MaxHealth","0"),&objT->MaxHealth,state);
        Marshal(node.GetAttributeValue("MaxHealthDamagedPercentage","66.666666"),&objT->MaxHealthDamagedPercentage,state);
        Marshal(node.GetAttributeValue("MaxHealthReallyDamagedPercentage","33.333333"),&objT->MaxHealthReallyDamagedPercentage,state);
        Marshal(node.GetAttributeValue("InitialHealth","-1.0"),&objT->InitialHealth,state);
        Marshal(node.GetAttributeValue("DodgePercent","0"),&objT->DodgePercent,state);
        Marshal(node.GetAttributeValue("EnteringDamagedTransitionTime","0s"),&objT->EnteringDamagedTransitionTime,state);
        Marshal(node.GetAttributeValue("EnteringReallyDamagedTransitionTime","0s"),&objT->EnteringReallyDamagedTransitionTime,state);
        Marshal(node.GetAttributeValue("GrabObject",null),&objT->GrabObject,state);
        Marshal(node.GetAttributeValue("HealingBuffFX",null),&objT->HealingBuffFX,state);
        Marshal(node.GetAttributeValue("DamagedAttributeModifier",null),&objT->DamagedAttributeModifier,state);
        Marshal(node.GetAttributeValue("ReallyDamagedAttributeModifier",null),&objT->ReallyDamagedAttributeModifier,state);
        Marshal(node.GetAttributeValue("CheerRadius","200.0"),&objT->CheerRadius,state);
        Marshal(node.GetAttributeValue("BurningDeathFX",null),&objT->BurningDeathFX,state);
        Marshal(node.GetChildNodes("DamageRangeModifier"),&objT->DamageRangeModifier,state);
        Marshal(node.GetAttributeValue("RemoveUpgradesOnDeath","false"),&objT->RemoveUpgradesOnDeath,state);
        Marshal(node.GetAttributeValue("BurningDeathBehavior","false"),&objT->BurningDeathBehavior,state);
        Marshal(node,(BodyModuleData*)objT,state);
    }
}
