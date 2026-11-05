using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 stores three ANSI strings inline per tutorial lesson.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1TutorialLesson
{
    public AnsiString Map;
    public AnsiString Summary;
    public AnsiString ImageName;
}

// Reborn: EP1 stores the tutorial lessons in source order.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1TutorialMapData
{
    public BaseAssetType Base;
    public List<Ep1TutorialLesson> Lesson;
}
