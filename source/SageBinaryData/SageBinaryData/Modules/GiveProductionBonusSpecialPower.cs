using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Uprising production bonuses extend the complete SpecialPower record with six source-owned percentage values.
[StructLayout(LayoutKind.Sequential)]
public struct GiveProductionBonusSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    public Percentage BuildCostBonus;
    public Percentage BuildCostBonusAllies;
    public Percentage BuildTimeBonus;
    public Percentage BuildTimeBonusAllies;
    public Percentage HarvesterBonus;
    public Percentage HarvesterBonusAllies;
}
