using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit complete grappling controls, weapon imports and optional validation/filter graphs with official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,GrapplingHookUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("PullRate",null),&objT->PullRate,state);
        Marshal(node.GetAttributeValue("MinBounceDelay","0.75s"),&objT->MinBounceDelay,state);
        Marshal(node.GetAttributeValue("MaxBounceDelay","1.5s"),&objT->MaxBounceDelay,state);
        Marshal(node.GetAttributeValue("BouncePercentage","20%"),&objT->BouncePercentage,state);
        Marshal(node.GetAttributeValue("CollidingPullRateFactor","0%"),&objT->CollidingPullRateFactor,state);
        Marshal(node.GetAttributeValue("SingleFireWeapon",null),&objT->SingleFireWeapon,state);
        Marshal(node.GetAttributeValue("ContinuousFireWeapon",null),&objT->ContinuousFireWeapon,state);
        Marshal(node.GetAttributeValue("ContinuousFireWeaponBlocked",null),&objT->ContinuousFireWeaponBlocked,state);
        Marshal(node.GetAttributeValue("ContinuousFireWeaponDelay","0.5s"),&objT->ContinuousFireWeaponDelay,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node.GetChildNode("KeepAliveObjectFilter",null),&objT->KeepAliveObjectFilter,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the actual identity-only MCV storage module instead of dropping it. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,SubStructureStorageBehaviorModuleData* objT,Tracker state)
    {if(node is not null)Marshal(node,(BehaviorModuleData*)objT,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the EP1 TankDraw extension separately from the legacy KW native type. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1W3DTankDrawModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode("TreadTerrainParticles",null),&objT->TreadTerrainParticles,state);
        Marshal(node.GetChildNodes("ArticulationBone"),&objT->ArticulationBone,state);
        Marshal(node,(W3DScriptedModelDrawModuleData*)objT,state);
    }
}
