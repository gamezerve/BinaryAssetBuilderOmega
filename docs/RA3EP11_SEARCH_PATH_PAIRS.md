# EP1 1.1 scoped search-path pair rebuilding

## Evidence and scope

This follows [provider-open forwarding](RA3EP11_PROVIDER_OPEN_FORWARDING.md).
The same 1.1 image, baseline, stock SKU and directory guards remain required.
The game and native allocators are never executed; detached fixtures do not
observe actual initialized search paths or native provider handles.

Implementation: `scripts/Get-Ra3Ep11SearchPathPairs.ps1`.
Regression: `scripts/Test-Ra3Ep11SearchPathPairs.ps1`.

| VA | Raw offset | Complete bytes | Role |
| --- | --- | ---: | --- |
| 0096A660 | 56A660 | 199 | Rebuild copied root strings and registration pairs |
| 004D7000 | 0D7000 | 273 | Config search-text writer and rebuild bridge |

Both complete bodies have independent SHA-256 pins. Whole-image identity is
checked through prerequisites and again after the audit.

## Rebuild, not append

`0096A660` clears context+2Ch's text buffer using the capacity at +34h, then
clears the pair buffer at +3Ch using eight times the field at +38h. It copies
the supplied NUL-terminated text into the text buffer, splits it in place on
semicolons and writes eight-byte pairs in increasing input order. A pair
contains a pointer into that copied buffer and the registration returned by
`0096A050` for the root.

The field at +38h is a configured **pair-capacity/iteration bound**, not an
active-token counter written by this routine. The builder never updates it.
The previously audited open wrapper iterates up to this bound but stops at a
null root pointer in the cleared buffer. Earlier reports' search-count field
describes that loop bound, not a freshly measured number of active paths.

The routine rebuilds the entire list on each call. An `add-search-path` caller
may append to an external text string before calling it; the builder itself
does not append to previous pairs. Duplicate roots remain separate entries.

## Root transformation and resolution order

For an intermediate token ending in `/` or `\`, only the final separator is
replaced with NUL before provider resolution. The semicolon is also replaced
with NUL. The final token is first resolved and stored, then has at most one
trailing separator removed. No whitespace trimming, interior slash conversion,
case conversion, dot-segment cleanup or duplicate elimination occurs in this
body. Buffer-relative root offsets retain the original token positions.

For example `BIG:/a//` becomes `BIG:/a/`, not `BIG:/a`. The next wrapper's
exact `%s/%s` join may therefore still produce double slashes. Case-insensitive
provider selection does not rewrite the stored root's prefix spelling.
Unknown explicit aliases can produce null registrations; the builder stores
the result without checking for null.

## Config bridge and safety boundary

`004D7000` resets global search text `00CF1910`, prepares semicolon-separated
config tokens through copy/tokenization and path-join calls, writes the
resulting text, and calls `0096A660` at `004D7104`. This closes the static
`set-search-path` writer-to-pair-builder link. Complete config tokenization,
join semantics and the sequence of all startup/config updates are not modeled
by the detached helper; a whole-body pin is not proof that those paths ran.

Native text copying and pair stores have no per-token capacity checks in this
reviewed builder. Leading/adjacent/trailing semicolons and empty strings also
reach previous-byte reads during trailing-separator inspection. Their safety
is not established. Detached parsing rejects empty or separator-only roots,
NUL/non-ASCII text and capacity overflow instead of emulating unsafe reads.
Its conservative limits are 1,023 input characters, 1..100 supplied pair slots
and the prerequisite provider router's 255-character per-token bound. These
are diagnostic policy limits, not recovered universal engine limits.

## Tests and release boundary

Six pair fixtures cover order/offsets, default resolution, duplicate roots,
whitespace, prefix case, repeated separators and unknown aliases. One rebuild
fixture ensures old paths do not survive; one supplied-hit composition fixture
passes resolved pairs to the previous default-open plan. Nine policy rejection
cases, sixteen private native-byte faults and complete stable repeat JSON pass.

Prerequisites retain thirteen available archives and 17,383 entries, reading
5,390,165 directory metadata bytes per invocation and zero archive payload
bytes. The large WorldBuilder BIN is not read/decompressed. Missing configured
MapsCampaign.big and its untouched Disabled-named neighbor remain separate.

Next: complete config/startup update ordering and the real search-path contents,
then validate an authored package. Authentic EP1 ProcessingHash, compiler
dependencies/layouts and successful in-game mod loading remain open. No
production guard, installed game file or registry setting was changed.
Effort remains approximately **52% complete / 48% remaining**; the 165 compiler
test groups were not rerun for this PowerShell-only milestone.
