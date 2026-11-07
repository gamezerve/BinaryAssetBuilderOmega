using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain all original gate timings, proxy text, geometry lists, audio imports and native update identity. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,GateOpenAndCloseBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.ResetFrames),"0"),&target->ResetFrames,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.PercentOpenForPathing),"0"),&target->PercentOpenForPathing,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.ProxyUnitsName),null),&target->ProxyUnitsName,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.SoundOpeningGateLoop),null),&target->SoundOpeningGateLoop,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.SoundFinishedOpeningGate),null),&target->SoundFinishedOpeningGate,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.SoundClosingGateLoop),null),&target->SoundClosingGateLoop,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.SoundFinishedClosingGate),null),&target->SoundFinishedClosingGate,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.TimeBeforePlayingOpenSound),"0"),&target->TimeBeforePlayingOpenSound,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.TimeBeforePlayingClosedSound),"0"),&target->TimeBeforePlayingClosedSound,state);
        Marshal(node.GetChildNodes(nameof(GateOpenAndCloseBehaviorModuleData.GeometriesForOpen)),&target->GeometriesForOpen,state);
        Marshal(node.GetChildNodes(nameof(GateOpenAndCloseBehaviorModuleData.GeometriesForClosed)),&target->GeometriesForClosed,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.OpenByDefault),"false"),&target->OpenByDefault,state);
        Marshal(node.GetAttributeValue(nameof(GateOpenAndCloseBehaviorModuleData.RepelCollidingUnits),"false"),&target->RepelCollidingUnits,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
}
