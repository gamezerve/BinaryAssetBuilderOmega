using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify teleport-storage native and metadata layouts against original Uprising spellbooks.
internal static class TeleportStoredObjectsContract
{
    internal const string Wire="TeleportStoredObjectsSpecialPowerModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all teleport-storage scalars and references and inherited fields against the original Chrono Swap and Chrono Rift powers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<TeleportStoredObjectsSpecialPowerModuleData>()!=Marshal.SizeOf<SpecialPowerModuleData>()+12)throw new InvalidDataException("EP1 teleport-storage header must add link/OCL words and an aligned boolean flag.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"TeleportStoredObjectsSpecialPower");
            // Reborn: qualify all stock storage modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: all retail samples omit the second-click flag, so compare its actual token offset instead of inferring an ABI from zero bytes.
                int ownTable=GameObjectStockReview.Tokens(captured.Bytes,module.Wrapper)[2].Offset;var ownFields=GameObjectStockReview.Tokens(captured.Bytes,ownTable);
                foreach(string field in new[]{"StoreObjectsLinkID","OCL","GetObjectsCurrentlyAtStorePosition"}){
                    int actual=ownFields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(field)].Offset-module.Native;int expected=Marshal.OffsetOf<TeleportStoredObjectsSpecialPowerModuleData>(field).ToInt32();
                    if(actual!=expected)throw new InvalidDataException("Teleport storage retail offset differs: "+field+" actual="+actual+", expected="+expected);
                }
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("TeleportStoredObjectsSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(TeleportStoredObjectsSpecialPowerModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(TeleportStoredObjectsSpecialPowerModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("TeleportStoredObjectsSpecialPower",$"Reborn: TeleportStoredObjectsSpecialPower EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<TeleportStoredObjectsSpecialPowerModuleData>()}).",count+" stock records");return grammar;
    }
}
