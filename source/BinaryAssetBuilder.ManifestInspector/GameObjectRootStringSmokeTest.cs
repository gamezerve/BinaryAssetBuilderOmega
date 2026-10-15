using System.Buffers.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: string allocation tests separate ASCII/native pointer ownership from unsupported general encoding and full object publication.
internal static class GameObjectRootStringSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: audit six stock string-header tokens, then exercise absent/empty/short/long content, moved module allocation and detached pointer failures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlSchemaSet schemas,string manifest)
    {
        foreach(string name in new[] {"GameObject:AlliedConstructionYard","GameObject:SovietAntiInfantryInfantry"})
        {
            var c=GameObjectStockReview.Capture(manifest,name);var tokens=GameObjectStockReview.Tokens(c.Bytes,0);
            foreach(var f in Ep1GameObjectStringTokens.Fields)
                if(tokens[FastHash.GetHashCode(f.Name)].Type!=f.Type || tokens[FastHash.GetHashCode(f.Name)].Offset!=c.Projection.NativeRoot+f.Offset)throw new InvalidDataException("Stock root string header contract differs.");
        }
        Ep1AlliedModuleSmokeTest.Output? populated=null;
        foreach(int mode in new[] {0,1,2,3})
        {
            var xml=Source("<BuildingDestructionBehavior id='ModuleTag_Strings'><DieMuxData/><Item/><Item/><Item/></BuildingDestructionBehavior>",mode);
            GameObjectScalarTokenSmokeTest.Validate(schemas,xml.OuterXml);var c=GameObjectScalarTokenSmokeTest.Compile(xml.OuterXml);Check(xml,c);
            if(mode==2)populated=c;
            if(mode>0 && c.Native==612)throw new InvalidDataException("Root strings did not move the module allocation.");
        }
        var original=populated!.Chunk;int failures=0;
        foreach(var f in Ep1GameObjectStringTokens.Fields)
        {
            int p=(int)Word(original.InstanceBuffer,f.Offset+4),length=(int)Word(original.InstanceBuffer,f.Offset);
            foreach(var fault in new[] {(f.Offset,uint.MaxValue),(f.Offset+4,4u),(f.Offset+4,(uint)populated.Native)})
            {var bad=Clone(original);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));failures++;}
            var unterminated=Clone(original);unterminated.InstanceBuffer[p+length]=1;Refuse(()=>Ep1GameObjectDestructionProjection.Compose(unterminated));failures++;
            var encoding=Clone(original);encoding.InstanceBuffer[p]=128;Refuse(()=>Ep1GameObjectDestructionProjection.Compose(encoding));failures++;
            var missing=Clone(original);missing.RelocationBuffer=Pack(Slots(original.RelocationBuffer).Where(s=>s!=f.Offset+4));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(missing));failures++;
        }
        var overlap=Clone(original);int first=(int)Word(overlap.InstanceBuffer,56);uint size=Word(overlap.InstanceBuffer,52);
        Put(overlap.InstanceBuffer,64,(uint)(first+1));Put(overlap.InstanceBuffer,60,size-1);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(overlap));failures++;
        Console.WriteLine($"GameObject root strings: PASS (six headers/two stock roots, absent/empty/ASCII/4096-byte values, moved list/module and three Items; {failures} malformed ownership/content refusals; no general encoding/full object support).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: combine moved root allocations with actual Core-normalized FX imports only after validating the separate raw authored module. */
    //-------------------------------------------------------------------------------------------------
    internal static void WithNamedFx(XmlSchemaSet schemas,string rawModule,string normalizedModule)
    {
        GameObjectScalarTokenSmokeTest.Validate(schemas,Source(rawModule,2).OuterXml);
        var xml=Source(normalizedModule,2);var compiled=GameObjectScalarTokenSmokeTest.Compile(xml.OuterXml);Check(xml,compiled);
        if(compiled.Chunk.ImportsBuffer.Length==0 || compiled.Native==612)throw new InvalidDataException("Moved root strings/Core import integration was not exercised.");
        Console.WriteLine("GameObject root strings + actual Core named FX: PASS (dynamic module ownership/native/import round trip).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare literal source strings with native and shifted output headers, including null versus allocated empty strings. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(XmlDocument source,Ep1AlliedModuleSmokeTest.Output native)
    {
        GameObjectDestructionProjectionSmokeTest.Authored(native);var output=Ep1GameObjectDestructionProjection.Compose(native.Chunk);
        int prefix=GameObjectStockReview.Tokens(output.InstanceBuffer,0)[FastHash.GetHashCode("BuildTime")].Offset-140;
        foreach(var f in Ep1GameObjectStringTokens.Fields)
        {
            bool present=source.DocumentElement!.HasAttribute(f.Name);string expected=source.DocumentElement.GetAttribute(f.Name);
            foreach(var data in new[] {(native.Chunk.InstanceBuffer,0),(output.InstanceBuffer,prefix)})
            {
                int at=data.Item2+f.Offset;uint len=Word(data.Item1,at),pointer=Word(data.Item1,at+4);
                if(!present){if(len!=0 || pointer!=0)throw new InvalidDataException("Absent root string allocated.");continue;}
                if(len!=expected.Length || pointer==0 || pointer>=data.Item1.Length || len>data.Item1.Length-pointer-1 || data.Item1[(int)(pointer+len)]!=0 || Encoding.ASCII.GetString(data.Item1,(int)pointer,(int)len)!=expected)
                    throw new InvalidDataException("Root string source/native/generated content differs: "+f.Name);
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled ASCII fixture attributes exercise XML escaping and different allocation lengths without changing global ANSI marshal policy. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Source(string module,int mode)
    {
        var d=new XmlDocument {XmlResolver=null};d.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='StringProbe'><Behaviors/></GameObject>");
        // Reborn: authored fragments without an explicit xmlns inherit the EA namespace before import, rather than becoming no-namespace behavior elements.
        var leaf=new XmlDocument {XmlResolver=null};leaf.LoadXml("<Wrapper xmlns='uri:ea.com:eala:asset'>"+module+"</Wrapper>");d.DocumentElement!.FirstChild!.AppendChild(d.ImportNode(leaf.DocumentElement!.FirstChild!,true));
        if(mode!=0)foreach(var f in Ep1GameObjectStringTokens.Fields)
            d.DocumentElement.SetAttribute(f.Name,mode==1?"":mode==3?new string('A',4096):"Reborn "+f.Name+" <tag> & / quote\"");
        return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test faults own all data before any edit. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled relocation removal keeps the actual stream's final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: repackage detached relocation membership without changing the original stream. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){var a=values.Append(uint.MaxValue).ToArray();var b=new byte[a.Length*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed root allocations must fail instead of silently producing incomplete tokens. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed root string admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded native/header reads remain independent of production layout inspection. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Root string test bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached faults use the native Win32 byte order. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
