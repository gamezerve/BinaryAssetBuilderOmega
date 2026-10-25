using System;
using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateData. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateData* target,Tracker state)
    {if(node==null)return;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataAIStrategicDirective. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataAIStrategicDirective* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state); target->Directive=(Ep1JoypadAIStrategicDirective)Enum.Parse(typeof(Ep1JoypadAIStrategicDirective),node.GetAttributeValue("Directive",null).GetText(),false);state.InplaceEndianToPlatform((uint*)&target->Directive);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataActivateHotSpot. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataActivateHotSpot* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataAssembleStructure. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataAssembleStructure* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataProduceStructureBase*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataAutoControlGroup. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataAutoControlGroup* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state); Marshal(node.GetChildNode("ObjectFilter",null),&target->ObjectFilter,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataBaseSpecialPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataBaseSpecialPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataUnitAbility*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataEnterCommandMode. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataEnterCommandMode* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state); target->CommandMode=(Ep1JoypadCommandMode)Enum.Parse(typeof(Ep1JoypadCommandMode),node.GetAttributeValue("CommandMode",null).GetText(),false);state.InplaceEndianToPlatform((uint*)&target->CommandMode);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataEvacuate. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataEvacuate* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataUnitAbility*)target,state); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataMultiplePowersTargetedSpecialPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataMultiplePowersTargetedSpecialPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataTargetedSpecialPower*)target,state); Marshal(node.GetChildNodes("AdditionalSpecialPowerTemplate"),&target->AdditionalSpecialPowerTemplate,state); Marshal(node.GetChildNodes("AdditionalRadiusCursor"),&target->AdditionalRadiusCursor,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataObjectUpgrade. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataObjectUpgrade* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataUpgrade*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenControlGroups. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenControlGroups* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenCoopCommunicator. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenCoopCommunicator* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenPlayerPowers. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenPlayerPowers* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenPlayerTechStore. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenPlayerTechStore* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenStances. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenStances* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateData*)target,state); Marshal(node.GetAttributeValue("MixedStanceStateData",null),&target->MixedStanceStateData,state); Marshal(node.GetChildNodes("StanceSettings"),&target->StanceSettings,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenStancesStanceSettings. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenStancesStanceSettings* target,Tracker state)
    {if(node==null)return;target->Stance=(Ep1JoypadStance)Enum.Parse(typeof(Ep1JoypadStance),node.GetAttributeValue("Stance",null).GetText(),false);state.InplaceEndianToPlatform((uint*)&target->Stance); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenUnitAbilities. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenUnitAbilities* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataOpenUnitsMenu. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataOpenUnitsMenu* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataPlaceReadyStructure. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataPlaceReadyStructure* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataPlayerControlGroup. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataPlayerControlGroup* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state); Marshal(node.GetAttributeValue("GroupShortcut","1"),&target->GroupShortcut,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataPlayerMultiplePowersTargetedPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataPlayerMultiplePowersTargetedPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataPlayerTargetedPower*)target,state); Marshal(node.GetChildNodes("AdditionalSpecialPowerTemplate"),&target->AdditionalSpecialPowerTemplate,state); Marshal(node.GetChildNodes("AdditionalRadiusCursor"),&target->AdditionalRadiusCursor,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataPlayerMultipleTargetsTargetedPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataPlayerMultipleTargetsTargetedPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataPlayerTargetedPower*)target,state); Marshal(node.GetAttributeValue("TargetMarkerTemplate",null),&target->TargetMarkerTemplate,state); Marshal(node.GetAttributeValue("TargetMarkerFXList",null),&target->TargetMarkerFXList,state); Marshal(node.GetAttributeValue("TargetCount","1"),&target->TargetCount,state); Marshal(node.GetAttributeValue("MinTargetDistance","0.0"),&target->MinTargetDistance,state); Marshal(node.GetChildNodes("AdditionalRadiusCursor"),&target->AdditionalRadiusCursor,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataPlayerPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataPlayerPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateData*)target,state); Marshal(node.GetAttributeValue("SpecialPower",null),&target->SpecialPower,state); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataPlayerTargetedPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataPlayerTargetedPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataPlayerPower*)target,state); Marshal(node.GetAttributeValue("RadiusCursor",null),&target->RadiusCursor,state); target->ValidTargetOutline=(Ep1JoypadOutlineDrawType)Enum.Parse(typeof(Ep1JoypadOutlineDrawType),node.GetAttributeValue("ValidTargetOutline","HOVER_ATTACK").GetText(),false);state.InplaceEndianToPlatform((uint*)&target->ValidTargetOutline); var tint=node.GetChildNode("ValidTargetTintColor",null);if(tint!=null){using var allocation=state.Push((void**)&target->ValidTargetTintColor,(uint)sizeof(RGBColor),1u);Marshal(tint,target->ValidTargetTintColor,state);}}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataPlayerUpgrade. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataPlayerUpgrade* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataUpgrade*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataProduceStructureBase. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataProduceStructureBase* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSpecialPower*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataProductionQueueType. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataProductionQueueType* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state); target->QueueType=(Ep1JoypadProductionQueueType)Enum.Parse(typeof(Ep1JoypadProductionQueueType),node.GetAttributeValue("QueueType",null).GetText(),false);state.InplaceEndianToPlatform((uint*)&target->QueueType);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataRedAlert. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataRedAlert* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataSetStance. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataSetStance* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSingleState*)target,state); target->Stance=(Ep1JoypadStance)Enum.Parse(typeof(Ep1JoypadStance),node.GetAttributeValue("Stance",null).GetText(),false);state.InplaceEndianToPlatform((uint*)&target->Stance);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataSingleState. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataSingleState* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateData*)target,state); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataSpecialPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataSpecialPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataBaseSpecialPower*)target,state); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataTargetedSpecialPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataTargetedSpecialPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataSpecialPower*)target,state); Marshal(node.GetAttributeValue("RadiusCursor",null),&target->RadiusCursor,state); target->ValidTargetOutline=(Ep1JoypadOutlineDrawType)Enum.Parse(typeof(Ep1JoypadOutlineDrawType),node.GetAttributeValue("ValidTargetOutline","HOVER_ATTACK").GetText(),false);state.InplaceEndianToPlatform((uint*)&target->ValidTargetOutline); var tint=node.GetChildNode("ValidTargetTintColor",null);if(tint!=null){using var allocation=state.Push((void**)&target->ValidTargetTintColor,(uint)sizeof(RGBColor),1u);Marshal(tint,target->ValidTargetTintColor,state);}}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataToggleSpecialPower. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataToggleSpecialPower* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataBaseSpecialPower*)target,state); Marshal(node.GetChildNodes("StateData"),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataUnitAbility. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataUnitAbility* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateData*)target,state); Marshal(node.GetAttributeValue("LogicCommand",null),&target->LogicCommand,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataUnpack. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataUnpack* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataProduceStructureBase*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all source-owned fields of JoypadCommandBarButtonTemplateDataUpgrade. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplateDataUpgrade* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1JoypadCommandBarButtonTemplateDataUnitAbility*)target,state); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: select only the concrete alternatives declared by the official EP1 button schema. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1JoypadCommandBarButtonTemplate* target,Tracker state)
    {if(node==null)return;var data=node.GetChildNode("Data",null);Node child;
        child=data.GetChildNode("ActivateHotSpot",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataActivateHotSpot,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("AIStrategicDirective",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataAIStrategicDirective,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("AssembleStructure",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataAssembleStructure,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("AutoControlGroup",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataAutoControlGroup,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("EnterCommandMode",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataEnterCommandMode,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("Evacuate",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataEvacuate,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("MultiplePowersTargetedSpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataMultiplePowersTargetedSpecialPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("ObjectUpgrade",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataObjectUpgrade,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("OpenCoopCommunicator",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataOpenCoopCommunicator,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("OpenControlGroups",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataOpenControlGroups,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("OpenPlayerPowers",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataOpenPlayerPowers,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("OpenPlayerTechStore",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataOpenPlayerTechStore,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("OpenStances",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataOpenStances,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("OpenUnitAbilities",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataOpenUnitAbilities,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("OpenUnitsMenu",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataOpenUnitsMenu,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlaceReadyStructure",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataPlaceReadyStructure,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlayerControlGroup",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataPlayerControlGroup,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlayerPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataPlayerPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlayerMultiplePowersTargetedPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataPlayerMultiplePowersTargetedPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlayerMultipleTargetsTargetedPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataPlayerMultipleTargetsTargetedPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlayerTargetedPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataPlayerTargetedPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlayerUpgrade",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataPlayerUpgrade,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("ProductionQueueType",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataProductionQueueType,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("RedAlert",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataRedAlert,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("SetStance",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataSetStance,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("SpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataSpecialPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("TargetedSpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataTargetedSpecialPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("ToggleSpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataToggleSpecialPower,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("Unpack",null);if(child!=null){MarshalEp1ButtonData<Ep1JoypadCommandBarButtonTemplateDataUnpack,Ep1JoypadCommandBarButtonTemplateData>(child,&target->Data,state);return;}
        throw new InvalidOperationException("JoypadCommandBarButtonTemplate requires one concrete Data child.");
    }
}
