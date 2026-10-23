using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal every EP1 Spawn attribute and optional child on the same enclosing Tracker, without the incompatible KW packed-flag offsets. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpawnBehaviorModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.SpawnNumberData),"0"),&objT->SpawnNumberData,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.SpawnReplaceDelayData),"0s"),&objT->SpawnReplaceDelayData,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.InitialBurst),"0"),&objT->InitialBurst,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.DamageTypesToPropagateToSlaves),"ALL"),&objT->DamageTypesToPropagateToSlaves,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.FadeInTime),"0s"),&objT->FadeInTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.InitialDelay),"0s"),&objT->InitialDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.IsOneShotData),"false"),&objT->IsOneShotData,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.CanReclaimOrphans),"false"),&objT->CanReclaimOrphans,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.AggregateHealth),"false"),&objT->AggregateHealth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.ExitByBudding),"false"),&objT->ExitByBudding,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.SpawnedRequireSpawner),"false"),&objT->SpawnedRequireSpawner,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.RespectCommandLimit),"false"),&objT->RespectCommandLimit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.KillSpawnBasedOnModelConditionState),"false"),&objT->KillSpawnBasedOnModelConditionState,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.ShareUpgrades),"false"),&objT->ShareUpgrades,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.SpawnInsideBuilding),"false"),&objT->SpawnInsideBuilding,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.CombineOnCreate),"false"),&objT->CombineOnCreate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.KillSpawnsOnDisabled),"false"),&objT->KillSpawnsOnDisabled,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.PassExperienceToSpawned),"false"),&objT->PassExperienceToSpawned,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.KillSpawnsOnCaptured),"false"),&objT->KillSpawnsOnCaptured,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.KillSpawnsWhenObjectInvalid),"false"),&objT->KillSpawnsWhenObjectInvalid,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.SpawnOnUnpack),"false"),&objT->SpawnOnUnpack,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpawnBehaviorModuleData.SetPurchasePrice),"false"),&objT->SetPurchasePrice,state);
        Marshal(node.GetChildNode(nameof(Ep1SpawnBehaviorModuleData.Die),null),&objT->Die,state);
        Marshal(node.GetChildNodes(nameof(Ep1SpawnBehaviorModuleData.SpawnTemplate)),&objT->SpawnTemplate,state);
        Marshal(node.GetChildNode(nameof(Ep1SpawnBehaviorModuleData.ObjectStatusValidation),null),&objT->ObjectStatusValidation,state);
        Marshal(node,(UpgradeModuleData*)objT,state);
    }
}
