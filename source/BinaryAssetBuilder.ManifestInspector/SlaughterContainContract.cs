using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: derive the complete slaughter-container metadata chain from the original Uprising crane and carrier.
internal static class SlaughterContainContract
{
    internal const string Wire="SlaughterHordeContainModuleData";
    internal const uint TypeId=0xC1947EDCu;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the full inherited record, native defaults, refund percentage, optional FX and inline entrance filter. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietCrane","Soviet/Structures/SovietCrane.xml"),("AlliedAntiStructureShip","Allied/Units/AlliedAntiStructureShip.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"SlaughterHordeContain").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("SlaughterHordeContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(SlaughterHordeContainModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(SlaughterHordeContainModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("SlaughterHordeContain",$"Reborn: SlaughterHordeContain EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<SlaughterHordeContainModuleData>()}).",count+" stock records");return grammar;
    }
}
