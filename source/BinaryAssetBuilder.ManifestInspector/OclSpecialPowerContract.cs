using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify OCL-power native and metadata layouts against original Uprising spellbooks.
internal static class OclSpecialPowerContract
{
    internal const string Wire="OCLSpecialPowerModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all OCL-power scalars and references and inherited fields against original PlayerSpellBook OCL powers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1OCLSpecialPowerModuleData>()!=524)throw new InvalidDataException("EP1 OCL-power header must be 524 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"OCLSpecialPower");
            // Reborn: qualify all stock storage modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: direct metadata anchors independently prove every scalar field, including zero/default values and the final byte flag.
                int ownTable=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var ownFields=GameObjectStockReview.Tokens(captured.Bytes,ownTable);
                foreach(string field in new[]{"OCL","CreateLocation","CreateLocationOffset","MaxCreateCount","NumberToSpawn","DestinationOCL","RegisterObjectsWithSpecialAbilityUpdate"}){
                    int actual=ownFields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(field)].Offset-module.Native;int expected=Marshal.OffsetOf<Ep1OCLSpecialPowerModuleData>(field).ToInt32();
                    if(actual!=expected)throw new InvalidDataException("OCL power retail offset differs: "+field+" actual="+actual+", expected="+expected);
                }
                if(ownFields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("UpgradeOCL")].Type!=0xA8B856F2u)throw new InvalidDataException("Retail OCL upgrade list signature differs.");
                grammar.QualifyOclUpgradePair(ownFields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("OCL")].Type);
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("OCLSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(Ep1OCLSpecialPowerModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1OCLSpecialPowerModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("OCLSpecialPower",$"Reborn: OCLSpecialPower EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1OCLSpecialPowerModuleData>()}).",count+" stock records");return grammar;
    }
}
