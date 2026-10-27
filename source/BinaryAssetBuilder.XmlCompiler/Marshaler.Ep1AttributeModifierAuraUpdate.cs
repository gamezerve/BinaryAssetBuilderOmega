using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize all official Uprising aura fields using retail offsets and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1AttributeModifierAuraUpdateModuleData* value, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue("AttributeModifierName", null), &value->AttributeModifierName, state);
        Marshal(node.GetAttributeValue("RefreshDelay", null), &value->RefreshDelay, state);
        Marshal(node.GetAttributeValue("Range", null), &value->Range, state);
        Marshal(node.GetAttributeValue("RequiredConditions", null), &value->RequiredConditions, state);
        // Reborn: retain the official SHRINK bit, which the legacy category Count omits.
        Ep1ToggleEnum(node.GetAttributeValue("AntiCategory", null), (uint*)&value->AntiCategory, state, typeof(AttributeModifierCategoryType), true);
        Marshal(node.GetAttributeValue("AntiFX", null), &value->AntiFX, state);
        Marshal(node.GetAttributeValue("MaxActiveRank", null), &value->MaxActiveRank, state);
        Marshal(node.GetAttributeValue("RequiredObjectStatusFlags", null), &value->RequiredObjectStatusFlags, state);
        Marshal(node.GetAttributeValue("DisabledTypesToProcess", "HELD"), &value->DisabledTypesToProcess, state);
        Marshal(node.GetChildNode("ObjectFilter", null), &value->ObjectFilter, state);
        Marshal(node.GetAttributeValue("AllowPowerWhenAttacking", "true"), &value->AllowPowerWhenAttacking, state);
        Marshal(node.GetAttributeValue("InitiallyActive", "false"), &value->InitiallyActive, state);
        Marshal(node.GetAttributeValue("RunWhileDead", null), &value->RunWhileDead, state);
        Marshal(node.GetAttributeValue("AllowSelf", null), &value->AllowSelf, state);
        Marshal(node.GetAttributeValue("AffectContainedOnly", null), &value->AffectContainedOnly, state);
        Marshal(node.GetAttributeValue("AffectHordeMembersOnly", null), &value->AffectHordeMembersOnly, state);
        Marshal(node.GetAttributeValue("AffectAttacheeOnly", null), &value->AffectAttacheeOnly, state);
        Marshal(node.GetAttributeValue("ShowDebugCircle", "false"), &value->ShowDebugCircle, state);
        Marshal(node, (UpgradeModuleData*)value, state);
    }
}
