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

// Reborn: author AttributeModifier roots through their actual 56-byte untokenized EP1 layout and common source/native proof.
internal static class AttributeModifierCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read selected original modifier declarations without copying any native stock payload into authored output. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Original(string id)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(GameObjectExtendedRootContract.Original("GlobalData/AttributeModifier.xml","AttributeModifier").Single(e=>e.GetAttribute("id")==id).OuterXml));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify lists, optional masks, percentages, FX imports and relocated shader references against original Uprising records. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<AttributeModifier>()!=56)throw new InvalidDataException("EP1 AttributeModifier header must be 56 bytes.");
        var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);
        if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked modifier stream required.");
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);
        if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Modifier stock checksum differs.");
        int count=0;
        foreach(string id in new[]{"AttributeModifier_RedAlert_Orange","Unit_Heroic","Modifier_Cover","AttributeModifier_IronCurtain","AttributeMod_AlliedSupportAircraftShrinkRay","AttributeModifier_JapanAntiInfantryInfantryBonzai"}){
            var source=Original(id);var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="AttributeModifier:"+id);
            if(asset.Tokenized!=0||asset.TypeHash!=0x74425C11u||asset.TypeId!=FastHash.GetHashCode("AttributeModifier"))throw new InvalidDataException("Modifier stock fingerprint differs.");
            long offset=8;foreach(var item in doc.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}
            var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(AttributeModifier),0,bytes,0,asset.References.ToArray());count++;
        }
        SdkBuildTests.MetadataProgress("AttributeModifier",$"Reborn: AttributeModifier EP1 qualification: PASS ({count} original native graphs; header=56).",count+" original records");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: asset IDs belong to the stream manifest; inherited source is flattened before native field proof. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog){var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize current Core-normalized modifiers through the existing writer without the old experimental standalone gates. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        AttributeModifier* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(AttributeModifier),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("AttributeModifier native allocation failed.");return chunk;
    }
}
