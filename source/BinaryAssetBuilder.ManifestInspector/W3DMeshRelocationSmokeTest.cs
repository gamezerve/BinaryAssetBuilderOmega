using System.Buffers.Binary;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: distinguish an empty end-of-buffer bone palette from corrupt mesh pointers.
internal static class W3DMeshRelocationSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test exact end bounds, nonempty palettes, unrelated pointers and malformed relocation slots. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        int checks=0;
        foreach(var sample in new[]{(0u,36u,64u,true),(0u,36u,63u,true),(0u,36u,0u,true),
            (0u,36u,65u,false),(1u,36u,64u,false),(0u,20u,64u,false),(0u,64u,0u,false)}){
            var bytes=new byte[64];Put(bytes,4,12);Put(bytes,32,sample.Item1);
            if(sample.Item2<=60)Put(bytes,(int)sample.Item2,sample.Item3);
            var relocations=new byte[8];Put(relocations,0,sample.Item2);Put(relocations,4,uint.MaxValue);
            bool accepted=true;
            try{W3DGeometryCompiler.VerifyMeshRelocations(bytes,relocations,"RelocationFixture");}
            catch(InvalidDataException){accepted=false;}
            if(accepted!=sample.Item4)throw new InvalidOperationException("Mesh relocation boundary result differs: "+sample);
            checks++;
        }
        Console.WriteLine("Reborn: mesh relocation proof: PASS ("+checks+" empty-palette, end-bound and corruption controls; no user build or game).");
    }

    // Reborn: construct independent little-endian pointer/count fixtures.
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
}
