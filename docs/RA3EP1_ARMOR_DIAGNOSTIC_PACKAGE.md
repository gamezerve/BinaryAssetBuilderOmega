# Reborn: armor linked-stream archive and identity boundary

Date: October 11, 2026. Workspace-only packaging; no installed file changed,
native code executed, game launched or mod-loading success claimed.

## Delivered

`ArmorDiagnosticPackage.Exercise` packages the actual OutputManager result
from the armor document test, after intermediate commit and linked readback.
The existing PoC runner now produces one `RebornArmorProbe.big`, checks its
complete directory and four payloads, and records its size/SHA-256 in
`readiness.json`. `DiagnosticArmorPackageValidated` is separate from the two
existing document/commit-link flags. All five test steps must pass before
any of these flags can become true.

The uncompressed BIG4 contains exactly:

```text
data\reborn_armor_probe.manifest
data\reborn_armor_probe.bin
data\reborn_armor_probe.relo
data\reborn_armor_probe.imp
```

There is no intermediate `.asset`, custom `.cdata`, stock patch base, version
marker, SKU or config in the archive. The asset remains
`ArmorTemplate:Ep1ArmorTokenProbe`; changing stream filenames does not change
asset IDs. This unused test asset does not constitute an observable gameplay
override. Mounting a BIG alone has not been proved sufficient to schedule its
manifest for loading. **Do not install or launch this diagnostic artifact.**

## Container evidence and checks

Only 16-byte headers were read from the user's EA-built RA3 deneme archive and
the installed Uprising GlobalStream archive; no full archive dump was taken.
Both show BIG4, a little-endian total-size word, and big-endian entry-count and
directory-size words. The observed total-size words equal actual file lengths:
21,388,429 and 35,465,904 bytes respectively. The new writer uses that header
contract and explicit ASCII directory names/offsets. Directory padding is
eight-byte aligned; payloads are uncompressed and contiguous. This offline
readback does not establish every engine alignment/compression requirement.

All input reads and total output are bounded to one MiB. Before output, the
packager checks v7/linked/EP1 header, checksum, single armor identity, tokenized
flag, name/source, reference absence, chunk sizes and exact compiled bytes.
Publication uses CreateNew, preserving an existing or competing file.
The existing BigArchive reader plus independent size/count/offset/name/length
checks recover the complete payloads and structurally validate the embedded
manifest. The independent archive-size check is important because BigArchive
does not enforce that header word itself.

Controls refuse an occupied output, changed linked payload before publication,
and four archive corruptions (size, count, offset and payload). Rejected input
creates no archive. Injected corruption is restored in finally, followed by
successful readback. This is a trusted offline diagnostic API, not a security
boundary against malicious concurrent replacement or compromised in-process
declarations.

## ProcessingHash is not a standalone manifest field

The earlier requirement to recover an authentic EA ProcessingHash before any
package work was stronger than the available runtime evidence supports.

| Identity | Current evidence | Consequence |
| --- | --- | --- |
| ProcessingHash | `AssetDeclarationDocument.ValidateInstances` seeds XML InstanceHash with it and document version; reuse checks compare it. `AssetStream.AssetEntry` has no separate field for it. | Project-local cache/rebuild domain; affects serialized InstanceHash indirectly. Stock equality is not a demonstrated loading prerequisite. |
| TypeHash | Serialized at asset entry +8. Reviewed 1.1 consumer at `004AB8D5` compares entry-like +8 to registered metadata +8 in the guarded path. | Preserve the characterized armor `A0E237D8`; do not emit zero or invent a replacement. |
| InstanceHash | Serialized at entry +12 and used in intermediate filenames and the output checksum. | Preserve actual current-Core identity and invalidation; neither stock equivalence nor universal runtime acceptance is proven. |
| AllTypesHash | Serialized in manifest header; characterized EP1 profile is `5454A8E9`. | Keep the EP1 target contract; RA3/KW headers are not converted by renaming. |

The runtime consumer evidence is scoped: it does not prove an exhaustive absence
of InstanceHash checks elsewhere, full loader precedence or successful loading.
Its skip branches are not permission to bypass hash validation or relabel a raw
asset tokenized. `Ra3Ep1ArmorPlugin` retains its experimental processing stamp,
and production/cache/reuse policies remain false. No EA hash was fabricated.

Relevant files: Core `SageXml/AssetDeclarationDocument.cs` (`ValidateInstances`,
`PrepareOutputInstances`, `ComputeOutputChecksum`), Utility
`AssetStream/AssetEntry.cs`, `Ra3Ep1ArmorPlugin.cs`, `ArmorDiagnosticPackage.cs`,
and [reviewed 1.1 hash consumer](RA3EP11_HASH_CONSUMER.md).

## Actual reproduction and remaining work

Successful ignored artifact: `RebornArmorPoc-c887e8efcb284f4180ffb918d2a583ec`.
All five steps passed, including both bounded stock armor comparisons. The
archive is 1,111 bytes, SHA-256
`5C7BD312BB0013E10ACA8E4880EC21917A04948DA76FC7867BCEC4043F03F20C`.
Fresh repeat `RebornArmorPoc-f206b85b11ca4af09fabb9be62e3e289` also passes
all five steps and yields the exact same archive length and SHA-256. The
standalone linker regression passes again (15 repair cases and direct guards).
The full 165-group compiler suite passed on the preceding actual-OutputManager
commit; it was not rerun for this focused archive extension. No new compiler
group is counted.
The first attempt `RebornArmorPoc-54883f83671a4391bda540ac23da3ed5` remains
failed: a deliberate corrupt-offset rejection came from BigArchive rather
than the test's expected message prefix. The control now accepts deliberate
InvalidDataException readback refusals; no archive/parser validation was relaxed.

Next select a stock armor actually used by a reachable unit/map, define a
small reversible scalar change, and prove its source/reference relationship.
Then prepare the explicit effective stream/config loading route and a bounded
runtime trial. New game/instrumentation authorization is separate. Merely
renaming this package or adding `_mod` cannot prove those steps.

`LaunchConfigurationPrepared`, `BehaviorOverridePrepared`, `GameExecuted`
and `ModLoaded` remain false. No new supported type/test group or SDK-completion
percentage is inferred from packaging this existing characterized payload.
