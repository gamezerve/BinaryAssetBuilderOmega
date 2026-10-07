# Consumed inheritance markers — October 8, 2026

## Pipeline finding

The apparent XML/XSD mismatch in the two AI libraries is a missing diagnostic
preprocessing step, not evidence that their schemas should be changed.

In `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/InstanceDeclaration.cs`,
the `XmlNode` setter captures the unqualified `inheritFrom` value into
`InheritFromHandle` and then removes the attribute from the XML node. This happens
before the node is joined and validated. The new owned fixture executes this
**actual setter** and the unchanged `NodeJoiner.Override` to pin the behavior.

In `Core/SageXml/DocumentProcessor.ProcessDocumentContents`, the loaded-document
order is ProcessExpressions, ProcessOverrides, then Validate. In
`AssetDeclarationDocument.OverrideInstance`, the non-BaseInheritableAsset refusal
is conditional on the base not being in `FindLocation.Self`. Imported tentative
bases still need BaseInheritableAsset eligibility and a directly included
defining document. `ValidateInstances` computes later export eligibility from
the BaseInheritableAsset-derived type set. Local inheritance does not therefore
grant imported inheritance authority.

The repository's three relevant XSD files byte-match their Uprising reference
counterparts (SHA-256 comparison):

- `SkirmishAI/AssetTypeTargetHeuristics.xsd`;
- `SkirmishAI/AssetTypeAIMicroManager.xsd`;
- `Base/AssetBase.xsd`.

The corresponding official RA3 reference types also extend BaseAssetType, not
BaseInheritableAsset. References inspected read-only are under
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)`:
`Uprising\Schemas (Uprising)` and `Red Alert 3\Schemas (RA3)`.
The authored XML has five undeclared inheritFrom occurrences in
AITargetHeuristicLibrary and 122 in AIMicroManagerLibrary. No reference XSD/XML,
synced project source or core declaration/joiner file is changed.

## Independent diagnostic contract

`--instance-markers` selects `diagnostic-direct-instance-markers-v1`. It includes
the earlier independently tested chain/root-file/removal/choice subsets, but
does not alter older flags. Locally, `SdkSelfAttributeInheritance.Apply` selects
`diagnostic-self-consumed-inheritance-markers-v1`.

Every admitted top-level inheritance marker is consumed on an owned join-source
clone before invoking the core. Original source bytes and raw hashes are
preserved. Resolved XML no longer retains the marker. Normal declared markers
are consumed too, so processed hashes intentionally differ from retained-marker
profiles even where admission is unchanged.

The **undeclared** attribute exception is restricted to exactly two named EA
types, AITargetingHeuristic and AIMicroManagerData, with bounded BaseAssetType
ancestry. It is not a general unknown-attribute exemption. Handles still need
safe literal identity, same type, an existing distinct base and a bounded acyclic
chain. Imported non-inheritable bases remain rejected by the existing eligibility
gate before local delegation. No nested inheritance, namespaced marker,
expression, same-handle imported override or transitive export is added.

Evidence adds `ConsumedMarkers(Type, DerivedId, BaseId, SchemaDeclared)`. The
base ID is canonical literal identity even if the authored handle is Type:id.
Per-document witnesses record only that owner's commands; child commands are
represented by their prepared-source hashes, not replayed as owner consumption.
Older profiles publish an empty consumption array and retain their previous XML
behavior. Scope failure atomically withholds transformed XML, processed hashes,
overlays, consumed markers, removals, imported bases and prepared-source witnesses.
Final schema failure still withholds all trusted dependency fields.

## Real comparison: no premature validation claim

The full 396-source / 664-Include audit was rerun with both --instance-choices and
--instance-markers. Both report **385 Validated / 11 RequiresPreprocessing**;
there are zero newly valid documents and zero earlier-valid regressions.
There are no SchemaInvalid, Limit, StaleSource or SourceRead documents. Both
return expected exit 2, create no output directory and retain 198 unresolved typed
file dependencies. The existing six path issues remain. ScopedGraphComplete,
FullDependencyCoverage and ProductionBuildReady are false.

The two AI owners now reach their **next actual semantic blocker**:

| Owner | Previous first blocker | New first blocker |
| --- | --- | --- |
| SkirmishAI/AITargetHeuristicLibrary.xml | Undeclared inheritFrom | Bitflag/list modifier semantics |
| SkirmishAI/AIMicroManagerLibrary.xml | Undeclared inheritFrom | Matched element-only branch semantics |

Neither failed owner publishes consumption/overlay/prepared witnesses or trusted
dependencies. This preserves atomic refusal rather than reporting partially
prepared documents as successes.

Raw source hashes, not native identities:

- Target library: `C6A692CCAE543F4E20CA601D457CF70AB0A9756DACB822465CC50D0B447C7A4A`.
- Micromanager library: `46665EB746F2E88BCFCD4B315209E65A5FCB523996BC5F01B634344A5475FF4E`.

Read-only follow-up finds five modifier occurrences across VitalKindOf and
ForbiddenKindOf. Examples are AITarget_A07_BombardBase / ForbiddenKindOf /
`+CONSTRUCTION_YARD`, AITarget_J04_ClosestFactoryHeuristic / VitalKindOf /
`+FS_FACTORY`, and AITarget_J05_ClosestStructureHeuristic_NoRefinery /
ForbiddenKindOf / `+REFINERY`.

The micromanager library contains 13 IgnoreTargets blocks, five with element
payload. Their ObjectFilter schema is element-only with optional repeated
IncludeThing/ExcludeThing children. Even authored empty instances do not have
schema `ContentType.Empty`, so the current matched-empty-complex gate refuses
them. Admitting these instances alone would not prove the five populated filters
safe. No branch is flattened or discarded to raise the admission count.

## Code, tests and next step

The changes are in SdkSelfAttributeInheritance, SdkInstanceInheritanceProfile,
SdkTypedSourceGraph, Program, and the registered SdkInstanceMarkersSmokeTest.
The explicit focused command is `sdk-instance-markers-self-test`.

Fixtures prove real core setter consumption, both reviewed local AI exceptions,
qualified/local-chain identity, declared imported owner-only witnesses, inherited
root file fields and final invalid-payload no-fields binding. Negative fixtures
cover arbitrary unknown fields/types, wrong marker namespace, expressions,
missing/same/cyclic/cross-type handles and imported non-inheritable bases. Older
choice-only admission stays closed to the undeclared markers. Three CLI probes
reject conflicting profiles, duplicate marker flags and marker flags on path-only
preflight (exit 1, no output).

The focused marker test, **all 132 compiler groups**, 33 enum checks, three
classifier fixtures and three marker CLI isolation probes pass. The full suite
requires permission for its existing owned temporary audio-file move.
Compiler registration count is 132. Models/marshallers stay 785/1390 and 762/1390;
all 48 EP1-only complex types retain model/marshaller coverage.

Next, pin core enum-list modification semantics, including token identity,
substring hazards, absent destination attributes and final enum membership.
Then consider a separate bounded bitflag option. Element-only filter matching
requires its own child identity/cardinality/text-copy proof; marker consumption
does not authorize general populated-branch merging.

The manual weighted estimate remains **51% complete / 49% remaining**. The
validation count does not rise here because the initial preprocessing mismatch
hid subsequent blockers. This is a corrected pipeline contract with executable
evidence, not a working-mod readiness increase or game-load proof.
