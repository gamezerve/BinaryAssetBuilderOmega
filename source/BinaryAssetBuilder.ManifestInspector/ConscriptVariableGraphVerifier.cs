using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reusable full native/metadata proof; authored identities select reviewed owners instead of fixed sizes or new list ordinals.
internal sealed class ConscriptVariableGraphVerifier
{
    private static readonly int[] Detached={44,48,412,416,420,424,428,432,468,472,476,480,484,488,492,496,500,532,536,540,552,560,564,568,572};
    private readonly XmlSchemaSet official;
    private readonly (byte[] Metadata,ManifestDocument Document,ManifestAsset Asset,long Offset,byte[] Bytes,GameObjectStockReview.Projection Projection) stock;
    private readonly GameObjectModuleReview.Proof topology;
    private readonly Dictionary<string,GameObjectModuleReview.Module> templates;
    // Reborn: sell modules use their actual building stock witness, independent of Conscript's original module list.
    private readonly Lazy<(byte[] Bytes,GameObjectModuleReview.Module Module,int End)> sell;
    // Reborn: QueueDock's new native model must first match independently read stock field/inheritance metadata.
    private readonly Lazy<bool> queueDock;
    // Reborn: queue exit binds the new EP1 Time/vector model to independent stock metadata before composition.
    private readonly Lazy<bool> queueExit;
    // Reborn: qualify the entire EP1 Spawn header and stock metadata independently of Conscript's original families.
    private readonly Lazy<bool> spawn;
    // Reborn: the unpack model must match the refinery's full independent Generic/Update/Contain stock chain before composition.
    private readonly Lazy<bool> unpack;
    // Reborn: EP1 production matches the refinery empty header and Barracks translation graph before source-sized composition.
    private readonly Lazy<StockMetadataGrammar> production;
    // Reborn: EP1 repair must match the refinery's 44-byte record and 312-byte literal metadata before composition.
    private readonly Lazy<bool> repair;
    // Reborn: EP1 engineer must reproduce the refinery's full payload, imports and metadata before composition.
    private readonly Lazy<bool> engineer;
    // Reborn: EP1 assisted placement must reproduce the refinery payload and metadata before composition.
    private readonly Lazy<bool> placement;
    // Reborn: the recovered infiltrator record must reproduce the refinery payload, default imports and metadata.
    private readonly Lazy<bool> infiltrator;
    // Reborn: EP1 structure placement must reproduce the refinery shape payload and metadata before composition.
    private readonly Lazy<bool> structurePlacement;
    // Reborn: LUAEventList stock qualification is lazy and runs only when the source authors that family.
    private readonly Lazy<bool> luaEventList;
    // Reborn: BuildingBehavior's existing native writer is qualified against both Power Plant stock modules before source-owned strings are admitted.
    private readonly Lazy<StockMetadataGrammar> buildingBehavior;
    // Reborn: legacy PowerUpgrade borrows a stock-qualified UpgradeModuleData base witness (EP1 ships no PowerUpgrade instance).
    private readonly Lazy<PowerUpgradeContract.Witness> powerUpgrade;
    // Reborn: assign-slaves inherits the existing SpecialPower ABI but has its own independently qualified concrete wrapper.
    private readonly Lazy<StockMetadataGrammar> assignSlaves;
    // Reborn: qualify base SpecialPower independently of its previously supported subclasses.
    private readonly Lazy<StockMetadataGrammar> specialPower;
    // Reborn: recall and parking each retain an independent lazily qualified EP1 metadata grammar.
    private readonly Dictionary<Type,Lazy<StockMetadataGrammar>> airfield=new();
    // Reborn: reactor death weapons have independently qualified native and inherited upgrade/death metadata.
    private readonly Lazy<StockMetadataGrammar> fireWeaponDeath;
    // Reborn: load automatic-deposit inherited metadata once per build from the local retail WorldBuilder catalog.
    private readonly Lazy<StockMetadataGrammar> autoDeposit;
    // Reborn: reuse the full retail-qualified slaughter/container metadata chain once per build.
    private readonly Lazy<StockMetadataGrammar> slaughter;
    // Reborn: retain the production-info marker's stock-qualified identity-only metadata chain.
    private readonly Lazy<StockMetadataGrammar> productionInfo;
    // Reborn: self-replacement shares an independently captured SpecialAbility metadata chain across the build.
    private readonly Lazy<StockMetadataGrammar> replaceSelf;
    // Reborn: qualify the new wall-builder ABI and metadata once per mixed build.
    // Reborn: timer-monitor metadata is qualified lazily when the source actually requests the module.
    // Reborn: qualify radar metadata only when authored by the source graph.
    // Reborn: power-manager metadata is learned independently from retail spellbooks.
    // Reborn: learn production-bonus inheritance and percentages from retail spellbook records.
    // Reborn: object-storage metadata is qualified independently against the original chronosphere powers.
    // Reborn: teleport storage uses independently qualified native and metadata records.
    // Reborn: teleport-update native fields, filters and shader metadata are qualified against original spellbook records.
    // Reborn: OCL powers qualify their EP1 offset, enum and reference metadata once per build.
    // Reborn: player upgrade metadata is qualified independently against both original spellbooks.
    // Reborn: template-ID storage metadata is qualified lazily against its retail module.
    // Reborn: weapon-fire abilities use the independently qualified EP1 field and metadata layout.
    private readonly Lazy<StockMetadataGrammar> weaponFireAbility;
    // Reborn: PropBody owns an independent retail-qualified body fragment rather than an ActiveBody snapshot.
    // Reborn: optional vision has a separately proved native record and root metadata owner.
    private readonly Lazy<StockMetadataGrammar> visionInfo;
    private readonly Lazy<StockMetadataGrammar> propBody;
    private readonly Lazy<StockMetadataGrammar> storeTemplate;
    private readonly Lazy<StockMetadataGrammar> playerUpgrade;
    private readonly Lazy<StockMetadataGrammar> oclPower;
    private readonly Lazy<StockMetadataGrammar> teleportUpdate;
    private readonly Lazy<StockMetadataGrammar> teleportStored;
    private readonly Lazy<StockMetadataGrammar> storeObjects;
    private readonly Lazy<StockMetadataGrammar> productionBonus;
    private readonly Lazy<StockMetadataGrammar> playerPowerManager;
    private readonly Lazy<StockMetadataGrammar> radar;
    private readonly Lazy<StockMetadataGrammar> monitorSpecialPowerTimer;
    // Reborn: notification metadata and native storage are qualified independently of any Conscript shape.
    private readonly Lazy<StockMetadataGrammar> notifyWhenAttacking;
    // Reborn: transports use independent EP1 containment and passenger-list metadata qualification.
    private readonly Lazy<StockMetadataGrammar> transportContain;
    // Reborn: attribute modifier upgrades retain their complete inherited upgrade graph and strong modifier reference.
    private readonly Lazy<StockMetadataGrammar> attributeModifierUpgrade;
    // Reborn: passenger ejection has its own retail-qualified EP1 native and metadata graph.
    private readonly Lazy<StockMetadataGrammar> ejectPassengers;
    // Reborn: run-off-map flags and waypoint strings use their own retail-qualified graph.
    private readonly Lazy<StockMetadataGrammar> runOffMap;
    // Reborn: assistance responses retain their independent EP1 native/status and metadata qualification.
    private readonly Lazy<StockMetadataGrammar> attackAssistResponse;
    // Reborn: garrisons use the retail-qualified inline roster and OpenContain metadata chain.
    private readonly Lazy<StockMetadataGrammar> garrisonContain;
    // Reborn: bunker invisibility has its own retail-qualified strong-reference metadata contract.
    // Reborn: qualify aura grammar only when this family is authored.
    // Reborn: load detector grammar only for authored detector modules.
    // Reborn: complete SlowDeath phase records have stock-qualified native and metadata ownership.
    // Reborn: aiming metadata is qualified only when an aiming behavior is authored.
    // Reborn: independently qualify collision metadata before composing authored modules.
    private readonly Lazy<StockMetadataGrammar> fireWeaponCollide;
    private readonly Lazy<StockMetadataGrammar> aimWeapon;
    private readonly Lazy<StockMetadataGrammar> slowDeath;
    private readonly Lazy<StockMetadataGrammar> stealthDetector;
    private readonly Lazy<StockMetadataGrammar> attributeModifierAura;
    private readonly Lazy<StockMetadataGrammar> invisibilityBehavior;
    // Reborn: subobject upgrade nuggets have a separate retail-qualified EP1 layout and metadata grammar.
    private readonly Lazy<StockMetadataGrammar> subObjectsUpgrade;
    private readonly Lazy<StockMetadataGrammar> wallBuilder;
    // Reborn: turret weapon slots use a retail-derived metadata grammar and independent source proof.
    private readonly Lazy<StockMetadataGrammar> weaponTurret;
    // Reborn: schema-proved draw leaves need the generic proof qualified on stock Conscript draws and a metadata grammar from stock draws.
    private readonly Dictionary<Type,Lazy<StockMetadataGrammar>> schemaFamilies=new();
    // Reborn: Geometry is a nonmodule owner with independently qualified source/native and stock metadata grammar.
    private readonly Lazy<StockMetadataGrammar> geometryGrammar;
    // Reborn: dependency weak IDs and optional record/list ownership have a separate stock-qualified proof.
    private readonly Lazy<StockMetadataGrammar> dependencyGrammar;
    // Reborn: optional chooser ownership is qualified on independent infantry/building AI witnesses, not fixed Conscript child presence.
    private readonly Lazy<StockMetadataGrammar> aiGrammar;
    // Reborn: jet AI carries a separate retail-qualified derived record and strong EVA/weapon imports.
    private readonly Lazy<StockMetadataGrammar> jetAiGrammar;
    // Reborn: optional root records and inline list items each retain their own independently stock-qualified grammar.
    private readonly Dictionary<string,Lazy<StockMetadataGrammar>> extendedRoots=new(StringComparer.Ordinal);
    // Reborn: verify all three original FX graphs against stock before accepting source-sized death/event layouts.
    private readonly Lazy<bool> fxList;
    // Reborn: both infantry and building tracker layouts retain independent stock qualification before flexible source admission.
    private readonly Lazy<bool> secondaryDamage;
    // Reborn: creation/destroy native records and literal metadata must match all original stock witnesses before flexible authoring.
    private readonly Lazy<bool> deathBehaviors;
    // Reborn: a populated Browser list is enabled only after the original WorldBuilder VL_RussianPortBuilding list/metadata witness passes.
    private readonly Lazy<bool> browserWitness;
    private readonly Lazy<bool> aiAbsentWitness;
    // Reborn: source/catalog expectations remain independent of Core indices and emitted native buffers.
    private readonly SchemaSet referenceSchemas;
    private readonly ManifestAsset[] catalog;
    private readonly XmlDocument baseline;
    private readonly Dictionary<string,AssetId[]> referenceTables=new(StringComparer.Ordinal);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind source templates to independently captured stock identities once per build. */
    //-------------------------------------------------------------------------------------------------
    internal ConscriptVariableGraphVerifier(XmlSchemaSet schemas,string manifest,XmlDocument baseline,SchemaSet referenceSchemas,ManifestAsset[] catalog)
    {
        official=schemas;stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");topology=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name);
        // Reborn: bool-only stock witnesses run in developer mode; source verification and metadata-producing grammar loads remain active in both modes.
        queueDock=new(()=>!SdkBuildTests.Enabled||QueueDockContract.QualifyStock(manifest));
        queueExit=new(()=>!SdkBuildTests.Enabled||QueueProductionExitContract.QualifyStock(manifest));
        spawn=new(()=>!SdkBuildTests.Enabled||SpawnBehaviorContract.QualifyStock(manifest,official,catalog));
        unpack=new(()=>!SdkBuildTests.Enabled||StructureUnpackContract.QualifyStock(manifest,official,catalog));
        production=new(()=>ProductionUpdateContract.QualifyGraph(manifest,baseline,referenceSchemas,catalog));
        repair=new(()=>!SdkBuildTests.Enabled||RepairUpdateContract.QualifyStock(manifest));
        engineer=new(()=>!SdkBuildTests.Enabled||EngineerContainContract.QualifyStock(manifest,official,catalog));
        placement=new(()=>!SdkBuildTests.Enabled||AssistedPlacementContract.QualifyStock(manifest,official));
        infiltrator=new(()=>!SdkBuildTests.Enabled||InfiltratorContainContract.QualifyStock(manifest,official,catalog));
        structurePlacement=new(()=>!SdkBuildTests.Enabled||StructurePlacementContract.QualifyStock(manifest,official));
        luaEventList=new(()=>!SdkBuildTests.Enabled||LUAEventListContract.QualifyStock(manifest,official));
        buildingBehavior=new(()=>BuildingBehaviorContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        specialPower=new(()=>SpecialPowerContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        assignSlaves=new(()=>AssignSlavesSpecialPowerContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        weaponTurret=new(()=>WeaponTurretContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        weaponFireAbility=new(()=>WeaponFireAbilityContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        visionInfo=new(()=>VisionInfoContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        propBody=new(()=>PropBodyContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        storeTemplate=new(()=>StoreTemplateIDContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        playerUpgrade=new(()=>PlayerUpgradePowerContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        oclPower=new(()=>OclSpecialPowerContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        teleportUpdate=new(()=>TeleportStoredUpdateContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        teleportStored=new(()=>TeleportStoredObjectsContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        storeObjects=new(()=>StoreObjectsContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        productionBonus=new(()=>ProductionBonusContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        playerPowerManager=new(()=>PlayerPowerManagerContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        radar=new(()=>RadarBehaviorContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        subObjectsUpgrade=new(()=>SubObjectsUpgradeContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        fireWeaponCollide=new(()=>FireWeaponCollideContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        aimWeapon=new(()=>AimWeaponContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        slowDeath=new(()=>SlowDeathContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        stealthDetector=new(()=>StealthDetectorContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        attributeModifierAura=new(()=>AttributeModifierAuraContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        invisibilityBehavior=new(()=>InvisibilityBehaviorContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        garrisonContain=new(()=>GarrisonContainContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        attackAssistResponse=new(()=>AttackAssistResponseContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        runOffMap=new(()=>RunOffMapContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        ejectPassengers=new(()=>EjectPassengersContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        attributeModifierUpgrade=new(()=>AttributeModifierUpgradeContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        transportContain=new(()=>TransportContainContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        notifyWhenAttacking=new(()=>NotifyWhenAttackingContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        monitorSpecialPowerTimer=new(()=>MonitorSpecialPowerTimerContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        wallBuilder=new(()=>WallBuilderContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        replaceSelf=new(()=>ReplaceSelfContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        productionInfo=new(()=>ProductionUpdateInfoContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        slaughter=new(()=>SlaughterContainContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        autoDeposit=new(()=>AutoDepositContract.Qualify(WorldBuilderAssetResolver.CatalogPath(),baseline,referenceSchemas,catalog));
        fireWeaponDeath=new(()=>FireWeaponDeathContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        // Reborn: both source families use their own native layout, independently witnessed on retail airfields.
        foreach(var type in new[]{typeof(Ep1RecallUnitsSpecialPowerModuleData),typeof(Ep1ParkingPlaceBehaviorModuleData)})airfield.Add(type,new(()=>AirfieldBehaviorContract.Qualify(type,manifest,baseline,referenceSchemas,catalog)));
        foreach(var family in SchemaNativeContract.Families)schemaFamilies.Add(family.Type,new(()=>QualifySchemaFamily(manifest,family)));
        geometryGrammar=new(()=>GameObjectGeometryContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        dependencyGrammar=new(()=>GameObjectDependencyContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        jetAiGrammar=new(()=>JetAIUpdateContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        aiGrammar=new(()=>AIUpdateContract.Qualify(manifest,baseline,referenceSchemas,catalog));
        foreach(string name in GameObjectExtendedRootContract.Names)extendedRoots.Add(name,new(()=>GameObjectExtendedRootContract.Qualify(name,manifest,baseline,referenceSchemas,catalog)));
        powerUpgrade=new(()=>PowerUpgradeContract.Create(stock.Bytes,topology.Containers.Single(c=>c.Name=="Behaviors").Modules,GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("AI")].Offset));
        fxList=new(()=>!SdkBuildTests.Enabled||FXListBehaviorContract.QualifyStock(manifest,official,referenceSchemas,baseline,catalog));
        secondaryDamage=new(()=>!SdkBuildTests.Enabled||SecondaryDamageTrackerContract.QualifyStock(manifest,official,referenceSchemas,baseline,catalog));
        browserWitness=new(()=>!SdkBuildTests.Enabled||GameObjectBrowserContract.Qualify(manifest));
        aiAbsentWitness=new(()=>!SdkBuildTests.Enabled||AIAbsenceContract.Qualify(manifest));
        deathBehaviors=new(()=>!SdkBuildTests.Enabled||DeathBehaviorContract.QualifyStock(manifest,official,referenceSchemas,baseline,catalog));
        templates=ConscriptModuleScalarContract.Owners(baseline).ToDictionary(o=>o.Element.GetAttribute("id"),o=>topology.Containers.Single(c=>c.Name==o.Container).Modules[o.Index],StringComparer.Ordinal);
        sell=new(()=>{
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:AlliedConstructionYard");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
            var module=modules.Single(m=>m.Type=="StructureSellUpdateModuleData");int index=Array.IndexOf(modules,module);
            int end=index+1<modules.Length?modules[index+1].Wrapper:GameObjectStockReview.Tokens(captured.Bytes,0)[FastHash.GetHashCode("AI")].Offset;
            return(captured.Bytes,module,end);
        });
        this.referenceSchemas=referenceSchemas;this.catalog=catalog;this.baseline=CopySource(baseline);
        if(!ConscriptReferenceTargets.Read(this.baseline,referenceSchemas,catalog).SequenceEqual(stock.Asset.References))throw new InvalidDataException("Reference source traversal differs from captured stock occurrence slots.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source identities/order select independently captured templates; changed counts never select stock offsets by the new ordinal. */
    //-------------------------------------------------------------------------------------------------
    internal Chunk Verify(XmlDocument source,Chunk native,InstanceDeclaration instance,Dictionary<string,string> origins,Chunk? supplied=null)
    {
        byte[] b=native.InstanceBuffer;var relos=Slots(native.RelocationBuffer);var imports=Slots(native.ImportsBuffer);int cut=Ep1GameObjectRootOwnersProjection.NativeTailStart(b);
        // Reborn: Browser owns its canonical item/text allocation, proved against authored tokens (and, when populated, the stock WorldBuilder witness).
        var browser=Ep1GameObjectBrowser.Inspect(b,relos);if(browser.Count!=0)_=browserWitness.Value;GameObjectBrowserContract.Verify(source,b,browser);var browserSlots=browser.Relocations.ToHashSet();
        // Reborn: verify authored fixed fields and restore only comparison bytes, never the actual output.
        var cleared=b.ToArray();var stockNative=stock.Bytes.AsSpan(stock.Projection.NativeRoot).ToArray();
        GameObjectScalarContract.VerifyAndRestore(source,b,cleared,stockNative);
        GameObjectRootAttributeContract.VerifyAndRestore(source,b,cleared,stockNative);
        ConscriptModuleScalarContract.Verify(source,b,0);
        // Reborn: independent plain-record expectations cover all implemented scalar attributes, including defaults and packed values.
        var recordValues=GameObjectNestedValueContract.Expected(source,b);
        // Reborn: nested record/module contracts already verify every historical scalar; the obsolete six-slot oracle cannot require exactly one movement row.
        var authoredValues=new Dictionary<int,uint>();
        var rootImports=GameObjectStrongReferenceContract.Fields.ToDictionary(f=>"GameObject."+f.Name,f=>string.IsNullOrWhiteSpace(source.DocumentElement!.GetAttribute(f.Name))?(AssetId?)null:new AssetId(FastHash.GetHashCode(f.Name),InstanceHandle.GetInstanceId(source.DocumentElement.GetAttribute(f.Name))));
        var originalStock=stock.Bytes.ToArray();foreach(int at in Detached){Put(cleared,at,0);Put(originalStock,stock.Projection.NativeRoot+at,0);}
        var root=new Chunk{InstanceBuffer=cleared,RelocationBuffer=Pack(relos.Where(s=>!Detached.Contains((int)s)&&!browserSlots.Contains(s))),ImportsBuffer=native.ImportsBuffer.ToArray()};
        var rootReferences=ReferenceTable(source,null);
        // Reborn: inline descriptions and optional pointed names use the same authored-text contract on variable graphs.
        var rootComparer=new SelectedNativeGraphComparer(originalStock,rootReferences,root,instance,requireAllReferences:false,rootText:GameObjectRootTextContract.Read(source),authoredValues:authoredValues,rootImports:rootImports){AuthoredRecordValues=recordValues};
        var selection=rootComparer.CompareRootSelection(stock.Projection.NativeRoot);
        var covered=selection.Covered;var ownedRelos=new HashSet<uint>(selection.Relocations);var ownedImports=new HashSet<uint>(selection.Imports);
        // Reborn: source-sized armor storage owns its actual array/imports separately from the detached typed root.
        var armor=ConscriptArmorSetContract.VerifyNative(source,b,instance);if(armor.Length>0)Cover(armor.Start,armor.Length);foreach(uint slot in armor.Relocations)Add(ownedRelos,slot);foreach(uint slot in armor.Imports)Add(ownedImports,slot);
        // Reborn: variable movement arrays separately own their real bytes, relocations and source-bound import occurrences.
        var movement=ConscriptLocomotorSetContract.VerifyNative(source,b,instance);if(movement.Length>0)Cover(movement.Start,movement.Length);foreach(uint slot in movement.Relocations)Add(ownedRelos,slot);foreach(uint slot in movement.Imports)Add(ownedImports,slot);
        if(browser.Count!=0){Cover(browser.Items,browser.End-browser.Items);foreach(uint slot in browser.Relocations)Add(ownedRelos,slot);}
        var baseBytes=b.AsSpan(0,cut).ToArray();foreach(var field in Ep1GameObjectRootOwnersProjection.Fields)Ep1GameObjectRootOwnersProjection.ClearSlot(baseBytes,field);
        Put(baseBytes,Ep1GameObjectBrowser.CountSlot,0);Put(baseBytes,Ep1GameObjectBrowser.ItemsSlot,0);
        var layout=Ep1GameObjectStringTokens.Inspect(baseBytes,relos.Where(s=>s<cut&&!browserSlots.Contains(s)&&!Ep1GameObjectRootOwnersProjection.Fields.Any(f=>f.PointerSlot==s)).ToArray(),imports.Where(s=>s<cut).ToArray(),true);
        var tokens=supplied??Ep1GameObjectEmptyRootProjection.ComposeOriginal(native,official);int prefix=tokens.InstanceBuffer.Length-b.Length;BehaviorContainerStockProof.VerifyTail(tokens,native,prefix);
        var rows=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0);var stockRows=GameObjectStockReview.Tokens(stock.Bytes,0);
        if(!rows.Keys.Order().SequenceEqual(stockRows.Keys.Order())||rows.Any(p=>p.Value.Type!=stockRows[p.Key].Type))throw new InvalidDataException("Variable graph root metadata fields differ.");
        var maps=new Dictionary<string,Dictionary<int,int>>(StringComparer.Ordinal);
        // Reborn: variable pointer-list metadata is verified leaf-by-leaf against independent stock tables, even for repeated templates.
        var containerLengths=new Dictionary<string,int>(StringComparer.Ordinal);
        foreach(string name in ConscriptVariableContainers.Names){
            int slot=name=="Draws"?472:name=="ClientUpdates"?492:496;uint pointer=Word(b,slot);var owner=ConscriptVariableContainers.Find(source,name);
            if(owner==null){if(pointer!=0)throw new InvalidDataException("Omitted module container retains native allocation: "+name);containerLengths.Add(name,0);continue;}
            // Reborn: source presence/count binds native storage before any narrowing or dictionary lookup, including malformed empty-list pointers.
            if(pointer<600||pointer>b.Length-8||pointer%4!=0)throw new InvalidDataException("Module container native pointer bounds/alignment differ: "+name);
            int header=(int)pointer,expectedCount=owner.ChildNodes.OfType<XmlElement>().Count();if(Word(b,header)!=expectedCount)throw new InvalidDataException("Source/native module container count differs: "+name);
            if(expectedCount==0){if(Word(b,header+4)!=0)throw new InvalidDataException("Empty module container native header differs: "+name);Cover(header,8);Add(ownedRelos,(uint)slot);VerifyContainerHeader(name,header,0);containerLengths.Add(name,12);}
        }
        // Reborn: retain the separately proved PropBody fragment for the direct root Body metadata row.
        (Chunk Tokens,int Prefix,int Start,int End)? propBodyPiece=null;
        var fixedGraphs=new Dictionary<int,(int Start,int End,Type Type,SelectedNativeGraphComparer Comparer)>();int behaviorLength=0,aiLength=0;XmlDocument? resolved=null;
        foreach(var group in ConscriptModuleScalarContract.Owners(source).GroupBy(o=>o.Container))
        {
            var owners=group.ToArray();int header=group.Key=="Body"?-1:group.Key=="AI"?480:(int)Word(b,group.Key switch{"Draws"=>472,"Behaviors"=>476,"ClientUpdates"=>492,"ClientBehaviors"=>496,_=>throw new InvalidDataException("Unknown module container.")});
            int end=group.Key switch{"Draws"=>layout.Draws.End,"Behaviors"=>layout.Body,"AI"=>layout.Ai.Modules[^1].End,"Body"=>layout.Geometry,_=>layout.Clients.Single(c=>c.Slot==(group.Key=="ClientUpdates"?492:496)).End};
            if(header>=0){if(Word(b,header)!=owners.Length)throw new InvalidDataException("Source/native module count differs.");Cover(header,8,group.Key=="AI");int items=(int)Word(b,header+4);Cover(items,owners.Length*4);Add(ownedRelos,(uint)(header+4));for(int i=0;i<owners.Length;i++)Add(ownedRelos,(uint)(items+i*4));}
            if(group.Key!="AI")Add(ownedRelos,(uint)(group.Key switch{"Body"=>488,"Draws"=>472,"Behaviors"=>476,"ClientUpdates"=>492,_=>496}));
            var map=new Dictionary<int,int>();var stockContainer=topology.Containers.Single(c=>c.Name==group.Key);
            bool variableContainer=ConscriptVariableContainers.Supports(group.Key);
            if(group.Key!="Behaviors"&&!variableContainer&&header>=0){for(int i=0;i<8;i++)map.Add(stockContainer.CountAt+i,header+i);int items=(int)Word(b,header+4);for(int i=0;i<owners.Length*4;i++)map.Add(stockContainer.Items+i,items+i);}
            int containerAt=variableContainer?rows[FastHash.GetHashCode(group.Key)].Offset:0;
            int containerNext=variableContainer?containerAt+12+owners.Length*12:0;
            if(variableContainer)VerifyContainerHeader(group.Key,header,owners.Length);
            int behaviorAt=group.Key=="Behaviors"?rows[FastHash.GetHashCode("Behaviors")].Offset:0;
            int behaviorNext=behaviorAt+12+owners.Length*12;
            int aiAt=group.Key=="AI"?rows[FastHash.GetHashCode("AI")].Offset:0,aiNext=aiAt+12+owners.Length*12;
            if(group.Key=="AI")VerifyContainerHeader("AI",480,owners.Length);
            foreach(var owner in owners)
            {
                int start=ConscriptModuleScalarContract.Address(b,0,owner),stop=owner.Index+1<owners.Length?ConscriptModuleScalarContract.Address(b,0,owners[owner.Index+1]):end;
                // Reborn: PropBody is an eight-byte Body leaf with source/native proof and a complete root metadata splice.
                if(owner.NativeType==typeof(PropBodyModuleData)){
                    var grammar=propBody.Value;var bodyNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container=="Body").Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,PropBodyContract.TypeId,bodyNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.TokenizePropBody(bodyNative,official);int size=grammar.Verify(metadata,bodyNative,owner.NativeType,proof.Records);Own(bodyNative,start,stop);propBodyPiece=(metadata,size,start,stop);continue;
                }
                // Reborn: own every authored assign-slaves field, import and allocation through the shared schema proof, then check metadata against stock.
                if(owner.NativeType==typeof(AssignSlavesTargetObjectSpecialPowerModuleData)){
                    var grammar=assignSlaves.Value;var assignNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,AssignSlavesSpecialPowerContract.TypeId,assignNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(assignNative,official,AssignSlavesSpecialPowerContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,assignNative,owner.NativeType,proof.Records);Own(assignNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: every airfield scalar, optional filter/vector, string and import is source-owned; generated metadata follows retail class tables.
                if(AirfieldBehaviorContract.Supports(owner.NativeType)){
                    var grammar=airfield[owner.NativeType].Value;var airfieldNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;string wire=AirfieldBehaviorContract.Wire(owner.NativeType);
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,FastHash.GetHashCode(wire),airfieldNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(airfieldNative,official,wire,owner.NativeType);int size=grammar.Verify(metadata,airfieldNative,owner.NativeType,proof.Records);Own(airfieldNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: variable death masks, optional offsets, probability rows and weapon/upgrade imports are checked against the actual authored source.
                // Reborn: preserve source-sized money lists and their independent inherited retail metadata during GameObject composition.
                // Reborn: composed slaughter leaves retain source-owned inherited fields, filters, refund amounts and real FX imports.
                // Reborn: production-info markers are source-owned module identities, including authored IDs and multiple instances.
                // Reborn: self-replacement preserves authored target lists, flags, timing and inherited ability fields through complete-object composition.
                // Reborn: wall builders preserve all source-owned fields, filter lists and relocated status masks during complete-object composition.
                // Reborn: source-sized turret weapons retain every nested AI, selector and strong import during object composition.
                if(owner.NativeType==typeof(Ep1WeaponSetUpdateModuleData)&&owner.Element.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName=="WeaponSlotTurret")){
                    var grammar=weaponTurret.Value;var turretNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,WeaponTurretContract.TypeId,turretNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(turretNative,official,WeaponTurretContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,turretNative,owner.NativeType,proof.Records);Own(turretNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: preserve authored timer-monitor imports and flags through complete GameObject composition.
                // Reborn: preserve authored radar identity through the full object allocator and token metadata.
                // Reborn: compose complete power-manager records including both strong reference slots.
                // Reborn: preserve complete production-bonus source values and inherited special-power references.
                // Reborn: retain radius, pair ID, OCL import and weak target marker during full-object composition.
                // Reborn: preserve the teleport flag, paired storage ID, OCL and inherited power data through object composition.
                // Reborn: compose the complete teleport-update payload with both optional filter graphs and shader import.
                // Reborn: compose source-owned OCL powers with complete EP1 location and upgrade/filter graphs.
                // Reborn: preserve the complete player upgrade list and inherited power values during GameObject composition.
                // Reborn: preserve the template-ID store identity through complete GameObject native and metadata composition.
                // Reborn: preserve all EP1 weapon-fire fields and the inherited ability payload in complete-object composition.
                if(owner.NativeType==typeof(Ep1WeaponFireSpecialAbilityUpdateModuleData)){
                    var grammar=weaponFireAbility.Value;var abilityNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,WeaponFireAbilityContract.TypeId,abilityNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(abilityNative,official,WeaponFireAbilityContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,abilityNative,owner.NativeType,proof.Records);Own(abilityNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(StoreTemplateIDBehaviorModuleData)){
                    var grammar=storeTemplate.Value;var storeNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,StoreTemplateIDContract.TypeId,storeNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(storeNative,official,StoreTemplateIDContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,storeNative,owner.NativeType,proof.Records);Own(storeNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(PlayerUpgradeSpecialPowerModuleData)){
                    var grammar=playerUpgrade.Value;var upgradeNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,PlayerUpgradePowerContract.TypeId,upgradeNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(upgradeNative,official,PlayerUpgradePowerContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,upgradeNative,owner.NativeType,proof.Records);Own(upgradeNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(Ep1OCLSpecialPowerModuleData)){
                    var grammar=oclPower.Value;var oclNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,OclSpecialPowerContract.TypeId,oclNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(oclNative,official,OclSpecialPowerContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,oclNative,owner.NativeType,proof.Records);Own(oclNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(TeleportStoredObjectsUpdateModuleData)){
                    var grammar=teleportUpdate.Value;var updateNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,TeleportStoredUpdateContract.TypeId,updateNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(updateNative,official,TeleportStoredUpdateContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,updateNative,owner.NativeType,proof.Records);Own(updateNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(TeleportStoredObjectsSpecialPowerModuleData)){
                    var grammar=teleportStored.Value;var teleportNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,TeleportStoredObjectsContract.TypeId,teleportNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(teleportNative,official,TeleportStoredObjectsContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,teleportNative,owner.NativeType,proof.Records);Own(teleportNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: complete base-power filters and optional records use the same source-sized graph composer as derived powers.
                if(owner.NativeType==typeof(SpecialPowerModuleData)){
                    var grammar=specialPower.Value;var powerNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,SpecialPowerContract.TypeId,powerNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(powerNative,official,SpecialPowerContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,powerNative,owner.NativeType,proof.Records);Own(powerNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(StoreObjectsSpecialPowerModuleData)){
                    var grammar=storeObjects.Value;var storeNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,StoreObjectsContract.TypeId,storeNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(storeNative,official,StoreObjectsContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,storeNative,owner.NativeType,proof.Records);Own(storeNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(GiveProductionBonusSpecialPowerModuleData)){
                    var grammar=productionBonus.Value;var bonusNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,ProductionBonusContract.TypeId,bonusNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(bonusNative,official,ProductionBonusContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,bonusNative,owner.NativeType,proof.Records);Own(bonusNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(Ep1PlayerPowerManagerBehaviorModuleData)){
                    var grammar=playerPowerManager.Value;var managerNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,PlayerPowerManagerContract.TypeId,managerNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(managerNative,official,PlayerPowerManagerContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,managerNative,owner.NativeType,proof.Records);Own(managerNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(RadarBehaviorModuleData)){
                    var grammar=radar.Value;var radarNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,RadarBehaviorContract.TypeId,radarNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(radarNative,official,RadarBehaviorContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,radarNative,owner.NativeType,proof.Records);Own(radarNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: splice only independently proved notification bytes and metadata into the actual authored module order.
                // Reborn: independently prove each source-sized visibility/texture/animation nugget before shared object composition.
                if(owner.NativeType==typeof(Ep1SubObjectsUpgradeModuleData)){
                    var grammar=subObjectsUpgrade.Value;var subobjectsNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,SubObjectsUpgradeContract.TypeId,subobjectsNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(subobjectsNative,official,SubObjectsUpgradeContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,subobjectsNative,owner.NativeType,proof.Records);Own(subobjectsNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: source-sized transport graphs retain every inherited and own passenger field.
                // Reborn: compose the actual authored modifier upgrade through the full source/native proof.
                // Reborn: compose each source-sized passenger-ejection graph with independent inherited/own field proof.
                // Reborn: preserve all authored waypoint and flight/death/trigger fields during object composition.
                // Reborn: independently prove authored assistance masks and slots before whole-object metadata composition.
                // Reborn: compose independently proved garrison and inherited passenger graphs in actual authored module order.
                // Reborn: prove the actual template identity and import occurrence before composing invisibility behavior metadata.
                // Reborn: compose the complete authored aura graph after independent source and metadata verification.
                // Reborn: compose each independently proved detector in its actual authored module position.
                // Reborn: compose source-sized death phases and optional masks after complete independent source/native proof.
                // Reborn: compose independently proved aiming defaults and edited values in actual source order.
                // Reborn: prove collision fields, references, optional records and retail metadata before splicing.
                if(owner.NativeType==typeof(Ep1FireWeaponCollideModuleData)){
                    var grammar=fireWeaponCollide.Value;var collideNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,FireWeaponCollideContract.TypeId,collideNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(collideNative,official,FireWeaponCollideContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,collideNative,owner.NativeType,proof.Records);Own(collideNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(AimWeaponBehaviorModuleData)){
                    var grammar=aimWeapon.Value;var aimNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,AimWeaponContract.TypeId,aimNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(aimNative,official,AimWeaponContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,aimNative,owner.NativeType,proof.Records);Own(aimNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(SlowDeathBehaviorModuleData)){
                    var grammar=slowDeath.Value;var deathNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,SlowDeathContract.TypeId,deathNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(deathNative,official,SlowDeathContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,deathNative,owner.NativeType,proof.Records);Own(deathNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(Ep1StealthDetectorUpdateModuleData)){
                    var grammar=stealthDetector.Value;var detectorNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,StealthDetectorContract.TypeId,detectorNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(detectorNative,official,StealthDetectorContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,detectorNative,owner.NativeType,proof.Records);Own(detectorNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(Ep1AttributeModifierAuraUpdateModuleData)){
                    var grammar=attributeModifierAura.Value;var auraNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,AttributeModifierAuraContract.TypeId,auraNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(auraNative,official,AttributeModifierAuraContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,auraNative,owner.NativeType,proof.Records);Own(auraNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(InvisibilityBehaviorModuleData)){
                    var grammar=invisibilityBehavior.Value;var invisibilityNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,InvisibilityBehaviorContract.TypeId,invisibilityNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(invisibilityNative,official,InvisibilityBehaviorContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,invisibilityNative,owner.NativeType,proof.Records);Own(invisibilityNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(GarrisonContainModuleData)){
                    var grammar=garrisonContain.Value;var garrisonNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,GarrisonContainContract.TypeId,garrisonNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(garrisonNative,official,GarrisonContainContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,garrisonNative,owner.NativeType,proof.Records);Own(garrisonNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(AttackAssistResponseBehaviorModuleData)){
                    var grammar=attackAssistResponse.Value;var assistNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,AttackAssistResponseContract.TypeId,assistNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(assistNative,official,AttackAssistResponseContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,assistNative,owner.NativeType,proof.Records);Own(assistNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(RunOffMapBehaviorModuleData)){
                    var grammar=runOffMap.Value;var runoffNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,RunOffMapContract.TypeId,runoffNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(runoffNative,official,RunOffMapContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,runoffNative,owner.NativeType,proof.Records);Own(runoffNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(EjectPassengersSpecialAbilityUpdateModuleData)){
                    var grammar=ejectPassengers.Value;var ejectionNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,EjectPassengersContract.TypeId,ejectionNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(ejectionNative,official,EjectPassengersContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,ejectionNative,owner.NativeType,proof.Records);Own(ejectionNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(AttributeModifierUpgradeModuleData)){
                    var grammar=attributeModifierUpgrade.Value;var upgradeNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,AttributeModifierUpgradeContract.TypeId,upgradeNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(upgradeNative,official,AttributeModifierUpgradeContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,upgradeNative,owner.NativeType,proof.Records);Own(upgradeNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(TransportContainModuleData)){
                    var grammar=transportContain.Value;var transportNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,TransportContainContract.TypeId,transportNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(transportNative,official,TransportContainContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,transportNative,owner.NativeType,proof.Records);Own(transportNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(NotifyWhenAttackingBehaviorModuleData)){
                    var grammar=notifyWhenAttacking.Value;var notificationNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,NotifyWhenAttackingContract.TypeId,notificationNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(notificationNative,official,NotifyWhenAttackingContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,notificationNative,owner.NativeType,proof.Records);Own(notificationNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(MonitorSpecialPowerTimerUpdateModuleData)){
                    var grammar=monitorSpecialPowerTimer.Value;var monitorNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,MonitorSpecialPowerTimerContract.TypeId,monitorNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(monitorNative,official,MonitorSpecialPowerTimerContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,monitorNative,owner.NativeType,proof.Records);Own(monitorNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(WallBuilderBehaviorModuleData)){
                    var grammar=wallBuilder.Value;var wallNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,WallBuilderContract.TypeId,wallNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(wallNative,official,WallBuilderContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,wallNative,owner.NativeType,proof.Records);Own(wallNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(ReplaceSelfSpecialAbilityModuleData)){
                    var grammar=replaceSelf.Value;var replaceNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,ReplaceSelfContract.TypeId,replaceNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(replaceNative,official,ReplaceSelfContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,replaceNative,owner.NativeType,proof.Records);Own(replaceNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(ProductionUpdateInfoModuleData)){
                    var grammar=productionInfo.Value;var infoNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,ProductionUpdateInfoContract.TypeId,infoNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(infoNative,official,ProductionUpdateInfoContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,infoNative,owner.NativeType,proof.Records);Own(infoNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(SlaughterHordeContainModuleData)){
                    var grammar=slaughter.Value;var slaughterNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,SlaughterContainContract.TypeId,slaughterNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(slaughterNative,official,SlaughterContainContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,slaughterNative,owner.NativeType,proof.Records);Own(slaughterNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(Ep1AutoDepositUpdateModuleData)){
                    var grammar=autoDeposit.Value;var depositNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,AutoDepositContract.TypeId,depositNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(depositNative,official,AutoDepositContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,depositNative,owner.NativeType,proof.Records);Own(depositNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                if(owner.NativeType==typeof(Ep1FireWeaponWhenDeadBehaviorModuleData)){
                    var grammar=fireWeaponDeath.Value;var deathWeaponNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,FireWeaponDeathContract.TypeId,deathWeaponNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(deathWeaponNative,official,FireWeaponDeathContract.Wire,owner.NativeType);int size=grammar.Verify(metadata,deathWeaponNative,owner.NativeType,proof.Records);Own(deathWeaponNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: chooser presence and all source-bound native fields/imports own a source-sized AI metadata fragment.
                if(group.Key=="AI"){
                    var aiNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container=="AI"&&o.Index==owner.Index).Element;
                    // Reborn: preserve the concrete AI class while the existing inline list splice remains shared.
                    bool jet=owner.NativeType==typeof(Ep1JetAIUpdateModuleData);var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();var proof=jet?JetAIUpdateContract.Verify(element,aiNative,references):AIUpdateContract.Verify(element,aiNative,references);var aiTokens=jet?Ep1JetAIUpdateTokenizer.Tokenize(aiNative):Ep1AIUpdateTokenizer.Tokenize(aiNative);int size=(jet?jetAiGrammar:aiGrammar).Value.Verify(aiTokens,aiNative,jet?typeof(JetAIUpdateContract.Native):typeof(AIUpdateContract.Native),proof.Records);Own(aiNative,start,stop);
                    int marker=aiAt+12+owner.Index*12;if(Word(tokens.InstanceBuffer,marker)!=0xB151A0EB||Word(tokens.InstanceBuffer,marker+4)!=0||Word(tokens.InstanceBuffer,marker+8)!=aiNext)throw new InvalidDataException("AI metadata source order differs.");
                    for(int offset=0;offset<size;offset+=12){uint target=Word(aiTokens.InstanceBuffer,offset+8);uint expected=target==0?0:target<size?target+(uint)aiNext:target-(uint)size+(uint)(prefix+start);if(Word(tokens.InstanceBuffer,aiNext+offset)!=Word(aiTokens.InstanceBuffer,offset)||Word(tokens.InstanceBuffer,aiNext+offset+4)!=Word(aiTokens.InstanceBuffer,offset+4)||Word(tokens.InstanceBuffer,aiNext+offset+8)!=expected)throw new InvalidDataException("AI metadata splice differs.");}aiNext+=size;continue;
                }
                // Reborn: creation/destroy share source-bound death storage, with explicit OCL and strong upgrade list imports where applicable.
                if(DeathBehaviorContract.Supports(owner.NativeType)){
                    _=deathBehaviors.Value;var deathNative=Extract(native,start,stop);string wire=owner.Element.LocalName+"ModuleData";var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(deathNative,official,wire,owner.NativeType);
                    DeathBehaviorContract.Verify(owner.Element,deathNative,metadata,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray(),catalog);Own(deathNative,start,stop);Splice(owner,metadata,start,metadata.InstanceBuffer.Length-deathNative.InstanceBuffer.Length);continue;
                }
                // Reborn: source-sized secondary damage effects own optional RGB/status records and exact modifier imports.
                if(owner.NativeType==typeof(SecondaryDamageTrackerModuleData)){
                    _=secondaryDamage.Value;var trackerNative=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(trackerNative,official,"SecondaryDamageTrackerModuleData",owner.NativeType);
                    SecondaryDamageTrackerContract.Verify(owner.Element,trackerNative,metadata,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray(),catalog);Own(trackerNative,start,stop);Splice(owner,metadata,start,metadata.InstanceBuffer.Length-trackerNative.InstanceBuffer.Length);continue;
                }
                // Reborn: variable FX events and complete death conditions own their actual lists, masks, strings and semantic imports.
                if(owner.NativeType==typeof(FXListBehaviorModuleData)){
                    _=fxList.Value;var fxNative=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(fxNative,official,"FXListBehaviorModuleData",owner.NativeType);
                    FXListBehaviorContract.Verify(owner.Element,fxNative,metadata,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray(),catalog);Own(fxNative,start,stop);Splice(owner,metadata,start,metadata.InstanceBuffer.Length-fxNative.InstanceBuffer.Length);continue;
                }
                // Reborn: unpack owns a fixed header and an optional concrete audio pointee with exact Core/import identity.
                if(owner.NativeType==typeof(Ep1StructureUnpackUpdateModuleData)){
                    _=unpack.Value;var unpackedNative=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(unpackedNative,official,"StructureUnpackUpdateModuleData",owner.NativeType);
                    StructureUnpackContract.Verify(owner.Element,unpackedNative,metadata,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray(),official,catalog);Own(unpackedNative,start,stop);Splice(owner,metadata,start,204);continue;
                }
                // Reborn: production upgrade translations use source-sized optional/list allocations, real imports and stock-learned nested metadata.
                if(owner.NativeType==typeof(Ep1ProductionUpdateModuleData)){
                    var grammar=production.Value;var produced=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;
                    var proof=SchemaNativeContract.Verify(element,owner.NativeType,0x6EC75439u,produced,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(produced,official,"ProductionUpdateModuleData",owner.NativeType);int size=grammar.Verify(metadata,produced,owner.NativeType,proof.Records);Own(produced,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: BuildingBehavior owns four independent optional strings and their exact relocations; these are subobject names, not asset imports.
                if(owner.NativeType==typeof(BuildingBehaviorModuleData)){
                    var buildingNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container==owner.Container&&o.Index==owner.Index).Element;var proof=SchemaNativeContract.Verify(element,owner.NativeType,FastHash.GetHashCode("BuildingBehaviorModuleData"),buildingNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());
                    var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(buildingNative,official,"BuildingBehaviorModuleData",owner.NativeType);int size=buildingBehavior.Value.Verify(metadata,buildingNative,owner.NativeType,proof.Records);Own(buildingNative,start,stop);Splice(owner,metadata,start,size);continue;
                }
                // Reborn: LUAEventList owns exactly one name string; no imports.
                if(owner.NativeType==typeof(LUAEventListBehaviorModuleData)){
                    _=luaEventList.Value;var listed=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(listed,official,"LUAEventListBehaviorModuleData",owner.NativeType);
                    LUAEventListContract.Verify(owner.Element,listed,metadata);Own(listed,start,stop);Splice(owner,metadata,start,LUAEventListContract.MetadataSize);continue;
                }
                // Reborn: structure placement owns its optional shape strings and Offset vector; no imports.
                if(owner.NativeType==typeof(StructurePlacementBehaviorModuleData)){
                    _=structurePlacement.Value;var placedShape=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(placedShape,official,"StructurePlacementBehaviorModuleData",owner.NativeType);
                    StructurePlacementContract.Verify(owner.Element,placedShape,metadata);Own(placedShape,start,stop);Splice(owner,metadata,start,StructurePlacementContract.MetadataSize);continue;
                }
                // Reborn: infiltrator owns inline strong imports plus two optional filter-reference pointees.
                if(owner.NativeType==typeof(InfiltratorContainModuleData)){
                    _=infiltrator.Value;var infiltrated=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(infiltrated,official,"InfiltratorContainModuleData",owner.NativeType);
                    InfiltratorContainContract.Verify(owner.Element,infiltrated,metadata,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray(),official,catalog);Own(infiltrated,start,stop);Splice(owner,metadata,start,InfiltratorContainContract.MetadataSize);continue;
                }
                // Reborn: assisted placement owns only its optional filter allocation; no imports.
                if(owner.NativeType==typeof(AssistedPlacementBehaviorModuleData)){
                    _=placement.Value;var placed=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(placed,official,"AssistedPlacementBehaviorModuleData",owner.NativeType);
                    AssistedPlacementContract.Verify(owner.Element,placed,metadata);Own(placed,start,stop);Splice(owner,metadata,start,metadata.InstanceBuffer.Length-placed.InstanceBuffer.Length);continue;
                }
                // Reborn: engineer owns optional voice/FX/filter allocations and strong FX/EVA/OCL imports with exact Core indices.
                if(owner.NativeType==typeof(Ep1EngineerContainModuleData)){
                    _=engineer.Value;var engineered=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(engineered,official,"EngineerContainModuleData",owner.NativeType);
                    EngineerContainContract.Verify(owner.Element,engineered,metadata,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray(),official,catalog);Own(engineered,start,stop);Splice(owner,metadata,start,metadata.InstanceBuffer.Length-engineered.InstanceBuffer.Length);continue;
                }
                // Reborn: repair is wholly inline; scalars, inline percents, empty audio and literal metadata are independently proved.
                if(owner.NativeType==typeof(Ep1RepairUpdateModuleData)){
                    _=repair.Value;var repaired=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(repaired,official,"RepairUpdateModuleData",owner.NativeType);RepairUpdateContract.Verify(owner.Element,repaired,metadata);Own(repaired,start,stop);Splice(owner,metadata,start,RepairUpdateContract.MetadataSize);continue;
                }
                // Reborn: Spawn's full source-bound graph includes weak lists, Die/status allocations and inherited upgrade storage.
                if(owner.NativeType==typeof(Ep1SpawnBehaviorModuleData)){
                    _=spawn.Value;var spawnedNative=Extract(native,start,stop);SpawnBehaviorContract.Verify(owner.Element,spawnedNative,catalog);Own(spawnedNative,start,stop);
                    var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(spawnedNative,official,"SpawnBehaviorModuleData",owner.NativeType);Splice(owner,metadata,start,metadata.InstanceBuffer.Length-spawnedNative.InstanceBuffer.Length);continue;
                }
                // Reborn: queue-exit has no heap/imports; each scalar, vector, packed bool and metadata target is independently proved.
                if(owner.NativeType==typeof(Ep1QueueProductionExitUpdateModuleData)){
                    _=queueExit.Value;var exit=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(exit,official,"QueueProductionExitUpdateModuleData",owner.NativeType);QueueProductionExitContract.Verify(owner.Element,exit,metadata);Own(exit,start,stop);Splice(owner,metadata,start,252);continue;
                }
                // Reborn: docking has a stock-qualified 52-byte header and independently owned source-bound strings, not a Conscript template dependency.
                if(owner.NativeType==typeof(QueueDockModuleData)){
                    _=queueDock.Value;var docking=Extract(native,start,stop);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(docking,official,"QueueDockModuleData",owner.NativeType);QueueDockContract.Verify(owner.Element,docking,metadata);Own(docking,start,stop);Splice(owner,metadata,start,192);continue;
                }
                // Reborn: variable destruction items have a complete source-bound native proof and an independent literal metadata writer already compared to stock.
                if(owner.NativeType==typeof(BuildingDestructionModuleData)){
                    var destruction=Extract(native,start,stop);BuildingDestructionContract.Verify(owner.Element,destruction,instance,catalog);Own(destruction,start,stop);
                    var generated=Ep1BehaviorMetadataTokenizer.Tokenize(destruction,official,"BuildingDestructionModuleData",owner.NativeType);var literal=Ep1DestructionTokenizer.Tokenize(destruction);
                    if(!Equal(generated,literal))throw new InvalidDataException("Generic/literal destruction metadata differ.");
                    Splice(owner,generated,start,generated.InstanceBuffer.Length-destruction.InstanceBuffer.Length);continue;
                }
                // Reborn: fixed sell values remain source-verified; only layout/metadata/padding come from the independent building witness.
                // Reborn: a schema-proved leaf owns its exact native graph and its source-sized metadata through independent stock-qualified proofs.
                if(variableContainer&&origins[owner.Element.GetAttribute("id")]==SchemaNativeContract.Origin){
                    if(!SchemaNativeContract.Supports(owner.NativeType))throw new InvalidDataException("Schema-proved origin for an unsupported family.");
                    var family=SchemaNativeContract.For(owner.NativeType);var grammar=schemaFamilies[owner.NativeType].Value;var leafNative=Extract(native,start,stop);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);
                    var element=ConscriptVariableContainers.Find(resolved,group.Key)!.ChildNodes.OfType<XmlElement>().ElementAt(owner.Index);if(element.GetAttribute("id")!=owner.Element.GetAttribute("id"))throw new InvalidDataException("Schema-proved source identity/order differs.");
                    var proved=SchemaNativeContract.Verify(element,owner.NativeType,family.TypeId,leafNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());Own(leafNative,start,stop);
                    var leafTokens=family.Leaf(leafNative);int leafPrefix=grammar.Verify(leafTokens,leafNative,owner.NativeType,proved.Records);
                    int leafRow=containerAt+12+owner.Index*12;
                    if(Word(tokens.InstanceBuffer,leafRow)!=0xB151A0EB||Word(tokens.InstanceBuffer,leafRow+4)!=0||Word(tokens.InstanceBuffer,leafRow+8)!=containerNext)throw new InvalidDataException("Variable module container wrapper/order differs: "+group.Key);
                    for(int at=0;at<leafPrefix;at+=12){uint target=Word(leafTokens.InstanceBuffer,at+8);uint expected=target==0?0:target<leafPrefix?target+(uint)containerNext:target-(uint)leafPrefix+(uint)(prefix+start);
                        if(Word(tokens.InstanceBuffer,containerNext+at)!=Word(leafTokens.InstanceBuffer,at)||Word(tokens.InstanceBuffer,containerNext+at+4)!=Word(leafTokens.InstanceBuffer,at+4)||Word(tokens.InstanceBuffer,containerNext+at+8)!=expected)throw new InvalidDataException("Schema-proved metadata splice differs: "+group.Key);}
                    containerNext+=leafPrefix;continue;
                }
                bool selling=owner.NativeType==typeof(StructureSellUpdateModuleData);bool powered=owner.NativeType==typeof(PowerUpgradeModuleData);var moduleStock=selling?sell.Value.Bytes:powered?powerUpgrade.Value.Bytes:stock.Bytes;
                var model=selling?sell.Value.Module:powered?powerUpgrade.Value.Module:templates[origins[owner.Element.GetAttribute("id")]];
                if(Word(b,start)!=model.TypeId||Word(b,start+4)!=FastHash.GetHashCode(owner.Element.GetAttribute("id").ToLowerInvariant()))throw new InvalidDataException("Source module type/local identity differs.");
                var leaf=Extract(native,start,stop);var compared=Clone(leaf);Put(compared.InstanceBuffer,4,Word(moduleStock,model.Native+4));
                // Reborn: fixed source/native values were independently checked; nested graphs and padding remain compared in full.
                foreach(var binding in ConscriptModuleScalarContract.Bindings(owner.NativeType))moduleStock.AsSpan(model.Native+binding.Offset,binding.Width).CopyTo(compared.InstanceBuffer.AsSpan(binding.Offset,binding.Width));
                // Reborn: source-sized weak arrays and optional nested animation records have explicit semantic and allocation contracts.
                var upgrade=group.Key=="Behaviors"?ConscriptUpgradeStructure.Read(owner,catalog):null;
                bool weapon=group.Key=="Behaviors"&&ConscriptWeaponSetContract.Supports(owner.NativeType);
                if(upgrade==null&&!weapon&&!selling)VerifyUpgradeTriggers(owner,origins[owner.Element.GetAttribute("id")],compared);
                // Reborn: the template supplies ABI/ownership, not a frozen referenced asset identity; source supplies checked semantic targets.
                // Reborn: implemented hardpoints have a complete source/native contract, so arbitrary new rows need no stock-shaped reference table.
                SelectedNativeGraphComparer? comparer=null;
                if(weapon)ConscriptWeaponSetContract.VerifyNative(owner.Element,leaf,instance);
                else{var expectedReferences=(group.Key=="Behaviors"||variableContainer)&&leaf.ImportsBuffer.Length>0?ReferenceTable(owner.Element,origins[owner.Element.GetAttribute("id")]):rootReferences;comparer=new SelectedNativeGraphComparer(moduleStock,expectedReferences,compared,instance,allowScriptCrLfEquivalent:group.Key=="Draws",requireAllReferences:false,captureNativeCoordinates:group.Key!="Behaviors",captureNativePaths:group.Key=="Behaviors",upgrade:upgrade);comparer.Compare(owner.NativeType,model.Native);if(!variableContainer)foreach(var coordinate in comparer.NativeOffsets)map.Add(coordinate.Key,start+coordinate.Value);}
                Own(leaf,start,stop);
                if(variableContainer){
                    // Reborn: repeated source instances get separate wrapper/table/native coordinates, not duplicate keys in a whole-stock map.
                    int stockIndex=Array.IndexOf(stockContainer.Modules,model);int stockEnd=stockIndex+1<stockContainer.Modules.Length?stockContainer.Modules[stockIndex+1].Wrapper:stockRows.Values.Select(v=>v.Offset).Where(at=>at>stockRows[FastHash.GetHashCode(group.Key)].Offset&&at<stock.Projection.NativeRoot).DefaultIfEmpty(stock.Projection.NativeRoot).Min();
                    int length=stockEnd-model.Wrapper,row=containerAt+12+owner.Index*12;
                    if(Word(tokens.InstanceBuffer,row)!=0xB151A0EB||Word(tokens.InstanceBuffer,row+4)!=0||Word(tokens.InstanceBuffer,row+8)!=containerNext)throw new InvalidDataException("Variable module container wrapper/order differs: "+group.Key);
                    var expected=stock.Bytes.AsSpan(model.Wrapper,length).ToArray();
                    for(int at=8;at<length;at+=12){uint target=Word(expected,at);if(target!=0)Put(expected,at,target>=model.Wrapper&&target<stockEnd?target-(uint)model.Wrapper+(uint)containerNext:comparer!.NativeOffsets.TryGetValue((int)target,out int coordinate)?(uint)(prefix+start+coordinate):throw new InvalidDataException("Variable module metadata target is not independently native-bound: "+group.Key));}
                    if(!tokens.InstanceBuffer.AsSpan(containerNext,length).SequenceEqual(expected))throw new InvalidDataException("Variable module stock metadata differs: "+group.Key);containerNext+=length;
                }
                if(group.Key=="Behaviors")
                {
                    var standalone=Ep1BehaviorMetadataTokenizer.Tokenize(leaf,official,model.Type,owner.NativeType);var stockModules=topology.Containers.Single(c=>c.Name=="Behaviors").Modules;int index=Array.IndexOf(stockModules,model);int stockEnd=selling?sell.Value.End:powered?powerUpgrade.Value.End:index+1<stockModules.Length?stockModules[index+1].Wrapper:stockRows[FastHash.GetHashCode("AI")].Offset;
                    int localPrefix;
                    if(weapon){
                        // Reborn: the independent literal stock-qualified hardpoint writer cross-checks generic schema metadata at source-sized extents.
                        var literal=Ep1WeaponSetTokenizer.Tokenize(leaf);if(!Equal(standalone,literal))throw new InvalidDataException("Generic/literal EP1 weapon metadata disagree.");Ep1WeaponSetSmokeTest.Verify(standalone,leaf,instance,catalog,false);localPrefix=standalone.InstanceBuffer.Length-leaf.InstanceBuffer.Length;
                    }else localPrefix=upgrade==null?BehaviorMetadataStockProof.Compare(standalone,leaf,moduleStock,model.Wrapper,stockEnd,owner.NativeType,comparer!.NativeRecords):UpgradeStructureMetadataProof.Compare(standalone,leaf,moduleStock,model.Wrapper,stockEnd,owner.NativeType,comparer!.NativeRecords,upgrade);
                    Splice(owner,standalone,start,localPrefix);
                }
            }
            //-------------------------------------------------------------------------------------------------
            /** Reborn: verify each source-ordered module wrapper and every metadata target without borrowing offsets from another family. */
            //-------------------------------------------------------------------------------------------------
            void Splice(ConscriptModuleScalarContract.Owner owner,Chunk standalone,int start,int localPrefix){
                int list=rows[FastHash.GetHashCode("Behaviors")].Offset;if(Word(tokens.InstanceBuffer,list)!=0x6C827073||Word(tokens.InstanceBuffer,list+4)!=0x5D448FD8||Word(tokens.InstanceBuffer,list+8)!=prefix+layout.List)throw new InvalidDataException("Variable behavior count metadata differs.");
                int row=list+12+owner.Index*12,at=(int)Word(tokens.InstanceBuffer,row+8);if(Word(tokens.InstanceBuffer,row)!=0xB151A0EB||Word(tokens.InstanceBuffer,row+4)!=0||at!=behaviorNext)throw new InvalidDataException("Variable behavior wrapper/order differs.");
                for(int offset=0;offset<localPrefix;offset+=12){uint target=Word(standalone.InstanceBuffer,offset+8);uint expected=target==0?0:target<localPrefix?target+(uint)at:target-(uint)localPrefix+(uint)(prefix+start);if(Word(tokens.InstanceBuffer,at+offset)!=Word(standalone.InstanceBuffer,offset)||Word(tokens.InstanceBuffer,at+offset+4)!=Word(standalone.InstanceBuffer,offset+4)||Word(tokens.InstanceBuffer,at+offset+8)!=expected)throw new InvalidDataException("Variable behavior metadata splice differs.");}
                behaviorNext+=localPrefix;
            }
            if(group.Key=="AI")aiLength=aiNext-aiAt;else if(group.Key=="Behaviors")behaviorLength=behaviorNext-behaviorAt;else if(variableContainer)containerLengths.Add(group.Key,containerNext-containerAt);else maps.Add(group.Key,map);
        }
        // Reborn: source-sized shape/contact arrays and optional placement/vector records own the actual Geometry allocation.
        Add(ownedRelos,500);var geometryNative=Extract(native,layout.Geometry,cut);resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);
        var geometryProof=SchemaNativeContract.Verify(GameObjectGeometryContract.Owner(resolved)!,typeof(Ep1Geometry),0,geometryNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());
        var geometryTokens=Ep1GeometryTokenizer.Tokenize(geometryNative);int geometryLength=geometryGrammar.Value.Verify(geometryTokens,geometryNative,typeof(Ep1Geometry),geometryProof.Records);Own(geometryNative,layout.Geometry,cut);
        // Reborn: VisionInfo is optional; its absence owns a null word, while presence owns exactly forty source-bound native bytes.
        (Chunk Tokens,int Start,int End)? visionPiece=null;var visionOwner=ConscriptVariableContainers.Find(source,"VisionInfo");int visionAt=(int)Word(b,560);
        if((visionOwner==null)!=(visionAt==0))throw new InvalidDataException("VisionInfo source/native presence differs.");
        if(visionOwner!=null){
            if(visionAt<600||visionAt%4!=0||visionAt>b.Length-40)throw new InvalidDataException("VisionInfo native pointer bounds differ.");resolved??=SchemaNativeContract.Resolve(source,referenceSchemas,catalog);var visionNative=new Chunk{InstanceBuffer=b.AsSpan(visionAt,40).ToArray(),RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};var proof=SchemaNativeContract.Verify(ConscriptVariableContainers.Find(resolved,"VisionInfo")!,typeof(Ep1VisionInfo),0,visionNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var visionTokens=Ep1VisionInfoTokenizer.Tokenize(visionNative);int size=visionInfo.Value.Verify(visionTokens,visionNative,typeof(Ep1VisionInfo),proof.Records);if(size!=132)throw new InvalidDataException("VisionInfo metadata extent differs.");Add(ownedRelos,560);Own(visionNative,visionAt,visionAt+40);visionPiece=(visionTokens,visionAt,visionAt+40);
        }
        // Reborn: retain independently proved optional dependency metadata for the enclosing root splice check.
        Chunk? dependencyNative=null,dependencyTokens=null;int dependencyStart=0,dependencyEnd=0,dependencyLength=0;
        var rootPieces=new Dictionary<string,(int Start,int End,int Prefix,Chunk Tokens)>(StringComparer.Ordinal);
        for(int i=0;i<Ep1GameObjectRootOwnersProjection.Fields.Count;i++)
        {
            var field=Ep1GameObjectRootOwnersProjection.Fields[i];int start=Ep1GameObjectRootOwnersProjection.NativeStart(b,field),end=Ep1GameObjectRootOwnersProjection.Fields.Skip(i+1).Select(f=>Ep1GameObjectRootOwnersProjection.NativeStart(b,f)).FirstOrDefault(at=>at!=0);if(end==0)end=b.Length;
            // Reborn: source presence/cardinality, every native field/import and complete variable root metadata are independently bound.
            if(extendedRoots.TryGetValue(field.Name,out var grammar)){
                var elements=GameObjectExtendedRootContract.Owners(resolved,field.Name);if(!field.InlineList&&elements.Length==0){if(start!=0)throw new InvalidDataException("Omitted root owner has native allocation: "+field.Name);continue;}
                if(!field.InlineList&&start==0)throw new InvalidDataException("Authored root owner has no allocation: "+field.Name);
                var leaf=field.InlineList?Ep1GameObjectRootOwnersProjection.WrapInlineList(native,field,start,end):Extract(native,start,end);
                var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();var proof=field.InlineList?SchemaNativeContract.VerifyInlineList(resolved.DocumentElement!,field.Name,field.NativeType,leaf,references):SchemaNativeContract.Verify(elements.Single(),field.NativeType,0,leaf,references);
                var metadata=field.InlineList?Ep1BehaviorMetadataTokenizer.TokenizeRootList(leaf,official,field.Name):Ep1BehaviorMetadataTokenizer.TokenizeRootRecord(leaf,official,field.Name);int size=grammar.Value.Verify(metadata,leaf,field.NativeType,proof.Records,field.InlineList);rootPieces.Add(field.Name,(start,end,size,metadata));
                if(start!=0){Add(ownedRelos,(uint)field.PointerSlot);Own(Extract(native,start,end),start,end);}continue;
            }
            // Reborn: an omitted dependency retains null storage; authored empty/populated records own complete source-sized tails.
            if(field.Name=="GameDependency"){
                var element=GameObjectDependencyContract.Owner(resolved);if(element==null){if(start!=0)throw new InvalidDataException("Omitted GameDependency has native storage.");continue;}if(start==0)throw new InvalidDataException("Authored GameDependency has no native storage.");
                dependencyStart=start;dependencyEnd=end;dependencyNative=Extract(native,start,end);var proof=SchemaNativeContract.Verify(element,typeof(GameDependencyType),0,dependencyNative,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());
                dependencyTokens=Ep1BehaviorMetadataTokenizer.TokenizeRootRecord(dependencyNative,official,"GameDependency");dependencyLength=dependencyGrammar.Value.Verify(dependencyTokens,dependencyNative,typeof(GameDependencyType),proof.Records);Add(ownedRelos,412);Own(dependencyNative,start,end);continue;
            }
            // Reborn: resource presence and transaction cardinality have a dedicated source/native contract instead of complete equality to the stock one-row record.
            if(field.Name=="ObjectResourceInfo"){var resource=GameObjectResourceContract.VerifyNative(source,b,end);if(resource.Length>0)Cover(resource.Start,resource.Length);foreach(uint slot in resource.Relocations)Add(ownedRelos,slot);continue;}
            Add(ownedRelos,(uint)field.Slot);Fixed(field.NativeType,(int)Word(stock.Bytes,stock.Projection.NativeRoot+field.Slot),start,end);
        }
        if(!ownedRelos.Order().SequenceEqual(relos.Order())||!ownedImports.Order().SequenceEqual(imports.Order())||!imports.Select(s=>Word(b,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Variable graph auxiliary/reference ownership differs.");
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i])gap=0;else if(b[i]!=0||++gap>3)throw new InvalidDataException("Unowned variable graph native allocation.");}
        // Reborn: every non-behavior metadata owner retains its independent stock field map; changed behavior extents are source-composed, not fixed at 88824.
        int rootLength=(stockRows.Count+1)*12;var metadataOwned=new bool[prefix];Array.Fill(metadataOwned,true,0,rootLength);
        var oldTables=stockRows.Values.Select(v=>v.Offset).Where(at=>at>=rootLength&&at<stock.Projection.NativeRoot).Distinct().Order().ToArray();
        foreach(var pair in stockRows)
        {
            int old=pair.Value.Offset,at=rows[pair.Key].Offset;
            // Reborn: VisionInfo metadata follows source presence, not the populated verification baseline.
            if(pair.Key==FastHash.GetHashCode("VisionInfo")){
                if(visionPiece is not { } vision){if(at!=0)throw new InvalidDataException("Omitted VisionInfo metadata must be null.");}
                else{const int visionLength=132;if(at<rootLength||at>prefix-visionLength||metadataOwned.AsSpan(at,visionLength).Contains(true)||!Equal(Detach(tokens,prefix,at,visionLength,vision.Start,vision.End),vision.Tokens))throw new InvalidDataException("VisionInfo metadata splice/ownership differs.");Array.Fill(metadataOwned,true,at,visionLength);}continue;
            }
            // Reborn: PropBody owns its complete metadata fragment independently of ActiveBody's longer stock tables.
            if(pair.Key==FastHash.GetHashCode("Body")&&propBodyPiece is { } prop){
                if(at<rootLength||at>prefix-prop.Prefix||metadataOwned.AsSpan(at,prop.Prefix).Contains(true)||!Equal(Detach(tokens,prefix,at,prop.Prefix,prop.Start,prop.End),prop.Tokens))throw new InvalidDataException("PropBody root metadata splice/ownership differs.");
                Array.Fill(metadataOwned,true,at,prop.Prefix);continue;
            }
            // Reborn: complete AI leaf fragments were independently proved above; their metadata extent follows chooser presence.
            // Reborn: an AI-less object owns exactly the stock zero-count declaration targeting the inline +480 count word.
            if(pair.Key==FastHash.GetHashCode("AI")&&!ConscriptModuleScalarContract.Owners(source).Any(o=>o.Container=="AI")){_=aiAbsentWitness.Value;
                if(at<rootLength||at>prefix-12||metadataOwned.AsSpan(at,12).Contains(true)||Word(tokens.InstanceBuffer,at)!=0x6C827073||Word(tokens.InstanceBuffer,at+4)!=0x5D448FD8||Word(tokens.InstanceBuffer,at+8)!=(uint)(prefix+480)||Word(b,480)!=0||Word(b,484)!=0)throw new InvalidDataException("Absent AI declaration/native storage differs.");
                Array.Fill(metadataOwned,true,at,12);continue;}
            if(pair.Key==FastHash.GetHashCode("AI")){if(aiLength<12||at<rootLength||at>prefix-aiLength||metadataOwned.AsSpan(at,aiLength).Contains(true))throw new InvalidDataException("AI metadata ownership differs.");Array.Fill(metadataOwned,true,at,aiLength);continue;}
            // Reborn: repeated root count markers bind inline headers; item/native targets bind independently proved tail records.
            string? extendedName=GameObjectExtendedRootContract.Names.SingleOrDefault(name=>FastHash.GetHashCode(name)==pair.Key);
            if(extendedName!=null){if(!rootPieces.TryGetValue(extendedName,out var piece)){if(at!=0)throw new InvalidDataException("Omitted root metadata has a target: "+extendedName);continue;}if(at<rootLength||at>prefix-piece.Prefix||metadataOwned.AsSpan(at,piece.Prefix).Contains(true))throw new InvalidDataException("Extended root metadata ownership differs: "+extendedName);
                var field=Ep1GameObjectRootOwnersProjection.Fields.Single(f=>f.Name==extendedName);
                for(int offset=0;offset<piece.Prefix;offset+=12){uint target=Word(piece.Tokens.InstanceBuffer,offset+8);uint expected=target==0?0:target<piece.Prefix?target+(uint)at:field.InlineList&&target==piece.Prefix?(uint)(prefix+field.Slot):checked(target-(uint)piece.Prefix+(uint)(prefix+piece.Start)-(field.InlineList?8u:0u));if(Word(tokens.InstanceBuffer,at+offset)!=Word(piece.Tokens.InstanceBuffer,offset)||Word(tokens.InstanceBuffer,at+offset+4)!=Word(piece.Tokens.InstanceBuffer,offset+4)||Word(tokens.InstanceBuffer,at+offset+8)!=expected)throw new InvalidDataException("Extended root metadata splice differs: "+extendedName);}
                Array.Fill(metadataOwned,true,at,piece.Prefix);continue;}
            // Reborn: dependency presence and its complete standalone table bind the actual root target independently of stock absence.
            if(pair.Key==FastHash.GetHashCode("GameDependency")){if(dependencyTokens==null){if(at!=0)throw new InvalidDataException("Absent GameDependency metadata has a target.");}else{if(at<rootLength||at>prefix-dependencyLength||metadataOwned.AsSpan(at,dependencyLength).Contains(true))throw new InvalidDataException("GameDependency metadata ownership differs.");if(!Equal(Detach(tokens,prefix,at,dependencyLength,dependencyStart,dependencyEnd),dependencyTokens))throw new InvalidDataException("GameDependency metadata root splice differs.");Array.Fill(metadataOwned,true,at,dependencyLength);}continue;}
            // Reborn: full Geometry metadata is source-sized and checked independently before comparing its enclosing root splice.
            if(pair.Key==FastHash.GetHashCode("Geometry")){if(at<rootLength||at>prefix-geometryLength||metadataOwned.AsSpan(at,geometryLength).Contains(true))throw new InvalidDataException("Geometry metadata ownership differs.");if(!Equal(Detach(tokens,prefix,at,geometryLength,layout.Geometry,cut),geometryTokens))throw new InvalidDataException("Geometry metadata root splice differs.");Array.Fill(metadataOwned,true,at,geometryLength);continue;}
            // Reborn: omitted lists are null declarations; present/empty/repeated lists own the exact prefix verified above.
            string? containerName=ConscriptVariableContainers.Names.SingleOrDefault(n=>FastHash.GetHashCode(n)==pair.Key);
            if(containerName!=null){int size=containerLengths[containerName];if(size==0){if(at!=0)throw new InvalidDataException("Omitted container metadata target differs: "+containerName);}else{if(at<rootLength||at>prefix-size||metadataOwned.AsSpan(at,size).Contains(true))throw new InvalidDataException("Variable container metadata ownership differs: "+containerName);Array.Fill(metadataOwned,true,at,size);}continue;}
            // Reborn: an absent owner has a null metadata target; an empty/present owner has its own exact source-sized transaction grammar.
            if(pair.Key==FastHash.GetHashCode("ObjectResourceInfo")){int size=GameObjectResourceContract.VerifyMetadata(source,b,tokens.InstanceBuffer,prefix,at);if(size>0){if(at<rootLength||at>prefix-size||metadataOwned.AsSpan(at,size).Contains(true))throw new InvalidDataException("ObjectResourceInfo metadata ownership differs.");Array.Fill(metadataOwned,true,at,size);}continue;}
            // Reborn: changed ArmorSet cardinality changes metadata extent, not its field grammar or source/native reference contract.
            if(pair.Key==FastHash.GetHashCode("ArmorSet")){int size=ConscriptArmorSetContract.VerifyMetadata(source,b,tokens.InstanceBuffer,prefix,at);if(at<rootLength||at>prefix-size||metadataOwned.AsSpan(at,size).Contains(true))throw new InvalidDataException("ArmorSet metadata owner overlaps another table.");Array.Fill(metadataOwned,true,at,size);continue;}
            // Reborn: movement metadata has independently checked source-sized extent, including the omitted-list count marker.
            if(pair.Key==FastHash.GetHashCode("LocomotorSet")){int size=ConscriptLocomotorSetContract.VerifyMetadata(source,b,tokens.InstanceBuffer,prefix,at);if(at<rootLength||at>prefix-size||metadataOwned.AsSpan(at,size).Contains(true))throw new InvalidDataException("LocomotorSet metadata owner overlaps another table.");Array.Fill(metadataOwned,true,at,size);continue;}
            // Reborn: display-name metadata directly targets its optional header, not a stock text identity or a separate table.
            string? pointed=GameObjectRootTextContract.Pointed.SingleOrDefault(n=>FastHash.GetHashCode(n)==pair.Key);
            if(pointed!=null){int slot=pointed=="DisplayName"?404:408;uint target=Word(b,slot);if(at!=(target==0?0:prefix+target))throw new InvalidDataException("Pointed root name metadata target differs: "+pointed);continue;}
            // Reborn: Browser always declares its count table; populated items target their native string records (stock WorldBuilder grammar).
            if(pair.Key==FastHash.GetHashCode("Browser")){int size=12+browser.Count*12;if(at<rootLength||at>prefix-size||metadataOwned.AsSpan(at,size).Contains(true)||Word(tokens.InstanceBuffer,at)!=0x6C827073||Word(tokens.InstanceBuffer,at+4)!=0x5D448FD8||Word(tokens.InstanceBuffer,at+8)!=prefix+Ep1GameObjectBrowser.CountSlot)throw new InvalidDataException("Browser metadata table differs.");
                for(int item=0;item<browser.Count;item++){int row=at+12+item*12;if(Word(tokens.InstanceBuffer,row)!=0xB151A0EB||Word(tokens.InstanceBuffer,row+4)!=0||Word(tokens.InstanceBuffer,row+8)!=prefix+browser.Items+item*8)throw new InvalidDataException("Browser metadata item row differs.");}
                Array.Fill(metadataOwned,true,at,size);continue;}
            if(old==0){if(at!=0)throw new InvalidDataException("Absent root metadata target differs.");continue;}
            if(old>=stock.Projection.NativeRoot){if(!rootComparer.NativeOffsets.TryGetValue(old,out int coordinate)||at!=prefix+coordinate)throw new InvalidDataException("Direct root metadata coordinate differs.");continue;}
            int index=Array.IndexOf(oldTables,old),end=index+1<oldTables.Length?oldTables[index+1]:stock.Projection.NativeRoot;
            string name=topology.Containers.Select(c=>c.Name).Concat(Ep1GameObjectRootOwnersProjection.Fields.Select(f=>f.Name)).Append("Geometry").SingleOrDefault(n=>FastHash.GetHashCode(n)==pair.Key)??"Root";
            int length=name=="Behaviors"?behaviorLength:end-old;
            if(length<12||length%12!=0||at<rootLength||at>prefix-length||metadataOwned.AsSpan(at,length).Contains(true))throw new InvalidDataException("Variable metadata owner overlap/bounds.");
            if(name!="Behaviors")
            {
                int slot=name=="Geometry"?500:Ep1GameObjectRootOwnersProjection.Fields.FirstOrDefault(f=>f.Name==name).Slot;
                if(fixedGraphs.TryGetValue(slot,out var graph)){var leaf=Extract(native,graph.Start,graph.End);BehaviorMetadataStockProof.Compare(Detach(tokens,prefix,at,length,graph.Start,graph.End),leaf,stock.Bytes,old,end,graph.Type,graph.Comparer.NativeRecords);}
                else{var coordinates=maps.GetValueOrDefault(name)??rootComparer.NativeOffsets;var expected=stock.Bytes.AsSpan(old,end-old).ToArray();for(int offset=8;offset<length;offset+=12){uint target=Word(expected,offset);if(target!=0)Put(expected,offset,target>=old&&target<end?target-(uint)old+(uint)at:coordinates.TryGetValue((int)target,out int coordinate)?(uint)(prefix+coordinate):throw new InvalidDataException("Unmapped stock metadata target: "+name));}if(!tokens.InstanceBuffer.AsSpan(at,length).SequenceEqual(expected))throw new InvalidDataException("Variable metadata stock owner differs: "+name);}
            }
            Array.Fill(metadataOwned,true,at,length);
        }
        if(metadataOwned.Contains(false))throw new InvalidDataException("Unowned variable graph metadata.");return tokens;

        // Reborn: unchanged nonmodule owners retain independent complete stock payload comparison despite shifted addresses/reference indices.
        //-------------------------------------------------------------------------------------------------
        /** Reborn: source-sized list metadata always binds the actual native count word, including an allocated-empty list. */
        //-------------------------------------------------------------------------------------------------
        void VerifyContainerHeader(string name,int header,int count){int at=rows[FastHash.GetHashCode(name)].Offset;if(at<0||at>prefix-12-count*12||Word(tokens.InstanceBuffer,at)!=0x6C827073||Word(tokens.InstanceBuffer,at+4)!=0x5D448FD8||Word(tokens.InstanceBuffer,at+8)!=prefix+header)throw new InvalidDataException("Variable module container count metadata differs: "+name);}
        void Fixed(Type type,int old,int start,int end){var leaf=Extract(native,start,end);var values=authoredValues.Where(p=>p.Key>=start&&p.Key<end).ToDictionary(p=>p.Key-start,p=>p.Value);var records=recordValues.Where(p=>p.Key>=start&&p.Key<end).ToDictionary(p=>p.Key-start,p=>p.Value);var comparer=new SelectedNativeGraphComparer(stock.Bytes,rootReferences,leaf,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true,authoredValues:values){AuthoredRecordValues=records};comparer.Compare(type,old);fixedGraphs.Add(type==typeof(Ep1Geometry)?500:Ep1GameObjectRootOwnersProjection.Fields.Single(f=>f.NativeType==type).Slot,(start,end,type,comparer));Own(leaf,start,end);}
        // Reborn: whole-graph ownership is the disjoint union of exact leaf allocations and their auxiliary sources.
        void Own(Chunk leaf,int start,int end){Cover(start,end-start);foreach(uint s in Slots(leaf.RelocationBuffer))Add(ownedRelos,s+(uint)start);foreach(uint s in Slots(leaf.ImportsBuffer))Add(ownedImports,s+(uint)start);}
        // Reborn: only the inline AI header may overlap the typed root's already verified bytes.
        void Cover(int start,int size,bool inline=false){if(start<0||size<0||start>b.Length-size||!inline&&covered.AsSpan(start,size).Contains(true))throw new InvalidDataException("Variable graph owner overlap/bounds.");Array.Fill(covered,true,start,size);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the schema proof on every stock Conscript leaf of the family (with its stock source) and learn the metadata grammar
        from the stock Conscript and SovietRefinery leaves of that family. */
    //-------------------------------------------------------------------------------------------------
    private StockMetadataGrammar QualifySchemaFamily(string manifest,SchemaNativeContract.Family family)
    {
        // Reborn: the Conscript baseline has no EVA-ready leaf; qualify it from the actual PlayerSpellBook source and native records.
        if(family.Type==typeof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData))return EvaReadyClientContract.Qualify(manifest,baseline,referenceSchemas,catalog);
        // Reborn: the Conscript baseline has no EVA-create leaf; qualify it from the actual SovietSuperWeapon source and native record.
        if(family.Type==typeof(Ep1EvaAnnounceClientCreateModuleData))return EvaCreateClientContract.Qualify(manifest,baseline,referenceSchemas,catalog);
        var grammar=new StockMetadataGrammar();
        // Reborn: only developer comparisons need a resolved stock-source probe; fast builds still learn and validate the native grammar.
        var resolvedBaseline=SdkBuildTests.Enabled?SchemaNativeContract.Resolve(baseline,referenceSchemas,catalog):baseline;var leaves=ConscriptVariableContainers.Find(resolvedBaseline,family.Container)?.ChildNodes.OfType<XmlElement>().ToArray()??Array.Empty<XmlElement>();int qualified=0;
        foreach(var module in topology.Containers.Single(c=>c.Name==family.Container).Modules.Where(m=>m.TypeId==family.TypeId)){
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);
            if(SdkBuildTests.Enabled){var element=leaves.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==Word(stock.Bytes,module.Native+4));SchemaNativeContract.VerifyStock(element,family.Type,family.TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}qualified++;
        }
        if(qualified==0)throw new InvalidDataException("No stock leaf qualified the schema proof for "+family.Type.Name+".");
        var refinery=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");
        foreach(var module in GameObjectModuleReview.Inspect(manifest,refinery.Asset.Name).Containers.Single(c=>c.Name==family.Container).Modules.Where(m=>m.TypeId==family.TypeId))grammar.Learn(refinery.Bytes,module.Wrapper,refinery.Projection.NativeRoot,family.Type);
        // Reborn: optional draw materials use independently qualified original shader/native witnesses, including all polymorphic constant kinds.
        if(family.Type==typeof(W3DScriptedModelDrawModuleData))ShaderMaterialContract.Qualify(grammar,manifest,baseline,referenceSchemas,catalog);
        // Reborn: optional draw-state ShadowInfo decals use the original SovietCrane repair-decal witness (native + metadata grammar).
        if(family.Type==typeof(W3DScriptedModelDrawModuleData))ScriptedShadowInfoContract.Qualify(grammar,manifest,baseline,referenceSchemas,catalog);
        // Reborn: frame-triggered draw FX events use original reactor spark records for native/import and nested metadata qualification.
        if(family.Type==typeof(W3DScriptedModelDrawModuleData))ScriptedFXEventContract.Qualify(grammar,manifest,baseline,referenceSchemas,catalog);
        // Reborn: model-state turret lists learn their native and metadata grammar from the original Sentry Gun.
        if(family.Type==typeof(W3DScriptedModelDrawModuleData))ScriptedTurretContract.Qualify(grammar,manifest,baseline,referenceSchemas,catalog);
        // Reborn: original Bear selectors qualify populated nested sound arrays beyond the Conscript voice-only shape.
        if(family.Type==typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData))ClientSoundArrayContract.Qualify(grammar,manifest,baseline,referenceSchemas,catalog);
        return grammar;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind original native occurrence slots to independently read authored targets on an unchanged comparison-source layout. */
    //-------------------------------------------------------------------------------------------------

    private AssetId[] ReferenceTable(XmlNode source,string? templateId)
    {
        string key=(templateId??"root")+"\n"+source.OuterXml;if(referenceTables.TryGetValue(key,out var existing))return existing;
        var document=CopySource(baseline);
        if(templateId==null){
            document=CopySource((XmlDocument)source);
            var actual=document.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Behaviors");
            actual.ParentNode!.ReplaceChild(document.ImportNode(baseline.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Behaviors"),true),actual);
            // Reborn: unrelated root/fixed owners retain baseline occurrence coordinates while variable leaf imports use their own source table.
            ConscriptVariableContainers.Restore(document,baseline);
            // Reborn: new root strong references have their own source/native proof; untouched stock owners keep stock occurrence coordinates.
            GameObjectExtendedRootContract.Mask(document,baseline);
            // Reborn: an AI-less object still reads the root reference table in stock occurrence coordinates, so the baseline AI owner is restored here only.
            AIAbsenceContract.RestoreForReferences(document,baseline);
            // Reborn: actual chooser imports are independently source-bound; untouched owners retain baseline occurrence coordinates.
            AIUpdateContract.Mask(document,baseline);
            // Reborn: untouched owners still use stock occurrence coordinates; the separate armor proof resolves actual authored rows.
            ConscriptArmorSetContract.Mask(document,baseline);
            // Reborn: other owners retain original occurrence coordinates; actual movement imports are independently checked from source.
            ConscriptLocomotorSetContract.Mask(document,baseline);
            // Reborn: stock occurrence coordinates for other owners keep their baseline root prefix; the actual root import contract independently checks authored presence/targets.
            GameObjectStrongReferenceContract.Mask(document,baseline);
        }else{
            var original=ConscriptModuleScalarContract.Owners(document).Single(o=>o.Element.GetAttribute("id")==templateId).Element;
            var replacement=(XmlElement)document.ImportNode(source,true);replacement.SetAttribute("id",templateId);original.ParentNode!.ReplaceChild(replacement,original);
        }
        var expected=ConscriptReferenceTargets.Read(document,referenceSchemas,catalog);
        if(expected.Length!=stock.Asset.References.Count)throw new InvalidDataException("Authored reference presence requires an unsupported native slot layout.");
        referenceTables.Add(key,expected);return expected;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate typed upgrade-list hashes against authored text and the catalog before restoring only comparison bytes to stock values. */
    //-------------------------------------------------------------------------------------------------
    private void VerifyUpgradeTriggers(ConscriptModuleScalarContract.Owner owner,string templateId,Chunk comparison)
    {
        var original=ConscriptModuleScalarContract.Owners(baseline).Single(o=>o.Element.GetAttribute("id")==templateId).Element;
        foreach(string name in new[]{"TriggeredBy","ConflictsWith"}){
            var authored=owner.Element.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name).ToArray();var defaults=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name).ToArray();
            if(authored.Length==0&&defaults.Length==0)continue;
            if(authored.Length!=defaults.Length)throw new InvalidDataException("Upgrade trigger list allocation changes require additional native layout support.");
            Type current=owner.NativeType;int offset=0;
            while(current.GetField(name)==null){var parent=current.GetField("Base")??throw new InvalidDataException("Upgrade trigger ABI field is not implemented.");offset+=System.Runtime.InteropServices.Marshal.OffsetOf(current,"Base").ToInt32();current=parent.FieldType;}
            var field=current.GetField(name)!;if(field.FieldType!=typeof(Relo.List<TypedAssetId<UpgradeTemplate>>))throw new InvalidDataException("Upgrade trigger ABI type differs.");
            offset+=System.Runtime.InteropServices.Marshal.OffsetOf(current,name).ToInt32();
            if(Word(comparison.InstanceBuffer,offset)!=authored.Length)throw new InvalidDataException("Upgrade trigger source/native count differs.");
            int list=(int)Word(comparison.InstanceBuffer,offset+4);
            for(int i=0;i<authored.Length;i++){
                string value=authored[i].InnerText.Trim();uint expected=InstanceHandle.GetInstanceId(value);
                if(catalog.Count(a=>a.TypeId==FastHash.GetHashCode("UpgradeTemplate")&&a.InstanceId==expected)!=1)throw SdkXmlDiagnostics.At(new InvalidDataException("Missing or ambiguous upgrade asset: UpgradeTemplate:"+value),authored[i]);
                if(Word(comparison.InstanceBuffer,list+i*4)!=expected)throw new InvalidDataException("Upgrade trigger native hash differs from authored source.");
                Put(comparison.InstanceBuffer,list+i*4,InstanceHandle.GetInstanceId(defaults[i].InnerText.Trim()));
            }
        }
    }

    // Reborn: materialize comparison XML without mutating captured baseline or authored source provenance.
    private static XmlDocument CopySource(XmlDocument source)=>SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));

    // Reborn: rebase a combined metadata owner and its native tail into a standalone stock-proof input without changing any occurrence index.
    private static Chunk Detach(Chunk output,int prefix,int table,int length,int start,int end){var leaf=Extract(output,prefix+start,prefix+end);var result=new Chunk{InstanceBuffer=new byte[length+leaf.InstanceBuffer.Length],RelocationBuffer=Pack(Slots(leaf.RelocationBuffer).Select(s=>s+(uint)length)),ImportsBuffer=Pack(Slots(leaf.ImportsBuffer).Select(s=>s+(uint)length))};output.InstanceBuffer.AsSpan(table,length).CopyTo(result.InstanceBuffer);leaf.InstanceBuffer.CopyTo(result.InstanceBuffer,length);foreach(uint slot in Slots(leaf.RelocationBuffer))Put(result.InstanceBuffer,length+(int)slot,Word(leaf.InstanceBuffer,(int)slot)+(uint)length);for(int at=8;at<length;at+=12){uint target=Word(result.InstanceBuffer,at);if(target!=0)Put(result.InstanceBuffer,at,target>=table&&target<table+length?target-(uint)table:target>=prefix+start&&target<prefix+end?target-(uint)(prefix+start)+(uint)length:throw new InvalidDataException("Variable metadata detachment crosses owners."));}return result;}

    // Reborn: extraction retains common Core indices but rejects pointers escaping their complete owner allocation.
    private static Chunk Extract(Chunk source,int start,int end){if(start<0||end<start||end>source.InstanceBuffer.Length)throw new InvalidDataException("Module extraction bounds.");var leaf=new Chunk{InstanceBuffer=source.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start&&s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start&&s<end).Select(s=>s-(uint)start))};foreach(uint s in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)s);if(target<start||target>=end)throw new InvalidDataException("Cross-module native pointer.");Put(leaf.InstanceBuffer,(int)s,target-(uint)start);}return leaf;}
    // Reborn: duplicate auxiliary ownership must fail rather than silently collapse in a set.
    private static void Add(HashSet<uint> slots,uint value){if(!slots.Add(value))throw new InvalidDataException("Duplicate variable graph auxiliary owner.");}
    // Reborn: diagnostics retain caller-owned snapshots and independent copies for mutation controls.
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    // Reborn: repeated output equality includes all three native streams.
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer)&&a.RelocationBuffer.SequenceEqual(b.RelocationBuffer)&&a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    // Reborn: explicit framing and unique aligned slots precede native partitioning.
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Variable graph auxiliary framing.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Any(s=>s%4!=0)||slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Duplicate/unaligned graph auxiliary slot.");return slots;}
    // Reborn: ordered nonempty auxiliary streams have one terminating sentinel.
    private static byte[] Pack(IEnumerable<uint> source){var slots=source.ToArray();if(slots.Length==0)return Array.Empty<byte>();var bytes=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(bytes,i*4,slots[i]);Put(bytes,slots.Length*4,uint.MaxValue);return bytes;}
    // Reborn: all diagnostic word reads stay within their supplied graph.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Variable graph word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
    // Reborn: only newly owned comparison arrays receive identity/coordinate patches.
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
    // Reborn: each deliberately corrupt graph must be refused, not merely reported as incomplete.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Variable module corruption accepted.");}
}
