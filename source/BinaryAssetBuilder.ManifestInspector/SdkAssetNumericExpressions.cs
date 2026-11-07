using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: evaluate source numeric arithmetic without executing code or changing legacy diagnostic expression profiles.
internal sealed class SdkAssetNumericExpressions
{
    private readonly string text;
    private readonly IReadOnlyDictionary<string,string> definitions;
    private int at,depth;
    private SdkAssetNumericExpressions(string expression,IReadOnlyDictionary<string,string> definitions){text=expression;this.definitions=definitions;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse finite scalar arithmetic with visible case-sensitive definitions and ordinary operator precedence. */
    //-------------------------------------------------------------------------------------------------
    internal static string Evaluate(string expression,IReadOnlyDictionary<string,string> definitions)
    {
        if(expression.Length>512||!expression.StartsWith('='))throw new InvalidDataException("Numeric expression exceeds its syntax bound.");
        var parser=new SdkAssetNumericExpressions(expression[1..],definitions);double value=parser.Sum();parser.White();
        if(parser.at!=parser.text.Length||!double.IsFinite(value))throw new InvalidDataException("Invalid or non-finite numeric expression: "+expression);
        return value.ToString("G17",CultureInfo.InvariantCulture);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize integral decimal substitutions within the range of the declared SageInt or SageUnsignedInt field. */
    //-------------------------------------------------------------------------------------------------
    internal static string NormalizeInteger(XmlNode slot,string value,XmlSchemaSet? schemas)
    {
        if (schemas == null || slot is not XmlAttribute { OwnerElement: { } owner } attribute)
            return value;
        int dot = value.IndexOf('.');
        if (dot < 1 || value.AsSpan(dot+1).IndexOfAnyExcept('0') >= 0)
            return value;
        if (ElementType(owner,schemas) is not XmlSchemaComplexType type ||
            type.AttributeUses[new XmlQualifiedName(attribute.LocalName,attribute.NamespaceURI)] is not XmlSchemaAttribute declared ||
            declared.AttributeSchemaType?.QualifiedName is not { } schemaType ||
            schemaType.Namespace != BinaryAssetBuilder.Core.SageXml.SchemaSet.XmlNamespace)
            return value;
        var integerText = value.AsSpan(0,dot);
        return schemaType.Name switch
        {
            "SageInt" when int.TryParse(integerText,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out int signed) =>
                signed.ToString(CultureInfo.InvariantCulture),
            "SageUnsignedInt" when integerText[0] != '-' && uint.TryParse(integerText,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture,out uint unsigned) =>
                unsigned.ToString(CultureInfo.InvariantCulture),
            _ => value
        };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: follow actual schema particles before inheritance, where document validation may still lack required base fields. */
    //-------------------------------------------------------------------------------------------------
    private static XmlSchemaType? ElementType(XmlElement element,XmlSchemaSet schemas,int depth=0)
    {
        if (depth > 128 || element.ParentNode is not XmlElement parent)
            return null;
        var name = new XmlQualifiedName(element.LocalName,element.NamespaceURI);
        if (parent.LocalName == "AssetDeclaration" && parent.ParentNode is XmlDocument &&
            parent.NamespaceURI == BinaryAssetBuilder.Core.SageXml.SchemaSet.XmlNamespace)
            return (schemas.GlobalElements[name] as XmlSchemaElement)?.ElementSchemaType;
        if (ElementType(parent,schemas,depth+1) is not XmlSchemaComplexType parentType)
            return null;
        var types = new HashSet<XmlSchemaType>();
        Collect(parentType.ContentTypeParticle);
        return types.Count == 1 ? types.Single() : null;

        // Reborn: ambiguous declarations and wildcard-only fields do not authorize conversion.
        void Collect(XmlSchemaParticle? particle,int level=0)
        {
            if (level > 64) return;
            if (particle is XmlSchemaElement child && child.QualifiedName == name && child.ElementSchemaType is { } type)
                types.Add(type);
            else if (particle is XmlSchemaGroupBase group)
                foreach (var item in group.Items.OfType<XmlSchemaParticle>()) Collect(item,level+1);
            else if (particle is XmlSchemaGroupRef reference)
                Collect(reference.Particle,level+1);
        }
    }

    // Reborn: addition and subtraction bind after products and quotients.
    private double Sum(){double value=Product();while(true){White();if(Take('+'))value+=Product();else if(Take('-'))value-=Product();else return value;}}
    // Reborn: division by zero is diagnosed at the original source slot.
    private double Product(){double value=Atom();while(true){White();if(Take('*'))value*=Atom();else if(Take('/')){double divisor=Atom();if(divisor==0)throw new InvalidDataException("Division by zero in numeric expression.");value/=divisor;}else return value;}}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept numeric literals, visible numeric definitions, unary signs and bounded parentheses; reject calls and unit guessing. */
    //-------------------------------------------------------------------------------------------------
    private double Atom()
    {
        if(++depth>64)throw new InvalidDataException("Numeric expression nesting exceeds 64.");
        try{White();if(Take('+'))return Atom();if(Take('-'))return -Atom();if(Take('(')){double value=Sum();White();if(!Take(')'))throw new InvalidDataException("Missing closing parenthesis in numeric expression.");return value;}
            int start=at;if(Take('$')){start=at;while(at<text.Length&&(char.IsAsciiLetterOrDigit(text[at])||text[at]=='_'))at++;string name=text[start..at];if(!definitions.TryGetValue(name,out string? literal)||!double.TryParse(literal,NumberStyles.Float,CultureInfo.InvariantCulture,out double scalar)||!double.IsFinite(scalar))throw new InvalidDataException("Numeric definition is missing or not a finite scalar: $"+name);return scalar;}
            while(at<text.Length&&(char.IsAsciiDigit(text[at])||text[at]=='.'))at++;
            if(at<text.Length&&text[at] is 'e' or 'E'){at++;if(at<text.Length&&text[at] is '+' or '-')at++;while(at<text.Length&&char.IsAsciiDigit(text[at]))at++;}
            if(at==start||!double.TryParse(text[start..at],NumberStyles.Float,CultureInfo.InvariantCulture,out double number)||!double.IsFinite(number))throw new InvalidDataException("Unsupported numeric expression token near: "+text[start..]);return number;
        }finally{depth--;}
    }
    // Reborn: whitespace has no semantic effect between arithmetic tokens.
    private void White(){while(at<text.Length&&char.IsWhiteSpace(text[at]))at++;}
    private bool Take(char token){if(at>=text.Length||text[at]!=token)return false;at++;return true;}
}
