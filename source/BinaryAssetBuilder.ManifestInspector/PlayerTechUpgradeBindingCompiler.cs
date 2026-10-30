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

// Reborn: authored protocol bindings retain the original EP1 ordered pairs of strong technology/upgrade imports.
internal static class PlayerTechUpgradeBindingCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read untouched original faction binding declarations for independent source/native witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Original(string id,SchemaSet schemas)
    {
        return SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(GameObjectExtendedRootContract.Original("System/PlayerTechUpgradeBindings.xml","PlayerTechUpgradeBinding").Single(e=>e.GetAttribute("id")==id).OuterXml));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify all three original faction binding lists against their retail global native graphs. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1PlayerTechUpgradeBinding>()!=12)throw new InvalidDataException("EP1 player-tech binding header must be 12 bytes.");var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked player-tech binding stream required.");string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Player-tech stock checksum differs.");int count=0;
        foreach(string id in new[]{"PlayerTechUpgradeBinding_Allied","PlayerTechUpgradeBinding_Soviet","PlayerTechUpgradeBinding_Japan"}){var source=Original(id,schemas);var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="PlayerTechUpgradeBinding:"+id);if(asset.Tokenized!=0||asset.TypeId!=FastHash.GetHashCode("PlayerTechUpgradeBinding"))throw new InvalidDataException("Player-tech stock fingerprint differs.");long offset=8;foreach(var item in doc.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(Ep1PlayerTechUpgradeBinding),0,bytes,0,asset.References.ToArray());count++;}
        SdkBuildTests.MetadataProgress("PlayerTechUpgradeBinding",$"Reborn: player-tech binding EP1 qualification: PASS ({count} original native graphs; header=12).",count+" original records");
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
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1PlayerTechUpgradeBinding* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1PlayerTechUpgradeBinding),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Player-tech native allocation failed.");return chunk;
    }
}
