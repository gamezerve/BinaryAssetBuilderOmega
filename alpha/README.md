# Uprising Mod SDK Alpha — local workspace candidate

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
CANNON=5000 and stock GUN=100. SovietArmor uses Soviet GUN=5000. Allied GUN/CANNON
and Soviet GUN have user-observed gameplay evidence. CombinedArmor uses Allied
GUN=250 and Soviet GUN=5000 in one stream; Soviet has positive user-observed damage,
while Allied GUN=250 is inconclusive. CombinedStrong uses GUN=5000 for both;
its Allied Peacekeeper test is positive, but Soviet was not re-tested in that
revision. These are qualitative observations, not exact multiplier measurements
or proof of every new build. Duplicate identities fail the whole build.

Also supported, separately from armor: one complete stock-shaped
`SovietAntiInfantryInfantryAK47` WeaponTemplate. Select ConscriptAK47 (Damage=50)
and edit only Damage in mods/ConscriptAK47/data/mod.xml. Integers 0..10000 with
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

Experimental armor source route: select `ArmorSource`. Its three-file graph
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

`uprising-alpha.paths` configures five literal `key=absolute-path` lines:
PowerShell7, StaticManifest, GlobalManifest, Driver and
CompilerDirectory. Update Driver and CompilerDirectory after relocating this
workspace. Launching the UI is a separate test; no preparation/build command
starts a game or modifies a game installation.
Legacy four-key armor configurations remain accepted, but cannot build weapons
without the explicit GlobalManifest setting.
The local `EALAModStudio.exe.config` selects CLR4 rather than repairing the
machine's old CLR2 compiler. The user-run Reborn GUI success path passed with it.

For an offline backend build, run from this workspace in PowerShell 7:

```powershell
# Reborn: compile only the supported sample into a fresh builtmods child; do not install or launch.
& .\scripts\Invoke-Ra3Ep1AlphaBuild.ps1 -ModXml .\mods\Reborn\data\mod.xml `
  -StaticManifest 'C:\YOUR\UNPACKED\data\static.manifest' `
  -CompilerDirectory (Join-Path $PWD 'tools/compiler') `
  -OutputRoot (Join-Path $PWD 'builtmods')
```

Output is a fresh `builtmods/RebornAlphaInput-<guid>` directory. The final
`alpha-build.json` points to a verified BIG/config/SKU package and its authored
receipt. Intermediate/admission reports alone do not mean the build succeeded.
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
remain unsupported.

For the separate dependency overlay candidate, see OVERLAY.md when supplied.
It requires user-supplied matching EA components/schema files rather than
bundling them. Compiler/fixture provenance review still precedes public release.
