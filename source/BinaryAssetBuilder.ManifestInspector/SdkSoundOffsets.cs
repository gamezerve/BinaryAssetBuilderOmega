using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit only typed bounded integer audio slots, not a general expression engine or native audio semantics.
internal static class SdkSoundOffsets
{
    internal const string Name = "diagnostic-sound-offsets-v1";
    private const string Ea = "uri:ea.com:eala:asset";
    private static readonly Regex Offset = new("\\A=\\$(?<name>[A-Za-z_][A-Za-z0-9_]{0,127})[ \\t]*(?<op>[+-])[ \\t]*(?<offset>[0-9]{1,4})\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));
    private static readonly Regex Constant = new("\\A=(?<value>-?[0-9]{1,4})\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));
    // Reborn: preserve exact original slot, schema type and operands independently of serialized assets or native type identities.
    internal sealed record Witness(string AssetType,string AssetId,string? ChildName,string Field,string SchemaType,string Expression,string? Definition,string? BaseValue,string? Operator,string? Offset,string Result);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: evaluate one typed integer literal or visible-definition offset within field-specific diagnostic bounds. */
    //-------------------------------------------------------------------------------------------------
    internal static Witness Evaluate(XmlNode slot,IReadOnlyDictionary<string,string> literals,XmlSchemaSet schemas)
    {
        if (slot is not XmlAttribute attribute || attribute.NamespaceURI.Length != 0 || attribute.OwnerElement == null || !schemas.IsCompiled)
            throw new InvalidDataException("Sound arithmetic requires a typed unqualified attribute.");
        XmlElement node = attribute.OwnerElement,owner = node;
        string? child = null;
        if (node.LocalName == "PitchShift" && node.ParentNode is XmlElement parent) { child = "PitchShift"; owner = parent; }
        if (owner.NamespaceURI != Ea || owner.Prefix.Length != 0 || node.NamespaceURI != Ea || node.Prefix.Length != 0
            || owner.LocalName is not ("AudioEvent" or "AudioEventOverridable")
            || owner.ParentNode is not XmlElement { LocalName:"AssetDeclaration",NamespaceURI:Ea }
            || schemas.GlobalTypes[new XmlQualifiedName(owner.LocalName,Ea)] is not XmlSchemaComplexType type || type.AttributeWildcard != null)
            throw new InvalidDataException("Sound arithmetic requires direct AudioEvent/AudioEventOverridable or its singleton PitchShift.");
        (string schemaType,int minimum,int maximum) = (child,attribute.Name) switch
        {
            (null,"Volume") => ("Percentage",0,200),
            (null,"MinVolume") => ("Percentage",0,100),
            (null,"VolumeShift") => ("Percentage",-100,100),
            (null,"MinRange" or "MaxRange") => ("SageReal",0,2048),
            ("PitchShift","Low" or "High") => ("SageReal",-12,12),
            _ => throw new InvalidDataException("Sound arithmetic field is outside the reviewed typed subset.")
        };
        if (child != null)
        {
            if (type.ContentTypeParticle is not XmlSchemaSequence sequence
                || sequence.Items.OfType<XmlSchemaElement>().SingleOrDefault(item => item.QualifiedName == new XmlQualifiedName(child,Ea)) is not { MinOccurs:0,MaxOccurs:1 } element
                || element.ElementSchemaType is not XmlSchemaComplexType range || range.QualifiedName != new XmlQualifiedName("RealRange",Ea)
                || range.ContentType != XmlSchemaContentType.Empty || range.AttributeWildcard != null)
                throw new InvalidDataException("Sound arithmetic PitchShift requires the exact empty singleton RealRange.");
            type = range;
        }
        if (type.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute field
            || field.AttributeSchemaType?.QualifiedName != new XmlQualifiedName(schemaType,Ea))
            throw new InvalidDataException("Sound arithmetic requires the exact schema-authored Percentage/SageReal type.");
        string expression = attribute.Value;
        if (expression.Length > 160) throw new InvalidDataException("Sound arithmetic expression exceeds 160 characters.");
        string? definition = null,basis = null,op = null,offset = null; int result;
        Match constant = Constant.Match(expression);
        if (constant.Success) result = Number(constant.Groups["value"].Value,minimum,maximum);
        else
        {
            Match match = Offset.Match(expression);
            if (!match.Success || !literals.TryGetValue(match.Groups["name"].Value,out basis))
                throw new InvalidDataException("Only integer constants or one visible =$NAME +/- unsigned integer sound offset are admitted.");
            definition = match.Groups["name"].Value; op = match.Groups["op"].Value; offset = match.Groups["offset"].Value;
            int left = Number(basis,minimum,maximum),right = Number(offset,0,1000);
            result = op == "+" ? left+right : left-right;
            if (result < minimum || result > maximum) throw new InvalidDataException("Sound arithmetic result exceeds its field's diagnostic bounds.");
        }
        return new(owner.LocalName,owner.GetAttribute("id"),child,attribute.Name,schemaType,expression,definition,basis,op,offset,result.ToString(CultureInfo.InvariantCulture));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: trust no arithmetic witness unless the actual final Core owner retains its exact calculated attribute or singleton pitch field. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument output,Witness[] witnesses)
    {
        var owners = output.DocumentElement!.ChildNodes.OfType<XmlElement>().GroupBy(node => node.LocalName+":"+node.GetAttribute("id"),StringComparer.Ordinal).ToDictionary(group => group.Key,group => group.ToArray(),StringComparer.Ordinal);
        foreach (var witness in witnesses)
        {
            if (!owners.TryGetValue(witness.AssetType+":"+witness.AssetId,out var matches) || matches.Length != 1 || matches[0].NamespaceURI != Ea)
                throw new InvalidDataException("Sound arithmetic final owner is absent or ambiguous.");
            XmlElement node = matches[0];
            if (witness.ChildName != null)
            {
                var children = node.ChildNodes.OfType<XmlElement>().Where(child => child.LocalName == witness.ChildName && child.NamespaceURI == Ea).ToArray();
                if (children.Length != 1) throw new InvalidDataException("Sound arithmetic final PitchShift is absent or ambiguous.");
                node = children[0];
            }
            if (!node.HasAttribute(witness.Field) || node.GetAttribute(witness.Field) != witness.Result)
                throw new InvalidDataException("Actual Core sound field differs from the calculated integer result.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use invariant signed decimal integers only; reject floats, units, percent signs, exponent syntax and out-of-scope values. */
    //-------------------------------------------------------------------------------------------------
    private static int Number(string value,int minimum,int maximum)
    {
        string digits = value.StartsWith('-') ? value[1..] : value;
        if (digits.Length is < 1 or > 4 || !digits.All(character => character is >= '0' and <= '9')
            || !int.TryParse(value,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out int result) || result < minimum || result > maximum)
            throw new InvalidDataException("Sound arithmetic operand requires a bounded invariant signed integer.");
        return result;
    }
}
