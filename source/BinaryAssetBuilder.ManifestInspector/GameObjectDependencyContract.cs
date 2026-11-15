using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: optional build prerequisites own typed weak IDs, source-sized arrays and independently proved dependency records.
internal static class GameObjectDependencyContract
{
    // Reborn: direct owner selection preserves absent versus authored-empty dependencies.
    internal static XmlElement? Owner(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="GameDependency");
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only a comparison copy restores stock presence; actual source must retain every dependency condition and weak list entry. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        var owner=Owner(source);var model=Owner(baseline);if(owner!=null)source.DocumentElement!.RemoveChild(owner);if(model==null)return;
        var following=model.NextSibling;XmlElement? next=null;while(following!=null){if(following is XmlElement element&&(next=source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==element.LocalName))!=null)break;following=following.NextSibling;}
        source.DocumentElement!.InsertBefore(source.ImportNode(model,true),next);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently prove the actual original refinery dependency source/native bytes before learning real stock field/list metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");
        // Reborn: fast builds need the independent token layout; only developer builds compare it with the original XML.
        if(SdkBuildTests.Enabled)
        {
            var original=GameObjectExtendedRootContract.Original("Soviet/Structures/SovietRefinery.xml","GameDependency").Single();
            var probe=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(baseline.DocumentElement!.OuterXml));
            Mask(probe,baseline);
            probe.DocumentElement!.InsertBefore(probe.ImportNode(original,true),probe.DocumentElement.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName=="ObjectResourceInfo"));
            var resolved=SchemaNativeContract.Resolve(probe,schemas,catalog);
            int native=checked((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(stock.Projection.NativeRoot+412,4)));
            SchemaNativeContract.VerifyStock(Owner(resolved)!,typeof(GameDependencyType),0,stock.Bytes,native,stock.Asset.References.ToArray());
        }
        var grammar=new StockMetadataGrammar();grammar.Learn(stock.Bytes,GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("GameDependency")].Offset,stock.Projection.NativeRoot,typeof(GameDependencyType));
        // Reborn: the refinery's real engineer filter supplies ObjectFilter grammar even though its own build dependency has no filter.
        var engineer=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.Type=="EngineerContainModuleData");grammar.Learn(stock.Bytes,engineer.Wrapper,stock.Projection.NativeRoot,typeof(Ep1EngineerContainModuleData));return grammar;
    }
}
