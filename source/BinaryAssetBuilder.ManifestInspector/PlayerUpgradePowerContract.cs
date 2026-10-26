using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify player-upgrade native and metadata layouts against original Uprising spellbooks.
internal static class PlayerUpgradePowerContract
{
    internal const string Wire="PlayerUpgradeSpecialPowerModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all player-upgrade scalars and references and inherited fields against the original base and EP1 player-upgrade powers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<PlayerUpgradeSpecialPowerModuleData>()!=Marshal.SizeOf<SpecialPowerModuleData>()+8)throw new InvalidDataException("EP1 player-upgrade header must add the count and pointer of a strong upgrade list.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml"),("PlayerSpellBook_EP1","EP1/System/PlayerSpellBook_EP1.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"PlayerUpgradeSpecialPower");
            // Reborn: qualify all stock player-upgrade modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: independently bind the upgrade list metadata to its actual native count slot and stock strong-reference signature.
                int ownTable=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var upgrade=GameObjectStockReview.Tokens(captured.Bytes,ownTable)[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Upgrade")];
                int countAt=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(upgrade.Offset+8));
                if(upgrade.Type!=0x4ABA1F29u||countAt!=module.Native+Marshal.OffsetOf<PlayerUpgradeSpecialPowerModuleData>(nameof(PlayerUpgradeSpecialPowerModuleData.Upgrade)).ToInt32())throw new InvalidDataException("Player upgrade stock list anchor/signature differs.");
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("PlayerUpgradeSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(PlayerUpgradeSpecialPowerModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(PlayerUpgradeSpecialPowerModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("PlayerUpgradeSpecialPower",$"Reborn: PlayerUpgradeSpecialPower EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<PlayerUpgradeSpecialPowerModuleData>()}).",count+" stock records");return grammar;
    }
}
