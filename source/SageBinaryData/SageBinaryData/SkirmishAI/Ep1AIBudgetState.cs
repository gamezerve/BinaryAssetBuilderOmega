using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: the concrete heuristic schema class supplies this original polymorphic class word.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIStateHeuristic : IPolymorphic { public uint TypeId; }
// Reborn: the original constant weight follows the concrete class word.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIStateConstantHeuristic { public Ep1AIStateHeuristic Base;public float Weight; }
// Reborn: EP1 opening-move predicates have a one-byte completion flag and native tail padding.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIStateOpeningMoveHeuristic { public Ep1AIStateHeuristic Base;public SageBool Complete; }
// Reborn: nearby-enemy predicates retain the inserted minimum value and optional filter pointer before their flag.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateEnemyNearbyHeuristic
{
    public Ep1AIStateHeuristic Base;
    public float Distance;
    public uint MinEnemyValue;
    public ObjectFilter* EnemyFilter;
    public SageBool EnemyNearby;
}
// Reborn: object existence uses the full inline filter and its EP1 water/land location mask.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIStateObjectOfTypeExistsHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Ep1AITargetLocationBitFlags Location;
    public ObjectFilter ObjectFilter;
    public SageBool PassIfExists;
}
// Reborn: power comparison includes both complete kind masks and all three independent tail flags.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIStatePowerAdvantageHeuristic
{
    public Ep1AIStateHeuristic Base;
    public float Value;
    public KindOfBitFlags OwnKindOf,EnemyKindOf;
    public SageBool Above,IncludeOwnDefenses,IncludeEnemyDefenses;
}
// Reborn: a state owns a source-sized ordered polymorphic heuristic list.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIStateDefinition
{
    public BaseAssetType Base;
    public PolymorphicList<Ep1AIStateHeuristic> Heuristic;
}
// Reborn: original budget states append the ordered bank-account share list to the full AI state base.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIBudgetStateDefinition
{
    public Ep1AIStateDefinition Base;
    public AIBankBudget Budget;
}
