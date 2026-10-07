using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove recursive preparation preserves direct-definition eligibility, explicit-root file provenance and bounded complete source witnesses.
internal static class SdkInstanceChainsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise child-first chains, diamonds, depth/cache/source bounds and atomic visibility/stale/type/merge failures without native compilation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset' abstract='true'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='Item'><xs:attribute name='id' type='xs:string'/><xs:attribute name='A' type='xs:int'/><xs:attribute name='B' type='xs:int'/><xs:attribute name='Filename' type='FileReference'/></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='Item' type='Item' minOccurs='0' maxOccurs='3'/><xs:element name='Tag' type='xs:string' minOccurs='0'/></xs:sequence><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='Non'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:choice minOccurs='0' maxOccurs='unbounded'><xs:element name='Asset' type='Asset'/><xs:element name='Non' type='Non'/></xs:choice></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceChains-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        File.WriteAllBytes(Path.Combine(data,"payload.bin"),new byte[] { 7 });
        var authored = new Dictionary<string,string> {
            ["Entry.xml"] = Includes("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'><Item id='x' A='3'/></Asset>",
            ["Middle.xml"] = Includes("Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'><Item id='x' A='2'/></Asset>",
            ["Leaf.xml"] = "<Asset id='Leaf' Value='7'><Item id='x' A='1' B='8' Filename='DATA:payload.bin'/></Asset>"
        };
        var paths = Fixture(authored); byte[] raw = File.ReadAllBytes(entry);
        var profile = new SdkInstanceInheritanceProfile(schemas,paths,chains:true); var result = profile.Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.ChainName && result.Evidence.Overlays.Length == 1 && result.Evidence.PreparedSources.Length == 3 && result.Evidence.ImportedBases.Single().PreparedSources.Length == 2,"Chain closure/overlay identity failed.");
        var owner = Parse(result.Bytes!).DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.LocalName == "Asset");
        Require(owner.GetAttribute("id") == "Owner" && owner.GetAttribute("Value") == "7" && owner.FirstChild!.Attributes!["A"]!.Value == "3" && owner.FirstChild.Attributes["B"]!.Value == "8","Child-first empty-node overlays or own-only export failed.");
        foreach (var witness in result.Evidence.PreparedSources)
            Require(witness.RawSha256 == Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(witness.SourcePath))),"Closure source identity differs.");
        Require(result.Evidence.PreparedSources.Single(source => source.SourcePath == entry).ProcessedSha256 == Convert.ToHexString(SHA256.HashData(result.Bytes!)),"Owner processed closure digest differs.");
        // Reborn: independently preparing the direct child must reproduce the digest published by its parent and per-base closure.
        string middlePath = Path.Combine(data,"Middle.xml");
        var middle = new SdkInstanceInheritanceProfile(schemas,paths,chains:true).Apply(middlePath,File.ReadAllBytes(middlePath));
        Require(middle.Bytes != null && middle.Evidence.ProcessedSha256 == result.Evidence.ImportedBases.Single().ProcessedSha256 && middle.Evidence.ProcessedSha256 == result.Evidence.PreparedSources.Single(source => source.SourcePath == middlePath).ProcessedSha256,"Direct-child processed witness is not independently reproducible.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceChains:true);
        Require(graph.ScopedGraphComplete && graph.Documents.Length == 3 && graph.Documents.All(document => document.Dependencies.Length == 1) && !graph.ProductionBuildReady && !Directory.Exists(output),"Chain graph/file-root/no-output evidence failed.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,rootFiles:true).Apply(entry,raw).Bytes == null,"Older direct-only profile gained chain preparation.");
        // Reborn: an owner cannot inherit a grandchild declaration without also directly including its defining source.
        var invalid = new Dictionary<string,string>(authored) { ["Entry.xml"] = Includes("Middle.xml")+"<Asset id='Owner' inheritFrom='Leaf'/>" }; Reject(Fixture(invalid));
        invalid["Entry.xml"] = Includes("Middle.xml","Leaf.xml")+"<Asset id='Owner' inheritFrom='Leaf'/>";
        Require(new SdkInstanceInheritanceProfile(schemas,Fixture(invalid),chains:true).Apply(entry,File.ReadAllBytes(entry)).Bytes != null,"An explicitly direct ancestor was incorrectly hidden.");
        // Reborn: a diamond coalesces shared preparation sources while each direct base witness retains only its own source closure.
        var diamond = new Dictionary<string,string>(authored) {
            ["Entry.xml"] = Includes("Middle.xml","Other.xml")+"<Asset id='Owner' inheritFrom='Middle'/><Asset id='Second' inheritFrom='Other'/>",
            ["Other.xml"] = Includes("Leaf.xml")+"<Asset id='Other' inheritFrom='Leaf'/>"
        };
        paths = Fixture(diamond); result = new SdkInstanceInheritanceProfile(schemas,paths,chains:true).Apply(entry,File.ReadAllBytes(entry));
        Require(result.Bytes != null && result.Evidence.PreparedSources.Length == 4 && result.Evidence.ImportedBases.All(witness => witness.PreparedSources.Length == 2),"Diamond closure was flattened, duplicated or contaminated by siblings.");
        // Reborn: reset raw/prepared caches between owner calls so editing a nested leaf cannot reuse an earlier valid preparation.
        paths = Fixture(authored); profile = new(schemas,paths,chains:true); Require(profile.Apply(entry,File.ReadAllBytes(entry)).Bytes != null,"Initial stale-cache probe failed.");
        File.WriteAllBytes(Path.Combine(data,"Leaf.xml"),Source(authored["Leaf.xml"]+"<!--changed-->")); Reject(paths,profile,"Stale instance source");
        foreach (var bad in new[] {
            new Dictionary<string,string>(authored) { ["Leaf.xml"] = Includes("Middle.xml")+authored["Leaf.xml"] },
            new Dictionary<string,string>(authored) { ["Middle.xml"] = authored["Middle.xml"].Replace("type='instance'","type='reference'",StringComparison.Ordinal) },
            new Dictionary<string,string>(authored) { ["Middle.xml"] = authored["Middle.xml"].Replace("type='instance'","type='all'",StringComparison.Ordinal) },
            new Dictionary<string,string>(authored) { ["Leaf.xml"] = authored["Leaf.xml"].Replace("DATA:payload.bin","payload.bin",StringComparison.Ordinal) },
            new Dictionary<string,string>(authored) { ["Middle.xml"] = authored["Middle.xml"].Replace("A='2'","A='=1+2'",StringComparison.Ordinal) },
            new Dictionary<string,string>(authored) { ["Middle.xml"] = Includes("Leaf.xml","Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'/>" },
            new Dictionary<string,string>(authored) { ["Leaf.xml"] = "<Non id='Leaf'/>", ["Middle.xml"] = Includes("Leaf.xml")+"<Non id='Middle' inheritFrom='Leaf'/>" },
            new Dictionary<string,string>(authored) { ["Leaf.xml"] = "<Asset id='Leaf'><Tag>left</Tag></Asset>", ["Middle.xml"] = Includes("Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'><Tag>right</Tag></Asset>" }
        }) Reject(Fixture(bad));
        paths = Fixture(authored); Reject(paths with { StoppedAtLimit = true });
        Reject(paths with { Includes = paths.Includes.Select(edge => edge.Document.EndsWith("Middle.xml",StringComparison.Ordinal) ? edge with { PhysicalPath = entry } : edge).ToArray() });
        // Reborn: final owner scalar validation remains separate from recursive import eligibility and exposes no trusted fields when invalid.
        invalid = new(authored) { ["Entry.xml"] = authored["Entry.xml"].Replace("A='3'","A='bad'",StringComparison.Ordinal) };
        graph = SdkTypedSourceGraph.BindGraph(schemas,Fixture(invalid),instanceChains:true);
        Require(graph.Documents.Single(document => document.SourcePath == entry).Status == "SchemaInvalid" && graph.Documents.Single(document => document.SourcePath == entry).Dependencies.Length == 0,"Invalid owner leaked trusted fields.");
        // Reborn: check exact 32-edge depth and cached semantic depth; path auditing's shared-source reuse does not authorize a deeper preparation path.
        var deep = new Dictionary<string,string> { ["Entry.xml"] = Includes("N1.xml")+"<Asset id='Owner' inheritFrom='N1'/>" };
        for (int index = 1; index <= 32; index++) deep["N"+index+".xml"] = index == 32 ? "<Asset id='N32' Value='7'/>" : Includes("N"+(index+1)+".xml")+"<Asset id='N"+index+"' inheritFrom='N"+(index+1)+"'/>";
        paths = Fixture(deep); result = new SdkInstanceInheritanceProfile(schemas,paths,chains:true).Apply(entry,File.ReadAllBytes(entry)); Require(result.Bytes != null && result.Evidence.PreparedSources.Length == 33,"Exact chain depth boundary failed.");
        deep["Entry.xml"] = Includes("N1.xml","Outer.xml")+"<Asset id='Owner' inheritFrom='N1'/>";
        deep["Outer.xml"] = Includes("N1.xml")+"<Asset id='Outer' inheritFrom='N1'/>"; paths = Fixture(deep); Require(!paths.StoppedAtLimit,"Memoized-depth fixture stopped during path audit."); Reject(paths,reason:"Cached instance preparation");
        // Reborn: shared raw input can amplify into many individually bounded prepared documents; charge all prepared bytes, not just captured raw bytes.
        var amplified = new Dictionary<string,string> { ["Leaf.xml"] = "<Asset id='Leaf'><Tag>"+new string('x',1048576)+"</Tag></Asset>" };
        for (int index = 0; index < 32; index++) amplified["B"+index+".xml"] = Includes("Leaf.xml")+"<Asset id='B"+index+"' inheritFrom='Leaf'/>";
        amplified["Entry.xml"] = Includes(Enumerable.Range(0,32).Select(index => "B"+index+".xml").ToArray())+"<Asset id='Owner' inheritFrom='B0'/>";
        paths = Fixture(amplified); Require(!paths.StoppedAtLimit,"Prepared amplification fixture exhausted raw source bounds."); Reject(paths,reason:"prepared instance closure bound");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceChains:true,instanceRootFiles:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting chain profiles admitted.");
        Console.WriteLine("SDK instance chains self-test: OK (child-first own-only export, direct-ancestor eligibility, empty-child overlays, explicit-root files, full per-base/owner closures and diamond coalescing, exact/cached depth, cycles/stale cache/forged edges/visibility/relative files/expressions/duplicate handles/text/type/prepared-byte rejection, invalid-owner no-fields and old-profile/output isolation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: snapshot only owned fixture files using the same bounded path audit as real diagnostic source graphs. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(Dictionary<string,string> files)
        {
            foreach (var pair in files) File.WriteAllBytes(Path.Combine(data,pair.Key),Source(pair.Value));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject an entire preparation transaction without leaking source closure witnesses from successfully prepared children. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report rejectedPaths,SdkInstanceInheritanceProfile? existing = null,string? reason = null)
        {
            var rejected = (existing ?? new(schemas,rejectedPaths,chains:true)).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.PreparedSources.Length == 0,"Rejected chain published partial evidence.");
            // Reborn: exact limit/cache probes must fail on their intended gate, not an unrelated earlier refusal.
            if (reason != null) Require(rejected.Evidence.Diagnostics.Any(diagnostic => diagnostic.Contains(reason,StringComparison.Ordinal)),"Chain rejected on an unexpected gate: "+string.Join("; ",rejected.Evidence.Diagnostics));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned instance edges in declaration order without filesystem discovery or reference edits. */
    //-------------------------------------------------------------------------------------------------
    private static string Includes(params string[] names) => "<Includes>"+string.Concat(names.Select(name => "<Include type='instance' source='"+name+"'/>"))+"</Includes>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode only owned EA source declarations for preparation regressions. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect owned diagnostic output without allowing external resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: surface chain-preparation failures without claiming native stream or game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
