using System;
using System.Globalization;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate the separate EP1 concrete model while retaining AIUpdateModuleData's original wire identity. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1AIUpdate(Node node,UpdateModuleData** objT,Tracker state)
    {
        if(node is null || node.GetAttributeValue("TypeId",null)?.GetText()!="148619278")throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Explicit EP1 AIUpdate class ID required.");
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1AIUpdateModuleData),1u))Marshal(node,(Ep1AIUpdateModuleData*)*objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve official AI source/default semantics in the reviewed 76-byte record; removed KW fields cannot shift EP1 state/chooser words. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(Ep1AIAscii(node.GetAttributeValue("AttackPriority",null)),&objT->AttackPriority,state);
        Ep1ToggleEnum(node.GetAttributeValue("AutoAcquireEnemiesWhenIdle",null),&objT->AutoAcquireEnemiesWhenIdle,state,typeof(AutoAcquireEnemiesType),true);
        Marshal(node.GetAttributeValue("StopChaseDistance","500.0"),&objT->StopChaseDistance,state);
        Marshal(node.GetAttributeValue("MinCowerTime","0s"),&objT->MinCowerTime,state);
        Marshal(node.GetAttributeValue("MaxCowerTime","0s"),&objT->MaxCowerTime,state);
        Marshal(node.GetAttributeValue("BurningDeathTime",null),&objT->BurningDeathTime,state);
        Marshal(node.GetAttributeValue("ComboLocoAttackDistance","80"),&objT->ComboLocoAttackDistance,state);
        Marshal(Ep1AIAscii(node.GetAttributeValue("SpecialContactPoints",null),true),&objT->SpecialContactPoints,state);
        Marshal(node.GetAttributeValue("IdleTargetTime","0s"),&objT->IdleTargetTime,state);
        Ep1AIUnsigned(node.GetAttributeValue("MaxCombineChildren","1"),&objT->MaxCombineChildren,state);
        Marshal(node.GetAttributeValue("SpawnOffsetRadius","0.0"),&objT->SpawnOffsetRadius,state);
        Ep1ToggleEnum(node.GetAttributeValue("StateMachine","AIStateMachine"),(uint*)&objT->StateMachine,state,typeof(Ep1AIStateMachineType),false);
        // Reborn: use actual signed enum values rather than Enum.GetNames ordering, which sorts the -1 sentinel last.
        string locomotor=node.GetAttributeValue("DefaultLocomotorSet","NORMAL").GetText();if(!Enum.IsDefined(typeof(Ep1RootLocomotorSetType),locomotor))throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Unknown EP1 default locomotor '{0}'.",locomotor);uint condition=unchecked((uint)(Ep1RootLocomotorSetType)Enum.Parse(typeof(Ep1RootLocomotorSetType),locomotor));state.InplaceEndianToPlatform(&condition);*(uint*)&objT->DefaultLocomotorSet=condition;
        Marshal(node.GetChildNode("UnitAITargetChooserData",null),&objT->UnitAITargetChooserData,state);
        Ep1AIBool(node.GetAttributeValue("StandGround","false"),&objT->StandGround);
        Ep1AIBool(node.GetAttributeValue("FadeOnPortals","false"),&objT->FadeOnPortals);
        // Reborn: the string-based schema accepts overflowing exponents; finite native values remain a separate admission requirement.
        foreach(float value in new[]{objT->StopChaseDistance,objT->MinCowerTime.Value,objT->MaxCowerTime.Value,objT->BurningDeathTime.Value,objT->ComboLocoAttackDistance,objT->IdleTargetTime.Value,objT->SpawnOffsetRadius})if(!float.IsFinite(value))throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 AI real/time fields must be finite.");
        Marshal(node,(UpdateModuleData*)objT,state);uint type=0x08DBC00E;state.InplaceEndianToPlatform(&type);*(uint*)objT=type;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the first chooser profile covers all primitive/default/reference fields but explicitly refuses nonempty ObjectVisionRange graphs. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UnitAITargetChooserData* objT,Tracker state)
    {
        if(node is null)return;
        if(node.GetChildNodes("ObjectVisionRange")?.GetCount()>0)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 AI ObjectVisionRange payload is not yet admitted; refusing instead of skipping.");
        Marshal(node.GetAttributeValue("IdleScanDelay","0.8s"),&objT->IdleScanDelay,state);
        Marshal(node.GetAttributeValue("ReacquireDelay","60s"),&objT->ReacquireDelay,state);
        Marshal(node.GetAttributeValue("StartupDelay","0s"),&objT->StartupDelay,state);
        Marshal(node.GetAttributeValue("SympathyRange","25.0"),&objT->SympathyRange,state);
        Ep1ToggleEnum(node.GetAttributeValue("CanPickDynamicTargetType","ALWAYS"),&objT->CanPickDynamicTargetType,state,typeof(Ep1DynamicTargetFlag),true);
        Ep1AIBool(node.GetAttributeValue("CheckVisionRange","false"),&objT->CheckVisionRange);
        Ep1AIBool(node.GetAttributeValue("CanPickDynamicTargets","true"),&objT->CanPickDynamicTargets);
        Ep1AIBool(node.GetAttributeValue("RotateToTargetWhenAiming","false"),&objT->RotateToTargetWhenAiming);
        Ep1AIBool(node.GetAttributeValue("CanAutoAcquireNonAutoAcquirable","false"),&objT->CanAutoAcquireNonAutoAcquirable);
        Ep1AIBool(node.GetAttributeValue("CanChaseAttack","true"),&objT->CanChaseAttack);
        Ep1AIBool(node.GetAttributeValue("CanOnlyAttackTargetsInFront","false"),&objT->CanOnlyAttackTargetsInFront);
        Marshal(node.GetAttributeValue("TargetingCompareList","DefaultTargetingCompareList"),&objT->TargetingCompareList,state);
        // Reborn: chooser time/scalar exponent overflow cannot become a successful native record.
        foreach(float value in new[]{objT->IdleScanDelay.Value,objT->ReacquireDelay.Value,objT->StartupDelay.Value,objT->SympathyRange})if(!float.IsFinite(value))throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 AI chooser real/time fields must be finite.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: optional chooser records allocate forty bytes without reusing the shorter legacy base structure. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1UnitAITargetChooserData** objT,Tracker state)
    {
        if(node is null)return;
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1UnitAITargetChooserData),1u))Marshal(node,*objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain decimal/hex/0.0 unsigned literals without fractional truncation; unresolved definitions must be prepared before this native seam. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Ep1AIUnsigned(Value value,uint* target,Tracker state)
    {
        string text=value?.GetText()??"0";uint result=text=="0.0"?0u:text.StartsWith("0x",StringComparison.Ordinal)&&text.Length is >=3 and <=10?uint.Parse(text.Substring(2),NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture):uint.Parse(text,NumberStyles.None,CultureInfo.InvariantCulture);state.InplaceEndianToPlatform(&result);*target=result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded ASCII is required before the legacy ANSI allocator; multibyte encoding and larger string lists need their own reviewed path. */
    //-------------------------------------------------------------------------------------------------
    private static Value Ep1AIAscii(Value value,bool list=false)
    {
        string text=value?.GetText();if(text is null)return value;
        if(text.Length>4096)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 AI text exceeds the reviewed 4096-character profile.");
        foreach(char c in text)if(c==0 || c>127)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 AI text currently requires non-NUL ASCII.");
        if(list && text.Split((char[])null,StringSplitOptions.RemoveEmptyEntries).Length>32)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"EP1 AI admits at most 32 special contact points.");return value;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: official SageBool accepts numeric zero/one as well as true/false; avoid the legacy bool.Parse rejection of numeric literals. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Ep1AIBool(Value value,SageBool* target)
    {
        string text=value?.GetText();bool result=text=="1" || string.Equals(text,"true",StringComparison.OrdinalIgnoreCase);
        if(!result && text!="0" && !string.Equals(text,"false",StringComparison.OrdinalIgnoreCase))throw new FormatException("EP1 AI boolean requires true/false/0/1.");target->Value=result;
    }
}
