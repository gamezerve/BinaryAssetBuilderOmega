using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify OCL-monitor storage and metadata against the original Natasha bombing-run monitor.
internal static class OCLMonitorContract
{
    internal const string Wire="OCLMonitorUpdateModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    // Reborn: the only retail monitor with available source; it uses the EP1-only RELEASE_WHEN_MASTER_CHANGES_TARGET option.
    internal static readonly (string Name,string Path)[] Witnesses={("SovietCommandoTech1","Soviet/Units/SovietCommandoTech1.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every original native record and learn its retail metadata before authored watchers are composed. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<OCLMonitorUpdateModuleData>()!=44)throw new InvalidDataException("EP1 OCLMonitorUpdate native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var original=GameObjectExtendedRootContract.Original(witness.Path,"OCLMonitorUpdate").Single();var probe=(XmlDocument)baseline.CloneNode(true);var behaviors=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);behaviors.AppendChild(imported);
                var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("OCLMonitorUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(OCLMonitorUpdateModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());
            }
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(OCLMonitorUpdateModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("OCLMonitorUpdate",$"Reborn: OCLMonitorUpdate native/metadata qualification: PASS ({count} original records, 44-byte root).",count+" original records");return grammar;
    }
}
