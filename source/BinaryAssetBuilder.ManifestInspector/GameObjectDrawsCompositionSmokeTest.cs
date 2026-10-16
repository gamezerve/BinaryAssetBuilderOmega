using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: join all four complete Conscript Draws with the selected root/armor/movement/Body/three-behavior/common-AI/geometry graph on one Core table and Tracker.
internal static class GameObjectDrawsCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test pointed-root/AI combinations, six behavior orders, absent/empty/subset/full Draws, authored edits and cross-container local identities together. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(SchemaSet schemas,string manifest,ManifestAsset[] catalog,XmlElement[] leaves,Chunk baseline,InstanceDeclaration baselineInstance,byte[] expectedDraws)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var tag=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("Draws")];if(tag.Type!=0xEFBBA791)throw new InvalidDataException("Independent root Draws wire tag differs.");
        var targets=new[]{"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"}.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).Concat(new[]{catalog.Single(a=>a.Name=="TargetingCompareList:DefaultTargetingCompareList")}).ToArray();
        int cases=0,maximumFields=0,maximumImports=0;Chunk? full=null;InstanceDeclaration? fullInstance=null;string? fullSource=null;
        foreach(int mask in new[]{0,15})foreach(int profile in new[]{0,2})foreach(int ai in new[]{0,2})
        {
            string source=Source(mask,profile,ai,cases%2!=0,cases%3,new[]{0,1,2,3});var pair=Build(source);var output=Ep1GameObjectDestructionProjection.Compose(pair.Native);Verify(output,pair.Native,pair.Instance,expectedDraws,baseline,baselineInstance,false);
            var repeat=Build(source);var other=Ep1GameObjectDestructionProjection.Compose(repeat.Native);if(!output.InstanceBuffer.SequenceEqual(other.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(other.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(other.ImportsBuffer))throw new InvalidDataException("Root/Draws repeat differs.");
            maximumFields=Math.Max(maximumFields,GameObjectStockReview.Tokens(output.InstanceBuffer,0).Count);maximumImports=Math.Max(maximumImports,pair.Instance.ReferencedInstances.Count);
            if(mask==15 && profile==2 && ai==2){full=pair.Native;fullInstance=pair.Instance;fullSource=source;}cases++;
        }
        foreach(int[]? order in new int[]?[]{null,Array.Empty<int>(),new[]{0,1},new[]{2,3}})
        {var pair=Build(Source(15,2,2,false,0,order));Verify(Ep1GameObjectDestructionProjection.Compose(pair.Native),pair.Native,pair.Instance,order?.Length==4?expectedDraws:null,null,null,false);cases++;}
        var edit=Xml(fullSource!);int id=0;foreach(XmlElement draw in edit.GetElementsByTagName("Draws",SchemaSet.XmlNamespace)[0]!.ChildNodes.OfType<XmlElement>()){draw.SetAttribute("id","RebornRootDraw"+id++);draw.SetAttribute(draw.LocalName=="ScriptedModelDraw"?"MaxRecoilDistance":"Width","17.5");}
        var destruction=(XmlElement)edit.GetElementsByTagName("BuildingDestructionBehavior",SchemaSet.XmlNamespace)[0]!;destruction.SetAttribute("AssociatedModule","RebornRootDraw0");var edited=Build(edit.OuterXml);Verify(Ep1GameObjectDestructionProjection.Compose(edited.Native),edited.Native,edited.Instance,expectedDraws,baseline,baselineInstance,true);cases++;
        int drawList=(int)Word(edited.Native.InstanceBuffer,472),drawItems=(int)Word(edited.Native.InstanceBuffer,drawList+4);for(int i=0;i<4;i++){int at=(int)Word(edited.Native.InstanceBuffer,drawItems+i*4);if(Word(edited.Native.InstanceBuffer,at+4)!=FastHash.GetHashCode(("RebornRootDraw"+i).ToLowerInvariant()) || Word(edited.Native.InstanceBuffer,at+(i<2?20:16))!=0x418C0000)throw new InvalidDataException("Root Draws authored edit not applied.");}
        int behaviorList=(int)Word(edited.Native.InstanceBuffer,476),destroy=Enumerable.Range(0,3).Select(i=>(int)Word(edited.Native.InstanceBuffer,behaviorList+8+i*4)).Single(at=>Word(edited.Native.InstanceBuffer,at)==0x0D96A2F4);if(Word(edited.Native.InstanceBuffer,destroy+52)!=FastHash.GetHashCode("rebornrootdraw0"))throw new InvalidDataException("Forward destruction-to-Draw link differs.");
        int nativeFaults=0,sourceFaults=0,detachedFaults=0;int list=(int)Word(full!.InstanceBuffer,472),items=(int)Word(full.InstanceBuffer,list+4),first=(int)Word(full.InstanceBuffer,items),body=(int)Word(full.InstanceBuffer,488),aiItems=(int)Word(full.InstanceBuffer,484),aiStart=(int)Word(full.InstanceBuffer,aiItems);
        foreach(var fault in new[]{(472,Word(full.InstanceBuffer,484)),(472,Word(full.InstanceBuffer,424)),(list,65u),(list+4,(uint)(list+12)),(items,(uint)aiStart),(first+4,Word(full.InstanceBuffer,body+4)),(first+4,Word(full.InstanceBuffer,aiStart+4)),(first+168,(uint)body),(528,(uint)first)}){var bad=Clone(full);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFaults++;}
        foreach(uint slot in new[]{472u,(uint)(list+4),(uint)items}){var bad=Clone(full);bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Where(s=>s!=slot));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));nativeFaults++;}
        // Reborn: native callers cannot evade cross-class behavior/Draw identity scope after bypassing source validation.
        var behaviorCollision=Clone(full);int behaviorItems=(int)Word(full.InstanceBuffer,(int)Word(full.InstanceBuffer,476)+4);Put(behaviorCollision.InstanceBuffer,first+4,Word(full.InstanceBuffer,(int)Word(full.InstanceBuffer,behaviorItems)+4));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(behaviorCollision));nativeFaults++;
        foreach(string bad in new[]{fullSource!.Replace("id=\"ModuleTag_Draw\"","id=\"bOdY\"",StringComparison.Ordinal),fullSource.Replace("id=\"ModuleTag_Draw\"","id=\"AI0\"",StringComparison.Ordinal),fullSource.Replace("id=\"ModuleTag_Draw\"","id=\"Destroy\"",StringComparison.Ordinal),fullSource.Replace("Texture=\"FXTracer\"","Texture=\"RebornMissingRootTexture\"",StringComparison.Ordinal),fullSource.Replace("AssociatedModule=\"Body\"","AssociatedModule=\"MissingRootDraw\"",StringComparison.Ordinal)}){Refuse(()=>{var pair=Build(bad);Ep1GameObjectDestructionProjection.Compose(pair.Native);});sourceFaults++;}
        var good=Ep1GameObjectDestructionProjection.Compose(full);int rootRow=FindRow(good.InstanceBuffer,FastHash.GetHashCode("Draws")),table=GameObjectStockReview.Tokens(good.InstanceBuffer,0)[FastHash.GetHashCode("Draws")].Offset;
        foreach(int at in new[]{rootRow+4,rootRow+8}.Concat(Enumerable.Range(0,expectedDraws.Length/4).Select(i=>table+i*4))){var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^1u);Refuse(()=>Verify(bad,full,fullInstance!,expectedDraws,baseline,baselineInstance,false));detachedFaults++;}
        int globalPrefix=GameObjectStockReview.Tokens(good.InstanceBuffer,0)[FastHash.GetHashCode("BuildTime")].Offset-140;
        foreach(int at in new[]{472,140,first+4,aiStart+4,body+44}){var bad=Clone(good);Put(bad.InstanceBuffer,globalPrefix+at,Word(bad.InstanceBuffer,globalPrefix+at)^1u);Refuse(()=>Verify(bad,full,fullInstance!,expectedDraws,baseline,baselineInstance,false));detachedFaults++;}
        foreach(bool relo in new[]{false,true}){var bad=Clone(good);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad,full,fullInstance!,expectedDraws,baseline,baselineInstance,false));detachedFaults++;}
        foreach(string field in new[]{"Body","Behaviors","AI"}){var bad=Clone(good);int at=FindRow(bad.InstanceBuffer,FastHash.GetHashCode(field))+8;Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^1u);Refuse(()=>Verify(bad,full,fullInstance!,expectedDraws,baseline,baselineInstance,false));detachedFaults++;}
        Console.WriteLine($"Shared selected root/Draws: PASS ({cases} source cases, up to {maximumFields} partial root fields/{maximumImports} shared Core imports, complete 4-Draw/10-model/84-animation graph, unchanged independent {expectedDraws.Length}-byte Draws oracle; {nativeFaults} native/{sourceFaults} source/{detachedFaults} detached refusals, Body/behavior/AI/Draw local identity and forward links; no complete GameObject stream, remaining-module closure, XInclude execution, package or gameplay claim).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve the existing selected root source and official child order while adding the original complete unnormalized Draws leaves. */
        //-------------------------------------------------------------------------------------------------
        string Source(int mask,int profile,int ai,bool reverse,int position,int[]? order)
        {
            var source=Xml(GameObjectAiCompositionSmokeTest.Source(targets,mask,profile,ai,reverse,position));var root=GameObjectCombatCompositionSmokeTest.Adapt(source.DocumentElement!,true);root.DocumentElement!.SetAttribute("id","RebornSelectedDrawRoot");
            foreach(XmlElement chooser in root.GetElementsByTagName("UnitAITargetChooserData",SchemaSet.XmlNamespace))chooser.SetAttribute("TargetingCompareList","DefaultTargetingCompareList");
            if(order!=null){var draws=root.CreateElement("Draws",SchemaSet.XmlNamespace);foreach(int index in order){var leaf=root.CreateElement(index<2?"ScriptedModelDraw":"TracerModelDraw",SchemaSet.XmlNamespace);foreach(XmlAttribute a in leaves[index].Attributes)if(a.Name!="xmlns" && a.Prefix!="xmlns")leaf.SetAttribute(a.LocalName,a.Value);foreach(XmlNode child in leaves[index].ChildNodes)leaf.AppendChild(root.ImportNode(child,true));draws.AppendChild(leaf);}root.DocumentElement.InsertBefore(draws,root.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]);}return root.OuterXml;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all official root and Draw source names pass one production Core normalization; explicit EP1 allocation uses only this same occurrence table and Tracker. */
        //-------------------------------------------------------------------------------------------------
        (Chunk Native,InstanceDeclaration Instance) Build(string source)
        {var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Root/Draw dependency missing/ambiguous.");return(GameObjectBodyProjectionSmokeTest.Compile(Xml(instance.XmlNode.OuterXml),selectedBehaviors:true),instance);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: root bindings, all shared occurrence words and inverse native/auxiliary bytes are independent of producer reexecution; Draw metadata is normalized back to the complete stock oracle. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,byte[]? expectedDraws,Chunk? baseline,InstanceDeclaration? baselineInstance,bool edited)
    {
        var roots=GameObjectStockReview.Tokens(output.InstanceBuffer,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140;var relos=Slots(native.RelocationBuffer);var imports=Slots(native.ImportsBuffer);
        foreach(var field in Ep1GameObjectRootValueTokens.AllFields)if(!roots.TryGetValue(FastHash.GetHashCode(field.Name),out var token) || token.Type!=field.Type || token.Offset!=prefix+field.Offset)throw new InvalidDataException("Root/Draw direct root field differs.");
        if(!Slots(output.RelocationBuffer).SequenceEqual(relos.Select(s=>s+(uint)prefix)) || !Slots(output.ImportsBuffer).SequenceEqual(imports.Select(s=>s+(uint)prefix)) || !imports.Select(s=>Word(native.InstanceBuffer,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Root/Draw shared occurrences/auxiliary origins differ.");
        var recovered=output.InstanceBuffer.AsSpan(prefix).ToArray();foreach(uint slot in relos){uint target=Word(recovered,(int)slot);if(target!=Word(native.InstanceBuffer,(int)slot)+prefix)throw new InvalidDataException("Root/Draw pointer rebase differs.");Put(recovered,(int)slot,target-(uint)prefix);}if(!recovered.SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("Root/Draw inverse native payload differs.");
        // Reborn: adding a root row must not move Body/behavior/AI metadata onto sibling tables; bind every concrete wrapper back to its actual native ModuleData identity.
        var body=roots[FastHash.GetHashCode("Body")];if(body.Type!=0xAFAD56F4)throw new InvalidDataException("Root/Draw Body tag differs.");BindModule(body.Offset,(int)Word(native.InstanceBuffer,488));
        BindList("Behaviors",0x24FCCAE2,(int)Word(native.InstanceBuffer,476));
        if(Word(native.InstanceBuffer,480)!=0)BindList("AI",0x5FD8691D,480);else if(roots.ContainsKey(FastHash.GetHashCode("AI")))throw new InvalidDataException("Empty root AI retains metadata.");
        int list=(int)Word(native.InstanceBuffer,472);if(list==0){if(roots.ContainsKey(FastHash.GetHashCode("Draws")))throw new InvalidDataException("Absent Draws retains metadata.");return;}
        var drawField=roots[FastHash.GetHashCode("Draws")];int table=drawField.Offset;if(drawField.Type!=0xEFBBA791 || Word(output.InstanceBuffer,table)!=0x6C827073 || Word(output.InstanceBuffer,table+4)!=0x5D448FD8 || Word(output.InstanceBuffer,table+8)!=prefix+list)throw new InvalidDataException("Root Draws count/table binding differs.");
        if(expectedDraws==null)return;int drawLength=baseline!.InstanceBuffer.Length,drawPrefix=expectedDraws.Length;if(table+drawPrefix!=prefix || Word(native.InstanceBuffer,list)!=4)throw new InvalidDataException("Root Draws metadata/native extent differs.");
        var metadata=output.InstanceBuffer.AsSpan(table,drawPrefix).ToArray();for(int at=8;at<drawPrefix;at+=12){uint target=Word(metadata,at);if(target==0)continue;Put(metadata,at,target>=table && target<table+drawPrefix?target-(uint)table:target>=prefix+list && target<prefix+list+drawLength?target-(uint)(prefix+list)+(uint)drawPrefix:throw new InvalidDataException("Root Draws metadata escapes owners."));}if(!metadata.SequenceEqual(expectedDraws))throw new InvalidDataException("Independent root-composed Draws metadata differs.");
        if(edited)return;var bytes=native.InstanceBuffer.AsSpan(list,drawLength).ToArray();var baselineSlots=Slots(baseline.RelocationBuffer);foreach(uint slot in baselineSlots)Put(bytes,(int)slot,Word(bytes,(int)slot)-(uint)list);
        foreach(uint slot in Slots(baseline.ImportsBuffer)){uint current=Word(bytes,(int)slot),original=Word(baseline.InstanceBuffer,(int)slot);if(current==0 || current>instance.ReferencedInstances.Count || original==0 || original>baselineInstance!.ReferencedInstances.Count)throw new InvalidDataException("Root Draws reference index escapes common table.");var a=instance.ReferencedInstances[(int)current-1];var b=baselineInstance.ReferencedInstances[(int)original-1];if(a.TypeId!=b.TypeId || a.InstanceId!=b.InstanceId)throw new InvalidDataException("Root Draws reference semantic identity changed.");Put(bytes,(int)slot,original);}if(!bytes.SequenceEqual(baseline.InstanceBuffer))throw new InvalidDataException("Complete root-composed Draws native graph differs from stock-proven standalone graph.");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: concrete wrapper/base chains must end at native TypeId/id fields, never a neighboring module or metadata origin. */
        //-------------------------------------------------------------------------------------------------
        void BindModule(int table,int nativeStart)
        {
            var b=output.InstanceBuffer;if(Word(b,table)!=2 || Word(b,table+4)!=Word(native.InstanceBuffer,nativeStart) || Word(b,table+8)!=table+12)throw new InvalidDataException("Root sibling concrete binding differs.");table+=12;int depth=0;
            while(Word(b,table)==1){if(++depth>16)throw new InvalidDataException("Root sibling base chain exceeds bound.");table=(int)Word(b,table+8);}
            var terminal=GameObjectStockReview.Tokens(b,table);if(!terminal.TryGetValue(FastHash.GetHashCode("TypeId"),out var type) || !terminal.TryGetValue(FastHash.GetHashCode("id"),out var id) || type.Offset!=prefix+nativeStart || id.Offset!=prefix+nativeStart+4)throw new InvalidDataException("Root sibling native identity target differs.");
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: list headers/items independently bind both pointer-based Behaviors and inline AI to their exact native count and concrete owners. */
        //-------------------------------------------------------------------------------------------------
        void BindList(string name,uint tag,int countAt)
        {
            var field=roots[FastHash.GetHashCode(name)];var b=output.InstanceBuffer;int count=(int)Word(native.InstanceBuffer,countAt),items=(int)Word(native.InstanceBuffer,countAt+4);
            if(field.Type!=tag || Word(b,field.Offset)!=0x6C827073 || Word(b,field.Offset+4)!=0x5D448FD8 || Word(b,field.Offset+8)!=prefix+countAt)throw new InvalidDataException("Root sibling list binding differs.");
            for(int i=0;i<count;i++){int marker=field.Offset+12+i*12;if(Word(b,marker)!=0xB151A0EB || Word(b,marker+4)!=0)throw new InvalidDataException("Root sibling list marker differs.");BindModule((int)Word(b,marker+8),(int)Word(native.InstanceBuffer,items+i*4));}
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached root row mutations find the actual field rather than assuming a future root descriptor ordinal. */
    //-------------------------------------------------------------------------------------------------
    private static int FindRow(byte[] bytes,uint name){for(int at=0;at<bytes.Length-12;at+=12){if(Word(bytes,at)==name)return at;if(Word(bytes,at)==0 && Word(bytes,at+4)==0 && Word(bytes,at+8)==0)break;}throw new InvalidDataException("Root Draws row missing.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative controls must fail explicitly without discarding malformed owners or unresolved identities. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or FormatException or OverflowException){return;}throw new InvalidDataException("Invalid root/Draws graph accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled source parsing never resolves external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string source){var xml=new XmlDocument{XmlResolver=null};xml.LoadXml(source);return xml;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every mutation owns all native/output buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary framing and duplicate slots must be checked before coordinate inversion. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Root Draws auxiliary framing differs.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Root Draws auxiliary duplicates.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain ordered Tracker slots and exactly one sentinel for nonempty streams. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.ToArray();if(values.Length==0)return Array.Empty<byte>();var bytes=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(bytes,i*4,values[i]);Put(bytes,values.Length*4,uint.MaxValue);return bytes;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all metadata/native reads are bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int at){if(at<0 || at>bytes.Length-4)throw new InvalidDataException("Root Draws read out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned expected/output/control buffers are patched. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
}
