using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize repeated sibling identity operations before allowing any duplicate-ID normalization.
internal static class SdkSiblingIdentitySemanticsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin identical/conflicting repeated keys, ordered remove/re-add and cross-QName destructive lookup while keeping graph admission closed. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:complexType name='State'><xs:attribute name='id' type='xs:string'/><xs:attribute name='State' type='xs:string'/><xs:attribute name='Difficulty' type='xs:string'/><xs:attribute name='Old' type='xs:int'/></xs:complexType><xs:complexType name='Asset'><xs:sequence><xs:element name='BuildState' type='State' minOccurs='0' maxOccurs='unbounded'/><xs:element name='StrategicState' type='State' minOccurs='0' maxOccurs='unbounded'/></xs:sequence><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        const string repeated = "<StrategicState id='same' State='attack' Difficulty='HARD BRUTAL'/>";
        XmlElement merged = Merge("",repeated+repeated);
        Require(States(merged).Length == 1 && States(merged)[0].GetAttribute("State") == "attack" && Valid(merged),"Identical repeated keys no longer coalesce under the unchanged core.");
        // Reborn: final schema validity alone cannot detect discarded conflicting attributes.
        merged = Merge("",repeated+"<StrategicState id='same' State='defend' Difficulty='EASY'/>");
        Require(States(merged).Length == 1 && States(merged)[0].GetAttribute("State") == "defend" && States(merged)[0].GetAttribute("Difficulty") == "EASY" && Valid(merged),"Core repeated-key last-write-wins semantics changed.");
        const string before = "<StrategicState id='same' State='old' Old='7'/><StrategicState id='other' State='keep'/>";
        const string remove = "<StrategicState id='same' i:joinAction='Remove'/>";
        const string replacement = "<StrategicState id='same' State='new' Difficulty='HARD BRUTAL'/>";
        merged = Merge(before,remove+replacement);
        Require(States(merged).Select(state => state.GetAttribute("id")).SequenceEqual(new[] { "other","same" }) && !States(merged)[1].HasAttribute("Old") && States(merged)[1].GetAttribute("State") == "new" && Valid(merged),"Remove/re-add must discard old-only attributes and append at the new position.");
        // Reborn: collapsing the pair to an ordinary overlay would retain old-only fields and its original position.
        var shortcut = Merge(before,replacement);
        Require(States(shortcut)[0].GetAttribute("id") == "same" && States(shortcut)[0].GetAttribute("Old") == "7" && Valid(shortcut),"Replacement shortcut no longer differs from ordered remove/re-add.");
        merged = Merge(before,replacement+remove);
        Require(States(merged).Length == 1 && States(merged)[0].GetAttribute("id") == "other" && Valid(merged),"Reversing command order must remove the new entry.");
        // Reborn: core repeated-key selection spans QNames; a differently named removal can delete the matched state silently.
        merged = Merge(before,"<BuildState id='same' i:joinAction='Remove'/>");
        Require(States(merged).Length == 1 && States(merged)[0].GetAttribute("id") == "other" && Valid(merged),"Cross-QName removal no longer deletes the matching StrategicState.");
        merged = Merge("",repeated+"<BuildState id='same' State='build'/>");
        Require(States(merged).Length == 1 && States(merged)[0].LocalName == "BuildState" && Valid(merged),"Cross-QName repeated-key replacement changed.");
        // Reborn: identical duplicates and command pairs still require separate source-bound admission; broad uniqueness guards are not weakened by this review.
        foreach (var bodies in new[] { (Before:"",After:repeated+repeated),(Before:before,After:remove+replacement),(Before:before,After:"<BuildState id='same' i:joinAction='Remove'/>") })
        {
            byte[] raw = Source("<Asset id='Base'>"+bodies.Before+"</Asset><Asset id='Derived' inheritFrom='Base'>"+bodies.After+"</Asset>");
            foreach (var result in new[] { SdkSelfAttributeInheritance.Apply(schemas,raw,childRemoval:true),SdkSelfAttributeInheritance.Apply(schemas,raw,upgrades:true),SdkSelfAttributeInheritance.Apply(schemas,raw,upgrades:true,objectCreationMarkers:true) })
                Require(result.Bytes == null && result.Evidence.ProcessedSha256 == null && result.Evidence.Overlays.Length == 0 && result.Evidence.Removals.Length == 0 && result.Evidence.ExpressionPreparation == null,"Sibling identity review widened admission or leaked partial evidence.");
        }
        Console.WriteLine("SDK sibling identity semantics self-test: OK (identical coalescing, conflicting last-write-wins, ordered remove/re-add vs overlay, reverse order, cross-QName deletion/replacement and unchanged atomic refusals)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: inspect unchanged core behavior with owned unvalidated nodes after declaration-time marker consumption. */
        //-------------------------------------------------------------------------------------------------
        XmlElement Merge(string beforeBody,string afterBody)
        {
            XmlDocument xml = new() { XmlResolver = null };
            xml.LoadXml(Encoding.UTF8.GetString(Source("<Asset id='Base'>"+beforeBody+"</Asset><Asset id='Derived'>"+afterBody+"</Asset>")));
            var assets = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
            return (XmlElement)NodeJoiner.Override(schemas,xml,assets[0],assets[1]);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: demonstrate that schema validation can pass after destructive keyed operations. */
        //-------------------------------------------------------------------------------------------------
        bool Valid(XmlElement asset)
        {
            var xml = new XmlDocument { XmlResolver = null,Schemas = schemas };
            xml.LoadXml(Encoding.UTF8.GetString(Source(asset.OuterXml)));
            bool valid = true; xml.Validate((_,_) => valid = false); return valid;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain direct repeated-state order as a semantic witness. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement[] States(XmlElement asset) => asset.ChildNodes.OfType<XmlElement>().ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned state-operation fixtures without altering official XML or schema sources. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:i='uri:ea.com:eala:asset:instance'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on changed core lookup or ordering rather than silently expanding source admission. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
