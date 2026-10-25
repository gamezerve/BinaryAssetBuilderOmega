using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile standalone EP1 button-state data through the existing image/localization-string writer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ButtonSingleStateData* target,Tracker state)
    {if(node==null)return;Marshal(node.GetChildNode("State",null),&target->State,state);}
}
