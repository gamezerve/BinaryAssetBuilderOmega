using System.Buffers.Binary;
using System.Runtime.InteropServices;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: certify the collision activation weight against original EP1 WorldBuilder props.
internal static class EnableDynamicsCollideContract
{
    internal const string Wire = "EnableDynamicsCollideModuleData";
    internal const uint TypeId = 0xFC7A9CB5u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify the default weight and retain the retail collision module metadata grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify()
    {
        if (Marshal.SizeOf<EnableDynamicsCollideModuleData>() != 12)
            throw new InvalidDataException("EP1 collision activation native layout differs.");

        const uint typeId = 0xFC7A9CB5u;
        const string identity = "GameObject:MY_AllyFence01";
        string path = WorldBuilderAssetResolver.CatalogPath();
        var stock = GameObjectStockReview.Capture(path, identity, allowNullArmorReferences: true);
        var module = GameObjectModuleReview.Inspect(path, identity, allowNullArmorReferences: true)
            .Containers.Single(container => container.Name == "Behaviors").Modules.Single(item => item.TypeId == TypeId);
        if (BinaryPrimitives.ReadSingleLittleEndian(stock.Bytes.AsSpan(module.Native + 8, 4)) != 0.1f)
            throw new InvalidDataException("Stock collision activation weight differs.");

        var grammar = new StockMetadataGrammar();
        grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(EnableDynamicsCollideModuleData));
        return grammar;
    }
}
