using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: enum values follow the unchanged EA StyleName XSD order.
public enum StyleName { TitleFont, MinorTitleFont, NormalFont, ColumnFont }

// Reborn: original EP1 embeds the 12-byte scroll settings in the Credits root.
[StructLayout(LayoutKind.Sequential)]
public struct ScrollInfo { public uint Rate; public uint RateEveryFrame; public SageBool ScrollDown; }

// Reborn: each original 24-byte style embeds a packed Color3 and 16-byte FontDesc.
[StructLayout(LayoutKind.Sequential)]
public struct CreditStyle { public StyleName Name; public Color Color; public FontDesc Font; }

// Reborn: preserve ordered literal/localization text strings, including intentional empty credit lines.
[StructLayout(LayoutKind.Sequential)]
public struct CreditElement { public StyleName Style; public List<AnsiString> Text; }

// Reborn: original EP1 Credits uses a 32-byte untokenized root with source-sized style and item arrays.
[StructLayout(LayoutKind.Sequential)]
public struct Credits { public BaseAssetType Base; public ScrollInfo Scroll; public List<CreditStyle> Style; public List<CreditElement> CreditItem; }
