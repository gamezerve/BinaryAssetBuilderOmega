using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: receiver distance modes retain the original EP1 schema/native ordinal order.
public enum ReceiverDistanceType { CIRCULAR, RECTANGULAR }

// Reborn: the 188-byte EP1 receiver uses optional RadiusY storage and an inline 120-byte source filter.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TechUpgradeReceiverModuleData
{
    public UpgradeModuleData Base;
    public float Radius;
    public float* RadiusY;
    public ReceiverDistanceType DistanceType;
    public String<sbyte> FXLineBoneName;
    public TypedAssetId<ModuleData> RadarDrawModule;
    public Angle RadarRotateSpeed,RadarRotateOffset;
    public ObjectFilter ObjectFilter;
}
