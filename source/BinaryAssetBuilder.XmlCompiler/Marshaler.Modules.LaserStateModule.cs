using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit laser particle lists, optional pointed-to records and the EP1 weapon requirement. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, LaserStateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LaserStateModuleData.LaserId), "0"), &objT->LaserId, state);
        Marshal(node.GetAttributeValue(nameof(LaserStateModuleData.OriginBoneName), null), &objT->OriginBoneName, state);
        Marshal(node.GetChildNodes(nameof(LaserStateModuleData.LaserEndParticleSystem)), &objT->LaserEndParticleSystem, state);
        Marshal(node.GetChildNodes(nameof(LaserStateModuleData.LaserStartParticleSystem)), &objT->LaserStartParticleSystem, state);
        Marshal(node.GetChildNode(nameof(LaserStateModuleData.EndOffset), null), &objT->EndOffset, state);
        Marshal(node.GetChildNode(nameof(LaserStateModuleData.ObjectStatusValidation), null), &objT->ObjectStatusValidation, state);
        Marshal(node.GetAttributeValue(nameof(LaserStateModuleData.RequiresWeapon), "true"), &objT->RequiresWeapon, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
