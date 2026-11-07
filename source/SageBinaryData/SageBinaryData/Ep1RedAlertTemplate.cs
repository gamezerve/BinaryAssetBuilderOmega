using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: original EP1 Red Alert thresholds, EVA imports, optional modifier handles and tail flags occupy a 96-byte header.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1RedAlertTemplate
{
    public BaseInheritableAsset Base;
    public uint OrangeAreaThreatThreshold,RedAreaThreatThreshold,MaxAllowedThreat;
    public float KillThreatMultiplier,DeathThreatMultiplier;
    public uint YellowAreaThreatDecayPerSecond,OrangeAreaThreatDecayPerSecond,RedAreaThreatDecayPerSecond;
    public uint RedAlertBuildingDamagePerSecond,RedAlertUpdatedBuildingDamagePerSecond;
    public Time RedAlertBuildingDamageIncreaseTime;
    public AssetReference<BaseAssetType> EvaEventToPlayOnRedAlertInitiateOwner,EvaEventToPlayOnRedAlertInitiateAlly,EvaEventToPlayOnRedAlertInitiateEnemy;
    public float YellowAlertTechPointMultiplier,OrangeAlertTechPointMultiplier,RedAlertTechPointMultiplier;
    public uint MaxDepositChunkSize;
    public AssetReference<AttributeModifier>* YellowAreaEffect;
    public AssetReference<AttributeModifier>* OrangeAreaEffect;
    public AssetReference<AttributeModifier>* RedAreaEffect;
    public AssetReference<AttributeModifier>* RedAlertEffect;
    public SageBool DisableNonRepairability,RedAlertDisableStructureBuilding,RedAlertShowAllUnitsAndStructures;
}
