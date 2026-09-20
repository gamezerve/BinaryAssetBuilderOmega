using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

public enum ReplaceSelfOptions
{
    COPY_TARGET_OBJECT,
    IGNORE_TERRAIN_RESTRICTIONS,
    COPY_UPGRADES,
    DISABLE_DURING_REPLACE,
    CHECK_BUILD_ASSISTANT,
    DISABLE_NEW_OBJECT_DURING_UNPACK,
    CLEAR_LOCATION,
    REPLACE_OVER_ENEMIES,
    TRANSFER_EXPERIENCE
}

[StructLayout(LayoutKind.Sequential)]
public struct ReplaceSelfOptionsBitFlags
{
    public const int Count = 9;
    public const int BitsInSpan = 32;
    public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

    public unsafe fixed uint Value[NumSpans];
}

[StructLayout(LayoutKind.Sequential)]
public struct ReplaceSelfSpecialAbilityModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public Time NewObjectUnpackTime;
    public ReplaceSelfOptionsBitFlags ReplaceOptions;
    public float ClearTriggerDistance;
    public List<TypedAssetId<GameObject>> ReplacementTemplate;
}

[StructLayout(LayoutKind.Sequential)]
public struct ProjectileReplaceSelfSpecialAbilityModuleData
{
    public ReplaceSelfSpecialAbilityModuleData Base;
    public AssetReference<WeaponTemplate> LaunchingWeapon;
    public unsafe AssetReference<ObjectCreationList>* OtherObjectCreationList;
}
