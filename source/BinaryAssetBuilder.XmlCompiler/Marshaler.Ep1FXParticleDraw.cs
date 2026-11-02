using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize all original EP1 angle axes and size channels in their recovered native order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1FXParticleUpdateRenderObject* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Rotation","ROTATION_OFF"),&objT->Rotation,state);
        Marshal(node.GetChildNode("AngleX",null),&objT->AngleX,state);
        Marshal(node.GetChildNode("AngularRateX",null),&objT->AngularRateX,state);
        Marshal(node.GetChildNode("AngleY",null),&objT->AngleY,state);
        Marshal(node.GetChildNode("AngularRateY",null),&objT->AngularRateY,state);
        Marshal(node.GetChildNode("AngleZ",null),&objT->AngleZ,state);
        Marshal(node.GetChildNode("AngularRateZ",null),&objT->AngularRateZ,state);
        Marshal(node.GetChildNode("AngularDamping",null),&objT->AngularDamping,state);
        Marshal(node.GetChildNode("StartSize",null),&objT->StartSize,state);
        Marshal(node.GetChildNode("SizeRate",null),&objT->SizeRate,state);
        Marshal(node.GetChildNode("SizeDamping",null),&objT->SizeDamping,state);
        Marshal(node,&objT->Base,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate the complete EP1 RenderObject update while keeping other update dispatch unchanged. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1ParticleUpdate(Node wrapper, FXParticleUpdateBase** objT, Tracker state)
    {
        if(wrapper is null)return;
        var children=wrapper.GetChildNodes(null);if(children is null||children.GetCount()!=1)return;
        var node=children.GetNextNode();uint typeId=0;Marshal(node.GetAttributeValue("TypeId","0"),&typeId,Tracker.NullTracker);
        if(typeId==0x0DFB6E4Au){
            using var context=state.Push((void**)objT,(uint)sizeof(Ep1FXParticleUpdateRenderObject),1u);
            Marshal(node,(Ep1FXParticleUpdateRenderObject*)*objT,state);
            // Reborn: match the normal polymorphic dispatcher class-word write using the original EP1 wire ID.
            state.InplaceEndianToPlatform(&typeId);*(uint*)*objT=typeId;
        }else Marshal(node,objT,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize only the three ObjectSet fields present in the original Uprising XSD and native records. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1FXParticleDrawRenderObjectObjectSet* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("RenderGroup",null),&objT->RenderGroup,state);
        Marshal(node.GetAttributeValue("NumObjects","0"),&objT->NumObjects,state);
        Marshal(node.GetAttributeValue("Percent","0.0"),&objT->Percent,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain original draw header/allocation order while using the EP1 list stride. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1FXParticleDrawRenderObject* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("SinkRate","0.0"),&objT->SinkRate,state);
        Marshal(node.GetAttributeValue("MultiRenderObjects","false"),&objT->MultiRenderObjects,state);
        Marshal(node.GetAttributeValue("SinkOnTerrainCollision","false"),&objT->SinkOnTerrainCollision,state);
        Marshal(node.GetChildNodes("ObjectSet"),&objT->ObjectSet,state);
        Marshal(node,&objT->Base,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select the EP1 RenderObject writer before allocation; other particle draws retain their existing dispatch. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1ParticleDraw(Node wrapper, FXParticleDrawBase** objT, Tracker state)
    {
        if(wrapper is null)return;
        var children=wrapper.GetChildNodes(null);if(children is null||children.GetCount()!=1)return;
        var node=children.GetNextNode();uint typeId=0;Marshal(node.GetAttributeValue("TypeId","0"),&typeId,Tracker.NullTracker);
        // Reborn: lightning has EP1 End*Change fields instead of the old KW End* names.
        if(typeId==0xB503BC4Cu){
            using var context=state.Push((void**)objT,(uint)sizeof(Ep1FXParticleDrawLightning),1u);
            Marshal(node,(Ep1FXParticleDrawLightning*)*objT,state);
            state.InplaceEndianToPlatform(&typeId);*(uint*)*objT=typeId;
        }else if(typeId==0xCB25505Bu){
            using var context=state.Push((void**)objT,(uint)sizeof(Ep1FXParticleDrawRenderObject),1u);
            Marshal(node,(Ep1FXParticleDrawRenderObject*)*objT,state);
            // Reborn: the runtime class remains FXParticleDrawRenderObject despite the isolated managed layout name.
            state.InplaceEndianToPlatform(&typeId);*(uint*)*objT=typeId;
        }else Marshal(node,objT,state);
    }
}
