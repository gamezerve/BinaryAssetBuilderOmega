using Relo;
using SageBinaryData;
using BinaryAssetBuilder.Core.Hashing;
using System;
using System.Collections.Generic;
using System.IO;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly select EP1 voice/sound ordinals on one client Tracker without altering the legacy KW client/root sound route. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1ClientBehaviors(Node node,PolymorphicList<ClientBehaviorModuleData>** objT,Tracker state)
    {
        if(node==null)return;var children=node.GetChildNodes(null);int count=children?.GetCount()??0;if(count>64)throw new InvalidDataException("Selected EP1 client count exceeds bound.");var ids=new HashSet<uint>();
        using(var header=state.Push((void**)objT,(uint)sizeof(PolymorphicList<ClientBehaviorModuleData>),1u))
        {
            (*objT)->Count=(uint)count;state.InplaceEndianToPlatform(&(*objT)->Count);if(count==0)return;
            using(var items=state.Push((void**)&(*objT)->Items,(uint)sizeof(ClientBehaviorModuleData*),(uint)count))for(int i=0;i<count;i++)
            {
                var child=children.GetNextNode();string id=child.GetAttributeValue("id",null)?.GetText();if(string.IsNullOrWhiteSpace(id) || !ids.Add(FastHash.GetHashCode(id.ToLowerInvariant())))throw new InvalidDataException("Selected client IDs must be nonempty/hash-distinct.");
                string type=child.GetAttributeValue("TypeId",null)?.GetText();
                if(type==0x5A3EDBF4u.ToString(System.Globalization.CultureInfo.InvariantCulture))MarshalPolymorphicType<ModelConditionAudioLoopClientBehaviorModuleData,ClientBehaviorModuleData>(child,&(*objT)->Items[i],state);
                else if(type==0x27EF791Du.ToString(System.Globalization.CultureInfo.InvariantCulture))MarshalPolymorphicType<AnimationSoundClientBehaviorModuleData,ClientBehaviorModuleData>(child,&(*objT)->Items[i],state);
                else if(type==0x327CFB76u.ToString(System.Globalization.CultureInfo.InvariantCulture))
                {
                    using(var module=state.Push((void**)&(*objT)->Items[i],(uint)sizeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),1u))Marshal(child,(Ep1ModelConditionSoundSelectorClientBehaviorModuleData*)(*objT)->Items[i],state);
                    uint wire=0x327CFB76;state.InplaceEndianToPlatform(&wire);*(uint*)(*objT)->Items[i]=wire;
                }
                else throw new InvalidDataException("Selected EP1 client class not implemented.");
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the twelve-byte EP1 voice entry preserves optional Sound pointees and inline typed EvaEvent occurrences under real EP1 voice ordinals. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AudioVoiceEntry* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue(nameof(Ep1AudioVoiceEntry.AudioType),null),&objT->AudioType,state);Marshal(node.GetAttributeValue("Sound",null),&objT->Base.Sound,state);Marshal(node.GetAttributeValue("EvaEvent",null),&objT->Base.EvaEvent,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the reviewed eight-byte sound entry uses EP1 sound ordinals without changing its strong-reference occurrence representation. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AudioSoundEntry* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue(nameof(Ep1AudioSoundEntry.Sound),null),&objT->Sound,state);Marshal(node.GetAttributeValue(nameof(Ep1AudioSoundEntry.AudioType),null),&objT->AudioType,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: named voice entries (e.g. ReturnToProducer) use the twelve-byte EP1 Sound/EvaEvent base followed by the Name hash. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AudioVoiceReferentialEntry* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue(nameof(Ep1AudioVoiceReferentialEntry.Name),null),&objT->Name,state);Marshal(node.GetAttributeValue("Sound",null),&objT->Base.Sound,state);Marshal(node.GetAttributeValue("EvaEvent",null),&objT->Base.EvaEvent,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain all voice entry families and ordering; object-specific entries keep their original record type and stay refused by the root tokenizer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AudioArrayVoice* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetChildNodes(nameof(Ep1AudioArrayVoice.AudioEntry)),&objT->AudioEntry,state);Marshal(node.GetChildNodes(nameof(Ep1AudioArrayVoice.ObjectSpecificEntry)),&objT->ObjectSpecificEntry,state);Marshal(node.GetChildNodes(nameof(Ep1AudioArrayVoice.NamedEntry)),&objT->NamedEntry,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: nullable EP1 voice arrays allocate exactly one owned twenty-four-byte header when present. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1AudioArrayVoice** objT,Tracker state)
    {if(node==null)return;using var context=state.Push((void**)objT,(uint)sizeof(Ep1AudioArrayVoice),1u);Marshal(node,*objT,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 sound arrays retain their complete ordered list on the original Tracker. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AudioArraySound* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetChildNodes(nameof(Ep1AudioArraySound.AudioEntry)),&objT->AudioEntry,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent sound arrays remain null; present-empty arrays retain a separate owned header. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node,Ep1AudioArraySound** objT,Tracker state)
    {if(node==null)return;using var context=state.Push((void**)objT,(uint)sizeof(Ep1AudioArraySound),1u);Marshal(node,*objT,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: selector flags and nullable priority/voice/sound fields retain all official source members with the EP1 array types. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ModelConditionSoundSelectorOverrideBlock* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetAttributeValue(nameof(Ep1ModelConditionSoundSelectorOverrideBlock.RequiredFlags),""),&objT->RequiredFlags,state);Marshal(node.GetAttributeValue(nameof(Ep1ModelConditionSoundSelectorOverrideBlock.ExcludedFlags),""),&objT->ExcludedFlags,state);Marshal(node.GetAttributeValue(nameof(Ep1ModelConditionSoundSelectorOverrideBlock.VoicePriorityOverride),null),&objT->VoicePriorityOverride,state);Marshal(node.GetChildNode(nameof(Ep1ModelConditionSoundSelectorOverrideBlock.AudioArrayVoice),null),&objT->AudioArrayVoice,state);Marshal(node.GetChildNode(nameof(Ep1ModelConditionSoundSelectorOverrideBlock.AudioArraySound),null),&objT->AudioArraySound,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the separate selector CLR name never becomes a guessed runtime class ID; its caller supplies the original reviewed 327CFB76 wire ID explicitly. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ModelConditionSoundSelectorClientBehaviorModuleData* objT,Tracker state)
    {if(node==null)return;Marshal(node.GetChildNodes(nameof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData.Override)),&objT->Override,state);Marshal(node,(ClientBehaviorModuleData*)objT,state);}
}
