# Isolated EP1 Multisound native proof

## Outcome — October 2, 2026

The new explicit native model/marshal path reproduces three selected Uprising
Multisound assets byte-for-byte, including every relocation/import byte and the
ordered AudioEvent dependency identities. Legacy Multisound remains 16/8 bytes
(root/child); the isolated EP1 path is 16/28. No production audio plugin or
diagnostic-build audio-root admission has been enabled.

| Stock asset | BIN | RELO | IMP |
|---|---:|---:|---:|
| GDI_Generic_VoiceDieMS | 464 | 8 | 68 |
| NOD_Generic_VoiceDieMS | 576 | 8 | 84 |
| Gui_GlobalShellPanelExpand | 72 | 8 | 12 |

These are per-instance native chunks, not linked stream totals. The selected
stock manifest is `D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest`;
its provenance/hash is recorded in [the audio audit](RA3EP1_AUDIO_FINGERPRINTS.md).
Only selected BIN/RELO/IMP slices are read; no full binary dumps are generated.

## Recovered structure and semantics

Reference RA3 compiler method `0x06000039` in
`Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.XmlCompiler.dll`
places child fields at the following offsets. Its metadata gives Size=28.

| Offset | Field | Encoding |
|---|---|---|
| 0 | Base | One-biased import selector for BaseAudioEventInfo reference |
| 4 | Weight | uint32, default 1000 |
| 8 | PitchShiftLow | Optional float pointer |
| 12 | PitchShiftHigh | Optional float pointer |
| 16 | Volume | Optional percentage pointer |
| 20 | PlayPercent | Optional percentage pointer |
| 24 | VolumeShift | Optional percentage pointer |

The reference method marshals Base first, then Weight, then the five pointers in
the listed order. Missing optional values leave null pointers. An explicit zero
allocates four bytes and adds a relocation: null and zero are not interchangeable.
Percentages are multiplied by float 0.01. The root has Base at 0, control flags
at 4 and the count/pointer pair at 8/12. Official control enumeration order
LOOP/PLAY_ONE is checked against the unchanged EP1 XSD.

The stock examples exercise default/explicit weights and absent optional pointers.
A separate synthetic two-child fixture exercises all five optional pointers,
explicit zero, float pitch, negative percentage, explicit weight zero and
LOOP/PLAY_ONE together: native **92/28/12** bytes. A zero-child fixture produces
16/0/0. Optional pointer semantics currently have reference-IL plus synthetic
golden evidence, not a stock example with authored pitch/volume overrides.

## Implementation and test boundaries

- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1Multisound.cs` defines
  Marshaler.Ep1Multisound and Ep1MultisoundSubsound, plus explicit Marshal
  overloads. Existing SageBinaryData.Multisound/MultisoundSubsoundRef and their
  old overloads are unchanged. New records are intentionally outside the normal
  production type table and the 785/762 inventory counters.
- `tests/fixtures/MultisoundPipeline.xsd` includes unchanged official audio/base
  components. `MultisoundProbe.xml` contains small stock source excerpts from
  official Sounds/Voice.xml and Sounds/SoundEffects.xml, not modified originals.
- `MultisoundNativeSmokeTest.Run` checks struct sizes/field offsets, schema enum
  order, actual core XML validation/default insertion/reference normalization,
  determinism, independently constructed complete native goldens, invalid enum /
  unsigned weight / float rejection, and successful recovery after those failures.
  Caller Settings are restored. No production stream files are written.
- The optional stock pass requires exact EP1 Multisound TypeId A3A7AF37,
  TypeHash F79C5A89, Tokenized=0, all concrete target TypeIds AudioEvent 844D7B9F,
  ordered instance IDs, and full native slice equality for all three fixtures.

This proof does not prepare an external dependency table for a production
ProcessInstance entry. The schema normalizer records reference indices; selected
stock metadata independently corroborates their names, order and concrete types.
Transitive/sibling resolution, fresh prepared tables, root eligibility,
platform/cache/output policy and mixed linked-stream readback remain separate
compiler-profile gates. Do not invoke this marshal overload on unvalidated,
unnormalized user input as if it were a checked SDK build command.

## Reproduce

After building the inspector Release/x86, run from the repository root:

```powershell
# Reborn: compare fixed native sound fixtures with bounded stock slices; never register a production audio processor.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe multisound-native-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest"
```

Omit the manifest argument to run synthetic/independent native tests only.
Both Release/x86 projects build, all **78 compiler groups** and layout checks
pass, and all 33 existing enum mappings pass. Coverage inventory remains
785/1,390 models and 762/1,390 typed marshallers; these counts do not imply stock
native compatibility. This adds one evidence group, not a production audio family.

## Next gate and progress

Follow-up: [the isolated checked Multisound profile](RA3EP1_MULTISOUND_PROFILE.md)
now implements the compiler-entry/policy and prepared identity-table gate below.
Mixed linked-stream proof and bounded command admission still remain open.

The isolated profile now covers explicit EP1 metadata, prepared concrete
AudioEvent/Multisound dependencies, duplicate/ambiguous/missing reference and
current-source tamper guards, and production/cache policy closure.
Next: prove mixed linked streams before considering bounded command admission.
AudioEvent's missing base fields, AudioEventLimitGroup, AudioFileRuntime/codecs,
SDK packaging, WorldBuilder and actual Uprising loading remain open.

Overall effort remains approximately **50% complete / 50% remaining**. This
closes the first Multisound native ABI proof, not end-to-end audio building.
