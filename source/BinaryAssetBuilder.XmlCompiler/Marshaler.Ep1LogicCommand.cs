using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly compile only UNIT_BUILD/Object; reject all unreviewed EP1 option/special-power payloads, without invoking KW dispatch. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1LogicCommand* value, Tracker state)
    {
        if (node is null || node.GetAttributeValue("Type", "NONE").GetText() != "UNIT_BUILD"
            || !string.IsNullOrEmpty(node.GetAttributeValue("Options", null)?.GetText())
            || node.GetChildNode("SpecialPower", null) != null || node.GetChildNode("Upgrade", null) != null
            || node.GetChildNode("AISpecialPowerInfo", null) != null || node.GetChildNode("Object", null) == null)
            throw new System.NotSupportedException("EP1 LogicCommand supports only UNIT_BUILD with Object and no Options/special payloads.");
        value->Type = Ep1LogicCommandType.UNIT_BUILD;
        Marshal(node.GetChildNode("Object", null), &value->Object, state);
        Marshal(node, (BaseAssetType*)value, state);
    }
}
