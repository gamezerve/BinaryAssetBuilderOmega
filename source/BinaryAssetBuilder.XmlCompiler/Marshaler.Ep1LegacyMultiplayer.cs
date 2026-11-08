using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original ConquerCardDefinitionBase fields in the independently observed PC layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ConquerCardDefinitionBase* value, Tracker state)
    {
        if(node is null)return;
        Marshal(node, &value->Base, state);
        Marshal(node.GetAttributeValue("Cost", null), &value->Cost, state);
        Marshal(node.GetAttributeValue("Maximum", "0"), &value->Maximum, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original ConquerCardUpgrade fields in the independently observed PC layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ConquerCardUpgrade* value, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Upgrade", null), &value->Upgrade, state);
        Marshal(node.GetAttributeValue("Cost", null), &value->Cost, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original ConquerCardVeterancy fields in the independently observed PC layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ConquerCardVeterancy* value, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Veterancy", null), &value->Veterancy, state);
        Marshal(node.GetAttributeValue("Cost", null), &value->Cost, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original ConquerCardDefinitionUnit fields in the independently observed PC layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ConquerCardDefinitionUnit* value, Tracker state)
    {
        if(node is null)return;
        Marshal(node, &value->Base, state);
        Marshal(node.GetAttributeValue("Unit", null), &value->Unit, state);
        Marshal(node.GetChildNodes("Upgrade"), &value->Upgrade, state);
        Marshal(node.GetChildNodes("Veterancy"), &value->Veterancy, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original ConquerCardDefinitionStructure fields in the independently observed PC layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ConquerCardDefinitionStructure* value, Tracker state)
    {
        if(node is null)return;
        Marshal(node, &value->Base, state);
        Marshal(node.GetAttributeValue("Structure", null), &value->Structure, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original ConquerCardDefinitionPower fields in the independently observed PC layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ConquerCardDefinitionPower* value, Tracker state)
    {
        if(node is null)return;
        Marshal(node, &value->Base, state);
        Marshal(node.GetAttributeValue("Power", null), &value->Power, state);
        Marshal(node.GetAttributeValue("Side", null), &value->Side, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original SiegeEvaEvent fields in the independently observed PC layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, SiegeEvaEvent* value, Tracker state)
    {
        if(node is null)return;
        Marshal(node, &value->Base, state);
        Marshal(node.GetAttributeValue("Seconds", null), &value->Seconds, state);
        Marshal(node.GetAttributeValue("EvaEvent", null), &value->EvaEvent, state);
        Marshal(node.GetAttributeValue("EventType", "ALL"), &value->EventType, state);
    }
}
