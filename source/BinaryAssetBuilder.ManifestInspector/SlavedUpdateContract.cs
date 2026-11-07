using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: slaved drones retain the original Uprising leash, repair, death and optional guard-position record.
internal static class SlavedUpdateContract
{
    internal const string Wire="SlavedUpdateModuleData";
    internal const uint TypeId=0xC8E1225Fu;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the complete original 112-byte drone module before learning its metadata grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<SlavedUpdateModuleData>()!=112 || Marshal.OffsetOf<SlavedUpdateModuleData>(nameof(SlavedUpdateModuleData.GuardPosOffset)).ToInt32()!=104)
            throw new InvalidDataException("EP1 SlavedUpdate layout differs.");
        const string identity="GameObject:AlliedAttackDrone_Leeched";
        var stock=GameObjectStockReview.Capture(manifest,identity,allowNullArmorReferences:true);
        var module=GameObjectModuleReview.Inspect(manifest,identity,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
        var source=(XmlDocument)baseline.CloneNode(true);
        var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
        var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Allied/Units/AlliedAttackDrone_Leeched.xml","SlavedUpdate").Single(),true);
        ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
        var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("SlavedUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
        SchemaNativeContract.VerifyStock(resolved,typeof(SlavedUpdateModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray(),required:true);
        var grammar=new StockMetadataGrammar();grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(SlavedUpdateModuleData));
        return grammar;
    }
}
