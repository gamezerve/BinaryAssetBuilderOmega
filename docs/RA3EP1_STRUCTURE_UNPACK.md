# EP1 StructureUnpackUpdate

EP1 derives `StructureUnpackUpdateModuleData` from
`GenericUnpackUpdateModuleData`, not directly from `UpdateModuleData` as the
legacy KW model did. The isolated EP1 structure reuses the existing correct
GenericUnpack native record, preserving all inherited fields.

| Offset | Native storage |
| --- | --- |
| 0 | Module type ID `0x977F8391` |
| 4 | Authored local module ID hash |
| 8 | UnpackTime, Time, default `0s` |
| 12 | Optional pointer to UnpackCompleteSound import word |
| 16 | OffsetHeightAboveWater, SageReal, default `5.0` |
| 20 | Audio import selector, present only when sound is authored |

The header is 20 bytes (24 bytes including a present sound word). The complete
204-byte metadata prefix includes Structure/Generic/Update/Contain/Behavior/
Module tables. A separate literal metadata writer checks generated tags and
targets, with both sound-present and sound-omitted forms. Its entire layout and
the nondefault 20-second stock delay are qualified against
`GameObject:SovietRefinery` before complete-object composition is accepted.

The sound is an `AudioEventInfoRef` constrained to `BaseAudioEventInfo`.
The source/catalog determine concrete asset identity independently of the
one-based Core selector. A present sound owns exactly one pointer relocation
and one import; an absent sound owns neither. Timing, water-height offset,
sound identity/presence and multiple local instances are authored values, not
requirements to match the stock refinery.

Implementation: `Ep1StructureUnpack.cs`, `Marshaler.Ep1StructureUnpack.cs` and
`StructureUnpackContract.cs`. The legacy KW structure and generic/unit models
remain separate and unchanged. Supporting this structure does not claim that
all GenericUnpack/UnitUnpack source families are enabled.

The `unpack` module-graph batch combines edited time/height and retargeted sound,
omitted/default fields and two independently named modules with mixed sound
presence. All output is isolated under `alpha/.work`; no Reborn source or
published package is changed. Offline validation is not a gameplay test.

## Verification — 2026-10-23

- Fresh toolchain build: succeeded, two warnings and no errors.
- Three complete-object native/metadata/stream/BIG cases passed, including
  retargeted sound, omitted sound/defaults and mixed-presence multiple modules.
- 37 native and 82 metadata corruptions were rejected.
- The unchanged original Conscript stream/BIG regression passed.
- Evidence: `alpha/.work/unpack-20261023-a/module-graph-report.json` and
  `alpha/.work/unpack-original-20261023-a`.
- Read-only Reborn preparation now reaches
  `SovietRefinery.xml(733,5): ProductionUpdate`. Its legacy record also differs
  from EP1: the KW-only `DoorOpeningTimeMetaUnit` shifts fields, while the
  schema's `NewObjectFadeInTime` and `GiveUpgradeToProduced` are absent in that
  record. Production support needs its own EP1 implementation, not just an
  admission-list change.

Tests did not launch a game or rebuild/publish the user's Reborn package.
