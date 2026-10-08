using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize sound singleton conflicts without normalizing reference XML or expanding graph admission.
internal static class SdkSoundSingletonSemanticsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin ordered field overlay, inherited retention and validation-hidden conflicts against the reviewed EP1 schema and unchanged Core. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null;
        var review = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(review.SchemaAdmitted && schemas != null,"Reviewed EP1 schema required for singleton characterization.");
        foreach (var item in new[] {
            (Child:"PitchShift",FirstLow:"-10",FirstHigh:"-5",LastLow:"-1",LastHigh:"1"),
            (Child:"NonInterruptibleTime",FirstLow:"0.0s",FirstHigh:"0.5s",LastLow:"0.0s",LastHigh:"0.8s") })
        {
            string first = $"<{item.Child} Low='{item.FirstLow}' High='{item.FirstHigh}'/>";
            string last = $"<{item.Child} Low='{item.LastLow}' High='{item.LastHigh}'/>";
            Require(!Valid(Source("<AudioEvent id='Owner'>"+first+last+"</AudioEvent>")),"Raw conflicting singleton pair unexpectedly validates.");
            XmlElement merged = Merge("",first+last);
            Check(merged,item.Child,item.LastLow,item.LastHigh);
            Require(Valid(Source(merged.OuterXml)),"Folded complete singleton fails reviewed EP1 schema.");
            Check(Merge("",last+first),item.Child,item.FirstLow,item.FirstHigh);
            Check(Merge("",first+first),item.Child,item.FirstLow,item.FirstHigh);
            // Reborn: omitted fields survive from earlier occurrences; keeping only the last child is not equivalent to Core overlay.
            string partial = $"<{item.Child} High='{item.LastHigh}'/>";
            Check(Merge("",first+partial),item.Child,item.FirstLow,item.LastHigh);
            Check(Merge(first,partial),item.Child,item.FirstLow,item.LastHigh);
            Check(Merge(first,last),item.Child,item.LastLow,item.LastHigh);
            Check(Merge(first,last+partial),item.Child,item.LastLow,item.LastHigh);
            // Reborn: schema-valid field combinations may still discard authored earlier values or invert a range.
            var inverted = Merge("",$"<{item.Child} Low='{item.LastHigh}' High='{item.FirstLow}'/>");
            Check(inverted,item.Child,item.LastHigh,item.FirstLow);
            Require(Valid(Source(inverted.OuterXml)),"Schema now rejects reversed-range fixture; update diagnostic assumptions explicitly.");
            byte[] raw = Source("<AudioEvent id='Base'/><AudioEvent id='Owner' inheritFrom='Base'>"+first+last+"</AudioEvent>");
            var refused = SdkSelfAttributeInheritance.Apply(schemas!,raw,audioTrees:true);
            Require(refused.Bytes == null && refused.Evidence.ProcessedSha256 == null && refused.Evidence.Overlays.Length == 0,
                "Singleton characterization expanded graph preparation or leaked partial output.");
        }
        var ordered = Merge("<PitchShift Low='-2' High='2'/><NonInterruptibleTime Low='0.0s' High='0.2s'/>",
            "<NonInterruptibleTime High='0.8s'/><PitchShift Low='-10' High='-5'/><PitchShift Low='-1' High='1'/>");
        Require(ordered.ChildNodes.OfType<XmlElement>().Select(child => child.LocalName).SequenceEqual(new[] { "PitchShift","NonInterruptibleTime" }),
            "Core no longer preserves schema sequence for matched singleton fields.");
        Require(Valid(Source(ordered.OuterXml)),"Merged mixed singleton order fails schema.");
        Console.WriteLine("SDK sound singleton semantics self-test: OK (reviewed EP1 schema, raw duplicate refusal, ordered last-field overlay, reverse order, inherited/partial retention, schema-hidden range conflicts, mixed-child order and unchanged atomic admission refusal; no native/game equivalence)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: execute actual unvalidated Core overlay on owned audio nodes only, never source normalization or native emission. */
        //-------------------------------------------------------------------------------------------------
        XmlElement Merge(string before,string after)
        {
            XmlDocument xml = new() { XmlResolver = null };
            xml.LoadXml(Encoding.UTF8.GetString(Source("<AudioEvent id='Base'>"+before+"</AudioEvent><AudioEvent id='Owner'>"+after+"</AudioEvent>")));
            var owners = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
            return (XmlElement)NodeJoiner.Override(schemas!,xml,owners[0],owners[1]);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: check isolated XML validity separately from semantic field retention or dependency/native compatibility. */
        //-------------------------------------------------------------------------------------------------
        bool Valid(byte[] bytes) => SdkEffectiveSchema.Bind(schemas!,"owned-sound-singleton.xml",bytes).XmlValidated;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete explicit singleton attribute sets rather than accepting a valid but lossy folded result. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(XmlElement owner,string child,string low,string high)
    {
        var nodes = owner.ChildNodes.OfType<XmlElement>().ToArray();
        Require(nodes.Length == 1 && nodes[0].LocalName == child && nodes[0].Attributes.Count == 2
            && nodes[0].GetAttribute("Low") == low && nodes[0].GetAttribute("High") == high,"Core singleton field projection differs from expected ordered overlay.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create owned in-memory audio fixtures without changing any reference XML or schema. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop when characterized sound singleton semantics or closed admission change. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
