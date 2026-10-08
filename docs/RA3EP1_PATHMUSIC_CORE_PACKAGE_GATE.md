# Current-Core PathMusic diagnostic publication gate — October 9, 2026

## Result and identity policy

Explicit Core-bound music package commands now require immutable current Core/native
preparation **before staging** and **immediately before no-overwrite publication**.
Verification brackets the existing two-reader package checks with current actual
Core checks. Old and new diagnostic commands remain distinct.

The package bytes and hash policy deliberately stay unchanged: TypeHash and
AllTypesHash are zero, InstanceHash/checksum are domain-separated diagnostic SHA
prefixes. Actual Core InstanceHash under synthetic ProcessingHash `52424D49` is a
freshness gate, **not silently substituted into manifest identity fields**. This
is not stock processing/type-table compatibility or a playable Uprising mod.

Because output bytes intentionally match the older profile, verification does
not attest which command originally published a package. It establishes current
Core/native consistency plus exact diagnostic readback, not historical provenance
or cryptographic publication authentication. A matching older diagnostic package
can pass the stronger current-Core verifier with valid current inputs.

## Implementation

`source/BinaryAssetBuilder.ManifestInspector/PathMusicCorePreparation.cs`:

- `PublishDiagnosticPackage`: pass the object's mandatory VerifyCurrent callback
  to the existing diagnostic publisher. Callers cannot replace this Core gate.
- `VerifyDiagnosticPackage`: VerifyCurrent before/after the existing independent
  manifest/utility/native/weak-target/full-byte readback.

`source/BinaryAssetBuilder.ManifestInspector/PathMusicPackageProbe.cs`, `Publish`:

- Existing parent/absent-output/reparse checks remain intact.
- Optional current gate runs before serialization or staging. Old callers without
  this gate retain their existing strict snapshot-only diagnostic policy.
- Staging files use exclusive creation and existing independent readback.
- An internal owned-test callback can inject a failure after staged verification;
  no CLI callback or bypass switch is exposed.
- Raw current-input and mandatory Core checks run after that callback and before
  Directory.Move, which does not overwrite a competing destination.
- Failures retain only their owned staging directory and report its exact path.
  No broad cleanup or existing-output replacement is introduced.

The checks are bounded snapshots, not atomic input/output locking or adversarial
ABA/reparse/staging-write race guarantees. Changes immediately after a final check
cannot be ruled out by this design. No production OutputManager commit, cache,
processor registration, native codec or reference DLL execution is enabled.

## Commands and example

Use the x86 Release inspector from the repository root:

```text
pathmusic-core-diagnostic-package tests/fixtures/PathMusicAuthoredProbe <new-package-directory>
pathmusic-core-diagnostic-verify tests/fixtures/PathMusicAuthoredProbe <existing-package-directory>
pathmusic-core-package-self-test
compiler-self-test
```

The package parent must exist and destination must not. The older
`pathmusic-diagnostic-package/verify` commands are unchanged in semantics and
serialized identities; their profile does not gain an implicit Core gate.

The checked-in two-event synthetic fixture was published and verified through
the new commands. It retains the same five files and 240-byte manifest,
44/16/8-byte linked BIN/RELO/IMP streams. Demonstration output is retained under
ignored `artifacts/`. This is diagnostic evidence, not authentic AUDIO input.

## Validation and remaining work

Compiler group **160** covers 1/3/8 closures, exact older-profile/repeated bytes,
fresh Core publication/readback, stale same-length timestamp-preserving edits
before staging, refreshed preparation/old-output refusal, late header edits after
staging, competing output preservation, binary corruption and retained staging.
Six failure staging directories are expected in the focused fixture: one late
input failure and one destination race per owner count. They are not approved
outputs and are retained for inspection.
Focused tests and all **160 compiler groups pass**. New package/verify and older
verify CLI commands each return exit 0; existing output and missing arguments
return exit 1. Initial dependency rebuild: three existing warnings, zero errors;
incremental x86 Release build: zero warnings/errors. No codec/reference execution.

Coverage remains 785/1,390 models and 762/1,390 marshallers (EP1-only 48/48).
Last official source graph remains 398 validated XML / four AUDIO issues / 198
header occurrences. Engineering estimate stays **52% complete / 48% remaining**.
No official source/schema/Core/registry files are changed.

Next bounded gate: a separately named/versioned experimental manifest profile
that explicitly carries the synthetic-domain actual Core InstanceHash, with the
correct identity checksum contract and independent readback. Do not silently
reinterpret the existing diagnostic policy. EP1 type-table/processing provenance,
authentic AUDIO or a validated stock-reference route, production registration and
observed in-game loading remain separate required gates.
