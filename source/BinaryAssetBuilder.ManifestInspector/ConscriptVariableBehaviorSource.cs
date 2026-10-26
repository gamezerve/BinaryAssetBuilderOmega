using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: schema-backed source matching admits variable reviewed Behaviors without guessing native layouts from new module positions.
internal static class ConscriptVariableBehaviorSource
{
    internal sealed record Plan(Dictionary<string,string> Origins,int BehaviorCount);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: describe the complete scalar-only sell source shape; native/metadata expectations come independently from stock AlliedConstructionYard. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner SellTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<StructureSellUpdate xmlns='uri:ea.com:eala:asset' id='RebornStockStructureSell' SellTime='2s' ActiveModelCondition='SOLD'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.StructureSellUpdateModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the comparison shape is not a stock-value gate; ordered destruction items have a separate source/native allocation contract. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner DestructionTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<BuildingDestructionBehavior xmlns='uri:ea.com:eala:asset' id='RebornDestructionTemplate'><DieMuxData DeathTypes='ALL'/><Item/></BuildingDestructionBehavior>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.BuildingDestructionModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: docking strings are authored values, not requirements to match the original refinery's bone names. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner QueueDockTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<QueueDockBehavior xmlns='uri:ea.com:eala:asset' id='RebornQueueDockTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.QueueDockModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: two required vectors describe source shape only; every coordinate/time/flag is independently source-bound. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner QueueExitTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<QueueProductionExitUpdate xmlns='uri:ea.com:eala:asset' id='RebornQueueExitTemplate'><UnitCreatePoint/><NaturalRallyPoint/></QueueProductionExitUpdate>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.Ep1QueueProductionExitUpdateModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Spawn has independent source/native contracts for every optional record and weak list, not a fixed refinery payload. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner SpawnTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<SpawnBehavior xmlns='uri:ea.com:eala:asset' id='RebornSpawnTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.Ep1SpawnBehaviorModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match complete nested payload shapes to reviewed templates while allowing their fixed-width values and local IDs to vary. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan Admit(XmlDocument source,XmlDocument baseline,XmlSchemaSet schemas)
    {
        // Reborn: field-family ordering is normalized in memory before source admission, retaining the authored nugget sequence.
        SubObjectsUpgradeContract.Normalize(source,schemas);
        ConscriptSharedSourceContract.Validate(source,baseline,schemas);
        if(!ConscriptSourceInput.Identity(source.DocumentElement!.GetAttribute("id")))throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid GameObject identity."),source.DocumentElement);
        // Reborn: honor official weapon sortBy/boolean semantics before assigning common reference occurrences.
        ConscriptWeaponSetContract.Normalize(source);
        var actual=Qualified(source);var model=Qualified(baseline);
        // Reborn: nested numeric overflow must identify its authored attribute instead of failing later inside the native marshaler.
        foreach(var owner in ConscriptModuleScalarContract.Owners(source))ConscriptUpgradeStructure.ValidateSource(owner);
        var owners=ConscriptModuleScalarContract.Owners(actual);var originals=ConscriptModuleScalarContract.Owners(model);
        // Reborn: this scalar-only family has its own independent stock anchor, rather than requiring an existing Conscript module.
        var behaviors=owners.Where(o=>o.Container=="Behaviors").ToArray();var templates=originals.Where(o=>o.Container=="Behaviors").Append(SellTemplate()).Append(DestructionTemplate()).Append(QueueDockTemplate()).Append(QueueExitTemplate()).Append(SpawnTemplate()).Append(UnpackTemplate()).Append(ProductionTemplate()).Append(RepairTemplate()).Append(EngineerTemplate()).Append(PlacementTemplate()).Append(InfiltratorTemplate()).Append(StructurePlacementTemplate()).Append(LUAEventListTemplate()).Append(PowerUpgradeTemplate()).Append(BuildingTemplate()).ToArray();
        // Reborn: module count is source-driven, not capped at the old trial budget; existing checked buffer/depth bounds protect compilation.
        if(behaviors.Length<1)throw SdkXmlDiagnostics.At(new InvalidDataException("Empty Behaviors container serialization is not implemented on this variable graph path."),source.GetElementsByTagName("Behaviors", "uri:ea.com:eala:asset")[0]);
        var origins=new Dictionary<string,string>(StringComparer.Ordinal);var selected=new List<ConscriptModuleScalarContract.Owner>();
        foreach(var owner in behaviors)
        {
            // Reborn: the assign-slaves leaf is source/schema-proved, not restricted to the original building's values, local ID or nested shape.
            // Reborn: populated turret lists use the complete source/native EP1 turret contract rather than infantry hardpoint shape matching.
            if(owner.NativeType==typeof(SageBinaryData.Ep1WeaponSetUpdateModuleData)&&owner.Element.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName=="WeaponSlotTurret")){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.AssignSlavesTargetObjectSpecialPowerModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            // Reborn: recall and parking accept schema-valid authored values and optional records, with independent EP1 source/native proofs.
            if(AirfieldBehaviorContract.Supports(owner.NativeType)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            // Reborn: death weapon references, death conditions and optional/list records are source-driven, not frozen to the inherited building sample.
            // Reborn: automatic-deposit source uses schema-sized lists and the independently captured EP1 native layout.
            // Reborn: slaughter filters, inherited container records and refund amounts follow the official schema.
            // Reborn: the production-info marker has no own fields; its local identity is authored normally.
            // Reborn: replacement targets, timing and inherited ability settings follow the official schema instead of fixed MCV sample values.
            // Reborn: wall length and connection filters use the full schema instead of stock-only values or names.
            // Reborn: timer-monitor power references and ready-condition flags follow the official schema.
            // Reborn: radar markers admit their schema-defined inherited identity without a fixed stock module ID.
            // Reborn: power managers accept authored power/EVA references and local module identities through the official schema.
            // Reborn: production bonuses are schema-driven values rather than fixed FreeTrade/MassProduction samples.
            // Reborn: object storage uses schema-owned radius/link values and authored strong/weak references.
            // Reborn: teleport storage accepts schema-defined link IDs, OCL references and the second-click selection flag.
            // Reborn: teleport-update source fields and filter graphs use the complete retail-qualified native contract.
            // Reborn: OCL powers accept complete EP1 location, filter and upgrade-list source graphs.
            // Reborn: player upgrade lists are authored freely under the official SpecialPower schema.
            // Reborn: template-ID store identities are authored normally under the official field-free module schema.
            // Reborn: weapon-fire abilities accept schema-valid own fields and inherited SpecialAbility graphs.
            if(owner.NativeType==typeof(SageBinaryData.Ep1WeaponFireSpecialAbilityUpdateModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.StoreTemplateIDBehaviorModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.PlayerUpgradeSpecialPowerModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.Ep1OCLSpecialPowerModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.TeleportStoredObjectsUpdateModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.TeleportStoredObjectsSpecialPowerModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            // Reborn: base SpecialPower accepts its schema-defined filters, sound, references and values through complete native proof.
            if(owner.NativeType==typeof(SageBinaryData.SpecialPowerModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.StoreObjectsSpecialPowerModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.GiveProductionBonusSpecialPowerModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.Ep1PlayerPowerManagerBehaviorModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.RadarBehaviorModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            // Reborn: attack notification values and friend filters are source-driven under the complete EP1 proof.
            // Reborn: subobject upgrade nuggets use their complete EP1 source/native proof, including inherited upgrade conditions.
            if(owner.NativeType==typeof(SageBinaryData.Ep1SubObjectsUpgradeModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            // Reborn: transport containment is admitted by full schema/native proof, not a fixed stock shape.
            // Reborn: attribute modifier upgrades are source-driven under the complete EP1 upgrade/reference proof.
            // Reborn: passenger ejection is admitted by complete EP1 ability/native proof.
            // Reborn: run-off-map waypoint names and flags are source-driven under the complete 20-byte EP1 contract.
            if(owner.NativeType==typeof(SageBinaryData.RunOffMapBehaviorModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.EjectPassengersSpecialAbilityUpdateModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.AttributeModifierUpgradeModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.TransportContainModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.NotifyWhenAttackingBehaviorModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.MonitorSpecialPowerTimerUpdateModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.WallBuilderBehaviorModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.ReplaceSelfSpecialAbilityModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.ProductionUpdateInfoModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.SlaughterHordeContainModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.Ep1AutoDepositUpdateModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            if(owner.NativeType==typeof(SageBinaryData.Ep1FireWeaponWhenDeadBehaviorModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);selected.Add(owner);continue;}
            // Reborn: exact existing identity breaks equivalent-template ties; new IDs may use any otherwise identical complete reviewed shape.
            var candidates=templates.Where(t=>t.Element.LocalName==owner.Element.LocalName).OrderByDescending(t=>t.Element.GetAttribute("id")==owner.Element.GetAttribute("id"));
            var matched=candidates.FirstOrDefault(t=>Matches(owner,t));
            if(matched==null)throw SdkXmlDiagnostics.At(new InvalidDataException("Behavior '"+owner.Element.LocalName+"' contains an unimplemented native layout or nested field. Covered hardpoint/Weapon and upgrade lists, compatible targets and fixed attributes are editable; other families/structures still require their EP1 processor support."),SdkXmlDiagnostics.Unmasked(owner.Element,source));
            origins.Add(owner.Element.GetAttribute("id"),matched.Element.GetAttribute("id"));selected.Add(matched);
        }
        // Reborn: reviewed draw/client families select their own complete stock shape independently of source count/order.
        var variableOwners=owners.Where(o=>ConscriptVariableContainers.Supports(o.Container)).ToArray();
        var variableTemplates=new Dictionary<string,ConscriptModuleScalarContract.Owner>(StringComparer.Ordinal);
        foreach(var owner in variableOwners){
            var matched=originals.Where(t=>t.Container==owner.Container&&t.NativeType==owner.NativeType).OrderByDescending(t=>t.Element.GetAttribute("id")==owner.Element.GetAttribute("id")).FirstOrDefault(t=>Matches(owner,t));
            // Reborn: opted-in families with a new nested shape are proved field-by-field from schema and source instead of a stock-shaped template.
            if(matched==null&&SchemaNativeContract.Supports(owner.NativeType)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);variableTemplates.Add(owner.Element.GetAttribute("id"),owner);continue;}
            if(matched==null)throw SdkXmlDiagnostics.At(new InvalidDataException("Module '"+owner.Element.LocalName+"' has an unsupported nested layout; draw/client count and order are source-driven, but this leaf still needs its EP1 nested-field processor."),SdkXmlDiagnostics.Unmasked(owner.Element,source));
            origins.Add(owner.Element.GetAttribute("id"),matched.Element.GetAttribute("id"));variableTemplates.Add(owner.Element.GetAttribute("id"),matched);
        }
        // Reborn: every other root/container retains the existing shape, but reviewed root and direct module fixed fields remain authorable.
        var copy=Clone(actual);var comparison=Clone(model);var oldBehaviors=Container(copy);
        // Reborn: an object without AI compares against the baseline without its AI container; native/metadata absence is proved separately.
        AIAbsenceContract.Mask(copy,comparison);
        oldBehaviors.ParentNode!.ReplaceChild(copy.ImportNode(Container(comparison),true),oldBehaviors);
        // Reborn: only the comparison copy restores baseline containers; actual source/native proof retains every authored module.
        ConscriptVariableContainers.Restore(copy,comparison);
        // Reborn: restore only the comparison body; authored PropBody is independently proved during native composition.
        if(ConscriptVariableContainers.Find(source,"Body")?.ChildNodes.OfType<XmlElement>().SingleOrDefault()?.LocalName=="PropBody"){var body=ConscriptVariableContainers.Find(copy,"Body")!;body.RemoveAll();foreach(XmlNode child in ConscriptVariableContainers.Find(comparison,"Body")!.ChildNodes)body.AppendChild(copy.ImportNode(child,true));}
        // Reborn: VisionInfo presence is source-owned; only the structural comparison restores the baseline record.
        VisionInfoContract.MaskComparison(copy,comparison);
        copy.DocumentElement!.SetAttribute("id",comparison.DocumentElement!.GetAttribute("id"));
        // Reborn: comparison-only IDs must not redirect topology diagnostics to an unrelated baseline asset or the entry document.
        // Reborn: independently proved AI classes may differ from the comparison infantry; restore comparison AI before identity/scalar masking.
        AIUpdateContract.Mask(copy,comparison);
        try{ConscriptSharedSourceContract.Mask(copy,comparison);}
        catch(InvalidDataException error)when(SdkXmlDiagnostics.Node(error)!=null){throw SdkXmlDiagnostics.At(new InvalidDataException(error.Message,error),SdkXmlDiagnostics.Unmasked(SdkXmlDiagnostics.Node(error),source));}
        // Reborn: existing plain-record scalar edits use the full variable graph verifier rather than the historical fixed snapshot oracle.
        ConscriptLocomotorSetContract.Mask(copy,comparison);
        GameObjectResourceContract.Mask(copy,comparison);
        // Reborn: optional dependency presence and list sizes are source-bound rather than absent-only stock comparison constants.
        GameObjectDependencyContract.Mask(copy,comparison);
        // Reborn: optional audio/shadow records and repeated projected records use independent full-root proofs.
        GameObjectExtendedRootContract.Mask(copy,comparison);
        // Reborn: the complete AI leaf contract owns chooser presence, nested defaults and optional imports independently of stock shape.

        // Reborn: source-sized Geometry has a complete independent native/metadata proof instead of a one-Shape comparison quota.
        GameObjectGeometryContract.Mask(copy,comparison);
        GameObjectNestedValueContract.Mask(copy,comparison);
        // Reborn: the native comparer checks text bytes and allocations independently after structural admission.
        GameObjectRootTextContract.Mask(copy,comparison);
        // Reborn: schema-valid ArmorSet lists are independently compiled, not pinned to the stock row count/presence.
        ConscriptArmorSetContract.Mask(copy,comparison);
        // Reborn: existing strong slots retain schema types/presence, but their target names are authored values on both compilation routes.
        ConscriptReferenceSpellings.Compare(copy,comparison,schemas,allowRetarget:true,allowUpgradeWeak:true);
        if(ConscriptSourceInput.Signature(copy.DocumentElement)!=ConscriptSourceInput.Signature(comparison.DocumentElement!))
        {
            string difference=ConscriptSourceInput.Difference(copy.DocumentElement,comparison.DocumentElement!,"GameObject",out var offending);
            throw SdkXmlDiagnostics.At(new InvalidDataException("GameObject contains an unsupported owner/layout: "+difference+". Root text, fixed values, source-sized ArmorSet and covered module lists are enabled; other structures still require EP1 processor support."),SdkXmlDiagnostics.Unmasked(offending,source));
        }
        foreach(var owner in owners.Where(o=>o.Container!="Behaviors"&&!ConscriptVariableContainers.Supports(o.Container)))
        {
            // Reborn: PropBody has an independent source/schema contract rather than the Conscript ActiveBody template.
            if(owner.NativeType==typeof(SageBinaryData.PropBodyModuleData)){origins.Add(owner.Element.GetAttribute("id"),SchemaNativeContract.Origin);continue;}
            var original=originals.Single(o=>o.Container==owner.Container&&o.Index==owner.Index);origins.Add(owner.Element.GetAttribute("id"),original.Element.GetAttribute("id"));
        }
        // Reborn: source-order reference occurrences must be stable across attribute formatting and added/cloned behavior identities.
        var aligned=Clone(model);var list=Container(aligned);list.RemoveAll();
        foreach(var template in selected)list.AppendChild(aligned.ImportNode(template.Element,true));
        // Reborn: align each reviewed authored leaf with its own template, not a module at the same baseline ordinal.
        foreach(string name in ConscriptVariableContainers.Names){var container=ConscriptVariableContainers.Find(aligned,name);var authored=ConscriptVariableContainers.Find(source,name);if(authored==null){container?.ParentNode!.RemoveChild(container);continue;}container!.RemoveAll();foreach(var owner in variableOwners.Where(o=>o.Container==name))container.AppendChild(aligned.ImportNode(variableTemplates[owner.Element.GetAttribute("id")].Element,true));}
        // Reborn: flexible upgrade children do not have to match the template's occurrence count during source-order alignment.
        var upgradeIds=owners.Where(o=>ConscriptUpgradeStructure.Supports(o.NativeType)).Select(o=>o.Element.GetAttribute("id")).ToHashSet(StringComparer.Ordinal);
        // Reborn: a prop body has no strong references to align with the ActiveBody comparison template.
        if(owners.Any(o=>o.NativeType==typeof(SageBinaryData.PropBodyModuleData))){var body=ConscriptVariableContainers.Find(aligned,"Body")!;body.RemoveAll();foreach(XmlNode child in ConscriptVariableContainers.Find(source,"Body")!.ChildNodes)body.AppendChild(aligned.ImportNode(child,true));}
        Align(source.DocumentElement!,aligned.DocumentElement!,upgradeIds);
        return new(origins,behaviors.Length);

        // Reborn: comparison copies materialize official defaults without introducing allocations into the user's authored source.
        XmlDocument Qualified(XmlDocument input){var document=Clone(input);document.Schemas.Add(schemas);document.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));foreach(XmlAttribute a in document.SelectNodes("//@*")!)a.Value=a.Value;return Clone(document);}
        // Reborn: only known ABI fixed fields and the own local ID are masked; all nested fields/text/reference identities stay visible.
        bool Matches(ConscriptModuleScalarContract.Owner owner,ConscriptModuleScalarContract.Owner template)
        {
            // Reborn: already schema-qualified identical leaves need no repeated whole-object clone/validation; native/metadata proof is still mandatory.
            if(ConscriptSourceInput.Signature(owner.Element)==ConscriptSourceInput.Signature(template.Element))return true;
            var a=Clone(model);var b=Clone(model);var left=ConscriptVariableContainers.Find(a,owner.Container)!;var right=ConscriptVariableContainers.Find(b,owner.Container)!;left.RemoveAll();right.RemoveAll();
            var node=(XmlElement)a.ImportNode(owner.Element,true);var basis=(XmlElement)b.ImportNode(template.Element,true);left.AppendChild(node);right.AppendChild(basis);node.SetAttribute("id",basis.GetAttribute("id"));
            foreach(var field in ConscriptModuleScalarContract.Bindings(template.NativeType)){node.RemoveAttribute(field.Name);if(basis.HasAttribute(field.Name))node.SetAttribute(field.Name,basis.GetAttribute(field.Name));}
            // Reborn: list counts and optional animation presence are validated from source/native contracts, not frozen to this stock example.
            if(ConscriptUpgradeStructure.Supports(template.NativeType))ConscriptUpgradeStructure.Mask(node,basis);
            // Reborn: the implemented EP1 hardpoint processor accepts source-sized weapon records, not only the captured two-weapon example.
            if(ConscriptWeaponSetContract.Supports(template.NativeType))ConscriptWeaponSetContract.Mask(node,basis);
            // Reborn: the full authored destruction list is proved independently after structural matching.
            if(template.NativeType==typeof(SageBinaryData.BuildingDestructionModuleData))BuildingDestructionContract.Mask(node,basis);
            // Reborn: docking string bytes and allocations have their own complete source/native contract.
            if(template.NativeType==typeof(SageBinaryData.QueueDockModuleData))QueueDockContract.Mask(node,basis);
            // Reborn: required inline vectors retain schema shape but do not freeze their source coordinates to stock.
            if(template.NativeType==typeof(SageBinaryData.Ep1QueueProductionExitUpdateModuleData))QueueProductionExitContract.Mask(node,basis);
            // Reborn: Spawn own optional records and weak row counts are source-bound rather than frozen to a comparison template.
            if(template.NativeType==typeof(SageBinaryData.Ep1SpawnBehaviorModuleData))SpawnBehaviorContract.Mask(node,basis);
            // Reborn: optional audio presence and identity use the unpack source/catalog/native contract, not a frozen template attribute.
            if(template.NativeType==typeof(SageBinaryData.Ep1StructureUnpackUpdateModuleData))StructureUnpackContract.Mask(node,basis);
            // Reborn: a production module's optional upgrade translation graph is source-owned, not tied to an empty refinery template.
            if(template.NativeType==typeof(SageBinaryData.Ep1ProductionUpdateModuleData))ProductionUpdateContract.Mask(node,basis);
            // Reborn: inline percent/audio record values are authored and proved separately; record presence stays visible.
            if(template.NativeType==typeof(SageBinaryData.Ep1RepairUpdateModuleData))RepairUpdateContract.Mask(node,basis);
            // Reborn: engineer references, voice hashes, status mask and the whole filter are proved by the engineer contract.
            if(template.NativeType==typeof(SageBinaryData.Ep1EngineerContainModuleData))EngineerContainContract.Mask(node,basis);
            // Reborn: the optional requirement filter is proved by the assisted placement contract.
            if(template.NativeType==typeof(SageBinaryData.AssistedPlacementBehaviorModuleData))AssistedPlacementContract.Mask(node,basis);
            // Reborn: infiltrator references, voice hashes, weak ID and status masks are proved by its own contract.
            if(template.NativeType==typeof(SageBinaryData.InfiltratorContainModuleData))InfiltratorContainContract.Mask(node,basis);
            // Reborn: the required restricted-area shape is proved by the structure placement contract.
            if(template.NativeType==typeof(SageBinaryData.StructurePlacementBehaviorModuleData))StructurePlacementContract.Mask(node,basis);
            // Reborn: the authored event-list name string is proved byte-for-byte by the LUAEventList contract.
            if(template.NativeType==typeof(SageBinaryData.LUAEventListBehaviorModuleData))LUAEventListContract.Mask(node,basis);
            // Reborn: optional subobject names are actual values; they do not need to match a stock template's presence or contents.
            if(template.NativeType==typeof(SageBinaryData.BuildingBehaviorModuleData))BuildingBehaviorContract.Mask(node,basis);
            // Reborn: complete FX death conditions and source-sized Event rows have independent source/native/metadata ownership.
            if(template.NativeType==typeof(SageBinaryData.FXListBehaviorModuleData))FXListBehaviorContract.Mask(node,basis);
            // Reborn: colors/effects/status presence and counts are source-bound, not frozen to the infantry witness.
            if(template.NativeType==typeof(SageBinaryData.SecondaryDamageTrackerModuleData))SecondaryDamageTrackerContract.Mask(node,basis);
            // Reborn: creation/destroy death masks, OCL identities and strong upgrade arrays are independently source-bound.
            if(DeathBehaviorContract.Supports(template.NativeType))DeathBehaviorContract.Mask(node,basis);
            ConscriptReferenceSpellings.Compare(a,b,schemas,allowRetarget:true,allowUpgradeWeak:true);
            return ConscriptSourceInput.Signature(node)==ConscriptSourceInput.Signature(basis);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: all unpack values and optional audio presence remain authored; this template selects only the implemented EP1 family. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner UnpackTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<StructureUnpackUpdate xmlns='uri:ea.com:eala:asset' id='RebornUnpackTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.Ep1StructureUnpackUpdateModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: production scalars are authored; weak bonus type and populated modifier/translation children have no template and stay refused. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner ProductionTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<ProductionUpdate xmlns='uri:ea.com:eala:asset' id='RebornProductionTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.Ep1ProductionUpdateModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: both required inline repair records describe source shape only; their values are independently source-bound. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner RepairTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<RepairUpdate xmlns='uri:ea.com:eala:asset' id='RebornRepairTemplate'><PercentOfBuildCostToRebuildInfo/><SelfAudio/></RepairUpdate>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.Ep1RepairUpdateModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: engineer values, references and the optional filter are authored; the template selects only the EP1 family. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner EngineerTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<EngineerContain xmlns='uri:ea.com:eala:asset' id='RebornEngineerTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.Ep1EngineerContainModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: hint/snapping values and the optional filter are authored; the template selects only the EP1 family. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner PlacementTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<AssistedPlacementBehavior xmlns='uri:ea.com:eala:asset' id='RebornPlacementTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.AssistedPlacementBehaviorModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every infiltrator value and reference is authored or schema-defaulted; the template selects only the family. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner InfiltratorTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<InfiltratorContain xmlns='uri:ea.com:eala:asset' id='RebornInfiltratorTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.InfiltratorContainModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the restricted-area shape is authored; the template only selects the EP1 family and its required record. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner StructurePlacementTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<StructurePlacementBehavior xmlns='uri:ea.com:eala:asset' id='RebornStructurePlacementTemplate'><RestrictedAreaShape/></StructurePlacementBehavior>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.StructurePlacementBehaviorModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the event-list name is authored; the template only selects the EP1 family and carries a schema-required placeholder name. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner LUAEventListTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<LUAEventList xmlns='uri:ea.com:eala:asset' id='RebornLUAEventListTemplate' EventListName='RebornTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.LUAEventListBehaviorModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the empty schema-valid template selects BuildingBehavior; each authored optional string is independently proved. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner BuildingTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<BuildingBehavior xmlns='uri:ea.com:eala:asset' id='RebornBuildingTemplate'/>");return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.BuildingBehaviorModuleData));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: PowerUpgrade adds no field; the template selects the family, while triggers/animation/flags use the shared upgrade contract. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptModuleScalarContract.Owner PowerUpgradeTemplate()
    {
        var xml=new XmlDocument();xml.LoadXml("<PowerUpgrade xmlns='uri:ea.com:eala:asset' id='RebornPowerUpgradeTemplate'/>");
        return new("Behaviors",0,xml.DocumentElement!,typeof(SageBinaryData.PowerUpgradeModuleData));
    }

    // Reborn: clone serialized values so XmlDocument does not lose default attribute value children during comparison.
    private static XmlDocument Clone(XmlDocument source)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));
    // Reborn: schema admission requires exactly one direct Behaviors container; missing/duplicate topology is never inferred.
    private static XmlElement Container(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Behaviors");
    // Reborn: keep authored child order and values while using reviewed template attribute order for Core occurrence assignment.
    private static void Align(XmlElement actual,XmlElement model,HashSet<string> upgradeIds)
    {
        var order=model.Attributes.OfType<XmlAttribute>().Select((a,i)=>(Key:(a.NamespaceURI,a.LocalName),Index:i)).ToDictionary(p=>p.Key,p=>p.Index);
        var attributes=actual.Attributes.OfType<XmlAttribute>().OrderBy(a=>order.GetValueOrDefault((a.NamespaceURI,a.LocalName),int.MaxValue)).ToArray();foreach(var a in attributes)actual.RemoveAttributeNode(a);foreach(var a in attributes)actual.Attributes.Append(a);
        bool flexible=upgradeIds.Contains(actual.GetAttribute("id"));
        // Reborn: optional display-name children contain text only and cannot alter Core asset-reference occurrence ordering.
        // Reborn: each source-sized movement row has one strong field, so stock row-count alignment cannot determine its occurrence order.
        // Reborn: Geometry contains no strong reference occurrences and its source-sized children must not align to a one-shape stock sample.
        bool Skip(XmlElement e)=>(flexible&&ConscriptUpgradeStructure.Fields.Contains(e.LocalName))||(actual.LocalName=="GameObject"&&(GameObjectRootTextContract.Pointed.Contains(e.LocalName)||GameObjectExtendedRootContract.Names.Contains(e.LocalName,StringComparer.Ordinal)||e.LocalName is "ArmorSet" or "LocomotorSet" or "ObjectResourceInfo" or "Geometry" or "VisionInfo" or "GameDependency" or "AI"))||(actual.LocalName=="WeaponSetUpdate"&&e.LocalName=="WeaponSlotHardpoint")||(actual.LocalName=="BuildingDestructionBehavior"&&e.LocalName is "Item" or "DieMuxData");
        // Reborn: Spawn weak rows and optional records contain no strong-reference occurrences requiring stock attribute-order alignment.
        bool SpawnSkip(XmlElement e)=>actual.LocalName=="SpawnBehavior"&&SpawnBehaviorContract.OwnChildren.Contains(e.LocalName)||actual.LocalName=="EngineerContain"&&e.LocalName=="CanEnterFilter"||actual.LocalName=="AssistedPlacementBehavior"&&e.LocalName=="RequirementObjectFilter"||actual.LocalName=="StructurePlacementBehavior"&&e.LocalName=="RestrictedAreaShape";
        // Reborn: the optional engineer filter holds only weak names and enum/flag values, never strong-reference occurrences.
        // Reborn: FX event occurrences retain authored order; schema/catalog and literal metadata validate all children instead of stock count alignment.
        bool FxSkip(XmlElement e)=>actual.LocalName=="FXListBehavior"&&e.LocalName is "DieMuxData" or "Event";
        // Reborn: tracker effects keep source order and are validated with independent typed import occurrences, not template count alignment.
        bool TrackerSkip(XmlElement e)=>actual.LocalName=="SecondaryDamageTracker"&&e.LocalName is "Color" or "EffectInfo" or "ObjectStatusValidation";
        // Reborn: strong death-upgrade occurrences keep authored order; source-sized lists cannot align to a fixed sample count.
        // Reborn: translation rows retain authored reference order without aligning their count to an empty stock template.
        bool DeathSkip(XmlElement e)=>(actual.LocalName is "CreateObjectDie" or "DestroyDie")&&(e.LocalName is "DieMuxData" or "UpgradeRequired" or "UpgradeForbidden")||actual.LocalName=="ProductionUpdate"&&e.LocalName=="GiveUpgradeToProduced";
        var children=actual.ChildNodes.OfType<XmlElement>().Where(e=>!Skip(e)&&!SpawnSkip(e)&&!FxSkip(e)&&!TrackerSkip(e)&&!DeathSkip(e)).ToArray();var models=model.ChildNodes.OfType<XmlElement>().Where(e=>!Skip(e)&&!SpawnSkip(e)&&!FxSkip(e)&&!TrackerSkip(e)&&!DeathSkip(e)).ToArray();if(children.Length!=models.Length)throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported variable source child structure."),actual);
        for(int i=0;i<children.Length;i++){if(children[i].LocalName!=models[i].LocalName)throw SdkXmlDiagnostics.At(new InvalidDataException("Unsupported variable source child order."),children[i]);Align(children[i],models[i],upgradeIds);}
    }
}
