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

// Reborn: authored special-power templates use the actual untokenized EP1 layout, not the legacy KW enum/EVA layout.
internal static class SpecialPowerTemplateCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: flatten original same-file special-power inheritance through Core for independent source/native witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Original(string id,SchemaSet schemas)
    {
        var owners=new[]{"GlobalData/SpecialPowerTemplates.xml","EP1/GlobalData/SpecialPowerTemplates_EP1.xml"}.SelectMany(path=>GameObjectExtendedRootContract.Original(path,"SpecialPowerTemplate")).ToDictionary(e=>e.GetAttribute("id"));var active=new HashSet<string>(StringComparer.Ordinal);return SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(Resolve(id).OuterXml));
        XmlElement Resolve(string key){if(active.Count>32||!active.Add(key))throw new InvalidDataException("Original special-power inheritance cycle.");var node=(XmlElement)owners[key].CloneNode(true);foreach(XmlNode text in node.SelectNodes(".//text()")!.Cast<XmlNode>().ToArray())if(string.IsNullOrWhiteSpace(text.Value)&&text.ParentNode!.ChildNodes.OfType<XmlElement>().Any())text.ParentNode.RemoveChild(text);if(node.HasAttribute("inheritFrom")){string basis=node.GetAttribute("inheritFrom");node.RemoveAttribute("inheritFrom");node=(XmlElement)NodeJoiner.Override(schemas.Schemas,node.OwnerDocument!,Resolve(basis),node);}active.Remove(key);return node;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify defaults, optional mask/prerequisite and native weak/strong arrays against selected original static assets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1SpecialPowerTemplate>()!=584)throw new InvalidDataException("EP1 special-power header must be 584 bytes.");var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked special-power stream required.");string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Special-power stock checksum differs.");int count=0;
        foreach(string id in new[]{"SpecialPowerOrbitalRefuse_Rank1","SpecialPowerOrbitalRefuse_Rank2","SpecialPowerOrbitalRefuse_Rank3","SpecialPower_ToggleAlliedGunshipAircraft","SpecialPower_UnpackReplaceSelf","SpecialPower_TriggerLeapDeploy","SpecialPowerIronCurtain"}){var source=Original(id,schemas);var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="SpecialPowerTemplate:"+id);if(asset.Tokenized!=0||asset.TypeHash!=3236296400u||asset.TypeId!=FastHash.GetHashCode("SpecialPowerTemplate"))throw new InvalidDataException("Special-power stock fingerprint differs.");long offset=8;foreach(var item in doc.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(Ep1SpecialPowerTemplate),0,bytes,0,asset.References.ToArray());count++;}
        SdkBuildTests.MetadataProgress("SpecialPowerTemplate",$"Reborn: special-power EP1 qualification: PASS ({count} original native graphs; header=584).",count+" original records");
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
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1SpecialPowerTemplate* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1SpecialPowerTemplate),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Special-power native allocation failed.");return chunk;
    }
}
