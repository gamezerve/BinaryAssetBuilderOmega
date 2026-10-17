using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the real normalized strong EVA occurrence and the explicit EP1 schema ordinal, never compile an event name as a string pointer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EvaEventEntry* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue("EvaEvent",null),&objT->EvaEvent,state);Ep1ToggleEnum(node.GetAttributeValue("EvaType",null),(uint*)&objT->EvaType,state,typeof(Ep1ThingTemplateEvaEventType),false);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: root EVA source order and every occurrence are retained on the caller's Tracker; legacy KW dispatch stays unchanged. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EvaEventArray* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetChildNodes("EvaEntry"),&objT->EvaEntry,state);}
}
