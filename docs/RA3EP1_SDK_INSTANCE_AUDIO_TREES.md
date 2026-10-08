# Bounded shallow audio document breadth — October 8, 2026

## Real result and limits

Independent `--instance-audio-trees` /
`diagnostic-direct-instance-audio-trees-v1` advances the real typed source graph
to **395 Validated / 1 RequiresPreprocessing**, over 396 documents and 664 Include
edges. Old/new comparison finds zero earlier-valid regressions. Earlier
`--instance-music-offsets` remains 394 / 2; earlier/default tree limits stay 8192.

Whole `Sounds/Voice.xml` validates: 1,698 overlays, 884 exact-reference
substitutions, four directly imported AudioEvent/AudioEventOverridable bases and
two prepared source witnesses (Voice and BaseSoundEffect). The source contains
1,996 assets, with largest original owner subtree 21 elements. The error was
aggregate document breadth, not an individual deep or giant audio owner.

- Raw Voice SHA-256: `445E29B231D72DDE658CD5111B9D561935E4CAD2B55CB6AEB8781678152372AA`.
- Prepared graph-owner SHA-256: `99E3B235F4AF9B2CDBBE28F67672D735E7C4D2072EAE0E879B2D36FB3D658E96`.

The delegated Self/Core budget reports original 9,794 descendant elements,
2,000 owners, largest original owner 21 elements, final 10,028 descendant
elements and 9,456 merge element-pair units. These measurements exclude the
AssetDeclaration root and include the four temporarily injected prepared bases.
Final graph-owner serialization removes those temporary bases; the graph hash
above identifies that owner-only output, not the intermediate Self/Core tree.

Repeated audits reproduce the prepared hash. All 396 raw source hashes match;
post-audit hashes show zero changed sources. Reference XML/XSD and Core code
remain unchanged. No output directory or native stream is emitted.

The remaining first source-preparation blocker is SoundEffects.xml: arithmetic
outside the MusicTrack.Volume subset. Its later complete-tree/merge/schema
admission is not claimed by this milestone. Global audits still exit 2: six path
issues and 198 missing AUDIO-header dependency occurrences remain.
ScopedGraphComplete, FullDependencyCoverage and ProductionBuildReady remain false.

## Independent bounded contract

`SdkAudioTreeBudget.Prove` activates only when the explicit profile receives
more than 8192 descendant elements. Small documents retain the old limit and
receive no broad-audio budget witness. Large documents must meet all these gates:

- At most 16,384 descendant elements, 2,048 asset owners and 1,024 metadata
  elements outside asset subtrees.
- Only direct unprefixed EA AudioEvent, AudioEventOverridable, Multisound or
  MusicTrack owners, with named compiled complex schema types and no attribute
  wildcard. Generic object/AI/upgrade documents cannot use this exception.
- Each owner has at most 128 elements, eight child levels, 64 attributes per
  element and 512 total XML nodes including the owner. The separate node cap
  prevents comments/text density from bypassing element-only measurements.

Existing 4 MiB source/injected/output/amplification, 32 MiB source closure,
source hashes, inheritance depth, exact child schema, modifier, visibility and
whole-owner validation guards remain in force. This is not general large-tree,
new join semantics or unrestricted expression admission.

`Charge` checks the fully resolved base and original derived owner against the
individual caps before each actual Core join and charges their element-count
product. At most 1,048,576 aggregate units are permitted per delegated document;
memoized bases do not skip repeated charges. This is a diagnostic element-pair
budget, not a complete CPU-cost model, latency promise or production resource
proof: schema lookup, metadata processing and non-element operations are not
represented by that counter. Separate structural/node/byte guards still apply.

`SdkSelfAttributeInheritance.Apply` selects the larger pre-normalization and
child-walk limits only after proof. The unchanged NodeJoiner performs all merges.
`Verify` checks final descendant and per-owner limits before serialization and
publishes budget evidence atomically with other owner events. Any later failure
clears transformed bytes, budget evidence, overlays and source closure witnesses.

`SdkInstanceInheritanceProfile` carries this scope into each defining source and
retains captured/prepared imported-base provenance. The new profile composes the
previous music/state/expression subsets without changing their defaults.
`SdkTypedSourceGraph` and Program enforce exclusive profile selection; self-only
evidence is `diagnostic-self-audio-trees-v1`. Final scalar schema failure still
publishes zero trusted dependency fields.

## Validation and next work

`SdkInstanceAudioTreesSmokeTest.Run` is compiler group 145. It executes 1,000
actual Core overlays and checks inherited Volume plus full anonymous sound order.
Fixtures cover old 8192 isolation, exact 16,384 element / 2,048 owner /
1,048,576 pair-unit boundaries, excessive total/owner/depth/attribute/comment/
metadata/work/final-output sizes, non-audio owners, stale/late atomic failures,
final invalid-schema zero fields and profile conflict. All 145 compiler groups,
33 enum checks, three Include-classifier fixtures and three CLI isolation probes
pass. Final incremental build has zero warnings/errors.

Model/marshaller inventory stays 785/1,390 and 762/1,390; all 48 EP1-only complex
types retain both. Manual weighted engineering effort stays **51% complete /
49% remaining**. XML admission is not 395/396 usable-mod readiness; native type
layouts/hashes, resource closure, serializers, WorldBuilder packaging and game
loading still need independent proof.

Next: inventory SoundEffects arithmetic by exact schema field, definition origin,
operand grammar and value range. Design a separate typed arithmetic contract;
keep MusicTrack's 0..100 integer subset and the broad-audio resource caps intact.
