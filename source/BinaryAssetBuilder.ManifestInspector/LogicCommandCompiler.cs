using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: general EP1 LogicCommand route (every Type, Options, SpecialPower/Upgrade/Object). The schema/native contract is proved on every
// original EP1 static command, including the complete optional Uprising AI power graph.
internal static class LogicCommandCompiler
{
    private const uint TypeHash=0x751AB28F;
    private static bool qualified;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the earlier bounded route (UNIT_BUILD with exactly one Object, no Options) keeps its own established compiler. */
    //-------------------------------------------------------------------------------------------------
    internal static bool IsRecruitment(XmlDocument source)
    {
        var root=source.DocumentElement!;var children=root.ChildNodes.OfType<XmlElement>().ToArray();
        return root.GetAttribute("Type")=="UNIT_BUILD"&&root.GetAttribute("Options").Trim().Length==0&&children.Length==1&&children[0].LocalName=="Object";
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit the command family while schema validation and current-source proof reject actual invalid data. */
    //-------------------------------------------------------------------------------------------------
    internal static void Admit(XmlDocument source)
    {
        // Reborn: actual schema/native verification owns admissibility; no artificial AI-field presence gate remains.
        if(source.DocumentElement?.LocalName!="LogicCommand")throw new InvalidDataException("LogicCommand source required.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the contract on all original EP1 static commands including AISpecialPowerInfo (byte, relocation and import ownership). */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(qualified)return;
        if(System.Runtime.InteropServices.Marshal.SizeOf<Ep1LogicCommand>()!=28)throw new InvalidDataException("EP1 LogicCommand ABI size differs.");
        // Reborn: independently anchored Uprising additions must retain their actual 188-byte layout.
        if(System.Runtime.InteropServices.Marshal.SizeOf<Ep1AiSpecialPowerInfo>()!=188||System.Runtime.InteropServices.Marshal.OffsetOf<Ep1AiSpecialPowerInfo>("RequiredForAttackObjectStatus").ToInt32()!=164||System.Runtime.InteropServices.Marshal.OffsetOf<Ep1AiSpecialPowerInfo>("SpecialPowerWeapon").ToInt32()!=176)throw new InvalidDataException("EP1 AISpecialPowerInfo ABI differs.");
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
        if(document.Validate().Count!=0||!document.Header.IsLinked||document.Header.ContainerPrefixSize!=4)throw new InvalidDataException("Valid linked EP1 static stream required for LogicCommand qualification.");
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(Word(header,0)!=0xBABB0000||Word(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("Static stream checksum differs.");
        var offsets=new Dictionary<string,(ManifestAsset Asset,long Offset)>(StringComparer.Ordinal);long offset=8;foreach(var entry in document.Assets){offsets[entry.Name]=(entry,offset);offset+=entry.InstanceDataSize;}
        var types=new SortedSet<string>(StringComparer.Ordinal);int proved=0,options=0;
        foreach(var original in Originals()){
            if(original.HasAttribute("inheritFrom"))continue;
            string id=original.GetAttribute("id");if(!offsets.TryGetValue("LogicCommand:"+id,out var stock))continue;
            if(stock.Asset.TypeHash!=TypeHash||stock.Asset.Tokenized!=0||stock.Asset.InstanceDataSize>4096)throw new InvalidDataException("Stock LogicCommand fingerprint differs: "+id);
            var bytes=AssetStreamProbe.ReadRange(bin,null,stock.Offset,stock.Asset.InstanceDataSize);
            try{SchemaNativeContract.VerifyStock(ProofSource(Xml(original.OuterXml),schemas,catalog).DocumentElement!,typeof(Ep1LogicCommand),0,bytes,0,stock.Asset.References.ToArray());}catch(InvalidDataException error){throw new InvalidDataException("LogicCommand retail proof failed for "+id+": "+error.Message,error);}
            types.Add(original.HasAttribute("Type")?original.GetAttribute("Type"):"NONE");var tokens=original.GetAttribute("Options").Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);if(tokens.Length!=0)options++;proved++;
        }
        if(proved<100||options==0)throw new InvalidDataException("Too few original EP1 LogicCommand witnesses were proved ("+proved+").");
        qualified=true;Console.WriteLine("Reborn: EP1 LogicCommand stock qualification: PASS ("+proved+" static commands, "+options+" with Options; types: "+string.Join(", ",types)+").");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: command usage follows typed references from the stock/mod union; a valid schema enum does not require a retail example. */
    //-------------------------------------------------------------------------------------------------
    internal static bool IsUsed(XmlDocument source,IReadOnlySet<AssetId> references)
    {
        var identity=SdkMissingDependencyAudit.Identity("LogicCommand",source.DocumentElement!.GetAttribute("id"));return references.Contains(new(identity.TypeId,identity.InstanceId));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the stream header owns the asset ID; the native record has no XML id. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run the EP1 marshaler on the Core-normalized node; Core owns every reference occurrence index. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=Xml(source.OuterXml);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1LogicCommand* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1LogicCommand),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("LogicCommand native allocation failed.");return chunk;
    }

    // Reborn: both base-game and EP1 command declarations qualify the same complete Uprising ABI.
    internal static XmlElement[] Originals()=>GameObjectExtendedRootContract.Original("GlobalData/LogicCommand.xml","LogicCommand").Concat(GameObjectExtendedRootContract.Original("EP1/GlobalData/LogicCommand_EP1.xml","LogicCommand")).ToArray();

    private static XmlDocument Xml(string text){var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));if(xml.DocumentElement!.NamespaceURI.Length==0)throw new InvalidDataException("LogicCommand source namespace missing.");return xml;}
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
