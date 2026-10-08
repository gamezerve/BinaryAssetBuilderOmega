using System.Security.Cryptography;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove only two exact library/all map aliases, not generic Windows trailing-dot normalization.
internal static class SdkKnownMapAliases
{
    internal const string Name = "diagnostic-known-map-aliases-v1";
    // Reborn: retain original Include spelling/role and ordinary source snapshot separately from admitted physical traversal.
    internal sealed record Witness(string Document,string Kind,string LogicalPath,string PhysicalPath,string Sha256,long Bytes);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match an exact source-library/all recipe, prove native dotted/ordinary equivalence and retain bounded confined source identity. */
    //-------------------------------------------------------------------------------------------------
    internal static Witness? Prove(string root,string document,string kind,string logical)
    {
        string normalized = logical.Replace('\\','/');
        var recipe = new[] {
            (Library:"SkirmishAI/Personalities/AIPersonalityLibrary.xml",Target:"DATA:maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml."),
            (Library:"SkirmishAI/States/AIStateLibrary.xml",Target:"DATA:maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml.") }
            .SingleOrDefault(item => normalized.Equals(item.Target,StringComparison.OrdinalIgnoreCase));
        if (recipe.Library == null) return null;
        if (kind != "all" || !document.Equals(Path.GetFullPath(Path.Combine(root,recipe.Library)),StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Known map alias requires its exact library and original all Include role.");
        if (!OperatingSystem.IsWindows()) throw new InvalidDataException("Known map alias requires separately observed Windows path semantics.");
        var resolved = SdkSourcePathAudit.Resolve(logical[..^1],Path.GetDirectoryName(document)!,root,root,null,null);
        string dotted = resolved.Path+".";
        if (!Path.GetFullPath(dotted).Equals(resolved.Path,StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Native map alias canonical identity changed.");
        byte[] ordinary = SdkEnvironmentPreflight.Read(resolved.Path,4*1048576),native = SdkEnvironmentPreflight.Read(dotted,4*1048576);
        if (!ordinary.AsSpan().SequenceEqual(native)) throw new InvalidDataException("Native map alias bytes differ from confined target.");
        return new(document,kind,logical,resolved.Path,Hash(ordinary),ordinary.Length);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: replay only a captured exact alias witness with unchanged source hash/role/context; no witness means strict resolution. */
    //-------------------------------------------------------------------------------------------------
    internal static SdkSourcePathAudit.Resolved ResolveCaptured(SdkSourcePathAudit.Report paths,string document,string kind,string logical,string confinement)
    {
        var witness = paths.KnownMapAliases.SingleOrDefault(item => item.Document.Equals(document,StringComparison.OrdinalIgnoreCase) && item.Kind == kind && item.LogicalPath == logical);
        if (witness == null) return SdkSourcePathAudit.Resolve(logical,Path.GetDirectoryName(document)!,confinement,paths.SourceRoot,paths.ArtRoot,paths.AudioRoot);
        var observed = Prove(paths.SourceRoot,document,kind,logical);
        if (observed != witness || !paths.Sources.Any(source => source.PhysicalPath.Equals(witness.PhysicalPath,StringComparison.OrdinalIgnoreCase) && source.Sha256 == witness.Sha256 && source.Bytes == witness.Bytes))
            throw new InvalidDataException("Captured known map alias identity changed or is absent from source inventory.");
        return new(witness.PhysicalPath,paths.SourceRoot);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source snapshot hashes never establish native asset identities or game loading. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
