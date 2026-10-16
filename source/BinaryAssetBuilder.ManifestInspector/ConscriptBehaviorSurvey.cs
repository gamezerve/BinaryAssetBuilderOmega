using System.Buffers.Binary;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk = Relo.Chunk;
using Tracker = Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: survey every original Conscript behavior through complete source/Core/native comparison; blocked rows are explicit incomplete work, never successful module admission.
internal static class ConscriptBehaviorSurvey
{
    internal sealed record Row(int Index,string Id,string Type,string Source,string Status,string Stage,string Detail,int Bin,int Relo,int Imp,int References);
    // Reborn: retain native-only evidence separately from optional complete shared metadata qualification.
    private sealed record SharedResult(int References,int Bin,int Relo,int Imp,int Refusals,int MetadataPrefix=0,int MetadataRefusals=0,int AuthoredContainers=0);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind all 36 original stock identities to the explicit read-only source chain, then retain each full leaf while reporting schema/reference/compiler/ABI blockers separately. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string sourceRoot,string manifest,string globalManifest,bool requireAll=false,bool includeTokens=false,bool requireTokens=false,bool compose=false)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed behavior schema required.");
            var alias=new XmlSchema{TargetNamespace=SchemaSet.XmlNamespace,ElementFormDefault=XmlSchemaForm.Qualified};alias.Items.Add(new XmlSchemaElement{Name="BehaviorsSurveyProbe",SchemaType=new XmlSchemaComplexType{ContentModel=new XmlSchemaComplexContent{Content=new XmlSchemaComplexContentExtension{BaseTypeName=new XmlQualifiedName("BaseAssetType",SchemaSet.XmlNamespace),Particle=new XmlSchemaSequence{Items={new XmlSchemaElement{Name="Behaviors",SchemaTypeName=new XmlQualifiedName("BehaviorModules",SchemaSet.XmlNamespace)}}}}}}});official.Add(alias);official.Compile();
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(XmlSchema seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();(typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)??throw new MissingMethodException("Behavior schema helper missing.")).Invoke(schemas,null);
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;var catalog=stock.Document.Assets.Concat(Catalog(globalManifest).Assets).ToArray();
            var paths=new[]{"BaseObjects/BaseObject.xml","BaseObjects/BaseInfantry.xml","Soviet/Units/SovietAntiInfantryInfantry.xml","GlobalData/GlobalDefines.xml"};var captures=paths.Select(p=>SdkEnvironmentPreflight.Read(Path.Combine(sourceRoot,p),1048576)).ToArray();var documents=captures.Select(b=>Xml(Encoding.UTF8.GetString(b).TrimStart('\uFEFF'))).ToArray();var objects=documents.Take(3).Select(d=>d.GetElementsByTagName("GameObject",SchemaSet.XmlNamespace).OfType<XmlElement>().Single()).ToArray();if(objects[0].HasAttribute("inheritFrom") || objects[1].GetAttribute("inheritFrom")!="BaseObject" || objects[2].GetAttribute("inheritFrom")!="BaseInfantry")throw new InvalidDataException("Explicit Conscript behavior chain changed.");
            // Reborn: capture one actual external definition, not a guessed stock literal or a substitute general include/expression engine.
            string scaler=documents[3].GetElementsByTagName("Define",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.GetAttribute("name")=="MAX_SECONDARY_DAMAGE_SCALER").GetAttribute("value");if(scaler!="200%")throw new InvalidDataException("Selected official secondary-damage definition drift.");
            var leaves=objects.SelectMany((o,i)=>o.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Behaviors" && e.NamespaceURI==SchemaSet.XmlNamespace).SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).Select(e=>(Leaf:e,Path:paths[i]))).ToArray();
            if(modules.Length!=36 || leaves.Length!=36 || leaves.Select(l=>FastHash.GetHashCode(l.Leaf.GetAttribute("id").ToLowerInvariant())).Distinct().Count()!=36)throw new InvalidDataException("All 36 distinct complete behavior sources required.");
            var rows=new List<Row>();int corruptions=0;var tokenRows=new List<object>();int tokensVerified=0,tokenRefusals=0,totalPrefix=0;
            // Reborn: this independent review is diagnostic evidence only; the runtime generator never accepts this stock-derived catalog as input.
            var typeReview=includeTokens?BehaviorMetadataTypeReview.Collect(official,stock.Bytes,modules,stock.Projection.NativeRoot):new SortedDictionary<string,uint>();
            for(int i=0;i<modules.Length;i++)
            {
                var module=modules[i];uint id=Word(stock.Bytes,module.Native+4);var source=leaves.Single(l=>FastHash.GetHashCode(l.Leaf.GetAttribute("id").ToLowerInvariant())==id);string stage="Schema";Chunk? native=null;InstanceDeclaration? instance=null;
                try
                {
                    var envelope=Xml("<BehaviorsSurveyProbe xmlns='uri:ea.com:eala:asset' id='RebornBehaviorSurvey'><Behaviors/></BehaviorsSurveyProbe>");envelope.DocumentElement!.FirstChild!.AppendChild(envelope.ImportNode(source.Leaf,true));Prepare((XmlElement)envelope.DocumentElement.FirstChild!.FirstChild!,scaler);envelope.Schemas.Add(official);envelope.Validate((_,e)=>throw new InvalidDataException(e.Message));var leaf=(XmlElement)envelope.DocumentElement.FirstChild!.FirstChild!;
                    if(leaf.SchemaInfo.SchemaType?.Name!=module.Type)throw new InvalidDataException("Source/stock concrete schema type differs.");stage="References";ResolveStrong(envelope,schemas,catalog);instance=ReferencePipelineSmokeTest.Normalize(schemas,envelope.OuterXml);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Behavior target missing/ambiguous: "+reference.Name);
                    stage="Compile";native=Compile(instance.XmlNode.FirstChild!.FirstChild!,module.TypeId);stage="Native";Type type=NativeType(module.TypeId,module.Type);var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),native,instance,captureNativeCoordinates:false,captureNativePaths:includeTokens);comparer.Compare(type,module.Native);if(comparer.NativeOffsets.Count!=0)throw new InvalidDataException("Native-only survey leaked metadata coordinates.");
                    var repeated=Compile(instance.XmlNode.FirstChild!.FirstChild!,module.TypeId);if(!native.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer) || !native.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer) || !native.ImportsBuffer.SequenceEqual(repeated.ImportsBuffer))throw new InvalidDataException("Complete behavior repeat differs.");
                    foreach(int at in new[]{0,4,native.InstanceBuffer.Length-1}){var bad=Clone(native);bad.InstanceBuffer[at]^=1;Refuse(()=>new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),bad,instance,captureNativeCoordinates:false).Compare(type,module.Native));corruptions++;}
                    foreach(bool relo in new[]{true,false})if((relo?native.RelocationBuffer:native.ImportsBuffer).Length!=0){var bad=Clone(native);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),bad,instance,captureNativeCoordinates:false).Compare(type,module.Native));corruptions++;}
                    rows.Add(new(i,source.Leaf.GetAttribute("id"),module.Type,source.Path,"NativeVerified","Complete","Complete stock graph, exact auxiliary ownership and semantic Core references; no metadata/package admission.",native.InstanceBuffer.Length,native.RelocationBuffer.Length,native.ImportsBuffer.Length,instance.ReferencedInstances.Count));
                    if(includeTokens)
                    {
                        try
                        {
                            var before=Clone(native);var tokens=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,official,module.Type,type);int end=i+1<modules.Length?modules[i+1].Wrapper:GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("AI")].Offset;int prefix=BehaviorMetadataStockProof.Compare(tokens,native,stock.Bytes,module.Wrapper,end,type,comparer.NativeRecords);
                            // Reborn: repeat production generation and verify both output determinism and unchanged caller-owned native input.
                            var repeat=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,official,module.Type,type);if(!Equal(tokens,repeat) || !Equal(native,before))throw new InvalidDataException("Behavior metadata repeat/input isolation differs.");
                            foreach(int at in new[]{0,4,8,prefix+4}){var bad=Clone(tokens);bad.InstanceBuffer[at]^=1;Refuse(()=>BehaviorMetadataStockProof.Compare(bad,native,stock.Bytes,module.Wrapper,end,type,comparer.NativeRecords));tokenRefusals++;}
                            foreach(bool relo in new[]{true,false})if((relo?tokens.RelocationBuffer:tokens.ImportsBuffer).Length!=0){var bad=Clone(tokens);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>BehaviorMetadataStockProof.Compare(bad,native,stock.Bytes,module.Wrapper,end,type,comparer.NativeRecords));tokenRefusals++;}
                            // Reborn: malformed input identity and orphan heap storage must be rejected by the production writer, not merely by the stock oracle.
                            var wrong=Clone(native);Put(wrong.InstanceBuffer,0,0);Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(wrong,official,module.Type,type));tokenRefusals++;
                            var orphan=Clone(native);orphan.InstanceBuffer=orphan.InstanceBuffer.Concat(new byte[]{0xA5,0,0,0}).ToArray();Refuse(()=>BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(orphan,official,module.Type,type));tokenRefusals++;
                            tokenRows.Add(new{Index=i,Type=module.Type,Status="MetadataVerified",Prefix=prefix});tokensVerified++;totalPrefix+=prefix;
                        }
                        catch(InvalidDataException e){tokenRows.Add(new{Index=i,Type=module.Type,Status="Blocked",Detail=e.Message});}
                    }
                }
                catch(Exception e) when(e is InvalidDataException or BinaryAssetBuilderException or FormatException or OverflowException or NotImplementedException or ArgumentException)
                {rows.Add(new(i,source.Leaf.GetAttribute("id"),module.Type,source.Path,"Blocked",stage,e.Message,native?.InstanceBuffer.Length??0,native?.RelocationBuffer.Length??0,native?.ImportsBuffer.Length??0,instance?.ReferencedInstances.Count??0));}
            }
            for(int i=0;i<paths.Length;i++)if(!SHA256.HashData(captures[i]).SequenceEqual(SHA256.HashData(SdkEnvironmentPreflight.Read(Path.Combine(sourceRoot,paths[i]),1048576))))throw new InvalidDataException("Official behavior source changed during survey.");
            int authored=Authored(official);int authoredReferenceMetadata=includeTokens?AuthoredReferenceMetadata(schemas,official,catalog):0;
            // Reborn: a complete shared graph is attempted only after every standalone full leaf has independently matched stock.
            var shared=rows.All(r=>r.Status=="NativeVerified")?Shared(schemas,modules,modules.Select(m=>leaves.Single(l=>FastHash.GetHashCode(l.Leaf.GetAttribute("id").ToLowerInvariant())==Word(stock.Bytes,m.Native+4)).Leaf).ToArray(),stock.Bytes,stock.Asset.References.ToArray(),catalog,scaler,compose):new SharedResult(0,0,0,0,0);
            // Reborn: source hashes also bracket the shared-container and authored controls, not only the standalone survey loop.
            for(int i=0;i<paths.Length;i++)if(!SHA256.HashData(captures[i]).SequenceEqual(SHA256.HashData(SdkEnvironmentPreflight.Read(Path.Combine(sourceRoot,paths[i]),1048576))))throw new InvalidDataException("Official behavior source changed during shared verification.");
            Console.WriteLine(JsonSerializer.Serialize(new{Profile="ExplicitConscript36BehaviorSurvey",CompleteLeaves=36,DistinctFamilies=modules.Select(m=>m.TypeId).Distinct().Count(),NativeVerified=rows.Count(r=>r.Status=="NativeVerified"),Blocked=rows.Count(r=>r.Status=="Blocked"),CorruptionRefusals=corruptions,AuthoredControls=authored,AuthoredReferenceMetadataControls=authoredReferenceMetadata,SharedNativeVerified=shared.Bin!=0,SharedCoreOccurrences=shared.References,SharedBin=shared.Bin,SharedRelo=shared.Relo,SharedImp=shared.Imp,SharedRefusals=shared.Refusals,SharedMetadataVerified=shared.MetadataPrefix!=0,SharedMetadataPrefix=shared.MetadataPrefix,SharedTokenBin=shared.MetadataPrefix==0?0:shared.MetadataPrefix+shared.Bin,SharedMetadataRefusals=shared.MetadataRefusals,AuthoredContainers=shared.AuthoredContainers,Rows=rows,TokenRows=tokenRows,TokensVerified=tokensVerified,TokenRefusals=tokenRefusals,TotalLeafMetadataBytes=totalPrefix,MetadataTypeReview=typeReview,SourcePreparation="Stable SlowDeath family grouping; one captured GlobalDefines MAX_SECONDARY_DAMAGE_SCALER value",GeneralInheritanceExecuted=false,MetadataGenerated=includeTokens && tokensVerified==36,NativeCoordinateMapExported=false,GameObjectPublicationEnabled=false,GameExecuted=false,InstalledFilesModified=false,AlphaCapabilitiesExpanded=false},new JsonSerializerOptions{WriteIndented=true}));
            if(requireAll && rows.Any(r=>r.Status!="NativeVerified"))throw new InvalidDataException("Complete Conscript native gate has blocked rows: "+string.Join("; ",rows.Where(r=>r.Status!="NativeVerified").Select(r=>r.Id+" ["+r.Stage+"] "+r.Detail)));
            if(requireTokens && (!includeTokens || tokensVerified!=36))throw new InvalidDataException("Complete Conscript metadata gate requires all 36 original leaves; verified="+tokensVerified);
            if(compose && shared.MetadataPrefix==0)throw new InvalidDataException("Complete Conscript shared metadata gate did not run.");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema-derived strong references resolve to one concrete catalog target before production Core normalization; weak IDs and numeric compiler indices are not authored. */
    //-------------------------------------------------------------------------------------------------
    private static void ResolveStrong(XmlDocument document,SchemaSet schemas,ManifestAsset[] catalog)
    {
        foreach(XmlNode node in document.SelectNodes("//* | //@*")!)
        {
            var type=node.SchemaInfo.SchemaType;if(type==null || !XmlSchemaType.IsDerivedFrom(type,schemas.XmlAssetReferenceType,XmlSchemaDerivationMethod.None))continue;string name=(node is XmlAttribute attribute?attribute.Value:node.InnerText).Trim();if(name.Length==0 || name.Contains(':'))continue;if(name.Contains('\\'))throw new InvalidDataException("Authored behavior compiler index refused.");
            string? referenceType=null;var declaration=node is XmlAttribute?node.SchemaInfo.SchemaAttribute?.UnhandledAttributes:node.SchemaInfo.SchemaElement?.UnhandledAttributes;referenceType=Annotation(declaration);
            for(XmlSchemaType? current=type;referenceType==null && current!=null;){referenceType=Annotation(current.UnhandledAttributes);var next=current.BaseXmlSchemaType;if(ReferenceEquals(next,current))break;current=next;}
            if(referenceType==null)throw new InvalidDataException("Strong behavior reference type missing.");var required=schemas.GetXmlType(referenceType)??throw new InvalidDataException("Behavior reference schema missing: "+referenceType);uint id=new InstanceHandle(name).InstanceId;var allowed=schemas.Schemas.GlobalTypes.Values.Cast<XmlSchemaType>().Where(t=>XmlSchemaType.IsDerivedFrom(t,required,XmlSchemaDerivationMethod.None)).Select(t=>FastHash.GetHashCode(t.Name!)).ToHashSet();var targets=catalog.Where(a=>a.InstanceId==id && allowed.Contains(a.TypeId)).ToArray();if(targets.Length!=1)throw new InvalidDataException("Behavior strong target missing/ambiguous: "+referenceType+":"+name);if(node is XmlAttribute a)a.Value=targets[0].Name;else node.InnerText=targets[0].Name;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only the official asset-schema namespace defines reference target annotations. */
        //-------------------------------------------------------------------------------------------------
        static string? Annotation(XmlAttribute[]? attributes)=>attributes?.SingleOrDefault(a=>a.LocalName=="refType" && a.NamespaceURI=="uri:ea.com:eala:asset:schema")?.Value;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: isolate one complete concrete dispatch on a diagnostic root Tracker; the reviewed EP1 weapon route avoids its known incompatible KW layout. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlNode source,uint type)
    {
        var document=Xml(source.OuterXml);document.DocumentElement!.SetAttribute("TypeId",type.ToString(CultureInfo.InvariantCulture));var ns=new XmlNamespaceManager(document.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);var node=new Node(document.DocumentElement.CreateNavigator()!,ns);GameObject* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(GameObject),false);
        Dispatch(node,(BehaviorModuleData**)&root->Body,tracker,type);
        var all=new Chunk();if(!tracker.MakeRelocatable(all))throw new InvalidDataException("Behavior native Tracker failed.");int start=(int)Word(all.InstanceBuffer,488);var relos=Slots(all.RelocationBuffer);var imports=Slots(all.ImportsBuffer);if(start<600 || start>all.InstanceBuffer.Length-8 || !relos.Where(s=>s<start).SequenceEqual(new[]{488u}) || imports.Any(s=>s<start))throw new InvalidDataException("Behavior diagnostic root ownership differs.");var native=new Chunk{InstanceBuffer=all.InstanceBuffer.AsSpan(start).ToArray(),RelocationBuffer=Pack(relos.Where(s=>s>=start).Select(s=>s-(uint)start)),ImportsBuffer=Pack(imports.Select(s=>s-(uint)start))};foreach(uint slot in Slots(native.RelocationBuffer)){uint target=Word(native.InstanceBuffer,(int)slot);if(target<start || target>=all.InstanceBuffer.Length)throw new InvalidDataException("Behavior pointer crosses diagnostic root.");Put(native.InstanceBuffer,(int)slot,target-(uint)start);}if(Word(native.InstanceBuffer,0)!=type)throw new InvalidDataException("Behavior dispatch class differs.");return native;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all 36 original leaves undergo one real Core normalization and one Tracker allocation; independent leaf walkers consume the common occurrence table exactly once. */
    //-------------------------------------------------------------------------------------------------
    private static SharedResult Shared(SchemaSet schemas,GameObjectModuleReview.Module[] modules,XmlElement[] leaves,byte[] stock,AssetId[] stockReferences,ManifestAsset[] catalog,string scaler,bool compose)
    {
        var document=Xml("<BehaviorsSurveyProbe xmlns='uri:ea.com:eala:asset' id='RebornShared36Behaviors'><Behaviors/></BehaviorsSurveyProbe>");foreach(var leaf in leaves){var clone=(XmlElement)document.ImportNode(leaf,true);Prepare(clone,scaler);document.DocumentElement!.FirstChild!.AppendChild(clone);}document.Schemas.Add(schemas.Schemas);document.Validate((_,e)=>throw new InvalidDataException(e.Message));
        // Reborn: the new schema-derived concrete resolver must reject both missing names and duplicate catalog identities before normalization.
        string creation=document.GetElementsByTagName("CreateObjectDie",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().GetAttribute("CreationList");var target=catalog.Single(a=>a.TypeId==FastHash.GetHashCode("ObjectCreationList") && a.InstanceId==new InstanceHandle(creation).InstanceId);foreach(bool ambiguous in new[]{false,true}){var bad=Xml(document.OuterXml);if(!ambiguous)bad.GetElementsByTagName("CreateObjectDie",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().SetAttribute("CreationList","RebornMissingBehaviorCreationList");bad.Schemas.Add(schemas.Schemas);bad.Validate((_,e)=>throw new InvalidDataException(e.Message));Refuse(()=>ResolveStrong(bad,schemas,ambiguous?catalog.Append(target).ToArray():catalog));}
        ResolveStrong(document,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,document.OuterXml);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Shared 36-behavior dependency missing/ambiguous.");var native=CompileAll(instance.XmlNode.FirstChild!,modules.Select(m=>m.TypeId).ToArray());Verify(native);var repeat=CompileAll(instance.XmlNode.FirstChild!,modules.Select(m=>m.TypeId).ToArray());if(!native.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !native.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !native.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))throw new InvalidDataException("Shared 36-behavior repeat differs.");int failures=2;
        foreach(var mutation in new (int At,uint Value)[]{(0,35),(4,12),(8,0),(12,Word(native.InstanceBuffer,8)),((int)Word(native.InstanceBuffer,8)+4,0)}){var bad=Clone(native);Put(bad.InstanceBuffer,mutation.At,mutation.Value);Refuse(()=>Verify(bad));failures++;}foreach(bool relo in new[]{true,false}){var bad=Clone(native);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad));failures++;}
        var metadata=compose?BehaviorContainerStockProof.Run(native,schemas.Schemas,modules,stock,stockReferences,instance,NativeType):(Prefix:0,Refusals:0);int authoredContainers=compose?AuthoredContainers(document,schemas,modules,stock,stockReferences):0;
        return new(instance.ReferencedInstances.Count,native.InstanceBuffer.Length,native.RelocationBuffer.Length,native.ImportsBuffer.Length,failures,metadata.Prefix,metadata.Refusals,authoredContainers);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: canonical count/item owners and every full native leaf are verified independently; metadata coordinate ambiguity is neither exported nor silently treated as token support. */
        //-------------------------------------------------------------------------------------------------
        void Verify(Chunk value)
        {
            var b=value.InstanceBuffer;if(Word(b,0)!=36 || Word(b,4)!=8 || Word(b,8)!=152)throw new InvalidDataException("Shared 36-behavior header differs.");var relos=Slots(value.RelocationBuffer);var imports=Slots(value.ImportsBuffer);var expectedParent=new[]{4u}.Concat(Enumerable.Range(0,36).Select(i=>(uint)(8+i*4)));if(!relos.Where(s=>s<152).Order().SequenceEqual(expectedParent.Order()) || imports.Any(s=>s<152) || !imports.Select(s=>Word(b,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Shared 36-behavior parent/occurrence ownership differs.");
            for(int i=0;i<36;i++){int start=checked((int)Word(b,8+i*4)),end=i==35?b.Length:checked((int)Word(b,12+i*4));if(start<152 || (start|end)%4!=0 || end<=start || end>b.Length)throw new InvalidDataException("Shared behavior leaf order/bounds differ.");var leaf=new Chunk{InstanceBuffer=b.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(relos.Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(imports.Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};foreach(uint slot in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Shared behavior pointer crosses leaf owners.");Put(leaf.InstanceBuffer,(int)slot,target-(uint)start);}Type type=NativeType(modules[i].TypeId,modules[i].Type);new SelectedNativeGraphComparer(stock,stockReferences,leaf,instance,requireAllReferences:false,captureNativeCoordinates:false).Compare(type,modules[i].Native);}
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate all complete source records on the same diagnostic Behaviors root slot; no separately compiled native payload is appended. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk CompileAll(XmlNode source,uint[] types)
    {
        // Reborn: the original gate still requires all 36 sources; separate authored controls may use explicit nonempty subsets and orders on the same allocator.
        var document=Xml(source.OuterXml);var children=document.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();int count=children.Length;if(count==0 || count>64 || types.Length!=count || children.Select(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())).Distinct().Count()!=count)throw new InvalidDataException("Distinct bounded source module identities required.");var ns=new XmlNamespaceManager(document.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);GameObject* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(GameObject),false);
        using(var header=tracker.Push((void**)&root->Behaviors,(uint)sizeof(Relo.PolymorphicList<BehaviorModuleData>),1u)){root->Behaviors->Count=(uint)count;tracker.InplaceEndianToPlatform(&root->Behaviors->Count);using var items=tracker.Push((void**)&root->Behaviors->Items,(uint)sizeof(BehaviorModuleData*),(uint)count);for(int i=0;i<count;i++){children[i].SetAttribute("TypeId",types[i].ToString(CultureInfo.InvariantCulture));Dispatch(new Node(children[i].CreateNavigator()!,ns),&root->Behaviors->Items[i],tracker,types[i]);}}
        var all=new Chunk();if(!tracker.MakeRelocatable(all))throw new InvalidDataException("Shared behavior Tracker failed.");int start=(int)Word(all.InstanceBuffer,476);var relos=Slots(all.RelocationBuffer);var imports=Slots(all.ImportsBuffer);if(start!=600 || !relos.Where(s=>s<start).SequenceEqual(new[]{476u}) || imports.Any(s=>s<start))throw new InvalidDataException("Shared behavior root allocation differs.");var native=new Chunk{InstanceBuffer=all.InstanceBuffer.AsSpan(start).ToArray(),RelocationBuffer=Pack(relos.Where(s=>s>=start).Select(s=>s-(uint)start)),ImportsBuffer=Pack(imports.Select(s=>s-(uint)start))};foreach(uint slot in Slots(native.RelocationBuffer)){uint target=Word(native.InstanceBuffer,(int)slot);if(target<start || target>=all.InstanceBuffer.Length)throw new InvalidDataException("Shared behavior pointer crosses diagnostic root.");Put(native.InstanceBuffer,(int)slot,target-(uint)start);}return native;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: share the actual concrete dispatch between standalone and combined proofs while retaining separate EP1 layouts for the known KW mismatches. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Dispatch(Node node,BehaviorModuleData** target,Tracker tracker,uint type)
    {if(type==0x431722A9)Marshaler.MarshalEp1WeaponSet(node,target,tracker);else if(type is 0xB5EB41ED or 0x79BFEDCB or 0x59D12990 or 0x786C9458 or 0x3B68B768 or 0x90CEB49C)Marshaler.MarshalEp1InfantryBehavior(node,target,tracker,type);else Marshaler.Marshal(node,target,tracker);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all native and metadata proof routes share the same explicit reviewed EP1 model selection; wire identities remain original. */
    //-------------------------------------------------------------------------------------------------
    private static Type NativeType(uint id,string name)=>id switch{0x431722A9=>typeof(Ep1WeaponSetUpdateModuleData),0xB5EB41ED=>typeof(Ep1SquishCollideModuleData),0x79BFEDCB=>typeof(Ep1PhysicsBehaviorModuleData),0x59D12990=>typeof(Ep1HitReactionBehaviorModuleData),0x786C9458=>typeof(Ep1StatusBitsUpgradeModuleData),0x3B68B768=>typeof(Ep1CreateObjectDieModuleData),0x90CEB49C=>typeof(Ep1FireWeaponWhenDeadBehaviorModuleData),_=>typeof(GameObject).Assembly.GetType("SageBinaryData."+name)??throw new InvalidDataException("Behavior native model absent.")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reverse/full, renamed/full and mixed/subset source scenarios use independent Core normalization and the same Tracker; check retained order, metadata owners and semantic import identities. */
    //-------------------------------------------------------------------------------------------------
    private static int AuthoredContainers(XmlDocument original,SchemaSet schemas,GameObjectModuleReview.Module[] modules,byte[] stock,AssetId[] stockReferences)
    {
        for(int scenario=0;scenario<3;scenario++)
        {
            var document=Xml(original.OuterXml);var parent=document.DocumentElement!.FirstChild!;var leaves=parent.ChildNodes.OfType<XmlElement>().ToArray();if(scenario==0){foreach(var leaf in leaves.Reverse())parent.AppendChild(leaf);}else if(scenario==1){leaves[0].SetAttribute("id","RebornAuthoredSharedHelper");}else{var kept=new HashSet<int>{3,7,12,21,23,31,32,33};for(int i=0;i<leaves.Length;i++)if(!kept.Contains(i))parent.RemoveChild(leaves[i]);}
            document.Schemas.Add(schemas.Schemas);document.Validate((_,e)=>throw new InvalidDataException(e.Message));var chosen=parent.ChildNodes.OfType<XmlElement>().ToArray();var types=chosen.Select(e=>FastHash.GetHashCode(e.SchemaInfo.SchemaType!.Name!)).ToArray();var instance=ReferencePipelineSmokeTest.Normalize(schemas,document.OuterXml);var native=CompileAll(instance.XmlNode.FirstChild!,types);var tokens=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.TokenizeContainer(native,schemas.Schemas);int prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;BehaviorContainerStockProof.VerifyTail(tokens,native,prefix);
            if(Word(native.InstanceBuffer,0)!=chosen.Length || Word(tokens.InstanceBuffer,0)!=0x6C827073 || Word(tokens.InstanceBuffer,4)!=0x5D448FD8 || Word(tokens.InstanceBuffer,8)!=prefix || !Slots(native.ImportsBuffer).Select(s=>Word(native.InstanceBuffer,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Authored behavior container count/Core ownership differs.");
            int next=12+chosen.Length*12;for(int i=0;i<chosen.Length;i++){int start=(int)Word(native.InstanceBuffer,8+i*4);if(Word(tokens.InstanceBuffer,12+i*12)!=0xB151A0EB || Word(tokens.InstanceBuffer,16+i*12)!=0 || Word(tokens.InstanceBuffer,20+i*12)!=next || Word(tokens.InstanceBuffer,next)!=2 || Word(tokens.InstanceBuffer,next+4)!=types[i] || Word(native.InstanceBuffer,start)!=types[i] || Word(native.InstanceBuffer,start+4)!=FastHash.GetHashCode(chosen[i].GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("Authored behavior metadata order/identity differs.");int end=i+1<chosen.Length?(int)Word(tokens.InstanceBuffer,32+i*12):prefix;if(end<=next || end>prefix)throw new InvalidDataException("Authored behavior metadata owner extent differs.");for(int at=next+8;at<end;at+=12){uint target=Word(tokens.InstanceBuffer,at);if(target!=0 && target<prefix && (target<next || target>=end))throw new InvalidDataException("Authored behavior metadata crosses siblings.");}next=end;}
            if(next!=prefix || !Equal(tokens,BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.TokenizeContainer(native,schemas.Schemas)))throw new InvalidDataException("Authored behavior metadata coverage/repeat differs.");
            // Reborn: reordered/subset Core indices must still resolve to the original semantic identities, independently walking every unchanged full native leaf against stock.
            for(int i=0;i<chosen.Length;i++)
            {
                int start=(int)Word(native.InstanceBuffer,8+i*4),end=i+1<chosen.Length?(int)Word(native.InstanceBuffer,12+i*4):native.InstanceBuffer.Length;var leaf=new Chunk{InstanceBuffer=native.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(native.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(native.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};foreach(uint slot in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Authored behavior pointer crosses leaf owners.");Put(leaf.InstanceBuffer,(int)slot,target-(uint)start);}
                if(scenario==1 && i==0){if(types[i]!=0x7F72F242 || leaf.InstanceBuffer.Length!=8 || leaf.RelocationBuffer.Length!=0 || leaf.ImportsBuffer.Length!=0)throw new InvalidDataException("Renamed helper must retain its complete eight-byte reference-free graph.");continue;}
                uint id=Word(leaf.InstanceBuffer,4);var originalModule=modules.Single(m=>Word(stock,m.Native+4)==id);new SelectedNativeGraphComparer(stock,stockReferences,leaf,instance,requireAllReferences:false,captureNativeCoordinates:false).Compare(NativeType(originalModule.TypeId,originalModule.Type),originalModule.Native);
            }
        }
        return 3;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: group only the five reviewed independent SlowDeath child families, retaining within-family order and every member; substitute only the single captured external scaler definition. */
    //-------------------------------------------------------------------------------------------------
    private static void Prepare(XmlElement leaf,string scaler)
    {
        if(leaf.LocalName=="SecondaryDamageTracker" && leaf.GetAttribute("MaxDamageClampScaler")=="=$MAX_SECONDARY_DAMAGE_SCALER")leaf.SetAttribute("MaxDamageClampScaler",scaler);
        if(leaf.LocalName!="SlowDeath")return;string[] families={"FX","OCL","Weapon","Sound","DieMuxData"};var children=leaf.ChildNodes.OfType<XmlElement>().ToArray();if(children.Any(e=>e.NamespaceURI!=SchemaSet.XmlNamespace || !families.Contains(e.LocalName)))throw new InvalidDataException("Unreviewed SlowDeath child family.");var inventory=families.Select(f=>children.Where(e=>e.LocalName==f).Select(e=>e.OuterXml).ToArray()).ToArray();foreach(var child in children.OrderBy(e=>Array.IndexOf(families,e.LocalName)))leaf.AppendChild(child);for(int i=0;i<families.Length;i++)if(!leaf.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==families[i]).Select(e=>e.OuterXml).SequenceEqual(inventory[i]))throw new InvalidDataException("SlowDeath source preparation lost members/order.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent edited/default/optional-pointer and legacy-size anchors keep six EP1 routes from merely reproducing their unedited stock samples. */
    //-------------------------------------------------------------------------------------------------
    private static int Authored(XmlSchemaSet schemas)
    {
        if(Marshal.SizeOf<Ep1SquishCollideModuleData>()!=16 || Marshal.SizeOf<SquishCollideModuleData>()!=20 || Marshal.SizeOf<Ep1PhysicsBehaviorModuleData>()!=104 || Marshal.SizeOf<PhysicsBehaviorModuleData>()!=104 || Marshal.SizeOf<Ep1HitReactionBehaviorModuleData>()!=48 || Marshal.SizeOf<HitReactionBehaviorModuleData>()!=36 || Marshal.SizeOf<Ep1StatusBitsUpgradeModuleData>()!=100 || Marshal.SizeOf<StatusBitsUpgradeModuleData>()!=104)throw new InvalidDataException("Independent EP1/KW infantry ABI sizes differ.");
        var squish=Build("<SquishCollide id='RebornSquish' CrushAnimTime='2.5s' UseVehicleStyleCrush='true' AllowAllyCrush='true'/>",0xB5EB41ED);if(squish.InstanceBuffer.Length!=16 || Word(squish.InstanceBuffer,8)!=Float(2.5f) || squish.InstanceBuffer[12]!=1 || squish.InstanceBuffer[13]!=1)throw new InvalidDataException("EP1 authored squish fields differ.");
        var physics=Build("<Physics id='RebornPhysics' ShockStunnedTimeLow='2s' ShockStunnedTimeHigh='3.5s' ShockStandingTime='4s'/>",0x79BFEDCB);if(Word(physics.InstanceBuffer,24)!=Float(2) || Word(physics.InstanceBuffer,28)!=Float(3.5f) || Word(physics.InstanceBuffer,32)!=Float(4))throw new InvalidDataException("EP1 authored physics Time fields differ.");
        var defaults=Build("<Physics id='RebornPhysicsDefaults'/>",0x79BFEDCB);if(Word(defaults.InstanceBuffer,24)!=Float(1) || Word(defaults.InstanceBuffer,28)!=Float(1) || Word(defaults.InstanceBuffer,32)!=Float(1.5f))throw new InvalidDataException("EP1 physics defaults differ.");
        var hit=Build("<HitReactionBehavior id='RebornHit' LightDamageHitReactionLifeTimer='1s' MediumDamageHitReactionLifeTimer='1.5s' HeavyDamageHitReactionLifeTimer='2s' RequiredDeathTypes='BITTEN' RotateToSourceWhenDamaged='true' RotateToSourceIgnoreIdleCheck='true' GoIdleWhenDamaged='true'><ObjectStatusValidation RequiredStatus='UNDER_FROZEN'/></HitReactionBehavior>",0x59D12990);int status=(int)Word(hit.InstanceBuffer,40);if(Word(hit.InstanceBuffer,8)!=Float(1) || Word(hit.InstanceBuffer,12)!=Float(1.5f) || Word(hit.InstanceBuffer,16)!=Float(2) || Word(hit.InstanceBuffer,32)!=(1u<<(int)DeathType.BITTEN) || status<48 || !Slots(hit.RelocationBuffer).Contains(40u) || hit.InstanceBuffer[44]!=1 || hit.InstanceBuffer[45]!=1 || hit.InstanceBuffer[46]!=1 || Enumerable.Range(status,32).Any(i=>hit.InstanceBuffer[i]!=0) || !Enumerable.Range(status+32,32).Any(i=>hit.InstanceBuffer[i]!=0))throw new InvalidDataException("EP1 authored hit reaction fields/pointer differ.");
        var upgrade=Build("<StatusBitsUpgrade id='RebornStatus' StatusToSet='WEAPON_UPGRADED_01' StatusToClear='UNDER_FROZEN'><TriggeredBy>Upgrade_Veterancy_HEROIC</TriggeredBy></StatusBitsUpgrade>",0x786C9458);if(upgrade.InstanceBuffer.Length!=104 || Word(upgrade.InstanceBuffer,16)!=100 || !Enumerable.Range(36,32).Any(i=>upgrade.InstanceBuffer[i]!=0) || !Enumerable.Range(68,32).Any(i=>upgrade.InstanceBuffer[i]!=0))throw new InvalidDataException("EP1 status upgrade root/list boundary/masks differ.");
        // Reborn: literal metadata-anchored layouts expose differences that the original empty/default fields alone could not distinguish.
        if(Marshal.SizeOf<Ep1CreateObjectDieModuleData>()!=72 || Marshal.SizeOf<CreateObjectDieModuleData>()!=80 || Marshal.SizeOf<Ep1FireWeaponWhenDeadBehaviorModuleData>()!=148 || Marshal.SizeOf<FireWeaponWhenDeadBehaviorModuleData>()!=144)throw new InvalidDataException("EP1 creation/death-weapon ABI sizes differ.");
        var creation=Build("<CreateObjectDie id='RebornCreation'><DieMuxData/></CreateObjectDie>",0x3B68B768);if(creation.InstanceBuffer.Length!=72 || Word(creation.InstanceBuffer,56)!=0 || Word(creation.InstanceBuffer,64)!=0)throw new InvalidDataException("EP1 creation list headers differ.");
        var death=Build("<FireWeaponWhenDead id='RebornDeathFlags' FireAtGround='true' UseMyKillerAsWeaponSource='true'><DieMuxData/></FireWeaponWhenDead>",0x90CEB49C);if(death.InstanceBuffer.Length!=148 || death.InstanceBuffer[140]!=0 || death.InstanceBuffer[141]!=0 || death.InstanceBuffer[142]!=0 || death.InstanceBuffer[143]!=1 || death.InstanceBuffer[144]!=1)throw new InvalidDataException("EP1 additional death-weapon flags differ.");
        var deathDefaults=Build("<FireWeaponWhenDead id='RebornDeathDefaults'><DieMuxData/></FireWeaponWhenDead>",0x90CEB49C);if(deathDefaults.InstanceBuffer.Length!=148 || deathDefaults.InstanceBuffer[143]!=0 || deathDefaults.InstanceBuffer[144]!=0)throw new InvalidDataException("EP1 death-weapon flag defaults differ.");
        var deathTokens=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(death,schemas,"FireWeaponWhenDeadBehaviorModuleData",typeof(Ep1FireWeaponWhenDeadBehaviorModuleData));int deathPrefix=deathTokens.InstanceBuffer.Length-death.InstanceBuffer.Length;if(OwnTarget(deathTokens,"FireAtGround")!=deathPrefix+143 || OwnTarget(deathTokens,"UseMyKillerAsWeaponSource")!=deathPrefix+144 || deathTokens.InstanceBuffer[deathPrefix+143]!=1 || deathTokens.InstanceBuffer[deathPrefix+144]!=1)throw new InvalidDataException("Authored death flags metadata targets differ.");
        foreach(string bad in new[]{"<SquishCollide id='Bad' CrushKillDelay='1s'/>","<HitReactionBehavior id='Bad' FastHitsResetReaction='true'/>","<StatusBitsUpgrade id='Bad' ApplyToContained='true'/>"})Refuse(()=>Ep1BasicModuleSmokeTest.Validate(schemas,bad,false));
        Refuse(()=>Compile(Xml("<Physics xmlns='uri:ea.com:eala:asset' id='Bad' ShockStandingTime='bad'/>").DocumentElement!,0x79BFEDCB));
        Refuse(()=>Compile(Xml("<Unknown xmlns='uri:ea.com:eala:asset' id='Bad'/>").DocumentElement!,0xDEADBEEF));
        Refuse(()=>Ep1BasicModuleSmokeTest.Validate(schemas,"<CreateObjectDie xmlns='uri:ea.com:eala:asset' id='Bad' DebrisPortionOfSelf='legacy'><DieMuxData/></CreateObjectDie>",false));
        return 14;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: edited controls use official schema admission and the same explicit allocator; no captured occurrence values are injected. */
        //-------------------------------------------------------------------------------------------------
        Chunk Build(string leaf,uint type){var document=Xml(leaf.Insert(leaf.IndexOf(' ')," xmlns='uri:ea.com:eala:asset'"));Ep1BasicModuleSmokeTest.Validate(schemas,document.OuterXml,false);var result=Compile(document.DocumentElement!,type);if(Word(result.InstanceBuffer,4)!=FastHash.GetHashCode(document.DocumentElement!.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("Authored EP1 module identity differs.");return result;}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: literal IEEE words check Time conversion independently of marshaler implementations. */
        //-------------------------------------------------------------------------------------------------
        static uint Float(float value)=>unchecked((uint)BitConverter.SingleToInt32Bits(value));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only validated linked EP1 catalogs supply concrete target identities. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Valid behavior target catalog required.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source parsing is bounded by captured inputs and forbids DTDs/external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){using var input=new StringReader(text);using var reader=XmlReader.Create(input,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null});var document=new XmlDocument{XmlResolver=null,PreserveWhitespace=true};document.Load(reader);return document;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: deliberately corrupted native words and malformed schema/Time/dispatch inputs must produce explicit data, format or builder refusals. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or FormatException or BinaryAssetBuilderException){return;}throw new InvalidDataException("Corrupt surveyed behavior accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controls modify owned copies only. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare all three owned buffers for deterministic metadata generation and immutable caller input. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: distinct real named upgrade assets exercise both corrected creation-list headers through actual Core normalization, not numeric authored occurrence indices. */
    //-------------------------------------------------------------------------------------------------
    private static int AuthoredReferenceMetadata(SchemaSet schemas,XmlSchemaSet official,ManifestAsset[] catalog)
    {
        var upgrades=catalog.Where(a=>a.TypeId==FastHash.GetHashCode("UpgradeTemplate")).GroupBy(a=>a.InstanceId).Where(g=>g.Count()==1).Select(g=>g.Single()).Take(2).ToArray();if(upgrades.Length!=2)throw new InvalidDataException("Two unambiguous authored upgrade targets required.");
        var document=Xml("<BehaviorsSurveyProbe xmlns='uri:ea.com:eala:asset' id='RebornCreationLists'><Behaviors><CreateObjectDie id='RebornCreationLists'><DieMuxData/><UpgradeRequired/><UpgradeForbidden/></CreateObjectDie></Behaviors></BehaviorsSurveyProbe>");document.GetElementsByTagName("UpgradeRequired",SchemaSet.XmlNamespace)[0]!.InnerText=upgrades[0].Name;document.GetElementsByTagName("UpgradeForbidden",SchemaSet.XmlNamespace)[0]!.InnerText=upgrades[1].Name;document.Schemas.Add(official);document.Validate((_,e)=>throw new InvalidDataException(e.Message));ResolveStrong(document,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,document.OuterXml);var native=Compile(instance.XmlNode.FirstChild!.FirstChild!,0x3B68B768);
        if(instance.ReferencedInstances.Count!=2 || native.InstanceBuffer.Length!=80 || Word(native.InstanceBuffer,56)!=1 || Word(native.InstanceBuffer,60)!=72 || Word(native.InstanceBuffer,64)!=1 || Word(native.InstanceBuffer,68)!=76 || !Slots(native.ImportsBuffer).Order().SequenceEqual(new[]{72u,76u}) || !Slots(native.RelocationBuffer).Order().SequenceEqual(new[]{60u,68u}))throw new InvalidDataException("Authored EP1 creation list storage/import ownership differs.");
        for(int i=0;i<2;i++){uint index=Word(native.InstanceBuffer,72+i*4);if(index==0 || index>2 || instance.ReferencedInstances[(int)index-1].TypeId!=upgrades[i].TypeId || instance.ReferencedInstances[(int)index-1].InstanceId!=upgrades[i].InstanceId)throw new InvalidDataException("Authored creation upgrade semantic identity differs.");}
        var tokens=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,official,"CreateObjectDieModuleData",typeof(Ep1CreateObjectDieModuleData));int prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;foreach(var field in new[]{("UpgradeRequired",56), ("UpgradeForbidden",64)}){int list=checked((int)OwnTarget(tokens,field.Item1));if(Word(tokens.InstanceBuffer,list)!=0x6C827073 || Word(tokens.InstanceBuffer,list+8)!=prefix+field.Item2 || Word(tokens.InstanceBuffer,list+12)!=0xB151A0EB)throw new InvalidDataException("Authored corrected creation metadata header differs.");}return 1;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal own-table row walking supplies an independent authored field-target assertion, without invoking the production schema descriptor helper. */
    //-------------------------------------------------------------------------------------------------
    private static uint OwnTarget(Chunk tokens,string name){uint hash=FastHash.GetHashCode(name);int table=checked((int)Word(tokens.InstanceBuffer,8));for(int rows=0;rows<256;rows++,table+=12){uint field=Word(tokens.InstanceBuffer,table);if(field==0)break;if(field==hash)return Word(tokens.InstanceBuffer,table+8);}throw new InvalidDataException("Authored behavior own metadata field absent: "+name);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact auxiliary arrays retain ordering and reject malformed sentinels or duplicate words. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Behavior auxiliary framing differs.");var result=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(result.Distinct().Count()!=result.Length)throw new InvalidDataException("Behavior duplicate auxiliary slot.");return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: locally rebased slots keep their original Tracker order and one nonempty sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.ToArray();if(values.Length==0)return Array.Empty<byte>();var bytes=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(bytes,i*4,values[i]);Put(bytes,values.Length*4,uint.MaxValue);return bytes;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded word reads never dump a native stock payload. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Behavior word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only extracted or deliberate mutation buffers receive patches. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
