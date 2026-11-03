using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize a tech store row as its ordered button import array. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerTechStoreTemplateRow* objT,Tracker state)
    {
        if(node is null)return;Marshal(node.GetChildNodes("Button"),&objT->Button,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the EP1 faction word and five source-sized tech store rows. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerTechStoreTemplate* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Faction",null),&objT->Faction,state);Marshal(node.GetChildNodes("Row"),&objT->Row,state);Marshal(node,(BaseAssetType*)objT,state);
    }
}
