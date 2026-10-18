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
