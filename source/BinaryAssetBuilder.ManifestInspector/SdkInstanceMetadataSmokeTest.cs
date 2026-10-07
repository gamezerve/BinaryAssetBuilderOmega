using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove source-backed definition-only all Includes without importing asset handles or admitting inherited expressions.
internal static class SdkInstanceMetadataSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test bounded reviewed definition leaves, unchanged older refusal, hidden exports/directives/stale edges and atomic evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Tags' minOccurs='0'><xs:complexType/></xs:element><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Defines' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Define' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='name' type='xs:string'/><xs:attribute name='value' type='xs:string'/><xs:attribute name='override' type='xs:boolean'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Asset' type='Asset' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceMetadata-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),leaf = Path.Combine(data,"Definitions.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string edge = "<Include type='all' source='DATA:Definitions.xml'/>";
        const string body = "<Includes>"+edge+"</Includes><Asset id='Base' Value='7'/><Asset id='Owner' inheritFrom='Base'/>";
        const string definitions = "<Tags/><Includes/><Defines><Define name='A' value='7'/><Define name='B' value='=$A'/></Defines>";
        var paths = Fixture(body,definitions); byte[] raw = File.ReadAllBytes(entry),leafRaw = File.ReadAllBytes(leaf);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,metadata:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.MetadataName && result.Evidence.MetadataDefinitionIncludes.Length == 1 && result.Evidence.MetadataDefinitionIncludes.Single().Definitions == 2 && result.Evidence.MetadataDefinitionIncludes.Single().AssetExports == 0 && result.Evidence.ImportedBases.Length == 0 && result.Evidence.PreparedSources.Length == 2,"Metadata proof/closure/export evidence failed: "+string.Join(";",result.Evidence.Diagnostics));
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceMetadata:true);
        Require(graph.ScopedGraphComplete && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && File.ReadAllBytes(leaf).SequenceEqual(leafRaw) && !Directory.Exists(output),"Metadata final binding/source/output isolation failed.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,upgrades:true).Apply(entry,raw).Bytes == null,"Older upgrade profile acquired all-Include admission.");
        // Reborn: a direct instance child may consume its own metadata leaf, but its parent must not replay the child's Include event as its own.
        File.WriteAllBytes(Path.Combine(data,"Middle.xml"),Source(body.Replace("id='Owner'","id='Middle'",StringComparison.Ordinal)));
        var chainedPaths = Fixture("<Includes><Include type='instance' source='DATA:Middle.xml'/></Includes><Asset id='Owner' inheritFrom='Middle'/>",definitions);
        var chained = new SdkInstanceInheritanceProfile(schemas,chainedPaths,metadata:true).Apply(entry,File.ReadAllBytes(entry));
        Require(chained.Bytes != null && chained.Evidence.PreparedSources.Length == 3 && chained.Evidence.ImportedBases.Single().PreparedSources.Length == 2 && chained.Evidence.MetadataDefinitionIncludes.Length == 0,"Nested metadata closure was lost or replayed as parent Include authority.");
        foreach (string bad in new[] {
            definitions+"<Asset id='Hidden' Value='9'/>",
            definitions.Replace("<Includes/>","<Includes><Include type='instance' source='DATA:Entry.xml'/></Includes>",StringComparison.Ordinal),
            definitions.Replace("<Tags/>","<Tags><Asset id='Hidden'/></Tags>",StringComparison.Ordinal),
            definitions.Replace("value='=$A'","value='=$FORWARD'",StringComparison.Ordinal),
            definitions.Replace("name='B'","name='A'",StringComparison.Ordinal),
            definitions.Replace("value='=$A'","value='=1+2'",StringComparison.Ordinal),
            definitions.Replace("name='A'","name='A' override='true'",StringComparison.Ordinal),
            definitions.Replace("<Tags/>","<Tags><?unsafe instruction?></Tags>",StringComparison.Ordinal),
            "<Defines/>",
            "<Defines>"+string.Concat(Enumerable.Range(0,513).Select(index => "<Define name='N"+index+"' value='7'/>"))+"</Defines>" })
            Reject(Fixture(body,bad));
        Reject(Fixture(body.Replace("type='all'","type='reference'",StringComparison.Ordinal),definitions));
        Reject(Fixture(body.Replace(edge,edge+edge,StringComparison.Ordinal),definitions));
        Reject(Fixture(body.Replace(edge,edge+edge.Replace("type='all'","type='instance'",StringComparison.Ordinal),StringComparison.Ordinal),definitions));
        // Reborn: definition-only leaves provide no asset handles even if a caller attempts to inherit a definition name.
        Reject(Fixture(body.Replace("inheritFrom='Base'","inheritFrom='A'",StringComparison.Ordinal),definitions));
        // Reborn: the new scope intentionally stops at inherited expression substitution, rather than guessing pipeline order.
        Reject(Fixture(body.Replace("Value='7'","Value='=$A'",StringComparison.Ordinal),definitions));
        paths = Fixture(body,definitions); File.WriteAllBytes(leaf,Source(definitions+"<!--stale-->")); Reject(paths);
        paths = Fixture(body,definitions);
        Reject(paths with { Includes = paths.Includes.Select(item => item with { PhysicalPath = entry }).ToArray() });
        Reject(paths with { StoppedAtLimit = true });
        var invalidPaths = Fixture(body.Replace("Value='7'","Value='bad'",StringComparison.Ordinal),definitions);
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceMetadata:true).Documents.Single(document => document.SourcePath == entry);
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Metadata proof bypassed final owner schema validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceMetadata:true,instanceUpgrades:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting metadata/upgrade profiles admitted.");
        Console.WriteLine("SDK instance metadata self-test: OK (reviewed definition leaf/closure/zero exports, unchanged older profile, hidden assets/nested Includes/Tags/directives/duplicate/forward/override/unsupported/oversized/stale/forged/role rejection, inherited expressions remain closed, final no-fields/source/output isolation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture owned metadata leaf fixtures without touching external game sources. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string owner,string metadata)
        {
            File.WriteAllBytes(entry,Source(owner)); File.WriteAllBytes(leaf,Source(metadata));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: no metadata/import/prepared/operation witness may survive an unsuccessful owner preparation. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,metadata:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.MetadataDefinitionIncludes.Length == 0 && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ConsumedMarkers.Length == 0 && rejected.Evidence.UpgradeNormalizations.Length == 0,"Metadata failure leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned definition-only Include witnesses without DTD or external source mutation. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail on widened metadata authority without asserting native/game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
