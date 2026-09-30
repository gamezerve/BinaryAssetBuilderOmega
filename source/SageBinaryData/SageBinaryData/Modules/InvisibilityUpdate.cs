using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

public enum InvisibilityUpdateOptions
{
    STARTS_ACTIVE,
    BROADCAST,
    BROADCAST_INVERSE
}

[StructLayout(LayoutKind.Sequential)]
public struct InvisibilityUpdateOptionsBitFlags
{
    public const int Count = 3;
    public const int BitsInSpan = 32;
    public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

    public unsafe fixed uint Value[NumSpans];
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct InvisibilityUpdateModuleData
{
    public UpdateModuleData Base;
    // Reborn: RA3/Uprising reference a shared invisibility template instead of embedding the KW nugget.
    public AssetReference<BaseAssetType> InvisibilityTemplate;
    public Time UpdatePeriod;
    public float RequiredNearbyObjectRange;
    public StringHash* NamedVoiceNameToUseAsVoiceMoveToStealthyArea;
    public StringHash* NamedVoiceNameToUseAsVoiceEnterStateMoveToStealthyArea;
    public ObjectFilter RequiresNearbyObjectFilter;
}
