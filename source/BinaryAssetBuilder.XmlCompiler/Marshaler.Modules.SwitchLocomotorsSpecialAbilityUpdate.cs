using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, SwitchLocomotorsSpecialAbilityUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SwitchLocomotorsSpecialAbilityUpdateModuleData.UseUpgradedLocomotor), null), &objT->UseUpgradedLocomotor, state);
        Marshal(node.GetAttributeValue(nameof(SwitchLocomotorsSpecialAbilityUpdateModuleData.TriggersLanding), "false"), &objT->TriggersLanding, state);
        Marshal(node.GetAttributeValue(nameof(SwitchLocomotorsSpecialAbilityUpdateModuleData.TriggersFlight), "false"), &objT->TriggersFlight, state);
        Marshal(node.GetAttributeValue(nameof(SwitchLocomotorsSpecialAbilityUpdateModuleData.BusyForDuration), "0s"), &objT->BusyForDuration, state);
        Marshal(node.GetAttributeValue(nameof(SwitchLocomotorsSpecialAbilityUpdateModuleData.LandingRange), "0"), &objT->LandingRange, state);
        Marshal(node.GetAttributeValue(nameof(SwitchLocomotorsSpecialAbilityUpdateModuleData.HordeMembersSpecialPowerTemplate), null), &objT->HordeMembersSpecialPowerTemplate, state);
        Marshal(node, (SpecialAbilityUpdateModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the EP1 locomotor selector, power import, timing, range and byte flags. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node, Ep1SwitchLocomotorsSpecialAbilityUpdateModuleData* objT, Tracker state)
    {
        Marshal(node.GetAttributeValue("BusyForDuration", "0s"), &objT->BusyForDuration, state);
        Marshal(node.GetAttributeValue("LandingRange", "0"), &objT->LandingRange, state);
        Marshal(node.GetAttributeValue("HordeMembersSpecialPowerTemplate", null), &objT->HordeMembersSpecialPowerTemplate, state);
        // Reborn: omitted selectors keep the zeroed word and authored selectors follow EP1's INVALID=-1 numbering.
        var locomotor = node.GetAttributeValue("LocomotorToSet", null);
        if (locomotor is not null)
        {
            string name = locomotor.GetText();
            if (!System.Enum.IsDefined(typeof(Ep1RootLocomotorSetType), name))
                throw new System.IO.InvalidDataException("Unknown EP1 LocomotorToSet: " + name);
            uint selector = unchecked((uint)(Ep1RootLocomotorSetType)System.Enum.Parse(typeof(Ep1RootLocomotorSetType), name));
            state.InplaceEndianToPlatform(&selector);
            *(uint*)&objT->LocomotorToSet = selector;
        }
        Marshal(node.GetAttributeValue("UseUpgradedLocomotor", null), &objT->UseUpgradedLocomotor, state);
        Marshal(node.GetAttributeValue("TriggersLanding", "false"), &objT->TriggersLanding, state);
        Marshal(node.GetAttributeValue("TriggersFlight", "false"), &objT->TriggersFlight, state);
        Marshal(node, &objT->Base, state);
    }
}
