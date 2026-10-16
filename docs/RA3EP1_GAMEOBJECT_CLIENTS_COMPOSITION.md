# Selected GameObject root with complete Conscript clients

## Outcome

The selected partial root/Draws/Body/behavior/common-AI graph now includes the
original **six complete ClientBehaviors and one complete ClientUpdate** on one
actual Core normalization pass, one occurrence table and one native Tracker.
The maximum combined case has **104 partial root fields / 237 strong reference
occurrences** (previous root/Draws gate: 102 / 189). No independently normalized
or compiled sibling bytes are appended to create the shared native graph.

All four complete Draws retain their independent **58,608-byte** stock metadata
oracle. Both client containers retain independently recovered complete stock
prefixes: **7,404 bytes** for ClientBehaviors, **216 bytes** for ClientUpdates
(24-byte list prefix plus the 192-byte original group-audio leaf prefix).

This is **not** the complete original 49-module Conscript. The test's Body,
three selected behaviors and common AI/root profiles are still the previously
reviewed authored composition fixtures, not proof that every original root or
behavior owner is closed. No SDK alpha admission, complete object stream,
package, general inheritance or new gameplay capability is claimed.

## Compiler/composer changes

- The selected source allocator detaches ClientBehaviors from the legacy KW
  dispatch, invokes the explicit EP1 allocator on the same Tracker, then
  allocates selected behavior/Body/geometry tails. ClientUpdates uses actual
  root dispatch with the reviewed internal class identity.
- `Ep1GameObjectClientRoots` partitions both client owners before the selected
  behavior envelope, excluding Draws, AI, armor/movement, strings and pointed
  root allocations. Sibling clients cannot alias or absorb each other's heap.
- The existing Draws/AI boundary checks now treat following client headers as
  independent owner boundaries. Every extracted client relocation must stay
  inside its own container, with exact complete auxiliary inventories.
- ClientUpdates reuses the same canonical pointer-list verifier/composer as
  Draws, but its only admitted leaf tokenizer is the independently stock-proven
  `Ep1GroupAudioClientTokenizer`. It does not gain other client update classes.
- Partial root metadata appends generated client tables after Draws. Official
  `ClientUpdateModules` / `ClientBehaviorModules` wire tags are independently
  checked against actual stock root fields. Absent and allocated-empty clients
  remain distinct. Native and metadata coordinate origins remain separate.
- Source and native identity scopes now include both client containers together
  with Draws, AI, Body and behaviors. Forward AssociatedModule links can target
  a client in the same object; unresolved/colliding IDs fail.

The selected native profile deliberately uses contiguous pre-behavior client
owners to support the existing bounded partial composer. It is not claimed to
be a general partitioner for arbitrary EA allocation order. Full root composition
will need the original source-owner graph rather than silently accepting layouts
outside this profile.

## Source and independent validation

Read-only official files: `BaseObjects/BaseObject.xml`,
`BaseObjects/BaseInfantry.xml`, `Soviet/Units/SovietAntiInfantryInfantry.xml`,
plus the earlier independently captured scripted/parachute/tracer Draw sources.
The explicit BaseObject -> BaseInfantry -> Conscript chain and the full six-leaf
client inventory are checked. Source SHA-256 values remain unchanged through
the client/root test. This explicit capture does not simulate a general include/
inheritance engine.

Plain Sound names resolve uniquely to schema-derived concrete audio asset
identities before one actual Core normalization over the entire selected object.
Every normalized strong target must exist exactly once in the real static/global
catalogs. Every import occurrence is consumed once; import values remain shared
across root, Draws and clients rather than being renumbered per fragment.

The test independently walks every original client native leaf against stock.
The second authored group-audio leaf and new-ID controls restore only their
expected IDs in owned comparison copies. The edited animation-sound range-cap
case is independently asserted as 17.5; its changed native topology is not
misrepresented as byte-identical to the unedited stock animation leaf.

Detached-output verification reverses the one global native shift and checks
all original native bytes and ordered RELO/IMP arrays. It binds root scalars,
sibling list headers, client markers and wrapper/base-chain identity targets.
The complete client and Draw prefixes normalize back to independent stock
oracles, not a second invocation of the composer.

## Scenarios and refusals

Thirteen combined source scenarios:

- Twelve combinations: ClientUpdates absent / allocated-empty / original single
  / two authored distinct-ID instances; ClientBehaviors absent / allocated-empty
  / all original six. Complete Draws is present in eight of these cases and
  absent in four, exercising AI/client boundaries without Draws as well.
- One full authored case: seven new client module IDs, AnimationSound range cap
  17.5 and a forward destruction-to-client AssociatedModule hash.

Each unedited case recompiles deterministically. The populated client leaves
remain complete, including the two BaseInfantry client behaviors. No voice,
frame list or sound-selector override is dropped to make the test pass.

**23 native refusals** cover sibling/root/AI/Draw aliases, bad counts/items,
cross-container IDs and nested pointers, missing root/header/item relocations,
missing client imports and an oversized pointer.

**Eight source refusals** cover client-to-Body/AI/Draw/behavior/update and
case-folded identity collisions, unknown update classes and duplicate containers.

**1,916 detached refusals** mutate every word of both original client prefixes,
their root tags/targets, representative native scalar/ID/pointer/reference
words, and both output auxiliary streams. These checks do not rerun the composer
to repair corrupted output. A missing terminal identity token is classified as
an explicit data error rather than an incidental dictionary lookup failure.

Existing populated EvaEvent, AudioArraySound and object-specific/named voice
token restrictions remain unchanged. This does not prove audio playback.

## Files / invocation / next work

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectClientRoots.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectStringTokens.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectDestructionProjection.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectAiRoot.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectDrawRoot.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1DrawsTokenizer.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1SelectedModuleIdentity.cs`
- `source/BinaryAssetBuilder.ManifestInspector/GameObjectBodyProjectionSmokeTest.cs`
- `source/BinaryAssetBuilder.ManifestInspector/GameObjectClientsCompositionSmokeTest.cs`

CLI: `gameobject-clients-composition-self-test <official-xml-root> <static.manifest> <global.manifest>`.
The Focused batch adds this group only with explicit `-UprisingXmlRoot`, while
all preceding standalone and shared root/Draws gates remain enabled separately.

Next major gate: close the remaining original Conscript Behaviors and original
root owners; then generate the full object/dependency stream and a genuinely
new-ID package for controlled gameplay validation. No reduced unit is substituted.

## Regression evidence

Final Focused batch with explicit official XML root: **65/65 groups passed** in
**230.342 seconds**; combined client group **23.529 seconds**. All eight runtime
hashes remain unchanged during the batch. Its initial build passes in 1.20
seconds with zero warnings/errors; the final incremental Inspector build also
has zero warnings/errors. The earlier dependency rebuild reports 111 existing
warnings and zero errors.

Evidence: `artifacts/ConscriptRootClients-a/batch-report.json`,
`compiler-build.log` and `gameobject-clients-composition.log`. The report records
`GameExecuted=false`, `InstalledFilesModified=false` and
`AlphaCapabilitiesExpanded=false`. No Full-profile run is claimed.
