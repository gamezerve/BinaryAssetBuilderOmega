using System;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile EP1 special-power target/tech fields and real EVA references without the KW string/enum layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1SpecialPowerTemplate* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.DisplayName), null), &objT->DisplayName, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.Description), null), &objT->Description, state);
        Ep1ToggleEnum(node.GetAttributeValue("TargetType",null),(uint*)&objT->TargetType,state,typeof(Ep1SpecialPowerTemplateTargetType),false);
        Marshal(node.GetAttributeValue("NumTargets","1"),&objT->NumTargets,state);
        Marshal(node.GetAttributeValue("Flags",null),&objT->Flags,state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.ReloadTime), null), &objT->ReloadTime, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.InitiateSound), null), &objT->InitiateSound, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.InitiateAtLocationSound), null), &objT->InitiateAtLocationSound, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.ViewObjectDuration), null), &objT->ViewObjectDuration, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.ViewObjectRange), null), &objT->ViewObjectRange, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.RadiusCursorRadius), null), &objT->RadiusCursorRadius, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.PreventConditions), null), &objT->PreventConditions, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.RequiredConditions), null), &objT->RequiredConditions, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.DisallowedTargetModelStates), null), &objT->DisallowedTargetModelStates, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.RequiredTargetObjectStatus), null), &objT->RequiredTargetObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.DisallowedTargetObjectStatus), null), &objT->DisallowedTargetObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.MaxCastRange), "1000000"), &objT->MaxCastRange, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.MinCastRange), null), &objT->MinCastRange, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.ForbiddenObjectRange), null), &objT->ForbiddenObjectRange, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.EvaEventToPlayOnSuccess), null), &objT->EvaEventToPlayOnSuccess, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.EvaEventToPlayWhenSelectingTarget), null), &objT->EvaEventToPlayWhenSelectingTarget, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.EvaEventToPlayOnInitiateOwner), null), &objT->EvaEventToPlayOnInitiateOwner, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.EvaEventToPlayOnInitiateAlly), null), &objT->EvaEventToPlayOnInitiateAlly, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.EvaEventToPlayOnInitiateEnemy), null), &objT->EvaEventToPlayOnInitiateEnemy, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsInitiateIntendToDoVoice), null), &objT->NameOfVoiceNameToUseAsInitiateIntendToDoVoice, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveVoice), null), &objT->NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveVoice, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsInitiateIntendToDoWhileUnderAttackVoice), null), &objT->NameOfVoiceNameToUseAsInitiateIntendToDoWhileUnderAttackVoice, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveWhileUnderAttackVoice), null), &objT->NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveWhileUnderAttackVoice, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoVoice), null), &objT->NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoVoice, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveVoice), null), &objT->NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveVoice, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoWhileUnderAttackVoice), null), &objT->NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoWhileUnderAttackVoice, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveWhileUnderAttackVoice), null), &objT->NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveWhileUnderAttackVoice, state);
        string restriction=node.GetAttributeValue("RestrictionType","UNRESTRICTED").GetText();uint ordinal=restriction=="UNRESTRICTED"?0u:restriction=="1"?1u:restriction=="2"?2u:throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Unknown EP1 special-power restriction.");state.InplaceEndianToPlatform(&ordinal);objT->RestrictionType=(SpecialPowerRestrictionType)ordinal;
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.Money), "0"), &objT->Money, state);
        Ep1AIBool(node.GetAttributeValue("WaypointModeTerminal","true"),&objT->WaypointModeTerminal);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.TimerImage), null), &objT->TimerImage, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SpecialPowerTemplate.ReachableTargetsCircleDisplayRadius), null), &objT->ReachableTargetsCircleDisplayRadius, state);
        Marshal(node.GetAttributeValue("RequiredPlayerTech",null),&objT->RequiredPlayerTech,state);
        Marshal(node.GetChildNodes("ForbiddenPlayerTech"),&objT->ForbiddenPlayerTech,state);
        Marshal(node.GetChildNode(nameof(Ep1SpecialPowerTemplate.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node.GetChildNode(nameof(Ep1SpecialPowerTemplate.ForbiddenObjectFilter), null), &objT->ForbiddenObjectFilter, state);
        Marshal(node.GetChildNode(nameof(Ep1SpecialPowerTemplate.GameDependency), null), &objT->GameDependency, state);
        Marshal(node, (BaseInheritableAsset*)objT, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source flag names map to exact EP1 bit positions; unknown names never disappear silently. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1SpecialPowerTemplateBitFlag* objT,Tracker state)
    {
        if(value is null)return;uint result=0;string[] names=Enum.GetNames(typeof(Ep1SpecialPowerTemplateFlag));
        foreach(string raw in value.GetText().Split((char[])null,StringSplitOptions.RemoveEmptyEntries)){bool add=raw[0]!='-';string name=raw[0]=='+'||raw[0]=='-'?raw.Substring(1):raw;if(name=="ALL"){result=add?uint.MaxValue:0;continue;}int bit=Array.IndexOf(names,name);if(bit<0)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Unknown EP1 special-power flag: {0}",name);if(add)result|=1u<<bit;else result&=~(1u<<bit);}
        state.InplaceEndianToPlatform(&result);objT->Value[0]=result;
    }
}
