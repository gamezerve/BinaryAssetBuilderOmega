using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: fixed-size EP1 root enums, KindOf masks and weak IDs are authored through the official schema and reviewed native ABI without introducing another field whitelist.
internal static class GameObjectRootAttributeContract
{
    internal enum Codec { Enum, KindOf, WeakId }
    internal sealed record Binding(Ep1GameObjectScalarTokens.Field Native,string? Default,Type ClrType,Codec Wire);
    private static readonly Lazy<Binding[]> bindings=new(Create);
    internal static IReadOnlyList<Binding> Bindings=>bindings.Value;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate every schema-backed fixed root attribute before structural masking or native allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source){foreach(var binding in Bindings)Bytes(source,binding);}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixed root value differences are semantic authoring inputs; child records, dynamic strings and strong references remain visible to the structural gate. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var binding in Bindings){string name=binding.Native.Name;candidate.DocumentElement!.RemoveAttribute(name);if(model.DocumentElement!.HasAttribute(name))candidate.DocumentElement.SetAttribute(name,model.DocumentElement.GetAttribute(name));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise enums, two multiword masks and every weak-ID slot together so swapped fields cannot pass a same-value test. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source)
    {
        int weak=0;
        foreach(var binding in Bindings)
        {
            string value=binding.Wire switch
            {
                Codec.KindOf=>binding.Native.Name=="KindOf"?"OBSTACLE SCORE":"SELECTABLE CAN_ATTACK",
                Codec.WeakId=>"Reborn"+binding.Native.Name+"Probe"+(++weak).ToString(System.Globalization.CultureInfo.InvariantCulture),
                Codec.Enum=>DifferentEnum(binding,source),
                _=>throw new InvalidDataException("Unknown fixed root probe codec.")
            };
            source.DocumentElement!.SetAttribute(binding.Native.Name,value);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare each authored/default value at its exact ABI width, then restore the stock oracle for whole-graph validation. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyAndRestore(XmlDocument source,byte[] actual,byte[] restored,byte[] baseline)
    {
        Verify(source,actual,0);
        foreach(var binding in Bindings)baseline.AsSpan(binding.Native.Offset,binding.Native.Width).CopyTo(restored.AsSpan(binding.Native.Offset,binding.Native.Width));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stream publication rechecks fixed root values independently of compiler repeatability. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,int prefix)
    {
        foreach(var binding in Bindings){var field=binding.Native;if(prefix<0 || prefix+field.Offset>native.Length-field.Width || !native.AsSpan(prefix+field.Offset,field.Width).SequenceEqual(Bytes(source,binding)))throw new InvalidDataException("GameObject fixed root value differs: "+field.Name);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected metadata reuses reviewed ownership while replacing only exact fixed-value payload bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Patch(XmlDocument source,byte[] target,int prefix)
    {
        foreach(var binding in Bindings)Bytes(source,binding).CopyTo(target.AsSpan(prefix+binding.Native.Offset,binding.Native.Width));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: intersect the official GameObject attributes, concrete native fields and already stock-qualified EP1 root descriptors. */
    //-------------------------------------------------------------------------------------------------
    private static Binding[] Create()
    {
        var schemas=ConscriptSourceInput.Schemas();var schema=(XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName("GameObject","uri:ea.com:eala:asset")]!;var native=typeof(GameObject);
        return Ep1GameObjectRootValueTokens.ExtraFields.Select(field=>(Field:field,Attribute:schema.AttributeUses[new XmlQualifiedName(field.Name)] as XmlSchemaAttribute,Native:native.GetField(field.Name))).Where(item=>item.Attribute!=null).Select(item=>
        {
            Type type=item.Native?.FieldType??throw new InvalidDataException("Fixed root ABI has no concrete field: "+item.Field.Name);Codec codec=type==typeof(KindOfBitFlags)?Codec.KindOf:type.IsEnum?Codec.Enum:type.IsGenericType && type.GetGenericTypeDefinition().Name=="TypedAssetId`1"?Codec.WeakId:throw new InvalidDataException("Fixed root codec not implemented: "+item.Field.Name+"/"+type.Name);
            if(item.Field.Width!=(codec==Codec.KindOf?40:4))throw new InvalidDataException("Fixed root width differs: "+item.Field.Name);
            return new Binding(item.Field,item.Attribute!.FixedValue??item.Attribute.DefaultValue,type,codec);
        }).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode exactly the legacy marshaler's enum ordinals, KindOf token operations and invariant weak-name hash. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Bytes(XmlDocument source,Binding binding)
    {
        string text=source.DocumentElement!.HasAttribute(binding.Native.Name)?source.DocumentElement.GetAttribute(binding.Native.Name):binding.Default??string.Empty;var bytes=new byte[binding.Native.Width];
        try
        {
            if(binding.Wire==Codec.WeakId){BinaryPrimitives.WriteUInt32LittleEndian(bytes,string.IsNullOrEmpty(text)?0:FastHash.GetHashCode(text.ToLowerInvariant()));return bytes;}
            if(binding.Wire==Codec.Enum){BinaryPrimitives.WriteInt32LittleEndian(bytes,Convert.ToInt32(Enum.Parse(binding.ClrType,text,false),System.Globalization.CultureInfo.InvariantCulture));return bytes;}
            foreach(string raw in text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries))
            {
                bool include=raw[0]!='-';string token=raw[0] is '+' or '-'?raw[1..]:raw;
                if(token=="ALL"){for(int span=0;span<10;span++)BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(span*4,4),uint.MaxValue);continue;}
                int value=(int)Enum.Parse<KindOf>(token,false);int at=value/32*4;uint current=BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4)),mask=1u<<(value%32);BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),include?current|mask:current^mask);
            }
            return bytes;
        }
        catch(Exception error)when(error is ArgumentException or OverflowException or FormatException or IndexOutOfRangeException){throw new InvalidDataException("Invalid fixed root "+binding.Native.Name+": "+text,error);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select a real declared enum value different from the current/default literal for grouped offset coverage. */
    //-------------------------------------------------------------------------------------------------
    private static string DifferentEnum(Binding binding,XmlDocument source)
    {
        string current=source.DocumentElement!.HasAttribute(binding.Native.Name)?source.DocumentElement.GetAttribute(binding.Native.Name):binding.Default??string.Empty;return Enum.GetNames(binding.ClrType).First(name=>name!=current);
    }
}
