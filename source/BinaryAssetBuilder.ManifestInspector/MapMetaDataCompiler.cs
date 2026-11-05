using System.Text;
using System.Xml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: AdditionalMaps catalogs use the same untokenized EP1 native metadata records as TheMapCache.
internal static class MapMetaDataCompiler
{
    internal const uint TypeHash = 0x481DA8E6;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Marshal each map and its start positions through the existing relocation-aware writer. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if (sizeof(MapMetaDataType) != 12 || sizeof(MetaDataObject) != 60 || sizeof(StartPositionObject) != 20)
            throw new InvalidDataException("EP1 map metadata native layout differs.");
        var names = new XmlNamespaceManager(source.OwnerDocument!.NameTable);
        names.AddNamespace("ea", source.NamespaceURI);
        MapMetaDataType* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(MapMetaDataType), false);
        Marshaler.Marshal(new Node(source.CreateNavigator()!, names), root, tracker);
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("EP1 map metadata allocation failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compare the complete original catalog with its extracted retail native graph. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyRetail(string manifest, string source)
    {
        var document = ManifestReader.Read(File.ReadAllBytes(manifest));
        var asset = document.Assets.Single(a => a.Name == "MapMetaData:TheMapCache");
        if (asset.TypeId != 0x5F969146 || asset.TypeHash != TypeHash || asset.Tokenized != 0 || asset.References.Count != 0)
            throw new InvalidDataException("EP1 map metadata fingerprint differs.");
        var xml = SdkXmlDiagnostics.Parse(File.ReadAllBytes(source));
        var root = xml.GetElementsByTagName("MapMetaData", "uri:ea.com:eala:asset")
            .OfType<XmlElement>().Single(e => e.GetAttribute("id") == "TheMapCache");
        var native = Compile(root);
        var stock = W3DContainerCompiler.ReadStock(manifest, document, asset);
        if (!native.InstanceBuffer.SequenceEqual(stock))
            throw new InvalidDataException("EP1 map metadata retail BIN comparison differs.");
        var relocation = AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest, ".relo"), null, 8, asset.RelocationDataSize);
        if (!native.RelocationBuffer.SequenceEqual(relocation) || native.ImportsBuffer.Length != 0)
            throw new InvalidDataException("EP1 map metadata retail relocation/import comparison differs.");
        Console.WriteLine("Reborn: MapMetaData retail BIN/RELO/IMP comparison PASS.");
    }
}
