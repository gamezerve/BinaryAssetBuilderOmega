using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class UprisingLayoutSmokeTest
{
    public static void Run()
    {
        ExpectSize<SageBinaryData.ArmorTemplate>(32);
        ExpectSize<SageBinaryData.ArmorSetBitFlags>(4);
        Expect(SageBinaryData.ArmorSetBitFlags.Count == 23, "ArmorSetBitFlags.Count", 23, SageBinaryData.ArmorSetBitFlags.Count);

        ExpectSize<SageBinaryData.AttributeModifier>(56);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Category), 4);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Duration), 8);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.StartFX), 12);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ModelConditionsSet), 20);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ObjectStatusToSet), 28);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ArmorSetType), 36);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Shader), 40);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Modifier), 44);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ReplaceInCategoryIfLongest), 52);

        Expect(SageBinaryData.ModelConditionBitFlags.Count == 463, "ModelConditionBitFlags.Count", 463, SageBinaryData.ModelConditionBitFlags.Count);
        Expect(SageBinaryData.ObjectStatusBitFlags.Count == 230, "ObjectStatusBitFlags.Count", 230, SageBinaryData.ObjectStatusBitFlags.Count);
        Expect(SageBinaryData.DisabledBitFlags.Count == 13, "DisabledBitFlags.Count", 13, SageBinaryData.DisabledBitFlags.Count);
        Expect((int)SageBinaryData.ArmorSetType.INVALID == 0, "ArmorSetType.INVALID", 0, (int)SageBinaryData.ArmorSetType.INVALID);
        Expect((int)SageBinaryData.ArmorSetType.SHIELDBODY_ENABLED == 22, "ArmorSetType.SHIELDBODY_ENABLED", 22, (int)SageBinaryData.ArmorSetType.SHIELDBODY_ENABLED);
        Expect((int)SageBinaryData.AttributeModifierCategoryType.SHRINK == 15, "AttributeModifierCategoryType.SHRINK", 15, (int)SageBinaryData.AttributeModifierCategoryType.SHRINK);
        Expect((int)SageBinaryData.AttributeType.RADIATION_ARMOR == 36, "AttributeType.RADIATION_ARMOR", 36, (int)SageBinaryData.AttributeType.RADIATION_ARMOR);

        ExpectSize<SageBinaryData.LocomotorTemplate>(396);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.PreferredHeightPitchingEpsilon), 84);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.ActiveModelConditions), 108);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.ReverseMoveSpeed), 208);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.SpeedBasedHeightOffset), 340);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.JetLocomotorData), 364);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.IgnoreLowSpeedAngleMultiplier), 393);
        Expect(SageBinaryData.LocomotorSurfaceBitFlags.Count == 11, "LocomotorSurfaceBitFlags.Count", 11, SageBinaryData.LocomotorSurfaceBitFlags.Count);

        ExpectSize<SageBinaryData.WeaponAiHintInfo>(12);
        ExpectOffset<SageBinaryData.WeaponAiHintInfo>(nameof(SageBinaryData.WeaponAiHintInfo.UseAsWarheadForDamageCalculations), 4);
        ExpectOffset<SageBinaryData.WeaponAiHintInfo>(nameof(SageBinaryData.WeaponAiHintInfo.IsAntiGarrisonWeapon), 8);
        ExpectSize<SageBinaryData.WeaponTemplate>(340);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.AttackRange), 4);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.PreferredTargetBone), 76);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.ImpactLoopSound), 96);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.RequiredFiringObjectStatus), 144);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.VirtualDamage), 228);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.WeaponAiHintInfo), 260);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.IncompatibleAttributeModifier), 292);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.ScatterIndependently), 300);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.ProjectileSelfUsesPathfinder), 312);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.UpdateBarrelModelConditions), 338);
        Expect(SageBinaryData.WeaponFlagsBitFlags.Count == 13, "WeaponFlagsBitFlags.Count", 13, SageBinaryData.WeaponFlagsBitFlags.Count);
        Expect(SageBinaryData.WeaponAntiBitFlags.Count == 14, "WeaponAntiBitFlags.Count", 14, SageBinaryData.WeaponAntiBitFlags.Count);

        Console.WriteLine("Uprising layout self-test: OK");
    }

    private static void ExpectSize<T>(int expected) where T : struct =>
        Expect(Marshal.SizeOf<T>() == expected, $"sizeof({typeof(T).Name})", expected, Marshal.SizeOf<T>());

    private static void ExpectOffset<T>(string field, int expected) where T : struct =>
        Expect(Marshal.OffsetOf<T>(field).ToInt32() == expected, $"offsetof({typeof(T).Name}.{field})", expected, Marshal.OffsetOf<T>(field).ToInt32());

    private static void Expect(bool condition, string name, int expected, int actual)
    {
        if (!condition)
        {
            throw new InvalidDataException($"{name}: expected {expected}, got {actual}.");
        }
    }
}
