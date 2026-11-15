using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, GiveOrRestoreUpgradeSpecialPowerModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(GiveOrRestoreUpgradeSpecialPowerModuleData.CommandButton), null), &objT->CommandButton, state);
        Marshal(node.GetAttributeValue(nameof(GiveOrRestoreUpgradeSpecialPowerModuleData.UpgradeToGive), null), &objT->UpgradeToGive, state);
        Marshal(node.GetAttributeValue(nameof(GiveOrRestoreUpgradeSpecialPowerModuleData.WeaponFlags), null), &objT->WeaponFlags, state);
        Marshal(node, (SpecialAbilityUpdateModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the EP1 command name and strong upgrade reference over the stock ability base. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1GiveOrRestoreUpgrade(Node node,BehaviorModuleData** destination,Tracker state)
    {
        using var context=state.Push((void**)destination,(uint)sizeof(Ep1GiveOrRestoreUpgradeSpecialPowerModuleData),1u);
        var result=(Ep1GiveOrRestoreUpgradeSpecialPowerModuleData*)*destination;
        Marshal(node,&result->Base,state);
        Marshal(node.GetAttributeValue("CommandButton",null),&result->CommandButton,state);
        Marshal(node.GetAttributeValue("UpgradeToGive",null),&result->UpgradeToGive,state);
        uint wire=Ep1GiveOrRestoreUpgradeSpecialPowerModuleData.Wire;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)result=wire;
    }
}
