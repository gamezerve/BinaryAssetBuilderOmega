using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep the obsolete beam writer out of the live Marshal overload set. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalLegacyBeamSpecialAbilityUpdate(Node node, LegacyBeamSpecialAbilityUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.DamagePerSecond), null), &objT->DamagePerSecond, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.JoinWithOtherBeams), "false"), &objT->JoinWithOtherBeams, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.DissapateWhenTargetDead), "true"), &objT->DissapateWhenTargetDead, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.LineType), "false"), &objT->LineType, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.TargetAttributeModifierAdd), null), &objT->TargetAttributeModifierAdd, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.SweepFX), null), &objT->SweepFX, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.ReflectorExtendDistance), "0"), &objT->ReflectorExtendDistance, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.TargetSamePlayerOnly), "false"), &objT->TargetSamePlayerOnly, state);
        Marshal(node.GetAttributeValue(nameof(LegacyBeamSpecialAbilityUpdateModuleData.PreferredTargetBone), null), &objT->PreferredTargetBone, state);
        Marshal(node, (SpecialAbilityUpdateModuleData*)objT, state);
    }
}
