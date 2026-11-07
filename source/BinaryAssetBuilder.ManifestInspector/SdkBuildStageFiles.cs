using System.Text.Json;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: persist independently qualified stream families without retaining a second combined BIG archive.
internal static class SdkBuildStageFiles
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: unpack one family using bounded I/O and refuse redirected or escaping runtime names. */
    //-------------------------------------------------------------------------------------------------
    internal static void Export(string input, string output, string family)
    {
        using var archive = BigArchive.Open(input);
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        byte[] buffer = new byte[65536];
        foreach (var entry in archive.Entries)
        {
            string name = entry.Name.Replace('/', '\\');
            string owner = name.StartsWith(@"data\aptui\", StringComparison.OrdinalIgnoreCase) ? "AptUI"
                : name.StartsWith(@"data\additionalmaps\mapmetadata_singleplayer.", StringComparison.OrdinalIgnoreCase)
                  || name.StartsWith(@"data\additionalmaps\mapmetadata_singleplayer\", StringComparison.OrdinalIgnoreCase) ? "MapCatalog"
                : name.StartsWith(@"data\additionalmaps\", StringComparison.OrdinalIgnoreCase) ? "Global" : "Static";
            if (owner != family) continue;
            if (!names.Add(name)) throw new InvalidDataException("Duplicate compiled stage entry: " + name);
            string target = Target(output, name);
            using var file = new FileStream(target, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            for (long offset = 0; offset < entry.StoredSize;)
            {
                int count = (int)Math.Min(buffer.Length, entry.StoredSize - offset);
                archive.ReadEntryRange(entry, offset, buffer.AsSpan(0, count));
                file.Write(buffer, 0, count); offset += count;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stage fresh AdditionalMaps families and their texture payloads directly from qualified compiler output. */
    //-------------------------------------------------------------------------------------------------
    internal static void ExportGlobals(string context, string output)
    {
        foreach (var global in SdkGlobalStreams.Load(context))
        {
            foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp", ".version" })
            {
                string target = Target(output, Path.ChangeExtension(global.Runtime, extension));
                if (extension == ".version") File.WriteAllText(target, "\r\n", Encoding.ASCII);
                else File.Copy(Path.ChangeExtension(global.Manifest, extension), target);
            }
            var manifest = ManifestReader.Read(File.ReadAllBytes(global.Manifest));
            var textures = manifest.Assets.Where(a => a.TypeId == 0x9312B9AC).ToArray();
            if (textures.Length == 0) continue;
            string compiled = Path.GetDirectoryName(Path.GetDirectoryName(global.Manifest))!;
            using var support = BigArchive.Open(Path.Combine(compiled, "support", "RebornConscriptProbe.big"));
            foreach (var texture in textures)
            {
                string name = OnDemandTextureCData.FileName(texture.TypeId, texture.InstanceId, texture.TypeHash, texture.InstanceHash);
                var entry = support.Entries.Single(e => e.Name.Equals(@"data\mod\cdata\" + name, StringComparison.OrdinalIgnoreCase));
                File.WriteAllBytes(Target(output, Path.ChangeExtension(global.Runtime, null) + @"\cdata\" + name), support.ReadEntry(entry));
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: persist the same three AptUI variants used by the current production packager. */
    //-------------------------------------------------------------------------------------------------
    internal static void ExportAptUI(string input, string output)
    {
        foreach (string path in Directory.GetFiles(input, "*.manifest", SearchOption.AllDirectories))
        {
            var manifest = ManifestReader.Read(File.ReadAllBytes(path));
            if (!manifest.Header.IsLinked || manifest.Validate().Count != 0) throw new InvalidDataException("Invalid AptUI stage manifest.");
            string name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
            foreach (string suffix in new[] { "", "_l", "_m" })
            foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
                File.Copy(Path.ChangeExtension(path, extension), Target(output, @"data\aptui\" + name + suffix + extension));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject stale strong imports when independently saved families are combined with changed global providers. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyLinks(string rootsJson, string sdk)
    {
        var roots = JsonSerializer.Deserialize<string[]>(File.ReadAllBytes(rootsJson)) ?? throw new InvalidDataException("Invalid stage roots.");
        var streams = roots.SelectMany(root => Directory.GetFiles(root, "*.manifest", SearchOption.AllDirectories))
            .Select(path => (Path: path, Document: ManifestReader.Read(File.ReadAllBytes(path)))).ToArray();
        var stock = SdkMissingDependencyAudit.CatalogManifestPaths(sdk).SelectMany(path => ManifestReader.Read(File.ReadAllBytes(path)).Assets).ToArray();
        var global = streams.Where(s => s.Path.Contains(Path.DirectorySeparatorChar + "additionalmaps" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)).SelectMany(s => s.Document.Assets).ToArray();
        // Reborn: LOD aliases may repeat static identities; separate early-global providers must remain unambiguous.
        var duplicate = global.GroupBy(a => new AssetId(a.TypeId,a.InstanceId)).FirstOrDefault(g => g.Count() > 1);
        if (duplicate != null) throw new InvalidDataException("Asset declared in multiple saved global streams: " + duplicate.First().Name);
        var statics = streams.Where(s => !s.Path.Contains(Path.DirectorySeparatorChar + "additionalmaps" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            && !s.Path.Contains(Path.DirectorySeparatorChar + "aptui" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)).SelectMany(s => s.Document.Assets).ToArray();
        var runtimes = roots.SelectMany(root => Directory.GetFiles(root, "*.manifest", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(root, path).Replace('/', '\\'))).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var stream in streams)
        {
            foreach (var reference in stream.Document.ReferencedManifests)
            {
                string name = reference.Path.Replace('/', '\\');
                if (!name.StartsWith(@"data\", StringComparison.OrdinalIgnoreCase)) name = @"data\" + name;
                if (name.StartsWith(@"data\additionalmaps\", StringComparison.OrdinalIgnoreCase) && !runtimes.Contains(name))
                    throw new InvalidDataException("Saved stream provider missing: " + reference.Path + ". Rebuild " + stream.Path);
            }
            TypeRegistryAudit.ValidateTarget(stream.Document.Header.Version, stream.Document.Header.AllTypesHash);
            if (!stream.Document.Header.IsLinked || stream.Document.Validate().Count != 0) throw new InvalidDataException("Invalid saved stream: " + stream.Path);
            bool early = stream.Path.Contains(Path.DirectorySeparatorChar + "additionalmaps" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            // Reborn: early globals may see retail catalogs and other globals, while later static/UI streams also see compiled static providers.
            var visible = stock.Concat(global).Concat(early ? Array.Empty<ManifestAsset>() : statics).Concat(stream.Document.Assets)
                .Select(a => new AssetId(a.TypeId, a.InstanceId)).ToHashSet();
            foreach (var asset in stream.Document.Assets)
            foreach (var reference in asset.References)
                if (!visible.Contains(reference)) throw new InvalidDataException("Saved stream strong target missing: " + asset.Name + " -> " + reference + ". Rebuild the owning stage.");
        }
        Console.WriteLine("Saved stream links verified: " + streams.Length + " manifests.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every extracted file must remain below its new, nonredirected stage payload directory. */
    //-------------------------------------------------------------------------------------------------
    private static string Target(string root, string name)
    {
        if (!name.StartsWith(@"data\", StringComparison.OrdinalIgnoreCase) || name.Any(c => c < 32 || c > 126)
            || name.Split('\\').Any(p => p is "" or "." or ".." || p.Contains(':')))
            throw new InvalidDataException("Invalid compiled runtime path: " + name);
        root = Path.GetFullPath(root);
        string target = Path.GetFullPath(Path.Combine(root, name));
        if (!target.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Stage path escaped its root.");
        for (string? part = Path.GetDirectoryName(target); part != null; part = Path.GetDirectoryName(part))
            if (Directory.Exists(part) && (File.GetAttributes(part) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Redirected compiled stage path.");
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        return target;
    }
}
