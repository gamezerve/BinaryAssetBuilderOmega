namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: guard the shared 24/32-bit terrain raster decoder against alpha, orientation and RLE regressions.
internal static class TextureRasterSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare equivalent RGB/RGBA images and both TGA packet forms without touching SDK art resources. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        byte[] pixels={1,2,3,255,4,5,6,255,7,8,9,255,10,11,12,255};int count=0;
        foreach(int stride in new[]{3,4})foreach(bool top in new[]{false,true})foreach(bool rle in new[]{false,true}){
            var bytes=new byte[18+(rle?1:0)+4*stride];bytes[2]=(byte)(rle?10:2);bytes[12]=2;bytes[14]=2;bytes[16]=(byte)(stride*8);bytes[17]=(byte)((top?0x20:0)|(stride==4?8:0));int at=18;if(rle)bytes[at++]=3;
            for(int p=0;p<4;p++){int source=top?p:(p<2?p+2:p-2);Buffer.BlockCopy(pixels,source*4,bytes,at,stride);at+=stride;}
            if(!Ep1TextureCompiler.ReadTga(bytes,"private raster fixture").Bgra.SequenceEqual(pixels))throw new InvalidDataException("TGA RGB/RGBA orientation or raw packet differs.");count++;
        }
        foreach(int stride in new[]{3,4}){
            var bytes=new byte[19+stride];bytes[2]=10;bytes[12]=2;bytes[14]=2;bytes[16]=(byte)(stride*8);bytes[17]=(byte)(0x20|(stride==4?8:0));bytes[18]=0x83;Buffer.BlockCopy(pixels,0,bytes,19,stride);
            var actual=Ep1TextureCompiler.ReadTga(bytes,"private repeated-pixel fixture").Bgra;
            for(int p=0;p<4;p++)if(!actual.AsSpan(p*4,4).SequenceEqual(pixels.AsSpan(0,4)))throw new InvalidDataException("TGA repeated packet differs.");count++;
        }
        Console.WriteLine($"Terrain raster decoder PASS: {count} RGB/RGBA, orientation and RLE cases; no source art writes.");
    }
}
