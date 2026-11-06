using System.Buffers.Binary;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: derive bridge metadata and field coordinates from the retail EP1 WorldBuilder records.
internal static class BridgeBuilderContract
{
    internal const string Wire = nameof(BridgeBuilderModuleData);
    internal const uint TypeId = 0xA2F69397u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify inline fields and direct optional bridge-part pointers against the original bridge objects. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify()
    {
        if (Marshal.SizeOf<BridgeBuilderModuleData>() != 52)
            throw new InvalidDataException("EP1 BridgeBuilder native size differs.");
        string manifest = WorldBuilderAssetResolver.CatalogPath();
        var grammar = new StockMetadataGrammar();
        foreach (string name in new[] { "GC_Bridge_01", "Bridge1" })
        {
            string asset = "GameObject:" + name;
            var stock = GameObjectStockReview.Capture(manifest, asset, allowNullArmorReferences: true);
            var module = GameObjectModuleReview.Inspect(manifest, asset, allowNullArmorReferences: true)
                .Containers.Single(c => c.Name == "Behaviors").Modules.Single(m => m.TypeId == TypeId);
            int table = BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Wrapper + 8));
            var fields = GameObjectStockReview.Tokens(stock.Bytes, table);
            foreach (var field in typeof(BridgeBuilderModuleData).GetFields().Where(f => f.Name != "Base"))
            {
                int target = fields[FastHash.GetHashCode(field.Name)].Offset;
                int slot = module.Native + Marshal.OffsetOf<BridgeBuilderModuleData>(field.Name).ToInt32();
                int expected = field.FieldType.IsPointer ? BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(slot)) : slot;
                if (target != expected)
                    throw new InvalidDataException("Stock BridgeBuilder field offset differs: " + field.Name);
            }
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(BridgeBuilderModuleData));
        }
        return grammar;
    }
}

