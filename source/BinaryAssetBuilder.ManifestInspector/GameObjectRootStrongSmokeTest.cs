using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: diagnostic normalized indices prove root/module import ownership, not named dependency resolution or target payload support.
internal static class GameObjectRootStrongSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: audit three root descriptors against two stock roots, then cover all presence combinations and detached ownership faults. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlSchemaSet schemas,string manifest)
    {
        foreach(string name in new[] {"GameObject:AlliedConstructionYard","GameObject:SovietAntiInfantryInfantry"})
        {
            var c=GameObjectStockReview.Capture(manifest,name);var tokens=GameObjectStockReview.Tokens(c.Bytes,0);
            foreach(var f in Ep1GameObjectStrongTokens.Fields)
                if(tokens[FastHash.GetHashCode(f.Name)].Type!=f.Type || tokens[FastHash.GetHashCode(f.Name)].Offset!=c.Projection.NativeRoot+f.Offset)
                    throw new InvalidDataException("Stock root strong-reference descriptor differs: "+f.Name);
        }
        Chunk? all=null;int failures=0;
        // Reborn: sparse shared-table indices, including index zero, must survive unchanged alongside a module FX import and moved root strings.
        uint[] indices={0,5,17};
        for(int mask=0;mask<8;mask++)
        {
            var xml=new XmlDocument {XmlResolver=null};
            xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='StrongProbe' EditorName='Reborn moved allocation'><Behaviors><BuildingDestructionBehavior id='ModuleTag_Strong'><DieMuxData/><Item FX='FxProbe'/></BuildingDestructionBehavior></Behaviors></GameObject>");
            for(int i=0;i<3;i++)if((mask&(1<<i))!=0)xml.DocumentElement!.SetAttribute(Ep1GameObjectStrongTokens.Fields[i].Name,"Target"+i);
            GameObjectScalarTokenSmokeTest.Validate(schemas,xml.OuterXml);
            for(int i=0;i<3;i++)if((mask&(1<<i))!=0)xml.DocumentElement!.SetAttribute(Ep1GameObjectStrongTokens.Fields[i].Name,"Target"+i+"\\"+indices[i]);
            ((XmlElement)xml.GetElementsByTagName("Item","uri:ea.com:eala:asset")[0]!).SetAttribute("FX","FxProbe\\31");
            var native=GameObjectScalarTokenSmokeTest.Compile(xml.OuterXml);var output=Ep1GameObjectDestructionProjection.Compose(native.Chunk);
            GameObjectDestructionProjectionSmokeTest.Authored(native);
            int prefix=GameObjectStockReview.Tokens(output.InstanceBuffer,0)[FastHash.GetHashCode("BuildTime")].Offset-140;
            int items=(int)Word(native.Chunk.InstanceBuffer,native.Native+68);
            var expected=new List<uint>();
            for(int i=0;i<3;i++)
            {
                int at=Ep1GameObjectStrongTokens.Fields[i].Offset;uint value=(mask&(1<<i))!=0?indices[i]+1:0;
                if(Word(native.Chunk.InstanceBuffer,at)!=value || Word(output.InstanceBuffer,prefix+at)!=value)throw new InvalidDataException("Root dependency index changed during projection.");
                if(value!=0)expected.Add((uint)at);
            }
            expected.Add((uint)(items+8));
            if(!Slots(native.Chunk.ImportsBuffer).OrderBy(s=>s).SequenceEqual(expected.OrderBy(s=>s)) ||
                !Slots(output.ImportsBuffer).OrderBy(s=>s).SequenceEqual(expected.Select(s=>s+(uint)prefix).OrderBy(s=>s)) ||
                Word(output.InstanceBuffer,prefix+items+8)!=32 || native.Native==612)
                throw new InvalidDataException("Root/module shared import ownership differs.");
            if(mask==7)all=native.Chunk;
        }
        foreach(var f in Ep1GameObjectStrongTokens.Fields)
        {
            var missing=Clone(all!);missing.ImportsBuffer=Pack(Slots(missing.ImportsBuffer).Where(s=>s!=f.Offset));Reject(missing);failures++;
            var zero=Clone(all!);Put(zero.InstanceBuffer,f.Offset,0);Reject(zero);failures++;
            var pointer=Clone(all!);pointer.RelocationBuffer=Pack(Slots(pointer.RelocationBuffer).Append((uint)f.Offset));Reject(pointer);failures++;
        }
        var unknown=Clone(all!);unknown.ImportsBuffer=Pack(Slots(unknown.ImportsBuffer).Append(140u));Reject(unknown);failures++;
        Console.WriteLine($"GameObject root strong slots: PASS (three descriptors/two stock roots, eight presence combinations, diagnostic shared root/module indices and dynamic strings; {failures} ownership refusals; no Core named-target closure/publication).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached import/relocation mutations never change the live native fixture. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed ownership must be refused before module extraction or output construction. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Chunk c){try{Ep1GameObjectDestructionProjection.Compose(c);}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed root import ownership admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect actual native slot lists without inferring them from compiler descriptors. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached faults retain one final stream sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.Append(uint.MaxValue).ToArray();var b=new byte[a.Length*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native import values are little-endian Win32 words, not pointers to dereference. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls write only detached buffers. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
