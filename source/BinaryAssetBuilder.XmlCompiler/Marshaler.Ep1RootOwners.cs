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
            // Reborn: projected buildability uses the object's real inline list header, not an allocated pointer record.
            if(field.InlineList){Marshal(source.GetChildNodes(field.Name),&root->ProjectedBuildabilityInfo,tracker);continue;}
            var node=source.GetChildNode(field.Name,null);
            // Reborn: absent resource information retains its null pointer; never synthesize an empty or zero-cost transaction.
            if(node==null){if(field.Optional)continue;throw new InvalidDataException("Root owner allocation is not yet optional on this profile: "+field.Name);}
            void** slot=(void**)((byte*)root+field.Slot);if(*slot!=null)throw new InvalidDataException("Root owner already allocated.");
            using var owner=tracker.Push(slot,(uint)System.Runtime.InteropServices.Marshal.SizeOf(field.NativeType),1u);
            switch(field.Name)
            {
                // Reborn: weak object/upgrade dependencies retain their real EP1 pointer/list layout on the common Tracker.
                case "GameDependency":Marshal(node,(GameDependencyType*)*slot,tracker);break;
                case "ObjectResourceInfo":Marshal(node,(ObjectResourceInfo*)*slot,tracker);break;
                case "SkirmishAIInformation":Marshal(node,(Ep1SkirmishAIInformation*)*slot,tracker);break;
                case "AudioArrayVoice":Marshal(node,(Ep1AudioArrayVoice*)*slot,tracker);break;
                // Reborn: root sounds use the already stock-qualified EP1 enum/reference model, not legacy KW ordinals.
                case "AudioArraySound":Marshal(node,(Ep1AudioArraySound*)*slot,tracker);break;
                case "EvaEventArray":Marshal(node,(Ep1EvaEventArray*)*slot,tracker);break;
                case "ShadowInfo":Marshal(node,(ShadowInfo*)*slot,tracker);break;
                case "CrusherInfo":Marshal(node,(Ep1CrusherInfo*)*slot,tracker);break;
            }
        }
    }
}
