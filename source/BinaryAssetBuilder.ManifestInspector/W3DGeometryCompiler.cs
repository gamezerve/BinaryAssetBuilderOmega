extern alias W3x;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: connect existing W3X mesh/collision writers and the EP1 hash-based hierarchy ABI to the normal authored stream.
internal static class W3DGeometryCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source-normalized references retain Core occurrence order; mesh polymorphic constants receive compiler-owned type IDs. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(InstanceDeclaration instance,SchemaSet schemas)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var root=xml.DocumentElement!;
        if(root.LocalName=="W3DHierarchy")return Hierarchy(root);
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);var node=new Node(root.CreateNavigator()!,ns);var chunk=new Chunk();
        if(root.LocalName=="W3DCollisionBox"){
            W3DCollisionBox* box;using var tracker=new Tracker((void**)&box,(uint)sizeof(W3DCollisionBox),false);Marshaler.Marshal(node,box,tracker);if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Collision box allocation failed.");
            var proof=Proof(instance,schemas);proof.DocumentElement!.RemoveAttribute("id");SchemaNativeContract.Verify(proof.DocumentElement,typeof(W3DCollisionBox),0,chunk,Array.Empty<AssetId>());return chunk;
        }
        AdmitMesh(root);
        foreach(var constant in root.GetElementsByTagName("Constants",SchemaSet.XmlNamespace).OfType<XmlElement>().SelectMany(e=>e.ChildNodes.OfType<XmlElement>())){
            uint id=constant.LocalName switch{"Texture"=>0xA59096A6u,"Float"=>0xDF08DF25u,"Int"=>0x89181982u,"Bool"=>0xA3F84C3Du,_=>throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported mesh shader constant: "+constant.LocalName),constant)};constant.SetAttribute("TypeId",id.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        W3DMesh* mesh;using(var tracker=new Tracker((void**)&mesh,(uint)sizeof(W3DMesh),false)){
            W3x::Marshaler.MarshalEp1Mesh(node,mesh,tracker);if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Mesh allocation failed.");
        }
        VerifyMesh(instance,chunk,schemas);return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 hierarchy pivots store a lowercase Name Poid (4 bytes), not the older KW AnsiString (8 bytes). */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Hierarchy(XmlElement source)
    {
        var pivots=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Pivot").ToArray();var bytes=new byte[checked(12+pivots.Length*100)];Put(bytes,4,(uint)pivots.Length);if(pivots.Length>0)Put(bytes,8,12);
        var names=new HashSet<uint>();var parents=new int[pivots.Length];
        for(int i=0;i<pivots.Length;i++){
            var p=pivots[i];int at=12+i*100;string name=p.GetAttribute("Name");uint hash=name.Length==0?0:FastHash.GetHashCode(name.ToLowerInvariant());if(!names.Add(hash))throw SdkXmlDiagnostics.At(new InvalidDataException("Duplicate hierarchy pivot Name/hash: "+name),p);
            int parent=p.HasAttribute("Parent")?XmlConvert.ToInt32(p.GetAttribute("Parent")):0;if(parent< -1||parent>=pivots.Length||parent==i)throw SdkXmlDiagnostics.At(new InvalidDataException("Hierarchy Parent must name a different valid pivot or -1 for a root."),p);parents[i]=parent;
            Put(bytes,at,hash);Put(bytes,at+4,unchecked((uint)parent));int field=at+8;
            foreach(var part in new[]{("Translation",new[]{"X","Y","Z"}),("Rotation",new[]{"X","Y","Z","W"}),("FixupMatrix",new[]{"M00","M10","M20","M30","M01","M11","M21","M31","M02","M12","M22","M32","M03","M13","M23","M33"})}){
                var child=Child(p,part.Item1);foreach(string attribute in part.Item2){Put(bytes,field,BitConverter.SingleToUInt32Bits(Float(child,attribute)));field+=4;}
            }
        }
        // Reborn: preserve authored pivot order; reject real parent cycles without imposing an unproven parent-before-child XML restriction.
        var state=new byte[pivots.Length];for(int i=0;i<parents.Length;i++){int current=i;while(current>=0&&state[current]==0){state[current]=1;current=parents[current];}if(current>=0&&state[current]==1)throw SdkXmlDiagnostics.At(new InvalidDataException("Hierarchy parent cycle."),pivots[current]);current=i;while(current>=0&&state[current]==1){state[current]=2;current=parents[current];}}
        return new Chunk{InstanceBuffer=bytes,RelocationBuffer=pivots.Length==0?Array.Empty<byte>():Pack(new uint[]{8}),ImportsBuffer=Array.Empty<byte>()};
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate array cardinalities and triangle/bone bounds before the existing unsafe vertex converter reads their memory. */
    //-------------------------------------------------------------------------------------------------
    internal static void AdmitMesh(XmlElement mesh)
    {
        foreach(XmlAttribute attribute in mesh.SelectNodes(".//@TypeId")!)throw SdkXmlDiagnostics.At(new InvalidDataException("Shader TypeId is compiler-generated; omit it from W3X."),attribute);
        var vertices=mesh.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Vertices").ToArray();int count=vertices.FirstOrDefault()?.ChildNodes.OfType<XmlElement>().Count()??0;if(count==0)throw SdkXmlDiagnostics.At(new InvalidDataException("Mesh requires nonempty vertex data."),mesh);
        var arrays=mesh.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "Vertices" or "Normals" or "Tangents" or "Binormals" or "VertexColors" or "TexCoords" or "BoneInfluences" or "ShadeIndices").ToArray();
        foreach(var array in arrays)if(array.ChildNodes.OfType<XmlElement>().Count()!=count)throw SdkXmlDiagnostics.At(new InvalidDataException("Mesh "+array.LocalName+" count differs from Vertices ("+count+")."),array);
        if(arrays.Count(e=>e.LocalName=="BoneInfluences")>4)throw SdkXmlDiagnostics.At(new InvalidDataException("PC mesh vertex declaration supports at most four bone influences per vertex."),mesh);
        if(arrays.Where(e=>e.LocalName=="BoneInfluences").SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).Select(e=>XmlConvert.ToUInt16(e.GetAttribute("Bone"))).Distinct().Count()>256)throw SdkXmlDiagnostics.At(new InvalidDataException("PC mesh skin palette exceeds its byte index capacity (256 bones)."),mesh);
        foreach(var triangle in Child(mesh,"Triangles").ChildNodes.OfType<XmlElement>()){
            var indices=triangle.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="V").ToArray();if(indices.Length!=3||indices.Any(e=>XmlConvert.ToUInt32(e.InnerText)>=count))throw SdkXmlDiagnostics.At(new InvalidDataException("Mesh triangle requires three valid vertex indices."),triangle);
        }
        // Reborn: an omitted tangent/binormal pair is valid; a single half cannot be silently discarded by the converter.
        if(arrays.Any(e=>e.LocalName=="Tangents")!=arrays.Any(e=>e.LocalName=="Binormals"))throw SdkXmlDiagnostics.At(new InvalidDataException("Mesh Tangents and Binormals must be provided together."),mesh);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove emitted vertex/declaration/index counts, source positions/UVs, skin maps and shader imports independently of the producer. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyMesh(InstanceDeclaration instance,Chunk chunk,SchemaSet schemas)
    {
        var proof=Proof(instance,schemas);var source=proof.DocumentElement!;var b=chunk.InstanceBuffer;int data=checked((int)Word(b,4));int count=checked((int)Word(b,data)),stride=checked((int)Word(b,data+4)),vertices=checked((int)Word(b,data+8));
        var positions=Child(source,"Vertices").ChildNodes.OfType<XmlElement>().ToArray();if(count!=positions.Length||stride<12||stride>512)throw new InvalidDataException("Mesh vertex count/stride differs.");Range(b,vertices,checked(count*stride));
        int declaration=checked((int)Word(b,data+16)),declarationSize=checked((int)Word(b,data+12));Range(b,declaration,declarationSize);
        int position=0,texcoords=0;var boneSources=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="BoneInfluences").Select(e=>e.ChildNodes.OfType<XmlElement>().ToArray()).ToArray();int palette=(int)Word(b,data+24),paletteCount=(int)Word(b,data+20);Range(b,palette,checked(paletteCount*2));
        foreach(var channel in Channels(b,declaration,declarationSize)){int offset=channel.Offset,type=channel.Type,usage=channel.Usage,index=channel.Index;if(offset>=stride)throw new InvalidDataException("Mesh declaration offset exceeds vertex stride.");
            if(usage is 0 or 3 or 6 or 7){string field=usage switch{0=>"Vertices",3=>"Normals",6=>"Tangents",_=>"Binormals"};var values=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==field).ElementAt(index).ChildNodes.OfType<XmlElement>().ToArray();if(type!=2||offset+12>stride)throw new InvalidDataException("Mesh vector declaration differs: "+field);if(usage==0)position++;for(int i=0;i<count;i++)foreach(var (name,delta) in new[]{("X",0),("Y",4),("Z",8)})EqualFloat(b,vertices+i*stride+offset+delta,Float(values[i],name));}
            if(usage==5){var uv=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="TexCoords").ElementAt(index).ChildNodes.OfType<XmlElement>().ToArray();if(type!=1||offset+8>stride)throw new InvalidDataException("Mesh UV declaration differs.");for(int i=0;i<count;i++){EqualFloat(b,vertices+i*stride+offset,Float(uv[i],"X"));EqualFloat(b,vertices+i*stride+offset+4,1-Float(uv[i],"Y"));}texcoords++;}
            if(usage==2){if(type!=4||offset+4>stride)throw new InvalidDataException("Mesh skin index declaration differs.");for(int i=0;i<count;i++)for(int j=0;j<boneSources.Length;j++){byte bone=b[vertices+i*stride+offset+j];if(bone>=paletteCount||BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(palette+bone*2,2))!=XmlConvert.ToUInt16(boneSources[j][i].GetAttribute("Bone")))throw new InvalidDataException("Mesh skin bone index differs.");}}
            if(usage==1){if(type!=4||offset+4>stride)throw new InvalidDataException("Mesh skin weight declaration differs.");for(int i=0;i<count;i++){float sum=boneSources.Sum(s=>Float(s[i],"Weight"));int total=0;if(sum<=0)throw new InvalidDataException("Mesh skin weights have zero total.");for(int j=0;j<4;j++){byte value=b[vertices+i*stride+offset+j];total+=value;float expected=j<boneSources.Length?Float(boneSources[j][i],"Weight")/sum:0;if(MathF.Abs(value/255f-expected)>1/255f+1e-6f)throw new InvalidDataException("Mesh quantized skin weight differs.");}if(total!=255)throw new InvalidDataException("Mesh quantized skin weights must sum to 255.");}}
        }
        if(position!=source.ChildNodes.OfType<XmlElement>().Count(e=>e.LocalName=="Vertices")||texcoords!=source.ChildNodes.OfType<XmlElement>().Count(e=>e.LocalName=="TexCoords"))throw new InvalidDataException("Mesh position/UV source channels were lost.");
        int shader=Marshal.OffsetOf<W3DMesh>(nameof(W3DMesh.FXShader)).ToInt32();SchemaNativeContract.VerifyStock(Child(source,"FXShader"),typeof(FXShaderMaterial),0,b,shader,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());
        // Reborn: imports must be in-bounds and consume exactly the Core reference occurrences; relocations must point inside this BIN chunk.
        var imports=Slots(chunk.ImportsBuffer);if(imports.Length!=instance.ReferencedInstances.Count)throw new InvalidDataException("Mesh shader import occurrence count differs.");foreach(uint slot in imports){uint value=Word(b,checked((int)slot));if(value==0||value>instance.ReferencedInstances.Count)throw new InvalidDataException("Mesh shader import index differs.");}
        foreach(uint slot in Slots(chunk.RelocationBuffer)){uint target=Word(b,checked((int)slot));if(target>=b.Length)throw new InvalidDataException("Mesh relocation target exceeds BIN.");}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore normalized reference spellings for independent schema/native proof without changing the real Core instance. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Proof(InstanceDeclaration instance,SchemaSet schemas)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));
        foreach(XmlNode node in xml.SelectNodes("//@* | //*[not(*)]")!){string value=node is XmlAttribute a?a.Value:node.InnerText;int slash=value.LastIndexOf('\\');if(slash>=0&&uint.TryParse(value[(slash+1)..],out uint index)&&index<instance.ReferencedInstances.Count){string name=instance.ReferencedInstances[(int)index].Name;if(node is XmlAttribute attribute)attribute.Value=name;else node.InnerText=name;}}
        xml.Schemas.Add(schemas.Schemas);xml.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse the stock EP1 ASCII declaration into independent semantic/offset/format expectations. */
    //-------------------------------------------------------------------------------------------------
    internal static (int Offset,int Type,int Usage,int Index)[] Channels(byte[] bytes,int start,int length)
    {
        Range(bytes,start,length);var channels=new System.Collections.Generic.List<(int,int,int,int)>();var seen=new HashSet<(int,int)>();
        foreach(string text in Encoding.ASCII.GetString(bytes,start,length).Split(' ',StringSplitOptions.RemoveEmptyEntries)){
            var match=System.Text.RegularExpressions.Regex.Match(text,"^([pncgbtiw])([0-9]+):([0-9A-Fa-f]+):([1-4])(f32|u8n?)$");if(!match.Success)throw new InvalidDataException("Unsupported EP1 vertex descriptor: "+text);
            int usage=match.Groups[1].Value switch{"p"=>0,"w"=>1,"i"=>2,"n"=>3,"t"=>5,"g"=>6,"b"=>7,"c"=>10,_=>throw new InvalidDataException("Unknown vertex semantic.")};int index=int.Parse(match.Groups[2].Value,System.Globalization.CultureInfo.InvariantCulture),offset=int.Parse(match.Groups[3].Value,System.Globalization.NumberStyles.HexNumber,System.Globalization.CultureInfo.InvariantCulture),components=int.Parse(match.Groups[4].Value,System.Globalization.CultureInfo.InvariantCulture);string format=match.Groups[5].Value;
            int type=format=="f32"?components-1:components==4?4:throw new InvalidDataException("Unsupported EP1 packed vertex width.");if(!seen.Add((usage,index)))throw new InvalidDataException("Duplicate EP1 vertex descriptor.");channels.Add((offset,type,usage,index));
        }
        if(channels.Count==0)throw new InvalidDataException("Empty EP1 vertex declaration.");return channels.ToArray();
    }

    // Reborn: source geometry values retain their exact float32 spelling and reject non-finite runtime coordinates.
    private static float Float(XmlElement source,string attribute){float value=source.HasAttribute(attribute)?XmlConvert.ToSingle(source.GetAttribute(attribute)):0;if(!float.IsFinite(value))throw SdkXmlDiagnostics.At(new InvalidDataException("Non-finite model field: "+attribute),source);return value;}
    // Reborn: XML model children are selected by schema name rather than authored formatting.
    private static XmlElement Child(XmlElement source,string name)=>source.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==name)??throw SdkXmlDiagnostics.At(new InvalidDataException("Missing model field: "+name),source);
    // Reborn: independent float verification compares emitted values to the source, not to a frozen stock value.
    private static void EqualFloat(byte[] b,int at,float value){if(Word(b,at)!=BitConverter.SingleToUInt32Bits(value))throw new InvalidDataException("Mesh source float differs at +"+at);}
    // Reborn: bounded native ranges prevent malformed pointers/counts from reaching readback logic.
    private static void Range(byte[] b,int at,int size){if(at<0||size<0||at>b.Length-size)throw new InvalidDataException("Model native range exceeds BIN.");}
    // Reborn: runtime words use the EP1 PC little-endian ABI.
    private static uint Word(byte[] b,int at){Range(b,at,4);return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    // Reborn: only newly allocated hierarchy buffers receive field writes.
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
    // Reborn: RELO/IMP framing has unique offsets and one sentinel for a nonempty list.
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8||b.Length%4!=0||Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Model auxiliary framing differs.");var values=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(values.Distinct().Count()!=values.Length)throw new InvalidDataException("Model auxiliary duplicate slot.");return values;}
    // Reborn: hierarchy's sole relocation is its pivot list pointer.
    private static byte[] Pack(uint[] values){var b=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(b,i*4,values[i]);Put(b,values.Length*4,uint.MaxValue);return b;}
}
