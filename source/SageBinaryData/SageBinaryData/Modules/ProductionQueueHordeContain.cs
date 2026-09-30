using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: official RA3 tokenizer reads a weak template ID and optional filter pointer at offsets zero/four.
[StructLayout(LayoutKind.Sequential)]
public struct ProductionQueueHordeContainModuleDataTemplateContainer
{
    public TypedAssetId<GameObject> Template;
    public unsafe ObjectFilter* ObjectFilter;
}

// Reborn: this production queue module derives directly from BehaviorModuleData, not HordeContain.
[StructLayout(LayoutKind.Sequential)]
public struct ProductionQueueHordeContainModuleData
{
    public BehaviorModuleData Base;
    public List<ProductionQueueHordeContainModuleDataTemplateContainer> TemplateContainer;
}
