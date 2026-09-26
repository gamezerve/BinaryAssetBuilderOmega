using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the EP1 Yuriko campaign hot-key map with the shared native HotKeyMap layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, YurikoHotKeys* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(YurikoHotKeys.Map), null), &objT->Map, state);
        Marshal(node, (BaseInheritableAsset*)objT, state);
    }
}
