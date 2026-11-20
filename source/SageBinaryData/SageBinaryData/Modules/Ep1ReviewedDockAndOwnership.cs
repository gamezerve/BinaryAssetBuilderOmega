using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: the EP1 dock stores its deposit flag immediately after the multiplier.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1SupplyCenterDockUpdateModuleData
{
    public const uint WireTypeId = 0xC2F1EC20u;
    public DockUpdateModuleData Base;
    public float ValueMultiplier;
    public SageBool DistributedDeposit;
}

// Reborn: EP1 temporary ownership appends the status flags cleared by the retail cleanup path.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1TemporaryOwnerSpecialPowerModuleData
{
    public const uint WireTypeId = 0xC662DEB4u;
    public SpecialPowerModuleData Base;
    public ObjectStatusBitFlags SlaveStatus;
}

// Reborn: the EP1 gate proxy has its own class ID and inherits the complete serialized gate record.
[StructLayout(LayoutKind.Sequential)]
public struct GateProxyBehaviorModuleData
{
    public const uint WireTypeId = 0x0D6BB67Bu;
    public GateOpenAndCloseBehaviorModuleData Base;
}

// Reborn: these two optional vector allocations are owned independently by the EP1 exit module.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SupplyCenterProductionExitUpdateModuleData
{
    public const uint WireTypeId = 0x05F72D93u;
    public UpdateModuleData Base;
    public Coord3D* UnitCreatePoint;
    public Coord3D* NaturalRallyPoint;
}

// Reborn: the EP1 repair dock adds one float after the inherited dock data.
[StructLayout(LayoutKind.Sequential)]
public struct RepairDockUpdateModuleData
{
    public const uint WireTypeId = 0x14B9EF65u;
    public DockUpdateModuleData Base;
    public float FramesForFullHeal;
}

// Reborn: the EP1 rail dock stores independent pull and push duration words.
[StructLayout(LayoutKind.Sequential)]
public struct RailedTransportDockUpdateModuleData
{
    public const uint WireTypeId = 0x729319CBu;
    public DockUpdateModuleData Base;
    public uint PullInsideDurationInFrames;
    public uint PushOutsideDurationInFrames;
}
