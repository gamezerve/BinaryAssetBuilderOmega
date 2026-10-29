using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: hierarchical turret slots retain the complete inherited turret and the authored parent link. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1WeaponSlotHierarchicalTurret* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("ParentID","0"),&objT->ParentID,state);Marshal(node,(Ep1WeaponSlotTurret*)objT,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 turret slot through its real inline hardpoint and 100-byte AI record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1WeaponSlotTurret* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1WeaponSlotHardpoint*)objT,state);
        Marshal(node.GetChildNode("TurretSettings",null),&objT->TurretSettings,state);
        Marshal(node.GetAttributeValue("ForceRotateObjectToMaintainFiringArc","true"),&objT->ForceRotateObjectToMaintainFiringArc,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every EP1 turret AI scalar/default, status mask and optional chooser pointer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TurretAIData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("TurretTurnRate","0"),&objT->TurretTurnRate,state);
        Marshal(node.GetAttributeValue("TurretPitchRate","0"),&objT->TurretPitchRate,state);
        Marshal(node.GetAttributeValue("MinimumPitch","0d"),&objT->MinimumPitch,state);
        Marshal(node.GetAttributeValue("PitchHeight","50%"),&objT->PitchHeight,state);
        Marshal(node.GetAttributeValue("ControlledWeaponSlots",""),&objT->ControlledWeaponSlots,state);
        Marshal(node.GetAttributeValue("InvalidIdleScanObjectStatus",""),&objT->InvalidIdleScanObjectStatus,state);
        Marshal(node.GetAttributeValue("IdleTurnRateModifier","50%"),&objT->IdleTurnRateModifier,state);
        Marshal(node.GetAttributeValue("MinIdleScanTime","0s"),&objT->MinIdleScanTime,state);
        Marshal(node.GetAttributeValue("MaxIdleScanTime","0s"),&objT->MaxIdleScanTime,state);
        Marshal(node.GetAttributeValue("MinIdleScanAngle","0"),&objT->MinIdleScanAngle,state);
        Marshal(node.GetAttributeValue("MaxIdleScanAngle","0"),&objT->MaxIdleScanAngle,state);
        Marshal(node.GetAttributeValue("MaxDeflectionClockwise","180d"),&objT->MaxDeflectionClockwise,state);
        Marshal(node.GetAttributeValue("MaxDeflectionAntiClockwise","180d"),&objT->MaxDeflectionAntiClockwise,state);
        Marshal(node.GetAttributeValue("NaturalTurretAngle","0d"),&objT->NaturalTurretAngle,state);
        Marshal(node.GetAttributeValue("BlindFireDistance","0"),&objT->BlindFireDistance,state);
        Marshal(node.GetAttributeValue("ComeToHaltJiggle","0d"),&objT->ComeToHaltJiggle,state);
        Marshal(node.GetAttributeValue("AllowsPitch","false"),&objT->AllowsPitch,state);
        Marshal(node.GetAttributeValue("RecenterWhenOutOfTurnRange","true"),&objT->RecenterWhenOutOfTurnRange,state);
        Marshal(node.GetAttributeValue("SetAngleModelStates","false"),&objT->SetAngleModelStates,state);
        var chooser=node.GetChildNode("TurretAITargetChooserData",null);
        if(chooser!=null){using var context=state.Push((void**)&objT->TurretAITargetChooserData,(uint)sizeof(Ep1TurretAITargetChooserData),1u);Marshal(chooser,objT->TurretAITargetChooserData,state);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the retail turret chooser defaults and real comparison import, refusing unimplemented vision payloads explicitly. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TurretAITargetChooserData* objT,Tracker state)
    {
        if(node is null)return;
        // Reborn: source-sized turret vision rules own their complete inline filters and optional filter children.
        Marshal(node.GetChildNodes("ObjectVisionRange"),&objT->Base.ObjectVisionRange,state);
        Marshal(node.GetAttributeValue("IdleScanDelay","0.8s"),&objT->Base.IdleScanDelay,state);
        Marshal(node.GetAttributeValue("ReacquireDelay","60s"),&objT->Base.ReacquireDelay,state);
        Marshal(node.GetAttributeValue("StartupDelay","0s"),&objT->Base.StartupDelay,state);
        Marshal(node.GetAttributeValue("SympathyRange","25"),&objT->Base.SympathyRange,state);
        Marshal(node.GetAttributeValue("CheckVisionRange","false"),&objT->Base.CheckVisionRange,state);
        Marshal(node.GetAttributeValue("CanPickDynamicTargets","true"),&objT->Base.CanPickDynamicTargets,state);
        Marshal(node.GetAttributeValue("RotateToTargetWhenAiming","false"),&objT->Base.RotateToTargetWhenAiming,state);
        Marshal(node.GetAttributeValue("CanAutoAcquireNonAutoAcquirable","false"),&objT->Base.CanAutoAcquireNonAutoAcquirable,state);
        Marshal(node.GetAttributeValue("CanChaseAttack","true"),&objT->Base.CanChaseAttack,state);
        Marshal(node.GetAttributeValue("CanOnlyAttackTargetsInFront","false"),&objT->Base.CanOnlyAttackTargetsInFront,state);
        Ep1ToggleEnum(node.GetAttributeValue("CanPickDynamicTargetType","ALWAYS"),(uint*)&objT->Base.CanPickDynamicTargetType,state,typeof(Ep1DynamicTargetFlag),true);
        Marshal(node.GetAttributeValue("TargetingCompareList","DefaultTurretTargetingCompareList"),&objT->TargetingCompareList,state);
        Marshal(node.GetAttributeValue("ActiveWhenPerformingSpecialAbility","false"),&objT->ActiveWhenPerformingSpecialAbility,state);
        Marshal(node.GetAttributeValue("CanAcquireDynamicIfAssignedOutOfRange","false"),&objT->CanAcquireDynamicIfAssignedOutOfRange,state);
        Marshal(node.GetAttributeValue("CanPickTargetsOutOfTurretAngle","false"),&objT->CanPickTargetsOutOfTurretAngle,state);
    }
}
