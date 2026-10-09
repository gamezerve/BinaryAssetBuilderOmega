# User-built Reborn package comparison

Read-only inspection, October 9, 2026. The user identified the KW samples as
WrathEd-built, RA3 Reborn as EA SDK-built, and RA3 deneme as a later campaign
map override experiment. Those build origins are user-provided provenance,
not independently reconstructed compiler histories.

## Selected inputs and bounds

- `D:\OneDrive\Documents\Command & Conquer 3 Kane's Wrath\Mods\Reborn`:
  selected latest `Reborn_1.06_Streams.big`, `Meta\Reborn_1.06_Maps.big`,
  and `Reborn_1.06.skudef`.
- `D:\OneDrive\Documents\Red Alert 3\Mods\Reborn`: selected latest
  `Reborn_1.07_Streams.big`, `Reborn_1.07_Misc.big`, and `Reborn_1.07.skudef`.
- `D:\OneDrive\Documents\Red Alert 3\Mods\deneme`:
  `Deneme_1.00_Streams.big`, `Deneme_1.00_Misc.big`, and `Deneme_1.00.skudef`.

Historical versions were listed, not all audited. BIG directory inspection
was capped at 100,000 entries, 16 MiB directory size and 2,048-character names,
with entry range checks. Selected sidecar reads were at most 64 bytes each;
the existing ManifestInspector read five exact manifest entries and performed
its structural checks. No entire BIG dump/hash, extraction, payload verification,
game launch or source file mutation was performed. OneDrive reparse attributes
were observed: these explicitly user-selected reference paths were read directly,
not admitted as production output paths by the stricter control runner.

## Observed differences

| Selected stream | Manifest version | AllTypesHash | Asset count | Checksum |
| --- | --- | --- | --- | --- |
| KW Reborn `data\static_mod.manifest` | 5 | `12B3E763` | 27,262 | `9CE520B2` |
| RA3 Reborn `data\mod.manifest` | 6 | `54EEE764` | 5,967 | `0FF32AB8` |
| RA3 Reborn Yokohama `map.manifest` | 6 | `54EEE764` | 1,376 | `ADA2C397` |
| RA3 deneme `data\mod.manifest` | 6 | `54EEE764` | 37 | `6FE726A8` |
| RA3 deneme Brighton Beach `map_mod.manifest` | 6 | `54EEE764` | 1,719 | `A51B7F21` |

All five selected manifests returned empty structural ValidationErrors.
This does not verify every referenced base, native type layout, sidecar payload
or runtime behavior. The existing EP1 target is v7 with AllTypesHash `5454A8E9`;
renaming these older streams would not convert their binary format/type table.

KW Streams contains 1,871 entries, including 1,811 cdata entries and twelve
each of BIN/IMP/MANIFEST/RELO/VERSION. Its archive is 1,011,156,653 bytes, but
only the 132,896-byte directory and selected small entry prefixes were read
in the index pass. Selected `static.version`, `static_l.version` and several
UI markers contain `_mod` followed by LF (`5F6D6F640A`); some metadata markers
contain LF alone. This naming scheme is not exclusive to the RA3 map sample.

RA3 Reborn `data\mod.version`, `mod_l.version` and `mod_m.version` contain
CRLF alone (`0D0A`). Corresponding `mod.*`, `mod_l.*` and `mod_m.*` streams
exist. Therefore a nonempty custom suffix is not mandatory for every mod.
Both RA3 SKUs contain `mod-game 1.13` and mount their Streams/Misc archives;
this is observed configuration text, not a verified installed-game version.

RA3 deneme's Misc BIG contains exactly eleven entries, including the map,
three TGA files, script XML/Lua, and this stream family:

```text
data\maps\official\camp_a01_brightonbeach_smith\map.version
data\maps\official\camp_a01_brightonbeach_smith\map_mod.manifest
data\maps\official\camp_a01_brightonbeach_smith\map_mod.bin
data\maps\official\camp_a01_brightonbeach_smith\map_mod.imp
data\maps\official\camp_a01_brightonbeach_smith\map_mod.relo
```

`map.version` contains exactly six bytes, `_mod` plus CRLF (`5F6D6F640D0A`).
There is no cdata or asset entry in this Misc archive. The deneme Streams BIG
has 35 entries: seven complete stream families, no cdata or asset entries.
Neither extension is therefore a universal requirement for an override package.

## Dependencies matter independently of filenames

Both inspected RA3 map manifests reference:

- `worldbuilder.11.manifest` with `IsPatch=true`;
- `static.manifest`, `global.manifest`, `audio.manifest` with `IsPatch=false`.

RA3 Reborn and deneme main `mod.manifest` reference the three latter streams
without a patch flag. KW `static_mod.manifest` references
`static_common_2.manifest` as a patch base, plus non-patch `Global.manifest`
and `AdditionalMaps/MapMetaData_Global.manifest`.

The Brighton Beach manifest declares total instance data 107,097,608 bytes,
whereas packaged BIN length is 22,389,180 bytes. A patch-base reference is
consistent with not storing every declared asset locally; do not label that
size difference corruption or proof of correct inheritance without inspecting
the base and individual inherited entries.

## Migration consequence

The user screenshot's unsuffixed marker plus suffixed stream family is
confirmed in an actual archive. Local `BuildMapV2.bat:229–235` implements the
same renaming and marker generation. `OutputManager.CreateVersionFile()` and
`DocumentProcessor` already expose custom-postfix output behavior;
`BinaryAsset` treats `.asset` as intermediate and `.cdata` as optional custom
output. A suffix separates stream filenames; it does not rename asset IDs or
resolve dependency/type/layout differences.

Use this package as a real naming/dependency regression reference, not as an
EP1-compatible payload. Next focused work is an EP1 package preflight covering
the marker bytes, exact stream siblings, checksum/header family, correct patch
base, imported asset identities, custom-data presence and suffix-slot composition.
The EP1 static audit has found suffix insertion and `.version` probing, but
the text marker's complete runtime interpretation remains unverified. Do not
copy `worldbuilder.11.manifest` into EP1 as an assumed compatible base, rebuild
over the user's samples, or claim that suffix renaming bypasses `-modconfig`
and game-loading gates. Overall effort remains approximately **52% / 48%**.
