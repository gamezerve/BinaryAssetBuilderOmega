using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: the original editor blast-door record has a 68-byte update header with inline proxy text and trailing gate flags.
[StructLayout(LayoutKind.Sequential)]
public struct GateOpenAndCloseBehaviorModuleData
{
    public UpdateModuleData Base;
    public uint ResetFrames,PercentOpenForPathing;
    public AnsiString ProxyUnitsName;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> SoundOpeningGateLoop,SoundFinishedOpeningGate,SoundClosingGateLoop,SoundFinishedClosingGate;
    public uint TimeBeforePlayingOpenSound,TimeBeforePlayingClosedSound;
    public List<AnsiString> GeometriesForOpen,GeometriesForClosed;
    public SageBool OpenByDefault,RepelCollidingUnits;
}
