using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 radar image using the linked texture reference and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1RadarTerrainImage* root, Tracker state)
    {
        Marshal(node.GetAttributeValue("MapBorder", "Orange"), &root->MapBorder, state);
        Marshal(node.GetAttributeValue("Texture", null), &root->Texture, state);
        Marshal(node.GetChildNode("ImageSize", null), &root->ImageSize, state);
        Marshal(node.GetChildNode("ImagePosition", null), &root->ImagePosition, state);
        Marshal(node.GetChildNode("TextureSize", null), &root->TextureSize, state);
        Marshal(node.GetAttributeValue("Rotated", "false"), &root->Rotated, state);
    }
}
