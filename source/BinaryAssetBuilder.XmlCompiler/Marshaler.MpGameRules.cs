using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the witnessed EP1 24-byte layout and unchanged official five-minute transfer default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1MpGameRules* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1MpGameRules.NumMinutesBeforePlayersCanTransferMoney),"5"),&objT->NumMinutesBeforePlayersCanTransferMoney,state);
        Marshal(node.GetChildNode(nameof(Ep1MpGameRules.SkirmishStartCash),null),&objT->SkirmishStartCash,state);
        Marshal(node,(BaseAssetType*)objT,state);
    }
    public static unsafe void Marshal(Node node, SkirmishStartCash* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SkirmishStartCash.LoCash), null), &objT->LoCash, state);
        Marshal(node.GetAttributeValue(nameof(SkirmishStartCash.HiCash), null), &objT->HiCash, state);
        Marshal(node.GetAttributeValue(nameof(SkirmishStartCash.ChoiceStepAmount), null), &objT->ChoiceStepAmount, state);
        Marshal(node.GetAttributeValue(nameof(SkirmishStartCash.DefaultChoiceIndex), "1"), &objT->DefaultChoiceIndex, state);
    }

    public static unsafe void Marshal(Node node, MpGameRules* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(MpGameRules.SkirmishStartCash), null), &objT->SkirmishStartCash, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }
}
