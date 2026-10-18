# Source-authored Conscript identities and offline trial packages

## Outcome — October 18, 2026

Two complete Conscript candidates now pass source -> native -> metadata ->
linked streams -> BIG/config qualification without installing or launching:

| Candidate | Identity | Authored change | Current runtime boundary |
| --- | --- | --- | --- |
| override | SovietAntiInfantryInfantry | BuildTime 4 -> 1 | Game test not performed |
| new-id | RebornConscriptTrial | New identity and BuildTime=1 | No recruitment/spawn integration |

These are real source recompilations, not edits to a stock BIN. All 49 modules,
six populated owners, geometry, 126 metadata fields and 275 normalized strong
reference occurrences remain intact. The expected visible override effect is
faster production of the existing Conscript; that is a **test hypothesis**, not
observed gameplay evidence.

## Source and complete-graph qualification

`source/BinaryAssetBuilder.ManifestInspector/GameObjectConscriptTrialSmokeTest.cs`
receives the already qualified captured source chain and invokes its actual
schema/normalization/allocation callback for each edited XML and a repeat.
Before compilation, source admission permits only the two literal identities
above and literal BuildTime=1. Restoring those two attributes must reproduce
the complete captured source exactly; unknown attributes, children and edits
refuse. No general source includes/inheritance engine is implied.

The new identity's hashed ID must not collide with any real GameObject in the
supplied catalogs. Both output declarations must have their authored semantic
identity and the same complete ordered dependency occurrences as the original.
Original BuildTime is independently anchored to float 4 at native offset 140;
authored BuildTime must be float 1. Restoring only that word must reproduce
every original native/RELO/IMP byte and pass the full independent original
stock ownership proof. This does not broadly ignore a mutable field.

Metadata is newly composed from each compiled graph. It must equal the fully
stock-qualified original metadata/native pair with only the corresponding
native BuildTime word changed at prefix 88,824 + offset 140. Repeated source
compilation must reproduce the same result. Additional native changes and
unchanged/wrong BuildTime controls refuse. Baseline source remains unchanged.

## Separate stream profile

`BoundedDiagnosticBuild.SerializeConscriptTrial` admits only the two fixed
identities with source BuildTime=1, matching semantic instance ID, EP1
GameObject type/hash, exact qualified sizes, the native float-1 word and 275
validated references identical to the normalized reference list.
`SerializeOriginalConscript` and default tokenized admission remain separate.

The shared stream proof runs all **580** refusal/readback controls for each
authored identity as well as the original-stock profile. Actual Core checksum
packing is retained. Instance hashes remain explicitly experimental hashes of
normalized XML, not full EA processed-document identities. External catalog
targets and runtime mapping are verified; their native/transitive payload
closure is not rebuilt or claimed.

## Offline package format

Only already stream-qualified bytes reach the packager. It reuses the existing
low-level BIG4 byte packing, **not armor source admission**. Independent BIG
header/range/index/readback checks require these fifteen entries:

`data\mod{,_l,_m}.{manifest,bin,relo,imp,version}`

Each suffix contains identical qualified stream bytes and a CRLF `.version`
marker, following the earlier observed default-mod-reader route. This is not
independent LOD selection support. There must be no unowned archive suffix.

Each fresh candidate also contains:

- `RebornConscriptProbe.big`
- `reborn_1.0.cfg` and `reborn_1.0.skudef`: only `add-big RebornConscriptProbe.big`
- `trial-source.xml`: captured flattened source plus the admitted edits
- `DO_NOT_LAUNCH_YET.txt`
- `trial-plan.json`: asset/BuildTime, archive/source SHA-256, profile, validation
  results, config path and explicit false runtime/install/gameplay flags

The SKU is a **mod fragment**, not a replacement for the stock game SKU.
All five prepared files are read back exactly; each is corrupted separately
and rejected before restoration. An occupied output directory refuses.
The receipt is written only after final restored package readback.

## Evidence and remaining runtime preparation

Focused batch `artifacts/ConscriptAuthoredTrials-a/batch-report.json` passed
**74/74 groups in 225.049 seconds**, four isolated workers. The complete
actual-original/native/metadata/three-stream/two-package group took **75.403
seconds**; previous all-module and selected-root cases passed in **23.883 /
24.876 seconds**. All eight frozen runtime hashes stayed unchanged. Batch
build: zero warnings/errors. No Full run or game/installation changes.

The authored gate passes **8 source/native refusals**. Each of three stream
profiles passes **580** controls. Each authored package rejects **5** file
corruptions plus an occupied output. Reports and candidate paths are in
`gameobject-original-native.log` and beneath that group's owned temporary
directory. Those are diagnostic artifacts, not a distributed SDK release.

The generated batch config paths are **256 characters for new-id and 258 for
override**. They do not meet the existing conservative 240-character launch
policy. **Do not launch directly from those artifact paths.** A fresh shorter
validated export and runtime preflight are required first; receipts correctly
retain `RuntimeReady=false` and `GameExecuted=false`.

The original-identity override can then be tested for production time after
separate controlled game-test authorization. The new-ID candidate does not
replace the original Conscript and cannot be recruited merely by loading its
stream: a CommandButton/CommandSet or explicit map/script spawn integration is
still needed. Arbitrary identity/field authoring and exported-alpha capability
expansion remain open. No new-ID gameplay or menu support is claimed.

## Short export and file map

The original-identity BuildTime=1 candidate is now exported to
`artifacts/Conscript-20261018-a`, unchanged from the source-qualified package:

| File | Purpose |
| --- | --- |
| RebornConscriptProbe.big | Actual 15-entry mod data archive, 400,971 bytes |
| reborn_1.0.cfg | Fixed `-modconfig` input; mounts that BIG only |
| reborn_1.0.skudef | Identical mod fragment, not the stock game SKU |
| trial-source.xml | Captured authored XML snapshot; editing it does not rebuild the BIG |
| trial-plan.json | Hashes, provenance, relocated config path and conservative false runtime flags |
| DO_NOT_LAUNCH_YET.txt | Reminder that preparation is not game-test authorization |

Archive SHA-256:
`3FCDFFB9909B39E9C3A6B75AC50791A19D3EFBAE5A83D6169C174197C50C8FB2`.
Source SHA-256:
`B7EDFA37B5F1A26FA2ED5610BFEB2B2F05496B5BF150B9E068F9A4AB066DC6D6`.
The absolute config path is **156 characters**, within the conservative 240
character limit; its basename also passes the reviewed short-identity policy.

`scripts/New-Ra3Ep1ConscriptTrial.ps1` pins those exact hashes and the fixed
receipt profile. It reads bounded files through read-share-only handles,
rejects reparse ancestry, accepts only a direct child of repository artifacts,
creates new files exclusively and never overwrites an occupied export. It
preserves source bytes/provenance and updates only the exported receipt's path.
`-CheckOnly -OutputDirectory <absolute-export>` verifies without creating files.
`-SelfTest` rejects four byte corruptions and a wrong BuildTime receipt without
creating output. Separate occupied/nested/overlong output refusals passed and
the candidate archive remained unchanged.

`scripts/Invoke-Ra3Ep1ConscriptTrial.ps1` selects **only** this fixed short
export. Without `-Run`, it rechecks the export, stock launcher/1.0/1.1/SKU/
campaign hashes, absence of known loose overrides, stock archive presence and
absence of Uprising processes. That preflight passed. The sandbox initially
denied the CIM process query; the same **non-launch** preflight then passed
with read-only host access. No checks were removed or relaxed.

At that preflight checkpoint its `-Run` branch had **not been executed or runtime-validated**. It is
reserved for one separately authorized launcher start using the stock working
directory and fixed config. It never retries, terminates games, installs files,
uses a debugger or records system-wide traces. Thirty-second process/argument
observations would be saved separately and would not certify config reads,
main-menu success or effective overrides. A new Soviet skirmish production-time
comparison is still required after authorization. The new-ID candidate is not
selected by this launcher and is not yet recruitable.

This update changes only scripts/docs and local export artifacts; the preceding
74/74 compiler/native/package regression remains its own evidence. No new C#
compiler build or full batch was necessary for this export-only step.

## SKUDef-only experiment — outcome pending

Following explicit user authorization, a distinct candidate was exported to
`artifacts/Conscript-Sku-20261018-a`, with only `reborn_1.0.skudef`, no CFG
counterpart and unchanged pinned archive/source bytes. The selected path is
163 characters. `New-Ra3Ep1ConscriptTrial.ps1 -SkuDefOnly` exports/checks this
format; it refuses a SKUDef-only check if a CFG counterpart is present. Four
SKUDef-only byte/receipt controls pass, the prior five legacy controls still
pass, and checking the legacy dual-file directory as SKUDef-only refuses.

`Invoke-Ra3Ep1ConscriptTrial.ps1 -SkuDef -Run` performed one controlled launcher
start. Report: `artifacts/ConscriptLaunch-970ba463f81a43b3b06730ee4d68a3d0/launch.json`.
The 30-second observer recorded 39 direct-child snapshots matching the selected
SKUDef argument; `SkuDefExperiment=true`, `CfgCounterpartPresent=false`,
`GameExecuted=true`, `Failure=null`, and `InstalledFilesModified=false`.
No automatic retry or process termination was performed.

Main-menu arrival, actual config consumption and BuildTime override remain
unproven pending user observation. This is not yet evidence to remove the
historical CFG route everywhere. The separate legacy candidate remains intact.
