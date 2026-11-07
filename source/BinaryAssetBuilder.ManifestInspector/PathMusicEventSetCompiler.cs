// Reborn: EP1 script music is resolved through a category-specific event set, not the global event catalog.
using System.Globalization;
using System.Xml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class PathMusicEventSetCompiler
{
    internal enum Category : uint { SHELL, SCRIPT }
    internal struct EventSet
    {
        public BaseAssetType Base;
        public Category Category;
        public Relo.List<AssetReference<BaseAssetType>> Event;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the retail category, ordered strong event imports and relocated vector. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlElement source)
    {
        if (sizeof(EventSet) != 16)
            throw new InvalidDataException("EP1 PathMusicEventSet requires the x86 layout.");
        EventSet* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(EventSet), false);
        root->Category = source.GetAttribute("Category") switch
        {
            "SHELL" => Category.SHELL,
            "SCRIPT" => Category.SCRIPT,
            _ => throw new InvalidDataException("Invalid PathMusicEventSet category.")
        };
        var events = source.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "Event").ToArray();
        root->Event.Count = (uint)events.Length;
        if (events.Length != 0)
        {
            using var items = tracker.Push((void**)&root->Event.Items, sizeof(uint), root->Event.Count);
            for (int index = 0; index < events.Length; index++)
            {
                string value = events[index].InnerText;
                int separator = value.LastIndexOf('\\');
                if (separator < 0 || !uint.TryParse(value.AsSpan(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out uint reference))
                    throw SdkXmlDiagnostics.At(new InvalidDataException("Unresolved PathMusicEventSet event: " + value), events[index]);
                tracker.AddReference((uint*)(root->Event.Items + index), reference);
            }
        }
        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("EP1 PathMusicEventSet native allocation failed.");
        return chunk;
    }
}
