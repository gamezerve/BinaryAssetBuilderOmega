using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all six percentage bonuses and inherited SpecialPower fields using official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, GiveProductionBonusSpecialPowerModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(GiveProductionBonusSpecialPowerModuleData.BuildCostBonus),"0"),&objT->BuildCostBonus,state);
        Marshal(node.GetAttributeValue(nameof(GiveProductionBonusSpecialPowerModuleData.BuildCostBonusAllies),"0"),&objT->BuildCostBonusAllies,state);
        Marshal(node.GetAttributeValue(nameof(GiveProductionBonusSpecialPowerModuleData.BuildTimeBonus),"0"),&objT->BuildTimeBonus,state);
        Marshal(node.GetAttributeValue(nameof(GiveProductionBonusSpecialPowerModuleData.BuildTimeBonusAllies),"0"),&objT->BuildTimeBonusAllies,state);
        Marshal(node.GetAttributeValue(nameof(GiveProductionBonusSpecialPowerModuleData.HarvesterBonus),"0"),&objT->HarvesterBonus,state);
        Marshal(node.GetAttributeValue(nameof(GiveProductionBonusSpecialPowerModuleData.HarvesterBonusAllies),"0"),&objT->HarvesterBonusAllies,state);
        Marshal(node,(SpecialPowerModuleData*)objT,state);
    }
}
