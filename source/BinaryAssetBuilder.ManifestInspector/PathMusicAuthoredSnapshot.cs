using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: freeze a bounded local authored music/header closure independently of missing official AUDIO inputs and production processing identities.
internal sealed class PathMusicAuthoredSnapshot
{
    // Reborn: immutable rows describe local weak closure and diagnostic content fingerprints, never stock processing/type hashes.
    internal sealed record Row(string Name,uint InstanceId,uint EventValue,string? Alternate,bool Cacheable,int BinBytes,int RelocationBytes,string BinSha256,string RelocationSha256);
    internal sealed record Report(string SourceSha256,string HeaderSha256,string DiagnosticFingerprint,Row[] Rows)
    {
        public bool ReadOnly => true;
        public bool SnapshotOnly => true;
        public bool OfficialAudioDependenciesResolved => false;
        public bool ProductionBuildReady => false;
    }
    private readonly string _directory;
    private readonly byte[] _source,_header;
    private readonly Row[] _rows;
    private readonly XmlSchemaSet _schemas;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain only private detached raw snapshots and immutable prepared row identities. */
    //-------------------------------------------------------------------------------------------------
    private PathMusicAuthoredSnapshot(string directory,byte[] source,byte[] header,Row[] rows,XmlSchemaSet schemas)
    { _directory = directory; _source = (byte[])source.Clone(); _header = (byte[])header.Clone(); _rows = rows.ToArray(); _schemas = schemas; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: report copied rows and a versioned diagnostic fingerprint, not Core InstanceHash or an authored production contract. */
    //-------------------------------------------------------------------------------------------------
    internal Report Preflight()
    {
        string source = Hash(_source),header = Hash(_header);
        string identity = "Reborn-PathMusicLocal-v1\n"+source+"\n"+header+"\n"+string.Join("\n",_rows.Select(row => row.Name+":"+row.InstanceId.ToString("X8")+":"+row.BinSha256+":"+row.RelocationSha256));
        return new(source,header,Hash(Encoding.UTF8.GetBytes(identity)),_rows.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare current raw bytes even after timestamp/size-preserving edits; re-admission alone never refreshes an old snapshot. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCurrent()
    {
        var current = Read(_directory,_schemas);
        if (!_source.AsSpan().SequenceEqual(current._source) || !_header.AsSpan().SequenceEqual(current._header)) throw new InvalidDataException("Authored music snapshot is stale; prepare current inputs again.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return freshly detached native chunks only after before/after current-input checks; no cache, package or processor registration. */
    //-------------------------------------------------------------------------------------------------
    internal Chunk[] Compile()
    {
        VerifyCurrent();
        Chunk[] chunks = _rows.Select(row => PathMusicRuntimeProbe.FromHeader(_header,row.Name,row.Alternate,row.Cacheable).Chunk).ToArray();
        for (int index = 0; index < chunks.Length; index++)
            if (Hash(chunks[index].InstanceBuffer) != _rows[index].BinSha256 || Hash(chunks[index].RelocationBuffer) != _rows[index].RelocationSha256 || chunks[index].ImportsBuffer.Length != 0)
                throw new InvalidDataException("Frozen authored music preparation differs.");
        VerifyCurrent(); return chunks;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only direct local events.xml/events.h with reviewed schema and exact local weak targets; official roots/Includes/inheritance stay outside this profile. */
    //-------------------------------------------------------------------------------------------------
    internal static PathMusicAuthoredSnapshot Read(string directory,XmlSchemaSet? schemas = null)
    {
        directory = Path.GetFullPath(directory);
        string sourcePath = Path.Combine(directory,"events.xml"),headerPath = Path.Combine(directory,"events.h");
        byte[] source = SdkEnvironmentPreflight.Read(sourcePath,32768),header = SdkEnvironmentPreflight.Read(headerPath,1048576);
        if (schemas == null)
        {
            var evidence = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
            if (!evidence.SchemaAdmitted || schemas == null) throw new InvalidDataException("Reviewed EP1 schema required.");
        }
        if (source.AsSpan().StartsWith(new byte[] { 0xEF,0xBB,0xBF })) throw new InvalidDataException("Authored music XML requires UTF-8 without BOM.");
        string text = new UTF8Encoding(false,true).GetString(source); XmlDocument xml = new() { XmlResolver = null };
        using (XmlReader reader = XmlReader.Create(new StringReader(text),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 32768 })) xml.Load(reader);
        if (xml.FirstChild is XmlDeclaration declaration && declaration.Encoding.Length != 0 && !declaration.Encoding.Equals("utf-8",StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Authored music declaration must use UTF-8.");
        const string ea = "uri:ea.com:eala:asset"; XmlElement? root = xml.DocumentElement;
        if (root?.LocalName != "AssetDeclaration" || root.NamespaceURI != ea || root.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.Name != "xmlns")
            || root.ChildNodes.OfType<XmlText>().Any(node => node.Value?.Trim().Length > 0)) throw new InvalidDataException("Plain EA authored declaration required.");
        XmlElement[] owners = root.ChildNodes.OfType<XmlElement>().ToArray();
        if (owners.Length is < 1 or > 8 || owners.Any(owner => owner.LocalName != "PathMusicEvent" || owner.NamespaceURI != ea || owner.HasChildNodes
            || owner.GetAttribute("PathfinderEventHeader") != "events.h" || owner.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.Name is not ("id" or "PathfinderEventHeader" or "RestartAlternateEvent" or "IsCacheable"))))
            throw new InvalidDataException("1..8 direct empty PathMusicEvent owners and events.h leaf required; no Include/inheritance/alias or runtime roots.");
        string[] names = owners.Select(owner => owner.GetAttribute("id")).ToArray();
        foreach (string name in names) PathMusicHeaderSemantics.ReviewLine("",name);
        if (names.Distinct(StringComparer.Ordinal).Count() != names.Length || names.Select(InstanceHandle.GetInstanceId).Distinct().Count() != names.Length)
            throw new InvalidDataException("Authored music names collide in the SAGE identity domain.");
        foreach (XmlElement owner in owners)
        {
            // Reborn: reject whitespace-normalized boolean spellings rather than silently changing their schema meaning.
            if (owner.HasAttribute("IsCacheable") && owner.GetAttribute("IsCacheable") is not ("true" or "false" or "1" or "0"))
                throw new InvalidDataException("Local cache boolean requires an exact true/false/1/0 literal.");
            if (owner.HasAttribute("RestartAlternateEvent") && !names.Contains(owner.GetAttribute("RestartAlternateEvent"),StringComparer.Ordinal))
                throw new InvalidDataException("Alternate must name an exact admitted local music owner.");
        }
        var binding = SdkEffectiveSchema.Bind(schemas!,sourcePath,source);
        if (!binding.XmlValidated || binding.Fields.Length != owners.Length || binding.Fields.Any(field => field.LogicalPath != "events.h"))
            throw new InvalidDataException("Authored music source failed reviewed schema/header field binding.");
        Row[] rows = owners.Select(owner =>
        {
            string name = owner.GetAttribute("id"),cache = owner.GetAttribute("IsCacheable");
            bool cacheable = cache.Length == 0 || cache is "true" or "1";
            string? alternate = owner.HasAttribute("RestartAlternateEvent") ? owner.GetAttribute("RestartAlternateEvent") : null;
            var prepared = PathMusicRuntimeProbe.FromHeader(header,name,alternate,cacheable); Chunk chunk = prepared.Chunk;
            return new Row(name,InstanceHandle.GetInstanceId(name),prepared.EventValue,alternate,cacheable,chunk.InstanceBuffer.Length,chunk.RelocationBuffer.Length,Hash(chunk.InstanceBuffer),Hash(chunk.RelocationBuffer));
        }).ToArray();
        if (!source.AsSpan().SequenceEqual(SdkEnvironmentPreflight.Read(sourcePath,32768)) || !header.AsSpan().SequenceEqual(SdkEnvironmentPreflight.Read(headerPath,1048576)))
            throw new InvalidDataException("Authored music changed during source/header preparation.");
        return new(directory,source,header,rows,schemas!);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fingerprint exact source/header/native bytes, never timestamps or claimed native processing identities. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
