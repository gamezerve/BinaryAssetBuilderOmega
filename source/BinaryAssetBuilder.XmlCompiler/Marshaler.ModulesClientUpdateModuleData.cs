using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, ClientUpdateModuleData** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        uint typeId = 0;
        Marshal(node.GetAttributeValue("TypeId", "0"), &typeId, Tracker.NullTracker);
        switch (typeId)
        {
            // Reborn: EP1's client audio class differs from the legacy behavior-update class.
            case 0x9826A297u:
                MarshalPolymorphicType<LargeGroupAudioClientUpdateModuleData, ClientUpdateModuleData>(node, objT, state);
                break;
            case 0x246E3C5Fu:
                MarshalPolymorphicType<SwayClientUpdateModuleData, ClientUpdateModuleData>(node, objT, state);
                break;
            case 0xAF51C3E2u:
                MarshalPolymorphicType<RadarMarkerClientUpdateModuleData, ClientUpdateModuleData>(node, objT, state);
                break;
            case 0xCA9ED83Du:
                MarshalPolymorphicType<LaserUpdateModuleData, ClientUpdateModuleData>(node, objT, state);
                break;
            case 0xA220E7D6u:
                // Reborn: EP1 EVA announcements use strong imports and the additional visibility flag.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData),1u))
                    Marshal(node,(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData*)*objT,state);
                uint evaReadyTypeId=0xA220E7D6u;state.InplaceEndianToPlatform(&evaReadyTypeId);*(uint*)*objT=evaReadyTypeId;
                break;
            case 0xD0EEFDDDu:
                // Reborn: EP1 create announcements use strong EVA imports instead of legacy string pointers.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1EvaAnnounceClientCreateModuleData),1u))
                    Marshal(node,(Ep1EvaAnnounceClientCreateModuleData*)*objT,state);
                uint evaCreateTypeId=0xD0EEFDDDu;state.InplaceEndianToPlatform(&evaCreateTypeId);*(uint*)*objT=evaCreateTypeId;
                break;
            case 0xF95B7D41u:
                MarshalPolymorphicType<BeaconClientUpdateModuleData, ClientUpdateModuleData>(node, objT, state);
                break;
            case 0x8FAF1398u:
                MarshalPolymorphicType<AnimatedParticleSysBoneClientUpdateModuleData, ClientUpdateModuleData>(node, objT, state);
                break;
            default:
                // Reborn: unsupported client updates must not silently leave null list entries.
                throw new BinaryAssetBuilderException(ErrorCode.InternalError, "Client update module {0:X08} is not implemented.", typeId);
        }
    }
}
