using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: registry identities use the complete explicit official EP1 enumeration, retaining its inserted categories.
public enum Ep1RegisteredObjectSetId { PLAYER_EVERYTHING,GARRISONS,NEUTRAL_TECH,TIBERIUM_FIELDS,ORE_MINES,SIGMA_FIELDS,GLOBAL_HARVESTERS,GLOBAL_TIBERIUM_REFINERIES,GLOBAL_ORE_REFINERIES,GLOBAL_SIGMA_REFINERIES,STRUCTURES,FACTION_STRUCTURES,PLAYER_NEUTRAL_TECH,UNITS,BEACONS,UNITS_AND_STRUCTURES,BASE_DEFENSES_AND_GARRISONS,MOBILE_GROUND_UNITS,MCVS,DOZERS,HARVESTERS,SURVEYORS,OUTPOSTS,INFANTRY,VEHICLES,SHIPS,AIRCRAFT,UNPACKS_INTO_BUILDING_UNITS,HORDES,HORDE_MEMBERS,ENGINEERS,ENEMY_ENGINEERS,VICTORY_CONDITION_STRUCTURES,CONYARDS,FACTORIES,UNIT_FACTORIES,RADARS,TECH_CENTERS,POWER_PLANTS,REFINERIES,REFINERIES_AND_REFINERY_EGGS,TIBERIUM_REFINERIES,ORE_REFINERIES,SIGMA_REFINERIES,SILOS,FARMS,BASE_DEFENSES,ENEMY_BASE_DEFENSES,AI_PLAYER_POWER_UPDATABLE,SKIRMISH_AI_DEFENSE_TARGETS,SKIRMISH_AI_THREATS_TO_DODGE,SKIRMISH_AI_NORMAL_COMBAT_UNITS,ENCLOSURES }
// Reborn: registry ownership follows the original GLOBAL/PLAYER ordering.
public enum Ep1RegisteredObjectSetType { GLOBAL,PLAYER }

// Reborn: registered sets have an inline full 120-byte filter after their two enum words.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1RegisteredObjectSet
{
    public BaseAssetType Base;
    public Ep1RegisteredObjectSetId SetId;
    public Ep1RegisteredObjectSetType Type;
    public ObjectFilter ObjectFilter;
}

// Reborn: player ranks retain both numeric grants and a variable strong PlayerTech list in the original global record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PlayerTechRank
{
    public BaseAssetType Base;
    public uint ExperiencePointsToNextRank,TechPointsGranted;
    public List<AssetReference<Ep1PlayerTech>> TechGranted;
}
