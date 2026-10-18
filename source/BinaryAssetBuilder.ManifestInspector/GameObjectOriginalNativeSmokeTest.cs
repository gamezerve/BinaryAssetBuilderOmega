using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify actual original Conscript root values and every native owner together; this bounded explicit source assembly is not a general inheritance engine or tokenized asset publisher.
internal static class GameObjectOriginalNativeSmokeTest
{
    // Reborn: detached pointers/count words are independently restored by the enclosing owner proof, never silently excluded from whole-object coverage.
    private static readonly int[] Detached={416,468,472,476,480,484,488,492,496,500,532,540,552,564};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: replace every synthetic root attribute/child with the captured three-object source chain, retaining the complete already reviewed original module sources. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(SchemaSet schemas,XmlDocument modules,XmlElement[] objects,XmlDocument definitions,string manifest,ManifestAsset[] catalog,string globalManifest,string? alphaSource=null,string? alphaOutput=null)
    {
        GameObjectRootDefaultSmokeTest.Run(schemas.Schemas);
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name);var source=Assemble();var pair=Build(source);var snapshot=Clone(pair.Native);var proof=Verify(pair.Native,pair.Instance);var repeat=Build(source);if(!Equal(snapshot,pair.Native) || !Equal(pair.Native,repeat.Native))throw new InvalidDataException("Original native repeat/input isolation differs.");int refusals=0;
        foreach(var field in Ep1GameObjectRootValueTokens.AllFields){var bad=Clone(pair.Native);bad.InstanceBuffer[field.Offset]^=1;Refuse(()=>Verify(bad,pair.Instance));refusals++;}
        foreach(int slot in new[]{416,468,472,476,484,488,492,496,500,532,540,552,564}){var bad=Clone(pair.Native);Put(bad.InstanceBuffer,slot,0);Refuse(()=>Verify(bad,pair.Instance));refusals++;}
        foreach(bool relo in new[]{true,false}){var bad=Clone(pair.Native);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad,pair.Instance));refusals++;}
        var orphan=Clone(pair.Native);orphan.InstanceBuffer=orphan.InstanceBuffer.Concat(new byte[]{1,0,0,0}).ToArray();Refuse(()=>Verify(orphan,pair.Instance));refusals++;
        var edit=(XmlDocument)source.CloneNode(true);edit.DocumentElement!.SetAttribute("BuildTime","5");var changed=Build(edit);Refuse(()=>Verify(changed.Native,changed.Instance));refusals++;
        edit=(XmlDocument)source.CloneNode(true);edit.DocumentElement!.SetAttribute("UnitTypeIcon","RebornMissingOriginalIcon");Refuse(()=>Build(edit));refusals++;
        Console.WriteLine($"Original Conscript native: PASS (all 600 root bytes and actual root values; {proof.Modules}/49 original modules + six populated owners + geometry; {proof.Relocations} relocation sources/{proof.Imports} imports, one Core/{pair.Instance.ReferencedInstances.Count} occurrences/one Tracker; BIN/RELO/IMP={pair.Native.InstanceBuffer.Length}/{pair.Native.RelocationBuffer.Length}/{pair.Native.ImportsBuffer.Length}; {refusals} native/source refusals; exact whole ownership union, one bounded Script CRLF/LF equivalence; no complete metadata/stream publication, general inheritance or new gameplay).");
        // Reborn: actual-root metadata must qualify independently after whole native ownership, without broadening alpha or publishing a stream.
        var tokens=GameObjectOriginalMetadataSmokeTest.Run(schemas.Schemas,pair.Native,pair.Instance,manifest);
        // Reborn: publish only a fresh diagnostic stream after complete native and metadata qualification; game packaging remains closed.
        GameObjectOriginalStreamSmokeTest.Run(tokens,pair.Instance,manifest,globalManifest);
        // Reborn: separately qualify explicit source edits/new identity and offline trial packages without changing the original-stock oracle or launching the game.
        // Reborn: compile the selected alpha XML through actual Core rather than copying a previously built trial package.
        GameObjectConscriptTrialSmokeTest.Run(source,pair.Native,pair.Instance,tokens,schemas,catalog,manifest,globalManifest,Build,(value,instance)=>{Verify(value,instance);},alphaSource,alphaOutput);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: explicit attribute override, additive ArmorSet/LocomotorSet and last direct singleton selection are qualified only for this captured chain by independent native stock equality. */
        //-------------------------------------------------------------------------------------------------
        XmlDocument Assemble()
        {
            if(objects.Length!=3 || objects[0].GetAttribute("id")!="BaseObject" || objects[0].HasAttribute("inheritFrom") || objects[1].GetAttribute("id")!="BaseInfantry" || objects[1].GetAttribute("inheritFrom")!="BaseObject" || objects[2].GetAttribute("id")!="SovietAntiInfantryInfantry" || objects[2].GetAttribute("inheritFrom")!="BaseInfantry")throw new InvalidDataException("Original source chain changed.");
            var xml=new XmlDocument{XmlResolver=null,PreserveWhitespace=true};var root=xml.CreateElement("GameObject",SchemaSet.XmlNamespace);xml.AppendChild(root);
            foreach(var obj in objects)foreach(XmlAttribute attribute in obj.Attributes)if(attribute.NamespaceURI.Length==0 && attribute.Name is not ("id" or "inheritFrom"))root.SetAttribute(attribute.Name,attribute.Value);root.SetAttribute("id","SovietAntiInfantryInfantry");
            var names=new HashSet<string>(new[]{"Draws","Behaviors","AI","Body","ClientUpdates","ClientBehaviors"},StringComparer.Ordinal);foreach(var module in modules.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>names.Contains(e.LocalName)))root.AppendChild(xml.ImportNode(module,true));
            foreach(var group in objects.SelectMany(o=>o.ChildNodes.OfType<XmlElement>()).Where(e=>!names.Contains(e.LocalName)).GroupBy(e=>e.LocalName))foreach(var element in group.Key is "ArmorSet" or "LocomotorSet"?group:group.TakeLast(1))root.AppendChild(xml.ImportNode(element,true));
            // Reborn: selecting the concrete DisplayName implements this one captured Replace directive; reject all other instance metadata instead of broadly stripping unknown inheritance instructions.
            foreach(XmlAttribute attribute in xml.SelectNodes("//@*")!.OfType<XmlAttribute>().Where(a=>a.NamespaceURI=="uri:ea.com:eala:asset:instance").ToArray()){if(attribute.LocalName!="joinAction" || attribute.Value!="Replace" || attribute.OwnerElement?.LocalName!="DisplayName")throw new InvalidDataException("Unreviewed original instance directive.");attribute.OwnerElement.RemoveAttributeNode(attribute);}
            var defines=definitions.GetElementsByTagName("Define",SchemaSet.XmlNamespace).OfType<XmlElement>().ToDictionary(e=>e.GetAttribute("name"),e=>e.GetAttribute("value"),StringComparer.Ordinal);
            if(defines["ACCOUNT_ORE"]!="0" || defines["STANDARD_SHROUD_CLEAR"]!="500.0")throw new InvalidDataException("Original root definition anchors changed.");
            foreach(XmlAttribute attribute in xml.SelectNodes("//@*")!){if(attribute.Value=="=$ACCOUNT_ORE")attribute.Value=defines["ACCOUNT_ORE"];else if(attribute.Value=="=$STANDARD_SHROUD_CLEAR")attribute.Value=defines["STANDARD_SHROUD_CLEAR"];if(attribute.Value.Contains('$'))throw new InvalidDataException("Unreviewed original root expression: "+attribute.LocalName);}
            var type=(XmlSchemaComplexType)((XmlSchemaElement)schemas.Schemas.GlobalElements[new XmlQualifiedName("GameObject",SchemaSet.XmlNamespace)]!).ElementSchemaType!;var order=new List<string>();Order(type.ContentTypeParticle);foreach(var child in root.ChildNodes.OfType<XmlElement>().OrderBy(e=>order.IndexOf(e.LocalName)).ToArray())root.AppendChild(child);Ep1GameObjectEmptyRootProjection.ValidateSource(root);return xml;
            //-------------------------------------------------------------------------------------------------
            /** Reborn: only admitted direct root declarations determine source ordering; nested particle order stays untouched. */
            //-------------------------------------------------------------------------------------------------
            void Order(XmlSchemaParticle particle){if(particle is XmlSchemaElement e)order.Add(e.Name!);else if(particle is XmlSchemaGroupBase group)foreach(XmlSchemaParticle child in group.Items)Order(child);else throw new InvalidDataException("Unreviewed original root source particle.");}
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: one real normalization resolves every concrete dependency before allocation on one actual Tracker. */
        //-------------------------------------------------------------------------------------------------
        (Chunk Native,InstanceDeclaration Instance) Build(XmlDocument input){GameObjectBodyProjectionSmokeTest.Validate(schemas.Schemas,input.OuterXml);var raw=(XmlDocument)input.CloneNode(true);raw.Schemas.Add(schemas.Schemas);raw.Validate((_,e)=>throw new InvalidDataException(e.Message));Ep1SelectedModuleIdentity.ValidateSource(raw.DocumentElement!,true);Ep1GameObjectEmptyRootProjection.ValidateSource(raw.DocumentElement!);ConscriptBehaviorSurvey.ResolveStrong(raw,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,raw.OuterXml);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Original root missing/ambiguous dependency.");return(GameObjectBodyProjectionSmokeTest.Compile(Xml(instance.XmlNode.OuterXml),true,true,true),instance);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: union independently walked root, container headers, full concrete modules, geometry and six EP1 owners; all native bytes and auxiliary sources must have one verified owner. */
        //-------------------------------------------------------------------------------------------------
        (int Modules,int Relocations,int Imports) Verify(Chunk native,InstanceDeclaration instance)
        {
            var b=native.InstanceBuffer;int cut=(int)Word(b,416);if(cut<600 || cut>b.Length)throw new InvalidDataException("Original root tail boundary differs.");var cleared=b.ToArray();var original=stock.Bytes.ToArray();foreach(int at in Detached){Put(cleared,at,0);Put(original,stock.Projection.NativeRoot+at,0);}var rootNative=new Chunk{InstanceBuffer=cleared,RelocationBuffer=Pack(Slots(native.RelocationBuffer).Where(s=>!Detached.Contains((int)s))),ImportsBuffer=native.ImportsBuffer.ToArray()};var selection=new SelectedNativeGraphComparer(original,stock.Asset.References.ToArray(),rootNative,instance,requireAllReferences:false,captureNativeCoordinates:false).CompareRootSelection(stock.Projection.NativeRoot);bool[] covered=selection.Covered;var relos=new HashSet<uint>(selection.Relocations);var imports=new HashSet<uint>(selection.Imports);
            var baseBytes=b.AsSpan(0,cut).ToArray();foreach(var field in Ep1GameObjectRootOwnersProjection.Fields)Put(baseBytes,field.Slot,0);var layout=Ep1GameObjectStringTokens.Inspect(baseBytes,Slots(native.RelocationBuffer).Where(s=>s<cut && !Ep1GameObjectRootOwnersProjection.Fields.Any(f=>f.Slot==s)).ToArray(),Slots(native.ImportsBuffer).Where(s=>s<cut).ToArray(),true);int modulesVerified=0,scripts=0;
            Container("Behaviors",476,layout.List,layout.Items,layout.Modules.ToArray(),layout.Body);
            Container("Draws",472,layout.Draws.Start,(int)Word(b,layout.Draws.Start+4),layout.Draws.Modules.ToArray(),layout.Draws.End);
            Container("AI",-1,480,layout.Ai.Items,layout.Ai.Modules.Select(m=>m.Start).ToArray(),layout.Ai.Modules[^1].End);
            foreach(var owner in layout.Clients)Container(owner.Slot==492?"ClientUpdates":"ClientBehaviors",owner.Slot,owner.Start,(int)Word(b,owner.Start+4),owner.Modules.ToArray(),owner.End);
            var body=topology.Containers.Single(c=>c.Name=="Body").Modules.Single();Parent(488,body.Native);Leaf(typeof(Ep1ActiveBodyModuleData),body.Native,layout.Body,layout.Geometry);modulesVerified++;
            Parent(500,(int)Word(stock.Bytes,stock.Projection.NativeRoot+500));Leaf(typeof(Ep1Geometry),(int)Word(stock.Bytes,stock.Projection.NativeRoot+500),layout.Geometry,cut);
            for(int i=0;i<Ep1GameObjectRootOwnersProjection.Fields.Count;i++){var field=Ep1GameObjectRootOwnersProjection.Fields[i];int start=(int)Word(b,field.Slot),end=i+1<Ep1GameObjectRootOwnersProjection.Fields.Count?(int)Word(b,Ep1GameObjectRootOwnersProjection.Fields[i+1].Slot):b.Length,old=(int)Word(stock.Bytes,stock.Projection.NativeRoot+field.Slot);Parent(field.Slot,old);Leaf(field.NativeType,old,start,end);}
            if(modulesVerified!=49 || scripts!=1 || !Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order()) || !Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order()) || !imports.Select(s=>Word(b,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Original complete native module/auxiliary/reference union differs.");int gap=0;for(int i=0;i<covered.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0 || ++gap>3)throw new InvalidDataException("Original complete native unowned allocation at "+i);}return(modulesVerified,relos.Count,imports.Count);
            //-------------------------------------------------------------------------------------------------
            /** Reborn: original container order/count and pointer ownership are checked separately from complete concrete leaf payloads. */
            //-------------------------------------------------------------------------------------------------
            void Container(string name,int slot,int header,int items,int[] starts,int end){var old=topology.Containers.Single(c=>c.Name==name);if(slot>=0)Parent(slot,old.CountAt);if(starts.Length!=old.Modules.Length || Word(b,header)!=starts.Length || Word(stock.Bytes,old.CountAt)!=starts.Length || items<600 || items>b.Length-starts.Length*4)throw new InvalidDataException("Original container topology differs: "+name);Cover(header,8,header==480);Cover(items,starts.Length*4);Add(relos,(uint)(header+4));for(int i=0;i<starts.Length;i++){if(Word(b,items+i*4)!=starts[i] || Word(b,starts[i])!=old.Modules[i].TypeId)throw new InvalidDataException("Original container concrete identity differs.");Add(relos,(uint)(items+i*4));Type type=name switch{"Behaviors"=>Ep1BehaviorMetadataTokenizer.ReviewedNativeType(old.Modules[i].TypeId),"Draws"=>i<2?typeof(W3DScriptedModelDrawModuleData):typeof(W3DTracerDrawModuleData),"AI"=>typeof(Ep1AIUpdateModuleData),"ClientUpdates"=>typeof(LargeGroupAudioClientUpdateModuleData),"ClientBehaviors"=>old.Modules[i].Type switch{"ModelConditionSoundSelectorClientBehaviorModuleData"=>typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),"AnimationSoundClientBehaviorModuleData"=>typeof(AnimationSoundClientBehaviorModuleData),"ModelConditionAudioLoopClientBehaviorModuleData"=>typeof(ModelConditionAudioLoopClientBehaviorModuleData),_=>throw new InvalidDataException("Unreviewed original client family.")},_=>throw new InvalidDataException("Unreviewed original container.")};Leaf(type,old.Modules[i].Native,starts[i],i+1<starts.Length?starts[i+1]:end,name=="Draws");modulesVerified++;}}
            //-------------------------------------------------------------------------------------------------
            /** Reborn: a nonzero stock parent must retain a bounded nonzero native pointer and its unique relocation source. */
            //-------------------------------------------------------------------------------------------------
            void Parent(int slot,int old){uint target=Word(b,slot);if(old==0 || target<600 || target>=b.Length)throw new InvalidDataException("Original parent pointer differs.");Add(relos,(uint)slot);}
            //-------------------------------------------------------------------------------------------------
            /** Reborn: each full concrete graph has independent exact native/semantic-reference/auxiliary validation before joining ownership coordinates. */
            //-------------------------------------------------------------------------------------------------
            void Leaf(Type type,int old,int start,int end,bool script=false){var leaf=Extract(native,start,end);var result=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,instance,allowScriptCrLfEquivalent:script,requireAllReferences:false,captureNativeCoordinates:false).Compare(type,old);scripts+=result.ScriptLineEndingEquivalences;Cover(start,end-start);foreach(uint s in Slots(leaf.RelocationBuffer))Add(relos,s+(uint)start);foreach(uint s in Slots(leaf.ImportsBuffer))Add(imports,s+(uint)start);}
            //-------------------------------------------------------------------------------------------------
            /** Reborn: only the explicitly detached inline AI header may already be root-covered; every heap owner must be disjoint. */
            //-------------------------------------------------------------------------------------------------
            void Cover(int at,int size,bool inline=false){if(at<0 || size<0 || at>b.Length-size || !inline && covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("Original native owner overlap/bounds.");Array.Fill(covered,true,at,size);}
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: strict independent owner extraction refuses every cross-owner pointer and retains shared occurrence indices. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk source,int start,int end){if(start<600 || end<=start || end>source.InstanceBuffer.Length)throw new InvalidDataException("Original native extraction bounds.");var leaf=new Chunk{InstanceBuffer=source.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};foreach(uint slot in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Original native pointer crosses owners.");Put(leaf.InstanceBuffer,(int)slot,target-(uint)start);}return leaf;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: no two independently proven owners may claim the same auxiliary source. */
    //-------------------------------------------------------------------------------------------------
    private static void Add(HashSet<uint> values,uint value){if(!values.Add(value))throw new InvalidDataException("Original native auxiliary owner duplicate.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalization output is internal XML, but DTDs/external resolution remain disabled. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){using var input=new StringReader(text);using var reader=XmlReader.Create(input,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var xml=new XmlDocument{XmlResolver=null,PreserveWhitespace=true};xml.Load(reader);return xml;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed data/source must explicitly fail rather than count as a partial success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or FormatException or OverflowException){return;}throw new InvalidDataException("Invalid original native graph admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent buffers protect actual source/native snapshots from mutation controls. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: repeat equality includes the exact ordered auxiliary streams. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit framing/unique aligned slots precede ownership partitioning. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Original auxiliary framing.");var result=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(result.Any(s=>s%4!=0) || result.Distinct().Count()!=result.Length)throw new InvalidDataException("Original duplicate/unaligned auxiliary.");return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary coordinate changes preserve order and one nonempty sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reads stay inside the bounded captured/native graph. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Original native word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned comparison/extraction/mutation arrays receive pointer patches. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
