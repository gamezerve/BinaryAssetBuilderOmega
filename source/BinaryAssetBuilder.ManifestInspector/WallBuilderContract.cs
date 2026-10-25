using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: derive the wall-builder native and metadata layout from original Uprising walls.
internal static class WallBuilderContract
{
    internal const string Wire="WallBuilderBehaviorModuleData";
    internal const uint TypeId=0x9590538Au;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify wall length, condition/status flags, inline filter and trailing boolean against Soviet and Allied retail wall records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<WallBuilderBehaviorModuleData>()!=228)throw new InvalidDataException("EP1 wall-builder header must be 228 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietWallPiece","Soviet/Structures/SovietWallPiece.xml"),("AlliedWallPiece","Allied/Structures/AlliedWallPiece.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"WallBuilderBehavior").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("WallBuilderBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(WallBuilderBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(WallBuilderBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("WallBuilderBehavior",$"Reborn: WallBuilderBehavior EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<WallBuilderBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
