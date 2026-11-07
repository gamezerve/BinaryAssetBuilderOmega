using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the original EP1 duration, width ratio and inherited identity fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,LaserScatterUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(LaserScatterUpdateModuleData.LaserLifetime),"1s"),&target->LaserLifetime,state);
        Marshal(node.GetAttributeValue(nameof(LaserScatterUpdateModuleData.LaserHitGroundWidthMultiplier),"10%"),&target->LaserHitGroundWidthMultiplier,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal all source-sized scatter settings without dropping negative signed amounts or delays. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,ScatterDamage* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(ScatterDamage.ScatterAmount),"0"),&target->ScatterAmount,state);
        Marshal(node.GetAttributeValue(nameof(ScatterDamage.ScatterDelay),"0s"),&target->ScatterDelay,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain optional weapon linkage, required scatter/filter records and the final native direction flag. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,WeaponScatterEffectUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(WeaponScatterEffectUpdateModuleData.WeaponTemplate),null),&target->WeaponTemplate,state);
        Marshal(node.GetChildNode(nameof(WeaponScatterEffectUpdateModuleData.ScatterSettings),null),&target->ScatterSettings,state);
        Marshal(node.GetChildNode(nameof(WeaponScatterEffectUpdateModuleData.Filter),null),&target->Filter,state);
        Marshal(node.GetAttributeValue(nameof(WeaponScatterEffectUpdateModuleData.RandomScatter3D),"false"),&target->RandomScatter3D,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
}
