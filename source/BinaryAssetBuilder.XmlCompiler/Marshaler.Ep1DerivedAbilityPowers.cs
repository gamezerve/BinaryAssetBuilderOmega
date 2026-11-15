using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 OCL power and its optional FX vector. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1CreateAndEnterObjectSpecialPowerModuleData* result,Tracker state)
    {
        Marshal(node,&result->Base,state);
        Marshal(node.GetChildNode("FXOffset",null),&result->FXOffset,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep the two authored ObjectStatus masks independent over the EP1 ability header. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1ObjectStatusSpecialAbilityUpdateModuleData* result,Tracker state)
    {
        Marshal(node,&result->Base,state);
        Marshal(node.GetAttributeValue("Status1",null),&result->Status1,state);
        Marshal(node.GetAttributeValue("Status2",null),&result->Status2,state);
    }
}
