using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: Restore the shared 36-byte fixed-button root used by Uprising tactical mouse assets.
    [StructLayout(LayoutKind.Sequential)]
    public struct UIMouseSimpleFixedButton
    {
        public BaseAssetType Base;
        public InGameUISimpleHelpTemplate MouseOverHelp;
    }

    // Reborn: Represent EP1's fieldless Red Alert button without changing its fixed-button ABI.
    [StructLayout(LayoutKind.Sequential)]
    public struct UIMouseTacticalRedAlertButton
    {
        public UIMouseSimpleFixedButton Base;
    }
}
