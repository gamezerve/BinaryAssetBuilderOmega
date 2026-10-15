using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recover geometry through independent stock field tokens and compare complete native records rather than assuming the legacy KW header.
internal static class Ep1GeometryNativeSmokeTest
{
    private const string Ns="uri:ea.com:eala:asset";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: audit stock collision/placement/contact/RING graphs, official enum/default contracts and authored/native/root-dispatch controls without publication. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,bool includeTokens=false)
    {
        if(Marshal.SizeOf<Ep1Geometry>()!=28 || Marshal.SizeOf<Ep1Shape>()!=48 || Marshal.SizeOf<ContactPoint>()!=20 || Marshal.OffsetOf<GameObject>("Geometry").ToInt32()!=500)throw new InvalidDataException("EP1 geometry ABI differs.");
        XmlSchemaSet? schemas=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>schemas=s);if(schemas==null)throw new InvalidDataException("Reviewed geometry schema required.");
        foreach(var pair in new[]{("GeometryType",Enum.GetNames<Ep1GeometryType>()),("ContactPointGenerationType",Enum.GetNames<ContactPointGenerationType>())})
        {
            var type=(XmlSchemaSimpleType)schemas.GlobalTypes[new XmlQualifiedName(pair.Item1,Ns)]!;var names=((XmlSchemaSimpleTypeRestriction)type.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f=>f.Value!);
            if(!names.SequenceEqual(pair.Item2))throw new InvalidDataException("EP1 geometry enum name/order drift.");
        }
        int stockCount=0;
        foreach(string name in new[]{"AlliedConstructionYard","SovietAntiInfantryInfantry","JapanFortressShip","AlliedPowerPlantTurbine","NanoSwarmHiveEffect"})
        {
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);int geometry=(int)Word(stock.Bytes,stock.Projection.NativeRoot+500);var root=GameObjectStockReview.Tokens(stock.Bytes,0);int table=root[FastHash.GetHashCode("Geometry")].Offset;
            var source=Decode(stock.Bytes,table);Validate(schemas,source);var native=Compile(source);
            var mapping=Compare(stock.Bytes,geometry,native.InstanceBuffer,0);CheckOwnership(native);Root(source,native);if(includeTokens)Ep1GeometryTokenSmokeTest.Stock(native,stock.Bytes,table,mapping);stockCount++;
        }
        int cases=0;Chunk? populated=null;
        foreach(int shapes in new[]{1,3,32})foreach(int contacts in new[]{0,2})foreach(bool rotation in new[]{false,true})foreach(bool placement in new[]{false,true})
        {
            string type=Enum.GetNames<Ep1GeometryType>()[cases%4],contact=Enum.GetNames<ContactPointGenerationType>()[cases%5];
            string shape=$"<Shape Type='{type}' ContactPointGeneration='{contact}' Name='shape name' Other='other geometry' MajorRadius='75' MinorRadius='12.5' Height='30' FrontAngle='90d' Active='false' UsedForHealthBox='true'><Offset x='-2' y='3' z='4'/></Shape>";
            var source=Xml("<Geometry xmlns='"+Ns+"' IsSmall='true'>"+(rotation?"<RotationAnchorOffset x='2' y='-3'/>":"")+(placement?"<PlacementShape Type='CYLINDER' MajorRadius='80' Height='5'/>":"")+string.Concat(Enumerable.Repeat(shape,shapes))+string.Concat(Enumerable.Repeat("<ContactPoint Label='contact label'><Pos x='11' y='-12' z='13'/></ContactPoint>",contacts))+"</Geometry>");
            Validate(schemas,source);var a=Compile(source);CheckOwnership(a);Root(source,a);var b=Compile(source);
            if(!a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) || !a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) || a.ImportsBuffer.Length!=0 || Word(a.InstanceBuffer,8)!=shapes || Word(a.InstanceBuffer,16)!=contacts || a.InstanceBuffer[24]!=1)throw new InvalidDataException("Authored geometry header/repeat differs.");
            int first=(int)Word(a.InstanceBuffer,12);
            if(Word(a.InstanceBuffer,first)!=(uint)Enum.Parse<Ep1GeometryType>(type) || Word(a.InstanceBuffer,first+4)!=(uint)Enum.Parse<ContactPointGenerationType>(contact) || Word(a.InstanceBuffer,first+16)!=BitConverter.SingleToUInt32Bits(75) || Word(a.InstanceBuffer,first+20)!=BitConverter.SingleToUInt32Bits(12.5f) || Word(a.InstanceBuffer,first+32)!=BitConverter.SingleToUInt32Bits(30) || Math.Abs(BitConverter.Int32BitsToSingle((int)Word(a.InstanceBuffer,first+36))-MathF.PI/2)>0.000001f || a.InstanceBuffer[first+44]!=0 || a.InstanceBuffer[first+45]!=1)throw new InvalidDataException("Authored shape enum/scalar/angle/bool differs.");
            cases++;
            // Reborn: the token follow-up shares source/native coverage but independently verifies metadata and corruption controls.
            if(includeTokens){Ep1GeometryTokenSmokeTest.Check(a);populated=a;}
        }
        var defaults=Compile(Xml("<Geometry xmlns='"+Ns+"'><Shape/></Geometry>"));int defaultShape=(int)Word(defaults.InstanceBuffer,12);
        foreach(int offset in new[]{16,20,32})if(Word(defaults.InstanceBuffer,defaultShape+offset)!=BitConverter.SingleToUInt32Bits(1f))throw new InvalidDataException("Shape radius/height default differs.");
        if(Word(defaults.InstanceBuffer,0)!=0 || Word(defaults.InstanceBuffer,4)!=0 || Word(defaults.InstanceBuffer,16)!=0 || defaults.InstanceBuffer[24]!=0 || Word(defaults.InstanceBuffer,defaultShape)!=0 || Word(defaults.InstanceBuffer,defaultShape+4)!=0 || defaults.InstanceBuffer[defaultShape+44]!=1 || defaults.InstanceBuffer[defaultShape+45]!=1)throw new InvalidDataException("Geometry/shape omitted defaults differ.");
        if(includeTokens){Ep1GeometryTokenSmokeTest.Check(defaults);Ep1GeometryTokenSmokeTest.Controls(populated!);}
        int failures=0;
        foreach(string bad in new[]{"<Geometry xmlns='"+Ns+"'/>","<Geometry xmlns='"+Ns+"'><Shape Type='UNKNOWN'/></Geometry>","<Geometry xmlns='"+Ns+"'><Shape ContactPointGeneration='UNKNOWN'/></Geometry>","<Geometry xmlns='"+Ns+"'><Shape/><ContactPoint/></Geometry>","<Geometry xmlns='"+Ns+"'><Shape MajorRadius='not-a-real'/></Geometry>","<Geometry xmlns='"+Ns+"'><PlacementShape/><PlacementShape/><Shape/></Geometry>"}){Refuse(()=>Validate(schemas,Xml(bad)));failures++;}
        Console.WriteLine($"EP1 geometry native: PASS ({stockCount} complete selected stock graphs via independent tokens, placement/contact/RING coverage, {cases} authored cases plus defaults, real root dispatch, {failures} schema refusals; token follow-up={includeTokens}, no full object publication/game).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent stock tokens reconstruct a semantic source projection, not captured source indices or copied native allocation bytes. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Decode(byte[] data,int table)
    {
        var source=Xml("<Geometry xmlns='"+Ns+"'/>");var fields=GameObjectStockReview.Tokens(data,table);source.DocumentElement!.SetAttribute("IsSmall",Bool(data,fields[FastHash.GetHashCode("IsSmall")].Offset));
        int rotation=fields[FastHash.GetHashCode("RotationAnchorOffset")].Offset;if(rotation!=0)Vector(source.DocumentElement,"RotationAnchorOffset",rotation,2);
        int placement=fields[FastHash.GetHashCode("PlacementShape")].Offset;if(placement!=0)Shape("PlacementShape",placement);
        List("Shape",row=>Shape("Shape",row));
        List("ContactPoint",row=>{var contact=source.CreateElement("ContactPoint",Ns);source.DocumentElement.AppendChild(contact);var f=GameObjectStockReview.Tokens(data,row);String(contact,"Label",f[FastHash.GetHashCode("Label")].Offset);Vector(contact,"Pos",f[FastHash.GetHashCode("Pos")].Offset,3);});return source;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: count-delimited stock list markers remain bounded and distinct from native items pointers. */
        //-------------------------------------------------------------------------------------------------
        void List(string name,Action<int> visit){int at=fields[FastHash.GetHashCode(name)].Offset;if(Word(data,at)!=0x6C827073 || Word(data,at+4)!=0x5D448FD8)throw new InvalidDataException("Stock geometry count metadata differs.");int count=(int)Word(data,(int)Word(data,at+8));if(count<0 || count>128)throw new InvalidDataException("Stock geometry list exceeds review bound.");for(int i=0;i<count;i++){int marker=at+12+i*12;if(Word(data,marker)!=0xB151A0EB || Word(data,marker+4)!=0)throw new InvalidDataException("Stock geometry element marker differs.");visit((int)Word(data,marker+8));}}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: vector tokens target native fixed-size coordinates directly, not another named-field metadata table. */
        //-------------------------------------------------------------------------------------------------
        void Vector(XmlElement parent,string name,int at,int dimension){var element=source.CreateElement(name,Ns);var axes=new[]{"x","y","z"};for(int i=0;i<dimension;i++)element.SetAttribute(axes[i],Real(data,at+i*4));parent.AppendChild(element);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: absent strings differ from explicitly allocated empty strings. */
        //-------------------------------------------------------------------------------------------------
        void String(XmlElement parent,string name,int at){if(Word(data,at+4)!=0)parent.SetAttribute(name,ReadString(data,at));else if(Word(data,at)!=0)throw new InvalidDataException("Stock absent geometry string has length.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: shape attributes are recovered by independent field names; front angles are zero in these selected stock graphs. */
        //-------------------------------------------------------------------------------------------------
        void Shape(string name,int at)
        {
            var f=GameObjectStockReview.Tokens(data,at);var shape=source.CreateElement(name,Ns);source.DocumentElement.AppendChild(shape);
            shape.SetAttribute("Type",Enum.GetName((Ep1GeometryType)Word(data,f[FastHash.GetHashCode("Type")].Offset))??throw new InvalidDataException("Unreviewed stock geometry type."));
            shape.SetAttribute("ContactPointGeneration",Enum.GetName((ContactPointGenerationType)Word(data,f[FastHash.GetHashCode("ContactPointGeneration")].Offset))??throw new InvalidDataException("Unreviewed stock contact type."));
            foreach(string field in new[]{"MajorRadius","MinorRadius","Height"})shape.SetAttribute(field,Real(data,f[FastHash.GetHashCode(field)].Offset));
            foreach(string field in new[]{"Name","Other"})String(shape,field,f[FastHash.GetHashCode(field)].Offset);
            foreach(string field in new[]{"Active","UsedForHealthBox"})shape.SetAttribute(field,Bool(data,f[FastHash.GetHashCode(field)].Offset));
            if(Word(data,f[FastHash.GetHashCode("FrontAngle")].Offset)!=0)throw new InvalidDataException("Selected stock geometry angle is no longer zero.");
            int offset=f[FastHash.GetHashCode("Offset")].Offset;if(offset!=0)Vector(shape,"Offset",offset,3);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every native header/record byte and recursively owned vector/string payload while allowing stock interning and different base addresses. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<int,int> Compare(byte[] stock,int original,byte[] native,int current)
    {
        // Reborn: token targets reference native records/vectors, so this independent ownership map also supports stock metadata rebasing.
        var mapping=new Dictionary<int,int>();
        Block(original,current,28,new[]{0,4,12,20});Vector(original,current,0,8);ShapePointer(original,current,4);
        int shapes=(int)Word(stock,original+8),contacts=(int)Word(stock,original+16),a=(int)Word(stock,original+12),b=(int)Word(native,current+12);
        for(int i=0;i<shapes;i++)Shape(a+i*48,b+i*48);
        a=(int)Word(stock,original+20);b=(int)Word(native,current+20);for(int i=0;i<contacts;i++){Block(a+i*20,b+i*20,20,new[]{4});Text(a+i*20,b+i*20);}return mapping;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: pointer values are compared through owner contents, never by subtracting a guessed global prefix from a shared stock string. */
        //-------------------------------------------------------------------------------------------------
        void Block(int x,int y,int length,int[] pointers){Map(x,y,length);var expected=stock.AsSpan(x,length).ToArray();foreach(int p in pointers){if((Word(stock,x+p)==0)!=(Word(native,y+p)==0))throw new InvalidDataException("Stock geometry pointer presence differs.");BinaryPrimitives.WriteUInt32LittleEndian(expected.AsSpan(p,4),Word(native,y+p));}if(!expected.AsSpan().SequenceEqual(native.AsSpan(y,length)))throw new InvalidDataException("Complete stock geometry native record differs.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: geometry rotation and shape offsets own fixed-sized vector records. */
        //-------------------------------------------------------------------------------------------------
        void Vector(int x,int y,int offset,int length){int a=(int)Word(stock,x+offset),b=(int)Word(native,y+offset);if(a!=0){Map(a,b,length);if(!stock.AsSpan(a,length).SequenceEqual(native.AsSpan(b,length)))throw new InvalidDataException("Stock geometry vector differs.");}}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: string length/terminator/content must all match even when addresses differ. */
        //-------------------------------------------------------------------------------------------------
        void Text(int x,int y){if(ReadString(stock,x)!=ReadString(native,y))throw new InvalidDataException("Stock geometry string differs.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: placement pointer presence is distinct from an optional shape offset. */
        //-------------------------------------------------------------------------------------------------
        void ShapePointer(int x,int y,int offset){int a=(int)Word(stock,x+offset);if(a!=0)Shape(a,(int)Word(native,y+offset));}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: shape equality covers all 48 bytes including the two flags and alignment padding. */
        //-------------------------------------------------------------------------------------------------
        void Shape(int x,int y){Block(x,y,48,new[]{12,28,40});Text(x+8,y+8);Text(x+24,y+24);Vector(x,y,40,12);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: stock metadata targets must map to one reviewed native field/vector owner, never an arbitrary offset heuristic. */
        //-------------------------------------------------------------------------------------------------
        void Map(int x,int y,int length){for(int i=0;i<length;i++)if(mapping.TryGetValue(x+i,out int old)){if(old!=y+i)throw new InvalidDataException("Ambiguous stock geometry native owner map.");}else mapping.Add(x+i,y+i);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: standalone compilation uses the new real marshaller overload with one owned Tracker. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlDocument source)
    {
        var ns=new XmlNamespaceManager(source.NameTable);ns.AddNamespace("ea",Ns);Ep1Geometry* geometry;using var tracker=new Tracker((void**)&geometry,(uint)sizeof(Ep1Geometry),false);Marshaler.Marshal(new Node(source.DocumentElement!.CreateNavigator()!,ns),geometry,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("EP1 geometry allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: actual root dispatch must select the 28-byte header; normalize only the root-to-standalone pointer origin for comparison. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Root(XmlDocument geometry,Chunk standalone)
    {
        var source=Xml("<GameObject xmlns='"+Ns+"' id='GeometryRoot'/>");source.DocumentElement!.AppendChild(source.ImportNode(geometry.DocumentElement!,true));var ns=new XmlNamespaceManager(source.NameTable);ns.AddNamespace("ea",Ns);GameObject* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(GameObject),false);Marshaler.Marshal(new Node(source.DocumentElement.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk) || Word(chunk.InstanceBuffer,500)!=600)throw new InvalidDataException("EP1 root geometry allocator differs.");
        var tail=chunk.InstanceBuffer.AsSpan(600).ToArray();foreach(uint slot in Slots(chunk.RelocationBuffer).Where(s=>s!=500))BinaryPrimitives.WriteUInt32LittleEndian(tail.AsSpan((int)slot-600,4),Word(chunk.InstanceBuffer,(int)slot)-600);
        if(!tail.SequenceEqual(standalone.InstanceBuffer) || !Slots(chunk.RelocationBuffer).Where(s=>s!=500).Select(s=>s-600).SequenceEqual(Slots(standalone.RelocationBuffer)))throw new InvalidDataException("Root/standalone geometry native graph differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all geometry relocations point at owned payload and no geometry field is a strong asset import. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckOwnership(Chunk chunk)
    {
        if(chunk.ImportsBuffer.Length!=0)throw new InvalidDataException("Geometry unexpectedly imports an asset.");var expected=new List<uint>();
        foreach(int pointer in new[]{0,4,12,20})Add(pointer);
        int placement=(int)Word(chunk.InstanceBuffer,4);if(placement!=0)Shape(placement);
        int shapes=(int)Word(chunk.InstanceBuffer,8),items=(int)Word(chunk.InstanceBuffer,12);for(int i=0;i<shapes;i++)Shape(items+i*48);
        int contacts=(int)Word(chunk.InstanceBuffer,16);items=(int)Word(chunk.InstanceBuffer,20);for(int i=0;i<contacts;i++)Add(items+i*20+4);
        if(!expected.Order().SequenceEqual(Slots(chunk.RelocationBuffer).Order()))throw new InvalidDataException("Geometry exact relocation ownership differs.");
        foreach(uint slot in expected)if(slot%4!=0 || slot>chunk.InstanceBuffer.Length-4 || Word(chunk.InstanceBuffer,(int)slot)<28 || Word(chunk.InstanceBuffer,(int)slot)>=chunk.InstanceBuffer.Length)throw new InvalidDataException("Geometry relocation exceeds owner bounds.");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: nullable pointers emit exactly one relocation only when a payload is present. */
        //-------------------------------------------------------------------------------------------------
        void Add(int slot){if(Word(chunk.InstanceBuffer,slot)!=0)expected.Add((uint)slot);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only the two string pointers and optional vector pointer belong to a native shape. */
        //-------------------------------------------------------------------------------------------------
        void Shape(int start){foreach(int offset in new[]{12,28,40})Add(start+offset);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate geometry with an otherwise minimal official GameObject wrapper and required Body. */
    //-------------------------------------------------------------------------------------------------
    private static void Validate(XmlSchemaSet schemas,XmlDocument source)=>GameObjectBodyProjectionSmokeTest.Validate(schemas,"<GameObject xmlns='"+Ns+"' id='GeometryProbe'><Body><ActiveBody id='Body' MaxHealth='100'/></Body>"+source.DocumentElement!.OuterXml+"</GameObject>");
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed XML/schema examples must fail before marshalling. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(InvalidDataException){return;}throw new InvalidDataException("Invalid geometry source admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixture and reconstructed stock XML cannot resolve external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument{XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: ASCII stock strings include their explicit null terminator and preserve absence separately from empty allocation. */
    //-------------------------------------------------------------------------------------------------
    private static string ReadString(byte[] b,int at){int length=(int)Word(b,at),pointer=(int)Word(b,at+4);if(pointer==0){if(length!=0)throw new InvalidDataException("Absent geometry string length differs.");return "";}if(length<0 || pointer<0 || pointer>b.Length-length-1 || b[pointer+length]!=0 || b.AsSpan(pointer,length).IndexOfAnyInRange((byte)128,byte.MaxValue)>=0)throw new InvalidDataException("Invalid bounded ASCII geometry string.");return Encoding.ASCII.GetString(b,pointer,length);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: finite stock real values round-trip as invariant XML literals. */
    //-------------------------------------------------------------------------------------------------
    private static string Real(byte[] b,int at){float value=BitConverter.Int32BitsToSingle((int)Word(b,at));if(!float.IsFinite(value))throw new InvalidDataException("Nonfinite stock geometry scalar.");return value.ToString("R",CultureInfo.InvariantCulture);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native flags must be exactly zero or one, not arbitrary nonzero bytes. */
    //-------------------------------------------------------------------------------------------------
    private static string Bool(byte[] b,int at)=>b[at] switch{0=>"false",1=>"true",_=>throw new InvalidDataException("Stock geometry boolean differs.")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validated Tracker streams end with a sentinel excluded from slot enumeration. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native and metadata word reads remain bounded little-endian operations. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
