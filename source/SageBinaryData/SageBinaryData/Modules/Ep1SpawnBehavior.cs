using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 adds a status-validation pointer before sixteen packed flags; the legacy KW record remains separate.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpawnBehaviorModuleData
{
    public UpgradeModuleData Base;
    public int SpawnNumberData;
    public Time SpawnReplaceDelayData;
    public int InitialBurst;
    public DamageBitFlags DamageTypesToPropagateToSlaves;
    public Time FadeInTime;
    public Time InitialDelay;
    public DieMuxDataType* Die;
    public List<TypedAssetId<GameObject>> SpawnTemplate;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public SageBool IsOneShotData;
    public SageBool CanReclaimOrphans;
    public SageBool AggregateHealth;
    public SageBool ExitByBudding;
    public SageBool SpawnedRequireSpawner;
    public SageBool RespectCommandLimit;
    public SageBool KillSpawnBasedOnModelConditionState;
    public SageBool ShareUpgrades;
    public SageBool SpawnInsideBuilding;
    public SageBool CombineOnCreate;
    public SageBool KillSpawnsOnDisabled;
    public SageBool PassExperienceToSpawned;
    public SageBool KillSpawnsOnCaptured;
    public SageBool KillSpawnsWhenObjectInvalid;
    public SageBool SpawnOnUnpack;
    public SageBool SetPurchasePrice;
}
