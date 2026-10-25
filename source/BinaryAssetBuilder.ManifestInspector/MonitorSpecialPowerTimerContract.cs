using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify timer-monitor native and metadata layouts against original Uprising superweapons.
internal static class MonitorSpecialPowerTimerContract
{
    internal const string Wire="MonitorSpecialPowerTimerUpdateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the special-power import and inline ready-condition mask against original Soviet and Allied records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<MonitorSpecialPowerTimerUpdateModuleData>()!=72)throw new InvalidDataException("EP1 timer-monitor header must be 72 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietSuperWeapon","Soviet/Structures/SovietSuperWeapon.xml"),("AlliedSuperWeaponAdvanced","Allied/Structures/AlliedSuperWeaponAdvanced.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"MonitorSpecialPowerTimerUpdate").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("MonitorSpecialPowerTimerUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(MonitorSpecialPowerTimerUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(MonitorSpecialPowerTimerUpdateModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("MonitorSpecialPowerTimerUpdate",$"Reborn: MonitorSpecialPowerTimerUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<MonitorSpecialPowerTimerUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
