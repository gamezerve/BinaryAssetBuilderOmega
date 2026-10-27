using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete EP1 assembly ability using the official defaults and weak GameObject template IDs. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,AssembleStructureSpecialAbilityUpdateModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("NewObjectAssemblyTime","0s"),&value->NewObjectAssemblyTime,state);
        Marshal(node.GetAttributeValue("ModelConditionsOnAssembly","INVALID"),&value->ModelConditionsOnAssembly,state);
        Marshal(node.GetChildNodes("StructureTemplate"),&value->StructureTemplate,state);
        Marshal(node.GetAttributeValue("BusyDuringAssembly","false"),&value->BusyDuringAssembly,state);
        Marshal(node,(SpecialAbilityUpdateModuleData*)value,state);
    }
}
