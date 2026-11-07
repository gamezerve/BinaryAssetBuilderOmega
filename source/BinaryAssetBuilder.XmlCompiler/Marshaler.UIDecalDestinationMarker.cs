using System;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the default marker and its ordered destination-specific render settings. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIDecalDestinationMarker* target, Tracker state)
    {
        Marshal(node.GetChildNode(nameof(UIDecalDestinationMarker.DefaultRenderInfo), null), &target->DefaultRenderInfo, state);
        Marshal(node.GetChildNodes(nameof(UIDecalDestinationMarker.DestinationTypeSettings)), &target->DestinationTypeSettings, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the destination enum followed by its inline render info. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIDecalDestinationMarkerDestinationTypeSettings* target, Tracker state)
    {
        target->DestinationType = (DestinationType)Enum.Parse(typeof(DestinationType), node.GetAttributeValue(nameof(UIDecalDestinationMarkerDestinationTypeSettings.DestinationType), null).GetText());
        state.InplaceEndianToPlatform((uint*)&target->DestinationType);
        Marshal(node.GetChildNode(nameof(UIDecalDestinationMarkerDestinationTypeSettings.RenderInfo), null), &target->RenderInfo, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: link the schema-resolved W3D model and serialize its bias and color. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node, UIDecalDestinationMarkerRenderInfo* target, Tracker state)
    {
        Marshal(node.GetAttributeValue(nameof(UIDecalDestinationMarkerRenderInfo.Model), null), &target->Model, state);
        Marshal(node.GetAttributeValue(nameof(UIDecalDestinationMarkerRenderInfo.ZBias), "0"), &target->ZBias, state);
        Marshal(node.GetChildNode(nameof(UIDecalDestinationMarkerRenderInfo.Color), null), &target->Color, state);
    }
}
