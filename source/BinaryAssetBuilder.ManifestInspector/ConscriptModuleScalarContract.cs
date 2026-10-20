using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reviewed complete module models expose fixed-width primitive attributes through official schemas, concrete ABI fields and independently qualified token coordinates.
internal static class ConscriptModuleScalarContract
{
    internal sealed record Binding(string Name,int Offset,int Width,string Codec,string? Default,uint Wire);
    internal sealed record Owner(string Container,int Index,XmlElement Element,Type NativeType);
    internal sealed record Family(string Container,string Element,int Instances,Binding[] Attributes);
    private static readonly Lazy<XmlSchemaSet> schemas=new(ConscriptSourceInput.Schemas);
    private static readonly ConcurrentDictionary<Type,Binding[]> registry=new();
    private static readonly Dictionary<string,int> slots=new(StringComparer.Ordinal){{"Draws",472},{"Behaviors",476},{"AI",480},{"Body",488},{"ClientUpdates",492},{"ClientBehaviors",496}};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source module order and reviewed concrete family select owners; nested lists and polymorphic identities are never inferred from scalar attributes. */
    //-------------------------------------------------------------------------------------------------
    internal static Owner[] Owners(XmlDocument source)
    {
        var result=new List<Owner>();
        foreach(var container in source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>slots.ContainsKey(e.LocalName)))
        {
            int index=0;foreach(var element in container.ChildNodes.OfType<XmlElement>())
            {
                Type type=container.LocalName switch
                {
                    "Behaviors"=>Ep1BehaviorMetadataTokenizer.ReviewedNativeType(FastHash.GetHashCode(Ep1BehaviorMetadataTokenizer.ReviewedWireName(element.LocalName))),
                    "Draws"=>element.LocalName switch{"ScriptedModelDraw"=>typeof(W3DScriptedModelDrawModuleData),"TracerModelDraw"=>typeof(W3DTracerDrawModuleData),_=>throw new InvalidDataException("Unreviewed scalar draw family.")},
                    "AI" when element.LocalName=="AIUpdate"=>typeof(Ep1AIUpdateModuleData),
                    "Body" when element.LocalName=="ActiveBody"=>typeof(Ep1ActiveBodyModuleData),
                    "ClientUpdates" when element.LocalName=="LargeGroupAudioClientUpdate"=>typeof(LargeGroupAudioClientUpdateModuleData),
                    "ClientBehaviors"=>element.LocalName switch{"ModelConditionSoundSelectorClientBehavior"=>typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),"AnimationSoundClientBehavior"=>typeof(AnimationSoundClientBehaviorModuleData),"ModelConditionAudioLoopClientBehavior"=>typeof(ModelConditionAudioLoopClientBehaviorModuleData),_=>throw new InvalidDataException("Unreviewed scalar client family.")},
                    _=>throw new InvalidDataException("Unreviewed module scalar owner.")
                };
                result.Add(new(container.LocalName,index++,element,type));
            }
        }
        return result.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: intersect inherited official attributes with primitive native members; strings, enums, flags, references, pointers and list structure stay separately gated. */
    //-------------------------------------------------------------------------------------------------
    internal static Binding[] Bindings(Type type)=>registry.GetOrAdd(type,model=>
    {
        string schemaName=model.Name.StartsWith("Ep1",StringComparison.Ordinal)?model.Name[3..]:model.Name;
        var schema=schemas.Value.GlobalTypes[new XmlQualifiedName(schemaName,"uri:ea.com:eala:asset")] as XmlSchemaComplexType??throw new InvalidDataException("Module scalar schema absent: "+schemaName);
        var result=new List<Binding>();
        foreach(XmlSchemaAttribute attribute in schema.AttributeUses.Values)
        {
            string name=attribute.QualifiedName.Name;if(name is "id" or "TypeId" || attribute.FixedValue!=null)continue;
            Type current=model;int offset=0;FieldInfo? field=null;
            for(int depth=0;depth<16;depth++){field=current.GetField(name);if(field!=null){offset+=Marshal.OffsetOf(current,name).ToInt32();break;}var parent=current.GetField("Base");if(parent==null)break;offset+=Marshal.OffsetOf(current,"Base").ToInt32();current=parent.FieldType;}
            if(field==null)continue;string codec=field.FieldType.Name;var contract=Codec(attribute.AttributeSchemaType!,codec);if(contract==null)continue;
            int width=Marshal.SizeOf(field.FieldType);if(width!=contract.Value.Width || offset<8 || offset>Marshal.SizeOf(model)-width)throw new InvalidDataException("Module scalar ABI width/bounds differs: "+schemaName+"."+name);
            result.Add(new(name,offset,width,codec,attribute.DefaultValue,contract.Value.Wire));
        }
        return result.OrderBy(b=>b.Offset).ToArray();
    });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose the effective per-family capability inventory for build evidence and user documentation without exporting EA source or resource bodies. */
    //-------------------------------------------------------------------------------------------------
    internal static Family[] Describe(XmlDocument source)=>Owners(source).GroupBy(o=>(o.Container,o.Element.LocalName,o.NativeType)).Select(group=>new Family(group.Key.Container,group.Key.LocalName,group.Count(),Bindings(group.Key.NativeType))).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allow only schema/native codec pairs with reviewed wire identities, including engine unit conversion wrappers. */
    //-------------------------------------------------------------------------------------------------
    private static (int Width,uint Wire)? Codec(XmlSchemaType schema,string native)
    {
        var names=new HashSet<string>();for(XmlSchemaType? current=schema;current!=null && names.Add(current.QualifiedName.Name);current=current.BaseXmlSchemaType){if(current is XmlSchemaSimpleType simple && simple.Content is XmlSchemaSimpleTypeList)return null;}
        return native switch
        {
            "Single" when names.Contains("SageReal") || schema.QualifiedName==new XmlQualifiedName("float",XmlSchema.Namespace)=>(4,0x645864F3u),
            "Int32" when names.Contains("SageInt")=>(4,0x23E650B8u),
            "UInt32" when names.Contains("SageUnsignedInt")=>(4,0x763177A4u),
            "UInt16" when names.Contains("SageUnsignedShort")=>(2,0x43501092u),
            "Byte" when schema.Datatype?.TypeCode==XmlTypeCode.UnsignedByte=>(1,0x30916B96u),
            "SageBool" when names.Contains("SageBool")=>(1,0x77E3F4F7u),
            "Time" when names.Contains("Time")=>(4,0x998669E8u),
            "Angle" when names.Contains("Angle")=>(4,0x4E16EA4Du),
            "Duration" when names.Contains("DurationUnsignedInt")=>(4,0x6F6D1AE7u),
            "Percentage" when names.Contains("Percentage")=>(4,0x645864F3u),
            "Velocity" when names.Contains("Velocity")=>(4,0x7A76C60Eu),
            _=>null
        };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate representability and canonicalize schema-legal boolean spellings before the legacy marshaler reads authored module attributes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source){foreach(var owner in Owners(source))foreach(var binding in Bindings(owner.NativeType)){_=Bytes(owner.Element,binding);if(binding.Codec=="SageBool" && owner.Element.HasAttribute(binding.Name))owner.Element.SetAttribute(binding.Name,Boolean(Literal(owner.Element,binding))?"true":"false");}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only admitted fixed-width attributes are normalized for structure comparison; owners, IDs, module order and children remain visible. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument baseline)
    {
        var models=Owners(baseline);var owners=Owners(candidate);if(owners.Length!=models.Length)throw new InvalidDataException("Module scalar owner count differs.");
        for(int i=0;i<owners.Length;i++){var owner=owners[i];var model=models[i];if(owner.Container!=model.Container || owner.Index!=model.Index || owner.NativeType!=model.NativeType || owner.Element.GetAttribute("id")!=model.Element.GetAttribute("id"))throw new InvalidDataException("Module scalar owner identity/order differs.");foreach(var binding in Bindings(owner.NativeType)){owner.Element.RemoveAttribute(binding.Name);if(model.Element.HasAttribute(binding.Name))owner.Element.SetAttribute(binding.Name,model.Element.GetAttribute(binding.Name));}}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: dynamic root strings may move every module; actual native container pointers locate fixed-width fields without captured absolute offsets. */
    //-------------------------------------------------------------------------------------------------
    internal static int Address(byte[] bytes,int prefix,Owner owner)
    {
        int slot=prefix+slots[owner.Container];if(owner.Container=="Body")return checked((int)Word(bytes,slot));int header=owner.Container=="AI"?slot:checked((int)Word(bytes,slot));int count=checked((int)Word(bytes,header));if(owner.Index<0 || owner.Index>=count || count>128)throw new InvalidDataException("Module scalar container count differs.");int items=checked((int)Word(bytes,header+4));return checked((int)Word(bytes,items+owner.Index*4));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact source-to-native bytes are checked again at serialization, including packed booleans, percentages and converted time/angle/velocity units. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,int prefix){foreach(var owner in Owners(source)){int start=Address(native,prefix,owner);foreach(var binding in Bindings(owner.NativeType)){int at=start+binding.Offset;if(at<0 || at>native.Length-binding.Width || !native.AsSpan(at,binding.Width).SequenceEqual(Bytes(owner.Element,binding)))throw new InvalidDataException("Module scalar native value differs: "+owner.Container+"/"+owner.Element.GetAttribute("id")+"/@"+binding.Name);}}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore only already-verified owned bytes before the complete stock graph oracle, retaining every pointer, dependency, list and non-scalar byte. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyAndRestore(XmlDocument source,byte[] actual,byte[] restored,byte[] baseline)
    {
        Verify(source,actual,0);foreach(var owner in Owners(source)){int start=Address(actual,0,owner),old=Address(baseline,0,owner);foreach(var binding in Bindings(owner.NativeType))baseline.AsSpan(old+binding.Offset,binding.Width).CopyTo(restored.AsSpan(start+binding.Offset,binding.Width));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent expected metadata retains all token rows and writes only typed values at dynamically resolved module addresses. */
    //-------------------------------------------------------------------------------------------------
    internal static void Patch(XmlDocument source,byte[] target,int prefix){foreach(var owner in Owners(source)){int start=Address(target,prefix,owner);foreach(var binding in Bindings(owner.NativeType))Bytes(owner.Element,binding).CopyTo(target.AsSpan(start+binding.Offset,binding.Width));}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify every schema/native offset against the independently stock-proved token directory before authored values can reuse these descriptors. */
    //-------------------------------------------------------------------------------------------------
    internal static int Qualify(XmlDocument baseline,byte[] native,byte[] tokens,int prefix)
    {
        Verify(baseline,native,0);int count=0;var root=GameObjectStockReview.Tokens(tokens,0);
        foreach(var owner in Owners(baseline))
        {
            int table=root[FastHash.GetHashCode(owner.Container)].Offset;int wrapper=owner.Container=="Body"?table:checked((int)Word(tokens,table+20+owner.Index*12));int start=Address(native,0,owner);
            if(Word(tokens,wrapper)!=2 || Word(tokens,wrapper+4)!=Word(native,start))throw new InvalidDataException("Module scalar concrete token/native identity differs.");
            foreach(var binding in Bindings(owner.NativeType))
            {
                int current=checked((int)Word(tokens,wrapper+8));(uint Type,int Offset)? found=null;
                for(int depth=0;depth<16;depth++){var fields=GameObjectStockReview.Tokens(tokens,current);if(fields.TryGetValue(FastHash.GetHashCode(binding.Name),out var value)){found=value;break;}if(!fields.TryGetValue(1,out var parent))break;current=parent.Offset;}
                if(found==null || found.Value.Type!=binding.Wire || found.Value.Offset!=prefix+start+binding.Offset)throw new InvalidDataException("Module scalar stock token binding differs: "+owner.Element.LocalName+"."+binding.Name);count++;
            }
        }
        return count;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutate every admitted module scalar in one full-object fixture using distinct values and boolean inversion. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source)
    {
        int index=0;foreach(var owner in Owners(source))foreach(var binding in Bindings(owner.NativeType)){index++;string value=binding.Codec switch{"SageBool"=>Boolean(Literal(owner.Element,binding))?"false":"true","Time"=>(1000+index*13).ToString(CultureInfo.InvariantCulture)+"ms","Angle"=>"45d","Percentage"=>"37%","Duration"=>"1001",_=>binding.Codec is "Single" or "Velocity"?(index+0.125f).ToString("R",CultureInfo.InvariantCulture):(index%127+1).ToString(CultureInfo.InvariantCulture)};owner.Element.SetAttribute(binding.Name,value);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: missing optional primitive attributes use their official default or native zero while preserving authored omission in XML. */
    //-------------------------------------------------------------------------------------------------
    private static string Literal(XmlElement source,Binding binding)=>source.HasAttribute(binding.Name)?source.GetAttribute(binding.Name):binding.Default??(binding.Codec=="SageBool"?"false":"0");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: historical MaxHealth numeric regression vectors share the same official module default as the generic capability route. */
    //-------------------------------------------------------------------------------------------------
    internal static string Literal(XmlElement source,string name,Type type)=>Literal(source,Bindings(type).Single(b=>b.Name==name));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent scalar encodings preserve signed/unsigned hex, engine units and finite float32 bounds. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Bytes(XmlElement source,Binding binding)
    {
        string text=Literal(source,binding).Trim();var bytes=new byte[binding.Width];
        try
        {
            // Reborn: unsigned source spellings retain official hexadecimal and zero-decimal compatibility before width checks.
            uint Unsigned()=>text=="0.0"?0:text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?uint.Parse(text[2..],NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture):uint.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture);
            switch(binding.Codec)
            {
                case "SageBool":bytes[0]=Boolean(text)?(byte)1:(byte)0;return bytes;
                case "Int32":BinaryPrimitives.WriteInt32LittleEndian(bytes,text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?int.Parse(text[2..],NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture):int.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture));return bytes;
                case "UInt32":BinaryPrimitives.WriteUInt32LittleEndian(bytes,Unsigned());return bytes;
                case "UInt16":BinaryPrimitives.WriteUInt16LittleEndian(bytes,checked((ushort)Unsigned()));return bytes;
                case "Byte":bytes[0]=checked((byte)Unsigned());return bytes;
                case "Duration":BinaryPrimitives.WriteUInt32LittleEndian(bytes,checked((uint)Math.Ceiling(Unsigned()*Marshaler.LOGICFRAMES_PER_MSEC_REAL)));return bytes;
            }
            float multiplier=1;
            if(binding.Codec=="Time"){if(text.EndsWith("ms",StringComparison.Ordinal)){multiplier=0.001f;text=text[..^2];}else if(text.EndsWith('s'))text=text[..^1];}
            else if(binding.Codec=="Angle"){if(text.EndsWith('d')){multiplier=Marshaler.RADS_PER_DEGREE;text=text[..^1];}else if(text.EndsWith('r'))text=text[..^1];}
            else if(binding.Codec=="Percentage"){multiplier=0.01f;text=text.TrimEnd('%');}
            else if(binding.Codec=="Velocity")multiplier=Marshaler.SECONDS_PER_LOGICFRAME_REAL;
            float value=float.Parse(text,NumberStyles.Float,CultureInfo.InvariantCulture)*multiplier;if(!float.IsFinite(value))throw new OverflowException();BinaryPrimitives.WriteSingleLittleEndian(bytes,value);return bytes;
        }
        catch(Exception error)when(error is FormatException or OverflowException){throw new InvalidDataException("Invalid module scalar "+source.LocalName+"."+binding.Name+": "+text,error);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: official SageBool accepts true/false in either initial case as well as numeric zero/one. */
    //-------------------------------------------------------------------------------------------------
    private static bool Boolean(string text)=>text.Trim() switch{"1"=>true,"0"=>false,_=>bool.Parse(text)};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded little-endian reads prevent malformed source-selected pointers from escaping the qualified graph. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int at){if(at<0 || at>bytes.Length-4)throw new InvalidDataException("Module scalar word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
