using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 damage-range children are twelve-byte percentage/percentage/import records, not KW DamageCreation records.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ActiveBodyDamageRangeModifier
{
    public Percentage Min;
    public Percentage Max;
    public AssetReference<AttributeModifier> AttributeModifier;
}

// Reborn: separate 76-byte EP1 body layout preserves legacy KW models and derived bodies unchanged.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ActiveBodyModuleData
{
    public BodyModuleData Base;
    public float MaxHealth;
    public Percentage MaxHealthDamagedPercentage;
    public Percentage MaxHealthReallyDamagedPercentage;
    public float InitialHealth;
    public float DodgePercent;
    public Time EnteringDamagedTransitionTime;
    public Time EnteringReallyDamagedTransitionTime;
    public Relo.String<sbyte> GrabObject;
    public AssetReference<FXList> HealingBuffFX;
    public AssetReference<AttributeModifier> DamagedAttributeModifier;
    public AssetReference<AttributeModifier> ReallyDamagedAttributeModifier;
    public float CheerRadius;
    public AssetReference<FXList> BurningDeathFX;
    public List<Ep1ActiveBodyDamageRangeModifier> DamageRangeModifier;
    public SageBool RemoveUpgradesOnDeath;
    public SageBool BurningDeathBehavior;
}
