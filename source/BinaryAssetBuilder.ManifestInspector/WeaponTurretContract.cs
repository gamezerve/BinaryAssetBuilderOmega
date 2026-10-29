using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: derive the weapon-turret native and metadata layout from the original Uprising Sentry Gun.
internal static class WeaponTurretContract
{
    internal const string Wire="WeaponSetUpdateModuleData";
    internal const uint TypeId=0x431722A9u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the complete turret, hardpoint base, nested AI and chooser against the original retail building. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1WeaponSetUpdateModuleData>()!=44||Marshal.SizeOf<Ep1WeaponSlotTurret>()!=132||Marshal.SizeOf<Ep1TurretAIData>()!=100||Marshal.SizeOf<Ep1TurretAITargetChooserData>()!=44)throw new InvalidDataException("EP1 weapon turret layout differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietBaseDefenseGround","Soviet/Structures/SovietBaseDefenseGround.xml"),("JapanBaseDefense","Japan/Structures/JapanBaseDefense.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(JapanModuleContract.Originals(witness.Item2,"WeaponSetUpdate",witness.Item1).Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("WeaponSetUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(Ep1WeaponSetUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1WeaponSetUpdateModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("WeaponSetUpdate",$"Reborn: WeaponSetUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1WeaponSetUpdateModuleData>()}).",count+" stock records");return grammar;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: load the WorldBuilder hierarchy witness only when authored hierarchical turrets actually require it. */
    //-------------------------------------------------------------------------------------------------
    internal static void QualifyHierarchical(StockMetadataGrammar grammar,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        // Reborn: the original Mecha King exists in WorldBuilder and qualifies six source-sized hierarchical turrets and ParentID links.
        if(Marshal.SizeOf<Ep1WeaponSlotHierarchicalTurret>()!=136)throw new InvalidDataException("EP1 hierarchical turret stride differs.");
        string world=WorldBuilderAssetResolver.CatalogPath();var mecha=GameObjectStockReview.Capture(world,"GameObject:JapanMechaKing");var mechaModule=GameObjectModuleReview.Inspect(world,mecha.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
        if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(JapanModuleContract.Originals("Japan/Units_SinglePlayerCampaign/JapanMechaKing.xml","WeaponSetUpdate","JapanMechaKing").Single(),true));var available=catalog.Concat(mecha.Document.Assets).GroupBy(a=>new AssetId(a.TypeId,a.InstanceId)).Select(g=>g.First()).ToArray();var resolved=SchemaNativeContract.Resolve(source,schemas,available).GetElementsByTagName("WeaponSetUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(resolved,typeof(Ep1WeaponSetUpdateModuleData),TypeId,mecha.Bytes,mechaModule.Native,mecha.Asset.References.ToArray());}
        grammar.Learn(mecha.Bytes,mechaModule.Wrapper,mecha.Projection.NativeRoot,typeof(Ep1WeaponSetUpdateModuleData));
        SdkBuildTests.MetadataProgress("Hierarchical weapon turrets","Reborn: hierarchical weapon turrets: PASS (original WorldBuilder Mecha King).","1 original six-turret graph");
    }

}
