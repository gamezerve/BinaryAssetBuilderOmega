using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ore-node field with its schema default and EVA/audio imports. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,OreNodeModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("MaximumGatheredValue","20000"),&value->MaximumGatheredValue,state);
        Marshal(node.GetAttributeValue("DeliveryAmount","1000"),&value->DeliveryAmount,state);
        Marshal(node.GetAttributeValue("DeliveryAmountWhenEmpty","0"),&value->DeliveryAmountWhenEmpty,state);
        Marshal(node.GetAttributeValue("CashMultiplier","100%"),&value->CashMultiplier,state);
        Marshal(node.GetAttributeValue("MoveToExtractTime","1s"),&value->MoveToExtractTime,state);
        Marshal(node.GetAttributeValue("ExtractTime","1s"),&value->ExtractTime,state);
        Marshal(node.GetAttributeValue("MoveToDeliverTime","1s"),&value->MoveToDeliverTime,state);
        Marshal(node.GetAttributeValue("DeliveryTime","1s"),&value->DeliveryTime,state);
        Marshal(node.GetAttributeValue("ExtractAnimation",null),&value->ExtractAnimation,state);
        Marshal(node.GetAttributeValue("DeliveryAnimation",null),&value->DeliveryAnimation,state);
        Marshal(node.GetAttributeValue("ReturnAnimation",null),&value->ReturnAnimation,state);
        Marshal(node.GetAttributeValue("MoveBackAnimation",null),&value->MoveBackAnimation,state);
        Marshal(node.GetAttributeValue("LowNotificationPercentage",null),&value->LowNotificationPercentage,state);
        Marshal(node.GetAttributeValue("LowNotificationEvaEventRef",null),&value->LowNotificationEvaEventRef,state);
        Marshal(node.GetAttributeValue("DepletedNotificationEvaEventRef",null),&value->DepletedNotificationEvaEventRef,state);
        Marshal(node.GetAttributeValue("DepletedNotificationAudioEventInfoRef",null),&value->DepletedNotificationAudioEventInfoRef,state);
        Marshal(node,(BehaviorModuleData*)value,state);
    }
}
