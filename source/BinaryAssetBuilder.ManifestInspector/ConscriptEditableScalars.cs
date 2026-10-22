using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: retain the original six-value regression vectors and existing nested-owner ABI bindings; root primitive authoring uses GameObjectScalarContract.
internal static class ConscriptEditableScalars
{
    // Reborn: Min/Max are historical test vectors only, not schema/native authoring limits.
    internal sealed record Field(string Path, string Attribute, float Min, float Max, bool Integer = false);
    internal static readonly Field[] Fields = {
        new(".", "BuildTime", 0.1f, 600),
        new("a:ObjectResourceInfo/a:BuildCost", "Amount", 0, 100000, true),
        new("a:Body/a:ActiveBody", "MaxHealth", 1, 100000),
        new("a:VisionInfo", "VisionRange", 1, 5000),
        new("a:VisionInfo", "ShroudClearingRange", 1, 5000),
        new("a:LocomotorSet", "Speed", 1, 500)
    };
    // Reborn: optional nested primitive defaults come from the same official schema on every build/receipt path.
    private static readonly Lazy<Dictionary<string,(string? Default,bool Required)>> defaults=new(()=>{
        var schemas=ConscriptSourceInput.Schemas();var result=new Dictionary<string,(string?,bool)>();foreach(var field in Fields.Where(f=>f.Path!="."&&f.Attribute!="MaxHealth")){
            string name=field.Path.Contains("BuildCost",StringComparison.Ordinal)?"MoneyTransaction":field.Path.Contains("VisionInfo",StringComparison.Ordinal)?"VisionInfo":"LocomotorSet";
            var type=(XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName(name,"uri:ea.com:eala:asset")]!;var attribute=(XmlSchemaAttribute)type.AttributeUses[new XmlQualifiedName(field.Attribute)]!;
            result.Add(field.Attribute,(attribute.FixedValue??attribute.DefaultValue,attribute.Use==XmlSchemaUse.Required));
        }return result;
    });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use authored values or actual schema/native defaults without inventing stock-value equality or gameplay-specific bounds. */
    //-------------------------------------------------------------------------------------------------
    internal static string Literal(XmlDocument source,Field field)
    {
        if(field.Path==".")return GameObjectScalarContract.Literal(source,field.Attribute);var element=Element(source,field);
        if(field.Attribute=="MaxHealth")return ConscriptModuleScalarContract.Literal(element,field.Attribute,typeof(Ep1ActiveBodyModuleData));
        if(element.HasAttribute(field.Attribute))return element.GetAttribute(field.Attribute);var rule=defaults.Value[field.Attribute];
        if(rule.Required)throw SdkXmlDiagnostics.At(new InvalidDataException("Required scalar attribute is absent: "+field.Attribute),element);return rule.Default??"0";
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read actual authored/default finite scalars using the concrete native type, not historical gameplay trial ranges. */
    //-------------------------------------------------------------------------------------------------
    internal static float Read(XmlDocument source, Field field)
    {
        // Reborn: optional MaxHealth follows its official module default rather than requiring an explicit historical six-value trial attribute.
        string text = Literal(source,field);
        // Reborn: Min/Max remain historical regression vectors, not authoring limits; the schema and concrete native type define representability.
        if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float value)
            || !float.IsFinite(value)
            || field.Integer && !uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            throw SdkXmlDiagnostics.At(new InvalidDataException($"Conscript {field.Attribute} must be {(field.Integer ? "an unsigned 32-bit integer" : "a finite 32-bit real")} allowed by its schema."),Element(source,field).GetAttributeNode(field.Attribute)??(XmlNode)Element(source,field));
        return value;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a unique direct hierarchy is mandatory; missing/duplicate/replaced owners cannot pass by numeric normalization. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement Element(XmlDocument source, Field field)
    {
        var ns = new XmlNamespaceManager(source.NameTable); ns.AddNamespace("a", "uri:ea.com:eala:asset");
        var nodes = source.DocumentElement!.SelectNodes(field.Path, ns)!;
        if (nodes.Count != 1 || nodes[0] is not XmlElement element)
            throw new InvalidDataException("Conscript scalar owner absent/ambiguous: " + field.Attribute);
        return element;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit independent root pointer/layout anchors map reviewed source values to unchanged native allocation coordinates. */
    //-------------------------------------------------------------------------------------------------
    internal static int[] Offsets(byte[] native, int pointerBias = 0)
    {
        int Word(int at) => checked((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(native.AsSpan(at, 4)));
        int Pointer(int at) => checked(Word(at) - pointerBias);
        int resource = Pointer(416), costs = Pointer(resource + 4), body = Pointer(488), vision = Pointer(560), locomotor = Pointer(432);
        if (Word(resource) != 1 || Word(428) != 1) throw new InvalidDataException("Conscript scalar owner count changed.");
        int[] offsets = { 140, costs + 4, body + (int)Marshal.OffsetOf<Ep1ActiveBodyModuleData>(nameof(Ep1ActiveBodyModuleData.MaxHealth)), vision, vision + 4, locomotor };
        if (offsets.Distinct().Count() != Fields.Length || offsets.Any(at => at < 0 || at > native.Length - 4))
            throw new InvalidDataException("Conscript scalar offset bounds/alias.");
        return offsets;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: native floats and the unsigned cost field use their actual binary encodings rather than numeric byte searches. */
    //-------------------------------------------------------------------------------------------------
    internal static uint Bits(XmlDocument source, Field field) => field.Integer
        ? uint.Parse(Literal(source,field), CultureInfo.InvariantCulture)
        : BitConverter.SingleToUInt32Bits(Read(source, field));
}
