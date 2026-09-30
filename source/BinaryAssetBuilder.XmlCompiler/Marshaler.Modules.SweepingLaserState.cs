using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse EP1 sweep options as native bit positions, including explicit additions/removals. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value, SweepingLaserOptionsBitFlags* objT, Tracker state)
    {
        if (value is null)
        {
            return;
        }
        uint bits = 0;
        foreach (string token in value.GetText().Split(WhiteSpaces, System.StringSplitOptions.RemoveEmptyEntries))
        {
            bool remove = token[0] == '-';
            string name = token[0] == '+' || remove ? token.Substring(1) : token;
            uint mask = name == "ALL" ? (1u << SweepingLaserOptionsBitFlags.Count) - 1u
                : 1u << (int)System.Enum.Parse<SweepingLaserOptionsType>(name);
            bits = remove ? bits & ~mask : bits | mask;
        }
        objT->Value[0] = bits;
        state.InplaceEndianToPlatform(&objT->Value[0]);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the recovered sweeping tail with EP1 angle and schema-default option bits. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, SweepingLaserStateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SweepingLaserStateModuleData.Radius), "10.0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(SweepingLaserStateModuleData.Angle), "0d"), &objT->Angle, state);
        Marshal(node.GetAttributeValue(nameof(SweepingLaserStateModuleData.SweepFXList), null), &objT->SweepFXList, state);
        Marshal(node.GetAttributeValue(nameof(SweepingLaserStateModuleData.VeteranSweepFXList), null), &objT->VeteranSweepFXList, state);
        Marshal(node.GetAttributeValue(nameof(SweepingLaserStateModuleData.SweepFXTimeout), "0s"), &objT->SweepFXTimeout, state);
        Marshal(node.GetAttributeValue(nameof(SweepingLaserStateModuleData.SweepWeapon), null), &objT->SweepWeapon, state);
        Marshal(node.GetAttributeValue(nameof(SweepingLaserStateModuleData.SweepingLaserOptions), "SWEEP_HORIZONTAL MOVE_BACK_AND_FORTH"), &objT->SweepingLaserOptions, state);
        Marshal(node, (LaserStateModuleData*)objT, state);
    }
}
