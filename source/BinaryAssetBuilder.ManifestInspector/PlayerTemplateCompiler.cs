using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class PlayerTemplateCompiler
{
    // Reborn: verify the EP1 root ABI before serializing a complete, inherited PlayerTemplate.
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if (sizeof(Ep1PlayerTemplate) != 0xBC ||
            Marshal.OffsetOf<Ep1PlayerTemplate>(nameof(Ep1PlayerTemplate.InitialTechPoints)).ToInt32() != 0x6C ||
            Marshal.OffsetOf<Ep1PlayerTemplate>(nameof(Ep1PlayerTemplate.TechProductionBonuses)).ToInt32() != 0xB0)
            throw new InvalidDataException("EP1 PlayerTemplate native layout differs.");
        var xml = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var ns = new XmlNamespaceManager(xml.NameTable);
        ns.AddNamespace("ea", SchemaSet.XmlNamespace);
        Ep1PlayerTemplate* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(Ep1PlayerTemplate), false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("PlayerTemplate native allocation failed.");
        return chunk;
    }
}
