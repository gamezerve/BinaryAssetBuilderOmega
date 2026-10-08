using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test broad shallow audio documents without widening generic trees, expression syntax or native build authority.
internal static class SdkInstanceAudioTreesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove actual large-owner preparation, exact boundaries and atomic resource/schema/source refusals. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='AudioEvent'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='Sound' type='xs:string' minOccurs='0' maxOccurs='unbounded'/></xs:sequence><xs:attribute name='Volume' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='AudioEvent' type='AudioEvent' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-AudioTrees-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        string body = Body(1000,9);
        var paths = Fixture(body); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,audioTrees:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.AudioTreeName && result.Evidence.AudioTree is { OriginalElements:10001,Owners:1001,LargestOriginalOwner:10,FinalElements:10001,MergePairUnits:10000 }
            && result.Evidence.Overlays.Length == 1000,"Broad audio budget/core evidence failed: "+string.Join("; ",result.Evidence.Diagnostics));
        var xml = Parse(result.Bytes!); var owner = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner999");
        Require(owner.GetAttribute("Volume") == "70" && owner.ChildNodes.OfType<XmlElement>().Select(child => child.InnerText).SequenceEqual(Enumerable.Range(0,9).Select(index => "sound"+index)),"Actual Core large-owner fields/order changed.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,musicOffsets:true).Apply(entry,raw).Bytes == null,"Earlier music scope widened its tree bound.");
        Require(SdkTypedSourceGraph.BindGraph(schemas,paths,instanceAudioTrees:true).ScopedGraphComplete && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Broad audio final source/schema/output scope changed.");
        Reject(Fixture(Body(1000,17)));
        Reject(Fixture(Body(2048,4)));
        Reject(Fixture(body+"<AudioEvent id='TooLarge'>"+Sounds(128)+"</AudioEvent>"));
        Reject(Fixture(body+"<AudioEvent id='Comments'>"+string.Concat(Enumerable.Repeat("<!--node-->",512))+"</AudioEvent>"));
        Reject(Fixture(body+"<AudioEvent id='Deep'>"+string.Concat(Enumerable.Repeat("<Sound>",9))+"x"+string.Concat(Enumerable.Repeat("</Sound>",9))+"</AudioEvent>"));
        Reject(Fixture(body.Replace("AudioEvent","GameObject",StringComparison.Ordinal)));
        Reject(Fixture(body+"<Tags>"+string.Concat(Enumerable.Repeat("<Tag/>",1025))+"</Tags>"));
        Reject(Fixture(body.Replace("id='Owner999'","id='Owner999' TypeId='late'",StringComparison.Ordinal)));
        // Reborn: final output amplification cannot ride through a larger raw-input budget.
        Reject(Fixture(Body(1000,9,20)));
        // Reborn: stale supplied source bytes cannot authorize large-tree expansion after a captured path snapshot.
        paths = Fixture(body); File.WriteAllBytes(entry,Source(body+"<!--changed-->")); Reject(paths);
        var invalidPaths = Fixture(body.Replace("id='Owner999'","id='Owner999' Volume='bad'",StringComparison.Ordinal));
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceAudioTrees:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Large audio budget bypassed final scalar schema validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceAudioTrees:true,instanceMusicOffsets:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting audio breadth scopes admitted.");
        // Reborn: pin exact pair-work boundary independently of elapsed time and prevent cached bases from bypassing charges.
        xml = Parse(Source(body)); var assets = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        var budget = SdkAudioTreeBudget.Prove(xml.DocumentElement!,assets,schemas)!;
        var wide = Parse(Source("<AudioEvent id='Wide'>"+Sounds(127)+"</AudioEvent>")).DocumentElement!.FirstChild as XmlElement;
        for (int index = 0; index < 64; index++) budget.Charge(wide!,wide!);
        Require(budget.Verify(xml.DocumentElement!).MergePairUnits == 1048576,"Exact audio pair-work boundary changed.");
        bool workRefused = false; try { budget.Charge(wide!,wide!); } catch (InvalidDataException) { workRefused = true; } Require(workRefused,"Audio pair-work excess admitted.");
        // Reborn: more than 64 attributes must fail before schema/merge allocation, even on otherwise shallow owners.
        xml = Parse(Source(body+"<AudioEvent id='Attrs' "+string.Join(" ",Enumerable.Range(0,64).Select(index => "a"+index+"='x'"))+"/>"));
        bool attrsRefused = false; try { SdkAudioTreeBudget.Prove(xml.DocumentElement!,xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray(),schemas); } catch (InvalidDataException) { attrsRefused = true; } Require(attrsRefused,"Audio owner attribute bound widened.");
        // Reborn: exact total/owner boundaries remain admissible while small documents retain the earlier limit contract.
        xml = Parse(Source(string.Concat(Enumerable.Range(0,128).Select(index => "<AudioEvent id='Exact"+index+"'>"+Sounds(127)+"</AudioEvent>"))));
        budget = SdkAudioTreeBudget.Prove(xml.DocumentElement!,xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray(),schemas)!;
        Require(budget.Verify(xml.DocumentElement!).FinalElements == 16384,"Exact 16384 audio element boundary refused.");
        xml = Parse(Source(Body(2047,4)));
        Require(SdkAudioTreeBudget.Prove(xml.DocumentElement!,xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray(),schemas)!.Verify(xml.DocumentElement!).Owners == 2048,"Exact 2048 owner boundary refused.");
        xml = Parse(Source("<AudioEvent id='Small'/>"));
        Require(SdkAudioTreeBudget.Prove(xml.DocumentElement!,xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray(),schemas) == null,"Small input unnecessarily acquired broad-audio authority.");
        // Reborn: an amplified individual output owner must fail even when final document breadth is still below the larger bound.
        xml = Parse(Source(body+"<AudioEvent id='FinalLarge'>"+Sounds(128)+"</AudioEvent>"));
        bool finalOwnerRefused = false; try { budget.Verify(xml.DocumentElement!); } catch (InvalidDataException) { finalOwnerRefused = true; } Require(finalOwnerRefused,"Oversized final audio owner admitted.");
        Console.WriteLine("SDK instance audio trees self-test: OK (1000 actual Core overlays, full owner fields/order, old 8192 isolation, total/owner/depth/attribute/metadata/pair-work/output limits, stale/late atomic refusal and final no-fields binding)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: snapshot only owned breadth fixtures under explicit data/output roots. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string contents)
        {
            File.WriteAllBytes(entry,Source(contents));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refuse all transformed bytes, resource evidence and earlier overlays after any whole-document failure. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,audioTrees:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.AudioTree == null && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.PreparedSources.Length == 0,"Rejected broad audio source leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate bounded shallow owners with stable anonymous sound order and one same-type base. */
    //-------------------------------------------------------------------------------------------------
    private static string Body(int owners,int sounds,int baseSounds = 0) => "<AudioEvent id='Base' Volume='70'>"+Sounds(baseSounds)+"</AudioEvent>"+string.Concat(Enumerable.Range(0,owners).Select(index => "<AudioEvent id='Owner"+index+"' inheritFrom='Base'>"+Sounds(sounds)+"</AudioEvent>"));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain anonymous repeated sound references without deduplication or payload I/O. */
    //-------------------------------------------------------------------------------------------------
    private static string Sounds(int count) => string.Concat(Enumerable.Range(0,count).Select(index => "<Sound>sound"+index+"</Sound>"));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned fixtures without editing reference source/schema files. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse owned bounded fixtures without external entity resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on changed breadth/work/source invariants without claiming production resource safety. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
