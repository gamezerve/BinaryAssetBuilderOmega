using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit only the two previously proved complete sound bodies under a separate opt-in stage.
internal static class SdkSoundSingletons
{
    private const string Ea = "uri:ea.com:eala:asset";
    // Reborn: source-local normalization witnesses never imply native byte or dependency/game compatibility.
    internal sealed record Witness(string OwnerId,string BaseId,string ChildName,int Before,int After);
    internal sealed record Plan(XmlElement Expected,Witness Witness);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema-check known singleton pairs, prove complete actual Core output and replace only owner children in memory while retaining inheritance/root fields. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan[] Normalize(XmlSchemaSet schemas,XmlDocument document)
    {
        var assets = document.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        if (document.SelectNodes(".//*")!.Count > 16384) throw new InvalidDataException("Sound singleton pre-normalization tree bound exceeded.");
        List<Plan> plans = new();
        foreach (var owner in assets.Where(node => node.LocalName == "AudioEvent" && node.GetAttribute("id") is "BuildingInfiltrated1" or "StreetLampCrush"))
        {
            string childName = owner.GetAttribute("id") == "BuildingInfiltrated1" ? "PitchShift" : "NonInterruptibleTime";
            var children = owner.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName == childName).ToArray();
            if (children.Length <= 1) continue;
            if (plans.Count >= 2 || children.Length != 2 || schemas.GlobalTypes[new XmlQualifiedName("AudioEvent",Ea)] is not XmlSchemaComplexType type
                || type.AttributeWildcard != null || type.ContentTypeParticle is not XmlSchemaSequence sequence
                || sequence.Items.OfType<XmlSchemaElement>().SingleOrDefault(node => node.QualifiedName == new XmlQualifiedName(childName,Ea)) is not XmlSchemaElement declaration
                || declaration.MinOccurs != 0 || declaration.MaxOccurs != 1 || declaration.ElementSchemaType is not XmlSchemaComplexType leaf
                || leaf.QualifiedName != new XmlQualifiedName(childName == "PitchShift" ? "RealRange" : "TimeRange",Ea)
                || leaf.ContentType != XmlSchemaContentType.Empty || leaf.AttributeWildcard != null)
                throw new InvalidDataException("Exact reviewed sound singleton sequence/schema required.");
            var basis = assets.Single(node => node.LocalName == "AudioEvent" && node.GetAttribute("id") == "BaseSoundEffect");
            // Reborn: reject unreviewed root attributes before the extra bounded Core probe; the later whole-source gate remains authoritative too.
            foreach (var source in new[] { basis,owner })
                foreach (XmlAttribute field in source.Attributes)
                    if (field.NamespaceURI != "http://www.w3.org/2000/xmlns/" && (field.NamespaceURI.Length != 0 || type.AttributeUses[new XmlQualifiedName(field.Name)] is not XmlSchemaAttribute))
                        throw new InvalidDataException("Exact schema root attributes required before sound singleton Core proof.");
            var expected = SdkSoundOwnerReview.Predict(basis,owner);
            var joining = (XmlElement)owner.CloneNode(true); joining.RemoveAttribute("inheritFrom");
            var actual = (XmlElement)NodeJoiner.Override(schemas,document,basis,joining);
            SdkSoundOwnerReview.Verify(expected,actual);
            // Reborn: retain every original owner root field/handle and all unrelated document metadata; only the proved complete direct child projection is replaced.
            foreach (XmlNode node in owner.ChildNodes.OfType<XmlElement>().ToArray()) owner.RemoveChild(node);
            foreach (XmlElement node in expected.ChildNodes.OfType<XmlElement>()) owner.AppendChild(document.ImportNode(node,true));
            plans.Add(new(expected,new(owner.GetAttribute("id"),"BaseSoundEffect",childName,2,1)));
        }
        return plans.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify the complete final owner after the real delegated merge and temporary base removal before publishing any singleton witness. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument output,Plan[] plans)
    {
        foreach (var plan in plans)
        {
            var owner = output.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.LocalName == "AudioEvent" && node.GetAttribute("id") == plan.Witness.OwnerId);
            SdkSoundOwnerReview.Verify(plan.Expected,owner);
        }
    }
}
