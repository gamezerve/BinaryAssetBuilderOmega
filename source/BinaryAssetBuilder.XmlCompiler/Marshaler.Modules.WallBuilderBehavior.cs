using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all Uprising wall-builder fields with the official defaults and source-sized connection-filter lists. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, WallBuilderBehaviorModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(WallBuilderBehaviorModuleData.MaxNumSegments), "5"), &objT->MaxNumSegments, state);
        Marshal(node.GetAttributeValue(nameof(WallBuilderBehaviorModuleData.SecondaryModelConditions), "WALL_SEGMENT"), &objT->SecondaryModelConditions, state);
        Marshal(node.GetAttributeValue(nameof(WallBuilderBehaviorModuleData.SecondaryObjectStatus), "WALL_SEGMENT"), &objT->SecondaryObjectStatus, state);
        Marshal(node.GetChildNode(nameof(WallBuilderBehaviorModuleData.ConnectingObjectFilter), null), &objT->ConnectingObjectFilter, state);
        Marshal(node.GetAttributeValue(nameof(WallBuilderBehaviorModuleData.StartInSecondaryModel), "false"), &objT->StartInSecondaryModel, state);
        Marshal(node, (BehaviorModuleData*)objT, state);
    }
}
