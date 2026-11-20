using System;
using System.IO;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    // Reborn: preserve the original token order from the EP1 readers and corresponding XSD lists.
    private static readonly string[] GameplayConditionNames = { "KILL", "CAPTURE", "UPGRADE" };
    private static readonly string[] GameplayModeNames = { "CAMPAIGN", "SKIRMISH", "ONLINE", "RANKED_ONLINE" };
    private static readonly string[] GameplayDifficultyNames = { "EASY", "NORMAL", "HARD", "BRUTAL" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the condition's defaults, inline primary object and independent optional records. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, GameplayConditionData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        MarshalGameplayMask(node, nameof(GameplayConditionData.GameplayConditionBitFlags), "", GameplayConditionNames, &target->GameplayConditionBitFlags, state);
        MarshalGameplayMask(node, nameof(GameplayConditionData.AllowedGameTypeBitFlags), "SKIRMISH RANKED_ONLINE", GameplayModeNames, &target->AllowedGameTypeBitFlags, state);
        MarshalGameplayMask(node, nameof(GameplayConditionData.GameDifficultyBitFlags), "EASY NORMAL HARD BRUTAL", GameplayDifficultyNames, &target->GameDifficultyBitFlags, state);
        var time = node.GetAttributeValue(nameof(GameplayConditionData.TimeReq), null);
        if (time != null)
        {
            using var allocation = state.Push((void**)&target->TimeReq, (uint)sizeof(Time), 1u);
            Marshal(time, target->TimeReq, state);
        }

        Marshal(node.GetChildNode(nameof(GameplayConditionData.PrimaryObject), null), &target->PrimaryObject, state);
        var objects = node.GetChildNode(nameof(GameplayConditionData.RequiredObjectCondition), null);
        if (objects != null)
        {
            using var allocation = state.Push((void**)&target->RequiredObjectCondition, (uint)sizeof(ObjectConditionData), 1u);
            Marshal(objects, target->RequiredObjectCondition, state);
        }

        var upgrades = node.GetChildNode(nameof(GameplayConditionData.RequiredUpgradeCondition), null);
        if (upgrades != null)
        {
            using var allocation = state.Push((void**)&target->RequiredUpgradeCondition, (uint)sizeof(UpgradeConditionData), 1u);
            Marshal(upgrades, &target->RequiredUpgradeCondition->Base, state);
            Marshal(upgrades.GetChildNodes(nameof(UpgradeConditionData.UpgradeRequired)), &target->RequiredUpgradeCondition->UpgradeRequired, state);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the linked trigger stores the normalized condition import and its own event ID. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, GameplayAchievementTriggerData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        Marshal(node.GetAttributeValue(nameof(GameplayAchievementTriggerData.GameplayConditionData), null), &target->GameplayConditionData, state);
        Marshal(node.GetAttributeValue(nameof(GameplayAchievementTriggerData.AchievementEventId), null), &target->AchievementEventId, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a required or optional object record contains the same base and optional DieMux pointer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ObjectConditionData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        Marshal(node.GetChildNode(nameof(ObjectConditionData.DieMuxData), null), &target->DieMuxData, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the original condition defaults and the shared EP1 filter allocation. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, BaseConditionData* target, Tracker state)
    {
        Marshal(node.GetAttributeValue(nameof(BaseConditionData.Count), "1"), &target->Count, state);
        Marshal(node.GetAttributeValue(nameof(BaseConditionData.Unique), "false"), &target->Unique, state);
        Marshal(node.GetChildNode(nameof(BaseConditionData.ObjectFilter), null), &target->ObjectFilter, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: each reviewed gameplay token sets its native DWORD bit. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalGameplayMask(Node node, string name, string fallback, string[] options, uint* target, Tracker state)
    {
        uint mask = 0;
        foreach (string token in node.GetAttributeValue(name, fallback).GetText().Split(WhiteSpaces, StringSplitOptions.RemoveEmptyEntries))
        {
            int index = Array.IndexOf(options, token);
            if (index < 0)
                throw new InvalidDataException("Invalid EP1 gameplay flag " + name + ": " + token);
            mask |= 1u << index;
        }

        state.InplaceEndianToPlatform(&mask);
        *target = mask;
    }
}
