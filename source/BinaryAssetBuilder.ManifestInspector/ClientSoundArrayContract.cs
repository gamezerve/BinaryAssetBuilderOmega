using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: populated selector sound arrays share the native root sound ABI and are qualified against the original Bear water override.
internal static class ClientSoundArrayContract
{
    internal const uint TypeId=0x327CFB76u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: supplement the selector grammar with original source/native/metadata evidence for populated voice-and-sound siblings. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(StockMetadataGrammar grammar,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1AudioSoundEntry>()!=8||Marshal.SizeOf<Ep1AudioArraySound>()!=8||Marshal.OffsetOf<Ep1ModelConditionSoundSelectorOverrideBlock>("AudioArraySound").ToInt32()!=128)throw new InvalidDataException("EP1 selector sound-array ABI differs.");
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietScoutInfantry");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="ClientBehaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();int count=0;
        foreach(var original in GameObjectExtendedRootContract.Original("Soviet/Units/SovietScoutInfantry.xml","ModelConditionSoundSelectorClientBehavior")){
            uint id=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var clients=source.GetElementsByTagName("ClientBehaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();clients.RemoveAll();clients.AppendChild(source.ImportNode(original,true));var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("ModelConditionSoundSelectorClientBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(resolved,typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());}
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData));count++;
        }
        if(count==0)throw new InvalidDataException("No populated selector sound-array stock witness.");
        SdkBuildTests.MetadataProgress("Client AudioArraySound",$"Reborn: client AudioArraySound EP1 qualification: PASS ({count} original Bear selector, mixed voice/sound arrays).",count+" original populated selectors");
    }
}
