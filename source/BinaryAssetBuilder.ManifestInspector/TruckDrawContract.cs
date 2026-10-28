using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify TruckDraw's EP1 layout from original vehicles and populated tread scrolling and their complete retail records.
internal static class TruckDrawContract
{
    internal const uint TypeId=0xA44C2999u;
    // Reborn: Mortarcycle qualifies wheel fields and the terrain import; Athena Cannon additionally qualifies populated scrolling records.
    internal static readonly (string Name,string Path)[] Witnesses={("SovietMortarcycle","EP1/Soviet/Units/SovietMortarcycle.xml"),("AlliedAntiStructureVehicle","Allied/Units/AlliedAntiStructureVehicle.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn only actual retail row identities; developer builds additionally compare all source fields and reference targets. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1W3DTruckDrawModuleData>()!=396)throw new InvalidDataException("EP1 TruckDraw native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Path,"TruckDraw");if(modules.Length==0||modules.Length!=originals.Length)throw new InvalidDataException("Original TruckDraw cardinality differs: "+witness.Name);
            foreach(var module in modules){
                if(SdkBuildTests.Enabled){var probe=(XmlDocument)baseline.CloneNode(true);var draws=probe.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();var original=originals.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native+4,4)));
                    var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);draws.AppendChild(imported);
                    var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("TruckDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(Ep1W3DTruckDrawModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
                grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1W3DTruckDrawModuleData));count++;
            }
        }
        // Reborn: inherited optional scripted fields retain the same independently qualified source/metadata capabilities.
        ShaderMaterialContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        ScriptedShadowInfoContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        ScriptedFXEventContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        ScriptedTurretContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        SdkBuildTests.MetadataProgress("TruckDraw",$"Reborn: TruckDraw EP1 native/metadata qualification: PASS ({count} original records, 396-byte root, wheel/cab strings, terrain reference and tread scrolling).",count+" original record(s)");
        return grammar;
    }
}
