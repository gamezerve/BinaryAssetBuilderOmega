using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: an EP1 upgrade OCL item has no Science field and owns one ordinary Core strong-reference occurrence. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1OCLUpgradePair* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1OCLUpgradePair.OCL),null),&objT->OCL,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every EP1 OCL power field and source-sized upgrade/filter graph without retaining the obsolete KW ABI. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1OCLSpecialPowerModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1OCLSpecialPowerModuleData.OCL),null),&objT->OCL,state);
        Marshal(node.GetAttributeValue(nameof(Ep1OCLSpecialPowerModuleData.CreateLocation),"CREATE_AT_EDGE_NEAR_SOURCE"),&objT->CreateLocation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1OCLSpecialPowerModuleData.CreateLocationOffset),"0"),&objT->CreateLocationOffset,state);
        Marshal(node.GetAttributeValue(nameof(Ep1OCLSpecialPowerModuleData.MaxCreateCount),"0"),&objT->MaxCreateCount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1OCLSpecialPowerModuleData.NumberToSpawn),"1"),&objT->NumberToSpawn,state);
        Marshal(node.GetAttributeValue(nameof(Ep1OCLSpecialPowerModuleData.DestinationOCL),null),&objT->DestinationOCL,state);
        Marshal(node.GetAttributeValue(nameof(Ep1OCLSpecialPowerModuleData.RegisterObjectsWithSpecialAbilityUpdate),"false"),&objT->RegisterObjectsWithSpecialAbilityUpdate,state);
        Marshal(node.GetChildNodes(nameof(Ep1OCLSpecialPowerModuleData.UpgradeOCL)),&objT->UpgradeOCL,state);
        Marshal(node.GetChildNode(nameof(Ep1OCLSpecialPowerModuleData.NearestSecondaryObjectFilter),null),&objT->NearestSecondaryObjectFilter,state);
        Marshal(node.GetChildNodes(nameof(Ep1OCLSpecialPowerModuleData.Upgrade)),&objT->Upgrade,state);
        Marshal(node,(SpecialPowerModuleData*)objT,state);
    }
}
