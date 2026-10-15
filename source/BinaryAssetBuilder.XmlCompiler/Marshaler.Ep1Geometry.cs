using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: selected object allocation appends geometry after Body on the same Tracker rather than preallocating an orphan legacy graph. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1Geometry(Node node,Ep1Geometry** objT,Tracker state)
    {
        Marshal(node,objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: apply official Shape defaults with the EP1 enum while preserving native field/allocation order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1Shape* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Type","SPHERE"),&objT->Type,state);
        Marshal(node.GetAttributeValue("ContactPointGeneration","NONE"),&objT->ContactPointGeneration,state);
        Marshal(node.GetAttributeValue("Name",null),&objT->Name,state);
        Marshal(node.GetAttributeValue("Active","true"),&objT->Active,state);
        Marshal(node.GetAttributeValue("UsedForHealthBox","true"),&objT->UsedForHealthBox,state);
        Marshal(node.GetAttributeValue("MajorRadius","1.0"),&objT->MajorRadius,state);
        Marshal(node.GetAttributeValue("MinorRadius","1.0"),&objT->MinorRadius,state);
        Marshal(node.GetAttributeValue("Other",null),&objT->Other,state);
        Marshal(node.GetAttributeValue("Height","1.0"),&objT->Height,state);
        Marshal(node.GetAttributeValue("FrontAngle",null),&objT->FrontAngle,state);
        Marshal(node.GetChildNode("Offset",null),&objT->Offset,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: an optional placement shape owns one full EP1 record, not an inline extra collision-list element. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1Shape** objT,Tracker state)
    {
        if(node is null)return;
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1Shape),1u))Marshal(node,*objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the 28-byte EP1 geometry header and its reviewed rotation/placement/collision/contact graphs without legacy fallback. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1Geometry* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("IsSmall","false"),&objT->IsSmall,state);
        Marshal(node.GetChildNode("RotationAnchorOffset",null),&objT->RotationAnchorOffset,state);
        Marshal(node.GetChildNode("PlacementShape",null),&objT->PlacementShape,state);
        Marshal(node.GetChildNodes("Shape"),&objT->Shape,state);
        Marshal(node.GetChildNodes("ContactPoint"),&objT->ContactPoint,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: GameObject's geometry pointer selects the separate EP1 header while the legacy Geometry model remains available to older consumers. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1Geometry** objT,Tracker state)
    {
        if(node is null)return;
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1Geometry),1u))Marshal(node,*objT,state);
    }
}
