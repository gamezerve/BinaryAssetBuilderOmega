using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 passenger-ejection record with original schema defaults and inherited SpecialAbility fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,EjectPassengersSpecialAbilityUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.TimePerPassenger),"1s"),&objT->TimePerPassenger,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.Speed),"10.0"),&objT->Speed,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.Radius),"20.0"),&objT->Radius,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.Height),"100.0"),&objT->Height,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.ApexHeight),"0.0"),&objT->ApexHeight,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.EjectionFX),null),&objT->EjectionFX,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.EjectionBoneName),null),&objT->EjectionBoneName,state);
        Marshal(node.GetChildNode(nameof(EjectPassengersSpecialAbilityUpdateModuleData.EjectionBoneOffset),null),&objT->EjectionBoneOffset,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.RunOffMapWhenDone),"false"),&objT->RunOffMapWhenDone,state);
        Marshal(node.GetAttributeValue(nameof(EjectPassengersSpecialAbilityUpdateModuleData.IgnoreTargetLocation),"false"),&objT->IgnoreTargetLocation,state);
        Marshal(node,(SpecialAbilityUpdateModuleData*)objT,state);
    }
}
