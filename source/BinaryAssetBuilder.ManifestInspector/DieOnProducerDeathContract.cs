using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify producer-death storage and metadata against the original Allied attack drone.
internal static class DieOnProducerDeathContract
{
    internal const string Wire="DieOnProducerDeathUpdateModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    // Reborn: the only retail producer-death watcher; it sets DieOnProducerDisabled and keeps the default interval.
    internal static readonly (string Name,string Path)[] Witnesses={("AlliedAttackDrone","Allied/Units/AlliedAttackDrone.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every original native record and learn its retail metadata before authored modules are composed. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<DieOnProducerDeathUpdateModuleData>()!=16)throw new InvalidDataException("EP1 DieOnProducerDeathUpdate native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var original=GameObjectExtendedRootContract.Original(witness.Path,"DieOnProducerDeathUpdate").Single();var probe=(XmlDocument)baseline.CloneNode(true);var behaviors=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);behaviors.AppendChild(imported);
                var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("DieOnProducerDeathUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(DieOnProducerDeathUpdateModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());
            }
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(DieOnProducerDeathUpdateModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("DieOnProducerDeathUpdate",$"Reborn: DieOnProducerDeathUpdate native/metadata qualification: PASS ({count} original records, 16-byte root).",count+" original records");return grammar;
    }
}
