using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write every EP1 map environment texture import, including DepthLut omitted by the legacy KW layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1EnvironmentObject* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Cloud",null),&objT->Cloud,state);
        Marshal(node.GetAttributeValue("Macro",null),&objT->Macro,state);
        Marshal(node.GetAttributeValue("Environment",null),&objT->Environment,state);
        Marshal(node.GetAttributeValue("DepthLut",null),&objT->DepthLut,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: water-area values use the ordinary schema enum parser and tracker endian handling. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, WaterBodyType* objT, Tracker state)
    {
        if(node is null)return;Marshal(node.GetValue(),objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write optional analysis counters with their original zero defaults and preserve water-area order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, MapAnalysisType* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("GarrisonableStructures","0"),&objT->GarrisonableStructures,state);
        Marshal(node.GetAttributeValue("TechStructures","0"),&objT->TechStructures,state);
        Marshal(node.GetAttributeValue("CellsExamined","0"),&objT->CellsExamined,state);
        Marshal(node.GetAttributeValue("WaterCells","0"),&objT->WaterCells,state);
        Marshal(node.GetAttributeValue("WaterAreas","0"),&objT->WaterAreas,state);
        Marshal(node.GetChildNodes("WaterAreaType"),&objT->WaterAreaType,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize all official EP1 map runtime children while leaving pipeline-only metadata out of the native graph. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1GameMap* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes("Waypoint"),&objT->Waypoint,state);
        Marshal(node.GetChildNodes("Structure"),&objT->Structure,state);
        Marshal(node.GetChildNodes("Unit"),&objT->Unit,state);
        Marshal(node.GetChildNodes("Prop"),&objT->Prop,state);
        Marshal(node.GetChildNodes("Audio"),&objT->Audio,state);
        Marshal(node.GetChildNodes("Unknown"),&objT->Unknown,state);
        Marshal(node.GetChildNodes("Road"),&objT->Road,state);
        var environment=node.GetChildNode("EnvironmentData",null);
        if(environment!=null){using var context=state.Push((void**)&objT->EnvironmentData,(uint)sizeof(Ep1EnvironmentObject),1);Marshal(environment,objT->EnvironmentData,state);}
        Marshal(node.GetChildNode("WorldDict",null),&objT->WorldDict,state);
        Marshal(node.GetChildNodes("PostEffect"),&objT->PostEffect,state);
        var analysis=node.GetChildNode("MapAnalysis",null);
        if(analysis!=null){using var context=state.Push((void**)&objT->MapAnalysis,(uint)sizeof(MapAnalysisType),1);Marshal(analysis,objT->MapAnalysis,state);}
        Marshal(node,(BaseAssetType*)objT,state);
    }
}
