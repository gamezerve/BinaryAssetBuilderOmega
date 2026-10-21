# Versioned Studio final packaging

<!-- Reborn: distinguish qualified compiler streams from loose-file packaging and from game execution evidence. -->
Studio's existing EA `GUIElementData.InputBoxType` supplies `modversion`, default
`1.00`. Numeric versions with two to four components (one to four digits each)
are accepted verbatim. Thus `1.00` is not normalized to `1.0`.

The default destination uses `Environment.SpecialFolder.MyDocuments`, respecting
Windows/OneDrive redirection, followed by `Red Alert 3 Uprising/Mods/<name>/`.
RebornTest version 1.00 yields:

- `RebornTest_1.00_Streams.big`: unchanged qualified backend stream archive.
- `RebornTest_1.00_Misc.big`: only when `alpha/mods/RebornTest/Misc/` has packageable files.
- `RebornTest_1.00.skudef`: `add-big` Streams then optional Misc, CRLF, relative filenames.

This is a mod SKUDef fragment, not a replacement game SKU. No game version,
game installation configuration or launch command is changed. Different versions
coexist. The prior short SKU basename test does not establish runtime compatibility
of every new public name; no new gameplay claim is made by packaging tests.

## Misc semantics

The official local reference's `defaultscript.cs` step 10 calls makebig with
the Misc folder as root and `-x:*.asset`. The new streaming BIG4 writer uses
the same root-relative backslash entry names and case-insensitive `.asset`
exclusion. For example `Misc/Data/Scripts/example.lua` becomes
`Data\Scripts\example.lua`, not `Misc\Data\Scripts\example.lua`.
All other loose content is copied as-is, not compiled. Unsupported XML, models
or audio do not gain compiler support by being placed here.

Missing/empty Misc, nested directories without files and excluded-only `.asset`
content produce no Misc BIG and no Misc SKUDef line. A zero-byte distributable
file still counts as a file and is packaged. Successful same-version rebuilds
remove an earlier owned Misc BIG when Misc is now absent/empty. Names must be printable ASCII;
duplicate case-insensitive names and redirected paths fail. The actual Windows
reparse tag distinguishes OneDrive cloud placeholders (accepted) from junctions,
symbolic links and unknown reparse tags (refused); the ReparsePoint attribute
alone cannot make this distinction. The BIG4 32-bit
size/offset limit is enforced; payload copying/hash readback uses bounded buffers,
not a whole-archive allocation. Source hashes and stored directory/payload hashes
are checked before publication.

## Publication and retention

The target directory keeps a `.reborn-publish.lock` coordination file and a
per-version `.<name>_<version>.package.json` ownership/hash receipt. A matching
same-version rebuild replaces only its verified two or three files and receipt.
Earlier three-file receipts, including the old empty Misc BIG, are accepted for
safe migration; unknown or modified Misc files are never silently removed.
Changed/unmanaged collisions fail; unrelated files and other versions remain.
Exclusive locks serialize publishers across SDK copies. Staging resides on the
destination volume; handled replacement failures restore previous files. If
rollback itself fails, the recovery staging directory is explicitly retained.
An external forced termination/power loss is not a filesystem-wide transaction;
the retained staging files are recovery material, not automatic startup recovery.
Studio's existing cancellation/publication guard stays held throughout this step.

`alpha/builtmods/<name>` is internal compatibility output, not the final user
destination. Its receipt links the `FinalPackage` paths. If the final package
commits but internal cache/log refresh fails, ALPHA112 reports that distinction
rather than falsely claiming no publication occurred. No source or game
installation files are modified.

Developer regressions set an explicit isolated final root under `alpha/.work`;
they never publish fixtures into the user's Documents mods. The helper check
covers names/SKU, nested Misc paths, `.asset` exclusion, deterministic rebuild,
other versions/notes, empty Misc, invalid versions, modified/unmanaged collisions,
partial-move rollback and concurrent-publication refusal. A real RebornTest
source build also verifies integration with the unchanged qualified stream bytes.
The optional-Misc Windows run passed 17 checks, including a real junction refusal and a
read-only check of this machine's OneDrive cloud-tagged Documents path. Studio's
adapter compiles against the shipped EALABuild API, and the readable-log
regression passes. The stream archive still hashes to
`87241958D0EC4CDB3A133FFC837E023A01B51716924F6B532A30BCBDE7EDF918`.
The full source-to-final build also completed at the actual known-folder location
`D:\OneDrive\Documents\Red Alert 3 Uprising\Mods\RebornTest` with version 1.00.
The first candidate had an empty Misc BIG; that earlier policy is superseded by
the optional-Misc behavior above, which removes that owned archive on rebuild;
nonempty nested/binary Misc payloads were verified in the isolated helper tests.
No game was launched for these packaging changes.
