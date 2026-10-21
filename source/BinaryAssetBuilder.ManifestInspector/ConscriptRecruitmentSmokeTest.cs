using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
// Reborn: use the actual Core/Utility external-manifest buffer for the mixed recruitment stream.
using BinaryAssetBuilder.Utility;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reuse the existing Barracks command slot by overriding its stock LogicCommand, not by patching an opaque stock binary or replacing CommandSet.
internal static class ConscriptRecruitmentSmokeTest
{
    internal const string CommandId = "Command_ConstructSovietAntiInfantryInfantry";
    internal const uint CommandType = 0x7D464170, CommandHash = 0x751AB28F;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: permit a fast independent command ABI gate without recompiling the complete Conscript or modifying the live developer mod. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed EP1 command schema absent.");
            var alias=new XmlSchema{TargetNamespace=SchemaSet.XmlNamespace,ElementFormDefault=XmlSchemaForm.Qualified};alias.Items.Add(new XmlSchemaElement{Name="LogicCommand",SchemaTypeName=new XmlQualifiedName("LogicCommand",SchemaSet.XmlNamespace)});official.Add(alias);official.Compile();
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(XmlSchema seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            Qualify(schemas,manifest);
        }
        finally{Settings.Current=saved;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete newly marshalled UNIT_BUILD bytes/auxiliary streams with a bounded independently captured EP1 stock command. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(SchemaSet schemas, string manifest)
    {
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));
        TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);
        if(!catalog.Header.IsLinked || catalog.Validate().Count!=0)throw new InvalidDataException("Recruitment stock metadata invalid.");
        var asset=catalog.Assets.Single(a=>a.Name=="LogicCommand:"+CommandId);
        if(asset.TypeId!=CommandType || asset.TypeHash!=CommandHash || asset.Tokenized!=0 || asset.InstanceDataSize!=32
            || asset.RelocationDataSize!=8 || asset.ImportsDataSize!=8
            || !asset.References.SequenceEqual(new[]{new AssetId(0x942FFF2D,InstanceHandle.GetInstanceId("SovietAntiInfantryInfantry"))}))
            throw new InvalidDataException("EP1 UNIT_BUILD fingerprint differs.");
        // Reborn: independently pin the existing stock consumer chain rather than inferring recruitment only from the XML names.
        var set=catalog.Assets.Single(a=>a.Name=="LogicCommandSet:SovietBarracksCommandSet");
        var barracks=catalog.Assets.Single(a=>a.Name=="GameObject:SovietBarracks");
        if(set.References.Count!=8 || set.References[1]!=new AssetId(CommandType,asset.InstanceId))throw new InvalidDataException("Stock CommandSet/command consumer chain differs.");
        // Reborn: GameObject.CommandSet is a weak ID, not a manifest import; verify the actual stock token/native word instead.
        long barracksOffset=8;foreach(var previous in catalog.Assets){if(ReferenceEquals(previous,barracks))break;barracksOffset+=previous.InstanceDataSize;}
        if(barracks.InstanceDataSize>1048576)throw new InvalidDataException("Barracks slice bound.");
        byte[] barracksBytes=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,".bin"),null,barracksOffset,barracks.InstanceDataSize);var rootFields=GameObjectStockReview.Tokens(barracksBytes,0);
        var commandSet=rootFields[FastHash.GetHashCode("CommandSet")];
        if(commandSet.Type!=0x85D44B5D || commandSet.Offset!=rootFields[FastHash.GetHashCode("KindOf")].Offset-4+152 || Word(barracksBytes,commandSet.Offset)!=set.InstanceId)throw new InvalidDataException("Stock Barracks weak CommandSet identity differs.");
        var expected=Compile(schemas,"SovietAntiInfantryInfantry");
        // Reborn: native allocation and Core-normalized references must repeat exactly, not only serialize the same saved buffer twice.
        var repeated=Compile(schemas,"SovietAntiInfantryInfantry");
        if(!expected.Chunk.InstanceData.SequenceEqual(repeated.Chunk.InstanceData) || !expected.Chunk.RelocationData.SequenceEqual(repeated.Chunk.RelocationData) || !expected.Chunk.ImportsData.SequenceEqual(repeated.Chunk.ImportsData)
            || !expected.Instance.ReferencedInstances.Select(h=>h.Name).SequenceEqual(repeated.Instance.ReferencedInstances.Select(h=>h.Name)))throw new InvalidDataException("EP1 command native/reference repeat differs.");
        foreach(var part in new[]{(".bin",asset.InstanceDataSize,expected.Chunk.InstanceData,0xBABB0000u),
            (".relo",asset.RelocationDataSize,expected.Chunk.RelocationData,0xBABE0000u),(".imp",asset.ImportsDataSize,expected.Chunk.ImportsData,0xBAB10000u)})
        {
            long offset=8;
            foreach(var previous in catalog.Assets){if(ReferenceEquals(previous,asset))break;offset+=part.Item1==".bin"?previous.InstanceDataSize:part.Item1==".relo"?previous.RelocationDataSize:previous.ImportsDataSize;}
            string path=Path.ChangeExtension(manifest,part.Item1);byte[] header=AssetStreamProbe.ReadRange(path,null,0,8);
            if(Word(header,0)!=part.Item4 || Word(header,4)!=catalog.Header.StreamChecksum
                || !AssetStreamProbe.ReadRange(path,null,offset,part.Item2).SequenceEqual(part.Item3))throw new InvalidDataException("EP1 UNIT_BUILD stock native/auxiliary differs.");
        }
        // Reborn: named enum order is independently compared with the official EP1 schema, never the incompatible legacy KW ordinals.
        var type=(XmlSchemaSimpleType)schemas.Schemas.GlobalTypes[new XmlQualifiedName("LogicCommandType",SchemaSet.XmlNamespace)]!;
        if(!((XmlSchemaSimpleTypeRestriction)type.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f=>f.Value).SequenceEqual(Enum.GetNames<Ep1LogicCommandType>()))throw new InvalidDataException("EP1 command enum drift.");
        foreach(string edit in new[]{"Type='STOP'","Options='OPTION_ONE'","Unknown='1'"})
            Refuse(()=>CompileXml(schemas,Source("RebornConscriptTrial").Replace("Type='UNIT_BUILD'",edit,StringComparison.Ordinal)));
        Refuse(()=>CompileXml(schemas,Source("RebornConscriptTrial").Replace("<Object>","<Upgrade>" ,StringComparison.Ordinal).Replace("</Object>","</Upgrade>",StringComparison.Ordinal)));
        Console.WriteLine("Conscript recruitment command: PASS (official EP1 enum, exact stock LogicCommand TypeHash/non-tokenized ABI, BIN/RELO/IMP=32/8/8, real Core GameObject import; four unsupported-source refusals; existing Barracks/CommandSet untouched).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: combine a fully qualified new GameObject with a freshly compiled command; prove the command import selects the unique local new identity. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string,byte[]> Compose(Dictionary<string,byte[]> objectStreams, InstanceDeclaration original, SchemaSet schemas, string manifest, string globalManifest,XmlDocument commandSource,ConscriptGraphQualification? graph=null)
    {
        var objectMetadata=ManifestReader.Read(objectStreams["diagnostic.manifest"]);var objectAsset=objectMetadata.Assets.Single();
        if(objectAsset.Name!=original.Handle.Name)throw new InvalidDataException("Authored GameObject identity differs from its qualified stream.");
        var game=Attach(original.XmlNode,"OriginalConscript.normalized.xml");game.ReferencedInstances.AddRange(original.ReferencedInstances);game.ValidatedReferencedInstances=new(game.ReferencedInstances);game.Handle.TypeHash=objectAsset.TypeHash;game.Handle.InstanceHash=objectAsset.InstanceHash;
        var objectBuffer=new AssetBuffer{InstanceData=objectStreams["diagnostic.bin"].AsSpan(8).ToArray(),RelocationData=objectStreams["diagnostic.relo"].AsSpan(8).ToArray(),ImportsData=objectStreams["diagnostic.imp"].AsSpan(8).ToArray()};
        // Reborn: compile exactly the authored command, never Source(target) or an ID-triggered synthetic command.
        var command=CompileXml(schemas,commandSource.DocumentElement!.OuterXml);var ordered=new[]{game,command.Instance};var chunks=new[]{objectBuffer,command.Chunk};
        uint checksum=(uint)typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{ordered})!;
        var runtime=new ReferencedFileBuffer();typeof(OutputManager).GetMethod("AddExternalManifestReferences",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{runtime,new[]{Path.GetFullPath(manifest),Path.GetFullPath(globalManifest)},"data\\static.manifest;data\\global.manifest"});
        var streams=BoundedDiagnosticBuild.SerializeConscriptRecruitment(ordered,chunks,checksum,runtime,graph);
        var repeat=BoundedDiagnosticBuild.SerializeConscriptRecruitment(ordered,chunks,checksum,runtime,graph);
        if(streams.Any(p=>!p.Value.SequenceEqual(repeat[p.Key])))throw new InvalidDataException("Recruitment serialization not deterministic.");
        string directory=Path.Combine(Path.GetTempPath(),"Reborn-Recruitment-"+Guid.NewGuid().ToString("N"));ModifierFXStreamSmokeTest.WriteNew(directory,streams);
        BoundedDiagnosticBuild.Verify(directory,ordered,chunks,streams,checksum,new[]{"data\\static.manifest","data\\global.manifest"},new[]{true,false});
        var parsed=ManifestReader.Read(streams["diagnostic.manifest"]);var target=parsed.Assets[1].References.Single();
        if(target!=new AssetId(parsed.Assets[0].TypeId,parsed.Assets[0].InstanceId)
            || Word(chunks[1].InstanceData,28)!=1 || parsed.Assets[0].Name!=original.Handle.Name)throw new InvalidDataException("Authored command must select the local GameObject identity.");
        // Reborn: missing/reversed local targets, wrong command import and truncation must fail the explicit two-asset publication gate.
        Refuse(()=>BoundedDiagnosticBuild.SerializeConscriptRecruitment(ordered.Reverse().ToArray(),chunks.Reverse().ToArray(),checksum,runtime,graph));
        Refuse(()=>BoundedDiagnosticBuild.SerializeConscriptRecruitment(new[]{command.Instance},new[]{command.Chunk},checksum,runtime,graph));
        byte[] saved=command.Chunk.InstanceData;try{command.Chunk.InstanceData=saved[..^1];Refuse(()=>BoundedDiagnosticBuild.SerializeConscriptRecruitment(ordered,chunks,checksum,runtime,graph));}finally{command.Chunk.InstanceData=saved;}
        try{command.Chunk.InstanceData=saved.ToArray();command.Chunk.InstanceData[28]=2;Refuse(()=>BoundedDiagnosticBuild.SerializeConscriptRecruitment(ordered,chunks,checksum,runtime,graph));}finally{command.Chunk.InstanceData=saved;}
        Console.WriteLine($"Conscript recruitment stream: PASS (new GameObject + overridden stock UNIT_BUILD command; tokenized/native flags, {game.ReferencedInstances.Count} external + one local dependency, deterministic Core checksum and independent Manifest/Utility byte readback; four two-asset refusals; no game execution).");
        return streams;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile one literal reviewed target through official schema validation and actual Core reference normalization. */
    //-------------------------------------------------------------------------------------------------
    private static (InstanceDeclaration Instance,AssetBuffer Chunk) Compile(SchemaSet schemas,string target)=>CompileXml(schemas,Source(target));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the source profile deliberately supports one construction command only; all unreviewed attributes and children fail before marshaling. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe (InstanceDeclaration Instance,AssetBuffer Chunk) CompileXml(SchemaSet schemas,string text)
    {
        var xml=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};xml.LoadXml(text);xml.Schemas.Add(schemas.Schemas);xml.Validate((_,e)=>throw new InvalidDataException(e.Message));
        var root=xml.DocumentElement!;var children=root.ChildNodes.OfType<XmlElement>().ToArray();
        if(!ConscriptSourceInput.Identity(root.GetAttribute("id")) || root.GetAttribute("Type")!="UNIT_BUILD" || root.Attributes.OfType<XmlAttribute>().Any(a=>a.NamespaceURI.Length==0 && a.Name is not ("id" or "Type" or "Options"))
            || root.GetAttribute("Options")!="" || children.Length!=1 || children[0].LocalName!="Object")throw new InvalidDataException("Only explicit UNIT_BUILD/Object commands are supported in this profile.");
        // Reborn: plain EA GameObjectRef names and explicit GameObject: names resolve to the same typed Core import; other reference types are rejected.
        string targetName=children[0].InnerText.Trim();if(targetName.StartsWith("GameObject:",StringComparison.Ordinal))targetName=targetName.Substring(11);if(!ConscriptSourceInput.Identity(targetName))throw new InvalidDataException("Invalid GameObject command target.");children[0].InnerText="GameObject:"+targetName;
        var normalized=ReferencePipelineSmokeTest.Normalize(schemas,xml.OuterXml);var instance=Attach(normalized.XmlNode,"ConscriptRecruitment.normalized.xml");instance.ReferencedInstances.AddRange(normalized.ReferencedInstances);instance.ValidatedReferencedInstances=new(instance.ReferencedInstances);
        if(instance.ReferencedInstances.Count!=1 || instance.ReferencedInstances[0].TypeId!=0x942FFF2D)throw new InvalidDataException("Recruitment command reference normalization differs.");
        instance.Handle.TypeHash=CommandHash;instance.Handle.InstanceHash=FastHash.GetHashCode(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));
        var doc=new XmlDocument{XmlResolver=null};doc.LoadXml(instance.XmlNode.OuterXml);var ns=new XmlNamespaceManager(doc.NameTable);ns.AddNamespace("a",SchemaSet.XmlNamespace);var node=new Node(doc.DocumentElement!.CreateNavigator(),ns);
        Ep1LogicCommand* native;using Tracker tracker=new((void**)&native,(uint)sizeof(Ep1LogicCommand),false);Marshaler.Marshal(node,native,tracker);var chunk=new Chunk();
        if(Marshal.SizeOf<Ep1LogicCommand>()!=28 || !tracker.MakeRelocatable(chunk))throw new InvalidDataException("Recruitment native allocation differs.");
        return(instance,new AssetBuffer{InstanceData=chunk.InstanceBuffer,RelocationData=chunk.RelocationBuffer,ImportsData=chunk.ImportsBuffer});
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: attach an explicit diagnostic source identity without inventing a complete EA processed-document cache receipt. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Attach(XmlNode node,string path)
    {
        var doc=new AssetDeclarationDocument();Type stateType=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic)!;object state=Activator.CreateInstance(stateType,true)!;stateType.GetField("SourcePath")!.SetValue(state,path);typeof(AssetDeclarationDocument).GetField("_current",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(doc,state);
        return new InstanceDeclaration(doc){XmlNode=node.CloneNode(true)};
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source is regenerated from the named reviewed target; no opaque captured command payload is copied into output. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(string target)=>"<LogicCommand xmlns='uri:ea.com:eala:asset' id='"+CommandId+"' Type='UNIT_BUILD'><Object>GameObject:"+target+"</Object></LogicCommand>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit refusal controls must not count successful invalid compilation as rejection. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or NotSupportedException or XmlSchemaValidationException){return;}throw new InvalidDataException("Invalid recruitment control accepted.");}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: all inspection reads are restricted to the newly owned bounded command/header buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
