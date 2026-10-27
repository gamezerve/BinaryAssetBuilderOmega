using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify StreamDraw's EP1 layout from original Desolator source and its complete retail native/metadata record.
internal static class StreamDrawContract
{
    internal const uint TypeId=0x60020441u;
    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn only actual retail row identities; developer builds additionally compare all source fields and reference targets. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var grammar=new StockMetadataGrammar();var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietDesolatorInfantry");
        var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==TypeId).ToArray();
        var originals=GameObjectExtendedRootContract.Original("EP1/Soviet/Units/SovietDesolatorInfantry.xml","StreamDraw");
        if(modules.Length==0||modules.Length!=originals.Length)throw new InvalidDataException("Original Desolator StreamDraw cardinality differs.");
        foreach(var module in modules){
            if(SdkBuildTests.Enabled){var probe=(XmlDocument)baseline.CloneNode(true);var draws=probe.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();var original=originals.Single(e=>BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native+4,4)));draws.AppendChild(probe.ImportNode(original,true));
                var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("StreamDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(Ep1W3DStreamDrawModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1W3DStreamDrawModuleData));
        }
        ShaderMaterialContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        SdkBuildTests.MetadataProgress("StreamDraw","Reborn: StreamDraw EP1 native/metadata qualification: PASS (original Desolator, 80-byte root, shader constants and particle imports).",modules.Length+" original record(s)");
        return grammar;
    }
}
