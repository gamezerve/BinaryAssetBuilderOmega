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
    /** Reborn: match both bare and type-qualified references to the same local Texture declaration. */
    //-------------------------------------------------------------------------------------------------
    internal static string TextureId(XmlElement radar)
    {
        const string prefix = "Texture:";
        string reference = radar.GetAttribute("Texture").Trim();
        return reference.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? reference[prefix.Length..] : reference;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a radar texture can be fitted only when all of its radar owners declare the same full, uncropped canvas. */
    //-------------------------------------------------------------------------------------------------
    internal static (int Width, int Height)? FullTextureSize(IEnumerable<XmlElement> radars)
    {
        (int Width, int Height)? result = null;
        foreach (var radar in radars)
        {
            var position = ReadCoordinate(radar, "ImagePosition");
            var image = ReadCoordinate(radar, "ImageSize");
            var size = ReadCoordinate(radar, "TextureSize");
            if (position != (0, 0) || size == null || image != size || (result != null && result != size))
                return null;
            result = size;
        }
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use explicit coordinate pairs and leave invalid XML to the normal schema diagnostics. */
    //-------------------------------------------------------------------------------------------------
    private static (int X, int Y)? ReadCoordinate(XmlElement radar, string name)
    {
        var element = radar[name, SchemaSet.XmlNamespace];
        if (element == null || !int.TryParse(element.GetAttribute("x"), out int x) || !int.TryParse(element.GetAttribute("y"), out int y))
            return null;
        return (x, y);
    }

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
