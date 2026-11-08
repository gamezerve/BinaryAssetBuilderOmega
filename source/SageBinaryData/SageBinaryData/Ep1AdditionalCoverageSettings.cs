using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: original EP1 money-sharing flags occupy exactly one native mask word.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1CoverageMoneyCategories
{
    public fixed uint Value[1];
}

// Reborn: original resource settings retain their singleton header and money transaction mask.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CoverageResourceSettings
{
    public BaseAssetType Base;
    public Ep1CoverageMoneyCategories SharedMoneyTransactionCategories;
}

// Reborn: the gift-manager anonymous record embeds the complete filter before its transfer flag.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CoverageObjectGiftSettings
{
    public ObjectFilter AllowObjectFilter;
    public SageBool AllowTransferToSkirmishPlayer;
}

// Reborn: the original gift-manager root owns its anonymous settings by value.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CoverageGiftManagerSettings
{
    public BaseAssetType Base;
    public Ep1CoverageObjectGiftSettings ObjectGiftSettings;
}

// Reborn: EP1 removed the old destroy-particles threshold from the 20-byte height-death update.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1HeightDieUpdateModuleData
{
    public UpdateModuleData Base;
    public float TargetHeight;
    public Duration InitialDelay;
    public SageBool TargetHeightIncludesStructures,OnlyWhenMovingDown,SnapToGroundOnDeath;
}
