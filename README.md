# BinaryAssetBuilder
A RA3 Uprising (EP1) port of the Kane's Wrath-based .NET BinaryAssetBuilder.

## Uprising progress — October 3, 2026

Active branch: `feature/ra3ep1-manifest-inspector`. This is not yet a usable
Uprising Mod SDK release. Replacing XML/XSD files alone is insufficient:
native layouts, type hashes, dependencies and in-game loading must also pass validation.

Approximately **50% complete / 50% remaining**. This is an engineering-effort
estimate, not a file-coverage metric or a measure of working game mods.
It is rounded from weighted workstreams and is not increased per commit.
The containment audit and attach-base recovery accumulated enough work to
reassess the native-layout workstream from 53% to 55%; the overall estimate
moves from about 49% to 50%. Major type-table and in-game gates remain open.

Measured inventory: **785/1,390** EP1 complex types have models and
**762/1,390** have typed marshallers. The compiler test runner invokes
**87 test groups** (some contain several fixtures). These counters can grow
without making a usable SDK; they measure coverage, not game compatibility.
The coverage script also reports `CompilerTestGroupsDeclared`; it counts
registered groups but does not execute them. The read-only custom audio framing audit
adds one group without changing model/marshaller counts; all 87 groups were executed.

Latest milestone: [Custom audio framing and four rejected records](docs/RA3EP1_AUDIO_CUSTOM_FRAMING.md).
12,947 identity-mapped EnglishAudio custom files pass bounded RAM/streamed block
lengths, declared sample totals and EOF checks (268,674 blocks). Four files reject;
the corpus audit deliberately reports INCOMPLETE and exits 1. Original archive
comparison is required before treating this as a complete framing proof. No
compressed payload decoding, encoder activation or production admission is enabled.

[AudioFile runtime envelope and streamed boundary](docs/RA3EP1_AUDIOFILE_RUNTIME.md):
The recovered 32-byte envelope differs from the legacy 28-byte ABI: subtitle
length/pointer are inline, shifting subsequent fields by four bytes. All 12,951
actual EnglishAudio envelopes/relocations pass; 1,280 embedded eight-byte headers
agree with native rate/channels/samples, and 11,671 records have no inline header.
This read-only audit does not decode/encode audio or admit AudioFile compilation.

[AudioEvent in bounded diagnostic builds](docs/RA3EP1_DIAGNOSTIC_AUDIOEVENT_BUILD.md):
`diagnostic-build` now admits checked AudioEvent roots as its sixth family.
Six-family Include graphs preserve source/metadata snapshots, dependency-first
ordering and exclusive verified publication. Authored AudioFile selector suffixes
and formulas reject before normalization; wrong/duplicate/missing external
AudioFile metadata reject. The committed four-root CLI example ran with actual
EnglishAudio metadata, producing 376/44/44 linked streams. AudioFile codecs,
production SDK output and in-game loading remain closed.

[Fixed AudioEvent / Multisound / FX stream](docs/RA3EP1_AUDIOEVENT_FX_STREAM.md):
A three-level Include graph now compiles local FX → Multisound → AudioEvent →
external AudioFile metadata. Native totals 312/36/28 become 320/44/36 linked
BIN/RELO/IMP bytes. Ordered concrete selectors, source attribution and runtime
mapping names round-trip through both readers. Selected AudioFile metadata must
be unique and match EP1 hash 53C81E47/tokenized false. Actual EnglishAudio mapping,
20 corruptions, missing/edit/recovery and existing-output preservation pass.
The separate general bounded admission now passes; this fixed proof does not enable codecs.

[Checked isolated AudioEvent profile](docs/RA3EP1_AUDIOEVENT_PROFILE.md):
An explicit Win32 ProcessInstance entry now reproduces all five stock native
records with prepared concrete AudioFile identities. Current XML scalar/range
edits are revalidated; stale/changed identities, reordered selectors, incomplete
tables, unsupported options, duplicate aliases and the 33rd reference reject.
Production/cache/reuse and AudioFile codecs stay closed. General bounded AudioEvent
admission now uses this narrow profile. Source copies and prepared tuples are checked separately
from the shared stream-admission fingerprint gate, now exercised by the fixed proof.

[Isolated AudioEvent native proof](docs/RA3EP1_AUDIOEVENT_NATIVE.md):
The recovered 128-byte base, 152-byte root and 12-byte weighted AudioFile references
match five real Uprising records' complete BIN/RELO/IMP bytes and ordered
AudioFile identities. Cryo's three-list loop and InitialDelay, IFV's nondefault
weight/shifted FADE_ON_KILL bit, and Yuriko's weighted footsteps/Delay now have
real stock comparisons, alongside the original impact. Independent defaults and
every implemented optional base pointer also pass. The legacy 96/120/8-byte
path remains unchanged. LimitGroup references reject; remaining stock evidence,
wider diagnostic options, AudioFile codecs and production/game loading remain open.

[Multisound in bounded diagnostic builds](docs/RA3EP1_DIAGNOSTIC_MULTISOUND_BUILD.md):
`diagnostic-build` now admits narrow authored/local Multisounds as its fifth
family. Nested local sounds use real dependency-first order; selected local cycles
reject. Five-family Include/snapshot/fingerprint/recovery/publication checks pass.
The committed DiagnosticMultisoundProbe.xml was executed through the actual CLI
with stock global metadata, producing three entries and 216/28/36 linked streams.
Narrow AudioEvent command admission now passes separately; AudioFile command
admission, production SDK output and game loading stay closed.

[Fixed local Multisound / FX mixed stream](docs/RA3EP1_MULTISOUND_FX_STREAM.md):
A three-level Include chain compiles modifier → local FX → local Multisound →
stock AudioEvent targets. Linked BIN/RELO/IMP sizes are 216/28/36 bytes; native
selectors, concrete reference tuples and sources round-trip through both readers.
Wrong/duplicate/missing external metadata, 20 stream corruptions and leaf edits /
loss / recovery are tested. The shared stream gate now explicitly requires every
selected root's prepared reference table. Narrow public Multisound diagnostic
admission now passes; production/game-loading gates remain closed.

[Checked Multisound compiler profile](docs/RA3EP1_MULTISOUND_PROFILE.md):
Explicit isolated ProcessInstance entries now match three complete stock native
buffers and prepared concrete dependency tuples. Missing/ambiguous/duplicate
targets, stale/tampered tables, current source edits and the 32/33-child boundary
are tested. Production/cache/reuse policies remain closed. Fixed mixed-stream
proof and narrow public Multisound admission now pass. Wider sound roots/options
remain unadmitted.

[Isolated Multisound native proof](docs/RA3EP1_MULTISOUND_NATIVE.md):
An explicit EP1 16/28-byte root/child path now matches three complete stock
Multisound BIN/RELO/IMP slices and ordered AudioEvent identities. Optional
pointers, explicit zero and percentage handling have independent synthetic goldens.
The old 16/8-byte legacy layout remains unchanged. Checked isolated ProcessInstance
and bounded Multisound roots are available; production audio registration stays closed.

[External audio fingerprints and native gaps](docs/RA3EP1_AUDIO_FINGERPRINTS.md):
Selected external AudioEvent/Multisound dependencies now require stock EP1 hashes
and non-tokenized metadata. Wrong fingerprints reject; restored inputs recover
identical output. Bounded native reads confirm old sound layouts cannot be reused:
AudioEvent root 120 vs 152 bytes, audio-file references 8 vs 12, and legacy
Multisound children 8 vs 28. Multisound and a narrow AudioEvent subset now have
isolated native proofs; public narrow AudioEvent admission now passes. AudioFile recovery and wider audio
profiles remain open.

[FX support in bounded diagnostic builds](docs/RA3EP1_DIAGNOSTIC_FX_BUILD.md):
`diagnostic-build` now accepts the isolated empty/two-Sound FX subset, alongside
shader/filter/modifier Include graphs. Real Uprising manifests resolve the concrete
audio targets; all three command-produced FX slices and reference tuples match stock.
Duplicate/ambiguous/wrong/missing targets, edited/frozen sources, unsupported options,
injected TypeIds/selectors and publication failures are tested. The checked-in
`DiagnosticFXProbe.xml` runs through the actual CLI and produces five entries with
572/72/44 linked BIN/RELO/IMP bytes. External audio is metadata-only, not rebuilt or
validated as a native payload. Production SDK output and in-game loading remain
unverified; this is not yet a playable mod. See the English usage/report for limits.

| Workstream | Status |
|---|---|
| Manifest v7, BIG/RefPack readers and stream headers | Implemented; checked with real game inputs and structural tests |
| EP1 schema inventory and enum migration | Inventoried; all 48 EP1-only complex types have models/marshallers |
| Shared RA3/KW native layouts and processors | In progress; model presence does not establish compatibility |
| Final EP1 type table / AllTypesHash | Incomplete; legacy hash and explicit experimental-profile policies block production output |
| SDK scripts and WorldBuilder packaging | Partial; not validated end to end |
| Loading a mod in Uprising | Not yet validated |

### Recently completed

- Admitted the narrow FX profile to `diagnostic-build`, preserving frozen inputs, explicit concrete audio mapping and exclusive verified publication. Four-family Include tests and a real CLI example pass; authored TypeIds and Sound selector suffixes now reject before silent core rewriting. See [FX command integration and usage](docs/RA3EP1_DIAGNOSTIC_FX_BUILD.md).

- Recovered FXList's 28-byte root and FXNugget's 44-byte base. Fixed four optional condition masks that were incorrectly inline (268-byte base); they now use pointers and allocate EP1 masks only when present. Empty, single-sound and two-mask source literals match real EP1 streams exactly (28/0/0, 80/12/8, 252/24/12). Added synthetic four-mask/empty-mask/default checks and advanced the legacy compiler revision to 4. Concrete AudioEvent/Multisound resolution, the isolated profile and bounded command now have separate follow-up proofs; full production FX registration remains closed. See [native FX evidence](docs/RA3EP1_FX_NATIVE.md).
- Added the stock-proven ObjectFilterAsset profile to `diagnostic-build` using a separate three-family declaration schema. Eleven emitted filter slices match the existing native goldens, which were also rechecked against the real EP1 static streams. Mixed included shader/filter/modifier entries preserve source identity, strong shader imports and inline weak GameObject IDs; unused tentative filters stay excluded. Expanded controls and official-invalid weak name syntax reject without publishing. The command's GameObject weak IDs do not establish target availability or in-game behavior. See [diagnostic filter integration](docs/RA3EP1_DIAGNOSTIC_FILTER_BUILD.md).
- Extended `diagnostic-build` to bounded nested `all`/`instance` Include graphs. Approved XML is frozen into deterministic flat snapshots; the real resolution closure excludes unused tentative roots and includes required local targets. Manifest entries retain their actual sanitized source identity. Native selection, frozen/edited sources, mixed external FX metadata, cycles/loss/recovery and path/graph limits are tested. A checked-in Include example builds through the actual command. Reference Includes and production SDK output remain disabled. See [bounded diagnostic command and usage](docs/RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
- Added `diagnostic-build` for bounded modifier/shader XML and explicit external manifest/runtime mappings. It freezes approved inputs, rejects unsupported controls/targets, compiles isolated profiles, verifies staged output through both readers and publishes only a new directory. Existing/raced destinations survive; late corruption rejects without publishing. See [bounded diagnostic command and usage](docs/RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
- Added a fixed two-family diagnostic stream round trip from actual resolved document/compiler entries. Manifest identities, ordered local/external references and all native slices agree through both readers; repeated output is deterministic and four corruptions reject. This exposed and fixed Utility.Manifest's four-byte linked offset for prefixed EP1 streams (now eight); prefixless v7 behavior and v6 rejection remain unchanged. No production commit/link, packaged FX stream or game loading is enabled. See [diagnostic stream proof](docs/RA3EP1_MODIFIER_SHADER_STREAM.md).
- Proved nested instance/all Includes with a local shader and external FX metadata in the same modifier: final native selectors match both resolved identities without compiling FX. Leaf edits, external mapping/manifest replacement and loss/recovery pass. Fixed two real core bugs: cached existence accepted a deleted Include source, and failed document calls left stale processing stacks that caused false circular-dependency errors on retry. True cycles and schema errors still reject repeatedly, including ordinary resident document reuse. Reference Include production builds remain blocked. See [Include/mixed-target proof](docs/RA3EP1_INCLUDED_MODIFIER_SHADER.md).
- Connected modifier and shader profiles in a real mixed document: forward references normalize and resolve to local shader identities, and final pointer/RELO/IMP selectors choose those exact targets. Removed targets fail repeatedly; restoration, source retargeting and poisoned-declaration reload recover correctly. This exposed and fixed stale XML id acceptance in both modifier modes. No production/linker output is enabled. See [modifier/shader graph proof](docs/RA3EP1_MODIFIER_SHADER_GRAPH.md).
- Added an isolated Win32 ShaderOverride compiler profile with fresh observed type metadata, bounded literal POIDs, checked injected TypeIds and detached current-value schema validation. Four actual descriptor/document compiler entries preserve source-native output, including PsychicCrush's documented difference. Default fields, unsigned priority boundaries, tampered controls/dependencies, fresh reload and production/cache/platform restrictions are tested. Full builder/inspector builds, 66 groups and layout checks pass. See [ShaderOverride profile](docs/RA3EP1_SHADER_OVERRIDE_PROFILE.md).
- Recovered ShaderOverride's 16-byte root and rule records, optional material-ID pointers and technique strings. Three supplied XML literals match stock EP1 exactly. PsychicCrush's supplied XML differs from stock by one replacement ID; an explicitly labeled detached stock variant matches all 460/132/0 bytes without changing the original XML or adding compiler exceptions. Full builder/inspector builds, 65 compiler groups, layout checks and 33 enum mappings pass. Production registration remains closed. See [ShaderOverride native proof and source variance](docs/RA3EP1_SHADER_OVERRIDE_NATIVE.md).
- Added a separate experimental ObjectFilter profile for the stock-proven NONE-rule infiltration subset. Eleven actual descriptor/document compiler entries match the native proof. It checks root identity, ordered GameObject weak metadata and nested injected TypeIds, rejects expanded controls, and forces fresh source reload. Production/cache/platform restrictions remain closed. See [filter profile](docs/RA3EP1_OBJECT_FILTER_PROFILE.md).
- Matched eleven ObjectFilterAsset roots byte-for-byte against stock EP1 BIN/RELO/IMP slices. Real document default/weak-reference normalization preserves the same output; separate synthetic checks cover optional status masks and include/exclude lists. This is the third focused asset-family proof, not production registration or proof of inherited filters. See [ObjectFilter native proof](docs/RA3EP1_OBJECT_FILTER_NATIVE.md).
- Fixed real output-dependency retry behavior: a failed reference pass no longer leaves a partial validated list that lets the next attempt succeed. Each preparation rechecks current external mappings and file dependencies; errors clear partial state and the visited marker. Ordered external resolution, local chains/cycles, recursive failure/recovery and weak self/tentative/external location tests pass without native output. See [dependency resolution retry proof](docs/RA3EP1_DEPENDENCY_RESOLUTION.md).
- Added direct diagnostic opt-in for normalized modifier imports, with a separate processing domain and strict index/name/type-to-table checks. The four stock goldens still match through the actual plugin entry; malformed/stale metadata is rejected. Real external lookup resolves all seven FX/Shader targets and refreshes correctly when mappings are removed/restored. Descriptor-created v1 and all production/cache gates remain unchanged. See [import profile and lookup proof](docs/RA3EP1_ATTRIBUTE_MODIFIER_IMPORTS.md#explicit-import-profile-follow-up).
- Corrected shared final import encoding: core XML indices remain zero-based, but BIN values are dependency-index + 1 (zero means null). Restored AttributeModifier's optional Shader pointer. Four import-bearing stock EP1 assets now match all native chunks and ordered dependency identities exactly; the no-dependency experimental profile remains restricted. Added boundary/null/repeated-serialization tests and invalidated old intermediate identities. See [import encoding proof](docs/RA3EP1_ATTRIBUTE_MODIFIER_IMPORTS.md).
- Added an isolated opt-in EP1 AttributeModifier profile for the validated no-dependency subset. Eight compiler-entry outputs match the native proof; real document stages preserve defaults and reject stale experimental reuse. Detached schema revalidation handles core TypeId insertion without trusting stale validity flags. Production, binary-cache and precompiled/session reuse remain denied. See [modifier profile](docs/RA3EP1_ATTRIBUTE_MODIFIER_PROFILE.md).
- Matched eight AttributeModifier assets byte-for-byte against real EP1 static/WorldBuilder BIN/RELO/IMP slices, including BLAT_TRIGGER and RADIATION_ARMOR. Fixed absent optional model masks incorrectly allocating 120 extra bytes; advanced the legacy processor cache revision without changing its KW type hashes. This establishes a second focused root-processor proof, not production registration. See [modifier native proof](docs/RA3EP1_ATTRIBUTE_MODIFIER_NATIVE.md).
- Proved document metadata reuse/reload across resident, plain-XML and compressed sessions. Reported same-signature XML/dependency edits and timestamp-preserving dependency size changes reload the source and change identity; quiet documents remain reusable. Fixed a real crash when cached documents have null stream hints. Deleted live dependencies reject rather than silently retaining stale metadata. See [document reuse proof](docs/RA3EP1_DOCUMENT_REUSE.md).
- Made monitor/cache handoff atomic: late events remain queued for the next build, failed initialization restores the consumed batch, and trust is captured with its paths. Incomplete reports still force hashes for known changes without falsely claiming completeness. Tested 500 concurrent events, overflow, replay/ownership guards and builder integration. See [atomic batch proof](docs/RA3EP1_MONITOR_BATCH_HANDOFF.md).
- Made watcher-reported edits force content hashing even with unchanged size/timestamp. Fixed resident metadata sampling and false monitor errors on unrelated unchanged files; added case-insensitive path matching, per-configuration invalidation and stream-hint tests. Watcher callbacks now retain both rename paths, include created/nested files and synchronize snapshots. See [watcher/cache proof](docs/RA3EP1_WATCHER_CACHE.md).
- Fixed timestamp-only file hash reuse: size changes now invalidate source/dependency hashes, deletion/restoration no longer leaves a zero/stale hash, and old sessions are invalidated. Tested full document strong/weak ID changes separately from file-content dependencies; missing file references now report FileNotFound. See [dependency hash proof](docs/RA3EP1_DEPENDENCY_HASHES.md).
- Made local/cache asset copies preserve existing output on rejection or failure: validate/stage before replacement, restore the previous asset if custom-data publication fails, and retain recovery backups if rollback is blocked. Fixed small final copy chunks and signed/overflowing chunk-length checks; 22 copy scenarios plus blocked-recovery tests pass. See [copy recovery proof](docs/RA3EP1_COPY_RECOVERY.md).
- Reconstructed the stored identity checksum exactly from global/static/WorldBuilder/EnglishAudio EP1 manifest metadata. All four match the official capacity-padded algorithm; all four reject the logical-length-only alternative. Preserved that compatibility contract and tested patch equivalence separately from strict intermediate/cache copies. See [checksum and cache audit](docs/RA3EP1_CHECKSUM_CACHE_AUDIT.md).
- Fixed coordinated BIN/RELO/IMP repair: a valid BIN can no longer cause a missing/broken auxiliary stream to be rebuilt with an empty payload. Reuse checks now include exact expected lengths and direct asset/link/version writes honor experimental profile restrictions. See [linker regression proof](docs/RA3EP1_LINKED_STREAM_REPAIR.md).
- Tested real intermediate asset commits, two-asset concatenation, patch-base payload exclusion and 15 independent linked-file repair cases using isolated synthetic fixtures; production EP1 compilation remains blocked.
- Exercised the real document loader/schema/default/hash stages with the EP1 armor profile; matched populated output to the golden-tested tokenizer.
- Closed production early-return paths and disabled experimental session/precompiled document reuse; verified stale cached declarations cannot replace source XML. See [document pipeline proof](docs/RA3EP1_ARMOR_DOCUMENT_PIPELINE.md).
- Added an opt-in Win32 EP1 armor plugin with exact tokenized compiler-entry output, isolated registrations and explicit production/cache restrictions. See [experimental profile](docs/RA3EP1_ARMOR_PROFILE.md).
- Recovered ArmorTemplate's missing tokenization step: exact `744/8/0` byte match with static EP1 armor and `128/0/0` with a WorldBuilder armor fixture.
- Added a schema-to-native-to-tokenized one-asset diagnostic writer; the production type registry/output gate remains unchanged. See [armor PoC](docs/RA3EP1_ARMOR_TOKEN_POC.md).
- Added a metadata-only EP1 type-registry audit with source SHA-256 fingerprints and conflict/target guards.
- Audited global/static/WorldBuilder/EnglishAudio: 254 observed root asset types, 249 unregistered and five with wrong KW type hashes. See [type-table audit](docs/RA3EP1_TYPE_TABLE_AUDIT.md).
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
import selecting dependency index zero (now correctly encoded as BIN word one). Repeated runs produce identical native chunks
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
