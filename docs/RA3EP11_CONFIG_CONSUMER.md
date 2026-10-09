# EP1 1.1 config file consumer

This read-only audit extends [mod-config command-line evidence](RA3EP11_MODCONFIG_BASELINE.md)
for image SHA-256 `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
No game is executed or installed config changed.

## Actual consumer chain

After a nonempty `00CF2118` option buffer, the top-level slice joins the path
through `004D69A0`, calls `004D6F10` with a null metadata output, tests AL,
then calls `004D86B0` at `004D98A6` when the probe succeeds.

`004D6F10` is **not the directive parser**. It probes a direct path or resolves
relative candidates from the semicolon-separated `00CF1910` search path and
calls `004D6DC0`. That helper uses an enumeration object, tests its success
byte, optionally copies four metadata words and disposes it. Full enumeration
and archive-provider semantics are not yet claimed.

`004D86B0` derives a base directory, opens through `004D7E90` with flags 401h,
queries virtual slot 2Ch for size, allocates size+1, writes a null terminator,
reads through slot 0Ch, closes through slot 08h, then calls `004D9040` with the
buffer and base path. It returns true after processing an opened handle,
without checking the reviewed read result or individual directive outcomes.
Allocation failure handling and size bounds are not established by this body.

`004D9040` scans null-terminated lines, skips/trims CR/LF/tab/space and copies
each trimmed line through `004D5410` before dispatching `004D8DE0`. Its >400h
length diagnostic is conditional; the paths can continue to copying. Do not
treat it as an unconditional safe line-length rejection. No native fuzzing is
performed; detached diagnostic fixtures cap lines at 1023 ASCII characters.

## Nine directive prefixes

All prefixes contain a trailing literal space and use imported `_strnicmp`.
Argument text is everything after the compared prefix; full quoting and CRT
locale semantics are not inferred. Unknown prefixes return without a reviewed
diagnostic in this dispatcher.

| Prefix | Reviewed route | Scope |
| --- | --- | --- |
| `add-big ` | `00995160`, mode 2 | Path joined against config base; full mount helper open |
| `add-bigs ` | `004D8CC0`, mode 0 | Enumeration helper open |
| `add-bigs-recurse ` | `004D8CC0`, mode 1 | Enumeration helper open |
| `set-search-path ` | `004D7000` | Search-path writer; full body not part of these pins |
| `add-config ` | `004D86B0` | Recursive config read; no cycle guard proven |
| `try-add-config ` | `004D6F10` then `004D86B0` | Probe gates recursive read |
| `add-search-path ` | `0096A660` after join/append | Search-path update helper open |
| `add-str ` | `004D87B0`, owner `00CF2364` | Appends a reference-counted string value |
| `add-manifest ` | `004D87B0`, owner `00CF2354` | Appends a reference-counted string value |

`add-manifest` queues a string; it does not itself prove a manifest was mounted,
compiled, accepted or loaded. In particular, option-handler support and a true
config-read result are not interchangeable with a successful mod build/load.

## Migration implications and tests

A future minimal package needs a verified config-relative BIG path and a
verified manifest-queue consumer, with stock dependencies preserved. Use
short ASCII paths/lines for initial validation; do not pass untrusted recursive
config graphs to the game. These are conservative PoC choices, not claims
that all longer/non-ASCII inputs are rejected by the engine.

`Get-Ra3Ep11ConfigConsumer.ps1` pins eight regions (six complete helpers plus
the top-level slice and literal block). `Test-Ra3Ep11ConfigConsumer.ps1`, run
under PowerShell 7 with explicit `-ImagePath`, checks eleven detached prefix
fixtures, three diagnostic-policy rejections, eighteen detached byte faults
and stable repeat JSON. Policy checks do not execute the native parser.

Full BIG mounting, manifest-queue consumption, recursive-cycle safety,
production readiness and game loading remain false. No ProcessingHash guess,
guard relaxation or stock file change. Effort remains **52% / 48%**; 165
compiler groups are not rerun by this standalone audit.

Next: resolve `00CF2354` queue readers and `00995160` mounting behavior, then
construct a detached package preflight that can be validated before any game run.
