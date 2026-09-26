using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Marshal the three Vector4 controls stored inline in an Uprising projectile-path node. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ProjectilePathNode* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(ProjectilePathNode.InVec), null), &objT->InVec, state);
        Marshal(node.GetChildNode(nameof(ProjectilePathNode.Point), null), &objT->Point, state);
        Marshal(node.GetChildNode(nameof(ProjectilePathNode.OutVec), null), &objT->OutVec, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an Uprising ProjectilePath using its EP1 Bezier-node list at the legacy list offset. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ProjectilePath* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(ProjectilePath.ComponentScale), null), &objT->ComponentScale, state);
        Marshal(node.GetChildNodes(nameof(ProjectilePath.Node)), &objT->Node, state);
        Marshal(node, (BaseInheritableAsset*)objT, state);
    }
}
