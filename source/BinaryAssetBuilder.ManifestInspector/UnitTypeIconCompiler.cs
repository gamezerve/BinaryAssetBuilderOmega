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

// Reborn: icons use their retail 12-byte untokenized root, typed image import and optional two-float offset.
internal static class UnitTypeIconCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use original EA icon declarations as independent native witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Original(string id,SchemaSet schemas)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(GameObjectExtendedRootContract.Original("GlobalData/UnitTypeIcons.xml","UnitTypeIcon").Single(e=>e.GetAttribute("id")==id).OuterXml));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify optional offsets and typed image imports against selected original static assets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<UnitTypeIcon>()!=12)throw new InvalidDataException("EP1 icon header must be 12 bytes.");var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked icon stream required.");string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("UnitTypeIcon stock checksum differs.");int count=0;
        foreach(string id in new[]{"RiflemenIcon","EngineerIcon","BearIcon"}){var source=Original(id,schemas);var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="UnitTypeIcon:"+id);if(asset.Tokenized!=0||asset.TypeHash!=0xF9E68B56u||asset.TypeId!=FastHash.GetHashCode("UnitTypeIcon"))throw new InvalidDataException("UnitTypeIcon stock fingerprint differs.");long offset=8;foreach(var item in doc.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(UnitTypeIcon),0,bytes,0,asset.References.ToArray());count++;}
        SdkBuildTests.MetadataProgress("UnitTypeIcon",$"Reborn: icon EP1 qualification: PASS ({count} original native graphs; header=12).",count+" original records");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: asset identity belongs to the stream manifest; no XML ID is stored in the native BaseAssetType word. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog){var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the normalized Core occurrence indices through the EP1 native writer with no token metadata wrapper. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);UnitTypeIcon* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(UnitTypeIcon),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("UnitTypeIcon native allocation failed.");return chunk;
    }
}
