using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the Uprising aura layout and inherited upgrade metadata against three original retail objects.
internal static class AttributeModifierAuraContract
{
    internal const string Wire="AttributeModifierAuraUpdateModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify all aura offsets, defaults, imports and filter graphs against Bunker, Tesla Trooper and Burst Drone records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1AttributeModifierAuraUpdateModuleData>()!=112)throw new InvalidDataException("EP1 aura header must be 112 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietBunker","Soviet/Structures/SovietBunker.xml"),("SovietHeavyAntiVehicleInfantry","Soviet/Units/SovietHeavyAntiVehicleInfantry.xml"),("JapanScoutInfantry","Japan/Units/JapanScoutInfantry.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"AttributeModifierAuraUpdate")){
                uint id=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: retail token targets independently anchor every inline aura field; ObjectFilter points to its own token table.
                int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
                foreach(var field in typeof(Ep1AttributeModifierAuraUpdateModuleData).GetFields().Where(f=>f.Name is not "Base" and not "ObjectFilter"))
                    if(fields[FastHash.GetHashCode(field.Name)].Offset-module.Native!=Marshal.OffsetOf<Ep1AttributeModifierAuraUpdateModuleData>(field.Name).ToInt32())throw new InvalidDataException("Stock aura field offset differs: "+field.Name);
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("AttributeModifierAuraUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(Ep1AttributeModifierAuraUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1AttributeModifierAuraUpdateModuleData));count++;
            }
        }
        // Reborn: the same named Upgrade/AnimAndDuration ABI appears in original aircraft and construction-yard subobject upgrades; reuse the populated stock animation grammar.
        foreach(string name in new[]{"AlliedBomberAircraft","AlliedConstructionYard","AlliedGunshipAircraft"}){var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);foreach(var module in GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==SubObjectsUpgradeContract.TypeId))grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));}
        SdkBuildTests.MetadataProgress("AttributeModifierAuraUpdate",$"Reborn: AttributeModifierAura EP1 native/metadata qualification: PASS ({count} original retail records; header=112).",count+" stock records");return grammar;
    }
}
