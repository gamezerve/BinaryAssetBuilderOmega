using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: retail EP1 slave watchers are 24 bytes: two upgrade imports, optional status validation, then four booleans.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SlaveWatcherBehaviorModuleData
{
    public UpdateModuleData Base;
    public AssetReference<UpgradeTemplate> GrantUpgrade;
    public AssetReference<UpgradeTemplate> RemoveUpgrade;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public SageBool ShareUpgrades;
    public SageBool LetSlaveLive;
    public SageBool WatchingSpawnedSlave;
    public SageBool ForceSameTeam;
}
