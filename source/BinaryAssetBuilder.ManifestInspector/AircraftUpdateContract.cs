using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify MiG return-to-producer and Kirov locomotor-switch modules against original EP1 source/native graphs.
internal static class AircraftUpdateContract
{
    internal static readonly (string Source,string Wire,Type Type,string Owner,string Path)[] Families={
        ("ReturnToProducerSpecialPower","ReturnToProducerSpecialPowerModuleData",typeof(ReturnToProducerSpecialPowerModuleData),"SovietFighterAircraft","Soviet/Units/SovietFighterAircraft.xml"),
        ("SwitchLocomotorUpdate","SwitchLocomotorUpdate",typeof(SwitchLocomotorUpdate),"SovietBomberAircraft","Soviet/Units/SovietBomberAircraft.xml")};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain inherited special-power grammar and independently verify every original return/switch field and native extent. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<ReturnToProducerSpecialPowerModuleData>()!=476||Marshal.SizeOf<SwitchLocomotorUpdate>()!=116)throw new InvalidDataException("Aircraft update EP1 ABI extents differ.");
        var grammar=SpecialPowerContract.Qualify(manifest,baseline,schemas,catalog);int count=0;
        foreach(var family in Families){
            uint typeId=FastHash.GetHashCode(family.Wire);var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+family.Owner);
            var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==typeId);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(family.Path,family.Source).Single(),true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,family.Type,typeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);count++;
        }
        SdkBuildTests.MetadataProgress("Aircraft updates",$"Reborn: aircraft updates: PASS ({count} original MiG/Kirov native and metadata graphs).",count+" original records");return grammar;
    }
}
