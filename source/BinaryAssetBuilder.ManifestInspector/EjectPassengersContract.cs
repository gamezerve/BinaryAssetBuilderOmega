using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently qualify the new passenger-ejection writer against original Uprising aircraft and Bullfrog records.
internal static class EjectPassengersContract
{
    internal const string Wire="EjectPassengersSpecialAbilityUpdateModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    internal static readonly (string Name,string Path)[] Witnesses={
        ("AlliedBomberAircraft","Allied/Units/AlliedBomberAircraft.xml"),
        ("SovietAntiAirShip","Soviet/Units/SovietAntiAirShip.xml")};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: anchor every own scalar field and populated Bullfrog offset in retail metadata, then prove the complete inherited native graph. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<SpecialAbilityUpdateModuleData>()!=252||Marshal.SizeOf<EjectPassengersSpecialAbilityUpdateModuleData>()!=292)throw new InvalidDataException("EP1 passenger-ejection ability/header ABI differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"TimePerPassenger","Speed","Radius","Height","ApexHeight","EjectionFX","EjectionBoneName","RunOffMapWhenDone","IgnoreTargetLocation"})if(fields[FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<EjectPassengersSpecialAbilityUpdateModuleData>(name).ToInt32())throw new InvalidDataException("Stock passenger-ejection field offset differs: "+name);
            int vector=Marshal.OffsetOf<EjectPassengersSpecialAbilityUpdateModuleData>("EjectionBoneOffset").ToInt32();if(fields[FastHash.GetHashCode("EjectionBoneOffset")].Type!=0x29368077u||fields[FastHash.GetHashCode("EjectionBoneOffset")].Offset!=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Native+vector)))throw new InvalidDataException("Stock passenger-ejection vector pointer/tag differs.");
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"EjectPassengersSpecialAbilityUpdate").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("EjectPassengersSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(EjectPassengersSpecialAbilityUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(EjectPassengersSpecialAbilityUpdateModuleData));count++;
        }
        // Reborn: optional inherited AnimAndDuration uses the same named record populated in stock aircraft/yard upgrades.
        foreach(string name in new[]{"AlliedBomberAircraft","AlliedConstructionYard","AlliedGunshipAircraft"}){var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);foreach(var module in GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==SubObjectsUpgradeContract.TypeId))grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));}
        SdkBuildTests.MetadataProgress("EjectPassengersSpecialAbilityUpdate",$"Reborn: EjectPassengersSpecialAbilityUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header=292).",count+" stock records");return grammar;
    }
}
