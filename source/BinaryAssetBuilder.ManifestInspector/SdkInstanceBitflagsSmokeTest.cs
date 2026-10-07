using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently prove bounded whole-token enum-list operations while retaining the unchanged core's known substring behavior.
internal static class SdkInstanceBitflagsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test ordered add/remove/no-op chains, core substring hazards, explicit empty/missing bases, atomic refusal and final no-fields binding. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='KindOfType'><xs:restriction base='xs:string'><xs:enumeration value='ALPHA'/><xs:enumeration value='ALPHA_EXTRA'/><xs:enumeration value='BETA'/><xs:enumeration value='GAMMA'/></xs:restriction></xs:simpleType><xs:simpleType name='KindOfBitFlags'><xs:list itemType='KindOfType'/></xs:simpleType><xs:complexType name='BaseAssetType'><xs:attribute name='id' type='xs:string' use='required'/></xs:complexType><xs:complexType name='Leaf'><xs:attribute name='Flags' type='KindOfBitFlags'/></xs:complexType><xs:complexType name='AITargetingHeuristic'><xs:complexContent><xs:extension base='BaseAssetType'><xs:sequence><xs:element name='Leaf' type='Leaf' minOccurs='0'/></xs:sequence><xs:attribute name='VitalKindOf' type='KindOfBitFlags'/><xs:attribute name='ForbiddenKindOf' type='KindOfBitFlags'/><xs:attribute name='Filename' type='FileReference'/><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='AITargetingHeuristic' type='AITargetingHeuristic' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceBitflags-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        File.WriteAllBytes(Path.Combine(data,"payload.bin"),new byte[] { 5 });
        string chain = "<AITargetingHeuristic id='Base' VitalKindOf='ALPHA BETA' ForbiddenKindOf='' Filename='DATA:payload.bin'/><AITargetingHeuristic id='Middle' inheritFrom='Base' VitalKindOf='+GAMMA -BETA +GAMMA' ForbiddenKindOf='+BETA'/><AITargetingHeuristic id='Owner' inheritFrom='AITargetingHeuristic:Middle' VitalKindOf='-ALPHA +BETA'/>";
        var paths = Fixture(chain); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,bitflags:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.BitflagName && result.Evidence.Bitflags.Length == 3 && result.Evidence.Bitflags.Sum(item => item.Operations) == 6 && result.Evidence.ConsumedMarkers.Length == 2,"Bitflag profile/operation/marker evidence failed.");
        var owner = Parse(result.Bytes!).DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner");
        Require(SdkBitflagModifiers.Tokens(owner.GetAttribute("VitalKindOf")).SequenceEqual(new[] { "GAMMA","BETA" }) && SdkBitflagModifiers.Tokens(owner.GetAttribute("ForbiddenKindOf")).SequenceEqual(new[] { "BETA" }) && !owner.HasAttribute("inheritFrom"),"Ordered whole-token operations or consumed markers differ.");
        Require(result.Evidence.Bitflags.Single(item => item.DerivedId == "Owner").BaseId == "Middle" && result.Evidence.Bitflags.Single(item => item.DerivedId == "Owner").Before.Contains("GAMMA",StringComparison.Ordinal),"Modifier proof used unresolved or qualified base identity.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,markers:true).Apply(entry,raw).Bytes == null,"Marker-only admission widened to enum modifiers.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceBitflags:true);
        Require(graph.ScopedGraphComplete && graph.Documents.Single().Dependencies.Length == 3 && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Post-modifier file binding/source/output isolation failed.");
        // Reborn: core addition skips a distinct shorter token found inside a longer token; final schema validation alone would not detect this semantic loss.
        Require(Core("ALPHA_EXTRA","+ALPHA") == "ALPHA_EXTRA","Core substring-add characterization changed.");
        // Reborn: core removal can corrupt another longer token even if the requested exact token also exists.
        Require(Core("ALPHA ALPHA_EXTRA","-ALPHA") == " _EXTRA","Core substring-removal characterization changed.");
        foreach (var bad in new[] {
            ("ALPHA_EXTRA","+ALPHA"),("ALPHA_EXTRA","-ALPHA"),("ALPHA ALPHA_EXTRA","-ALPHA"),
            ("BETA","-ALPHA"),("ALPHA","-ALPHA -ALPHA"),("ALPHA","+UNKNOWN"),("ALPHA","+alpha"),
            ("ALPHA","+BETA GAMMA"),("ALPHA","+BETA  +GAMMA"),("ALPHA"," +BETA"),("ALPHA","+BETA "),
            ("ALPHA ALPHA","+BETA"),("UNKNOWN","+BETA"),("ALPHA","+"),("ALPHA","=FLAGS") })
            Reject(Fixture("<AITargetingHeuristic id='Base' VitalKindOf='"+bad.Item1+"'/><AITargetingHeuristic id='Owner' inheritFrom='Base' VitalKindOf='"+bad.Item2+"'/>"));
        Reject(Fixture("<AITargetingHeuristic id='Base'/><AITargetingHeuristic id='Owner' inheritFrom='Base' VitalKindOf='+ALPHA'/>"));
        Reject(Fixture("<AITargetingHeuristic id='Base' VitalKindOf='+ALPHA'/><AITargetingHeuristic id='Owner' inheritFrom='Base'/>"));
        Reject(Fixture("<AITargetingHeuristic id='Base' VitalKindOf='ALPHA'/><AITargetingHeuristic id='Owner' inheritFrom='Base'><Leaf Flags='+ALPHA'/></AITargetingHeuristic>"));
        // Reborn: a late failure after an earlier successful modifier must withhold all previously accumulated evidence.
        Reject(Fixture(chain+"<AITargetingHeuristic id='Bad' inheritFrom='Owner' VitalKindOf='-ALPHA'/>"));
        paths = Fixture(chain.Replace("Filename='DATA:payload.bin'","Filename='DATA:payload.bin' Value='bad'",StringComparison.Ordinal));
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceBitflags:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Modifier proof bypassed final payload schema binding.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceBitflags:true,instanceMarkers:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting bitflag/marker profiles admitted.");
        Console.WriteLine("SDK instance bitflags self-test: OK (ordered whole-token add/remove/no-op/local chains, explicit empty bases, core substring-add/removal hazards, atomic missing/unknown/duplicate/collision/mixed/whitespace/child/late failure rejection, owner evidence, consumed markers, final no-fields/source/output and older-profile isolation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: construct owned fixture sources under an explicit data root without editing external references. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body)
        {
            File.WriteAllBytes(entry,Source(body));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: pin unchanged core enum-list behavior using owned unvalidated nodes, not a replacement implementation. */
        //-------------------------------------------------------------------------------------------------
        string Core(string before,string modifiers)
        {
            var xml = Parse(Source("<AITargetingHeuristic id='Base' VitalKindOf='"+before+"'/><AITargetingHeuristic id='Owner' VitalKindOf='"+modifiers+"'/>"));
            var assets = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
            return ((XmlElement)NodeJoiner.Override(schemas,xml,assets[0],assets[1])).GetAttribute("VitalKindOf");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: enforce all-or-nothing modifier/marker/source evidence even if a previous asset was successfully joined. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,bitflags:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Bitflags.Length == 0 && rejected.Evidence.ConsumedMarkers.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.Removals.Length == 0,"Bitflag refusal leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned EA modifier fixtures without loading external DTDs or payloads. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect owned XML with external resolution disabled. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail on changed core/modifier contracts without asserting native stream or game equivalence. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
