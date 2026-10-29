using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile ChargeWeaponUpdateModuleData with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ChargeWeaponUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("ChargeTime","1.0s"),&objT->ChargeTime,state);
        Marshal(node.GetAttributeValue("OverChargeTime","0.0s"),&objT->OverChargeTime,state);
        Marshal(node.GetAttributeValue("OverChargePercent","0%"),&objT->OverChargePercent,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile PrematureDischargeSpecialPowerModuleData with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PrematureDischargeSpecialPowerModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node,(SpecialPowerModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile SpiderHoleUpdateModuleData with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpiderHoleUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Radius","0"),&objT->Radius,state);
        Marshal(node.GetAttributeValue("ScanFrequency","1s"),&objT->ScanFrequency,state);
        Marshal(node.GetChildNode("ObjectFilter",null),&objT->ObjectFilter,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile ActivateSpecialPowerOnCreate with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateSpecialPowerOnCreate* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetChildNodes("SpecialPowerTemplate"),&objT->SpecialPowerTemplate,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile MovementSpecialPowerModuleData with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MovementSpecialPowerModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("MovementType","NONE"),&objT->MovementType,state);
        Marshal(node.GetAttributeValue("Duration","0s"),&objT->Duration,state);
        Marshal(node.GetAttributeValue("ModelCondition",null),&objT->ModelCondition,state);
        Marshal(node.GetAttributeValue("ObjectStatus",null),&objT->ObjectStatus,state);
        Marshal(node.GetAttributeValue("LocomotorSet","INVALID"),&objT->LocomotorSet,state);
        Marshal(node.GetAttributeValue("BusyDurationOnExit","0s"),&objT->BusyDurationOnExit,state);
        Marshal(node.GetAttributeValue("CollisionBounceHeight","20.0"),&objT->CollisionBounceHeight,state);
        Marshal(node.GetAttributeValue("CollisionBounceDistance","50.0"),&objT->CollisionBounceDistance,state);
        Marshal(node.GetAttributeValue("CollisionBounceSpeedScalar","0.5"),&objT->CollisionBounceSpeedScalar,state);
        Marshal(node,(SpecialPowerModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile TimedOpacityUpdateModuleData with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TimedOpacityUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Opacity","0.5"),&objT->Opacity,state);
        Marshal(node.GetAttributeValue("Duration","5s"),&objT->Duration,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile MonitorConditionUpdateModuleData with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MonitorConditionUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("ModelConditionFlags",null),&objT->ModelConditionFlags,state);
        Marshal(node.GetAttributeValue("OCL",null),&objT->OCL,state);
        Marshal(node.GetAttributeValue("PreDelay",null),&objT->PreDelay,state);
        Marshal(node.GetAttributeValue("PostDelay",null),&objT->PostDelay,state);
        Marshal(node.GetAttributeValue("ActionsOnTriggered",null),&objT->ActionsOnTriggered,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile LifetimeUpdateModuleData with original EP1 field storage and official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1LifetimeUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("MinLifetime","0s"),&objT->MinLifetime,state);
        Marshal(node.GetAttributeValue("MaxLifetime","0s"),&objT->MaxLifetime,state);
        Marshal(node.GetAttributeValue("DeathType",null),&objT->DeathType,state);
        Marshal(node.GetAttributeValue("CountdownSound",null),&objT->CountdownSound,state);
        Marshal(node.GetAttributeValue("CountdownCompleteSound",null),&objT->CountdownCompleteSound,state);
        Marshal(node.GetAttributeValue("TimeBeforeDeathThatCountdownCompletes","0s"),&objT->TimeBeforeDeathThatCountdownCompletes,state);
        Marshal(node.GetAttributeValue("TimeBetweenCountdownSounds","1s"),&objT->TimeBetweenCountdownSounds,state);
        Marshal(node.GetAttributeValue("NumberOfCountdownSoundPlays",null),&objT->NumberOfCountdownSoundPlays,state);
        Marshal(node.GetAttributeValue("WaitForWakeUp","false"),&objT->WaitForWakeUp,state);
        Marshal(node.GetAttributeValue("ScoreKill","false"),&objT->ScoreKill,state);
        Marshal(node.GetAttributeValue("IgnoreProjectileState",null),&objT->IgnoreProjectileState,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    // Reborn: select the actual EP1 movement enum and monitor mask without accepting numeric fallback ordinals.
    public static unsafe void Marshal(Value value,SPMovementType* objT,Tracker state)=>Ep1ToggleEnum(value,(uint*)objT,state,typeof(SPMovementType),false);
    public static unsafe void Marshal(Value value,MonitorConditionActionBitFlags* objT,Tracker state)=>Ep1ToggleEnum(value,(uint*)objT,state,typeof(MonitorConditionActionType),true);
}
