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

// Reborn: reviewed complete module models expose fixed-width primitive/enum/flag attributes through official schemas, concrete ABI fields and independently qualified token coordinates.
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
                Type type;try{type=container.LocalName switch
                {
                    "Behaviors"=>Ep1BehaviorMetadataTokenizer.ReviewedNativeType(FastHash.GetHashCode(Ep1BehaviorMetadataTokenizer.ReviewedWireName(element.LocalName))),
                    "Draws"=>element.LocalName switch{"ScriptedModelDraw"=>typeof(W3DScriptedModelDrawModuleData),"TracerModelDraw"=>typeof(W3DTracerDrawModuleData),"StreamDraw"=>typeof(Ep1W3DStreamDrawModuleData),"LaserDraw"=>typeof(Ep1W3DLaserDrawModuleData),"TruckDraw"=>typeof(Ep1W3DTruckDrawModuleData),_=>throw new InvalidDataException("Unreviewed scalar draw family.")},
                    // Reborn: jet AI owns a separate EP1 record instead of inheriting legacy KW offsets.
                    "AI" when element.LocalName=="JetAIUpdate"=>typeof(Ep1JetAIUpdateModuleData),
                    "AI" when element.LocalName=="AIUpdate"=>typeof(Ep1AIUpdateModuleData),
                    // Reborn: PropBody has no own scalars but participates in source identities and whole-object validation.
                    "Body" when element.LocalName=="PropBody"=>typeof(PropBodyModuleData),
                    "Body" when element.LocalName=="ActiveBody"=>typeof(Ep1ActiveBodyModuleData),
                    // Reborn: use the EP1 strong-EVA record for readiness announcements.
                    "ClientUpdates" when element.LocalName=="EvaAnnounceSpecialPowerReadyClientUpdate"=>typeof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData),
                    "ClientUpdates" when element.LocalName=="EvaAnnounceClientCreate"=>typeof(Ep1EvaAnnounceClientCreateModuleData),
                    "ClientUpdates" when element.LocalName=="LargeGroupAudioClientUpdate"=>typeof(LargeGroupAudioClientUpdateModuleData),
                    "ClientBehaviors"=>element.LocalName switch{"ModelConditionSoundSelectorClientBehavior"=>typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),"AnimationSoundClientBehavior"=>typeof(AnimationSoundClientBehaviorModuleData),"ModelConditionAudioLoopClientBehavior"=>typeof(ModelConditionAudioLoopClientBehaviorModuleData),_=>throw new InvalidDataException("Unreviewed scalar client family.")},
                    _=>throw new InvalidDataException("Unreviewed module scalar owner: "+container.LocalName+"/"+element.LocalName+".")
                };}catch(InvalidDataException error){throw SdkXmlDiagnostics.At(error,element);}
                result.Add(new(container.LocalName,index++,element,type));
            }
        }
        return result.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: intersect inherited official attributes with fixed native members; strings, references, pointers and dynamic list structure stay separately gated. */
    //-------------------------------------------------------------------------------------------------
    internal static Binding[] Bindings(Type type)=>registry.GetOrAdd(type,model=>BuildBindings(model,8));

    // Reborn: nonmodule records have no module TypeId/id prefix; reuse the exact schema/native codec intersection from offset zero.
    internal static Binding[] RecordBindings(Type type)=>BuildBindings(type,0);

    // Reborn: share attribute discovery while preserving the different module and plain-record ABI prefixes.
    private static Binding[] BuildBindings(Type model,int minimumOffset)
    {
        string schemaName=model.Name.StartsWith("Ep1",StringComparison.Ordinal)?model.Name[3..]:model.Name;
        var schema=schemas.Value.GlobalTypes[new XmlQualifiedName(schemaName,"uri:ea.com:eala:asset")] as XmlSchemaComplexType??throw new InvalidDataException("Module scalar schema absent: "+schemaName);
        var result=new List<Binding>();
        foreach(XmlSchemaAttribute attribute in schema.AttributeUses.Values)
        {
            string name=attribute.QualifiedName.Name;if(name is "id" or "TypeId" || attribute.FixedValue!=null)continue;
            Type current=model;int offset=0;FieldInfo? field=null;
            for(int depth=0;depth<16;depth++){field=current.GetField(name);if(field!=null){offset+=Marshal.OffsetOf(current,name).ToInt32();break;}var parent=current.GetField("Base");if(parent==null)break;offset+=Marshal.OffsetOf(current,"Base").ToInt32();current=parent.FieldType;}
            // Reborn: extend the descriptor intersection with schema-matched enums and fixed flags, never pointers or dynamic lists.
            if(field==null)continue;var extended=ExtendedCodec(attribute.AttributeSchemaType!,field.FieldType);string codec=extended?.Codec??field.FieldType.Name;var contract=extended.HasValue?(extended.Value.Width,extended.Value.Wire):Codec(attribute.AttributeSchemaType!,codec);if(contract==null)continue;
            int width=field.FieldType.IsEnum?4:Marshal.SizeOf(field.FieldType);if(width!=contract.Value.Item1 || offset<minimumOffset || offset>Marshal.SizeOf(model)-width)throw new InvalidDataException("Module scalar ABI width/bounds differs: "+schemaName+"."+name);
            result.Add(new(name,offset,width,codec,attribute.DefaultValue,contract.Value.Item2));
        }
        return result.OrderBy(b=>b.Offset).ToArray();
    }

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
            "UInt32" when names.Contains("SageUnsignedInt")||names.Contains("SageDurationUnsignedInt")=>(4,0x763177A4u),
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
    /** Reborn: bind only exact enum schema/native names or explicit fixed uint bit buffers with a schema-declared enum item type and reviewed wire identity. */
    //-------------------------------------------------------------------------------------------------
    private static (int Width,uint Wire,string Codec)? ExtendedCodec(XmlSchemaType schema,Type native)
    {
        if(!Ep1BehaviorMetadataTokenizer.TryReviewedScalarSignature(schema.Name??string.Empty,out uint wire))return null;
        // Reborn: the existing EP1 AI writer stores this schema flag list in a uint, not a fixed-buffer wrapper.
        if(native==typeof(uint)&&schema.Name=="AutoAcquireEnemiesBitFlags")return (4,wire,"Flags:"+typeof(AutoAcquireEnemiesType).FullName);
        // Reborn: EP1 root movement uses the reviewed EP1 enum, whose additional literals must not be encoded through the legacy KW enum.
        // Reborn: OCL source locations use EP1 ordinals rather than the shorter legacy KW enum.
        if(native.IsEnum&&(native.Name==schema.Name||native==typeof(Ep1OCLCreateLocType)&&schema.Name=="OCLCreateLocType"||native==typeof(Ep1RootLocomotorSetType)&&schema.Name=="LocomotorSetType"||native==typeof(Ep1ProductionQueueType)&&schema.Name=="ProductionQueueType"||native==typeof(Ep1AIStateMachineType)&&schema.Name=="StateMachineType")&&Enum.GetUnderlyingType(native)==typeof(int))return (4,wire,"Enum:"+native.FullName);
        if(schema is not XmlSchemaSimpleType {Content:XmlSchemaSimpleTypeList list}||!native.Name.EndsWith("BitFlags",StringComparison.Ordinal)||native.Name!=schema.Name)return null;
        // Reborn: EP1 collision flag ordinals differ from the legacy KW enum despite sharing a four-byte buffer.
        if(schema.Name=="FireWeaponFlagsBitFlags"&&native==typeof(FireWeaponFlagsBitFlags))return (4,wire,"Flags:"+typeof(Ep1FireWeaponFlagsType).FullName);
        var buffer=native.GetField("Value")?.GetCustomAttribute<System.Runtime.CompilerServices.FixedBufferAttribute>();var item=typeof(GameObject).Assembly.GetType("SageBinaryData."+list.BaseItemType?.Name);
        if(buffer==null||buffer.ElementType!=typeof(uint)||buffer.Length<1||buffer.Length>64||item==null||!item.IsEnum)return null;
        return (checked(buffer.Length*4),wire,"Flags:"+item.FullName);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate representability and canonicalize schema-legal boolean spellings before the legacy marshaler reads authored module attributes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source)
    {
        foreach(var owner in Owners(source)){
            if(owner.NativeType==typeof(BuildingDestructionModuleData))BuildingDestructionContract.Validate(owner.Element);
            if(owner.NativeType==typeof(QueueDockModuleData))QueueDockContract.Validate(owner.Element);
            if(owner.NativeType==typeof(Ep1QueueProductionExitUpdateModuleData))QueueProductionExitContract.Validate(owner.Element);
            if(owner.NativeType==typeof(Ep1SpawnBehaviorModuleData))SpawnBehaviorContract.Validate(owner.Element);
            if(owner.NativeType==typeof(Ep1ProductionUpdateModuleData))ProductionUpdateContract.Validate(owner.Element);
            if(owner.NativeType==typeof(Ep1RepairUpdateModuleData))RepairUpdateContract.Validate(owner.Element);
            if(owner.NativeType==typeof(Ep1EngineerContainModuleData))EngineerContainContract.Validate(owner.Element);
            if(owner.NativeType==typeof(AssistedPlacementBehaviorModuleData))AssistedPlacementContract.Validate(owner.Element);
            if(owner.NativeType==typeof(InfiltratorContainModuleData))InfiltratorContainContract.Validate(owner.Element);
            if(owner.NativeType==typeof(StructurePlacementBehaviorModuleData))StructurePlacementContract.Validate(owner.Element);
            if(owner.NativeType==typeof(LUAEventListBehaviorModuleData))LUAEventListContract.Validate(owner.Element);
            // Reborn: model subobject text has a separate complete string/ownership contract rather than stock name equality.
            if(owner.NativeType==typeof(BuildingBehaviorModuleData))BuildingBehaviorContract.Validate(owner.Element);
            // Reborn: FX nested event booleans and complete death-condition values need the same early source-located validation.
            if(owner.NativeType==typeof(FXListBehaviorModuleData))FXListBehaviorContract.Validate(owner.Element);
            // Reborn: nested tracker records are validated independently of the original Conscript shape.
            if(owner.NativeType==typeof(SecondaryDamageTrackerModuleData))SecondaryDamageTrackerContract.Validate(owner.Element);
            // Reborn: creation and destroy families share full authored death-condition validation.
            if(DeathBehaviorContract.Supports(owner.NativeType))DeathBehaviorContract.Validate(owner.Element);
            // Reborn: nested death conditions retain source-located validation on death weapons as well as Die-module families.
            if(owner.NativeType==typeof(Ep1FireWeaponWhenDeadBehaviorModuleData))DeathBehaviorContract.Validate(owner.Element);
            foreach(var binding in Bindings(owner.NativeType)){_=Bytes(owner.Element,binding);if(binding.Codec=="SageBool"&&owner.Element.HasAttribute(binding.Name))owner.Element.SetAttribute(binding.Name,Boolean(Literal(owner.Element,binding))?"true":"false");}
        }
    }

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
        // Reborn: bound serialized pointers/counts before narrowing, without the historical 128-module test quota or incidental overflow exceptions.
        int slot=prefix+slots[owner.Container];uint pointer=owner.Container=="AI"?(uint)slot:Word(bytes,slot);
        if(pointer>=bytes.Length||pointer%4!=0)throw new InvalidDataException("Module scalar owner pointer bounds/alignment differ.");
        int header=(int)pointer;if(owner.Container=="Body")return header;
        uint count=Word(bytes,header),items=Word(bytes,header+4);
        if(owner.Index<0||owner.Index>=count||items>=bytes.Length||items%4!=0||count>(bytes.Length-items)/4)throw new InvalidDataException("Module scalar container count/items differ.");
        uint target=Word(bytes,(int)items+owner.Index*4);if(target>=bytes.Length||target%4!=0)throw new InvalidDataException("Module scalar leaf pointer bounds/alignment differ.");return (int)target;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact source-to-native bytes are checked again at serialization, including packed booleans, percentages and converted time/angle/velocity units. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,int prefix){foreach(var owner in Owners(source)){int start=Address(native,prefix,owner);foreach(var binding in Bindings(owner.NativeType)){int at=start+binding.Offset;var expected=Bytes(owner.Element,binding);if(at<0 || at>native.Length-binding.Width || !native.AsSpan(at,binding.Width).SequenceEqual(expected))throw new InvalidDataException("Module scalar native value differs: "+owner.Container+"/"+owner.Element.GetAttribute("id")+"/@"+binding.Name+" expected="+Convert.ToHexString(expected)+" actual="+(at<0||at>native.Length-binding.Width?"<out of bounds>":Convert.ToHexString(native.AsSpan(at,binding.Width)))+" record="+Convert.ToHexString(native.AsSpan(start,Math.Min(Math.Max(0,native.Length-start),Marshal.SizeOf(owner.NativeType))))+" index="+owner.Index);}}}

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
        int index=0;foreach(var owner in Owners(source))foreach(var binding in Bindings(owner.NativeType)){index++;string value=binding.Codec switch{"SageBool"=>Boolean(Literal(owner.Element,binding))?"false":"true","Time"=>(1000+index*13).ToString(CultureInfo.InvariantCulture)+"ms","Angle"=>"45d","Percentage"=>"37%","Duration"=>"1001",_=>binding.Codec is "Single" or "Velocity"?(index+0.125f).ToString("R",CultureInfo.InvariantCulture):(index%127+1).ToString(CultureInfo.InvariantCulture)};if(binding.Codec.StartsWith("Enum:",StringComparison.Ordinal))value=(ProbeNames(binding).FirstOrDefault(n=>n!=Literal(owner.Element,binding))??ProbeNames(binding)[0]);else if(binding.Codec.StartsWith("Flags:",StringComparison.Ordinal))value=string.Join(" ",ProbeNames(binding).Where(n=>Convert.ToInt32(Enum.Parse(ValueType(binding),n),CultureInfo.InvariantCulture)>=0).TakeLast(2));owner.Element.SetAttribute(binding.Name,value);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: missing optional primitive attributes use their official default or native zero while preserving authored omission in XML. */
    //-------------------------------------------------------------------------------------------------
    private static string Literal(XmlElement source,Binding binding)=>source.HasAttribute(binding.Name)?source.GetAttribute(binding.Name):binding.Default??(binding.Codec.StartsWith("Flags:",StringComparison.Ordinal)?string.Empty:binding.Codec=="SageBool"?"false":"0");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: historical MaxHealth numeric regression vectors share the same official module default as the generic capability route. */
    //-------------------------------------------------------------------------------------------------
    internal static string Literal(XmlElement source,string name,Type type)=>Literal(source,Bindings(type).Single(b=>b.Name==name));

    // Reborn: plain-record receipts use the same actual native descriptor/defaults as source verification, without a module prefix requirement.
    internal static string RecordLiteral(XmlElement source,string name,Type type)=>Literal(source,RecordBindings(type).Single(b=>b.Name==name));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent scalar encodings preserve signed/unsigned hex, engine units and finite float32 bounds. */
    //-------------------------------------------------------------------------------------------------
    internal static byte[] Bytes(XmlElement source,Binding binding)
    {
        string text=Literal(source,binding).Trim();var bytes=new byte[binding.Width];
        try
        {
            // Reborn: enum ordinals and fixed flag bit positions come from concrete native enum definitions, with no arbitrary numeric or field-specific whitelist.
            if(binding.Codec.StartsWith("Enum:",StringComparison.Ordinal)){var type=ValueType(binding);var enumValue=Enum.Parse(type,text,false);if(!Enum.IsDefined(type,enumValue))throw new FormatException();BinaryPrimitives.WriteInt32LittleEndian(bytes,Convert.ToInt32(enumValue,CultureInfo.InvariantCulture));return bytes;}
            if(binding.Codec.StartsWith("Flags:",StringComparison.Ordinal))
            {
                var type=ValueType(binding);
                foreach(string token in text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries))
                {
                    if(token=="ALL"){Array.Fill(bytes,(byte)255);continue;}
                    if(token[0] is '+' or '-')throw SdkXmlDiagnostics.At(new InvalidDataException("Prefixed module flag operations require the corresponding legacy marshaler repair: "+source.LocalName+"."+binding.Name),(XmlNode?)source.GetAttributeNode(binding.Name)??source);
                    var enumValue=Enum.Parse(type,token,false);if(!Enum.IsDefined(type,enumValue))throw new FormatException();int bit=Convert.ToInt32(enumValue,CultureInfo.InvariantCulture);if(bit==-1)continue;if(bit<0||bit>=bytes.Length*8)throw new OverflowException();int at=bit/32*4;uint current=BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),current|1u<<(bit%32));
                }
                return bytes;
            }
            // Reborn: unsigned source spellings retain official hexadecimal and zero-decimal compatibility before width checks.
            uint Unsigned()=>text=="0.0"?0:text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?uint.Parse(text[2..],NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture):uint.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture);
            switch(binding.Codec)
            {
                case "SageBool":bytes[0]=Boolean(text)?(byte)1:(byte)0;return bytes;
                case "Int32":BinaryPrimitives.WriteInt32LittleEndian(bytes,text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?int.Parse(text[2..],NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture):int.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture));return bytes;
                case "UInt32":BinaryPrimitives.WriteUInt32LittleEndian(bytes,Unsigned());return bytes;
                case "UInt16":BinaryPrimitives.WriteUInt16LittleEndian(bytes,checked((ushort)Unsigned()));return bytes;
                case "Byte":bytes[0]=checked((byte)Unsigned());return bytes;
                // Reborn: official xs:byte crusher levels are signed -128..127, not unsigned bytes or float encodings.
                case "SByte":bytes[0]=unchecked((byte)sbyte.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture));return bytes;
                case "Duration":BinaryPrimitives.WriteUInt32LittleEndian(bytes,checked((uint)Math.Ceiling(Unsigned()*Marshaler.LOGICFRAMES_PER_MSEC_REAL)));return bytes;
            }
            float multiplier=1;
            if(binding.Codec=="Time"){if(text.EndsWith("ms",StringComparison.Ordinal)){multiplier=0.001f;text=text[..^2];}else if(text.EndsWith('s'))text=text[..^1];}
            else if(binding.Codec=="Angle"){if(text.EndsWith('d')){multiplier=Marshaler.RADS_PER_DEGREE;text=text[..^1];}else if(text.EndsWith('r'))text=text[..^1];}
            else if(binding.Codec=="Percentage"){multiplier=0.01f;text=text.TrimEnd('%');}
            else if(binding.Codec=="Velocity")multiplier=Marshaler.SECONDS_PER_LOGICFRAME_REAL;
            float value=float.Parse(text,NumberStyles.Float,CultureInfo.InvariantCulture)*multiplier;if(!float.IsFinite(value))throw new OverflowException();BinaryPrimitives.WriteSingleLittleEndian(bytes,value);return bytes;
        }
        catch(Exception error)when(error is FormatException or OverflowException or ArgumentException){throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid module value "+source.LocalName+"."+binding.Name+": "+text,error),(XmlNode?)source.GetAttributeNode(binding.Name)??source);}
    }

    // Reborn: enum codec identities are resolved only from this compiler's concrete native-model assembly.
    private static Type ValueType(Binding binding)=>typeof(GameObject).Assembly.GetType(binding.Codec[(binding.Codec.IndexOf(':')+1)..])??throw new InvalidDataException("Native enum codec type missing.");

    // Reborn: grouped probes use only named values shared by the concrete enum and its official schema, not numeric test ordinals.
    private static string[] ProbeNames(Binding binding)
    {
        // Reborn: the concrete EP1 enum alias shares the official LocomotorSetType schema, including AI default-movement probes.
        var type=ValueType(binding);string schemaName=type==typeof(Ep1RootLocomotorSetType)?"LocomotorSetType":type==typeof(Ep1ProductionQueueType)?"ProductionQueueType":type.Name;var schema=schemas.Value.GlobalTypes[new XmlQualifiedName(schemaName,"uri:ea.com:eala:asset")] as XmlSchemaSimpleType;
        var allowed=(schema?.Content as XmlSchemaSimpleTypeRestriction)?.Facets.OfType<XmlSchemaEnumerationFacet>().Select(f=>f.Value).ToHashSet(StringComparer.Ordinal)??throw new InvalidDataException("Enum probe schema facets unavailable: "+type.Name);
        return Enum.GetNames(type).Where(allowed.Contains).ToArray();
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
