using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the complete Uprising prop draw while preserving its original concrete class ID. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1W3DPropDrawModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1W3DPropDrawModuleData.Model),null),&target->Model,state);
        Marshal(node,(DrawModuleData*)target,state);
        uint wire=0x148AA464u;state.InplaceEndianToPlatform(&wire);*(uint*)target=wire;
    }
}
