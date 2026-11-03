using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile the campaign enumeration, campaign and mission records as one typed asset family.
internal static class CampaignManagerCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: ignore formatting whitespace inside schema-empty campaign movie and commander elements. */
    //-------------------------------------------------------------------------------------------------
    internal static void NormalizeFormatting(XmlElement root)
    {
        foreach (var leaf in root.ChildNodes.OfType<XmlElement>().Where(e =>
                     e.LocalName is "IntroMovie" or "FinalMovie" or "AlternateFinalMovie" or "AICommander"))
        foreach (var whitespace in leaf.ChildNodes.Cast<XmlNode>().Where(n =>
                     n.NodeType is XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace).ToArray())
            leaf.RemoveChild(whitespace);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select the native EP1 record and marshal normalized references through the shared tracker. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe (Chunk Data, Type Layout) Compile(XmlNode source)
    {
        var xml = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var element = xml.DocumentElement!;
        var ns = new XmlNamespaceManager(xml.NameTable);
        ns.AddNamespace("ea", SchemaSet.XmlNamespace);
        var node = new Node(element.CreateNavigator()!, ns);
        void* root;
        Type layout = element.LocalName switch
        {
            "CampaignEnumerator" => typeof(Ep1CampaignEnumerator),
            "CampaignTemplate" => typeof(Ep1CampaignTemplate),
            "MissionTemplate" => typeof(Ep1MissionTemplate),
            _ => throw new InvalidDataException("Unknown EP1 campaign record: " + element.LocalName)
        };
        int size = layout == typeof(Ep1CampaignEnumerator) ? sizeof(Ep1CampaignEnumerator)
            : layout == typeof(Ep1CampaignTemplate) ? sizeof(Ep1CampaignTemplate) : sizeof(Ep1MissionTemplate);
        using var tracker = new Tracker(&root, (uint)size, false);
        if (layout == typeof(Ep1CampaignEnumerator))
            Marshaler.Marshal(node, (Ep1CampaignEnumerator*)root, tracker);
        else if (layout == typeof(Ep1CampaignTemplate))
            Marshaler.Marshal(node, (Ep1CampaignTemplate*)root, tracker);
        else
            Marshaler.Marshal(node, (Ep1MissionTemplate*)root, tracker);
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("EP1 campaign native allocation failed.");
        return (chunk, layout);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare all original campaign graphs against the linked global stream. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string global, SchemaSet schemas, ManifestAsset[] catalog)
    {
        var document = ManifestReader.Read(File.ReadAllBytes(global));
        int count = 0;
        foreach (string type in new[] { "CampaignEnumerator", "CampaignTemplate", "MissionTemplate" })
        foreach (var original in GameObjectExtendedRootContract.Original("GlobalData/CampaignTemplates.xml", type))
        {
            var source = SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));
            NormalizeFormatting(source.DocumentElement!);
            ConscriptBehaviorSurvey.ResolveStrong(source, schemas, catalog);
            var instance = ReferencePipelineSmokeTest.Normalize(schemas, source.OuterXml);
            var (native, layout) = Compile(instance.XmlNode);
            var asset = document.Assets.Single(a => a.Name == type + ":" + original.GetAttribute("id"));
            new SelectedNativeGraphComparer(W3DContainerCompiler.ReadStock(global, document, asset),
                asset.References.ToArray(), native, instance, captureNativeCoordinates: false).Compare(layout, 0);
            count++;
        }
        Console.WriteLine($"Reborn: campaign native comparison PASS ({count} original assets).");
    }
}
