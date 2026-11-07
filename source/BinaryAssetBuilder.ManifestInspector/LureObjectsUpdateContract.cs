using System.Buffers.Binary;
using System.Runtime.InteropServices;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify Yuriko's per-rank lure guard tables from the original static object.
internal static class LureObjectsUpdateContract
{
    internal const string Wire = "LureObjectsUpdateModuleData";
    internal const uint TypeId = 0xB65AB72Au;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check the original link IDs, vector counts and metadata before using the native writer. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest)
    {
        const string identity = "GameObject:JapanYurikoTech3";
        if (Marshal.SizeOf<LureObjectsUpdateModuleData>() != 72 ||
            Marshal.OffsetOf<LureObjectsUpdateModuleData>(nameof(LureObjectsUpdateModuleData.GuardOffset)).ToInt32() != 64 ||
            Marshal.SizeOf<Vector3>() != 12)
            throw new InvalidDataException("EP1 lure guard layout differs.");

        var stock = GameObjectStockReview.Capture(manifest, identity, allowNullArmorReferences: true);
        var modules = GameObjectModuleReview.Inspect(manifest, identity, allowNullArmorReferences: true)
            .Containers.Single(container => container.Name == "Behaviors").Modules.Where(module => module.TypeId == TypeId).ToArray();
        var ranks = modules.Select(module => (
            Link: BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Native + 8, 4)),
            OffsetCount: BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Native + 64, 4))));
        if (!ranks.OrderBy(rank => rank.Link).SequenceEqual(new[] { (101, 3), (102, 5), (103, 7) }))
            throw new InvalidDataException("Stock Yuriko lure guard ranks differ.");

        var grammar = new StockMetadataGrammar();
        foreach (var module in modules)
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(LureObjectsUpdateModuleData));
        return grammar;
    }
}
