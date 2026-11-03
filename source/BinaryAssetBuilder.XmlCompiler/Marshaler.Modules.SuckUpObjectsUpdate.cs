using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the satellite lift settings and target filter with EP1 schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, SuckUpObjectsUpdateModuleData* root, Tracker state)
    {
        Marshal(node.GetAttributeValue("InitialDelay", "0.5s"), &root->InitialDelay, state);
        Marshal(node.GetAttributeValue("LiftVelocity", "1.0"), &root->LiftVelocity, state);
        Marshal(node.GetAttributeValue("DeathHeight", "420"), &root->DeathHeight, state);
        Marshal(node.GetAttributeValue("Radius", "10.0"), &root->Radius, state);
        Marshal(node.GetAttributeValue("Duration", "10.0s"), &root->Duration, state);
        Marshal(node.GetChildNode("ObjectFilter", null), &root->ObjectFilter, state);
        Marshal(node, (UpdateModuleData*)root, state);
    }
}
