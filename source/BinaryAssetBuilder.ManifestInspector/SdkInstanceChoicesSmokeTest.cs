using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit nested flat repeated-choice copying separately from singleton replacement or populated-branch matching.
internal static class SdkInstanceChoicesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify repeated alternatives, chained direct-source witnesses, removal composition and atomic choice/identity/cardinality refusals. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset' abstract='true'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='Rule'><xs:attribute name='id' type='xs:string'/><xs:attribute name='Label' type='xs:string' use='required'/><xs:attribute name='Filename' type='FileReference'/></xs:complexType><xs:complexType name='Heuristic'><xs:choice minOccurs='1' maxOccurs='3'><xs:element name='Map' type='Rule'/><xs:element name='Path' type='Rule'/></xs:choice></xs:complexType><xs:complexType name='Single'><xs:choice><xs:element name='Map' type='Rule'/><xs:element name='Path' type='Rule'/></xs:choice></xs:complexType><xs:complexType name='Structural'><xs:sequence><xs:choice><xs:element name='Map' type='Rule'/><xs:element name='Path' type='Rule'/></xs:choice></xs:sequence></xs:complexType><xs:complexType name='OptionalAlternative'><xs:choice maxOccurs='3'><xs:element name='Map' type='Rule' minOccurs='0'/><xs:element name='Path' type='Rule'/></xs:choice></xs:complexType><xs:complexType name='Move'><xs:sequence><xs:element name='Heuristic' type='Heuristic'/></xs:sequence><xs:attribute name='id' type='xs:string'/><xs:attribute name='Name' type='xs:string' use='required'/></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='OpeningMove' type='Move' minOccurs='0' maxOccurs='unbounded'/><xs:element name='Item' type='Rule' minOccurs='0' maxOccurs='4'/><xs:element name='Single' type='Single' minOccurs='0'/><xs:element name='Structural' type='Structural' minOccurs='0'/><xs:element name='OptionalAlternative' type='OptionalAlternative' minOccurs='0'/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Asset' type='Asset' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceChoices-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        File.WriteAllBytes(Path.Combine(data,"payload.bin"),new byte[] { 3 });
        string leaf = "<Asset id='Leaf'>"+Move("base","<Map Label='first' Filename='DATA:payload.bin'/><Map Label='second'/><Path Label='third'/>")+"<Item id='delete' Label='payload'/></Asset>";
        string middle = Include("Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'>"+Move("middle","<Path Label='middle'/>")+"</Asset>";
        string owner = Include("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'>"+Move("owner","<Map Label='owner'/>")+"<Item id='delete' i:joinAction='Remove'/></Asset>";
        var paths = Fixture(owner,middle,leaf); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,choices:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.ChoiceName && result.Evidence.PreparedSources.Length == 3 && result.Evidence.Removals.Length == 1,"Choice preparation closure/profile/removal witnesses failed.");
        XmlDocument xml = Parse(result.Bytes!); var asset = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.LocalName == "Asset");
        var moves = asset.ChildNodes.OfType<XmlElement>().ToArray();
        Require(moves.Length == 3 && moves.Select(move => move.GetAttribute("Name")).SequenceEqual(new[] { "base","middle","owner" }),"Anonymous repeated OpeningMove branches matched, reordered or failed removal composition.");
        var rules = moves[0].FirstChild!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(rules.Select(rule => rule.LocalName).SequenceEqual(new[] { "Map","Map","Path" }) && rules.Select(rule => rule.GetAttribute("Label")).SequenceEqual(new[] { "first","second","third" }),"Repeated alternative slots collapsed or reordered.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,removals:true).Apply(entry,raw).Bytes == null,"Older removal-only profile widened to choice copying.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceChoices:true);
        Require(graph.ScopedGraphComplete && graph.PreprocessingProfile == SdkInstanceInheritanceProfile.ChoiceName && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && graph.Documents.All(document => document.Dependencies.Length == 1) && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Choice final binding/dependency/source/output isolation failed.");
        foreach (string bad in new[] {
            Move("overflow","<Map Label='1'/><Map Label='2'/><Path Label='3'/><Path Label='4'/>"),
            Move("duplicate","<Map id='same' Label='1'/><Path id='same' Label='2'/>"),
            Move("duplicate","<Map id='same' Label='1'/><Map id='same' Label='2'/>"),
            Move("unsafe","<Map id='../unsafe' Label='1'/>"),Move("unknown","<Unknown Label='1'/>"),
            Move("expression","<Map Label='=NAME'/>"),Move("directive","<Map Label='1' i:joinAction='Remove'/>"),
            Move("prefix","<p:Map xmlns:p='uri:ea.com:eala:asset' Label='1'/>"),
            "<Single><Map Label='1'/></Single>","<Single><Map Label='1'/><Path Label='2'/></Single>",
            "<Structural><Map Label='1'/></Structural>","<OptionalAlternative><Map Label='1'/></OptionalAlternative>" })
            Reject(Fixture(Include("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'>"+bad+"</Asset>",middle,leaf));
        // Reborn: a matched populated sequence branch remains closed even when its descendants are safe repeated choices.
        Reject(Fixture(Include("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'><OpeningMove id='key' Name='owner'><Heuristic><Map Label='owner'/></Heuristic></OpeningMove></Asset>",Include("Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'/>","<Asset id='Leaf'><OpeningMove id='key' Name='base'><Heuristic><Map Label='base'/></Heuristic></OpeningMove></Asset>"));
        // Reborn: empty required choice payload is not trusted merely because no destructive copy occurs; final schema binding withholds all fields.
        paths = Fixture(Include("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'>"+Move("empty","")+"</Asset>",middle,leaf);
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceChoices:true).Documents.Single(document => document.SourcePath == entry);
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Empty choice bypassed final required cardinality.");
        // Reborn: freshly captured graphs cannot authorize changed child bytes or cyclic direct source closures.
        paths = Fixture(owner,middle,leaf); File.AppendAllText(Path.Combine(data,"Leaf.xml")," "); Reject(paths);
        Reject(Fixture(owner,Include("Entry.xml")+"<Asset id='Middle'/>",leaf));
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceChoices:true,instanceRemovals:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting choice/removal flags were admitted.");
        Console.WriteLine("SDK instance choices self-test: OK (nested unit repeated alternatives/order, aggregate cardinality, anonymous branch identity, direct chained witnesses/removals/root files, final no-fields binding, atomic duplicate/cross-QName/unsafe/unknown/expression/directive/prefix/singleton/structural/nonunit/matched-branch/stale/cyclic refusal and older-profile isolation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: write and capture only owned choice fixtures under an explicit diagnostic data root. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string entryBody,string middleBody,string leafBody)
        {
            File.WriteAllBytes(entry,Source(entryBody)); File.WriteAllBytes(Path.Combine(data,"Middle.xml"),Source(middleBody)); File.WriteAllBytes(Path.Combine(data,"Leaf.xml"),Source(leafBody));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refuse the entire source closure without leaking partial processed hashes, imported witnesses or removals. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var refused = new SdkInstanceInheritanceProfile(schemas,captured,choices:true).Apply(entry,File.ReadAllBytes(entry));
            Require(refused.Bytes == null && refused.Evidence.ProcessedSha256 == null && refused.Evidence.Overlays.Length == 0 && refused.Evidence.Removals.Length == 0 && refused.Evidence.ImportedBases.Length == 0 && refused.Evidence.PreparedSources.Length == 0,"Choice scope failure leaked partial closure evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct anonymous opening-move branches without promoting Name to an inheritance key. */
    //-------------------------------------------------------------------------------------------------
    private static string Move(string name,string rules) => "<OpeningMove Name='"+name+"'><Heuristic>"+rules+"</Heuristic></OpeningMove>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: name direct instance fixture sources without introducing transitive export authority. */
    //-------------------------------------------------------------------------------------------------
    private static string Include(string source) => "<Includes><Include type='instance' source='"+source+"'/></Includes>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned EA choice fixtures with the explicit removal namespace. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:i='uri:ea.com:eala:asset:instance'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect owned transformed XML without following external resources. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail choice admission regressions without claiming native binary or game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
