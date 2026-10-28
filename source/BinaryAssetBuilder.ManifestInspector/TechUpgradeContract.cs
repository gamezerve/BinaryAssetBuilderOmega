using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify original Allied transmitter inheritance, optional radii and inline filters.
internal static class TechUpgradeContract
{
    internal const string Wire="TechUpgradeModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read the original Outpost tech-level module independently of authored mod include chains. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement Original(int level)=>GameObjectExtendedRootContract.Original("Allied/Structures/AlliedOutPost.xml","TechUpgrade").Single(e=>e.GetAttribute("id")=="ModuleTag_TechUpgrade_Tech"+level);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind all original transmitter records to their real source fragments and retain complete inherited upgrade/animation grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<TechUpgradeModuleData>()!=176)throw new InvalidDataException("EP1 tech-upgrade header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(string name in new[]{"AlliedOutpost","AlliedConstructionYard"}){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);
            var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original("Allied/Structures/"+(name=="AlliedOutpost"?"AlliedOutPost":name)+".xml","TechUpgrade")){
                uint id=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(m.Native+4))==id);
                if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("TechUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(TechUpgradeModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
                grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(TechUpgradeModuleData));count++;
            }
        }
        // Reborn: existing original subobject nuggets supply the shared optional AnimAndDuration grammar for inherited upgrades.
        foreach(string name in new[]{"AlliedBomberAircraft","AlliedConstructionYard","AlliedGunshipAircraft"}){var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);foreach(var module in GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==SubObjectsUpgradeContract.TypeId))grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));}
        SdkBuildTests.MetadataProgress("TechUpgrade",$"Reborn: TechUpgrade: PASS ({count} original records; complete 176-byte native and metadata graphs).",count+" original records");return grammar;
    }
}
