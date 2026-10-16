using System.Globalization;
using System.IO;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly allocate reviewed EP1 infantry records and preserve original wire class IDs without changing legacy KW dispatch or hashing the isolated CLR names. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1InfantryBehavior(Node node,BehaviorModuleData** objT,Tracker state,uint type)
    {
        if(node==null || node.GetAttributeValue("TypeId",null)?.GetText()!=type.ToString(CultureInfo.InvariantCulture))throw new InvalidDataException("Explicit EP1 infantry wire ID required.");
        if(type==0xB5EB41ED){using var scope=state.Push((void**)objT,(uint)sizeof(Ep1SquishCollideModuleData),1u);Marshal(node,(Ep1SquishCollideModuleData*)*objT,state);}
        else if(type==0x79BFEDCB){using var scope=state.Push((void**)objT,(uint)sizeof(Ep1PhysicsBehaviorModuleData),1u);Marshal(node,(Ep1PhysicsBehaviorModuleData*)*objT,state);}
        else if(type==0x59D12990){using var scope=state.Push((void**)objT,(uint)sizeof(Ep1HitReactionBehaviorModuleData),1u);Marshal(node,(Ep1HitReactionBehaviorModuleData*)*objT,state);}
        else if(type==0x786C9458){using var scope=state.Push((void**)objT,(uint)sizeof(Ep1StatusBitsUpgradeModuleData),1u);Marshal(node,(Ep1StatusBitsUpgradeModuleData*)*objT,state);}
        else throw new InvalidDataException("Unreviewed EP1 infantry class.");state.InplaceEndianToPlatform(&type);*(uint*)*objT=type;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain both official crush flags and default one-second animation without reading unrelated KW-only fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SquishCollideModuleData* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("CrushAnimTime","1s"),&objT->CrushAnimTime,state);Marshal(node.GetAttributeValue("UseVehicleStyleCrush","false"),&objT->UseVehicleStyleCrush,state);Marshal(node.GetAttributeValue("AllowAllyCrush","false"),&objT->AllowAllyCrush,state);Marshal(node,(CollideModuleData*)objT,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve every official physics source field with float Time conversion and the independently schema/stock-checked EP1 defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PhysicsBehaviorModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("MaxXRotationVelocity","360.0"),&objT->MaxXRotationVelocity,state);Marshal(node.GetAttributeValue("MaxYRotationVelocity","360.0"),&objT->MaxYRotationVelocity,state);Marshal(node.GetAttributeValue("MaxZRotationVelocity","360.0"),&objT->MaxZRotationVelocity,state);Marshal(node.GetAttributeValue("GravityMultiplier","1.0"),&objT->GravityMultiplier,state);
        Marshal(node.GetAttributeValue("ShockStunnedTimeLow","1s"),&objT->ShockStunnedTimeLow,state);Marshal(node.GetAttributeValue("ShockStunnedTimeHigh","1s"),&objT->ShockStunnedTimeHigh,state);Marshal(node.GetAttributeValue("ShockStandingTime","1.5s"),&objT->ShockStandingTime,state);
        Marshal(node.GetAttributeValue("FirstHeight","1.3"),&objT->FirstHeight,state);Marshal(node.GetAttributeValue("SecondHeight","1.3"),&objT->SecondHeight,state);Marshal(node.GetAttributeValue("FirstPercentIndent","33"),&objT->FirstPercentIndent,state);Marshal(node.GetAttributeValue("SecondPercentIndent","66"),&objT->SecondPercentIndent,state);Marshal(node.GetAttributeValue("BounceCount","2"),&objT->BounceCount,state);Marshal(node.GetAttributeValue("BounceDistance","5.0"),&objT->BounceDistance,state);Marshal(node.GetAttributeValue("BounceFirstHeight","1.3"),&objT->BounceFirstHeight,state);Marshal(node.GetAttributeValue("BounceSecondHeight","1.3"),&objT->BounceSecondHeight,state);Marshal(node.GetAttributeValue("BounceFirstPercentIndent","33"),&objT->BounceFirstPercentIndent,state);Marshal(node.GetAttributeValue("BounceSecondPercentIndent","66"),&objT->BounceSecondPercentIndent,state);
        Marshal(node.GetAttributeValue("GroundHitFX",null),&objT->GroundHitFX,state);Marshal(node.GetAttributeValue("GroundBounceFX",null),&objT->GroundBounceFX,state);Marshal(node.GetAttributeValue("FirstPercentHeight","33"),&objT->FirstPercentHeight,state);Marshal(node.GetAttributeValue("SecondPercentHeight","66"),&objT->SecondPercentHeight,state);Marshal(node.GetAttributeValue("CurveFlattenMinDist","0"),&objT->CurveFlattenMinDist,state);
        Marshal(node.GetAttributeValue("TumbleRandomly","false"),&objT->TumbleRandomly,state);Marshal(node.GetAttributeValue("AllowBouncing","false"),&objT->AllowBouncing,state);Marshal(node.GetAttributeValue("KillWhenRestingOnGround","false"),&objT->KillWhenRestingOnGround,state);Marshal(node.GetAttributeValue("OrientToFlightPath","false"),&objT->OrientToFlightPath,state);Marshal(node.GetAttributeValue("IgnoreTerrainHeight","false"),&objT->IgnoreTerrainHeight,state);Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 hit reaction retains Time timers, inline death mask, nullable validation and all three rotation/idle flags in their literal stock positions. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1HitReactionBehaviorModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("LightDamageHitReactionLifeTimer","0s"),&objT->LightDamageHitReactionLifeTimer,state);Marshal(node.GetAttributeValue("MediumDamageHitReactionLifeTimer","0s"),&objT->MediumDamageHitReactionLifeTimer,state);Marshal(node.GetAttributeValue("HeavyDamageHitReactionLifeTimer","0s"),&objT->HeavyDamageHitReactionLifeTimer,state);
        Marshal(node.GetAttributeValue("LightDamageHitReactionThreshold","0"),&objT->LightDamageHitReactionThreshold,state);Marshal(node.GetAttributeValue("MediumDamageHitReactionThreshold","0"),&objT->MediumDamageHitReactionThreshold,state);Marshal(node.GetAttributeValue("HeavyDamageHitReactionThreshold","0"),&objT->HeavyDamageHitReactionThreshold,state);Marshal(node.GetAttributeValue("RequiredDeathTypes",null),&objT->RequiredDeathTypes,state);Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);Marshal(node.GetAttributeValue("RotateToSourceWhenDamaged","false"),&objT->RotateToSourceWhenDamaged,state);Marshal(node.GetAttributeValue("RotateToSourceIgnoreIdleCheck","false"),&objT->RotateToSourceIgnoreIdleCheck,state);Marshal(node.GetAttributeValue("GoIdleWhenDamaged","false"),&objT->GoIdleWhenDamaged,state);Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain complete upgrade lists and both inline status masks without allocating or reading the KW-only ApplyToContained tail. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StatusBitsUpgradeModuleData* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("StatusToSet",null),&objT->StatusToSet,state);Marshal(node.GetAttributeValue("StatusToClear",null),&objT->StatusToClear,state);Marshal(node,(UpgradeModuleData*)objT,state);}
}
