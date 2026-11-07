using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: Uprising's target and sort ordinals differ from the retained KW models.
public enum Ep1TargetingHeuristicType { EnemyStructure,FriendlyStructure,EnemyUnit,FriendlyUnit,UnderAttackBy,EnemyUnitCloseToShore,EnemyStructureCloseToShore,NeutralGarrison,EnemyGarrison,Expansion,TechBuilding,Bridge,Beacon,Crate,Help,Husk,Point,UnpackBuildingPlacement,NamedObject,Waypoint,Team }
public enum Ep1AITargetSortType { Distance,ThreatToGround,ThreatToAir,ThreatToWater,Random,ClosestToEnemy }
public enum Ep1AIBeaconType { Invalid=-1,Assault=0,HoldPosition=1,HoldObject=2 }
public enum Ep1AITargetLocationType { Water,Land }

// Reborn: the original Water/Land flags occupy one native word.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AITargetLocationBitFlags { public fixed uint Value[1]; }
[StructLayout(LayoutKind.Sequential)]
public struct Ep1BuildingPlacementDistance { public float Distance; }

// Reborn: native EP1 target heuristics use optional beacon/name pointers and retain four boolean flags after the lists.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AITargetingHeuristic
{
    public BaseAssetType Base;
    public Ep1TargetingHeuristicType HeuristicType;
    public Ep1AITargetSortType SortType;
    public KindOfBitFlags VitalKindOf,ForbiddenKindOf;
    public ObjectStatusBitFlags VitalObjectStatus,ForbiddenObjectStatus;
    public float* ThreatFinderRange;
    public float SearchRange;
    public Ep1AIBeaconType* Beacon;
    public AnsiString* Name;
    public Ep1AITargetLocationBitFlags Location;
    public float MaxDistanceToShore;
    // Reborn: optional relationship masks are pointees; the header retains a four-byte pointer at +180.
    public ObjectFilterRelationshipBitMask* Relationship;
    public float MinDistanceFromEnemy,MinDistanceFromConyardEntrance;
    public Time IgnoreMinDistanceFromConyardEntranceForTime;
    public AIBaseSelectionCriteria* BaseSelection;
    public List<Ep1BuildingPlacementDistance> BuildingPlacementDistance;
    public SageBool Destroyed,HighestPriorityEnemyPlayersOnly,UnderAttack,RequireUniqueTarget;
}
