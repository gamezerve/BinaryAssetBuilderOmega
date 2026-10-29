using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the captured concrete falling-movement class and official omitted-field defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,ProjectileMoveToGroundData* objT,Tracker state)
    {
        if(node is null)return;
        uint type=0x87767DF6u;state.InplaceEndianToPlatform(&type);objT->Base.TypeId=type;
        Marshal(node.GetAttributeValue("InitialSpeed","0"),&objT->InitialSpeed,state);
        Marshal(node.GetAttributeValue("Acceleration",null),&objT->Acceleration,state);
        Marshal(node.GetAttributeValue("TerminalVelocity",null),&objT->TerminalVelocity,state);
        Marshal(node.GetAttributeValue("RotationSpeedPerSecond","0d"),&objT->RotationSpeedPerSecond,state);
        Marshal(node.GetAttributeValue("TerminalRotation","90d"),&objT->TerminalRotation,state);
        Marshal(node.GetAttributeValue("ForwardVelocityDamping","100%"),&objT->ForwardVelocityDamping,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the real 28-byte projectile header and require the reviewed state-machine/movement pairing. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,ProjectileUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        if((node.GetAttributeValue("StateMachine","BombProjectile")?.GetText())!="BombProjectile")throw new System.IO.InvalidDataException("ProjectileUpdate movement mode is not implemented; currently BombProjectile/MoveToGround is supported.");
        var movement=node.GetChildNode("Movement",null)?.GetChildNode("MoveToGround",null)??throw new System.IO.InvalidDataException("ProjectileUpdate requires Movement/MoveToGround.");
        Marshal(node,(UpdateModuleData*)objT,state);
        Ep1ToggleEnum(node.GetAttributeValue("StateMachine","BombProjectile"),(uint*)&objT->StateMachine,state,typeof(ProjectileStateMachineType),false);
        Marshal(node.GetAttributeValue("MinDetonationHeight",null),&objT->MinDetonationHeight,state);
        Marshal(node.GetAttributeValue("MinTargetDistance",null),&objT->MinTargetDistance,state);
        Ep1ToggleEnum(node.GetAttributeValue("Flags",null),&objT->Flags,state,typeof(ProjectileUpdateOptions),true);
        using(var context=state.Push((void**)&objT->Movement,(uint)sizeof(ProjectileMoveToGroundData),1u))Marshal(movement,objT->Movement,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit every inherited slow-death field while the dispatcher retains the concrete ship class identity. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,ShipSlowDeathBehaviorModuleData* objT,Tracker state)
    {if(node is null)return;Marshal(node,(SlowDeathBehaviorModuleData*)objT,state);}
}
