# Configured alpha relocation — October 11, 2026

## Outcome

The overlay now includes `scripts/Update-Ra3Ep1AlphaPaths.ps1`. After copying or
moving a configured workspace, close Studio/builds and run this helper from
the new copy. It derives Driver/CompilerDirectory from its own workspace, retains
valid external PowerShell7/StaticManifest paths or accepts explicit replacements,
and validates stock BIN/RELO/IMP siblings before changing configuration.

Exactly four literal path keys are accepted; missing/duplicate/unknown keys,
redirected paths, missing files, long/non-ASCII workspace paths and ambiguous
arguments are refused. Only uprising-alpha.paths is atomically replaced, with a
byte-exact uniquely named .bak file. A concurrent detected config change refuses;
the pending file is retained for inspection. Backups contain personal settings
and must stay outside distributed archives. Original samples, build outputs,
stock files, SDK originals and registry are not altered.

This improves relocation ergonomics, not self-contained portability. PowerShell
7, x86 .NET 8, .NET Framework 4 and unpacked Uprising stock streams still must
exist on the target computer. Copied configured workspaces already contain the
user-supplied EA/schema files; bare overlays still need initial setup. No helper
launches Studio/game or downloads/installs dependencies.

## Evidence

Overlay candidate `UprisingOverlay-20261011-c` contains 23 named files, plus
the relative manifest requiring eight external files. ZIP SHA256:
`F26DC30004DAC34871573F0263A7E99D4CA01DD23A436562C555ADB6D7004BC3`.
Fresh setup `UprisingOverlay-test-c` repeats the existing dependency/inventory,
5000/250 build and DTD/critical-marker controls successfully.

`Test-Ra3Ep1AlphaRelocation.ps1` copies the configured inventory and old config
to fresh sibling `UprisingOverlay-moved-c`; it does not move/delete the original
or recursively copy old build artifacts. An owned builtmods sentinel tests output
preservation. A nonexistent manifest override refuses with unchanged config and
no backup. Repair then succeeds: both internal paths point to the new copy,
the single backup matches original config bytes, the source config remains
unchanged and the sentinel survives. Both sample builds and existing negative
controls pass again in the relocated copy with the original known BIG digests.
An additional fresh `UprisingOverlay-moved-d` repeat also tests a duplicate
Driver key: it refuses without changing malformed config bytes or adding a
backup. After restoring the owned fixture's valid bytes, both sample builds
and the existing negative controls pass again.

These are same-computer offline relocation tests, not a new computer/runtime
installation test or GUI/game replay. Copying user dependency files does not
grant redistribution rights. Earlier workspaces remain unchanged and may lack
the new helper. Updated scripts/docs are committed to the user's repository;
candidate ZIPs remain local and PublicRedistributionReviewed remains false.
