using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: authored locomotors use the existing untokenized 396-byte native writer, qualified against original Uprising locomotor records.
internal static class LocomotorTemplateCompiler
{
    // Reborn: plain infantry, same-file inheritance, water condition/status pointers, forbidden status, bounce-kick map, jet data and transition FX.
    private static readonly string[] Witnesses={"HumanLocomotor","AlliedAttackDrone_Leeched_Locomotor","HumanWaterLocomotor","JapanAntiAirShipWaterLocomotor_Attacking","AlliedAntiInfantryVehicleLocomotor","JapanAntiAirShipAirLocomotor","JapanAntiAirShipTestLocomotor"};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: flatten original same-file locomotor inheritance through Core for independent source/native witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Original(string id,SchemaSet schemas)
    {
        var owners=GameObjectExtendedRootContract.Original("GlobalData/Locomotor.xml","LocomotorTemplate").ToDictionary(e=>e.GetAttribute("id"));var active=new HashSet<string>(StringComparer.Ordinal);return SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(Resolve(id).OuterXml));
        XmlElement Resolve(string key){if(active.Count>32||!active.Add(key))throw new InvalidDataException("Original locomotor inheritance cycle.");var node=(XmlElement)owners[key].CloneNode(true);ConscriptVariableBehaviorSource.DropLeafFormatting(node);foreach(XmlNode text in node.SelectNodes(".//text()")!.Cast<XmlNode>().ToArray())if(string.IsNullOrWhiteSpace(text.Value)&&text.ParentNode!.ChildNodes.OfType<XmlElement>().Any())text.ParentNode.RemoveChild(text);if(node.HasAttribute("inheritFrom")){string basis=node.GetAttribute("inheritFrom");node.RemoveAttribute("inheritFrom");node=(XmlElement)NodeJoiner.Override(schemas.Schemas,node.OwnerDocument!,Resolve(basis),node);}active.Remove(key);return node;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify defaults, optional pointer records and real imports against selected original static assets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<LocomotorTemplate>()!=396)throw new InvalidDataException("EP1 locomotor header must be 396 bytes.");var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked locomotor stream required.");string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Locomotor stock checksum differs.");int count=0;
        foreach(string id in Witnesses){var source=Original(id,schemas);var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="LocomotorTemplate:"+id);if(asset.Tokenized!=0||asset.TypeId!=FastHash.GetHashCode("LocomotorTemplate"))throw new InvalidDataException("Locomotor stock fingerprint differs.");long offset=8;foreach(var item in doc.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(LocomotorTemplate),0,bytes,0,asset.References.ToArray());count++;}
        SdkBuildTests.MetadataProgress("LocomotorTemplate",$"Reborn: locomotor EP1 qualification: PASS ({count} original native graphs; header=396).",count+" original records");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: asset identity belongs to the stream manifest; no XML ID is stored in the native base word. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog){var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the normalized Core occurrence indices through the native locomotor writer with no token metadata wrapper. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);LocomotorTemplate* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(LocomotorTemplate),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Locomotor native allocation failed.");return chunk;
    }
}
