using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the Iga campaign map carries non-tokenized RadarTerrainImage records with this type hash.
internal static class RadarTerrainImageCompiler
{
    internal const uint TypeHash = 0xFBB7431E;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the retail 40-byte radar record with its strong texture import. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var ns = new XmlNamespaceManager(xml.NameTable);
        ns.AddNamespace("ea", SchemaSet.XmlNamespace);
        Ep1RadarTerrainImage* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(Ep1RadarTerrainImage), false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("EP1 radar terrain image allocation failed.");
        return chunk;
    }
}
