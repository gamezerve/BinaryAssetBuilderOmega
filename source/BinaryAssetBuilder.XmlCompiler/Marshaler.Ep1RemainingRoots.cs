using Relo;
using SageBinaryData;
using System.IO;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile base-selection masks using actual enum values, preserving the official PRIMARY_TYPE_MAX alias. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,AIBaseSelectionCriteria* objT,Tracker state)
    {
        if(node==null)return;
        foreach(string name in new[]{"RequiredBaseTypes","ExcludedBaseTypes","PreferredBaseTypes","PreferredNotBaseTypes"}){
            uint bits=0;foreach(string token in node.GetAttributeValue(name,"").GetText().Split(WhiteSpaces,System.StringSplitOptions.RemoveEmptyEntries)){
                if(!System.Enum.TryParse(token,false,out AIBaseType value)||!System.Enum.IsDefined(typeof(AIBaseType),value))throw new InvalidDataException("Unknown AI base flag: "+token);
                bits|=1u<<(int)value;
            }
            state.InplaceEndianToPlatform(&bits);*((uint*)objT+System.Array.IndexOf(new[]{"RequiredBaseTypes","ExcludedBaseTypes","PreferredBaseTypes","PreferredNotBaseTypes"},name))=bits;
        }
        Ep1ToggleEnum(node.GetAttributeValue("SortOrder","PREFER_NEWEST"),(uint*)&objT->SortOrder,state,typeof(AIBaseSortingOrder),false);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate an optional criteria record only when the source authors it. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,AIBaseSelectionCriteria** objT,Tracker state)
    {
        if(node==null)return;state.Push((void**)objT,(uint)sizeof(AIBaseSelectionCriteria),1);Marshal(node,*objT,state);state.Pop();
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile SkirmishAI scalars, optional masks, base criteria and dodge filters; contextual-power entries need their separate native model. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SkirmishAIInformation* objT,Tracker state)
    {
        if(node==null)return;if((node.GetChildNodes("AIContextualPower")?.GetCount()??0)!=0)throw new InvalidDataException("EP1 SkirmishAI AIContextualPower native model is not implemented.");
        Marshal(node.GetAttributeValue("BaseBuildingLocation","FRONT"),&objT->BaseBuildingLocation,state);Marshal(node.GetAttributeValue("AdditionalRotation","0r"),&objT->AdditionalRotation,state);Marshal(node.GetAttributeValue("BaseBuildingMaxNudges","3"),&objT->BaseBuildingMaxNudges,state);Marshal(node.GetAttributeValue("CrushSpeedBonus","0.0"),&objT->CrushSpeedBonus,state);Marshal(node.GetAttributeValue("OutOfRangePenalty","1.3"),&objT->OutOfRangePenalty,state);
        Marshal(node.GetAttributeValue("ConquerMetricsOverrideDPS",null),&objT->ConquerMetricsOverrideDPS,state);Marshal(node.GetAttributeValue("ConquerMetricsOverrideDamageType","UNDEFINED"),&objT->ConquerMetricsOverrideDamageType,state);Marshal(node.GetAttributeValue("ConquerMetricsOverrideAntiMask","ANTI_GROUND ANTI_WATER"),&objT->ConquerMetricsOverrideAntiMask,state);Marshal(node.GetAttributeValue("ConquerMetricsOverrideForbiddenAntiMask",""),&objT->ConquerMetricsOverrideForbiddenAntiMask,state);
        // Reborn: optional masks allocate a pointee, distinct from the inline attack/forbidden masks.
        var defending=node.GetAttributeValue("ConquerMetricsOverrideDefendingAntiMask",null);if(defending!=null){state.Push((void**)&objT->ConquerMetricsOverrideDefendingAntiMask,(uint)sizeof(WeaponAntiBitFlags),1);Marshal(defending,objT->ConquerMetricsOverrideDefendingAntiMask,state);state.Pop();}
        Marshal(node.GetAttributeValue("ConquerMetricsDpsBonusAgainstDefenses","1.0"),&objT->ConquerMetricsDpsBonusAgainstDefenses,state);Ep1ToggleEnum(node.GetAttributeValue("NearResourceNodeType","ORE"),(uint*)&objT->NearResourceNodeType,state,typeof(Ep1EconomyResourceType),false);Marshal(node.GetAttributeValue("DodgeSafetyDistance","300.0"),&objT->DodgeSafetyDistance,state);Marshal(node.GetAttributeValue("OverrideInnerDistanceToTarget",null),&objT->OverrideInnerDistanceToTarget,state);Marshal(node.GetAttributeValue("OverrideOuterDistanceToTarget",null),&objT->OverrideOuterDistanceToTarget,state);Marshal(node.GetAttributeValue("DodgingRequiredObjectStatus",null),&objT->DodgingRequiredObjectStatus,state);
        Marshal(node.GetAttributeValue("BlockIfUnpackTargetBuildingCannotBeRebuilt","true"),&objT->BlockIfUnpackTargetBuildingCannotBeRebuilt,state);Marshal(node.GetAttributeValue("UnitBuilderStandardCombatUnit","false"),&objT->UnitBuilderStandardCombatUnit,state);Marshal(node.GetAttributeValue("TargetDuringCrush","false"),&objT->TargetDuringCrush,state);Marshal(node.GetAttributeValue("ContainedMembersProvideWeapon","false"),&objT->ContainedMembersProvideWeapon,state);Marshal(node.GetAttributeValue("CanFireUpCliffs","false"),&objT->CanFireUpCliffs,state);Marshal(node.GetAttributeValue("DoesSecondaryDamage","false"),&objT->DoesSecondaryDamage,state);
        Marshal(node.GetChildNode("ConstructionBaseSelectionCriteria",null),&objT->ConstructionBaseSelectionCriteria,state);Marshal(node.GetChildNode("ResourceNodeBaseClosenessTestSelectionCriteria",null),&objT->ResourceNodeBaseClosenessTestSelectionCriteria,state);Marshal(node.GetChildNode("DodgeWithUnits",null),&objT->DodgeWithUnits,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use EP1 crusher internal offsets and complete scalar/reference/flag source semantics; extra-crush level and kill-delay lists are source-sized and retain nested filters and conditions. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CrusherLevelModelConditionInfo* objT,Tracker state)
    {
        if(node==null)return;
        // Reborn: EP1 stores the condition mask inline; an omitted/empty mask leaves the zero span words.
        Marshal(node.GetAttributeValue("ModelConditionMatch",""),&objT->ModelConditionMatch,state);Marshal(node.GetChildNode("ObjectFilter",null),&objT->ObjectFilter,state);
        Marshal(node.GetAttributeValue("CrusherLevel","0"),&objT->CrusherLevel,state);Marshal(node.GetAttributeValue("CrushableLevel","127"),&objT->CrushableLevel,state);Marshal(node.GetAttributeValue("CrushEqualLevelProps","false"),&objT->CrushEqualLevelProps,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 kill-delay records own their ObjectFilter inline. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CrushKillDelayForObjectFilter* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("CrushKillDelay",".5s"),&objT->CrushKillDelay,state);Marshal(node.GetAttributeValue("CrushDecelerationPercent","0"),&objT->CrushDecelerationPercent,state);Marshal(node.GetChildNode("ObjectFilter",null),&objT->ObjectFilter,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 crusher root record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CrusherInfo* objT,Tracker state)
    {
        if(node==null)return;
        // Reborn: both EP1 list headers own repeated native child records and all nested condition/filter allocations.
        Marshal(node.GetChildNodes("ExtraCrushLevels"),&objT->ExtraCrushLevels,state);
        Marshal(node.GetChildNodes("ExtraCrushKillDelays"),&objT->ExtraCrushKillDelays,state);
        Marshal(node.GetAttributeValue("CrusherLevel","0"),&objT->CrusherLevel,state);Marshal(node.GetAttributeValue("CrushableLevel","127"),&objT->CrushableLevel,state);Marshal(node.GetAttributeValue("CrushKnockback","0"),&objT->CrushKnockback,state);Marshal(node.GetAttributeValue("CrushZFactor","1"),&objT->CrushZFactor,state);Marshal(node.GetAttributeValue("MinCrushVelocityPercent","1"),&objT->MinCrushVelocityPercent,state);Marshal(node.GetAttributeValue("CrushDecelerationPercent","0"),&objT->CrushDecelerationPercent,state);Marshal(node.GetAttributeValue("CrushWeapon",null),&objT->CrushWeapon,state);Marshal(node.GetAttributeValue("DefaultCrushKillDelay",".5s"),&objT->DefaultCrushKillDelay,state);
        Marshal(node.GetAttributeValue("CrushEqualLevelProps","false"),&objT->CrushEqualLevelProps,state);Marshal(node.GetAttributeValue("UseCrushAttack","true"),&objT->UseCrushAttack,state);Marshal(node.GetAttributeValue("CrushOnlyWhileCharging","false"),&objT->CrushOnlyWhileCharging,state);Marshal(node.GetAttributeValue("CrushAllies","false"),&objT->CrushAllies,state);Marshal(node.GetAttributeValue("CrushAircraftWhileStationary","false"),&objT->CrushAircraftWhileStationary,state);Marshal(node.GetAttributeValue("UseDirectionCheck","true"),&objT->UseDirectionCheck,state);Marshal(node.GetAttributeValue("ShouldSquishOnCollide","true"),&objT->ShouldSquishOnCollide,state);Marshal(node.GetAttributeValue("DecelerateForAllCollides","false"),&objT->DecelerateForAllCollides,state);Marshal(node.GetAttributeValue("CannotCrushTarget","false"),&objT->CannotCrushTarget,state);
    }
}
