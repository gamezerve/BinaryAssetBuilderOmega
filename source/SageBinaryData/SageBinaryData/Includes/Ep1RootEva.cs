using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: the original EP1 schema has thirty EVA ordinals; these are not KW's fixed pointer-field positions.
public enum Ep1ThingTemplateEvaEventType
{
    dieOwner,dieAlly,dieEnemy,soldOwner,damagedOwner,damagedFromShroudedSourceOwner,damagedByFireOwner,
    secondDamageFarFromFirstOwner,secondDamageFarFromFirstScanRange,secondDamageFarFromFirstTimeoutMS,
    damagedAlly,ambushed,repairingOwner,enemyObjectSighted,enemyObjectSightedAfterRespawn,onFirstSightingEnemy,
    onFirstSightingNonEnemy,detectedEnemy,detectedAlly,detectedOwner,availableForProduction,productionStarted,
    productionComplete,placementFailed,cannotBuildDueToFullBuildQueue,buildOnHold,buildCancelled,promotedOwner,
    manuallyPoweredOff,manuallyPoweredOn
}

// Reborn: schema/catalog resolution enforces EvaEvent identity; the shared native handle type avoids inventing a missing EvaEvent CLR asset model.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EvaEventEntry { public AssetReference<BaseAssetType> EvaEvent; public Ep1ThingTemplateEvaEventType EvaType; }

// Reborn: EP1 root EVA is one ordered list header, not the older KW GameObjectEvaEvents pointer block.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EvaEventArray { public List<Ep1EvaEventEntry> EvaEntry; }
