using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify all four remaining Hammer leech/upgrade families against complete original EP1 records.
internal static class HammerBehaviorContract
{
    internal const string Path="Soviet/Units/SovietAntiVehicleVehicleTech1.xml";
    internal static readonly (string Source,string Wire,Type Type)[] Families={
        ("SecondaryObjectListenerModule","SecondaryObjectListenerModule",typeof(SecondaryObjectListenerModule)),
        ("ConvergingLaserState","ConvergingLaserStateModuleData",typeof(ConvergingLaserStateModuleData)),
        ("ObjectCreationUpgrade","ObjectCreationUpgradeModuleData",typeof(ObjectCreationUpgradeModuleData)),
        ("AudioLoopUpgrade","AudioLoopUpgradeModuleData",typeof(AudioLoopUpgradeModuleData))};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify original module values/imports and retain captured nested/list metadata grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<SecondaryObjectListenerModule>()!=24||Marshal.SizeOf<ObjectEffectTuple>()!=128||Marshal.SizeOf<ConvergingLaserStateModuleData>()!=140||Marshal.SizeOf<ObjectCreationUpgradeModuleData>()!=72||Marshal.SizeOf<AudioLoopUpgradeModuleData>()!=48)
            throw new InvalidDataException("Hammer behavior EP1 ABI extents differ.");
        var grammar=new StockMetadataGrammar();var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiVehicleVehicleTech1");
        var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;int count=0;
        foreach(var family in Families){
            uint typeId=FastHash.GetHashCode(family.Wire);var originals=GameObjectExtendedRootContract.Original(Path,family.Source);
            var selected=modules.Where(m=>m.TypeId==typeId).ToArray();if(selected.Length==0||selected.Length!=originals.Length)throw new InvalidDataException("Hammer original module cardinality differs: "+family.Source);
            foreach(var module in selected){
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                    var original=originals.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native+4)));
                    behaviors.AppendChild(source.ImportNode(original,true));
                    var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(element,family.Type,typeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());
                }
                grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);count++;
            }
        }
        // Reborn: plain laser witnesses qualify populated inherited particles, offsets and status graphs for converging states too.
        foreach(var witness in LaserStateContract.Witnesses){var laser=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);foreach(var module in GameObjectModuleReview.Inspect(manifest,laser.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==LaserStateContract.TypeId))grammar.Learn(laser.Bytes,module.Wrapper,laser.Projection.NativeRoot,typeof(LaserStateModuleData));}
        SdkBuildTests.MetadataProgress("Hammer behaviors",$"Reborn: Hammer behaviors: PASS ({count} original listener/converging/creation/audio records).",count+" original records");return grammar;
    }
}
