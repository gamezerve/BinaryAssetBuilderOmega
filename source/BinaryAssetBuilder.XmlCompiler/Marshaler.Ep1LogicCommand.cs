using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 LogicCommand in the official allocation order (SpecialPower, Upgrade, Object). the optional AI graph uses its separate EP1 record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1LogicCommand* value, Tracker state)
    {
        if (node is null) return;
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
        // Reborn: append the optional AI record after the three command-reference pointees, preserving official allocation order.
        var ai=node.GetChildNode("AISpecialPowerInfo",null);
        if(ai!=null){using Tracker.Context allocation=state.Push((void**)&value->AISpecialPowerInfo,(uint)sizeof(Ep1AiSpecialPowerInfo),1);Marshal(ai,value->AISpecialPowerInfo,state);}
        Marshal(node, (BaseAssetType*)value, state);
    }
}
