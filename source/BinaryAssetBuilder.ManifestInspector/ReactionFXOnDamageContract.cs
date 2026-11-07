using System.Buffers.Binary;
using System.Runtime.InteropServices;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: use the original Yuriko reaction tables to qualify the native lists and optional trigger fields.
internal static class ReactionFXOnDamageContract
{
    internal const string Wire = "ReactionFXOnDamageModuleData";
    internal const uint TypeId = 0x8C3B49E2u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn the populated damage and healing lists, including voice, audio and filter pointer metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest)
    {
        const string identity = "GameObject:JapanYurikoTech3";
        const int damageOffset = 8;
        const int healingOffset = 16;
        if (Marshal.SizeOf<ReactionFXOnDamageModuleData>() != 24 ||
            Marshal.SizeOf<ReactionFXTriggerData>() != 24 ||
            Marshal.OffsetOf<ReactionFXOnDamageModuleData>(nameof(ReactionFXOnDamageModuleData.DamageReactionFXTrigger)).ToInt32() != damageOffset ||
            Marshal.OffsetOf<ReactionFXOnDamageModuleData>(nameof(ReactionFXOnDamageModuleData.HealingReactionFXTrigger)).ToInt32() != healingOffset)
            throw new InvalidDataException("EP1 reaction FX layout differs.");

        var stock = GameObjectStockReview.Capture(manifest, identity, allowNullArmorReferences: true);
        var module = GameObjectModuleReview.Inspect(manifest, identity, allowNullArmorReferences: true)
            .Containers.Single(container => container.Name == "Behaviors").Modules.Single(module => module.TypeId == TypeId);
        if (BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native + damageOffset, 4)) != 5 ||
            BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native + healingOffset, 4)) != 2)
            throw new InvalidDataException("Stock Yuriko damage and healing trigger counts differ.");

        var grammar = new StockMetadataGrammar();
        grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(ReactionFXOnDamageModuleData));
        return grammar;
    }
}
