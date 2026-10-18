# One live alpha SDK and experiment retention

## Stable development workflow

The repository's `alpha/` is the single local SDK workspace. Do not create a
dated alpha/overlay for each compiler change. Editable sources remain in
`alpha/mods/<name>/data/mod.xml`. Open `alpha/EALAModStudio.exe`
for the existing EA interface. EA components are local copies from the user's
reference SDK and are ignored by Git; this is not a redistribution clearance.

Successful Release/x86 builds of BinaryAssetBuilder build the alpha toolchain
project; direct ManifestInspector builds and solution builds also invoke
`Sync-Ra3Ep1DevSdk.ps1` and refresh
the same eight files under `alpha/tools/compiler`. Incomplete/failed compiler
builds never enter that target. Tool directories are staged, hash checked and
swapped with rollback while holding the shared SDK lock. An active cooperative
mod build holds that lock; sync waits up to 15 seconds and fails visibly rather
than silently mixing generations. Retry synchronization once the build ends.
`-p:SkipAlphaSync=true` is an explicit diagnostic opt-out, not the normal path.

For a local setup, run `scripts/Sync-Ra3Ep1DevSdk.ps1 -Initialize` in PowerShell
7 after building the compiler. Personal dependency/runtime paths are stored
in ignored `alpha/uprising-alpha.paths`. Existing settings and mod XML are not
silently replaced. Studio references the live repository driver, not a copied
stale script. This repository development SDK is not a standalone portable ZIP.

`Invoke-Ra3Ep1DevAlphaBuild.ps1` wraps the already qualified alpha backend. It
publishes `alpha/builtmods/<name>/<name>.big`, `mod_1.0.skudef`, `build.json` and
`last-build.log`. No CFG is emitted in that final directory. The short fixed
SKUDef basename avoids the observed Uprising identity-length restriction.
Each successful build replaces only the previous complete managed output for
that mod; failures leave the last good package intact. Unknown output files
cause refusal, not deletion. Sources and game installations are not modified.
Backend intermediates stay under a fresh `alpha/.work/<id>` and are removed
afterward; latest failure/validation logs remain. Embedded backend receipts
retain historical temporary paths for provenance, not usable live file links.

Supported input remains the existing fixed-yard, combined-yard, AK47 and
experimental armor-source profiles. An up-to-date compiler does **not** make
every developer diagnostic an alpha build capability. General GameObject,
art, FX/audio/map compilation and new-ID recruitment are not silently enabled.

## Validation of this integration

Both direct ManifestInspector and direct BinaryAssetBuilder Release/x86 builds
passed with zero warnings/errors, including automatic synchronization. The
12-case development SDK check covers all six supplied samples, deterministic
repeat output, a real XML edit changing the BIG, malformed XML preserving the
last good package, and refusal while the shared SDK lock is held. Final outputs
are checked for the exact SKUDef bytes, receipt/package hash agreement and no
CFG counterpart. This is offline integration evidence, not new gameplay proof.

The existing exact-reference CLR4 CodeDOM loader harness passed against the
stable alpha root: adapter compilation, interface, configuration, three option
entries and step selection. It does not execute build callbacks. No Studio window or game was
launched for that check. `BuildMod` / `OpenSDK` helper files are internal
maintenance conveniences only; the user's entry remains EALAModStudio.exe.

## Preserved lessons before cleanup

- Uprising linked streams use the separately reviewed EP1 v7 type identities,
  hashes and tokenized layouts; legacy KW marshalers cannot be accepted merely
  because they validate the XML. Fourteen root defaults required correction.
- Armor GUN/CANNON and selected AK47 changes have user-observed gameplay
  evidence. Fresh packages/other values are not automatically gameplay-proven.
- The actual original Conscript has a qualified 600-byte root, 49 modules,
  six populated owners, geometry and 275 ordered strong occurrences on one
  Tracker. Full metadata covers 126 fields and 21 disjoint owner intervals.
  One bounded Script CRLF/LF equivalence is explicit, not a general ignore rule.
- Full Conscript BuildTime 4 -> 1 was compiled from source, serialized and
  packaged. The user observed approximately one-second production in game.
  This is original-ID override evidence, not proof every module/LUA path ran.
- A separate SKUDef-only candidate worked without a CFG counterpart. Final
  live SDK outputs therefore use BIG + SKUDef. Historical CFG experiments are
  retained as evidence, not a required second game configuration format.
- A compiled new Conscript identity is not automatically recruitable: menu/
  CommandButton/CommandSet or explicit spawn integration remains separate.
- Short basename/absolute ASCII path policies matter. The long intermediate
  Conscript paths were 256/258 characters; the successful isolated SKUDef was
  163. The mod SKU is a fragment, never the stock game SKU replacement.
- GUI failure propagation requires Critical output, not just a child exit
  code. Exact CLR4/CodeDOM references and the local exe.config were qualified;
  unsupported XML must fail, never be silently skipped.
- Runtime command-line snapshots are not proof of config reads or overrides.
  Human gameplay feedback is separate from immutable raw observations.

Detailed derivations remain in the existing technical reports, tests and Git
history, especially RA3EP1_CONSCRIPT_AUTHORED_TRIALS.md and the SDK release plan.

## Retention policy

`Compress-Ra3Ep1LegacyExperiments.ps1` plans by default; `-Apply` is explicit.
Targets are only marked old UprisingAlpha/UprisingOverlay siblings and
unreferenced artifact child directories. `sources/`, the reference RA3 SDK,
original schema/XML/game installations and editable live SDK mods are excluded.
Active Studio workspaces, runtime ETL/minidumps and source-code-pinned artifact
inputs are retained. Three historically pinned armor packages are copied,
digest-checked and referenced via `artifacts/LegacyLaunches/` before removing
their former full SDK snapshots.

All contents of selected obsolete directories, including differing mod files,
are saved in `.maintenance/cleanup-<id>/complete-experiments.zip`. Before any
deletion, every archive entry's length/SHA-256 is compared with the original
inventory, source size/timestamps are rechecked and paths/reparse descendants
are revalidated. The inventory and final result are saved next to the ZIP.
Restore by extracting the archive under the original ChatGPT project workspace
root using `inventory.json` for the path map. This full local backup is ignored
by Git; it is not a public release or a permanent requirement to keep every
generated DLL uncompressed. Cleanup does not erase the only copy of mod inputs.

The initial plan selected 311 generated directories / 117,989 files /
2,527,122,802 bytes, before compression, while retaining live dependencies.
Actual completion metrics are recorded after verified cleanup, not inferred
from this plan.
