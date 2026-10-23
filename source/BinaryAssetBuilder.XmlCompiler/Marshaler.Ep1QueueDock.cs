using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal schema-default empty bone names and the optional docking condition without changing other create behaviors. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,QueueDockModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(QueueDockModuleData.DockStartBoneName),""),&objT->DockStartBoneName,state);
        Marshal(node.GetAttributeValue(nameof(QueueDockModuleData.DockActionBoneName),""),&objT->DockActionBoneName,state);
        Marshal(node.GetAttributeValue(nameof(QueueDockModuleData.DockEndBoneName),""),&objT->DockEndBoneName,state);
        Marshal(node.GetAttributeValue(nameof(QueueDockModuleData.QueueBonePrefix),""),&objT->QueueBonePrefix,state);
        Marshal(node.GetAttributeValue(nameof(QueueDockModuleData.TurnTableBoneName),""),&objT->TurnTableBoneName,state);
        Marshal(node.GetAttributeValue(nameof(QueueDockModuleData.DockingAnimation),null),&objT->DockingAnimation,state);
        Marshal(node,(CreateModuleData*)objT,state);
    }
}
