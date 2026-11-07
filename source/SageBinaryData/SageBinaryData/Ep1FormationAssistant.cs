using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: formation unit definitions retain both variable filter arrays and their inline weak preview identity.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FormationAssistantUnitDefinition
{
    public BaseAssetType Base;
    public List<ObjectFilter> IncludeObjectFilter,ExcludeObjectFilter;
    public TypedAssetId<GameObject> PreviewObject;
}

// Reborn: each original formation cell stores a variable list of strong unit-definition handles.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FormationAssistantCellContainer
{
    public List<AssetReference<Ep1FormationAssistantUnitDefinition>> Definition;
}

// Reborn: formation rows preserve every authored cell in order.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FormationAssistantRowDefinition
{
    public List<Ep1FormationAssistantCellContainer> Cell;
}

// Reborn: the formation asset owns ordered rows rather than a fixed maximum grid.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FormationAssistantTemplate
{
    public BaseAssetType Base;
    public List<Ep1FormationAssistantRowDefinition> Row;
}

// Reborn: drag-selection criteria keep their strong formation handle and original numeric thresholds.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FormationSelectionTemplate
{
    public BaseAssetType Base;
    public AssetReference<Ep1FormationAssistantTemplate> FormationAssistantTemplate;
    public float MaxDragLength;
    public uint MaxUnitsSelected;
}

// Reborn: the global assistant has four inline weak preview identities and an optional filter graph.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1FormationAssistant
{
    public BaseAssetType Base;
    public TypedAssetId<GameObject> DefaultPreviewObject,FacingArrowTemplateHead,FacingArrowTemplateBody,FacingArrowTemplateBase;
    public float RowPadding,ColumnPadding,ActivateDragDistance,ActivateTime;
    public ObjectFilter* ObjectFilter;
}
