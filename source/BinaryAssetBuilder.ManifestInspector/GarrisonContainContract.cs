using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify garrison native and metadata layouts against the original Uprising Bunker and Allied Base Defense.
internal static class GarrisonContainContract
{
    internal const string Wire="GarrisonContainModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the garrison flags, inline roster and complete inherited containment fields against original Bunker and Allied Base Defense records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<GarrisonContainModuleData>()!=168)throw new InvalidDataException("EP1 garrison header must be 168 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietBunker","Soviet/Structures/SovietBunker.xml"),("AlliedBaseDefense","Allied/Structures/AlliedBaseDefense.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            // Reborn: actual retail metadata independently anchors every own primitive slot; the InitialRoster by-value record is independently covered by the full native proof.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"MobileGarrison","ImmuneToClearBuildingAttacks","ResetInitialTeamOnCapture"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<GarrisonContainModuleData>(name).ToInt32())throw new InvalidDataException("Stock GarrisonContain field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"GarrisonContain").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("GarrisonContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(GarrisonContainModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(GarrisonContainModuleData));count++;
        }
        // Reborn: optional inherited member-status items retain the same exact schema-qualified scalar grammar as other OpenContain descendants.
        grammar.QualifyTransportItems(schemas.Schemas);
        SdkBuildTests.MetadataProgress("GarrisonContain",$"Reborn: GarrisonContain EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<GarrisonContainModuleData>()}).",count+" stock records");return grammar;
    }
}
