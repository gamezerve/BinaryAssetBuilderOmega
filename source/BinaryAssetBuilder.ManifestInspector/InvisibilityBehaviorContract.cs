using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify invisibility behavior native and metadata layouts against the original Uprising Soviet Bunker.
internal static class InvisibilityBehaviorContract
{
    internal const string Wire="InvisibilityBehaviorModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the strong invisibility-template field and inherited identity against the original Soviet Bunker record. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<InvisibilityBehaviorModuleData>()!=12)throw new InvalidDataException("EP1 invisibility behavior header must be 12 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietBunker","Soviet/Structures/SovietBunker.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            // Reborn: actual retail metadata independently anchors the template import slot and its concrete strong-reference wire signature.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"InvisibilityTemplate"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<InvisibilityBehaviorModuleData>(name).ToInt32())throw new InvalidDataException("Stock InvisibilityBehavior field offset differs: "+name);
            if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("InvisibilityTemplate")].Type!=0x914CA765u)throw new InvalidDataException("Stock InvisibilityTemplate reference signature differs.");
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"InvisibilityBehavior").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("InvisibilityBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(InvisibilityBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(InvisibilityBehaviorModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("InvisibilityBehavior",$"Reborn: InvisibilityBehavior EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<InvisibilityBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
