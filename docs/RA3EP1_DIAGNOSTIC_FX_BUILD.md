# FX integration into bounded diagnostic builds

## Outcome

`diagnostic-build` now admits the isolated empty/two-Sound FXList subset alongside
ShaderOverride, ObjectFilterAsset and AttributeModifier. This is the fourth
explicit experimental native family, not an expanded production EP1 registry.
Audio roots/payload compilers, particle/EVA nuggets, wider masks/options and full
SDK/game loading remain outside admission. Production output and cache/reuse
policies remain closed.

The command's snapshot grammar permits only a direct FXList, its direct NuggetList
and direct Sound leaves. The declaration schema imports unchanged official FX,
audio/weather/view components and drift-checked extracted FX module enums.
Concrete audio inheritance is available without admitting audio root declarations.
Explicit plugin mapping uses FX TypeId 86682E78 / TypeHash 17B3B82D. Ordering is
shader, weak filter, FX, modifier, then ordinal name within each family. Existing
three-family relative ordering is unchanged; admitted FX audio dependencies are
external-only, so this remains a deliberately bounded acyclic rank scheme.

All prior XML/Include/root/mapping/native-size/publication limits remain. FX roots
count toward the same 32-root graph limit and case-insensitive duplicate-ID checks.
Each nonlocal strong identity must occur in exactly one explicitly mapped manifest.
Duplicate identical audio identities across mappings reject, even though the core
lookup deduplicates candidate identities; sibling AudioEvent/Multisound ambiguity
fails with ReferencingError. Missing/unrelated target types also reject.
Selected AudioEvent/Multisound targets additionally require stock TypeHash
560C2E45/F79C5A89 respectively and Tokenized=0. Wrong hashes/flags reject;
restoration recovers identical streams. Unselected records are not gated.

Authored input now rejects compiler-injected TypeId attributes for all families
and pre-normalized Sound Value suffixes containing a backslash. A regression test
found that the older core could silently rewrite an authored `name\0` selector;
the command must reject this before normalization rather than accept hidden controls.
Literal untyped names or supported explicit AudioEvent/Multisound names remain
subject to the official schema and checked concrete dependency-table rules.

## Usage

`DiagnosticFXProbe.xml` is a checked-in four-family example. It includes the
existing shader/filter example with `all`, and stock FXListProbe.xml with
`instance`. The modifier references both sound FX roots and the shader; unused
FX_NONE stays tentative. Three external sound identities must be available.

Run from the repository root after a Release/x86 inspector build. The parent
Release directory must exist and the new output directory must not exist:

```powershell
# Reborn: emit verified diagnostic files only; explicit runtime names are not validated game packaging instructions.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-build tests/fixtures/DiagnosticFXProbe.xml Release/MyFXPoC "D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest=data/global.manifest" "D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest=data/static.manifest" "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest=data/audio.manifest"
```

The left sides are physical metadata files; right sides are explicit relative
runtime-name examples. Adjust these to the intended game stream mapping. They
are serialized, not copied, packaged or verified as a working game load path.
No audio BIN/RELO/IMP payload is rebuilt or validated by this command.

The example was executed through the actual CLI with those local Uprising inputs:
five entries, native totals 564/64/36 bytes, linked BIN/RELO/IMP sizes
**572/72/44**, and a 644-byte manifest. The warning notice accompanies the streams.
Shader/filter roots retain source input-0001.xml, local FX input-0002.xml and the
modifier source.xml. Absolute source paths are not embedded.

## Verification

`DiagnosticFXBuildSmokeTest.Run` exercises the actual public service:

- Three stock FX roots match full independent native BIN/RELO/IMP goldens and
  expected concrete audio identities/selectors.
- Explicit AudioEvent and Multisound literals compile; a wrongly typed sibling
  reference cannot widen to the unrelated concrete target.
- A nested four-family Include graph produces five entries and 504/44/28 native
  bytes, preserving sources and modifier-to-local shader/FX selectors. Unused
  tentative FX with absent audio is not forced into output.
- Repeated builds are identical. Frozen FX snapshots retain approved names after
  live edits; fresh changed inputs reject missing targets. Edits to original XML
  and external manifest files after compilation do not change staged output.
- Duplicate exact targets, ambiguous siblings, wrong/missing types, repeated
  failures, target recovery, wider options/masks/nuggets, formulas, injected
  identities/selectors, audio roots, excess/duplicate FX roots all reject.
- Existing output survives; staged corruption publishes nothing and known files
  are cleaned; a raced destination preserves its owner marker. Caller settings
  are restored, and original source files remain unchanged during ordinary builds.
- The checked-in example builds via the service. An optional stock pass compares
  command-produced slices and complete concrete reference tuples with actual
  Uprising static FX assets; all three match exactly.

Synthetic audio manifests carry observed stock hashes/flags and prove metadata
admission only; they do not establish native audio compatibility. The optional real comparison reads
manifest metadata and selected FX slices, never full binaries or audio payloads.

```text
diagnostic-fx-build-self-test [ep1-global-manifest ep1-static-manifest ep1-audio-manifest]
diagnostic-build-self-test
compiler-self-test
layout-self-test
```

Both Release/x86 projects build; all 77 compiler groups, layout tests and 33 enum
mappings pass. Inventory remains 785/1,390 models and 762/1,390 typed marshallers.
The command self-test aggregate includes this new FX integration group.

## Remaining work

This closes bounded FX command admission, not a production SDK/game-load gate.
External audio payload formats/layouts, additional FX nuggets and
other processors, complete EP1 type registration/aggregate identity, SDK scripts,
WorldBuilder integration and actual Uprising loading remain separate workstreams.
Overall effort estimate remains approximately 50% complete / 50% remaining.
Checked family counts are not an estimate of working game-mod compatibility.

Related: [isolated FX compiler](RA3EP1_FX_PROFILE.md),
[audio fingerprints and ABI gaps](RA3EP1_AUDIO_FINGERPRINTS.md),
[fixed mixed stream proof](RA3EP1_MODIFIER_FX_STREAM.md),
[general diagnostic limits](RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
