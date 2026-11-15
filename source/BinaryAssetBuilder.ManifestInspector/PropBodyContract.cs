using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify prop body native and metadata layouts against original Uprising spellbooks.
internal static class PropBodyContract
{
    internal const string Wire="PropBodyModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compose the original EA InactiveBody row over the verified retail Body parent. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar QualifyInactive(StockMetadataGrammar propBody)
    {
        if(Marshal.SizeOf<InactiveBodyModuleData>()!=8)
            throw new InvalidDataException("EP1 InactiveBody allocation differs.");
        var rule=new StockMetadataGrammar.Rule(typeof(InactiveBodyModuleData).FullName!,0x29234E6Eu,
            new StockMetadataGrammar.RuleField[]{new(1,0xF49198C2u,2)});
        return StockMetadataGrammar.Import(propBody.Export().Where(r=>r.Class==null).Append(rule));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the identity-only prop body module against original PlayerSpellBook record. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<PropBodyModuleData>()!=8)throw new InvalidDataException("EP1 prop body header must be 8 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Body").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Body",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"PropBody").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("PropBody",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(PropBodyModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(PropBodyModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("PropBody",$"Reborn: PropBody EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<PropBodyModuleData>()}).",count+" stock records");return grammar;
    }
}
