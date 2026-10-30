using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official DOTNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DOTNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("DamageInterval",null),&value->DamageInterval,state);
        Marshal(node.GetAttributeValue("DamageDuration",null),&value->DamageDuration,state);
        Marshal(node.GetAttributeValue("RemoveIfHealed","true"),&value->RemoveIfHealed,state);
        Marshal(node,(DamageNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official DamageAndSpawnNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DamageAndSpawnNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("SpawnProbability","1.0"),&value->SpawnProbability,state);
        Marshal(node.GetAttributeValue("SpawnedModelConditionFlags",null),&value->SpawnedModelConditionFlags,state);
        Marshal(node.GetChildNode("SpawnTemplate",null),&value->SpawnTemplate,state);
        Marshal(node,(DamageNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official TerrainCraterNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TerrainCraterNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Depth","0"),&value->Depth,state);
        Marshal(node.GetAttributeValue("Lift","0%"),&value->Lift,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official WeaponOCLNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1WeaponOCLNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("WeaponOCL",null),&value->WeaponOCL,state);
        Marshal(node.GetAttributeValue("TargetAsPrimaryObject","false"),&value->TargetAsPrimaryObject,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ActivateLaserNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateLaserNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Lifetime","1s"),&value->Lifetime,state);
        Marshal(node.GetAttributeValue("LaserId","0"),&value->LaserId,state);
        Marshal(node.GetAttributeValue("HitGroundFX",null),&value->HitGroundFX,state);
        Marshal(node.GetAttributeValue("OnlyIfInactive","false"),&value->OnlyIfInactive,state);
        Marshal(node.GetAttributeValue("OverShootDistance","0"),&value->OverShootDistance,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ActivateStreamNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateStreamNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Lifetime","1s"),&value->Lifetime,state);
        Marshal(node.GetAttributeValue("StreamId","0"),&value->StreamId,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ActivateLiftObjectNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateLiftObjectNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Lifetime","1s"),&value->Lifetime,state);
        Marshal(node.GetAttributeValue("LiftObjectLinkID","0"),&value->LiftObjectLinkID,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ActivateLinearDamageNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateLinearDamageNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Lifetime","1s"),&value->Lifetime,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ActivateCircularDamageNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ActivateCircularDamageNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Lifetime","1s"),&value->Lifetime,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official SlavesAttackNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SlavesAttackNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("SkipSlavesCanAttackCheck","false"),&value->SkipSlavesCanAttackCheck,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official LeechHealthDamageNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1LeechHealthDamageNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("PercentDamageToHeal","100%"),&value->PercentDamageToHeal,state);
        Marshal(node,(DamageNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official SecondaryDamageNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SecondaryDamageNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Amount",null),&value->Amount,state);
        Marshal(node.GetAttributeValue("DamageTaperOff","-1.0"),&value->DamageTaperOff,state);
        Marshal(node.GetAttributeValue("Arc","360d"),&value->Arc,state);
        Marshal(node.GetAttributeValue("ArcInverted","false"),&value->ArcInverted,state);
        Marshal(node.GetAttributeValue("PercentDamageToContained","0%"),&value->PercentDamageToContained,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official SpecialPowerNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpecialPowerNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("SpecialPowerTemplate",null),&value->SpecialPowerTemplate,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official AttributeModifierNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AttributeModifierNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("AttributeModifierName",null),&value->AttributeModifierName,state);
        Marshal(node.GetAttributeValue("AttributeModifierOwnerName",null),&value->AttributeModifierOwnerName,state);
        Marshal(node.GetAttributeValue("DamageFXType",null),&value->DamageFXType,state);
        Marshal(node.GetAttributeValue("DamageArc","360d"),&value->DamageArc,state);
        Marshal(node.GetAttributeValue("AntiCategories",null),&value->AntiCategories,state);
        Marshal(node.GetAttributeValue("AntiFX",null),&value->AntiFX,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official DamageContainedNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1DamageContainedNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("MaxUnitsToDamage","100"),&value->MaxUnitsToDamage,state);
        Marshal(node.GetAttributeValue("WindowBlastFX",null),&value->WindowBlastFX,state);
        Marshal(node.GetChildNode("DamageObjectFilter",null),&value->DamageObjectFilter,state);
        Marshal(node,(DamageNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official LuaEventNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1LuaEventNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("EventName",null),&value->EventName,state);
        Marshal(node.GetAttributeValue("SendToEnemies","false"),&value->SendToEnemies,state);
        Marshal(node.GetAttributeValue("SendToAllies","false"),&value->SendToAllies,state);
        Marshal(node.GetAttributeValue("SendToNeutral","false"),&value->SendToNeutral,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official LineDamageNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1LineDamageNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("OffsetAngle","0d"),&value->OffsetAngle,state);
        Marshal(node.GetAttributeValue("LineWidth","0"),&value->LineWidth,state);
        Marshal(node.GetAttributeValue("LineLengthLeadIn","0"),&value->LineLengthLeadIn,state);
        Marshal(node.GetAttributeValue("LineLengthLeadOut","0"),&value->LineLengthLeadOut,state);
        Marshal(node.GetAttributeValue("UseDynamicLineLength","false"),&value->UseDynamicLineLength,state);
        Marshal(node.GetAttributeValue("OverShootDistance","0"),&value->OverShootDistance,state);
        Marshal(node,(DamageNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official SeedTiberiumNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SeedTiberiumNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("FieldAmount","0"),&value->FieldAmount,state);
        Marshal(node.GetAttributeValue("SpawnedInFieldBonus","0"),&value->SpawnedInFieldBonus,state);
        Marshal(node,(Ep1WeaponOCLNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ParalyzeNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ParalyzeNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("EffectArc","360d"),&value->EffectArc,state);
        Marshal(node.GetAttributeValue("DurationSeconds","0s"),&value->DurationSeconds,state);
        Marshal(node.GetAttributeValue("ParalyzeType","NONE"),&value->ParalyzeType,state);
        Marshal(node.GetAttributeValue("RemoveParalyzeType","NONE"),&value->RemoveParalyzeType,state);
        Marshal(node.GetAttributeValue("ParalyzeFX",null),&value->ParalyzeFX,state);
        Marshal(node.GetAttributeValue("OnlyAffectOwnerWhenTriggered","false"),&value->OnlyAffectOwnerWhenTriggered,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official InformationWarfareNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1InformationWarfareNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("InfoWarType","NONE"),&value->InfoWarType,state);
        Marshal(node.GetAttributeValue("RadarJamRadius","0"),&value->RadarJamRadius,state);
        Marshal(node.GetAttributeValue("RadarJamDuration","0s"),&value->RadarJamDuration,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official SpendStolenTiberiumNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpendStolenTiberiumNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("AmountToSpend","1"),&value->AmountToSpend,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ReportWeaponFiredStatNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ReportWeaponFiredStatNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ScatterProjectileNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ScatterProjectileNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("ScatterMin","0"),&value->ScatterMin,state);
        Marshal(node.GetAttributeValue("ScatterMax","0"),&value->ScatterMax,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official FireOnObjectsNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FireOnObjectsNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Weapon",null),&value->Weapon,state);
        Marshal(node.GetAttributeValue("FireOnVictimObject",null),&value->FireOnVictimObject,state);
        Marshal(node.GetAttributeValue("VictimMustBeAlive","true"),&value->VictimMustBeAlive,state);
        Marshal(node.GetChildNode("Filter",null),&value->Filter,state);
        Marshal(node,(DamageNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official TintObjectsNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TintObjectsNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("PreColorTime","2s"),&value->PreColorTime,state);
        Marshal(node.GetAttributeValue("PostColorTime","2s"),&value->PostColorTime,state);
        Marshal(node.GetAttributeValue("SustainedColorTime","1s"),&value->SustainedColorTime,state);
        Marshal(node.GetAttributeValue("Frequency","1.0"),&value->Frequency,state);
        Marshal(node.GetAttributeValue("Amplitude","1.0"),&value->Amplitude,state);
        Marshal(node.GetChildNode("Color",null),&value->Color,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official ContainedObjectAttackNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ContainedObjectAttackNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official InformTargetNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1InformTargetNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("MinTimeToImpactFudgeFactor","0.85"),&value->MinTimeToImpactFudgeFactor,state);
        Marshal(node.GetAttributeValue("MaxTimeToImpactFudgeFactor","1.15"),&value->MaxTimeToImpactFudgeFactor,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official AttachNugget field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AttachNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("AttachModuleId",null),&value->AttachModuleId,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official StripMaxHealthPercentNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StripMaxHealthPercentNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("AmountToStrip","50%"),&value->AmountToStrip,state);
        Marshal(node.GetAttributeValue("DamageType","UNDEFINED"),&value->DamageType,state);
        Marshal(node.GetAttributeValue("DeathType","NORMAL"),&value->DeathType,state);
        Marshal(node.GetAttributeValue("DamageFXType","UNDEFINED"),&value->DamageFXType,state);
        Marshal(node,(WeaponEffectNugget*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official GrapplingHookNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1GrapplingHookNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(DamageNuggetType*)value,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official LeechPercentMaxHealthDamageNuggetType field and its schema default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1LeechPercentMaxHealthDamageNuggetType* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("PercentMaxHealthToTake","100%"),&value->PercentMaxHealthToTake,state);
        Marshal(node,(DamageNuggetType*)value,state);
    }
}
