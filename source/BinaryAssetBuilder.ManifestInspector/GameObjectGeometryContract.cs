using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: Geometry shape/contact counts and optional records belong to authored source, not the Conscript stock layout.
internal static class GameObjectGeometryContract
{
    // Reborn: select the direct root owner, retaining source provenance for unsupported presence diagnostics.
    internal static XmlElement? Owner(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="Geometry");
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore only a comparison copy; actual Geometry is proved independently from its schema-qualified source and stock metadata grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        var actual=Owner(source);var model=Owner(baseline);
        // Reborn: remove only comparison geometry when the authored object omits this optional owner.
        if(actual==null){if(model!=null)baseline.DocumentElement!.RemoveChild(model);return;}
        if(model==null)throw new InvalidDataException("Geometry comparison witness is missing.");
        source.DocumentElement!.ReplaceChild(source.ImportNode(model,true),actual);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: original dying Kirov data proves that omitted Geometry owns neither a native allocation nor a metadata table. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyAbsent(string manifest)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietBomberAircraft_Dying",allowNullArmorReferences:true);
        var row=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("Geometry")];
        if(System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(stock.Projection.NativeRoot+500))!=0||row.Type!=FastHash.GetHashCode("Geometry")||row.Offset!=0)throw new InvalidDataException("Retail omitted Geometry native/metadata declaration differs.");
        return true;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify independent schema/native derivation on the real stock Conscript and learn Geometry row grammar from infantry and refinery stock. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var rows=GameObjectStockReview.Tokens(stock.Bytes,0);
        var source=SchemaNativeContract.Resolve(baseline,schemas,catalog);int native=checked((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(stock.Projection.NativeRoot+500,4)));
        SchemaNativeContract.VerifyStock(Owner(source)!,typeof(Ep1Geometry),0,stock.Bytes,native,stock.Asset.References.ToArray());
        var grammar=new StockMetadataGrammar();grammar.Learn(stock.Bytes,rows[FastHash.GetHashCode("Geometry")].Offset,stock.Projection.NativeRoot,typeof(Ep1Geometry));
        var refinery=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");grammar.Learn(refinery.Bytes,GameObjectStockReview.Tokens(refinery.Bytes,0)[FastHash.GetHashCode("Geometry")].Offset,refinery.Projection.NativeRoot,typeof(Ep1Geometry));
        // Reborn: existing geometry stock witnesses cover optional placement/rotation/contact and EP1 RING records, not only infantry/building collision shapes.
        foreach(string name in new[]{"AlliedConstructionYard","JapanFortressShip","AlliedPowerPlantTurbine","NanoSwarmHiveEffect"}){var witness=GameObjectStockReview.Capture(manifest,"GameObject:"+name);grammar.Learn(witness.Bytes,GameObjectStockReview.Tokens(witness.Bytes,0)[FastHash.GetHashCode("Geometry")].Offset,witness.Projection.NativeRoot,typeof(Ep1Geometry));}
        return grammar;
    }
}
