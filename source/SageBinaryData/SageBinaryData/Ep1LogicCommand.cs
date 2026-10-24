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

// Reborn: EP1 LogicCommandOptions (11 values, EP1 AssetTypeLogicCommand.xsd order; adds NEED_TARGET_ORIENTATION before OK_FOR_MULTI_EXECUTE).
public enum Ep1LogicCommandOptions
{
    NEED_TARGET_ENEMY_OBJECT, NEED_TARGET_NEUTRAL_OBJECT, NEED_TARGET_ALLY_OBJECT, FIRED_BY_SCRIPT, OPTION_ONE, OPTION_TWO,
    OPTION_THREE, AUTO_ABILITY_TRIGGERED, NEED_TARGET_POS, NEED_TARGET_ORIENTATION, OK_FOR_MULTI_EXECUTE
}

// Reborn: official BitFlags<11,LogicCommandOptions> is one 32-bit span at +8.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1LogicCommandOptionsBitFlags
{
    public const int Count = 11;
    public unsafe fixed uint Value[1];
}

// Reborn: the complete 28-byte EP1 root (official RA3 SDK marshal order: Type@4, Options@8, SpecialPower*@12, Upgrade*@16, Object*@20, AISpecialPowerInfo*@24).
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LogicCommand
{
    public BaseInheritableAsset Base;
    public Ep1LogicCommandType Type;
    public Ep1LogicCommandOptionsBitFlags Options;
    public AssetReference<SpecialPowerTemplate>* SpecialPower;
    public AssetReference<UpgradeTemplate>* Upgrade;
    public AssetReference<GameObject>* Object;
    public void* AISpecialPowerInfo;
}
