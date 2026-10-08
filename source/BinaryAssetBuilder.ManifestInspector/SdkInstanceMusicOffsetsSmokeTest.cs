using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test music-only arithmetic independently of the unavailable EA evaluator, native emission and large audio tree admission.
internal static class SdkInstanceMusicOffsetsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin actual pre-overlay values, source contexts, boundaries, atomic refusals and unchanged older profiles. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Percentage'><xs:restriction base='xs:string'><xs:pattern value='[0-9]{1,3}'/></xs:restriction></xs:simpleType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='MusicTrack'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:attribute name='Volume' type='Percentage'/><xs:attribute name='A' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Defines' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Define' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='name' type='xs:string'/><xs:attribute name='value' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='MusicTrack' type='MusicTrack' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-MusicOffsets-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),leaf = Path.Combine(data,"Definitions.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string defines = "<Defines><Define name='VOL' value='70'/></Defines>";
        const string includes = "<Includes><Include type='all' source='DATA:Definitions.xml'/></Includes>";
        const string assets = "<MusicTrack id='Base' Volume='55'/><MusicTrack id='Owner' inheritFrom='Base' Volume='=$VOL + 5'/>";
        var paths = Fixture(includes+assets,defines); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,musicOffsets:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.MusicOffsetName && result.Evidence.ExpressionPreparation?.MusicVolumeOffsets.Single() is { AssetId:"Owner",Definition:"VOL",BaseValue:"70",Operator:"+",Offset:"5",Result:"75" }
            && result.Evidence.ExpressionPreparation.Substitutions == 1 && result.Evidence.ExpressionPreparation.DefinitionSources.Length == 2,"Music arithmetic/context evidence failed: "+string.Join("; ",result.Evidence.Diagnostics));
        Require(Volume(result.Bytes!,"Owner") == "75","Core inherited Volume differs from the independently calculated integer result.");
        // Reborn: reject tampered post-Core values and missing owners instead of trusting expression-stage evidence alone.
        foreach (string altered in new[] { Encoding.UTF8.GetString(result.Bytes!).Replace("Volume=\"75\"","Volume=\"74\"",StringComparison.Ordinal),Encoding.UTF8.GetString(result.Bytes!).Replace("id=\"Owner\"","id=\"Other\"",StringComparison.Ordinal) })
        {
            XmlDocument changed = new() { XmlResolver = null }; changed.LoadXml(altered); bool refused = false;
            try { SdkMusicVolumeOffsets.Verify(changed,result.Evidence.ExpressionPreparation!.MusicVolumeOffsets); } catch (InvalidDataException) { refused = true; }
            Require(refused,"Altered post-Core music owner bypassed verification.");
        }
        Require(new SdkInstanceInheritanceProfile(schemas,paths,crossStateRemovals:true).Apply(entry,raw).Bytes == null && new SdkIncludeDefineProfile(paths,true).Apply(entry,raw,beforeInheritance:true).Bytes == null,"Earlier profiles gained music offsets.");
        Require(SdkTypedSourceGraph.BindGraph(schemas,paths,instanceMusicOffsets:true).ScopedGraphComplete && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Final music source/schema/output isolation failed.");
        foreach (var pair in new[] { ("=$VOL - 5","65"),("=$VOL + 0","70"),("=$VOL + 30","100"),("=$VOL - 70","0") })
        {
            paths = Fixture(defines+assets.Replace("=$VOL + 5",pair.Item1,StringComparison.Ordinal),defines);
            var accepted = new SdkInstanceInheritanceProfile(schemas,paths,musicOffsets:true).Apply(entry,File.ReadAllBytes(entry));
            Require(accepted.Bytes != null && Volume(accepted.Bytes,"Owner") == pair.Item2,"Integer offset boundary result changed.");
        }
        foreach (string bad in new[] { "=$VOL + 31","=$VOL - 71","=$VOL * 5","=$VOL + 5.0","=$VOL + -5","=$VOL + $VOL","=$VOL + 5 + 1","=$MISSING + 5","=$vol + 5","=$VOL+5","=$VOL + 1000" })
            Reject(Fixture(includes+assets.Replace("=$VOL + 5",bad,StringComparison.Ordinal),defines));
        foreach (string badValue in new[] { "70.0","70%","-1","101","=70","NaN","1,0" })
            Reject(Fixture(includes+assets,defines.Replace("value='70'","value='"+badValue+"'",StringComparison.Ordinal)));
        foreach (string bad in new[] { assets.Replace("Volume='=$VOL + 5'","A='=$VOL + 5'",StringComparison.Ordinal),assets.Replace("id='Owner'","id='=$VOL + 5'",StringComparison.Ordinal),assets.Replace("id='Owner'","id='Owner' TypeId='late'",StringComparison.Ordinal),assets.Replace("MusicTrack","AudioEvent",StringComparison.Ordinal) })
            Reject(Fixture(includes+bad,defines));
        // Reborn: late source failure must erase an earlier computed offset and all captured closure authority.
        Reject(Fixture(includes+assets+"<MusicTrack id='Late' inheritFrom='Base' A='=$MISSING'/>",defines));
        var tooMany = defines+"<MusicTrack id='Base' Volume='55'/>"+string.Concat(Enumerable.Range(0,17).Select(index => "<MusicTrack id='Owner"+index+"' inheritFrom='Base' Volume='=$VOL + 5'/>"));
        Reject(Fixture(tooMany,defines));
        var wrongSchemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd.Replace("name='Volume' type='Percentage'","name='Volume' type='xs:string'",StringComparison.Ordinal)) },"entry.xsd").Schemas;
        Require(SdkLocalDefineProfile.ApplyMusicOffsets(Source(defines+assets),null,wrongSchemas,true).Bytes == null,"Wrong schema Volume type bypassed gate.");
        paths = Fixture(includes+assets,defines); File.WriteAllBytes(leaf,Source(defines+"<!--stale-->")); Reject(paths);
        // Reborn: defining-source values are resolved before import and must not be replayed with the consuming owner's literal.
        File.WriteAllBytes(Path.Combine(data,"Middle.xml"),Source("<Defines><Define name='CHILD' value='60'/></Defines><MusicTrack id='Middle' Volume='=$CHILD + 5'/>"));
        paths = Fixture("<Includes><Include type='instance' source='DATA:Middle.xml'/></Includes>"+defines+"<MusicTrack id='Owner' inheritFrom='Middle' A='1'/>",defines);
        var imported = new SdkInstanceInheritanceProfile(schemas,paths,musicOffsets:true).Apply(entry,File.ReadAllBytes(entry));
        Require(imported.Bytes != null && Volume(imported.Bytes,"Owner") == "65" && imported.Evidence.ImportedBases.Length == 1 && imported.Evidence.PreparedSources.Length == 2,"Imported music defining context was lost.");
        var invalidPaths = Fixture(includes+assets.Replace("Volume='=$VOL + 5'","Volume='=$VOL + 5' A='bad'",StringComparison.Ordinal),defines);
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceMusicOffsets:true).Documents.Single(document => document.SourcePath == entry);
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Music arithmetic bypassed final scalar validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceMusicOffsets:true,instanceCrossStateRemovals:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting music scopes admitted.");
        Console.WriteLine("SDK instance music offsets self-test: OK (integer boundaries, exact schema/slot, local/imported defining contexts, core result, earlier isolation, unsupported/range/identity/stale/late/slot-limit atomic refusals and final no-fields validation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: snapshot owned fixtures only; never modify synced/reference audio XML or schemas. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body,string definitions)
        {
            File.WriteAllBytes(entry,Source(body)); File.WriteAllBytes(leaf,Source(definitions));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject whole owners atomically instead of exposing partially resolved arithmetic or imported sources. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,musicOffsets:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.ExpressionPreparation == null && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.Overlays.Length == 0,"Rejected music owner leaked partial authority.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check actual owner Volume after the unchanged Core overlay without compiling native assets. */
    //-------------------------------------------------------------------------------------------------
    private static string Volume(byte[] bytes,string id)
    {
        XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes));
        return xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == id).GetAttribute("Volume");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned diagnostic source fixtures without external writes. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop when a narrowed expression or source-context invariant changes. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
