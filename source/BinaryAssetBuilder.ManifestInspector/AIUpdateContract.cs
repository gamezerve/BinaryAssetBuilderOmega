using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using AnsiString=Relo.String<sbyte>;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-shaped AIUpdate verification uses typed schema/native views over the unchanged production EP1 ABI.
internal static class AIUpdateContract
{
    // Reborn: these proof views retain actual field widths while exposing flag/list/base ownership hidden by older uint/void storage.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct DynamicBitFlags { public fixed uint Value[1]; }
    [StructLayout(LayoutKind.Sequential)]
    internal struct BaseChooser
    {
        public Time IdleScanDelay,ReacquireDelay,StartupDelay;
        public float SympathyRange;
        public DynamicBitFlags CanPickDynamicTargetType;
        public Relo.List<Ep1ObjectVisionRangeItem> ObjectVisionRange;
        public SageBool CheckVisionRange,CanPickDynamicTargets,RotateToTargetWhenAiming,CanAutoAcquireNonAutoAcquirable,CanChaseAttack,CanOnlyAttackTargetsInFront;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct Chooser { public BaseChooser Base;public Relo.AssetReference<TargetingCompareList> TargetingCompareList; }
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Native
    {
        public UpdateModuleData Base;
        public AnsiString AttackPriority;
        public AutoAcquireEnemiesBitFlags AutoAcquireEnemiesWhenIdle;
        public float StopChaseDistance;
        public Time MinCowerTime,MaxCowerTime,BurningDeathTime;
        public float ComboLocoAttackDistance;
        public Relo.List<AnsiString> SpecialContactPoints;
        public Time IdleTargetTime;
        public uint MaxCombineChildren;
        public float SpawnOffsetRadius;
        public Ep1AIStateMachineType StateMachine;
        public Ep1RootLocomotorSetType DefaultLocomotorSet;
        public Chooser* UnitAITargetChooserData;
        public SageBool StandGround,FadeOnPortals;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore only comparison AI leaves; actual source, native values, chooser presence and typed references are proved separately. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        var actual=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Container=="AI").ToArray();var model=ConscriptModuleScalarContract.Owners(baseline).Where(o=>o.Container=="AI").ToArray();
        // Reborn: an AI-less object has nothing to restore (its zero-count declaration is proved by AIAbsenceContract); other count changes stay unsupported.
        if(actual.Length==0)return;
        if(actual.Length!=model.Length)throw new InvalidDataException("Source-sized AI module containers are not implemented yet.");
        for(int i=0;i<actual.Length;i++){var owner=actual[i].Element;owner.ParentNode!.ReplaceChild(source.ImportNode(model[i].Element,true),owner);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind the full source leaf to every native byte/import; unsupported contact/vision lists stay explicit rather than skipped. */
    //-------------------------------------------------------------------------------------------------
    internal static SchemaNativeContract.Result Verify(XmlElement source,Relo.Chunk native,IReadOnlyList<AssetId> references)
    {
        if(source.HasAttribute("SpecialContactPoints")&&!string.IsNullOrWhiteSpace(source.GetAttribute("SpecialContactPoints")))throw SdkXmlDiagnostics.At(new InvalidDataException("AIUpdate SpecialContactPoints item metadata is not implemented."),source.GetAttributeNode("SpecialContactPoints"));
        return SchemaNativeContract.Verify(source,typeof(Native),0x08DBC00Eu,native,references);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent stock Conscript, refinery and flak-trooper witnesses qualify chooser-present, chooser-absent and one-vision-item graphs before authored composition. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Native>()!=76||Marshal.SizeOf<Chooser>()!=40||Marshal.SizeOf<BaseChooser>()!=36)throw new InvalidDataException("AIUpdate proof/native ABI size differs.");
        var grammar=new StockMetadataGrammar();
        foreach(string name in new[]{"SovietAntiInfantryInfantry","SovietRefinery","SovietAntiVehicleInfantry"}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="AI").Modules.Single();var document=(XmlDocument)baseline.CloneNode(true);
            if(name!="SovietAntiInfantryInfantry"){var original=(XmlElement)document.ImportNode(GameObjectExtendedRootContract.Original(name=="SovietRefinery"?"Soviet/Structures/SovietRefinery.xml":"Soviet/Units/SovietAntiVehicleInfantry.xml","AI").Single(),true);
                // Reborn: the vision witness (one ObjectVisionRange item) uses a GlobalDefines idle-scan delay; resolve it from the original defines.
                foreach(var attribute in original.SelectNodes(".//@*")!.OfType<XmlAttribute>().Where(a=>a.Value.StartsWith("=$",StringComparison.Ordinal)).ToArray())attribute.Value=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").Single(e=>e.GetAttribute("name")==attribute.Value[2..]).GetAttribute("value");
                var previous=document.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="AI");previous.ParentNode!.ReplaceChild(original,previous);}
            var resolved=SchemaNativeContract.Resolve(document,schemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container=="AI").Element;
            SchemaNativeContract.VerifyStock(element,typeof(Native),0x08DBC00Eu,captured.Bytes,module.Native,captured.Asset.References.ToArray());grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Native));
        }
        return grammar;
    }
}
