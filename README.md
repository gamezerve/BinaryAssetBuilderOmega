# BinaryAssetBuilder
A RA3 Uprising (EP1) port of the Kane's Wrath-based .NET BinaryAssetBuilder.

## Uprising progress — September 30, 2026

Active branch: `feature/ra3ep1-manifest-inspector`. This is not yet a usable
Uprising Mod SDK release. Replacing XML/XSD files alone is insufficient:
native layouts, type hashes, dependencies and in-game loading must also pass validation.

Approximately **50% complete / 50% remaining**. This is an engineering-effort
estimate, not a file-coverage metric or a measure of working game mods.
It is rounded from weighted workstreams and is not increased per commit.
The containment audit and attach-base recovery accumulated enough work to
reassess the native-layout workstream from 53% to 55%; the overall estimate
moves from about 49% to 50%. Major type-table and in-game gates remain open.

Measured inventory: **784/1,390** EP1 complex types have models and
**760/1,390** have typed marshallers. The compiler test runner invokes
**47 test groups** (some contain several fixtures). These counters can grow
without making a usable SDK; they measure coverage, not game compatibility.
The coverage script also reports `CompilerTestGroupsDeclared`; it counts
registered groups but does not execute them. In this work block, model and
marshaller counts are unchanged; registered test groups increased by one.

| Workstream | Status |
|---|---|
| Manifest v7, BIG/RefPack readers and stream headers | Implemented; checked with real game inputs and structural tests |
| EP1 schema inventory and enum migration | Inventoried; all 48 EP1-only complex types have models/marshallers |
| Shared RA3/KW native layouts and processors | In progress; model presence does not establish compatibility |
| Final EP1 type table / AllTypesHash | Incomplete; a safety gate blocks production output with the wrong hash |
| SDK scripts and WorldBuilder packaging | Partial; not validated end to end |
| Loading a mod in Uprising | Not yet validated |

### Recently completed

- Corrected ScriptedModel, dependency/death masks, invisibility and tint layouts.
- Restored missing RA3 fields in OpenContain and PassengerData, removed
  KW-only fields and preserved EP1 mask expansions.
- Removed KW slot/grab/weapon-set fields from TransportContain and corrected
  GarrisonContain's InitialRoster from a pointer to an inline record.
- Added transport/garrison regression checks to the layout and compiler tests.
- Audited HordeGarrison's vector pointers and restored the missing
  ContestableGarrisonContain model, marshaller and behavior-module dispatch.
- Audited Heal/Tunnel containment tails; restored SlaughterHordeContain's
  missing FX reference and normalized percentage conversion for cash refunds.
- Corrected HordeContain's EVA asset reference, removed KW-only root fields,
  and restored the 16-byte RankInfo stride for nested position lists.
- Restored ProductionQueueHordeContain's model, marshallers and dispatch.
- Recovered AttachUpdate's masks, imports, optional pointers and EP1 flags/bone-name extension.
- Restored LeechTargetingAttachUpdate and MoneyGainAttachUpdate model/marshaller/dispatch support.
- Restored InfiltratorContain's missing masks, effects, weak ID, EVA/FX imports and optional filter-reference pointers.
- Recovered LaserState's string, particle lists and pointer fields; added EP1 RequiresWeapon and SweepingLaser angle/options.
- Restored the missing ConvergingLaserState model, marshaller and behavior dispatch.
- Verified schema-inserted infiltrator defaults through dependency-index normalization and native imports against real EP1 manifests.
- Fixed inherited refType lookup and typed weak-reference normalization; invalidated old session caches.
- Added explicit external lookup/runtime stream mapping and preserved separate normal/patch entries.
- Fixed patch-base retention and asset lookup; reject wrong-target bases and refresh changed external manifests.

Latest verification: Release/x86 build, `layout-self-test` and
`compiler-self-test` passed. The two-passenger OpenContain fixture produces
`428/8/0`, HordeTransport `364/8/0`, and Garrison `168/0/0` bytes of
`bin/relo/imp`. The new HordeGarrison fixture produces `220/16/0`; the
ContestableGarrison dispatch fixture produces `240/8/0` (including its
four-byte outer pointer slot). These are native-marshalling tests, not proof
that a mod loads in-game.

The latest containment leaf tests also pass: Slaughter `344/0/8`, Heal
`188/0/0`, and Tunnel `192/0/0`. Slaughter verifies that a refund value of
`25` is stored as `0.25` and that its FX reference creates an import entry.
HordeContain's two-rank fixture passes with `592/20/12`, checking nested
positions, weak unit IDs, leader defaults and EVA/modifier import slots.
ProductionQueueHordeContain's dispatch fixture passes with `156/16/0`,
checking two eight-byte template records and an optional filter pointer.
AttachUpdate checks cover an empty root and a populated EP1 bone-name/mask/import
fixture; its standalone EVA asset dependency remains unported.
The preceding Release/x86 rebuild and its 43 compiler test groups passed. AttachUpdate
fixtures produce `368/0/0` and `576/32/16`; Leech dispatch produces `372/8/0`
and MoneyGain dispatch `448/12/0`. The enum check now verifies 33 schema mappings.

The focused reference-pipeline test now validates infiltrator's four named
defaults: official schema default insertion, production normalization into
`name\dependency-index`, native imports and matching type/instance IDs in real
EP1 global/static manifests. The suffix is a dependency-table index, not an
asset hash or arbitrary numeric asset ID. Raw-marshalling fixtures alone do
not exercise this pipeline. The focused harness stubs unrelated asset layouts;
it does not claim a complete SDK build or game load.

Previous layout-block verification: the full Release/x86 rebuild, layout tests, all 45
compiler test groups and 33 enum mappings pass. Infiltrator emits `144/12/32`,
populated LaserState `136/24/12`, Sweeping dispatch `80/8/0` and Converging
dispatch `144/8/0` bytes of `bin/relo/imp`. The sweeping fixture also checks
EP1 angle conversion, option removal and RequiresWeapon=false. A Turkish-culture
regression caught and fixed culture-sensitive weak-ID hashing: uppercase-I
asset names now use invariant normalization. These tests still do not prove
in-game loading. Overall effort remains approximately 50% complete.

Previous reference-block verification: full Release/x86 dependency rebuild, final inspector
rebuild, layout tests, all 46 compiler groups and 33 enum mappings pass.
The schema-default infiltrator fixture produces `136/0/20`, including a real
import at dependency index zero. Repeated runs produce identical native chunks
and dependency identities; a previous-revision session cache is rejected.
Inherited reference types, attribute overrides, invalid explicit types and
typed/trimmed weak names are regression-tested. The four default targets exist
in the local Uprising global/static manifests, checked without opening BIN data.

Current verification: Release/x86 full dependency rebuild, final test-runner
rebuild, all 47 compiler groups, layout checks, writer round trip and 33 enum
mappings pass. External-link tests cover runtime path serialization, settings
round trip, normal/patch roles, retained base metadata, same-path manifest
refresh, wrong-target rejection and production lookup against real EP1 manifests.
No game BIN was opened or modified; generated fixtures contain only metadata.

External builds now require an explicit runtime-name mapping in addition to
local manifest files; otherwise production emission fails rather than silently
omitting the stream references. Physical paths are lookup inputs, not game paths.

```xml
<!-- Reborn: pair local lookup files with relative names visible to the game's stream loader. -->
<Settings ExternalManifests="Base/global.manifest;Base/static.manifest"
          ExternalManifestReferences="global.manifest;static.manifest" />
```

`/emr` supplies the same mapping on the command line. Names are paired in order,
must be relative `.manifest` paths, and cannot contain traversal or empty
segments. Reference-bearing manifests are regenerated even when asset checksums
match, preventing stale runtime names. The final EP1 type-table safety gate
remains enabled; this is not yet a validated SDK/game-load build.

### Next steps and acceptance gates

1. Audit remaining containment derivatives and other shared native layouts.
2. Complete the EP1 type table and verify `AllTypesHash=0x5454A8E9`.
3. Compare tokenized output with real Uprising asset fixtures.
4. Complete SDK source/schema/dependency paths and WorldBuilder packaging.
5. Load a minimal mod in Uprising and verify byte-identical repeat builds.

Detailed findings, file/class references and limitations:
[EP1 implementation status](docs/RA3EP1_IMPLEMENTATION_STATUS.md).
This summary and the detailed report are updated together as work progresses.
Changes are published only to the working branch in
`gamezerve/BinaryAssetBuilderOmega`; no pushes or PRs go to the Qibbi upstream.

## Legacy KW asset list (historical reference)

The following list and type IDs come from the earlier KW implementation.
They are not a Uprising compatibility checklist or a verified EP1 type table.

### Implemented Asset Types
* [ ] TestGameObject
* [ ] TestTexture
* [ ] TestTextureCollection
* [x] WeaponTemplate
* [x] LocomotorTemplate
* [ ] GameObject                                                0x132408DB
* [ ] FXParticleSystemTemplate                                  0xA148D511
* [ ] Weather                                                   0x368A8BA2
* [ ] ShadowMap                                                 0xC6389FA6
* [ ] WaterTransparency                                         0x331DA6CE
* [ ] Texture
* [ ] OnDemandTexture
* [ ] W3DMesh                                                   0xC9D7E778
* [ ] W3DContainer                                              0x909DD93F
* [ ] W3DHierarchy                                              0x3BC26A7A
* [ ] W3DAnimation                                              0xCC069193
* [ ] W3DCollisionBox                                           0xC917E725
* [ ] ArmyDefinition                                            0x57213EA5
* [ ] AIPersonalityDefinition                                   0x7DCE182F
* [ ] FXList                                                    0xEBE8A8A4
* [ ] ObjectCreationList                                        0x683D4DE5
* [ ] ObjectFilterAsset                                         0x25970AF7
* [ ] SpecialPowerTemplate                                      0x5EF0ACA9
* [ ] UpgradeTemplate                                           0x1E53F384
* [ ] SkirmishOpeningMove                                       0x21EE29FA
* [ ] AIStateDefinition                                         0x262BE85F
* [ ] AIStrategicStateDefinition                                0x1E27DA26
* [ ] AIBudgetStateDefinition                                   0xA10F9630
* [ ] AITargetingHeuristic                                      0xB7A2C222
* [ ] GameMap                                                   0x3EC9C79B
* [x] AttributeModifier
* [x] ArmorTemplate
* [ ] MissionTemplate                                           0x0D283295
* [ ] TheaterOfWarTemplate                                      0xE60C9724
* [ ] CampaignTemplate                                          0xAC60B530
* [ ] RadiusCursorLibrary                                       0xD62B490F
* [ ] AudioFile                                                 0x46410F77
* [ ] AudioEvent                                                0x1B886049
* [ ] MusicTrack                                                0x1469548A
* [ ] DialogEvent                                               0x8655CDB4
* [ ] AmbientStream                                             0xDABB1C4B
* [ ] Multisound                                                0x12B1C67C
* [ ] MusicPalette                                              0x6A7AF822
* [ ] MusicScriptConditionNugget_LocalPlayerIsObserver          0xAFB6AF3A
* [ ] MusicScriptConditionNugget_UnitsFarFromBase               0xD889BF98
* [ ] MusicScriptConditionNugget_TimeFromStartOfLevel           0xAA4A9E23
* [ ] MusicScriptConditionNugget_TrackPlayedCount               0x4FCFFAB1
* [ ] MusicScriptConditionNugget_SpecificTrackTypePlaying       0xBCAD9B77
* [ ] MusicScriptConditionNugget_AnyTrackPlaying                0x337BC326
* [ ] MusicScriptConditionNugget_ObjectsOfTypeExist             0x9586411C
* [ ] MusicScriptConditionNugget_EvaEventPlayedRecently         0x1F200F13
* [ ] MusicScriptConditionNugget_ObjectsNearEvaEvent            0x0EC4D160
* [ ] MusicScriptConditionNugget_ScoredKillCount                0x5C0F93DC
* [ ] MusicScriptConditionNugget_Not                            0xB886383B
* [ ] MusicScriptConditionNugget_Or                             0x81114695
* [ ] MusicScriptConditionNugget_And                            0x10173347
* [ ] MusicScriptTrack                                          0x702C8407
* [ ] LocalBuildListMonitor                                     0x99CC030A
* [ ] MpGameRules                                               0xEDDBB607
* [ ] ExperienceLevelTemplate                                   0xAE55047B
* [ ] MissionObjectiveList                                      0xC385A8C1
* [ ] StringHashTable                                           0x2C112832
* [ ] InGameUISettings                                          0x49FE3760
* [ ] DamageFX                                                  0x4DF81EBD
* [ ] MultiplayerSettings                                       0x1BAF4C42
* [ ] OnlineChatColors                                          0xF3645AA7
* [ ] MultiplayerColor                                          0x966F336A
* [ ] GameLODPreset                                             0x19DAC24D
* [ ] StaticGameLOD                                             0xBEAF1CC9
* [ ] DynamicGameLOD                                            0x71BAD792
* [ ] AudioLOD                                                  0x3ABBF00F
* [ ] VideoEventList                                            0x999FCBE3
* [ ] UIConfigList                                              0xB3B7607A
* [ ] PackedTextureImage                                        0x2FAEB748
* [ ] OnDemandTextureImage                                      0xF3F4AEEC
* [ ] TerrainTextureAtlas
* [ ] Mouse                                                     0x73FE99B0
* [ ] Achievement                                               0xC8D16E6D
* [ ] StanceTemplate                                            0x5C6E0E41
* [ ] TargetingCompareList                                      0x57CA5C81
* [ ] TargetingDistanceCompare                                  0xED45F096
* [ ] TargetingCombatChainCompare                               0x553808EF
* [ ] TargetingInTurretArcCompare                               0xCD24391A
* [ ] Road                                                      0xDCF3C28B
* [ ] Environment                                               0x878C42E0
* [ ] LogicCommand                                              0x97D0A46E
* [ ] LogicCommandSet                                           0x6D148BD7
* [ ] MiscAudio                                                 0xFA4817E2
* [ ] AudioSettings                                             0x89AA7DDE
* [ ] CrowdResponse                                             0x66FB33A0
* [ ] MapMetaData                                               0x59013A51
* [ ] LargeGroupAudioMap                                        0x9CBC0553
* [ ] AptAptData                                                0x36866072
* [ ] AptConstData                                              0x1CE8E595
* [ ] AptDatData                                                0x3BF7FEB9
* [ ] AptGeometryData                                           0x58F89E8B
* [ ] MappableKey                                               0xE005A668
* [ ] HotKeySlot                                                0x1AC54E60
* [ ] DefaultHotKeys                                            0x0E12479D
* [ ] InGameUIGroupSelectionCommandSlots                        0xF6CE1A68
* [ ] InGameUILookAtCommandSlots                                0x8F9F9918
* [ ] InGameUITacticalCommandSlots                              0xC24AEFF1
* [ ] InGameUIVoiceChatCommandSlots                             0x3592E352
* [ ] InGameUISideBarCommandSlots                               0xAF956455
* [ ] InGameUIPlayerPowerCommandSlots                           0x4AB425C6
* [ ] InGameUIUnitAbilityCommandSlots                           0x9DAA4182
* [ ] GameScriptList                                            0x5AC6FA18
* [ ] IntelDB                                                   0xFBB64F90
* [ ] BootupDisplaySequence                                     0x84C1C2F0
* [ ] UnitTypeIcon                                              0xF7AB74BE
* [ ] ImageSequence                                             0x217CF953
* [ ] UnitOverlayIconSettings                                   0xDFC78E66
* [ ] TheVersion                                                0xF659EF49
* [ ] DLContent                                                 0x4E1A5713
* [ ] PhaseEffect                                               0x4877D566
* [ ] ConnectionLineManager                                     0x7AEB73B2
* [ ] InGameUIFixedElementHotKeySlotMap                         0x475EA260

### Tiberium Wars Only Types
* [ ] AudioFileMP3Passthrough                                   0x610DB321
* [ ] MP3MusicTrack
* [ ] MP3DialogEvent
* [ ] MP3AmbientStream
* [ ] UnitAbilityButtonTemplateStore                            0x5A48D289
* [ ] PlayerPowerButtonTemplateStore                            0xDB57AB4F
* [ ] CommandSet                                                0x3CFF78A1

### Kane's Wrath Only Types
* [ ] UnitAbilityButtonTemplate
* [ ] PlayerPowerButtonTemplate
* [ ] StrikeForceBuildTemplate
* [ ] MetagameOperationsInfoType
* [ ] MetaGameUITacticalCommandSlots
* [ ] MetaGameUICommonOpCommandSlots
* [ ] MetaGameMapZoneData
* [ ] MetaGameStaticData
* [ ] ButtonSingleStateData
* [ ] JoypadCommandBarTemplate
* [ ] JoypadCommandBarButtonTemplate
* [ ] UIJoypadCommandBarButtonBuild
* [ ] UIJoypadCommandBarHomogenousGroup
* [ ] UIJoypadCommandBarMixedGroup
* [ ] UIJoypadCommandBarSingleUnit
* [ ] UIJoypadCommandBarStances
* [ ] UIJoypadCommandBarTopMenu
* [ ] UIJoypadCommandBarMgTopMenu
