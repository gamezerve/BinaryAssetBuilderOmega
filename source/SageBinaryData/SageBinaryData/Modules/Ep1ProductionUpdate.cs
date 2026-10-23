using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Type reuses the existing stock-qualified Ep1ProductionQueueType (GameObject/Ep1RootEnums.cs).

// Reborn: EP1 ProductionModifier has no Hero flags; this item layout is declared for type completeness only and populated lists remain refused.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ProductionModifier
{
    public AssetReference<UpgradeTemplate> RequiredUpgrade;
    public float CostMultiplier;
    public float TimeMultiplier;
    public ObjectFilter ModifierFilter;
}

// Reborn: EP1 upgrade translation pairs are stock-qualified against the Barracks and retain two strong import words per source row.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1UpgradeTranslation
{
    public AssetReference<UpgradeTemplate> RequiredUpgrade;
    public AssetReference<UpgradeTemplate> ProvidedUpgrade;
}

// Reborn: optional EP1 GiveUpgradeToProduced pointee with a source-sized translation list; absent remains null.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1GiveUpgradeToProduced
{
    public List<Ep1UpgradeTranslation> UpgradeTranslate;
}

// Reborn: 84-byte EP1 record taken from stock GameObject:SovietRefinery metadata. It has no KW DoorOpeningTimeMetaUnit,
// SetBonusModelConditionOnSpeedBonus or SpeedBonusAudioLoop, adds NewObjectFadeInTime/GiveUpgradeToProduced and packs four booleans at the tail.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ProductionUpdateModuleData
{
    public UpdateModuleData Base;
    public uint MaxQueueEntries;
    public int NumDoorAnimations;
    public Time DoorOpeningTime;
    public Time DoorWaitOpenTime;
    public Time DoorCloseTime;
    public uint ConstructionCompleteDuration;
    public DisabledBitFlags DisabledTypesToProcess;
    public uint UnitInvulnerableTime;
    public uint SpecialPrepModelconditionTime;
    public TypedAssetId<GameObject> BonusForType;
    public Ep1ProductionQueueType Type;
    public Time ProductionTimeDelayScalar;
    public Time NewObjectFadeInTime;
    public List<QuantityModifier> QuantityModifier;
    public List<Ep1ProductionModifier> ProductionModifier;
    public Ep1GiveUpgradeToProduced* GiveUpgradeToProduced;
    public SageBool GiveNoXP;
    public SageBool VeteranUnitsFromVeteranFactory;
    public SageBool SecondaryQueue;
    public SageBool IgnorePreReqs;
}
