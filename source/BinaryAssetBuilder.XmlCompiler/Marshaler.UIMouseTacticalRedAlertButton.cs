using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the shared fixed-button root with its required embedded mouse-over help. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIMouseSimpleFixedButton* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(UIMouseSimpleFixedButton.MouseOverHelp), null), &objT->MouseOverHelp, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile EP1's fieldless Red Alert button through the verified fixed-button base. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIMouseTacticalRedAlertButton* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node, (UIMouseSimpleFixedButton*)objT, state);
    }
}
