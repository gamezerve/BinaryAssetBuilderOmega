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
- `alpha/studio/uprising-alpha.paths.example`: current literal path configuration
  for PowerShell7/Driver/StaticManifest/CompilerDirectory. The adapter reads
  `uprising-alpha.paths`; the old XML template is historical only.
- `alpha/mods/Reborn/data/mod.xml`: familiar one-asset declaration sample.
- `scripts/Invoke-Ra3Ep1AlphaBuild.ps1`: accepts that declaration or standalone
  armor, bounds input at sixteen KiB, prohibits DTD resolution/includes and
  unexpected declaration contents, and invokes the existing admission gate.

The backend now packages **GUN 0..10000** after admitting selected XML,
through fresh Core identity generation and the actual scoped OutputManager.
It no longer replays the fixed-5000 fixture: `CanonicalFixedRecipeUsed=false`.
Only the GUN coefficient of the thirteen-entry flattened AlliedConYardArmor
is editable; this is not arbitrary asset-declaration compilation.
Development mode emits fresh repository-local output. The isolated workspace
passes explicit prebuilt compiler and `builtmods` output paths; no automatic
Documents installation is performed.

Diagnostics now use EA's inspected case-sensitive text prefixes:
`Warning: ALPHA001` for partial support,
`Warning: ALPHA002` for the narrow editable scope and `Error: ALPHA100`
for detailed failure, plus `Critical: ALPHA101` to trigger the failure callback.
EA Studio's process completion does not inspect ExitCode; only Critical sets
its stepFailed flag. User screenshots confirm warning/error categorization,
successful completion and, after this correction, the failure callback without
Build Completed. The repeat screenshot is of Build Log, not the critical tree
row; that row is not separately observed.

## Verification

The adapter compiled against the existing EA assembly with .NET 4 csc, then
again with explicit .NET 2 mscorlib/System/System.Xml references and no default
library/config references. This verifies source/API compatibility, not actual
Studio loading. The native .NET 2 csc executable failed to start in this
environment with a system DLL relocation error; no installation was repaired.
The successful fallback compile used backslash paths after an initial
forward-slash command was interpreted incorrectly by csc.

In the earlier fixed prototype, GUN-250 input produced a deliberate
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

The editable backend subsequently passed five actual package coefficients
(0/100/250/5000/10000), repeated GUN-250 byte equality, original-input preservation,
Core hash invalidation, single-float BIN changes and occupied/corrupt/mismatched
source refusals at `RebornAuthoredRegression-20261011-a`. The fixed-5000 package
still matches the user-tested SHA256. A complete standalone GUN-250 backend
build passed at `RebornAlphaInput-608b465d5c9d45f4a7ef5b663d282ff9`.
The backend builds current inspector source before use and captures compiler
diagnostics in `compiler-build.log`; `-StaticManifest` can override the current
local stock baseline default. The Studio adapter always passes an explicitly
configured stock path; its example configuration now requires all three paths.
Validate-only GUN-250 and a complete EA declaration GUN-5000 build also passed;
an unsupported armor identity returned `ERROR ALPHA100` with exit code 1.
The updated adapter compiled again with the .NET 2 reference assemblies.
These changes do not prove a real Studio GUI run.

Subsequent loader inspection found that EA CodeDOM references only
System.dll/EALABuild.dll and its dispatcher searches `Warning:`/`Error:`.
The adapter dropped System.Xml for four literal path settings and diagnostics
were corrected. Exact-reference compilation/interface/config/options passed
under CLR4; the isolated copy selects CLR4 through its own app configuration.
Workspace and fresh ZIP extraction builds passed without the source tree.
See [workspace evidence and distribution limits](RA3EP1_ALPHA_WORKSPACE.md).

## Remaining alpha gates

1. Editable XML binding to fresh Core/OutputManager/package output is completed
   for the narrow GUN-only scope; wider capabilities remain explicitly unsupported.
2. An isolated workspace and offline loader/extraction checks are prepared,
   leaving the reference SDK intact. The user's October 11 screenshot/full log
   confirm real GUI loading, Reborn discovery, output capture and successful
   completion; independent package readback matches the game-tested digest.
   A subsequent screenshot confirms ALPHA001/ALPHA002 under Warnings.
   ALPHA100 error classification and the corrected failure callback now also
   have user screenshot evidence. Local help and updated extraction remain. See the
   workspace report for the exact manual-run evidence and its limits.
3. Keep familiar Build Data/Make BIG/Create SKUDef stages while clearly marking
   unsupported LOD, STR, misc, map, hash-repair and copy/install features. The
   prototype's single operation is not yet full step-for-step UI parity.
4. Explicit stock/runtime/prebuilt paths, `builtmods` output and a clean extracted
   package passed offline. Public distribution/rights and metadata review remain.

The release objective is now an **EA-UI-compatible Uprising SDK Alpha with a
limited capability matrix**, not broad feature parity before the first alpha.
