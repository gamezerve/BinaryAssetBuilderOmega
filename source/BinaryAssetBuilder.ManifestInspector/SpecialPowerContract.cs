using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify base special-power native and metadata layouts against original Uprising spellbooks.
internal static class SpecialPowerContract
{
    internal const string Wire="SpecialPowerModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve the original retail witness aliases from their actual GlobalDefines source; authored builds retain their normal definition context. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement[] Originals()
    {
        var definitions=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").ToDictionary(d=>d.GetAttribute("name"),d=>d.GetAttribute("value"),StringComparer.Ordinal);
        return GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","SpecialPower").Select(original=>{
            var element=(XmlElement)original.CloneNode(true);
            foreach(var attribute in element.SelectNodes(".//@*")!.Cast<XmlAttribute>())if(attribute.Value.StartsWith('='))attribute.Value=SdkDefinitionSubset.Evaluate(attribute.Value,name=>definitions.GetValueOrDefault(name));
            return element;
        }).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all base special-power scalars and references and inherited fields against the original Emperor's Resolve powers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=Originals();
            // Reborn: qualify all stock base-power modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("SpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(SpecialPowerModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(SpecialPowerModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("SpecialPower",$"Reborn: SpecialPower EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<SpecialPowerModuleData>()}).",count+" stock records");return grammar;
    }
}
