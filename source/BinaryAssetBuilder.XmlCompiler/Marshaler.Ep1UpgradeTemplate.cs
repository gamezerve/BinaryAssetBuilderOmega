using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the complete EP1 upgrade record with real Core EVA/audio/FX imports and source-owned optional dependency data. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UpgradeTemplate* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.DisplayName),null),&value->DisplayName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.Description),null),&value->Description,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.TypeDescription),null),&value->TypeDescription,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.AcquireHint),null),&value->AcquireHint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.Type),null),&value->Type,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.BuildTime),null),&value->BuildTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.BuildCost),null),&value->BuildCost,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.ResearchSound),null),&value->ResearchSound,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.ResearchCompleteEvaEvent),null),&value->ResearchCompleteEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.UnitSpecificSound),null),&value->UnitSpecificSound,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.UpgradeFX),null),&value->UpgradeFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.LocalPlayerGainsUpgradeEvaEvent),null),&value->LocalPlayerGainsUpgradeEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.AlliedPlayerGainsUpgradeEvaEvent),null),&value->AlliedPlayerGainsUpgradeEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.EnemyPlayerGainsUpgradeEvaEvent),null),&value->EnemyPlayerGainsUpgradeEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.LocalPlayerLosesUpgradeEvaEvent),null),&value->LocalPlayerLosesUpgradeEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.AlliedPlayerLosesUpgradeEvaEvent),null),&value->AlliedPlayerLosesUpgradeEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.EnemyPlayerLosesUpgradeEvaEvent),null),&value->EnemyPlayerLosesUpgradeEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.LocalPlayerProductionStartedEvaEvent),null),&value->LocalPlayerProductionStartedEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.LocalPlayerBuildOnHoldEvaEvent),null),&value->LocalPlayerBuildOnHoldEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.LocalPlayerBuildCancelledEvaEvent),null),&value->LocalPlayerBuildCancelledEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.UseObjectTemplateForCostDiscount),null),&value->UseObjectTemplateForCostDiscount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.GroupName),null),&value->GroupName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.GroupOrder),null),&value->GroupOrder,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.SkirmishAIHeuristic),null),&value->SkirmishAIHeuristic,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.Options),null),&value->Options,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.IconImage),null),&value->IconImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UpgradeTemplate.WaypointQueueable),null),&value->WaypointQueueable,state);
        Marshal(node.GetChildNode(nameof(Ep1UpgradeTemplate.GameDependency),null),&value->GameDependency,state);
        Marshal(node,(BaseInheritableAsset*)value,state);
    }
}
