using System.Globalization;
using System.Xml;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 stores nullable event references and an ordered transition vector in the untokenized state.
internal static class PathMusicGameDynamicStateCompiler
{
    private unsafe struct State
    {
        internal uint Base;
        internal uint* EnterEvent;
        internal uint* ExitEvent;
        internal uint TransitionCount;
        internal uint* Transitions;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the EP1 state through the same relocation and import tracker used by the other asset processors. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlElement source)
    {
        if (sizeof(State) != 20)
            throw new InvalidDataException("EP1 PathMusicGameDynamicState requires the x86 layout.");

        State* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(State), false);
        WriteEvent(source.GetAttribute("EnterEvent"), &root->EnterEvent, tracker);
        WriteEvent(source.GetAttribute("ExitEvent"), &root->ExitEvent, tracker);

        var transitions = source.ChildNodes.OfType<XmlElement>()
            .Where(element => element.LocalName == "Transition").ToArray();
        root->TransitionCount = (uint)transitions.Length;
        if (transitions.Length != 0)
        {
            using var context = tracker.Push((void**)&root->Transitions, sizeof(uint), root->TransitionCount);
            for (int index = 0; index < transitions.Length; index++)
                tracker.AddReference(root->Transitions + index, ReferenceIndex(transitions[index].InnerText));
        }

        var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk))
            throw new InvalidDataException("PathMusicGameDynamicState native allocation failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a populated event is an imported reference behind a relocated pointer. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void WriteEvent(string value, uint** destination, Tracker tracker)
    {
        if (value.Length == 0) return;
        using var context = tracker.Push((void**)destination, sizeof(uint), 1);
        tracker.AddReference(*destination, ReferenceIndex(value));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require a normalized Core reference index instead of emitting unresolved names. */
    //-------------------------------------------------------------------------------------------------
    private static uint ReferenceIndex(string value)
    {
        int separator = value.LastIndexOf('\\');
        if (separator >= 0 && uint.TryParse(value.AsSpan(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out uint index))
            return index;
        throw new InvalidDataException("Unresolved PathMusic state reference: " + value);
    }
}
