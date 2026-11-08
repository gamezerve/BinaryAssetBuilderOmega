using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal official seconds-based bombard delay bounds. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CoverageTimeRange* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Low",null),&target->Low,state);
        Marshal(node.GetAttributeValue("High",null),&target->High,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every bombardment field including optional status validation and strong weapon storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,BombardAreaUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("IncomingAngle","0d"),&target->IncomingAngle,state);
        Marshal(node.GetAttributeValue("WeaponName",null),&target->WeaponName,state);
        Marshal(node.GetAttributeValue("MaxShots","1"),&target->MaxShots,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&target->ObjectStatusValidation,state);
        Marshal(node.GetChildNode("TimeDelayRange",null),&target->TimeDelayRange,state);
        Marshal(node.GetChildNode("RadiusSpreadRange",null),&target->RadiusSpreadRange,state);
        Marshal(node.GetAttributeValue("UsePlayerStore","false"),&target->UsePlayerStore,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the official alliance sharing enum in source declaration order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,UnifiedControlType* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(UnifiedControlType),false);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain complete capture inheritance and alliance mode. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,UnifiedControlBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("UnificationType","SAME_PLAYER"),&target->UnificationType,state);
        Marshal(node,(CaptureBehaviorModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal a source ownership threshold and its concrete upgrade import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,UpgradeMap* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("OwnershipAmount",null),&target->OwnershipAmount,state);
        Marshal(node.GetAttributeValue("ProvidedUpgrade",null),&target->ProvidedUpgrade,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal all upgrade thresholds and the full unified capture base. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,CaptureAndGiveUpgradeBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes("UpgradeMap"),&target->UpgradeMap,state);
        Marshal(node,(UnifiedControlBehaviorModuleData*)target,state);
    }
}
