using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(string text, ReplaceSelfOptionsBitFlags* objT, Tracker state)
    {
        string[] tokens = text.Split(WhiteSpaces, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string token in tokens)
        {
            bool includeToken = token[0] != '-';
            if (string.Equals(token, "ALL", System.StringComparison.Ordinal))
            {
                for (int index = 0; index < ReplaceSelfOptionsBitFlags.NumSpans; index++)
                {
                    objT->Value[index] = uint.MaxValue;
                }
                continue;
            }

            ReplaceSelfOptions value = (ReplaceSelfOptions)(-1);
            Marshal(token, &value, state);
            if (value != (ReplaceSelfOptions)(-1))
            {
                uint uintValue = (uint)value;
                if (includeToken)
                {
                    objT->Value[uintValue / ReplaceSelfOptionsBitFlags.BitsInSpan] |=
                        1u << (int)(uintValue % ReplaceSelfOptionsBitFlags.BitsInSpan);
                }
                else
                {
                    objT->Value[uintValue / ReplaceSelfOptionsBitFlags.BitsInSpan] ^=
                        1u << (int)(uintValue % ReplaceSelfOptionsBitFlags.BitsInSpan);
                }
            }
        }

        for (int index = 0; index < ReplaceSelfOptionsBitFlags.NumSpans; index++)
        {
            state.InplaceEndianToPlatform(&objT->Value[index]);
        }
    }

    public static unsafe void Marshal(Value value, ReplaceSelfOptionsBitFlags* objT, Tracker state)
    {
        if (value is not null)
        {
            Marshal(value.GetText(), objT, state);
        }
    }

    public static unsafe void Marshal(Node node, ReplaceSelfSpecialAbilityModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ReplaceSelfSpecialAbilityModuleData.NewObjectUnpackTime), "0s"), &objT->NewObjectUnpackTime, state);
        Marshal(node.GetAttributeValue(nameof(ReplaceSelfSpecialAbilityModuleData.ReplaceOptions), nameof(ReplaceSelfOptions.DISABLE_DURING_REPLACE)), &objT->ReplaceOptions, state);
        Marshal(node.GetAttributeValue(nameof(ReplaceSelfSpecialAbilityModuleData.ClearTriggerDistance), "120.0"), &objT->ClearTriggerDistance, state);
        Marshal(node.GetChildNodes(nameof(ReplaceSelfSpecialAbilityModuleData.ReplacementTemplate)), &objT->ReplacementTemplate, state);
        Marshal(node, (SpecialAbilityUpdateModuleData*)objT, state);
    }

    public static unsafe void Marshal(Node node, ProjectileReplaceSelfSpecialAbilityModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ProjectileReplaceSelfSpecialAbilityModuleData.LaunchingWeapon), null), &objT->LaunchingWeapon, state);
        Marshal(node.GetAttributeValue(nameof(ProjectileReplaceSelfSpecialAbilityModuleData.OtherObjectCreationList), null), &objT->OtherObjectCreationList, state);
        Marshal(node, (ReplaceSelfSpecialAbilityModuleData*)objT, state);
    }
}
