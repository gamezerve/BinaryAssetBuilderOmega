using System.Buffers.Binary;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class StrafeAreaTargetChooserContract
{
    internal const string Wire = nameof(StrafeAreaTargetChooserModuleData);
    internal const uint TypeId = 0xD692DC8Du;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the chooser header and nested metadata against the original Final Squadron seeds and aircraft. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest)
    {
        if (Marshal.SizeOf<StrafeAreaTargetChooserModuleData>() != 24)
            throw new InvalidDataException("EP1 StrafeAreaTargetChooser native header size differs.");

        var grammar = new StockMetadataGrammar();
        foreach (string name in new[] { "JapanFinalSquadronSeed_L1", "JapanFinalSquadronSeed_L2", "JapanFinalSquadronSeed_L3", "JapanFinalSquadronAircraft" })
        {
            string asset = "GameObject:" + name;
            var stock = GameObjectStockReview.Capture(manifest, asset, allowNullArmorReferences: true);
            var module = GameObjectModuleReview.Inspect(manifest, asset, allowNullArmorReferences: true)
                .Containers.Single(c => c.Name == "Behaviors").Modules.Single(m => m.TypeId == TypeId);
            var fields = GameObjectStockReview.Tokens(stock.Bytes, Read(module.Wrapper + 8));
            foreach (string field in new[] { "Radius", "MaxAttacksPerTarget", "OverflowOptions" })
                if (fields[FastHash.GetHashCode(field)].Offset - module.Native != Marshal.OffsetOf<StrafeAreaTargetChooserModuleData>(field).ToInt32())
                    throw new InvalidDataException("Stock StrafeAreaTargetChooser field offset differs: " + field);

            int filterSlot = module.Native + Marshal.OffsetOf<StrafeAreaTargetChooserModuleData>("ObjectFilter").ToInt32();
            int filterTable = fields[FastHash.GetHashCode("ObjectFilter")].Offset;
            var filterFields = GameObjectStockReview.Tokens(stock.Bytes, filterTable);
            if (filterFields[FastHash.GetHashCode("id")].Offset != Read(filterSlot))
                throw new InvalidDataException("Stock StrafeAreaTargetChooser filter pointer differs.");

            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(StrafeAreaTargetChooserModuleData));

            // Reborn: original table and native offsets are Win32 little-endian words.
            int Read(int offset) => BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(offset, 4));
        }
        return grammar;
    }
}
