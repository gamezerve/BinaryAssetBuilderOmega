using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: Uprising exposes the native dynamics allocation limits as a standalone inheritable asset.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsSettings
    {
        public BaseInheritableAsset Base;
        public int MaximumObjects;
        public int MaximumContacts;
        public int MaximumContactPairs;
        public int MaximumJoints;
        public SageBool CreateGlobalIsland;
    }
}
