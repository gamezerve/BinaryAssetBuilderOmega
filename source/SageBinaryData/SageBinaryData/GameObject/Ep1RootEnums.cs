namespace SageBinaryData;

// Reborn: EP1 radar uses official forced values; keep the older shared RadarPriorityType untouched.
public enum Ep1RadarPriorityType { NOT_ON_RADAR = -2, INVALID = -1, STRUCTURE = 0, UNIT = 1, COMMANDO = 2 }

// Reborn: EP1 adds four editor classes after the shared legacy prefix without changing other users of EditorSortingType.
public enum Ep1EditorSortingType
{
    NONE, STRUCTURE, UNIT, SHRUBBERY, MISC_MAN_MADE, MISC_NATURAL, DEBRIS, SYSTEM, AUDIO, TEST,
    FOR_REVIEW, ROAD, WAYPOINT, EMITTERS, SCORCH, OBSOLETE, DESTRUCTIBLE_PROPS, OPTIMIZED_PROPS, CAMPAIGN_UNITS, CAMPAIGN_STRUCTURES
}

// Reborn: stock MAIN_STRUCTURE=0 and WATERCRAFT=6 confirm the engine INVALID=-1 policy, not zero-based XSD declaration ordinals.
public enum Ep1ProductionQueueType { INVALID = -1, MAIN_STRUCTURE, OTHER_STRUCTURE, INFANTRY, VEHICLE, AIRCRAFT, UPGRADE, WATERCRAFT }

// Reborn: root locomotor records use INVALID=-1, independently of the reviewed toggle-state ordinal encoding and legacy COMBO2/COMBO3 names.
public enum Ep1RootLocomotorSetType { INVALID = -1, NORMAL, NORMAL_UPGRADED, FREEFALL, WANDER, PANIC, TAXIING, SUPERSONIC, MOUNTED, ENRAGED, SCARED, CONTAINED, COMBO, ATTACKING, MOVING_TO_TRANSPORT, WALL_SCALING, CHANGING_FIRINGARC, BURNINGDEATH, EXITING_PRODUCTION_STRUCTURE, SPECIALABILITY_ACTIVE }
