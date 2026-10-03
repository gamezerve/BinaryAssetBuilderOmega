# Checked isolated EP1 AudioEvent compiler profile

Date: October 3, 2026. This is an explicit experimental ProcessInstance entry,
not public diagnostic-build admission, a production SDK or playable mod.
Overall weighted effort remains approximately 50% complete / 50% remaining.

## Entry and supported shapes

`source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ra3Ep1AudioEventPlugin.cs`
implements IAssetBuilderPlugin and IAssetBuilderOutputPolicy.
Descriptor: `tests/fixtures/Ep1AudioEventProfile.xml`.
Profile name: RA3EP1-AudioEvent-Experimental-v1, version 1, Win32 only.
TypeID 844D7B9F, TypeHash 560C2E45, ProcessingHash 560C2E45 XOR 45503141,
AllTypesHash 5454A8E9; non-tokenized, no custom data, no build cache, no
compiled-document reuse or production output. Metadata objects are fresh.
AudioFile, AudioEventOverridable and other types are not registered.

The entry uses the isolated 128/152/12-byte native records, not legacy audio.
It admits official finite base scalar attributes, default or explicit SOUNDFX
slider, and these stock-proven shapes:

- Attack / Sound / Decay with default child Volume 100 and unsigned weights;
  at most 32 references total. Repeated concrete identities/aliases reject.
- PitchShift / PerFilePitchShift (RealRange), Delay / InitialDelay (IntRange),
  NonInterruptibleTime (TimeRange), at most one of each.
- Control tokens: LOOP, INTERRUPT, FADE_ON_KILL, IMMEDIATE_DECAY_ON_KILL.
  This is a shape/layout proof, not validation of every combination's gameplay semantics.

Root ID is bounded to 128 characters, normalized reference names to 256,
root XML to 65,536 characters and direct root nodes to 128.
LimitGroup, VolumeSliderMultiplier, MinRangeShift, MaxRangeShift, nondefault
child Volume, other explicit sliders and unproven control flags reject.
Inheritance, authored TypeId mismatches, unresolved formulas, custom/weak/file
metadata, nested reference content and foreign attributes reject.

## Current-state and dependency checks

The current root must retain official PSVI type AudioEvent, loaded schemas,
matching instance identity/hash and an exactly complete prepared strong table.
Every core-normalized ordinal is checked against current text, original
AudioFile identity and prepared concrete TypeID 166B084D/name/hash identity.
Missing/null, extra, changed or reordered slots cannot authorize compilation.
Only the resolved instance identity is checked here: prepared core handles do
not preserve independent external record tokenization, duplicate-record counts
or stock TypeHash as an admission proof.

Current XML is copied, checked injected TypeIds are removed in the detached
copy, and current values/order/defaults are schema-validated again. Finite
scalar/range/time checks precede marshalling; malformed current values cannot
borrow old PSVI. No caller XML modification occurs.
Expected native buffer sizes are independently bounded from reference/range
counts and allocations. The entry does not use earlier compiled bytes.

## Evidence and tests

`Ep1AudioEventProfileSmokeTest.Run` is compiler group 83. It tests:

- Descriptor/default and explicit mapping policies, fresh metadata, unsupported
  types/platforms and readiness revocation after failed reinitialization.
- Five selected stock source fixtures through actual ProcessInstance entries,
  byte equality with native compilation, repeated determinism and unchanged XML.
- Identity/hash/TypeId tampering, formulas, NaN/overflow, invalid flags,
  current changed weight/volume/range values, stale PSVI, reordered child sequence,
  foreign attributes, nested elements, processing instructions and oversized XML.
- Original/concrete table changes, wrong type/name/index, null/extra slots,
  custom/weak/file metadata and two repeated missing-target attempts followed by recovery.
- Forced fresh resident-source loading despite precompiled requests.
- Empty root, exact 32-reference positive case and 33-reference rejection;
  explicit AudioFile names, duplicate aliases and unsupported options.
- Output-policy rejection before missing input/cache access; no emitted streams.

Optional actual-game run resolves AudioFile identities from
`D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest`
alongside the global manifest, then reproduces all five global native slices:
Impact 364/20/68; Cryo loop 320/24/56; Cryo water 228/24/20; IFV repair
212/16/20; Yuriko steps 396/24/76. Ordered prepared reference tuples also match.
The native golden group independently checks these records; see
[native evidence](RA3EP1_AUDIOEVENT_NATIVE.md) for IDs, source flattening and limits.

Synthetic AudioFile manifests in the default suite are metadata-only fixtures
with stock fingerprint 53C81E47/tokenized false, not audio payloads or codec proofs.
The actual audio manifest is likewise used for resolution, not decoding.

## Reproduce and remaining gates

```powershell
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe ep1-audioevent-profile-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe ep1-audioevent-profile-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest" "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest"
```

The full compiler suite has 83 declared groups; structural model/marshaller
inventory stays 785/1,390 and 762/1,390. Both Release/x86 projects, layout and
the 33 existing enum mappings are separately checked.

Next: require unique compatible external AudioFile metadata at stream admission,
prove an owned mixed AudioEvent/Multisound/FX stream, then consider narrow public
diagnostic admission. Wider actual audio evidence, LimitGroup, AudioFile codec
generation, aggregate type tables, packaging and in-game loading remain open.
Do not treat this profile as permission to enable the production audio registry.
