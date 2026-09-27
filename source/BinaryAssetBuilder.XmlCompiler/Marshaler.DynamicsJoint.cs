using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Allocate and compile an optional EP1 joint Vector3. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Vector3** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(Vector3), 1u);
        Marshal(node, *objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile a 12-byte EP1 joint link and its optional position. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsJointLinkType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLinkType.BoneName), null), &objT->BoneName, state);
        Marshal(node.GetChildNode(nameof(DynamicsJointLinkType.Position), null), &objT->Position, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the EP1 joint frame in native child-then-parent order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsJointFrameType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(DynamicsJointFrameType.Child), null), &objT->Child, state);
        Marshal(node.GetChildNode(nameof(DynamicsJointFrameType.Parent), null), &objT->Parent, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the verified EP1 swing, twist, inertia, and optional limit position fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsJointLimitsType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLimitsType.SwingType), "SWING_LOCKED"), &objT->SwingType, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLimitsType.SwingDisplacementLimit), "0"), &objT->SwingDisplacementLimit, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLimitsType.SwingAngleLimit), "0"), &objT->SwingAngleLimit, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLimitsType.TwistType), "TWIST_LOCKED"), &objT->TwistType, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLimitsType.TwistDisplacementLimit), "0"), &objT->TwistDisplacementLimit, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLimitsType.TwistAngleLimit), "0"), &objT->TwistAngleLimit, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsJointLimitsType.InertiaOverride), "0.2"), &objT->InertiaOverride, state);
        Marshal(node.GetChildNode(nameof(DynamicsJointLimitsType.Position), null), &objT->Position, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile one 56-byte EP1 dynamics joint. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsJointType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(DynamicsJointType.Frame), null), &objT->Frame, state);
        Marshal(node.GetChildNode(nameof(DynamicsJointType.Limits), null), &objT->Limits, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the EP1 dynamics joint-set list. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsJointSetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNodes(nameof(DynamicsJointSetType.Joint)), &objT->Joint, state);
    }
}
