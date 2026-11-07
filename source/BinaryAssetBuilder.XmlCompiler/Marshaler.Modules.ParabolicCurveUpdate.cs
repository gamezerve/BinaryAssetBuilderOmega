using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the complete inherited update header for the original EP1 parabolic-flight marker. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,ParabolicCurveUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UpdateModuleData*)target,state);
    }
}
