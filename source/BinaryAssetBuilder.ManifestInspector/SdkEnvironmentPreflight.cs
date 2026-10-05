using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: validate explicit EP1 SDK paths as read-only planning evidence, never execute reference builders or mutate registry/settings.
internal static class SdkEnvironmentPreflight
{
    // Reborn: report snapshot evidence separately from production readiness, include resolution and game-data availability.
    internal sealed record Mapping(string PhysicalManifest,string RuntimeManifest,string Sha256,int AssetCount);
    internal sealed record Report(string Target,string SchemaRoot,string SchemaEntry,int SchemaFiles,string SchemaCatalogSha256,
        string SourceRoot,string SourceEntry,string SourceEntrySha256,string OutputDirectory,Mapping[] ExternalMappings,
        bool ReadOnly,bool SnapshotOnly,bool SchemaCatalogMatches,bool IncludedSourcesValidated,bool ProductionBuildReady,
        string ExpectedGameAllTypesHash,string[] Limitations);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require explicit target/root paths and current schema/manifest evidence without writing outputs or selecting legacy plugins. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(string target,string schemaRoot,string sourceRoot,string sourceEntry,string output,string[] externalPairs)
    {
        if (target != "ra3ep1") throw new NotSupportedException("SDK preflight admits only explicit target ra3ep1; no RA3/KW fallback.");
        schemaRoot = DirectoryPath(schemaRoot); sourceRoot = DirectoryPath(sourceRoot); sourceEntry = Absolute(sourceEntry); output = Absolute(output);
        if (!Inside(sourceRoot,sourceEntry)) throw new InvalidDataException("SDK source entry must be a child of the explicit source root.");
        if (!sourceEntry.EndsWith(".xml",StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("SDK source entry must be XML.");
        string parent = Path.GetDirectoryName(output) ?? throw new InvalidDataException("SDK output requires a named new child directory.");
        DirectoryPath(parent);
        if (Directory.Exists(output) || File.Exists(output) || Inside(sourceRoot,output) || Inside(schemaRoot,output)) throw new InvalidDataException("SDK output must be absent and outside source/schema roots; no overwrite is authorized.");
        byte[] xml = Read(sourceEntry,4*1048576);
        // Reborn: entry syntax inspection prohibits external entity/DTD execution; it is not full schema validation or Include resolution.
        using (MemoryStream bytes = new(xml,false))
        using (XmlReader reader = XmlReader.Create(bytes,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 }))
        {
            XmlDocument document = new() { XmlResolver = null }; document.Load(reader);
            if (document.DocumentElement?.LocalName != "AssetDeclaration" || document.DocumentElement.NamespaceURI != "uri:ea.com:eala:asset")
                throw new InvalidDataException("SDK source entry requires an EA AssetDeclaration document.");
        }
        if (externalPairs.Length > 8) throw new InvalidDataException("SDK preflight admits at most eight explicit external mappings.");
        List<Mapping> mappings = new(); HashSet<string> physical = new(StringComparer.OrdinalIgnoreCase),runtime = new(StringComparer.OrdinalIgnoreCase);
        foreach (string pair in externalPairs)
        {
            string[] parts = pair.Split('=',2);
            if (parts.Length != 2) throw new InvalidDataException("SDK external mapping must be absolute.manifest=relative-runtime.manifest.");
            string path = Absolute(parts[0]); string name = RuntimeName(parts[1]);
            if (!path.EndsWith(".manifest",StringComparison.OrdinalIgnoreCase) || !physical.Add(path) || !runtime.Add(name)) throw new InvalidDataException("SDK external manifests/runtime names must be unique and explicitly named.");
            byte[] bytes = Read(path,16*1048576);
            // Reborn: bound decompressed metadata before the shared reader; tiny RefPack headers must not authorize a huge allocation.
            byte[] metadata = RefPack.IsCompressed(bytes) ? RefPack.Decompress(bytes,16*1048576) : bytes;
            var manifest = ManifestReader.Read(metadata);
            TypeRegistryAudit.ValidateTarget(manifest.Header.Version,manifest.Header.AllTypesHash);
            if (!manifest.Header.IsLinked || manifest.Validate().Count != 0 || manifest.ReferencedManifests.Any(reference => reference.IsPatch))
                throw new InvalidDataException("SDK external input must be a valid linked EP1 manifest without patch bases.");
            mappings.Add(new(path,name,Convert.ToHexString(SHA256.HashData(bytes)),manifest.Assets.Count));
        }
        // Reborn: reject malformed paths/source/mappings before the larger schema catalog read; identical selected/baseline roots need one current snapshot.
        string baseline = BaselineSchemaRoot(); var approved = Catalog(baseline);
        var candidate = schemaRoot.Equals(baseline,StringComparison.OrdinalIgnoreCase) ? approved : Catalog(schemaRoot);
        if (candidate.Count != approved.Count || candidate.Any(row => !approved.TryGetValue(row.Key,out string? hash) || hash != row.Value))
            throw new InvalidDataException("SDK schema catalog differs from the staged EP1 baseline; RA3/custom schemas are not silently admitted.");
        if (!candidate.ContainsKey("cnc3types.xsd")) throw new InvalidDataException("SDK schema entry CnC3Types.xsd is absent.");
        return new("ra3ep1",schemaRoot,Path.Combine(schemaRoot,"CnC3Types.xsd"),candidate.Count,Digest(candidate),sourceRoot,sourceEntry,
            Convert.ToHexString(SHA256.HashData(xml)),output,mappings.ToArray(),true,true,true,false,false,"0x5454A8E9",
            new[] { "Paths/evidence only: no schema compilation, Includes/art/audio lookup or full source graph validation.",
                "Schema catalog parity is against this checkout's staged baseline, not final compiler type-table parity.",
                "No registry/environment fallback, registry writes, reference BAB launch, cache changes, native codecs or output creation.",
                "Manifest metadata only: adjacent BIN/RELO/IMP/custom files and runtime availability are not validated.",
                "Snapshot-only: repeat checks before a future build; no atomic concurrent-filesystem guarantee.",
                "Production processors, cache/type hashes, WorldBuilder packaging and Uprising game loading remain open." });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate only the checkout's staged schema baseline; never discover an SDK or game through registry/user environment. */
    //-------------------------------------------------------------------------------------------------
    internal static string BaselineSchemaRoot() => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"..","..","schemas","ra3ep1","xsd"));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: enumerate only bounded XSD metadata with reparse checks before descent, including exact relative names and content hashes. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string,string> Catalog(string root)
    {
        Dictionary<string,string> rows = new(StringComparer.OrdinalIgnoreCase); int directories = 0; long total = 0; Visit(root,0); return rows;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: inspect each directory before walking it; oversized/reparse schema trees cannot expand the planning read scope. */
        //-------------------------------------------------------------------------------------------------
        void Visit(string directory,int depth)
        {
            CheckPath(directory); if (++directories > 128 || depth > 8) throw new InvalidDataException("SDK schema directory bound exceeded.");
            foreach (string path in Directory.EnumerateFileSystemEntries(directory))
            {
                // Reborn: this parent was checked before enumeration; inspect each immediate child before descent/read without rescanning every ancestor per XSD.
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("SDK schema catalog rejects reparse children.");
                if (Directory.Exists(path)) { Visit(path,depth+1); continue; }
                if (!path.EndsWith(".xsd",StringComparison.OrdinalIgnoreCase)) continue;
                string name = Path.GetRelativePath(root,path).Replace('\\','/').ToLowerInvariant();
                if (name.Length > 512 || rows.Count >= 1024) throw new InvalidDataException("SDK schema file/name bound exceeded.");
                byte[] bytes = Read(path,2*1048576,false); total += bytes.Length;
                if (total > 64*1048576 || !rows.TryAdd(name,Convert.ToHexString(SHA256.HashData(bytes)))) throw new InvalidDataException("SDK schema aggregate/alias bound exceeded.");
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: hash an ordered length-delimited metadata catalog, not an asserted EA AllTypesHash or production cache identity. */
    //-------------------------------------------------------------------------------------------------
    private static string Digest(Dictionary<string,string> catalog)
    {
        using MemoryStream bytes = new(); using BinaryWriter writer = new(bytes,Encoding.UTF8,true);
        foreach (var row in catalog.OrderBy(row => row.Key,StringComparer.Ordinal)) { writer.Write(row.Key); writer.Write(row.Value); }
        writer.Flush(); return Convert.ToHexString(SHA256.HashData(bytes.ToArray()));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject drive-relative/current-directory/environment discovery; all selected physical paths must be explicit absolute literals. */
    //-------------------------------------------------------------------------------------------------
    private static string Absolute(string path)
    {
        if (!Path.IsPathFullyQualified(path) || path.IndexOfAny(new[] { '*','?' }) >= 0) throw new InvalidDataException("SDK physical paths must be absolute literals without wildcards.");
        // Reborn: do not open Windows device namespaces, reserved leaves or alternate data streams through an apparently ordinary metadata path.
        if (path.StartsWith(@"\\?\",StringComparison.Ordinal) || path.StartsWith(@"\\.\",StringComparison.Ordinal)
            || path.IndexOf(':',path.Length > 1 && path[1] == ':' ? 2 : 0) >= 0) throw new InvalidDataException("SDK physical paths cannot name device namespaces or alternate data streams.");
        string full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        foreach (string part in full.Split(new[] { Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar },StringSplitOptions.RemoveEmptyEntries))
        {
            string stem = part.Split('.')[0].TrimEnd(' ').ToUpperInvariant();
            if (stem is "CON" or "PRN" or "AUX" or "NUL" || (stem.Length == 4 && (stem.StartsWith("COM",StringComparison.Ordinal) || stem.StartsWith("LPT",StringComparison.Ordinal)) && stem[3] is >= '1' and <= '9'))
                throw new InvalidDataException("SDK physical paths cannot name Windows reserved devices.");
        }
        return full;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve only an existing non-reparse explicit directory without modifying it. */
    //-------------------------------------------------------------------------------------------------
    private static string DirectoryPath(string path)
    { path = Absolute(path); CheckPath(path); if (!Directory.Exists(path)) throw new DirectoryNotFoundException(path); return path; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete path segments so sibling prefixes cannot impersonate a selected source/schema root. */
    //-------------------------------------------------------------------------------------------------
    private static bool Inside(string root,string path) => path.StartsWith(Path.TrimEndingDirectorySeparator(root)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep game-visible manifest names relative and separate from physical source locations or lookup macros. */
    //-------------------------------------------------------------------------------------------------
    private static string RuntimeName(string name)
    {
        name = name.Replace('/','\\').ToLowerInvariant();
        if (name.Length is < 10 or > 256 || !name.EndsWith(".manifest",StringComparison.Ordinal) || name.Split('\\').Any(part => part.Length == 0 || part is "." or "..")
            || name.Any(value => !(value is >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '-' or '.' or '\\')))
            throw new InvalidDataException("SDK runtime manifest must be a bounded relative literal without traversal/selectors.");
        return name;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read one bounded metadata file through one handle and reject observed growth/reparse ancestry, never dump a binary stream. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Read(string path,int limit,bool checkAncestors = true)
    {
        // Reborn: only catalog callers may reuse just-checked parent/child metadata; arbitrary source/manifest reads always inspect full ancestry.
        if (checkAncestors) CheckPath(path); using FileStream stream = new(path,FileMode.Open,FileAccess.Read,FileShare.Read);
        if (stream.Length > limit) throw new InvalidDataException("SDK metadata file exceeds its bound.");
        byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes); if (stream.ReadByte() != -1) throw new InvalidDataException("SDK metadata grew during read."); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect existing ancestors before any read/traversal; this does not promise adversarial filesystem isolation. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckPath(string path)
    {
        for (string? current = path; current != null; current = Path.GetDirectoryName(current))
            if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("SDK preflight rejects reparse paths.");
    }
}
