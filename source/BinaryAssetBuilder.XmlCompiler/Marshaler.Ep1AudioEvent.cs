using System;
using System.Runtime.InteropServices;
using Relo;
using SageBinaryData;

// Reborn: native AudioEvent evidence is isolated from legacy KW records and never registers a production audio processor.
public static partial class Marshaler
{
    // Reborn: reference IL 0x0600003D establishes the 128-byte Win32 base, including reserved unimplemented LimitGroup slots.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1BaseSingleSound
    {
        public BaseAudioEventInfo Base;
        public Percentage Volume;
        public Percentage VolumeShift;
        public Percentage PerFileVolumeShift;
        public Percentage MinVolume;
        public float ShrunkenPitchModifier;
        public Percentage ShrunkenVolumeModifier;
        public Percentage PlayPercent;
        public int Limit;
        public AudioPriority Priority;
        public AudioTypeFlags Type;
        public uint Control;
        public float MinRange;
        public float MaxRange;
        public Percentage LowPassCutoff;
        public Percentage ZoomedInOffscreenVolumePercent;
        public Percentage ZoomedInOffscreenMinVolumePercent;
        public Percentage ZoomedInOffscreenOcclusionPercent;
        public Percentage ReverbEffectLevel;
        public Percentage DryLevel;
        public AudioVolumeSlider* SubmixSlider;
        public RealRange* PitchShift;
        public RealRange* PerFilePitchShift;
        public IntRange* Delay;
        public IntRange* InitialDelay;
        public List<AudioVolumeSliderMultiplier> VolumeSliderMultiplier;
        public RealRange* MinRangeShift;
        public RealRange* MaxRangeShift;
        public List<uint> LimitGroup;
        public Ep1TimeRange* NonInterruptibleTime;
    }

    // Reborn: native root adds three 8-byte lists at offsets 128, 136 and 144.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1AudioEvent
    {
        public Ep1BaseSingleSound Base;
        public List<Ep1AudioFileRefWithWeight> Attack;
        public List<Ep1AudioFileRefWithWeight> Sound;
        public List<Ep1AudioFileRefWithWeight> Decay;
    }

    // Reborn: reference IL 0x06000037 adds inline normalized Volume to the legacy 8-byte reference/weight pair.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1AudioFileRefWithWeight
    {
        public AssetReference<AudioFile> Base;
        public uint Weight;
        public Percentage Volume;
    }

    // Reborn: reference IL 0x0600038B allocates two native second-valued floats, not millisecond integers.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1TimeRange { public Time Low; public Time High; }

    // Reborn: official EP1 inserts SMART_LIMITING before FADE_ON_KILL; preserve the unchanged legacy seven-value enum.
    public enum Ep1AudioControlFlag { LOOP, SEQUENTIAL, RANDOMSTART, INTERRUPT, SMART_LIMITING, FADE_ON_KILL, FADE_ON_START, ALLOW_KILL_MID_FILE, IMMEDIATE_DECAY_ON_KILL }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unsupported group references before allocating native tables, then retain reference Attack/Sound/Decay-before-base order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1AudioEvent* objT, Tracker state)
    {
        if (node is null) return;
        if (node.GetChildNode("LimitGroup", null) != null) throw new NotSupportedException("Native AudioEvent LimitGroup references are not recovered yet.");
        Marshal(node.GetChildNodes("Attack"), &objT->Attack, state);
        Marshal(node.GetChildNodes("Sound"), &objT->Sound, state);
        Marshal(node.GetChildNodes("Decay"), &objT->Decay, state);
        Marshal(node, &objT->Base, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reproduce recovered reference fields and allocation order without changing legacy sound marshalling. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node, Ep1BaseSingleSound* objT, Tracker state)
    {
        Marshal(node.GetAttributeValue("Volume", "100"), &objT->Volume, state);
        Marshal(node.GetAttributeValue("VolumeShift", "0"), &objT->VolumeShift, state);
        Marshal(node.GetAttributeValue("PerFileVolumeShift", "0"), &objT->PerFileVolumeShift, state);
        Marshal(node.GetAttributeValue("MinVolume", "0"), &objT->MinVolume, state);
        Marshal(node.GetAttributeValue("ShrunkenPitchModifier", "1.0"), &objT->ShrunkenPitchModifier, state);
        Marshal(node.GetAttributeValue("ShrunkenVolumeModifier", "100"), &objT->ShrunkenVolumeModifier, state);
        Marshal(node.GetAttributeValue("PlayPercent", "100"), &objT->PlayPercent, state);
        Marshal(node.GetAttributeValue("Limit", "0"), &objT->Limit, state);
        Marshal(node.GetAttributeValue("Priority", "NORMAL"), &objT->Priority, state);
        Marshal(node.GetAttributeValue("Type", ""), &objT->Type, state);
        string control = node.GetAttributeValue("Control", "").GetText();
        uint flags = 0;
        foreach (string word in control.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!Enum.TryParse(word, false, out Ep1AudioControlFlag flag) || !Enum.IsDefined(typeof(Ep1AudioControlFlag), flag))
                throw new NotSupportedException("Unknown EP1 AudioEvent control.");
            flags |= 1u << (int)flag;
        }
        state.InplaceEndianToPlatform(&flags); objT->Control = flags;
        Marshal(node.GetAttributeValue("MinRange", "160"), &objT->MinRange, state);
        Marshal(node.GetAttributeValue("MaxRange", "640"), &objT->MaxRange, state);
        Marshal(node.GetAttributeValue("LowPassCutoff", "0"), &objT->LowPassCutoff, state);
        Marshal(node.GetAttributeValue("ZoomedInOffscreenVolumePercent", "50"), &objT->ZoomedInOffscreenVolumePercent, state);
        Marshal(node.GetAttributeValue("ZoomedInOffscreenMinVolumePercent", "100"), &objT->ZoomedInOffscreenMinVolumePercent, state);
        Marshal(node.GetAttributeValue("ZoomedInOffscreenOcclusionPercent", "20"), &objT->ZoomedInOffscreenOcclusionPercent, state);
        Marshal(node.GetAttributeValue("ReverbEffectLevel", "0"), &objT->ReverbEffectLevel, state);
        Marshal(node.GetAttributeValue("DryLevel", "100"), &objT->DryLevel, state);
        Marshal(node.GetAttributeValue("SubmixSlider", null), &objT->SubmixSlider, state);
        Marshal(node.GetChildNode("PitchShift", null), &objT->PitchShift, state);
        Marshal(node.GetChildNode("PerFilePitchShift", null), &objT->PerFilePitchShift, state);
        Marshal(node.GetChildNode("Delay", null), &objT->Delay, state);
        Marshal(node.GetChildNode("InitialDelay", null), &objT->InitialDelay, state);
        Marshal(node.GetChildNodes("VolumeSliderMultiplier"), &objT->VolumeSliderMultiplier, state);
        Marshal(node.GetChildNode("MinRangeShift", null), &objT->MinRangeShift, state);
        Marshal(node.GetChildNode("MaxRangeShift", null), &objT->MaxRangeShift, state);
        Node time = node.GetChildNode("NonInterruptibleTime", null);
        if (time != null)
        {
            using Tracker.Context context = state.Push((void**)&objT->NonInterruptibleTime, (uint)sizeof(Ep1TimeRange), 1u);
            Marshal(time.GetAttributeValue("Low", null), &objT->NonInterruptibleTime->Low, state);
            Marshal(time.GetAttributeValue("High", null), &objT->NonInterruptibleTime->High, state);
        }
        Marshal(node, &objT->Base, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate imports first, then marshal default weight and percentage volume exactly as reference IL. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1AudioFileRefWithWeight* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node, &objT->Base, state);
        Marshal(node.GetAttributeValue("Weight", "1000"), &objT->Weight, state);
        Marshal(node.GetAttributeValue("Volume", "100"), &objT->Volume, state);
    }
}
