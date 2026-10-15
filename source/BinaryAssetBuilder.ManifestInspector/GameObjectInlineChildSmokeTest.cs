using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: child-element syntax must not turn inline EP1 enum/color words into allocated pointer owners.
internal static class GameObjectInlineChildSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin stock direct targets/zero values, official enum inventories and every authored enum/color combination through native and token composition. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        XmlSchemaSet? schemas=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>schemas=s);if(schemas==null)throw new InvalidDataException("Reviewed inline root schema required.");
        if(Marshal.SizeOf<GameObject>()!=600 || Marshal.OffsetOf<GameObject>("Buildable").ToInt32()!=436 || Marshal.OffsetOf<GameObject>("ThingClass").ToInt32()!=440 || Marshal.OffsetOf<GameObject>("DisplayColor").ToInt32()!=460 || typeof(GameObject).GetField("Buildable")!.FieldType.IsPointer || typeof(GameObject).GetField("ThingClass")!.FieldType.IsPointer || typeof(GameObject).GetField("DisplayColor")!.FieldType.IsPointer)throw new InvalidDataException("Inline root model ABI/type differs.");
        foreach(var pair in new[]{("BuildableStatus",Enum.GetNames<BuildableStatus>()),("ThingClassType",Enum.GetNames<ThingClassType>())})
        {
            var type=(XmlSchemaSimpleType)schemas.GlobalTypes[new XmlQualifiedName(pair.Item1,SchemaSet.XmlNamespace)]!;var facets=((XmlSchemaSimpleTypeRestriction)type.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().ToArray();if(!facets.Select(f=>f.Value!).SequenceEqual(pair.Item2) || facets.Any(f=>(f.UnhandledAttributes??Array.Empty<XmlAttribute>()).Any(a=>a.LocalName=="forceValue")))throw new InvalidDataException("Inline enum schema inventory/policy drift.");
        }
        var color=(XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName("Color4",SchemaSet.XmlNamespace)]!;if(!(color.UnhandledAttributes??Array.Empty<XmlAttribute>()).Any(a=>a.NamespaceURI=="uri:ea.com:eala:asset:schema" && a.LocalName=="byValue" && a.Value=="true"))throw new InvalidDataException("Official Color4 by-value policy drift.");
        foreach(string name in new[]{"AlliedConstructionYard","SovietAntiInfantryInfantry","JapanFortressShip","AlliedPowerPlantTurbine","NanoSwarmHiveEffect"})
        {
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var fields=GameObjectStockReview.Tokens(stock.Bytes,0);
            foreach(var f in Fields())if(fields[FastHash.GetHashCode(f.Name)].Type!=f.Type || fields[FastHash.GetHashCode(f.Name)].Offset!=stock.Projection.NativeRoot+f.Offset || Word(stock.Bytes,stock.Projection.NativeRoot+f.Offset)!=0)throw new InvalidDataException("Selected stock inline child target/value differs.");
        }
        int cases=0,sourceFaults=0,nativeFaults=0,metadataFaults=0;Chunk? populated=null;
        foreach(string buildable in Enum.GetNames<BuildableStatus>())foreach(string thing in Enum.GetNames<ThingClassType>())foreach(int palette in new[]{0,1,2,3})
        {
            var source=Source(buildable,thing,palette);GameObjectBodyProjectionSmokeTest.Validate(schemas,source.OuterXml);var native=GameObjectBodyProjectionSmokeTest.Compile(source,selectedBehaviors:true);var output=Ep1GameObjectDestructionProjection.Compose(native);Verify(output,native,(uint)Enum.Parse<BuildableStatus>(buildable),(uint)Enum.Parse<ThingClassType>(thing),Color(palette));
            var repeat=GameObjectBodyProjectionSmokeTest.Compile(source,selectedBehaviors:true);if(!repeat.InstanceBuffer.SequenceEqual(native.InstanceBuffer) || !repeat.RelocationBuffer.SequenceEqual(native.RelocationBuffer))throw new InvalidDataException("Inline root repeat differs.");populated=native;cases++;
        }
        var defaults=Source("YES","UNSPECIFIED",2);defaults.DocumentElement!.RemoveChild(defaults.GetElementsByTagName("Buildable",SchemaSet.XmlNamespace)[0]!);defaults.DocumentElement.RemoveChild(defaults.GetElementsByTagName("ThingClass",SchemaSet.XmlNamespace)[0]!);GameObjectBodyProjectionSmokeTest.Validate(schemas,defaults.OuterXml);var absent=GameObjectBodyProjectionSmokeTest.Compile(defaults,selectedBehaviors:true);Verify(Ep1GameObjectDestructionProjection.Compose(absent),absent,0,0,0);cases++;
        foreach(var fault in new[]{("Buildable","UNKNOWN"),("ThingClass","UNKNOWN")}){var bad=Source("YES","UNSPECIFIED",1);bad.GetElementsByTagName(fault.Item1,SchemaSet.XmlNamespace)[0]!.InnerText=fault.Item2;Refuse(()=>GameObjectBodyProjectionSmokeTest.Validate(schemas,bad.OuterXml));sourceFaults++;}
        foreach(var fault in new[]{("R","256"),("G","-1"),("A","not-a-byte")}){var bad=Source("YES","UNSPECIFIED",1);((XmlElement)bad.GetElementsByTagName("DisplayColor",SchemaSet.XmlNamespace)[0]!).SetAttribute(fault.Item1,fault.Item2);Refuse(()=>GameObjectBodyProjectionSmokeTest.Validate(schemas,bad.OuterXml));sourceFaults++;}
        foreach(var fault in new[]{(436,4u),(436,uint.MaxValue),(440,16u),(440,uint.MaxValue)}){var bad=Clone(populated!);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFaults++;}
        foreach(int slot in new[]{436,440,460}){var bad=Clone(populated!);bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Append((uint)slot));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFaults++;}
        var good=Ep1GameObjectDestructionProjection.Compose(populated!);foreach(var field in Fields()){int index=Ep1GameObjectRootValueTokens.AllFields.Select((f,i)=>(f,i)).Single(p=>p.f.Name==field.Name).i;foreach(int offset in new[]{4,8}){var bad=Clone(good);Put(bad.InstanceBuffer,index*12+offset,Word(bad.InstanceBuffer,index*12+offset)^4u);Refuse(()=>Verify(bad,populated!,3,15,Color(3)));metadataFaults++;}}
        Console.WriteLine($"GameObject inline children: PASS (five independent stock direct targets, {cases} enum/color/default source/native/token cases, preserved 600-byte root and no extra allocations; {sourceFaults} schema/{nativeFaults} native/{metadataFaults} metadata refusals; no positive nonzero stock/gameplay/general object publication claim).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit palette literals pin packed BGRA bytes and distinguish absent color from present-empty alpha defaults. */
    //-------------------------------------------------------------------------------------------------
    private static uint Color(int palette)=>palette switch{0=>0xFF000000u,1=>0x44112233u,2=>0u,_=>0xFFFF0080u};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: by-value root children precede the shared behavior/body/geometry graph in actual official source order. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Source(string buildable,string thing,int palette)
    {
        string color=palette switch{0=>"<DisplayColor/>",1=>"<DisplayColor R='17' G='34' B='51' A='68'/>",2=>"",_=>"<DisplayColor R='255' G='0' B='128'/>"};var d=new XmlDocument{XmlResolver=null};d.LoadXml($"<GameObject xmlns='uri:ea.com:eala:asset' id='InlineChildren'><Buildable>{buildable}</Buildable><ThingClass>{thing}</ThingClass>{color}<Behaviors><BuildingDestructionBehavior id='Destroy'><DieMuxData/><Item/></BuildingDestructionBehavior></Behaviors><Body><ActiveBody id='Body' MaxHealth='100'/></Body><Geometry><Shape/></Geometry></GameObject>");return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent metadata tags/targets, literal values and inverse native relocation prove inline fields never become allocation pointers. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native,uint buildable,uint thing,uint color)
    {
        var b=output.InstanceBuffer;var fields=GameObjectStockReview.Tokens(b,0);int prefix=fields[FastHash.GetHashCode("BuildTime")].Offset-140;
        if(native.InstanceBuffer.Length!=856 || Word(native.InstanceBuffer,476)!=600 || Word(native.InstanceBuffer,488)!=704 || Word(native.InstanceBuffer,500)!=780 || native.ImportsBuffer.Length!=0 || output.ImportsBuffer.Length!=0)throw new InvalidDataException("Inline children changed shared native allocation order/extent.");
        foreach(var f in Fields()){var field=fields[FastHash.GetHashCode(f.Name)];uint expected=f.Offset==436?buildable:f.Offset==440?thing:color;if(field.Type!=f.Type || field.Offset!=prefix+f.Offset || Word(b,prefix+f.Offset)!=expected || Slots(native.RelocationBuffer).Contains((uint)f.Offset) || Slots(output.RelocationBuffer).Contains((uint)(prefix+f.Offset)))throw new InvalidDataException("Inline child type/value/target/relocation differs.");}
        var recovered=b.AsSpan(prefix).ToArray();foreach(uint slot in Slots(output.RelocationBuffer)){if(slot<prefix || Word(b,(int)slot)<prefix)throw new InvalidDataException("Inline child output pointer escaped native graph.");Put(recovered,(int)slot-prefix,Word(b,(int)slot)-(uint)prefix);}
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)))throw new InvalidDataException("Inline child native inverse differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test declarations are independent of production root descriptors. */
    //-------------------------------------------------------------------------------------------------
    private static (string Name,int Offset,uint Type)[] Fields()=>new[]{("Buildable",436,0xF57E48C8u),("ThingClass",440,0x66BA691Bu),("DisplayColor",460,0x9E3E975Bu)};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all invalid profile sources/native/output mutations must fail explicitly. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed inline root child admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls own their buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native/output slot streams must retain their expected sentinel framing. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Inline child slot stream malformed.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: owned relocation mutations preserve normal sentinel encoding. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all metadata/native reads are bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Inline child word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned output/control arrays are changed. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
