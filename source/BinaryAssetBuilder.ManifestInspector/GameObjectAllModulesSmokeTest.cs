using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: all 49 original Conscript module graphs share a selected partial root, one actual Core occurrence table and one Tracker; this is not the complete original root or asset publication.
internal static class GameObjectAllModulesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain all original behavior/Body/AI/client sources and complete previously captured Draws, then independently verify every enclosing metadata/native owner. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(SchemaSet schemas,string sourceRoot,string manifest,ManifestAsset[] catalog,XmlElement[] draws,bool includeRootOwners=false,bool includeEmptyRoots=false,bool includeOriginalRoot=false,string? originalGlobalManifest=null)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name);var paths=new[]{"BaseObjects/BaseObject.xml","BaseObjects/BaseInfantry.xml","Soviet/Units/SovietAntiInfantryInfantry.xml","GlobalData/GlobalDefines.xml"};var captures=paths.Select(p=>SdkEnvironmentPreflight.Read(Path.Combine(sourceRoot,p),1048576)).ToArray();var documents=captures.Select(b=>Xml(Encoding.UTF8.GetString(b).TrimStart('\uFEFF'))).ToArray();var objects=documents.Take(3).Select(d=>d.GetElementsByTagName("GameObject",SchemaSet.XmlNamespace).OfType<XmlElement>().Single()).ToArray();
        if(objects[0].HasAttribute("inheritFrom") || objects[1].GetAttribute("inheritFrom")!="BaseObject" || objects[2].GetAttribute("inheritFrom")!="BaseInfantry")throw new InvalidDataException("Explicit all-module source chain changed.");string scaler=documents[3].GetElementsByTagName("Define",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.GetAttribute("name")=="MAX_SECONDARY_DAMAGE_SCALER").GetAttribute("value");if(scaler!="200%")throw new InvalidDataException("Selected all-module scaler drift.");
        var targets=new[]{"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"}.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).ToArray();var document=GameObjectCombatCompositionSmokeTest.Adapt(Xml(GameObjectAiCompositionSmokeTest.Source(targets,15,2,1,false,0)).DocumentElement!,true);document.DocumentElement!.SetAttribute("id","RebornAllOriginalModules");
        foreach(string name in new[]{"Draws","Behaviors","AI","Body","ClientUpdates","ClientBehaviors"})
        {
            var container=document.CreateElement(name,SchemaSet.XmlNamespace);IEnumerable<XmlElement> leaves=name=="Draws"?draws:name is "Body" or "AI"?objects[2].ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName==name).ChildNodes.OfType<XmlElement>():objects.SelectMany(o=>o.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name).SelectMany(e=>e.ChildNodes.OfType<XmlElement>()));
            // Reborn: the two standalone scripted aliases regain their actual GameObject element names without losing any source members.
            int leafIndex=0;foreach(var leaf in leaves){var copy=document.CreateElement(name=="Draws" && leafIndex++<2?"ScriptedModelDraw":leaf.LocalName,SchemaSet.XmlNamespace);foreach(XmlAttribute a in leaf.Attributes)if(a.Name!="xmlns" && a.Prefix!="xmlns")copy.SetAttribute(a.LocalName,a.Value);foreach(XmlNode child in leaf.ChildNodes)copy.AppendChild(document.ImportNode(child,true));if(name=="Behaviors")ConscriptBehaviorSurvey.Prepare(copy,scaler);container.AppendChild(copy);}
            var previous=document.DocumentElement.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==name);if(previous!=null)document.DocumentElement.ReplaceChild(container,previous);else document.DocumentElement.InsertBefore(container,document.GetElementsByTagName(name=="Draws"?"Behaviors":"Geometry",SchemaSet.XmlNamespace)[0]);
        }
        // Reborn: retain the effective official chooser default explicitly, as in the existing client/root gate; no captured occurrence index is inserted.
        foreach(XmlElement chooser in document.GetElementsByTagName("UnitAITargetChooserData",SchemaSet.XmlNamespace))if(!chooser.HasAttribute("TargetingCompareList"))chooser.SetAttribute("TargetingCompareList","DefaultTargetingCompareList");
        // Reborn: snapshot before invoking the producer, so source-buffer mutation cannot hide behind a post-composition baseline.
        var pair=Build(document);var baseline=Clone(pair.Native);var output=Ep1GameObjectDestructionProjection.Compose(pair.Native,schemas.Schemas);int modulesVerified=Verify(output,pair.Native,pair.Instance);var repeat=Build(document);if(!Equal(pair.Native,baseline) || !Equal(output,Ep1GameObjectDestructionProjection.Compose(repeat.Native,schemas.Schemas)))throw new InvalidDataException("All-module repeat/input isolation differs.");int refusals=0;
        // Reborn: the previous selected/default profile must not silently admit the newly expanded behavior inventory.
        Refuse(()=>Ep1GameObjectDestructionProjection.Compose(pair.Native));Refuse(()=>GameObjectBodyProjectionSmokeTest.Compile(Xml(pair.Instance.XmlNode.OuterXml),selectedBehaviors:true));refusals+=2;
        int behavior=(int)Word(pair.Native.InstanceBuffer,476),body=(int)Word(pair.Native.InstanceBuffer,488),client=(int)Word(pair.Native.InstanceBuffer,496);foreach(var fault in new (int At,uint Value)[]{(behavior,35),(476,(uint)client),(496,(uint)behavior),(488,(uint)(behavior+8)),(body+4,Word(pair.Native.InstanceBuffer,(int)Word(pair.Native.InstanceBuffer,behavior+8)+4))}){var bad=Clone(pair.Native);Put(bad.InstanceBuffer,fault.At,fault.Value);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad,schemas.Schemas));refusals++;}
        var roots=GameObjectStockReview.Tokens(output.InstanceBuffer,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140;foreach(string name in new[]{"Behaviors","Draws","AI","Body","ClientUpdates","ClientBehaviors"}){int row=FindRow(output.InstanceBuffer,FastHash.GetHashCode(name));foreach(int at in new[]{row+4,row+8,roots[FastHash.GetHashCode(name)].Offset+4,roots[FastHash.GetHashCode(name)].Offset+8}){var bad=Clone(output);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(bad,pair.Native,pair.Instance));refusals++;}}
        foreach(int at in new[]{472,476,480,488,492,496}){var bad=Clone(output);bad.InstanceBuffer[prefix+at]^=1;Refuse(()=>Verify(bad,pair.Native,pair.Instance));refusals++;}foreach(bool relo in new[]{true,false}){var bad=Clone(output);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad,pair.Native,pair.Instance));refusals++;}
        foreach(string name in new[]{"Body","AI","ClientUpdates","ClientBehaviors"}){var bad=Xml(document.OuterXml);var leaf=bad.GetElementsByTagName(name,SchemaSet.XmlNamespace)[0]!.ChildNodes.OfType<XmlElement>().First();leaf.SetAttribute("id",bad.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!.ChildNodes.OfType<XmlElement>().First().GetAttribute("id"));Refuse(()=>Build(bad));refusals++;}
        // Reborn: a separate strict route joins the original six populated owners while retaining this existing independent all-module oracle.
        if(includeRootOwners)GameObjectRootOwnersSmokeTest.Run(schemas,document,objects,documents[3],manifest,catalog,(output,native,instance)=>Verify(output,native,instance,false),includeEmptyRoots);
        // Reborn: actual original root/value qualification is separate from synthetic-root metadata compatibility and does not broaden default dispatch.
        if(includeOriginalRoot)GameObjectOriginalNativeSmokeTest.Run(schemas,document,objects,documents[3],manifest,catalog,originalGlobalManifest??throw new InvalidDataException("Original Conscript global stream mapping absent."));
        for(int i=0;i<paths.Length;i++)if(!SHA256.HashData(captures[i]).SequenceEqual(SHA256.HashData(SdkEnvironmentPreflight.Read(Path.Combine(sourceRoot,paths[i]),1048576))))throw new InvalidDataException("Official all-module source changed.");
        Console.WriteLine($"All original Conscript modules: PASS ({modulesVerified}/49 full native graphs and stock-bound metadata on one Core table/{pair.Instance.ReferencedInstances.Count} occurrences/one Tracker; {roots.Count} selected partial-root fields; BIN/RELO/IMP={output.InstanceBuffer.Length}/{output.RelocationBuffer.Length}/{output.ImportsBuffer.Length}, prefix={prefix}; {refusals} default-profile/ownership/source/detached refusals; no complete original root, general inheritance, GameObject asset stream/package/game or alpha expansion).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: schema admission and global module-ID scope precede one actual normalization and the explicit reviewed-family allocator. */
        //-------------------------------------------------------------------------------------------------
        (Chunk Native,InstanceDeclaration Instance) Build(XmlDocument input)
        {var raw=Xml(input.OuterXml);GameObjectBodyProjectionSmokeTest.Validate(schemas.Schemas,raw.OuterXml);raw.Schemas.Add(schemas.Schemas);raw.Validate((_,e)=>throw new InvalidDataException(e.Message));Ep1SelectedModuleIdentity.ValidateSource(raw.DocumentElement!,reviewedBehaviors:true);ConscriptBehaviorSurvey.ResolveStrong(raw,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,raw.OuterXml);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("All-module dependency missing/ambiguous: "+reference.Name);return(GameObjectBodyProjectionSmokeTest.Compile(Xml(instance.XmlNode.OuterXml),selectedBehaviors:true,reviewedBehaviors:true),instance);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: every original module graph and metadata target is checked independently; whole native/auxiliary inversion and exact Core consumption precede per-container comparisons. */
        //-------------------------------------------------------------------------------------------------
        int Verify(Chunk result,Chunk native,InstanceDeclaration instance,bool allOccurrences=true)
        {
            var n=native.InstanceBuffer;var b=result.InstanceBuffer;var root=GameObjectStockReview.Tokens(b,0);int shift=root[FastHash.GetHashCode("BuildTime")].Offset-140;BehaviorContainerStockProof.VerifyTail(result,native,shift);if(allOccurrences && !Slots(native.ImportsBuffer).Select(s=>Word(n,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("All-module common occurrence consumption differs.");foreach(var field in Ep1GameObjectRootValueTokens.AllFields)if(!root.TryGetValue(FastHash.GetHashCode(field.Name),out var token) || token.Type!=field.Type || token.Offset!=shift+field.Offset)throw new InvalidDataException("All-module root field binding differs.");
            var layout=Ep1GameObjectStringTokens.Inspect(n,Slots(native.RelocationBuffer),Slots(native.ImportsBuffer),reviewedBehaviors:true);if(layout.Modules.Count!=36 || layout.Draws.Modules.Count!=4 || layout.Ai.Modules.Count!=1 || layout.Clients.Sum(c=>c.Modules.Count)!=7 || layout.Body==0)throw new InvalidDataException("All 49 original module owners required.");
            foreach(string name in new[]{"Behaviors","Draws","AI","Body","ClientUpdates","ClientBehaviors"})if(!root.TryGetValue(FastHash.GetHashCode(name),out var token) || token.Type!=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode(name)].Type)throw new InvalidDataException("All-module root container wire differs.");
            var behaviors=Extract(native,layout.List,layout.Body);var observed=Detach(result,shift,root[FastHash.GetHashCode("Behaviors")].Offset,17052,layout.List,layout.Body);BehaviorContainerStockProof.Run(behaviors,schemas.Schemas,topology.Containers.Single(c=>c.Name=="Behaviors").Modules,stock.Bytes,stock.Asset.References.ToArray(),instance,(id,_)=>Ep1BehaviorMetadataTokenizer.ReviewedNativeType(id),observed,controls:false);
            Check("Draws",layout.Draws.Start,layout.Draws.End,layout.Draws.Modules.ToArray(),new[]{typeof(W3DScriptedModelDrawModuleData),typeof(W3DScriptedModelDrawModuleData),typeof(W3DTracerDrawModuleData),typeof(W3DTracerDrawModuleData)},58608,layout.Draws.Start);
            Check("AI",layout.Ai.Items,layout.Ai.Modules[0].End,layout.Ai.Modules.Select(m=>m.Start).ToArray(),new[]{typeof(Ep1AIUpdateModuleData)},576,480);
            Check("Body",layout.Body,layout.Geometry==0?n.Length:layout.Geometry,new[]{layout.Body},new[]{typeof(Ep1ActiveBodyModuleData)},324,-1);
            foreach(var owner in layout.Clients){Type[] types=owner.Slot==492?new[]{typeof(LargeGroupAudioClientUpdateModuleData)}:new[]{typeof(ModelConditionAudioLoopClientBehaviorModuleData),typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),typeof(ModelConditionAudioLoopClientBehaviorModuleData),typeof(ModelConditionAudioLoopClientBehaviorModuleData),typeof(AnimationSoundClientBehaviorModuleData),typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData)};Check(owner.Slot==492?"ClientUpdates":"ClientBehaviors",owner.Start,owner.End,owner.Modules.ToArray(),types,owner.Slot==492?216:7404,owner.Start);}
            return 49;
            //-------------------------------------------------------------------------------------------------
            /** Reborn: stock coordinates are translated only by independently compared native field maps, never by calling a token producer for expected bytes. */
            //-------------------------------------------------------------------------------------------------
            void Check(string name,int start,int end,int[] starts,Type[] types,int length,int header)
            {
                var original=topology.Containers.Single(c=>c.Name==name);if(starts.Length!=original.Modules.Length || types.Length!=starts.Length)throw new InvalidDataException("All-module stock inventory differs: "+name);var map=new Dictionary<int,int>();if(header>=0){for(int j=0;j<8;j++)map.Add(original.CountAt+j,header+j);int items=(int)Word(n,header+4);for(int j=0;j<starts.Length*4;j++)map.Add(original.Items+j,items+j);}
                for(int i=0;i<starts.Length;i++){int leafEnd=i+1<starts.Length?starts[i+1]:end;var leaf=Extract(native,starts[i],leafEnd);var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,instance,allowScriptCrLfEquivalent:name=="Draws",requireAllReferences:false);var proof=comparer.Compare(types[i],original.Modules[i].Native);if(proof.ScriptLineEndingEquivalences!=(name=="Draws" && i==1?1:0))throw new InvalidDataException("All-module Script newline boundary differs.");foreach(var entry in comparer.NativeOffsets)map.Add(entry.Key,starts[i]+entry.Value);}
                int table=root[FastHash.GetHashCode(name)].Offset,stockTable=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode(name)].Offset;if(table<0 || table>shift-length)throw new InvalidDataException("All-module metadata bounds differ: "+name);var expected=stock.Bytes.AsSpan(stockTable,length).ToArray();for(int at=8;at<length;at+=12){uint target=Word(expected,at);if(target!=0)Put(expected,at,target>=stockTable && target<stockTable+length?target-(uint)stockTable+(uint)table:map.TryGetValue((int)target,out int coordinate)?checked((uint)(shift+coordinate)):throw new InvalidDataException("All-module stock target lacks field owner: "+name));}if(!b.AsSpan(table,length).SequenceEqual(expected))throw new InvalidDataException("All-module independent stock metadata differs: "+name);
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detach actual composed metadata/native output while refusing every cross-owner target; do not regenerate observed metadata. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Detach(Chunk output,int shift,int table,int length,int start,int end)
    {var result=Extract(output,shift+start,shift+end,length);output.InstanceBuffer.AsSpan(table,length).CopyTo(result.InstanceBuffer);for(int at=8;at<length;at+=12){uint target=Word(result.InstanceBuffer,at);if(target!=0)Put(result.InstanceBuffer,at,target>=table && target<table+length?target-(uint)table:target>=shift+start && target<shift+end?target-(uint)(shift+start)+(uint)length:throw new InvalidDataException("All-module detached metadata crosses owners."));}return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: an extracted native owner retains common reference indices while rebasing only its own pointers and auxiliary sources. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk source,int start,int end,int prefix=0)
    {if(start<0 || end<=start || end>source.InstanceBuffer.Length)throw new InvalidDataException("All-module extraction bounds differ.");var result=new Chunk{InstanceBuffer=new byte[prefix+end-start],RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start+(uint)prefix)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start+(uint)prefix))};source.InstanceBuffer.AsSpan(start,end-start).CopyTo(result.InstanceBuffer.AsSpan(prefix));foreach(uint slot in Slots(result.RelocationBuffer)){uint target=Word(result.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("All-module native pointer crosses owners.");Put(result.InstanceBuffer,(int)slot,target-(uint)start+(uint)prefix);}return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw source is bounded before parsing and cannot resolve DTDs or external entities; original script whitespace is retained. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string source){using var text=new StringReader(source);using var reader=XmlReader.Create(text,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var result=new XmlDocument{XmlResolver=null,PreserveWhitespace=true};result.Load(reader);return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: deliberate corruptions must produce explicit schema/data refusals. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException){return;}throw new InvalidDataException("Corrupt all-module graph accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: corruption controls never modify a live compiled input. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: full three-buffer repeat equality includes exact auxiliary order. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: root-row searches stop at the bounded terminator. */
    //-------------------------------------------------------------------------------------------------
    private static int FindRow(byte[] b,uint hash){for(int at=0;at<=b.Length-12;at+=12){uint name=Word(b,at);if(name==0)break;if(name==hash)return at;}throw new InvalidDataException("All-module root row missing.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every auxiliary source is unique and the final sentinel is mandatory for nonempty arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("All-module auxiliary framing differs.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("All-module auxiliary duplicate slot.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve actual order and absent-stream representation when detaching owners. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> source){var values=source.ToArray();if(values.Length==0)return Array.Empty<byte>();var result=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(result,i*4,values[i]);Put(result,values.Length*4,uint.MaxValue);return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all reads remain inside the admitted captured/generated byte arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("All-module word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: writes affect only owned buffers and retain little-endian format. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value){if(at<0 || at>b.Length-4)throw new InvalidDataException("All-module patch bounds differ.");BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);}
}
