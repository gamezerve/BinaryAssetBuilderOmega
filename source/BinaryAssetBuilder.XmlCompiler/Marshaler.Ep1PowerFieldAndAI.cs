using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the EP1 power mask and the sound/EVA references with their original storage. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1SpecialPowerUpdateModuleData* value,Tracker state)
    {
        Marshal(node,&value->Base,state);
        Marshal(node.GetAttributeValue("SpecialPowerTemplate",null),&value->SpecialPowerTemplate,state);
        Marshal(node.GetAttributeValue("DisabledTypesToIgnore","HELD"),&value->DisabledTypesToIgnore,state);
        Ep1AIBool(node.GetAttributeValue("StartsPaused","false"),&value->StartsPaused);
        var sound=node.GetChildNode("InitiateSound",null);
        if(sound is null)return;
        using var allocation=state.Push((void**)&value->InitiateSound,(uint)sizeof(Ep1SoundOrEvaEvent),1u);
        Marshal(sound.GetAttributeValue("Sound",null),&value->InitiateSound->Sound,state);
        Marshal(sound.GetAttributeValue("EvaEvent",null),&value->InitiateSound->EvaEvent,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the inherited weapon list and status data before the derived damage field. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1DamageFieldUpdateModuleData* value,Tracker state)
    {
        Marshal(node,&value->Base,state);
        Marshal(node.GetAttributeValue("Radius","0"),&value->Radius,state);
        Marshal(node.GetAttributeValue("RequiredUpgrade",null),&value->RequiredUpgrade,state);
        Marshal(node.GetChildNode("ObjectFilter",null),&value->ObjectFilter,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep the Horde discriminator after serializing the shared EP1 AI header. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1HordeAIUpdate(Node node,UpdateModuleData** destination,Tracker state)
    {
        using var allocation=state.Push((void**)destination,(uint)sizeof(Ep1HordeAIUpdateModuleData),1u);
        var value=(Ep1HordeAIUpdateModuleData*)*destination;
        Marshal(node,&value->Base,state);
        uint wire=Ep1HordeAIUpdateModuleData.Wire;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)value=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the truck's delays, distances and harvesting flags in the verified EP1 tail. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1SupplyTruckAIUpdate(Node node,UpdateModuleData** destination,Tracker state)
    {
        using var allocation=state.Push((void**)destination,(uint)sizeof(Ep1SupplyTruckAIUpdateModuleData),1u);
        var value=(Ep1SupplyTruckAIUpdateModuleData*)*destination;
        Marshal(node,&value->Base,state);
        Ep1AIUnsigned(node.GetAttributeValue("MaxBoxes","0"),&value->MaxBoxes,state);
        Marshal(node.GetAttributeValue("SupplyCenterActionDelay",null),&value->SupplyCenterActionDelay,state);
        Marshal(node.GetAttributeValue("SupplyWarehouseActionDelay",null),&value->SupplyWarehouseActionDelay,state);
        Marshal(node.GetAttributeValue("SupplyWarehouseScanDistance",null),&value->SupplyWarehouseScanDistance,state);
        Marshal(node.GetAttributeValue("DepletedAnnounceDistance","2000"),&value->DepletedAnnounceDistance,state);
        Marshal(node.GetAttributeValue("HarvestActivationRange",null),&value->HarvestActivationRange,state);
        Marshal(node.GetAttributeValue("HarvestPreparationTime",null),&value->HarvestPreparationTime,state);
        Marshal(node.GetAttributeValue("HarvestActionTime",null),&value->HarvestActionTime,state);
        Ep1AIBool(node.GetAttributeValue("HarvestTrees","false"),&value->HarvestTrees);
        Ep1AIBool(node.GetAttributeValue("HarvestTiberium","false"),&value->HarvestTiberium);
        Ep1AIBool(node.GetAttributeValue("HarvestsFromRearEnd","false"),&value->HarvestsFromRearEnd);
        Ep1AIBool(node.GetAttributeValue("CanPathThroughUnitsWhileDocking","true"),&value->CanPathThroughUnitsWhileDocking);
        Ep1AIBool(node.GetAttributeValue("CanPathThroughUnitsWhileHarvesting","true"),&value->CanPathThroughUnitsWhileHarvesting);
        uint wire=Ep1SupplyTruckAIUpdateModuleData.Wire;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)value=wire;
    }
}
