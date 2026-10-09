# Version-marker and stream-family audit

October 9, 2026. `StreamVariantAudit.cs` adds a bounded read-only package check,
based on the user's actual KW and RA3 builds. No game, compiler processor or
native codec is invoked by the command.

## Result

| Reference | Marker bytes | Candidate stem | Manifest | Checksum |
| --- | --- | --- | --- | --- |
| KW Reborn 1.06 Streams | `5F6D6F640A` | `data/static_mod` | v5 / `12B3E763` | `9CE520B2` |
| RA3 Reborn 1.07 Streams | `0D0A` | `data/mod` | v6 / `54EEE764` | `0FF32AB8` |
| RA3 deneme 1.00 Misc | `5F6D6F640D0A` | `data/maps/official/camp_a01_brightonbeach_smith/map_mod` | v6 / `54EEE764` | `A51B7F21` |

All three actual packages pass: one exact marker, the complete candidate
MANIFEST/BIN/RELO/IMP family, structurally valid linked known-profile manifest
and matching sidecar checksum headers. Their asset counts are 27,262 / 5,967 /
1,719 respectively. Profile names are format/hash labels, not proof of which
game patch or compiler generated a package. These references are not EP1 mods.

One additional real negative was checked: RA3 Reborn's Yokohama Misc map has
`map.manifest/bin/relo/imp` but no matching `map.version` in that archive. This
marker-based command refuses it instead of inventing a default suffix. That is
a limitation of this command's explicit marker-required scope, **not proof
that a directly addressed stream without a version marker is invalid**.

The candidate is deliberately calculated from the **caller-supplied stem** plus
the marker's trimmed suffix. It is not a reimplementation of native suffix
slot priority, language/mode combinations or reader fallback logic.
`data/mod_l` would be a separate explicit stem. Whitespace-only or zero-length
markers produce an empty diagnostic suffix; the native engine's treatment of
every such encoding is not established by this tool.

## Bounds and policy

- BIG directory: 100,000 entries / 16 MiB. `BigArchive.Open()` now supports
  optional tighter bounds; legacy default limits are retained.
- Marker: at most 64 stored bytes; ASCII whitespace or a conservative
  underscore-prefixed alphanumeric/underscore suffix, at most 32 characters.
  NUL, non-ASCII/BOM, traversal and embedded directives are refused.
- Virtual stem: relative, no empty segments, dot/traversal, drive prefix or
  shell-like separators; slash normalization. Case/slash-colliding entries
  are refused rather than assigned guessed archive precedence.
- Selected manifest: at most 4 MiB stored and expanded. RefPack expansion is
  explicitly bounded before parsing; unknown format/hash profiles, structural
  errors and non-linked manifests are refused.
- Three sidecar reads: only four-byte v5/v6 checksum headers or eight-byte v7
  marker/checksum headers. EP1 markers are BIN `BABB0000`, RELO `BABE0000`,
  IMP `BAB10000`. Compressed sidecars are not supported by this narrow command.
  The independent stock EP1 prefix audit supports its separately reviewed
  RefPack subset; this command does not silently decode large sidecars.

The archive remains opened read-only, denying ordinary concurrent writers.
No entire BIG read/hash, asset payload dump, extraction, rename or rewrite is
performed. Explicit user-selected OneDrive reference paths may be read; this
is not production output path admission. Index stability against unusual
filesystem-provider behavior is not independently established here.

## Reproduction

After a Release/x86 build, from the repository root:

```powershell
# Reborn: detached metadata tests and one explicit reference-family audit; no game or mod installation.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe stream-variant-self-test
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe stream-variant-audit 'D:\OneDrive\Documents\Red Alert 3\Mods\deneme\Deneme_1.00_Misc.big' 'data/maps/official/camp_a01_brightonbeach_smith/map'
```

Detached tests pass: 16 positive cases and 26 refusals (markers, stems,
v5/v6/v7 headers, checksum faults, missing/colliding names, wrong magic,
short header and unsupported sidecar kind). The v7 headers are synthetic
fixtures, not another real EP1 package validation.
Release/x86 build succeeded. Workload resolution was disabled only for the
build process to avoid this sandbox's service-manager access failure; no
dependency installation or restore was performed. The full dependency build
reported 110 warnings / zero errors; the incremental inspector build reported
zero warnings/errors. These warnings were not silenced or fixed in this change.

The existing full compiler suite (165 groups, unchanged registration) was
rerun and finished `Uprising compiler self-test: OK`; the layout suite also
passed. The initial sandbox suite stopped at a fixture move permission denial;
the scoped permitted rerun succeeded. No native codec or game was invoked.
The new variant tests are an independent CLI suite, not a fabricated increase
in compiler group count. `git diff --check` passed.

## Explicitly unresolved

JSON keeps `CDataValidated`, `PatchBaseResolved`, `CompletePayloadsValidated`,
`RuntimeSuffixSelectionProven`, `GameExecuted`, `ModPackageLoaded` and
`ProductionBuildReady` false. Header checksum equality is not recomputed payload
integrity. Inherited base entries mean BIN length cannot simply be compared to
the manifest's total declared instance size. The selected base manifests and
imports still need independent resolution, and cdata needs asset-specific
identity/framing checks. Overall effort remains approximately **52% / 48%**.
