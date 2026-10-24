using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 recall stores the aircraft limit before its trailing return-voice boolean; the legacy KW record lacks this field.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1RecallUnitsSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    public int MaxAircraftToRecall;
    public SageBool PlayReturnVoiceFromRecalledUnit;
}

// Reborn: EP1 parking removes KW HealAmount and adds queue/production limits, the helipad name and the production height.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ParkingPlaceBehaviorModuleData
{
    public UpdateModuleData Base;
    public int NumRows;
    public int NumCols;
    public float ApproachHeight;
    public Time DoorOpenTime;
    public Time DoorCloseTime;
    public float QueueDistance;
    public int LimitedProductionAircraftSpots;
    public AnsiString HelipadBoneName;
    public float ProducedAboveHeight;
    public ObjectFilter* CanParkHereFilter;
    public Coord3D* DefaultRallyPointOffset;
    public SageBool ParkInHangars;
    public SageBool LowersOntoParkingPlaceOnProduction;
    public SageBool CanSetRallyPoint;
    public SageBool DoorsUnpackOpen;
}
