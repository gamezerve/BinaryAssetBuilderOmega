using System;
using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 AssembleStructureUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AssembleStructureUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1ProduceStructureUnitAbilityButtonBaseTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 BaseSpecialPowerUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1BaseSpecialPowerUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1UnitAbilityButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 BuildUnitUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1BuildUnitUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1UnitAbilityButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 BuildWallUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1BuildWallUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1SingleStateUnitAbilityButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 EvacuateUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EvacuateUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1SingleStateUnitAbilityButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 MultiplePowersTargetedSpecialPowerUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MultiplePowersTargetedSpecialPowerUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1TargetedSpecialPowerUnitAbilityButtonTemplateData*)target,state); Marshal(node.GetChildNodes("AdditionalSpecialPowerTemplate"),&target->AdditionalSpecialPowerTemplate,state); Marshal(node.GetChildNodes("AdditionalRadiusCursor"),&target->AdditionalRadiusCursor,state); Marshal(node.GetChildNodes("AdditionalValidTargetCursor"),&target->AdditionalValidTargetCursor,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 ObjectUpgradeUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ObjectUpgradeUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1SingleStateUnitAbilityButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 PlayerUpgradeUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerUpgradeUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1SingleStateUnitAbilityButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 ProduceStructureUnitAbilityButtonBaseTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ProduceStructureUnitAbilityButtonBaseTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1SpecialPowerUnitAbilityButtonTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 SingleStateUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SingleStateUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1UnitAbilityButtonTemplateData*)target,state); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 SpecialPowerUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SpecialPowerUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1BaseSpecialPowerUnitAbilityButtonTemplateData*)target,state); Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 TargetedSpecialPowerUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TargetedSpecialPowerUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1SpecialPowerUnitAbilityButtonTemplateData*)target,state); Marshal(node.GetAttributeValue("ValidTargetCursor","Arrow"),&target->ValidTargetCursor,state); Marshal(node.GetAttributeValue("InvalidTargetCursor","Invalid"),&target->InvalidTargetCursor,state); Marshal(node.GetAttributeValue("RadiusCursor",null),&target->RadiusCursor,state); target->ValidTargetOutline=(Ep1ButtonOutline)Enum.Parse(typeof(Ep1ButtonOutline),node.GetAttributeValue("ValidTargetOutline","HOVER_ATTACK").GetText(),false);state.InplaceEndianToPlatform((uint*)&target->ValidTargetOutline); var tint=node.GetChildNode("ValidTargetTintColor",null);if(tint!=null){using var allocation=state.Push((void**)&target->ValidTargetTintColor,(uint)sizeof(RGBColor),1u);Marshal(tint,target->ValidTargetTintColor,state);}}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 ToggleSpecialPowerUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ToggleSpecialPowerUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1BaseSpecialPowerUnitAbilityButtonTemplateData*)target,state); Marshal(node.GetChildNodes("StateData"),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 UnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 UnpackUnitAbilityButtonTemplateData record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UnpackUnitAbilityButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1ProduceStructureUnitAbilityButtonBaseTemplateData*)target,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: dispatch every official EP1 unit-button Data alternative using its original class identity. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UnitAbilityButtonTemplate* target,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("LogicCommand",null),&target->LogicCommand,state);var data=node.GetChildNode("Data",null);Node child;
        child=data.GetChildNode("AssembleStructure",null);if(child!=null){MarshalEp1ButtonData<Ep1AssembleStructureUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("BuildUnit",null);if(child!=null){MarshalEp1ButtonData<Ep1BuildUnitUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("BuildWall",null);if(child!=null){MarshalEp1ButtonData<Ep1BuildWallUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("Evacuate",null);if(child!=null){MarshalEp1ButtonData<Ep1EvacuateUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("MultiplePowersTargetedSpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1MultiplePowersTargetedSpecialPowerUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("ObjectUpgrade",null);if(child!=null){MarshalEp1ButtonData<Ep1ObjectUpgradeUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("PlayerUpgrade",null);if(child!=null){MarshalEp1ButtonData<Ep1PlayerUpgradeUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("SpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1SpecialPowerUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("TargetedSpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1TargetedSpecialPowerUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("ToggleSpecialPower",null);if(child!=null){MarshalEp1ButtonData<Ep1ToggleSpecialPowerUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("Unpack",null);if(child!=null){MarshalEp1ButtonData<Ep1UnpackUnitAbilityButtonTemplateData,Ep1UnitAbilityButtonTemplateData>(child,&target->Data,state);return;}
        throw new InvalidOperationException("UnitAbilityButtonTemplate requires one concrete Data child.");
    }
}
