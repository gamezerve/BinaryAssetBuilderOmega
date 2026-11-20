using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep the EP1-only behavior layouts separate from the shared legacy dispatcher. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe bool TryMarshalEp1ReviewedBehavior(Node node, BehaviorModuleData** target, Tracker state, uint type)
    {
        switch (type)
        {
            case Ep1SupplyCenterDockUpdateModuleData.WireTypeId:
                MarshalPolymorphicType<Ep1SupplyCenterDockUpdateModuleData, BehaviorModuleData>(node, target, state);
                break;
            case Ep1TemporaryOwnerSpecialPowerModuleData.WireTypeId:
                MarshalPolymorphicType<Ep1TemporaryOwnerSpecialPowerModuleData, BehaviorModuleData>(node, target, state);
                break;
            case GateProxyBehaviorModuleData.WireTypeId:
                MarshalPolymorphicType<GateProxyBehaviorModuleData, BehaviorModuleData>(node, target, state);
                break;
            case SupplyCenterProductionExitUpdateModuleData.WireTypeId:
                MarshalPolymorphicType<SupplyCenterProductionExitUpdateModuleData, BehaviorModuleData>(node, target, state);
                break;
            case RepairDockUpdateModuleData.WireTypeId:
                MarshalPolymorphicType<RepairDockUpdateModuleData, BehaviorModuleData>(node, target, state);
                break;
            case RailedTransportDockUpdateModuleData.WireTypeId:
                MarshalPolymorphicType<RailedTransportDockUpdateModuleData, BehaviorModuleData>(node, target, state);
                break;
            default:
                return false;
        }

        state.InplaceEndianToPlatform(&type);
        *(uint*)*target = type;
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the multiplier and deposit byte at the offsets used by the EP1 dock action. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1SupplyCenterDockUpdateModuleData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SupplyCenterDockUpdateModuleData.ValueMultiplier), "1.0"), &target->ValueMultiplier, state);
        Marshal(node.GetAttributeValue(nameof(Ep1SupplyCenterDockUpdateModuleData.DistributedDeposit), "false"), &target->DistributedDeposit, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: append the authored slave status mask to the existing EP1 special power fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1TemporaryOwnerSpecialPowerModuleData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        Marshal(node.GetAttributeValue(nameof(Ep1TemporaryOwnerSpecialPowerModuleData.SlaveStatus), null), &target->SlaveStatus, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a gate proxy serializes all inherited gate settings with its own runtime class. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, GateProxyBehaviorModuleData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent exit vectors remain null and present vectors have separate relocations. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, SupplyCenterProductionExitUpdateModuleData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        Marshal(node.GetChildNode(nameof(SupplyCenterProductionExitUpdateModuleData.UnitCreatePoint), null), &target->UnitCreatePoint, state);
        Marshal(node.GetChildNode(nameof(SupplyCenterProductionExitUpdateModuleData.NaturalRallyPoint), null), &target->NaturalRallyPoint, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the repair dock's heal duration as the retail reader's float. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, RepairDockUpdateModuleData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        Marshal(node.GetAttributeValue(nameof(RepairDockUpdateModuleData.FramesForFullHeal), "0"), &target->FramesForFullHeal, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the rail dock's two unsigned SageDuration values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, RailedTransportDockUpdateModuleData* target, Tracker state)
    {
        Marshal(node, &target->Base, state);
        Marshal(node.GetAttributeValue(nameof(RailedTransportDockUpdateModuleData.PullInsideDurationInFrames), "0"), &target->PullInsideDurationInFrames, state);
        Marshal(node.GetAttributeValue(nameof(RailedTransportDockUpdateModuleData.PushOutsideDurationInFrames), "0"), &target->PushOutsideDurationInFrames, state);
    }
}
