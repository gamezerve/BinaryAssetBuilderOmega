# Multisound admission in bounded diagnostic builds

## Outcome — October 3, 2026

`diagnostic-build` now accepts the narrow isolated Multisound profile as its
fifth native family, alongside ShaderOverride, ObjectFilterAsset, FXList and
AttributeModifier. FX can refer to a locally compiled Multisound; nested local
Multisounds are allowed only when the selected dependency graph is acyclic.
AudioEvent/AudioFile roots and encoded audio compilation remain closed.
Production SDK output, cache/reuse and game-load claims remain forbidden.

Admission retains the profile's 0–32 direct weighted Subsound leaves,
default/PLAY_ONE control, prepared concrete AudioEvent/Multisound identity checks,
and rejection of optional pitch/volume/percentage overrides, LOOP, inheritance,
custom/weak/file metadata and unsupported nesting. Empty local Multisounds need
no external mappings. Referenced external audio is still metadata-only.

The source snapshot grammar allows only direct Multisound roots and direct
Subsound leaves. Authored backslash selector suffixes and unresolved formulas in
Subsound text reject before core normalization can rewrite them. Multisounds
count toward the existing graph-wide 32-root limit and safe ASCII ID/duplicate
checks. All prior XML/Include/path/metadata/payload/publication limits remain.

## Dependency order and selected cycles

The earlier fixed family rank is no longer sufficient for nested Multisounds.
`BoundedDiagnosticBuild.OrderDiagnosticRoots` now walks actual prepared local
dependencies first, with deterministic family/ordinal-name ordering as tie-breaks.
The preferred independent-family order is shader, filter, Multisound, FX, modifier;
all four earlier families retain their relative order on existing valid inputs.
Native per-entry reference order and selectors are not changed by stream ordering.

A consumer named AConsumer referencing ZDependency therefore emits ZDependency
first. An active traversal edge rejects selected self/two-node/multi-node cycles
before native compilation/publication. This does not validate recursion hidden
inside external audio payloads. Unselected instance-Include roots remain tentative;
their missing references are not forced into output.

## Usage

`tests/fixtures/DiagnosticMultisoundProbe.xml` is a checked-in direct local chain:
Multisound → two existing AudioEvents, FX → that Multisound, modifier → that FX.
Run from the repository root after a Release/x86 inspector build. The parent
output directory must exist and the requested child directory must be new.

```powershell
# Reborn: compile a bounded local sound chain using stock metadata; the output is diagnostic, not a playable mod.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-build tests/fixtures/DiagnosticMultisoundProbe.xml Release/MySoundPoC "D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest=data/global.manifest"
```

This example was executed through the actual CLI with the local EP1 global
manifest: three entries, native **208/20/28**, linked **216/28/36** BIN/RELO/IMP
bytes and a **400-byte manifest**. It embeds source.xml, not absolute paths.
DIAGNOSTIC_ONLY.txt now explicitly notes that local Multisound is experimental
and external AudioEvent/AudioFile payloads are not rebuilt. Notices for outputs
without local Multisound remain unchanged.

Physical manifests are read-only input. Relative runtime names are explicit
serialized examples, not proven game packaging paths. No dependency payloads
are copied, compiled or validated merely because their metadata matches.

## Verification

`DiagnosticMultisoundBuildSmokeTest.Run` exercises the actual Build service:

- Five-family all/instance Include graph: shader 40, weak filter 124, local
  Multisound 72, FX 80, modifier 56 native BIN bytes. Total native **372/28/28**;
  linked **380/36/36**. Ordered concrete tuples, one-biased selectors and rewritten
  input source names are checked independently. Unused sound stays uncompiled.
- Repeated/frozen/restored outputs are byte-identical; edited fresh missing
  references fail. Live original XML/manifest changes during staged publication
  cannot replace the approved snapshots, and caller sources/settings are restored.
- Wrong selected AudioEvent TypeHash/Tokenized, duplicate external identity and
  repeated missing target failures reject; restored metadata recovers identical
  streams. An optional real-stock pass checks the five-family graph against
  observed global AudioEvent identities/fingerprints.
- Nested local sound ordering emits the dependency before its alphabetically
  earlier consumer. Self/two-node cycles must fail specifically at the local
  dependency cycle check, not incidentally as a missing reference.
- LOOP/optional controls, authored selectors/formulas/TypeIds, inheritance,
  nested shapes, duplicate/case-folded IDs and graph root excess reject.
- Existing output survives. Staged checksum corruption publishes nothing; raced
  destination owner markers survive. No staging directories remain.
- The committed CLI example builds through the service; an empty sound also
  builds without external mappings. Aggregate diagnostic-build-self-test now
  includes this integration group.

Both Release/x86 projects build. All **81 compiler groups**, layout checks and
33 enum mappings pass; previous real-stock FX command goldens still pass.
Inventory remains 785/1,390 models and 762/1,390 typed marshallers. This adds a
checked command family, not a production SDK registry or codec implementation.

```text
diagnostic-multisound-build-self-test [ep1-global-manifest ...]
diagnostic-build-self-test
compiler-self-test
```

Implementation: DiagnosticSourceGraph.Read/Visit; DiagnosticAssetPipeline.xsd;
BoundedDiagnosticBuild.Build, OrderDiagnosticRoots and Rank; existing isolated
Ra3Ep1MultisoundPlugin. Selected external uniqueness/fingerprints, metadata/source
snapshots, native readback and exclusive staged publication are reused, not bypassed.
Legacy KW models, official XML/XSD, production registry and game files are untouched.

## Remaining work

Recover and test AudioEvent's missing BaseSingleSound fields/offsets and
AudioEventLimitGroup dependencies. AudioFileRuntime/encoded audio requires a
separate investigation; MP3 passthrough is not stock EP1 codec compatibility.
Broader audio/FX options, complete EP1 registry/aggregate derivation, other native
processors, SDK/WorldBuilder packaging and actual Uprising loading remain open.
Overall engineering estimate remains approximately **50% complete / 50% remaining**;
the narrow command gate is closed, not the end-to-end SDK/game-load gate.

Related: [sound profile](RA3EP1_MULTISOUND_PROFILE.md),
[fixed mixed stream](RA3EP1_MULTISOUND_FX_STREAM.md),
[general diagnostic limits](RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
