using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write all DynamicsUpdate fields and their official optional defaults on the root Tracker. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,DynamicsUpdateModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Motion",null),&objT->Motion,state);
        Marshal(node.GetAttributeValue("Collision",null),&objT->Collision,state);
        Marshal(node.GetAttributeValue("IntuitiveCollision","COLLIDES_WITH_GROUND_ONLY"),&objT->IntuitiveCollision,state);
        Marshal(node.GetAttributeValue("ContactTag","NONE"),&objT->ContactTag,state);
        Marshal(node.GetAttributeValue("DrawPriority","REMOVABLE"),&objT->DrawPriority,state);
        Marshal(node.GetAttributeValue("FreezeDistanceFromCamera","450.0"),&objT->FreezeDistanceFromCamera,state);
        Marshal(node.GetChildNode("Volume",null),&objT->Volume,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: an absent volume is null; a present volume owns its full retail EP1 list/header layout. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1DynamicsVolumeType** objT,Tracker state)
    {if(node==null)return;using var context=state.Push((void**)objT,(uint)sizeof(Ep1DynamicsVolumeType),1u);Marshal(node,*objT,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse the existing shape writers while preserving the actual EP1 ReportContacts offset at seventy-two. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DynamicsVolumeType* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Mass","0"),&objT->Mass,state);
        Marshal(node.GetAttributeValue("Bounciness","30%"),&objT->Bounciness,state);
        Marshal(node.GetAttributeValue("Spinniness","2.0"),&objT->Spinniness,state);
        Marshal(node.GetAttributeValue("StaticFriction","0.6"),&objT->StaticFriction,state);
        Marshal(node.GetAttributeValue("KineticFriction","0.5"),&objT->KineticFriction,state);
        Marshal(node.GetAttributeValue("AngularDrag","0.1"),&objT->AngularDrag,state);
        Marshal(node.GetAttributeValue("LinearDrag","0.1"),&objT->LinearDrag,state);
        Marshal(node.GetAttributeValue("ContactTag","NONE"),&objT->ContactTag,state);
        Marshal(node.GetAttributeValue("ReportContacts","false"),&objT->ReportContacts,state);
        Marshal(node.GetChildNodes("Sphere"),&objT->Sphere,state);
        Marshal(node.GetChildNodes("Capsule"),&objT->Capsule,state);
        Marshal(node.GetChildNodes("Box"),&objT->Box,state);
        Marshal(node.GetChildNodes("Cylinder"),&objT->Cylinder,state);
        Marshal(node.GetChildNodes("Triangle"),&objT->Triangle,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: optional shape transforms retain their separate allocation and official lowercase quaternion components. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DynamicsShapeType* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetChildNode("Translation",null),&objT->Translation,state);Marshal(node.GetChildNode("Rotation",null),&objT->Rotation,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: a present quaternion owns four literal real components, including valid authored zero rotations. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1DynamicsQuaternion** objT,Tracker state)
    {
        if(node==null)return;using var context=state.Push((void**)objT,(uint)sizeof(Ep1DynamicsQuaternion),1u);
        Marshal(node.GetAttributeValue("x","0.0"),&(*objT)->x,state);Marshal(node.GetAttributeValue("y","0.0"),&(*objT)->y,state);Marshal(node.GetAttributeValue("z","0.0"),&(*objT)->z,state);Marshal(node.GetAttributeValue("w","0.0"),&(*objT)->w,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile an official sphere shape without reordering or omitting its inherited optional transforms. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DynamicsSphereShapeType* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Radius",null),&objT->Radius,state);
        Marshal(node,(Ep1DynamicsShapeType*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile an official capsule shape without reordering or omitting its inherited optional transforms. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DynamicsCapsuleShapeType* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Radius",null),&objT->Radius,state);
        Marshal(node.GetAttributeValue("HalfLength",null),&objT->HalfLength,state);
        Marshal(node,(Ep1DynamicsShapeType*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile an official box shape without reordering or omitting its inherited optional transforms. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DynamicsBoxShapeType* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("HalfSizeX",null),&objT->HalfSizeX,state);
        Marshal(node.GetAttributeValue("HalfSizeY",null),&objT->HalfSizeY,state);
        Marshal(node.GetAttributeValue("HalfSizeZ",null),&objT->HalfSizeZ,state);
        Marshal(node.GetAttributeValue("EdgeRadius","0"),&objT->EdgeRadius,state);
        Marshal(node,(Ep1DynamicsShapeType*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile an official cylinder shape without reordering or omitting its inherited optional transforms. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DynamicsCylinderShapeType* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Radius",null),&objT->Radius,state);
        Marshal(node.GetAttributeValue("HalfLength",null),&objT->HalfLength,state);
        Marshal(node.GetAttributeValue("EdgeRadius","0"),&objT->EdgeRadius,state);
        Marshal(node,(Ep1DynamicsShapeType*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile an official triangle shape without reordering or omitting its inherited optional transforms. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DynamicsTriangleShapeType* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("X1",null),&objT->X1,state);
        Marshal(node.GetAttributeValue("Y1",null),&objT->Y1,state);
        Marshal(node.GetAttributeValue("Z1",null),&objT->Z1,state);
        Marshal(node.GetAttributeValue("X2",null),&objT->X2,state);
        Marshal(node.GetAttributeValue("Y2",null),&objT->Y2,state);
        Marshal(node.GetAttributeValue("Z2",null),&objT->Z2,state);
        Marshal(node.GetAttributeValue("X3",null),&objT->X3,state);
        Marshal(node.GetAttributeValue("Y3",null),&objT->Y3,state);
        Marshal(node.GetAttributeValue("Z3",null),&objT->Z3,state);
        Marshal(node.GetAttributeValue("EdgeRadius","0"),&objT->EdgeRadius,state);
        Marshal(node,(Ep1DynamicsShapeType*)objT,state);
    }
}
