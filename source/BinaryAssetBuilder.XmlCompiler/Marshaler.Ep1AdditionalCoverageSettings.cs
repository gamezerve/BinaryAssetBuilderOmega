using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse only original money transaction categories and reject unknown mask tokens. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1CoverageMoneyCategories* target,Tracker state)
    {
        if(value is null)return;
        string[] categories={"UNKNOWN","STARTING_CASH","HARVESTER","CAPTURE_BONUS","AUTO_DEPOSIT","SELL_OBJECT","BOUNTY","CASH_CHEAT","DEVASTATE","CANCEL_BUILD","MONEY_CRATE","DEATH_REFUND","SLAUGHTER","TERRAIN_RESOURCE","SHARED_DEPOSIT","PRODUCTION","BUILD_WALLSPAN","DOZER_CONSTRUCT","SELF_REPAIR","PLAYERKILLED","SCRIPTED_MONEY_ACTION","SPECIAL_POWER","AI_MONEY_LENDER","PLAYER_GIVE_MONEY","PLAYER_CLONE","TRANSFER_ASSETS","CASH_HACK","INFILTRATOR","PILLAGE","STEAL_MONEY"};
        foreach(string token in value.GetText().Split((char[])null,System.StringSplitOptions.RemoveEmptyEntries))
        {
            int bit=System.Array.IndexOf(categories,token);
            if(bit<0)throw new System.IO.InvalidDataException("Unknown money transaction category: "+token);
            target->Value[0]|=1u<<bit;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the original resource singleton without introducing a new runtime subsystem. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CoverageResourceSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1CoverageResourceSettings.SharedMoneyTransactionCategories),null),&target->SharedMoneyTransactionCategories,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the gift filter graph and original default transfer permission. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CoverageObjectGiftSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1CoverageObjectGiftSettings.AllowObjectFilter),null),&target->AllowObjectFilter,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CoverageObjectGiftSettings.AllowTransferToSkirmishPlayer),"true"),&target->AllowTransferToSkirmishPlayer,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the original by-value gift settings and its full filter data. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CoverageGiftManagerSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1CoverageGiftManagerSettings.ObjectGiftSettings),null),&target->ObjectGiftSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile only the EP1 height-death fields and preserve their original defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1HeightDieUpdateModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1HeightDieUpdateModuleData.TargetHeight),null),&target->TargetHeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1HeightDieUpdateModuleData.InitialDelay),"0"),&target->InitialDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1HeightDieUpdateModuleData.TargetHeightIncludesStructures),"false"),&target->TargetHeightIncludesStructures,state);
        Marshal(node.GetAttributeValue(nameof(Ep1HeightDieUpdateModuleData.OnlyWhenMovingDown),"false"),&target->OnlyWhenMovingDown,state);
        Marshal(node.GetAttributeValue(nameof(Ep1HeightDieUpdateModuleData.SnapToGroundOnDeath),"false"),&target->SnapToGroundOnDeath,state);
        Marshal(node,(UpdateModuleData*)target,state);
    }
}
