using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep protocol and upgrade imports as independent typed references in each binding. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerTechUpgradeBindingInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("TechDependency",null),&target->TechDependency,state);
        Marshal(node.GetAttributeValue("UpgradeToGrant",null),&target->UpgradeToGrant,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize any schema-valid number of protocol bindings using the standard relocatable list. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerTechUpgradeBinding* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes("Binding"),&target->Binding,state);
    }
}
