using System;
using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize exact original AIEnemySortingType ordinals, preserving negative sentinel values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIEnemySortingType* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AIEnemySortingType>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AIEnemySortingType)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize exact original AIFeedbackWorthySkirmishGameEvent ordinals, preserving negative sentinel values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIFeedbackWorthySkirmishGameEvent* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AIFeedbackWorthySkirmishGameEvent>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AIFeedbackWorthySkirmishGameEvent)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize exact original AIStrategicDirective ordinals, preserving negative sentinel values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIStrategicDirective* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AIStrategicDirective>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AIStrategicDirective)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize exact original AIStrategicDirectiveEvent ordinals, preserving negative sentinel values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIStrategicDirectiveEvent* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AIStrategicDirectiveEvent>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AIStrategicDirectiveEvent)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize exact original AIUnitBuilderUnitChoiceShape ordinals, preserving negative sentinel values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIUnitBuilderUnitChoiceShape* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AIUnitBuilderUnitChoiceShape>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AIUnitBuilderUnitChoiceShape)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain an optional player-power strong handle only when its source value is present. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,AssetReference<BaseAssetType>** target,Tracker state)
    { if(value is null)return;using var context=state.Push((void**)target,4,1);Marshal(value,*target,state); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the optional original opening-move heuristic list in its own record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIHeuristicChoice* target,Tracker state)
    { if(node is null)return;Marshal(node,&target->Value,state); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve absence of an optional heuristic record rather than allocating an empty list automatically. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIHeuristicChoice** target,Tracker state)
    { if(node is null)return;using var context=state.Push((void**)target,8,1);Marshal(node,*target,state); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIStrategicState attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStrategicState* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicState.State),null),&target->State,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicState.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIBudgetState attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIBudgetState* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIBudgetState.State),null),&target->State,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBudgetState.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIBuildState attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIBuildState* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildState.State),null),&target->State,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildState.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIBuildDelay attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIBuildDelay* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelay.Delay),"0s"),&target->Delay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelay.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIResourceMultiplierCheat attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIResourceMultiplierCheat* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIResourceMultiplierCheat.Percentage),"100%"),&target->Percentage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIResourceMultiplierCheat.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIBuildDelayRange attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIBuildDelayRange* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelayRange.id),null),&target->id,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelayRange.MinDelay),"0s"),&target->MinDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelayRange.MaxDelay),"0s"),&target->MaxDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelayRange.MinTTKRatio),"1"),&target->MinTTKRatio,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelayRange.MaxTTKRatio),"1"),&target->MaxTTKRatio,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildDelayRange.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIUnitBuilderUnitChoiceStrategy attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIUnitBuilderUnitChoiceStrategy* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategy.Shape),"FLAT"),&target->Shape,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategy.StandardDeviation),"1"),&target->StandardDeviation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategy.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIUnitBuilderUnitChoiceStrategyAdaptive attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIUnitBuilderUnitChoiceStrategyAdaptive* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategyAdaptive.MinEffectiveness),"0.5"),&target->MinEffectiveness,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategyAdaptive.MaxEffectiveness),"0.5"),&target->MaxEffectiveness,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategyAdaptive.MinStandardDeviation),"0.5"),&target->MinStandardDeviation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategyAdaptive.MaxStandardDeviation),"0.5"),&target->MaxStandardDeviation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategyAdaptive.MinEfficiency),"1"),&target->MinEfficiency,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategyAdaptive.MaxEfficiency),"1"),&target->MaxEfficiency,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderUnitChoiceStrategyAdaptive.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIUnitBuilderEvaluationDelayRange attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIUnitBuilderEvaluationDelayRange* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderEvaluationDelayRange.UseAllAvailableQueues),"true"),&target->UseAllAvailableQueues,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderEvaluationDelayRange.MinDelay),"0s"),&target->MinDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderEvaluationDelayRange.MaxDelay),"0s"),&target->MaxDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderEvaluationDelayRange.MinEfficiency),"1"),&target->MinEfficiency,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderEvaluationDelayRange.MaxEfficiency),"1"),&target->MaxEfficiency,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderEvaluationDelayRange.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIPersonalityDefMap attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIPersonalityDefMap* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefMap.Side),null),&target->Side,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefMap.Personality),null),&target->Personality,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIWeightedOpeningMove attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIWeightedOpeningMove* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIWeightedOpeningMove.Name),null),&target->Name,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIWeightedOpeningMove.Priority),"0"),&target->Priority,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIWeightedOpeningMove.Weight),"100%"),&target->Weight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIWeightedOpeningMove.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
        Marshal(node.GetChildNode(nameof(Ep1AIWeightedOpeningMove.Heuristic),null),&target->Heuristic,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIStrategicDirectiveFeedbackMap attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStrategicDirectiveFeedbackMap* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicDirectiveFeedbackMap.StrategicDirective),null),&target->StrategicDirective,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicDirectiveFeedbackMap.Event),null),&target->Event,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicDirectiveFeedbackMap.FeedbackKey),null),&target->FeedbackKey,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AISkirmishGameEventFeedbackMap attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AISkirmishGameEventFeedbackMap* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AISkirmishGameEventFeedbackMap.Event),null),&target->Event,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AISkirmishGameEventFeedbackMap.FeedbackKey),null),&target->FeedbackKey,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIUnitCap attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIUnitCap* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitCap.Cap),"100"),&target->Cap,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitCap.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AISpecificUnitCap attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AISpecificUnitCap* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AISpecificUnitCap.Unit),null),&target->Unit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AISpecificUnitCap.Cap),"1"),&target->Cap,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AISpecificUnitCap.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIUnitBuilderModifier attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIUnitBuilderModifier* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderModifier.Unit),null),&target->Unit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderModifier.OffensiveModifier),"1.0"),&target->OffensiveModifier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderModifier.DefensiveModifier),"1.0"),&target->DefensiveModifier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIUnitBuilderModifier.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIStateFeedbackEntry attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateFeedbackEntry* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.FeedbackKey),null),&target->FeedbackKey,state);
        var evaEvent=node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.EvaEvent),null);if(evaEvent is not null&&evaEvent.GetText().Length!=0)Marshal(evaEvent,&target->EvaEvent,state);
        var sound=node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.Sound),null);if(sound is not null&&sound.GetText().Length!=0)Marshal(sound,&target->Sound,state);
        var text=node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.Text),null);if(text is not null&&text.GetText().Length!=0)Marshal(text,&target->Text,state);
        var video=node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.Video),null);if(video is not null&&video.GetText().Length!=0)Marshal(video,&target->Video,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.FeedbackTo),"ALLIES ENEMIES NEUTRAL"),&target->FeedbackTo,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.Timeout),null),&target->Timeout,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.OnlyPlayIfNoOtherAliveInstancesOfCommander),"false"),&target->OnlyPlayIfNoOtherAliveInstancesOfCommander,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateFeedbackEntry.ConditionOnFlag),null),&target->ConditionOnFlag,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIPlayerPowerChoice attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIPlayerPowerChoice* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIPlayerPowerChoice.PlayerPower),null),&target->PlayerPower,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPlayerPowerChoice.Weight),"20%"),&target->Weight,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIPersonalityDefinition attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIPersonalityDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.PersonalityType),null),&target->PersonalityType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.PersonalityUIName),null),&target->PersonalityUIName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.SkirmishPersonality),"false"),&target->SkirmishPersonality,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.CoopPersonality),"false"),&target->CoopPersonality,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.SecondsTillTargetsExpire),"15.0"),&target->SecondsTillTargetsExpire,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.DesiredExcessPowerBuffer),"0"),&target->DesiredExcessPowerBuffer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.BaseCompactness),"0.5"),&target->BaseCompactness,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.DefaultThreatFindRadius),"500.0"),&target->DefaultThreatFindRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UnitBuilderCostEffectivenessWeight),"1.0"),&target->UnitBuilderCostEffectivenessWeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UnitBuilderMoneyVersusTimePreference),"0.5"),&target->UnitBuilderMoneyVersusTimePreference,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UnitBuilderOverallOffensivePreference),"100%"),&target->UnitBuilderOverallOffensivePreference,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UnitBuilderOverallDefensivePreference),"100%"),&target->UnitBuilderOverallDefensivePreference,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UnitBuilderEnemyTowerWeight),"100%"),&target->UnitBuilderEnemyTowerWeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UnitBuilderEnemyNearbyDistance),"800.0"),&target->UnitBuilderEnemyNearbyDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UnitBuilderEnemyNearbyMultiplier),"4"),&target->UnitBuilderEnemyNearbyMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.BuildTechEfficiencyThreshold),"0.5"),&target->BuildTechEfficiencyThreshold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.BuildTechMinPowerAdvantage),"1.1"),&target->BuildTechMinPowerAdvantage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.BuildExtraFactoryMinTime),"240s"),&target->BuildExtraFactoryMinTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.ResourceNodeSearchRadius),"500.0"),&target->ResourceNodeSearchRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.ResourceNodeValueRemainingForExpansion),"3000"),&target->ResourceNodeValueRemainingForExpansion,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RefineryPlacementStartDistanceDelta),"0.0"),&target->RefineryPlacementStartDistanceDelta,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RefineryPlacementIncreaseTickAmount),"60.0"),&target->RefineryPlacementIncreaseTickAmount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RefineryPlacementNumberOfIncreaseTicksBeforeGivingUp),"3"),&target->RefineryPlacementNumberOfIncreaseTicksBeforeGivingUp,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.ReactiveDefenseRadius),"1200.0"),&target->ReactiveDefenseRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RepairBuildingsAtDifficulty),null),&target->RepairBuildingsAtDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.UsePlayerPowersAtDifficulty),"EASY MEDIUM HARD BRUTAL"),&target->UsePlayerPowersAtDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EmergencyManagerHandleNoPowerAtDifficulty),null),&target->EmergencyManagerHandleNoPowerAtDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EmergencyManagerHandleNoIncomeAtDifficulty),null),&target->EmergencyManagerHandleNoIncomeAtDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EmergencyManagerHandleNoConyardAtDifficulty),null),&target->EmergencyManagerHandleNoConyardAtDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EmergencyManagerHandleUnderAttackAtDifficulty),null),&target->EmergencyManagerHandleUnderAttackAtDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EmergencyManagerHandleConyardInDangerAtDifficulty),null),&target->EmergencyManagerHandleConyardInDangerAtDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.ConyardInDangerEmergencyUnderAttackTime),"10s"),&target->ConyardInDangerEmergencyUnderAttackTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.ConyardInDangerEmergencyHealthPercent),"50%"),&target->ConyardInDangerEmergencyHealthPercent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.ConyardInDangerEmergencyEngineerDistance),"200.0"),&target->ConyardInDangerEmergencyEngineerDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EnemySortingBrutalDifficulty),"NearestFirst_BreakTies"),&target->EnemySortingBrutalDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EnemySortingHardDifficulty),"NearestFirst_BreakTies"),&target->EnemySortingHardDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EnemySortingMediumDifficulty),"Flat"),&target->EnemySortingMediumDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EnemySortingEasyDifficulty),"StrongestFirst_AllowTies"),&target->EnemySortingEasyDifficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.EnemyResortingTime),"15s"),&target->EnemyResortingTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.CleanupRefineries),"false"),&target->CleanupRefineries,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.CanBuildInAlliedBuildRadii),"false"),&target->CanBuildInAlliedBuildRadii,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.StructureSaveChanceEasy),"0.0"),&target->StructureSaveChanceEasy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.StructureSaveChanceMedium),"0.0"),&target->StructureSaveChanceMedium,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.StructureSaveChanceHard),"0.0"),&target->StructureSaveChanceHard,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.BaseBuilderKeepUnderAttackFlagOnTime),"4s"),&target->BaseBuilderKeepUnderAttackFlagOnTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.BaseBuilderUpdateUnderAttackTime),"200ms"),&target->BaseBuilderUpdateUnderAttackTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.BaseBuilderUpdateEnemiesInBaseTime),"1s"),&target->BaseBuilderUpdateEnemiesInBaseTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.MinHarvesters),"0"),&target->MinHarvesters,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.MaxStrategicTargets),"32"),&target->MaxStrategicTargets,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.AssaultBeaconRadius),"300.0"),&target->AssaultBeaconRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.HoldObjectBeaconRadius),"300.0"),&target->HoldObjectBeaconRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.HoldPositionBeaconRadius),"300.0"),&target->HoldPositionBeaconRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RedAlertPowerAdvantage),"0.5"),&target->RedAlertPowerAdvantage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RedAlertIncome),"50.0"),&target->RedAlertIncome,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RedAlertProductionBuildings),"2"),&target->RedAlertProductionBuildings,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.RedAlertAttackTime),"15s"),&target->RedAlertAttackTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.MaxWallDestroyTimeToCountAsGoThroughWalls),"1s"),&target->MaxWallDestroyTimeToCountAsGoThroughWalls,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.CommanderPortrait),"AI_Portrait_TEMP"),&target->CommanderPortrait,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.SkirmishTimeTillGreeting),"1.5s"),&target->SkirmishTimeTillGreeting,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIPersonalityDefinition.CoopMonitorUpdateTime),"5s"),&target->CoopMonitorUpdateTime,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.Side)),&target->Side,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.PersonalityMap)),&target->PersonalityMap,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.UnitCap)),&target->UnitCap,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.BuildDelay)),&target->BuildDelay,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.BuildDelayRange)),&target->BuildDelayRange,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.UnitBuilderUnitChoiceStrategy)),&target->UnitBuilderUnitChoiceStrategy,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.UnitBuilderUnitChoiceStrategyAdaptive)),&target->UnitBuilderUnitChoiceStrategyAdaptive,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.UnitBuilderEvaluationDelayRange)),&target->UnitBuilderEvaluationDelayRange,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.ResourceMultiplierCheat)),&target->ResourceMultiplierCheat,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.StrategicDirectiveFeedback)),&target->StrategicDirectiveFeedback,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.SkirmishGameEventFeedback)),&target->SkirmishGameEventFeedback,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.PowerChoice)),&target->PowerChoice,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.OpeningMove)),&target->OpeningMove,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.StrategicState)),&target->StrategicState,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.BudgetState)),&target->BudgetState,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.BuildState)),&target->BuildState,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.SpecificUnitCap)),&target->SpecificUnitCap,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.UnitModifier)),&target->UnitModifier,state);
        Marshal(node.GetChildNode(nameof(Ep1AIPersonalityDefinition.DefaultTacticHomeBaseCriteria),null),&target->DefaultTacticHomeBaseCriteria,state);
        Marshal(node.GetChildNode(nameof(Ep1AIPersonalityDefinition.CountAsStructureForBuildPurposesFilter),null),&target->CountAsStructureForBuildPurposesFilter,state);
        Marshal(node.GetChildNode(nameof(Ep1AIPersonalityDefinition.InTroubleSkirmishFeedbackControl),null),&target->InTroubleSkirmishFeedbackControl,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIPersonalityDefinition.FeedbackEntry)),&target->FeedbackEntry,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original AIInTroubleSkirmishFeedbackControl attribute, graph and ordered list without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIInTroubleSkirmishFeedbackControl* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.OnlyIfLocalPlayerIs),null),&target->OnlyIfLocalPlayerIs,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.TimeoutAfterPlaying),null),&target->TimeoutAfterPlaying,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.TimeBetweenScans),null),&target->TimeBetweenScans,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.TimeThatConditionsMustBeMet),null),&target->TimeThatConditionsMustBeMet,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.NumberOfObjectsRecentlyLost),null),&target->NumberOfObjectsRecentlyLost,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.TimeForNumberOfObjectLost),null),&target->TimeForNumberOfObjectLost,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.ObjectLostFilter),null),&target->ObjectLostFilter,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIInTroubleSkirmishFeedbackControl.MinPowerAdvantageEnemiesInMyBaseVsAllMyUnits),null),&target->MinPowerAdvantageEnemiesInMyBaseVsAllMyUnits,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate an optional original trouble-feedback control only when authored. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIInTroubleSkirmishFeedbackControl** target,Tracker state)
    { if(node is null)return;using var context=state.Push((void**)target,(uint)sizeof(Ep1AIInTroubleSkirmishFeedbackControl),1);Marshal(node,*target,state); }
}
