using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize official EP1 tracker values, four optional filters and source-sized strong EmotionTemplate reference lists. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EmotionTrackerUpdateModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("TauntAndPointDistance","0.0"),&value->TauntAndPointDistance,state);
        Marshal(node.GetAttributeValue("HeroScanDistance","0.0"),&value->HeroScanDistance,state);
        Marshal(node.GetAttributeValue("TauntAndPointUpdateDelay","2s"),&value->TauntAndPointUpdateDelay,state);
        Marshal(node.GetAttributeValue("FearScanDistance","0.0"),&value->FearScanDistance,state);
        Marshal(node.GetAttributeValue("ImmuneToFearLevel","5"),&value->ImmuneToFearLevel,state);
        Marshal(node.GetChildNode("TauntAndPointExcluded",null),&value->TauntAndPointExcluded,state);
        Marshal(node.GetChildNode("AfraidOf",null),&value->AfraidOf,state);
        Marshal(node.GetChildNode("AlwaysAfraidOf",null),&value->AlwaysAfraidOf,state);
        Marshal(node.GetChildNode("PointAt",null),&value->PointAt,state);
        Marshal(node.GetChildNodes("AddEmotion"),&value->AddEmotion,state);
        Marshal(node.GetAttributeValue("IgnoreVeterancy","false"),&value->IgnoreVeterancy,state);
        Marshal(node,(UpdateModuleData*)value,state);
    }
}
