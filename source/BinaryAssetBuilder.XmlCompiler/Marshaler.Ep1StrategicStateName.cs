using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: link the strategic state through Core's occurrence table and retain the authored display name. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StrategicStateName* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StrategicStateName.State),null),&target->State,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StrategicStateName.Name),null),&target->Name,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
