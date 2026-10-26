using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify attack-assist response native and metadata layouts against the original Uprising Tesla Coil.
internal static class AttackAssistResponseContract
{
    internal const string Wire="AttackAssistResponseBehaviorModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the status mask and both weapon-slot words against original Tesla Coil, Mirage Tower and Tesla Trooper records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<AttackAssistResponseBehaviorModuleData>()!=48)throw new InvalidDataException("EP1 attack-assist response header must be 48 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietBaseDefenseAdvanced","Soviet/Structures/SovietBaseDefenseAdvanced.xml"),("AlliedBaseDefenseAdvanced","Allied/Structures/AlliedBaseDefenseAdvanced.xml"),("SovietHeavyAntiVehicleInfantry","Soviet/Units/SovietHeavyAntiVehicleInfantry.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            // Reborn: actual retail metadata independently anchors every own field, including the full 32-byte object-status mask.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"ObjectStatus","WeaponSlotID","WeaponSlotType"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<AttackAssistResponseBehaviorModuleData>(name).ToInt32())throw new InvalidDataException("Stock AttackAssistResponseBehavior field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"AttackAssistResponseBehavior").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("AttackAssistResponseBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(AttackAssistResponseBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(AttackAssistResponseBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("AttackAssistResponseBehavior",$"Reborn: AttackAssistResponseBehavior EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<AttackAssistResponseBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
