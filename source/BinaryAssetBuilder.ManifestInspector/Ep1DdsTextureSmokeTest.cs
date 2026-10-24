using System.Buffers.Binary;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: DDS tests use independent fixed-size payload cases and never invoke an encoder or touch user mod sources.
internal static class Ep1DdsTextureSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove legacy formats/mips/kinds, exact byte preservation and genuine malformed/unsupported/conversion refusals. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        int accepted=0,rejected=0;
        foreach(var sample in new[]{("DXT1",4,4,3,24,0u,1),("DXT3",4,4,1,16,0u,1),("DXT5",16,8,5,208,0u,1),("DXT1",4,4,1,48,0xFE00u,1),("DXT5",4,4,3,112,0x200000u,4)}){
            var bytes=Fixture(sample.Item1,sample.Item2,sample.Item3,sample.Item4,sample.Item5,sample.Item6,sample.Item7);Accept(bytes);
        }
        var rgb=new byte[144];Ep1TextureCompiler.Header(2,2).CopyTo(rgb,0);Accept(rgb);var xrgb=rgb.ToArray();Put(xrgb,80,0x40);Put(xrgb,104,0);Accept(xrgb);
        var rgb565=new byte[136];Ep1TextureCompiler.Header(2,2).CopyTo(rgb565,0);Put(rgb565,80,0x40);Put(rgb565,88,16);Put(rgb565,92,0xF800);Put(rgb565,96,0x7E0);Put(rgb565,100,0x1F);Put(rgb565,104,0);Accept(rgb565);
        var valid=Fixture("DXT1",4,4,3,24);Refuse(()=>Ep1DdsTexture.Inspect(valid[..^1],"truncated.dds"));Refuse(()=>Ep1DdsTexture.Inspect(valid[..64],"header.dds"));
        foreach(var edit in new[]{(0,0u),(4,120u),(76,28u),(16,0u),(28,4u),(84,0x30315844u)}){var bad=valid.ToArray();Put(bad,edit.Item1,edit.Item2);Refuse(()=>Ep1DdsTexture.Inspect(bad,"bad.dds"));}
        foreach(string attributes in new[]{"OutputFormat='DXT5'","GenerateMipMaps='false'","Type='CubeTexture'"}){var source=Texture(attributes);Refuse(()=>Ep1DdsTexture.Prepare(source,valid,"conversion.dds",source.Attributes.OfType<XmlAttribute>().Select(a=>a.Name).ToHashSet()));}
        var noMips=Fixture("DXT1",4,4,1,8);var generate=Texture("GenerateMipMaps='true'");Refuse(()=>Ep1DdsTexture.Prepare(generate,noMips,"mips.dds",new HashSet<string>{"GenerateMipMaps"}));
        Console.WriteLine("Reborn: DDS texture self-test: PASS ("+accepted+" format/mip/kind cases, "+rejected+" invalid/conversion controls, exact DDS embedding).");

        // Reborn: compare the complete source bytes inside the EP1 root, plus its only relocation and absence of imports.
        void Accept(byte[] bytes){var texture=Texture("");var saved=bytes.ToArray();Ep1DdsTexture.Prepare(texture,bytes,"fixture.dds",new HashSet<string>());var chunk=Ep1TextureCompiler.Compile(texture,bytes,"fixture.dds");if(!bytes.SequenceEqual(saved)||!chunk.InstanceBuffer.AsSpan(12).SequenceEqual(saved)||!chunk.RelocationBuffer.SequenceEqual(new byte[]{4,0,0,0,255,255,255,255})||chunk.ImportsBuffer.Length!=0)throw new InvalidDataException("DDS payload/root/auxiliary preservation differs.");accepted++;}
        // Reborn: negative controls must be clear input/capability refusals, not unrelated runtime exceptions.
        void Refuse(Action action){try{action();}catch(InvalidDataException){rejected++;return;}throw new InvalidDataException("DDS negative control accepted.");}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete emitted texture roots/DDS bytes against original EP1 data, without dumping whole BIN streams. */
    //-------------------------------------------------------------------------------------------------
    internal static void CompareStock(string stockPath,string builtPath)
    {
        var stock=ManifestReader.Read(SdkEnvironmentPreflight.Read(stockPath,64*1048576));var built=ManifestReader.Read(SdkEnvironmentPreflight.Read(builtPath,64*1048576));int count=0;
        foreach(var asset in built.Assets.Where(a=>a.TypeName=="Texture")){
            var original=stock.Assets.SingleOrDefault(a=>a.TypeId==asset.TypeId&&a.InstanceId==asset.InstanceId);if(original==null)continue;
            var a=W3DContainerCompiler.ReadStock(stockPath,stock,original);var b=W3DContainerCompiler.ReadStock(builtPath,built,asset);if(asset.TypeHash!=original.TypeHash||asset.Tokenized!=0||!a.SequenceEqual(b))throw new InvalidDataException("DDS stock root/payload differs: "+asset.Name);count++;
        }
        if(count==0)throw new InvalidDataException("No stock DDS textures compared.");Console.WriteLine("Reborn: DDS stock comparison: PASS ("+count+" complete Texture roots and unchanged DDS mip payloads).");
    }

    // Reborn: fixed fixture byte counts are independent of the parser's mip-size implementation.
    internal static byte[] Fixture(string format,int width,int height,int mips,int payload,uint caps=0,int depth=1){var b=new byte[128+payload];Put(b,0,0x20534444);Put(b,4,124);Put(b,8,0x21007);Put(b,12,(uint)height);Put(b,16,(uint)width);Put(b,24,(uint)depth);Put(b,28,(uint)mips);Put(b,76,32);Put(b,80,4);Encoding.ASCII.GetBytes(format).CopyTo(b,84);Put(b,108,0x401008);Put(b,112,caps);return b;}
    // Reborn: tests begin with omitted raster conversion options, as in the user's DDS declarations.
    private static XmlElement Texture(string attributes){var xml=new XmlDocument{XmlResolver=null};xml.LoadXml("<Texture xmlns='uri:ea.com:eala:asset' id='DdsProbe' File='fixture.dds' "+attributes+"/>");return xml.DocumentElement!;}
    // Reborn: only owned synthetic fixtures receive header patches.
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
