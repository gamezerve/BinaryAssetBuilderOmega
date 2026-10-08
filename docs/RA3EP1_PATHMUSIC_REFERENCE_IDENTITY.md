# Pinned reference PathMusic processing metadata — October 9, 2026

## Result

Read-only static analysis of the pinned repository reference audio compiler now
recovers the event processing metadata and verifies its dispatch paths. No assembly
is loaded or executed. This is evidence about that **specific RA3 reference DLL**,
not an authenticated Uprising compiler or an approved production processing domain.

| Field | Pinned reference | Observed EP1 stock |
| --- | --- | --- |
| PathMusicEvent TypeId | `9A651D89` | `9A651D89` |
| PathMusicEvent ProcessingHash | `76D0CEE6` | Not recovered |
| PathMusicEvent TypeHash | `76D0CEEF` | `599CDAF2` |
| AllTypesHash | `54EEE764` | `5454A8E9` |
| Plugin Win32 VersionNumber | `20B00004` | Not recovered |
| HasCustomData | false | Not established by this metadata review |

The reference music ProcessingHash is a direct literal in the reviewed event
metadata branch, **not** a value to derive by XORing the plugin version or by
substituting the observed EP1 TypeHash. These values do not establish which part
of the mismatch comes from game generation, SDK/schema changes, or authored versus
processed runtime type metadata. That provenance question remains open.

The previously proved tracker layout remains useful but does not imply compatible
processing identities. Do not paste either the RA3 reference hashes or EP1 stock
hashes into the local diagnostic package and claim a working Uprising SDK.
Existing package TypeHash/AllTypesHash remain zero; the existing Core identity
experiment retains its separate synthetic ProcessingHash `52424D49`.

## Reference and exact methods

Reference path:
`Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.AudioCompiler.dll`.
SHA-256:
`A14D15ADDEF502C2DF6F6BA272D6098C4C131FBEA9C3ED0149C11C4556347139`.

The reader first checks the exact whole-DLL pin, including the previous header
parser/event processor evidence, then reviews these named method bodies:

| Method | Token | RVA | IL bytes |
| --- | --- | --- | --- |
| GetPathMusicExtendedTypeInformation | `0600011F` | `14A8` | 236 |
| GetAllTypesHash | `0600011D` | `143C` | 6 |
| GetVersionNumber | `0600011E` | `1450` | 75 |
| GetExtendedTypeInformation | `06000121` | `167C` | 62 |
| ProcessInstance | `0600011C` | `687C` | 130 |

The JSON report includes a SHA-256 for every selected IL body. The event metadata
method's body hash is
`C6B70E690D65DF23D797336BA39B97D862215DBEEB7DBFD4496C9BD95C2F7037`.

Event metadata branch evidence: IL `004D` loads type ID `9A651D89`; branch
`0052` enters `008E`. IL `008F` loads ProcessingHash and `0094` stores it into
the metadata member named ProcessingHash. IL `009A` loads false for HasCustomData;
`00A1` loads TypeHash; field assignment names/tokens are checked. The native
TypeName field at token `040000FE` identifies PathMusicEvent.

GetExtendedTypeInformation's event branch enters `0036` and calls `0600011F`.
ProcessInstance's event branch enters `007A` and calls `06000119`, the previously
reviewed ProcessPathMusicEventInstance. Therefore literal proximity alone is not
the identity argument: actual branch targets, call tokens, field names and exact
whole-artifact provenance are checked.

GetVersionNumber's zero-platform branch returns `20B00004` for Win32. The
platform-one return is distinct and deliberately not selected. GetAllTypesHash
returns its literal directly. This does not replace the separate Core document
version seed used by InstanceHash hashing.

## Implementation and commands

`source/BinaryAssetBuilder.ManifestInspector/PathMusicReferenceIdentity.cs`:
`Inspect` reads at most 4 MiB with reparse guards, verifies exact artifact/body
metadata and post-checks the raw snapshot. `Verify` extracts reviewed IL literals,
validates field and dispatch witnesses and reports explicit RA3/EP1 separation.

```text
pathmusic-reference-identity "Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.AudioCompiler.dll"
pathmusic-reference-identity-self-test
compiler-self-test
```

Commands use the x86 Release inspector from the repository root. The identity
command outputs JSON only. `ReferenceProcessingHashRecovered=true` refers to the
RA3 reference; `Ep1ProcessingHashRecovered=false`, `ReferenceExecuted=false`,
`ReferenceTypeHashMatchesEp1=false`, `ProductionBuildReady=false` remain explicit.

Compiler group **158** tests the actual pinned reference, expected identity and
dispatch evidence, repeated method reports, copied-report ownership and changed,
truncated/empty in-memory reference refusal. No reference file is modified.
Repeated CLI reports agree and return exit 0; missing arguments and an unreviewed
artifact return exit 1.
All **158 compiler groups pass**. Initial dependency rebuild has three existing
warnings and zero errors; incremental x86 Release build has zero warnings/errors.
No official source/schema/Core/registry changes or reference/native execution.

Coverage remains 785/1,390 models and 762/1,390 marshallers (EP1-only 48/48).
Last official expanded graph remains 398 valid XML, four AUDIO path issues and
198 required-header occurrences. Effort remains **52% complete / 48% remaining**.

## Next gate

Bind actual current Core identity to immutable native music preparation under a
clearly declared experimental domain, checking source/header/normalized XML and
identity freshness before serialization. Independently establish EP1 processing
metadata/type-table provenance or an explicit replacement processing contract;
the recovered RA3 constants are not that contract. Authentic AUDIO sources/header
or a validated stock-reference route, production registration and in-game loading
remain required before calling this a usable SDK.
