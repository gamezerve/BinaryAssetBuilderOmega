using System.Runtime.InteropServices;
namespace SageBinaryData;
// Reborn: Uprising button-state assets embed the existing image import and three string descriptors after the base asset word.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ButtonSingleStateData { public BaseAssetType Base; public ButtonState State; }
