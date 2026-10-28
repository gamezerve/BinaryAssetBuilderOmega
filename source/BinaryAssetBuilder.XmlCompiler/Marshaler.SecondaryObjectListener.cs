using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the schema-ordered listener notification flags without changing unrelated bitflag writers. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,SecondaryObjectListenerBitFlags* objT,Tracker state)
    {
        if(value is null)return;
        foreach(string token in value.GetText().Split(WhiteSpaces,System.StringSplitOptions.RemoveEmptyEntries)){
            if(token=="ALL"){objT->Value[0]=3;continue;}
            var name=token.TrimStart('+','-');
            if(!System.Enum.TryParse<SecondaryObjectListenerOption>(name,out var option)||!System.Enum.IsDefined(typeof(SecondaryObjectListenerOption),option))throw new System.IO.InvalidDataException("Unknown listener flag: "+token);
            uint bit=1u<<(int)option;if(token[0]=='-')objT->Value[0]&=~bit;else objT->Value[0]|=bit;
        }
        state.InplaceEndianToPlatform(&objT->Value[0]);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile each authored object filter and strong UpgradeSelf reference list in source order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,ObjectEffectTuple* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode("ObjectFilter",null),&objT->ObjectFilter,state);
        Marshal(node.GetChildNodes("UpgradeSelf"),&objT->UpgradeSelf,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 secondary-object listener and its optional validation graph. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,SecondaryObjectListenerModule* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Flags",null),&objT->Flags,state);
        Marshal(node.GetChildNode("SelfValidation",null),&objT->SelfValidation,state);
        Marshal(node.GetChildNodes("ObjectEffectTuple"),&objT->ObjectEffectTuple,state);
        Marshal(node,(BehaviorModuleData*)objT,state);
    }
}
