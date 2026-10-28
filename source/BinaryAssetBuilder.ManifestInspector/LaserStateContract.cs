using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify laser-state storage and metadata against original Natasha, Apocalypse/Tesla and Future Tank records.
internal static class LaserStateContract
{
    internal const string Wire="LaserStateModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    // Reborn: end-only particles, origin bones with start particles and end offsets, and status validation with RequiresWeapon=false.
    internal static readonly (string Name,string Path)[] Witnesses={("SovietCommandoTech1","Soviet/Units/SovietCommandoTech1.xml"),("SovietAntiVehicleVehicleTech1","Soviet/Units/SovietAntiVehicleVehicleTech1.xml"),("AlliedFutureTank","EP1/Allied/Units/AlliedFutureTank.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every original native record and learn its retail metadata before authored laser states are composed. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<LaserStateModuleData>()!=48)throw new InvalidDataException("EP1 LaserState native header differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Path,"LaserState");if(modules.Length==0||modules.Length!=originals.Length)throw new InvalidDataException("Original LaserState cardinality differs: "+witness.Name);
            foreach(var module in modules){
                if(SdkBuildTests.Enabled){var probe=(XmlDocument)baseline.CloneNode(true);var behaviors=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var original=originals.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native+4,4)));
                    var imported=(XmlElement)probe.ImportNode(original,true);ConscriptVariableBehaviorSource.DropLeafFormatting(imported);behaviors.AppendChild(imported);
                    var element=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("LaserState",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(LaserStateModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
                grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(LaserStateModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("LaserState",$"Reborn: LaserState native/metadata qualification: PASS ({count} original records, 48-byte root).",count+" original records");return grammar;
    }
}
