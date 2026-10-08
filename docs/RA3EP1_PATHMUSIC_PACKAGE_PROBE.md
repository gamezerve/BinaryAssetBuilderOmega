# Local PathMusic diagnostic package — October 8, 2026

## Result and identity boundary

The strict local authored music profile can now produce a bounded five-file
diagnostic package: `diagnostic.manifest`, `.bin`, `.relo`, `.imp` and a mandatory
`DIAGNOSTIC_ONLY.txt`. It is **not a playable Uprising mod** or a production SDK
build. No official AUDIO root/header, processor registration, cache integration,
EA processing identity, general dependency graph or game loading is admitted.

Identity policy is deliberately explicit:

- TypeId `9A651D89` is the observed music type ID.
- TypeHash and AllTypesHash are **zero**, not copied from stock `599CDAF2` and
  `5454A8E9`. This diagnostic package does not claim a compatible EA type table.
- InstanceId uses the existing SAGE name identity.
- InstanceHash and StreamChecksum are domain-separated SHA-256 prefixes over
  the captured diagnostic fingerprint (and row name for InstanceHash). They are
  **not** Core/EA processing hashes or collision-resistant authentication fields.
- Exact full-byte comparisons, not the truncated checksum, establish readback
  equality to the current frozen preparation.

## Implementation

`source/BinaryAssetBuilder.ManifestInspector/PathMusicPackageProbe.cs`:

- `Serialize`: use the existing utility AssetEntry/ManifestHeader v7 writer,
  preserve authored order, append exact detached tracker chunks and linked
  magic/checksum headers. Local alternate weak IDs have no manifest references
  or imports, consistent with the previously observed runtime layout.
- `Verify`: require the exact five-file set and bounded reparse-checked reads;
  compare every file to current frozen expected bytes; independently parse with
  ManifestReader and Utility.Manifest; check all names, identities, flags,
  aggregate sizes, linked offsets and empty reference/import domains. Manually
  decode event/cache/pointer/padding and weak target IDs; require each target to
  resolve uniquely inside the local closure and RELO `8, FFFFFFFF` per alternate.
- `Publish`: require an existing non-redirected parent and absent destination;
  validate current inputs before staging, verify exclusively created staging
  files and recheck inputs immediately before no-overwrite directory rename.
  Failed staging is retained with its path reported, never broadly deleted.

The maximum supported closure is still 1–8 owners. Each package file read is
capped at 16 KiB. No custom payload, external manifest or native codec is needed.
The existing tracker uses its normal Win32 allocation helpers.

Input and filesystem checks are bounded snapshots, **not** an atomic concurrent
transaction or an adversarial ABA/reparse-race guarantee. Publication does not
invoke OutputManager.CommitManifest, register a production plugin, or relax
existing production-output policies.

## Commands

From the repository root, invoke the x86 Release inspector with:

```text
pathmusic-diagnostic-package tests/fixtures/PathMusicAuthoredProbe <new-package-directory>
pathmusic-diagnostic-verify tests/fixtures/PathMusicAuthoredProbe <existing-package-directory>
pathmusic-package-self-test
compiler-self-test
```

The package directory parent must already exist. The checked-in fixture uses
arbitrary synthetic event values 1/2, not an authentic EP1 header. Its stream
sizes including linked headers are BIN **44**, RELO **16**, IMP **8** bytes.
Creating this output proves diagnostic framing/readback only.

## Tests and next gate

Compiler group **156** exercises 1/3/8-owner closures, independent metadata and
utility readback, local weak IDs/relocations, exact repeat output and returned
buffer detachment. Tests reject truncated/extended/flipped/missing/orphan files,
targeted type/instance/hash/native/header/relocation corruption, existing output
files/directories, and same-length timestamp-preserving header edits before any
staging is created. Refreshed inputs reject old output and produce new packages;
restoring exact inputs restores old-output verification.

The checked-in two-event fixture was also published and verified through both
CLI commands (exit 0), with a 240-byte manifest and 44/16/8-byte linked streams.
An existing output destination and missing arguments each return exit 1.
The owned demonstration output is retained under ignored `artifacts/`; reference
source/schema/Core/registry files are unchanged.
All **156 compiler groups pass**. The initial dependency rebuild reports three
existing warnings and zero errors; incremental x86 Release build has zero
warnings/errors. No reference audio DLL or native codec is executed.

Model/marshaller coverage remains 785/1,390 and 762/1,390, EP1-only 48/48.
Last expanded official graph: 398 validated XML, zero XML blockers, four AUDIO
issues and 198 header occurrences. Local package success does not resolve those
dependencies. Engineering estimate stays **52% complete / 48% remaining**.

Next gate: connect authored PathMusic processing to actual Core identity and
reference processing/hash semantics without relabeling diagnostic digests; then
establish an authentic EP1 dependency or explicit precompiled-reference contract,
production processor/type-table admission, and an independently observed game
loading test. A package with stock hashes pasted onto these bytes is not proof.
