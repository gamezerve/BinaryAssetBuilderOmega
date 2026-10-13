using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: official EP1 names differ from legacy KW locomotor labels; ordinal zero denotes INVALID before the native minus-one bias.
public enum Ep1ToggleLocomotorSetType { INVALID, NORMAL, NORMAL_UPGRADED, FREEFALL, WANDER, PANIC, TAXIING, SUPERSONIC, MOUNTED, ENRAGED, SCARED, CONTAINED, COMBO, ATTACKING, MOVING_TO_TRANSPORT, WALL_SCALING, CHANGING_FIRINGARC, BURNINGDEATH, EXITING_PRODUCTION_STRUCTURE, SPECIALABILITY_ACTIVE }

// Reborn: stock toggle states embed AI at +132 and two count/items lists at +288/+296; this model alone does not admit module publication.
[StructLayout(LayoutKind.Sequential)]
public struct ToggleStatusNugget
{
    public AssetReference<AttributeModifier> AttributeModifier;
    public ModelConditionBitFlags ModelConditions;
    public ObjectStatusBitFlags ObjectStatus;
    public AssetReference<UpgradeTemplate> Upgrade;
    public uint ArmorSet;
    public uint LocomotorSet;
    public TypedAssetId<ModuleData> AIUpdateModuleId;
    public AnsiString GenericLuaEventParameter;
    public AssetReference<BaseAudioEventInfo, AudioEventInfo> EnterStateSound;
    public AssetReference<BaseAudioEventInfo, AudioEventInfo> InitiateIntentToExitStateSound;
    public uint DisabledType;
    public AIToggleSpecialPowerInfo SkirmishAiInfo;
    public List<SpecialPowerModuleInfo> Module;
    public List<AnsiString> ActiveGeometry;
}

// Reborn: retain the reviewed 252-byte special-ability base, inline toggle list and packed bools in the 264-byte concrete module.
[StructLayout(LayoutKind.Sequential)]
public struct ToggleStatusSpecialAbilityUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public List<ToggleStatusNugget> ToggleState;
    public SageBool ReloadAllWeaponsOnToggle;
    public SageBool ForceIdleAfterToggle;
}
