using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 LogicCommand in the official allocation order (SpecialPower, Upgrade, Object). AISpecialPowerInfo has a different EP1
        record size than RA3 and is not reviewed yet, so it is refused rather than written with a guessed layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1LogicCommand* value, Tracker state)
    {
        if (node is null) return;
        if (node.GetChildNode("AISpecialPowerInfo", null) != null)
            throw new System.NotSupportedException("EP1 LogicCommand AISpecialPowerInfo is not supported yet (its EP1 record layout is not reviewed).");
        string type = node.GetAttributeValue("Type", "NONE")?.GetText() ?? "NONE";
        if (!System.Enum.TryParse(type, false, out Ep1LogicCommandType parsed) || !System.Enum.IsDefined(parsed) || type != parsed.ToString())
            throw new System.NotSupportedException("Unknown EP1 LogicCommand Type: " + type);
        value->Type = parsed;
        uint options = 0;
        foreach (string token in (node.GetAttributeValue("Options", null)?.GetText() ?? "").Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries))
        {
            if (!System.Enum.TryParse(token, false, out Ep1LogicCommandOptions option) || !System.Enum.IsDefined(option) || token != option.ToString())
                throw new System.NotSupportedException("Unknown EP1 LogicCommand option: " + token);
            options |= 1u << (int)option;
        }
        value->Options.Value[0] = options;
        Marshal(node.GetChildNode("SpecialPower", null), &value->SpecialPower, state);
        Marshal(node.GetChildNode("Upgrade", null), &value->Upgrade, state);
        Marshal(node.GetChildNode("Object", null), &value->Object, state);
        Marshal(node, (BaseAssetType*)value, state);
    }
}
