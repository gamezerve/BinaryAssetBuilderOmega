using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct GameDependencyType
    {
        // Reborn: RA3 stores all variable-width dependency masks as relocated pointers.
        public ModelConditionBitFlags* RequiredModelConditionsAny;
        public ModelConditionBitFlags* ForbiddenModelConditions;
        public ObjectStatusBitFlags* RequiredObjectStatusAny;
        public List<TypedAssetId<GameObject>> RequiredObject;
        public List<TypedAssetId<UpgradeTemplate>> ForbiddenUpgrade;
        public List<TypedAssetId<UpgradeTemplate>> NeededUpgrade;
        public ObjectFilter* ObjectFilter;
    }
}
