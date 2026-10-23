using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read the actual EP1 schema fields, including ExitDelaySeconds, and preserve both required source vectors and packed booleans. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1QueueProductionExitUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1QueueProductionExitUpdateModuleData.PlacementViewAngle),null),&objT->PlacementViewAngle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1QueueProductionExitUpdateModuleData.ExitDelaySeconds),"0s"),&objT->ExitDelaySeconds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1QueueProductionExitUpdateModuleData.InitialBurst),"0"),&objT->InitialBurst,state);
        Marshal(node.GetAttributeValue(nameof(Ep1QueueProductionExitUpdateModuleData.AllowAirborneCreation),"false"),&objT->AllowAirborneCreation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1QueueProductionExitUpdateModuleData.NoExitPath),"false"),&objT->NoExitPath,state);
        Marshal(node.GetAttributeValue(nameof(Ep1QueueProductionExitUpdateModuleData.CanRallyToSlaughter),"false"),&objT->CanRallyToSlaughter,state);
        Marshal(node.GetAttributeValue(nameof(Ep1QueueProductionExitUpdateModuleData.ClearAlliesFromDestination),"true"),&objT->ClearAlliesFromDestination,state);
        Marshal(node.GetChildNode(nameof(Ep1QueueProductionExitUpdateModuleData.UnitCreatePoint),null),&objT->UnitCreatePoint,state);
        Marshal(node.GetChildNode(nameof(Ep1QueueProductionExitUpdateModuleData.NaturalRallyPoint),null),&objT->NaturalRallyPoint,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
