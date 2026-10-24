using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 PackedTextureImage is an untokenized 36-byte XML asset (official RA3 SDK XmlCompiler marshal): Base@0, Texture import @4,
// Dimensions@8, Coords@16, TextureDimensions@24, Rotated@32. The existing marshaler is proved against stock records before authored use.
internal static class PackedTextureImageCompiler
{
    private static bool qualified;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the writer and schema/native contract on the stock records whose SDK sample sources exist (same values as stock). */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(qualified)return;
        if(System.Runtime.InteropServices.Marshal.SizeOf<PackedTextureImage>()!=36)throw new InvalidDataException("PackedTextureImage ABI size differs.");
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
        if(document.Validate().Count!=0||!document.Header.IsLinked||document.Header.ContainerPrefixSize!=4)throw new InvalidDataException("Valid linked EP1 static stream required for PackedTextureImage qualification.");
        // Reborn: dimensions come from the official SDK sample TGAs (Texture qualification proves their pixels sit at these stock coordinates).
        foreach(var (id,texture,w,h,x,y) in new[]{("Portrait_AlliedHarbingerGunship","PackedImagesPortraits_004",101,76,104,157),("AUA_Harbinger_Cannon","PackedImages_004",82,82,85,1),("AUA_Harbinger_Chaingun","PackedImages_004",82,82,169,1)}){
            var asset=document.Assets.Single(a=>a.Name=="PackedTextureImage:"+id);
            if(asset.InstanceDataSize!=36||asset.RelocationDataSize!=0||asset.ImportsDataSize!=8||asset.Tokenized!=0)throw new InvalidDataException("Stock PackedTextureImage fingerprint differs: "+id);
            var bytes=Range(manifest,document,asset);
            var source=Xml("<PackedTextureImage xmlns='uri:ea.com:eala:asset' id='"+id+"' Texture='"+texture+"' Rotated='false'><Dimensions x='"+w+"' y='"+h+"'/><Coords x='"+x+"' y='"+y+"'/><TextureDimensions x='256' y='256'/></PackedTextureImage>");
            SchemaNativeContract.VerifyStock(ProofSource(source,schemas,catalog).DocumentElement!,typeof(PackedTextureImage),0,bytes,0,asset.References.ToArray());
        }
        qualified=true;Console.WriteLine("Reborn: EP1 PackedTextureImage stock qualification: PASS (3 SDK sample images; untokenized 36-byte native).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the stream header owns the asset ID; the native record has no XML id. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run the existing marshaler on the Core-normalized node; Core owns the Texture reference occurrence index. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=Xml(source.OuterXml);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);PackedTextureImage* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(PackedTextureImage),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("PackedTextureImage native allocation failed.");return chunk;
    }

    private static byte[] Range(string manifest,ManifestDocument document,ManifestAsset asset)
    {
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(Word(header,0)!=0xBABB0000||Word(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("Static stream checksum differs.");
        long offset=8;foreach(var entry in document.Assets){if(ReferenceEquals(entry,asset))break;offset+=entry.InstanceDataSize;}return AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);
    }

    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
