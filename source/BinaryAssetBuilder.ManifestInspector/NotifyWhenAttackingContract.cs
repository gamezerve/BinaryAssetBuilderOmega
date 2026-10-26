using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify attack-notification native and metadata layouts against the original Uprising Tesla Coil.
internal static class NotifyWhenAttackingContract
{
    internal const string Wire="NotifyWhenAttackingBehaviorModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the notification fields and complete friend-filter graph against the original SovietBaseDefenseAdvanced record. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<NotifyWhenAttackingBehaviorModuleData>()!=40)throw new InvalidDataException("EP1 attack-notification header must be 40 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietBaseDefenseAdvanced","Soviet/Structures/SovietBaseDefenseAdvanced.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            // Reborn: actual retail metadata independently anchors every own primitive slot; AcceptableFriends is a pointed-to table, not a scalar.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"NotifyRadius","AssistResponseModuleId","SleepTime","MaxFriendsToNotify","WeaponSlotID","WeaponSlotType","HasMobileFriends","IgnoreDisabledStatus"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<NotifyWhenAttackingBehaviorModuleData>(name).ToInt32())throw new InvalidDataException("Stock NotifyWhenAttackingBehavior field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"NotifyWhenAttackingBehavior").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("NotifyWhenAttackingBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(NotifyWhenAttackingBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(NotifyWhenAttackingBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("NotifyWhenAttackingBehavior",$"Reborn: NotifyWhenAttackingBehavior EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<NotifyWhenAttackingBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
