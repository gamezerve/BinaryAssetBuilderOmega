using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct BoobyTrapUpdateModuleData
{
    // Reborn: retain the retail EP1 bone, filter and modifier offsets.
    public Ep1AttachUpdateModuleData Base;
    public ObjectStatusBitFlags FireWeaponConditionsOnParent;
    public ObjectStatusBitFlags PreventEnteringSetStatusOnParent;
    public AssetReference<WeaponTemplate> SpecialWeapon;
    public unsafe ObjectFilter* PreventEnteringFilter;
    public SageBool AllowTriggerOnAllies;
}
