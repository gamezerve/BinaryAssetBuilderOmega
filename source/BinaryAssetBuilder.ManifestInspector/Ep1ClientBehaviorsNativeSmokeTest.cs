using System.Buffers.Binary;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk = Relo.Chunk;
using Tracker = Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile all six complete Conscript client behaviors, including the two explicitly captured BaseInfantry leaves, without claiming an inheritance engine or audio playback.
internal static class Ep1ClientBehaviorsNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: one official-type envelope and Core table cover all three client families, complete native stock equality, edits and fail-closed corruption/dispatch controls. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string sourceRoot,string manifest,string globalManifest,bool includeTokens=false)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed client schema required.");
            var alias=new XmlSchema{TargetNamespace=SchemaSet.XmlNamespace,ElementFormDefault=XmlSchemaForm.Qualified};
            alias.Items.Add(new XmlSchemaElement{Name="ClientsProbe",SchemaType=new XmlSchemaComplexType{ContentModel=new XmlSchemaComplexContent{Content=new XmlSchemaComplexContentExtension{BaseTypeName=new XmlQualifiedName("BaseAssetType",SchemaSet.XmlNamespace),Particle=new XmlSchemaSequence{Items={new XmlSchemaElement{Name="ClientBehaviors",SchemaTypeName=new XmlQualifiedName("ClientBehaviorModules",SchemaSet.XmlNamespace)}}}}}}});official.Add(alias);official.Compile();
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);
            foreach(XmlSchema seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();
            // Reborn: rebind only the reference-normalization helpers; stale seed inheritance/dependency caches are never used as a general source processor.
            var setup=typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)??throw new MissingMethodException("Client Core schema helper missing.");setup.Invoke(schemas,null);
            foreach(var surface in new[]{("ThingTemplateVoiceType",Enum.GetNames<Ep1ThingTemplateVoiceType>()),("ThingTemplateSoundType",Enum.GetNames<Ep1ThingTemplateSoundType>()),("ThingTemplateObjectSpecificVoiceType",Enum.GetNames<ThingTemplateObjectSpecificVoiceType>())})
            {var simple=(XmlSchemaSimpleType)official.GlobalTypes[new XmlQualifiedName(surface.Item1,SchemaSet.XmlNamespace)]!;if(!((XmlSchemaSimpleTypeRestriction)simple.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f=>f.Value).SequenceEqual(surface.Item2))throw new InvalidDataException("Client audio enum schema/order drift: "+surface.Item1);}
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var catalog=stock.Document.Assets.Concat(Catalog(globalManifest).Assets).ToArray();var topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="ClientBehaviors");
            var files=new[]{Path.Combine(sourceRoot,"BaseObjects","BaseObject.xml"),Path.Combine(sourceRoot,"BaseObjects","BaseInfantry.xml"),Path.Combine(sourceRoot,"Soviet","Units","SovietAntiInfantryInfantry.xml")};var captures=files.Select(f=>SdkEnvironmentPreflight.Read(f,1048576)).ToArray();var documents=captures.Select(b=>Xml(Encoding.UTF8.GetString(b).TrimStart('\uFEFF'))).ToArray();
            var gameObjects=documents.Select(d=>d.GetElementsByTagName("GameObject",SchemaSet.XmlNamespace).OfType<XmlElement>().Single()).ToArray();
            if(gameObjects[1].GetAttribute("inheritFrom")!="BaseObject" || gameObjects[2].GetAttribute("inheritFrom")!="BaseInfantry" || gameObjects[0].SelectNodes("ea:ClientBehaviors",Ns(gameObjects[0]))!.Count!=0)throw new InvalidDataException("Selected explicit client source chain changed.");
            var leaves=gameObjects.Skip(1).SelectMany(g=>g.SelectNodes("ea:ClientBehaviors/*",Ns(g))!.Cast<XmlElement>()).ToArray();if(leaves.Length!=6 || topology.Modules.Length!=6)throw new InvalidDataException("All six complete client leaves required.");
            var source=Xml("<ClientsProbe xmlns='uri:ea.com:eala:asset' id='RebornClients'><ClientBehaviors/></ClientsProbe>");foreach(var leaf in leaves)source.DocumentElement!.FirstChild!.AppendChild(source.ImportNode(leaf,true));int resolved=ResolveSounds(source,schemas,catalog);var pair=Build(source.OuterXml,schemas,catalog);var native=pair.Native;
            var coordinateMap=new Dictionary<int,int>();int records=0,strings=0,imports=0,failures=0;
            for(int i=0;i<6;i++)
            {
                var leaf=Leaf(native,i);int start=(int)Word(native.InstanceBuffer,8+i*4);var module=topology.Modules[i];Type type=leaves[i].LocalName switch{"ModelConditionAudioLoopClientBehavior"=>typeof(ModelConditionAudioLoopClientBehaviorModuleData),"ModelConditionSoundSelectorClientBehavior"=>typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),"AnimationSoundClientBehavior"=>typeof(AnimationSoundClientBehaviorModuleData),_=>throw new InvalidDataException("Unreviewed client family.")};
                string wireName=leaves[i].LocalName+"ModuleData";if(module.Type!=wireName || module.TypeId!=FastHash.GetHashCode(wireName) || Word(leaf.InstanceBuffer,4)!=FastHash.GetHashCode(leaves[i].GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("Client source/stock class or identity order differs.");
                var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,pair.Instance,requireAllReferences:false);var result=comparer.Compare(type,module.Native);records+=result.Records;strings+=result.Strings;imports+=result.Imports;foreach(var entry in comparer.NativeOffsets)coordinateMap.Add(entry.Key,entry.Value+start);
                foreach(int at in new[]{0,4,8,leaf.InstanceBuffer.Length-1}){var bad=Clone(leaf);bad.InstanceBuffer[at]^=1;Refuse(()=>new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),bad,pair.Instance,requireAllReferences:false).Compare(type,module.Native));failures++;}
                foreach(bool relo in new[]{false,true}){var bad=Clone(leaf);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),bad,pair.Instance,requireAllReferences:false).Compare(type,module.Native));failures++;}
                Console.WriteLine($"Complete client {leaves[i].GetAttribute("id")}: {type.Name}; BIN/RELO/IMP={leaf.InstanceBuffer.Length}/{leaf.RelocationBuffer.Length}/{leaf.ImportsBuffer.Length}; stock native PASS.");
            }
            CheckOccurrences(native,pair.Instance);if(imports!=pair.Instance.ReferencedInstances.Count)throw new InvalidDataException("Combined client imports not fully consumed.");
            var repeat=Build(source.OuterXml,schemas,catalog);if(!native.InstanceBuffer.SequenceEqual(repeat.Native.InstanceBuffer) || !native.RelocationBuffer.SequenceEqual(repeat.Native.RelocationBuffer) || !native.ImportsBuffer.SequenceEqual(repeat.Native.ImportsBuffer))throw new InvalidDataException("Complete shared client repeat differs.");
            // Reborn: bind every stock metadata target to the complete independently compared native graph, but do not copy or publish these tables as generated metadata.
            for(int i=0;i<8;i++)coordinateMap.Add(topology.CountAt+i,i);for(int i=0;i<24;i++)coordinateMap.Add(topology.Items+i,8+i);
            var rootTokens=GameObjectStockReview.Tokens(stock.Bytes,0);int table=rootTokens[FastHash.GetHashCode("ClientBehaviors")].Offset,end=rootTokens[FastHash.GetHashCode("Geometry")].Offset;if(table+84!=topology.Modules[0].Wrapper || end<=topology.Modules[^1].Wrapper || (end-table)%12!=0)throw new InvalidDataException("Client stock metadata bounds differ.");int bindings=0;
            for(int at=table+8;at<end;at+=12){uint target=Word(stock.Bytes,at);if(target==0 || target>=table && target<end)continue;if(!coordinateMap.ContainsKey((int)target))throw new InvalidDataException("Client stock metadata target escapes independently compared native graph.");bindings++;}
            // Reborn: optional generated-table proof uses stock only as an independent expected result after native ownership has already been established.
            if(includeTokens)TokenProof(native,stock.Bytes,table,end,coordinateMap);
            int edits=Authored(source,schemas,catalog,includeTokens),sourceFailures=0;string firstSound=source.GetElementsByTagName("ModelConditionSound",SchemaSet.XmlNamespace)[0]!.Attributes!["Sound"]!.Value;
            EnumControls(source,schemas,catalog,firstSound);edits++;
            foreach(string bad in new[]{source.OuterXml.Replace("Sound=\""+firstSound+"\"","Sound=\"RebornMissingClientAudio\"",StringComparison.Ordinal),source.OuterXml.Replace("Frame=\"3\"","Frame=\"\"",StringComparison.Ordinal),source.OuterXml.Replace("AudioType=\"voiceAttack\"","AudioType=\"RebornInvalidVoice\"",StringComparison.Ordinal),source.OuterXml.Replace("id=\"Frozen\"","id=\"moduletag_shockdeathoverland\"",StringComparison.Ordinal)}){Refuse(()=>Build(bad,schemas,catalog));sourceFailures++;}
            Refuse(()=>Compile(pair.Instance.XmlNode,true));sourceFailures++;
            Refuse(()=>Compile(pair.Instance.XmlNode,true,legacy:true));sourceFailures++;
            // Reborn: exercise the EP1 marshaler directly so Core catalog validation cannot hide a populated EvaEvent ABI refusal.
            var unreviewedEva=Xml(pair.Instance.XmlNode.OuterXml);((XmlElement)unreviewedEva.GetElementsByTagName("AudioEntry",SchemaSet.XmlNamespace)[0]!).SetAttribute("EvaEvent","RebornUnsupportedEva");Refuse(()=>Compile(unreviewedEva.DocumentElement!));sourceFailures++;
            var ambiguous=Xml(source.OuterXml);var audio=catalog.First(a=>a.Name==ambiguous.GetElementsByTagName("ModelConditionSound",SchemaSet.XmlNamespace)[0]!.Attributes!["Sound"]!.Value);Refuse(()=>Build(source.OuterXml,schemas,catalog.Concat(new[]{audio}).ToArray()));sourceFailures++;
            for(int i=0;i<files.Length;i++)if(!SHA256.HashData(captures[i]).SequenceEqual(SHA256.HashData(SdkEnvironmentPreflight.Read(files[i],1048576))))throw new InvalidDataException("Official client source changed during verification.");
            Console.WriteLine($"EP1 complete client native graphs: PASS (six leaves/three families, two explicit BaseInfantry and four direct source leaves, one Core table/{imports} strong slots, {records} typed visits/{strings} strings, {resolved} concrete sound names, {bindings} stock metadata/native bindings in {end-table} bytes, {edits} authored variants, {failures} native/{sourceFailures} source/dispatch refusals; generated metadata {(includeTokens?"verified":"not requested")}; no general inheritance, root-container integration, package, audio playback or gameplay claim).");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: authored IDs, optional range-cap pointers and ordered frame lists are literal edits to all six complete source leaves, not native byte patches. */
    //-------------------------------------------------------------------------------------------------
    private static int Authored(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog,bool includeTokens)
    {
        int cases=0;foreach(bool cap in new[]{false,true})foreach(string frames in new[]{"1","1 7 23"})
        {
            var edited=Xml(source.OuterXml);int index=0;foreach(XmlElement leaf in edited.DocumentElement!.FirstChild!.ChildNodes.OfType<XmlElement>())leaf.SetAttribute("id","RebornClient"+index++);var animation=(XmlElement)edited.GetElementsByTagName("AnimationSoundClientBehavior",SchemaSet.XmlNamespace)[0]!;if(cap)animation.SetAttribute("MaxUpdateRangeCap","17.5");var sound=(XmlElement)animation.GetElementsByTagName("Sound",SchemaSet.XmlNamespace)[0]!;sound.SetAttribute("Frame",frames);
            var pair=Build(edited.OuterXml,schemas,catalog);CheckOccurrences(pair.Native,pair.Instance);if(includeTokens)CheckTokenRoundTrip(pair.Native);for(int i=0;i<6;i++){int at=(int)Word(pair.Native.InstanceBuffer,8+i*4);if(Word(pair.Native.InstanceBuffer,at+4)!=FastHash.GetHashCode(("RebornClient"+i).ToLowerInvariant()))throw new InvalidDataException("Authored client module ID not applied.");}
            var native=Leaf(pair.Native,4);uint pointer=Word(native.InstanceBuffer,8);if(cap?pointer==0 || Word(native.InstanceBuffer,(int)pointer)!=0x418C0000:pointer!=0)throw new InvalidDataException("Client range-cap presence/value differs.");int items=(int)Word(native.InstanceBuffer,16),frameCount=(int)Word(native.InstanceBuffer,items+16),frameItems=(int)Word(native.InstanceBuffer,items+20);int[] expected=frames.Split(' ').Select(int.Parse).ToArray();if(frameCount!=expected.Length || !Enumerable.Range(0,frameCount).Select(i=>unchecked((int)Word(native.InstanceBuffer,frameItems+i*4))).SequenceEqual(expected))throw new InvalidDataException("Client ordered frame values differ.");cases++;
        }
        // Reborn: singleton, reversed and subset containers prove the writer does not depend on Conscript's six fixed module positions.
        if(includeTokens)foreach(int[] selection in new[]{new[]{0},new[]{1},new[]{4},new[]{5,4,3,2,1,0},new[]{4,1,0}}){var edited=Xml(source.OuterXml);var container=edited.DocumentElement!.FirstChild!;var original=container.ChildNodes.OfType<XmlElement>().Select(e=>(XmlElement)e.CloneNode(true)).ToArray();container.RemoveAll();foreach(int i in selection)container.AppendChild(original[i]);var pair=Build(edited.OuterXml,schemas,catalog);CheckOccurrences(pair.Native,pair.Instance);CheckTokenRoundTrip(pair.Native);cases++;}return cases;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every generated stock-prefix byte, retain native/auxiliary data exactly and reject malformed or explicitly unreviewed graph variants. */
    //-------------------------------------------------------------------------------------------------
    private static void TokenProof(Chunk native,byte[] stock,int table,int end,Dictionary<int,int> coordinates)
    {
        int prefix=end-table;var expected=stock.AsSpan(table,prefix).ToArray();for(int at=8;at<prefix;at+=12){uint target=Word(expected,at);if(target==0)continue;Put(expected,at,target>=table && target<end?target-(uint)table:checked((uint)(prefix+coordinates[(int)target])));}
        var tokens=CheckTokenRoundTrip(native);if(tokens.InstanceBuffer.Length-native.InstanceBuffer.Length!=prefix || !tokens.InstanceBuffer.AsSpan(0,prefix).SequenceEqual(expected)){int mismatch=Enumerable.Range(0,Math.Min(prefix,tokens.InstanceBuffer.Length)).FirstOrDefault(i=>tokens.InstanceBuffer[i]!=expected[i]);throw new InvalidDataException("Generated client stock metadata differs at "+mismatch+"; prefix="+(tokens.InstanceBuffer.Length-native.InstanceBuffer.Length));}
        int failures=0;
        foreach(bool relo in new[]{false,true}){var bad=Clone(native);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(bad));failures++;}
        int loopItems=(int)Word(native.InstanceBuffer,44);
        foreach(var mutation in new (int At,uint Value)[]{(0,65),(4,12),(8,0),(32,0xDEADBEEF),(36,0),(12,32),(loopItems+60,0xFFFFFFFF)}){var bad=Clone(native);Put(bad.InstanceBuffer,mutation.At,mutation.Value);Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(bad));failures++;}
        // Reborn: optional EvaEvent uses a different reviewed metadata shape than the legacy string marshaler, and populated sound arrays have no independent metadata proof yet.
        int selector=(int)Word(native.InstanceBuffer,12),over=(int)Word(native.InstanceBuffer,selector+12),voices=(int)Word(native.InstanceBuffer,over+124),entries=(int)Word(native.InstanceBuffer,voices+4);
        foreach(int at in new[]{entries+4,voices+8,voices+16,over+128}){var bad=Clone(native);Put(bad.InstanceBuffer,at,1);Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(bad));failures++;}
        // Reborn: cross-leaf aliases, duplicate identities, invalid ordinals and extra storage cannot be admitted by a merely byte-preserving round trip.
        foreach(var mutation in new (int At,uint Value)[]{(selector+4,Word(native.InstanceBuffer,36)),(entries+8,67),(selector+12,48),(loopItems,4),(over+124,(uint)over)}){var bad=Clone(native);Put(bad.InstanceBuffer,mutation.At,mutation.Value);Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(bad));failures++;}
        var appended=Clone(native);appended.InstanceBuffer=appended.InstanceBuffer.Concat(new byte[4]).ToArray();Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(appended));failures++;
        var duplicate=Clone(native);duplicate.RelocationBuffer=Pack(Slots(native.RelocationBuffer).Concat(new[]{4u}));Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(duplicate));failures++;
        var empty=new Chunk{InstanceBuffer=new byte[8],RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};if(CheckTokenRoundTrip(empty).InstanceBuffer.Length!=20)throw new InvalidDataException("Client empty token list differs.");
        Console.WriteLine($"EP1 generated client metadata: PASS ({prefix} independent stock-prefix bytes; six complete leaves; unchanged native/RELO/IMP round trip; {failures} malformed/unreviewed refusals; empty container; no root/package/gameplay claim).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detach only the newly generated metadata and reverse its single native shift; all original bytes, slot order and reference values must survive exactly. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk CheckTokenRoundTrip(Chunk native)
    {
        var tokens=BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(native);var repeat=BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize(native);int prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;if(prefix<12 || prefix%12!=0 || !tokens.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !tokens.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !tokens.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))throw new InvalidDataException("Client token repeat/framing differs.");
        var detached=tokens.InstanceBuffer.AsSpan(prefix).ToArray();foreach(uint slot in Slots(native.RelocationBuffer))Put(detached,(int)slot,Word(detached,(int)slot)-(uint)prefix);
        if(!detached.SequenceEqual(native.InstanceBuffer) || !Slots(tokens.RelocationBuffer).SequenceEqual(Slots(native.RelocationBuffer).Select(s=>s+(uint)prefix)) || !Slots(tokens.ImportsBuffer).SequenceEqual(Slots(native.ImportsBuffer).Select(s=>s+(uint)prefix)))throw new InvalidDataException("Client token native/auxiliary round trip differs.");return tokens;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: one authored selector exercises every official EP1 voice/sound ordinal and optional priority while literal legacy anchors prove the KW enums remain unchanged. */
    //-------------------------------------------------------------------------------------------------
    private static void EnumControls(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog,string sound)
    {
        if(Marshal.SizeOf<Ep1AudioVoiceEntry>()!=12 || Marshal.SizeOf<Ep1AudioSoundEntry>()!=8 || Marshal.SizeOf<Ep1AudioArrayVoice>()!=24 || Marshal.SizeOf<Ep1AudioArraySound>()!=8 || Marshal.SizeOf<Ep1ModelConditionSoundSelectorOverrideBlock>()!=132 || Marshal.SizeOf<Ep1ModelConditionSoundSelectorClientBehaviorModuleData>()!=16 || (int)ThingTemplateVoiceType.voiceMove!=4 || (int)ThingTemplateVoiceType.voiceAttack!=14 || (int)ThingTemplateSoundType.soundAmbient!=4 || (int)Ep1ThingTemplateVoiceType.voiceMove!=5 || (int)Ep1ThingTemplateVoiceType.voiceAttack!=17 || (int)Ep1ThingTemplateSoundType.soundAmbient!=6)throw new InvalidDataException("Independent EP1/legacy audio ABI/enum anchors differ.");
        var xml=Xml(source.OuterXml);var selector=(XmlElement)xml.GetElementsByTagName("ModelConditionSoundSelectorClientBehavior",SchemaSet.XmlNamespace)[1]!;var over=selector.ChildNodes.OfType<XmlElement>().Single();over.RemoveAll();over.SetAttribute("VoicePriorityOverride","17");
        foreach(var family in new[]{("AudioArrayVoice",Enum.GetNames<Ep1ThingTemplateVoiceType>()),("AudioArraySound",Enum.GetNames<Ep1ThingTemplateSoundType>())}){var array=xml.CreateElement(family.Item1,SchemaSet.XmlNamespace);foreach(string name in family.Item2){var entry=xml.CreateElement("AudioEntry",SchemaSet.XmlNamespace);entry.SetAttribute("Sound",sound);entry.SetAttribute("AudioType",name);array.AppendChild(entry);}over.AppendChild(array);}
        var pair=Build(xml.OuterXml,schemas,catalog);CheckOccurrences(pair.Native,pair.Instance);var native=Leaf(pair.Native,5).InstanceBuffer;int record=(int)Word(native,12),priority=(int)Word(native,record+120);if(priority==0 || Word(native,priority)!=17)throw new InvalidDataException("Client optional priority value differs.");
        foreach(var family in new[]{(124,67,12,8),(128,39,8,4)}){int array=(int)Word(native,record+family.Item1);if(array==0 || Word(native,array)!=family.Item2)throw new InvalidDataException("Client complete enum-array count differs.");int entries=(int)Word(native,array+4);for(int i=0;i<family.Item2;i++)if(Word(native,entries+i*family.Item3+family.Item4)!=i)throw new InvalidDataException("Client authored EP1 enum ordinal differs.");}
        Console.WriteLine("Client enum/ABI controls: 67 voice + 39 sound values PASS; optional priority=17; original KW enum anchors unchanged.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: concrete schema-derived audio types are resolved uniquely before actual Core normalization, never authored captured reference indices. */
    //-------------------------------------------------------------------------------------------------
    internal static int ResolveSounds(XmlDocument document,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var audio=schemas.GetXmlType("BaseAudioEventInfo")??throw new InvalidDataException("Audio base schema missing.");var allowed=schemas.Schemas.GlobalTypes.Values.Cast<XmlSchemaType>().Where(t=>XmlSchemaType.IsDerivedFrom(t,audio,XmlSchemaDerivationMethod.None)).Select(t=>FastHash.GetHashCode(t.Name!)).ToHashSet();int resolved=0;
        foreach(XmlElement element in document.SelectNodes("//*[@Sound]")!){string name=element.GetAttribute("Sound").Trim();if(name.Length==0 || name.Contains(':'))continue;var id=new InstanceHandle(name).InstanceId;var targets=catalog.Where(a=>a.InstanceId==id && allowed.Contains(a.TypeId)).ToArray();if(targets.Length!=1)throw new InvalidDataException("Client sound missing/ambiguous: "+name);element.SetAttribute("Sound",targets[0].Name);resolved++;}return resolved;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw source validation and client-local ID uniqueness precede one Core table; every strong target must exist exactly once in the real catalogs. */
    //-------------------------------------------------------------------------------------------------
    private static (Chunk Native,InstanceDeclaration Instance) Build(string source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var raw=Xml(source);var ids=new HashSet<uint>();foreach(XmlElement leaf in raw.DocumentElement!.FirstChild!.ChildNodes.OfType<XmlElement>())if(string.IsNullOrWhiteSpace(leaf.GetAttribute("id")) || !ids.Add(FastHash.GetHashCode(leaf.GetAttribute("id").ToLowerInvariant())))throw new InvalidDataException("Client IDs missing/colliding.");var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Client dependency missing/ambiguous.");return(Compile(instance.XmlNode),instance);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: actual ClientBehaviors dispatch allocates every source record on one Tracker; extracting its complete owner removes only the empty parent root. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlNode source,bool unknown=false,bool legacy=false)
    {
        var xml=Xml("<GameObject xmlns='uri:ea.com:eala:asset'/>");var clients=(XmlElement)xml.ImportNode(source.FirstChild!,true);foreach(XmlElement leaf in clients.ChildNodes.OfType<XmlElement>())leaf.SetAttribute("TypeId",(unknown?0xDEADBEEFu:FastHash.GetHashCode(leaf.LocalName+"ModuleData")).ToString(CultureInfo.InvariantCulture));
        // Reborn: the explicit EP1 allocator avoids legacy KW enum dispatch and retains the real GameObject ClientBehaviors pointer slot on one Tracker.
        GameObject* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(GameObject),false);
        // Reborn: the legacy unknown-type control must exercise the actual formerly-silent dispatcher, not only the new selected allocator.
        if(legacy){xml.DocumentElement!.AppendChild(clients);Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!,Ns(xml.DocumentElement)),root,tracker);}else Marshaler.MarshalEp1ClientBehaviors(new Node(clients.CreateNavigator()!,Ns(clients)),&root->ClientBehaviors,tracker);
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Client Tracker failed.");int list=(int)Word(chunk.InstanceBuffer,496);var relos=Slots(chunk.RelocationBuffer);var imports=Slots(chunk.ImportsBuffer);if(list<600 || !relos.Where(s=>s<list).SequenceEqual(new[]{496u}) || imports.Any(s=>s<list))throw new InvalidDataException("Client parent-container ownership differs.");var native=new Chunk{InstanceBuffer=chunk.InstanceBuffer.AsSpan(list).ToArray(),RelocationBuffer=Pack(relos.Where(s=>s>=list).Select(s=>s-(uint)list)),ImportsBuffer=Pack(imports.Select(s=>s-(uint)list))};foreach(uint slot in Slots(native.RelocationBuffer)){uint target=Word(native.InstanceBuffer,(int)slot);if(target<list || target>=chunk.InstanceBuffer.Length)throw new InvalidDataException("Client pointer crosses parent owner.");Put(native.InstanceBuffer,(int)slot,target-(uint)list);}return native;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent leaf comparison retains the common strong occurrence values while rebasing only pointer coordinates. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Leaf(Chunk native,int index){int start=(int)Word(native.InstanceBuffer,8+index*4),end=index+1==Word(native.InstanceBuffer,0)?native.InstanceBuffer.Length:(int)Word(native.InstanceBuffer,12+index*4);var leaf=new Chunk{InstanceBuffer=native.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(native.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(native.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};foreach(uint slot in Slots(leaf.RelocationBuffer))Put(leaf.InstanceBuffer,(int)slot,Word(leaf.InstanceBuffer,(int)slot)-(uint)start);return leaf;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all shared Core occurrences are consumed exactly once by strong import slots, including repeated sound targets and allocated-empty reference pointers. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckOccurrences(Chunk native,InstanceDeclaration instance){if(!Slots(native.ImportsBuffer).Select(s=>Word(native.InstanceBuffer,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Client shared occurrence consumption differs.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only independently valid linked target catalogs supply client sound identities. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);if(!document.Header.IsLinked || document.Validate().Count!=0)throw new InvalidDataException("Client target catalog invalid.");return document;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw malformed source/native/dispatch controls must refuse rather than skipping complete client leaves. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or FormatException or OverflowException){return;}throw new InvalidDataException("Malformed client graph accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded source parsing disables DTDs/external entities and retains authored whitespace. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string source){using var input=new StringReader(source);using var reader=XmlReader.Create(input,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var xml=new XmlDocument{XmlResolver=null,PreserveWhitespace=true};xml.Load(reader);return xml;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact schema namespaces distinguish official leaves from foreign lookalikes. */
    //-------------------------------------------------------------------------------------------------
    private static XmlNamespaceManager Ns(XmlNode node){var ns=new XmlNamespaceManager(node.OwnerDocument!.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);return ns;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: corruption controls own all three native buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact empty/sentinel-framed auxiliary arrays retain order and reject duplicates. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Client auxiliary framing differs.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Client duplicate auxiliary slot.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: newly extracted streams keep their real Tracker slot order and one sentinel when nonempty. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.ToArray();if(values.Length==0)return Array.Empty<byte>();var bytes=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(bytes,i*4,values[i]);Put(bytes,values.Length*4,uint.MaxValue);return bytes;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native/token reads never escape their bounded buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int at){if(at<0 || at>bytes.Length-4)throw new InvalidDataException("Client word out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned mutation/extraction buffers receive native pointer edits. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
}
