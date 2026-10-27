using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the aiming layout and defaults against original Uprising Flak Trooper and Javelin Soldier records.
internal static class AimWeaponContract
{
    internal const string Wire="AimWeaponBehaviorModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the four aiming floats, their offsets and the Update/Contain inheritance chain against both original infantry records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<AimWeaponBehaviorModuleData>()!=24)throw new InvalidDataException("EP1 aiming header must be 24 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietAntiVehicleInfantry","Soviet/Units/SovietAntiVehicleInfantry.xml"),("AlliedAntiVehicleInfantry","Allied/Units/AlliedAntiVehicleInfantry.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            // Reborn: actual retail metadata independently anchors every own field, including negative and positive threshold defaults.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"AimLowThreshold","AimHighThreshold","AimNearDistance","AimFarDistance"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<AimWeaponBehaviorModuleData>(name).ToInt32())throw new InvalidDataException("Stock AimWeapon field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"AimWeapon").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("AimWeapon",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(AimWeaponBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(AimWeaponBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("AimWeapon",$"Reborn: AimWeapon EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<AimWeaponBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
