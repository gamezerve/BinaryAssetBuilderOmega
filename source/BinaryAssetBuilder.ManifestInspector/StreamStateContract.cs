using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify stream-state storage and metadata against both original Desolator stream identifiers.
internal static class StreamStateContract
{
    internal const string Wire="StreamStateModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify both original native records; edited signed StreamId values remain governed by the official XSD. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<StreamStateModuleData>()!=12)throw new InvalidDataException("EP1 StreamState native header differs.");
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietDesolatorInfantry");var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
        var originals=GameObjectExtendedRootContract.Original("EP1/Soviet/Units/SovietDesolatorInfantry.xml","StreamState");if(modules.Length!=2||originals.Length!=2)throw new InvalidDataException("Original Desolator StreamState witnesses differ.");var grammar=new StockMetadataGrammar();
        foreach(var module in modules){
            if(SdkBuildTests.Enabled){var probe=(XmlDocument)baseline.CloneNode(true);var behaviors=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var original=originals.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native+4,4)));var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);behaviors.AppendChild(imported);
                var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("StreamState",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(StreamStateModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(StreamStateModuleData));
        }
        SdkBuildTests.MetadataProgress("StreamState","Reborn: StreamState native/metadata qualification: PASS (two original Desolator records, 12-byte root).","2 original records");return grammar;
    }
}
