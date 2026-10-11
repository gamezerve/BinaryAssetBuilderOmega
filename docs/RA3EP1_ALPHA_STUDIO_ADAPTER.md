# EA Mod BuildStudio alpha backend prototype

## User-selected direction

Retain the existing EA RA3 Mod BuildStudio interface and its selected-mod,
XML/build/log/output workflow. Release a limited alpha with visible capability
warnings instead of waiting for every type to be complete. Unsupported input
must still fail the build: warnings are not permission to omit assets silently
or publish a broken stream family.

## Inspected integration seam

The local reference `Working RA3 Compiler for Reference/defaultscript.cs`
implements `EALABuildScript`. It is a Bibber extended script, not an untouched
EA default. Its `buildMod` picks `mods/<name>/data/mod.xml`; step 6 invokes BAB,
step 10 creates BIGs, step 11 writes SKUDef. Other steps include destructive
cleanup, RA3 registry assumptions, hash/merge/sort helpers and map compilation.
None of that script was changed or executed here.

Read-only assembly metadata confirms `EALABuild.dll` exposes `initialize`,
`buildMod`, success/failure callbacks, option descriptions, `DisplayLine`,
`RunStep`, `OnBuildComplete`, and `RunExecutableArguments` output-redirection
and window-control properties. The local SDK documentation describes Build
Output and Error Log panes. These are the adapter seam, not evidence of a
completed UI integration or identical warning-parser behavior.

## Added prototype

- `alpha/studio/defaultscript.cs`: implements the existing interface and
  starts the reviewed PowerShell backend as one build step. It sets output
  redirection and no console window; no game/registry/install/cleanup actions.
- `alpha/studio/uprising-alpha.example.xml`: explicit runtime/backend path
  configuration template. The prototype reads `uprising-alpha.xml` from the
  isolated Studio working directory. Placeholder paths must be replaced.
- `alpha/mods/Reborn/data/mod.xml`: familiar one-asset declaration sample.
- `scripts/Invoke-Ra3Ep1AlphaBuild.ps1`: accepts that declaration or standalone
  armor, bounds input at sixteen KiB, prohibits DTD resolution/includes and
  unexpected declaration contents, and invokes the existing admission gate.

The backend currently packages **GUN 5000 only** after admitting equivalent
selected XML, by rebuilding the known canonical fixture. It explicitly warns
about that restriction. Other coefficients can be valid at the admission
level yet not package-enabled. The receipt records selected-input provenance
and `CanonicalFixedRecipeUsed`, rather than claiming arbitrary compilation.
It emits fresh repository-local output, not a release-style `builtmods` tree
or automatic Documents installation yet.

Diagnostics are explicit text: `WARNING ALPHA001` for partial support,
`WARNING ALPHA002` for the canonical-recipe restriction and `ERROR ALPHA100`
for failure. A nonzero backend exit must trigger Studio's failure callback.
Build Output redirection is requested; Error Log parsing/coloring and actual
callback behavior still need a real UI test. Do not claim these diagnostics
already appear as correctly categorized Error Log rows.

## Verification

The adapter compiled against the existing EA assembly with .NET 4 csc, then
again with explicit .NET 2 mscorlib/System/System.Xml references and no default
library/config references. This verifies source/API compatibility, not actual
Studio loading. The native .NET 2 csc executable failed to start in this
environment with a system DLL relocation error; no installation was repaired.
The successful fallback compile used backslash paths after an initial
forward-slash command was interpreted incorrectly by csc.

Fixed-5000 input passed validation-only. GUN-250 input produced a deliberate
nonzero refusal, distinguishing valid input from unavailable packaging.
The EA-style `AssetDeclaration` sample also passed validation-only. A complete
backend run succeeded at `RebornAlphaInput-23013c8409c44312b18b174ccb99a23d`;
its linked package is `RebornArmorDefault-d36175ddadc945f78c9dbe910c67c5a9`
and matches the earlier user-tested digest. The callback step-counter protocol
was checked against `GetNextExecutableStep` and `RunStep` IL, then retained
in the adapter. This is static evidence, not an observed GUI callback run.
No Studio/game window has been opened, reference SDK file replaced, registry
written or game install modified. Compiled test DLLs stay in local artifacts;
the EA binaries are not newly bundled or published by this step.

## Remaining alpha gates

1. Connect admitted editable XML to fresh Core/OutputManager/package output.
2. Prepare an isolated Studio workspace, leaving the existing reference SDK
   intact; verify script loading, mod dropdown discovery, callback/exit status,
   output capture and Error Log classification with a real UI test.
3. Keep familiar Build Data/Make BIG/Create SKUDef stages while clearly marking
   unsupported LOD, STR, misc, map, hash-repair and copy/install features. The
   prototype's single operation is not yet full step-for-step UI parity.
4. Parameterize stock/runtime locations, normalize outputs under `builtmods`,
   then test a clean extracted package and review distribution contents.

The release objective is now an **EA-UI-compatible Uprising SDK Alpha with a
limited capability matrix**, not broad feature parity before the first alpha.
