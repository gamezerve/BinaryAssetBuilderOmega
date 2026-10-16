using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.InteropServices;
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

// Reborn: independently recover complete EP1 vision records/tables, then join authored/default values to the selected shared Core object graph.
internal static class Ep1VisionInfoSmokeTest
{
    private static readonly string[] Names={"VisionRange","ShroudClearingRange","ShroudClearingHeight","VisionSide","VisionRear","VisionBonusTestRadius","VisionBonusTestSegments","VisionBonusPercentPerFoot","MaxVisionBonusPercent","MinVisionBonusPercent"};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock parity, schema/default drift, every pointed-root presence mask and detached failures remain separate from object publication. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest)
    {
        var saved=Settings.Current;
        try
        {
            if(Marshal.SizeOf<Ep1VisionInfo>()!=40 || Marshal.SizeOf<VisionInfo>()!=36 || Marshal.SizeOf<GameObject>()!=600 || Marshal.OffsetOf<GameObject>("VisionInfo").ToInt32()!=560 || Marshal.OffsetOf<Ep1VisionInfo>("ShroudClearingHeight").ToInt32()!=8 || Marshal.OffsetOf<Ep1VisionInfo>("VisionBonusTestSegments").ToInt32()!=24)throw new InvalidDataException("EP1 vision ABI differs.");
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed vision schema required.");
            foreach(string name in new[]{"AlliedConstructionYard","SovietAntiInfantryInfantry","JapanFortressShip"})Stock(official,manifest,name);
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootVisionPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);Drift(official,schemas.Schemas);
            var catalog=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();string[] types={"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"};var targets=types.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).ToArray();
            int cases=0;Chunk? populated=null;InstanceDeclaration? populatedInstance=null;
            foreach(int mask in Enumerable.Range(0,16))foreach(int profile in new[]{0,1,2})foreach(bool present in new[]{false,true})foreach(bool reverse in new[]{false,true})foreach(int position in new[]{0,1,2})
            {
                string source=Source(mask,profile,present,reverse,position);var pair=Compile(source,profile==2);var output=Ep1GameObjectDestructionProjection.Compose(pair.Native);Verify(output,pair.Native,pair.Instance,targets,mask,profile,present);
                var repeat=Compile(source,profile==2);var again=Ep1GameObjectDestructionProjection.Compose(repeat.Native);if(!output.InstanceBuffer.SequenceEqual(again.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(again.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(again.ImportsBuffer))throw new InvalidDataException("Vision repeat differs.");
                if(mask==15 && profile==2 && present && !reverse && position==0){populated=pair.Native;populatedInstance=pair.Instance;}cases++;
            }
            int nativeFailures=0,metadataFailures=0,pointer=(int)Word(populated!.InstanceBuffer,560);
            foreach(uint value in new[]{0u,(uint)(pointer+1),Word(populated.InstanceBuffer,424),(uint)populated.InstanceBuffer.Length}){var bad=Clone(populated);Put(bad.InstanceBuffer,560,value);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFailures++;}
            foreach(int i in Enumerable.Range(0,10).Where(i=>i!=6)){var bad=Clone(populated);Put(bad.InstanceBuffer,pointer+i*4,0x7F800000);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFailures++;}
            var missing=Clone(populated);missing.RelocationBuffer=Pack(Slots(missing.RelocationBuffer).Where(s=>s!=560));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(missing));nativeFailures++;
            var imported=Clone(populated);imported.ImportsBuffer=Pack(Slots(imported.ImportsBuffer).Append(560u));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(imported));nativeFailures++;
            var good=Ep1GameObjectDestructionProjection.Compose(populated);var root=GameObjectStockReview.Tokens(good.InstanceBuffer,0);int table=root[FastHash.GetHashCode("VisionInfo")].Offset;
            foreach(int i in Enumerable.Range(0,10))foreach(int delta in new[]{4,8}){var bad=Clone(good);Put(bad.InstanceBuffer,table+i*12+delta,Word(bad.InstanceBuffer,table+i*12+delta)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets,15,2,true));metadataFailures++;}
            foreach(int at in new[]{table+120,table+6*12+4}){var bad=Clone(good);Put(bad.InstanceBuffer,at,at==table+120?1u:0x23E650B8u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets,15,2,true));metadataFailures++;}
            int sourceFailures=0;string valid=Source(15,2,true,false,0);
            foreach(string bad in new[]{valid.Replace("VisionRange='-12.5'","VisionRange='NaN'",StringComparison.Ordinal),valid.Replace("VisionBonusTestSegments='4294967295'","VisionBonusTestSegments='4294967296'",StringComparison.Ordinal),valid.Replace("VisionBonusTestSegments='4294967295'","VisionBonusTestSegments='-1'",StringComparison.Ordinal),valid.Replace("VisionRange='-12.5'","UnknownVisionField='1'",StringComparison.Ordinal)}){Refuse(()=>{var pair=Compile(bad,true);Ep1GameObjectDestructionProjection.Compose(pair.Native);});sourceFailures++;}
            StandaloneControls();
            Console.WriteLine($"EP1 vision: PASS (three complete independent stock native/metadata graphs; {cases} actual-Core shared graph cases, all 16 other pointed-root masks and six behavior orders, up to {Ep1GameObjectRootValueTokens.AllFields.Count+10} partial root fields; {nativeFailures} root native/{metadataFailures} metadata/{sourceFailures} source refusals; 22 standalone refusals; legacy record retained, no general GameObject publication/alpha/game).");
            //-------------------------------------------------------------------------------------------------
            /** Reborn: preserve the existing pointed-root source profiles while appending vision in the actual official child order through adaptation. */
            //-------------------------------------------------------------------------------------------------
            string Source(int mask,int profile,bool present,bool reverse,int position)
            {
                string text=GameObjectPointerRootSmokeTest.Source(targets,mask,profile,reverse,position).Replace("RootPointerProbe","RootVisionProbe",StringComparison.Ordinal);
                return present?text.Insert(text.LastIndexOf("</RootVisionProbe>",StringComparison.Ordinal),Vision(profile)):text;
            }
            //-------------------------------------------------------------------------------------------------
            /** Reborn: raw official validation precedes actual Core normalization and unique stock target closure, never captured numeric source indices. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Compile(string source,bool strings)
            {
                var raw=Xml(source);GameObjectBodyProjectionSmokeTest.Validate(official,GameObjectCombatCompositionSmokeTest.Adapt(raw.DocumentElement!,strings).OuterXml);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Vision target missing/ambiguous.");return(GameObjectBodyProjectionSmokeTest.Compile(GameObjectCombatCompositionSmokeTest.Adapt(instance.XmlNode,strings),selectedBehaviors:true),instance);
            }
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete independent stock tables and native bytes using only recovered semantic literals, not copied producer prefixes. */
    //-------------------------------------------------------------------------------------------------
    private static void Stock(XmlSchemaSet schemas,string manifest,string name)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var roots=GameObjectStockReview.Tokens(stock.Bytes,0);var root=roots[FastHash.GetHashCode("VisionInfo")];int pointer=(int)Word(stock.Bytes,stock.Projection.NativeRoot+560);if(root.Type!=0x649F97D1 || pointer<stock.Projection.NativeRoot+600 || pointer>stock.Bytes.Length-40)throw new InvalidDataException("Stock vision root differs.");var fields=GameObjectStockReview.Tokens(stock.Bytes,root.Offset);if(fields.Count!=10)throw new InvalidDataException("Stock vision inventory differs.");
        var source=Xml("<VisionInfo xmlns='"+SchemaSet.XmlNamespace+"'/>");for(int i=0;i<10;i++){var f=fields[FastHash.GetHashCode(Names[i])];if(f.Type!=(i==6?0x763177A4u:0x645864F3u) || f.Offset!=pointer+i*4)throw new InvalidDataException("Stock vision field target/type differs.");float value=BitConverter.Int32BitsToSingle(unchecked((int)Word(stock.Bytes,f.Offset)));if(i!=6 && !float.IsFinite(value))throw new InvalidDataException("Stock vision float differs.");source.DocumentElement!.SetAttribute(Names[i],i==6?Word(stock.Bytes,f.Offset).ToString(CultureInfo.InvariantCulture):(i is 3 or 4 or 7 or 8 or 9?value*100:value).ToString("R",CultureInfo.InvariantCulture));}
        GameObjectBodyProjectionSmokeTest.Validate(schemas,"<GameObject xmlns='"+SchemaSet.XmlNamespace+"' id='StockVision'><Behaviors><ObjectSMCHelper id='Helper'/></Behaviors><Body><ActiveBody id='Body' MaxHealth='100'/></Body>"+source.DocumentElement!.OuterXml+"</GameObject>");var native=Native(source);var output=Ep1VisionInfoTokenizer.Tokenize(native);if(!native.InstanceBuffer.AsSpan().SequenceEqual(stock.Bytes.AsSpan(pointer,40)))throw new InvalidDataException("Complete stock vision native differs.");
        var metadata=stock.Bytes.AsSpan(root.Offset,132).ToArray();for(int i=0;i<10;i++)Put(metadata,i*12+8,(uint)(132+i*4));if(!metadata.AsSpan().SequenceEqual(output.InstanceBuffer.AsSpan(0,132)))throw new InvalidDataException("Complete independent stock vision prefix differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal profiles pin default values, fractional percentages, unsigned maximum and the newly restored height word independently. */
    //-------------------------------------------------------------------------------------------------
    internal static string Vision(int profile)=>profile switch{0=>"<VisionInfo/>",1=>"<VisionInfo VisionRange='425' ShroudClearingRange='600' ShroudClearingHeight='75' VisionSide='25' VisionRear='50' VisionBonusTestRadius='150' VisionBonusTestSegments='16' VisionBonusPercentPerFoot='2' MaxVisionBonusPercent='80' MinVisionBonusPercent='-25'/>",_=>"<VisionInfo VisionRange='-12.5' ShroudClearingRange='0.25' ShroudClearingHeight='-75' VisionSide='125' VisionRear='-25' VisionBonusTestRadius='-2' VisionBonusTestSegments='4294967295' VisionBonusPercentPerFoot='-2' MaxVisionBonusPercent='150' MinVisionBonusPercent='-100'/>"};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit expected float/uint bits are source contracts, not values decoded from the producer under test. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Expected(int profile)
    {
        float[] values=profile switch{0=>new[]{0f,0,50,1,1,100,0,0,0.5f,-0.5f},1=>new[]{425f,600,75,0.25f,0.5f,150,0,0.02f,0.8f,-0.25f},_=>new[]{-12.5f,0.25f,-75,1.25f,-0.25f,-2,0,-0.02f,1.5f,-1f}};var words=values.Select(BitConverter.SingleToUInt32Bits).ToArray();words[6]=profile==0?8u:profile==1?16u:uint.MaxValue;
        // Reborn: the current float32 percentage conversion multiplies 80 by float32 .01, producing 3F4CCCCC rather than rounded literal .8 (3F4CCCCD).
        if(profile==1)words[8]=0x3F4CCCCC;return words;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: previous root/geometry/import verification remains active while vision owns exactly one additional field/table/native record. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,ManifestAsset[] targets,int mask,int profile,bool present,int extraRoots=0,int tailMetadata=0,IReadOnlyList<int>? extraImportSlots=null)
    {
        GameObjectPointerRootSmokeTest.Verify(output,native,instance,targets,mask,profile,(present?1:0)+extraRoots,(present?132:0)+tailMetadata,extraImportSlots);var roots=GameObjectStockReview.Tokens(output.InstanceBuffer,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140,pointer=(int)Word(native.InstanceBuffer,560);if(!present){if(pointer!=0 || roots.ContainsKey(FastHash.GetHashCode("VisionInfo")))throw new InvalidDataException("Absent vision emitted data.");return;}
        var field=roots[FastHash.GetHashCode("VisionInfo")];if(field.Type!=0x649F97D1 || field.Offset!=prefix-tailMetadata-132 || pointer<600 || pointer>=Word(native.InstanceBuffer,476) || Word(output.InstanceBuffer,prefix+560)!=prefix+pointer || !Slots(native.RelocationBuffer).Contains(560u))throw new InvalidDataException("Vision composed root owner differs.");var rows=GameObjectStockReview.Tokens(output.InstanceBuffer,field.Offset);if(rows.Count!=10)throw new InvalidDataException("Vision composed field inventory differs.");var words=Expected(profile);for(int i=0;i<10;i++){var row=rows[FastHash.GetHashCode(Names[i])];if(row.Type!=(i==6?0x763177A4u:0x645864F3u) || row.Offset!=prefix+pointer+i*4 || Word(output.InstanceBuffer,row.Offset)!=words[i])throw new InvalidDataException("Vision composed literal/type/target differs.");}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: owned standalone native allocation uses the new EP1 overload and has no pointers, imports or second tracker. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Native(XmlDocument source)
    {
        var ns=new XmlNamespaceManager(source.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1VisionInfo* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1VisionInfo),false);Marshaler.Marshal(new Node(source.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Vision allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all nine real positions reject NaN/infinity; no legacy width, trailing storage or auxiliary slots can become a valid fragment. */
    //-------------------------------------------------------------------------------------------------
    private static void StandaloneControls()
    {
        var good=Native(Xml("<VisionInfo xmlns='"+SchemaSet.XmlNamespace+"'/>"));if(!Enumerable.Range(0,10).Select(i=>Word(good.InstanceBuffer,i*4)).SequenceEqual(Expected(0)))throw new InvalidDataException("Standalone vision defaults differ.");foreach(int i in Enumerable.Range(0,10).Where(i=>i!=6))foreach(uint value in new[]{0x7FC00000u,0x7F800000u}){var bad=Clone(good);Put(bad.InstanceBuffer,i*4,value);Refuse(()=>Ep1VisionInfoTokenizer.Tokenize(bad));}
        foreach(int size in new[]{36,44}){var bad=Clone(good);bad.InstanceBuffer=new byte[size];Refuse(()=>Ep1VisionInfoTokenizer.Tokenize(bad));}var relo=Clone(good);relo.RelocationBuffer=Pack(new[]{0u});Refuse(()=>Ep1VisionInfoTokenizer.Tokenize(relo));var imp=Clone(good);imp.ImportsBuffer=Pack(new[]{0u});Refuse(()=>Ep1VisionInfoTokenizer.Tokenize(imp));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: duplicated focused declarations must retain all ten official attribute types, defaults and use constraints. */
    //-------------------------------------------------------------------------------------------------
    private static void Drift(XmlSchemaSet official,XmlSchemaSet focused)
    {
        var key=new XmlQualifiedName("VisionInfo",SchemaSet.XmlNamespace);var a=(XmlSchemaComplexType)official.GlobalTypes[key]!;var b=(XmlSchemaComplexType)focused.GlobalTypes[key]!;if(a.AttributeUses.Count!=10 || b.AttributeUses.Count!=10)throw new InvalidDataException("Vision schema inventory drift.");foreach(XmlSchemaAttribute field in a.AttributeUses.Values){var other=(XmlSchemaAttribute)b.AttributeUses[field.QualifiedName]!;if(other==null || other.AttributeSchemaType!.QualifiedName!=field.AttributeSchemaType!.QualifiedName || other.DefaultValue!=field.DefaultValue || other.Use!=field.Use)throw new InvalidDataException("Vision schema type/default drift.");}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: linked EP1 target metadata is validated before admitting named reference identities. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Vision catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: failures remain explicit rather than silently stripping an unsupported child. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(Exception e)when(e is InvalidDataException or OverflowException or FormatException){return;}throw new InvalidDataException("Malformed vision admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source parsing does not resolve external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument{XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls own every buffer they change. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: nonempty auxiliary streams require one trailing sentinel and no sentinel-only pseudo-slot. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Vision slot stream malformed.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: extracted mutation slots retain standard empty or sentinel framing. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded little-endian word access never dereferences captured pointers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Vision word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only caller-owned arrays receive metadata/native mutation writes. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
