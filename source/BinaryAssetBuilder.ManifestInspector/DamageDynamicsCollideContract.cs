using System.Runtime.InteropServices;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the Yuriko collision damage module uses an inline list of untagged damage records.
internal static class DamageDynamicsCollideContract
{
    internal const string Wire = "DamageDynamicsCollideModuleData";
    internal const uint TypeId = 0x19BF3331u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn the collision and nested damage metadata from the SDK's stock WorldBuilder catalog. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify()
    {
        if (Marshal.SizeOf<DamageDynamicsCollideModuleData>() != 20 || Marshal.SizeOf<DamageNuggetType>() != 156)
            throw new InvalidDataException("EP1 collision damage native layout differs.");

        string path = WorldBuilderAssetResolver.CatalogPath();
        var grammar = new StockMetadataGrammar();
        foreach (string name in new[] { "ClientFlingableExplodingBarrel", "ClientFlingableCryoBarrel" })
        {
            string identity = "GameObject:" + name;
            var stock = GameObjectStockReview.Capture(path, identity, allowNullArmorReferences: true);
            var module = GameObjectModuleReview.Inspect(path, identity, allowNullArmorReferences: true)
                .Containers.Single(container => container.Name == "Behaviors").Modules.Single(item => item.TypeId == TypeId);
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(DamageDynamicsCollideModuleData));
        }

        return grammar;
    }
}
