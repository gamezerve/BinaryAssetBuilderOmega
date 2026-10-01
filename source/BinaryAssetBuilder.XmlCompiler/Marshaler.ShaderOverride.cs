using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain official optional POID allocation and rule field order; material names hash to weak IDs rather than strong imports. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ShaderOverrideRule* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(ShaderOverrideRule.IfOriginalShaderIs), null), &objT->IfOriginalShaderIs, state);
        Marshal(node.GetAttributeValue(nameof(ShaderOverrideRule.ReplaceShaderName), null), &objT->ReplaceShaderName, state);
        Marshal(node.GetAttributeValue(nameof(ShaderOverrideRule.ReplaceTechniqueName), null), &objT->ReplaceTechniqueName, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the observed priority and ordered 16-byte rule list without registering an unvalidated production processor. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ShaderOverride* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(ShaderOverride.Priority), "1"), &objT->Priority, state);
        Marshal(node.GetChildNodes(nameof(ShaderOverride.Rule)), &objT->Rule, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }
}
