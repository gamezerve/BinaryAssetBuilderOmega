using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original MoneyCrateCollideModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MoneyCrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("MoneyProvided","0"),&target->MoneyProvided,state);
        Marshal(node,(Ep1CrateCollideModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original UnitCrateCollideModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UnitCrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("UnitCount","0"),&target->UnitCount,state);
        Marshal(node.GetAttributeValue("UnitType",null),&target->UnitType,state);
        Marshal(node,(Ep1CrateCollideModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original VeterancyCrateCollideModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1VeterancyCrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("RangeOfEffect","0"),&target->RangeOfEffect,state);
        Marshal(node.GetAttributeValue("AffectsUpToLevel","0"),&target->AffectsUpToLevel,state);
        Marshal(node.GetAttributeValue("AddsOwnerVeterancy","false"),&target->AddsOwnerVeterancy,state);
        Marshal(node.GetAttributeValue("IsPilot","false"),&target->IsPilot,state);
        Marshal(node,(Ep1CrateCollideModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original SalvageCrateCollideModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SalvageCrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("PorterChance","0"),&target->PorterChance,state);
        Marshal(node.GetAttributeValue("BannerChance","0"),&target->BannerChance,state);
        Marshal(node.GetAttributeValue("LevelUpChance","0"),&target->LevelUpChance,state);
        Marshal(node.GetAttributeValue("LevelUpRadius","0"),&target->LevelUpRadius,state);
        Marshal(node.GetAttributeValue("ResourceChance","0"),&target->ResourceChance,state);
        Marshal(node.GetAttributeValue("MinimumResource","0"),&target->MinimumResource,state);
        Marshal(node.GetAttributeValue("MaximumResource","0"),&target->MaximumResource,state);
        Marshal(node.GetAttributeValue("Upgrade",null),&target->Upgrade,state);
        Marshal(node.GetAttributeValue("AllowAIPickup","false"),&target->AllowAIPickup,state);
        Marshal(node,(Ep1CrateCollideModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original RandomCrateCollideModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1RandomCrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("AttributeCrateChance","0"),&target->AttributeCrateChance,state);
        Marshal(node.GetAttributeValue("UnitCrateChance","0"),&target->UnitCrateChance,state);
        Marshal(node.GetAttributeValue("BombCrateChance","0"),&target->BombCrateChance,state);
        Marshal(node.GetAttributeValue("HealCrateChance","0"),&target->HealCrateChance,state);
        Marshal(node.GetAttributeValue("MoneyCrateChance","0"),&target->MoneyCrateChance,state);
        Marshal(node.GetAttributeValue("ShroudCrateChance","0"),&target->ShroudCrateChance,state);
        Marshal(node.GetAttributeValue("VeterancyCrateChance","0"),&target->VeterancyCrateChance,state);
        Marshal(node.GetAttributeValue("AllowAIPickup","false"),&target->AllowAIPickup,state);
        Marshal(node,(Ep1CrateCollideModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original SpawnCrateUpdateModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpawnCrateUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("MaxCrates","20"),&target->MaxCrates,state);
        Marshal(node.GetAttributeValue("CreateFrequency","30s"),&target->CreateFrequency,state);
        Marshal(node.GetAttributeValue("CrateLifetime","120s"),&target->CrateLifetime,state);
        Marshal(node.GetAttributeValue("MapExclusionMargin","50.0"),&target->MapExclusionMargin,state);
        Marshal(node.GetAttributeValue("BlockingUnitRange","30.0"),&target->BlockingUnitRange,state);
        Marshal(node.GetAttributeValue("BlockingStartPositionRange","300.0"),&target->BlockingStartPositionRange,state);
        Marshal(node.GetAttributeValue("Flags","DELETE_EXPIRED_CRATES"),&target->Flags,state);
        Marshal(node.GetChildNodes("CrateList"),&target->CrateList,state);
        Marshal(node.GetChildNode("BlockingUnits",null),&target->BlockingUnits,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original EnergyDisablingBehaviorModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EnergyDisablingBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(BehaviorModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original UnitVisionBehaviorModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UnitVisionBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("UpdateRate","1"),&target->UpdateRate,state);
        Marshal(node,(BehaviorModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original SqueezeFlingUpdateModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SqueezeFlingUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("ScanTime","0.2s"),&target->ScanTime,state);
        Marshal(node.GetAttributeValue("DelayBetweenVictimsTime","0.3s"),&target->DelayBetweenVictimsTime,state);
        Marshal(node.GetAttributeValue("EffectDistance","125.0"),&target->EffectDistance,state);
        Marshal(node.GetAttributeValue("GatherAcceleration","0.01"),&target->GatherAcceleration,state);
        Marshal(node.GetAttributeValue("ModelLength","100.0"),&target->ModelLength,state);
        Marshal(node.GetAttributeValue("ArmTemplate",null),&target->ArmTemplate,state);
        Marshal(node.GetAttributeValue("VictimShader",null),&target->VictimShader,state);
        Marshal(node.GetAttributeValue("HeightOffGround","50.0"),&target->HeightOffGround,state);
        Marshal(node.GetChildNode("Filter",null),&target->Filter,state);
        Marshal(node.GetChildNode("KillFilter",null),&target->KillFilter,state);
        Marshal(node.GetChildNodes("ParticleData"),&target->ParticleData,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original SqueezeFlingParticleDataType field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SqueezeFlingParticleDataType* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("SourceBone",null),&target->SourceBone,state);
        Marshal(node.GetAttributeValue("DestinationBone",null),&target->DestinationBone,state);
        Marshal(node.GetAttributeValue("ParticleSystem",null),&target->ParticleSystem,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original ReplaceDrawModuleBehaviorModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ReplaceDrawModuleBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("ConsumeItem","true"),&target->ConsumeItem,state);
        Marshal(node,(BehaviorModuleData*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every original GatherEjectUpdateModuleData field, default, list and tracked import. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1GatherEjectUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Lifetime","10s"),&target->Lifetime,state);
        Marshal(node.GetAttributeValue("Slots","5"),&target->Slots,state);
        Marshal(node.GetAttributeValue("ScanTime","0.2s"),&target->ScanTime,state);
        Marshal(node.GetAttributeValue("HeightOffGround","50.0"),&target->HeightOffGround,state);
        Marshal(node.GetAttributeValue("EffectDistance","125.0"),&target->EffectDistance,state);
        Marshal(node.GetAttributeValue("LiftOffGroundDistance","75.0"),&target->LiftOffGroundDistance,state);
        Marshal(node.GetAttributeValue("GatherAcceleration","0.01"),&target->GatherAcceleration,state);
        Marshal(node.GetChildNode("RotationSpeed",null),&target->RotationSpeed,state);
        Marshal(node.GetChildNode("UnitSpacing",null),&target->UnitSpacing,state);
        Marshal(node.GetChildNode("Filter",null),&target->Filter,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain complete capture inheritance and the original ally energy sharing ratio. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,DistributeEnergyToAlliesBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("ComradePercentage","100.0"),&target->ComradePercentage,state);
        Marshal(node,(CaptureBehaviorModuleData*)target,state);
    }
}
