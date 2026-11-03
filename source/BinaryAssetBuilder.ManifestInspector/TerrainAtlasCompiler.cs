using System.Buffers.Binary;
using System.Xml;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: emit the witnessed EP1 28-byte terrain atlas root, 12-byte tile rectangles and embedded DDS textures from authored raster resources.
internal static class TerrainAtlasCompiler
{
    internal const uint TypeId=0x5F74536D,TypeHash=0x18E913EA;
    private const int Border=16,Mips=5;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently read atlas headers, tile bounds, texture formats and relocation ownership before the generated native data enters a map stream. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement source,Chunk chunk,Func<string,Ep1TextureCompiler.Image> load)
    {
        var bytes=chunk.InstanceBuffer;var tiles=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Tile").ToArray();
        if(bytes.Length<28||Word(bytes,0)!=0||Word(bytes,4)!=tiles.Length||chunk.ImportsBuffer.Length!=0)throw new InvalidDataException("Terrain atlas root/import contract differs.");
        int table=checked((int)Word(bytes,8));if(tiles.Length==0?table!=0:table!=bytes.Length-tiles.Length*12)throw new InvalidDataException("Terrain atlas tile table storage differs.");
        var rectangles=new List<(int X,int Y,int R,int B)>();int size=source.HasAttribute("AtlasSize")?XmlConvert.ToInt32(source.GetAttribute("AtlasSize")):2048;
        for(int i=0;i<tiles.Length;i++){
            int at=table+i*12,x=BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(at+4)),y=BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(at+6)),r=BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(at+8)),b=BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(at+10));
            var image=load(tiles[i].GetAttribute("BaseTexture"));
            if(Word(bytes,at)!=XmlConvert.ToUInt32(tiles[i].GetAttribute("TextureID"))||r-x!=image.Width||b-y!=image.Height||x<Border||y<Border||r+Border>size||b+Border>size)throw new InvalidDataException("Terrain atlas source rectangle differs.");
            if(rectangles.Any(v=>x-Border<v.R+Border&&r+Border>v.X-Border&&y-Border<v.B+Border&&b+Border>v.Y-Border))throw new InvalidDataException("Terrain atlas tiles overlap.");rectangles.Add((x,y,r,b));
        }
        int expected=28;
        foreach(int at in new[]{12,20}){
            int offset=checked((int)Word(bytes,at)),length=checked((int)Word(bytes,at+4));if(offset!=expected||length<128||offset>bytes.Length-length)throw new InvalidDataException("Terrain atlas DDS allocation differs.");
            var dds=Ep1DdsTexture.Inspect(bytes.AsSpan(offset,length).ToArray(),"authored terrain atlas");
            if(dds.Width!=size||dds.Height!=size||dds.Mips!=Mips||dds.Kind!="StandardTexture"||dds.Format!=(at==12?"A8R8G8B8":"X1R5G5B5"))throw new InvalidDataException("Terrain atlas DDS format/dimensions differ.");expected+=length;
        }
        if(expected!=(tiles.Length==0?bytes.Length:table))throw new InvalidDataException("Terrain atlas unowned payload bytes.");
        var slots=tiles.Length==0?new uint[]{12,20,uint.MaxValue}:new uint[]{8,12,20,uint.MaxValue};
        if(chunk.RelocationBuffer.Length!=slots.Length*4||!slots.Select((v,i)=>Word(chunk.RelocationBuffer,i*4)==v).All(v=>v))throw new InvalidDataException("Terrain atlas relocation ownership differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original atlas root/rectangle geometry using labelled synthetic rasters; retail pixels are not substituted for missing source textures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Probe(XmlElement source,string manifest,string output)
    {
        var document=ManifestReader.Read(File.ReadAllBytes(manifest));var asset=document.Assets.Single(a=>a.TypeId==TypeId);
        if(asset.TypeHash!=TypeHash||asset.Tokenized!=0||asset.References.Count!=0)throw new InvalidDataException("EP1 terrain atlas fingerprint differs.");
        var stock=W3DContainerCompiler.ReadStock(manifest,document,asset);int count=checked((int)Word(stock,4)),table=checked((int)Word(stock,8));
        var nodes=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Tile").ToArray();if(count!=nodes.Length||table!=stock.Length-count*12)throw new InvalidDataException("Retail/source atlas table count differs.");
        var dimensions=new Dictionary<string,(int Width,int Height)>(StringComparer.OrdinalIgnoreCase);
        for(int i=0;i<count;i++){
            int at=table+i*12;if(Word(stock,at)!=XmlConvert.ToUInt32(nodes[i].GetAttribute("TextureID")))throw new InvalidDataException("Retail/source terrain ID order differs.");
            int width=BinaryPrimitives.ReadUInt16LittleEndian(stock.AsSpan(at+8))-BinaryPrimitives.ReadUInt16LittleEndian(stock.AsSpan(at+4));
            int height=BinaryPrimitives.ReadUInt16LittleEndian(stock.AsSpan(at+10))-BinaryPrimitives.ReadUInt16LittleEndian(stock.AsSpan(at+6));
            foreach(string field in new[]{"BaseTexture","NormalTexture"})dimensions[nodes[i].GetAttribute(field)]=(width,height);
        }
        var native=Compile(source,path=>{var d=dimensions[path];var pixels=new byte[checked(d.Width*d.Height*4)];for(int i=0;i<pixels.Length;i+=4){pixels[i]=255;pixels[i+1]=128;pixels[i+2]=128;pixels[i+3]=255;}return new(d.Width,d.Height,pixels);});
        int generatedTable=checked((int)Word(native.InstanceBuffer,8));
        if(Word(native.InstanceBuffer,4)!=count||!stock.AsSpan(table,count*12).SequenceEqual(native.InstanceBuffer.AsSpan(generatedTable,count*12)))throw new InvalidDataException("Terrain atlas source/native rectangle geometry differs.");
        foreach(int at in new[]{12,20}){
            int offset=checked((int)Word(native.InstanceBuffer,at)),length=checked((int)Word(native.InstanceBuffer,at+4));
            var info=Ep1DdsTexture.Inspect(native.InstanceBuffer.AsSpan(offset,length).ToArray(),"synthetic terrain atlas");
            if(info.Mips!=Mips||info.Kind!="StandardTexture")throw new InvalidDataException("Terrain atlas DDS mip/resource contract differs.");
        }
        File.WriteAllBytes(Path.Combine(output,"synthetic-atlas.bin"),native.InstanceBuffer);
        Console.WriteLine($"Terrain atlas geometry/DDS PASS: {count} source tiles; retail rectangles; five mip levels; synthetic pixels only, no authored-texture or game validation.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve source tile IDs/order, pack complete tiles with mip-safe borders and encode lossless color plus retail-format normal data. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Compile(XmlElement source,Func<string,Ep1TextureCompiler.Image> load)
    {
        int size=source.HasAttribute("AtlasSize")?XmlConvert.ToInt32(source.GetAttribute("AtlasSize")):2048;
        if(size<64||size>4096||(size&(size-1))!=0)throw new InvalidDataException("Terrain atlas size must be a power of two from 64 through 4096.");
        var color=new byte[checked(size*size*4)];var normal=new byte[color.Length];var tiles=new List<(uint Id,int X,int Y,int W,int H)>();var ids=new HashSet<uint>();
        int x=0,y=0,row=0;
        foreach(var tile in source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Tile")){
            uint id=XmlConvert.ToUInt32(tile.GetAttribute("TextureID"));if(!ids.Add(id))throw new InvalidDataException("Duplicate terrain TextureID: "+id);
            var baseImage=load(tile.GetAttribute("BaseTexture"));var normalImage=load(tile.GetAttribute("NormalTexture"));
            if(baseImage.Width!=normalImage.Width||baseImage.Height!=normalImage.Height)throw new InvalidDataException("Terrain base/normal dimensions differ for TextureID "+id);
            int width=baseImage.Width,height=baseImage.Height;
            if(width<1||height<1||width+2*Border>size||height+2*Border>size)throw new InvalidDataException("Terrain tile does not fit its atlas: "+id);
            if(x+width+2*Border>size){x=0;y+=row;row=0;}
            if(y+height+2*Border>size)throw new InvalidDataException("Terrain tiles exceed AtlasSize; increase the atlas size.");
            // Reborn: Cell lists select used parts of a tile; retaining its complete raster preserves every cell without dropping authored terrain data.
            foreach(var cell in tile.ChildNodes.OfType<XmlElement>())if(cell.LocalName!="Cell"||XmlConvert.ToInt32(cell.GetAttribute("IndexX"))*32>=width||XmlConvert.ToInt32(cell.GetAttribute("IndexY"))*32>=height)throw new InvalidDataException("Terrain Cell is outside its source tile: "+id);
            Blit(baseImage,color,size,x,y);Blit(normalImage,normal,size,x,y);
            tiles.Add((id,x+Border,y+Border,width,height));x+=width+2*Border;row=Math.Max(row,height+2*Border);
        }
        byte[] baseDds=Dds(color,size,false),normalDds=Dds(normal,size,true);
        int normalAt=checked(28+baseDds.Length),tilesAt=checked(normalAt+normalDds.Length);var bytes=new byte[checked(tilesAt+tiles.Count*12)];
        Put(bytes,4,(uint)tiles.Count);Put(bytes,8,tiles.Count==0?0u:(uint)tilesAt);Put(bytes,12,28);Put(bytes,16,(uint)baseDds.Length);Put(bytes,20,(uint)normalAt);Put(bytes,24,(uint)normalDds.Length);
        baseDds.CopyTo(bytes,28);normalDds.CopyTo(bytes,normalAt);
        for(int i=0;i<tiles.Count;i++){var t=tiles[i];int at=tilesAt+i*12;Put(bytes,at,t.Id);Short(bytes,at+4,t.X);Short(bytes,at+6,t.Y);Short(bytes,at+8,t.X+t.W);Short(bytes,at+10,t.Y+t.H);}
        var relo=new byte[tiles.Count==0?12:16];int slot=0;if(tiles.Count!=0){Put(relo,slot,8);slot+=4;}Put(relo,slot,12);Put(relo,slot+4,20);Put(relo,slot+8,uint.MaxValue);
        return new Chunk{InstanceBuffer=bytes,RelocationBuffer=relo,ImportsBuffer=Array.Empty<byte>()};
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: wrap the source raster into the sixteen-pixel border so each terrain tile repeats seamlessly through the five mip levels. */
    //-------------------------------------------------------------------------------------------------
    private static void Blit(Ep1TextureCompiler.Image image,byte[] target,int size,int left,int top)
    {
        if(image.Bgra.Length!=checked(image.Width*image.Height*4))throw new InvalidDataException("Terrain raster byte count differs.");
        for(int y=-Border;y<image.Height+Border;y++)for(int x=-Border;x<image.Width+Border;x++){
            int sx=(x%image.Width+image.Width)%image.Width,sy=(y%image.Height+image.Height)%image.Height;
            Buffer.BlockCopy(image.Bgra,(sy*image.Width+sx)*4,target,((top+y+Border)*size+left+x+Border)*4,4);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit standard PC DDS mip chains; color stays lossless BGRA and normals use the X1R5G5B5 format witnessed in the EP1 atlas. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Dds(byte[] image,int size,bool normal)
    {
        using var memory=new MemoryStream();var header=Ep1TextureCompiler.Header(size,size);
        Put(header,8,0x2100F);Put(header,20,(uint)(size*(normal?2:4)));Put(header,28,Mips);Put(header,108,0x401008);
        if(normal){Put(header,80,0x40);Put(header,88,16);Put(header,92,0x7C00);Put(header,96,0x3E0);Put(header,100,0x1F);Put(header,104,0);}
        memory.Write(header);int width=size;byte[] pixels=image;
        for(int level=0;level<Mips;level++){
            if(normal){var packed=new byte[pixels.Length/2];for(int p=0;p<pixels.Length/4;p++)BinaryPrimitives.WriteUInt16LittleEndian(packed.AsSpan(p*2,2),(ushort)(((pixels[p*4+2]>>3)<<10)|((pixels[p*4+1]>>3)<<5)|(pixels[p*4]>>3)));memory.Write(packed);}
            else memory.Write(pixels);
            if(level==Mips-1)break;
            int nextWidth=width/2;var next=new byte[checked(nextWidth*nextWidth*4)];
            for(int y=0;y<nextWidth;y++)for(int x=0;x<nextWidth;x++)for(int c=0;c<4;c++){
                int at=((y*2)*width+x*2)*4+c;next[(y*nextWidth+x)*4+c]=(byte)((pixels[at]+pixels[at+4]+pixels[at+width*4]+pixels[at+width*4+4]+2)/4);
            }
            pixels=next;width=nextWidth;
        }
        return memory.ToArray();
    }
    // Reborn: all native root/table words use explicit PC little-endian encoding.
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
    // Reborn: terrain rectangle coordinates use the original unsigned 16-bit atlas ABI.
    private static void Short(byte[] b,int at,int value)=>BinaryPrimitives.WriteUInt16LittleEndian(b.AsSpan(at,2),checked((ushort)value));
    // Reborn: read only bounded little-endian root and table fields during independent retail geometry comparison.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
