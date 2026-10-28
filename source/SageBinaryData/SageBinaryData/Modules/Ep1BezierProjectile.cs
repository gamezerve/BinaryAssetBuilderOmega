using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData
{
    // Reborn: EP1 projectile discriminators and all nineteen flag ordinals follow their official XSD order.
    public enum BezierProjectileType { MISSILE,TORPEDO }
    public enum BezierProjectileFlag { TUMBLE_RANDOMLY,DETONATE_CALLS_KILL,ORIENT_TO_FLIGHT_PATH,CRUSH_STYLE,NO_DETONATE,DIE_ON_IMPACT,IGNORE_TERRAIN_HEIGHT,PING_PONG_SIDEWAYS_DRIFT,ADJUST_STRAIGHT_ONLY,DONT_DIE_ON_DETONATE,DONT_SET_NO_ATTACK_STATUS,DONT_TRACK_TARGET,DETONATE_ON_APEX,USE_ATTACHPOS_ON_VICTIM,IGNORE_CONTACT_POINTS,TARGET_OFFSET_ALONG_TARGET_VECTOR,FIRE_STRAIGHT_THEN_CURVE,DONT_DETONATE_WITHOUT_COLLISION,DONT_GO_UNDERWATER }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BezierProjectileBitFlag { public const int Count=19; public const int BitsInSpan=32; public const int NumSpans=1; public fixed uint Value[1]; }
    // Reborn: the 256-byte EP1 record has a packed Flags/Type prefix and acceleration/disabled tail, instead of KW's individual booleans.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1BezierProjectileBehaviorModuleData
    {
        public UpdateModuleData Base;
        public BezierProjectileBitFlag Flags;
        public BezierProjectileType Type;
        public float FireStraightDistance;
        public float FirstHeight;
        public float SecondHeight;
        public float FirstHeightMin;
        public float FirstHeightMax;
        public float SecondHeightMin;
        public float SecondHeightMax;
        public Percentage FirstPercentIndent;
        public Percentage SecondPercentIndent;
        public Time FinalStuckTime;
        public Time PreLandingStateTime;
        public int BounceCount;
        public float BounceDistance;
        public float BounceFirstHeight;
        public float BounceSecondHeight;
        public Percentage BounceFirstPercentIndent;
        public Percentage BounceSecondPercentIndent;
        public KindOfBitFlags GarrisonHitKillRequiredKindOf;
        public KindOfBitFlags GarrisonHitKillForbiddenKindOf;
        public uint GarrisonHitKillCount;
        public AssetReference<FXList> GarrisonHitKillFX;
        public AssetReference<FXList> GroundHitFX;
        public AssetReference<FXList> MaxDistanceReachedFX;
        public AssetReference<FXList> GroundBounceFX;
        public AssetReference<WeaponTemplate> GroundHitWeapon;
        public AssetReference<WeaponTemplate> GroundBounceWeapon;
        public Velocity FlightPathAdjustDistPerSecond;
        public Percentage FirstPercentHeight;
        public Percentage SecondPercentHeight;
        public float CurveFlattenMinDist;
        public EmotionType PreLandingEmotion;
        public float PreLandingEmotionRadius;
        public Time InvisibleTime;
        public Time FadeInTime;
        public Time PostLandingStateTime;
        public EmotionType PostLandingEmotion;
        public float PostLandingEmotionRadius;
        public float SidewaysDrift;
        public float MaxDistanceToTravel;
        public float TargetVectorOffset;
        public float Acceleration;
        public DisabledBitFlags DisabledTypesToProcess;
    }
}
