using System;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain native optional allocations and derive relationship bits from the original enum order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,SelectiveVisibilityBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(SelectiveVisibilityBehaviorModuleData.UpdateRate),null),&target->UpdateRate,state);
        var players=node.GetAttributeValue(nameof(SelectiveVisibilityBehaviorModuleData.AllowedPlayers),null);
        if(players is not null)
        {
            using Tracker.Context context=state.Push((void**)&target->AllowedPlayers,(uint)sizeof(RelationshipBitFlags),1u);
            foreach(string token in players.GetText().Split(WhiteSpaces,StringSplitOptions.RemoveEmptyEntries))
                target->AllowedPlayers->Value[0]|=1u<<(int)Enum.Parse<RelationshipType>(token,false);
        }
        Marshal(node,(BehaviorModuleData*)target,state);
    }
}
