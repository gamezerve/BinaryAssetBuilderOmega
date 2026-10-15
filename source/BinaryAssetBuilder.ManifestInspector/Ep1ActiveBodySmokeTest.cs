using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: stock-proven standalone EP1 ActiveBody/native/token fragment gate; legacy dispatch and complete GameObject remain unchanged.
internal static class Ep1ActiveBodySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: audit selected stock body graphs, then batch official defaults, explicit values, real Core imports and bounded child lists. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        var saved=Settings.Current;
        try
        {
            if(Marshal.SizeOf<Ep1ActiveBodyModuleData>()!=76 || Marshal.SizeOf<Ep1ActiveBodyDamageRangeModifier>()!=12)throw new InvalidDataException("EP1 body ABI differs.");
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed body schema required.");
            Settings.Current=new Settings {SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"ActiveBodyPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);
            foreach(var selected in new[]{("GameObject:AlliedConstructionYard",4000),("GameObject:SovietAntiInfantryInfantry",100)})
            {
                string source=$"<ActiveBody xmlns='uri:ea.com:eala:asset' id='ModuleTag_Body' MaxHealth='{selected.Item2}'/>";
                Validate(official,source);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);var native=Compile(instance.XmlNode);var output=Check(native);
                var stock=GameObjectStockReview.Capture(manifest,selected.Item1);
                var module=GameObjectModuleReview.Inspect(manifest,selected.Item1).Containers.Single(c=>c.Name=="Body").Modules.Single();
                if(module.TypeId!=0x5A0E3D9B || !native.InstanceBuffer.SequenceEqual(stock.Bytes.AsSpan(module.Native,76).ToArray()))throw new InvalidDataException("Complete selected EP1 body native payload differs.");
                var expected=stock.Bytes.AsSpan(module.Wrapper,324).ToArray();
                for(int at=8;at<324;at+=12){uint target=Word(expected,at);if(target!=0)Put(expected,at,target>=module.Native?target-(uint)module.Native+324:target-(uint)module.Wrapper);}
                if(!output.InstanceBuffer.AsSpan(0,324).SequenceEqual(expected))throw new InvalidDataException("Generated body inheritance/own/list prefix differs from stock.");
            }
            var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));
            TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);
            if(catalog.Validate().Count!=0 || !catalog.Header.IsLinked)throw new InvalidDataException("Valid body target catalog required.");
            var fx=catalog.Assets.First(a=>a.TypeId==0x86682E78);var modifier=catalog.Assets.First(a=>a.TypeId==0xC5E07887);
            Chunk? populated=null;
            foreach(int count in new[]{0,1,3,64})
            {
                string children=string.Concat(Enumerable.Repeat($"<DamageRangeModifier Min='25%' Max='75%' AttributeModifier='{modifier.Name}'/>",count));
                string source=$"<ActiveBody xmlns='uri:ea.com:eala:asset' id='BodyProbe' MaxHealth='1234' MaxHealthDamagedPercentage='50%' MaxHealthReallyDamagedPercentage='25%' InitialHealth='900' DodgePercent='7' EnteringDamagedTransitionTime='2s' EnteringReallyDamagedTransitionTime='500ms' CheerRadius='300' RemoveUpgradesOnDeath='true' BurningDeathBehavior='true' HealingBuffFX='{fx.Name}' DamagedAttributeModifier='{modifier.Name}' ReallyDamagedAttributeModifier='{modifier.Name}' BurningDeathFX='{fx.Name}'>{children}</ActiveBody>";
                Validate(official,source);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);
                var native=Compile(instance.XmlNode);Check(native);
                var repeat=Compile(ReferencePipelineSmokeTest.Normalize(schemas,source).XmlNode);
                if(!native.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !native.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !native.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))throw new InvalidDataException("Body repeat differs.");
                foreach(var value in new[]{(8,1234f),(12,.5f),(16,.25f),(20,900f),(24,7f),(28,2f),(32,.5f),(56,300f)})
                    if(Word(native.InstanceBuffer,value.Item1)!=BitConverter.SingleToUInt32Bits(value.Item2))throw new InvalidDataException("Body explicit scalar/percentage/time differs.");
                if(native.InstanceBuffer[72]!=1 || native.InstanceBuffer[73]!=1 || instance.ReferencedInstances.Count!=4+count)throw new InvalidDataException("Body flags/reference occurrences differ.");
                // Reborn: real Core assigns parent occurrences before child records; match every stored word to concrete catalog identity independently.
                var expectedSlots=new[]{44,48,52,60}.Concat(Enumerable.Range(0,count).Select(i=>76+i*12+8)).ToArray();
                for(int i=0;i<expectedSlots.Length;i++)
                {
                    var r=instance.ReferencedInstances[i];var target=i is 0 or 3?fx:modifier;
                    if(Word(native.InstanceBuffer,expectedSlots[i])!=i+1 || r.TypeId!=target.TypeId || r.InstanceId!=target.InstanceId || catalog.Assets.Count(a=>a.TypeId==r.TypeId && a.InstanceId==r.InstanceId)!=1)throw new InvalidDataException("Body Core/catalog import binding differs.");
                }
                for(int i=0;i<count;i++)if(Word(native.InstanceBuffer,76+i*12)!=0x3E800000 || Word(native.InstanceBuffer,80+i*12)!=0x3F400000)throw new InvalidDataException("Body child percentage values differ.");
                if(count==3)populated=native;
            }
            int failures=0;
            foreach(var fault in new[]{(0,0u),(64,65u),(68,0u),(36,1u),(40,76u),(72,2u),(68,uint.MaxValue)})
            {var bad=Clone(populated!);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1ActiveBodyTokenizer.Tokenize(bad));failures++;}
            var missing=Clone(populated!);missing.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Ep1ActiveBodyTokenizer.Tokenize(missing));failures++;
            var missingRelo=Clone(populated!);missingRelo.RelocationBuffer=Array.Empty<byte>();Refuse(()=>Ep1ActiveBodyTokenizer.Tokenize(missingRelo));failures++;
            // Reborn: exercise the independent output verifier, not only production refusal, with detached own/base/list/child metadata corruption.
            var goodOutput=Check(populated!);int metadataFailures=0;
            foreach(int at in Enumerable.Range(0,15).SelectMany(i=>new[]{28+i*12,32+i*12}).Concat(new[]{0,4,8,12,16,20,228,232,236,252,256,260,280,284,292,296,208,212,312,316,320,324,328,332,368,372,380,384,392,396}))
            {var bad=Clone(goodOutput);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,populated!));metadataFailures++;}
            foreach(string bad in new[]{"<ActiveBody xmlns='uri:ea.com:eala:asset' id='Bad' MaxHealthDamaged='10'/>","<ActiveBody xmlns='uri:ea.com:eala:asset' id='Bad'><DamageRangeModifier Min='1' Max='2'/></ActiveBody>"})
            {Refuse(()=>Validate(official,bad));failures++;}
            Console.WriteLine($"EP1 ActiveBody: PASS (76-byte separate ABI, two complete selected stock native graphs/generated inheritance prefixes, actual Core four root imports + 0/1/3/64 range children, explicit/default values, deterministic fragments, {failures} input and {metadataFailures} metadata refusals; no legacy dispatch replacement, general body/source closure/GameObject composition/package/game).");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate raw body source in its actual official GameObject container before internal normalized TypeId injection. */
    //-------------------------------------------------------------------------------------------------
    private static void Validate(XmlSchemaSet schemas,string body)
    {
        var binding=SdkEffectiveSchema.Bind(schemas,"in-memory-ep1-body.xml",Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'><GameObject id='BodyValidation'><Body>"+body+"</Body></GameObject></AssetDeclaration>"));
        if(!binding.XmlValidated)throw new InvalidDataException("EP1 body schema refused: "+string.Join("; ",binding.Errors));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile through an explicit EP1 model overload, keeping legacy KW derived-body layouts and dispatch untouched. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlNode source)
    {
        var d=new XmlDocument {XmlResolver=null};d.LoadXml(source.OuterXml);d.DocumentElement!.SetAttribute("TypeId","1510882715");
        var ns=new XmlNamespaceManager(d.NameTable);ns.AddNamespace("ea","uri:ea.com:eala:asset");
        Ep1ActiveBodyModuleData* body;using var tracker=new Tracker((void**)&body,(uint)sizeof(Ep1ActiveBodyModuleData),false);
        // Reborn: standalone roots need the same concrete ID initialization normally owned by polymorphic allocation; base marshaling writes only the weak id.
        body->Base.Base.Base.TypeId=0x5A0E3D9B;
        Marshaler.Marshal(new Node(d.DocumentElement.CreateNavigator()!,ns),body,tracker);var chunk=new Chunk();
        if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("EP1 body allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently walk generated body/base/list/child tokens and reverse native plus auxiliary shifts. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native)
    {
        var result=Ep1ActiveBodyTokenizer.Tokenize(native);Verify(result,native);return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify already-generated or detached-corrupted output without invoking the producer again. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk result,Chunk native)
    {
        var b=result.InstanceBuffer;int count=(int)Word(native.InstanceBuffer,64),prefix=324+count*60;
        if(Word(b,0)!=2 || Word(b,4)!=0x5A0E3D9B || Word(b,8)!=12 || Word(b,12)!=1 || Word(b,16)!=FastHash.GetHashCode("BodyModuleData") || Word(b,20)!=228 || Word(b,228)!=1 || Word(b,232)!=FastHash.GetHashCode("BehaviorModuleData") || Word(b,236)!=252 || Word(b,252)!=1 || Word(b,256)!=FastHash.GetHashCode("ModuleData") || Word(b,260)!=276)throw new InvalidDataException("Body inherited token chain differs.");
        var terminal=GameObjectStockReview.Tokens(b,276);
        if(terminal.Count!=2 || terminal[FastHash.GetHashCode("TypeId")].Type!=0x23E650B8 || terminal[FastHash.GetHashCode("TypeId")].Offset!=prefix || terminal[FastHash.GetHashCode("id")].Type!=0xBDF07965 || terminal[FastHash.GetHashCode("id")].Offset!=prefix+4)throw new InvalidDataException("Body terminal base tokens differ.");
        var fields=GameObjectStockReview.Tokens(b,12);
        foreach(var f in Ep1ActiveBodyTokenizer.Fields)if(fields[FastHash.GetHashCode(f.Name)].Type!=f.Type || fields[FastHash.GetHashCode(f.Name)].Offset!=prefix+f.Offset)throw new InvalidDataException("Body own token binding differs.");
        if(fields[FastHash.GetHashCode("DamageRangeModifier")].Type!=0x751527B1 || fields[FastHash.GetHashCode("DamageRangeModifier")].Offset!=312 || Word(b,312)!=0x6C827073 || Word(b,316)!=0x5D448FD8 || Word(b,320)!=prefix+64)throw new InvalidDataException("Body range list token differs.");
        for(int i=0;i<count;i++)
        {
            if(Word(b,324+i*12)!=0xB151A0EB || Word(b,328+i*12)!=0)throw new InvalidDataException("Body child marker differs.");
            int table=(int)Word(b,332+i*12);
            if(table!=324+count*12+i*48)throw new InvalidDataException("Body child token table ownership differs.");
            var child=GameObjectStockReview.Tokens(b,table);
            if(child.Count!=3)throw new InvalidDataException("Body child token field count differs.");
            foreach(var f in new[]{("Min",0,0x645864F3u),("Max",4,0x645864F3u),("AttributeModifier",8,0x09DDCFF3u)})
                if(!child.TryGetValue(FastHash.GetHashCode(f.Item1),out var token) || token.Type!=f.Item3 || token.Offset!=prefix+76+i*12+f.Item2)throw new InvalidDataException("Body child token differs.");
        }
        var recovered=b.AsSpan(prefix).ToArray();if(count!=0)Put(recovered,68,Word(recovered,68)-(uint)prefix);
        if(!recovered.SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("Body native/index round trip differs.");
        foreach(var pair in new[]{(result.RelocationBuffer,native.RelocationBuffer),(result.ImportsBuffer,native.ImportsBuffer)})
            if(!Slots(pair.Item1).Select(s=>s-(uint)prefix).SequenceEqual(Slots(pair.Item2)))throw new InvalidDataException("Body auxiliary round trip differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached fault buffers preserve the actual input before corruption. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent slot streams remain absent during independent comparison. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected malformed schema/native inputs must fail explicitly, not publish partial output. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed EP1 body admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all diagnostic reads use bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation and inverse-shift tests modify owned diagnostic buffers only. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
