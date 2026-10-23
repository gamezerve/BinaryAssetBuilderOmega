using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 engineer marshaling; blank optional references stay absent, and pointee allocation order matches the stock refinery. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EngineerContainModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EngineerContainModuleData.ImmediatelyEnabled),"false"),&objT->ImmediatelyEnabled,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.ReplaceWith)),&objT->ReplaceWith,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.NameOfVoiceToUseForFriendlyEnter)),&objT->NameOfVoiceToUseForFriendlyEnter,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.NameOfVoiceToUseForHostileEnter)),&objT->NameOfVoiceToUseForHostileEnter,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EngineerContainModuleData.ObjectStatusToSet),"NO_REFUND"),&objT->ObjectStatusToSet,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.FXForRepair)),&objT->FXForRepair,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.FXForCapture)),&objT->FXForCapture,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.FXForReplace)),&objT->FXForReplace,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.FXForCaptureAndReplace)),&objT->FXForCaptureAndReplace,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.EvaEventForRepair)),&objT->EvaEventForRepair,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.EvaEventForCapture)),&objT->EvaEventForCapture,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.EvaEventForReplace)),&objT->EvaEventForReplace,state);
        Marshal(EngineerValue(node,nameof(Ep1EngineerContainModuleData.EvaEventForCaptureAndReplace)),&objT->EvaEventForCaptureAndReplace,state);
        Marshal(node.GetChildNode(nameof(Ep1EngineerContainModuleData.CanEnterFilter),null),&objT->CanEnterFilter,state);
        Marshal(node,(ContainModuleData*)objT,state);
    }

    // Reborn: the EP1 schema's empty EvaEvent default and blank optional names mean "no reference", never an empty-name import.
    private static Value EngineerValue(Node node,string name){var value=node.GetAttributeValue(name,null);return value!=null&&value.GetText().Trim().Length!=0?value:null;}
}
