using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify ore-node storage and metadata against the original retail ore node.
internal static class OreNodeContract
{
    internal const string Wire="OreNodeModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    // Reborn: the only retail ore-node record; OreNode inherits its behavior from BaseOreNode.
    internal static readonly (string Name,string Path)[] Witnesses={("OreNode","BaseObjects/BaseOreNode.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: replace EA define expressions in an isolated original with the official GlobalDefines values. */
    //-------------------------------------------------------------------------------------------------
    internal static void ApplyDefines(XmlElement element)
    {
        var defines=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").ToDictionary(e=>e.GetAttribute("name"),e=>e.GetAttribute("value"));
        foreach(var attribute in element.Attributes.OfType<XmlAttribute>().ToArray())if(attribute.Value.StartsWith("=$"))attribute.Value=defines[attribute.Value[2..]];
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every original native record and learn its retail metadata before authored ore nodes are composed. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<OreNodeModuleData>()!=72)throw new InvalidDataException("EP1 OreNodeBehavior native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var original=GameObjectExtendedRootContract.Original(witness.Path,"OreNodeBehavior").Single();var probe=(XmlDocument)baseline.CloneNode(true);var behaviors=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);ApplyDefines(imported);behaviors.AppendChild(imported);
                var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("OreNodeBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(OreNodeModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());
            }
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(OreNodeModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("OreNodeBehavior",$"Reborn: OreNodeBehavior native/metadata qualification: PASS ({count} original records, 72-byte root).",count+" original records");return grammar;
    }
}
