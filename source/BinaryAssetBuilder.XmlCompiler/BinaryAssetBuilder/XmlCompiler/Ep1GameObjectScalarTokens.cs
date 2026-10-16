using System;
using System.Collections.Generic;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: explicit scalar-only root descriptors, independently audited against two stock objects; indirect, reference and enum fields are not inferred.
public static class Ep1GameObjectScalarTokens
{
    // Reborn: width distinguishes packed bool/byte/ushort storage from full words without deriving offsets from reflection at emission time.
    public readonly record struct Field(string Name, int Offset, uint Type, int Width);
    // Reborn: tags and native offsets are observed EP1 contracts, not XSD attribute ordering or a captured prefix buffer.
    public static IReadOnlyList<Field> Fields { get; } = Array.AsReadOnly(new Field[] {
        new("FenceWidth",88,0x645864F3u,4), new("FenceXOffset",92,0x645864F3u,4), new("RemoveTerrainRadius",96,0x645864F3u,4),
        new("EmotionRange",100,0x645864F3u,4), new("PlacementViewAngle",104,0x4E16EA4Du,4), new("JustBuiltTime",108,0x998669E8u,4),
        new("FactoryExitWidth",112,0x645864F3u,4), new("FactoryExtraBibWidth",116,0x645864F3u,4),
        new("BountyValue",136,0x23E650B8u,4), new("BuildTime",140,0x645864F3u,4), new("EnergyProduction",144,0x23E650B8u,4), new("EnergyBonus",148,0x23E650B8u,4),
        new("VoicePriority",172,0x23E650B8u,4), new("MinZIncreaseForVoiceMoveToHigherGround",176,0x645864F3u,4),
        new("CampnessValue",184,0x763177A4u,4), new("CampnessValueRadius",188,0x645864F3u,4), new("Scale",192,0x645864F3u,4),
        new("HealthBoxHeightOffset",196,0x645864F3u,4), new("LiveCameraFXPitch",200,0x645864F3u,4),
        new("FormationWidth",204,0x23E650B8u,4), new("FormationDepth",208,0x23E650B8u,4), new("InstanceScaleFuzziness",212,0x645864F3u,4),
        new("StructureRubbleHeight",216,0x645864F3u,4), new("RamPower",220,0x23E650B8u,4), new("RamZMult",224,0x645864F3u,4),
        new("ShockwaveResistance",228,0x645864F3u,4), new("CommandPoints",232,0x23E650B8u,4), new("CommandPointBonus",236,0x23E650B8u,4),
        new("VoiceAttackChargeTimeout",240,0x763177A4u,4), new("VoiceMoveLandToWaterTimeout",244,0x998669E8u,4),
        new("VoiceMoveWaterToLandTimeout",248,0x998669E8u,4), new("VoiceSelectUnderFireTimeout",252,0x998669E8u,4), new("VoiceSelectUnderFireDamageTime",256,0x998669E8u,4),
        new("MaxDistanceForEngaged",260,0x645864F3u,4), new("EngagedStateTimeout",264,0x763177A4u,4), new("ThreatLevel",268,0x645864F3u,4),
        new("SlopeLimitIndex",272,0x763177A4u,4), new("PathfindDiameter",276,0x645864F3u,4), new("SupplyOverride",280,0x23E650B8u,4),
        new("FiringArc",292,0x645864F3u,4), new("CamouflageDetectorLevel",296,0x23E650B8u,4), new("SelectionPriority",300,0x23E650B8u,4), new("PathPriority",304,0x23E650B8u,4),
        new("HasFiredRecentlyTime",364,0x998669E8u,4), new("ReinvisibilityDelay",372,0x998669E8u,4),
        new("InvisibilityOpacityMin",376,0x645864F3u,4), new("InvisibilityOpacityMax",380,0x645864F3u,4),
        new("SubGroupPriority",388,0x23E650B8u,4), new("EvaEventSecondDamageFarFromFirstScanRange",392,0x645864F3u,4),
        // Reborn: this unsigned duration uses the stock frame-duration tag, not Time/float or a raw unsigned-millisecond primitive.
        new("EvaEventSecondDamageFarFromFirstTimeoutMS",396,0x6F6D1AE7u,4),
        new("RefundValue",584,0x43501092u,2), new("MaxSimultaneousOfType",586,0x43501092u,2), new("TransportSlotCount",588,0x30916B96u,1),
        new("IsTrainable",589,0x77E3F4F7u,1), new("IsForbidden",590,0x77E3F4F7u,1), new("IsPrerequisite",591,0x77E3F4F7u,1),
        new("IsGrabbable",592,0x77E3F4F7u,1), new("IsHarvestable",593,0x77E3F4F7u,1), new("ForceLuaRegistration",594,0x77E3F4F7u,1),
        new("KeepSelectableWhenDead",595,0x77E3F4F7u,1), new("IsAutoBuilt",596,0x77E3F4F7u,1), new("CanPathThroughGates",597,0x77E3F4F7u,1),
        new("BuildInProximityToSamePlayerStucture",598,0x77E3F4F7u,1)
    });
}
