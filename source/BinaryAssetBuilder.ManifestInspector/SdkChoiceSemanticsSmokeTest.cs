using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize core choice behavior before admitting any new diagnostic particle scope.
internal static class SdkChoiceSemanticsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove repeated anonymous choice preservation, aggregate occurrence semantics and unsafe keyed/singleton matching without widening existing profiles. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:complexType name='Rule'><xs:simpleContent><xs:extension base='xs:string'><xs:attribute name='id' type='xs:string'/><xs:attribute name='Weight' type='xs:int'/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name='RepeatedChoice'><xs:choice minOccurs='1' maxOccurs='3'><xs:element name='Map' type='Rule'/><xs:element name='Path' type='Rule'/></xs:choice></xs:complexType><xs:complexType name='SingleChoice'><xs:choice><xs:element name='Map' type='Rule'/><xs:element name='Path' type='Rule'/></xs:choice></xs:complexType><xs:complexType name='Move'><xs:sequence><xs:element name='Heuristic' type='RepeatedChoice'/></xs:sequence><xs:attribute name='Name' type='xs:string'/></xs:complexType><xs:complexType name='Asset'><xs:sequence><xs:element name='OpeningMove' type='Move' minOccurs='0' maxOccurs='unbounded'/><xs:element name='Single' type='SingleChoice' minOccurs='0'/></xs:sequence><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string baseBody = Move("base","<Map>first</Map><Map>second</Map><Path>third</Path>");
        string derivedBody = Move("derived","<Path>fourth</Path>");
        XmlElement merged = Merge(baseBody,derivedBody);
        var moves = merged.ChildNodes.OfType<XmlElement>().ToArray();
        Require(moves.Length == 2 && moves[0].GetAttribute("Name") == "base" && moves[1].GetAttribute("Name") == "derived","Anonymous OpeningMove entries matched by Name instead of remaining separate.");
        var rules = moves[0].FirstChild!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(rules.Select(rule => rule.LocalName).SequenceEqual(new[] { "Map","Map","Path" }) && rules.Select(rule => rule.InnerText).SequenceEqual(new[] { "first","second","third" }),"Repeated anonymous choice alternatives collapsed or reordered during one-sided copying.");
        Require(Valid(merged),"Repeated Map alternative must validate: choice aggregate cardinality is not per-alternative MaxOccurs=1.");
        Require(!Valid(Merge(Move("too-many","<Map>a</Map><Map>b</Map><Map>c</Map><Map>d</Map>"),"")),"Core copying unexpectedly enforced choice aggregate cardinality; final schema gate assumptions changed.");
        // Reborn: even a single populated side can silently replace cross-QName repeated-choice siblings sharing an ID.
        merged = Merge(Move("collision","<Map id='same'>first</Map><Path id='same'>second</Path>"),"");
        rules = merged.FirstChild!.FirstChild!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(rules.Length == 1 && rules[0].LocalName == "Path" && rules[0].InnerText == "second","Core cross-QName choice ID replacement changed; review before admission.");
        // Reborn: same-QName repeated-choice IDs merge attributes but append text, so leaf matching cannot be treated as literal replacement.
        merged = Merge(Move("same-key","<Map id='same' Weight='1'>left</Map><Map id='same' Weight='2'>right</Map>"),"");
        rules = merged.FirstChild!.FirstChild!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(rules.Length == 1 && rules[0].InnerText == "leftright" && rules[0].GetAttribute("Weight") == "2","Core same-key choice text/attribute behavior changed.");
        // Reborn: singleton choices replace a different alternative even without IDs; malformed input can become apparently valid output.
        merged = Merge("<Single><Map>first</Map><Path>last</Path></Single>","");
        Require(merged.FirstChild!.ChildNodes.Count == 1 && merged.FirstChild.FirstChild!.LocalName == "Path" && Valid(merged),"Singleton choice replacement characterization changed.");
        // Reborn: existing diagnostic flags must stay closed for both safe-copy examples and destructive choice examples until a separate scope is reviewed.
        foreach (string body in new[] { baseBody,Move("collision","<Map id='same'>first</Map><Path id='same'>second</Path>"),"<Single><Map>first</Map><Path>last</Path></Single>" })
        {
            byte[] raw = Source("<Asset id='Base'>"+body+"</Asset><Asset id='Derived' inheritFrom='Base'/>");
            foreach (var result in new[] { SdkSelfAttributeInheritance.Apply(schemas,raw,treeCopy:true),SdkSelfAttributeInheritance.Apply(schemas,raw,childMerge:true),SdkSelfAttributeInheritance.Apply(schemas,raw,childRemoval:true) })
                Require(result.Bytes == null && result.Evidence.ProcessedSha256 == null && result.Evidence.Overlays.Length == 0 && result.Evidence.Removals.Length == 0,"An existing profile admitted choice particles or leaked partial evidence.");
        }
        Console.WriteLine("SDK choice semantics self-test: OK (anonymous repeated alternatives/order, aggregate vs alternative cardinality, anonymous OpeningMove identity, cross-QName keyed replacement, same-key text append, singleton destructive replacement and unchanged atomic diagnostic refusals)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: exercise the unchanged core joiner with unvalidated owned fixture nodes, matching diagnostic preprocessing conditions. */
        //-------------------------------------------------------------------------------------------------
        XmlElement Merge(string before,string after)
        {
            XmlDocument xml = new() { XmlResolver = null };
            xml.LoadXml(Encoding.UTF8.GetString(Source("<Asset id='Base'>"+before+"</Asset><Asset id='Derived' inheritFrom='Base'>"+after+"</Asset>")));
            var assets = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
            return (XmlElement)NodeJoiner.Override(schemas,xml,assets[0],assets[1]);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: independently bind merged fixtures to show what final validation catches and what destructive preprocessing can conceal. */
        //-------------------------------------------------------------------------------------------------
        bool Valid(XmlElement asset)
        {
            XmlDocument xml = new() { XmlResolver = null,Schemas = schemas };
            xml.LoadXml(Encoding.UTF8.GetString(Source(asset.OuterXml)));
            bool valid = true; xml.Validate((_,_) => valid = false); return valid;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct anonymous repeated opening-move branches without inventing ID matching semantics for Name. */
    //-------------------------------------------------------------------------------------------------
    private static string Move(string name,string rules) => "<OpeningMove Name='"+name+"'><Heuristic>"+rules+"</Heuristic></OpeningMove>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned particle characterization fixtures without touching official XML/XSD. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on changed core semantics before any future choice admission can rely on stale assumptions. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
