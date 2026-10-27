using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;
// Reborn: qualify variable crusher children on the actual EP1 miner and grinder, including optional masks, filters and both list families.
internal static class CrusherChildrenContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: extend the existing root grammar with independently source-qualified populated retail crusher graphs. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(StockMetadataGrammar grammar,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1CrusherInfo>()!=52||Marshal.SizeOf<Ep1CrusherLevelModelConditionInfo>()!=68||Marshal.SizeOf<Ep1CrushKillDelayForObjectFilter>()!=128)throw new InvalidDataException("EP1 crusher root/level/kill-delay ABI differs.");
        foreach(var witness in new[]{("SovietMiner","Soviet/Units/SovietMiner.xml"),("SovietGrinderVehicle","EP1/Soviet/Units/SovietGrinderVehicle.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var source=(XmlDocument)baseline.CloneNode(true);var previous=GameObjectExtendedRootContract.Owners(source,"CrusherInfo").Single();previous.ParentNode!.ReplaceChild(Effective(source,schemas,witness.Item2),previous);
            var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var owner=GameObjectExtendedRootContract.Owners(resolved,"CrusherInfo").Single();var field=BinaryAssetBuilder.XmlCompiler.Ep1GameObjectRootOwnersProjection.Fields.Single(f=>f.Name=="CrusherInfo");int native=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(captured.Projection.NativeRoot+field.Slot));
            SchemaNativeContract.VerifyStock(owner,typeof(Ep1CrusherInfo),0,captured.Bytes,native,captured.Asset.References.ToArray());
            grammar.Learn(captured.Bytes,GameObjectStockReview.Tokens(captured.Bytes,0)[FastHash.GetHashCode("CrusherInfo")].Offset,captured.Projection.NativeRoot,typeof(Ep1CrusherInfo));
        }
        SdkBuildTests.MetadataProgress("CrusherInfo children","Reborn: CrusherInfo EP1 extra levels/kill delays: PASS (2 populated retail graphs, optional model conditions, object filters and native/metadata ownership).","2 populated stock graphs");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retail crusher graphs are the official overlay of the witness onto BaseVehicle (BaseMiner adds no CrusherInfo), so inherited
        DefaultCrushKillDelay and the appended base ExtraCrushLevels/ExtraCrushKillDelays are part of the expected stock record. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement Effective(XmlDocument document,SchemaSet schemas,string relative)
    {
        var basis=GameObjectExtendedRootContract.Original("BaseObjects/BaseVehicle.xml","CrusherInfo").Single();var derived=GameObjectExtendedRootContract.Original(relative,"CrusherInfo").Single();
        var merged=(XmlElement)NodeJoiner.Override(schemas.Schemas,document,Wrap(basis),Wrap(derived));return merged.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="CrusherInfo");
        // Reborn: the Core joiner receives structured XML, as in SdkGameObjectInheritance; formatting whitespace is not payload.
        XmlElement Wrap(XmlElement crusher){var owner=document.CreateElement("GameObject",SchemaSet.XmlNamespace);owner.SetAttribute("id","RebornCrusherWitness");var copy=(XmlElement)document.ImportNode(crusher,true);Strip(copy);owner.AppendChild(copy);return owner;}
        static void Strip(XmlElement element){foreach(var node in element.ChildNodes.Cast<XmlNode>().ToArray()){if(node is XmlWhitespace or XmlSignificantWhitespace or XmlComment||node is XmlText&&string.IsNullOrWhiteSpace(node.Value))element.RemoveChild(node);else if(node is XmlElement child)Strip(child);}}
    }
}
