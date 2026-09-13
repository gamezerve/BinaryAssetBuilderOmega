namespace BinaryAssetBuilder.ManifestInspector;

internal static class UtilityManifestVerifier
{
    public static void Verify(string path, string? entryName)
    {
        string inputPath = Path.GetFullPath(path);
        string? temporaryPath = null;
        try
        {
            if (inputPath.EndsWith(".big", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(entryName))
                {
                    throw new ArgumentException("--entry is required when utility-verify reads a BIG archive.");
                }

                using BigArchive archive = BigArchive.Open(inputPath);
                BigEntry entry = archive.Entries.SingleOrDefault(candidate =>
                    candidate.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidDataException($"BIG archive does not contain '{entryName}'.");
                byte[] stored = archive.ReadEntry(entry);
                byte[] payload = RefPack.IsCompressed(stored) ? RefPack.Decompress(stored) : stored;
                temporaryPath = Path.Combine(Path.GetTempPath(), $"bab-ep1-{Guid.NewGuid():N}.manifest");
                File.WriteAllBytes(temporaryPath, payload);
                inputPath = temporaryPath;
            }

            using BinaryAssetBuilder.Utility.Manifest manifest = new BinaryAssetBuilder.Utility.Manifest();
            if (!manifest.Load(inputPath, false))
            {
                throw new InvalidDataException($"Utility reader could not load '{path}'.");
            }

            Console.WriteLine($"Utility reader OK: {path}{(entryName is null ? string.Empty : $"::{entryName}")}");
            Console.WriteLine(
                $"  Version={manifest.Version} Assets={manifest.AssetCount:N0} " +
                $"AllTypesHash=0x{manifest.AllTypesHash:X8} Linked={manifest.IsLinked}");
        }
        finally
        {
            if (temporaryPath is not null && File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}
