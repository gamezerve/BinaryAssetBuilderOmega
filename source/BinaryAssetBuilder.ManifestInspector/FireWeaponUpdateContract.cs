using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the complete firing update layout against the original Uprising Tesla Trooper and Tesla Tank records.
internal static class FireWeaponUpdateContract
{
    internal const string Wire="FireWeaponUpdateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the firing update fields, their offsets and the Update/Contain inheritance chain against both original Tesla unit records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1FireWeaponUpdateModuleData>()!=32||Marshal.SizeOf<Ep1FireWeaponUpdateModuleDataFireWeaponNugget>()!=16)throw new InvalidDataException("EP1 firing update header must be 32 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietHeavyAntiVehicleInfantry","Soviet/Units/SovietHeavyAntiVehicleInfantry.xml"),("SovietAntiVehicleVehicleTech2","Soviet/Units/SovietAntiVehicleVehicleTech2.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);
            var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"FireWeaponUpdate")){
            uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
            var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            // Reborn: actual retail metadata independently anchors every own field, including the root interval, disabled flags and trailing AliveOnly byte.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"FireInterval","ActiveWhenDisabled","AliveOnly"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<Ep1FireWeaponUpdateModuleData>(name).ToInt32())throw new InvalidDataException("Stock firing update field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("FireWeaponUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(Ep1FireWeaponUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1FireWeaponUpdateModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("FireWeaponUpdate",$"Reborn: FireWeaponUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1FireWeaponUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
