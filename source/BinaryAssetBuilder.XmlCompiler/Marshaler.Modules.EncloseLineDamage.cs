using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete original enclosure update and its inline protection filter. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,EncloseBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(EncloseBehaviorModuleData.EnclosureHitFX),null),&target->EnclosureHitFX,state);
        Marshal(node.GetAttributeValue(nameof(EncloseBehaviorModuleData.ScalableDrawModuleId),null),&target->ScalableDrawModuleId,state);
        Marshal(node.GetAttributeValue(nameof(EncloseBehaviorModuleData.ScaledBoneName),null),&target->ScaledBoneName,state);
        Marshal(node.GetAttributeValue(nameof(EncloseBehaviorModuleData.EnclosureRadius),"300"),&target->EnclosureRadius,state);
        Marshal(node.GetChildNode(nameof(EncloseBehaviorModuleData.Filter),null),&target->Filter,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the complete source-authored line damage timing, weapon, forbidden statuses and optional nested graphs. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,LineSegmentDamageUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.NodeLifetime),null),&target->NodeLifetime,state);
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.DamageInterval),null),&target->DamageInterval,state);
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.Distance),null),&target->Distance,state);
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.WeaponName),null),&target->WeaponName,state);
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.ObjectStatusForbiddenForDamage),null),&target->ObjectStatusForbiddenForDamage,state);
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.MinDistanceTolerance),"10.0"),&target->MinDistanceTolerance,state);
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.MaxValidHeightOffGround),"0.0"),&target->MaxValidHeightOffGround,state);
        Marshal(node.GetAttributeValue(nameof(LineSegmentDamageUpdateModuleData.NodeFXList),null),&target->NodeFXList,state);
        Marshal(node.GetChildNode(nameof(LineSegmentDamageUpdateModuleData.ObjectFilter),null),&target->ObjectFilter,state);
        Marshal(node.GetChildNode(nameof(LineSegmentDamageUpdateModuleData.ObjectStatusValidation),null),&target->ObjectStatusValidation,state);
        Marshal(node.GetChildNode(nameof(LineSegmentDamageUpdateModuleData.ParticleSystems),null),&target->ParticleSystems,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
}
