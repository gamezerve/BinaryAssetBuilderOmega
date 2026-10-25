using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: category ordinals follow the Uprising MoneyTransactionCategory XSD, independently observed as AUTO_DEPOSIT=4 in retail OilDerrick.
public enum Ep1MoneyTransactionCategory
{
    UNKNOWN, STARTING_CASH, HARVESTER, CAPTURE_BONUS, AUTO_DEPOSIT, SELL_OBJECT, BOUNTY,
    CASH_CHEAT, DEVASTATE, CANCEL_BUILD, MONEY_CRATE, DEATH_REFUND, SLAUGHTER,
    TERRAIN_RESOURCE, SHARED_DEPOSIT, PRODUCTION, BUILD_WALLSPAN, DOZER_CONSTRUCT,
    SELF_REPAIR, PLAYERKILLED, SCRIPTED_MONEY_ACTION, SPECIAL_POWER, AI_MONEY_LENDER,
    PLAYER_GIVE_MONEY, PLAYER_CLONE, TRANSFER_ASSETS, CASH_HACK, INFILTRATOR, PILLAGE, STEAL_MONEY
}

// Reborn: retail EP1 stores three source-sized transaction lists, not KW's scalar deposit/upgrade fields; its header is 48 bytes on x86.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AutoDepositUpdateModuleData
{
    public UpdateModuleData Base;
    public Time DepositInterval;
    public AutoDepositBitFlags Flags;
    public Ep1MoneyTransactionCategory TransactionCategory;
    public List<MoneyTransaction> InitialCaptureBonus;
    public List<MoneyTransaction> Deposit;
    public List<MoneyTransaction> ResourceCap;
    public SageBool GiveNoXP;
    public SageBool OnlyWhenGarrisoned;
}
