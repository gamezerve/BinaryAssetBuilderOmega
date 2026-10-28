using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official slave-watcher field with its schema default and optional status validation. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,SlaveWatcherBehaviorModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("GrantUpgrade",null),&value->GrantUpgrade,state);
        Marshal(node.GetAttributeValue("RemoveUpgrade",null),&value->RemoveUpgrade,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&value->ObjectStatusValidation,state);
        Marshal(node.GetAttributeValue("ShareUpgrades","false"),&value->ShareUpgrades,state);
        Marshal(node.GetAttributeValue("LetSlaveLive","false"),&value->LetSlaveLive,state);
        Marshal(node.GetAttributeValue("WatchingSpawnedSlave","false"),&value->WatchingSpawnedSlave,state);
        Marshal(node.GetAttributeValue("ForceSameTeam","false"),&value->ForceSameTeam,state);
        Marshal(node,(UpdateModuleData*)value,state);
    }
}
