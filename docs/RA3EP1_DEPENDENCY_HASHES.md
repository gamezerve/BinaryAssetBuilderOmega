# Dependency identity and file-hash invalidation

Date: October 1, 2026

## Outcome

Fixed a real file-cache invalidation gap: `FileHashItem` previously checked only
last-write time. A timestamp-preserving edit that changed file size could retain
the old source/dependency hash after a reset. File signatures now include length.
Deletion also invalidates the previous successful signature, so restoration with
an old timestamp does not retain the missing-file zero hash.

Full document fixtures distinguish runtime asset-ID dependencies from embedded
file-content dependencies. A target content change changes the target's identity
hash, not necessarily its parent's; changing reference IDs or a referenced file's
content changes the parent hash. No production profile was enabled or fabricated.

## Code and compatibility

- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/FileHashItem.cs`:
  `IsDirty` samples length and timestamp once per reset/build. `UpdateHash` stores
  both after successful hashing. Missing files return zero and invalidate the old
  successful signature. `Reset` clears the current sample, retaining the previous
  signature for comparison. `WriteXml` appends length to the existing record;
  `ReadXml` accepts old records but marks their unknown length for one fresh hash.
- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/Session/SessionCache.cs`:
  session format versions increase from 16/18 to 17/19 (`VERSION5`/other). The
  initial checkpoint used **19** and rejected version-18 sessions. The later
  [watcher/cache block](RA3EP1_WATCHER_CACHE.md) advances the active EP1 version
  to **20** and forces watcher-reported same-signature edits to rehash.
  The [atomic monitor handoff](RA3EP1_MONITOR_BATCH_HANDOFF.md) subsequently
  advances the active version to **21** to reject potentially stale sessions.
  The VERSION5 branch is updated but not separately runtime-tested here.
- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/AssetDeclarationDocument.cs`:
  `HandleFileReferenceType` reports `ErrorCode.FileNotFound` when no file hash item
  exists rather than dereferencing absent metadata. `ValidateInstances` and
  `UpdateDocumentHashes` retain their original identity construction.

Valid content hashing, asset type hashes, processor revisions, document checksum
padding, cache filename identities and native output bytes are unchanged. The
session version changes because dependency detection/serialized file signatures
changed; `DocumentProcessor.Version` is unchanged.

## Identity construction observed

`ValidateInstances()` hashes the schema-defaulted/merged XML with a seed derived
from ProcessingHash and DocumentProcessor.Version. Reference normalization then
records declared reference type IDs, assigns strong-reference dependency indices
and strips weak-reference prefixes where allowed by the schema. The reference
type-ID stream and referenced-file content hashes are mixed into InstanceHash.

`UpdateDocumentHashes()` separately records referenced-file path/content hashes
in DependentFileHash. `ReInitialize()` compares document content, dependent-file
hashes, type hashes, processor hashes and prior output availability. Strong asset
references use runtime TypeId/InstanceId imports, so the parent need not embed the
target's content hash just because the target's XML changes.

This explanation is limited to these inspected core paths. It does not prove that
every plugin consumes only runtime IDs or that all embedded-data processors have
correct dependency declarations.

## Reproduce and measured checks

```powershell
# Reborn: create small source/hash fixtures without production manifests or native output.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe dependency-hash-self-test
```

The 55th compiler test group, `DependencyHashSmokeTest.Run()`, checks:

- Unchanged FileHashItem reset cycles remain clean and retain the content hash.
- Modern serialization round-trips the length signature; legacy five-field
  records force a fresh hash without changing valid content-hash values.
- Timestamp-preserving file-size change is dirty and changes the content hash.
- Deletion produces zero; restoration with the old timestamp and length rehashes.
- A same-size edit with a changed timestamp is dirty and changes the hash.
- Repeated fresh document builds have identical hashes.
- Included target content changes the target hash while parent hash stays stable.
- Changing a strong or weak reference ID changes the parent hash.
- Referenced file content changes parent InstanceHash and document DependentFileHash
  while leaving the target hash unchanged.
- Changing an unreferenced file does not affect document identities.
- Missing FileReference reports FileNotFound. No production manifest appears.

The document fixture includes official EP1 primitive, asset-base and reference
schemas, plus an explicitly synthetic `DependencyProbe` root. The existing null
plugin leaves its TypeHash at zero; it has no marshaller or approved compiler.
Each comparison uses a fresh session and `GenerateOutput=false`, with an `all`
include for the target. A `reference` include would request a child stream build
in the current pipeline and was deliberately not used to bypass production gates.

Changing reference IDs in this fixture tests hashing/normalization, **not** target
resolution: output generation and native imports are not executed, and the changed
ID need not resolve. Compiled-session reuse and real plugin byte output are also
outside this focused full-document comparison. FileHashItem's real retained-cache
behavior is exercised separately with explicit `Reset` cycles.

## Remaining limits

- An edit preserving **both size and timestamp** can still evade metadata-only
  invalidation if it is not reported. The later watcher/cache block now forces
  reported changes to rehash, but content is not rehashed unconditionally.
- Hash items remain memoized within one build until reset. Mid-build source mutation
  is not a supported reproducibility guarantee or tested race scenario.
- This does not prove cached inheritance/defines, all inclusion modes, every plugin's
  file declarations, network-cache equivalence or transitive compiled-session reuse.
- Patch-stream checksum reconstruction and full type-table/processor approval remain
  open. Experimental profiles continue to deny production and compiled-cache reuse.

Overall effort remains approximately 50% complete / 50% remaining; model/marshaller
coverage is unchanged. Next: evaluate watcher-driven forced rehash and compiled
document invalidation, then expand target-specific build proof without relaxing gates.
