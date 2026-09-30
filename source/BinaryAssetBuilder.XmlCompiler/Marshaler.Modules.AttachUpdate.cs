using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the recovered RA3 attach layout and EP1 bone-name extension with schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, AttachUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentStatus), null), &objT->ParentStatus, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentStatusAttached), null), &objT->ParentStatusAttached, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.AttachedObjectStatus), null), &objT->AttachedObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.AttachedModelConditions), null), &objT->AttachedModelConditions, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ForbiddenParentStatus), null), &objT->ForbiddenParentStatus, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.IgnoreForbiddenParentStatusStatus), null), &objT->IgnoreForbiddenParentStatusStatus, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentStatusToCopy), null), &objT->ParentStatusToCopy, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentStatusToPrefer), null), &objT->ParentStatusToPrefer, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.Range), null), &objT->Range, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.CloseEnoughRange), "1000.0"), &objT->CloseEnoughRange, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentOwnerAttachmentEvaEvent), null), &objT->ParentOwnerAttachmentEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentAllyAttachmentEvaEvent), null), &objT->ParentAllyAttachmentEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentEnemyAttachmentEvaEvent), null), &objT->ParentEnemyAttachmentEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.AttachFXList), null), &objT->AttachFXList, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.DetachFXList), null), &objT->DetachFXList, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentOwnerDiedEvaEvent), null), &objT->ParentOwnerDiedEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentAllyDiedEvaEvent), null), &objT->ParentAllyDiedEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentEnemyDiedEvaEvent), null), &objT->ParentEnemyDiedEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.InitialAttachDelay), null), &objT->InitialAttachDelay, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.IdleScanDelay), "0.35s"), &objT->IdleScanDelay, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.Flags), "STICK_TO_PARENT TELEPORT USE_GEOMETRY"), &objT->Flags, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentDeathTypeToListenFor), "ALL"), &objT->ParentDeathTypeToListenFor, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.NoDieIfStatusActive), null), &objT->NoDieIfStatusActive, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ParentPositionElevation), null), &objT->ParentPositionElevation, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.BounceAmount), "0.2"), &objT->BounceAmount, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.BounceTimeout), "0.33s"), &objT->BounceTimeout, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.DamageTypesToNotLeech), null), &objT->DamageTypesToNotLeech, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.DeathTypesToNotLeech), null), &objT->DeathTypesToNotLeech, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.ForbiddenParentStatusDieDeathType), "NORMAL"), &objT->ForbiddenParentStatusDieDeathType, state);
        Marshal(node.GetChildNode(nameof(AttachUpdateModuleData.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node.GetChildNodes(nameof(AttachUpdateModuleData.ModifierToLeechFromParent)), &objT->ModifierToLeechFromParent, state);
        Marshal(node.GetAttributeValue(nameof(AttachUpdateModuleData.AttachBoneName), null), &objT->AttachBoneName, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
