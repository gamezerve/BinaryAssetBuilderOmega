# EP1 1.1 stock package preflight

Explicit installation examined:
`D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising`.
No installed file, archive, config or registry value is modified. No game is run.

`RA3EP1_english_1.1.SkuDef` is 455 bytes, SHA-256
`416C0752EA6891F13916D60FB1C8C3B8E5F342F7E919214866FC5D6EE962D75E`.
It selects `Data\ra3ep1_1.1.game`, sets search path `big:;.`, and names fourteen
BIG archives. The colocated 1.0 SKU has the same displayed directives except
its selected game filename; this is not proof of launcher argument forwarding.

## Installation discrepancy

The 1.1 SKU names `Data\MapsCampaign.big`, but that exact file is missing here.
The directory contains `MapsCampaign (Disabled).big` (103,915,845 bytes).
The separate SteamLibrary installation contains an original-named campaign
archive. Neither is substituted: the preflight reports the missing configured
path, rather than silently borrowing another installation or renaming a file.
Whether the disabled name is intentional must be settled before a clean
stock/game-load baseline. No checksum identity of the renamed file is claimed.

## Bounded inventory

Thirteen available configured archives contain 17,383 indexed entries,
including 639 manifest entries. Three directory passes read 3,234,099 metadata
bytes per preflight invocation, plus two 455-byte SKU snapshots. **Zero archive
payload bytes** are read. Every archive directory is bounded and rehashed;
only metadata paths, offsets, sizes and index identities are reported.

| Archive | Core manifest entry | Stored bytes | Archive offset |
| --- | --- | ---: | ---: |
| StaticStream.big | data/static.manifest | 552,437 | 163,702,976 |
| StaticStream.big | data/static_l.manifest | 525,493 | 751,380,288 |
| StaticStream.big | data/static_m.manifest | 525,120 | 754,344,832 |
| GlobalStream.big | data/global.manifest | 417,235 | 422,784 |
| WBData.big | data/worldbuilder.manifest | 2,204,287 | 1,395,734,336 |

The lowercase `_l`/`_m` entries corroborate variant-name availability, but
native archive case handling is not proven by detached filename fixtures.
Stored lengths are not assumed decompressed lengths. These fresh directory
identities do not validate payload bytes against earlier unpacked manifests.

MapsMultiplayer.big has duplicate directory names for the map_mp_2_feasel6
map, art and four stream files. They are retained as evidence; the preflight
does not guess precedence, collapse duplicates or extract ambiguous entries.

## Reproduction and boundaries

`Get-Ra3Ep11StockPackagePreflight.ps1` takes explicit 1.1 `-ImagePath`, 1.0
`-BaselineImagePath` and `-SkuDefinitionPath` under PowerShell 7. It rejects
reparse inputs through the inherited strict readers, bounds stock directives,
keeps paths within the SKU directory, rejects duplicate targets and reports
missing configured archives. The stock-only directive policy is not a general
native config interpreter. It reuses the validated BIG directory reader.

`Test-Ra3Ep11StockPackagePreflight.ps1` checks the reviewed stock SKU identity,
five unique core manifest names, byte accounting, complete nested repeat JSON
and false payload/compiler/game flags. This is a repeat/evidence-contract test,
not new native fuzzing. With thirteen archives, two invocations read 6,468,198
archive-directory metadata bytes and zero archive payload bytes.

Production readiness, authentic EP1 ProcessingHash, fresh manifest/sidecar
payload validation and game loading remain false. Engineering estimate stays
**52% / 48%**; the 165 compiler groups are not rerun. Next: bounded extraction/
decompression of uniquely indexed core manifest metadata, compared with the
existing unpacked evidence; BIG payload formats must be checked before reads.
