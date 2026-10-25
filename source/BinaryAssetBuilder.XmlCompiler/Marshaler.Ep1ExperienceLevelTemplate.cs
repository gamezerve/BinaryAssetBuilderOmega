using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile EP1 experience levels with optional condition/prerequisite storage and real weak/strong reference lists. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1ExperienceLevelTemplate* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.Rank), "0"), &objT->Rank, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.RequiredExperience), "1"), &objT->RequiredExperience, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.ExperienceAward), "0"), &objT->ExperienceAward, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.ExperienceAwardOwnGuysDie), "0"), &objT->ExperienceAwardOwnGuysDie, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.Prerequisite), null), &objT->Prerequisite, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.LevelUpOCL), null), &objT->LevelUpOCL, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.ModelConditionState), null), &objT->ModelConditionState, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.InformUpdateModule), "false"), &objT->InformUpdateModule, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.SinglePlayerOnly), "false"), &objT->SinglePlayerOnly, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.MultiPlayerOnly), "false"), &objT->MultiPlayerOnly, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.ShowLevelUpTint), "false"), &objT->ShowLevelUpTint, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.LevelUpTintPreColorTime), "0.0s"), &objT->LevelUpTintPreColorTime, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.LevelUpTintPostColorTime), "0.0s"), &objT->LevelUpTintPostColorTime, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.LevelUpTintSustainColorTime), "0.0s"), &objT->LevelUpTintSustainColorTime, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.LevelUpTintFrequency), "0.0"), &objT->LevelUpTintFrequency, state);
        Marshal(node.GetAttributeValue(nameof(Ep1ExperienceLevelTemplate.LevelUpTintAmplitude), "0.0"), &objT->LevelUpTintAmplitude, state);
        Marshal(node.GetChildNode(nameof(Ep1ExperienceLevelTemplate.LevelUpTintColor), null), &objT->LevelUpTintColor, state);
        Marshal(node.GetChildNodes(nameof(Ep1ExperienceLevelTemplate.Upgrade)), &objT->Upgrade, state);
        Marshal(node.GetChildNodes(nameof(Ep1ExperienceLevelTemplate.LevelUpFxList)), &objT->LevelUpFxList, state);
        Marshal(node.GetChildNodes(nameof(Ep1ExperienceLevelTemplate.Target)), &objT->Target, state);
        Marshal(node.GetChildNodes(nameof(Ep1ExperienceLevelTemplate.AttributeModifier)), &objT->AttributeModifier, state);
        // Reborn: EP1 decal components have a different ABI and are rejected explicitly rather than written with the KW decal layout.
        if(node.GetChildNode("Decal",null)!=null)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 experience decal components are not implemented.");
        Marshal(node, (BaseAssetType*)objT, state);
    }
}
