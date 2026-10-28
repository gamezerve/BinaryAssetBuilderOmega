using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify LaserDraw's EP1 layout from original laser units (with and without status validation) and their complete retail records.
internal static class LaserDrawContract
{
    internal const uint TypeId=0x2FDDEC4Du;
    // Reborn: Natasha (two shader-constant lasers), Mirage (fade times and a status-validation record) and Future Tank (four validated lasers).
    internal static readonly (string Name,string Path)[] Witnesses={("SovietCommandoTech1","Soviet/Units/SovietCommandoTech1.xml"),("AlliedAntiAirVehicleTech1","Allied/Units/AlliedAntiAirVehicleTech1.xml"),("AlliedFutureTank","EP1/Allied/Units/AlliedFutureTank.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn only actual retail row identities; developer builds additionally compare all source fields and reference targets. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1W3DLaserDrawModuleData>()!=112)throw new InvalidDataException("EP1 LaserDraw native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Path,"LaserDraw");if(modules.Length==0||modules.Length!=originals.Length)throw new InvalidDataException("Original LaserDraw cardinality differs: "+witness.Name);
            foreach(var module in modules){
                if(SdkBuildTests.Enabled){var probe=(XmlDocument)baseline.CloneNode(true);var draws=probe.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();var original=originals.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native+4,4)));
                    var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);draws.AppendChild(imported);
                    var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("LaserDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(Ep1W3DLaserDrawModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
                grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1W3DLaserDrawModuleData));count++;
            }
        }
        ShaderMaterialContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        SdkBuildTests.MetadataProgress("LaserDraw",$"Reborn: LaserDraw EP1 native/metadata qualification: PASS ({count} original records, 112-byte root, shader constants and status validation).",count+" original record(s)");
        return grammar;
    }
}
