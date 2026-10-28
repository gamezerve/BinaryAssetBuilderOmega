using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the missing Grinder/Apocalypse/MCV module families together from original EP1 source and payloads.
internal static class VehicleBatchAbilityContract
{
    internal static readonly (string Source,string Wire,Type Type,string Owner,string Path)[] Families={
        ("SpecialAbilityUpdate","SpecialAbilityUpdateModuleData",typeof(SpecialAbilityUpdateModuleData),"SovietGrinderVehicle","EP1/Soviet/Units/SovietGrinderVehicle.xml"),
        ("GrapplingHookUpdate","GrapplingHookUpdateModuleData",typeof(GrapplingHookUpdateModuleData),"SovietAntiVehicleVehicleTech3","Soviet/Units/SovietAntiVehicleVehicleTech3.xml"),
        ("SubStructureStorageBehavior","SubStructureStorageBehaviorModuleData",typeof(SubStructureStorageBehaviorModuleData),"SovietMCV","Soviet/Units/SovietMCV.xml")};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify all original fields/references and retain literal retail row grammars for authored module composition. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<SpecialAbilityUpdateModuleData>()!=252||Marshal.SizeOf<GrapplingHookUpdateModuleData>()!=52||Marshal.SizeOf<SubStructureStorageBehaviorModuleData>()!=8)throw new InvalidDataException("Vehicle batch behavior ABI differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var family in Families){
            uint typeId=FastHash.GetHashCode(family.Wire);var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+family.Owner);
            var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==typeId);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(family.Path,family.Source).Single(),true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,family.Type,typeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);count++;
        }
        SdkBuildTests.MetadataProgress("Vehicle batch abilities",$"Reborn: vehicle batch abilities: PASS ({count} original Grinder/Apocalypse/MCV module records).",count+" original records");return grammar;
    }
}
