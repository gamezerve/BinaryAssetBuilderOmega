using System.IO;
using System.Xml;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the EP1 tutorial lesson list and its native string relocations. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe Chunk CompileEp1TutorialMapData(XmlNode source)
    {
        var ns = new XmlNamespaceManager(source.OwnerDocument!.NameTable);
        ns.AddNamespace("ea", source.NamespaceURI);
        var lessons = new Node(source.CreateNavigator()!, ns).GetChildNodes("Lesson");
        int count = lessons.GetCount();
        Ep1TutorialMapData* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(Ep1TutorialMapData), false);
        root->Lesson.Count = (uint)count;
        if (count > 0)
        {
            using Tracker.Context context = tracker.Push((void**)&root->Lesson.Items, (uint)sizeof(Ep1TutorialLesson), (uint)count);
            for (int i = 0; i < count; i++)
            {
                var lesson = lessons.GetNextNode();
                var target = &root->Lesson.Items[i];
                Marshal(lesson.GetAttributeValue("Map", null), &target->Map, tracker);
                Marshal(lesson.GetAttributeValue("Summary", null), &target->Summary, tracker);
                Marshal(lesson.GetAttributeValue("ImageName", null), &target->ImageName, tracker);
            }
        }

        var result = new Chunk();
        if (!tracker.MakeRelocatable(result))
            throw new InvalidDataException("EP1 tutorial native allocation failed.");
        return result;
    }
}
