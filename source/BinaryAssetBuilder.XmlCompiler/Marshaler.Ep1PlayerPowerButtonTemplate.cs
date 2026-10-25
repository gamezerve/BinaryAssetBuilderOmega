using System;
using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the standalone EP1 button root and its schema-selected data variant. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1PlayerPowerButtonTemplate* target, Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("SpecialPower",null),&target->SpecialPower,state);
        var data=node.GetChildNode("Data",null);
        var child=data.GetChildNode("Power",null);if(child!=null){MarshalEp1ButtonData<Ep1PlayerPowerButtonTemplateData,Ep1PlayerPowerButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("TargetedPower",null);if(child!=null){MarshalEp1ButtonData<Ep1TargetedPlayerPowerButtonTemplateData,Ep1PlayerPowerButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("MultipleTargetsTargetedPower",null);if(child!=null){MarshalEp1ButtonData<Ep1MultipleTargetsTargetedPlayerPowerButtonTemplateData,Ep1PlayerPowerButtonTemplateData>(child,&target->Data,state);return;}
        child=data.GetChildNode("MultiplePowersTargetedPower",null);if(child!=null){MarshalEp1ButtonData<Ep1MultiplePowersTargetedPlayerPowerButtonTemplateData,Ep1PlayerPowerButtonTemplateData>(child,&target->Data,state);return;}
        throw new InvalidOperationException("PlayerPowerButtonTemplate requires one concrete Data child.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate each compatibility model, retaining the retail zero class word for the unextended Power variant. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1ButtonData<T,U>(Node node,U** target,Tracker state) where T:unmanaged where U:unmanaged,IPolymorphic
    {MarshalPolymorphicType<T,U>(node,target,state);uint wire=typeof(T)==typeof(Ep1PlayerPowerButtonTemplateData)?0:BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(typeof(T).Name.Substring(3));state.InplaceEndianToPlatform(&wire);*(uint*)*target=wire;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the normalized state-data import; the polymorphic allocator writes the concrete class word. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerPowerButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("StateData",null),&target->StateData,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit targeted cursor imports, explicit outline ordinals and optional tint storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TargetedPlayerPowerButtonTemplateData* target,Tracker state)
    {
        if(node==null)return;Marshal(node,(Ep1PlayerPowerButtonTemplateData*)target,state);
        Marshal(node.GetAttributeValue("ValidTargetCursor","Arrow"),&target->ValidTargetCursor,state);Marshal(node.GetAttributeValue("InvalidTargetCursor","Invalid"),&target->InvalidTargetCursor,state);Marshal(node.GetAttributeValue("RadiusCursor",null),&target->RadiusCursor,state);
        target->ValidTargetOutline=(Ep1ButtonOutline)Enum.Parse(typeof(Ep1ButtonOutline),node.GetAttributeValue("ValidTargetOutline","HOVER_ATTACK").GetText(),false);state.InplaceEndianToPlatform((uint*)&target->ValidTargetOutline);
        var tint=node.GetChildNode("ValidTargetTintColor",null);if(tint!=null){using var allocation=state.Push((void**)&target->ValidTargetTintColor,(uint)sizeof(RGBColor),1u);Marshal(tint,target->ValidTargetTintColor,state);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: multi-target buttons retain weak marker identities and independent targeting defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MultipleTargetsTargetedPlayerPowerButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1TargetedPlayerPowerButtonTemplateData*)target,state);Marshal(node.GetAttributeValue("TargetMarkerTemplate",null),&target->TargetMarkerTemplate,state);Marshal(node.GetAttributeValue("TargetMarkerFXList",null),&target->TargetMarkerFXList,state);Marshal(node.GetAttributeValue("TargetCount","1"),&target->TargetCount,state);Marshal(node.GetAttributeValue("MinTargetDistance","0.0"),&target->MinTargetDistance,state);Marshal(node.GetChildNodes("AdditionalRadiusCursor"),&target->AdditionalRadiusCursor,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: chained power lists use ordinary Core import occurrences in authored order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MultiplePowersTargetedPlayerPowerButtonTemplateData* target,Tracker state)
    {if(node==null)return;Marshal(node,(Ep1TargetedPlayerPowerButtonTemplateData*)target,state);Marshal(node.GetChildNodes("AdditionalSpecialPowerTemplate"),&target->AdditionalSpecialPowerTemplate,state);Marshal(node.GetChildNodes("AdditionalRadiusCursor"),&target->AdditionalRadiusCursor,state);Marshal(node.GetChildNodes("AdditionalValidTargetCursor"),&target->AdditionalValidTargetCursor,state);}
}
