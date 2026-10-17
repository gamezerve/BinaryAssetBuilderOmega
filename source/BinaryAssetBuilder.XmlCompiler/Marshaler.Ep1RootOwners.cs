using Relo;
using SageBinaryData;
using System.IO;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate all six reviewed root owners after the existing module/geometry graph on its actual Tracker, never by appending separately compiled native chunks. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1RootOwners(Node source,GameObject* root,Tracker tracker)
    {
        foreach(var field in BinaryAssetBuilder.XmlCompiler.Ep1GameObjectRootOwnersProjection.Fields)
        {
            var node=source.GetChildNode(field.Name,null);if(node==null)throw new InvalidDataException("Six complete root owners required: "+field.Name);
            void** slot=(void**)((byte*)root+field.Slot);if(*slot!=null)throw new InvalidDataException("Root owner already allocated.");
            using var owner=tracker.Push(slot,(uint)System.Runtime.InteropServices.Marshal.SizeOf(field.NativeType),1u);
            switch(field.Name)
            {
                case "ObjectResourceInfo":Marshal(node,(ObjectResourceInfo*)*slot,tracker);break;
                case "SkirmishAIInformation":Marshal(node,(Ep1SkirmishAIInformation*)*slot,tracker);break;
                case "AudioArrayVoice":Marshal(node,(Ep1AudioArrayVoice*)*slot,tracker);break;
                case "EvaEventArray":Marshal(node,(Ep1EvaEventArray*)*slot,tracker);break;
                case "ShadowInfo":Marshal(node,(ShadowInfo*)*slot,tracker);break;
                case "CrusherInfo":Marshal(node,(Ep1CrusherInfo*)*slot,tracker);break;
            }
        }
    }
}
