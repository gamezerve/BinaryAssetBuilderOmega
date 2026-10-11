# Isolated Uprising SDK Alpha workspace

## Outcome — October 11, 2026

The supported armor backend now runs from a prepared local workspace containing
prebuilt compiler files and focused schema fixtures, without a source tree or
SDK rebuild during each mod build. GUN-5000 and GUN-250 packages retain their
known digests. Fresh ZIP extraction into another directory passes both builds,
inventory preservation and DTD rejection. The user subsequently opened the
extracted Studio and built Reborn successfully; its screenshot/log and output
readback now add real GUI success evidence. This is not a publicly distributable
SDK release.

## Preparation

Run `scripts/New-Ra3Ep1AlphaWorkspace.ps1` in PowerShell 7 with an explicit fresh
`-OutputDirectory`, `-PowerShell7` executable and `-StaticManifest` path. Optional
`-StudioDirectory` supplies your existing SDK; the development default is the
local reference SDK folder. Source/reference files stay unchanged.

The output contains only named compiler/runtime files, the minimum current
armor schema graph and fixtures, the adapter, local help, two sample mods,
project license and explicitly supplied EA Studio/EALABuild/EULA files.
No game executable, archive, stock stream, native codec, PDB, source tree,
old `.guidata` or build/test directory is copied. Existing output and reparse
ancestry are refused. The preparer checks source/copy SHA256 and records origins
in `workspace-preparation.json`. The generated config-path length must fit the
existing conservative 240-character ASCII policy.

`Invoke-Ra3Ep1AlphaBuild.ps1 -CompilerDirectory <workspace>/tools/compiler
-OutputRoot <workspace>/builtmods` explicitly enables prebuilt mode. With no
CompilerDirectory, repository development mode still builds current source.
The Studio adapter passes those workspace locations automatically. Original
XML is held read-only, every build gets a fresh child, and final receipts say
whether prebuilt mode was used. The unpacked stock streams remain an explicitly
configured external read-only dependency; PowerShell 7 and x86 .NET 8 are not bundled.

## Actual loader findings and corrections

Read-only `ModStudioUI.loadBuildScript` IL (method token `0x0600000C`) declares
only `System.dll` and `EALABuild.dll` as CodeDOM references. The old adapter's
System.Xml configuration dependency therefore was not verified by the earlier
manual compilation with an extra System.Xml reference. The current adapter
uses `uprising-alpha.paths`: exactly four literal `key=absolute-path` settings,
with missing/unknown/duplicate keys refused. The old XML example is historical.

`processInputLine` (`0x06000028`) searches case-sensitively for `Critical:`,
`Error:` and `Warning:`. The previous uppercase diagnostic prefixes did not
match. The backend now emits `Warning: ALPHA001/ALPHA002` and `Error: ALPHA100`.
This follows inspected dispatch; actual colored Error Log rows remain unobserved.
The reference loader scans immediate mod directories, explaining the prepared
Reborn/Armor250 folder layout. The application also reads the optional RA3
UserDataLeafName registry value; the alpha backend does not depend on it or
write registry settings.

Because native CLR2 csc previously failed to start locally, only the isolated
copy receives `EALAModStudio.exe.config` selecting installed CLR4/CodeDOM.
The original Studio and system configuration are not changed. A console loader
harness compiled the adapter with the exact two declared references, discovered
and instantiated its EA interface, ran initialize, read all paths and checked
the two option labels/single-step scope successfully under CLR4. No GUI,
build callback, registry read or game launch is performed by that harness.
The subsequent user-run GUI build confirms CLR4 compatibility for this success
path, not every original Studio feature.

## Evidence

- `UprisingAlpha-20261011-b`: first prebuilt GUN-5000 build and exact-reference
  CLR4 CodeDOM/interface/config/options harness passed.
- `UprisingAlpha-20261011-c`: inventory checks, prebuilt 5000/250 builds,
  diagnostic prefixes, original file preservation and DTD refusal passed.
  Preparation into this occupied directory refused without overwriting it.
- `UprisingAlpha-extracted-a`: fresh ZIP extraction repeated the offline tests;
  no source build tree and no compiler rebuild log appeared.
- Final local candidate `UprisingAlpha-20261011-d` adds project-owned local help.
  Its 28-file ZIP extracted to `UprisingAlpha-extracted-b` and repeated all offline
  workspace checks plus the CLR4 exact-reference loader harness successfully.
  Local archive SHA256:
  `E3CB52CBE3657E5011A48D7FE0EA29BE001B2EA88B9A7972655FA93D457594CB`.
- GUN-5000 BIG SHA256:
  `DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC`.
- GUN-250 BIG SHA256:
  `E97EAEE9D2B423AC7360C9A401FAB1EC9A55B60F50B9F02BB74CEDC9E8B38F7E`.

`Test-Ra3Ep1AlphaWorkspace.ps1` reproduces workspace checks.
`Test-Ra3Ep1AlphaExtraction.ps1` stages only inventory files, excluding prior
outputs and live configuration, creates a LOCAL-USE-ONLY ZIP, extracts a fresh
sibling and regenerates path configuration before those tests. It retains
staging for inspection; nothing is deleted. The preparation provenance still
names the original workspace, while `extraction-test.json` separately records
the relocated tested path. Only the local ZIP is created, not a public release.

## User-run Studio success — October 11, 2026

The user supplied a screenshot of the actual EA window and its complete pasted
build log. Reborn appears selected; the Build Log shows `Build Completed` and
total time `00:00:01.8285614`. This was a manual user run, not an automated UI
observation. The output was independently read back at:

`UprisingAlpha-extracted-b/builtmods/RebornAlphaInput-44b8674bcb974d39ae1ead0867fa6319`.

`alpha-build.json` confirms GUN=5000, prebuilt compiler use, selected-input
admission, no fixed-recipe replay, no installed-file modification and no game
execution. The 3,552-byte BIG has SHA256
`DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC`,
identical to the earlier game-tested package. The log also records fifteen
entries and passed detached package controls. This closes the selected sample's
real GUI loading/dropdown/output/success-callback gate. It does not establish
warning tree classification, the GUI failure callback, local help rendering,
another coefficient's GUI build or a new gameplay run. Existing receipts keep
their original false runtime flags; no historical artifact was rewritten.

## Remaining gates / distribution limits

Next observe the GUI failure callback, Error Log categorization and local help.
Successful loading, Reborn selection, output capture and completion now have
user-provided GUI evidence plus independent package readback. The adapter currently
uses one combined validation/compile/package step, not every original SDK checkbox.
No game test is part of preparation or extraction. Changed coefficients still
need separately authorized runtime checks if new gameplay evidence is desired.

These local workspaces/ZIPs include user-supplied EA components and local absolute
provenance paths. They are not suitable public release uploads. Public packaging
requires rights/dependency review, removal of personal path metadata and likely
a user-supplied-SDK overlay/setup route. The repository MIT license is not assumed
to cover every EA component or official schema. Only project code/docs/templates
are committed and pushed by this step, not generated ZIPs or copied EA binaries.
The full compiler suite passed in the preceding editable-build step; this step
changes adapter/scripts/docs rather than compiler serialization and uses focused
workspace/loader/extraction tests instead of claiming another full-suite run.
