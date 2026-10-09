# EP1 1.1 bounded installed-config inventory

## Result and exact scope

On October 9, 2026, the selected Steam installation contained **no
`filesystem.cfg` candidate** in either of these nonrecursive loose directories:

- `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising`
- The installation's `Data` directory.

There were also **no `.cfg` or `.skudef` entries** in the directories of the
13 available BIG archives selected by `RA3EP1_english_1.1.SkuDef`, covering
17,383 entries. The four small loose configs below were ASCII, read twice
with identical SHA-256 values. This is a scoped inventory, not proof of
absence throughout the filesystem, other installations or all runtime roots.

| Loose file | Bytes | SHA-256 |
| --- | ---: | --- |
| `RA3EP1_english_1.0.SkuDef` | 455 | `63BDBE83BDA6976052A56EAC96A5381756C80FE4445C81C2EAAEE8CC521DF1B3` |
| `RA3EP1_english_1.1.SkuDef` | 455 | `416C0752EA6891F13916D60FB1C8C3B8E5F342F7E919214866FC5D6EE962D75E` |
| `Data\ra3ep1_wb_1.0.cfg` | 355 | `33D4D748937E83979B81EA50DD914AFE6C7EA50D3AA512D8F42D20FD6058B0D0` |
| `Data\WorldBuilder.cfg` | 355 | `33D4D748937E83979B81EA50DD914AFE6C7EA50D3AA512D8F42D20FD6058B0D0` |

The two SKU files differ only in `set-exe`: `Data\ra3ep1_1.0.game` versus
`Data\ra3ep1_1.1.game`. Both set `big:;.` and list the same 14 archive paths.
Both WorldBuilder configs are byte-identical, set `big:;.`, and list bare
archive filenames instead of `Data\`-prefixed paths. Their first three mount
directives are Campaign, Multiplayer, Tutorial; the SKU lists Campaign,
Tutorial, Multiplayer. Do not substitute a WorldBuilder config for a game
config: relative-root resolution, consumers and resulting mount order still
need runtime or further native evidence.

Configured `Data\MapsCampaign.big` remains absent. The Disabled-named file
was neither substituted nor opened. A different installation was not used
to complete this archive set.

## What this changes for the first loading test

The [scoped startup audit](RA3EP11_STARTUP_CONFIG_ORDER.md) recovered a
`filesystem.cfg` read followed by a synthesized `set-search-path` when that
read fails. This inventory makes that fallback a relevant test hypothesis,
**not a proved runtime outcome**. Actual joined roots, current directory,
additional config candidates and later startup updates may matter.

Do not create `filesystem.cfg` in the installation merely because the name
appears in the executable. Next, establish the launcher's selected executable
and argument forwarding, then specify baseline and isolated `-modconfig`
cases and an observable config-read signal. A menu screenshot alone does
not prove consumption. No executable, codec or game was launched here.

## Reproduction and safety contract

Run `scripts/Test-Ra3Ep11ConfigInventory.ps1` with absolute `-ImagePath`,
`-BaselineImagePath` and `-SkuDefinitionPath` selecting this 1.1 profile.
`scripts/Get-Ra3Ep11ConfigInventory.ps1 -AsJson` exposes archive directory
identities and config contents/provenance without writing extracted files.

The inventory reuses stock preflight and its checked BIG-directory reader.
`Get-Ep11ConfigEntries` independently checks name termination, ASCII bytes,
entry bounds, candidate limits and ambiguous normalized config names.
`Get-Ep11ConfigText` accepts at most 64 KiB; it identifies RefPack markers
without decoding them and reports non-ASCII content as opaque. No native
parser behavior is inferred from this stricter diagnostic policy.

Available archives are checked against preflight identities, held with
read-only handles denying writers, and directory identity is checked again.
Selected config payloads, if any, are read twice under a combined 64-candidate
limit. Loose config contents are independently reread. Discovery is not an
atomic snapshot across all files/directories; repeat validation is required
before runtime experiments.

This observed run read 5,390,165 archive-directory metadata bytes and
**zero archive payload bytes**, plus 3,240 loose config bytes and 910 SKU
preflight bytes. No full `worldbuilder.bin` dump or hash was taken.
Validation includes three text classification fixtures, two directory
selection fixtures, one oversized-text refusal, six private malformed
directory refusals and repeat JSON/content pins. These are diagnostic
PowerShell tests, not extra compiler test groups.

Overall effort remains approximately **52% / 48%**. Config discovery narrows
Phase A preparation but does not establish effective overrides, valid
authored asset hashes, a playable package or successful game loading.
