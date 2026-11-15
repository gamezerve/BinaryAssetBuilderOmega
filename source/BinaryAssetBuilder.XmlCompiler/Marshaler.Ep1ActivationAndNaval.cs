using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal up to four local activation rows with their original order, Poid and trigger defaults. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1ActivateModuleSpecialAbilityUpdateModuleData* result,Tracker state)
    {
        var rows=node.GetChildNodes("SpecialPowerModules");
        if(rows!=null&&rows.GetCount()>4)
            throw new System.IO.InvalidDataException("EP1 activation permits at most four SpecialPowerModules rows.");
        Marshal(node,&result->Base,state);
        Marshal(rows,&result->SpecialPowerModules,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the optional naval modifier import at +0x08. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1AttributeModifierNavalModuleData* result,Tracker state)
    {
        Marshal(node,&result->Base,state);
        Marshal(node.GetAttributeValue("AttributeModifier",null),&result->AttributeModifier,state);
    }
}
