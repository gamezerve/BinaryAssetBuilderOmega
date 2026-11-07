using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original EP1 radius component and schema defaults, retaining real texture occurrences. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1RadiusDecalComponent* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.Texture),null),&target->Texture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.Texture2),null),&target->Texture2,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.Style),null),&target->Style,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.OpacityMin),"1.0"),&target->OpacityMin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.OpacityMax),"1.0"),&target->OpacityMax,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.OpacityPulsesPerSecond),"0.0"),&target->OpacityPulsesPerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.RotationMin),"0.0"),&target->RotationMin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.RotationMax),"0.0"),&target->RotationMax,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.RotationDegreesPerSecond),"0.0"),&target->RotationDegreesPerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.MinRadius),"1.0"),&target->MinRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.MaxRadius),"1.0"),&target->MaxRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.RadiusPulsesPerSecond),"0.0"),&target->RadiusPulsesPerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.PositionOffsetMin),"0.0"),&target->PositionOffsetMin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.PositionOffsetMax),"0.0"),&target->PositionOffsetMax,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RadiusDecalComponent.PositionOffsetPulsesPerSecond),"0.0"),&target->PositionOffsetPulsesPerSecond,state);
        Marshal(node.GetChildNode(nameof(Ep1RadiusDecalComponent.Color),null),&target->Color,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve every authored EP1 radius component in list order without a fixed component limit. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1RadiusDecalTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1RadiusDecalTemplate.Component)),&target->Component,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the standalone cursor asset with its complete source-sized EP1 decal template. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1RadiusCursor* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1RadiusCursor.DecalTemplate),null),&target->DecalTemplate,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
