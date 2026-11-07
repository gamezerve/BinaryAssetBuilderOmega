using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: optimized prop draws are anchored to actual Uprising WorldBuilder source, native bytes, imports and metadata.
internal static class PropDrawContract
{
    internal const uint TypeId=0x148AA464u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove every original prop field and learn the original class grammar without using generated output as its oracle. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1W3DPropDrawModuleData>()!=12)throw new InvalidDataException("EP1 PropDraw header must be twelve bytes.");
        string manifest=WorldBuilderAssetResolver.CatalogPath();
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:AM_Trash01",allowNullArmorReferences:true);
        var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Draws").Modules.Single(m=>m.TypeId==TypeId);
        var source=(XmlDocument)baseline.CloneNode(true);var draws=source.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();
        draws.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original("Civilian/Amsterdam_AM/Destructible_Props/AM_Trash01.xml","PropDraw").Single(),true));
        var known=catalog.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToHashSet();
        var available=catalog.Concat(stock.Document.Assets.Where(a=>!known.Contains(new(a.TypeId,a.InstanceId)))).ToArray();
        var resolved=SchemaNativeContract.Resolve(source,schemas,available).GetElementsByTagName("PropDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
        SchemaNativeContract.VerifyStock(resolved,typeof(Ep1W3DPropDrawModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray(),required:true);
        var grammar=new StockMetadataGrammar();grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1W3DPropDrawModuleData));return grammar;
    }
}
