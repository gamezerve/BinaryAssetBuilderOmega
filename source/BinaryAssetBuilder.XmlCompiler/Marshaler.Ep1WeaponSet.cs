using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve independent required/forbidden masks and both EP1 flags without mutating the KW weapon-row ABI. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1WeaponSlotWeaponData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Ordering",null),&objT->Ordering,state);
        Marshal(node.GetAttributeValue("Template",null),&objT->Template,state);
        Marshal(node.GetAttributeValue("Upgrade",null),&objT->Upgrade,state);
        Marshal(node.GetAttributeValue("ObjectStatus",""),&objT->ObjectStatus,state);
        Marshal(node.GetAttributeValue("ForbiddenObjectStatus",""),&objT->ForbiddenObjectStatus,state);
        Marshal(node.GetAttributeValue("IsPlayerUpgradePermanent","false"),&objT->IsPlayerUpgradePermanent,state);
        Marshal(node.GetAttributeValue("ConquerMetricsAlwaysCount","false"),&objT->ConquerMetricsAlwaysCount,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the bounded inline hardpoint family with its stock zero TypeId word and require already-ordered weapon records. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1WeaponSlotHardpoint* objT,Tracker state)
    {
        if(node is null)return;
        // Reborn: Tracker sizes the actual authored list; the former sixteen-row diagnostic quota is not an engine/schema limit.
        var weapons=node.GetChildNodes("Weapon");
        Marshal(node.GetAttributeValue("ID",null),&objT->ID,state);
        Marshal(node.GetAttributeValue("InterleavedStyle","INTERLEAVE_RANDOM"),&objT->InterleavedStyle,state);
        Marshal(node.GetAttributeValue("WeaponChoiceCriteria","PREFER_MOST_DAMAGE"),&objT->WeaponChoiceCriteria,state);
        Marshal(node.GetAttributeValue("AllowInterleavedFiring","false"),&objT->AllowInterleavedFiring,state);
        Marshal(node.GetAttributeValue("AutoChooseWeapon","true"),&objT->AutoChooseWeapon,state);
        Marshal(weapons,&objT->Weapon,state);
        for(uint i=1;i<objT->Weapon.Count;i++)if(objT->Weapon.Items[i].Ordering<objT->Weapon.Items[i-1].Ordering)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 diagnostic hardpoint requires source-order normalization.");
        // Reborn: these direct list records are statically typed; both reviewed stock graphs leave their TypeId field zero, unlike the polymorphic module root.
        objT->TypeId=0;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: isolated EP1 root rejects unsupported turrets/contained slots rather than silently leaving them empty. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1WeaponSetUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        foreach(string name in new[]{"WeaponSlotTurret","WeaponSlotHierarchicalTurret","WeaponSlotContained"})if(node.GetChildNodes(name)?.GetCount()>0)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 diagnostic weapon slot {0} is not admitted.",name);
        // Reborn: source-sized hardpoint arrays reuse the implemented ABI without the former eight-hardpoint trial quota.
        var slots=node.GetChildNodes("WeaponSlotHardpoint");
        Marshal(node.GetAttributeValue("ActiveWhenDisabledBy","HELD"),&objT->ActiveWhenDisabledBy,state);
        Marshal(slots,&objT->WeaponSlotHardpoint,state);Marshal(node,(UpdateModuleData*)objT,state);
    }
}
