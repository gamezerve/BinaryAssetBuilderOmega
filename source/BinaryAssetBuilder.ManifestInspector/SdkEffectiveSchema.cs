using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile only captured staged XSD bytes with external resolution disabled; enumerate effective inherited file attributes without building assets.
internal static class SdkEffectiveSchema
{
    // Reborn: effective attributes are not validated source instances, element-particle expansion or complete dependency closure.
    internal sealed record Field(string OwnerType,string Name,string FileType,string DeclarationUri,bool PipelineOnly);
    internal sealed record Evidence(XmlSchemaSet Schemas,int IncludedFiles,string[] Errors,bool Compiled);
    // Reborn: typed logical values are not resolved payloads or Include/dependency closure evidence.
    internal sealed record BoundField(string OwnerType,string Kind,string Name,string LogicalPath);
    internal sealed record Binding(string SourcePath,string SourceSha256,bool XmlValidated,BoundField[] Fields,string[] Errors);
    internal sealed record Report(string SchemaRoot,string SchemaCatalogSha256,int IncludedFiles,bool SchemaCompiled,Field[] EffectiveFileAttributes,string[] Errors,
        string? RequestedSource,Binding? SourceBinding,bool ReadOnly,bool SnapshotOnly,bool FullDependencyCoverage,bool ProductionBuildReady,string[] Limitations);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fingerprint the staged catalog and compile its CnC3Types Include closure without registry/settings/native compiler operations. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(string? source = null)
    {
        // Reborn: reject unsupported physical source syntax even when a schema error would otherwise skip source binding.
        source = source == null ? null : SdkEnvironmentPreflight.Absolute(source);
        string root = SdkEnvironmentPreflight.BaselineSchemaRoot(); var catalog = SdkEnvironmentPreflight.Catalog(root);
        Dictionary<string,byte[]> snapshots = new(StringComparer.OrdinalIgnoreCase);
        foreach (var row in catalog)
        {
            byte[] bytes = SdkEnvironmentPreflight.Read(Path.Combine(root,row.Key.Replace('/',Path.DirectorySeparatorChar)),2*1048576);
            if (Convert.ToHexString(SHA256.HashData(bytes)) != row.Value) throw new InvalidDataException("Schema changed during effective snapshot capture.");
            snapshots.Add(row.Key,bytes);
        }
        var evidence = Compile(snapshots,"cnc3types.xsd");
        var after = SdkEnvironmentPreflight.Catalog(root);
        if (after.Count != catalog.Count || catalog.Any(row => !after.TryGetValue(row.Key,out string? hash) || hash != row.Value)) throw new InvalidDataException("Effective schema catalog changed during inspection.");
        return new(root,SdkEnvironmentPreflight.Digest(catalog),evidence.IncludedFiles,evidence.Compiled,
            evidence.Compiled ? Attributes(evidence.Schemas) : Array.Empty<Field>(),evidence.Errors,
            source,evidence.Compiled && source != null ? Bind(evidence.Schemas,source,SdkEnvironmentPreflight.Read(source,4*1048576)) : null,true,true,false,false,new[] {
                "Source binding is skipped when schema compilation fails; effective attributes then remain empty, not complete.",
                "Effective global named-type attributes and one validated source only; no Include traversal, payload resolution, asset inheritance or full dependency closure.",
                "Diagnostic declaration URIs identify in-memory schema provenance, not physical files opened at those URIs.",
                "Captured bytes/consecutive catalog checks are snapshot-only; no atomic filesystem or EA AllTypesHash/game-load proof." });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: follow bounded relative XSD Includes in memory, flatten exactly once like core ReadSchema, and fail closed on imports/redefines or compiler diagnostics. */
    //-------------------------------------------------------------------------------------------------
    internal static Evidence Compile(IReadOnlyDictionary<string,byte[]> snapshots,string entry)
    {
        // Reborn: the entry has the same captured-relative namespace rules as its Include children.
        entry = IncludeName("",entry);
        if (snapshots.Count > 1024 || snapshots.Values.Any(bytes => bytes.Length > 2*1048576) || snapshots.Values.Sum(bytes => (long)bytes.Length) > 64*1048576)
            throw new InvalidDataException("Effective schema snapshot bounds exceeded.");
        XmlSchemaSet set = new() { XmlResolver = null }; List<string> errors = new(); HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
        set.ValidationEventHandler += (_,args) => Error(args.Message);
        Visit(entry,0);
        if (errors.Count == 0) { try { set.Compile(); } catch (XmlSchemaException error) { Error(error.Message); } }
        return new(set,visited.Count,errors.ToArray(),errors.Count == 0 && set.IsCompiled);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain at most 64 bounded diagnostics; any warning or error closes effective-schema readiness. */
        //-------------------------------------------------------------------------------------------------
        void Error(string message) { if (errors.Count < 64) errors.Add(message.Length > 1024 ? message[..1024] : message); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: parse captured Include documents once, never resolve a schemaLocation URI through the network or filesystem. */
        //-------------------------------------------------------------------------------------------------
        void Visit(string name,int depth)
        {
            if (depth > 32) throw new InvalidDataException("Effective schema Include depth exceeded.");
            if (!visited.Add(name)) return;
            if (!snapshots.TryGetValue(name,out byte[]? bytes)) throw new InvalidDataException("Uncaptured schema Include: "+name);
            using MemoryStream input = new(bytes,false);
            // Reborn: this URI supplies diagnostic provenance only; the resolver remains null and never opens it.
            string uri = "file:///C:/Reborn-InMemorySchemas/"+string.Join('/',name.Split('/').Select(Uri.EscapeDataString));
            using XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 2*1048576 },uri);
            XmlSchema schema = XmlSchema.Read(reader,(_,args) => Error(args.Message)) ?? throw new InvalidDataException("XSD document required.");
            foreach (XmlSchemaObject item in schema.Includes)
            {
                if (item is not XmlSchemaInclude include || string.IsNullOrEmpty(include.SchemaLocation)) throw new InvalidDataException("Only explicit captured XSD Includes are admitted; imports/redefines reject.");
                Visit(IncludeName(name,include.SchemaLocation),depth+1);
            }
            schema.Includes.Clear();
            try { set.Add(schema); } catch (XmlSchemaException error) { Error(error.Message); }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: enumerate compiled AttributeUses so inherited/group attributes and prohibited restrictions follow .NET XSD semantics rather than name guesses. */
    //-------------------------------------------------------------------------------------------------
    internal static Field[] Attributes(XmlSchemaSet set)
    {
        if (!set.IsCompiled) throw new InvalidDataException("Compiled effective schema required.");
        var file = set.GlobalTypes[new XmlQualifiedName("FileReference","uri:ea.com:eala:asset")] as XmlSchemaType ?? throw new InvalidDataException("EA FileReference type required.");
        List<Field> fields = new();
        foreach (XmlSchemaComplexType type in set.GlobalTypes.Values.OfType<XmlSchemaComplexType>().OrderBy(type => type.QualifiedName.ToString(),StringComparer.Ordinal))
            foreach (XmlSchemaAttribute attribute in type.AttributeUses.Values.OfType<XmlSchemaAttribute>().OrderBy(attribute => attribute.QualifiedName.ToString(),StringComparer.Ordinal))
                if (attribute.Use != XmlSchemaUse.Prohibited && attribute.AttributeSchemaType != null && XmlSchemaType.IsDerivedFrom(attribute.AttributeSchemaType,file,XmlSchemaDerivationMethod.None))
                {
                    if (fields.Count >= 4096) throw new InvalidDataException("Effective attribute bound exceeded.");
                    fields.Add(new(type.QualifiedName.ToString(),attribute.QualifiedName.ToString(),attribute.AttributeSchemaType.QualifiedName.ToString(),attribute.SourceUri ?? "",
                        attribute.UnhandledAttributes?.Any(value => value.LocalName == "pipelineOnly" && value.NamespaceURI == "uri:ea.com:eala:asset:schema" && value.Value is "true" or "1") == true));
                }
        return fields.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate one captured XML with the compiled schema, then select annotated file types including inherited attributes and xsi:type; never follow Includes or read payloads. */
    //-------------------------------------------------------------------------------------------------
    internal static Binding Bind(XmlSchemaSet set,string source,byte[] bytes)
    {
        if (!set.IsCompiled || bytes.Length > 4*1048576) throw new InvalidDataException("Compiled schema and bounded source bytes required.");
        XmlDocument xml = new() { XmlResolver = null,Schemas = set }; List<string> errors = new();
        using MemoryStream input = new(bytes,false); using XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 }); xml.Load(reader);
        // Reborn: one-source admission follows the same EA declaration boundary as the source-path profile.
        if (xml.DocumentElement?.LocalName != "AssetDeclaration" || xml.DocumentElement.NamespaceURI != "uri:ea.com:eala:asset") throw new InvalidDataException("EA AssetDeclaration required for typed source binding.");
        // Reborn: schema hints/Includes are not resolver authority; XmlDocument validates against only the supplied compiled set.
        xml.Validate((_,args) => { if (errors.Count < 64) errors.Add(args.Message.Length > 1024 ? args.Message[..1024] : args.Message); });
        List<BoundField> fields = new();
        var file = set.GlobalTypes[new XmlQualifiedName("FileReference","uri:ea.com:eala:asset")] as XmlSchemaType ?? throw new InvalidDataException("EA FileReference type required.");
        if (errors.Count == 0)
            foreach (XmlElement element in xml.SelectNodes("//*")!)
            {
                Select(element,element.SchemaInfo.SchemaType,element.ParentNode as XmlElement,"element");
                foreach (XmlAttribute attribute in element.Attributes) Select(attribute,attribute.SchemaInfo.SchemaType,element,"attribute");
            }
        return new(source,Convert.ToHexString(SHA256.HashData(bytes)),errors.Count == 0,errors.Count == 0 ? fields.ToArray() : Array.Empty<BoundField>(),errors.ToArray());

        //-------------------------------------------------------------------------------------------------
        /** Reborn: fail closed on oversized typed values/inventories instead of returning a partial dependency selection as validated. */
        //-------------------------------------------------------------------------------------------------
        void Select(XmlNode node,XmlSchemaType? type,XmlElement? owner,string kind)
        {
            if (errors.Count != 0 || type == null || !XmlSchemaType.IsDerivedFrom(type,file,XmlSchemaDerivationMethod.None)) return;
            string value = node is XmlAttribute attribute ? attribute.Value : node.InnerText;
            if (fields.Count >= 2048 || value.Length > 512) { errors.Add("Typed source file-field count/value bound exceeded."); return; }
            fields.Add(new(owner?.SchemaInfo.SchemaType?.QualifiedName.ToString() ?? "(anonymous)",kind,node.Name,value));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: canonicalize only bounded relative Include segments within the captured schema namespace, not arbitrary URI or device paths. */
    //-------------------------------------------------------------------------------------------------
    private static string IncludeName(string parent,string logical)
    {
        if (logical.Length is < 1 or > 512 || logical != logical.Trim() || logical.StartsWith('/') || logical.StartsWith('\\') || logical.Any(value => char.IsControl(value) || ":;$%*?".Contains(value))) throw new InvalidDataException("Unsafe schema Include literal.");
        List<string> parts = parent.Split('/').SkipLast(1).ToList();
        foreach (string part in logical.Replace('\\','/').Split('/'))
        {
            if (part == ".") continue;
            if (part == "..") { if (parts.Count == 0) throw new InvalidDataException("Schema Include escaped snapshot root."); parts.RemoveAt(parts.Count-1); continue; }
            if (part.Length == 0 || part.EndsWith('.') || part.EndsWith(' ')) throw new InvalidDataException("Unsafe schema Include segment."); parts.Add(part);
        }
        string result = string.Join('/',parts).ToLowerInvariant();
        if (!result.EndsWith(".xsd",StringComparison.Ordinal)) throw new InvalidDataException("XSD Include required.");
        return result;
    }
}
