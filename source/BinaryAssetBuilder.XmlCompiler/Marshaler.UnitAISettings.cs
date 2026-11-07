using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write all UnitAISettings fields in the EP1 singleton layout, reusing the normal filter and reference writers. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UnitAISettings* objT, Tracker state)
    {
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.SupplyBoxesPerTree), "90"), &objT->SupplyBoxesPerTree, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.ValuePerSupplyBox), "400"), &objT->ValuePerSupplyBox, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.MinDistFromEdgeOfMapForBuild), "30.0"), &objT->MinDistFromEdgeOfMapForBuild, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.SupplyBuildBorder), "20.0"), &objT->SupplyBuildBorder, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.MaxLineBuildObjects), "50"), &objT->MaxLineBuildObjects, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.MaxTunnelCapacity), "3"), &objT->MaxTunnelCapacity, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.MinLowEnergyProductionSpeed), "0.5"), &objT->MinLowEnergyProductionSpeed, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.MaxLowEnergyProductionSpeed), "0.8"), &objT->MaxLowEnergyProductionSpeed, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.LowEnergyPenaltyModifier), "1.0"), &objT->LowEnergyPenaltyModifier, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.DamageRadiusMinimumForSplash), "0.0"), &objT->DamageRadiusMinimumForSplash, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.SellPercentage), "50%"), &objT->SellPercentage, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.UnlookPersistDuration), "10s"), &objT->UnlookPersistDuration, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.DefaultStructureRubbleHeight), "8.0"), &objT->DefaultStructureRubbleHeight, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.TimeAfterDamageUntilRepairAllowed), "0"), &objT->TimeAfterDamageUntilRepairAllowed, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.SelectionFlashSaturationFactor), "0.5"), &objT->SelectionFlashSaturationFactor, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.GarrisonedRangeMultiplier), "1.5"), &objT->GarrisonedRangeMultiplier, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.GroupMoveClickToGatherAreaFactor), "0.5"), &objT->GroupMoveClickToGatherAreaFactor, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.FormationMoveClusterRange), "200.0"), &objT->FormationMoveClusterRange, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.InvisibilityOpacityCycleFrames), "60"), &objT->InvisibilityOpacityCycleFrames, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.BuilderFadeOutTime), "1s"), &objT->BuilderFadeOutTime, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.BuilderFadeInTime), "1s"), &objT->BuilderFadeInTime, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.BuilderMoveFromNewStructureDistance), "10.0"), &objT->BuilderMoveFromNewStructureDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.PartitionCellSize), "40.0"), &objT->PartitionCellSize, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.TerrainResourceCellSize), "20.0"), &objT->TerrainResourceCellSize, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.DefaultStartingCash), "1500"), &objT->DefaultStartingCash, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.MovementPenaltyDamageState), "REALLY_DAMAGED"), &objT->MovementPenaltyDamageState, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.ParachuteDriftSpeed), "2.0"), &objT->ParachuteDriftSpeed, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.ParachuteLandTime), "1.5s"), &objT->ParachuteLandTime, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.LinearDragSubmerged), "0.85"), &objT->LinearDragSubmerged, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.AutomaticReverseAngle), "10d"), &objT->AutomaticReverseAngle, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.AutomaticReverseDistance), "40"), &objT->AutomaticReverseDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.SkipMoveMinDistance), "10.0"), &objT->SkipMoveMinDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.CompleteTargetingCompareList), null), &objT->CompleteTargetingCompareList, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.ScatterDistance), "100.0"), &objT->ScatterDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.AttackMovementPersistFrames), "2"), &objT->AttackMovementPersistFrames, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.AIIgnoreDuplicateMoveCommandDistance), "20.0"), &objT->AIIgnoreDuplicateMoveCommandDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.HumanIgnoreDuplicateMoveCommandDistance), "5.0"), &objT->HumanIgnoreDuplicateMoveCommandDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.ConsiderHumanDuplicateMoveCommandDistance), "100.0"), &objT->ConsiderHumanDuplicateMoveCommandDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.CaptureWaitDistance), "20.0"), &objT->CaptureWaitDistance, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.CaptureWaitTime), "2s"), &objT->CaptureWaitTime, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.CaptureWaitModelConditions), null), &objT->CaptureWaitModelConditions, state);
        Marshal(node.GetChildNode(nameof(UnitAISettings.VictoryConditionUnitObjectFilter), null), &objT->VictoryConditionUnitObjectFilter, state);
        Marshal(node.GetChildNode(nameof(UnitAISettings.VictoryConditionStructureObjectFilter), null), &objT->VictoryConditionStructureObjectFilter, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.SelectionFlashHouseColor), "false"), &objT->SelectionFlashHouseColor, state);
        Marshal(node.GetAttributeValue(nameof(UnitAISettings.EnableHouseColor), "true"), &objT->EnableHouseColor, state);
    }
}
