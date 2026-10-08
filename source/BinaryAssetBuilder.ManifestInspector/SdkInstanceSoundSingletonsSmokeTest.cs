using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test known full-owner sound singleton admission independently of arithmetic-only and all earlier scopes.
internal static class SdkInstanceSoundSingletonsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove imported/local base handling, final projection, unchanged old refusal, atomic late/stale failures and exclusive profile selection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var review = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        if (!review.SchemaAdmitted || schemas == null) throw new InvalidDataException("Reviewed schema required.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-SoundSingletons-"+Guid.NewGuid().ToString("N")),entry = Path.Combine(root,"Entry.xml"),basis = Path.Combine(root,"Base.xml"),output = root+"-NoOutput";
        Directory.CreateDirectory(root);
        const string baseBody = "<AudioEvent id='BaseSoundEffect' Volume='100' MinVolume='0' Priority='NORMAL' MinRange='300' MaxRange='1000'/>";
        const string building = "<AudioEvent id='BuildingInfiltrated1' inheritFrom='AudioEvent:BaseSoundEffect' Volume='55'><PitchShift Low='-10' High='-5'/><InitialDelay Low='0' High='50'/><PitchShift Low='-1' High='1'/><Sound>WBSpy_infiltrateBldgP</Sound></AudioEvent>";
        const string street = "<AudioEvent id='StreetLampCrush' inheritFrom='AudioEvent:BaseSoundEffect' Volume='50'><NonInterruptibleTime Low='0.0s' High='0.5s'/><PitchShift Low='-10' High='10'/><NonInterruptibleTime Low='0.0s' High='0.8s'/><Delay Low='0' High='100'/><Sound>WBStreetLamp_crushA</Sound><Sound>WBStreetLamp_crushB</Sound><Sound>WBStreetLamp_crushC</Sound></AudioEvent>";
        const string includes = "<Includes><Include type='instance' source='DATA:Base.xml'/></Includes>";
        var paths = Fixture(includes+building+street,baseBody); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,soundSingletons:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.SoundSingletonName && result.Evidence.SoundSingletons.Length == 2
            && result.Evidence.ImportedBases.Length == 1 && result.Evidence.PreparedSources.Length == 2 && result.Evidence.Overlays.Length == 2,
            "Known singleton imported proof failed: "+string.Join("; ",result.Evidence.Diagnostics));
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceSoundSingletons:true);
        Require(graph.ScopedGraphComplete && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Whole owned graph/raw/output scope differs.");
        Reject(paths,old:true);
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var plansXml = new XmlDocument { XmlResolver = null }; plansXml.LoadXml(Encoding.UTF8.GetString(Source(baseBody+building+street)));
        var plans = SdkSoundSingletons.Normalize(schemas,plansXml);
        SdkSoundSingletons.Verify(xml,plans);
        xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.GetAttribute("id") == "StreetLampCrush").SetAttribute("MinRange","999");
        bool mismatch = false; try { SdkSoundSingletons.Verify(xml,plans); } catch (InvalidDataException) { mismatch = true; }
        Require(mismatch,"Changed final inherited field escaped full owner verification.");
        paths = Fixture(baseBody+building+street,baseBody);
        Require(new SdkInstanceInheritanceProfile(schemas,paths,soundSingletons:true).Apply(entry,File.ReadAllBytes(entry)).Evidence.SoundSingletons.Length == 2,"Local empty base proof failed.");
        foreach (var changed in new[] {
            building.Replace("WBSpy_infiltrateBldgP","OtherSound",StringComparison.Ordinal),
            building.Replace("Low='-10'","Low='-9'",StringComparison.Ordinal),
            building.Replace("BuildingInfiltrated1","UnknownOwner",StringComparison.Ordinal),
            building.Replace("<Sound>","<PitchShift Low='-2' High='2'/><Sound>",StringComparison.Ordinal),
            building.Replace("Low='-1' High='1'","High='1'",StringComparison.Ordinal),
            building.Replace("inheritFrom='AudioEvent:BaseSoundEffect'","",StringComparison.Ordinal),
            building.Replace("Volume='55'","Volume='55' Unexpected='1'",StringComparison.Ordinal),
            building.Replace("Volume='55'","Volume='55' xmlns:i='uri:ea.com:eala:asset:instance' i:joinAction='Replace'",StringComparison.Ordinal),
            building.Replace("<Sound>","<Sound Extra='1'>",StringComparison.Ordinal) }) Reject(Fixture(includes+changed+street,baseBody));
        Reject(Fixture(includes+building+street,baseBody.Replace("/>","><PitchShift Low='-2' High='2'/></AudioEvent>",StringComparison.Ordinal)));
        Reject(Fixture(includes+building+street+"<AudioEvent id='Late' Unexpected='1'/>",baseBody));
        paths = Fixture(includes+building+street,baseBody); File.WriteAllBytes(basis,Source(baseBody.Replace("Volume='100'","Volume='90'",StringComparison.Ordinal))); Reject(paths);
        paths = Fixture(includes+building+street,baseBody);
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceSoundOffsets:true,instanceSoundSingletons:true); } catch (ArgumentException) { conflict = true; }
        Require(conflict,"Conflicting sound profiles accepted.");
        // Reborn: named schema identity alone is insufficient; a widened singleton cardinality cannot reuse this proof.
        const string changedXsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:complexType name='RealRange'><xs:attribute name='Low'/><xs:attribute name='High'/></xs:complexType><xs:complexType name='AudioEvent'><xs:sequence><xs:element name='PitchShift' type='RealRange' minOccurs='0' maxOccurs='2'/></xs:sequence></xs:complexType></xs:schema>";
        var changedSchemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(changedXsd) },"entry.xsd").Schemas;
        plansXml.LoadXml(Encoding.UTF8.GetString(Source(baseBody+building)));
        bool schemaRejected = false; try { SdkSoundSingletons.Normalize(changedSchemas,plansXml); } catch (InvalidDataException) { schemaRejected = true; }
        Require(schemaRejected,"Changed singleton maxOccurs accepted.");
        var boundOwners = plansXml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        for (int index = 0; index < 65; index++) boundOwners[1].SetAttribute("Extra"+index,"1");
        bool attributesRejected = false; try { SdkSoundOwnerReview.Predict(boundOwners[0],boundOwners[1]); } catch (InvalidDataException) { attributesRejected = true; }
        Require(attributesRejected,"Root attribute resource bound escaped prediction.");
        Console.WriteLine("SDK instance sound singletons self-test: OK (complete local/imported owner proof, actual final field tamper detection, whole fixture graph, old scope refusal, changed/unknown/extra/partial bodies, populated base, late/stale atomic refusal and exclusive CLI/API scope; no native emission)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture only owned source fixtures beneath an explicit root; never modify reference XML. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body,string baseXml)
        {
            File.WriteAllBytes(entry,Source(body)); File.WriteAllBytes(basis,Source(baseXml));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),root,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: any rejected preparation must erase all earlier singleton/overlay/arithmetic/imported source evidence. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured,bool old = false)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,soundOffsets:old,soundSingletons:!old).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.SoundSingletons.Length == 0 && rejected.Evidence.ExpressionPreparation == null
                && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.PreparedSources.Length == 0,"Rejected singleton source leaked partial authority.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned audio fixture documents without touching official source/schema files. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on scope widening or loss of complete owner/source atomicity. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
