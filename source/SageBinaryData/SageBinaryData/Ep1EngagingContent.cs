using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: loading-screen content stores three native strings, not texture/video asset handles.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EngagingContent
{
    public BaseAssetType Base;
    public AnsiString TitleId,DescId,ImageId;
}

// Reborn: groups retain the authored sequence of real EngagingContent imports in the original twelve-byte root.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EngagingContentGroup
{
    public BaseAssetType Base;
    public List<AssetReference<Ep1EngagingContent>> EngagingContent;
}

// Reborn: a map sequence has one required map string and two independently optional inline group handles.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EngagingContentSequence
{
    public BaseAssetType Base;
    public AnsiString MapName;
    public AssetReference<Ep1EngagingContentGroup> RequiredContent,RandomContent;
}
