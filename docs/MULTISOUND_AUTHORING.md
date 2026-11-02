# Multisound authoring

<!-- Reborn: describe the normal alpha authoring route separately from the older isolated diagnostic profile. -->
`Multisound` is supported through the ordinary `Static_Mod.xml` include,
inheritance, dependency audit, compilation and BIG/SKUDef pipeline. Use the
existing alpha `EALAModStudio.exe` to build it. No WAV payload is stored in the
Multisound itself: its `Subsound` entries import other audio events, including
local AudioEvents and other Multisounds.

The two SampleMod declarations `GDI_Titan_VoiceSelectMS` and
`GDI_Titan_VoiceSelectBattleMS` already use valid Uprising syntax. Their
`Subsound` text does not need a KW-specific conversion.

The original Uprising XSD defines:

- `Control`: empty by default; `LOOP` and `PLAY_ONE` flags. Original Uprising
  sources include `LOOP` on its own, so the compiler accepts that form too.
- Any number of ordered `Subsound` entries, including an empty list.
- Each subsound has `Weight` (unsigned integer, default 1000).
- Optional `PitchShiftLow`, `PitchShiftHigh`, `Volume`, `PlayPercent` and
  `VolumeShift`. Missing scalar overrides stay null pointers; explicit zero
  remains an allocated zero value. Percentage values use EA's percentage
  conversion.

Inherited source values use the common EA source preparation route. Repeated
references and nested Multisounds retain their own ordered native import slots.
Missing or ambiguous required sound targets still fail the normal dependency
check. This change does not enable the separate `MultisoundOverridable` type,
music/dialogue processors, or previously unsupported AudioEvent fields.

## Native evidence and validation

The production writer uses the existing recovered EP1 structure:
`Marshaler.Ep1Multisound` (16-byte root) and
`Marshaler.Ep1MultisoundSubsound` (28-byte child). The old KW/legacy 8-byte
subsound structure is unchanged and is not used here.

Developer builds verify 528 original Uprising records from
`SageXML/Sounds/SoundEffects.xml` and `Voice.xml` against bounded reads of the
original global stream. This covers stock defaults, weights and controls.
Optional scalar pointers are established by the recovered reference marshaler,
the Uprising XSD and independent authored native checks; the surveyed retail
XML does not use those optional overrides.

The durable authoring regression exercises seven emitted assets with local and
nested imports, repeated references, inheritance, all optional overrides, LOOP,
an empty list and final stream readback (22 checks). Existing native goldens
also cover exact BIN/RELO/IMP output and invalid schema inputs. Fast builds
skip retail qualification but keep source/schema/dependency and current-source
native validation. In-game listening remains a separate user test.

Implementation: `Ep1MultisoundCompiler.cs`, normal admission in
`ConscriptSourcePreparation.cs` / `ConscriptSourceInput.cs`, typed dispatch in
`ConscriptMultiAssetBuild.cs`, and the simple-content reference proof in
`SchemaNativeContract.cs`. The older experimental Multisound plugin keeps its
previous restricted contract for diagnostic compatibility.
