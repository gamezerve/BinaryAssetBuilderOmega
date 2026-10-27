using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete Uprising collision weapon record without changing the legacy KW model. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FireWeaponCollideModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("CollideWeapon",null),&value->CollideWeapon,state);
        Ep1ToggleEnum(node.GetAttributeValue("Flags","NONE"),(uint*)&value->Flags,state,typeof(Ep1FireWeaponFlagsType),true);
        Marshal(node.GetAttributeValue("RequiredStatus",null),&value->RequiredStatus,state);
        Marshal(node.GetAttributeValue("ForbiddenStatus",null),&value->ForbiddenStatus,state);
        Marshal(node.GetAttributeValue("WhereToFire","TARGET_OBJECT"),&value->WhereToFire,state);
        Marshal(node.GetAttributeValue("DisabledTypesToProcess","DEFAULT"),&value->DisabledTypesToProcess,state);
        Marshal(node.GetAttributeValue("DisableType","DEFAULT"),&value->DisableType,state);
        Marshal(node.GetAttributeValue("DisableDuration","0s"),&value->DisableDuration,state);
        Marshal(node.GetAttributeValue("FiringInterval","0s"),&value->FiringInterval,state);
        // Reborn: an omitted stance has a null pointer, whereas GUARD is an allocated ordinal zero.
        var stance=node.GetAttributeValue("ForbiddenStance",null);
        if(stance!=null){using(var context=state.Push((void**)&value->ForbiddenStance,4u,1u))Marshal(stance,value->ForbiddenStance,state);}
        Marshal(node.GetChildNode("TargetObjectFilter",null),&value->TargetObjectFilter,state);
        Marshal(node.GetAttributeValue("FireOnce","false"),&value->FireOnce,state);
        Marshal(node,(CollideModuleData*)value,state);
    }
}
