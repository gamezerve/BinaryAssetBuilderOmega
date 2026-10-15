using System.Buffers.Binary;
using System.Globalization;
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

// Reborn: the common EP1 AI/chooser native port uses independent stock bytes and actual Core references, not complete GameObject token/publication authority.
internal static class Ep1AIUpdateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare two complete selected stock graphs, every state/locomotor literal, source defaults and actual inline AI root dispatch. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest,bool includeTokens=false)
    {
        var saved=Settings.Current;
        try
        {
            if(Marshal.SizeOf<Ep1AIUpdateModuleData>()!=76 || Marshal.SizeOf<Ep1UnitAITargetChooserData>()!=40 || Marshal.OffsetOf<Ep1AIUpdateModuleData>("StateMachine").ToInt32()!=60 || Marshal.OffsetOf<Ep1AIUpdateModuleData>("UnitAITargetChooserData").ToInt32()!=68 || Marshal.OffsetOf<Ep1UnitAITargetChooserData>("TargetingCompareList").ToInt32()!=36)throw new InvalidDataException("EP1 AI ABI differs.");
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed AI schema required.");
            string[] states=EnumNames(official,"StateMachineType"),locomotors=EnumNames(official,"LocomotorSetType");if(!states.SequenceEqual(Enum.GetNames<Ep1AIStateMachineType>()) || locomotors.Length!=20 || locomotors.Where((n,i)=>(int)Enum.Parse<Ep1RootLocomotorSetType>(n)!=i-1).Any() || !EnumNames(official,"TargetChooserDynamicTargetFlag").SequenceEqual(Enum.GetNames<Ep1DynamicTargetFlag>()) || !EnumNames(official,"AutoAcquireEnemiesType").SequenceEqual(Enum.GetNames<AutoAcquireEnemiesType>()))throw new InvalidDataException("EP1 AI enum declaration drift.");
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AIUpdatePipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);Drift(official,schemas.Schemas);
            var catalog=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();uint targetType=FastHash.GetHashCode("TargetingCompareList");var target=catalog.Single(a=>a.TypeId==targetType && a.Name=="TargetingCompareList:DefaultTargetingCompareList");
            Stock("AlliedConstructionYard","<AIUpdate id='ModuleTag_AIUpdate' AutoAcquireEnemiesWhenIdle='NO' StateMachine='StructureAIStateMachine'/>");
            Stock("SovietAntiInfantryInfantry","<AIUpdate id='ModuleTag_AI' AutoAcquireEnemiesWhenIdle='YES' StateMachine='UnitAIStateMachine'><UnitAITargetChooserData/></AIUpdate>");
            int cases=0;
            foreach(string state in states)foreach(string locomotor in locomotors)foreach(bool chooser in new[]{false,true})
            {
                int profile=cases%3;string text=$"<AIUpdate id='AuthoredAI' StateMachine='{state}' DefaultLocomotorSet='{locomotor}'"+(profile==0?"":profile==1?" AttackPriority='' SpecialContactPoints=''":" AttackPriority='Attack &amp; Defend' SpecialContactPoints='LEFT CENTER RIGHT'")+">"+(chooser?"<UnitAITargetChooserData/>":"")+"</AIUpdate>";var pair=Compile(text);var native=pair.Native;var b=native.InstanceBuffer;
                if(Word(b,0)!=0x08DBC00E || Word(b,4)!=FastHash.GetHashCode("authoredai") || Word(b,60)!=(uint)Array.IndexOf(states,state) || unchecked((int)Word(b,64))!=(int)Enum.Parse<Ep1RootLocomotorSetType>(locomotor) || Word(b,20)!=BitConverter.SingleToUInt32Bits(500) || Word(b,36)!=BitConverter.SingleToUInt32Bits(80) || Word(b,52)!=1 || b[72]!=0 || b[73]!=0)throw new InvalidDataException("Authored AI enum/default fields differ.");
                if(ReadText(b,8)!=(profile==2?"Attack & Defend":"") || Word(b,40)!=(profile==2?3u:0u))throw new InvalidDataException("AI authored string/list differs.");if(profile==2){int list=(int)Word(b,44);var words=new[]{"LEFT","CENTER","RIGHT"};for(int i=0;i<3;i++)if(ReadText(b,list+i*8)!=words[i])throw new InvalidDataException("AI contact-point order differs.");}
                CheckChooser(native,chooser,target,pair.Instance);var repeat=Compile(text).Native;if(!b.SequenceEqual(repeat.InstanceBuffer) || !native.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !native.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))throw new InvalidDataException("AI native repeat differs.");cases++;
                // Reborn: nonempty contacts have native support but no admitted element-token contract; test their refusal plus an explicitly separate empty-contact source variant.
                if(includeTokens){if(profile==2){Ep1AIUpdateTokenSmokeTest.RefuseContacts(native);Ep1AIUpdateTokenSmokeTest.Check(Compile(text.Replace(" SpecialContactPoints='LEFT CENTER RIGHT'"," SpecialContactPoints=''",StringComparison.Ordinal)).Native);}else Ep1AIUpdateTokenSmokeTest.Check(native);}
            }
            for(int mask=0;mask<32;mask++){string value=string.Join(" ",Enum.GetNames<AutoAcquireEnemiesType>().Where((_,i)=>(mask&(1<<i))!=0));var native=Compile("<AIUpdate id='Flags' AutoAcquireEnemiesWhenIdle='"+value+"'/>").Native;if(Word(native.InstanceBuffer,16)!=mask)throw new InvalidDataException("AI acquire mask differs.");}
            for(int mask=0;mask<16;mask++){string value=string.Join(" ",Enum.GetNames<Ep1DynamicTargetFlag>().Where((_,i)=>(mask&(1<<i))!=0));var native=Compile("<AIUpdate id='Flags'><UnitAITargetChooserData CanPickDynamicTargetType='"+value+"'/></AIUpdate>").Native;int chooser=(int)Word(native.InstanceBuffer,68);if(Word(native.InstanceBuffer,chooser+16)!=mask)throw new InvalidDataException("AI chooser dynamic mask differs.");}
            for(int mask=0;mask<64;mask++){string[] names={"CheckVisionRange","CanPickDynamicTargets","RotateToTargetWhenAiming","CanAutoAcquireNonAutoAcquirable","CanChaseAttack","CanOnlyAttackTargetsInFront"};string attrs=string.Join(" ",names.Select((n,i)=>n+"='"+((mask&(1<<i))!=0?"true":"false")+"'"));var native=Compile("<AIUpdate id='Flags'><UnitAITargetChooserData "+attrs+"/></AIUpdate>").Native;int chooser=(int)Word(native.InstanceBuffer,68);for(int i=0;i<6;i++)if(native.InstanceBuffer[chooser+28+i]!=((mask>>i)&1))throw new InvalidDataException("AI chooser bool packing differs.");}
            var edits=Compile("<AIUpdate id='Edits' StopChaseDistance='23.5' StandGround='true' FadeOnPortals='true' MinCowerTime='1.25s' MaxCowerTime='2.5s' BurningDeathTime='3s' ComboLocoAttackDistance='45' IdleTargetTime='4s' MaxCombineChildren='0xFFFFFFFF' SpawnOffsetRadius='-8'><UnitAITargetChooserData IdleScanDelay='2s' ReacquireDelay='3s' StartupDelay='4s' SympathyRange='-5'/></AIUpdate>").Native;
            foreach(var field in new[]{(20,23.5f),(24,1.25f),(28,2.5f),(32,3f),(36,45f),(48,4f),(56,-8f)})if(Word(edits.InstanceBuffer,field.Item1)!=BitConverter.SingleToUInt32Bits(field.Item2))throw new InvalidDataException("AI edited primitive differs.");if(Word(edits.InstanceBuffer,52)!=uint.MaxValue || edits.InstanceBuffer[72]!=1 || edits.InstanceBuffer[73]!=1)throw new InvalidDataException("AI edited uint/flags differ.");int editedChooser=(int)Word(edits.InstanceBuffer,68);foreach(var field in new[]{(0,2f),(4,3f),(8,4f),(12,-5f)})if(Word(edits.InstanceBuffer,editedChooser+field.Item1)!=BitConverter.SingleToUInt32Bits(field.Item2))throw new InvalidDataException("AI chooser edited scalar differs.");
            foreach(string literal in new[]{"0","0.0","0x0","4294967295"}){var b=Compile("<AIUpdate id='Unsigned' MaxCombineChildren='"+literal+"'/>").Native.InstanceBuffer;if(Word(b,52)!=(literal=="4294967295"?uint.MaxValue:0u))throw new InvalidDataException("AI unsigned lexical value differs.");}
            // Reborn: the admitted text and list limits are positively exercised, not only rejected just beyond their boundary.
            var textBoundary=Compile("<AIUpdate id='Boundary' AttackPriority='"+new string('R',4096)+"'/>").Native;if(ReadText(textBoundary.InstanceBuffer,8)!=new string('R',4096))throw new InvalidDataException("AI text boundary differs.");var listBoundary=Compile("<AIUpdate id='Boundary' SpecialContactPoints='"+string.Join(" ",Enumerable.Repeat("POINT",32))+"'/>").Native;if(Word(listBoundary.InstanceBuffer,40)!=32)throw new InvalidDataException("AI contact-list boundary differs.");
            // Reborn: the token boundary also admits the maximum reviewed ASCII text while explicitly refusing the native-only contact-list maximum.
            if(includeTokens){Ep1AIUpdateTokenSmokeTest.Check(textBoundary);Ep1AIUpdateTokenSmokeTest.RefuseContacts(listBoundary);}
            // Reborn: every admitted official boolean spelling must preserve all eight packed AI/chooser bits, including numeric zero/one.
            foreach(string literal in new[]{"true","True","1","false","False","0"}){var b=Compile("<AIUpdate id='Boolean' StandGround='"+literal+"' FadeOnPortals='"+literal+"'><UnitAITargetChooserData "+string.Join(" ",new[]{"CheckVisionRange","CanPickDynamicTargets","RotateToTargetWhenAiming","CanAutoAcquireNonAutoAcquirable","CanChaseAttack","CanOnlyAttackTargetsInFront"}.Select(n=>n+"='"+literal+"'"))+"/></AIUpdate>").Native.InstanceBuffer;byte expected=literal is "true" or "True" or "1"?(byte)1:(byte)0;int chooser=(int)Word(b,68);if(b[72]!=expected || b[73]!=expected || b.AsSpan(chooser+28,6).ToArray().Any(v=>v!=expected))throw new InvalidDataException("AI numeric/named boolean spelling differs.");}
            InlineRoot();int failures=0;
            foreach(string bad in new[]{"<AIUpdate id='Bad' UnknownAIField='1'/>","<AIUpdate id='Bad' StateMachine='UNKNOWN'/>","<AIUpdate id='Bad' StateMachine='7'/>","<AIUpdate id='Bad' DefaultLocomotorSet='UNKNOWN'/>","<AIUpdate id='Bad' AutoAcquireEnemiesWhenIdle='32'/>","<AIUpdate id='Bad' MaxCombineChildren='1.5'/>","<AIUpdate id='Bad' MaxCombineChildren='4294967296'/>","<AIUpdate id='Bad'><UnitAITargetChooserData CanPickDynamicTargetType='UNKNOWN'/></AIUpdate>","<AIUpdate id='Bad'><UnitAITargetChooserData TargetingCompareList='MissingRebornCompare'/></AIUpdate>","<AIUpdate id='Bad'><UnitAITargetChooserData TargetingCompareList='WeaponTemplate:WrongType'/></AIUpdate>","<AIUpdate id='Bad'><UnitAITargetChooserData><ObjectVisionRange><Filter/></ObjectVisionRange></UnitAITargetChooserData></AIUpdate>"}){Refuse(()=>Compile(bad));failures++;}
            Refuse(()=>Compile("<AIUpdate id='Bad'><UnitAITargetChooserData/></AIUpdate>",catalog.Where(a=>a!=target).ToArray()));failures++;Refuse(()=>Compile("<AIUpdate id='Bad'><UnitAITargetChooserData/></AIUpdate>",catalog.Append(target).ToArray()));failures++;
            // Reborn: schema-legal exponents can overflow float32; native admission must reject them after normalization too.
            foreach(string bad in new[]{"<AIUpdate id='Bad' StopChaseDistance='1e999'/>","<AIUpdate id='Bad'><UnitAITargetChooserData SympathyRange='-1e999'/></AIUpdate>"}){Refuse(()=>Compile(bad));failures++;}
            // Reborn: refuse unsupported text encoding/size/list counts before invoking the legacy ANSI allocator.
            foreach(string bad in new[]{"<AIUpdate id='Bad' AttackPriority='é'/>","<AIUpdate id='Bad' SpecialContactPoints='LEFT é'/>","<AIUpdate id='Bad' AttackPriority='"+new string('R',4097)+"'/>","<AIUpdate id='Bad' SpecialContactPoints='"+string.Join(" ",Enumerable.Repeat("P",33))+"'/>"}){Refuse(()=>Compile(bad));failures++;}
            if(includeTokens)Ep1AIUpdateTokenSmokeTest.Controls(Compile("<AIUpdate id='TokenControls' AttackPriority='AI priority'><UnitAITargetChooserData/></AIUpdate>").Native);
            Console.WriteLine($"EP1 AIUpdate: PASS (two complete selected stock native graphs; {cases} state/locomotor/chooser/string cases, 32 acquire/16 dynamic/64 chooser-bool masks, edits/defaults/unsigned syntax; inline AI counts 0/1/2 and real concrete dispatch; {failures} source/dependency refusals; empty ObjectVisionRange only; "+(includeTokens?"generated bounded AI fragments, empty contact-token lists only; ":"no generated AI tokens; ")+"no shared combat projection/general object package/game).");
            //-------------------------------------------------------------------------------------------------
            /** Reborn: actual Core normalization supplies occurrence indices; admission requires unique independently catalogued type/instance identities. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Compile(string leaf,ManifestAsset[]? admitted=null)
            {
                string text=WithNamespace(leaf);Validate(official,text);var instance=ReferencePipelineSmokeTest.Normalize(schemas,text);foreach(var reference in instance.ReferencedInstances)if((admitted??catalog).Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("AI dependency missing/wrong/ambiguous.");return(Native(instance.XmlNode),instance);
            }
            //-------------------------------------------------------------------------------------------------
            /** Reborn: independent stock field tags/offsets, all native bytes and semantic strong identities are checked without captured source indices. */
            //-------------------------------------------------------------------------------------------------
            void Stock(string name,string leaf)
            {
                var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var module=GameObjectModuleReview.Inspect(manifest,"GameObject:"+name).Containers.Single(c=>c.Name=="AI").Modules.Single();if(module.TypeId!=0x08DBC00E)throw new InvalidDataException("Stock AI concrete differs.");var fields=GameObjectStockReview.Tokens(stock.Bytes,module.Wrapper+12);
                foreach(var f in new[]{("AttackPriority",8,0x1A2933AAu),("AutoAcquireEnemiesWhenIdle",16,0x8B0FB9DAu),("StopChaseDistance",20,0x645864F3u),("StandGround",72,0x77E3F4F7u),("MinCowerTime",24,0x998669E8u),("MaxCowerTime",28,0x998669E8u),("BurningDeathTime",32,0x998669E8u),("ComboLocoAttackDistance",36,0x645864F3u),("FadeOnPortals",73,0x77E3F4F7u),("IdleTargetTime",48,0x998669E8u),("MaxCombineChildren",52,0x763177A4u),("SpawnOffsetRadius",56,0x645864F3u),("StateMachine",60,0xAC6E35E0u),("DefaultLocomotorSet",64,0x845D12B2u)}){var field=fields[FastHash.GetHashCode(f.Item1)];if(field.Type!=f.Item3 || field.Offset!=module.Native+f.Item2)throw new InvalidDataException("Independent stock AI field binding differs.");}if(fields.Count!=17)throw new InvalidDataException("Stock AI own-field inventory differs.");
                var contacts=fields[FastHash.GetHashCode("SpecialContactPoints")];if(contacts.Type!=0x6D66AE18 || Word(stock.Bytes,contacts.Offset)!=0x6C827073 || Word(stock.Bytes,contacts.Offset+4)!=0x5D448FD8 || Word(stock.Bytes,contacts.Offset+8)!=module.Native+40 || Word(stock.Bytes,module.Native+40)!=0 || Word(stock.Bytes,module.Native+44)!=0)throw new InvalidDataException("Selected stock AI empty contact-list differs.");
                var pair=Compile(leaf);var expected=stock.Bytes.AsSpan(module.Native,76).ToArray();uint original=Word(expected,68),actual=Word(pair.Native.InstanceBuffer,68);Put(expected,68,actual);if(!expected.AsSpan().SequenceEqual(pair.Native.InstanceBuffer.AsSpan(0,76)) || (original==0)!=(actual==0))throw new InvalidDataException("Complete stock AI native record differs.");
                var chooserField=fields[FastHash.GetHashCode("UnitAITargetChooserData")];if(chooserField.Type!=0x243BEF01 || (chooserField.Offset==0)!=(original==0))throw new InvalidDataException("Stock chooser pointer/table presence differs.");
                if(original!=0){StockChooserFields(stock.Bytes,chooserField.Offset,(int)original);var chooser=stock.Bytes.AsSpan((int)original,40).ToArray();uint stockReference=Word(chooser,36);if(stockReference==0 || stockReference>stock.Asset.References.Count || stock.Asset.References[(int)stockReference-1]!=new AssetId(target.TypeId,target.InstanceId))throw new InvalidDataException("Stock chooser reference identity differs.");Put(chooser,36,Word(pair.Native.InstanceBuffer,(int)actual+36));if(!chooser.AsSpan().SequenceEqual(pair.Native.InstanceBuffer.AsSpan((int)actual,40)))throw new InvalidDataException("Complete stock EP1 chooser record differs.");}
                CheckChooser(pair.Native,original!=0,target,pair.Instance);
                // Reborn: token follow-up compares the complete independent stock prefix, not just a generated table against another generated table.
                if(includeTokens)Ep1AIUpdateTokenSmokeTest.Stock(pair.Native,stock.Bytes,module.Wrapper,module.Native);
            }
            //-------------------------------------------------------------------------------------------------
            /** Reborn: one real shared Core occurrence table feeds the existing inline AI root container and the new concrete EP1 dispatch. */
            //-------------------------------------------------------------------------------------------------
            void InlineRoot()
            {
                foreach(int count in new[]{0,1,2}){string source="<AIProbe xmlns='"+SchemaSet.XmlNamespace+"' id='RootAI'>"+string.Concat(Enumerable.Range(0,count).Select(i=>"<AIUpdate id='AI"+i+"'><UnitAITargetChooserData/></AIUpdate>"))+"</AIProbe>";var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);var output=Root(instance.XmlNode);if(Word(output.InstanceBuffer,480)!=count || instance.ReferencedInstances.Count!=count)throw new InvalidDataException("Inline AI source/count differs.");int items=(int)Word(output.InstanceBuffer,484);for(int i=0;i<count;i++){int module=(int)Word(output.InstanceBuffer,items+i*4),chooser=(int)Word(output.InstanceBuffer,module+68);if(Word(output.InstanceBuffer,module)!=0x08DBC00E || Word(output.InstanceBuffer,module+4)!=FastHash.GetHashCode("ai"+i) || Word(output.InstanceBuffer,chooser+36)!=i+1 || instance.ReferencedInstances[i].TypeId!=target.TypeId || instance.ReferencedInstances[i].InstanceId!=target.InstanceId)throw new InvalidDataException("Inline AI dispatch/shared reference differs.");}if(!Slots(output.ImportsBuffer).SequenceEqual(Enumerable.Range(0,count).Select(i=>Word(output.InstanceBuffer,(int)Word(output.InstanceBuffer,items+i*4)+68)+36)))throw new InvalidDataException("Inline AI import ownership differs.");}
                Refuse(()=>Root(Xml("<AIProbe xmlns='"+SchemaSet.XmlNamespace+"'>"+string.Concat(Enumerable.Range(0,3).Select(i=>"<AIUpdate id='AI"+i+"'/>"))+"</AIProbe>").DocumentElement!));
                // Reborn: until AI metadata composition is implemented, an otherwise valid selected Body/SMC graph containing AI must fail rather than publish a partial object.
                var normalized=ReferencePipelineSmokeTest.Normalize(schemas,"<AIProbe xmlns='"+SchemaSet.XmlNamespace+"' id='UnpublishedAI'><AIUpdate id='AI'><UnitAITargetChooserData/></AIUpdate></AIProbe>");var graph=Xml("<GameObject xmlns='"+SchemaSet.XmlNamespace+"' id='UnpublishedAI'><Behaviors><ObjectSMCHelper id='Helper'/></Behaviors><AI/><Body><ActiveBody id='Body' MaxHealth='100'/></Body></GameObject>");var admitted=(XmlElement)graph.ImportNode(normalized.XmlNode.ChildNodes.OfType<XmlElement>().Single(),true);admitted.SetAttribute("TypeId","148619278");graph.GetElementsByTagName("AI",SchemaSet.XmlNamespace)[0]!.AppendChild(admitted);var native=GameObjectBodyProjectionSmokeTest.Compile(graph,selectedBehaviors:true);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(native));
            }
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock chooser inherited fields anchor every native offset independently, including packed booleans and empty vision-list storage. */
    //-------------------------------------------------------------------------------------------------
    private static void StockChooserFields(byte[] bytes,int table,int native)
    {
        var own=GameObjectStockReview.Tokens(bytes,table);if(own.Count!=2 || !own.TryGetValue(1,out var parent) || parent.Type!=0xAA42EB9C || own[FastHash.GetHashCode("TargetingCompareList")].Type!=0x1B85B469 || own[FastHash.GetHashCode("TargetingCompareList")].Offset!=native+36)throw new InvalidDataException("Stock chooser own/base metadata differs.");var fields=GameObjectStockReview.Tokens(bytes,parent.Offset);if(fields.Count!=12)throw new InvalidDataException("Stock chooser base inventory differs.");
        foreach(var f in new[]{("IdleScanDelay",0,0x998669E8u),("ReacquireDelay",4,0x998669E8u),("StartupDelay",8,0x998669E8u),("SympathyRange",12,0x645864F3u),("CanPickDynamicTargetType",16,0x344E0F38u),("CheckVisionRange",28,0x77E3F4F7u),("CanPickDynamicTargets",29,0x77E3F4F7u),("RotateToTargetWhenAiming",30,0x77E3F4F7u),("CanAutoAcquireNonAutoAcquirable",31,0x77E3F4F7u),("CanChaseAttack",32,0x77E3F4F7u),("CanOnlyAttackTargetsInFront",33,0x77E3F4F7u)}){var field=fields[FastHash.GetHashCode(f.Item1)];if(field.Type!=f.Item3 || field.Offset!=native+f.Item2)throw new InvalidDataException("Stock chooser primitive field binding differs.");}
        var vision=fields[FastHash.GetHashCode("ObjectVisionRange")];if(vision.Type!=0x2E11FC54 || Word(bytes,vision.Offset)!=0x6C827073 || Word(bytes,vision.Offset+4)!=0x5D448FD8 || Word(bytes,vision.Offset+8)!=native+20 || Word(bytes,native+20)!=0 || Word(bytes,native+24)!=0)throw new InvalidDataException("Selected stock chooser empty vision list differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: standalone source receives the concrete wire ID from the EP1 marshaler, not a hash of the separate C# type name. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Native(XmlNode node)
    {
        var ns=new XmlNamespaceManager(node.OwnerDocument!.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1AIUpdateModuleData* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1AIUpdateModuleData),false);Marshaler.Marshal(new Node(node.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("AI native allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the real root marshaler owns inline count/items and uses the concrete dispatch; validation/source preparation is deliberately separate. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Root(XmlNode probe)
    {
        var source=Xml("<GameObject xmlns='"+SchemaSet.XmlNamespace+"'><AI/></GameObject>");var ai=source.DocumentElement!.FirstChild!;foreach(XmlElement child in probe.ChildNodes.OfType<XmlElement>()){var imported=(XmlElement)source.ImportNode(child,true);imported.SetAttribute("TypeId","148619278");ai.AppendChild(imported);}var ns=new XmlNamespaceManager(source.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);GameObject* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(GameObject),false);Marshaler.Marshal(new Node(source.DocumentElement.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("AI root allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: optional chooser defaults, exact semantic import, native slot ownership and padding remain independently visible. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckChooser(Chunk native,bool present,ManifestAsset target,InstanceDeclaration instance)
    {
        var b=native.InstanceBuffer;int chooser=(int)Word(b,68);var expected=new List<uint>();if(Word(b,12)!=0)expected.Add(12);if(Word(b,44)!=0){expected.Add(44);int items=(int)Word(b,44);for(int i=0;i<Word(b,40);i++)if(Word(b,items+i*8+4)!=0)expected.Add((uint)(items+i*8+4));}if(present)expected.Add(68);if(!Slots(native.RelocationBuffer).Order().SequenceEqual(expected.Order()))throw new InvalidDataException("AI native relocation ownership differs.");
        if(!present){if(chooser!=0 || instance.ReferencedInstances.Count!=0 || native.ImportsBuffer.Length!=0)throw new InvalidDataException("Absent AI chooser retains references/storage.");return;}if(chooser<76 || chooser>b.Length-40 || instance.ReferencedInstances.Count!=1 || instance.ReferencedInstances[0].TypeId!=target.TypeId || instance.ReferencedInstances[0].InstanceId!=target.InstanceId || Word(b,chooser+36)!=1 || !Slots(native.ImportsBuffer).SequenceEqual(new[]{(uint)(chooser+36)}))throw new InvalidDataException("AI chooser real reference/owner differs.");
        foreach(var value in new[]{(0,0.8f),(4,60f),(8,0f),(12,25f)})if(Word(b,chooser+value.Item1)!=BitConverter.SingleToUInt32Bits(value.Item2))throw new InvalidDataException("AI chooser default time/scalar differs.");if(Word(b,chooser+16)!=8 || Word(b,chooser+20)!=0 || Word(b,chooser+24)!=0 || !b.AsSpan(chooser+28,8).SequenceEqual(new byte[]{0,1,0,0,1,0,0,0}))throw new InvalidDataException("AI chooser default flags/list/padding differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: included official declarations must retain all attribute/default/use contracts of the admitted schema candidate. */
    //-------------------------------------------------------------------------------------------------
    private static void Drift(XmlSchemaSet official,XmlSchemaSet focused)
    {
        foreach(string name in new[]{"AIUpdateModuleData","UnitAITargetChooserData","BaseAITargetChooserData"}){var key=new XmlQualifiedName(name,SchemaSet.XmlNamespace);var a=(XmlSchemaComplexType)official.GlobalTypes[key]!;var b=(XmlSchemaComplexType)focused.GlobalTypes[key]!;if(a.AttributeUses.Count!=b.AttributeUses.Count)throw new InvalidDataException("AI attribute inventory drift.");foreach(XmlSchemaAttribute f in a.AttributeUses.Values){var other=(XmlSchemaAttribute)b.AttributeUses[f.QualifiedName]!;if(other==null || other.DefaultValue!=f.DefaultValue || other.Use!=f.Use || other.AttributeSchemaType!.QualifiedName!=f.AttributeSchemaType!.QualifiedName)throw new InvalidDataException("AI attribute contract drift.");}}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw modules validate in their actual official AI container before focused normalization or internally injected TypeId. */
    //-------------------------------------------------------------------------------------------------
    private static void Validate(XmlSchemaSet schemas,string leaf)=>GameObjectBodyProjectionSmokeTest.Validate(schemas,"<GameObject xmlns='"+SchemaSet.XmlNamespace+"' id='AIValidation'><Behaviors/><AI>"+leaf+"</AI><Body><ActiveBody id='Body' MaxHealth='100'/></Body></GameObject>");
    //-------------------------------------------------------------------------------------------------
    /** Reborn: official literal order is read independently, including the locomotor enum's leading negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static string[] EnumNames(XmlSchemaSet schemas,string name)=>((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)schemas.GlobalTypes[new XmlQualifiedName(name,SchemaSet.XmlNamespace)]!).Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f=>f.Value!).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all normalized reference identities must come from valid linked EP1 metadata. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("AI catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw source cannot supply compiler reference indices or concrete TypeId words. */
    //-------------------------------------------------------------------------------------------------
    private static string WithNamespace(string text){if(text.Contains('\\') || text.Contains("TypeId",StringComparison.Ordinal))throw new InvalidDataException("Authored AI compiler indices/TypeId refused.");return text.Replace("<AIUpdate ","<AIUpdate xmlns='"+SchemaSet.XmlNamespace+"' ",StringComparison.Ordinal);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed source, unsupported child graphs and dependency failures remain hard refusals, not partial success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or FormatException or OverflowException or BinaryAssetBuilderException){return;}throw new InvalidDataException("Invalid AI source admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded ASCII header inspection is separate from arbitrary Unicode/localization support. */
    //-------------------------------------------------------------------------------------------------
    private static string ReadText(byte[] b,int at){uint length=Word(b,at),pointer=Word(b,at+4);if(pointer==0){if(length!=0)throw new InvalidDataException("AI missing string has length.");return "";}if(pointer>=b.Length || length>b.Length-pointer-1 || b[(int)(pointer+length)]!=0)throw new InvalidDataException("AI string bounds differ.");return Encoding.ASCII.GetString(b,(int)pointer,(int)length);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled XML parsing never resolves external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument{XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: empty or sentinel-framed slot streams have explicit bounds. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("AI auxiliary framing differs.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock pointers are interpreted only as bounded little-endian offsets. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("AI word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: canonical stock comparisons modify only private copied buffers. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
