using System.Buffers.Binary;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: use the retail Allied and Japan strafe modules to qualify EP1 metadata.
internal static class StrafeAreaUpdateContract
{
    internal const string Wire = nameof(StrafeAreaUpdateModuleData);
    internal static readonly uint TypeId = FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify the native layout and learn the retail metadata with and without a primary weapon. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest)
    {
        var grammar = new StockMetadataGrammar();
        foreach (string name in new[] { "AlliedSatelliteSweepShroudRevealer", "JapanFinalSquadronAircraft" })
        {
            string asset = "GameObject:" + name;
            var stock = GameObjectStockReview.Capture(manifest, asset, allowNullArmorReferences: true);
            var module = GameObjectModuleReview.Inspect(manifest, asset, allowNullArmorReferences: true)
                .Containers.Single(c => c.Name == "Behaviors").Modules.Single(m => m.TypeId == TypeId);
            int table = BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Wrapper + 8));
            var fields = GameObjectStockReview.Tokens(stock.Bytes, table);
            foreach (var field in typeof(StrafeAreaUpdateModuleData).GetFields().Where(f => f.Name != "Base"))
            {
                int offset = fields[FastHash.GetHashCode(field.Name)].Offset;
                // Reborn: retail metadata uses target zero for absent optional references.
                if (offset == 0 && field.FieldType == typeof(Relo.AssetReference<WeaponTemplate>))
                    continue;
                if (offset - module.Native != Marshal.OffsetOf<StrafeAreaUpdateModuleData>(field.Name).ToInt32())
                    throw new InvalidDataException("Stock StrafeAreaUpdate field offset differs: " + field.Name);
            }
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(StrafeAreaUpdateModuleData));
        }
        return grammar;
    }
}
