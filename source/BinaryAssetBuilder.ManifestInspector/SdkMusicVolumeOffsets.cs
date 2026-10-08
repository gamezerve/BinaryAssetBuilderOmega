using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove only bounded integer offsets in direct MusicTrack.Volume slots, never general audio arithmetic or EA evaluator equivalence.
internal static class SdkMusicVolumeOffsets
{
    internal const string Name = "diagnostic-music-volume-offsets-v1";
    private const string Ea = "uri:ea.com:eala:asset";
    private static readonly Regex Form = new("\\A=\\$(?<name>[A-Za-z_][A-Za-z0-9_]{0,127})[ \\t]+(?<op>[+-])[ \\t]+(?<offset>[0-9]{1,3})\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));
    // Reborn: computation evidence records the authored slot and resolved operands, not native serializer or game proof.
    internal sealed record Witness(string AssetId,string Expression,string Definition,string BaseValue,string Operator,string Offset,string Result);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact schema Percentage Volume and one literal integer offset with all operands/result within 0..100. */
    //-------------------------------------------------------------------------------------------------
    internal static Witness Evaluate(XmlNode slot,IReadOnlyDictionary<string,string> literals,XmlSchemaSet schemas)
    {
        if (slot is not XmlAttribute attribute || attribute.NamespaceURI.Length != 0 || attribute.Name != "Volume"
            || attribute.OwnerElement is not { LocalName:"MusicTrack",NamespaceURI:Ea,Prefix:"" } owner
            || owner.ParentNode is not XmlElement { LocalName:"AssetDeclaration",NamespaceURI:Ea }
            || !schemas.IsCompiled || schemas.GlobalTypes[new XmlQualifiedName("MusicTrack",Ea)] is not XmlSchemaComplexType type
            || type.AttributeWildcard != null || type.AttributeUses[new XmlQualifiedName("Volume")] is not XmlSchemaAttribute field
            || field.AttributeSchemaType?.QualifiedName != new XmlQualifiedName("Percentage",Ea))
            throw new InvalidDataException("Music offsets require a direct MusicTrack.Volume with exact Percentage schema type.");
        string expression = attribute.Value;
        if (expression.Length > 160) throw new InvalidDataException("Music offset expression exceeds 160 characters.");
        Match match = Form.Match(expression);
        if (!match.Success || !literals.TryGetValue(match.Groups["name"].Value,out string? basis))
            throw new InvalidDataException("Only exact =$NAME +/- integer music volume offsets with visible literal definitions are admitted.");
        int left = Number(basis),right = Number(match.Groups["offset"].Value);
        int result = match.Groups["op"].Value == "+" ? left+right : left-right;
        if (result is < 0 or > 100) throw new InvalidDataException("Music volume offset result must remain within 0..100.");
        return new(owner.GetAttribute("id"),expression,match.Groups["name"].Value,basis,match.Groups["op"].Value,match.Groups["offset"].Value,result.ToString(CultureInfo.InvariantCulture));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: withhold music computation authority unless every authored owner retains the exact calculated Volume after actual Core inheritance. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument output,Witness[] witnesses)
    {
        foreach (var witness in witnesses)
        {
            var owners = output.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(asset => asset.NamespaceURI == Ea && asset.LocalName == "MusicTrack" && asset.GetAttribute("id") == witness.AssetId).ToArray();
            if (owners.Length != 1 || owners[0].GetAttribute("Volume") != witness.Result)
                throw new InvalidDataException("Actual Core music owner differs from the calculated Volume offset.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject floats, units, signs, locale syntax and oversized operands rather than guessing Percentage conversion semantics. */
    //-------------------------------------------------------------------------------------------------
    private static int Number(string value)
    {
        if (value.Length is < 1 or > 3 || !value.All(character => character is >= '0' and <= '9')
            || !int.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out int result) || result > 100)
            throw new InvalidDataException("Music volume operands require literal integers within 0..100.");
        return result;
    }
}
