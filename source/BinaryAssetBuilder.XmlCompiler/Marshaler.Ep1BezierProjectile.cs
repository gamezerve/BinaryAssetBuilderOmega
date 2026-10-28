using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse the complete EP1 Bezier flag word without changing legacy projectile boolean storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,BezierProjectileBitFlag* objT,Tracker state)
    {
        if(value==null)return;uint bits=0;
        foreach(string token in value.GetText().Split(WhiteSpaces,System.StringSplitOptions.RemoveEmptyEntries)){bool remove=token[0]=='-';string name=token[0]=='+'||remove?token.Substring(1):token;uint mask=name=="ALL"?(1u<<BezierProjectileBitFlag.Count)-1u:1u<<(int)System.Enum.Parse<BezierProjectileFlag>(name);bits=remove?bits&~mask:bits|mask;}
        objT->Value[0]=bits;state.InplaceEndianToPlatform(&objT->Value[0]);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every official EP1 field with its schema default, preserving source-driven FX/weapon occurrences and native units. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1BezierProjectileBehaviorModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Flags","PING_PONG_SIDEWAYS_DRIFT"),&objT->Flags,state);
        Marshal(node.GetAttributeValue("Type","MISSILE"),&objT->Type,state);
        Marshal(node.GetAttributeValue("FireStraightDistance",null),&objT->FireStraightDistance,state);
        Marshal(node.GetAttributeValue("FirstHeight",null),&objT->FirstHeight,state);
        Marshal(node.GetAttributeValue("SecondHeight",null),&objT->SecondHeight,state);
        Marshal(node.GetAttributeValue("FirstHeightMin",null),&objT->FirstHeightMin,state);
        Marshal(node.GetAttributeValue("FirstHeightMax",null),&objT->FirstHeightMax,state);
        Marshal(node.GetAttributeValue("SecondHeightMin",null),&objT->SecondHeightMin,state);
        Marshal(node.GetAttributeValue("SecondHeightMax",null),&objT->SecondHeightMax,state);
        Marshal(node.GetAttributeValue("FirstPercentIndent",null),&objT->FirstPercentIndent,state);
        Marshal(node.GetAttributeValue("SecondPercentIndent",null),&objT->SecondPercentIndent,state);
        Marshal(node.GetAttributeValue("FinalStuckTime",null),&objT->FinalStuckTime,state);
        Marshal(node.GetAttributeValue("PreLandingStateTime",null),&objT->PreLandingStateTime,state);
        Marshal(node.GetAttributeValue("BounceCount",null),&objT->BounceCount,state);
        Marshal(node.GetAttributeValue("BounceDistance",null),&objT->BounceDistance,state);
        Marshal(node.GetAttributeValue("BounceFirstHeight",null),&objT->BounceFirstHeight,state);
        Marshal(node.GetAttributeValue("BounceSecondHeight",null),&objT->BounceSecondHeight,state);
        Marshal(node.GetAttributeValue("BounceFirstPercentIndent",null),&objT->BounceFirstPercentIndent,state);
        Marshal(node.GetAttributeValue("BounceSecondPercentIndent",null),&objT->BounceSecondPercentIndent,state);
        Marshal(node.GetAttributeValue("GarrisonHitKillRequiredKindOf",null),&objT->GarrisonHitKillRequiredKindOf,state);
        Marshal(node.GetAttributeValue("GarrisonHitKillForbiddenKindOf",null),&objT->GarrisonHitKillForbiddenKindOf,state);
        Marshal(node.GetAttributeValue("GarrisonHitKillCount",null),&objT->GarrisonHitKillCount,state);
        Marshal(node.GetAttributeValue("GarrisonHitKillFX",null),&objT->GarrisonHitKillFX,state);
        Marshal(node.GetAttributeValue("GroundHitFX",null),&objT->GroundHitFX,state);
        Marshal(node.GetAttributeValue("MaxDistanceReachedFX",null),&objT->MaxDistanceReachedFX,state);
        Marshal(node.GetAttributeValue("GroundBounceFX",null),&objT->GroundBounceFX,state);
        Marshal(node.GetAttributeValue("GroundHitWeapon",null),&objT->GroundHitWeapon,state);
        Marshal(node.GetAttributeValue("GroundBounceWeapon",null),&objT->GroundBounceWeapon,state);
        Marshal(node.GetAttributeValue("FlightPathAdjustDistPerSecond",null),&objT->FlightPathAdjustDistPerSecond,state);
        Marshal(node.GetAttributeValue("FirstPercentHeight",null),&objT->FirstPercentHeight,state);
        Marshal(node.GetAttributeValue("SecondPercentHeight",null),&objT->SecondPercentHeight,state);
        Marshal(node.GetAttributeValue("CurveFlattenMinDist",null),&objT->CurveFlattenMinDist,state);
        Marshal(node.GetAttributeValue("PreLandingEmotion",null),&objT->PreLandingEmotion,state);
        Marshal(node.GetAttributeValue("PreLandingEmotionRadius",null),&objT->PreLandingEmotionRadius,state);
        Marshal(node.GetAttributeValue("InvisibleTime",null),&objT->InvisibleTime,state);
        Marshal(node.GetAttributeValue("FadeInTime",null),&objT->FadeInTime,state);
        Marshal(node.GetAttributeValue("PostLandingStateTime",null),&objT->PostLandingStateTime,state);
        Marshal(node.GetAttributeValue("PostLandingEmotion",null),&objT->PostLandingEmotion,state);
        Marshal(node.GetAttributeValue("PostLandingEmotionRadius",null),&objT->PostLandingEmotionRadius,state);
        Marshal(node.GetAttributeValue("SidewaysDrift",null),&objT->SidewaysDrift,state);
        Marshal(node.GetAttributeValue("MaxDistanceToTravel","1000000"),&objT->MaxDistanceToTravel,state);
        Marshal(node.GetAttributeValue("TargetVectorOffset",null),&objT->TargetVectorOffset,state);
        Marshal(node.GetAttributeValue("Acceleration","0.0"),&objT->Acceleration,state);
        Marshal(node.GetAttributeValue("DisabledTypesToProcess",""),&objT->DisabledTypesToProcess,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
