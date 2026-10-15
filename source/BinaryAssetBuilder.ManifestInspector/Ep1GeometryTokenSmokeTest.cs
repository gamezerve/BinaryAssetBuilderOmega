using System.Buffers.Binary;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: generated geometry metadata has an independent stock oracle, detached field verifier and adversarial native ownership tests.
internal static class Ep1GeometryTokenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize independently captured metadata through proven native owners and compare every generated prefix byte. */
    //-------------------------------------------------------------------------------------------------
    internal static void Stock(Chunk native,byte[] stock,int table,Dictionary<int,int> mapping)
    {
        var output=Check(native);int prefix=output.InstanceBuffer.Length-native.InstanceBuffer.Length;var expected=stock.AsSpan(table,prefix).ToArray();
        for(int at=8;at<prefix;at+=12)
        {
            uint target=Word(expected,at);if(target==0)continue;
            if(target>=table && target<table+prefix)Put(expected,at,target-(uint)table);
            else if(mapping.TryGetValue((int)target,out int mapped))Put(expected,at,(uint)(prefix+mapped));
            else throw new InvalidDataException("Stock geometry metadata target has no independently reviewed owner.");
        }
        if(!output.InstanceBuffer.AsSpan(0,prefix).SequenceEqual(expected))throw new InvalidDataException("Complete generated geometry prefix differs from stock.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare metadata and all native/auxiliary coordinates without consulting the producer's field descriptors. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Check(Chunk native){var output=Ep1GeometryTokenizer.Tokenize(native);Verify(output,native);return output;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed ownership, enum/flag/scalar/string/list/slot inputs and detached metadata must fail rather than yielding partial output. */
    //-------------------------------------------------------------------------------------------------
    internal static void Controls(Chunk native)
    {
        // Reborn: explicit empty strings and the contact-list admission boundary must be positive cases, not merely negative bounds checks.
        foreach(int count in new[]{1,128}){var source=new System.Xml.XmlDocument{XmlResolver=null};source.LoadXml("<Geometry xmlns='uri:ea.com:eala:asset'><Shape Name='' Other=''/>"+string.Concat(Enumerable.Repeat("<ContactPoint Label=''><Pos x='0' y='0' z='0'/></ContactPoint>",count))+"</Geometry>");Check(Ep1GeometryNativeSmokeTest.Compile(source));}
        int shapes=(int)Word(native.InstanceBuffer,8),items=(int)Word(native.InstanceBuffer,12),contacts=(int)Word(native.InstanceBuffer,16),points=(int)Word(native.InstanceBuffer,20),failures=0,metadataFailures=0;
        foreach(var fault in new[]{(8,0u),(8,33u),(16,129u),(12,0u),(12,Word(native.InstanceBuffer,4)),(20,(uint)items),(0,(uint)items),(4,(uint)items),(items,4u),(items+4,5u),(items+16,0x7F800000u),(items+32,0x7FC00000u),(items+40,12u),(items+8,uint.MaxValue),(items+12,(uint)items),(24,2u),(items+44,0x00000102u),(items+44,0x01000100u),(points+8,0x7F800000u)})
        {var bad=Clone(native);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GeometryTokenizer.Tokenize(bad));failures++;}
        var missing=Clone(native);missing.RelocationBuffer=Pack(Slots(native.RelocationBuffer).Where(s=>s!=12));Refuse(()=>Ep1GeometryTokenizer.Tokenize(missing));failures++;
        var duplicate=Clone(native);duplicate.RelocationBuffer=Pack(Slots(native.RelocationBuffer).Append(12u));Refuse(()=>Ep1GeometryTokenizer.Tokenize(duplicate));failures++;
        var extra=Clone(native);extra.RelocationBuffer=Pack(Slots(native.RelocationBuffer).Append((uint)(items+16)));Refuse(()=>Ep1GeometryTokenizer.Tokenize(extra));failures++;
        var malformed=Clone(native);malformed.RelocationBuffer[^1]=0;Refuse(()=>Ep1GeometryTokenizer.Tokenize(malformed));failures++;
        var imported=Clone(native);imported.ImportsBuffer=Pack(new[]{(uint)(items+16)});Refuse(()=>Ep1GeometryTokenizer.Tokenize(imported));failures++;
        var tail=Clone(native);tail.InstanceBuffer=tail.InstanceBuffer.Concat(new byte[4]).ToArray();Refuse(()=>Ep1GeometryTokenizer.Tokenize(tail));failures++;
        var text=Clone(native);int name=(int)Word(text.InstanceBuffer,items+12);text.InstanceBuffer[name]=255;Refuse(()=>Ep1GeometryTokenizer.Tokenize(text));failures++;
        var overlap=Clone(native);Put(overlap.InstanceBuffer,items+24,Word(native.InstanceBuffer,items+8)-1);Put(overlap.InstanceBuffer,items+28,(uint)(name+1));Refuse(()=>Ep1GeometryTokenizer.Tokenize(overlap));failures++;
        var terminator=Clone(native);terminator.InstanceBuffer[name+(int)Word(native.InstanceBuffer,items+8)]=65;Refuse(()=>Ep1GeometryTokenizer.Tokenize(terminator));failures++;
        var rotation=Clone(native);Put(rotation.InstanceBuffer,(int)Word(native.InstanceBuffer,0),0x7FC00000u);Refuse(()=>Ep1GeometryTokenizer.Tokenize(rotation));failures++;
        var padding=Clone(native);padding.InstanceBuffer[25]=1;Refuse(()=>Ep1GeometryTokenizer.Tokenize(padding));failures++;
        foreach(int length in new[]{0,27,75}){var bad=new Chunk{InstanceBuffer=new byte[length],RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};Refuse(()=>Ep1GeometryTokenizer.Tokenize(bad));failures++;}
        var good=Check(native);int placement=72,shapeList=216,shapeTable=shapeList+12+shapes*12,contactList=shapeList+12+shapes*156,contactTable=contactList+12+contacts*12;
        foreach(int at in new[]{0,4,8,16,20,24,28,32,36,40,44,48,52,56,60,placement+4,placement+8,placement+128,shapeList,shapeList+4,shapeList+8,shapeList+12,shapeList+16,shapeList+20,shapeTable+4,shapeTable+8,shapeTable+124,shapeTable+128,shapeTable+132,contactList,contactList+4,contactList+8,contactList+12,contactList+20,contactTable+4,contactTable+8,contactTable+16,contactTable+20,contactTable+24})
        {var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,native));metadataFailures++;}
        // Reborn: inverse native equality must also detect edits beyond token tables.
        var payload=Clone(good);int prefix=good.InstanceBuffer.Length-native.InstanceBuffer.Length;Put(payload.InstanceBuffer,prefix+items+16,0);Refuse(()=>Verify(payload,native));metadataFailures++;
        Console.WriteLine($"EP1 geometry tokens: PASS (five complete independent stock prefixes, 24 authored graphs/defaults plus empty-string/128-contact boundaries, {failures} native ownership and {metadataFailures} detached metadata/payload refusals; no object composition/publication/game).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent table sizes/types/targets and inverse RELO rebasing verify output without invoking Tokenize again. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output,Chunk native)
    {
        var b=output.InstanceBuffer;int shapes=(int)Word(native.InstanceBuffer,8),contacts=(int)Word(native.InstanceBuffer,16),placement=(int)Word(native.InstanceBuffer,4),items=(int)Word(native.InstanceBuffer,12),points=(int)Word(native.InstanceBuffer,20),rotation=(int)Word(native.InstanceBuffer,0);
        int shapeList=72+(placement==0?0:144),contactList=shapeList+12+shapes*156,prefix=contactList+12+contacts*48;
        if(b.Length!=prefix+native.InstanceBuffer.Length || output.ImportsBuffer.Length!=0)throw new InvalidDataException("Geometry token/native extent differs.");
        Fields(0,new[]{("IsSmall",0x77E3F4F7u,prefix+24),("RotationAnchorOffset",0x4AE31F24u,rotation==0?0:prefix+rotation),("PlacementShape",0xE7210555u,placement==0?0:72),("Shape",0xE7210555u,shapeList),("ContactPoint",0x6B8EE8EFu,contactList)});
        if(placement!=0)Shape(72,placement);
        Count(shapeList,prefix+8);for(int i=0;i<shapes;i++){int table=shapeList+12+shapes*12+i*144;Marker(shapeList+12+i*12,table);Shape(table,items+i*48);}
        Count(contactList,prefix+16);for(int i=0;i<contacts;i++){int table=contactList+12+contacts*12+i*36;Marker(contactList+12+i*12,table);Fields(table,new[]{("Label",0x1A2933AAu,prefix+points+i*20),("Pos",0x29368077u,prefix+points+i*20+8)});}
        var recovered=b.AsSpan(prefix).ToArray();foreach(uint slot in Slots(output.RelocationBuffer)){if(slot<prefix || slot>b.Length-4 || Word(b,(int)slot)<prefix)throw new InvalidDataException("Geometry rebased pointer escapes native owner.");Put(recovered,(int)slot-prefix,Word(b,(int)slot)-(uint)prefix);}
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)))throw new InvalidDataException("Geometry native/relocation inverse differs.");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: field types and exact targets are independent golden declarations, not production descriptor reuse. */
        //-------------------------------------------------------------------------------------------------
        void Fields(int at,(string Name,uint Type,int Target)[] expected){var rows=GameObjectStockReview.Tokens(b,at);if(rows.Count!=expected.Length)throw new InvalidDataException("Geometry token field inventory differs.");foreach(var e in expected)if(!rows.TryGetValue(FastHash.GetHashCode(e.Name),out var row) || row.Type!=e.Type || row.Offset!=e.Target)throw new InvalidDataException("Geometry field type/target differs.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: fixed-size vectors bind directly to native storage, while string tokens bind to inline length/pointer headers. */
        //-------------------------------------------------------------------------------------------------
        void Shape(int table,int at){int offset=(int)Word(native.InstanceBuffer,at+40);Fields(table,new[]{("Type",0xB0D49164u,prefix+at),("ContactPointGeneration",0x465508CEu,prefix+at+4),("Name",0x1A2933AAu,prefix+at+8),("Active",0x77E3F4F7u,prefix+at+44),("UsedForHealthBox",0x77E3F4F7u,prefix+at+45),("MajorRadius",0x645864F3u,prefix+at+16),("MinorRadius",0x645864F3u,prefix+at+20),("Other",0x1A2933AAu,prefix+at+24),("Height",0x645864F3u,prefix+at+32),("FrontAngle",0x4E16EA4Du,prefix+at+36),("Offset",0x29368077u,offset==0?0:prefix+offset)});}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: count metadata targets the root native count word, never the list's pointer or first record. */
        //-------------------------------------------------------------------------------------------------
        void Count(int at,int target){if(Word(b,at)!=0x6C827073 || Word(b,at+4)!=0x5D448FD8 || Word(b,at+8)!=target)throw new InvalidDataException("Geometry list count token differs.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: each native inline element has one concrete-free metadata marker. */
        //-------------------------------------------------------------------------------------------------
        void Marker(int at,int target){if(Word(b,at)!=0xB151A0EB || Word(b,at+4)!=0 || Word(b,at+8)!=target)throw new InvalidDataException("Geometry list marker differs.");}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all malformed native and detached output controls must fail explicitly. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed geometry token graph admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls never change live compiler buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached relocation streams retain an explicit checked sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Geometry verification slot stream malformed.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: owned malformed slot sequences keep the standard sentinel so ownership errors are tested independently. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently read metadata/native words only from bounded little-endian spans. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Geometry verification word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: corruption controls and stock coordinate normalization write only owned arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
