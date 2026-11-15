using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: the EP1 reader allocates eight bytes per upgrade/template pair and 72 per model-condition offset.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1SubStructureBuildUpgradeModuleData
{
    public const uint WireTypeId = 0x3176B098u;
    public UpgradeModuleData Base;
    public List<Ep1AvailableSubStructure> AvailableSubStructure;
    public List<Ep1SubStructureOffset> StructureOffset;
}

[StructLayout(LayoutKind.Sequential)]
public struct Ep1AvailableSubStructure
{
    public TypedAssetId<UpgradeTemplate> UpgradeTrigger;
    public TypedAssetId<GameObject> ObjectTemplate;
}

[StructLayout(LayoutKind.Sequential)]
public struct Ep1SubStructureOffset
{
    public ModelConditionBitFlags ModelCondition;
    public Coord3D Offset;
}
