using System.Runtime.InteropServices;
using Relo;
using SageBinaryData;

// Reborn: native-only EP1 sound records are separate from the unchanged legacy audio ABI and are not production plugins.
public static partial class Marshaler
{
    // Reborn: recovered Win32 root is 16 bytes; only its child stride differs from the old Multisound model.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1Multisound
    {
        public BaseAudioEventInfo Base;
        public MultisoundControlFlags Control;
        public List<Ep1MultisoundSubsound> Subsound;
    }

    // Reborn: reference RA3 IL offsets 0/4/8/12/16/20/24 and stock EP1 imports establish a 28-byte child.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1MultisoundSubsound
    {
        public AssetReference<BaseAudioEventInfo, AudioEventInfo> Base;
        public uint Weight;
        public float* PitchShiftLow;
        public float* PitchShiftHigh;
        public Percentage* Volume;
        public Percentage* PlayPercent;
        public Percentage* VolumeShift;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the isolated EP1 root using core-normalized reference selectors; no audio processor is registered. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1Multisound* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(Ep1Multisound.Control), ""), &objT->Control, state);
        Marshal(node.GetChildNodes(nameof(Ep1Multisound.Subsound)), &objT->Subsound, state);
        Marshal(node, &objT->Base, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve missing pointers versus explicit zero, percentage normalization and reference-first allocation order from reference IL. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1MultisoundSubsound* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node, &objT->Base, state);
        Marshal(node.GetAttributeValue(nameof(Ep1MultisoundSubsound.Weight), "1000"), &objT->Weight, state);
        Marshal(node.GetAttributeValue(nameof(Ep1MultisoundSubsound.PitchShiftLow), null), &objT->PitchShiftLow, state);
        Marshal(node.GetAttributeValue(nameof(Ep1MultisoundSubsound.PitchShiftHigh), null), &objT->PitchShiftHigh, state);
        Marshal(node.GetAttributeValue(nameof(Ep1MultisoundSubsound.Volume), null), &objT->Volume, state);
        Marshal(node.GetAttributeValue(nameof(Ep1MultisoundSubsound.PlayPercent), null), &objT->PlayPercent, state);
        Marshal(node.GetAttributeValue(nameof(Ep1MultisoundSubsound.VolumeShift), null), &objT->VolumeShift, state);
    }
}
