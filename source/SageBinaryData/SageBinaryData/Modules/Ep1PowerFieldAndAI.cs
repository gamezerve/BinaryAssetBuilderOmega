using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 inserts the disabled mask before the optional sound pointer.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpecialPowerUpdateModuleData
{
    public const uint Wire = 0xB9B2A9B3u;
    public UpdateModuleData Base;
    public AssetReference<SpecialPowerTemplate> SpecialPowerTemplate;
    public DisabledBitFlags DisabledTypesToIgnore;
    public Ep1SoundOrEvaEvent* InitiateSound;
    public SageBool StartsPaused;
}

// Reborn: EP1 uses the 32-byte firing base before the damage field's own data.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1DamageFieldUpdateModuleData
{
    public const uint Wire = 0xF3B802D5u;
    public Ep1FireWeaponUpdateModuleData Base;
    public int Radius;
    public AssetReference<UpgradeTemplate> RequiredUpgrade;
    public ObjectFilter* ObjectFilter;
}

// Reborn: Horde retains its own wire identity over the fieldless EP1 AI extension.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1HordeAIUpdateModuleData
{
    public const uint Wire = 0x55D77703u;
    public Ep1AIUpdateModuleData Base;
}

// Reborn: the supply truck tails begin at +0x4c after the EP1 AI header.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1SupplyTruckAIUpdateModuleData
{
    public const uint Wire = 0x9AB8094Du;
    public Ep1AIUpdateModuleData Base;
    public uint MaxBoxes;
    public Time SupplyCenterActionDelay, SupplyWarehouseActionDelay;
    public float SupplyWarehouseScanDistance, DepletedAnnounceDistance, HarvestActivationRange;
    public Time HarvestPreparationTime, HarvestActionTime;
    public SageBool HarvestTrees, HarvestTiberium, HarvestsFromRearEnd;
    public SageBool CanPathThroughUnitsWhileDocking, CanPathThroughUnitsWhileHarvesting;
}
