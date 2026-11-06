using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class WeatherCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the EP1 Weather layout before writing the native root and its optional records. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if (sizeof(Ep1Weather) != 24 || sizeof(RainType) != 60 || sizeof(Ep1LightningType) != 12 ||
            sizeof(CloudType) != 8 || sizeof(WeatherData) != 12)
            throw new InvalidDataException("EP1 Weather native layout differs.");
        var xml = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var namespaces = new XmlNamespaceManager(xml.NameTable);
        namespaces.AddNamespace("ea", SchemaSet.XmlNamespace);
        Ep1Weather* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(Ep1Weather), false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, namespaces), root, tracker);
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("Weather native allocation failed.");
        return chunk;
    }
}

