using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test separately typed sound integer arithmetic without widening MusicTrack, resource limits or native compilation authority.
internal static class SdkInstanceSoundOffsetsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove exact root/pitch results after Core merging, field/type/range guards, source contexts and atomic failures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Percentage'><xs:restriction base='xs:string'><xs:pattern value='-?[0-9]+'/></xs:restriction></xs:simpleType><xs:simpleType name='SageReal'><xs:restriction base='xs:string'><xs:pattern value='-?[0-9]+'/></xs:restriction></xs:simpleType><xs:complexType name='RealRange'><xs:attribute name='Low' type='SageReal' use='required'/><xs:attribute name='High' type='SageReal' use='required'/></xs:complexType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='AudioEvent'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='PitchShift' type='RealRange' minOccurs='0' maxOccurs='1'/></xs:sequence><xs:attribute name='Volume' type='Percentage'/><xs:attribute name='MinVolume' type='Percentage'/><xs:attribute name='VolumeShift' type='Percentage'/><xs:attribute name='MinRange' type='SageReal'/><xs:attribute name='MaxRange' type='SageReal'/><xs:attribute name='A' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='AudioEventOverridable'><xs:complexContent><xs:extension base='AudioEvent'/></xs:complexContent></xs:complexType><xs:complexType name='MusicTrack'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:attribute name='Volume' type='Percentage'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Defines' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Define' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='name' type='xs:string'/><xs:attribute name='value' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:choice minOccurs='0' maxOccurs='unbounded'><xs:element name='AudioEvent' type='AudioEvent'/><xs:element name='AudioEventOverridable' type='AudioEventOverridable'/><xs:element name='MusicTrack' type='MusicTrack'/></xs:choice></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-SoundOffsets-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),leaf = Path.Combine(data,"Definitions.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string definitions = "<Defines><Define name='VOL' value='50'/><Define name='MIN' value='90'/><Define name='RANGE' value='400'/><Define name='MAX' value='900'/><Define name='SHIFT' value='-10'/><Define name='PLOW' value='-1'/><Define name='PHIGH' value='1'/></Defines>";
        const string includes = "<Includes><Include type='all' source='DATA:Definitions.xml'/></Includes>";
        const string assets = "<AudioEvent id='Base' Volume='50'><PitchShift Low='-2' High='2'/></AudioEvent><AudioEvent id='Owner' inheritFrom='Base' Volume='=$VOL +85' MinVolume='=$MIN+0' MinRange='=$RANGE -50' MaxRange='=$MAX + 600' VolumeShift='=$SHIFT+0'><PitchShift Low='=$PLOW +0' High='=$PHIGH+0'/></AudioEvent><AudioEventOverridable id='BaseO' Volume='50'/><AudioEventOverridable id='OwnerO' inheritFrom='BaseO' Volume='=60'/><MusicTrack id='Song' Volume='=$VOL + 5'/>";
        var paths = Fixture(includes+assets,definitions); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,soundOffsets:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.SoundOffsetName && result.Evidence.ExpressionPreparation is { Substitutions:9 } expression
            && expression.SoundOffsets.Length == 8 && expression.MusicVolumeOffsets.Length == 1 && expression.DefinitionSources.Length == 2,"Typed sound expression/context evidence failed: "+string.Join("; ",result.Evidence.Diagnostics));
        XmlDocument xml = Parse(result.Bytes!); var owner = Owner(xml,"Owner");
        Require(owner.GetAttribute("Volume") == "135" && owner.GetAttribute("MinVolume") == "90" && owner.GetAttribute("VolumeShift") == "-10" && owner.GetAttribute("MinRange") == "350" && owner.GetAttribute("MaxRange") == "1500"
            && ((XmlElement)owner.FirstChild!).GetAttribute("Low") == "-1" && ((XmlElement)owner.FirstChild!).GetAttribute("High") == "1" && Owner(xml,"OwnerO").GetAttribute("Volume") == "60" && Owner(xml,"Song").GetAttribute("Volume") == "55","Actual Core typed root/pitch/Music values differ.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,audioTrees:true).Apply(entry,raw).Bytes == null,"Earlier audio tree profile gained arithmetic.");
        Require(SdkTypedSourceGraph.BindGraph(schemas,paths,instanceSoundOffsets:true).ScopedGraphComplete && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Typed sound final source/schema/output scope changed.");
        foreach (string bad in new[] { "=$VOL +151","=$VOL -51","=$VOL *5","=$VOL /5","=$VOL +5.0","=$VOL +5%","=$VOL +$MIN","=$VOL +5+1","=$MISSING +5","=$vol +5","=$VOL +-5","=$VOL +1001","=NaN","=1e2" })
            Reject(Fixture(includes+assets.Replace("=$VOL +85",bad,StringComparison.Ordinal),definitions));
        foreach (string value in new[] { "50.0","50%","+50","201","NaN","1,0" })
            Reject(Fixture(includes+assets,definitions.Replace("value='50'","value='"+value+"'",StringComparison.Ordinal)));
        foreach (string bad in new[] { assets.Replace("Volume='=$VOL +85'","A='=$VOL +85'",StringComparison.Ordinal),assets.Replace("id='Owner'","id='=$VOL +5'",StringComparison.Ordinal),assets.Replace("id='Owner'","id='Owner' TypeId='late'",StringComparison.Ordinal),assets.Replace("=$SHIFT+0","=-101",StringComparison.Ordinal),assets.Replace("=$PLOW +0","=-13",StringComparison.Ordinal),assets.Replace("=$MAX + 600","=2049",StringComparison.Ordinal),assets.Replace("=$VOL + 5","=$VOL +85",StringComparison.Ordinal) })
            Reject(Fixture(includes+bad,definitions));
        // Reborn: late owner failures erase earlier arithmetic/source/import witnesses.
        Reject(Fixture(includes+assets+"<AudioEvent id='Late' inheritFrom='Base' Volume='=$UNKNOWN+1'/>",definitions));
        paths = Fixture(includes+assets,definitions); File.WriteAllBytes(leaf,Source(definitions+"<!--stale-->")); Reject(paths);
        var invalidPaths = Fixture(includes+assets.Replace("id='Owner'","id='Owner' A='bad'",StringComparison.Ordinal),definitions);
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceSoundOffsets:true).Documents.Single(document => document.SourcePath == entry);
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Typed arithmetic bypassed final scalar schema binding.");
        foreach (string changed in new[] { Encoding.UTF8.GetString(result.Bytes!).Replace("Volume=\"135\"","Volume=\"134\"",StringComparison.Ordinal),Encoding.UTF8.GetString(result.Bytes!).Replace("Low=\"-1\"","Low=\"0\"",StringComparison.Ordinal),Encoding.UTF8.GetString(result.Bytes!).Replace("id=\"Owner\"","id=\"Absent\"",StringComparison.Ordinal) })
        {
            bool refused = false; try { SdkSoundOffsets.Verify(Parse(Encoding.UTF8.GetBytes(changed)),result.Evidence.ExpressionPreparation!.SoundOffsets); } catch (InvalidDataException) { refused = true; } Require(refused,"Changed final root/pitch/owner bypassed verification.");
        }
        foreach (string drift in new[] { xsd.Replace("name='Volume' type='Percentage'","name='Volume' type='xs:string'",StringComparison.Ordinal),xsd.Replace("name='PitchShift' type='RealRange' minOccurs='0' maxOccurs='1'","name='PitchShift' type='RealRange' minOccurs='0' maxOccurs='2'",StringComparison.Ordinal) })
        {
            var changed = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(drift) },"entry.xsd").Schemas;
            Require(SdkLocalDefineProfile.ApplySoundOffsets(Source(definitions+assets),null,changed,true).Bytes == null,"Schema type/cardinality drift bypassed sound gate.");
        }
        // Reborn: exact arithmetic slot limit and numeric boundaries are checked without native serializers.
        string constants = string.Concat(Enumerable.Range(0,512).Select(index => "<AudioEvent id='C"+index+"' Volume='=60'/>"));
        Require(SdkLocalDefineProfile.ApplySoundOffsets(Source(constants),null,schemas,true).Evidence.SoundOffsets.Length == 512,"Exact 512 sound slot boundary refused.");
        Require(SdkLocalDefineProfile.ApplySoundOffsets(Source(constants+"<AudioEvent id='TooMany' Volume='=60'/>"),null,schemas,true).Bytes == null,"513 sound arithmetic slots admitted.");
        var boundaries = SdkLocalDefineProfile.ApplySoundOffsets(Source("<AudioEvent id='Limits' Volume='=200' MinVolume='=100' VolumeShift='=-100' MinRange='=0' MaxRange='=2048'><PitchShift Low='=-12' High='=12'/></AudioEvent>"),null,schemas,true);
        Require(boundaries.Bytes != null && boundaries.Evidence.SoundOffsets.Length == 7,"Typed numeric boundary refused.");
        // Reborn: imported assets use their own defining literal, not a consumer's unrelated local table.
        File.WriteAllBytes(Path.Combine(data,"Middle.xml"),Source("<Defines><Define name='CHILD' value='60'/></Defines><AudioEvent id='Middle' Volume='=$CHILD+5'/>"));
        paths = Fixture("<Includes><Include type='instance' source='DATA:Middle.xml'/></Includes>"+definitions+"<AudioEvent id='Owner' inheritFrom='Middle'/>",definitions);
        var imported = new SdkInstanceInheritanceProfile(schemas,paths,soundOffsets:true).Apply(entry,File.ReadAllBytes(entry));
        Require(imported.Bytes != null && Owner(Parse(imported.Bytes),"Owner").GetAttribute("Volume") == "65" && imported.Evidence.PreparedSources.Length == 2,"Imported sound defining context changed.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceSoundOffsets:true,instanceAudioTrees:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting sound profiles admitted.");
        // Reborn: repeated singleton review exposes successful arithmetic without permitting owner normalization or leaking stale-source authority.
        var repeatedPaths = Fixture(includes+assets.Replace("<PitchShift Low='=$PLOW +0' High='=$PHIGH+0'/>","<PitchShift Low='-2' High='2'/><PitchShift Low='=$PLOW +0' High='=$PHIGH+0'/>",StringComparison.Ordinal),definitions);
        var review = SdkSoundExpressionReview.Inspect(repeatedPaths,schemas,entry);
        Require(review.Substitutions == 9 && review.SoundCalculations == 8 && review.RepeatedSingletons.Single() is { AssetId:"Owner",ChildName:"PitchShift",SchemaType:"RealRange" }
            && review.RepeatedSingletons.Single().Occurrences[1]["Low"] == "-1" && !review.OwnerInheritanceValidated && !review.ProductionBuildReady && !Directory.Exists(output),"Expression-stage review granted singleton/owner admission.");
        Require(new SdkInstanceInheritanceProfile(schemas,repeatedPaths,soundOffsets:true).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Repeated singleton owner silently admitted.");
        File.WriteAllBytes(leaf,Source(definitions+"<!--stale-review-->"));
        bool staleReview = false; try { SdkSoundExpressionReview.Inspect(repeatedPaths,schemas,entry); } catch (InvalidDataException) { staleReview = true; } Require(staleReview,"Stale expression review published authority.");
        Console.WriteLine("SDK instance sound offsets self-test: OK (typed root/singleton-pitch offsets, signed operands/constants, bounds and 512-slot limit, actual Core retention, local/imported contexts, old Music/tree isolation, schema/stale/late atomic refusals and final no-fields binding)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture only owned typed audio fixtures beneath explicit roots. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body,string defs)
        {
            File.WriteAllBytes(entry,Source(body)); File.WriteAllBytes(leaf,Source(defs));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: whole-owner preparation failures withhold all calculation and captured-source authority. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,soundOffsets:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.ExpressionPreparation == null && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.Overlays.Length == 0,"Rejected sound source leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select one original diagnostic owner without inferring native identity or resource payloads. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement Owner(XmlDocument xml,string id) => xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == id);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned source fixtures without modifying reference audio/XML/schema files. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse only owned bounded fixtures without external entity resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on changed typed arithmetic, stage order or source authority invariants. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
