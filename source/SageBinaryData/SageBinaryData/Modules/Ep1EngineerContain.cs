using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: 92-byte EP1 engineer record from the stock SovietRefinery witness. Unlike the KW model, ObjectStatusToSet is an inline
// mask, EVA events are inline strong references (not string pointers), and the filter pointer precedes the packed boolean.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1EngineerContainModuleData
{
    public ContainModuleData Base;
    public AssetReference<ObjectCreationList> ReplaceWith;
    public StringHash* NameOfVoiceToUseForFriendlyEnter;
    public StringHash* NameOfVoiceToUseForHostileEnter;
    public ObjectStatusBitFlags ObjectStatusToSet;
    public AssetReference<FXList>* FXForRepair;
    public AssetReference<FXList>* FXForCapture;
    public AssetReference<FXList>* FXForReplace;
    public AssetReference<FXList>* FXForCaptureAndReplace;
    public AssetReference<BaseAssetType> EvaEventForRepair;
    public AssetReference<BaseAssetType> EvaEventForCapture;
    public AssetReference<BaseAssetType> EvaEventForReplace;
    public AssetReference<BaseAssetType> EvaEventForCaptureAndReplace;
    public ObjectFilter* CanEnterFilter;
    public SageBool ImmediatelyEnabled;
}
