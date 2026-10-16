using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, ClientBehaviorModuleData** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        uint typeId = 0;
        Marshal(node.GetAttributeValue("TypeId", "0"), &typeId, Tracker.NullTracker);
        switch (typeId)
        {
            case 0xF9115AA9u:
                MarshalPolymorphicType<UpgradeSoundSelectorClientBehaviorModuleData, ClientBehaviorModuleData>(node, objT, state);
                break;
            case 0xB4A8B1F4u:
                MarshalPolymorphicType<RandomSoundSelectorClientBehaviorModuleData, ClientBehaviorModuleData>(node, objT, state);
                break;
            case 0x327CFB76u:
                MarshalPolymorphicType<ModelConditionSoundSelectorClientBehaviorModuleData, ClientBehaviorModuleData>(node, objT, state);
                break;
            case 0x5A3EDBF4u:
                MarshalPolymorphicType<ModelConditionAudioLoopClientBehaviorModuleData, ClientBehaviorModuleData>(node, objT, state);
                break;
            case 0x27EF791Du:
                MarshalPolymorphicType<AnimationSoundClientBehaviorModuleData, ClientBehaviorModuleData>(node, objT, state);
                break;
            default:
                // Reborn: unknown client classes must fail instead of silently dropping part of the original complete object.
                throw new System.IO.InvalidDataException($"Client behavior module {typeId:X08} is not implemented.");
        }
    }
}
