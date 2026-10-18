using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 has no KW SELL ordinal and adds AISpecialPowerInfo at +24; keep legacy LogicCommand unchanged.
public enum Ep1LogicCommandType
{
    NONE, SPECIAL_POWER, STOP, OBJECT_UPGRADE, PLAYER_UPGRADE, DOZER_CONSTRUCT,
    UNIT_BUILD, EVACUATE, EXIT_CONTAINER, SET_STANCE, ATTACK_MOVE, CANCEL_UNIT_BUILD,
    CANCEL_UPGRADE, CONSTRUCTION_YARD_CONSTRUCT, HORDE_TOGGLE_FORMATION, GARRISON_BUILDING, COLLECT
}

// Reborn: the bounded UNIT_BUILD route uses the complete 28-byte EP1 root, with one separately allocated Object reference.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LogicCommand
{
    public BaseInheritableAsset Base;
    public Ep1LogicCommandType Type;
    public uint Options;
    public AssetReference<SpecialPowerTemplate>* SpecialPower;
    public AssetReference<UpgradeTemplate>* Upgrade;
    public AssetReference<GameObject>* Object;
    public void* AISpecialPowerInfo;
}
