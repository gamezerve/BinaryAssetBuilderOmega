using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove direct-instance base eligibility, snapshot identity and copy-only behavior without production output or imported file-path guessing.
internal static class SdkInstanceInheritanceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise imported/local chains, atomic visibility/type/provenance failures and old-profile/source/output isolation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset' abstract='true'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='Filename' type='FileReference' minOccurs='0'/><xs:element name='Tag' type='xs:string' minOccurs='0'/></xs:sequence><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='Non'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/><xs:attribute name='Value' type='xs:int'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:choice minOccurs='0' maxOccurs='unbounded'><xs:element name='Asset' type='Asset'/><xs:element name='Non' type='Non'/></xs:choice></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceInheritance-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"),basePath = Path.Combine(data,"Bases.xml"),otherPath = Path.Combine(data,"Other.xml");
        const string bases = "<Asset id='Root' Value='7'/><Asset id='Base' inheritFrom='Root'/>";
        const string includes = "<Includes><Include type='instance' source='Bases.xml'/></Includes>";
        const string owner = "<Asset id='Child' inheritFrom='Base'><Filename>local.bin</Filename></Asset><Asset id='Grandchild' inheritFrom='Child'/>";
        File.WriteAllBytes(Path.Combine(data,"local.bin"),new byte[] { 7 }); File.WriteAllBytes(otherPath,Source("<Asset id='Other'/>") );
        var paths = Fixture(bases,includes+owner); byte[] raw = File.ReadAllBytes(entry),baseRaw = File.ReadAllBytes(basePath);
        var result = new SdkInstanceInheritanceProfile(schemas.Schemas,paths).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.Name && result.Evidence.RawSha256 == Convert.ToHexString(SHA256.HashData(raw)) && result.Evidence.ProcessedSha256 == Convert.ToHexString(SHA256.HashData(result.Bytes)) && result.Evidence.Overlays.Length == 2 && result.Evidence.ImportedBases.Length == 1,"Imported chain/evidence failed.");
        var witness = result.Evidence.ImportedBases.Single();
        Require(witness.SourcePath == basePath && witness.BaseId == "Base" && witness.RawSha256 == Convert.ToHexString(SHA256.HashData(baseRaw)) && witness.ProcessedSha256 != witness.RawSha256,"Imported source identity differs.");
        XmlDocument xml = new(); xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!)); var assets = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName == "Asset").ToArray();
        Require(assets.Select(asset => asset.GetAttribute("id")).SequenceEqual(new[] { "Child","Grandchild" }) && assets.All(asset => asset.GetAttribute("Value") == "7") && SdkEffectiveSchema.Bind(schemas.Schemas,entry,result.Bytes!).XmlValidated,"Injected bases leaked or expanded local base attributes were lost.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,instanceInheritance:true);
        Require(graph.ScopedGraphComplete && graph.PreprocessingProfile == SdkInstanceInheritanceProfile.Name && graph.Documents.Single(document => document.SourcePath == entry).Dependencies.Length == 2 && !graph.ProductionBuildReady && File.ReadAllBytes(entry).SequenceEqual(raw) && File.ReadAllBytes(basePath).SequenceEqual(baseRaw) && !Directory.Exists(Path.Combine(root,"NoOutput")),"Imported graph/source/output isolation differs.");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfTreeCopy:true).Documents.Single(document => document.SourcePath == entry).Status == "RequiresPreprocessing","Local-only graph silently gained imports.");
        foreach (var invalid in new[] {
            (bases,includes.Replace("instance","all",StringComparison.Ordinal)+owner),
            (bases,includes.Replace("instance","reference",StringComparison.Ordinal)+owner),
            ("<Includes><Include type='instance' source='Other.xml'/></Includes>"+bases,includes+owner),
            ("<Non id='Base' Value='7'/>",includes+"<Non id='Child' inheritFrom='Base'/>"),
            ("<Asset id='Base'><Filename>local.bin</Filename></Asset>",includes+"<Asset id='Child' inheritFrom='Base'/>"),
            ("<Asset id='Base'><Tag>A</Tag></Asset>",includes+"<Asset id='Child' inheritFrom='Base'><Tag>B</Tag></Asset>"),
            ("<Asset id='Base' Value='bad'/>",includes+owner),
            (bases,includes+"<Asset id='Base' inheritFrom='Base'/>"),
            (bases,includes+"<Asset id='Child' inheritFrom='Missing'/>"),
            (bases,includes+"<Asset id='Child' inheritFrom='Non:Base'/>"),
            (bases,includes+"<Asset id='Child' inheritFrom='Base' Value='=1+2'/>"),
            (bases,includes.Replace("</Includes>","<Include type='instance' source='Bases.xml'/></Includes>",StringComparison.Ordinal)+owner) })
        {
            var invalidPaths = Fixture(invalid.Item1,invalid.Item2);
            var rejected = new SdkInstanceInheritanceProfile(schemas.Schemas,invalidPaths).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ImportedBases.Length == 0,"Rejected import published partial evidence.");
        }
        // Reborn: different direct source documents with the same handle cannot silently choose the first origin.
        File.WriteAllBytes(otherPath,Source("<Asset id='Base' Value='3'/>") );
        var duplicatePaths = Fixture(bases,includes.Replace("</Includes>","<Include type='instance' source='Other.xml'/></Includes>",StringComparison.Ordinal)+owner);
        Require(new SdkInstanceInheritanceProfile(schemas.Schemas,duplicatePaths).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Duplicate imported origin accepted.");
        // Reborn: amplification after many otherwise eligible imported overlays still withholds every partial witness/processed digest.
        var amplifiedPaths = Fixture("<Asset id='Base'><Tag>"+new string('x',70000)+"</Tag></Asset>",includes+string.Concat(Enumerable.Range(0,70).Select(index => "<Asset id='D"+index+"' inheritFrom='Base'/>")));
        var amplified = new SdkInstanceInheritanceProfile(schemas.Schemas,amplifiedPaths).Apply(entry,File.ReadAllBytes(entry));
        Require(amplified.Bytes == null && amplified.Evidence.ImportedBases.Length == 0 && amplified.Evidence.Overlays.Length == 0,"Amplified import published partial evidence.");
        // Reborn: owner scalar validation remains a separate final gate; admitted imported witnesses cannot make invalid resource fields trusted.
        var invalidOwnerPaths = Fixture(bases,includes+"<Asset id='Child' inheritFrom='Base' Value='bad'><Filename>local.bin</Filename></Asset>");
        var invalidOwner = SdkTypedSourceGraph.BindGraph(schemas.Schemas,invalidOwnerPaths,instanceInheritance:true).Documents.Single(document => document.SourcePath == entry);
        Require(invalidOwner.Status == "SchemaInvalid" && invalidOwner.Dependencies.Length == 0,"Invalid imported owner leaked typed fields.");
        // Reborn: local handle priority is preserved, without trusting or reading an unneeded foreign base document.
        var localPaths = Fixture("<Asset id='Base' Value='9'/>",includes+"<Asset id='Base' Value='4'/><Asset id='Child' inheritFrom='Base'/>");
        var local = new SdkInstanceInheritanceProfile(schemas.Schemas,localPaths).Apply(entry,File.ReadAllBytes(entry));
        Require(local.Bytes != null && local.Evidence.ImportedBases.Length == 0,"Local priority probe failed.");
        XmlDocument localXml = new(); localXml.LoadXml(Encoding.UTF8.GetString(local.Bytes!));
        Require(localXml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Child").GetAttribute("Value") == "4","Self visibility did not precede instance visibility.");
        var stalePaths = Fixture(bases,includes+owner); File.WriteAllBytes(basePath,Source("<Asset id='Base' Value='8'/>") );
        Require(new SdkInstanceInheritanceProfile(schemas.Schemas,stalePaths).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Stale imported bytes accepted.");
        paths = Fixture(bases,includes+owner);
        // Reborn: failed inventory/edge completeness must not authorize a new import path or emit any imported witness evidence.
        var wrongEdge = paths with { Includes = paths.Includes.Select(edge => edge with { PhysicalPath = otherPath }).ToArray() };
        Require(new SdkInstanceInheritanceProfile(schemas.Schemas,wrongEdge).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Forged instance edge accepted.");
        Require(new SdkInstanceInheritanceProfile(schemas.Schemas,paths with { StoppedAtLimit = true }).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Incomplete instance graph accepted.");
        Require(new SdkInstanceInheritanceProfile(schemas.Schemas,paths).Apply(entry,Source(includes+owner+"<!--changed-->" )).Bytes == null,"Stale owner source accepted.");
        var forged = paths with { Sources = paths.Sources.Select(source => source.PhysicalPath == basePath ? source with { PhysicalPath = Path.Combine(data,"..","Bases.xml") } : source).ToArray() };
        bool confinement = false; try { _ = new SdkInstanceInheritanceProfile(schemas.Schemas,forged); } catch (InvalidDataException) { confinement = true; } Require(confinement,"Forged inventory path accepted.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfTreeCopy:true,instanceInheritance:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting instance profile flags accepted.");
        var capped = Fixture(bases,"<Includes>"+string.Concat(Enumerable.Repeat("<Include type='instance' source='Bases.xml'/>",65))+"</Includes>"+owner);
        Require(new SdkInstanceInheritanceProfile(schemas.Schemas,capped).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"64 direct-instance Include cap bypassed.");
        Console.WriteLine("SDK instance inheritance self-test: OK (direct instance/source-local and owner-local chains, type eligibility, witness identity, injected-base removal, Self priority, atomic all/reference/transitive/non-inheritable/file-provenance/both-sided/invalid/same-handle/cross-type/duplicate/expression/stale rejection, confined inventory and old-profile/resource/source/output isolation; no production build)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture fresh owned source graphs for each imported-visibility probe; no synced reference files are touched. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string baseBody,string ownerBody)
        { File.WriteAllBytes(basePath,Source(baseBody)); File.WriteAllBytes(entry,Source(ownerBody)); return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>())); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode only owned source-backed instance fixtures with the EA declaration namespace. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail imported inheritance regressions explicitly without equating diagnostic XML with native/game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
