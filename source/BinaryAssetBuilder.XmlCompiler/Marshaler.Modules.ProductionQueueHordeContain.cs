using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile weak template names and optional production queue filters in native record order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ProductionQueueHordeContainModuleDataTemplateContainer* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ProductionQueueHordeContainModuleDataTemplateContainer.Template), null), &objT->Template, state);
        Marshal(node.GetChildNode(nameof(ProductionQueueHordeContainModuleDataTemplateContainer.ObjectFilter), null), &objT->ObjectFilter, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Restore the missing RA3/EP1 production queue containment module and template list. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ProductionQueueHordeContainModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNodes(nameof(ProductionQueueHordeContainModuleData.TemplateContainer)), &objT->TemplateContainer, state);
        Marshal(node, (BehaviorModuleData*)objT, state);
    }
}
