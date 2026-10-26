using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify weapon-fire ability native and metadata layouts against original Uprising spellbooks.
internal static class WeaponFireAbilityContract
{
    internal const string Wire="WeaponFireSpecialAbilityUpdateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all weapon-fire ability scalars and references and inherited fields against original spellbook weapon-fire powers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1WeaponFireSpecialAbilityUpdateModuleData>()!=284)throw new InvalidDataException("EP1 weapon-fire ability header must be 284 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"WeaponFireSpecialAbilityUpdate");
            // Reborn: qualify all stock storage modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: direct metadata anchors independently prove every scalar field, including zero/default values and the final byte flag.
                int ownTable=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var ownFields=GameObjectStockReview.Tokens(captured.Bytes,ownTable);
                foreach(string field in new[]{"SpecialWeapon","WhichSpecialWeapon","SkipContinue","CanTriggerWhileDead","BusyForDuration","NeedLivingTargets","PlayWeaponPreFireFX","OffsetTargetLocationFromObject","WeaponSlotID","WeaponSlotType","DestealthClosestObjectMatchingFilter","ModelConditionsOnUnpack"}){
                    int actual=ownFields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(field)].Offset-module.Native;int expected=Marshal.OffsetOf<Ep1WeaponFireSpecialAbilityUpdateModuleData>(field).ToInt32();
                    if(actual!=expected)throw new InvalidDataException("Weapon-fire ability retail offset differs: "+field+" actual="+actual+", expected="+expected);
                }
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("WeaponFireSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(Ep1WeaponFireSpecialAbilityUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1WeaponFireSpecialAbilityUpdateModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("WeaponFireSpecialAbilityUpdate",$"Reborn: WeaponFireSpecialAbilityUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1WeaponFireSpecialAbilityUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
