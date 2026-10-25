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

// Reborn: authored power-button templates use the actual untokenized EP1 layout, not the legacy KW enum/EVA layout.
internal static class PlayerPowerButtonCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: flatten original same-file power-button inheritance through Core for independent source/native witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Original(string id,SchemaSet schemas)
    {
        var owners=new[]{"UI/Mouse/PlayerPowerButtonTemplates.xml","EP1/UI/Mouse/PlayerPowerButtonTemplates_EP1.xml"}.SelectMany(path=>GameObjectExtendedRootContract.Original(path,"PlayerPowerButtonTemplate")).ToDictionary(e=>e.GetAttribute("id"));var active=new HashSet<string>(StringComparer.Ordinal);return SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(Resolve(id).OuterXml));
        XmlElement Resolve(string key){if(active.Count>32||!active.Add(key))throw new InvalidDataException("Original power-button inheritance cycle.");var node=(XmlElement)owners[key].CloneNode(true);foreach(XmlNode text in node.SelectNodes(".//text()")!.Cast<XmlNode>().ToArray())if(string.IsNullOrWhiteSpace(text.Value)&&text.ParentNode!.ChildNodes.OfType<XmlElement>().Any())text.ParentNode.RemoveChild(text);if(node.HasAttribute("inheritFrom")){string basis=node.GetAttribute("inheritFrom");node.RemoveAttribute("inheritFrom");node=(XmlElement)NodeJoiner.Override(schemas.Schemas,node.OwnerDocument!,Resolve(basis),node);}active.Remove(key);return node;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify all concrete data variants, weak markers and strong import arrays against selected original static assets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1PlayerPowerButtonTemplate>()!=12)throw new InvalidDataException("EP1 power-button header must be 12 bytes.");var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked power-button stream required.");string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Power-button stock checksum differs.");int count=0;
        foreach(string id in new[]{"ButtonPlayerPoweOrbitalRefuse_Rank1","ButtonPlayerPowerSatelliteSweep","ButtonPlayerPowerChronoSwap","ButtonPlayerPowerPsychicSlam_Rank1","ButtonPlayerPowerYurikoShieldRank1"}){var source=Original(id,schemas);var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="PlayerPowerButtonTemplate:"+id);if(asset.Tokenized!=0||asset.TypeHash!=2789252562u||asset.TypeId!=FastHash.GetHashCode("PlayerPowerButtonTemplate"))throw new InvalidDataException("Power-button stock fingerprint differs.");long offset=8;foreach(var item in doc.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(Ep1PlayerPowerButtonTemplate),0,bytes,0,asset.References.ToArray());count++;}
        SdkBuildTests.MetadataProgress("PlayerPowerButtonTemplate",$"Reborn: power-button EP1 qualification: PASS ({count} original native graphs; header=12).",count+" original records");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind the four official Data alternatives to their corresponding EP1 native records. */
    //-------------------------------------------------------------------------------------------------
    internal static Type DataType(XmlElement e)=>e.LocalName switch{
        "Power"=>typeof(Ep1PlayerPowerButtonTemplateData),"TargetedPower"=>typeof(Ep1TargetedPlayerPowerButtonTemplateData),"MultipleTargetsTargetedPower"=>typeof(Ep1MultipleTargetsTargetedPlayerPowerButtonTemplateData),"MultiplePowersTargetedPower"=>typeof(Ep1MultiplePowersTargetedPlayerPowerButtonTemplateData),_=>throw new InvalidDataException("Unsupported power-button Data type: "+e.LocalName)};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: asset identity belongs to the stream manifest; no XML ID is stored in the native BaseAssetType word. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog){var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the normalized Core occurrence indices through the EP1 native writer with no token metadata wrapper. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1PlayerPowerButtonTemplate* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1PlayerPowerButtonTemplate),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Power-button native allocation failed.");return chunk;
    }
}
