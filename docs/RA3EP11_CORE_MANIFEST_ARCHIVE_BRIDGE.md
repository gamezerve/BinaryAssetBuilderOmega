# EP1 1.1 fresh core manifest archive bridge

This read-only follow-up closes the narrower stock-directory/unpacked-manifest
identity gap from [stock preflight](RA3EP11_STOCK_PACKAGE_PREFLIGHT.md).
It uses the configured 1.1 installation's uniquely indexed core manifests.
No files are extracted to disk, no game/compiler/codec is run and no instance
BIN/RELO/IMP payload is read. The missing campaign archive remains reported.

Four fresh manifest payloads match the complete SHA-256 of existing pinned
unpacked evidence, not merely type-name/hash summaries:

| Archive / entry | Stored bytes | Expanded bytes | Asset entries | Encoding |
| --- | ---: | ---: | ---: | --- |
| GlobalStream.big / data/global.manifest | 417,235 | 1,402,185 | 11,357 | RefPack |
| StaticStream.big / data/static.manifest | 552,437 | 1,811,695 | 13,872 | RefPack |
| WBData.big / data/worldbuilder.manifest | 2,204,287 | 2,204,287 | 17,339 | Raw |
| EnglishAudio.big / data/audio.manifest | 285,954 | 990,541 | 12,951 | RefPack |

All four retain prefixed little-endian linked version 7 and all-types identity
`5454A8E9`. The existing descriptor projection now has a fresh archive bridge
for all 55,519 entries. This validates those selected manifests' captured
identity, not every stock file, sidecar contents or production compiler hash.
The `_l`/`_m` variant manifests are not included in this four-manifest bridge.

## Bounds and integrity

`Get-Ra3Ep11CoreManifestArchiveBridge.ps1` requires explicit 1.1 image, 1.0
baseline and stock SKU paths. It revalidates index SHA/length before each
selected read, rejects missing/ambiguous entry names, bounds stored manifests
to 4 MiB and expanded output to 32 MiB, and compares two complete stored reads.
It uses the existing `RefPack.cs` source pinned to SHA-256
`2CFA7E92E6C56BA8319F9110FFAD0AA2C61C3345A65DD40EE24A24433695A5E4`,
compiled as managed code with a small byte-array adapter. No native decoder
or imported EA routine is used. Source changes require a new review.

One invocation reads 6,919,826 selected archive-manifest bytes (two passes)
and 4,986,249 archive-directory metadata bytes including the stock preflight.
Adjacent BIN/RELO/IMP bytes remain zero; the 1.39 GB worldbuilder instance
stream is not opened or dumped. Earlier report flags describing no fresh
archive extraction remain historical; this follow-up explicitly validates
selected payloads in memory rather than changing their original scope.

`Test-Ra3Ep11CoreManifestArchiveBridge.ps1` requires all four expanded SHA
matches, three compressed/one raw selection, linked projections, complete
nested repeat JSON, and three malformed detached RefPack rejections (size
mismatch, truncated literals, oversized declared output). Two invocations
read 13,839,652 selected manifest bytes, not a full archive dump.

Production readiness, authentic EP1 ProcessingHash and game loading remain
false. Engineering estimate stays **52% / 48%**; 165 compiler groups are not
rerun. Next: twelve uniquely indexed sidecar header/length comparisons against
these fresh manifest chunk totals, with strict compressed-header handling.
