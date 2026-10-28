using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify slave-watcher storage and metadata against all four original Uprising records.
internal static class SlaveWatcherContract
{
    internal const string Wire="SlaveWatcherBehaviorModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    // Reborn: status-validation (forbidden/required), absent validation with a RemoveUpgrade import, and two boolean profiles.
    internal static readonly (string Name,string Path)[] Witnesses={("SovietDesolatorInfantry","EP1/Soviet/Units/SovietDesolatorInfantry.xml"),("AlliedAntiAirVehicleTech1","Allied/Units/AlliedAntiAirVehicleTech1.xml"),("AlliedBaseDefense","Allied/Structures/AlliedBaseDefense.xml"),("SovietAntiVehicleVehicleTech1","Soviet/Units/SovietAntiVehicleVehicleTech1.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every original native record and learn its retail metadata before authored watchers are composed. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<SlaveWatcherBehaviorModuleData>()!=24)throw new InvalidDataException("EP1 SlaveWatcherBehavior native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var original=GameObjectExtendedRootContract.Original(witness.Path,"SlaveWatcherBehavior").Single();var probe=(XmlDocument)baseline.CloneNode(true);var behaviors=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);behaviors.AppendChild(imported);
                var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("SlaveWatcherBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(SlaveWatcherBehaviorModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());
            }
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(SlaveWatcherBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("SlaveWatcherBehavior",$"Reborn: SlaveWatcherBehavior native/metadata qualification: PASS ({count} original records, 24-byte root).",count+" original records");return grammar;
    }
}
