# Whole-token-proven bitflag modifiers — October 8, 2026

## Real result

The independent `--instance-bitflags` profile advances the 396-source /
664-Include graph from **385 Validated / 11 RequiresPreprocessing** to **386 / 10**.
The newly valid owner is `SkirmishAI/AITargetHeuristicLibrary.xml`. Its five
inheritance overlays, five consumed markers and five signed modifier operations
are recorded with one prepared source and no typed file-dependency fields.
There are zero earlier-valid regressions and no SchemaInvalid/Limit/StaleSource/
SourceRead documents. The earlier marker-only option remains at 385 / 11.

The final audit was repeated after tightening operand/token bounds and confirmed
the same processed XML hash. Expected exit 2, no output directory. Six existing
path issues and 198 unresolved typed dependency occurrences of the missing AUDIO
header remain. ScopedGraphComplete, FullDependencyCoverage and ProductionBuildReady
are false. No reference XML/XSD or core joiner edits, native stream emission or
game-load proof are included.

Raw source SHA-256:
`C6A692CCAE543F4E20CA601D457CF70AB0A9756DACB822465CC50D0B447C7A4A`

Processed owner-only XML SHA-256:
`0430144C0B290731427EDF593F5790567175D4FCBD31812FBAB50A5E987DBA0E`

These are XML witnesses, not native type/asset hashes.

## Why direct core admission is not sufficient

`NodeJoiner.ReplaceNodeAttributes` uses string Contains for membership and string
Replace for removal, not whole-token identity. Owned fixtures execute the actual
core and show:

- Base `ALPHA_EXTRA`, modifier `+ALPHA`: the distinct ALPHA token is not added.
  The output can still validate against the enum-list schema, hiding semantic loss.
- Base `ALPHA ALPHA_EXTRA`, modifier `-ALPHA`: both occurrences of the text are
  removed, leaving ` _EXTRA`, not the intended `ALPHA_EXTRA` token.

The new gate does not fix or replace the core implementation. It proves in
advance that the particular requested operation agrees with whole-token
semantics, then compares the actual core result's ordered tokens to that proof.
Ambiguous operations remain closed even if an output-only schema check would pass.

## Narrow independent contract

Option/profile: `--instance-bitflags` /
`diagnostic-direct-instance-bitflags-v1`. Local delegated profile:
`diagnostic-self-bitflag-modifiers-v1`. It includes consumed-marker/choice/removal/
chain/root-file guards, without changing earlier options.

Only top-level inherited **AITargetingHeuristic** attributes **VitalKindOf** and
**ForbiddenKindOf** are newly admitted. The compiled type must be the exact EA
KindOfBitFlags direct list whose item has only enumeration facets. Enumeration
members must be unique bounded ASCII identifier tokens; arbitrary list types,
restrictions and operator/whitespace token identities are refused.

Modifiers are at most 1024 characters, 64 operations, each a single `+` or `-`
followed by a case-sensitive known enum token, separated by exactly one ASCII
space after XML parsing. Empty operations, leading/trailing/doubled spaces,
mixed unsigned tokens and unknown names are refused before core invocation.
Operand length is checked before splitting, not after allocation.

The fully resolved base must have the attribute **explicitly present**; defaults
or a missing field are not guessed. An explicitly empty list is allowed. Base
text is capped at 4096 characters / 512 unique literal enum tokens. Additions
preserve order and adding an already present exact token is a no-op. Removals
require an existing exact token; a repeated removal fails after the first deletion.
Short-token membership/removal collisions with longer tokens are refused.
All operations are proved sequentially against the updated token state.

The unchanged core performs the actual modification. Its ordered nonempty tokens
must equal the prediction. Original raw spacing is not normalized away: the real
minus operation leaves a double space, which is preserved in processed XML and
reported After evidence. Final schema validation remains mandatory before any
trusted file fields are returned.

Pre-join amplification accounting additionally charges retained base list text
even though a derived modifier attribute exists. Existing 4 MiB processed/source,
32 MiB source-closure, depth/chain/node and confinement limits remain in force.
Nested child modifiers, arbitrary fields/types, expressions, populated branch
matching and imported non-inheritable bases remain closed.

Evidence adds `Bitflags(Type, DerivedId, BaseId, Attribute, Before, Modifiers,
After, Operations)`, only for commands executed in that source owner. Older
options publish an empty array. Failure withholds transformed bytes, processed
hashes, overlays, marker/removal/modifier events and imported/prepared witnesses
together; final schema failure still publishes no trusted dependency fields.

## Real operations

| Derived id | Field | Modifier |
| --- | --- | --- |
| AITarget_A07_BombardBase | ForbiddenKindOf | +CONSTRUCTION_YARD |
| AITarget_J04_ClosestFactoryHeuristic | VitalKindOf | +FS_FACTORY |
| AITarget_J05_ClosestStructureHeuristic_NoRefinery | ForbiddenKindOf | +REFINERY |
| AITarget_J05_ClosestStructureHeuristic_AddCiv | VitalKindOf | +CIVILIAN_BUILDING |
| AITarget_J05_ClosestStructureHeuristic_AddCiv | ForbiddenKindOf | -CIVILIAN_BUILDING |

The first three inherit ClosestStructureHeuristic. The last two inherit the
already resolved NoRefinery asset: its added REFINERY flag survives the later
CIVILIAN_BUILDING removal. This proves source-local modifier chaining, not native
reference graph closure or runtime AI behavior.

## Code and verification

- `SdkBitflagModifiers.Syntax/Prove` performs bounded schema/token proof.
- `SdkSelfAttributeInheritance.CheckLeaf/Resolve` gates fields, charges retention,
  invokes the unchanged core, checks its result and records witnesses.
- `SdkInstanceInheritanceProfile` propagates the independent scope child-first.
- `SdkTypedSourceGraph` and `Program` expose `--instance-bitflags` only on the
  typed graph command and reject competing options.
- `SdkInstanceBitflagsSmokeTest` is registered in the compiler runner and exposed
  as `sdk-instance-bitflags-self-test`.

The owned group covers ordered add/remove/no-op chains, explicit empty bases,
actual core substring hazards, missing fields, unknown/duplicate/base tokens,
mixed/whitespace operations, child modifiers, late atomic refusal, marker/source/
field isolation and final invalid-payload no-fields binding. Three CLI probes
reject conflicting profiles, duplicate bitflag flags and flags on path-only
preflight (exit 1, no output).

The focused group, **all 133 compiler groups**, 33 enum checks, three classifier
fixtures and three bitflag CLI probes pass. The full suite requires permission
for its existing owned temporary audio-file move. Final build has zero warnings/
errors. Registered compiler groups: 133. Model/marshaller counts stay
785/1390 and 762/1390; EP1-only coverage remains 48/48 for both.

## Remaining work

Ten first blockers remain: three Sounds directive/expression owners, three
duplicate/unsafe-ID owners, one AIMicroManager matched element-only branch,
one Upgrade occurrence, one ObjectCreationLists Include-role and one EP1
cross-QName collision. The next bounded scope is matched ObjectFilter branches
in AIMicroManagerLibrary; its five populated filters must not be treated as
schema-empty leaves or discarded. Child matching/text/cardinality semantics
need separate proof before any admission.

Manual effort stays **51% complete / 49% remaining**. A validated source library
is useful pipeline progress but does not close native dispatch/type tables,
production build integration or in-game validation. The XML ratio is not the
percentage of working mods.
