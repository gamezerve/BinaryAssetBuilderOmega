using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 placement-hint names set their schema ordinal bits; prefixed operations are refused by the source contract. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string text,AssistanceHintBitFlags* objT,Tracker state)
    {
        foreach(string token in text.Split(WhiteSpaces,System.StringSplitOptions.RemoveEmptyEntries))
        {
            if(string.Equals(token,"ALL",System.StringComparison.Ordinal)){for(int i=0;i<AssistanceHintBitFlags.NumSpans;i++)objT->Value[i]=uint.MaxValue;continue;}
            if(System.Enum.TryParse(token,false,out AssistanceHintType value)&&(uint)value<AssistanceHintBitFlags.Count)objT->Value[(uint)value/AssistanceHintBitFlags.BitsInSpan]|=1u<<(int)((uint)value%AssistanceHintBitFlags.BitsInSpan);
        }
        for(int i=0;i<AssistanceHintBitFlags.NumSpans;i++)state.InplaceEndianToPlatform(&objT->Value[i]);
    }

    public static unsafe void Marshal(Value value,AssistanceHintBitFlags* objT,Tracker state)
    {
        if(value is null)return;
        Marshal(value.GetText(),objT,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 assisted placement; the optional requirement filter is the only pointee. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,AssistedPlacementBehaviorModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(AssistedPlacementBehaviorModuleData.PlacementHint),"NONE"),&objT->PlacementHint,state);
        Marshal(node.GetAttributeValue(nameof(AssistedPlacementBehaviorModuleData.SnappingDistance),"0"),&objT->SnappingDistance,state);
        Marshal(node.GetChildNode(nameof(AssistedPlacementBehaviorModuleData.RequirementObjectFilter),null),&objT->RequirementObjectFilter,state);
        Marshal(node,(BehaviorModuleData*)objT,state);
    }
}
