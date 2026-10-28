using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the stock-qualified EP1 attach layout, typed EVA/FX references and optional masks with official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1AttachUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentStatus), null), &objT->ParentStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentStatusAttached), null), &objT->ParentStatusAttached, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.AttachedObjectStatus), null), &objT->AttachedObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.AttachedModelConditions), null), &objT->AttachedModelConditions, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ForbiddenParentStatus), null), &objT->ForbiddenParentStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.IgnoreForbiddenParentStatusStatus), null), &objT->IgnoreForbiddenParentStatusStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentStatusToCopy), null), &objT->ParentStatusToCopy, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentStatusToPrefer), null), &objT->ParentStatusToPrefer, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.Range), null), &objT->Range, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.CloseEnoughRange), "1000.0"), &objT->CloseEnoughRange, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentOwnerAttachmentEvaEvent), null), &objT->ParentOwnerAttachmentEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentAllyAttachmentEvaEvent), null), &objT->ParentAllyAttachmentEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentEnemyAttachmentEvaEvent), null), &objT->ParentEnemyAttachmentEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.AttachFXList), null), &objT->AttachFXList, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.DetachFXList), null), &objT->DetachFXList, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentOwnerDiedEvaEvent), null), &objT->ParentOwnerDiedEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentAllyDiedEvaEvent), null), &objT->ParentAllyDiedEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentEnemyDiedEvaEvent), null), &objT->ParentEnemyDiedEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.InitialAttachDelay), null), &objT->InitialAttachDelay, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.IdleScanDelay), "0.35s"), &objT->IdleScanDelay, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.Flags), "STICK_TO_PARENT TELEPORT USE_GEOMETRY"), &objT->Flags, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentDeathTypeToListenFor), "ALL"), &objT->ParentDeathTypeToListenFor, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.NoDieIfStatusActive), null), &objT->NoDieIfStatusActive, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ParentPositionElevation), null), &objT->ParentPositionElevation, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.BounceAmount), "0.2"), &objT->BounceAmount, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.BounceTimeout), "0.33s"), &objT->BounceTimeout, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.DamageTypesToNotLeech), null), &objT->DamageTypesToNotLeech, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.DeathTypesToNotLeech), null), &objT->DeathTypesToNotLeech, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.ForbiddenParentStatusDieDeathType), "NORMAL"), &objT->ForbiddenParentStatusDieDeathType, state);
        Marshal(node.GetChildNode(nameof(Ep1AttachUpdateModuleData.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node.GetChildNodes(nameof(Ep1AttachUpdateModuleData.ModifierToLeechFromParent)), &objT->ModifierToLeechFromParent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttachUpdateModuleData.AttachBoneName), ""), &objT->AttachBoneName, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
