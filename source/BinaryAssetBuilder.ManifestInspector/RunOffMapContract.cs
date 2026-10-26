using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify run-off-map native and metadata layouts against original Uprising support-bomber and satellite objects.
internal static class RunOffMapContract
{
    internal const string Wire="RunOffMapBehaviorModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the waypoint strings, trigger/death/flight flags and default values against original Uprising support-bomber and satellite records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<RunOffMapBehaviorModuleData>()!=20)throw new InvalidDataException("EP1 run-off-map header must be 20 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietSupportBomber","Soviet/Units/BaseSovietSupportBomber.xml"),("AlliedSatelliteSweepShroudRevealer","Allied/Props/AlliedSatelliteSweepShroudRevealer.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            // Reborn: actual retail metadata independently anchors every own field, including the waypoint string header.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"RequiresSpecificTrigger","RunOffMapWaypointName","DieOnMap","FlyingOffMap"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<RunOffMapBehaviorModuleData>(name).ToInt32())throw new InvalidDataException("Stock RunOffMapBehavior field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"RunOffMapBehavior").Single(),true));
                // Reborn: instance directives have already served their source-overlay purpose before leaf compilation.
                foreach(var attribute in source.GetElementsByTagName("RunOffMapBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().Attributes.Cast<XmlAttribute>().Where(a=>a.LocalName=="joinAction").ToArray())attribute.OwnerElement!.RemoveAttributeNode(attribute);
                // Reborn: this specific retail support bomber stores literal RunOffMap although its published base XML omits the attribute; qualify that documented comparison-only override, never impose it on authored defaults. The satellite independently qualifies the omitted/null case.
                if(witness.Item1=="SovietSupportBomber")source.GetElementsByTagName("RunOffMapBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().SetAttribute("RunOffMapWaypointName","RunOffMap");
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("RunOffMapBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(RunOffMapBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(RunOffMapBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("RunOffMapBehavior",$"Reborn: RunOffMapBehavior EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<RunOffMapBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
