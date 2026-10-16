using Relo;
using SageBinaryData;
using BinaryAssetBuilder.Core.Hashing;
using System;
using System.Collections.Generic;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly compile bounded reviewed behavior families on the enclosing object's Tracker, preserving original wire IDs and the isolated EP1 ABI fixes. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1ReviewedBehaviors(Node node,PolymorphicList<BehaviorModuleData>** objT,Tracker state)
    {
        var children=node?.GetChildNodes(null);int count=children?.GetCount()??0;if(count<1 || count>64)throw new System.IO.InvalidDataException("Reviewed EP1 behavior list must be bounded/nonempty.");var ids=new HashSet<uint>();
        using var header=state.Push((void**)objT,(uint)sizeof(PolymorphicList<BehaviorModuleData>),1u);(*objT)->Count=(uint)count;state.InplaceEndianToPlatform(&(*objT)->Count);using var items=state.Push((void**)&(*objT)->Items,(uint)sizeof(BehaviorModuleData*),(uint)count);
        for(int i=0;i<count;i++){var child=children.GetNextNode();string id=child.GetAttributeValue("id",null)?.GetText();if(string.IsNullOrWhiteSpace(id) || !ids.Add(FastHash.GetHashCode(id.ToLowerInvariant())) || !uint.TryParse(child.GetAttributeValue("TypeId",null)?.GetText(),out uint type))throw new System.IO.InvalidDataException("Reviewed behavior class/identity required.");BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.ReviewedNativeType(type);if(type==0x431722A9)MarshalEp1WeaponSet(child,&(*objT)->Items[i],state);else if(type is 0xB5EB41ED or 0x79BFEDCB or 0x59D12990 or 0x786C9458 or 0x3B68B768 or 0x90CEB49C)MarshalEp1InfantryBehavior(child,&(*objT)->Items[i],state,type);else Marshal(child,&(*objT)->Items[i],state);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit EP1 weapon allocation preserves the original wire ID instead of hashing the separate C# model's name. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1WeaponSet(Node node,BehaviorModuleData** objT,Tracker state)
    {
        if(node is null || node.GetAttributeValue("TypeId",null)?.GetText()!="1125589673")throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Explicit EP1 WeaponSetUpdate ID required.");
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1WeaponSetUpdateModuleData),1u))Marshal(node,(Ep1WeaponSetUpdateModuleData*)*objT,state);
        uint type=0x431722A9;state.InplaceEndianToPlatform(&type);*(uint*)*objT=type;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: opt into one/two reviewed module types on the same Tracker; unknown classes and duplicate local IDs fail rather than falling back to KW. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1SelectedBehaviors(Node node,PolymorphicList<BehaviorModuleData>** objT,Tracker state)
    {
        var children=node?.GetChildNodes(null);int count=children?.GetCount()??0;
        if(count<1 || count>3)throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Selected EP1 behavior envelope requires one to three modules.");
        var ids=new HashSet<uint>();
        using(var header=state.Push((void**)objT,(uint)sizeof(PolymorphicList<BehaviorModuleData>),1u))
        {
            (*objT)->Count=(uint)count;state.InplaceEndianToPlatform(&(*objT)->Count);
            using(var items=state.Push((void**)&(*objT)->Items,(uint)sizeof(BehaviorModuleData*),(uint)count))
                for(int i=0;i<count;i++)
                {
                    var child=children.GetNextNode();string id=child.GetAttributeValue("id",null)?.GetText();
                    if(string.IsNullOrEmpty(id) || !ids.Add(FastHash.GetHashCode(id.ToLowerInvariant())))throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Selected EP1 modules require distinct nonempty local IDs.");
                    string type=child.GetAttributeValue("TypeId",null)?.GetText();
                    if(type=="227975924")MarshalPolymorphicType<BuildingDestructionModuleData,BehaviorModuleData>(child,&(*objT)->Items[i],state);
                    else if(type=="1125589673")MarshalEp1WeaponSet(child,&(*objT)->Items[i],state);
                    else if(type=="2138239554")MarshalPolymorphicType<ObjectSMCHelperModuleData,BehaviorModuleData>(child,&(*objT)->Items[i],state);
                    else throw new BinaryAssetBuilderException(ErrorCode.InternalError,"Unreviewed selected EP1 behavior class.");
                }
        }
    }
}
