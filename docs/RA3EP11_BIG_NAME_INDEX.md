# EP1 1.1 scoped BIG name index

## Scope

This follows [BIG mount metadata](RA3EP11_BIG_MOUNT_METADATA.md), using the same
pinned 1.1 image, baseline and stock SKU. It reads directory metadata only.
It does not run native code, allocate an engine archive node, inspect archive
payloads or change compiler/game behavior.

Implementation: `scripts/Get-Ra3Ep11BigNameIndex.ps1`.
Regression: `scripts/Test-Ra3Ep11BigNameIndex.ps1`.

## Complete reviewed bodies

| VA | Raw offset | Bytes | Scope |
| --- | --- | ---: | --- |
| 00996880 | 596880 | 149 | Build eight-byte name index |
| 009960E0 | 5960E0 | 97 | Lowercase name hash |
| 00996150 | 596150 | 25 | Unsigned hash comparator |
| 009961F0 | 5961F0 | 87 | Header-class entry count |
| 00996450 | 596450 | 763 | Enumeration/name lookup dispatcher |
| 009962E0 | 5962E0 | 357 | Indexed name search |
| 00992DE0 | 592DE0 | 109 | ASCII case-insensitive name comparison |

Each complete body has an independent SHA-256 guard in the audit. The whole
image identity is checked by prerequisites and again at the end. The inspected
PE import-name table resolves `00BD8360` to `MSVCR80.dll!qsort` and `00BD859C`
to `MSVCR80.dll!tolower`; recorded original thunks and symbol bytes are checked
without loading either DLL. These offsets apply only to the pinned image.

## Construction and hash arithmetic

The mount's mode bit value 2 invokes `00996880` on archive-node+0Ch. The helper:

1. Obtains the entry count through `009961F0`.
2. Requests count times eight bytes through the allocator's virtual slot +08h.
3. Enumerates records through `00996450` with a null name and an ordinal.
4. Hashes each returned name with seed `1505h` (5381).
5. Stores a hash and the absolute directory-record pointer in each pair.
6. Publishes the array at index-object+08h and calls `qsort`, stride eight,
   using the unsigned hash comparator `00996150`.

For printable ASCII under the modeled lowercase mapping, the recurrence is
`h = (33 * h + lowercase(byte)) modulo 2^32`; the return value is byte-swapped.
This is a BIG **file-name** hash, not an asset type hash or compiler
ProcessingHash. No relation to authentic EP1 compiler identity is inferred.

ASCII case variants hash identically. Separators are not normalized by the
hash body: `data/static.manifest` gives `5B543C9C`, while
`data\static.manifest` gives `E8FC93C1`. Any normalization performed by outer
path preparation is a separate unresolved scope. Native `tolower` can depend
on CRT locale; the diagnostic rejects non-printable/non-ASCII names instead
of pretending to emulate that state.

The count helper uses a big-endian four-byte count at directory+08h for BIG
and Viv4 classes, and a big-endian two-byte count at +04h for class C0FB.
The detached reader remains restricted to validated BIG4/BIGF directories.
In the enumeration dispatcher, BIG4 uses two four-byte record fields; BIGF
maps the fourth magic byte's derived width to four. Alternative-width/native
trailer handling is not a general-format diagnostic implementation.

## Indexed lookup: collision caution

`009962E0` hashes the query and binary-searches the sorted unsigned hashes.
On an equal hash it first compares the query with the midpoint record's name
through `00992DE0`. If that comparison fails, it scans adjacent equal-hash
records in both directions.

The reviewed instructions pass the **midpoint candidate name**, not the
original query, to those adjacent comparisons: `00996391` and `009963CD`
retain ESI from the midpoint-name calculation at `0099634B..00996356`.
Consequently, a generic claim that this routine correctly resolves arbitrary
distinct-name collisions is not justified. This is an instruction-level
observation, not an executed exploit or an implemented correction. Higher
callers, duplicate selection, failure safety and archive precedence remain
unproven. The diagnostic reports this boundary explicitly.

The detached names `ar` and `c0` both give `38775900`, so collisions are not
merely hypothetical arithmetic. The fixture retains both records; it does not
claim to reproduce native qsort equal-key order or native collision resolution.

## Stock directory-only model and validation

Thirteen available configured archives contain 17,383 entries. Each directory
is re-read and matched to its preflight hash/length. A bounded parser models
the index from original name bytes, without slash replacement, checking count,
declared directory size, printable names, termination and payload range.

The report emits a deterministic SHA-256 of little-endian hash/record-offset
pairs sorted by unsigned hash, then directory-relative record offset. These
are **diagnostic relative pairs**, not native absolute pointers or a captured
native index. The explicit offset tie-break ensures repeatability and does
not assert native `qsort` stability.

There are six repeated-hash groups in MapsMultiplayer.big, corresponding to
its already observed repeated names. No distinct case-insensitive names share
a hash within any of the thirteen available stock directories. This inventory
does not prove collision-free authored mods or cross-archive precedence.

One audit invocation reads 5,390,165 directory metadata bytes, including its
prerequisites, and **zero archive payload bytes**. The large WorldBuilder BIN
is not read or decompressed. The configured MapsCampaign.big remains missing;
its Disabled-named neighbor is not substituted or renamed.

Regression checks eight independent hash constants, three name-policy
rejections, one two-entry collision directory, six malformed directories,
sixteen private image-byte faults and a second complete stable JSON result.
Native prepared array bytes, full path lookup/unmount/precedence, non-ASCII
locale semantics, authentic compiler ProcessingHash and game loading remain
false/unproven. No production guard is relaxed.

## Migration checkpoint

The next native package task is outer lookup/precedence and its path
normalization contract, not replacing compiler hashes with this unrelated
file-name hash. A usable SDK still requires compiler dependencies/layouts and
authentic EP1 hashes plus an authored package accepted in-game.
Effort remains approximately **52% complete / 48% remaining**. The 165 compiler
test groups were not rerun for this PowerShell-only evidence milestone.
