using System.Runtime.InteropServices;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: learn the Yuriko shield's inherited sphere layout and FX imports from all three retail ranks.
internal static class YurikoShieldSphereContract
{
    internal const string Wire = "YurikoShieldSphereUpdateModuleData";
    internal const uint TypeId = 0x95D855B6u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: confirm the retail header, shield durations, and metadata before composing the authored shield. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest)
    {
        const string identity = "GameObject:JapanYurikoTech3";
        const int durationOffset = 28;
        if (Marshal.SizeOf<YurikoShieldSphereUpdateModuleData>() != 328 ||
            Marshal.OffsetOf<SphereModuleUpdateModuleData>(nameof(SphereModuleUpdateModuleData.Duration)).ToInt32() != durationOffset ||
            Marshal.OffsetOf<YurikoShieldSphereUpdateModuleData>(nameof(YurikoShieldSphereUpdateModuleData.MajorShieldHitFX)).ToInt32() != 312 ||
            Marshal.OffsetOf<YurikoShieldSphereUpdateModuleData>(nameof(YurikoShieldSphereUpdateModuleData.MinorShieldHitFX)).ToInt32() != 316 ||
            Marshal.OffsetOf<YurikoShieldSphereUpdateModuleData>(nameof(YurikoShieldSphereUpdateModuleData.MinorShieldDamageTypes)).ToInt32() != 320)
            throw new InvalidDataException("EP1 Yuriko shield layout differs.");

        var stock = GameObjectStockReview.Capture(manifest, identity, allowNullArmorReferences: true);
        var modules = GameObjectModuleReview.Inspect(manifest, identity, allowNullArmorReferences: true)
            .Containers.Single(container => container.Name == "Behaviors").Modules.Where(module => module.TypeId == TypeId).ToArray();
        var durations = modules.Select(module => BitConverter.ToSingle(stock.Bytes, module.Native + durationOffset));
        if (!durations.Order().SequenceEqual(new[] { 5f, 7f, 9f }))
            throw new InvalidDataException("Stock Yuriko shield ranks differ.");

        var grammar = new StockMetadataGrammar();
        foreach (var module in modules)
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(YurikoShieldSphereUpdateModuleData));
        return grammar;
    }
}
