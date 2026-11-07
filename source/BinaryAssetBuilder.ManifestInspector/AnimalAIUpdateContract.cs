using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: separate proof views retain independently interpreted AI flags and chooser records beneath the original animal extension.
internal static class AnimalAIUpdateContract
{
    internal const uint TypeId=0xCD56659Bu;
    [StructLayout(LayoutKind.Sequential)]
    internal struct Native
    {
        public AIUpdateContract.Native Base;
        public int FleeRadius,FleeDistance,WanderPercentage,MinDistance,MaxDistance,Radius;
        public Time FleeUpdateTimer,WanderUpdateTimer;
        public float InitialFleeBlindlyRadius;
        public SageBool AfraidOfCastles;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove all authored animal fields, inherited fields, allocations and reference slots independently of the writer. */
    //-------------------------------------------------------------------------------------------------
    internal static SchemaNativeContract.Result Verify(XmlElement source,Relo.Chunk native,IReadOnlyList<AssetId> references)
    {
        if(!string.IsNullOrWhiteSpace(source.GetAttribute("SpecialContactPoints")))throw SdkXmlDiagnostics.At(new InvalidDataException("AnimalAIUpdate inherited SpecialContactPoints item metadata is not implemented."),source.GetAttributeNode("SpecialContactPoints"));
        return SchemaNativeContract.Verify(source,typeof(Native),TypeId,native,references);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify both actual Uprising WorldBuilder civilian AI records from their source and complete native/metadata graphs. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Native>()!=116||Marshal.SizeOf<Ep1AnimalAIUpdateModuleData>()!=116)throw new InvalidDataException("EP1 animal AI header must be 116 bytes.");
        string manifest=WorldBuilderAssetResolver.CatalogPath();var grammar=new StockMetadataGrammar();
        foreach(string identity in new[]{"CivilianFemale1","CivilianMale1"})
        {
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+identity,allowNullArmorReferences:true);
            var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="AI").Modules.Single(m=>m.TypeId==TypeId);
            var source=(XmlDocument)baseline.CloneNode(true);var ai=source.GetElementsByTagName("AI",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();ai.RemoveAll();
            ai.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original("BaseObjects/BaseCivilian.xml","AnimalAIUpdate").Single(),true));
            var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("AnimalAIUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
            SchemaNativeContract.VerifyStock(resolved,typeof(Native),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray(),required:true);
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Native));
        }
        return grammar;
    }
}
