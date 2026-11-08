using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: original locale substitutions pair the requested size with a complete by-value font descriptor.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FontSubstitute
{
    public uint Size;
    public FontDesc Font;
}

// Reborn: original locale records retain their twenty-byte source-name/substitution-list header.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FontSubstitution
{
    public BaseInheritableAsset Base;
    public AnsiString Name;
    public List<Ep1FontSubstitute> Substitute;
}
