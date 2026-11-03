using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify assisted-placement info storage and metadata against the original retail ore node.
internal static class AssistedPlacementInfoContract
{
    internal const string Wire="AssistedPlacementInfoBehaviorModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    // Reborn: the only retail record: four hints (one filtered, three faction-specific), a restriction filter and a suggested offset.
    internal static readonly (string Name,string Path)[] Witnesses={("OreNode","Neutral/Structures/OreNode.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every original native record and learn its retail metadata before authored placement info are composed. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<AssistedPlacementInfoBehaviorModuleData>()!=36)throw new InvalidDataException("EP1 AssistedPlacementInfoBehavior native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var original=GameObjectExtendedRootContract.Original(witness.Path,"AssistedPlacementInfoBehavior").Single();var probe=(XmlDocument)baseline.CloneNode(true);var behaviors=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);behaviors.AppendChild(imported);
                var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("AssistedPlacementInfoBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(AssistedPlacementInfoBehaviorModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());
            }
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(AssistedPlacementInfoBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("AssistedPlacementInfoBehavior",$"Reborn: AssistedPlacementInfoBehavior native/metadata qualification: PASS ({count} original records, 36-byte root).",count+" original records");return grammar;
    }
}
