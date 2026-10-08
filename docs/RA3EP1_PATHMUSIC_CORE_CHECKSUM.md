# Bounded PathMusic Core checksum contract — October 9, 2026

## Result and scope

The read-only `pathmusic-core-checksum` command now compares the existing Core
output checksum to an independent byte-layout reconstruction for the same
fresh immutable local music preparation. This specializes the earlier general
`ChecksumAudit` contract to actual synthetic-domain PathMusic InstanceHash values;
it does not newly recover an EA or EP1 algorithm or processing constant.

ProcessingHash remains synthetic `52424D49`, current document version is 23,
TypeHash is zero, and the admitted closure contains only 1–8 local owners with
no strong references. Existing diagnostic manifest/stream bytes, package
publication commands and their SHA-prefix identity policy are unchanged.

This is a prerequisite for a **future separately versioned experimental Core
manifest profile**, not that profile's implementation or production admission.
No official AUDIO dependencies, stock EP1 identities or game loading are proved.

## Exact checksum contract

Each owner contributes five little-endian 32-bit words in authored output order:

1. TypeId (`9A651D89` for the admitted music owner).
2. TypeHash (zero in this experimental identity projection).
3. InstanceId (actual SAGE name identity).
4. InstanceHash (actual independently verified synthetic-domain Core identity).
5. `ReferencedInstances.Count` (strong reference count, zero for this closure).

The unchanged Core `AssetDeclarationDocument.ComputeOutputChecksum` writes these
words through BinaryWriter into a default MemoryStream, then hashes **GetBuffer**,
not ToArray. For 1, 3 and 8 owners, used lengths are 20, 60 and 160 bytes, while
capacity is 256 bytes and the remaining tail is zero. The independent audit
constructs a fresh fixed 256-byte array with explicit little-endian offsets,
then uses the existing FastHash primitive. It is independent of Core's packing
and capacity calculation, not an independent implementation of FastHash itself.

An empty identity list returns zero. The audit refuses more than eight owners;
the fixed capacity must not be generalized to larger production documents.
The earlier general checksum audit covers growth boundaries separately.

Weak-reference target values, referenced-file paths, native chunk bytes and
strong-reference target values do not directly enter this output checksum.
Strong-reference **count** does. XML/header/weak-type inputs affect the preceding
Core InstanceHash; output checksum is not a content digest or authenticity proof.
Checksum equality alone does not establish raw snapshot equality or game compatibility.

## Implementation and safety

`source/BinaryAssetBuilder.ManifestInspector/PathMusicCoreChecksum.cs`:

- `Inspect` requires a privately admitted PathMusicCorePreparation and calls
  VerifyCurrent before and after computing evidence.
- `Identity` reconstructs identity-only declarations through public Core XML
  initialization, verifies name-derived IDs against the captured preparation,
  and supplies its verified InstanceHash. No mutable instance is taken from Core
  processing or stored in preparation.
- `Core` invokes the existing internal checksum function through reflection;
  no Core implementation or official reference file is modified.
- `Independent` writes explicit field offsets into bounded owned memory.
- The report explicitly says synthetic domain, EP1 processing hash unrecovered,
  production false and package policy unchanged.

No package is written by the command. No codec or reference DLL is executed,
no processor/cache is registered, and no registry/environment setting is edited.
Freshness checks remain bounded snapshots, not atomic filesystem locking or
protection against every concurrent input/ABA race.

## Validation and reproduction

From the repository root, use the x86 Release inspector:

```text
pathmusic-core-checksum tests/fixtures/PathMusicAuthoredProbe
pathmusic-core-checksum-self-test
compiler-self-test
```

The checked-in two-owner fixture produces matching actual/expected checksum
`CF2CABDA`: 40 used bytes, 256 capacity bytes, ProcessingHash `52424D49`,
document version 23. Two CLI runs agree; missing arguments return exit 1.

The focused test covers fresh 1/3/8 owner preparations and repeated evidence;
used-length versus padded hashing; owner reversal; InstanceHash, TypeHash and
strong-count mutations; direct weak/file exclusion; strong target replacement
with unchanged count; stale header refusal, explicit refresh and restoration;
empty and oversized-list boundaries. Mutation declarations are owned test objects,
not permission to admit strong references to the local authored music profile.

All 161 compiler groups pass. Model/marshaller inventory remains 785/1,390 and
762/1,390. Engineering estimate remains **52% complete / 48% remaining**.

Next: consume the verified identities/checksum in a separately named/versioned
experimental manifest profile, keep zero type/catalog hashes, prove two-reader
readback and cross-profile refusal, and preserve freshness/no-overwrite gates.
Authentic EP1 processing/type-table provenance, AUDIO resolution and in-game
validation remain independent open migration gates.
