using System.Buffers.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: common inline AI joins the selected real Core armor/movement/body/behavior/geometry/vision graph, not a generally publishable GameObject.
internal static class GameObjectAiCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: combine every prior pointed-root mask, AI count/profile and behavior order, optionally consuming a genuinely new Core armor target. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest,string? authoredDirectory=null)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed shared AI schema required.");
            foreach(string name in new[]{"AlliedConstructionYard","SovietAntiInfantryInfantry"}){var stockRoot=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var field=GameObjectStockReview.Tokens(stockRoot.Bytes,0)[FastHash.GetHashCode("AI")];if(field.Type!=0x5FD8691D || Word(stockRoot.Bytes,field.Offset)!=0x6C827073 || Word(stockRoot.Bytes,field.Offset+4)!=0x5D448FD8 || Word(stockRoot.Bytes,field.Offset+8)!=stockRoot.Projection.NativeRoot+480)throw new InvalidDataException("Independent stock inline AI count contract differs.");}
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootAiPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);var stock=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();var catalog=stock;ManifestAsset? authored=null;
            if(authoredDirectory!=null){var built=GameObjectArmorCompositionSmokeTest.BuildArmor(authoredDirectory);authored=built.Assets.Single(a=>a.Name=="ArmorTemplate:RebornComposedArmor");if(stock.Any(a=>a.TypeId==authored.TypeId && a.InstanceId==authored.InstanceId))throw new InvalidDataException("Shared AI authored target collision.");catalog=catalog.Concat(built.Assets).ToArray();}
            var targets=new[]{"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"}.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).Concat(new[]{catalog.Single(a=>a.Name=="TargetingCompareList:DefaultTargetingCompareList")}).ToArray();if(authored!=null)targets[2]=authored;
            int cases=0;Chunk? populated=null;InstanceDeclaration? populatedInstance=null;
            foreach(int mask in Enumerable.Range(0,16))foreach(int profile in new[]{0,1,2})foreach(int count in new[]{0,1,2})foreach(bool reverse in new[]{false,true})foreach(int position in new[]{0,1,2})
            {
                string source=Source(targets,mask,profile,count,reverse,position);var pair=Compile(source,catalog);var output=Ep1GameObjectDestructionProjection.Compose(pair.Native);Verify(output,pair.Native,pair.Instance,targets,mask,profile,count);var repeat=Compile(source,catalog);var other=Ep1GameObjectDestructionProjection.Compose(repeat.Native);if(!output.InstanceBuffer.SequenceEqual(other.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(other.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(other.ImportsBuffer))throw new InvalidDataException("Shared AI repeat differs.");if(mask==15 && profile==2 && count==2 && !reverse && position==0){populated=pair.Native;populatedInstance=pair.Instance;}cases++;
            }
            int items=(int)Word(populated!.InstanceBuffer,484),first=(int)Word(populated.InstanceBuffer,items),second=(int)Word(populated.InstanceBuffer,items+4),chooser=(int)Word(populated.InstanceBuffer,first+68),body=(int)Word(populated.InstanceBuffer,488),nativeFailures=0,sourceFailures=0,metadataFailures=0;
            foreach(var fault in new[]{(480,3u),(480,0u),(484,0u),(484,Word(populated.InstanceBuffer,424)),(items,(uint)second),(items+4,(uint)first),(items,(uint)(first+1)),(first,0u),(first+4,Word(populated.InstanceBuffer,body+4)),(first+12,Word(populated.InstanceBuffer,128)),(528,(uint)first),(chooser+36,0u)}){var bad=Clone(populated);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFailures++;}
            foreach(uint slot in new[]{484u,(uint)items,(uint)(items+4),(uint)(first+12),(uint)(first+68)}){var bad=Clone(populated);bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Where(s=>s!=slot));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFailures++;}
            var extraImport=Clone(populated);extraImport.ImportsBuffer=Pack(Slots(extraImport.ImportsBuffer).Append((uint)(first+4)));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(extraImport));nativeFailures++;
            string valid=Source(targets,15,2,2,false,0);
            foreach(string bad in new[]{valid.Replace("id='AI0'","id='bOdY'",StringComparison.Ordinal),valid.Replace("id='AI1'","id='ai0'",StringComparison.Ordinal),valid.Replace("id='AI0'","id=''",StringComparison.Ordinal),valid.Replace("AssociatedModule='Body'","AssociatedModule='MissingAI'",StringComparison.Ordinal),valid.Replace("<AIUpdate id='AI0'","<AIUpdate UnknownAI='1' id='AI0'",StringComparison.Ordinal),valid.Replace("<UnitAITargetChooserData/>","<UnitAITargetChooserData TargetingCompareList='MissingSharedCompare'/>",StringComparison.Ordinal),valid.Replace("<UnitAITargetChooserData/>","<UnitAITargetChooserData><ObjectVisionRange><Filter/></ObjectVisionRange></UnitAITargetChooserData>",StringComparison.Ordinal)}){Refuse(()=>Compile(bad,catalog));sourceFailures++;}
            if(authored!=null){Refuse(()=>Compile(valid,stock));sourceFailures++;}
            // Reborn: a destruction link may resolve forward to an admitted AI module in the same local scope, not only Body or Behaviors.
            var linked=Compile(valid.Replace("AssociatedModule='Body'","AssociatedModule='AI0'",StringComparison.Ordinal),catalog);var linkedOutput=Ep1GameObjectDestructionProjection.Compose(linked.Native);int behaviorList=(int)Word(linked.Native.InstanceBuffer,476);int destruction=Enumerable.Range(0,3).Select(i=>(int)Word(linked.Native.InstanceBuffer,behaviorList+8+i*4)).Single(at=>Word(linked.Native.InstanceBuffer,at)==0x0D96A2F4);if(Word(linked.Native.InstanceBuffer,destruction+52)!=FastHash.GetHashCode("ai0") || linkedOutput.InstanceBuffer.Length<=linked.Native.InstanceBuffer.Length)throw new InvalidDataException("Shared AI local AssociatedModule link differs.");
            var good=Ep1GameObjectDestructionProjection.Compose(populated);var roots=GameObjectStockReview.Tokens(good.InstanceBuffer,0);int aiTable=roots[FastHash.GetHashCode("AI")].Offset,aiRow=FindRow(good.InstanceBuffer,FastHash.GetHashCode("AI"));
            foreach(int at in new[]{aiRow+4,aiRow+8}.Concat(Enumerable.Range(0,9).Select(i=>aiTable+i*4))){var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^1u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets,15,2,2));metadataFailures++;}
            for(int i=0;i<2;i++){int table=(int)Word(good.InstanceBuffer,aiTable+20+i*12);for(int at=table;at<table+552;at+=4){var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^1u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets,15,2,2));metadataFailures++;}}
            Console.WriteLine($"Shared GameObject AI: PASS ({cases} actual-Core combinations, all pointed-root masks and six behavior orders, AI counts 0/1/2, up to 100 partial root fields; {nativeFailures} native/{sourceFailures} source/{metadataFailures} detached metadata refusals; local Body/behavior/AI scope and named references; empty AI contact/vision lists only, no general object stream/package/game).");
            //-------------------------------------------------------------------------------------------------
            /** Reborn: schema-valid raw source must precede actual Core normalization and unique stock/authored dependency identity admission. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Compile(string source,ManifestAsset[] admitted)
            {
                if(source.Contains('\\') || source.Contains("TypeId",StringComparison.Ordinal))throw new InvalidDataException("Authored shared AI compiler indices/TypeId refused.");var raw=Xml(source);GameObjectBodyProjectionSmokeTest.Validate(official,GameObjectCombatCompositionSmokeTest.Adapt(raw.DocumentElement!,true).OuterXml);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var r in instance.ReferencedInstances)if(admitted.Count(a=>a.TypeId==r.TypeId && a.InstanceId==r.InstanceId)!=1)throw new InvalidDataException("Shared AI dependency missing/wrong/ambiguous.");return(GameObjectBodyProjectionSmokeTest.Compile(GameObjectCombatCompositionSmokeTest.Adapt(instance.XmlNode,true),selectedBehaviors:true),instance);
            }
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: appended focused AI leaves retain one Core occurrence table; adaptation restores actual official AI/Body/root-child source order. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(ManifestAsset[] targets,int mask,int profile,int count,bool reverse,int position)
    {
        string source=GameObjectPointerRootSmokeTest.Source(targets,mask,profile,reverse,position).Replace("RootPointerProbe","RootAiProbe",StringComparison.Ordinal);string children=Ep1VisionInfoSmokeTest.Vision(profile);
        for(int i=0;i<count;i++){string text=profile==0?"":profile==1?" AttackPriority=''":" AttackPriority='AI &amp; &lt;priority&gt; "+new string('R',80)+"'";children+=$"<AIUpdate id='AI{i}' StateMachine='{(i==0?"UnitAIStateMachine":"StructureAIStateMachine")}' DefaultLocomotorSet='{(profile==2?"SPECIALABILITY_ACTIVE":"NORMAL")}' StandGround='{(profile==2?"true":"false")}' FadeOnPortals='{(profile==1?"true":"false")}'"+text+">"+(HasChooser(profile,i)?"<UnitAITargetChooserData/>":"")+"</AIUpdate>";}
        return source.Insert(source.LastIndexOf("</RootAiProbe>",StringComparison.Ordinal),children);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: profile zero has no chooser, profile one has only the first, and profile two has all, exercising mixed two-module ownership. */
    //-------------------------------------------------------------------------------------------------
    private static bool HasChooser(int profile,int index)=>profile==2 || profile==1 && index==0;
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all older independent graph checks remain active; AI list/class fragments are separately inverted into their reviewed local coordinates. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,ManifestAsset[] targets,int mask,int profile,int count)
    {
        var b=output.InstanceBuffer;var n=native.InstanceBuffer;var roots=GameObjectStockReview.Tokens(b,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140,items=(int)Word(n,484);var extra=new List<int>();int metadata=count==0?0:12+count*12;for(int i=0;i<count;i++){metadata+=HasChooser(profile,i)?552:348;if(HasChooser(profile,i)){int module=(int)Word(n,items+i*4);extra.Add((int)Word(n,module+68)+36);}}
        Ep1VisionInfoSmokeTest.Verify(output,native,instance,targets,mask,profile,true,count==0?0:1,metadata,extra);if(Word(n,480)!=count)throw new InvalidDataException("Shared AI native count differs.");if(count==0){if(items!=0 || roots.ContainsKey(FastHash.GetHashCode("AI")))throw new InvalidDataException("Absent shared AI retains storage/metadata.");return;}
        var field=roots[FastHash.GetHashCode("AI")];if(field.Type!=0x5FD8691D || field.Offset!=prefix-metadata || Word(b,field.Offset)!=0x6C827073 || Word(b,field.Offset+4)!=0x5D448FD8 || Word(b,field.Offset+8)!=prefix+480 || Word(b,prefix+484)!=prefix+items)throw new InvalidDataException("Shared AI root/list count binding differs.");int next=field.Offset+12+count*12,reference=instance.ReferencedInstances.Count-extra.Count;
        for(int i=0;i<count;i++)
        {
            int start=(int)Word(n,items+i*4),chooser=(int)Word(n,start+68),fragmentPrefix=HasChooser(profile,i)?552:348;if(Word(b,field.Offset+12+i*12)!=0xB151A0EB || Word(b,field.Offset+16+i*12)!=0 || Word(b,field.Offset+20+i*12)!=next || Word(b,prefix+items+i*4)!=prefix+start || Word(n,start)!=0x08DBC00E || Word(n,start+4)!=FastHash.GetHashCode("ai"+i) || Word(n,start+60)!=(i==0?7u:6u) || Word(n,start+64)!=(profile==2?18u:0u) || n[start+72]!=(profile==2?1:0) || n[start+73]!=(profile==1?1:0) || (chooser!=0)!=HasChooser(profile,i))throw new InvalidDataException("Shared AI concrete identity/state/default fields differ.");string text=ReadText(n,start+8);if(text!=(profile==2?"AI & <priority> "+new string('R',80):""))throw new InvalidDataException("Shared AI text literal differs.");
            if(chooser!=0){var r=instance.ReferencedInstances[reference];if(r.TypeId!=targets[9].TypeId || r.InstanceId!=targets[9].InstanceId || Word(b,prefix+chooser+36)!=reference+1)throw new InvalidDataException("Shared AI named occurrence identity/index differs.");reference++;}
            int end=start+76,stringPointer=(int)Word(n,start+12),stringLength=(int)Word(n,start+8);if(stringPointer!=0)end=Math.Max(end,stringPointer+stringLength+1);if(chooser!=0)end=Math.Max(end,chooser+40);end=(end+3)&~3;
            var original=Extract(native,start,end,0);var local=Extract(output,prefix+start,prefix+end,fragmentPrefix);b.AsSpan(next,fragmentPrefix).CopyTo(local.InstanceBuffer);for(int at=8;at<fragmentPrefix;at+=12){uint target=Word(local.InstanceBuffer,at);if(target==0)continue;if(target>=next && target<next+fragmentPrefix)Put(local.InstanceBuffer,at,target-(uint)next);else if(target>=prefix+start && target<prefix+end)Put(local.InstanceBuffer,at,target-(uint)(prefix+start)+(uint)fragmentPrefix);else throw new InvalidDataException("Shared AI metadata crosses fragment owners.");}Ep1AIUpdateTokenSmokeTest.Verify(local,original);next+=fragmentPrefix;
        }
        if(next!=prefix || reference!=instance.ReferencedInstances.Count)throw new InvalidDataException("Shared AI final metadata/occurrence extent differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source/output fragment extraction changes only owned pointer addresses and auxiliary coordinates, never strong occurrence words. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk input,int start,int end,int prefix)
    {
        var c=new Chunk{InstanceBuffer=new byte[prefix+end-start],RelocationBuffer=Pack(Slots(input.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start+(uint)prefix)),ImportsBuffer=Pack(Slots(input.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start+(uint)prefix))};input.InstanceBuffer.AsSpan(start,end-start).CopyTo(c.InstanceBuffer.AsSpan(prefix));foreach(uint slot in Slots(c.RelocationBuffer)){uint target=Word(c.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Shared AI extraction cross-owner pointer.");Put(c.InstanceBuffer,(int)slot,target-(uint)start+(uint)prefix);}return c;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached root-row mutations find the actual field without assuming a future descriptor ordinal. */
    //-------------------------------------------------------------------------------------------------
    private static int FindRow(byte[] b,uint hash){for(int at=0;at<b.Length-12;at+=12){if(Word(b,at)==hash)return at;if(Word(b,at)==0 && Word(b,at+4)==0 && Word(b,at+8)==0)break;}throw new InvalidDataException("Shared AI root row missing.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only valid linked EP1 catalogs supply admitted named target identities. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Shared AI catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source/native/metadata failures remain explicit rather than stripped partial output. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or FormatException or OverflowException or BinaryAssetBuilderException){return;}throw new InvalidDataException("Invalid shared AI graph admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native strings are bounded headers; this is not Unicode/localization-table admission. */
    //-------------------------------------------------------------------------------------------------
    private static string ReadText(byte[] b,int at){uint length=Word(b,at),pointer=Word(b,at+4);if(pointer==0){if(length!=0)throw new InvalidDataException("Shared AI text presence differs.");return "";}if(pointer>=b.Length || length>b.Length-pointer-1 || b[(int)(pointer+length)]!=0)throw new InvalidDataException("Shared AI text bounds differ.");return Encoding.ASCII.GetString(b,(int)pointer,(int)length);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled source parsing does not resolve external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument{XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: corruption controls privately own every mutated buffer. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary decoding independently requires empty or sentinel-framed word storage. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Shared AI auxiliary framing differs.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation/extraction buffers preserve slot order and one trailing sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all native/metadata reads are bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Shared AI word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned expected/mutation buffers receive word writes. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
