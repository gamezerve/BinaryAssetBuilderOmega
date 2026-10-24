using System.Buffers.Binary;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: preserve precompiled legacy DDS payloads; this is not a DXT encoder, mip generator or DX10/DX11 texture converter.
internal static class Ep1DdsTexture
{
    internal sealed record Info(int Width,int Height,int Depth,int Mips,string Format,string Kind,int Faces);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify DDS headers and every declared mip/face/slice byte using the legacy Direct3D layout, not unreliable pitch hints. */
    //-------------------------------------------------------------------------------------------------
    internal static Info Inspect(byte[] bytes,string path)
    {
        if(bytes.Length<128||W(bytes,0)!=0x20534444||W(bytes,4)!=124||W(bytes,76)!=32)throw new InvalidDataException("Invalid/truncated DDS header: "+path);
        uint width=W(bytes,16),height=W(bytes,12),caps=W(bytes,112),rawDepth=W(bytes,24),rawMips=W(bytes,28);bool cube=(caps&0x200)!=0,volume=(caps&0x200000)!=0;
        if(width==0||height==0||width>int.MaxValue||height>int.MaxValue||cube&&volume)throw new InvalidDataException("Invalid DDS dimensions/resource kind: "+path);
        if(cube&&((caps&0xFC00)!=0xFC00||width!=height))throw new InvalidDataException("DDS cube input requires six square faces: "+path);
        uint depth=volume?rawDepth:1;if(depth==0||depth>int.MaxValue)throw new InvalidDataException("Invalid DDS volume depth: "+path);
        int maxMips=1;for(uint n=Math.Max(width,Math.Max(height,depth));n>1;n>>=1)maxMips++;
        uint mips=rawMips==0?1:rawMips;if(mips>maxMips)throw new InvalidDataException("DDS mip count exceeds its dimensions: "+path);
        uint flags=W(bytes,80),bits=W(bytes,88);string format;int block=0,pixel=0;
        if((flags&4)!=0){
            string fourcc=Encoding.ASCII.GetString(bytes,84,4);format=fourcc;
            block=fourcc switch{"DXT1"=>8,"DXT3" or "DXT5"=>16,_=>throw new InvalidDataException("DDS format '"+fourcc+"' is not supported by this legacy texture path (DX10/BC6/BC7 conversion is not implemented): "+path)};
        }else if((flags&0x40)!=0){
            if(bits==32&&W(bytes,92)==0xFF0000&&W(bytes,96)==0xFF00&&W(bytes,100)==0xFF&&W(bytes,104) is 0 or 0xFF000000){format=(flags&1)!=0&&W(bytes,104)==0xFF000000?"A8R8G8B8":"X8R8G8B8";pixel=4;}
            else if(bits==16&&W(bytes,92)==0xF800&&W(bytes,96)==0x7E0&&W(bytes,100)==0x1F&&W(bytes,104)==0){format="R5G6B5";pixel=2;}
            else throw new InvalidDataException("DDS RGB bit masks/format are not supported yet: "+path);
        }else throw new InvalidDataException("DDS palette/luminance/custom formats are not supported yet: "+path);
        long expected=0,aligned=0;uint w=width,h=height,d=depth;int faces=cube?6:1;
        try{checked{for(int i=0;i<mips;i++){
            if(block!=0){long level=Math.Max(1L,(w+3L)/4)*Math.Max(1L,(h+3L)/4)*block*d*faces;expected+=level;aligned+=level;}
            else{long row=(long)w*pixel;expected+=row*h*d*faces;aligned+=((row+3)&~3L)*h*d*faces;}
            w=Math.Max(1,w/2);h=Math.Max(1,h/2);d=Math.Max(1,d/2);
        }}}catch(OverflowException error){throw new InvalidDataException("DDS declared surface dimensions exceed representable payload sizes: "+path,error);}
        if(bytes.Length-128!=expected&&bytes.Length-128!=aligned)throw new InvalidDataException("DDS surface/mip payload length differs: expected "+expected+" bytes, actual "+(bytes.Length-128)+" (truncated or unexpected data): "+path);
        return new((int)width,(int)height,(int)depth,(int)mips,format,cube?"CubeTexture":volume?"VolumeTexture":"StandardTexture",faces);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only explicitly authored conversion requests may conflict; absent raster defaults cannot reinterpret precompiled DDS data. */
    //-------------------------------------------------------------------------------------------------
    internal static void Prepare(XmlElement texture,byte[] bytes,string path,ISet<string> authored)
    {
        var info=Inspect(bytes,path);
        if(authored.Contains("Type")&&texture.GetAttribute("Type")!=info.Kind)throw SdkXmlDiagnostics.At(new InvalidDataException("Texture Type conflicts with DDS header kind "+info.Kind+": "+path),texture.GetAttributeNode("Type"));
        if(authored.Contains("OutputFormat")){
            string requested=texture.GetAttribute("OutputFormat");if(requested!=info.Format&&!(requested=="DXT1A"&&info.Format=="DXT1"))throw SdkXmlDiagnostics.At(new InvalidDataException("DDS OutputFormat conversion from "+info.Format+" to "+requested+" is not implemented; precompile the requested DDS format: "+path),texture.GetAttributeNode("OutputFormat"));
        }
        if(authored.Contains("GenerateMipMaps")){
            bool requested=XmlConvert.ToBoolean(texture.GetAttribute("GenerateMipMaps"));int full=1;for(int n=Math.Max(info.Width,Math.Max(info.Height,info.Depth));n>1;n>>=1)full++;
            if(requested&&info.Mips!=full||!requested&&info.Mips!=1)throw SdkXmlDiagnostics.At(new InvalidDataException("DDS mip generation/removal is not implemented; supply the desired mip chain in the DDS file: "+path),texture.GetAttributeNode("GenerateMipMaps"));
        }
        // Reborn: prepared pipeline-only metadata describes the captured bytes; runtime format/mips remain authoritative in the unchanged DDS header.
        texture.SetAttribute("Type",info.Kind);texture.SetAttribute("OutputFormat",info.Format);texture.SetAttribute("GenerateMipMaps",info.Mips>1?"true":"false");texture.SetAttribute("AllowAutomaticResize","false");
    }

    // Reborn: word reads are bounded even for malformed source headers.
    private static uint W(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("DDS header word out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
