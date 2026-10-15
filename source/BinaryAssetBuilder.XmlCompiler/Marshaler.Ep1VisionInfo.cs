using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the stock-corroborated ten-field EP1 vision record with official defaults and percentage conversion. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1VisionInfo* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("VisionRange","0.00"),&objT->VisionRange,state);
        Marshal(node.GetAttributeValue("ShroudClearingRange","0.0"),&objT->ShroudClearingRange,state);
        Marshal(node.GetAttributeValue("ShroudClearingHeight","50.0"),&objT->ShroudClearingHeight,state);
        Marshal(node.GetAttributeValue("VisionSide","100"),&objT->VisionSide,state);
        Marshal(node.GetAttributeValue("VisionRear","100"),&objT->VisionRear,state);
        Marshal(node.GetAttributeValue("VisionBonusTestRadius","100"),&objT->VisionBonusTestRadius,state);
        Marshal(node.GetAttributeValue("VisionBonusTestSegments","8"),&objT->VisionBonusTestSegments,state);
        Marshal(node.GetAttributeValue("VisionBonusPercentPerFoot","0"),&objT->VisionBonusPercentPerFoot,state);
        Marshal(node.GetAttributeValue("MaxVisionBonusPercent","50"),&objT->MaxVisionBonusPercent,state);
        Marshal(node.GetAttributeValue("MinVisionBonusPercent","-50"),&objT->MinVisionBonusPercent,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate one optional EP1 vision owner on the root Tracker, without changing legacy VisionInfo overloads. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1VisionInfo** objT,Tracker state)
    {
        if(node is null)return;
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1VisionInfo),1u))Marshal(node,*objT,state);
    }
}
