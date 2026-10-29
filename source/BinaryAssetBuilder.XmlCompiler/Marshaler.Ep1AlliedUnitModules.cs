using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ActivateModuleSpecialPower using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateModuleSpecialPower* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetChildNodes("Modules"),&objT->Modules,state);
        Marshal(node,(SpecialPowerModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ActivateModuleInfo using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateModuleInfo* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Deactivate","false"),&objT->Deactivate,state);
        Marshal(node,(SpecialPowerModuleInfo*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BeamSpecialAbilityUpdateModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1BeamSpecialAbilityUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("TargetAttributeModifierAdd",null),&objT->TargetAttributeModifierAdd,state);
        Marshal(node.GetAttributeValue("TargetObjectStatusAdd",null),&objT->TargetObjectStatusAdd,state);
        Marshal(node.GetAttributeValue("SweepFX",null),&objT->SweepFX,state);
        Marshal(node.GetAttributeValue("ReflectorExtendDistance","0"),&objT->ReflectorExtendDistance,state);
        Marshal(node.GetAttributeValue("PreferredTargetBone",null),&objT->PreferredTargetBone,state);
        Marshal(node.GetAttributeValue("WeaponTemplate",null),&objT->WeaponTemplate,state);
        Marshal(node.GetAttributeValue("BeamOptions",""),&objT->BeamOptions,state);
        Marshal(node.GetAttributeValue("BeamDuration","0s"),&objT->BeamDuration,state);
        Marshal(node.GetAttributeValue("SweepDistance","0.0"),&objT->SweepDistance,state);
        Marshal(node.GetAttributeValue("DamageInterval","0s"),&objT->DamageInterval,state);
        Marshal(node.GetAttributeValue("JoinWithOtherBeams","false"),&objT->JoinWithOtherBeams,state);
        Marshal(node.GetAttributeValue("DissapateWhenTargetDead","true"),&objT->DissapateWhenTargetDead,state);
        Marshal(node.GetAttributeValue("LineType","false"),&objT->LineType,state);
        Marshal(node.GetAttributeValue("TargetSamePlayerOnly","false"),&objT->TargetSamePlayerOnly,state);
        Marshal(node,(SpecialAbilityUpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DominateEnemySpecialPowerModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DominateEnemySpecialPowerModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("DominateRadius",null),&objT->DominateRadius,state);
        Marshal(node.GetAttributeValue("TriggerFX",null),&objT->TriggerFX,state);
        Marshal(node.GetAttributeValue("DominatedFX",null),&objT->DominatedFX,state);
        Marshal(node.GetChildNode("AttributeModifierAffects",null),&objT->AttributeModifierAffects,state);
        Marshal(node.GetAttributeValue("PermanentlyConvert","false"),&objT->PermanentlyConvert,state);
        Marshal(node,(SpecialAbilityUpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal InvisibilityBroadcastUpdateModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1InvisibilityBroadcastUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("InvisibilityTemplate",null),&objT->InvisibilityTemplate,state);
        Marshal(node.GetAttributeValue("UpdatePeriod","1s"),&objT->UpdatePeriod,state);
        Marshal(node.GetAttributeValue("BroadcastRange","0"),&objT->BroadcastRange,state);
        Marshal(node.GetAttributeValue("ScalableDrawModuleId",null),&objT->ScalableDrawModuleId,state);
        Marshal(node.GetAttributeValue("ScaledBoneName",null),&objT->ScaledBoneName,state);
        Marshal(node.GetChildNode("BroadcastObjectFilter",null),&objT->BroadcastObjectFilter,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node.GetAttributeValue("AffectsSelf","true"),&objT->AffectsSelf,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal LinearDamageUpdateModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1LinearDamageUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("StartDistanceOffset","0"),&objT->StartDistanceOffset,state);
        Marshal(node.GetAttributeValue("Radius","10.0"),&objT->Radius,state);
        Marshal(node.GetAttributeValue("Length",null),&objT->Length,state);
        Marshal(node.GetAttributeValue("SweepFXList",null),&objT->SweepFXList,state);
        Marshal(node.GetAttributeValue("SweepWeapon",null),&objT->SweepWeapon,state);
        Marshal(node.GetAttributeValue("Speed","1.0"),&objT->Speed,state);
        Marshal(node.GetAttributeValue("Locomotor",null),&objT->Locomotor,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node.GetChildNode("BlockingObjectFilter",null),&objT->BlockingObjectFilter,state);
        Marshal(node.GetAttributeValue("FollowTerrain","true"),&objT->FollowTerrain,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal MagneticEffectUpdateModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MagneticEffectUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("ScalableDrawModuleId",null),&objT->ScalableDrawModuleId,state);
        Marshal(node.GetAttributeValue("ScaledBoneName",null),&objT->ScaledBoneName,state);
        Marshal(node.GetAttributeValue("Radius","250.0"),&objT->Radius,state);
        Marshal(node.GetAttributeValue("MaximumPullVelocity","20.0"),&objT->MaximumPullVelocity,state);
        Marshal(node.GetAttributeValue("MinimumPullVelocity","4.0"),&objT->MinimumPullVelocity,state);
        Marshal(node.GetAttributeValue("VictimStatusToSet","MAGNETIZED"),&objT->VictimStatusToSet,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node.GetChildNode("ObjectFilter",null),&objT->ObjectFilter,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ReconstituteStateSpecialAbilityModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ReconstituteStateSpecialAbilityModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("UpdateModuleId",null),&objT->UpdateModuleId,state);
        Marshal(node.GetAttributeValue("TimeStepsBack","0s"),&objT->TimeStepsBack,state);
        Marshal(node.GetAttributeValue("LeavePositionFX",null),&objT->LeavePositionFX,state);
        Marshal(node.GetAttributeValue("ReturnPositionFX",null),&objT->ReturnPositionFX,state);
        Marshal(node.GetAttributeValue("PostReconstituteAttributeModifier",null),&objT->PostReconstituteAttributeModifier,state);
        Marshal(node,(SpecialAbilityUpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal SpecialDisguiseTerrainData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpecialDisguiseTerrainData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("TerrainClass",null),&objT->TerrainClass,state);
        Marshal(node.GetAttributeValue("ObjectTemplate",null),&objT->ObjectTemplate,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal SpecialDisguiseUpdateModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpecialDisguiseUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("OpacityTarget","0"),&objT->OpacityTarget,state);
        Marshal(node.GetAttributeValue("DisguiseFX",null),&objT->DisguiseFX,state);
        Marshal(node.GetAttributeValue("ScanRadiusForPropDisguise","0.0"),&objT->ScanRadiusForPropDisguise,state);
        Marshal(node.GetAttributeValue("DisguiseIdleDelay","1s"),&objT->DisguiseIdleDelay,state);
        Marshal(node.GetChildNodes("TerrainData"),&objT->TerrainData,state);
        Marshal(node.GetAttributeValue("TriggerInstantlyOnCreate","false"),&objT->TriggerInstantlyOnCreate,state);
        Marshal(node,(SpecialAbilityUpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal StoreCurrentStateUpdateModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StoreCurrentStateUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("SleepTime","0s"),&objT->SleepTime,state);
        Marshal(node.GetAttributeValue("MaxTime","5s"),&objT->MaxTime,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ModelConditionEvaEventSelectorOverrideBlock using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ModelConditionEvaEventSelectorOverrideBlock* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("RequiredFlags",""),&objT->RequiredFlags,state);
        Marshal(node.GetAttributeValue("ExcludedFlags",""),&objT->ExcludedFlags,state);
        Marshal(node.GetChildNode("EvaEventArray",null),&objT->EvaEventArray,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ModelConditionEvaEventSelectorClientBehaviorModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ModelConditionEvaEventSelectorClientBehaviorModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetChildNodes("Override"),&objT->Override,state);
        Marshal(node,(ClientBehaviorModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UpgradeSoundSelectorOverrideBlock using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UpgradeSoundSelectorOverrideBlock* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("RequiredModelConditions",""),&objT->RequiredModelConditions,state);
        Marshal(node.GetAttributeValue("ConflictingModelConditions",""),&objT->ConflictingModelConditions,state);
        Marshal(node.GetAttributeValue("VoicePriorityOverride",null),&objT->VoicePriorityOverride,state);
        Marshal(node.GetChildNode("AudioArrayVoice",null),&objT->AudioArrayVoice,state);
        Marshal(node.GetChildNode("AudioArraySound",null),&objT->AudioArraySound,state);
        Marshal(node.GetChildNodes("RequiredUpgrade"),&objT->RequiredUpgrade,state);
        Marshal(node.GetChildNodes("ConflictingUpgrade"),&objT->ConflictingUpgrade,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UpgradeSoundSelectorClientBehaviorModuleData using EP1 native storage and the official field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UpgradeSoundSelectorClientBehaviorModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetChildNodes("Override"),&objT->Override,state);
        Marshal(node,(ClientBehaviorModuleData*)objT,state);
    }
    // Reborn: use the original five named beam flags; the historical compiler has no equivalent EP1 mask.
    public static unsafe void Marshal(Value value,Ep1BeamOptionsBitFlags* objT,Tracker state) => Ep1ToggleEnum(value,(uint*)objT,state,typeof(Ep1BeamOptionsType),true);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent EVA arrays remain null; present arrays allocate their own complete source-sized record. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1EvaEventArray** objT,Tracker state)
    {if(node==null)return;using var context=state.Push((void**)objT,(uint)sizeof(Ep1EvaEventArray),1u);Marshal(node,*objT,state);}

}
