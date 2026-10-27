using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the Uprising detector layout and inherited upgrade metadata against three original retail objects.
internal static class StealthDetectorContract
{
    internal const string Wire="StealthDetectorUpdateModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify all detector offsets, defaults, imports and mask graphs against the three original faction scouts. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1StealthDetectorUpdateModuleData>()!=164)throw new InvalidDataException("EP1 stealth detector header must be 164 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietScoutInfantry","Soviet/Units/SovietScoutInfantry.xml"),("AlliedScoutInfantry","Allied/Units/AlliedScoutInfantry.xml"),("JapanScoutInfantry","Japan/Units/JapanScoutInfantry.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"StealthDetectorUpdate")){
                uint id=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: retail token targets independently anchor every own detector field, including the inline string header and both KindOf masks.
                int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
                foreach(var field in typeof(Ep1StealthDetectorUpdateModuleData).GetFields().Where(f=>f.Name!="Base"))
                    if(fields[FastHash.GetHashCode(field.Name)].Offset-module.Native!=Marshal.OffsetOf<Ep1StealthDetectorUpdateModuleData>(field.Name).ToInt32())throw new InvalidDataException("Stock detector field offset differs: "+field.Name);
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("StealthDetectorUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(Ep1StealthDetectorUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1StealthDetectorUpdateModuleData));count++;
            }
        }
        // Reborn: the same named Upgrade/AnimAndDuration ABI appears in original aircraft and construction-yard subobject upgrades; reuse the populated stock animation grammar.
        foreach(string name in new[]{"AlliedBomberAircraft","AlliedConstructionYard","AlliedGunshipAircraft"}){var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);foreach(var module in GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==SubObjectsUpgradeContract.TypeId))grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));}
        SdkBuildTests.MetadataProgress("StealthDetectorUpdate",$"Reborn: StealthDetectorUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header=164).",count+" stock records");return grammar;
    }
}
