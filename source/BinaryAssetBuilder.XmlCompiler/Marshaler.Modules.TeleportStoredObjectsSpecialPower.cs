using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete teleport power using official defaults and the normal Core OCL reference index. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, TeleportStoredObjectsSpecialPowerModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsSpecialPowerModuleData.StoreObjectsLinkID),"0"),&objT->StoreObjectsLinkID,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsSpecialPowerModuleData.OCL),null),&objT->OCL,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsSpecialPowerModuleData.GetObjectsCurrentlyAtStorePosition),"false"),&objT->GetObjectsCurrentlyAtStorePosition,state);
        Marshal(node,(SpecialPowerModuleData*)objT,state);
    }
}
