using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: retail EP1 ejection modules extend the 252-byte ability base with scalar fields, bone data and two byte flags (292-byte header).
[StructLayout(LayoutKind.Sequential)]
public struct EjectPassengersSpecialAbilityUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public Time TimePerPassenger;
    public float Speed;
    public float Radius;
    public float Height;
    public float ApexHeight;
    public AssetReference<FXList> EjectionFX;
    public AnsiString EjectionBoneName;
    public unsafe Coord3D* EjectionBoneOffset;
    public SageBool RunOffMapWhenDone;
    public SageBool IgnoreTargetLocation;
}
