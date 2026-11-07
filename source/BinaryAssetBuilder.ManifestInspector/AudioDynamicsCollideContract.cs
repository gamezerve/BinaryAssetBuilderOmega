using System.Runtime.InteropServices;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 impact audio uses inline magnitude thresholds and strong audio references.
internal static class AudioDynamicsCollideContract
{
    internal const string Wire = "AudioDynamicsCollideModuleData";
    internal const uint TypeId = 0xD6C03AC2u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify sound entries with both populated and empty imports from stock WorldBuilder props. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify()
    {
        if (Marshal.SizeOf<AudioDynamicsCollideModuleData>() != 20 || Marshal.SizeOf<MagnitudeSoundSelectorEntry>() != 8)
            throw new InvalidDataException("EP1 collision audio native layout differs.");

        string path = WorldBuilderAssetResolver.CatalogPath();
        var grammar = new StockMetadataGrammar();
        foreach (string name in new[] { "ClientFlingableRock01", "ClientFlingableExplodingBarrel" })
        {
            string identity = "GameObject:" + name;
            var stock = GameObjectStockReview.Capture(path, identity, allowNullArmorReferences: true);
            var module = GameObjectModuleReview.Inspect(path, identity, allowNullArmorReferences: true)
                .Containers.Single(container => container.Name == "Behaviors").Modules.Single(item => item.TypeId == TypeId);
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(AudioDynamicsCollideModuleData));
        }

        return grammar;
    }
}
