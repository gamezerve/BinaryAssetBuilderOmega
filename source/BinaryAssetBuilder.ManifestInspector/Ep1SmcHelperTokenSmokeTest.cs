using System.Buffers.Binary;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: generate and independently audit the required helper's inherited metadata, not only its previously ported native name/layout.
internal static class Ep1SmcHelperTokenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare both selected stock helper prefixes and actual legacy-compatible native dispatch, then test malformed input/output contracts. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        XmlSchemaSet? schemas=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>schemas=s);if(schemas==null)throw new InvalidDataException("Reviewed helper schema required.");
        const string source="<ObjectSMCHelper id='ObjectSMCHelper'/>";Ep1BasicModuleSmokeTest.Validate(schemas,source,false);
        var compiled=Ep1BasicModuleSmokeTest.Compile(source,"ObjectSMCHelperModuleData",false);var native=new Chunk {InstanceBuffer=compiled.Chunk.InstanceBuffer.AsSpan(compiled.Native,8).ToArray(),RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};var output=Ep1SmcHelperTokenizer.Tokenize(native);Verify(output,native);
        foreach(string name in new[]{"GameObject:AlliedConstructionYard","GameObject:SovietAntiInfantryInfantry"})
        {
            var c=GameObjectStockReview.Capture(manifest,name);var module=GameObjectModuleReview.Inspect(manifest,name).Containers.SelectMany(g=>g.Modules).Single(m=>m.Type=="ObjectSMCHelperModuleData");
            if(!native.InstanceBuffer.SequenceEqual(c.Bytes.AsSpan(module.Native,8).ToArray()))throw new InvalidDataException("Complete SMC native payload differs.");
            var expected=c.Bytes.AsSpan(module.Wrapper,168).ToArray();for(int at=8;at<168;at+=12){uint target=Word(expected,at);if(target!=0)Put(expected,at,target>=(uint)module.Native?target-(uint)module.Native+168:target-(uint)module.Wrapper);}
            if(!expected.SequenceEqual(output.InstanceBuffer.AsSpan(0,168).ToArray()))throw new InvalidDataException("Generated SMC inheritance prefix differs from stock.");
        }
        // Reborn: smallest no-Body/no-Armor helper projection proves the new eight-byte native owner boundary, not just a three-module special case.
        var composed=Ep1GameObjectDestructionProjection.Compose(compiled.Chunk);var root=GameObjectStockReview.Tokens(composed.InstanceBuffer,0);
        if(root.Count!=87 || compiled.Chunk.InstanceBuffer.Length!=620)throw new InvalidDataException("Single-helper minimal projection contract differs.");
        int failures=0;foreach(int length in new[]{0,7,9}){var bad=new Chunk {InstanceBuffer=new byte[length],RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};Refuse(()=>Ep1SmcHelperTokenizer.Tokenize(bad));failures++;}
        foreach(bool imports in new[]{false,true}){var bad=Clone(native);if(imports)bad.ImportsBuffer=new byte[4];else bad.RelocationBuffer=new byte[4];Refuse(()=>Ep1SmcHelperTokenizer.Tokenize(bad));failures++;}
        var wrong=Clone(native);Put(wrong.InstanceBuffer,0,0);Refuse(()=>Ep1SmcHelperTokenizer.Tokenize(wrong));failures++;
        int metadataFailures=0;foreach(int at in new[]{0,4,8,16,20,24,40,44,48,64,68,72,88,92,96,112,116,120,136,140,148,152,156,168,172}){var bad=Clone(output);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,native));metadataFailures++;}
        Console.WriteLine($"EP1 SMC helper tokens: PASS (8-byte native/168-byte generated metadata, two complete selected stock matches, real native dispatch, minimal 620-byte object projection, {failures} input and {metadataFailures} output refusals; no complete object/publication/game).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify every inherited row/terminator, terminal identity and preserved native byte before trusting generated output. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native)
    {
        var b=output.InstanceBuffer;if(b.Length!=176 || Word(b,0)!=2 || Word(b,4)!=0x7F72F242 || Word(b,8)!=12 || output.RelocationBuffer.Length!=0 || output.ImportsBuffer.Length!=0)throw new InvalidDataException("SMC output wrapper differs.");
        string[] bases={"ObjectHelperModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"};for(int i=0;i<bases.Length;i++){int at=12+i*24;if(Word(b,at)!=1 || Word(b,at+4)!=FastHash.GetHashCode(bases[i]) || Word(b,at+8)!=at+24 || Word(b,at+12)!=0 || Word(b,at+16)!=0 || Word(b,at+20)!=0)throw new InvalidDataException("SMC base marker/terminator differs.");}
        var fields=GameObjectStockReview.Tokens(b,132);if(fields.Count!=2 || fields[FastHash.GetHashCode("TypeId")].Type!=0x23E650B8 || fields[FastHash.GetHashCode("TypeId")].Offset!=168 || fields[FastHash.GetHashCode("id")].Type!=0xBDF07965 || fields[FastHash.GetHashCode("id")].Offset!=172 || !b.AsSpan(168).SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("SMC native identity output differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative controls do not mutate live compiler data. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid native or metadata graphs must fail explicitly. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed SMC fragment admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all diagnostic reads are bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutations and independent coordinate rewrites use owned buffers only. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint v)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),v);
}
