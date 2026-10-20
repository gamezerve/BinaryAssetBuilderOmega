# Uprising Mod SDK Alpha — live repository workspace

## Build your mod

<!-- Reborn: explain the readable Studio log and full diagnostic location without changing the user's entry point. -->
Studio now shows the official-style numbered build phases, durations and source
errors/warnings. Unsupported or unused phases are explicitly skipped, not
reported as compiled. JSON receipts and internal stack traces are retained in
`alpha/.work/last-detail.log` (success) or `last-failure-detail.log` (backend
failure), rather than filling the Build Log pane. Successful output keeps a
readable `builtmods/<name>/last-build.log`; current failures are recorded in
`.work/last-failure.log`. This presentation change does not enable new asset
types or resolve existing inheritance restrictions.

Current RebornTest uses five EA-shaped XML files: `data/mod.xml` (Includes),
`data/Conscript.xml` (derived unit), `data/ConscriptBase.xml` (complete instance base),
`data/Defines.xml` (BuildTime definition), `data/CommandData.xml` (explicit command).
Change `REBORN_CONSCRIPT_BUILD_TIME` in Defines.xml to edit the inherited unit's
BuildTime. Actual Core inheritance runs; the instance base is not emitted.
There is **no automatic LogicCommand generation**. Read the
[source frontend support table](../docs/RA3EP1_EA_SOURCE_FRONTEND.md) before using
inheritance, source aliases, omission rules or arbitrary modules.

DATA/ART/AUDIO use mod-first, then stock SDK lookup; DATA defaults to adjacent
SageXML. This local SDK now contains an exact original Uprising source copy there;
the external SdkDataRoot override is removed, and UprisingXmlRoot uses the same
local copy. The stock library is ignored by Git pending redistribution review.
ART basename lookup tries direct mod
Art, mod two-character fanout, then SDK fanout. AUDIO searches mod then SDK Audio.
See [source search details](../docs/RA3EP1_SOURCE_SEARCH.md). Locating resources
does not yet enable W3X/audio compilation.

Open **`alpha/EALAModStudio.exe`**. This is the user-facing entry point.
Select `Reborn` or `RebornTest`, edit its `alpha/mods/<name>/data/mod.xml`, then click
**Build Mod** in Studio. Read Build Log / Error Log for the result.

Successful builds publish these files in the same SDK:

- `alpha/builtmods/<name>/<name>.big`
- `alpha/builtmods/<name>/mod_1.0.skudef`
- `alpha/builtmods/<name>/build.json` and `last-build.log`

No CFG counterpart is required by this workflow. This temporary output directory
is reused per mod, not multiplied into dated SDK copies. A failed build leaves
the last successful output intact; it is not evidence of a new successful build.
No automatic installation or game launch occurs.

Closing Studio while its own build is active asks whether to cancel that build.
**Yes** stops its worker process tree, releases the SDK lock and closes Studio;
**No** keeps Studio and the build running. A completed/idle build needs no prompt.
Other Studio sessions and unrelated processes are not cancelled. The empty
`.sdk.lock` file may remain: lock ownership is a live OS handle, not file existence.
Restart an already-open Studio once after installing this update. Earlier builds
started without this lifecycle helper cannot be retroactively adopted.

Compiler maintenance refreshes `alpha/tools/compiler` after a successful
Release/x86 build of BinaryAssetBuilder or ManifestInspector. Your mod XML and
personal paths are preserved. Studio builds and synchronization share a lock to
avoid mixed compiler generations. An up-to-date toolchain does not expand the
supported input scope listed below. Internal maintenance scripts are not an
additional user workflow.

## Workspace ownership

Only two mod folders belong in the Studio list:

- `Reborn`: the user's mod. Developer tests do not change its sources or build its outputs.
- `RebornTest`: the developer's reusable test mod. Its `data/mod.xml` includes
  the derived unit in `Conscript.xml` and explicit command in `CommandData.xml`.
  `ConscriptBase.xml` supplies the actual instance base; `DATA:Defines.xml` supplies
  the owner-visible BuildTime definition. General derived child merges remain closed.

Select **RebornTest → Build Mod** to compile that Conscript from the selected
XML through Core, including complete native/metadata/reference/stream checks.
The output is `alpha/builtmods/RebornTest/RebornTest.big` and `mod_1.0.skudef`.
This is not a cached trial-BIG export. The unchanged BuildTime=1 regression fixture
must still reproduce the previous game-observed candidate. New scalar values are
offline-qualified, not new gameplay observations.

This Conscript route accepts the original identity or a custom collision-free
ASCII ID, schema/ABI-backed root primitive and fixed enum/KindOf/weak-ID edits,
optional stock-target root strong references with dependency reindexing, and the
384 direct primitive slots in the existing 49 modules and the existing nested
numeric owners on the same complete Conscript shape. An explicit
UNIT_BUILD command may target that local object. Other GameObject/module changes
and arbitrary asset mixes still fail; this is not a general unit compiler.
It additionally needs `UprisingXmlRoot` in the local paths file, pointing to the
matching official Uprising XML directory. Preserve the supplied complete XML;
unreviewed changes are rejected, not silently ignored. ALPHA005 marks this scope.

| XML location | Editable capability | Validation |
| --- | --- | --- |
| GameObject | Current scalar processor registry (63 attributes), including Scale, ThreatLevel, BountyValue, root booleans, Time/Angle/duration values | Official schema + concrete finite/native type; no six-field gameplay caps |
| GameObject | 15 fixed root values: KindOf masks, six enums and seven weak IDs | Official schema + concrete EP1 enum/bit/hash ABI; grouped native/metadata checks |
| GameObject | Optional root strong references: UnitTypeIcon, UnitIntro, CrowdResponse | Real stock target of the correct family required; removing/emptying/restoring icon and intro reindexes all later imports. CrowdResponse has no available stock catalog target |
| GameObject | Six root ASCII strings: Description, DescriptionTransformed, TypeDescription, TypeDescriptionTransformed, EditorName and ExperienceScalarTable | Each may be added, removed, emptied or resized independently up to 4096 bytes; native pointers, token targets, RELO and IMP coordinates are rebased |
| ObjectResourceInfo/BuildCost | Amount | Schema-valid unsigned 32-bit integer |
| Body/ActiveBody | MaxHealth | Schema-valid finite real |
| Existing Draws/Behaviors/AI/Body/Client modules | 384 direct primitive slots across the current 49 module instances | Official schema/default + concrete fixed-width native codec + independently qualified token offset/type; [full field inventory](../docs/RA3EP1_MODULE_PRIMITIVE_CAPABILITIES.md) |
| VisionInfo | VisionRange / ShroudClearingRange | Schema-valid finite real |
| LocomotorSet | Speed | Schema-valid finite real |

Keep structural/module topology unchanged. Root strings use bounded dynamic
allocation, but remain non-NUL ASCII. Root strong-reference presence is editable;
module/list imports outside these three root slots retain their existing topology. The current
RebornTest new-ID trial uses time=2, cost=15, health=750, vision=400, shroud=700
and speed=100, plus Scale=1.2 and ThreatLevel=15 (the latter two are new offline
tests, not a new gameplay confirmation).
Speed is the existing GameObject LocomotorSet speed; the referenced locomotor
asset is not replaced. Decimal/scientific finite values are accepted except cost.
Former conservative alpha ranges no longer gate these values. Schema-valid
numbers are not a guarantee of sensible or safe gameplay tuning.
See [schema/native capability contract](../docs/RA3EP1_GAMEOBJECT_SCALAR_CAPABILITIES.md).
See [qualification and gameplay checklist](../docs/RA3EP1_CONSCRIPT_SCALARS.md).

The earlier original-ID grouped scalar trial has qualitative user confirmation.
For `RebornConscriptTrial`, `CommandData.xml` explicitly declares the freshly compiled
`Command_ConstructSovietAntiInfantryInfantry` LogicCommand override pointing to
the new object. The existing Conscript Barracks slot/icon produces the new unit;
no extra menu slot or CommandSet edit is made. Other Barracks choices and the
original GameObject remain stock. The preceding new-ID package has user-reported
gameplay success; this source-frontend revision is independently build-validated.
See [new-ID build and test](../docs/RA3EP1_CONSCRIPT_RECRUITMENT.md).

Previous armor/AK47 samples now live under `tests/fixtures/live-alpha/`, outside
the Studio mod list. Automated checks run them sequentially through RebornTest
and restore its original source directory afterward. Historical generated sample
outputs were moved recoverably to `alpha/.work/legacy-fixture-outputs/`.

This is a repository-linked local SDK, **not a standalone portable distribution**.
EA binaries and personal paths are generated locally and ignored by Git.
See [maintenance and retained lessons](../docs/RA3EP1_LIVE_ALPHA_AND_RETENTION.md).

## Supported input and evidence

This is a limited, experimental **local-use candidate**, not a public release
or a complete Uprising SDK. It retains the existing EA Mod BuildStudio interface.
User-provided screenshot/log evidence now confirms actual Studio loading,
Reborn selection, output capture and successful completion, with independently
verified package bytes. A subsequent Error Log screenshot confirms ALPHA001 and
ALPHA002 under Warnings. The corrected InvalidXml screenshot also confirms the
failure callback without Build Completed. Updated local packaging passes clean
extraction with the fix. These checks do not establish all
original Studio features.

Supported: one flattened `AlliedConYardArmor` or `SovietConYardArmor`, or one of
each together in an AssetDeclaration. Each has thirteen ordered armor
entries. Each Percent and five optional root scalar fields (Default, DamageScalar,
SideDamageScalar, RearDamageScalar, FlankedPenalty) accept canonical integers
0..10000. Omitted scalars retain 100. Identity, list shape and damage order remain
fixed in this legacy route. The experimental armor source route below adds bounded
includes/inheritance and custom armor identities. Unsupported XML
fails rather than being silently omitted. Reborn uses GUN=5000; ArmorCannon uses
CANNON=5000 and stock GUN=100. Soviet GUN=5000 is supported. Allied GUN/CANNON
and Soviet GUN have user-observed gameplay evidence. CombinedArmor uses Allied
GUN=250 and Soviet GUN=5000 in one stream; Soviet has positive user-observed damage,
while Allied GUN=250 is inconclusive. CombinedArmorStrong uses GUN=5000 for both;
its Allied Peacekeeper test is positive, but Soviet was not re-tested in that
revision. These are qualitative observations, not exact multiplier measurements
or proof of every new build. Duplicate identities fail the whole build.

Also supported, separately from armor: one complete stock-shaped
`SovietAntiInfantryInfantryAK47` WeaponTemplate. The archived ConscriptAK47 fixture
uses Damage=50; only Damage is editable in that profile. Integers 0..10000 with
an optional `.0` suffix are accepted. All other fields, reference names, identity
and child order must remain unchanged. Standalone XML or a one-member
AssetDeclaration is accepted. Mixed armor/weapon streams are not supported.
The earlier diagnostic Damage=50 candidate has positive user-observed damage
evidence; freshly authored packages are not automatically gameplay-verified.

Prerequisites: PowerShell 7, an x86 .NET 8 runtime, your existing RA3 Mod SDK
Studio components and compatible .NET Framework 4/CodeDOM environment, plus an
unpacked Uprising `static.manifest` and its corresponding BIN/RELO/IMP files.
Weapon builds additionally require an explicit unpacked `global.manifest`.
Game binaries, stock streams, codecs and WorldBuilder are not provided.

Experimental armor source route: the archived `ArmorSource` fixture's three-file graph
compiles two owner-declared armors: `AlliedConYardArmor` inherits a prepared base
with GUN=5000, and `RebornNewArmor` is a new identity without a gameplay consumer.
The imported base is not emitted. This new package has no gameplay proof.

An AssetDeclaration with Includes/Defines/Tags, custom armor IDs or more than two
armors selects this route automatically; `-ArmorSource` forces it for an otherwise
legacy-shaped declaration. It accepts 1..32 owner armors with custom IDs, 0..64
unique non-ALL damage entries each, finite unsigned values 0..10000 with optional
decimal/percent notation, and the five optional root scalars. Exact `=$NAME`
field references, reviewed definition aliases/suffix concatenation/nonnegative
integer multiply-add, local/imported `inheritFrom`, directly visible `instance`
bases and Include-free definition-only `all` leaves are supported. Paths stay
inside the mod data root. Limits: 32 sources, 64 Include edges, 64 KiB per file,
one MiB total. ALPHA004 marks the experimental boundary.

This is not full EA source semantics: same-ID source shadowing, definition
overrides, child replacement by Damage, arbitrary expressions/all Includes,
reference Includes, nonempty Tags and mixed armor/weapon streams fail. A child
list that produces duplicate damage kinds is rejected. New IDs require consuming
GameObjects, which are not build-enabled here. The old fixed samples retain
their original admission and golden package checks. `-ValidateOnly` audits and
prepares source input without publishing a BIG or successful alpha receipt.

Both base and child may contain Armor entries when their Damage kinds are
distinct: Core appends base entries first, then child entries. This is proven
through imported/local chains and independent literal package equality; it is
not replacement of an existing GUN entry. See repository example
`samples/armor/source-append/mod.xml`. All three XML files must stay together
inside the selected mod data root. This documentation follow-up does not modify
historical evidence or claim new gameplay proof.

`uprising-alpha.paths` configures literal `key=absolute-path` lines:
PowerShell7, StaticManifest, GlobalManifest, Driver,
CompilerDirectory and UprisingXmlRoot (required for Conscript only).
Update Driver and CompilerDirectory after relocating this
workspace. Launching the UI is a separate test; no preparation/build command
starts a game or modifies a game installation.
Legacy four-key armor configurations remain accepted, but cannot build weapons
without the explicit GlobalManifest setting.
The local `EALAModStudio.exe.config` selects CLR4 rather than repairing the
machine's old CLR2 compiler. The user-run Reborn GUI success path passed with it.

Studio uses the live repository build driver. Backend intermediates use a
private `alpha/.work/<id>` directory and are removed after the build. The final
`alpha/builtmods/<name>/build.json` embeds the verified backend receipts;
their temporary paths are historical provenance, not current file links.
Intermediate/admission reports alone do not mean the build succeeded.
The SKU is a mod fragment, not a replacement stock SKU. No automatic install
or launch is provided. The armor observations above and the earlier diagnostic
AK47 Damage=50 trial are behavioral evidence for those specific candidates;
newly authored values/packages are initially offline validated.

The preparation inventory records source/copy hashes and origins. Existing EA
SDK files are copied locally from your supplied installation, not newly
published by this project. The project LICENSE does not establish rights for
all EA components or official schema material. Public redistribution review
is still open; do not upload this workspace as a reviewed release ZIP.

This candidate contains a focused compiler executable with additional developer
commands; only the documented armor, single-AK47 and bounded explicit Conscript
source backends are part of alpha scope. General model/texture/audio/map and
arbitrary GameObject/module builds remain unsupported. Diagnostic command
presence is not evidence that an asset type is enabled in Studio.

Older dated workspaces and the dependency-overlay route are historical
maintenance experiments, not additional user entry points. Compiler/fixture
provenance review still precedes public release.
