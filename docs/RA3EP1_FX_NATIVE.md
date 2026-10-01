# Native EP1 FX root and optional-mask recovery

## Outcome

The existing FXList root is already 28 bytes, but FXNugget incorrectly stored
four inline ModelConditionBitFlags, making its base 268 bytes. Official RA3
compiler metadata/IL and actual Uprising chunks require four optional pointers
and a 44-byte base. FXList.cs now uses pointers; the existing overload marshals
each supplied EP1 mask into a separate 60-byte allocation. Absent masks stay null.
Derived SoundFXNugget is therefore 48 bytes, not 272 bytes.

The Weather fallback was also `WEATHER`, an invalid enum token. It now uses the
official `INVALID` (6), including schema-free native calls. Legacy Plugin revision
advances from 3 to 4, updating mapped processing stamps and session plugin version
so old native intermediates are not accepted under the corrected layout.
The legacy KW type registry and aggregate remain unchanged; this is not EP1
production registration. Existing isolated profile domains remain unchanged.

## Evidence

Official reference assembly:
`Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.XmlCompiler.dll`,
SHA-256 `D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6`.
FXNugget metadata size is 44. `<Module>` method `060000D0` / RVA `822D4`
marshals masks at 4/8/12/16 using pointer overload `060003BB`; Weather is at 20,
optional filters at 24/28 and disabled-type list at 32. This RA3 evidence is
confirmed against EP1 native output rather than assumed unchanged everywhere.

Supplied official Uprising source:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\GlobalData\FXList.xml`,
SHA-256 `82BF07E82A47E01164D1EB0EA37DADFBCA2D854423DC95BEFE15CAA296CFE4F7`.
FXListProbe.xml preserves three selected literal definitions; the original is
read-only and was not altered.

Observed EP1 FXList identity: TypeId `86682E78`, TypeHash `17B3B82D`, Tokenized=0.
The commented KW entry's hash `EBE8A8A4` must not be enabled as an EP1 processor.

| Selected literal | BIN/RELO/IMP | Evidence |
|---|---|---|
| FX_NONE | 28/0/0 | Empty root, no pointers/dependencies |
| FX_DebrisHitGround | 80/12/8 | One Sound, audio selector 1 |
| FX_ALL_AntiGroundAircraft_VoiceDie | 252/24/12 | Two Sounds, required/excluded FLYING pointers, selectors 1/2 |

All three match exact selected ranges in
`D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest`
and its BIN/RELO/IMP siblings. No full binary dump was made.

## Layout and native proof

FXList: BaseAssetType at 0, CullTracking/min/max at 4/8/12, polymorphic list
count/pointer at 16/20, PlayEvenIfShrouded/Tailorable at 24/25.
FXNugget: TypeId at 0, four mask pointers at 4/8/12/16, Weather 20, optional
filter pointers 24/28, disabled list 32/36, booleans 40/41/42. Sound Value is at 44.

For one Sound, the root's list pointer is 28, pointer-array entry is 32, Sound
starts at 32 and its audio import selector is at 76. RELO is 20/28/sentinel;
IMP is 76/sentinel. Weather is 6 at absolute 52.
For two Sounds, the pointer array starts at 28, nuggets at 36 and 144, masks at
84 and 192. Native selectors at 80/188 are one/two; RELO is
20/28/32/48/160/sentinel and IMP is 80/188/sentinel.

Synthetic tests exercise all four independent USER_1..USER_4 masks: total native
320 bytes, allocations at 80/140/200/260, exact enum bits and relocation entries.
An explicit empty optional mask still allocates sixty zero bytes (140-byte total),
unlike an absent pointer. Invalid weather rejects through the official schema.
These extra tests are structural coverage, not additional stock goldens.

FXListPipeline.xsd imports official FX/reference/filter/mask/weather/view types.
Only FXTriggerType and FXActionType enum declarations are extracted verbatim
from official Modules/BaseModules.xsd to avoid unrelated W3D/invisibility module
trees. The test compares their values and order against the official file.
There are no invented native asset-layout stubs in this fixture.

## Derived audio gate follow-up

The native-only proof below retains its original scope. A separate
[core external audio resolution proof](RA3EP1_FX_AUDIO_RESOLUTION.md) now
selects concrete AudioEvent/Multisound identities against actual Uprising manifests,
preserving normalized selectors and rejecting missing/ambiguous metadata.
That audio proof alone does not enable a validated FX ProcessInstance entry.
A subsequent [isolated FX profile](RA3EP1_FX_PROFILE.md) now provides this entry,
and [bounded diagnostic command admission](RA3EP1_DIAGNOSTIC_FX_BUILD.md) now
passes. Production registration remains closed.

## Original native-only proof boundary

The proof processes real schema/default/reference stages with no FX plugin, then
marshals the normalized declarations directly. It does not call a validated FX
ProcessInstance entry, production output/dependency preparation, or package audio.

Official AudioEventInfoRef declares BaseAudioEventInfo (`4053C714`). Stock strong
dependencies use concrete AudioEvent (`844D7B9F`) and Multisound (`A3A7AF37`).
The native test verifies ordered instance-name hashes and exact one-biased import
selectors, and records those observed concrete types. It does **not** claim that
the core's derived-type resolution has selected them or that raw base-type
references can be substituted for stock strong dependency identities.

Thus the native-only proof does not register FXList. Derived audio lookup and
the isolated profile now have separate proofs linked above. FXList remains
excluded from the production registry. A separate
[fixed modifier/FX stream proof](RA3EP1_MODIFIER_FX_STREAM.md) now passes;
bounded command admission and publication tests also now pass. Native byte
similarity alone was not used to admit FXList to the diagnostic command.
ParticleSystem and other nugget variants, optional filters, inheritance/custom
processing, FX cache reuse, complete EP1 aggregate/type table, SDK/WorldBuilder
packaging and actual game loading still need separate validation.

## Files and verification

- SageBinaryData/FXList.cs: optional mask fields and corrected derived sizes.
- Marshaler.FXList.cs: official weather fallback; pointer overload selection is automatic.
- XmlCompiler/Plugin.cs: shared legacy revision 4.
- FXListNativeSmokeTest: Run/Compile/Compare, all-mask/default/schema tests and
  official extracted-enum drift checks.
- UprisingLayoutSmokeTest: 28/44/48 sizes and recovered base offsets.
- FXListPipeline.xsd / FXListProbe.xml: focused official declaration proof.

```text
fx-native-self-test [ep1-static-manifest ...]
compiler-self-test
layout-self-test
```

Both Release/x86 builds, all 73 compiler groups, layout tests and 33 enum mappings
pass. Inventory remains 785/1,390 models and 762/1,390 typed marshallers. Overall
engineering estimate remains approximately 50% complete / 50% remaining; an
additional native proof is not equivalent to playable SDK readiness.
