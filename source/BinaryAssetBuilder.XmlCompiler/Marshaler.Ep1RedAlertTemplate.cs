using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the full EP1 Red Alert economy/threat record and link the authored EVA and attribute-modifier targets. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1RedAlertTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.OrangeAreaThreatThreshold),"0"),&target->OrangeAreaThreatThreshold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAreaThreatThreshold),"0"),&target->RedAreaThreatThreshold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.MaxAllowedThreat),"0"),&target->MaxAllowedThreat,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.KillThreatMultiplier),"1.0"),&target->KillThreatMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.DeathThreatMultiplier),"1.0"),&target->DeathThreatMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.YellowAreaThreatDecayPerSecond),"5"),&target->YellowAreaThreatDecayPerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.OrangeAreaThreatDecayPerSecond),"5"),&target->OrangeAreaThreatDecayPerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAreaThreatDecayPerSecond),"5"),&target->RedAreaThreatDecayPerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAlertBuildingDamagePerSecond),"0"),&target->RedAlertBuildingDamagePerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAlertUpdatedBuildingDamagePerSecond),"0"),&target->RedAlertUpdatedBuildingDamagePerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAlertBuildingDamageIncreaseTime),"0s"),&target->RedAlertBuildingDamageIncreaseTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.EvaEventToPlayOnRedAlertInitiateOwner),null),&target->EvaEventToPlayOnRedAlertInitiateOwner,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.EvaEventToPlayOnRedAlertInitiateAlly),null),&target->EvaEventToPlayOnRedAlertInitiateAlly,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.EvaEventToPlayOnRedAlertInitiateEnemy),null),&target->EvaEventToPlayOnRedAlertInitiateEnemy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.YellowAlertTechPointMultiplier),"1.0"),&target->YellowAlertTechPointMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.OrangeAlertTechPointMultiplier),"1.0"),&target->OrangeAlertTechPointMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAlertTechPointMultiplier),"1.0"),&target->RedAlertTechPointMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.MaxDepositChunkSize),"100"),&target->MaxDepositChunkSize,state);
        Marshal(node.GetChildNode(nameof(Ep1RedAlertTemplate.YellowAreaEffect),null),&target->YellowAreaEffect,state);
        Marshal(node.GetChildNode(nameof(Ep1RedAlertTemplate.OrangeAreaEffect),null),&target->OrangeAreaEffect,state);
        Marshal(node.GetChildNode(nameof(Ep1RedAlertTemplate.RedAreaEffect),null),&target->RedAreaEffect,state);
        Marshal(node.GetChildNode(nameof(Ep1RedAlertTemplate.RedAlertEffect),null),&target->RedAlertEffect,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.DisableNonRepairability),"false"),&target->DisableNonRepairability,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAlertDisableStructureBuilding),"true"),&target->RedAlertDisableStructureBuilding,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RedAlertTemplate.RedAlertShowAllUnitsAndStructures),"true"),&target->RedAlertShowAllUnitsAndStructures,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }
}
