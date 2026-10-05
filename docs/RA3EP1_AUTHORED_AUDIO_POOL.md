# Explicit variable AudioFile pool preflight

October 4, 2026. Diagnostic-only milestone; not a production AudioFile plugin or
playable Uprising SDK. Overall estimated effort remains 50% complete / 50% remaining.

## Input and output contract

`authored-audio-pool-preflight <source-directory>` requires `audio-pool.json`:

```json
{"version":1,"sources":["tone0.xml","tone1.xml","tone2.xml"]}
```

The inventory has exactly these two unique properties. Sources are ordered, distinct
XML leaf names, with 1–8 entries; no automatic directory scan is performed. Each
source contains exactly one direct AudioFile under the EA AssetDeclaration namespace.
It uses the existing narrow profile: 250 ms mono 48 kHz PCM16 WAV input, XAS,
quality 75, RAM or streamed playback, bounded printable subtitle and literal ID.
Direct WAV dependencies may be shared or distinct. IDs are checked in the actual
case-insensitive SAGE instance-ID hash domain, not only compared as strings.

Inventory/XML must be strict UTF-8 without BOM. Limits are 4,096 inventory bytes,
8,192 bytes per XML, 24,044 bytes per WAV, 17 files and 262,144 aggregate bytes.
XML source stems allow ASCII letters, digits, underscore and hyphen, with a
100-character filename limit and lowercase `.xml` extension. WAV leaf admission
retains the existing input profile. Paths, Includes, inheritance, DTDs, unsupported
settings, reserved Windows devices (including additional extensions), duplicate
JSON fields, source aliases, and SAGE ID collisions reject before core loading.
Unlisted caller files are ignored, not incorporated into the pool.

Accepted bytes and row metadata are privately frozen. Installation writes only a
fresh owned temporary directory using exclusive creation. Exact copied bytes are
rechecked before each core load and at completion; originals are re-read and
compared before returning. Same-size/timestamp-preserving changes and inventory
reordering invalidate the snapshot. Publicly returned arrays and PCM buffers are
detached copies. Existing reparse ancestry is rejected; these checks are not an
atomic hostile-filesystem transaction or security sandbox.

Output is the owned-copy path and ordered metadata rows: source name, asset name,
SAGE ID, core hash and streamed flag. No codec is called and no manifest/bin/relo/imp,
AudioEvent graph, worker acceptance marker or production cache is published.

## Implementation

Paths below are relative to the repository root:

- `source/BinaryAssetBuilder.ManifestInspector/AuthoredAudioPool.cs`:
  `Read` validates/freezes the explicit inventory; `Definition` validates each XML
  against the trusted diagnostic schema; `Install`, `VerifyCopies` and
  `VerifyCurrent` maintain byte evidence; `Preflight` runs each real core preparation.
- `AudioFileIdentitySmokeTest.Build(..., fileName)` supplies the actual isolated
  DocumentProcessor identity path with output/cache disabled.
- `AudioFileCorePreparation.Prepare` and `VerifyCurrent` bind current core instances
  to their frozen XML/WAV dependencies. Diagnostic hashes do not establish EA cache parity.
- `AuthoredAudioPoolSmokeTest.Run` exercises the new contract;
  `CompilerSmokeTest.TestAuthoredAudioPool` registers it in the default suite.
- `Program.cs` exposes `authored-audio-pool-preflight` and
  `authored-audio-pool-self-test`. Neither changes the fixed native-worker protocol.

## Executed evidence

Release/x86 build and all 102 registered compiler groups passed. The coverage script
independently counts 102 declarations; it does not itself run tests. All 33 enum
checks passed. Structural coverage is unchanged: 785/1,390 models and 762/1,390
typed marshallers.

Fixtures exercised 1, 3 and 8 sources with distinct WAV leaves, plus 8 sources sharing
one WAV. Order, deduplicated dependency accounting, core identities, nonzero fixture
hashes, immutable snapshots, refusal to overwrite installed copies and absence of
binary output passed. Negative tests cover malformed/oversized/invalid-UTF8/BOM
JSON and XML, aliases, devices, path traversal, inheritance/Includes/DTDs, unsupported
compression, bad IDs, empty/oversized WAVs, changed originals and changed owned copies.
Restoring exact original bytes restores validation.

A direct CLI run accepted an eight-source fixture. Owned output:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioPool-502ae1ef68754e3d9bc4471eb5b45608`.

| Source | Asset | SAGE ID | Core hash | Streamed |
| --- | --- | --- | --- | --- |
| tone0.xml | PoolAsset_0 | 104A36F0 | 45620EED | false |
| tone1.xml | PoolAsset_1 | 3006E7CC | AC5EA7D7 | true |
| tone2.xml | PoolAsset_2 | D158A267 | 998D646A | false |
| tone3.xml | PoolAsset_3 | AD22EF0E | 5801930D | true |
| tone4.xml | PoolAsset_4 | 003225C7 | 3014018D | false |
| tone5.xml | PoolAsset_5 | B8DB843F | 896A3868 | true |
| tone6.xml | PoolAsset_6 | A6C8A614 | A9D00ACD | false |
| tone7.xml | PoolAsset_7 | CC7C368C | 4566867F | true |

These hashes describe this owned diagnostic fixture, not portable EA production
hash goldens. Native encoding was not executed for the variable pool in this milestone.

## Subsequent milestone and risks

[Supervised variable raw encoding](RA3EP1_AUDIO_POOL_WORKER.md) now binds the pool
to explicit managed/native child modes and independently checks each raw leaf.
The historical preflight evidence above remains managed-only. Dynamic package
publication is now covered by the separate [variable package milestone](RA3EP1_AUDIO_POOL_PACKAGE.md).
Selected event references are still pending.

Keep the fixed supervised encoder separate. Next generalize dynamic package
record/reference/import tables and test missing/reordered/duplicate/tampered results
before publication. Only then attach selected event references to the variable pool.

Broader WAV durations/codecs, music graphs, production processor/hash/cache parity,
remaining type/layout work, WorldBuilder integration and actual Uprising game loading
remain open. Source/XSD substitution alone still does not establish compatibility.
