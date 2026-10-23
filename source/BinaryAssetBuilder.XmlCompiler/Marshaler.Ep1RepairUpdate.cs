using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 repair defaults (HealAmountPerSecond=10) with the shared nested percent/audio record marshalers. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1RepairUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1RepairUpdateModuleData.Toggleable),"true"),&objT->Toggleable,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RepairUpdateModuleData.RepairableWhenDead),"false"),&objT->RepairableWhenDead,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RepairUpdateModuleData.RepopThreshold),"0.2"),&objT->RepopThreshold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RepairUpdateModuleData.HealAmountPerSecond),"10"),&objT->HealAmountPerSecond,state);
        Marshal(node.GetChildNode(nameof(Ep1RepairUpdateModuleData.PercentOfBuildCostToRebuildInfo),null),&objT->PercentOfBuildCostToRebuildInfo,state);
        Marshal(node.GetChildNode(nameof(Ep1RepairUpdateModuleData.SelfAudio),null),&objT->SelfAudio,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
