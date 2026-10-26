using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete Uprising nugget list and inherited upgrade conditions without using KW root-level visibility fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SubObjectsUpgradeModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes("Nugget"),&value->Nugget,state);
        Marshal(node,(UpgradeModuleData*)value,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve every nugget's visibility strings, timing, weak upgrade, texture imports and optional animation. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SubObjectsUpgradeNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("ShowSubObjects",null),&value->ShowSubObjects,state);
        Marshal(node.GetAttributeValue("HideSubObjects",null),&value->HideSubObjects,state);
        Marshal(node.GetAttributeValue("FadeTime","0.5s"),&value->FadeTime,state);
        Marshal(node.GetAttributeValue("WaitBeforeFadeTime","0.0s"),&value->WaitBeforeFadeTime,state);
        Marshal(node.GetAttributeValue("Upgrade",null),&value->Upgrade,state);
        Marshal(node.GetAttributeValue("SkipFadeOnCreate","false"),&value->SkipFadeOnCreate,state);
        Marshal(node.GetAttributeValue("HideSubObjectsOnRemove","false"),&value->HideSubObjectsOnRemove,state);
        Marshal(node.GetAttributeValue("UnHideSubObjectsOnRemove","false"),&value->UnHideSubObjectsOnRemove,state);
        Marshal(node.GetChildNodes("UpgradeTexture"),&value->UpgradeTexture,state);
        Marshal(node.GetChildNode("CustomAnimAndDuration",null),&value->CustomAnimAndDuration,state);
    }
}
