using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Allocate and compile an optional dynamics Quaternion. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Quaternion** objT, Tracker state)
    {
        if (node is null) return;
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(Quaternion), 1u);
        Marshal(node, *objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the shared translation and rotation pointers for a dynamics shape. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsShapeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetChildNode(nameof(DynamicsShapeType.Translation), null), &objT->Translation, state);
        Marshal(node.GetChildNode(nameof(DynamicsShapeType.Rotation), null), &objT->Rotation, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an official dynamics sphere shape. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsSphereShapeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsSphereShapeType.Radius), null), &objT->Radius, state);
        Marshal(node, (DynamicsShapeType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an official dynamics capsule shape. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsCapsuleShapeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsCapsuleShapeType.Radius), null), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsCapsuleShapeType.HalfLength), null), &objT->HalfLength, state);
        Marshal(node, (DynamicsShapeType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an official dynamics box shape. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsBoxShapeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsBoxShapeType.HalfSizeX), null), &objT->HalfSizeX, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsBoxShapeType.HalfSizeY), null), &objT->HalfSizeY, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsBoxShapeType.HalfSizeZ), null), &objT->HalfSizeZ, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsBoxShapeType.EdgeRadius), "0"), &objT->EdgeRadius, state);
        Marshal(node, (DynamicsShapeType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an official dynamics cylinder shape. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsCylinderShapeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsCylinderShapeType.Radius), null), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsCylinderShapeType.HalfLength), null), &objT->HalfLength, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsCylinderShapeType.EdgeRadius), "0"), &objT->EdgeRadius, state);
        Marshal(node, (DynamicsShapeType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an official dynamics triangle shape. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsTriangleShapeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.X1), null), &objT->X1, state); Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.Y1), null), &objT->Y1, state); Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.Z1), null), &objT->Z1, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.X2), null), &objT->X2, state); Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.Y2), null), &objT->Y2, state); Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.Z2), null), &objT->Z2, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.X3), null), &objT->X3, state); Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.Y3), null), &objT->Y3, state); Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.Z3), null), &objT->Z3, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsTriangleShapeType.EdgeRadius), "0"), &objT->EdgeRadius, state);
        Marshal(node, (DynamicsShapeType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the official 76-byte dynamics volume and its five shape lists. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsVolumeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.Mass), "0"), &objT->Mass, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.Bounciness), "30%"), &objT->Bounciness, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.Spinniness), "2.0"), &objT->Spinniness, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.StaticFriction), "0.6"), &objT->StaticFriction, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.KineticFriction), "0.5"), &objT->KineticFriction, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.AngularDrag), "0.1"), &objT->AngularDrag, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.LinearDrag), "0.1"), &objT->LinearDrag, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.ContactTag), "NONE"), &objT->ContactTag, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsVolumeType.ReportContacts), "false"), &objT->ReportContacts, state);
        Marshal(node.GetChildNodes(nameof(DynamicsVolumeType.Sphere)), &objT->Sphere, state);
        Marshal(node.GetChildNodes(nameof(DynamicsVolumeType.Capsule)), &objT->Capsule, state);
        Marshal(node.GetChildNodes(nameof(DynamicsVolumeType.Box)), &objT->Box, state);
        Marshal(node.GetChildNodes(nameof(DynamicsVolumeType.Cylinder)), &objT->Cylinder, state);
        Marshal(node.GetChildNodes(nameof(DynamicsVolumeType.Triangle)), &objT->Triangle, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an official 84-byte named bone volume. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsBoneVolumeType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsBoneVolumeType.BoneName), null), &objT->BoneName, state);
        Marshal(node, (DynamicsVolumeType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the bone-volume list root. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsBoneVolumeSetType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetChildNodes(nameof(DynamicsBoneVolumeSetType.BoneVolume)), &objT->BoneVolume, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an official dynamics lifetime pair. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsLifetime* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(DynamicsLifetime.Delay), "0s"), &objT->Delay, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsLifetime.FadeTime), null), &objT->FadeTime, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the verified 256-byte EP1 dynamics draw root and appended joint pointer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, W3DDynamicsDrawModuleData* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(W3DDynamicsDrawModuleData.Collision), null), &objT->Collision, state);
        Marshal(node.GetAttributeValue(nameof(W3DDynamicsDrawModuleData.IntuitiveCollision), "COLLIDES_WITH_GROUND_ONLY"), &objT->IntuitiveCollision, state);
        Marshal(node.GetAttributeValue(nameof(W3DDynamicsDrawModuleData.DrawPriority), "MUST_BE_DRAWN"), &objT->DrawPriority, state);
        Marshal(node.GetAttributeValue(nameof(W3DDynamicsDrawModuleData.LODPriority), "VERY_LOW"), &objT->LODPriority, state);
        Marshal(node.GetAttributeValue(nameof(W3DDynamicsDrawModuleData.Explodiness), "0"), &objT->Explodiness, state);
        Marshal(node.GetAttributeValue(nameof(W3DDynamicsDrawModuleData.FlingPerturbation), "0"), &objT->FlingPerturbation, state);
        MarshalOptional(node.GetChildNode(nameof(W3DDynamicsDrawModuleData.BoneVolumes), null), &objT->BoneVolumes, state);
        MarshalOptional(node.GetChildNode(nameof(W3DDynamicsDrawModuleData.Lifetime), null), &objT->Lifetime, state);
        MarshalOptional(node.GetChildNode(nameof(W3DDynamicsDrawModuleData.Joints), null), &objT->Joints, state);
        Marshal(node.GetAttributeValue(nameof(W3DDynamicsDrawModuleData.InitiallyActive), "true"), &objT->InitiallyActive, state);
        Marshal(node, (W3DScriptedModelDrawModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Allocate one optional unmanaged dynamics child and invoke its typed marshaller. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalOptional<T>(Node node, T** objT, Tracker state) where T : unmanaged
    {
        if (node is null) return;
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(T), 1u);
        if (typeof(T) == typeof(DynamicsBoneVolumeSetType)) Marshal(node, (DynamicsBoneVolumeSetType*)*objT, state);
        else if (typeof(T) == typeof(DynamicsLifetime)) Marshal(node, (DynamicsLifetime*)*objT, state);
        else if (typeof(T) == typeof(DynamicsJointSetType)) Marshal(node, (DynamicsJointSetType*)*objT, state);
    }
}
