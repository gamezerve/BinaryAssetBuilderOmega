using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the stock EP1 recall limit and voice flag while retaining the existing SpecialPower reference processor. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1RecallUnitsSpecialPowerModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1RecallUnitsSpecialPowerModuleData.MaxAircraftToRecall),"4"),&objT->MaxAircraftToRecall,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RecallUnitsSpecialPowerModuleData.PlayReturnVoiceFromRecalledUnit),"true"),&objT->PlayReturnVoiceFromRecalledUnit,state);
        Marshal(node,(SpecialPowerModuleData*)objT,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve all Uprising parking attributes and optional filter/vector records in their independently observed EP1 order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1ParkingPlaceBehaviorModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.NumRows),null),&objT->NumRows,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.NumCols),null),&objT->NumCols,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.ApproachHeight),null),&objT->ApproachHeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.ParkInHangars),null),&objT->ParkInHangars,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.DoorOpenTime),"0s"),&objT->DoorOpenTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.DoorCloseTime),"0s"),&objT->DoorCloseTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.LowersOntoParkingPlaceOnProduction),"false"),&objT->LowersOntoParkingPlaceOnProduction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.CanSetRallyPoint),"true"),&objT->CanSetRallyPoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.DoorsUnpackOpen),"true"),&objT->DoorsUnpackOpen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.QueueDistance),"100"),&objT->QueueDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.LimitedProductionAircraftSpots),"0"),&objT->LimitedProductionAircraftSpots,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.HelipadBoneName),"HeliPark01"),&objT->HelipadBoneName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParkingPlaceBehaviorModuleData.ProducedAboveHeight),"500.0"),&objT->ProducedAboveHeight,state);
        Marshal(node.GetChildNode(nameof(Ep1ParkingPlaceBehaviorModuleData.CanParkHereFilter),null),&objT->CanParkHereFilter,state);
        Marshal(node.GetChildNode(nameof(Ep1ParkingPlaceBehaviorModuleData.DefaultRallyPointOffset),null),&objT->DefaultRallyPointOffset,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
