
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class OnDemandTextureImageCompiler
{
    internal static XmlDocument ProofSource(XmlDocument source, SchemaSet schemas, ManifestAsset[] catalog)
    {
        var proof = SchemaNativeContract.Resolve(source, schemas, catalog);
        proof.DocumentElement!.RemoveAttribute("id");
        return proof;
    }

    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var ns = new XmlNamespaceManager(xml.NameTable);
        ns.AddNamespace("ea", SchemaSet.XmlNamespace);

        OnDemandTextureImage* root;

        using var tracker = new Tracker((void**)&root, (uint)sizeof(OnDemandTextureImage), false);

        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, ns), root, tracker);

        var chunk = new Chunk();

        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("OnDemandTextureImage native allocation failed.");

        return chunk;
    }
}
