using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify Stingray circular damage and Dreadnought health drain against their original EP1 source and binary graphs.
internal static class NavalUpdateContract
{
    internal static readonly (string Source,string Wire,Type Type,string Owner,string Path)[] Families={
        ("CircularDamageUpdate","CircularDamageUpdateModuleData",typeof(CircularDamageUpdateModuleData),"SovietAntiNavyShipTech1","Soviet/Units/SovietAntiNavyShipTech1.xml"),
        ("LoseHealthUpdate","LoseHealthUpdateModuleData",typeof(LoseHealthUpdateModuleData),"SovietAntiStructureShip","Soviet/Units/SovietAntiStructureShip.xml")};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove native extents, original field values/imports and nested metadata grammar before authoring either naval update. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<CircularDamageUpdateModuleData>()!=152||Marshal.SizeOf<LoseHealthUpdateModuleData>()!=24)throw new InvalidDataException("Naval update EP1 ABI extents differ.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var family in Families){
            uint typeId=FastHash.GetHashCode(family.Wire);var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+family.Owner);
            var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==typeId);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(family.Path,family.Source).Single(),true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,family.Type,typeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);count++;
        }
        SdkBuildTests.MetadataProgress("Naval updates",$"Reborn: naval updates: PASS ({count} original circular-damage/health-drain native and metadata graphs).",count+" original records");return grammar;
    }
}
