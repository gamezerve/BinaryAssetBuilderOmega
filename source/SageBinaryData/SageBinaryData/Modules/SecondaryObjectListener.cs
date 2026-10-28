using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 listener notifications use the two option bits in original schema order.
public enum SecondaryObjectListenerOption { NOTIFY_WEAPON_SET_UPDATE, NOTIFY_SUBOBJECT_UPGRADE_MUX }

// Reborn: both listener flags occupy one native word.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SecondaryObjectListenerBitFlags { public const int Count=2; public fixed uint Value[1]; }

// Reborn: each 128-byte retail effect entry embeds its required 120-byte filter before the strong upgrade list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ObjectEffectTuple
{
    public ObjectFilter ObjectFilter;
    public List<AssetReference<UpgradeTemplate>> UpgradeSelf;
}

// Reborn: the retail listener has a 24-byte root, optional status validation and ordered effect tuples.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SecondaryObjectListenerModule
{
    public BehaviorModuleData Base;
    public SecondaryObjectListenerBitFlags Flags;
    public ObjectStatusValidationDataType* SelfValidation;
    public List<ObjectEffectTuple> ObjectEffectTuple;
}
