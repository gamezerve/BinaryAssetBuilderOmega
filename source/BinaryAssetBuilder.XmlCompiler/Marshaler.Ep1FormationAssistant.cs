using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize source-sized FormationAssistantUnitDefinition fields in their original EP1 native layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FormationAssistantUnitDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1FormationAssistantUnitDefinition.IncludeObjectFilter)),&target->IncludeObjectFilter,state);
        Marshal(node.GetChildNodes(nameof(Ep1FormationAssistantUnitDefinition.ExcludeObjectFilter)),&target->ExcludeObjectFilter,state);
        Marshal(node.GetChildNode(nameof(Ep1FormationAssistantUnitDefinition.PreviewObject),null),&target->PreviewObject,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize source-sized FormationAssistantCellContainer fields in their original EP1 native layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FormationAssistantCellContainer* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1FormationAssistantCellContainer.Definition)),&target->Definition,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize source-sized FormationAssistantRowDefinition fields in their original EP1 native layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FormationAssistantRowDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1FormationAssistantRowDefinition.Cell)),&target->Cell,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize source-sized FormationAssistantTemplate fields in their original EP1 native layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FormationAssistantTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1FormationAssistantTemplate.Row)),&target->Row,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize source-sized FormationSelectionTemplate fields in their original EP1 native layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FormationSelectionTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FormationSelectionTemplate.FormationAssistantTemplate),null),&target->FormationAssistantTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationSelectionTemplate.MaxDragLength),null),&target->MaxDragLength,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationSelectionTemplate.MaxUnitsSelected),null),&target->MaxUnitsSelected,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize source-sized FormationAssistant fields in their original EP1 native layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FormationAssistant* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.DefaultPreviewObject),null),&target->DefaultPreviewObject,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.FacingArrowTemplateHead),null),&target->FacingArrowTemplateHead,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.FacingArrowTemplateBody),null),&target->FacingArrowTemplateBody,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.FacingArrowTemplateBase),null),&target->FacingArrowTemplateBase,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.RowPadding),null),&target->RowPadding,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.ColumnPadding),null),&target->ColumnPadding,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.ActivateDragDistance),null),&target->ActivateDragDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FormationAssistant.ActivateTime),null),&target->ActivateTime,state);
        Marshal(node.GetChildNode(nameof(Ep1FormationAssistant.ObjectFilter),null),&target->ObjectFilter,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

}
