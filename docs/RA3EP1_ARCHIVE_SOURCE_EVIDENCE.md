# EP1 archive source provenance inventory — October 9, 2026

## Outcome

Read-only directory inventory of the **14 top-level BIG archives** under
`D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data`
examined **17,497 entry records**. Two passes read **2,174,196 directory bytes**,
with **zero payload bytes** read. No entries matched `RA3EPMus.h`,
`Pathfinder`, `PathMusic`, `RA3EPMus` or `audioassets`. No `.h/.hpp`, compiler
`.dll/.exe/.pdb`, or standalone `.mpf/.mus` entries were found in these directories.

This narrows the authoring-input search: these selected shipping archives do not
advertise the missing header/source XML/compiler artifacts as separate named
entries. It does **not** prove their absence inside payloads, other installation
directories, nested archives, another language/version, or the rest of the machine.
No authentic EP1 ProcessingHash or missing header was recovered. Stock BIN word4
values must not be promoted into a fabricated authoring header.

The candidate filter returned 25 records: 19 library XMLs, script events XML/XSD,
and four shader `terraintracks` names. The shader matches are filename false
positives from the deliberately broad `track` filter, not Pathfinder evidence.
None of these candidate payloads was read or decoded.

## Reproducible tool

`scripts/Get-Ra3Ep1ArchiveSourceEvidence.ps1` accepts explicit absolute archive
paths only. It opens files read-only, rejects reparse ancestors, checks BIG4/BIGF
magic, caps directories at 16 MiB and 100,000 records, caps ASCII entry names at
4,096 bytes and candidate output at 10,000 records. It validates each entry's
directory/payload boundary without reading its data. It reports extension counts,
all matching candidate offsets/sizes and case-insensitive separator-normalized
duplicate names. Duplicate entries remain visible; the tool never chooses a
winner or extracts an ambiguous entry.

It reopens each selected archive, compares total length and the SHA-256 of the
entire raw directory before returning. The reported hash is **directory-only**,
not an archive/payload integrity hash. The total-size field at offset 4 is not
interpreted; bounds use the actual file length. This is a bounded observation,
not an atomic filesystem transaction or protection against adversarial ABA/path
replacement. No payload/source search or authentic identity claim is enabled.

```powershell
# Reborn: explicitly enumerate only the selected installation's top-level BIG files.
$archiveFiles = Get-ChildItem -LiteralPath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data' -Filter '*.big' -File
./scripts/Get-Ra3Ep1ArchiveSourceEvidence.ps1 -ArchivePaths $archiveFiles.FullName -AsJson
# Reborn: exercise owned metadata fixtures without reading game data or invoking codecs.
./scripts/Test-Ra3Ep1ArchiveSourceEvidence.ps1
```

The tests cover BIG4/BIGF, exact header-name candidates, repeat hashes, JSON,
duplicate-name reporting, runtime-container/non-source separation, relative path
rejection, and ten malformed cases: truncated header, bad magic, excessive count,
large/small directory, offset inside directory, oversized payload range, empty
name, missing name terminator and non-ASCII name. Tiny owned fixtures are retained
under a unique temporary directory; their exact path is printed. These are script
tests, **not a new compiler test group**; the existing compiler count stays 165.

## Observed directories

| Archive | Records | Directory bytes | Directory SHA-256 |
| --- | ---: | ---: | --- |
| Apt.big | 2,244 | 104,890 | `48216A6B38016C7395FC40437540395A6294FE4F18284A76DF3915186C4A17F4` |
| English.big | 2 | 87 | `B11C9717230A2CF7D0E2109F6B22DE907CD1C1644EAD1343BED50B6182502DF1` |
| EnglishAudio.big | 12,955 | 867,839 | `0BA9E62312380FB243F1E50F854BB760E74F4147F5F89808996581AE33C75627` |
| EnglishMovieAudio.big | 299 | 9,980 | `D09F9299A74EF51EEE615BEA152F1C2FEBF95E4C44A66608651F996CC9F98A63` |
| GlobalStream.big | 13 | 568 | `2F9FA4BA429D8D581BA0E0E569F5C495034C80D3722BD00305D320C8A9B09382` |
| Libraries.big | 37 | 2,311 | `6751AF204DB5DE4308FA070A1573B0C32D75D14A17D3CC649BAB003ED114B62C` |
| MapsCampaign.big | 114 | 9,065 | `88C4024B55731CABFD7340F7D682A010F2E08075916DBA24412E27017E99AB8A` |
| MapsMultiplayer.big | 369 | 23,247 | `9324DF37A0F03386C63DA120C9C634C18CED5D3DCC00E0B11257CF11F53879B6` |
| MapsTutorial.big | 88 | 6,500 | `14B69C1FED0DC6E881D0C3E2D5F9C956F2AA6037C2A42828EEEB1EFE85F8EEF0` |
| Misc.big | 230 | 10,187 | `A076727B26F4404661AE8CC42A89A7171DDFC860932E473278A1971A770843B7` |
| Shaders.big | 199 | 9,090 | `EC0D0ECFF1634010ACC9CC0DE737B1B0DED2B7DCDD1BB4ABA8D0BBB4CAB4EAEA` |
| StaticStream.big | 113 | 7,214 | `FCBE0A431F150C91AF78EFB8181239D3D047417F8C393EA3BCA4F8B72829B1E4` |
| Terrain.big | 822 | 35,666 | `5B5DC8CBECB0F5FC3BA7B9F0070DA923B3B720F198D4E89C88605F032197D8D1` |
| WBData.big | 12 | 454 | `841DA4428555BBEB9613742C69023B632219325505DB2944ECFC790159BAD0D8` |

`MapsMultiplayer.big` has six normalized duplicate names beneath
`data/maps/official/map_mp_2_feasel6/`: `map_mp_2_feasel6_art.tga`,
`map_mp_2_feasel6.map`, `map.bin`, `map.imp`, `map.manifest`, `map.relo`.
The other thirteen selected archives have none. This affects any future exact
archive-entry lookup: ambiguity needs explicit handling, not a first-match rule.
No existing extraction/lookup policy was changed for this audit.

## Migration implications and next gate

The shipping archive index is not a source SDK. Source/XML substitution still
cannot supply the authentic music header, EP1 processor metadata or complete
AUDIO closure. The private selected music package v2 remains isolated and cannot
be made production-compatible by substituting stock type hashes or guessing
ProcessingHash. `worldbuilder.bin` remains a raw compiled stream, not a PE;
imports/exports must target the actual tool/game binaries instead.

Next useful work is a bounded static review of the actual EP1 executable's
Pathfinder/toolchain references or authentic external authoring materials, and
an independently recoverable production type-table workstream. No further
synthetic music version is warranted by this negative inventory. Existing
header and four AUDIO path issues remain open; no root alias, registry setting,
official XML/XSD, Core guard, native codec or game data was modified.

Overall effort estimate remains **52% complete / 48% remaining**. No game load
or usable SDK release is claimed. No compiler rebuild/full-suite rerun is needed
for these standalone read-only scripts; their own tests passed and all fourteen
real archive directory checks completed successfully.
