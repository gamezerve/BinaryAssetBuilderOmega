# Conscript complete client-behavior metadata

## Outcome and evidence boundary

`Ep1ClientBehaviorsTokenizer.Tokenize(Chunk)` generates recursive metadata for
all six original Conscript ClientBehaviors from their complete native container.
It accepts no stock asset, token template, source file or unit identity as input.
The prefix matches **all 7,404 bytes** of the independent stock oracle after
independently established native-coordinate mapping: three audio loops, two
sound selectors and the animation-sound module, without dropping source members.

This is an offline diagnostic gate, **not whole GameObject admission** in the
SDK alpha, asset publication, general inheritance processing or audio/gameplay
evidence. The complete 49-module Conscript remains the target; no reduced unit
is substituted.

## Generator and ownership

- Original wire classes remain loop `5A3EDBF4`, selector `327CFB76` and animation
  sound `27EF791D`; the separate EP1 CLR selector name is not hashed.
- Parent chains, field order, polymorphic wrappers, list markers and terminated
  record tables are generated recursively from literal reviewed descriptors.
- Sound pointers retain absent versus allocated-empty distinctions. Animation
  Sound is instead a direct strong-reference word. Optional model masks,
  range/priority pointers, ordered frames and EP1 voice ordinals are checked.
- Allocations cannot overlap, alias another leaf or escape canonical leaf
  bounds. Only up to three zero alignment bytes can remain unowned.
- Limits: native 2 MiB, metadata 4 MiB, 64 modules, 4,096 list entries, depth 32,
  100,000 record visits. Pointer alignment, IDs and exact auxiliary inventories
  are enforced; metadata and native target patches are distinct.
- Native pointers and RELO/IMP slots shift once; auxiliary order and import
  occurrence values remain unchanged. Inputs are not modified.

The independent native comparison still covers all original leaves on one Core
table: **48 strong occurrences, 380 typed visits**, and 296 stock metadata
targets bound to the native graph. Official source files remain read-only and
their SHA-256 hashes are checked again after the test.

## Tests

CLI: `ep1-client-behaviors-token-self-test <official-xml-root> <static.manifest> <global.manifest>`.
Focused batch inclusion requires explicit `-UprisingXmlRoot`; the original
native-only command remains independently available.

The token gate checks complete stock-prefix equality, deterministic output and
exact native/RELO/IMP round trips. Nine authored token cases cover four
combinations of six new local IDs, optional range cap and one/three ordered
frames, then each family alone, reversed six-leaf order and a mixed subset.
The empty container produces a 12-byte prefix plus its 8-byte native header.

Twenty token refusals exercise missing/duplicate auxiliaries, bad count/header/
order/type/identity, undefined flags, cross-leaf/overlapping pointers, invalid
voice ordinal, extra storage and explicitly unreviewed populated children.
The inherited native gate retains its 67-voice/39-sound authored enum case,
36 native corruptions and **eight** source/dispatch refusals. That enum case's
populated AudioArraySound is native-only evidence, not token admission. The
token command therefore reports ten authored variants total, nine token-enabled.

## Explicitly unsupported populated children

The six original leaves have no populated AudioArraySound, ObjectSpecificEntry
or NamedEntry. These fail closed pending independent metadata evidence; even a
present-empty AudioArraySound allocation is currently refused.

Official `EvaEventRef` is a **strong AssetReference**; stock metadata targets the
native entry's four-byte slot directly. The legacy SoundOrEvaEvent declaration
uses a string pointer there. Empty stock values did not expose the discrepancy
in the preceding native proof. This change does not guess a populated ABI:
the EP1 voice marshaler rejects nonempty EvaEvent before calling the legacy base
marshaler, and the tokenizer rejects nonzero EvaEvent slots. A direct
post-normalization source control checks that guard without relying on Core's
missing-target rejection. The KW route is unchanged. Positive populated stock/
EA serializer evidence and an isolated EP1 reference record remain required.

## Files and next gate

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1ClientBehaviorsTokenizer.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1ClientBehaviors.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ClientBehaviorsNativeSmokeTest.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Program.cs`
- `scripts/Test-Ra3Ep1Batch.ps1`

Next: compose ClientBehaviors and previously reviewed ClientUpdates with the
selected root/Draws/Body/behavior/AI graph on one Tracker and Core table. Remaining
original Conscript root/behavior owners, full object metadata/dependencies,
mixed streams/packages and new-ID gameplay still follow.

## Regression

Final Focused batch with explicit official XML root: **64/64** in **210.929
seconds**; native client group 7.124 seconds, token group 7.016 seconds. All eight
runtime hashes remain unchanged through the batch. Its initial build succeeds
in 19.04 seconds with the two pre-existing audio nullable-context warnings and
zero errors. The earlier full dependency rebuild had 111 existing warnings and
zero errors; the focused incremental Inspector build had zero warnings/errors.

Evidence: `artifacts/ConscriptClientBehaviorsTokens-a/batch-report.json`,
`compiler-build.log`, `ep1-client-behaviors-native.log` and
`ep1-client-behaviors-tokens.log`. The report records `GameExecuted=false`,
`InstalledFilesModified=false` and `AlphaCapabilitiesExpanded=false`.
No Full-profile run is claimed.
