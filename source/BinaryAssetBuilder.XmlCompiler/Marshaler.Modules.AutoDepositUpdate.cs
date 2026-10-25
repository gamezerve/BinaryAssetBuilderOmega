using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, AutoDepositUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.DepositInterval), "0.0"), &objT->DepositInterval, state);
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.DepositAmount), "0"), &objT->DepositAmount, state);
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.InitialCaptureBonus), "0"), &objT->InitialCaptureBonus, state);
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.Upgrade), null), &objT->Upgrade, state);
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.UpgradeBonusScalar), "1.0"), &objT->UpgradeBonusScalar, state);
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.GiveNoXP), "false"), &objT->GiveNoXP, state);
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.OnlyWhenGarrisoned), "false"), &objT->OnlyWhenGarrisoned, state);
        Marshal(node.GetAttributeValue(nameof(AutoDepositUpdateModuleData.Flags), nameof(AutoDepositFlagsType.NONE)), &objT->Flags, state);
        Marshal(node.GetChildNode(nameof(AutoDepositUpdateModuleData.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile Uprising deposits with all three transaction lists and official defaults while retaining the legacy KW writer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1AutoDepositUpdateModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AutoDepositUpdateModuleData.DepositInterval), "0s"), &objT->DepositInterval, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AutoDepositUpdateModuleData.Flags), "NONE"), &objT->Flags, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AutoDepositUpdateModuleData.TransactionCategory), "AUTO_DEPOSIT"), &objT->TransactionCategory, state);
        Marshal(node.GetChildNodes(nameof(Ep1AutoDepositUpdateModuleData.InitialCaptureBonus)), &objT->InitialCaptureBonus, state);
        Marshal(node.GetChildNodes(nameof(Ep1AutoDepositUpdateModuleData.Deposit)), &objT->Deposit, state);
        Marshal(node.GetChildNodes(nameof(Ep1AutoDepositUpdateModuleData.ResourceCap)), &objT->ResourceCap, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AutoDepositUpdateModuleData.GiveNoXP), "false"), &objT->GiveNoXP, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AutoDepositUpdateModuleData.OnlyWhenGarrisoned), "false"), &objT->OnlyWhenGarrisoned, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
