using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 PC Texture assets, as written by the official RA3 SDK TextureCompiler (EmbedPCTexture): a 12-byte root
// {0, relocated pointer to +12, byte length} followed by the complete nvDXT DDS file. This first route covers the uncompressed
// profile only: OutputFormat=A8R8G8B8, GenerateMipMaps=false, StandardTexture, from an uncompressed or RLE 32-bit TGA with 8 alpha bits.
// Reborn: DDS inputs bypass raster conversion and preserve their validated DXT/RGB format, mip chain and resource kind.
// TGA encoding still uses the uncompressed profile; this does not add a DXT encoder or mip generator.
internal static class Ep1TextureCompiler
{
    internal const uint TypeHash=0x9F5FF8DA;
    internal const int RootSize=12,HeaderSize=128,MaxDimension=4096;
    internal static readonly uint TypeId=FastHash.GetHashCode("Texture");
    internal sealed record Image(int Width,int Height,byte[] Bgra);
    private static bool qualified;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: D3DX-equivalent TGA read for the admitted profile: BGRA bytes, rows top-down (bottom-left origin is flipped). */
    //-------------------------------------------------------------------------------------------------
    internal static Image ReadTga(byte[] b,string path)
    {
        if(b.Length<18)throw new InvalidDataException("TGA header is truncated: "+path);
        int idLength=b[0],colorMap=b[1],type=b[2],width=U16(b,12),height=U16(b,14),bpp=b[16],descriptor=b[17];
        if(colorMap!=0||type is not (2 or 10))throw new InvalidDataException("Only true-color TGA (type 2 or RLE type 10) is supported: "+path);
        if (bpp != 32 || (descriptor & 0x0F) is not (0 or 8)) throw new InvalidDataException("Only 32-bit TGA with 0 or 8 alpha attribute bits is supported: " + path);
        if ((descriptor&0x10)!=0)throw new InvalidDataException("Right-to-left TGA origin is not supported: "+path);
        if(width<1||height<1||width>MaxDimension||height>MaxDimension)throw new InvalidDataException("TGA dimensions are outside 1.."+MaxDimension+": "+path);
        int pixels=checked(width*height),at=18+idLength;var raw=new byte[pixels*4];
        if(type==2){if(at>b.Length-raw.Length)throw new InvalidDataException("TGA pixel data is truncated: "+path);Buffer.BlockCopy(b,at,raw,0,raw.Length);}
        else{
            for(int p=0;p<pixels;){
                if(at>=b.Length)throw new InvalidDataException("TGA RLE data is truncated: "+path);int packet=b[at++],count=(packet&0x7F)+1;if(p+count>pixels)throw new InvalidDataException("TGA RLE packet overruns the image: "+path);
                if((packet&0x80)!=0){if(at>b.Length-4)throw new InvalidDataException("TGA RLE data is truncated: "+path);for(int i=0;i<count;i++)Buffer.BlockCopy(b,at,raw,(p+i)*4,4);at+=4;}
                else{if(at>b.Length-count*4)throw new InvalidDataException("TGA RLE data is truncated: "+path);Buffer.BlockCopy(b,at,raw,p*4,count*4);at+=count*4;}
                p+=count;
            }
        }
        if((descriptor&0x20)!=0)return new(width,height,raw);
        var flipped=new byte[raw.Length];int row=width*4;for(int y=0;y<height;y++)Buffer.BlockCopy(raw,(height-1-y)*row,flipped,y*row,row);return new(width,height,flipped);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: nvDXT A8R8G8B8/no-mip DDS header exactly as on the 153 stock EP1 textures of this profile. */
    //-------------------------------------------------------------------------------------------------
    internal static byte[] Header(int width,int height)
    {
        var h=new byte[HeaderSize];Put(h,0,0x20534444);Put(h,4,124);Put(h,8,0x81007);Put(h,12,(uint)height);Put(h,16,(uint)width);Put(h,20,checked((uint)(width*height*4)));
        Put(h,76,32);Put(h,80,0x41);Put(h,88,32);Put(h,92,0x00FF0000);Put(h,96,0x0000FF00);Put(h,100,0x000000FF);Put(h,104,0xFF000000);Put(h,108,0x1000);return h;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only the reviewed profile from the effective (schema-defaulted) Texture source. */
    //-------------------------------------------------------------------------------------------------
    internal static void Admit(XmlElement texture)
    {
        string Value(string name,string fallback)=>texture.HasAttribute(name)?texture.GetAttribute(name).Trim():fallback;
        string format=Value("OutputFormat","DXT1"),mips=Value("GenerateMipMaps","true"),kind=Value("Type","StandardTexture");
        if(kind!="StandardTexture")throw new InvalidDataException("Texture Type '"+kind+"' is not supported yet; only StandardTexture.");
        if(format!="A8R8G8B8")throw new InvalidDataException("Texture OutputFormat '"+format+"' is not supported yet; only A8R8G8B8 (uncompressed) is compiled. DXT compression needs the nvDXT encoder.");
        if(mips is not ("false" or "0"))throw new InvalidDataException("Texture GenerateMipMaps must be false for the uncompressed route (mipmap generation needs the nvDXT filter).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: root + DDS for one authored texture; the only relocation is the data pointer at +4, there are no imports. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Compile(XmlElement texture,byte[] tga,string path)
    {
        // Reborn: DDS is already compiled; validate and embed its complete unchanged header/surface/mip bytes in the same EP1 root.
        if(path.EndsWith(".dds",StringComparison.OrdinalIgnoreCase)){
            var info=Ep1DdsTexture.Inspect(tga,path);if(texture.GetAttribute("Type")!=info.Kind||texture.GetAttribute("OutputFormat")!=info.Format)throw new InvalidDataException("Prepared DDS metadata differs from its captured payload: "+path);
            var embedded=new byte[checked(RootSize+tga.Length)];Put(embedded,4,RootSize);Put(embedded,8,(uint)tga.Length);tga.CopyTo(embedded,RootSize);return new Chunk{InstanceBuffer=embedded,RelocationBuffer=Pack(4),ImportsBuffer=Array.Empty<byte>()};
        }
        Admit(texture);var image=ReadTga(tga,path);int length=checked(HeaderSize+image.Bgra.Length);
        var bytes=new byte[checked(RootSize+length)];Put(bytes,0,0);Put(bytes,4,RootSize);Put(bytes,8,(uint)length);Header(image.Width,image.Height).CopyTo(bytes,RootSize);image.Bgra.CopyTo(bytes,RootSize+HeaderSize);
        return new Chunk{InstanceBuffer=bytes,RelocationBuffer=Pack(4),ImportsBuffer=Array.Empty<byte>()};
    }

    // Reborn: the reported texture size accompanies the EA-style compile line.
    internal static string Describe(Chunk chunk){var dds=Ep1DdsTexture.Inspect(chunk.InstanceBuffer.AsSpan(RootSize).ToArray(),"compiled Texture");return dds.Width+"x"+dds.Height+" "+dds.Format+", "+dds.Mips+" mip level(s), "+dds.Kind;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the writer on original EP1 data before any authored texture is emitted:
        1) a stock A8R8G8B8/no-mip texture has exactly this root, relocation and DDS header;
        2) the SDK sample TGAs decode to the same visible pixels the official pipeline packed into the stock atlases. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest)
    {
        if(qualified)return;
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
        if(document.Validate().Count!=0||!document.Header.IsLinked||document.Header.ContainerPrefixSize!=4)throw new InvalidDataException("Valid linked EP1 static stream required for Texture qualification.");
        var cloud=Stock(manifest,document,"Texture:DecalCloudSegment");
        if(U32(cloud.Bytes,0)!=0||U32(cloud.Bytes,4)!=RootSize||U32(cloud.Bytes,8)!=cloud.Bytes.Length-RootSize||!cloud.Relocations.SequenceEqual(new uint[]{4})||cloud.Imports.Length!=0)throw new InvalidDataException("Stock Texture root/relocation layout differs.");
        int width=(int)U32(cloud.Bytes,RootSize+16),height=(int)U32(cloud.Bytes,RootSize+12);
        if(!cloud.Bytes.AsSpan(RootSize,HeaderSize).SequenceEqual(Header(width,height))||cloud.Bytes.Length!=RootSize+HeaderSize+width*height*4)throw new InvalidDataException("Stock A8R8G8B8 DDS header differs from the generated header.");
        string images=SdkSampleImages();int pixels=0;
        foreach(var (atlas,file,x0,y0,packed) in new[]{("Texture:PackedImagesPortraits_004","Portrait_AlliedHarbingerGunship.tga",104,157,"PackedTextureImage:Portrait_AlliedHarbingerGunship"),("Texture:PackedImages_004","AUA_Harbinger_Cannon.tga",85,1,"PackedTextureImage:AUA_Harbinger_Cannon")}){
            var stock=Stock(manifest,document,atlas);int aw=(int)U32(stock.Bytes,RootSize+16),ah=(int)U32(stock.Bytes,RootSize+12);
            if(!stock.Bytes.AsSpan(RootSize,HeaderSize).SequenceEqual(Header(aw,ah)))throw new InvalidDataException("Stock atlas header differs: "+atlas);
            var entry=document.Assets.Single(a=>a.Name==packed);var record=Stock(manifest,document,packed).Bytes;
            if(U32(record,16)!=x0||U32(record,20)!=y0||entry.References.Count!=1||entry.References[0].InstanceId!=document.Assets.Single(a=>a.Name==atlas).InstanceId)throw new InvalidDataException("Stock packed-image witness placement differs: "+packed);
            string path=Path.Combine(images,file);var image=ReadTga(SdkEnvironmentPreflight.Read(path,16*1048576),path);
            if(U32(record,8)!=image.Width||U32(record,12)!=image.Height)throw new InvalidDataException("SDK sample TGA dimensions differ from the stock packed image: "+file);
            for(int y=0;y<image.Height;y++)for(int x=0;x<image.Width;x++){
                var source=image.Bgra.AsSpan((y*image.Width+x)*4,4);var target=stock.Bytes.AsSpan(RootSize+HeaderSize+((y0+y)*aw+x0+x)*4,4);
                // Reborn: the stock packer bleeds colour into fully transparent texels; every visible texel must match exactly.
                if(source[3]==0?target[3]!=0:!source.SequenceEqual(target))throw new InvalidDataException("SDK sample TGA pixel differs from the stock atlas: "+file+" ("+x+","+y+")");
                pixels++;
            }
        }
        qualified=true;Console.WriteLine("Reborn: EP1 Texture stock qualification: PASS (A8R8G8B8 root/DDS header on Texture:DecalCloudSegment; "+pixels+" SDK sample texels match stock atlases).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the official SDK sample images ship beside the reference compiler in this repository. */
    //-------------------------------------------------------------------------------------------------
    private static string SdkSampleImages()
    {
        for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent){string path=Path.Combine(folder.FullName,"Working RA3 Compiler for Reference","Art","images");if(File.Exists(Path.Combine(path,"Portrait_AlliedHarbingerGunship.tga")))return path;}
        throw new InvalidDataException("Texture qualification requires the official SDK sample images (Working RA3 Compiler for Reference/Art/images).");
    }

    private sealed record Slice(byte[] Bytes,uint[] Relocations,uint[] Imports);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded read of one stock asset's bin/relo/imp chunks, bound to the linked stream checksum. */
    //-------------------------------------------------------------------------------------------------
    private static Slice Stock(string manifest,ManifestDocument document,string name)
    {
        var asset=document.Assets.Single(a=>a.Name==name);if(asset.InstanceDataSize>4*1048576)throw new InvalidDataException("Stock witness is too large: "+name);
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(U32(header,0)!=0xBABB0000||U32(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("Static stream checksum differs.");
        long offset=8,relo=8,imp=8;foreach(var entry in document.Assets){if(ReferenceEquals(entry,asset))break;offset+=entry.InstanceDataSize;relo+=entry.RelocationDataSize;imp+=entry.ImportsDataSize;}
        var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);
        uint[] Slots(string extension,long at,int size){if(size==0)return Array.Empty<uint>();var b=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,extension),null,at,size);if(size%4!=0||U32(b,size-4)!=uint.MaxValue)throw new InvalidDataException("Stock auxiliary framing differs: "+name);return Enumerable.Range(0,size/4-1).Select(i=>U32(b,i*4)).ToArray();}
        return new(bytes,Slots(".relo",relo,asset.RelocationDataSize),Slots(".imp",imp,asset.ImportsDataSize));
    }

    // Reborn: content-addressed resource names let the compile stage re-prove the exact bytes captured during preparation.
    internal static string Sha256(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes));

    private static byte[] Pack(params uint[] slots){var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    private static int U16(byte[] b,int at)=>BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(at,2));
    private static uint U32(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("Texture word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
