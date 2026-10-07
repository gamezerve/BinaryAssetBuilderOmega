using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize one-sided matched micromanager filter copying without enabling general recursive XML merging.
internal static class SdkInstanceFiltersSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove ordered duplicate leaves, inherited payload retention, atomic refusal, final schema validation and older-profile isolation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='WeakReference'><xs:restriction base='xs:string'/></xs:simpleType><xs:complexType name='BaseAssetType'><xs:attribute name='id' type='xs:string' use='required'/></xs:complexType><xs:complexType name='ObjectFilter'><xs:sequence><xs:element name='IncludeThing' type='WeakReference' minOccurs='0' maxOccurs='unbounded'/><xs:element name='ExcludeThing' type='WeakReference' minOccurs='0' maxOccurs='unbounded'/></xs:sequence><xs:attribute name='Rule' type='xs:string'/><xs:attribute name='Include' type='xs:string'/><xs:attribute name='Filename' type='FileReference'/><xs:attribute name='id' type='xs:string'/></xs:complexType><xs:complexType name='AIMicroManagerData'><xs:complexContent><xs:extension base='BaseAssetType'><xs:sequence><xs:element name='IgnoreTargets' type='ObjectFilter' minOccurs='0'/><xs:element name='OtherFilter' type='ObjectFilter' minOccurs='0'/></xs:sequence><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='AIMicroManagerData' type='AIMicroManagerData' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceFilters-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput");
        Directory.CreateDirectory(data); File.WriteAllBytes(Path.Combine(data,"payload.bin"),new byte[] { 7 });
        const string leaves = "<IncludeThing>Unit_A</IncludeThing><IncludeThing>Unit_A</IncludeThing><ExcludeThing>Unit_B</ExcludeThing>";
        string chain = "<AIMicroManagerData id='Base'><IgnoreTargets Include='A' Filename='DATA:payload.bin'/></AIMicroManagerData><AIMicroManagerData id='Middle' inheritFrom='Base'><IgnoreTargets Rule='ANY'>"+leaves+"</IgnoreTargets></AIMicroManagerData><AIMicroManagerData id='Owner' inheritFrom='Middle'><IgnoreTargets Rule='NONE'/></AIMicroManagerData>";
        var paths = Fixture(chain); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,filters:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.FilterName && result.Evidence.Filters.Length == 2 && result.Evidence.ConsumedMarkers.Length == 2,"Filter profile/copy/marker evidence failed: "+string.Join(";",result.Evidence.Diagnostics));
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var filter = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner").ChildNodes.OfType<XmlElement>().Single();
        Require(filter.GetAttribute("Rule") == "NONE" && filter.GetAttribute("Include") == "A" && filter.GetAttribute("Filename") == "DATA:payload.bin" && filter.InnerXml.Contains("Unit_A",StringComparison.Ordinal),"Matched filter attributes were not overlaid/retained.");
        var expected = new[] { new SdkFilterCopies.Leaf("IncludeThing","Unit_A"),new SdkFilterCopies.Leaf("IncludeThing","Unit_A"),new SdkFilterCopies.Leaf("ExcludeThing","Unit_B") };
        Require(SdkFilterCopies.Leaves(filter).SequenceEqual(expected) && result.Evidence.Filters.Single(item => item.DerivedId == "Middle").PayloadSource == "derived" && result.Evidence.Filters.Single(item => item.DerivedId == "Owner").PayloadSource == "base","Leaf order/multiplicity or source evidence changed.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,bitflags:true).Apply(entry,raw).Bytes == null,"Older bitflag profile admitted populated matched filters.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceFilters:true);
        Require(graph.ScopedGraphComplete && graph.Documents.Single().Dependencies.Length == 3 && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Final binding/source/output isolation failed.");
        // Reborn: two populated sides are refused even if their payloads are equal; a late failure must erase earlier copy evidence.
        Reject(Fixture(chain+"<AIMicroManagerData id='Bad' inheritFrom='Owner'><IgnoreTargets>"+leaves+"</IgnoreTargets></AIMicroManagerData>"));
        foreach (string payload in new[] { "<IncludeThing id='X'>Unit_A</IncludeThing>","<Unknown>Unit_A</Unknown>","<IncludeThing><IncludeThing>Unit_A</IncludeThing></IncludeThing>","<IncludeThing><![CDATA[Unit_A]]></IncludeThing>","<IncludeThing>=Unit_A</IncludeThing>","<IncludeThing/>","<IncludeThing>"+new string('A',129)+"</IncludeThing>",string.Concat(Enumerable.Repeat("<IncludeThing>Unit_A</IncludeThing>",129)),"<IncludeThing xmlns:p='uri:ea.com:eala:asset'><p:IncludeThing>Unit_A</p:IncludeThing></IncludeThing>" })
            Reject(Fixture("<AIMicroManagerData id='Base'><IgnoreTargets/></AIMicroManagerData><AIMicroManagerData id='Owner' inheritFrom='Base'><IgnoreTargets>"+payload+"</IgnoreTargets></AIMicroManagerData>"));
        Reject(Fixture(chain.Replace("IgnoreTargets","OtherFilter",StringComparison.Ordinal)));
        Reject(Fixture(chain.Replace("<IgnoreTargets Rule='NONE'/>","<IgnoreTargets Rule='NONE'/><IgnoreTargets/>",StringComparison.Ordinal)));
        paths = Fixture(chain.Replace("id='Base'","id='Base' Value='bad'",StringComparison.Ordinal));
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceFilters:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Filter copying bypassed final schema validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceFilters:true,instanceBitflags:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting filter/bitflag profiles admitted.");
        Console.WriteLine("SDK instance filters self-test: OK (one-sided payload copying, ordered duplicate weak references, attribute overlays, resolved local chains, older-profile isolation, atomic two-sided/unsafe/oversized/other-branch/late refusal, final no-fields/source/output checks)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture owned fixture sources without changing external SDK or game XML. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body)
        {
            File.WriteAllBytes(entry,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>"));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refuse partial filter or prior inheritance evidence when any asset fails the scope proof. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,filters:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Filters.Length == 0 && rejected.Evidence.Bitflags.Length == 0 && rejected.Evidence.ConsumedMarkers.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.Removals.Length == 0,"Filter refusal leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail when the diagnostic filter contract changes without claiming native asset or game equivalence. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
