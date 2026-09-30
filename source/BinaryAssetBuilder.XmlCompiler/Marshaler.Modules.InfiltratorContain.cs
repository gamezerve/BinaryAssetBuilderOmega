using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the recovered infiltrator fields; named defaults still require upstream reference normalization. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, InfiltratorContainModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.BlockedDuration), "2s"), &objT->BlockedDuration, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.InfiltratedStatusFilter), "DESTROYED UNDER_CONSTRUCTION SOLD UNPACKING"), &objT->InfiltratedStatusFilter, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.CanEnterFilter), "InfiltrationCanEnterObjectFilter"), &objT->CanEnterFilter, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.ReplaceWith), null), &objT->ReplaceWith, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.NameOfVoiceToUseForHostileEnter), "VoiceInfiltrate"), &objT->NameOfVoiceToUseForHostileEnter, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.NameOfVoiceToUseForInfiltrationDeath), "VoiceInfiltrateDeath"), &objT->NameOfVoiceToUseForInfiltrationDeath, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.EvaEventForInfiltratingPlayer), "EnemyBuildingInfiltrated"), &objT->EvaEventForInfiltratingPlayer, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.EvaEventForInfiltratedPlayer), "OurBuildingInfiltrated"), &objT->EvaEventForInfiltratedPlayer, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.FXForInfiltrate), "FX_Building_Infiltrated_Generic"), &objT->FXForInfiltrate, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.Duration), "0s"), &objT->Duration, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.UnitDuration), "0s"), &objT->UnitDuration, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.Amount), "0.0"), &objT->Amount, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.Effect), null), &objT->Effect, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.ObjectRef), null), &objT->ObjectRef, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.UnitFilter), null), &objT->UnitFilter, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.StructureFilter), null), &objT->StructureFilter, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.ObjectStatusToSet), "NO_REFUND"), &objT->ObjectStatusToSet, state);
        Marshal(node.GetAttributeValue(nameof(InfiltratorContainModuleData.ImmediatelyEnabled), "false"), &objT->ImmediatelyEnabled, state);
        Marshal(node, (ContainModuleData*)objT, state);
    }
}
