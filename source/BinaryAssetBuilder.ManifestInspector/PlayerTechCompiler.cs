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

// Reborn: authored PlayerTech nodes retain the original EP1 cost, weak FX and strong dependency-list ABI.
internal static class PlayerTechCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: flatten original same-file player-tech inheritance through Core for independent source/native witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Original(string id,SchemaSet schemas)
    {
        var owners=new[]{"System/PlayerTechs.xml","EP1/System/PlayerTechs_EP1.xml"}.SelectMany(path=>GameObjectExtendedRootContract.Original(path,"PlayerTech")).ToDictionary(e=>e.GetAttribute("id"));var active=new HashSet<string>(StringComparer.Ordinal);return SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(Resolve(id).OuterXml));
        XmlElement Resolve(string key){if(active.Count>32||!active.Add(key))throw new InvalidDataException("Original player-tech inheritance cycle.");var node=(XmlElement)owners[key].CloneNode(true);foreach(XmlNode text in node.SelectNodes(".//text()")!.Cast<XmlNode>().ToArray())if(string.IsNullOrWhiteSpace(text.Value)&&text.ParentNode!.ChildNodes.OfType<XmlElement>().Any())text.ParentNode.RemoveChild(text);if(node.HasAttribute("inheritFrom")){string basis=node.GetAttribute("inheritFrom");node.RemoveAttribute("inheritFrom");node=(XmlElement)NodeJoiner.Override(schemas.Schemas,node.OwnerDocument!,Resolve(basis),node);}active.Remove(key);return node;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify defaults, weak grant FX and strong technology dependency lists against selected original global assets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1PlayerTech>()!=20)throw new InvalidDataException("EP1 player-tech header must be 20 bytes.");var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked player-tech stream required.");string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Player-tech stock checksum differs.");int count=0;
        foreach(string id in new[]{"PlayerTech_Allied","PlayerTech_Allied_AirPower","PlayerTech_Generic_Rank_3","PlayerTech_Yuriko_Shield_Rank2","PlayerTech_Yuriko_Shield_Rank3"}){var source=Original(id,schemas);var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="PlayerTech:"+id);if(asset.Tokenized!=0||asset.TypeHash!=1090016465u||asset.TypeId!=FastHash.GetHashCode("PlayerTech"))throw new InvalidDataException("Player-tech stock fingerprint differs.");long offset=8;foreach(var item in doc.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(Ep1PlayerTech),0,bytes,0,asset.References.ToArray());count++;}
        SdkBuildTests.MetadataProgress("PlayerTech",$"Reborn: player-tech EP1 qualification: PASS ({count} original native graphs; header=20).",count+" original records");
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
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1PlayerTech* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1PlayerTech),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Player-tech native allocation failed.");return chunk;
    }
}
