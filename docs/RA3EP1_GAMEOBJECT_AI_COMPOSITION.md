# Shared inline AI, combat, geometry and pointed-root projection

October 15, 2026. The common EP1 AI fragments now join the existing selected
GameObject root/armor/movement/Body/three-behavior/Geometry/VisionInfo graph. AI is
an **inline count/items list at +480/+484**, not a pointer-to-list header. Its
stock-pinned root type tag is **5FD8691D**. Counts 0, 1 and 2 are supported in this
selected projection; three entries refuse. The native root remains 600 bytes.

This is a shared source/native/token milestone, not complete GameObject asset
publication. The largest selected partial root reaches **100 fields**, compared
with the 126-field stock root inventory. An absent AI list emits no extra field
in the partial profile, preserving previous no-AI cases.

## Native ownership and metadata composition

AI modules allocate before Behaviors on the same real Tracker as other root
children. Optional camera/vision records can follow the last AI allocation. The
new Ep1GameObjectAiRoot gate therefore partitions each AI record using the inline
pointer array, subsequent modules and independently reviewed other root owners;
it does not blindly consume all bytes up to Behaviors as an AI tail.

The gate refuses array/record/string aliases, out-of-order modules, cross-owner
pointers and auxiliary words crossing fragment boundaries. Root strings, armor,
movement and pointed child records/text participate in disjointness checks. Each
extracted AI is independently checked by its standalone tokenizer before its
local auxiliary slots are admitted into the root's exact ownership inventory.

The AI metadata follows existing fragment/pointed-root metadata, with a count
anchor at native root+480, ordered element markers and generated concrete class
fragments. Standalone AI metadata/native origins remain distinct when joined;
the complete original native pointer graph and auxiliary slot locations rebase
globally once. Strong occurrence index values do not rebase.

## One Core table and one selected module-ID scope

A focused source fixture extends the existing vision/root/combat closure with
actual official AI declarations. Raw adapted GameObject source validates against
the reviewed official schema candidate before focused Core normalization. The
normalizer creates one occurrence table for root/armor/movement/body/weapons/FX
and AI chooser references. Unique target identities come from independent linked
stock catalogs, or a separately built genuine new armor target.

The source adapter restores actual AI-before-Body child order while retaining
already normalized reference indices. Tests deliberately retain repeated chooser
occurrences as distinct indices, including mixed two-AI graphs with zero, one or
two chooser references. No captured stock numeric index is accepted as authored
source input. This reference seam is not the complete general DocumentProcessor
or instance/inheritance/default source-graph pipeline.

Local id hashes now share one selected scope across Body, Behaviors and common
AI modules. Empty, duplicate and case-folded collisions refuse before allocation;
native duplicates refuse independently. A destruction AssociatedModule link can
resolve forward to an AI id in that scope. Unknown targets still refuse. Draw,
client and other AI classes are not silently included in this scoped resolver.

## Tests and evidence boundaries

`gameobject-ai-composition-self-test <static> <global>` covers **864** combinations:
all 16 earlier pointed-root presence masks, three text/default/chooser profiles,
AI counts 0/1/2 and all six selected behavior orders. Vision is present with three
reviewed profiles; armor/movement/Body/geometry and previous independent fragment
checks remain active. Repeated outputs match native, relocation and import bytes.

Independent verification checks the inline list/tag/markers, concrete ids, local
module hashes, state/default locomotor values, text, native flags and actual Core
target identities/indices. Each composed AI fragment is separately inverted into
local metadata/native coordinates, then checked against the literal-stock-hash
standalone verifier. It does not regenerate metadata to repair detached output.

The initial standalone shared test rejects **18 native**, **7 source** and
**287 detached metadata** corruptions, including every metadata word in both
present-chooser AI fragments. Array/pointer/record aliases, missing relocation
slots, unknown class/id/ref inputs and scope collisions cannot silently succeed.

`gameobject-authored-ai-composition-self-test <fresh-output> <static> <global>`
repeats the same combinations with a genuinely built RebornComposedArmor target;
removing that target must refuse. That mode builds the armor asset, not a complete
GameObject stream or playable package. Both modes remain empty-only for AI contact
and vision lists; native-only contact support is not broadened by composition.

Stock proof is deliberately scoped: the root AI tag/count relationship is checked
in two independent stock objects; their complete AI prefixes/native graphs were
checked in the preceding standalone gates. The synthesized combined object is
not claimed to equal an original whole stock object or to have gameplay proof.
Non-null Flammability/camera stock evidence remains separately open.

## Files and remaining gates

`Ep1GameObjectAiRoot.cs`, `Ep1GameObjectStringTokens.cs`,
`Ep1GameObjectDestructionProjection.cs`, `Ep1SelectedModuleIdentity.cs`, selected
allocation/adaptation helpers, `GameObjectAiCompositionSmokeTest.cs` and
`tests/fixtures/RootAiPipeline.xsd`. Existing verifiers accept explicit extra
metadata/import ownership rather than dropping their previous checks.

The old minimal Body/SMC/AI refusal in the native AI test becomes a positive
partial-composition control; historical native/token reports describe the earlier
boundary. Complete publication is still disabled. Remaining gates include other
root/Draw/client/container graphs, nonempty AI lists and other concrete classes,
general source/processor closure, mixed GameObject streams and runtime acceptance.
The Studio alpha and installed files are unchanged; no game is launched.

## Final regression

`artifacts/GameObjectAiComposition-a/batch-report.json`: **55/55 Focused groups
pass in 423.644 seconds**, stock/authored shared-AI groups 26.400/26.070 seconds.
Each mode covers 864 combinations and rejects 18 native/287 detached metadata
faults; source refusals are 7/8 respectively, including missing new armor in the
authored mode. Eight runtime hashes remain stable. The batch build succeeds in
63.30 seconds with two existing CS8632 audio warnings and no errors.
GameExecuted, InstalledFilesModified and AlphaCapabilitiesExpanded are false.
No Full-suite or gameplay run is claimed.
