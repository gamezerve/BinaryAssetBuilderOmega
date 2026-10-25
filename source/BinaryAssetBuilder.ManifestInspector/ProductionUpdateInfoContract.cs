using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: derive the identity-only production-info metadata chain from original Uprising construction yards.
internal static class ProductionUpdateInfoContract
{
    internal const string Wire="ProductionUpdateInfoModuleData";
    internal const uint TypeId=0x1B337783u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the eight-byte marker and inherited behavior identity on Soviet and Allied retail construction yards. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<ProductionUpdateInfoModuleData>()!=8)throw new InvalidDataException("EP1 production-info marker must be eight bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietConstructionYard","Soviet/Structures/SovietConstructionYard.xml"),("AlliedConstructionYard","Allied/Structures/AlliedConstructionYard.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"ProductionUpdateInfo").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("ProductionUpdateInfo",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(ProductionUpdateInfoModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(ProductionUpdateInfoModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("ProductionUpdateInfo",$"Reborn: ProductionUpdateInfo EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<ProductionUpdateInfoModuleData>()}).",count+" stock records");return grammar;
    }
}
