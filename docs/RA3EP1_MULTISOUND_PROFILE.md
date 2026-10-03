# Isolated checked Multisound compiler profile

## Outcome — October 3, 2026

`Ra3Ep1MultisoundPlugin` now provides a checked ProcessInstance entry for the
stock-proven weighted/default Multisound subset. It uses the separate 16/28-byte
EP1 root/child model, not the unchanged legacy 16/8-byte sound ABI.

The profile is explicitly experimental: production output, build cache and
compiled-document reuse are forbidden. Public `diagnostic-build` still does not
admit Multisound roots. This is not an audio SDK release or a playable mod.

Checked compiler buffers and prepared concrete reference tuples match all three
selected stock roots exactly:

| Asset | Native BIN/RELO/IMP |
|---|---|
| GDI_Generic_VoiceDieMS | 464/8/68 |
| NOD_Generic_VoiceDieMS | 576/8/84 |
| Gui_GlobalShellPanelExpand | 72/8/12 |

The optional stock test reads the local EP1 global manifest and only selected
native slices from adjacent BIN/RELO/IMP streams. Input provenance is recorded
in [the audio audit](RA3EP1_AUDIO_FINGERPRINTS.md). No full binaries are dumped.

## Admission and policy

- Profile name: `RA3EP1-Multisound-Experimental-v1`; version 1, Win32 only.
- Fresh type metadata: TypeId A3A7AF37, TypeHash F79C5A89,
  ProcessingHash `F79C5A89 xor 45503141`, Tokenized=false, custom data/cache=false.
  AllTypesHash 5454A8E9 is the profile's observed target identity, not proof of a
  recovered complete production type registry.
- Official schema-bound Multisound root, current matching ID (1–128 characters),
  no inheritance/custom/weak/file metadata, and a complete prepared strong table.
- Zero to 32 direct Subsound leaves, plain normalized reference text and optional
  unsigned Weight (official default 1000). Only default/PLAY_ONE root control is
  admitted. LOOP and all five optional pitch/percentage attributes stay closed
  here despite separate synthetic native tests: stock optional-value evidence is
  still missing.
- Each normalized selector must equal its actual ordinal and match the original
  dependency slot and concrete AudioEvent/Multisound identity. Untyped references
  resolve through BaseAudioEventInfo; explicit sibling types cannot widen.
  Duplicate concrete children and unused/extra slots reject.
- Current scalar values are validated in a detached XML copy before marshalling;
  corrupted injected TypeIds, foreign attributes, formulas, nested children,
  processing instructions, bad selectors or stale reference names reject.
  The caller's normalized XML remains unchanged.

The profile consumes the core-prepared identity table; that table does not retain
external manifest TypeHash/Tokenized or count identical identities across mapped
streams. Cross-stream uniqueness and selected external native fingerprints are
separate stream-admission responsibilities (already present for FX sounds in the
bounded command). Synthetic targets prove resolution only, not audio payload ABI.
Acceptance of a typed external Multisound identity does not validate its payload,
playback, recursion/cycle behavior or game support.

## Verification

`Ep1MultisoundProfileSmokeTest.Run` covers:

- XML descriptor and explicit type mapping, fresh metadata and policy rejection
  before missing input/cache access, even when a caller asks to enable reuse/cache.
- Three actual compiler entries versus native buffers and optional stock tuples.
- Unprepared/stale/extra/null/wrong concrete and original table slots; missing
  targets reject repeatedly and restored targets recover identical output.
- Untyped sibling ambiguity rejects with ReferencingError; explicit AudioEvent
  and Multisound references select the exact sibling; a wrong explicit type fails.
- Two original type names resolving to the same concrete target reject as duplicate.
- Current XML ID, weight, control, reference text and injected identity tampering;
  a valid weight edit produces new bytes rather than earlier compiled output.
- Fresh changed source identities, poisoned resident XML reload despite caller
  UsePrecompiled=true, empty roots, successful 32-child boundary and 33rd rejection.
- Unsupported platform initialization revokes readiness; valid reinitialization
  restores deterministic output. No production BIN files are emitted and caller
  Settings are restored.

Both Release/x86 projects build; all **79 compiler test groups**, layout checks
and 33 existing enum mappings pass. Inventory remains 785/1,390 models and
762/1,390 typed marshallers; isolated native types are outside the normal inventory.

## Reproduce

After a Release/x86 inspector build, from the repository root:

```powershell
# Reborn: exercise the checked profile and bounded stock comparisons; never generate production audio streams.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe ep1-multisound-profile-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest"
```

Without the manifest argument, fixed native/dependency/policy fixtures still run.
`tests/fixtures/Ep1MultisoundProfile.xml` selects the isolated plugin explicitly;
it is not a replacement production SDK configuration.

Implementation: `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ra3Ep1MultisoundPlugin.cs`
(Initialize, GetExtendedTypeInformation, ProcessInstance, CheckImport),
`source/BinaryAssetBuilder.ManifestInspector/Ep1MultisoundProfileSmokeTest.cs`,
and the existing `Marshaler.Ep1Multisound.cs` native path. Production registry,
legacy models/marshal overloads, official XML/XSD and game files are unchanged.

## Next gate

Prove a fixed mixed FX/local-Multisound stream with external stock AudioEvent
targets: actual local closure, concrete selectors, external uniqueness/fingerprints,
full manifest/utility readback and failure/recovery guards. Only afterward consider
bounded command admission. AudioEvent/AudioFile native recovery, encoded audio
processing, broader processors, full EP1 registry, SDK/WorldBuilder packaging and
actual Uprising loading remain open.

Overall effort remains approximately **50% complete / 50% remaining**. This
closes the isolated Multisound compiler-entry gate, not end-to-end audio building.

Related: [native sound proof](RA3EP1_MULTISOUND_NATIVE.md),
[FX profile](RA3EP1_FX_PROFILE.md), [bounded FX command](RA3EP1_DIAGNOSTIC_FX_BUILD.md).
