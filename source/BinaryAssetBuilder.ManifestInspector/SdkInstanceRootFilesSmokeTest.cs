using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove explicit-root inherited fields independently from relative-path provenance and native file hash rewriting.
internal static class SdkInstanceRootFilesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise root-qualified attribute/element inheritance, consumer-authored overrides, missing/unsafe roots and earlier-profile isolation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset' abstract='true'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='Filename' type='FileReference' minOccurs='0'/></xs:sequence><xs:attribute name='Header' type='FileReference'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Asset' type='Asset' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceRootFiles-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),bases = Path.Combine(data,"Base"),consumer = Path.Combine(data,"Owner"),audio = Path.Combine(root,"Audio"),art = Path.Combine(root,"Art");
        foreach (string directory in new[] { bases,consumer,audio,Path.Combine(art,"AB") }) Directory.CreateDirectory(directory);
        string entry = Path.Combine(consumer,"Entry.xml"),basePath = Path.Combine(bases,"Bases.xml"),output = Path.Combine(root,"NoOutput");
        File.WriteAllBytes(Path.Combine(data,"source.bin"),new byte[] { 1,2,3 }); File.WriteAllBytes(Path.Combine(audio,"track.bin"),new byte[] { 4 });
        File.WriteAllBytes(Path.Combine(consumer,"relative.bin"),new byte[] { 5,6 }); File.WriteAllBytes(Path.Combine(bases,"relative.bin"),new byte[] { 7,8,9,10,11 }); File.WriteAllBytes(Path.Combine(art,"AB","AB.w3x"),new byte[] { 12 });
        const string include = "<Includes><Include type='instance' source='DATA:Base/Bases.xml'/></Includes>";
        const string baseBody = "<Asset id='Base' Header='DATA:source.bin'><Filename>AuDiO:track.bin</Filename></Asset>";
        var paths = Fixture(baseBody,include+"<Asset id='Child' inheritFrom='Base' Header='relative.bin'/>"); byte[] raw = File.ReadAllBytes(entry),baseRaw = File.ReadAllBytes(basePath);
        var result = new SdkInstanceInheritanceProfile(schemas.Schemas,paths,rootFiles:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.RootFileName && result.Evidence.ImportedBases.Single().RootQualifiedFields.Length == 2,"Root-qualified attribute/element inheritance failed.");
        XmlDocument xml = new(); xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!)); var child = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(element => element.LocalName == "Asset");
        Require(child.GetAttribute("Header") == "relative.bin" && child.InnerText == "AuDiO:track.bin","Root literal or consumer override was rewritten.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,instanceRootFiles:true); var owner = graph.Documents.Single(document => document.SourcePath == entry);
        Require(graph.ScopedGraphComplete && graph.PreprocessingProfile == SdkInstanceInheritanceProfile.RootFileName && owner.Dependencies.Single(field => field.Name == "Header").PhysicalPath == Path.Combine(consumer,"relative.bin") && owner.Dependencies.Single(field => field.Name == "Header").Bytes == 2 && owner.Dependencies.Single(field => field.Name == "Filename").PhysicalPath == Path.Combine(audio,"track.bin") && !graph.ProductionBuildReady && File.ReadAllBytes(entry).SequenceEqual(raw) && File.ReadAllBytes(basePath).SequenceEqual(baseRaw) && !Directory.Exists(output),"Root/consumer provenance, resource stat or source/output isolation differs.");
        Require(new SdkInstanceInheritanceProfile(schemas.Schemas,paths).Apply(entry,raw).Bytes == null && SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,instanceInheritance:true).Documents.Single(document => document.SourcePath == entry).Status == "RequiresPreprocessing","Original instance profile silently admitted imported file fields.");
        foreach (string value in new[] { "relative.bin","ROOT:source.bin","C:/source.bin" })
        {
            var invalid = Fixture("<Asset id='Base' Header='"+value+"'/>",include+"<Asset id='Child' inheritFrom='Base'/>");
            var rejected = new SdkInstanceInheritanceProfile(schemas.Schemas,invalid,rootFiles:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ProcessedSha256 == null,"Relative/unknown/native-absolute imported path admitted.");
        }
        // Reborn: root-qualified XML admission does not bypass lexical confinement or authorize external payload reads.
        foreach (string value in new[] { "DATA:../outside.bin","DATA:source.bin.","DATA:$MISSING","DATA:C:/outside.bin" })
        {
            var invalid = Fixture("<Asset id='Base' Header='"+value+"'/>",include+"<Asset id='Child' inheritFrom='Base'/>");
            var invalidGraph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,invalid,instanceRootFiles:true); var invalidOwner = invalidGraph.Documents.Single(document => document.SourcePath == entry);
            Require(invalidOwner.Status == "Validated" && invalidOwner.Dependencies.Single().Issue == "ResourcePath" && invalidOwner.Dependencies.Single().PhysicalPath == null && !invalidGraph.ScopedGraphComplete && !Directory.Exists(output),"Unsafe root-qualified field gained physical path authority.");
        }
        var artPaths = Fixture("<Asset id='Base' Header='ART:AB.w3x'/>",include+"<Asset id='Child' inheritFrom='Base'/>");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,artPaths,instanceRootFiles:true).Documents.Single(document => document.SourcePath == entry).Dependencies.Single().PhysicalPath == Path.Combine(art,"AB","AB.w3x"),"Inherited ART basename fanout differs.");
        var missing = Fixture(baseBody,include+"<Asset id='Child' inheritFrom='Base'/>",false);
        var missingGraph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,missing,instanceRootFiles:true); var missingOwner = missingGraph.Documents.Single(document => document.SourcePath == entry);
        Require(missingOwner.Status == "Validated" && missingOwner.Dependencies.Single(field => field.Name == "Filename").Issue == "ResourcePath" && !missingGraph.ScopedGraphComplete,"Missing AUDIO root became a trusted resource or blocked XML eligibility.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,instanceInheritance:true,instanceRootFiles:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting instance profiles accepted.");
        Console.WriteLine("SDK instance root files self-test: OK (DATA/ART/AUDIO attribute/element roots, ART fanout, consumer-relative override versus defining directory, pre-overlay witness fields, literal/raw/output isolation, old-profile and relative/unknown/absolute rejection, unsafe/missing-root no-authority and option conflicts; no native file rewriting)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: create only owned source graphs with explicit optional alias roots, never synced reference data. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string baseXml,string ownerXml,bool audioConfigured = true)
        { File.WriteAllBytes(basePath,Source(baseXml)); File.WriteAllBytes(entry,Source(ownerXml)); return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()),art,audioConfigured ? audio : null); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode bounded owned alias-root fixtures using the EA asset declaration namespace. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: surface alias-root regression failures without claiming payload hashes, native cache preparation or game loading. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
