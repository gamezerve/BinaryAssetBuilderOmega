using System.Text.Json;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compare saved campaign map patches with the same retail spawn targets used by the resolver.
internal static class WorldBuilderSpawnAudit
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: report only maps whose borrowed records refer to missing spawn assets, following missing chains transitively. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string catalog, string mapList, IEnumerable<string> providers)
    {
        var world = ManifestReader.Read(File.ReadAllBytes(catalog));
        using var spawns = new WorldBuilderSpawnDependencies(catalog, world);
        var retail = world.Assets.ToDictionary(WorldBuilderAssetResolver.Id);
        var shared = providers.SelectMany(path => ManifestReader.Read(File.ReadAllBytes(path)).Assets)
            .Select(WorldBuilderAssetResolver.Id).ToHashSet();
        var report = new List<object>();
        foreach (string path in File.ReadLines(mapList).Where(line => !string.IsNullOrWhiteSpace(line)))
        {
            var map = ManifestReader.Read(File.ReadAllBytes(path));
            var present = new HashSet<AssetId>(shared);
            present.UnionWith(map.Assets.Select(WorldBuilderAssetResolver.Id));
            var visited = new HashSet<AssetId>();
            var missing = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var asset in map.Assets.Where(asset => asset.InstanceDataSize == 0))
                Check(WorldBuilderAssetResolver.Id(asset), asset.Name);
            report.Add(new { Path = path, Missing = missing });

            // Reborn: explain each omitted target with an owner name and walk any omitted retail spawn chain.
            void Check(AssetId id, string owner)
            {
                if (!visited.Add(id)) return;
                foreach (var target in spawns.Read(id))
                {
                    if (present.Contains(target) || !retail.TryGetValue(target, out var entry)) continue;
                    missing.TryAdd(entry.Name, owner);
                    Check(target, entry.Name);
                }
            }
        }
        Console.WriteLine(JsonSerializer.Serialize(report));
    }
}
