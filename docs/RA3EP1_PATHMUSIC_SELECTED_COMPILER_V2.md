# Local PathMusic Core selection profile v2 — October 9, 2026

## Result and strict scope

The explicit `pathmusic-selected-compile-v2` command now enters the existing Core
`AddOutputInstance` stage under **declared nonzero local type metadata**, producing
real Core-selected owner membership and actual validated dependency tables before
contained memory-only native plugin dispatch.

This removes the zero-TypeHash selection limitation **only for the new local v2
profile**. The older controlled v1 and experimental package v1 keep zero TypeHash
and their prior semantics. No Core guard is changed, no prepared handle is
temporarily assigned a sentinel to pass selection, and no dependency table is
fabricated. GenerateOutput=true, OutputManager/CompileInstance, production streams,
cache/reuse and game loading remain closed or untested.

## Declared type identity, not recovered EA metadata

Local TypeHash is `48E303B8`, the first little-endian word of SHA-256 over this exact
UTF-8 contract text (LF after both lines):

```text
Reborn-PathMusic-LocalType-v2
Win32;root=16;base@0=0;event@4=canonical-nonzero-int32;weak-pointer@8=16-or-null;cache-byte@12;zero-padding@13..15;alternate=uint32-local-SAGE-ID;relo=8,FFFFFFFF;imports=none
```

Full digest: `B803E34872A95F8D2616BB029B69460C80AFE3667BE8C7C8DDFB4E1469FFB2C1`.
This is an explicit versioned local native-contract identity recipe, **not EA's
TypeHash algorithm, an official schema fingerprint, cryptographic authenticity or
stock namespace uniqueness proof**. It differs from observed EP1 `599CDAF2` and
RA3 reference `76D0CEEF`; neither stock constant is silently substituted.

The private plugin supplies this metadata before fresh Core instance creation.
ProcessingHash remains synthetic `52424D49`, document version 23 and AllTypesHash
zero. EP1 ProcessingHash remains unrecovered. No official AUDIO dependency or
full stock source closure is admitted. TypeHash is exposed in the JSON report,
with SyntheticTypeDomain=true and LocalProfileVersion=2.

## Implementation and dependency semantics

`source/BinaryAssetBuilder.ManifestInspector/PathMusicControlledCompiler.cs`:

- `CompileSelected` explicitly selects private local v2 metadata; callers cannot
  supply an arbitrary TypeHash/ProcessingHash or a fabricated preflight report.
- Fresh Core XML preparation follows the same strict full reviewed-schema/local
  immutable admission as v1. Only 1–8 local PathMusicEvent owners and one bounded
  canonical header are admitted; Include/inheritance/external AUDIO stay closed.
- `PrepareSelected` creates only the same initially empty output-selection set
  used by the existing Core stage, then calls unchanged AddOutputInstance for all
  authored seeds. Core itself creates the dependency tables and validates files.
- Exact owner object membership/count, declared metadata and non-null empty
  ValidatedReferencedInstances/AllDependentInstances are required. No strong
  imports or tentative/external owners are admitted.
- Weak self references and cycles stay weak: all local owners are explicit seeds,
  no strong dependency/import is introduced. Private admission also enforces the
  exact captured weak target, because normal Core weak resolution alone does not
  establish the stricter exact local closure contract.
- ErrorLevel=1 requires real missing-file/strong-reference failure, rather than
  accepting Core warning fallback. Failed selection never reaches plugin admission.
- LocalPlugin.Validate requires prepared tables for v2 and absent tables for v1,
  plus the existing exact identity/XML/header/platform/freshness checks.
- Ordered checksum is compared with independent expected identities carrying
  the declared local metadata. Native bytes and synthetic InstanceHash remain
  equal to v1; checksum differs because TypeHash is one checksum-table word.

Reports include CoreOutputSelectionSkipped=false, CoreDependenciesPrepared=true,
ProductionBuildReady=false, cache/reuse false and GameLoadProved=false. The shared
v1 report gains additive explicit type/profile/dependency fields with zero TypeHash,
profile version 1 and selection skipped; package formats are unchanged.

This is real Core dependency-stage integration plus private in-memory plugin
dispatch, **not a standard output compilation or SDK build**. Test-only callbacks
can inject owned faults but cannot override mandatory gates. Settings are restored
on success and failure. No codecs/reference DLLs, registry/environment mutations
or game stream output are introduced. Snapshot checks are not atomic/ABA guarantees.

## Validation and reproduction

Use the x86 Release inspector from the repository root:

```text
pathmusic-selected-compile-v2 tests/fixtures/PathMusicAuthoredProbe
pathmusic-selected-compiler-v2-self-test
compiler-self-test
```

The two-event checked-in fixture reports TypeHash `48E303B8`, checksum `117BE305`,
two processor calls, InstanceHashes `9CE92A17`/`BA9DF8C5`, native sizes 20/8/0 and
16/0/0. Two CLI reports agree exactly and exit 0; missing arguments return 1.

Focused tests pin the hash recipe; cover 1/3/8 original metadata assignment before
selection and actual tables afterward; weak self/cycles; v1 native/hash invariance
and checksum separation; repeatability and unchanged input file set; missing files,
strong/weak targets, skipped zero-hash owners, lost prepared tables and stale inputs;
restoration, settings recovery and production/cache/reuse guards. Old controlled,
experimental package and all other regressions remain enabled.

All 164 compiler groups pass; incremental build has zero warnings/errors.
Inventory is unchanged: 785/1,390 models, 762/1,390 marshallers, 48/48 EP1-only.
Engineering estimate remains **52% complete / 48% remaining**. Local stage
integration does not close the authentic EP1 metadata/AUDIO/game compatibility gates.

Next: carry actual selected/dependency-prepared v2 owner identities and native
buffers into a separately versioned experimental package with independent
readback and unchanged no-overwrite/freshness guards. Keep v1 bytes and stock
identity recovery separate; production integration still requires authentic EP1
processing/type-table provenance, official dependencies and game-load validation.
