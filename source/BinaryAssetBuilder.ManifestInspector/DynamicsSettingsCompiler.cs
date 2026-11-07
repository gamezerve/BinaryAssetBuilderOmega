using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: expose the existing EP1 dynamics marshaler to authored asset streams.
internal static class DynamicsSettingsCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the map physics capacities and global island flag in the 24-byte EP1 record. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if (sizeof(DynamicsSettings) != 24)
            throw new InvalidDataException("EP1 DynamicsSettings native layout differs.");

        var xml = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var namespaces = new XmlNamespaceManager(xml.NameTable);
        namespaces.AddNamespace("ea", SchemaSet.XmlNamespace);
        DynamicsSettings* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(DynamicsSettings), false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, namespaces), root, tracker);
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("EP1 DynamicsSettings native allocation failed.");
        return chunk;
    }
}
