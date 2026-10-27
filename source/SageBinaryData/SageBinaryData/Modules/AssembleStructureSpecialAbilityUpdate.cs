using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: the retail Engineer uses a 272-byte assembly ability with a source-sized weak structure list.
[StructLayout(LayoutKind.Sequential)]
public struct AssembleStructureSpecialAbilityUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public Time NewObjectAssemblyTime;
    public ModelConditionFlagType ModelConditionsOnAssembly;
    public List<TypedAssetId<GameObject>> StructureTemplate;
    public SageBool BusyDuringAssembly;
}
