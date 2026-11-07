using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate the actual EP1 animal record and restore its derived identity after inherited AI marshalling. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1AnimalAIUpdate(Node node,UpdateModuleData** target,Tracker state)
    {
        using var context=state.Push((void**)target,(uint)sizeof(Ep1AnimalAIUpdateModuleData),1u);
        Marshal(node,(Ep1AnimalAIUpdateModuleData*)*target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original flee/wander scalar and the full independently qualified EP1 AI base. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AnimalAIUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("FleeRadius","20"),&target->FleeRadius,state);
        Marshal(node.GetAttributeValue("FleeDistance","100"),&target->FleeDistance,state);
        Marshal(node.GetAttributeValue("WanderPercentage","35"),&target->WanderPercentage,state);
        Marshal(node.GetAttributeValue("MinDistance","15"),&target->MinDistance,state);
        Marshal(node.GetAttributeValue("MaxDistance","50"),&target->MaxDistance,state);
        Marshal(node.GetAttributeValue("Radius","25"),&target->Radius,state);
        Marshal(node.GetAttributeValue("FleeUpdateTimer","0.8s"),&target->FleeUpdateTimer,state);
        Marshal(node.GetAttributeValue("WanderUpdateTimer","2.0s"),&target->WanderUpdateTimer,state);
        Marshal(node.GetAttributeValue("InitialFleeBlindlyRadius","50.0"),&target->InitialFleeBlindlyRadius,state);
        Marshal(node.GetAttributeValue("AfraidOfCastles","true"),&target->AfraidOfCastles,state);
        Marshal(node,(Ep1AIUpdateModuleData*)target,state);
        uint wire=0xCD56659Bu;state.InplaceEndianToPlatform(&wire);*(uint*)target=wire;
    }
}
