# Complete selected Conscript client-behavior native graphs

## Outcome

All **six complete original client-behavior leaves / three families** now match
stock native graphs through one actual Core reference table and one explicit
EP1 Tracker allocation. The source contains four direct leaves; the other two
are explicitly captured from BaseInfantry. None is discarded or reduced.

| Source module | Native BIN / RELO / IMP |
| --- | --- |
| ModuleTag_ShockDeathOverLand (inherited source) | 144 / 12 / 8 |
| Frozen (inherited source selector) | 476 / 92 / 20 |
| ModuleTag_YurikoVoiceDieFall | 144 / 12 / 8 |
| ModuleTag_YurikoVoiceUp | 144 / 12 / 8 |
| ModuleTag_ASCB (complete animation sound list) | 3,084 / 296 / 164 |
| ModuleTag_MolotatovCocktail | 188 / 20 / 8 |

The independent typed verifier visits **380 fields/records**, checks all native
bytes/padding/pointed records/list members, exact relocation/import ownership and
all **48 strong sound occurrences** against stock type/instance identities.
Every shared Core occurrence is consumed exactly once; repeated builds match all
three buffers. Empty Sound attributes retain allocated reference pointers with
zero reference values, not strong imports or silently removed voice entries.

The complete 7,404-byte stock ClientBehaviors metadata region has **296 native
target bindings**, all anchored in the independently compared record/header map.
This is metadata/native binding evidence, **not generated metadata equality**:
no stock tables are copied into compiler output and no client tokenizer is
claimed by this checkpoint.

## Actual KW-to-EP1 defect found and isolated

The legacy voice/sound enums lack EP1 entries. An old enum can therefore reject
valid XML or compile an existing name to the wrong ordinal:

| Name | Original KW ordinal | Official EP1 ordinal |
| --- | ---: | ---: |
| voiceMove | 4 | 5 |
| voiceAttack | 14 | 17 |
| soundAmbient | 4 | 6 |

New `Ep1ThingTemplateVoiceType` (**67 values**) and `Ep1ThingTemplateSoundType`
(**39 values**) preserve the exact reviewed official schema order. Separate EP1
voice/sound entry, array and selector records keep the original native ABI but
do not alter the existing KW enums or legacy root/client audio route. The
object-specific voice enum still matches all six official names/order and remains
on its original type.

`Marshaler.MarshalEp1ClientBehaviors` is an explicit selected path. It allocates
all modules on one Tracker, enforces client-local hash-distinct IDs/count bounds,
uses the original loop/animation records and the separate EP1 selector graph.
The separate selector CLR name never becomes a guessed runtime class ID: the
caller writes the stock **327CFB76** wire ID explicitly. Loop/animation wire IDs
remain **5A3EDBF4 / 27EF791D**. Known legacy dispatch cases are unchanged.

The legacy dispatcher's unreachable throw after `break` was also corrected:
unknown client classes now fail explicitly rather than silently disappearing.
Both legacy and selected unknown-class routes have refusal controls.

## Authored and corruption controls

Four full-container source variants change all six module IDs, optional
MaxUpdateRangeCap (absent / 17.5) and the first animation's ordered Frame list
(`1` / `1 7 23`). Literal native hashes, cap presence/value and frame counts/
contents are checked without patching compiled source output.

A fifth source variant places **all 67 voice and 39 sound categories** into
ordered arrays on one selector, plus VoicePriorityOverride=17. Every native enum
word and ABI size is checked, as are the original KW enum anchors. The 39 sound
category controls are authored/schema-grounded evidence; the selected original
stock client graphs do not contain a populated AudioArraySound. This does not
prove audio playback or every other optional audio-entry family.

**36 native and 7 source/dispatch refusals** pass: each leaf's class, identity,
root/list word, tail byte and missing relocation/import streams; missing sound
targets, an empty required Frame list, invalid voice category, case-folded module
ID collision, ambiguous typed targets and both unknown dispatch routes.

## Source/reference preparation and limits

Read-only sources:

- `BaseObjects/BaseObject.xml`: explicitly checked to have no client leaves.
- `BaseObjects/BaseInfantry.xml`: its two complete client leaves are retained.
- `Soviet/Units/SovietAntiInfantryInfantry.xml`: all four direct leaves retained.

The known `BaseObject -> BaseInfantry -> Conscript` source chain and six-leaf
inventory are checked. All three captured source files remain SHA-256 unchanged
throughout each test. This explicit source selection is **not execution of a
general inheritance/include engine** and does not prove closure of the full unit.

Nonempty plain Sound names resolve uniquely to schema-derived concrete audio
asset identities in validated static/global catalogs before actual Core
normalization. No captured reference indices are authored. Weak animation IDs
remain native hashes and are checked against stock as part of complete graph
comparison; this does not newly compile W3X animation assets.

The standalone diagnostic `ClientsProbe` envelope uses the unchanged reviewed
official ClientBehaviorModules type and a re-bound reference-normalization seam.
It is not a production schema-set factory or general source processor. The
selected compiler is not wired into the legacy whole-root allocator or SDK alpha.

## Files and next gate

- `source/SageBinaryData/SageBinaryData/Includes/Ep1ObjectSound.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1ClientBehaviors.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.ModulesClientBehaviorModuleData.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ClientBehaviorsNativeSmokeTest.cs`

CLI: `ep1-client-behaviors-native-self-test <official-xml-root> <static.manifest> <global.manifest>`.
The Focused batch includes this group only with explicit `-UprisingXmlRoot`.

Next: generate independently verified recursive metadata for all three client
families, then join ClientUpdates/ClientBehaviors with the selected root/Draws/
Body/behavior/AI graph. The original remaining Conscript behavior/root owners,
complete object metadata/dependencies, mixed Core stream and new-ID gameplay
acceptance remain open. No new GameObject/package, alpha expansion, audio
encoding/playback, game launch or installation change is claimed here.

## Regression evidence

Final Focused regression with explicit official XML root: **63/63** groups in
**210.082 seconds**; client group **7.387 seconds**. The batch's initial build
passes in 1.24 seconds with zero warnings/errors; the final changed Inspector
build also has zero warnings/errors. Earlier dependency/XmlCompiler rebuilds
report their existing warning totals (111 and two audio nullable-context
warnings), with zero errors. All eight runtime hashes remain unchanged during
the final batch. No Full-profile run is claimed.

Local evidence: `artifacts/ConscriptClientBehaviorsNative-a/batch-report.json`,
`compiler-build.log` and `ep1-client-behaviors-native.log`. The report records
`GameExecuted=false`, `InstalledFilesModified=false` and
`AlphaCapabilitiesExpanded=false`.
