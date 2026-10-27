using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write each EP1 firing entry with its real weapon import, initial delay and optional three-dimensional offset. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FireWeaponUpdateModuleDataFireWeaponNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("WeaponName",null),&value->WeaponName,state);
        Marshal(node.GetAttributeValue("FireDelay",null),&value->FireDelay,state);
        Marshal(node.GetChildNode("Offset",null),&value->Offset,state);
        Marshal(node.GetAttributeValue("OneShot","false"),&value->OneShot,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the shared EP1 firing interval, source-sized nugget list, status masks and official AliveOnly default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FireWeaponUpdateModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("FireInterval",null),&value->FireInterval,state);
        Marshal(node.GetAttributeValue("ActiveWhenDisabled",null),&value->ActiveWhenDisabled,state);
        Marshal(node.GetChildNodes("FireWeaponNugget"),&value->FireWeaponNugget,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&value->ObjectStatusValidation,state);
        Marshal(node.GetAttributeValue("AliveOnly","false"),&value->AliveOnly,state);
        Marshal(node,(UpdateModuleData*)value,state);
    }
}
