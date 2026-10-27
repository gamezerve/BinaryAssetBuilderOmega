using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the complete capture ability layout against original Uprising Soviet, Allied and Imperial Engineer records.
internal static class CaptureAbilityContract
{
    internal const string Wire="CaptureSpecialAbilityUpdateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the capture ability fields, their offsets and the SpecialAbility/Update inheritance chain against original faction Engineer records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<CaptureSpecialAbilityUpdateModuleData>()!=252)throw new InvalidDataException("EP1 capture ability header must be 252 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietEngineer","Includes/CaptureBehavior.xml"),("AlliedEngineer","Includes/CaptureBehavior.xml"),("JapanEngineer","Includes/CaptureBehavior.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);
            var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"CaptureSpecialAbilityUpdate")){
            uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
            var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            // Reborn: the concrete capture class has no own fields; its full native extent and inherited values are checked below.
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("CaptureSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(CaptureSpecialAbilityUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(CaptureSpecialAbilityUpdateModuleData));count++;
            }
        }
        // Reborn: the inherited optional animation record reuses independently captured stock SubObjectsUpgrade metadata.
        var animationStock=GameObjectStockReview.Capture(manifest,"GameObject:AlliedBomberAircraft");
        foreach(var module in GameObjectModuleReview.Inspect(manifest,animationStock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==SubObjectsUpgradeContract.TypeId))grammar.Learn(animationStock.Bytes,module.Wrapper,animationStock.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));
        SdkBuildTests.MetadataProgress("CaptureSpecialAbilityUpdate",$"Reborn: CaptureSpecialAbilityUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<CaptureSpecialAbilityUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
