using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile the untokenized EP1 destination decal settings from authored XML.
internal static class UIDecalDestinationMarkerCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal Core-normalized render imports and relocate the destination override list. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if (sizeof(UIDecalDestinationMarker) != 32 || sizeof(UIDecalDestinationMarkerDestinationTypeSettings) != 24)
            throw new InvalidDataException("EP1 destination marker native layout differs.");

        var xml = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var namespaces = new XmlNamespaceManager(xml.NameTable);
        namespaces.AddNamespace("ea", SchemaSet.XmlNamespace);
        UIDecalDestinationMarker* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(UIDecalDestinationMarker), false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, namespaces), root, tracker);
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("EP1 destination marker native allocation failed.");
        return chunk;
    }
}
