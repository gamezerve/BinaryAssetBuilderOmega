using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: EP1 scatter delays and filters are inline records; the final boolean follows the complete filter.
[StructLayout(LayoutKind.Sequential)]
public struct ScatterDamage
{
    public int ScatterAmount;
    public Time ScatterDelay;
}

// Reborn: layout and optional weapon import match the original prismatic weapon's complete 144-byte record.
[StructLayout(LayoutKind.Sequential)]
public struct WeaponScatterEffectUpdateModuleData
{
    public UpdateModuleData Base;
    // Reborn: the original populated prism record stores an optional reference pointee, relocated separately from its import.
    public unsafe AssetReference<WeaponTemplate>* WeaponTemplate;
    public ScatterDamage ScatterSettings;
    public ObjectFilter Filter;
    public SageBool RandomScatter3D;
}
