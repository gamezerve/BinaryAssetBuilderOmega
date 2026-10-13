using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal one inline AI state with separate module/string lists, strict EP1 enums and unchanged typed-reference encodings. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ToggleStatusNugget* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.AttributeModifier), null), &objT->AttributeModifier, state);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.ModelConditions), null), &objT->ModelConditions, state);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.ObjectStatus), null), &objT->ObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.Upgrade), null), &objT->Upgrade, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(ToggleStatusNugget.ArmorSet), "INVALID"), &objT->ArmorSet, state, typeof(ArmorSetType), false);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(ToggleStatusNugget.LocomotorSet), "INVALID"), &objT->LocomotorSet, state, typeof(Ep1ToggleLocomotorSetType), false, -1);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.AIUpdateModuleId), null), &objT->AIUpdateModuleId, state);
        // Reborn: stock retains a non-null terminator-only string for the empty default; it still requires relocation.
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.GenericLuaEventParameter), ""), &objT->GenericLuaEventParameter, state);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.EnterStateSound), null), &objT->EnterStateSound, state);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusNugget.InitiateIntentToExitStateSound), null), &objT->InitiateIntentToExitStateSound, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(ToggleStatusNugget.DisabledType), "DEFAULT"), &objT->DisabledType, state, typeof(DisabledType), false);
        var ai = node.GetChildNode(nameof(ToggleStatusNugget.SkirmishAiInfo), null);
        if (ai == null) throw new BinaryAssetBuilderException(ErrorCode.InternalError, "EP1 toggle state requires SkirmishAiInfo.");
        Marshal(ai, &objT->SkirmishAiInfo, state);
        var modules = node.GetChildNodes(nameof(ToggleStatusNugget.Module));
        if (modules != null && modules.GetCount() > 4) throw new BinaryAssetBuilderException(ErrorCode.InternalError, "EP1 toggle state permits at most four Module children.");
        Marshal(modules, &objT->Module, state);
        Marshal(node.GetChildNodes(nameof(ToggleStatusNugget.ActiveGeometry)), &objT->ActiveGeometry, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile ordered nonempty toggle states without enabling polymorphic dispatch until complete selected graphs are validated. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ToggleStatusSpecialAbilityUpdateModuleData* objT, Tracker state)
    {
        if (node is null) return;
        var states = node.GetChildNodes(nameof(ToggleStatusSpecialAbilityUpdateModuleData.ToggleState));
        if (states == null || states.GetCount() == 0) throw new BinaryAssetBuilderException(ErrorCode.InternalError, "EP1 toggle update requires at least one ToggleState.");
        Marshal(states, &objT->ToggleState, state);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusSpecialAbilityUpdateModuleData.ReloadAllWeaponsOnToggle), "true"), &objT->ReloadAllWeaponsOnToggle, state);
        Marshal(node.GetAttributeValue(nameof(ToggleStatusSpecialAbilityUpdateModuleData.ForceIdleAfterToggle), "false"), &objT->ForceIdleAfterToggle, state);
        Marshal(node, (SpecialAbilityUpdateModuleData*)objT, state);
    }
}
