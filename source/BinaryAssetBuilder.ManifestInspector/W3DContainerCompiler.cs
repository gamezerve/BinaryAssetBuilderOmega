using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: W3DContainer is an ordinary untokenized XML asset; W3X is its source extension, not a different binary format.
internal static class W3DContainerCompiler
{
    private static bool qualified;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the existing XML marshaler with Core-owned hierarchy and render reference occurrence indices. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        W3DContainer* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(W3DContainer),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();
        if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("W3DContainer native allocation failed.");return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently check every authored bone, string, import and list against the EP1 16-byte root/subobject ABI. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(InstanceDeclaration instance,Chunk chunk)
    {
        if(Marshal.SizeOf<W3DContainer>()!=16||Marshal.SizeOf<SubObject>()!=16)throw new InvalidDataException("W3DContainer ABI size differs.");
        var source=instance.XmlNode;var b=chunk.InstanceBuffer;var relos=new HashSet<uint>();var imports=new HashSet<uint>();var covered=new bool[b.Length];
        Own(0,16);if(Word(b,0)!=0)throw new InvalidDataException("W3DContainer base differs.");
        Reference(4,source.Attributes?["Hierarchy"]?.Value??"");
        var children=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="SubObject").ToArray();
        if(Word(b,8)!=children.Length)throw new InvalidDataException("W3DContainer subobject count differs.");
        int start=checked((int)Word(b,12));if(children.Length>0){relos.Add(12);Own(start,checked(children.Length*16));}else if(start!=0)throw new InvalidDataException("Empty W3DContainer list pointer differs.");
        for(int i=0;i<children.Length;i++){
            var child=children[i];int at=start+i*16;uint bone=XmlConvert.ToUInt32(child.GetAttribute("BoneIndex"));
            if(Word(b,at)!=bone)throw new InvalidDataException("W3DContainer BoneIndex differs.");
            string name=child.GetAttribute("SubObjectID");int length=checked((int)Word(b,at+4)),text=checked((int)Word(b,at+8));
            if(length!=name.Length)throw new InvalidDataException("W3DContainer SubObjectID length differs.");
            Own(text,checked(length+1));relos.Add((uint)(at+8));
            if(b[text+length]!=0||Encoding.Latin1.GetString(b,text,length)!=name)throw new InvalidDataException("W3DContainer SubObjectID differs or uses an unsupported ANSI character.");
            var render=child.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="RenderObject").ChildNodes.OfType<XmlElement>().Single();Reference(at+12,render.InnerText);
        }
        if(!Slots(chunk.RelocationBuffer).Order().SequenceEqual(relos.Order())||!Slots(chunk.ImportsBuffer).Order().SequenceEqual(imports.Order())||imports.Count!=instance.ReferencedInstances.Count)throw new InvalidDataException("W3DContainer RELO/IMP ownership differs.");
        int padding=0;for(int i=0;i<b.Length;i++){if(covered[i])padding=0;else if(b[i]!=0||++padding>3)throw new InvalidDataException("W3DContainer unowned native storage.");}

        // Reborn: Core stores zero-based indices in normalized XML; MakeRelocatable writes index+1 because native zero means null.
        void Reference(int at,string value){uint expected=value.Contains('\\')?checked(uint.Parse(value[(value.LastIndexOf('\\')+1)..],System.Globalization.CultureInfo.InvariantCulture)+1):0;if(Word(b,at)!=expected||expected>instance.ReferencedInstances.Count)throw new InvalidDataException("W3DContainer reference index differs at +"+at+": expected="+expected+", actual="+Word(b,at)+", occurrences="+instance.ReferencedInstances.Count+", value="+value);if(expected!=0)imports.Add((uint)at);}
        // Reborn: array and string ranges must remain inside the freshly compiled chunk without aliased owner storage.
        void Own(int at,int count){if(at<0||count<0||at>b.Length-count)throw new InvalidDataException("W3DContainer native range differs.");for(int i=at;i<at+count;i++){if(covered[i])throw new InvalidDataException("W3DContainer overlapping native storage.");covered[i]=true;}}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reconstruct selected original EP1 containers and compare their complete typed native graph, never copy stock into authored output. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(qualified)return;var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));
        var samples=document.Assets.Where(a=>a.Name.StartsWith("W3DContainer:",StringComparison.Ordinal)&&a.InstanceDataSize<=65536).Take(3).ToArray();
        if(samples.Length==0)throw new InvalidDataException("EP1 W3DContainer stock witnesses missing.");
        foreach(var asset in samples){
            if(asset.Tokenized!=0)throw new InvalidDataException("W3DContainer unexpectedly tokenized.");var bytes=ReadStock(manifest,document,asset);
            var xml=Decode(bytes,asset,catalog);xml.Schemas.Add(schemas.Schemas);xml.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
            ConscriptBehaviorSurvey.ResolveStrong(xml,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,xml.OuterXml);var chunk=Compile(instance.XmlNode);Verify(instance,chunk);
            new SelectedNativeGraphComparer(bytes,asset.References.ToArray(),chunk,instance,captureNativeCoordinates:false).Compare(typeof(W3DContainer),0);
        }
        qualified=true;Console.WriteLine("Reborn: EP1 W3DContainer stock qualification: PASS ("+samples.Length+" complete native graphs, hierarchy/mesh/collision imports).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode the fixed stock fields to source witnesses; the separate writer and native comparer must reproduce every value. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Decode(byte[] b,ManifestAsset asset,ManifestAsset[] catalog)
    {
        var xml=new XmlDocument{XmlResolver=null};var root=xml.CreateElement("W3DContainer",SchemaSet.XmlNamespace);xml.AppendChild(root);root.SetAttribute("id",asset.Name[(asset.Name.IndexOf(':')+1)..]);
        string Target(uint index){if(index<1||index>asset.References.Count)throw new InvalidDataException("Stock W3DContainer import index differs.");var id=asset.References[(int)index-1];return catalog.First(a=>a.TypeId==id.TypeId&&a.InstanceId==id.InstanceId).Name;}
        if(Word(b,4)!=0)root.SetAttribute("Hierarchy",Target(Word(b,4)));int count=checked((int)Word(b,8)),start=checked((int)Word(b,12));if(count>4096)throw new InvalidDataException("Stock W3DContainer list exceeds witness bound.");
        for(int i=0;i<count;i++){
            int at=checked(start+i*16);var child=xml.CreateElement("SubObject",SchemaSet.XmlNamespace);root.AppendChild(child);child.SetAttribute("BoneIndex",Word(b,at).ToString(System.Globalization.CultureInfo.InvariantCulture));int length=checked((int)Word(b,at+4)),text=checked((int)Word(b,at+8));if(length>65536||text<0||text>b.Length-length)throw new InvalidDataException("Stock W3DContainer string bounds differ.");child.SetAttribute("SubObjectID",Encoding.Latin1.GetString(b,text,length));
            string name=Target(Word(b,at+12));var render=xml.CreateElement("RenderObject",SchemaSet.XmlNamespace);child.AppendChild(render);string type=name[..name.IndexOf(':')];var leaf=xml.CreateElement(type=="W3DMesh"?"Mesh":type=="W3DCollisionBox"?"CollisionBox":throw new InvalidDataException("Unknown W3DContainer render type: "+type),SchemaSet.XmlNamespace);render.AppendChild(leaf);leaf.InnerText=name;
        }
        return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded witness reads check the linked stream checksum and never dump the full BIN. */
    //-------------------------------------------------------------------------------------------------
    internal static byte[] ReadStock(string manifest,ManifestDocument document,ManifestAsset asset)
    {
        string path=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(path,null,0,8);if(Word(header,0)!=0xBABB0000||Word(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("W3DContainer stream checksum differs.");long offset=8;foreach(var prior in document.Assets.TakeWhile(a=>a!=asset))offset+=prior.InstanceDataSize;return AssetStreamProbe.ReadRange(path,null,offset,asset.InstanceDataSize);
    }

    // Reborn: auxiliary arrays have one sentinel and unique source slots; an empty array has no sentinel.
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("W3DContainer auxiliary framing differs.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("W3DContainer duplicate auxiliary slot.");return slots;}
    // Reborn: every word read is bounded independently of a pointer/count stored in the input.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("W3DContainer word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
