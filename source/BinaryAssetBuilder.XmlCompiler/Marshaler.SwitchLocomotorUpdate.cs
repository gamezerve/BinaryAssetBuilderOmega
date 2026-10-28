using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the complete source-driven EP1 locomotor switch settings and both inline masks with official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,SwitchLocomotorUpdate* objT,Tracker state)
    {
        if(node is null)return;
        // Reborn: encode the original EP1 enum explicitly, including ATTACKING and SPECIALABILITY_ACTIVE, without changing KW dispatch.
        var locomotor=node.GetAttributeValue("LocomotorSet",null);
        if(locomotor is not null){
            string name=locomotor.GetText();
            if(!System.Enum.IsDefined(typeof(Ep1RootLocomotorSetType),name))throw new System.IO.InvalidDataException("Unknown EP1 switch locomotor set: "+name);
            uint encoded=unchecked((uint)(Ep1RootLocomotorSetType)System.Enum.Parse(typeof(Ep1RootLocomotorSetType),name));state.InplaceEndianToPlatform(&encoded);*(uint*)&objT->LocomotorSet=encoded;
        }
        Marshal(node.GetAttributeValue("UpdatePeriod","1.0s"),&objT->UpdatePeriod,state);
        Marshal(node.GetAttributeValue("RevealShroudToAllPlayersRadius","0"),&objT->RevealShroudToAllPlayersRadius,state);
        Marshal(node.GetAttributeValue("MinimumHealth","0%"),&objT->MinimumHealth,state);
        Marshal(node.GetAttributeValue("ObjectStatusWhileActive",null),&objT->ObjectStatusWhileActive,state);
        Marshal(node.GetAttributeValue("ModelConditionWhileActive",null),&objT->ModelConditionWhileActive,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
