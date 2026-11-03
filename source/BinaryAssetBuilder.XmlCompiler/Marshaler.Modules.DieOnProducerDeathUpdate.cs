using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize both official producer-death fields with their schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,DieOnProducerDeathUpdateModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("SleepInterval","0.5s"),&value->SleepInterval,state);
        Marshal(node.GetAttributeValue("DieOnProducerDisabled","false"),&value->DieOnProducerDisabled,state);
        Marshal(node,(UpdateModuleData*)value,state);
    }
}
