using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: pin complete owner projection checks without depending on external game/source files in default tests.
internal static class SdkSoundOwnerReviewSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove whole explicit field/leaf/order retention and reject dropped, reordered, renamed or altered owner payloads. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null;
        var review = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        if (!review.SchemaAdmitted || schemas == null) throw new InvalidDataException("Reviewed schema required.");
        foreach (var item in new[] {
            (Id:"BuildingInfiltrated1",Body:"<PitchShift Low='-10' High='-5'/><InitialDelay Low='0' High='50'/><PitchShift Low='-1' High='1'/><Sound>WBSpy_infiltrateBldgP</Sound>"),
            (Id:"StreetLampCrush",Body:"<NonInterruptibleTime Low='0.0s' High='0.5s'/><PitchShift Low='-10' High='10'/><NonInterruptibleTime Low='0.0s' High='0.8s'/><Delay Low='0' High='100'/><Sound>WBStreetLamp_crushA</Sound><Sound>WBStreetLamp_crushB</Sound><Sound>WBStreetLamp_crushC</Sound>") })
        {
            var xml = Parse("<AudioEvent id='BaseSoundEffect' Volume='100' MinVolume='0' Priority='NORMAL' MinRange='300' MaxRange='1000'/><AudioEvent id='"+item.Id+"' inheritFrom='AudioEvent:BaseSoundEffect' Volume='55' Type='WORLD SHROUDED EVERYONE'>"+item.Body+"</AudioEvent>");
            var nodes = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
            var expected = SdkSoundOwnerReview.Predict(nodes[0],nodes[1]);
            var owner = (XmlElement)nodes[1].CloneNode(true); owner.RemoveAttribute("inheritFrom");
            var actual = (XmlElement)NodeJoiner.Override(schemas,xml,nodes[0],owner);
            SdkSoundOwnerReview.Verify(expected,actual);
            if (actual.GetAttribute("Volume") != "55" || actual.GetAttribute("MinVolume") != "0" || actual.GetAttribute("Priority") != "NORMAL") throw new InvalidDataException("Root override/inherited retention differs.");
            if (!SdkEffectiveSchema.Bind(schemas,"owned-sound-owner.xml",System.Text.Encoding.UTF8.GetBytes(Parse(actual.OuterXml).OuterXml)).XmlValidated) throw new InvalidDataException("Complete owned projection fails schema.");
            foreach (var mutate in new Action<XmlElement>[] {
                node => node.RemoveAttribute("MinVolume"),
                node => node.SetAttribute("Volume","56"),
                node => node.SetAttribute("Unexpected","1"),
                node => node.RemoveChild(node.FirstChild!),
                node => node.AppendChild(node.FirstChild!.CloneNode(true)),
                node => node.AppendChild(node.FirstChild!),
                node => node.ChildNodes.OfType<XmlElement>().Last().InnerText = "WrongReference",
                node => ((XmlElement)node.FirstChild!).SetAttribute("High","999"),
                node => node.AppendChild(node.OwnerDocument!.CreateTextNode("unexpected")),
                node => ((XmlElement)node.FirstChild!).AppendChild(node.OwnerDocument!.CreateElement("Nested","uri:ea.com:eala:asset")) })
            {
                var changed = (XmlElement)actual.CloneNode(true); mutate(changed);
                Refuses(() => SdkSoundOwnerReview.Verify(expected,changed));
            }
            var alteredOwner = (XmlElement)nodes[1].CloneNode(true);
            alteredOwner.ChildNodes.OfType<XmlElement>().Last().InnerText = "ChangedAuthoredReference";
            Refuses(() => SdkSoundOwnerReview.Predict(nodes[0],alteredOwner));
            alteredOwner = (XmlElement)nodes[1].CloneNode(true); alteredOwner.SetAttribute("Volume","=$X+1");
            Refuses(() => SdkSoundOwnerReview.Predict(nodes[0],alteredOwner));
            alteredOwner = (XmlElement)nodes[1].CloneNode(true); alteredOwner.SetAttribute("inheritFrom","AudioEvent:OtherBase");
            Refuses(() => SdkSoundOwnerReview.Predict(nodes[0],alteredOwner));
            var populatedBase = (XmlElement)nodes[0].CloneNode(true); populatedBase.AppendChild(xml.CreateElement("PitchShift","uri:ea.com:eala:asset"));
            Refuses(() => SdkSoundOwnerReview.Predict(populatedBase,nodes[1]));
        }
        Console.WriteLine("SDK sound owner review self-test: OK (complete explicit root/child projection, inherited retention, actual Core/schema agreement, field/reference/order/multiplicity/nested-payload tamper refusals and literal/base identity guards; no admission/native emission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct owned in-memory audio fixtures without reading or altering external source files. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(string body)
    {
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>"); return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require changed complete projections to fail rather than silently discarding unreviewed fields. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); } catch (InvalidDataException) { return; }
        throw new InvalidDataException("Changed sound owner projection unexpectedly accepted.");
    }
}
