using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify player-power-manager native and metadata layouts against original Uprising spellbooks.
internal static class PlayerPowerManagerContract
{
    internal const string Wire="PlayerPowerManagerBehaviorModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the power and EVA import player-power-manager module against original base and EP1 spellbook records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1PlayerPowerManagerBehaviorModuleData>()!=16)throw new InvalidDataException("EP1 player-power-manager header must be 16 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml"),("PlayerSpellBook_EP1","EP1/System/PlayerSpellBook_EP1.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"PlayerPowerManager");
            // Reborn: qualify both optional-EVA states when present without relying on fixed module positions or source counts.
            foreach(var original in originals.GroupBy(e=>string.IsNullOrEmpty(e.GetAttribute("OnAvailableEvaEvent"))).Select(g=>g.First())){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("PlayerPowerManager",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(Ep1PlayerPowerManagerBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1PlayerPowerManagerBehaviorModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("PlayerPowerManager",$"Reborn: PlayerPowerManager EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1PlayerPowerManagerBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
