using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, AttributeModifierListType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AttributeModifierListType.Type), null), &objT->Type, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifierListType.Value), "0"), &objT->Value, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, AttributeModifier* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.Category), null), &objT->Category, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.Duration), "0s"), &objT->Duration, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.ReplaceInCategoryIfLongest), "false"), &objT->ReplaceInCategoryIfLongest, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.IgnoreIfAnticategoryActive), "false"), &objT->IgnoreIfAnticategoryActive, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.StartFX), null), &objT->StartFX, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.EndFX), null), &objT->EndFX, state);
        // Reborn: absent optional masks remain null; fabricating empty values allocates 120 bytes absent from real EP1 native roots.
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.ModelConditionsSet), null), &objT->ModelConditionsSet, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.ModelConditionsClear), null), &objT->ModelConditionsClear, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.ObjectStatusToSet), null), &objT->ObjectStatusToSet, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.StackingLimit), "1"), &objT->StackingLimit, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.ArmorSetType), null), &objT->ArmorSetType, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifier.Shader), null), &objT->Shader, state);
        Marshal(node.GetChildNodes(nameof(AttributeModifier.Modifier)), &objT->Modifier, state);
        Marshal(node, (BaseInheritableAsset*)objT, state);
    }
}
