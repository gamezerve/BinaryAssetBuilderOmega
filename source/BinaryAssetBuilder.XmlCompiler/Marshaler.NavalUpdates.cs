using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the official circular-damage option with the original bit position and endian conversion. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,CircularDamageUpdateOptionsTypeBitFlags* objT,Tracker state)
    {
        if(value is null)return;
        foreach(string token in value.GetText().Split(WhiteSpaces,System.StringSplitOptions.RemoveEmptyEntries)){
            if(token=="ALL"){objT->Value[0]=1;continue;}
            if(token.TrimStart('+','-')!="ONLY_ON_WATER")throw new System.IO.InvalidDataException("Unknown circular-damage flag: "+token);
            if(token[0]=='-')objT->Value[0]&=~1u;else objT->Value[0]|=1;
        }
        state.InplaceEndianToPlatform(&objT->Value[0]);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the full retail circular-damage layout, typed imports and required inline object filter. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,CircularDamageUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Flags",null),&objT->Flags,state);
        Marshal(node.GetAttributeValue("MaxRadius","100.0"),&objT->MaxRadius,state);
        Marshal(node.GetAttributeValue("ArcDistance","10.0"),&objT->ArcDistance,state);
        Marshal(node.GetAttributeValue("SweepFXList",null),&objT->SweepFXList,state);
        Marshal(node.GetAttributeValue("SweepWeapon",null),&objT->SweepWeapon,state);
        Marshal(node.GetAttributeValue("FXTimeout","0.25s"),&objT->FXTimeout,state);
        Marshal(node.GetChildNode("ObjectFilter",null),&objT->ObjectFilter,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit editable health thresholds, damage and death mode with the official defaults and optional status gate. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,LoseHealthUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("HealthTrigger","10%"),&objT->HealthTrigger,state);
        Marshal(node.GetAttributeValue("DamagePerSecond","100"),&objT->DamagePerSecond,state);
        Marshal(node.GetAttributeValue("DeathType","NONE"),&objT->DeathType,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
