using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: connect the existing attribute-modifier upgrade writer to source-sized EP1 metadata and independent stock qualification.
internal static class AttributeModifierUpgradeContract
{
    internal const string Wire="AttributeModifierUpgradeModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the strong modifier word and full Upgrade inheritance against original air-power, naval-power and Soviet vehicle records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<AttributeModifierUpgradeModuleData>()!=40||Marshal.OffsetOf<AttributeModifierUpgradeModuleData>("AttributeModifier").ToInt32()!=36)throw new InvalidDataException("EP1 AttributeModifierUpgrade must add one strong modifier word to the 36-byte upgrade base.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("AlliedBomberAircraft","Includes/AlliedAirPowerUpgrade.xml"),("JapanNavyScoutShip","Japan/Units/JapanNavyScoutShip.xml"),("SovietAntiVehicleVehicleTech1","Soviet/Units/SovietAntiVehicleVehicleTech1.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"AttributeModifierUpgrade")){
                uint id=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var modifier=GameObjectStockReview.Tokens(captured.Bytes,table)[FastHash.GetHashCode("AttributeModifier")];
                if(modifier.Type!=0x09DDCFF3u||modifier.Offset-module.Native!=36)throw new InvalidDataException("Stock AttributeModifierUpgrade reference signature/offset differs.");
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("AttributeModifierUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(AttributeModifierUpgradeModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(AttributeModifierUpgradeModuleData));count++;
            }
        }
        // Reborn: the same named Upgrade/AnimAndDuration ABI appears in original aircraft and construction-yard subobject upgrades; reuse the populated stock animation grammar.
        foreach(string name in new[]{"AlliedBomberAircraft","AlliedConstructionYard","AlliedGunshipAircraft"}){var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);foreach(var module in GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==SubObjectsUpgradeContract.TypeId))grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));}
        SdkBuildTests.MetadataProgress("AttributeModifierUpgrade",$"Reborn: AttributeModifierUpgrade EP1 native/metadata qualification: PASS ({count} original retail records; header=40).",count+" stock records");return grammar;
    }
}
