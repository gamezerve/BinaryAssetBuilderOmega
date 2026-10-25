using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: death weapons retain their existing EP1 marshaler; authored death conditions, offsets and probability rows use complete source/native proof.
internal static class FireWeaponDeathContract
{
    internal const string Wire="FireWeaponWhenDeadBehaviorModuleData";
    internal const uint TypeId=0x90CEB49Cu;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: anchor the reactor explosion, infantry catalyst and populated probability list to independently captured retail payloads and metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1FireWeaponWhenDeadBehaviorModuleData>()!=148)throw new InvalidDataException("EP1 death-weapon header must be 148 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietPowerPlantAdvanced","Soviet/Structures/SovietPowerPlantAdvanced.xml"),("SovietAntiInfantryInfantry","BaseObjects/BaseInfantry.xml"),("SovietMiner","Includes/SovietTerrorDroneSpawnUpgradePlayerPower.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"FireWeaponWhenDead")){
                uint local=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4,4))==local);
                // Reborn: required native metadata is independent of optional recompilation of the original stock XML.
                if(!SdkBuildTests.Enabled){grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1FireWeaponWhenDeadBehaviorModuleData));count++;continue;}
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("FireWeaponWhenDead",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                try{SchemaNativeContract.VerifyStock(resolved,typeof(Ep1FireWeaponWhenDeadBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());}catch(InvalidDataException error){throw new InvalidDataException("Death-weapon stock witness "+witness.Item1+":"+original.GetAttribute("id")+": "+error.Message,error);}grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1FireWeaponWhenDeadBehaviorModuleData));count++;
            }
        }
        if(count<3)throw new InvalidDataException("Death-weapon original stock witnesses are incomplete.");
        SdkBuildTests.MetadataProgress("FireWeaponWhenDead",$"Reborn: FireWeaponWhenDead EP1 native/metadata qualification: PASS ({count} original reactor/building module(s)).",count+" stock records");return grammar;
    }
}
