using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 removes the legacy Science word; each upgrade pair contains exactly one strong OCL reference.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1OCLUpgradePair { public AssetReference<ObjectCreationList> OCL; }

// Reborn: preserve all fifteen official EP1 location ordinals, including the inserted path-line option.
public enum Ep1OCLCreateLocType
{
    CREATE_AT_EDGE_NEAR_SOURCE, CREATE_AT_EDGE_NEAR_TARGET,
    CREATE_AT_EDGE_NEAR_TARGET_AND_MOVE_TO_LOCATION, CREATE_AT_LOCATION,
    USE_OWNER_OBJECT, CREATE_ABOVE_LOCATION, CREATE_AT_EDGE_FARTHEST_FROM_TARGET,
    CREATE_CLOSEST_TO_SPAWN_POINT, USE_SECONDARY_OBJECT_LOCATION,
    USE_MULTIPLE_SECONDARY_OBJECT_LOCATIONS, CREATE_AT_EDGE_ALONG_PATH_LINE,
    CREATE_AT_EDGE_NEAR_SECONDARY_TARGET, CREATE_AT_OFFSET_FROM_TARGET,
    CREATE_AT_OFFSET_FROM_TARGET_AND_MOVE_TO_TARGET,
    CREATE_AT_OFFSET_FROM_TARGET_ALONG_SECONDARY_OBJECT_VECTOR_AND_MOVE_TO_TARGET
}

// Reborn: retail EP1 adds CreateLocationOffset to the legacy power and uses four-byte UpgradeOCL items; header size is 524 bytes on x86.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1OCLSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    public AssetReference<ObjectCreationList> OCL;
    public Ep1OCLCreateLocType CreateLocation;
    public float CreateLocationOffset;
    public uint MaxCreateCount;
    public uint NumberToSpawn;
    public AssetReference<ObjectCreationList> DestinationOCL;
    public List<Ep1OCLUpgradePair> UpgradeOCL;
    public ObjectFilter* NearestSecondaryObjectFilter;
    public List<AssetReference<UpgradeTemplate>> Upgrade;
    public SageBool RegisterObjectsWithSpecialAbilityUpdate;
}
