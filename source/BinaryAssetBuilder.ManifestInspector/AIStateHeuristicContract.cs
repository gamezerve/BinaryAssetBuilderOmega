using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: each admitted authored heuristic requires its own independent original global source/native witness before emission.
internal static class AIStateHeuristicContract
{
    private sealed record Witness(string Schema,string File,string Asset,int Index);
    private static readonly Witness[] Witnesses={
        new("AIStateConstantHeuristic","EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_29.xml","AIStrategicStateDefinition:AIState_CC29_CoreUnpack_001",0),
        new("AIStateTimerHeuristic","EP1/SkirmishAI/States/AlliedBaseStates_EP1.xml","AIStrategicStateDefinition:AlliedCaptureTechAggressive",0),
        new("AIStateEnemyNearbyHeuristic","EP1/SkirmishAI/States/AlliedBaseStates_EP1.xml","AIStrategicStateDefinition:AlliedCaptureTechAggressive",1),
        new("AIStateIntervalHeuristic","EP1/SkirmishAI/States/AlliedBaseStates_EP1.xml","AIStrategicStateDefinition:AlliedDogPackAttack",1),
        new("AIStateOpeningMoveHeuristic","EP1/SkirmishAI/States/AlliedBaseStates_EP1.xml","AIStrategicStateDefinition:ShatterAttack",0),
        new("AIStatePathToTargetHeuristic","EP1/SkirmishAI/States/JapanBaseStates_EP1.xml","AIStrategicStateDefinition:JapanTenguNoPathAttack",2),
        new("AIStateObjectOfTypeExistsHeuristic","EP1/SkirmishAI/States/JapanBaseStates_EP1.xml","AIStrategicStateDefinition:YurikoSpamAttack",0),
        new("AIStateScriptedFlagHeuristic","EP1/SkirmishAI/States/JapanBaseStates_EP1.xml","AIStrategicStateDefinition:CC17_BossAttack",0),
        new("AIStatePowerAdvantageHeuristic","SkirmishAI/States/AIBudgetStateLibrary.xml","AIBudgetStateDefinition:HeavyDefenseBudget",4),
        new("AIStateIncomeAdvantageHeuristic","SkirmishAI/States/AIStateLibrary.xml","AIStrategicStateDefinition:Cleanup",1),
        new("AIStateIsBuildingHeuristic","SkirmishAI/States/AlliedBaseStates.xml","AIStrategicStateDefinition:AlliedCaptureTech_EASY",0),
        new("AIStateTechBuildingShareHeuristic","SkirmishAI/States/AlliedBaseStates.xml","AIStrategicStateDefinition:AlliedCaptureTech_EASY",2),
        new("AIStateAllNearbyNodesFullHeuristic","SkirmishAI/States/AlliedBaseStates.xml","AIStrategicStateDefinition:AlliedExpansion",3),
        new("AIStateTimeSinceLastExpansionHeuristic","SkirmishAI/States/AlliedBaseStates.xml","AIStrategicStateDefinition:AlliedExpansion_MEDIUM",1),
        new("AIStateResourceNodeShareHeuristic","SkirmishAI/States/AlliedBaseStates.xml","AIStrategicStateDefinition:AlliedExpansion_MEDIUM",5),
        new("AIStateCanBuildBuildingsOnSurfaceHeuristic","SkirmishAI/States/AlliedBaseStates.xml","AIStrategicStateDefinition:AlliedNavalBase",3),
        new("AIAlliedBeaconExistsHeuristic","SkirmishAI/States/CoopStates.xml","AIStrategicStateDefinition:CommandAssault_Gather_Land",0),
        new("AIStateOpenNodeHeuristic","SkirmishAI/States/JapanBaseStates.xml","AIStrategicStateDefinition:JapanExpansion",5),
        new("AIStateMapNameHeuristic","SkirmishAI/States/JapanBaseStates.xml","AIBuildStateDefinition:JapanRequiresLandStructureBuildState",0),
    };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove every requested concrete heuristic against independently stored original global bytes, including optional pointers and masks. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(IEnumerable<XmlDocument> sources,string global,SchemaSet schemas,ManifestAsset[] available)
    {
        var types=new Dictionary<string,(Type Type,uint Wire)>();
        foreach(var xml in sources.Where(s=>s.DocumentElement!.LocalName is "AIBudgetStateDefinition" or "AIBuildStateDefinition" or "AIStrategicStateDefinition" or "AIPersonalityDefinition"))
        {
            var resolved=SchemaNativeContract.Resolve(xml,schemas,available);
            foreach(XmlElement list in resolved.GetElementsByTagName("Heuristic",SchemaSet.XmlNamespace))
            foreach(var leaf in list.ChildNodes.OfType<XmlElement>())types.TryAdd(leaf.SchemaInfo.SchemaType!.QualifiedName.Name,Ep1AIHeuristicLayout.For(leaf));
        }
        if(types.Count==0)return;
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(global,64*1048576));TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);
        if(!catalog.Header.IsLinked||catalog.Validate().Count!=0)throw new InvalidDataException("Invalid original AI heuristic witness catalog.");
        foreach(var requested in types)
        {
            var witness=Witnesses.Single(w=>w.Schema==requested.Key);int colon=witness.Asset.IndexOf(':');
            var asset=catalog.Assets.Single(a=>a.Name==witness.Asset);if(asset.Tokenized!=0)throw new InvalidDataException("AI heuristic witness must be native.");
            var source=GameObjectExtendedRootContract.Original(witness.File,witness.Asset[..colon]).Single(e=>e.GetAttribute("id")==witness.Asset[(colon+1)..]);
            var lists=source.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Heuristic"&&e.NamespaceURI==SchemaSet.XmlNamespace).ToArray();
            if(lists.Length!=1)throw new InvalidDataException("Original AI heuristic owner list differs.");
            var leaves=lists[0].ChildNodes.OfType<XmlElement>().ToArray();var bytes=W3DContainerCompiler.ReadStock(global,catalog,asset);
            uint count=Word(bytes,4),items=Word(bytes,8);if(count!=leaves.Length||witness.Index<0||witness.Index>=count)throw new InvalidDataException("Original AI heuristic source/native count differs.");
            int native=checked((int)Word(bytes,checked((int)items+4*witness.Index)));
            var probe=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<AIBudgetStateDefinition xmlns=\""+SchemaSet.XmlNamespace+"\" id=\"RebornAIHeuristicWitness\"><Heuristic/> <Budget/></AIBudgetStateDefinition>"));
            probe.GetElementsByTagName("Heuristic",SchemaSet.XmlNamespace)[0]!.AppendChild(probe.ImportNode(leaves[witness.Index],true));
            var proof=SchemaNativeContract.Resolve(probe,schemas,available).GetElementsByTagName("Heuristic",SchemaSet.XmlNamespace)[0]!.ChildNodes.OfType<XmlElement>().Single();
            var family=Ep1AIHeuristicLayout.For(proof);if(family!=requested.Value||Word(bytes,native)!=family.Wire)throw new InvalidDataException("Original AI heuristic class differs.");
            SchemaNativeContract.VerifyStock(proof,family.Type,family.Wire,bytes,native,asset.References.ToArray(),required:true);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep every original heuristic list/class read inside its independently bounded owner asset. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int at)
    { if(at<0||at>bytes.Length-4)throw new InvalidDataException("Original AI heuristic witness exceeds its owner.");return System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4)); }
}
