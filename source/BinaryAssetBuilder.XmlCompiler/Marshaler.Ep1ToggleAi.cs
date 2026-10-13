using System;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal EP1 toggle AI fields with optional pointer storage and strict named enum/mask input. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, AISpecialPowerWeaponData* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.Weapon), null), &objT->Weapon, state);
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.TotalValueGreaterThan), "0"), &objT->TotalValueGreaterThan, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.AoeAttackPowerLocationPreference), "PREFER_NEAREST"), &objT->AoeAttackPowerLocationPreference, state, typeof(Ep1AoeAttackPowerLocationPreference), false);
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.AoeAttackPowerBlockAreaAfterActivationTime), "0s"), &objT->AoeAttackPowerBlockAreaAfterActivationTime, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.AoeAttackEmanationLocation), "EMANATE_FROM_SELF"), &objT->AoeAttackEmanationLocation, state, typeof(Ep1AoeAttackPowerEmanationLocation), false);
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.ToggleRangeHysteresis), "0.0"), &objT->ToggleRangeHysteresis, state);
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.IsDisablingAttack), "false"), &objT->IsDisablingAttack, state);
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.AffectsFriendlies), "false"), &objT->AffectsFriendlies, state);
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.AffectsSelf), "false"), &objT->AffectsSelf, state);
        Marshal(node.GetAttributeValue(nameof(AISpecialPowerWeaponData.ToggleShortRangeTargetForcesTarget), "false"), &objT->ToggleShortRangeTargetForcesTarget, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal EP1 toggle AI fields with optional pointer storage and strict named enum/mask input. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, AIToggleSpecialPowerInfo* objT, Tracker state)
    {
        if (node is null) return;
        Ep1ToggleEnum(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.ToggleHint), null), &objT->ToggleHint, state, typeof(Ep1ToggleStatusAiHint), false);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.UseOnlyWhenPowerAdvantageIsAbove), null), &objT->UseOnlyWhenPowerAdvantageIsAbove, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.UseOnlyWhenPowerAdvantageIsBelow), null), &objT->UseOnlyWhenPowerAdvantageIsBelow, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.UseOnlyWhenCostAdvantageIsAbove), null), &objT->UseOnlyWhenCostAdvantageIsAbove, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.UseOnlyWhenCostAdvantageIsBelow), null), &objT->UseOnlyWhenCostAdvantageIsBelow, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NeverUseWhenFurtherThanNUnitsFromTarget), null), &objT->NeverUseWhenFurtherThanNUnitsFromTarget, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NeverUseWhenCloserThanNUnitsFromTarget), null), &objT->NeverUseWhenCloserThanNUnitsFromTarget, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NeverUseWhenHealthIsBelow), null), &objT->NeverUseWhenHealthIsBelow, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.MaxZDifferenceFromTarget), null), &objT->MaxZDifferenceFromTarget, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NeverUseInState), ""), &objT->NeverUseInState, state, typeof(Ep1MicroManagerState), true);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NeverUseDuringTask), ""), &objT->NeverUseDuringTask, state, typeof(Ep1MicroManagerUnitTask), true);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.SelfDefenseDamageLookAheadTime), null), &objT->SelfDefenseDamageLookAheadTime, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.SelfDefenseDamageLookBehindTime), null), &objT->SelfDefenseDamageLookBehindTime, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.SelfDefenseDamageFraction), null), &objT->SelfDefenseDamageFraction, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NumOtherAttackersOnTarget), "1"), &objT->NumOtherAttackersOnTarget, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.LeechWeaponUpgrade), null), &objT->LeechWeaponUpgrade, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.LeechWeaponTargetHealthFraction), "0.1"), &objT->LeechWeaponTargetHealthFraction, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.LeechWeaponSOLModuleId), null), &objT->LeechWeaponSOLModuleId, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.LeechHealthActivatePowerAdvantage), "1.7"), &objT->LeechHealthActivatePowerAdvantage, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.LeechHealthDeactivatePowerAdvantage), "1.2"), &objT->LeechHealthDeactivatePowerAdvantage, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.StealthOthersInvisibilityModuleId), null), &objT->StealthOthersInvisibilityModuleId, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.ShieldMinCostProtected), "0"), &objT->ShieldMinCostProtected, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.ShieldMinEnemyDPS), "1000.0"), &objT->ShieldMinEnemyDPS, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.ShieldCostAdjustPastDistanceFraction), "0.5"), &objT->ShieldCostAdjustPastDistanceFraction, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.ShieldCostAdjustMultiplierAtEdge), "0.0"), &objT->ShieldCostAdjustMultiplierAtEdge, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NeverUseWhenTargetIsAnyKindOf), null), &objT->NeverUseWhenTargetIsAnyKindOf, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.Location), "Water Land"), &objT->Location, state, typeof(Ep1AiTargetLocation), true);
        var child = node.GetChildNode(nameof(AIToggleSpecialPowerInfo.StateWeapon), null);
        if (child != null)
        {
            using Tracker.Context context = state.Push((void**)&objT->StateWeapon, (uint)sizeof(AISpecialPowerWeaponData), 1);
            Marshal(child, objT->StateWeapon, state);
        }
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.OnlyUseWhenHaveTarget), "false"), &objT->OnlyUseWhenHaveTarget, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.NeverUseWhenClear), "false"), &objT->NeverUseWhenClear, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.DontUseIfInRange), "false"), &objT->DontUseIfInRange, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.EnemyMustBeInVisionRange), "false"), &objT->EnemyMustBeInVisionRange, state);
        Marshal(node.GetAttributeValue(nameof(AIToggleSpecialPowerInfo.AlwaysUseWhenEnemiesInRange), "false"), &objT->AlwaysUseWhenEnemiesInRange, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown/numeric enum tokens and preserve official enum-index bits rather than treating XML lists as arrays. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Ep1ToggleEnum(Value value, uint* target, Tracker state, Type type, bool flags)
    {
        string text = value?.GetText() ?? "";
        string[] tokens = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        if (!flags && tokens.Length != 1) throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Missing or multiple EP1 toggle enum values.");
        string[] names = Enum.GetNames(type); uint result = 0;
        foreach (string token in tokens)
        {
            int index = Array.IndexOf(names, token);
            if (index < 0 || (flags && index >= 32)) throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Unknown EP1 toggle enum token '{0}'.", token);
            if (flags) result |= 1u << index; else result = (uint)index;
        }
        state.InplaceEndianToPlatform(&result); *target = result;
    }
}
