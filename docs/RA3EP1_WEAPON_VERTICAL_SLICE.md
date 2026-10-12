# First weapon vertical slice — October 12, 2026

## Target and current evidence

The next gameplay type candidate is
`WeaponTemplate:SovietAntiInfantryInfantryAK47`, the Conscript's normal primary
weapon. The reference `GlobalData/Weapon.xml` at lines 3733 onward declares
AttackRange=150, WeaponSpeed=999999, FireSoundPerClip=SOV_Conscript_GunFireMS,
FireFX/FireVeteranFX=FX_MuzzleSoviet, three range-duration children and one
DamageNugget with Damage=5, DamageType=GUN. This is not the Molotov weapon.
The reference source stays unchanged.

`WeaponStockReview.Inspect` and CLI `weapon-stock-review` now read the static
manifest metadata and only the selected 2508-byte BIN slice plus its eight-byte
header. They verify EP1 target/type fingerprint, tokenized status, checksum,
root token names/tags/bounds and the two known scalar/native offset relationships.
They do not dump the full stock BIN or compile/patch a weapon.

| Stock property | Observed value |
| --- | --- |
| TypeId / TypeHash | 94D4D96E / 27E8D984 |
| BIN slice offset / length | 344299100 / 2508 |
| RELO / IMP lengths | 28 / 0 |
| Root token entries | 109 |
| Native root base / native root size | 1980 / 340 |
| AttackRange token value offset | 1984 (base + 4), float 150 |
| WeaponSpeed token value offset | 2024 (base + 44), float 999999 |

Manifest SHA256: `74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081`.
Slice SHA256: `2B0C3AABB5521C7C8315A1A5EA22DFCF8D681C6546E9AC58DBF0D0495B2A8C6B`.
Manifest references, in order: `A3A7AF37:9EF0AACB`, then twice
`86682E78:902735BD`. The latter is named FXList:FX_MuzzleSoviet in static;
the first is external to that manifest's named assets. It is now resolved from
global metadata as Multisound:SOV_Conscript_GunFireMS (not from the audio manifest).
Zero IMP bytes do not mean no dependencies.

## Why the existing processor is not enough yet

`SageBinaryData.WeaponTemplate` and `Marshaler.WeaponTemplate.cs` already implement
the 340-byte native root; compiler/layout fixtures cover selected fields.
The real tokenized payload is much larger and includes nested tables/allocations.
Initial root scalar recovery alone was insufficient. The fixed native/token
reproduction below now matches this specimen, but still does not establish
general schema/default admission or source dependency/linking. The experimental armor
tokenizer cannot simply be applied to weapons. Alpha WeaponTemplate input
remains refused; the six-sample e alpha binaries are not changed by this probe.

## Next implementation gates

1. Resolve each stock reference and reproduce schema defaults, child order,
   enum/bitmask values and duration allocations for this unchanged weapon.
2. Recover the full root and nested DamageNugget token trees, including typed
   asset references and relocation words. Recompile from XML and compare all
   2508 BIN bytes, 28 RELO bytes and manifest dependency order independently.
3. Admit a narrow fixed-identity edit (initially Damage=5 to a distinct value),
   ensuring all other output is retained from actual compilation, not a stock
   binary patch. Add malformed/dependency/corruption/repeat controls.
4. Extend scoped Core publication and default-reader package verification only
   after this proof; keep arbitrary GameObject/Weapon builds closed.
5. Request a separately authorized fresh-skirmish test with stock armor and
   normal Conscript AK-47. Armor overrides must not confound the weapon test.

Progress update: the fixed-layout diagnostic below satisfies full BIN/RELO
reproduction for this unchanged specimen and isolates its Damage edit. It does
not close general schema/default admission, source dependency preparation,
scoped Core publication or gameplay gates above.

## Initial stock-review checks

The focused decoder self-test passes a synthetic root and five corruptions
(tag, out-of-bounds pointer, duplicate field, wrong scalar, native offset drift).
Two real stock reviews return identical JSON. An additional manifest lacking
the required Multisound identity is refused. Incremental build: zero warnings/errors;
the preceding rebuild had 111 existing warnings and zero errors. No new full
generic regression suite, package build, GUI or game run was performed here.

## Damage token chain and direct names

The review now follows Nuggets through its count record and polymorphic class
table instead of scanning for a coincidental float. Observed list table: 1428;
class table: 1452; DamageNuggetType derived token table: 1464; native nugget: 2352.
The native list has one member. Damage token points to 2392 (native + 40),
float 5. DamageType points to 2428 (native + 76), enum 21, GUN.
The class hashes are C0F53E99 for DamageNuggetType and C7C2CC04 for its
WeaponEffectNugget base. This stock-review step verifies selected scalar/class/list
relationships only; the later native/token reproduction extends byte coverage.

`weapon-stock-review <static.manifest> <global.manifest>` verifies the three
direct reference names against EP1 metadata. Global SHA256:
`08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
The Multisound itself references AudioEvent:SOV_Conscript_GunFire and
AudioEvent:SOV_Conscript_GunFireDelayed; recursively proving those and the
underlying audio files remains separate from resolving weapon-level names.
Supplying audio.manifest alone as the additional manifest is refused because
it does not resolve the required Multisound. Static-only review still reports
unresolved external names without claiming direct-name verification.

Checks pass: existing five synthetic root corruptions, three detached copies
of the actual weapon slice (wrong damage, list count and nugget class), identical
repeat JSON and wrong-reference-manifest refusal. Incremental build has zero
warnings/errors. These changes do not add a weapon build to the exported e SDK.

## Real native XML marshaling comparison

Historical note: this section describes the initial schema-free comparison.
The focused schema preparation update below replaces manual reference-index
and nugget TypeId injection in the current probe.

`weapon-native-probe <reference-Weapon.xml> <ep1-static-manifest>` selects the
unchanged AK47 XML into an owned document, verifies its three expected textual
references, then prepares fixed diagnostic indices and the known nugget TypeId.
It invokes the actual weapon marshaler twice; this is not stock-byte patching.
It does not validate the full XML against XSD or prove general dependency
preparation, and does not create a tokenized stream or package.

The initial native compile differed at six byte positions. Corrections:

- `Marshaler.WeaponTemplate.cs`: FinishAttackOnceStarted defaults to false,
  matching the official EP1 XSD and stock byte, not the old true fallback.
- `WeaponTemplate.cs`: DamageNugget InvalidTargetStatus starts at native +108,
  before DamageScalarDetails at +140; the 156-byte struct size is unchanged.
- `Includes/DamageFX.cs`: VERSION7 uses the full EP1 schema enum order,
  DEFAULT=-1 and SOVIET_AK47=45; the legacy non-VERSION7 branch is retained.

After those corrections, all 528 normalized native BIN bytes match.
Compiled and normalized-stock SHA256:
`3DA83658B59755C6F5FDFDA98E2AE740E51798CC33CB85CBB7CB28BE8D70A841`.
The compiler emits 28 native RELO bytes and 16 IMP bytes; stock tokenized
representation has 28 RELO and zero IMP, so native equality is not full
tokenized output equality. Remaining gates include complete token/reference
encoding, dependency preparation and scoped publication. WeaponTemplate
remains disabled in the exported e alpha.

Independent stock RELO reads also match all normalized native relocation
locations. An actual second XML compile with Damage=50 changes only the
four-byte Damage slot at native offset 412; RELO/IMP stay identical. Repeated
baseline compilation is deterministic. The broad compiler self-test passes
with the corrected defaults/ABI/enum, and final focused layout/native checks
pass. An overlapping incremental build initially hit the running test's EXE
file lock; retry after the test completed succeeded with zero warnings/errors.

## Fixed token-layout reproduction — October 12, 2026

`WeaponTokenLayoutReview.cs` identifies 13 tables with 159 explicit records;
six zero field-table terminators bring the token prefix to 1980 bytes. Tables
include 109 root fields, three RangeDuration subtrees, a one-element nugget
count table and concrete class wrapper, a derived DamageNugget table with
its base table, and five empty count tables. Count tables/class wrappers use
explicit lengths; treating every subtree as zero-terminated was incorrect.

`WeaponTokenReproduction.cs` stores the reviewed field/tag/offset descriptors
as code, writes records into a fresh prefix, and appends actual XML-marshaled
native data. It does not copy a stock prefix at execution time. This is still
a stock-derived, fixed-shape diagnostic recipe, not schema-driven general
tokenization. Only a finite Damage scalar 0..10000 may differ from the proven
native fingerprint; different fields, allocations, references or list shapes
are refused. Relocation locations are 88,244,248,256,268,368, then the sentinel;
native pointers and those locations are shifted by the generated prefix.
The tokenized representation has no IMP entries; its three dependency indices
remain in BIN and must be paired with the ordered manifest references.

Observed full BIN (2508 bytes) and RELO (28 bytes) exactly match stock;
BIN SHA256 is `2B0C3AABB5521C7C8315A1A5EA22DFCF8D681C6546E9AC58DBF0D0495B2A8C6B`.
An actual Damage=50 XML compile changes only the scalar slot at offset 2392;
all other token/native bytes and RELO are retained. Seven detached negative
controls refuse truncated native data, NaN/negative/oversized damage, a changed
root default, wrong relocation and wrong import locations. Repeat reproduction
is deterministic and caller BIN buffers remain unchanged.

The optional global manifest argument to `weapon-native-probe` independently
checks all three direct reference names/order via the stock-review path, which
also binds BIN headers to the manifest checksum. This does not recursively
prove audio closure or bind arbitrary authored references to compiler indices.
Production/schema gates stay closed; no BIG, install, GUI or game run occurred.
Final focused stock/layout/token checks pass, two full JSON reports are identical,
and supplying audio.manifest instead of global.manifest is refused. The first rebuild had 111 existing
warnings and no errors; subsequent incremental rebuilds had no warnings/errors.
The prior broad regression still covers the preceding ABI/default/enum fixes;
it was not rerun for this diagnostic-only addition.

## Focused schema/default/reference preparation

`tests/fixtures/WeaponDocumentPipeline.xsd` includes untouched official EP1
WeaponTemplate, primitive/reference, damage/death/DamageFX, status/filter and
audio/FX type definitions. Its weapon/container roots are a focused harness,
not the complete SDK schema. Two extracted FX enum definitions are checked
against the official BaseModules definitions before use; no missing-type
stubs or edits to the official XSD catalog are introduced.

`WeaponSchemaPreparation.cs` validates the selected weapon, inserts defaults
and invokes the actual Core `HandleAssetReferenceType` normalization through
the existing shared reference-test seam. Numeric indices are no longer
assigned by the probe. DamageNugget TypeId is derived from its validated
schema type, not a manually injected tag. Optional stock metadata checks the
schema-derived concrete candidate names/order, including BaseAudioEventInfo
resolution to Multisound. This is not a complete DocumentProcessor/plugin/
OutputManager build or recursive dependency-closure proof; the report explicitly
sets `FullCoreDocumentBuildUsed=false`.

Schema insertion exposed a native allocation bug: the default empty
UnderAttackOverrideEvaEvent produced a 12-byte empty-string allocation and two
additional relocation entries. `Marshaler.WeaponTemplate.cs` now preserves
the stock null override for empty/whitespace EvaEventRef values. This is scoped
to that weapon field, not a blanket removal of schema defaults. A compiler
regression checks absent versus empty equality and retains nonempty allocation.

Baseline and authored Damage=50 XML both pass focused schema/Core reference
preparation, then retain the full BIN/RELO proof and isolated edit. Eight
detached controls reject unknown attribute, invalid damage enum, unknown child,
missing sound reference, wrong FX reference, unsupported identity, inheritance
and unresolved expression. Source XML and global Settings are preserved.
These are diagnostic errors, not new BuildStudio weapon capabilities.
The exported e alpha remains unchanged. Next: full document/dependency
preparation with a private weapon processor, scoped output and default-reader
package checks, before a separately authorized gameplay test.

Verification: focused schema/token proof passes twice with identical JSON;
layout and stock-review self-tests pass. The full compiler regression also
passes, including absent/empty/nonempty EVA allocation checks. Final build has
zero warnings/errors; the intermediate XmlCompiler rebuild had two existing
nullable-context warnings. No output package or gameplay evidence is added.

## Actual Core document and private plugin dispatch

`WeaponDocumentProbe.cs` adds `weapon-document-probe <reference-Weapon.xml>
<static.manifest> <global.manifest> <new-output-directory>`. It selects the
reference AK47 into an owned AssetDeclaration source, snapshots the external
manifests and loads that file with actual DocumentProcessor (no precompiled
reuse or output). The real AddOutputInstance dependency preparation resolves
the ordered Multisound/FXList/FXList targets; only the weapon is queued locally.
The private registry then dispatches a WeaponTemplate plugin. This diagnostic
uses the previously proven fixed token recipe, not a general token compiler.

The plugin rejects unprepared/foreign dependencies and unsupported identities,
requires current schema identity, verifies Core-injected TypeId annotations
against their schema-name hashes and revalidates a detached copy before native
compilation. Production output, build cache and compiled-document reuse remain
disabled. Its private processing revision is not claimed to be an EA hash.

The full 2508-byte baseline must match the independently reviewed stock SHA256.
Repeat compilation is deterministic. Damage=50 changes only the four-byte slot
at offset 2392 and changes Core's instance hash from 4094310166 to 3643575635;
restoring the source restores both bytes and hash. Four additional controls
reject a forged TypeId, unknown post-validation attribute, removal of all
external mappings after successful preparation and an unsupported type ID.
Restored external mapping permits compilation again. A JSON receipt records
the evidence in the owned output directory. No BIN/RELO/IMP stream, BIG,
installation change, GUI update or game launch is made by this command.

Verification: Release/x86 build passes with zero warnings/errors; the document
probe passes its stock/repeat/edit/restoration and four refusal checks. Earlier
schema-only receipts remain historical evidence, not full-document receipts.
Next gate: narrowly authorized OutputManager stream emission with independent
reader checks, followed by package checks and separately authorized gameplay.
The exported e alpha is unchanged and WeaponTemplate remains unavailable there.

## Scoped actual OutputManager streams

`weapon-stream-probe` takes the same four arguments as `weapon-document-probe`
but explicitly opts into diagnostic stream emission. `WeaponStreamProbe.cs`
uses `OutputManager.CreateExperimentalWeaponDiagnostic`, backed by the separate
`ExperimentalWeaponOutputScope.cs`. The shared internal scope contract leaves
the existing armor admission rules unchanged; ordinary production policy,
build cache, precompiled reuse, cleanup and version-file publication stay closed.

The weapon scope freezes one exact document, source bytes, concrete and nominal
reference identities/hashes, plugin/processing revision and two external
manifest snapshots. It independently checks the full BIN stock fingerprint
after normalizing only Damage to 5, its finite 0..10000 bound, and all seven
RELO locations; arbitrary weapon shapes remain refused. Runtime external paths
are explicitly `static.manifest;global.manifest`, not inferred from snapshot
filenames. This mapping is diagnostic evidence, not an independently proven
runtime package/load recipe or recursive audio-closure proof.

Actual `PrepareOutputInstances`, intermediate `.asset` commit, `CommitManifest`
and `LinkStream` generate one linked v7 stream. Both readers validate identity,
instance hash, tokenized flag, checksum, allTypesHash, the three ordered concrete
reference IDs and two external manifest paths. BIN/RELO/IMP magic, checksum,
sizes and all payload bytes match the expected compiler buffers. Two fresh
baseline outputs are byte-identical. A fresh Damage=50 document changes the
instance hash, stream checksum and expected BIN scalar; restoring source
restores the original compiled bytes/hash.

Nine scoped controls reject modified in-memory bytes, corrupted intermediate,
changed XML TypeId, source edit, cache enablement, reordered runtime mapping,
external snapshot corruption, null document and cleanup. Restored inputs permit
byte-identical relinking. Owned artifact directories retain diagnostics;
no original reference XML/manifest, game installation or exported alpha changes.
No BIG, config, version file or game launch is generated by this command.

Next: diagnostic package/default-reader validation and then a separately
authorized weapon gameplay test. General WeaponTemplate authoring is not yet
enabled in the alpha or GUI.

Verification: the final isolated Release/x86 build has zero errors and 56
existing warnings, with no warnings attributed to the new scope/probe files.
Final-tool stream checks pass for baseline, repeat and Damage=50, including all
nine scoped controls and ordinary production-writer denial. The no-output
document command, stock-review and layout tests also pass. Actual single-armor
and two-armor document/stream regressions both retain their stock matches and
policy refusals. Test artifacts stay local and are not part of the alpha export.
The full managed compiler regression also passes. Its temporary directory was
redirected into an owned repository artifact directory after the initial system
TEMP move was denied by sandbox permissions; no production source was changed
to work around that environment limitation. The broad run used the initial
scope-enabled tool build; final-build focused probes above cover the subsequent
scope field rename, reference-signature strengthening and added runtime/ordinary
production-denial controls.

## Offline default-reader weapon candidates

`weapon-package-probe` accepts the same four inputs as `weapon-stream-probe`
and explicitly runs the complete source/schema/Core/dependency/plugin/scoped
stream chain before packaging. Neither older command generates a BIG or config.
`WeaponDiagnosticPackage.cs` independently binds each stream's identity,
checksum, full payload, ordered direct reference IDs and explicit static/global
runtime paths to the actual Core result. The stock-shaped fingerprint and
relocation locations are rechecked before publication.

Only the existing BIG4 framing serializer is reused from the armor path;
armor authoring and package admission are not widened. Each candidate has
fifteen entries: `data\\mod`, `data\\mod_l`, `data\\mod_m` families, each with
manifest/BIN/RELO/IMP and a two-byte blank CRLF version marker. All three
families carry byte-identical compiled streams. Both independent readers
validate embedded identities, hashes, ordered references and external paths.
Exact directory ranges, names, lengths, full payloads and EOF are checked.

`weaponprobe_1.0.cfg` and `weaponprobe_1.0.skudef` contain only
`add-big RebornWeaponProbe.big`. The SKU is a mod fragment, not a replacement
game SKU. Short sibling candidate directories avoid deep scope paths; config
paths must satisfy the existing conservative 240-character printable-ASCII
bound and the fixed filename's 11-character identity fits the reviewed 15-byte
limit. These detached checks are not runtime-safety or config-consumption proof.
Reports include requested arguments as data only, and require the installation
as working directory; no launch is implemented here.

Baseline Damage=5, repeated baseline and edited Damage=50 candidates pass.
Repeated baseline BIGs are byte-identical. Each candidate exercises ten
refusals: occupied destination, corrupt BIN, altered concrete reference ID,
altered external runtime path, four BIG directory/payload corruptions and two
changed config fragments. Source streams/packages/fragments are restored after
each owned corruption test. Reader scratch copies stay outside the BIG.
The per-candidate `weapon-package.json` records SHA256, size, damage, Core hash,
checksum, paths and offline evidence. The parent document receipt distinguishes
`DiagnosticPackagesBuilt=true` from `ModPackageBuilt=false` (no playable SDK
package certification). Partial failed artifacts remain retained without a
success receipt; existing output is never overwritten by a new preparation.

No armor overrides, stock assets, game binaries or stock SKU are bundled.
No installation changes, alpha export, GUI changes or game execution occur.
RuntimeReady, config consumption, effective weapon override and recursive
audio closure remain false/unproven. Next gate: fresh installation/process
preflight and separately authorized gameplay comparison in a new skirmish with
Conscript's normal AK47, not Molotov. Only after that should the bounded weapon
profile enter the alpha; arbitrary weapons and IDs remain future work.

Verification: final Release/x86 build passes with zero warnings/errors. The
package probe passes baseline/repeat/Damage=50 candidates and ten package
controls each; the edited candidate is 8730 bytes with SHA256
`932415FE98AC7C527604A01923AC9D7910E6C6116C6F59A42C51912C0DCEBC5E`.
The existing stream-only command still reports no packages and its stream
controls pass. The single-armor actual Core/package regression also passes.
Core layouts/output scope and the shared BIG serializer were not changed in
this addition; the preceding full compiler regression is retained evidence,
not claimed as a newly rerun full suite for the packaging wrapper.

## First separately authorized weapon launch

`scripts/Invoke-Ra3Ep1WeaponTrial.ps1` pins the reviewed Damage=50 candidate
path, 8730-byte size and SHA256 rather than selecting a mutable latest build.
Its default invocation is preflight-only. It reuses the strict reviewed
launcher/game/SKU/campaign identity and known loose-override checks, validates
both exact config fragments, verifies stock archive presence and refuses any
existing named Uprising process. Read-share handles freeze the candidate files
through the observation period. Only an explicit `-Run` starts one normal
launcher with stock working directory and separately quoted fixed arguments.
No debugger, system-wide trace, retries, installation writes or termination
are implemented. Process snapshots remain non-atomic and PID reuse is not fully
excluded, matching the existing armor observer's limits.

The user explicitly authorized this single Damage=50 start. Preflight passed;
the initial sandbox CIM denial was resolved by running the same read-only
preflight with host access, not by omitting process exclusion. One launch then
completed its 30-second observer without error: 37 direct 1.1 child snapshots
carried the exact requested config argument. The local launch receipt is
`artifacts/WeaponLaunch-05870f31a9be4ce8840e9b1cec943d07/launch.json`.
These are visible argument observations, not config-read, manifest-load,
main-menu or gameplay proof. Subsequent user feedback on the requested normal
Conscript AK47 damage comparison is positive, as recorded below. The alpha
remains unchanged; no second start is authorized by this single-trial approval.

## User-observed Damage=50 behavior

After the single authorized launch, the user answered the requested
fresh-Soviet-skirmish/normal-AK47 comparison with "hasar artmış" (damage
increased). This supplies the first positive user-observed gameplay evidence
for the exact Damage=50 candidate, SHA256
`932415FE98AC7C527604A01923AC9D7910E6C6116C6F59A42C51912C0DCEBC5E`.
The package contains only WeaponTemplate:SovietAntiInfantryInfantryAK47 and no
armor override; the offline payload differs from the stock weapon only at the
reviewed Damage slot, apart from corresponding identity/checksum fields.

No exact multiplier, quantified stock-versus-edit timing, independent runtime
file-read trace or recursive audio closure is claimed. The immutable launch
receipt retains its technical config-consumption/override flags as false;
this separate user report is behavioral evidence, not a rewrite of the
observer's capabilities. No additional launch or installation change follows
this feedback. The next implementation gate is a bounded editable AK47 authoring
profile and alpha integration, not arbitrary weapons/IDs or full SDK completion.
