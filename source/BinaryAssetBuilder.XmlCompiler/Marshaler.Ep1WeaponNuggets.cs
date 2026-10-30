using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate one EP1 concrete nugget record and keep the official class identity (not the CLR model name) in its TypeId word. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1Nugget<T>(Node node,WeaponEffectNugget** objT,Tracker state,uint typeId) where T:unmanaged
    {
        using(var context=state.Push((void**)objT,(uint)sizeof(T),1u)){
            var nugget=(T*)*objT;
            if(typeof(T)==typeof(Ep1ProjectileNuggetType))Marshal(node,(Ep1ProjectileNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1SuppressionNuggetType))Marshal(node,(Ep1SuppressionNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1MetaImpactNuggetType))Marshal(node,(Ep1MetaImpactNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1DOTNuggetType))Marshal(node,(Ep1DOTNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1DamageAndSpawnNuggetType))Marshal(node,(Ep1DamageAndSpawnNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1TerrainCraterNuggetType))Marshal(node,(Ep1TerrainCraterNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1WeaponOCLNuggetType))Marshal(node,(Ep1WeaponOCLNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ActivateLaserNuggetType))Marshal(node,(Ep1ActivateLaserNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ActivateStreamNuggetType))Marshal(node,(Ep1ActivateStreamNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ActivateLiftObjectNuggetType))Marshal(node,(Ep1ActivateLiftObjectNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ActivateLinearDamageNuggetType))Marshal(node,(Ep1ActivateLinearDamageNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ActivateCircularDamageNuggetType))Marshal(node,(Ep1ActivateCircularDamageNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1SlavesAttackNuggetType))Marshal(node,(Ep1SlavesAttackNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1LeechHealthDamageNuggetType))Marshal(node,(Ep1LeechHealthDamageNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1SecondaryDamageNuggetType))Marshal(node,(Ep1SecondaryDamageNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1SpecialPowerNuggetType))Marshal(node,(Ep1SpecialPowerNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1AttributeModifierNuggetType))Marshal(node,(Ep1AttributeModifierNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1DamageContainedNuggetType))Marshal(node,(Ep1DamageContainedNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1LuaEventNuggetType))Marshal(node,(Ep1LuaEventNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1LineDamageNuggetType))Marshal(node,(Ep1LineDamageNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1SeedTiberiumNuggetType))Marshal(node,(Ep1SeedTiberiumNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ParalyzeNuggetType))Marshal(node,(Ep1ParalyzeNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1InformationWarfareNuggetType))Marshal(node,(Ep1InformationWarfareNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1SpendStolenTiberiumNuggetType))Marshal(node,(Ep1SpendStolenTiberiumNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ReportWeaponFiredStatNuggetType))Marshal(node,(Ep1ReportWeaponFiredStatNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ScatterProjectileNuggetType))Marshal(node,(Ep1ScatterProjectileNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1FireOnObjectsNuggetType))Marshal(node,(Ep1FireOnObjectsNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1TintObjectsNuggetType))Marshal(node,(Ep1TintObjectsNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1ContainedObjectAttackNuggetType))Marshal(node,(Ep1ContainedObjectAttackNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1InformTargetNuggetType))Marshal(node,(Ep1InformTargetNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1AttachNugget))Marshal(node,(Ep1AttachNugget*)nugget,state);
            else if(typeof(T)==typeof(Ep1StripMaxHealthPercentNuggetType))Marshal(node,(Ep1StripMaxHealthPercentNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1GrapplingHookNuggetType))Marshal(node,(Ep1GrapplingHookNuggetType*)nugget,state);
            else if(typeof(T)==typeof(Ep1LeechPercentMaxHealthDamageNuggetType))Marshal(node,(Ep1LeechPercentMaxHealthDamageNuggetType*)nugget,state);
            else throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 weapon nugget writer is not implemented: {0}",typeof(T).Name);
        }
        state.InplaceEndianToPlatform(&typeId);*(uint*)*objT=typeId;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 projectile nugget, including the optional barrel override pointer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ProjectileNuggetType* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("WarheadTemplate",null),&objT->WarheadTemplate,state);
        Marshal(node.GetAttributeValue("ProjectileTemplate",null),&objT->ProjectileTemplate,state);
        Marshal(node.GetAttributeValue("WeaponLaunchBoneSlotOverride",nameof(WeaponSlotType.NO_WEAPON)),&objT->WeaponLaunchBoneSlotOverride,state);
        Marshal(node.GetChildNode("AttackOffset",null),&objT->AttackOffset,state);
        Marshal(node.GetChildNodes("VeterancyProjectiles"),&objT->VeterancyProjectiles,state);
        Marshal(node.GetChildNode("SpecificBarrelOverride",null),&objT->SpecificBarrelOverride,state);
        Marshal(node,(WeaponEffectNugget*)objT,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 suppression nugget in official schema order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SuppressionNuggetType* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Suppression","0"),&objT->Suppression,state);
        Marshal(node.GetAttributeValue("DurationSeconds","0s"),&objT->DurationSeconds,state);
        Marshal(node.GetAttributeValue("SuppressionTaperOff","-1.0"),&objT->SuppressionTaperOff,state);
        Marshal(node.GetAttributeValue("SuppressionArc","360d"),&objT->SuppressionArc,state);
        Marshal(node.GetAttributeValue("SuppressionArcInverted","false"),&objT->SuppressionArcInverted,state);
        Marshal(node,(WeaponEffectNugget*)objT,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 meta-impact nugget without the KW-only HeroResist word. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MetaImpactNuggetType* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("ShockWaveAmount","0"),&objT->ShockWaveAmount,state);
        Marshal(node.GetAttributeValue("ShockWaveRadius","0"),&objT->ShockWaveRadius,state);
        Marshal(node.GetAttributeValue("ShockWaveArc","360d"),&objT->ShockWaveArc,state);
        Marshal(node.GetAttributeValue("ShockWaveArcInverted","false"),&objT->ShockWaveArcInverted,state);
        Marshal(node.GetAttributeValue("ShockWaveTaperOff","0"),&objT->ShockWaveTaperOff,state);
        Marshal(node.GetAttributeValue("ShockWaveSpeed","0"),&objT->ShockWaveSpeed,state);
        Marshal(node.GetAttributeValue("ShockWaveZMult","1"),&objT->ShockWaveZMult,state);
        Marshal(node.GetAttributeValue("CyclonicFactor","0"),&objT->CyclonicFactor,state);
        Marshal(node.GetAttributeValue("ShockwaveDelaySeconds","0s"),&objT->ShockwaveDelaySeconds,state);
        Marshal(node.GetAttributeValue("Suppression","0"),&objT->Suppression,state);
        Marshal(node.GetAttributeValue("InvertShockWave","false"),&objT->InvertShockWave,state);
        Marshal(node.GetAttributeValue("FlipDirection","false"),&objT->FlipDirection,state);
        Marshal(node.GetAttributeValue("OnlyWhenJustDied","false"),&objT->OnlyWhenJustDied,state);
        Marshal(node.GetAttributeValue("ShockWaveClearRadius","false"),&objT->ShockWaveClearRadius,state);
        Marshal(node.GetAttributeValue("ShockWaveClearWaveMult","2"),&objT->ShockWaveClearWaveMult,state);
        Marshal(node.GetAttributeValue("ShockWaveClearFlingHeight","100"),&objT->ShockWaveClearFlingHeight,state);
        Marshal(node.GetChildNode("KillObjectFilter",null),&objT->KillObjectFilter,state);
        Marshal(node,(WeaponEffectNugget*)objT,state);
    }
}
