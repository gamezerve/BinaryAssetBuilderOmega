# Uprising Mod SDK Alpha — live repository workspace

## Build your mod

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

Compiler maintenance refreshes `alpha/tools/compiler` after a successful
Release/x86 build of BinaryAssetBuilder or ManifestInspector. Your mod XML and
personal paths are preserved. Studio builds and synchronization share a lock to
avoid mixed compiler generations. An up-to-date toolchain does not expand the
supported input scope listed below. Internal maintenance scripts are not an
additional user workflow.

## Workspace ownership

Only two mod folders belong in the Studio list:

- `Reborn`: the user's mod. Developer tests do not change its sources or build its outputs.
- `RebornTest`: the developer's reusable test mod. Its current `data/mod.xml`
  contains the complete original-ID Conscript with **BuildTime=1**.

Select **RebornTest → Build Mod** to compile that Conscript from the selected
XML through Core, including complete native/metadata/reference/stream checks.
The output is `alpha/builtmods/RebornTest/RebornTest.big` and `mod_1.0.skudef`.
This is not a cached trial-BIG export. The generated BIG must reproduce the
previous game-observed candidate. A fresh run is not a new gameplay observation.

This Conscript route accepts only the exact reviewed original identity and
BuildTime=1 source profile. Other changes, arbitrary GameObjects, additional
assets and new-ID recruitment still fail; this is not a general unit compiler.
It additionally needs `UprisingXmlRoot` in the local paths file, pointing to the
matching official Uprising XML directory. Preserve the supplied complete XML;
unreviewed changes are rejected, not silently ignored. ALPHA005 marks this scope.

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
commands; only the documented fixed armor, experimental armor source and single-AK47
backends are part of the alpha scope. Model/texture/audio/map/GameObject builds
remain unsupported, except the exact original-ID Conscript BuildTime=1 profile
documented above.

Older dated workspaces and the dependency-overlay route are historical
maintenance experiments, not additional user entry points. Compiler/fixture
provenance review still precedes public release.
