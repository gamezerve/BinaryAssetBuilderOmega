using Relo;
using SageBinaryData;
using AnsiString=Relo.String<sbyte>;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal required scroll attributes into the original inline EP1 record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,ScrollInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Rate",null),&target->Rate,state);
        Marshal(node.GetAttributeValue("RateEveryFrame",null),&target->RateEveryFrame,state);
        Marshal(node.GetAttributeValue("ScrollDown",null),&target->ScrollDown,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain enum selection, packed BGRA color and font string ownership in each source-authored style. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,CreditStyle* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Name",null),&target->Name,state);
        Marshal(node.GetChildNode("Color",null),&target->Color,state);
        Marshal(node.GetChildNode("Font",null),&target->Font,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: each Text element owns one terminated string; empty entries are real blank lines, never tokenized or discarded. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,CreditElement* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Style",null),&target->Style,state);
        var texts=node.GetChildNodes("Text");int count=texts.GetCount();target->Text.Count=(uint)count;
        // Reborn: an absent Text list has no pointee; a present empty Text element still owns its blank string.
        if(count==0)return;
        using Tracker.Context context=state.Push((void**)&target->Text.Items,(uint)sizeof(AnsiString),(uint)count);
        for(int i=0;i<count;i++)Marshal(texts.GetNextNode().GetValue()?.GetText()??string.Empty,&target->Text.Items[i],state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all authored credit styles/items through existing relocation-aware list allocation. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Credits* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode("Scroll",null),&target->Scroll,state);
        Marshal(node.GetChildNodes("Style"),&target->Style,state);
        Marshal(node.GetChildNodes("CreditItem"),&target->CreditItem,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
