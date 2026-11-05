using System.Globalization;
using System.Xml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 movie events store an imported Video pointer after the shared fade fields.
internal static class VideoEventListCompiler
{
    private struct Ep1MovieEvent
    {
        internal VideoEvent Base;
        internal uint Video;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the ordered Movie and TitleCard nodes using the EP1 event discriminators. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlElement source)
    {
        if (sizeof(VideoEventList) != 12 || sizeof(Ep1MovieEvent) != 16)
            throw new InvalidDataException("EP1 VideoEventList requires the x86 layout.");

        var list = source.ChildNodes.OfType<XmlElement>().Single(element => element.LocalName == "EventList");
        var events = list.ChildNodes.OfType<XmlElement>().ToArray();
        var ns = new XmlNamespaceManager(source.OwnerDocument!.NameTable);
        ns.AddNamespace("ea", source.NamespaceURI);

        VideoEventList* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(VideoEventList), false);
        root->EventList.Count = (uint)events.Length;
        if (events.Length != 0)
        {
            using var items = tracker.Push((void**)&root->EventList.Items, (uint)sizeof(VideoEvent*), root->EventList.Count);
            for (int index = 0; index < events.Length; index++)
            {
                var element = events[index];
                var node = new Node(element.CreateNavigator()!, ns);
                VideoEvent** destination = root->EventList.Items + index;
                switch (element.LocalName)
                {
                    case "Movie":
                        string reference = element.GetAttribute("MovieAsset");
                        int separator = reference.LastIndexOf('\\');
                        if (separator < 0 || !uint.TryParse(reference.AsSpan(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out uint import))
                            throw SdkXmlDiagnostics.At(new InvalidDataException("Unresolved Video asset: " + reference), element.GetAttributeNode("MovieAsset"));

                        using (tracker.Push((void**)destination, (uint)sizeof(Ep1MovieEvent), 1))
                        {
                            var movie = (Ep1MovieEvent*)*destination;
                            movie->Base.TypeId = 0xB74490CA;
                            Marshaler.Marshal(node, &movie->Base, tracker);
                            tracker.AddReference(&movie->Video, import);
                        }
                        break;
                    case "TitleCard":
                        using (tracker.Push((void**)destination, (uint)sizeof(TitleCardEvent), 1))
                        {
                            var card = (TitleCardEvent*)*destination;
                            card->Base.TypeId = 0x33A73683;
                            Marshaler.Marshal(node, card, tracker);
                        }
                        break;
                    default:
                        throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported video event: " + element.LocalName), element);
                }
            }
        }

        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("EP1 VideoEventList native allocation failed.");
        return chunk;
    }
}
