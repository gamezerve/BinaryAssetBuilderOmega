using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: general EP1 LogicCommand route (every Type, Options, SpecialPower/Upgrade/Object). The schema/native contract is proved on every
// original EP1 static command whose source has no AISpecialPowerInfo; AISpecialPowerInfo is refused until its EP1 record is reviewed.
internal static class LogicCommandCompiler
{
    private const uint TypeHash=0x751AB28F;
    private static bool qualified;
    // Reborn: Types and Options tokens exhibited by the proved stock commands; authored values outside these sets are reported, not hidden.
    private static readonly SortedSet<string> witnessedTypes=new(StringComparer.Ordinal),witnessedOptions=new(StringComparer.Ordinal);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the earlier bounded route (UNIT_BUILD with exactly one Object, no Options) keeps its own established compiler. */
    //-------------------------------------------------------------------------------------------------
    internal static bool IsRecruitment(XmlDocument source)
    {
        var root=source.DocumentElement!;var children=root.ChildNodes.OfType<XmlElement>().ToArray();
        return root.GetAttribute("Type")=="UNIT_BUILD"&&root.GetAttribute("Options").Trim().Length==0&&children.Length==1&&children[0].LocalName=="Object";
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject the unreviewed AI payload at its own source node before any native allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Admit(XmlDocument source)
    {
        var ai=source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName=="AISpecialPowerInfo");
        if(ai!=null)throw SdkXmlDiagnostics.At(new InvalidDataException("LogicCommand AISpecialPowerInfo is not supported yet: its EP1 record differs from RA3 and has not been reviewed. Remove it or wait for AISpecialPowerInfo support."),ai);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the contract on all original EP1 static commands without AISpecialPowerInfo (byte, relocation and import ownership). */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(qualified)return;
        if(System.Runtime.InteropServices.Marshal.SizeOf<Ep1LogicCommand>()!=28)throw new InvalidDataException("EP1 LogicCommand ABI size differs.");
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
        if(document.Validate().Count!=0||!document.Header.IsLinked||document.Header.ContainerPrefixSize!=4)throw new InvalidDataException("Valid linked EP1 static stream required for LogicCommand qualification.");
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(Word(header,0)!=0xBABB0000||Word(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("Static stream checksum differs.");
        var offsets=new Dictionary<string,(ManifestAsset Asset,long Offset)>(StringComparer.Ordinal);long offset=8;foreach(var entry in document.Assets){offsets[entry.Name]=(entry,offset);offset+=entry.InstanceDataSize;}
        var types=new SortedSet<string>(StringComparer.Ordinal);int proved=0,options=0;
        foreach(var original in GameObjectExtendedRootContract.Original("GlobalData/LogicCommand.xml","LogicCommand")){
            if(original.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName=="AISpecialPowerInfo")||original.HasAttribute("inheritFrom"))continue;
            string id=original.GetAttribute("id");if(!offsets.TryGetValue("LogicCommand:"+id,out var stock))continue;
            if(stock.Asset.TypeHash!=TypeHash||stock.Asset.Tokenized!=0||stock.Asset.InstanceDataSize>4096)throw new InvalidDataException("Stock LogicCommand fingerprint differs: "+id);
            var bytes=AssetStreamProbe.ReadRange(bin,null,stock.Offset,stock.Asset.InstanceDataSize);
            SchemaNativeContract.VerifyStock(ProofSource(Xml(original.OuterXml),schemas,catalog).DocumentElement!,typeof(Ep1LogicCommand),0,bytes,0,stock.Asset.References.ToArray());
            types.Add(original.HasAttribute("Type")?original.GetAttribute("Type"):"NONE");var tokens=original.GetAttribute("Options").Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);if(tokens.Length!=0)options++;witnessedOptions.UnionWith(tokens);proved++;
        }
        if(proved<100||options==0)throw new InvalidDataException("Too few original EP1 LogicCommand witnesses were proved ("+proved+").");
        witnessedTypes.UnionWith(types);qualified=true;Console.WriteLine("Reborn: EP1 LogicCommand stock qualification: PASS ("+proved+" static commands, "+options+" with Options; types: "+string.Join(", ",types)+").");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a Type or Options value with no proved stock command is compiled from its schema ordinal/bit, but reported as a warning
        because the game behaviour of that combination has no original EP1 witness. */
    //-------------------------------------------------------------------------------------------------
    internal static void ReportUnwitnessed(XmlDocument source)
    {
        var root=source.DocumentElement!;string id=root.GetAttribute("id"),type=root.HasAttribute("Type")?root.GetAttribute("Type").Trim():"NONE";
        if(!witnessedTypes.Contains(type))Console.WriteLine("Warning: UNWITNESSEDCOMMAND LogicCommand:"+id+" uses Type="+type+", which no original EP1 static command uses; it is compiled from the schema ordinal ("+(Enum.TryParse(type,false,out Ep1LogicCommandType t)?((int)t).ToString():"?")+") but has no stock witness.");
        foreach(string token in root.GetAttribute("Options").Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries).Where(t=>!witnessedOptions.Contains(t)).Distinct())
            Console.WriteLine("Warning: UNWITNESSEDCOMMAND LogicCommand:"+id+" uses Options="+token+", which no original EP1 static command uses; it is compiled from the schema bit ("+(Enum.TryParse(token,false,out Ep1LogicCommandOptions o)?((int)o).ToString():"?")+") but has no stock witness.");
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

    private static XmlDocument Xml(string text){var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));if(xml.DocumentElement!.NamespaceURI.Length==0)throw new InvalidDataException("LogicCommand source namespace missing.");return xml;}
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
