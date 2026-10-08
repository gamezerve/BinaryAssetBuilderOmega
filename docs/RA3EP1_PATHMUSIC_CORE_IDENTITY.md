# PathMusic actual Core identity proof — October 8, 2026

## Result and boundary

The local music profile now passes through the **actual DocumentProcessor** with
a private hash-only registration. Each actual Core InstanceHash is compared to
an independently reconstructed XML/file/weak-type calculation before reporting
success. The report exposes XML, header-file and padded dependency hash components,
normalized physical paths, cache/default provenance and actual/expected hashes.

This does **not** recover the EA PathMusic ProcessingHash. The proof deliberately
uses synthetic ProcessingHash `52424D49`, current DocumentProcessor version 23,
and zero TypeHash/AllTypesHash. `SyntheticProcessingDomain=true`,
`ReferenceProcessingHashRecovered=false`, `ProductionBuildReady=false`.
Existing diagnostic music packages retain their separate SHA-prefix identities;
this command does not relabel, rewrite or publish a package.

## Actual identity contract

Core source locations:

- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/AssetDeclarationDocument.cs`,
  `ValidateInstances`: hash default-attributed authored XML **before** physical
  FileReference/TypeId normalization, seeded by ProcessingHash and document version;
  then XOR the hash of the dependency MemoryStream's complete backing capacity.
- `HandleAssetReferenceType`: a present music weak reference writes its schema
  target **type ID** `9A651D89` into the dependency buffer, then records a typed
  weak handle. It does not write a strong selector or the target InstanceHash.
- `HandleFileReferenceType`: retain logical `events.h`, normalize the DOM to its
  lowercased absolute physical path and record the file dependency.
- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/FileHashItem.cs`,
  `UpdateHash`: seed with file size and fold reader chunks. This proof restricts
  headers below one MiB and independently reconstructs one bounded chunk.
- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/Hashing/HashProvider.cs`,
  `GetXmlHash`, and `HashingWriter`: serialized XML folded in 512-character blocks.

For this narrow one-header profile the dependency backing buffer is 256 bytes:
with no alternate, header hash at offset 0; with alternate, target type ID at
offset 0 and header hash at offset 4; remaining capacity is zero-filled. Hashing
only the used four/eight bytes would not reproduce the actual Core calculation.

## Important observations

Header edits invalidate **all** owners sharing that header, even if only one
selected constant changes. An unused header comment also changes the Core file
identity while leaving parsed event constants unchanged. A preserved timestamp
and length do not hide such edits in this proof, which always processes fresh
documents with fresh caches and rechecks raw inputs.

Explicit `IsCacheable="true"` and an omitted/default-true attribute have identical
runtime cache values but **different authored Core hashes**. The defaulted
XmlAttribute has `Specified=false` and is omitted by `XmlNode.WriteTo` during XML
hashing. Tests verify specified/serialized provenance, unchanged dependency hashes
and different XML hashes rather than assuming semantic equality implies hash
equality. Reports make this distinction visible.

Relocating identical source/header inputs to another physical directory does not
change their individual Core identities: physical path mutation occurs after XML
hashing. This is not evidence that arbitrary aliases or spelling changes are
interchangeable; the strict local authored profile still requires exact leaves.

## Implementation and scope

`source/BinaryAssetBuilder.ManifestInspector/PathMusicCoreIdentity.cs`:

- `Inspect`: first admit the existing strict local snapshot; read bounded raw
  bytes; validate a focused schema harness; run actual Core processing; independently
  fold XML/weak/header inputs; verify all handles/dependency metadata and normalized
  DOMs; recheck raw snapshots and native preparation before returning value records.
- `Build`: create a fresh SessionCache and metadata-only PluginRegistry with
  output disabled; restore global Settings in `finally`. The private plugin refuses
  ProcessInstance, production output, build-cache use and compiled-document reuse.
- `tests/fixtures/PathMusicIdentityPipeline.xsd` includes unchanged official EP1
  base/ref/music types. An unrelated transition-only MusicScriptConditionRef stub
  retains the official target annotation; no condition asset or layout is admitted.
  The focused harness is not the full production schema catalog. Existing reviewed
  full-schema local admission remains the preceding gate.

No official XML/XSD, reference DLL, Core implementation, production registry or
environment setting is edited. Reports are bounded snapshots, not atomic concurrent
filesystem guarantees, complete native audio compilation or stock hash equality.

## Commands and validation

Invoke the x86 Release inspector from the repository root:

```text
pathmusic-core-identity tests/fixtures/PathMusicAuthoredProbe
pathmusic-core-identity-self-test
compiler-self-test
```

The identity command writes JSON only; there is no output directory or package
switch. Compiler group **157** covers 1/3/8 actual owners, independent folding,
repeat/physical-directory independence, shared header edits with preserved
timestamps, unused comments, restore/recovery, XML cache changes, explicit/default
provenance, synthetic processor isolation, weak type-word presence and long-name
XML crossing the 512-character boundary.

All **157 compiler groups pass**. The checked-in two-event CLI fixture exits 0
and repeated reports agree: actual/expected Core hashes are `9CE92A17` and
`BA9DF8C5` under the synthetic domain. Missing arguments return exit 1.
Initial dependency rebuild: three existing warnings, zero errors; incremental
x86 Release build: zero warnings/errors. No native codec/reference DLL execution.

Coverage remains 785/1,390 models and 762/1,390 marshallers, EP1-only 48/48.
Last official graph remains 398 validated XML / four AUDIO issues / 198 header
occurrences. Engineering effort stays **52% complete / 48% remaining**.

Next required gate: recover/pin reference PathMusic processing metadata and
authored-to-runtime transformation semantics, or explicitly justify a separate
EP1 processing domain; bind current Core identity to immutable native preparation
without enabling production output prematurely. Authentic AUDIO input or a proved
stock-reference contract, type-table/processor admission and game loading remain
open. A matching synthetic-domain Core hash is not a matching stock EA hash.
