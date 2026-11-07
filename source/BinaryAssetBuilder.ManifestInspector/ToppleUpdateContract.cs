using System.Runtime.InteropServices;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the prop toppling record without borrowing another update module's layout.
internal static class ToppleUpdateContract
{
    internal const string Wire = "ToppleUpdateModuleData";
    internal const uint TypeId = 0x06DE2D62u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn the EP1 toppling metadata from original props with populated and empty FX imports. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify()
    {
        if (Marshal.SizeOf<ToppleUpdateModuleData>() != 44)
            throw new InvalidDataException("EP1 ToppleUpdate native layout differs.");

        string path = WorldBuilderAssetResolver.CatalogPath();
        var grammar = new StockMetadataGrammar();
        foreach (string name in new[] { "MY_AllyFence01", "TH_JapanOilDrum02" })
        {
            string identity = "GameObject:" + name;
            var stock = GameObjectStockReview.Capture(path, identity, allowNullArmorReferences: true);
            var module = GameObjectModuleReview.Inspect(path, identity, allowNullArmorReferences: true)
                .Containers.Single(container => container.Name == "Behaviors").Modules.Single(item => item.TypeId == TypeId);
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(ToppleUpdateModuleData));
        }

        return grammar;
    }
}
