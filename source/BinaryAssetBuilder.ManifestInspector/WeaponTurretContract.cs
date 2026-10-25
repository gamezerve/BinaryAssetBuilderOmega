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
        foreach(var witness in new[]{("SovietBaseDefenseGround","Soviet/Structures/SovietBaseDefenseGround.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"WeaponSetUpdate").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("WeaponSetUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(Ep1WeaponSetUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1WeaponSetUpdateModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("WeaponSetUpdate",$"Reborn: WeaponSetUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1WeaponSetUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
