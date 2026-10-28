using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the original ore-delivery module and two armor-mode tracking records without copying retail payloads into authored output.
internal static class MinerBehaviorContract
{
    internal static readonly Type[] Types={typeof(OreMinerModuleData),typeof(TrackIncomingWeaponFireBehaviorModuleData),typeof(TrackDamageModuleData)};
    internal static bool Supports(Type type)=>Types.Contains(type);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: recover inherited delivery settings and the original Soviet-specific status gate as one effective source module. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement Original(string name)
    {
        if(name!="OreMinerBehavior")return GameObjectExtendedRootContract.Original("Soviet/Units/SovietMiner.xml",name).Single();
        var owner=(XmlElement)GameObjectExtendedRootContract.Original("BaseObjects/BaseMiner.xml",name).Single().CloneNode(true);
        var validation=GameObjectExtendedRootContract.Original("Soviet/Units/SovietMiner.xml",name).Single().ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="ObjectStatusValidation");owner.AppendChild(owner.OwnerDocument!.ImportNode(validation,true));return owner;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every own field, optional status mask, inheritance row and native module extent on the real Soviet Miner. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<OreMinerModuleData>()!=20||Marshal.SizeOf<TrackIncomingWeaponFireBehaviorModuleData>()!=8||Marshal.SizeOf<TrackDamageModuleData>()!=16)throw new InvalidDataException("Miner behavior EP1 native extents differ.");
        var grammar=new StockMetadataGrammar();var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietMiner");var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        foreach(var item in new[]{("OreMinerBehavior",typeof(OreMinerModuleData)),("TrackIncomingWeaponFireBehavior",typeof(TrackIncomingWeaponFireBehaviorModuleData)),("TrackDamage",typeof(TrackDamageModuleData))}){
            uint typeId=FastHash.GetHashCode(item.Item2.Name);var module=modules.Single(m=>m.TypeId==typeId);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(Original(item.Item1),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(item.Item1,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,item.Item2,typeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,item.Item2);
        }
        SdkBuildTests.MetadataProgress("Miner behaviors","Reborn: miner behavior EP1 native/metadata qualification: PASS (ore delivery, incoming-fire tracking and damage tracking; three retail records).","3 original records");return grammar;
    }
}
