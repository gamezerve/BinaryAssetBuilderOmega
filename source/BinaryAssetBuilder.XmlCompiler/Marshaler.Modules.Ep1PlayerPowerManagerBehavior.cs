using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve both required power and optional EVA imports using the complete Uprising manager layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1PlayerPowerManagerBehaviorModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1PlayerPowerManagerBehaviorModuleData.SpecialPowerTemplate),null),&objT->SpecialPowerTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1PlayerPowerManagerBehaviorModuleData.OnAvailableEvaEvent),null),&objT->OnAvailableEvaEvent,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
