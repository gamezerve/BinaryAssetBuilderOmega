using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the Core selectors for the required technology and button-state references. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PurchasePlayerTechButtonTemplate* target,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("PlayerTech",null),&target->PlayerTech,state);Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
}
