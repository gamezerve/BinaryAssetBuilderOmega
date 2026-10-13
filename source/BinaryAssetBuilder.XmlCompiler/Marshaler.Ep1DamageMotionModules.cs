using Relo;
using SageBinaryData;
using System;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain typed modifier imports, two named option bits and packed bool defaults in the sixteen-byte effect record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, EffectInfoType* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(EffectInfoType.Fraction), null), &objT->Fraction, state);
        Marshal(node.GetAttributeValue(nameof(EffectInfoType.AttributeModifier), null), &objT->AttributeModifier, state);
        uint flags = 0;
        string text = node.GetAttributeValue(nameof(EffectInfoType.Options), "")?.GetText() ?? "";
        foreach (string token in text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!Enum.TryParse(token, out EffectInfoFlagType value) || !Enum.IsDefined(typeof(EffectInfoFlagType), value) || token != value.ToString())
                throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Unknown EP1 effect option '{0}'.", token);
            flags |= 1u << (int)value;
        }
        state.InplaceEndianToPlatform(&flags); objT->Options = flags;
        Marshal(node.GetAttributeValue(nameof(EffectInfoType.OnEnterDisable), "false"), &objT->OnEnterDisable, state);
        Marshal(node.GetAttributeValue(nameof(EffectInfoType.OnEnterCancelSpecialAbilities), "false"), &objT->OnEnterCancelSpecialAbilities, state);
        Marshal(node.GetAttributeValue(nameof(EffectInfoType.SetUpgradedArmor), "false"), &objT->SetUpgradedArmor, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal tracker color, ordered effects/imports and optional existing status-mask payload without treating optional children as embedded fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, SecondaryDamageTrackerModuleData* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(SecondaryDamageTrackerModuleData.UpdateDuration), "1s"), &objT->UpdateDuration, state);
        Marshal(node.GetAttributeValue(nameof(SecondaryDamageTrackerModuleData.DissipateAmount), "0"), &objT->DissipateAmount, state);
        Marshal(node.GetAttributeValue(nameof(SecondaryDamageTrackerModuleData.MaxDamageClampScaler), "125%"), &objT->MaxDamageClampScaler, state);
        var color = node.GetChildNode(nameof(SecondaryDamageTrackerModuleData.Color), null);
        if (color != null)
        {
            using Tracker.Context context = state.Push((void**)&objT->Color, (uint)sizeof(RGBColor), 1);
            Marshal(color, objT->Color, state);
        }
        Marshal(node.GetChildNodes(nameof(SecondaryDamageTrackerModuleData.EffectInfo)), &objT->EffectInfo, state);
        Marshal(node.GetChildNode(nameof(SecondaryDamageTrackerModuleData.ObjectStatusValidation), null), &objT->ObjectStatusValidation, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the stock inline condition mask and literal real-valued freefall parameters. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, FreefallUpdateModuleData* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(FreefallUpdateModuleData.ModelConditionsToSet), null), &objT->ModelConditionsToSet, state);
        Marshal(node.GetAttributeValue(nameof(FreefallUpdateModuleData.InitialVelocity), "0"), &objT->InitialVelocity, state);
        Marshal(node.GetAttributeValue(nameof(FreefallUpdateModuleData.Acceleration), "1"), &objT->Acceleration, state);
        Marshal(node.GetAttributeValue(nameof(FreefallUpdateModuleData.TerminalVelocity), "15"), &objT->TerminalVelocity, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
