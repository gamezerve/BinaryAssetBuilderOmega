using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: use the existing core joiner for independently admitted sequence/repeated-choice copy and empty-child matching scopes; populated matching remains closed.
internal static class SdkSelfAttributeInheritance
{
    internal const string Name = "diagnostic-self-attribute-inheritance-v1";
    // Reborn: one-sided flat child copying is independently explicit; matching/replacing children remains closed.
    internal const string ChildCopyName = "diagnostic-self-child-copy-v1";
    // Reborn: complex leaf children require independent admission, without opening nested trees or populated-child matching.
    internal const string ComplexChildCopyName = "diagnostic-self-complex-child-copy-v1";
    // Reborn: nested sequence trees require a separate copy-only admission profile with bounded recursive schema lookup.
    internal const string TreeCopyName = "diagnostic-self-tree-copy-v1";
    // Reborn: two-sided matching is independently admitted only for empty complex children; populated branches/text concatenation remain closed.
    internal const string ChildMergeName = "diagnostic-self-empty-child-merge-v1";
    // Reborn: literal keyed empty-child removal is independently scoped; missing targets, singleton/nested removal and other directives remain closed.
    internal const string ChildRemovalName = "diagnostic-self-empty-child-removal-v1";
    // Reborn: nested flat repeated-choice copying is independent of older sequence-only profiles; singleton and populated matching stay closed.
    internal const string ChoiceCopyName = "diagnostic-self-repeated-choice-copy-v1";
    // Reborn: consumed inheritance markers follow core declaration loading; undeclared markers are limited to two reviewed local AI types.
    internal const string MarkerName = "diagnostic-self-consumed-inheritance-markers-v1";
    // Reborn: top-level AI enum-list modifiers are independently admitted only when core substring operations equal whole-token semantics.
    internal const string BitflagName = "diagnostic-self-bitflag-modifiers-v1";
    // Reborn: one-sided payload copying in matched micromanager ObjectFilters is independent of earlier empty-complex matching.
    internal const string FilterName = "diagnostic-self-object-filter-copy-v1";
    // Reborn: complementary upgrade singleton normalization is independent of older filter-copy admission.
    internal const string UpgradeName = "diagnostic-self-upgrade-normalization-v1";
    // Reborn: the local ObjectCreationList pipeline marker has independent evidence when composed by the expression-stage profile.
    internal const string ObjectCreationName = "diagnostic-self-object-creation-markers-v1";
    // Reborn: identical StrategicState normalization is explicit and cannot widen earlier sibling-identity guards.
    internal const string IdenticalStateName = "diagnostic-self-identical-states-v1";
    // Reborn: record ordered weak-reference payload witnesses separately from attribute overlays and native reference binding.
    internal sealed record Filter(string Type,string DerivedId,string BaseId,string ChildName,string PayloadSource,SdkFilterCopies.Leaf[] Leaves);
    // Reborn: modifier witnesses are diagnostic XML identities and actual core results, not native asset hashes.
    internal sealed record Bitflag(string Type,string DerivedId,string BaseId,string Attribute,string Before,string Modifiers,string After,int Operations);
    // Reborn: record consumed owner markers separately from overlays and retain whether the schema declared the pipeline attribute.
    internal sealed record Marker(string Type,string DerivedId,string BaseId,bool SchemaDeclared);
    private const string Instance = "uri:ea.com:eala:asset:instance";
    // Reborn: record actual source-directed removals separately from inheritance overlays and processed source hashes.
    internal sealed record Removal(string Type,string DerivedId,string BaseId,string ChildName,string ChildId);
    // Reborn: overlay evidence identifies source-level handles and transformed bytes, never native asset/stream identities.
    internal sealed record Overlay(string Type,string DerivedId,string BaseId);
    internal sealed record Evidence(string Profile,string RawSha256,string? ProcessedSha256,Overlay[] Overlays,string[] Diagnostics)
    {
        // Reborn: imported base identities are separate from local overlay handles and do not imply native asset hashes.
        public SdkInstanceInheritanceProfile.ImportedBase[] ImportedBases { get; init; } = Array.Empty<SdkInstanceInheritanceProfile.ImportedBase>();
        // Reborn: recursive preparation publishes the full diagnostic source closure separately from direct inherited handles.
        public SdkInstanceInheritanceProfile.PreparedSource[] PreparedSources { get; init; } = Array.Empty<SdkInstanceInheritanceProfile.PreparedSource>();
        // Reborn: removal witnesses report only commands executed in this owner document, not replayed inherited commands.
        public Removal[] Removals { get; init; } = Array.Empty<Removal>();
        // Reborn: older profiles retain their XML markers and publish no consumption witnesses.
        public Marker[] ConsumedMarkers { get; init; } = Array.Empty<Marker>();
        // Reborn: record only source-owner modifier operations that the core executed and the token proof verified.
        public Bitflag[] Bitflags { get; init; } = Array.Empty<Bitflag>();
        // Reborn: report only matched owner filters whose actual copied payload agrees with the pre-allocation proof.
        public Filter[] Filters { get; init; } = Array.Empty<Filter>();
        // Reborn: source-local normalization witnesses remain atomic with all other inheritance evidence.
        public SdkUpgradeNormalization.Witness[] UpgradeNormalizations { get; init; } = Array.Empty<SdkUpgradeNormalization.Witness>();
        // Reborn: Include-only metadata witnesses are separate from local normalization/imported asset authority.
        public SdkMetadataDefinitions.Witness[] MetadataDefinitionIncludes { get; init; } = Array.Empty<SdkMetadataDefinitions.Witness>();
        // Reborn: pre-overlay substitutions have their own raw/processed source identity and are withheld atomically on later failure.
        public SdkLocalDefineProfile.Evidence? ExpressionPreparation { get; init; }
        // Reborn: normalization events are source-owner-local and withheld atomically if any later gate fails.
        public SdkIdenticalStates.Witness[] IdenticalStates { get; init; } = Array.Empty<SdkIdenticalStates.Witness>();
    }
    internal sealed record Result(byte[]? Bytes,Evidence Evidence);
    private const string Ea = "uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expand local asset chains with independently admitted copy/empty-child matching scopes and reject the entire document on unsupported semantics. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Apply(XmlSchemaSet schemas,byte[] bytes,bool childCopy = false,bool complexChildCopy = false,bool treeCopy = false,bool childMerge = false,bool childRemoval = false,bool choiceCopy = false,bool consumeMarkers = false,bool bitflags = false,bool filters = false,bool upgrades = false,bool objectCreationMarkers = false,bool identicalStates = false)
    {
        string raw = Convert.ToHexString(SHA256.HashData(bytes));
        string profile = identicalStates ? IdenticalStateName : objectCreationMarkers ? ObjectCreationName : upgrades ? UpgradeName : filters ? FilterName : bitflags ? BitflagName : consumeMarkers ? MarkerName : choiceCopy ? ChoiceCopyName : childRemoval ? ChildRemovalName : childMerge ? ChildMergeName : treeCopy ? TreeCopyName : complexChildCopy ? ComplexChildCopyName : childCopy ? ChildCopyName : Name;
        // Reborn: the new local scope composes tested overlay rules without silently changing any earlier defaults.
        upgrades |= identicalStates;
        // Reborn: filter admission includes the tested bitflag/marker scopes without changing earlier profile defaults.
        filters |= upgrades;
        // Reborn: explicit upgrade normalization composes previous guards while leaving every earlier default unchanged.
        bitflags |= filters;
        // Reborn: explicit modifier admission includes consumed markers while older profiles keep rejecting list modifiers.
        consumeMarkers |= bitflags;
        // Reborn: marker consumption builds on the tested choice/removal subsets without broadening older flags.
        choiceCopy |= consumeMarkers;
        // Reborn: the explicit choice profile retains every earlier removal, matching, tree and resource guard.
        childRemoval |= choiceCopy;
        // Reborn: removal builds on the tested empty-child merge subset without widening its older flags.
        childMerge |= childRemoval;
        // Reborn: retain all existing tree guards when independently enabling the narrower matched-empty-child gate.
        treeCopy |= childMerge;
        // Reborn: recursive copying includes complex leaf admission without changing either earlier profile's scope.
        complexChildCopy |= treeCopy;
        // Reborn: the independently named complex profile includes the copy-only gate, never the child merge gate.
        childCopy |= complexChildCopy;
        try
        {
            if (!schemas.IsCompiled || bytes.Length > 4*1048576) throw new InvalidDataException("Compiled schema and bounded source required.");
            // Reborn: child-copy parsing follows core's default whitespace handling so formatting XmlWhitespace nodes do not become unexpected sequence children.
            XmlDocument xml = new() { XmlResolver = null,PreserveWhitespace = !childCopy };
            using (MemoryStream input = new(bytes,false))
            using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
            XmlElement? root = xml.DocumentElement;
            if (root?.LocalName != "AssetDeclaration" || root.NamespaceURI != Ea) throw new InvalidDataException("EA AssetDeclaration required.");
            var assets = root.ChildNodes.OfType<XmlElement>().Where(element => element.LocalName is not ("Includes" or "Defines" or "Tags")).ToArray();
            if (assets.Length > 4096) throw new InvalidDataException("4096 local asset bound exceeded.");
            // Reborn: count original asset/tree elements once per document; resolved copies are separately covered by amplification limits.
            int treeNodes = assets.Length;
            Dictionary<string,XmlElement> originals = new(StringComparer.Ordinal),resolved = new(StringComparer.Ordinal);
            // Reborn: track semantic chain height as well as active recursion so memoized/forward declarations cannot bypass the chain bound.
            Dictionary<string,int> heights = new(StringComparer.Ordinal);
            HashSet<string> active = new(StringComparer.Ordinal); List<Overlay> overlays = new();
            // Reborn: any later failure withholds all earlier removal witnesses along with partial transformed XML.
            List<Removal> removals = new();
            // Reborn: consumption evidence is atomic and source-owner-local, never inherited from an imported prepared base.
            List<Marker> markers = new();
            // Reborn: any failure withholds all previously executed modifier evidence together with transformed XML.
            List<Bitflag> bitflagEvidence = new();
            // Reborn: filter evidence is atomic with all other local overlay operations.
            List<Filter> filterEvidence = new();
            // Reborn: count the original tree before folding and retain only actually normalized source-owner events.
            List<SdkUpgradeNormalization.Witness> upgradeEvidence = new();
            if (upgrades && assets.Any(asset => asset.LocalName == "UpgradeTemplate" && asset.ChildNodes.OfType<XmlElement>().Count(child => child.LocalName == "GameDependency") > 1))
            {
                if (root.SelectNodes(".//*")!.Count > 8192) throw new InvalidDataException("8192-element pre-normalization tree bound exceeded.");
                foreach (var asset in assets)
                {
                    var witness = SdkUpgradeNormalization.Normalize(schemas,asset);
                    if (witness != null) upgradeEvidence.Add(witness);
                }
                assets = root.ChildNodes.OfType<XmlElement>().Where(element => element.LocalName is not ("Includes" or "Defines" or "Tags")).ToArray();
            }
            // Reborn: count the raw tree before folding, then normalize only proved identical direct-state groups in memory.
            List<SdkIdenticalStates.Witness> identicalStateEvidence = new();
            if (identicalStates)
            {
                if (root.SelectNodes(".//*")!.Count > 8192) throw new InvalidDataException("8192-element pre-normalization tree bound exceeded.");
                foreach (var asset in assets) identicalStateEvidence.AddRange(SdkIdenticalStates.Normalize(schemas,asset));
            }
            // Reborn: bound aggregate inherited-attribute amplification before allocating each merged node, not only after final serialization.
            long expandedBytes = bytes.Length+1024L;
            foreach (var asset in assets)
            {
                string id = asset.GetAttribute("id");
                if (asset.NamespaceURI != Ea || !Token(id) || !originals.TryAdd(asset.LocalName+":"+id,asset)) throw new InvalidDataException("Unsupported or duplicate local asset identity.");
                // Reborn: require the whole document's asset slice to stay inside its expression-free admitted scope; unrelated unsupported assets cannot ride along unprocessed.
                CheckLeaf(asset);
            }
            // Reborn: inherited attributes on nested/metadata elements cannot silently bypass this top-level-only profile.
            if (root.SelectNodes(".//*")!.OfType<XmlElement>().Any(element => element.HasAttribute("inheritFrom") && !assets.Contains(element))) throw new InvalidDataException("Nested inheritFrom is outside the profile.");
            foreach (var asset in assets.Where(element => element.HasAttribute("inheritFrom"))) Resolve(asset.LocalName+":"+asset.GetAttribute("id"),0);
            foreach (var asset in assets.Where(element => element.HasAttribute("inheritFrom"))) root.ReplaceChild(resolved[asset.LocalName+":"+asset.GetAttribute("id")],asset);
            using MemoryStream output = new();
            using (XmlWriter writer = XmlWriter.Create(output,new XmlWriterSettings { Encoding = new UTF8Encoding(false),NewLineHandling = NewLineHandling.None })) xml.Save(writer);
            byte[] processed = output.ToArray();
            if (processed.Length > 4*1048576) throw new InvalidDataException("Processed inheritance XML exceeds 4 MiB.");
            return new(processed,new(profile,raw,Convert.ToHexString(SHA256.HashData(processed)),overlays.ToArray(),Array.Empty<string>()) { Removals = removals.ToArray(),ConsumedMarkers = markers.ToArray(),Bitflags = bitflagEvidence.ToArray(),Filters = filterEvidence.ToArray(),UpgradeNormalizations = upgradeEvidence.ToArray(),IdenticalStates = identicalStateEvidence.ToArray() });

            //-------------------------------------------------------------------------------------------------
            /** Reborn: local handles precede imported visibility; reject same-handle overrides, missing bases, cross-type inheritance and cycles. */
            //-------------------------------------------------------------------------------------------------
            XmlElement Resolve(string handle,int depth)
            {
                if (depth > 32 || active.Contains(handle)) throw new InvalidDataException("Local inheritance cycle/depth bound exceeded.");
                if (resolved.TryGetValue(handle,out var previous)) return previous;
                if (!originals.TryGetValue(handle,out var asset)) throw new InvalidDataException("Base is not a uniquely captured same-document asset; Include visibility remains closed.");
                // Reborn: recursive tree validation already visited every original asset; do not count memoized resolution as newly authored tree nodes.
                if (!treeCopy) CheckLeaf(asset);
                active.Add(handle);
                XmlElement result = (XmlElement)asset.CloneNode(true);
                int height = 0;
                if (asset.HasAttribute("inheritFrom"))
                {
                    string target = asset.GetAttribute("inheritFrom"),type = asset.LocalName;
                    string baseId = target;
                    if (target.Contains(':'))
                    { var parts = target.Split(':'); if (parts.Length != 2 || parts[0] != type) throw new InvalidDataException("Cross-type inherited handle is outside the profile."); baseId = parts[1]; }
                    if (!Token(baseId) || baseId == asset.GetAttribute("id")) throw new InvalidDataException("Empty/unsafe base or same-handle imported override remains closed.");
                    XmlElement baseAsset = Resolve(type+":"+baseId,depth+1);
                    // Reborn: collect only this overlay's paired-filter plans; recursive source overlays retain their own event identity.
                    List<(string Child,SdkFilterCopies.Plan Plan)> filterPlans = new();
                    // Reborn: prove every selected modifier against the fully resolved explicit base field before core allocation.
                    List<(string Attribute,SdkBitflagModifiers.Plan Plan)> plans = new();
                    if (bitflags)
                        foreach (XmlAttribute attribute in asset.Attributes)
                            if (attribute.NamespaceURI.Length == 0 && (attribute.Name is "VitalKindOf" or "ForbiddenKindOf") && (attribute.Value.Contains('+') || attribute.Value.Contains('-')))
                            {
                                if (!baseAsset.HasAttribute(attribute.Name)) throw new InvalidDataException("Bitflag modifier requires an explicit resolved base attribute; defaults remain closed.");
                                var ownerType = (XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName(type,Ea)]!;
                                plans.Add((attribute.Name,SdkBitflagModifiers.Prove((XmlSchemaAttribute)ownerType.AttributeUses[new XmlQualifiedName(attribute.Name)]!,baseAsset.GetAttribute(attribute.Name),attribute.Value)));
                            }
                    // Reborn: both populated sides require actual child matching semantics and cannot pass a copy-only admission rule.
                    if (childCopy && asset.ChildNodes.OfType<XmlElement>().Any() && (baseAsset.ChildNodes.OfType<XmlElement>().Any() || asset.ChildNodes.OfType<XmlElement>().Any(RemoveCommand)))
                    {
                        if (!childMerge) throw new InvalidDataException("Both base and derived contain children; child merge semantics remain closed.");
                        CheckMerge(baseAsset,asset,filterPlans);
                    }
                    height = heights[type+":"+baseId]+1;
                    if (height > 32) throw new InvalidDataException("32-link local inheritance chain bound exceeded.");
                    foreach (XmlAttribute attribute in baseAsset.Attributes)
                        if (attribute.NamespaceURI.Length == 0 && !asset.HasAttribute(attribute.Name)) expandedBytes += Encoding.UTF8.GetByteCount(attribute.OuterXml)+1L;
                    // Reborn: modifier fields retain base text despite authored replacement attributes; charge that additional amplification before joining.
                    foreach (var plan in plans) expandedBytes += Encoding.UTF8.GetByteCount(plan.Plan.Before)+1L;
                    if (childCopy) foreach (XmlElement child in baseAsset.ChildNodes.OfType<XmlElement>()) expandedBytes += Encoding.UTF8.GetByteCount(child.OuterXml)+1L;
                    if (expandedBytes > 4*1048576) throw new InvalidDataException("Inherited attribute amplification exceeds 4 MiB before merge.");
                    // Reborn: core InstanceDeclaration.XmlNode consumes inheritFrom before joining; retain the original handle only in diagnostic evidence.
                    XmlElement joinSource = asset;
                    if (consumeMarkers)
                    {
                        joinSource = (XmlElement)asset.CloneNode(true); joinSource.RemoveAttribute("inheritFrom");
                        var schemaType = (XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName(type,Ea)]!;
                        markers.Add(new(type,asset.GetAttribute("id"),baseId,schemaType.AttributeUses[new XmlQualifiedName("inheritFrom")] is XmlSchemaAttribute));
                    }
                    result = (XmlElement)NodeJoiner.Override(schemas,xml,baseAsset,joinSource);
                    // Reborn: confirm exact anonymous payload order and multiplicity after the actual core merge; do not deduplicate or concatenate references.
                    foreach (var plan in filterPlans)
                    {
                        var actual = result.ChildNodes.OfType<XmlElement>().Single(child => child.LocalName == plan.Child);
                        var leaves = SdkFilterCopies.Leaves(actual);
                        if (!leaves.SequenceEqual(plan.Plan.Expected)) throw new InvalidDataException("Core ObjectFilter payload differs from one-sided copy proof.");
                        filterEvidence.Add(new(type,asset.GetAttribute("id"),baseId,plan.Child,plan.Plan.PayloadSource,leaves));
                    }
                    // Reborn: trust no transformed output unless actual core token order/membership matches the pre-allocation proof.
                    foreach (var plan in plans)
                    {
                        string after = result.GetAttribute(plan.Attribute);
                        if (!SdkBitflagModifiers.Tokens(after).SequenceEqual(plan.Plan.Expected,StringComparer.Ordinal)) throw new InvalidDataException("Core bitflag result differs from whole-token proof.");
                        bitflagEvidence.Add(new(type,asset.GetAttribute("id"),baseId,plan.Attribute,plan.Plan.Before,plan.Plan.Modifiers,after,plan.Plan.Operations));
                    }
                    overlays.Add(new(type,asset.GetAttribute("id"),baseId));
                }
                active.Remove(handle); resolved.Add(handle,result); heights.Add(handle,height); return result;
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: mirror core singleton-name/repeated-ID selection without permitting cross-QName replacement, matched text append or recursive populated branches. */
            //-------------------------------------------------------------------------------------------------
            void CheckMerge(XmlElement baseAsset,XmlElement derived,List<(string Child,SdkFilterCopies.Plan Plan)> filterPlans)
            {
                var type = (XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName(derived.LocalName,Ea)]!;
                if (type.ContentTypeParticle is not XmlSchemaSequence sequence) throw new InvalidDataException("Direct sequence merge schema required.");
                var before = baseAsset.ChildNodes.OfType<XmlElement>().ToArray();
                var after = derived.ChildNodes.OfType<XmlElement>().ToArray();
                // Reborn: core ID lookup spans sibling QNames; refuse collisions even where another singleton would otherwise be selected by name.
                foreach (var child in after.Where(child => child.HasAttribute("id")))
                    if (before.Any(old => old.GetAttribute("id") == child.GetAttribute("id") && old.LocalName != child.LocalName))
                        throw new InvalidDataException("Cross-QName child ID collisions remain closed.");
                Dictionary<string,int> counts = before.GroupBy(child => child.LocalName).ToDictionary(group => group.Key,group => group.Count(),StringComparer.Ordinal);
                foreach (var child in after)
                {
                    var declaration = sequence.Items.OfType<XmlSchemaElement>().Single(item => item.QualifiedName == new XmlQualifiedName(child.LocalName,Ea));
                    XmlElement? matched = declaration.MaxOccurs > 1
                        ? child.HasAttribute("id") ? before.SingleOrDefault(old => old.GetAttribute("id") == child.GetAttribute("id")) : null
                        : before.SingleOrDefault(old => old.LocalName == child.LocalName);
                    // Reborn: core warns on absent removal and matches IDs across QNames; this profile instead requires an existing exact named/keyed empty-complex target.
                    if (RemoveCommand(child))
                    {
                        if (!childRemoval || matched == null || matched.LocalName != child.LocalName) throw new InvalidDataException("Removal requires an existing same-QName direct child ID.");
                        counts[child.LocalName]--;
                        // Reborn: report the resolved base's literal ID consistently even when inheritFrom used a qualified Type:id handle.
                        removals.Add(new(derived.LocalName,derived.GetAttribute("id"),baseAsset.GetAttribute("id"),child.LocalName,child.GetAttribute("id")));
                        continue;
                    }
                    if (matched != null)
                    {
                        // Reborn: only AIMicroManagerData/IgnoreTargets may pair a named element-only filter; all other populated/branch matching stays closed.
                        if (filters && derived.LocalName == "AIMicroManagerData" && child.LocalName == "IgnoreTargets")
                        { filterPlans.Add((child.LocalName,SdkFilterCopies.Prove(declaration,matched,child))); continue; }
                        if (declaration.ElementSchemaType is not XmlSchemaComplexType leaf || leaf.ContentType != XmlSchemaContentType.Empty)
                            throw new InvalidDataException("Matched children require empty complex content; text append and populated branch merging remain closed.");
                    }
                    else
                    {
                        counts.TryGetValue(child.LocalName,out int count); counts[child.LocalName] = ++count;
                        if (count > declaration.MaxOccurs) throw new InvalidDataException("Merged child occurrence bound exceeded before core allocation.");
                    }
                }
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: gate asset attributes and the selected child scope before invoking the core joiner; expressions/directives/list modifiers remain closed. */
            //-------------------------------------------------------------------------------------------------
            void CheckLeaf(XmlElement asset)
            {
                // Reborn: core's unvalidated schema lookup uses node.Name rather than LocalName; prefixed trees are conservatively refused.
                if (treeCopy && asset.Prefix.Length != 0) throw new InvalidDataException("Prefixed asset/tree names require separately reviewed core lookup.");
                if (!childCopy && asset.ChildNodes.OfType<XmlNode>().Any(node => node is not XmlComment && !((node.NodeType is XmlNodeType.Text or XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace) && string.IsNullOrWhiteSpace(node.Value)))) throw new InvalidDataException("Only attribute-only base/derived assets are admitted.");
                var name = new XmlQualifiedName(asset.LocalName,Ea);
                if (schemas.GlobalTypes[name] is not XmlSchemaComplexType type || type.AttributeWildcard != null) throw new InvalidDataException("Exact named complex asset schema without attribute wildcard required.");
                foreach (XmlAttribute attribute in asset.Attributes)
                {
                    if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                    if (attribute.NamespaceURI.Length != 0 || attribute.Name == "TypeId" || attribute.Value.StartsWith('=')) throw new InvalidDataException("Namespaced directives, TypeId or unevaluated attributes require broader preprocessing.");
                    // Reborn: only reviewed local marker exceptions bypass schema attribute lookup; ObjectCreationList additionally requires the explicit expression-stage flag.
                    if (consumeMarkers && attribute.Name == "inheritFrom" && ReviewedMarkerType(type,objectCreationMarkers)) continue;
                    if (type.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute use) throw new InvalidDataException("Unknown asset attribute cannot be hidden by an overlay.");
                    if (use.AttributeSchemaType?.Datatype?.Variety == XmlSchemaDatatypeVariety.List && (attribute.Value.Contains('+') || attribute.Value.Contains('-')))
                    {
                        // Reborn: only signed top-level KindOf fields on inherited AITargetingHeuristic assets receive the separately reviewed syntax gate.
                        if (!bitflags || asset.LocalName != "AITargetingHeuristic" || !asset.HasAttribute("inheritFrom") || attribute.Name is not ("VitalKindOf" or "ForbiddenKindOf")) throw new InvalidDataException("Bitflag/list modifiers require separately reviewed semantics.");
                        SdkBitflagModifiers.Syntax(use,attribute.Value);
                    }
                }
                if (childCopy) CheckChildren(asset,type);
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: admit sequence trees or independently selected nested unit repeated choices; keep singleton/structural particles and child directives closed. */
            //-------------------------------------------------------------------------------------------------
            void CheckChildren(XmlElement asset,XmlSchemaComplexType type,int depth = 0)
            {
                // Reborn: cap authored tree recursion before descending; source size and inheritance-chain limits are independent.
                if (treeCopy && depth > 32) throw new InvalidDataException("32-level child tree depth bound exceeded.");
                var children = asset.ChildNodes.OfType<XmlElement>().ToArray();
                // Reborn: leaf elements also consume one depth level, so the tree bound includes every authored child rather than only complex branches.
                if (treeCopy && depth >= 32 && children.Length > 0) throw new InvalidDataException("32-level child tree depth bound exceeded.");
                foreach (XmlNode node in asset.ChildNodes)
                    if (node is not XmlElement && node is not XmlComment && !((node.NodeType is XmlNodeType.Text or XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace) && string.IsNullOrWhiteSpace(node.Value))) throw new InvalidDataException("Meaningful asset text or non-element content is outside child-copy scope.");
                if (children.Length == 0) return;
                // Reborn: only nested flat repeated choices with exactly one element per alternative slot are new; top-level/singleton/structural particles stay closed.
                XmlSchemaObjectCollection declarations;
                bool repeatedChoice = false;
                if (type.ContentTypeParticle is XmlSchemaSequence sequence) declarations = sequence.Items;
                else if (choiceCopy && depth > 0 && type.ContentTypeParticle is XmlSchemaChoice choice && choice.MaxOccurs > 1)
                {
                    declarations = choice.Items; repeatedChoice = true;
                    if (declarations.OfType<XmlSchemaObject>().Any(item => item is not XmlSchemaElement element || element.MinOccurs != 1 || element.MaxOccurs != 1)
                        || children.Length < choice.MinOccurs || children.Length > choice.MaxOccurs)
                        throw new InvalidDataException("Repeated choice requires unit element alternatives and bounded aggregate cardinality before copying.");
                }
                else throw new InvalidDataException("Flat direct sequence child schema required; only explicit nested repeated-choice copying is admitted.");
                if (declarations.OfType<XmlSchemaObject>().Any(item => item is not XmlSchemaElement)) throw new InvalidDataException("Structural child particles remain closed.");
                Dictionary<XmlQualifiedName,int> counts = new();
                // Reborn: core repeated-child matching compares IDs across sibling names; uniqueness must therefore span all direct siblings, not one QName.
                HashSet<string> siblingIds = new(StringComparer.Ordinal);
                foreach (var child in children)
                {
                    // Reborn: a bounded element inventory prevents a small source with many tiny nested children from creating unbounded joiner work.
                    if (treeCopy && (++treeNodes > 8192 || child.Prefix.Length != 0)) throw new InvalidDataException("8192-element child tree bound or unprefixed core lookup required.");
                    // Reborn: unique literal sibling IDs cannot select an already copied node on the sole populated side; duplicates/unsafe identities remain closed.
                    if (treeCopy && child.HasAttribute("id") && (!Token(child.GetAttribute("id")) || !siblingIds.Add(child.GetAttribute("id")))) throw new InvalidDataException("Tree child IDs must be bounded literals unique across all siblings.");
                    var qualified = new XmlQualifiedName(child.LocalName,child.NamespaceURI);
                    var declaration = declarations.OfType<XmlSchemaElement>().SingleOrDefault(item => item.QualifiedName == qualified);
                    if (child.NamespaceURI != Ea || declaration == null) throw new InvalidDataException("Unknown sequence child is outside the profile.");
                    // Reborn: only empty direct repeated-child command stubs on inherited top-level assets can bypass required payload attributes before they are consumed by the core.
                    if (childRemoval && RemoveCommand(child))
                    {
                        if (depth != 0 || !asset.HasAttribute("inheritFrom") || declaration.MaxOccurs <= 1
                            || declaration.ElementSchemaType is not XmlSchemaComplexType empty || empty.ContentType != XmlSchemaContentType.Empty
                            || empty.AttributeWildcard != null || empty.AttributeUses[new XmlQualifiedName("id")] is not XmlSchemaAttribute
                            || !child.HasAttribute("id") || child.HasChildNodes
                            || child.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/"
                                && !(attribute.NamespaceURI.Length == 0 && attribute.Name == "id")
                                && !(attribute.NamespaceURI == Instance && attribute.LocalName == "joinAction" && attribute.Value == "Remove")))
                            throw new InvalidDataException("Only literal Remove/id stubs for direct repeated empty-complex children on inherited assets are admitted.");
                        continue;
                    }
                    // Reborn: complex admission includes branches only under the tree profile; every effective attribute is checked before the core sees it.
                    if (complexChildCopy && declaration.ElementSchemaType is XmlSchemaComplexType leaf)
                    {
                        if (leaf.AttributeWildcard != null || (leaf.ContentType is not (XmlSchemaContentType.TextOnly or XmlSchemaContentType.Empty) && !(treeCopy && leaf.ContentType == XmlSchemaContentType.ElementOnly))) throw new InvalidDataException("Only simpleContent/empty complex leaf children are admitted; nested/mixed content remains closed.");
                        foreach (XmlAttribute attribute in child.Attributes)
                        {
                            if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                            if (attribute.NamespaceURI.Length != 0 || attribute.Name is "TypeId" or "inheritFrom" || (attribute.Name == "id" && !treeCopy) || attribute.Value.StartsWith('=')) throw new InvalidDataException("Complex child identity, directives or expressions require broader preprocessing.");
                            if (leaf.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute use) throw new InvalidDataException("Unknown complex child attribute.");
                            if (use.AttributeSchemaType?.Datatype?.Variety == XmlSchemaDatatypeVariety.List && (attribute.Value.Contains('+') || attribute.Value.Contains('-'))) throw new InvalidDataException("Complex child list modifiers remain closed.");
                        }
                        if (leaf.ContentType == XmlSchemaContentType.Empty && !string.IsNullOrWhiteSpace(child.InnerText)) throw new InvalidDataException("Empty complex child contains text.");
                    }
                    else if (declaration.ElementSchemaType is not XmlSchemaSimpleType || child.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")) throw new InvalidDataException("Only attribute-free simple sequence children are admitted.");
                    counts.TryGetValue(qualified,out int count); counts[qualified] = ++count;
                    // Reborn: repeated choice slots may select the same unit alternative repeatedly; aggregate cardinality was checked before core allocation.
                    if (!repeatedChoice && count > declaration.MaxOccurs) throw new InvalidDataException("Child occurrence bound exceeded before copying.");
                    // Reborn: recurse through schema-selected declarations under the active sequence/repeated-choice gate; singleton/group/wildcard/mixed branches remain closed.
                    if (treeCopy && declaration.ElementSchemaType is XmlSchemaComplexType branch && branch.ContentType == XmlSchemaContentType.ElementOnly)
                    { CheckChildren(child,branch,depth+1); continue; }
                    foreach (XmlNode node in child.ChildNodes)
                    {
                        if (node is not XmlComment && node.NodeType is not (XmlNodeType.Text or XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace)) throw new InvalidDataException("Nested/CDATA/directive child content requires broader merging.");
                        if (node is not XmlComment && node.Value?.StartsWith('=') == true) throw new InvalidDataException("Child node expressions require preprocessing before inheritance.");
                    }
                    if (child.InnerText.StartsWith('=')) throw new InvalidDataException("Child expressions require preprocessing before inheritance.");
                }
            }
        }
        catch (Exception error) when (error is IOException or InvalidDataException or XmlException or ArgumentException or BinaryAssetBuilderException)
        { return new(null,new(profile,raw,null,Array.Empty<Overlay>(),new[] { error.Message[..Math.Min(error.Message.Length,512)] })); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit bounded literal local identity tokens without guessing hash/case/traversal normalization. */
    //-------------------------------------------------------------------------------------------------
    private static bool Token(string value) => value.Length is > 0 and <= 128 && value.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.');

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recognize only the exact instance-namespace Remove command; other directive spellings stay under the earlier refusal rules. */
    //-------------------------------------------------------------------------------------------------
    private static bool RemoveCommand(XmlElement element) => element.GetAttribute("joinAction",Instance) == "Remove";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit reviewed local EA pipeline markers with BaseAssetType ancestry; the ObjectCreationList exception is separately explicit, not a generic schema mismatch bypass. */
    //-------------------------------------------------------------------------------------------------
    private static bool ReviewedMarkerType(XmlSchemaComplexType type,bool objectCreationMarkers = false)
    {
        // Reborn: the third local pipeline-marker exception is explicit to expression-stage preparation, never an imported-inheritance or older-profile expansion.
        if (type.QualifiedName.Namespace != Ea || !(type.QualifiedName.Name is "AITargetingHeuristic" or "AIMicroManagerData" || objectCreationMarkers && type.QualifiedName.Name == "ObjectCreationList")) return false;
        for (int depth = 0; depth < 32; depth++)
        {
            if (type.QualifiedName == new XmlQualifiedName("BaseAssetType",Ea)) return true;
            if (type.BaseXmlSchemaType is not XmlSchemaComplexType parent || ReferenceEquals(type,parent)) break;
            type = parent;
        }
        return false;
    }
}
