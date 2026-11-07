using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, DynamicsCollideModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node, (BehaviorModuleData*)objT, state);
    }

    public static unsafe void Marshal(Node node, MagnitudeSoundSelectorEntry* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(MagnitudeSoundSelectorEntry.MinimumMagnitude), null), &objT->MinimumMagnitude, state);
        Marshal(node.GetAttributeValue(nameof(MagnitudeSoundSelectorEntry.Sound), null), &objT->Sound, state);
    }

    public static unsafe void Marshal(Node node, MagnitudeSoundSelectorTable* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNodes(nameof(MagnitudeSoundSelectorTable.Entry)), &objT->Entry, state);
    }

    public static unsafe void Marshal(Node node, AudioDynamicsCollideModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AudioDynamicsCollideModuleData.MinimumImpactVelocity), null), &objT->MinimumImpactVelocity, state);
        Marshal(node.GetChildNode(nameof(AudioDynamicsCollideModuleData.MagnitudeSoundSelector), null), &objT->MagnitudeSoundSelector, state);
        Marshal(node, (DynamicsCollideModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the authored collision weight or the EP1 default of 0.1. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, EnableDynamicsCollideModuleData* target, Tracker state)
    {
        if (node is null)
            return;

        Marshal(node.GetAttributeValue(nameof(EnableDynamicsCollideModuleData.Weight), "0.1"), &target->Weight, state);
        Marshal(node, &target->Base, state);
    }

    public static unsafe void Marshal(Node node, DamageDynamicsCollideModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DamageDynamicsCollideModuleData.MaxMagnitude), "10.0"), &objT->MaxMagnitude, state);
        Marshal(node.GetChildNodes(nameof(DamageDynamicsCollideModuleData.DamageNugget)), &objT->DamageNugget, state);
        Marshal(node, (DynamicsCollideModuleData*)objT, state);
    }
}
