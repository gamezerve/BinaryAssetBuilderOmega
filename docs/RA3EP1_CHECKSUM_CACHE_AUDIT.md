# EP1 checksum and cache-equivalence audit

Date: October 1, 2026

## Findings

The apparently suspicious checksum padding is **required compatibility behavior**,
not a bug to fix by hashing only `MemoryStream.Length`. Four real EP1 manifests
exactly reproduce their stored checksums with the current identity packing and
default capacity padding. None matches the logical-length-only alternative.

The tokenized type-hash exception in `Utility.Manifest.IsEquivalent()` also
exists in the official RA3 Utility DLL. It is a patch-base equivalence predicate,
not the rule used when copying intermediate asset files from local/build caches.
`BinaryAsset.CopyAsset()` requires all four identity/hash fields to match.

Production checksum/equivalence semantics have **not changed**. The checksum
calculation was extracted into `AssetDeclarationDocument.ComputeOutputChecksum()`
so the exact production helper can be regression-tested. Experimental output and
compiled-document/cache restrictions remain in force. No cache version bump is
needed for this behavior-preserving extraction.

## Official RA3 evidence

Reference directory:
`D:\OneDrive\Documents\GitHub\BinaryAssetBuilderOmega2\Working RA3 Compiler for Reference\tools`

- `BinaryAssetBuilder.Core.dll`, SHA-256
  `B36D5015F97532457D64072034F9F5C629A4C060A2D34C49C4F89FD6E6F79E47`.
  Type `BinaryAssetBuilder.Core.AssetDeclarationDocument`, method token
  `0600006C` (`PrepareOutputInstances`), IL `014B–01A5`: writes TypeId,
  TypeHash, InstanceId, InstanceHash and reference count in that order.
  IL `01D6`: calls `MemoryStream.GetBuffer()`; IL `01DB`: hashes that array.
  IL `01E7–01EE`: an empty stream explicitly produces zero.
- `BinaryAssetBuilder.Utility.dll`, SHA-256
  `F71D8F4B10002637DEB72EAC68E14599E38BAC07221F0ED2D1DFC751D8FFD96B`.
  `BinaryAssetBuilder.Utility.Manifest.IsEquivalent`, token `0600012B`:
  InstanceId, InstanceHash and TypeId must match; TypeHash must match **or the
  stored base asset must be tokenized**. The candidate's tokenization flag is
  not the deciding flag.

These are targeted managed-method metadata/IL reads. The official binaries were
not modified or loaded for native execution. Current source namespace placement
differs (`Core.SageXml.AssetDeclarationDocument`); the method behavior matches.

## Real Uprising checksum reconstruction

Metadata root: `D:\TEMP\Red Alert 3 Uprising Source Data`.

| Manifest under root | Assets | Identity bytes | Capacity bytes | Stored / padded checksum | Logical-only candidate |
|---|---:|---:|---:|---|---|
| `Global Data\data\global.manifest` | 11,357 | 227,140 | 262,144 | `85FE7DA0` | `3A3BA286` |
| `Static Data\data\static.manifest` | 13,872 | 277,440 | 524,288 | `EA62EA71` | `A5FAAE2F` |
| `WorldBuilder\data\worldbuilder.manifest` | 17,339 | 346,780 | 524,288 | `F2AC0280` | `29B6267F` |
| `EnglishAudio\data\audio.manifest` | 12,951 | 259,020 | 262,144 | `395CE3A2` | `AF1583BE` |

Every record is 20 bytes: five little-endian 32-bit fields, with reference count
last. A new default MemoryStream grows to accommodate sequential writes; unused
capacity bytes remain zero and participate in the hash. Empty output returns
zero instead of hashing an empty array. Output ordering matters.

Manifest SHA-256 fingerprints, in table order:

```text
08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD
74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081
09B3DB8EF50A3E184F9CF4B84A60C6E0B4045B893815AC38137C2195034B8854
0CF62FBECC89F15746E523042A6F0422869D98FE41EB742C3104324A81D9A2E4
```

The audit reads only manifest files, rejects invalid/non-EP1 metadata and
non-`.manifest` inputs, and caps each input at 16 MiB. It never opens companion
BIN/RELO/IMP files. In particular, the 1.2+ GB WorldBuilder BIN was **not read**.
Patch streams have not been included in this four-manifest checksum proof.

Reproduce after a Release/x86 build:

```powershell
# Reborn: compare metadata-derived checksum candidates without reading game binary payloads.
$ep1AuditRoot = 'D:\TEMP\Red Alert 3 Uprising Source Data'
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe checksum-audit "$ep1AuditRoot\Global Data\data\global.manifest" "$ep1AuditRoot\Static Data\data\static.manifest" "$ep1AuditRoot\WorldBuilder\data\worldbuilder.manifest" "$ep1AuditRoot\EnglishAudio\data\audio.manifest"
# Reborn: exercise official checksum packing and distinct patch/intermediate-copy contracts.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe checksum-self-test
```

The audit prints JSON containing paths, SHA-256, asset counts, logical/capacity
lengths, stored checksum, both candidates and match flags. A mismatch is reported
as evidence, not silently treated as a successful reconstruction.

## Regression and cache distinctions

The compiler runner's 53rd group covers:

- Actual production checksum helper at 0, 1, 12, 13, 25, 26, 51 and 52 records,
  spanning the initial allocation and three capacity-growth boundaries. Packing
  is independently constructed with explicit little-endian writes; this is not
  an independent reimplementation of the hash algorithm.
- Output-order sensitivity, TypeHash/InstanceHash sensitivity and reference-count
  sensitivity. Changing reference identities while keeping their count leaves
  this helper's checksum unchanged.
- A raw and a tokenized base asset: exact tuple accepts, TypeHash mismatch accepts
  only for the tokenized base; InstanceId, InstanceHash and TypeId mismatches
  always reject. Opposite candidate tokenization flags do not change the rule.
- The real private `BinaryAsset.CopyAsset()` seam with isolated synthetic files:
  an exact four-field tuple copies byte-for-byte; changing any one of TypeId,
  TypeHash, InstanceId or InstanceHash rejects and publishes no destination.
  This tests copying, not schema compilation or network cache initialization.

`IsEquivalent()` is used by `Manifest.GetBaseStreamPosition()`. The tokenization
exception can allow matching a logically equivalent tokenized base across native
layout revisions, but its presence does not establish that a particular EP1
processor is compatible. Keep it separate from binary-cache reuse policy.

## Risks and next steps

The checksum includes identity/hash tuples and reference **counts**, not actual
compiled bytes or reference IDs. Upstream InstanceHash normally reflects other
document changes, but this audit does not prove that every dependency/content
change reaches it. A same-length corrupted linked payload with its header intact
still passes the current link reuse check. Do not use this checksum as a payload
digest or an authentication/security boundary.

At this audit's initial checkpoint, `BinaryAsset.CopyAsset()` deleted an existing
destination in its catch path and could leave its `.tmp` candidate behind. The
original rejection fixtures owned fresh paths and did not prove preservation of
previous valid output. This has since been corrected and tested with existing
destinations, custom-data failures and rollback locks; see [copy recovery proof](RA3EP1_COPY_RECOVERY.md).
No real cache or game file was exposed to the failure fixtures.

Further work: verify upstream InstanceHash/dependency invalidation, patch checksum
reconstruction, copy-failure recovery and target-specific cache namespace policy.
Production EP1 type-table approval and game loading remain unvalidated. Overall
effort estimate remains approximately 50% complete / 50% remaining.
