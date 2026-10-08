# Controlled in-memory PathMusic compiler — October 9, 2026

## Result and newly characterized integration gate

`pathmusic-controlled-compile` now processes the strict 1–8 local authored owner
closure through fresh Core XML preparation and a privately registered local music
plugin, returning detached native/checksum evidence without writing streams.
Native buffers match the immutable preparation and experimental v1 package contract.

**This is not standard Core output/dependency pipeline integration.** The first
integration test demonstrated that `AssetDeclarationDocument.AddOutputInstance`
returns immediately when `instance.Handle.TypeHash == 0`. The existing v1 music
profile deliberately has zero TypeHash. Consequently normal Core output selection
does not prepare its dependency tables or select these owners for output.

The new path characterizes and preserves this guard. It does not change Core,
substitute a nonzero placeholder TypeHash, or manufacture empty validated Core
dependency tables. `CoreOutputSelectionSkipped=true` is included in the CLI report.
Native dispatch is a separate memory-only call through the registered plugin after
private local snapshot closure admission, **not** CompileInstance, OutputManager,
GenerateOutput=true or a full SDK build. This distinction is a migration blocker,
not a completed production integration milestone.

ProcessingHash remains synthetic `52424D49`, document version 23, and both type
and catalog hashes remain zero. Actual stock EP1 processing provenance, official
AUDIO resolution, cache/reuse, production output and game loading remain unproved.

## Implementation

`source/BinaryAssetBuilder.ManifestInspector/PathMusicControlledCompiler.cs`:

- `Compile` privately reads immutable Core/native preparation and obtains frozen
  expected chunks, then creates fresh settings, SessionCache, DocumentProcessor
  and a registry mapped only to PathMusicEvent (`9A651D89`).
- Existing reviewed full-schema/local closure admission runs before the focused
  unchanged music schema harness. No official XSD or Core implementation changes.
- `ProveOutputSelectionSkipped` invokes existing AddOutputInstance with a fresh
  output set, requiring zero selected owners and absent ValidatedReferencedInstances
  and AllDependentInstances. No temporary hash substitution is used.
- Private `LocalPlugin.Admit` binds the complete ordered actual Core owner objects
  to verified captured name/ID/InstanceHash and normalized XML, plus exact header
  SHA. There is no public plugin factory or production configuration descriptor.
- `ProcessInstance` checks initialization/platform, object membership, current
  preparation, exact XML, handle/processing identity, no strong references/custom
  data/inheritance, one captured header and exact weak alternate projection.
  Standard dependency tables must remain absent, not be forged as prepared.
- Canonical captured header bytes drive a fresh existing Win32 tracker serializer;
  returned native arrays are detached. Freshness is checked before and after.
- Actual buffers must match all frozen BIN/RELO/IMP bytes. Ordered Core checksum
  must also match independent padded reconstruction and private preparation.
- Production registry validation and GenerateOutput=true are required to refuse
  under the exact local policy before source lookup. BuildCache=false, no compiled
  document reuse, and zero AllTypesHash remain mandatory.
- Settings.Current is restored in finally, including late-input/mutation failure.

There is no file output or package publication in this command. A callback exists
only for owned test faults; CLI exposes no mutation hook or guard-bypass switch.
No codecs or official mixed-mode reference DLL are executed. Bounded freshness
checks are not atomic filesystem locking or arbitrary concurrent/ABA guarantees.

## Reproduction and validation

Use the x86 Release inspector from the repository root:

```text
pathmusic-controlled-compile tests/fixtures/PathMusicAuthoredProbe
pathmusic-controlled-compiler-self-test
compiler-self-test
```

Two CLI runs of the checked-in two-event fixture return identical JSON and exit 0.
ProcessorCalls=2, checksum `CF2CABDA`, hashes `9CE92A17`/`BA9DF8C5`, native chunk
sizes 20/8/0 and 16/0/0. ProductionBuildReady, CanUseBuildCache,
CanReuseCompiledDocuments and GameLoadProved are false; CoreOutputSelectionSkipped
is true. Missing arguments return 1.

Focused tests cover 1/3/8 owners, fresh dispatch/repeatability and frozen native
checksums; foreign object refusal; detached buffers and metadata; changed XML
identity/path/cache/alternate/unknown/inheritance/child fields; changed hashes,
processing identity, custom-data/file/strong/weak projections and forged dependency
tables; wrong type/platform and failed reinitialization; late header mutation,
restoration and settings recovery; unchanged input-directory file sets.

Owned XML mutation tests restore both schema-default versus explicit attribute
provenance and self-closing element state. Adding/removing a child alone changes
`<Event/>` into `<Event></Event>` and is not exact captured XML restoration. The
plugin's strict equality check remains intact; no semantic-equality bypass is added.

All 163 compiler groups pass. Incremental build has zero warnings/errors.
Inventory remains 785/1,390 models, 762/1,390 marshallers and 48/48 EP1-only coverage.
Engineering effort remains **52% complete / 48% remaining**. This characterizes a
real integration gate and adds contained native dispatch, not a usable SDK release.

Next: establish an explicitly justified nonzero music metadata policy and authentic
EP1 processing/type-table provenance before revisiting standard Core selection and
dependency preparation. Do not silently adopt RA3 constants or alter experimental
v1's zero-hash policy. Official AUDIO dependencies and game-load proof remain
separate prerequisites for production integration.
