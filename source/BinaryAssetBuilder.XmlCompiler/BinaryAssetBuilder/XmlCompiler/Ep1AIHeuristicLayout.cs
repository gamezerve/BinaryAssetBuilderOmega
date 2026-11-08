using System;
using System.Linq;
using System.Xml;
using SageBinaryData;
namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: production and proof use only this explicit original-source/native-qualified class mapping.
public static class Ep1AIHeuristicLayout
{
    public sealed record Family(string Element,string Schema,Type Type,uint Wire);
    private static readonly Family[] Families={
        // Reborn: this campaign-only class is proved by the original PC runtime ABI, not an invented stock asset.
        new("StructureOrHarvesterUnderAttackHeuristic","AIStateStructureOrHarvesterUnderAttackHeuristic",typeof(Ep1AIStateStructureOrHarvesterUnderAttackHeuristic),0x6B660E54u),
        new("ConstantHeuristic","AIStateConstantHeuristic",typeof(Ep1AIStateConstantHeuristic),0x51FB6D92u),
        new("TimerHeuristic","AIStateTimerHeuristic",typeof(Ep1AIStateTimerHeuristic),0x10A03AC0u),
        new("EnemyNearbyHeuristic","AIStateEnemyNearbyHeuristic",typeof(Ep1AIStateEnemyNearbyHeuristic),0x67B985A7u),
        new("IntervalHeuristic","AIStateIntervalHeuristic",typeof(Ep1AIStateIntervalHeuristic),0x8199E979u),
        new("OpeningMoveHeuristic","AIStateOpeningMoveHeuristic",typeof(Ep1AIStateOpeningMoveHeuristic),0x7EE4C9B4u),
        new("PathToTargetHeuristic","AIStatePathToTargetHeuristic",typeof(Ep1AIStatePathToTargetHeuristic),0xF1B0E2B3u),
        new("ObjectOfTypeExistsHeuristic","AIStateObjectOfTypeExistsHeuristic",typeof(Ep1AIStateObjectOfTypeExistsHeuristic),0xBE76E654u),
        new("ScriptedFlagHeuristic","AIStateScriptedFlagHeuristic",typeof(Ep1AIStateScriptedFlagHeuristic),0x809071B7u),
        new("PowerAdvantageHeuristic","AIStatePowerAdvantageHeuristic",typeof(Ep1AIStatePowerAdvantageHeuristic),0xADC0BB89u),
        new("IncomeAdvantageHeuristic","AIStateIncomeAdvantageHeuristic",typeof(Ep1AIStateIncomeAdvantageHeuristic),0x52DEE998u),
        new("IsBuildingHeuristic","AIStateIsBuildingHeuristic",typeof(Ep1AIStateIsBuildingHeuristic),0x45C8CCA2u),
        new("TechBuildingShareHeuristic","AIStateTechBuildingShareHeuristic",typeof(Ep1AIStateTechBuildingShareHeuristic),0xCBA1A93Bu),
        new("AllNearbyNodesFullHeuristic","AIStateAllNearbyNodesFullHeuristic",typeof(Ep1AIStateAllNearbyNodesFullHeuristic),0x38B3FD56u),
        new("TimeSinceLastExpansionHeuristic","AIStateTimeSinceLastExpansionHeuristic",typeof(Ep1AIStateTimeSinceLastExpansionHeuristic),0xE40EBC24u),
        new("ResourceNodeShareHeuristic","AIStateResourceNodeShareHeuristic",typeof(Ep1AIStateResourceNodeShareHeuristic),0x874A6E09u),
        new("CanBuildBuildingsOnSurfaceHeuristic","AIStateCanBuildBuildingsOnSurfaceHeuristic",typeof(Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic),0x9EE54386u),
        new("BeaconExistsHeuristic","AIAlliedBeaconExistsHeuristic",typeof(Ep1AIAlliedBeaconExistsHeuristic),0x7E198900u),
        new("OpenNodeHeuristic","AIStateOpenNodeHeuristic",typeof(Ep1AIStateOpenNodeHeuristic),0x84ACF588u),
        new("MapNameHeuristic","AIStateMapNameHeuristic",typeof(Ep1AIStateMapNameHeuristic),0xDAFA6EB8u),
    };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: return only an explicitly reviewed class whose original schema alternative has already validated. */
    //-------------------------------------------------------------------------------------------------
    public static (Type Type,uint Wire) For(XmlElement element)
    {
        string name=element.SchemaInfo.SchemaType?.QualifiedName.Name??throw new InvalidOperationException("AI heuristic schema type missing.");
        var matches=Families.Where(f=>f.Schema==name&&f.Element==element.LocalName).ToArray();
        if(matches.Length!=1)throw new NotSupportedException("AI heuristic source family not reviewed: "+name);
        return (matches[0].Type,matches[0].Wire);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive private writer class words from the admitted concrete source alternatives without changing authored/proof XML. */
    //-------------------------------------------------------------------------------------------------
    public static void PrepareWriter(XmlElement root)
    {
        foreach(XmlElement owner in root.GetElementsByTagName("Heuristic","uri:ea.com:eala:asset"))
        foreach(XmlNode item in owner.ChildNodes)
        {
            if(item is not XmlElement leaf)continue;
            var matches=Families.Where(f=>f.Element==leaf.LocalName&&leaf.NamespaceURI=="uri:ea.com:eala:asset").ToArray();
            if(matches.Length!=1)throw new NotSupportedException("AI heuristic source family not reviewed: "+leaf.LocalName);
            leaf.SetAttribute("TypeId",XmlConvert.ToString(matches[0].Wire));
        }
    }
}
