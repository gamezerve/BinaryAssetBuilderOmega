using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal EP1 production scalars and stock-qualified upgrade translations; other populated modifier families remain explicit errors. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ProductionUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        // Reborn: populated quantity/production modifier lists still need their own stock-qualified item ABI.
        foreach(string child in new[]{"QuantityModifier","ProductionModifier"})
            if(node.GetChildNode(child,null)!=null)throw new System.IO.InvalidDataException("EP1 ProductionUpdate "+child+" is not implemented yet; remove it or wait for its native layout support.");
        if(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.BonusForType),null)!=null)throw new System.IO.InvalidDataException("EP1 ProductionUpdate BonusForType is not implemented yet.");
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.MaxQueueEntries),"99"),&objT->MaxQueueEntries,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.NumDoorAnimations),"0"),&objT->NumDoorAnimations,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.DoorOpeningTime),"0s"),&objT->DoorOpeningTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.DoorWaitOpenTime),"0s"),&objT->DoorWaitOpenTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.DoorCloseTime),"0s"),&objT->DoorCloseTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.ConstructionCompleteDuration),"0"),&objT->ConstructionCompleteDuration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.DisabledTypesToProcess),null),&objT->DisabledTypesToProcess,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.UnitInvulnerableTime),"0"),&objT->UnitInvulnerableTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.SpecialPrepModelconditionTime),"0"),&objT->SpecialPrepModelconditionTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.Type),nameof(Ep1ProductionQueueType.INVALID)),&objT->Type,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.ProductionTimeDelayScalar),"0s"),&objT->ProductionTimeDelayScalar,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.NewObjectFadeInTime),"0s"),&objT->NewObjectFadeInTime,state);
        // Reborn: one enclosing Tracker owns the optional translation record and every source-counted upgrade-reference pair.
        Marshal(node.GetChildNode(nameof(Ep1ProductionUpdateModuleData.GiveUpgradeToProduced),null),&objT->GiveUpgradeToProduced,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.GiveNoXP),"false"),&objT->GiveNoXP,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.VeteranUnitsFromVeteranFactory),"false"),&objT->VeteranUnitsFromVeteranFactory,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.SecondaryQueue),"false"),&objT->SecondaryQueue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ProductionUpdateModuleData.IgnorePreReqs),"false"),&objT->IgnorePreReqs,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 upgrade translation rows have two Core-normalized strong upgrade import words. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UpgradeTranslation* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTranslation.RequiredUpgrade),null),&objT->RequiredUpgrade,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTranslation.ProvidedUpgrade),null),&objT->ProvidedUpgrade,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate the translation list from actual authored rows; schema validation owns its minimum occurrence requirement. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1GiveUpgradeToProduced* objT,Tracker state)
    {
        if(node is null)return;Marshal(node.GetChildNodes(nameof(Ep1GiveUpgradeToProduced.UpgradeTranslate)),&objT->UpgradeTranslate,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent GiveUpgradeToProduced stays null; present data has one independently owned eight-byte list header. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1GiveUpgradeToProduced** objT,Tracker state)
    {
        if(node is null)return;using Tracker.Context context=state.Push((void**)objT,(uint)sizeof(Ep1GiveUpgradeToProduced),1u);Marshal(node,*objT,state);
    }
}
