using System.Buffers.Binary;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: use the original Magnetic Satellite assets to qualify the EP1 lift module.
internal static class SuckUpObjectsUpdateContract
{
    internal const string Wire = "SuckUpObjectsUpdateModuleData";
    internal static readonly uint TypeId = FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check the retail field offsets and learn the inline filter metadata from all three tiers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest)
    {
        var grammar = new StockMetadataGrammar();
        foreach (int tier in new[] { 1, 2, 3 })
        {
            string name = "GameObject:MagneticSatelliteEffectLvl" + tier;
            var stock = GameObjectStockReview.Capture(manifest, name, allowNullArmorReferences: true);
            var module = GameObjectModuleReview.Inspect(manifest, name, allowNullArmorReferences: true)
                .Containers.Single(c => c.Name == "Behaviors").Modules.Single(m => m.TypeId == TypeId);
            int table = BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Wrapper + 8));
            var fields = GameObjectStockReview.Tokens(stock.Bytes, table);
            foreach (string field in new[] { "InitialDelay", "LiftVelocity", "DeathHeight", "Radius", "Duration" })
                if (fields[FastHash.GetHashCode(field)].Offset - module.Native != Marshal.OffsetOf<SuckUpObjectsUpdateModuleData>(field).ToInt32())
                    throw new InvalidDataException("Stock SuckUpObjectsUpdate field offset differs: " + field);
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(SuckUpObjectsUpdateModuleData));
        }
        return grammar;
    }
}
