using System.Buffers.Binary;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: authoring capabilities come from the official schema intersected with reviewed processor ABI descriptors, not a trial-specific six-field whitelist.
internal static class GameObjectScalarContract
{
    internal sealed record Binding(Ep1GameObjectScalarTokens.Field Native,string? Default,string ClrType);
    private static readonly Lazy<Binding[]> bindings=new(Create);
    internal static IReadOnlyList<Binding> Bindings=>bindings.Value;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: optional root attributes use their official/default native value without materializing every optional field into authored XML. */
    //-------------------------------------------------------------------------------------------------
    internal static string Literal(XmlDocument source,string name){var binding=Bindings.Single(b=>b.Native.Name==name);return source.DocumentElement!.HasAttribute(name)?source.DocumentElement.GetAttribute(name):binding.Default??(binding.ClrType=="SageBool"?"false":"0");}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: one grouped regression exercises each supported primitive codec through the real compiler, not a ladder of gameplay field trials. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source)
    {
        // Reborn: distinct same-type sentinels detect swapped offsets; flip every boolean from its actual original/default state.
        int index=0;
        foreach(var binding in Bindings){index++;string text=binding.ClrType switch{"Single"=>(index+0.125f).ToString("R",CultureInfo.InvariantCulture),"Int32" or "UInt32" or "UInt16" or "Byte"=>index.ToString(CultureInfo.InvariantCulture),"SageBool"=>XmlConvert.ToBoolean(Literal(source,binding.Native.Name).Trim())?"false":"true","Angle"=>"90d","Time"=>(1500+37*index).ToString(CultureInfo.InvariantCulture)+"ms","Duration"=>"1001",_=>throw new InvalidDataException("Missing scalar probe codec.")};source.DocumentElement!.SetAttribute(binding.Native.Name,text);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require a schema attribute and matching concrete native primitive for every existing scalar processor descriptor. */
    //-------------------------------------------------------------------------------------------------
    private static Binding[] Create()
    {
        var schemas=ConscriptSourceInput.Schemas();var type=(XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName("GameObject","uri:ea.com:eala:asset")]!;
        return Ep1GameObjectScalarTokens.Fields.Select(field=>
        {
            if(type.AttributeUses[new XmlQualifiedName(field.Name)] is not XmlSchemaAttribute attribute)throw new InvalidDataException("Scalar ABI has no official attribute: "+field.Name);
            string expected=field.Type switch{0x645864F3=>"Single",0x23E650B8=>"Int32",0x763177A4=>"UInt32",0x43501092=>"UInt16",0x30916B96=>"Byte",0x77E3F4F7=>"SageBool",0x4E16EA4D=>"Angle",0x998669E8=>"Time",0x6F6D1AE7=>"Duration",_=>throw new InvalidDataException("Scalar wire codec not implemented: "+field.Name)};
            if(typeof(SageBinaryData.GameObject).GetField(field.Name)?.FieldType.Name!=expected)throw new InvalidDataException("Scalar schema/native codec mismatch: "+field.Name);
            return new Binding(field,attribute.FixedValue??attribute.DefaultValue,expected);
        }).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate finite/native-representable values and canonicalize XML boolean spellings accepted by XSD but not legacy bool.Parse. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source)
    {
        foreach(var binding in Bindings)
        {
            Bytes(source,binding);
            if(binding.ClrType=="SageBool" && source.DocumentElement!.HasAttribute(binding.Native.Name))source.DocumentElement.SetAttribute(binding.Native.Name,XmlConvert.ToBoolean(source.DocumentElement.GetAttribute(binding.Native.Name).Trim())?"true":"false");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: remove only supported primitive differences from the structural comparison; modules, references, strings and allocations stay independently gated. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var binding in Bindings){string name=binding.Native.Name;candidate.DocumentElement!.RemoveAttribute(name);if(model.DocumentElement!.HasAttribute(name))candidate.DocumentElement.SetAttribute(name,model.DocumentElement.GetAttribute(name));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every authored/default scalar at its actual ABI width, then restore baseline bytes for whole-graph stock ownership validation. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyAndRestore(XmlDocument source,byte[] actual,byte[] restored,byte[] baseline)
    {
        Verify(source,actual,0);
        foreach(var binding in Bindings){var field=binding.Native;baseline.AsSpan(field.Offset,field.Width).CopyTo(restored.AsSpan(field.Offset,field.Width));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialization rechecks source-to-native primitive values independently of repeat-build determinism. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,int prefix)
    {
        foreach(var binding in Bindings){var field=binding.Native;if(prefix<0 || prefix+field.Offset>native.Length-field.Width || !native.AsSpan(prefix+field.Offset,field.Width).SequenceEqual(Bytes(source,binding)))throw new InvalidDataException("GameObject scalar native value differs: "+field.Name);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected metadata keeps existing ownership/tables and writes actual typed scalar encodings, including packed one/two-byte values. */
    //-------------------------------------------------------------------------------------------------
    internal static void Patch(XmlDocument source,byte[] target,int prefix)
    {
        foreach(var binding in Bindings)Bytes(source,binding).CopyTo(target.AsSpan(prefix+binding.Native.Offset,binding.Native.Width));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode schema-validated scalar literals without invented gameplay ranges; nonfinite/overflow values fail before native allocation. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Bytes(XmlDocument source,Binding binding)
    {
        var field=binding.Native;string text=Literal(source,field.Name).Trim();var bytes=new byte[field.Width];
        try
        {
            switch(binding.ClrType)
            {
                case "Single":BinaryPrimitives.WriteSingleLittleEndian(bytes,Finite(text));break;
                case "Int32":BinaryPrimitives.WriteInt32LittleEndian(bytes,int.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture));break;
                case "UInt32":BinaryPrimitives.WriteUInt32LittleEndian(bytes,uint.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture));break;
                case "UInt16":BinaryPrimitives.WriteUInt16LittleEndian(bytes,ushort.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture));break;
                case "Byte":bytes[0]=byte.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture);break;
                case "SageBool":bytes[0]=XmlConvert.ToBoolean(text)?(byte)1:(byte)0;break;
                case "Angle":{float value=text.EndsWith('d')?Finite(text[..^1])*Marshaler.RADS_PER_DEGREE:Finite(text.EndsWith('r')?text[..^1]:text);if(!float.IsFinite(value))throw new OverflowException();BinaryPrimitives.WriteSingleLittleEndian(bytes,value);break;}
                case "Time":{float value=text.EndsWith("ms",StringComparison.Ordinal)?Finite(text[..^2])*0.001f:Finite(text.EndsWith('s')?text[..^1]:text);if(!float.IsFinite(value))throw new OverflowException();BinaryPrimitives.WriteSingleLittleEndian(bytes,value);break;}
                case "Duration":BinaryPrimitives.WriteUInt32LittleEndian(bytes,checked((uint)Math.Ceiling(uint.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture)*Marshaler.LOGICFRAMES_PER_MSEC_REAL)));break;
                default:throw new InvalidDataException("Unknown scalar codec.");
            }
        }
        catch(Exception error)when(error is FormatException or OverflowException){throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid schema/native scalar "+field.Name+": "+text,error),(XmlNode?)source.DocumentElement!.GetAttributeNode(field.Name)??source.DocumentElement);}
        return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: XSD float lexical validation alone can allow NaN/INF; those cannot enter this finite-value authoring capability. */
    //-------------------------------------------------------------------------------------------------
    private static float Finite(string text){float value=float.Parse(text,NumberStyles.Float,CultureInfo.InvariantCulture);if(!float.IsFinite(value))throw new FormatException("Finite float required.");return value;}
}
