using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class ObjectFilterCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the EP1 standalone filter with the shared inline filter marshaler and weak IDs. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlElement source)
    {
        if (sizeof(ObjectFilterAsset) != 124)
            throw new InvalidDataException("EP1 ObjectFilterAsset native layout differs.");

        var namespaces = new XmlNamespaceManager(source.OwnerDocument.NameTable);
        namespaces.AddNamespace("ea", SchemaSet.XmlNamespace);
        var node = new Node(source.CreateNavigator()!, namespaces);
        ObjectFilterAsset* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(ObjectFilterAsset), false);
        Marshaler.Marshal(node, root, tracker);

        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk) || chunk.ImportsBuffer.Length != 0)
            throw new InvalidDataException("ObjectFilterAsset marshalling failed or produced strong imports.");
        return chunk;
    }
}
