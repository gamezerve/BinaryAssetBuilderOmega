using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify Allied receiver inheritance fragments against original Barracks tech-level payloads.
internal static class TechUpgradeReceiverContract
{
    internal const string Wire="TechUpgradeReceiverModuleData";
    internal const uint TypeId=0x1773BB2Eu;
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read the unchanged original tech-level fragment independently of any authored mod include chain. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement Original(int level)=>GameObjectExtendedRootContract.Original($"Includes/AlliedTechUpgradeReceiverLevel{level}.xml","TechUpgradeReceiver").Single();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind both original receiver records to their real source fragments and retain complete inherited upgrade/animation grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<TechUpgradeReceiverModuleData>()!=188)throw new InvalidDataException("EP1 receiver header differs.");
        var grammar=new StockMetadataGrammar();var stock=GameObjectStockReview.Capture(manifest,"GameObject:AlliedBarracks");
        var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
        foreach(int level in new[]{2,3}){
            var original=Original(level);uint id=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(m.Native+4))==id);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("TechUpgradeReceiver",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(TechUpgradeReceiverModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(TechUpgradeReceiverModuleData));
        }
        // Reborn: existing original subobject nuggets supply the shared optional AnimAndDuration grammar for inherited upgrades.
        foreach(string name in new[]{"AlliedBomberAircraft","AlliedConstructionYard","AlliedGunshipAircraft"}){var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);foreach(var module in GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==SubObjectsUpgradeContract.TypeId))grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));}
        SdkBuildTests.MetadataProgress("TechUpgradeReceiver","Reborn: TechUpgradeReceiver: PASS (two original Allied tech-level records; complete 188-byte native and metadata graphs).","2 original records");return grammar;
    }
}
