using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: preserve the single RA3/EP1 money-gain action value.
public enum MoneyGainAttachActionFlag
{
    ON_DETACH_PARENT_DEAD
}

// Reborn: official RA3 IL stores action, normalized price and an optional validation pointer after AttachUpdate.
[StructLayout(LayoutKind.Sequential)]
public struct MoneyGainAttachUpdateModuleData
{
    public AttachUpdateModuleData Base;
    public MoneyGainAttachActionFlag ActionType;
    public Percentage PurchasePricePercent;
    public unsafe ObjectStatusValidationDataType* MoneyGainObjectStatusValidation;
}
