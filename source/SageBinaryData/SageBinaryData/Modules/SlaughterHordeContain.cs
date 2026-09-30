using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct SlaughterHordeContainModuleData
{
    public HordeGarrisonContainModuleData Base;
    // Reborn: RA3 stores a normalized percentage and an FX reference before the inline filter.
    public Percentage CashBackPercent;
    public ObjectStatusBitFlags CanAlwaysEnterStatus;
    public Relo.AssetReference<FXList> SlaughterFX;
    public ObjectFilter CanAlwaysEnterObjectFilter;
}
