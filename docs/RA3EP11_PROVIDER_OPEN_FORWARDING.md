# EP1 1.1 scoped provider-open forwarding

## Scope and evidence

This follows [provider routing](RA3EP11_PROVIDER_ROUTING.md). The same pinned
1.1 image, baseline, SKU and stock-directory guards remain required. No native
provider, allocator, game or codec is executed. Detached plans use supplied
resolved providers and hit booleans, not native stream handles.

Implementation: `scripts/Get-Ra3Ep11ProviderOpenForwarding.ps1`.
Regression: `scripts/Test-Ra3Ep11ProviderOpenForwarding.ps1`.

| VA | Raw offset | Complete bytes | Role |
| --- | --- | ---: | --- |
| 0096BA00 | 56BA00 | 412 | Initialize/open provider-backed wrapper |
| 0096ABC0 | 56ABC0 | 58 | Route, open, inspect handle and clean up existence probe |

Both complete bodies have independent SHA-256 pins. The exact NUL-terminated
join format `%s/%s` at VA `00C7F1E0` is checked separately. Whole image identity
is checked by prerequisites and after the audit.

## Explicit provider path

`0096BA00` receives path, open option and an already-selected registration.
It initializes the wrapper: native handle at +08h, registration +0Ch and
interface +10h (loaded from registration+C0h). It removes one leading `./`
or `.\` from the path, then compares the registration to context+28h.

For a non-default registration it directly calls interface virtual slot +0Ch
with that path, original open option and an output pointer. Interior slashes,
prefix spelling, dot segments and repeated separators are not normalized by
this branch. A successful returned handle is stored at wrapper+08h; interface
slot +20h supplies the low/high size stored at wrapper+18h/+1Ch.

Earlier evidence ties BIG's +0Ch slot to `009956D0`. Composition therefore
matters: provider routing accepts ASCII case variants of `big:`, while BIG's
own preparation strips exact lowercase `big:`. The forwarding branch does not
lowercase it. A detached `BIG:/data/file` query selects the supplied BIG
provider but reaches BIG preparation still containing `BIG:`. That is a
reviewed path distinction, **not** a successful runtime test or proof of all
callers' behavior. Similarly, interior backslashes survive this forwarding.

The existence probe `0096ABC0` calls router `0096A050` with a null qualification
output, passes the selected registration and option zero to `0096BA00`, tests
the wrapper handle and calls cleanup `0096BBA0`. Thus it does not request the
router's optional output-path qualification. It routes **before** the wrapper
removes a leading dot-separator: the separate forwarding fixture for
`./big:/...` must not be mistaken for proof that this complete probe accepts
that prefix form.

## Default provider path

When the selected registration equals context+28h, the wrapper reads the
search count at context+38h and eight-byte search pairs at context+3Ch. Each
pair holds a root string pointer and a registration pointer.

It attempts these pairs in increasing index order. A null root pointer stops
the loop, rather than skipping an entry. If a pair's registration is the
default registration, the code substitutes that registration's next link.
The detached plan takes already-resolved target identities and does not
emulate this pointer substitution or claim it is always safe.

A relative query is joined exactly with `%s/%s`; a query beginning with `/`
or `\` is copied without the root. This is concatenation, not path cleanup:
a root ending with `/` produces a double slash. The first non-null handle
stops further attempts. With no search entries, no provider open occurs;
there is no extra raw default-provider fallback in this reviewed body.

This reveals a second ordering mechanism beyond registered provider aliases
and BIG's internal archive list. None of those static algorithms establishes
the actual startup search-pair contents, registration order or mod-over-stock
priority without recovering the initialization/config callers.

## Boundaries and tests

The native wrapper dereferences its supplied registration before a null check.
No wrapper null-registration safety is established when routing returns null.
Native stack path construction uses input-derived copying/formatting; buffer
safety is not proven. Output-pointer tagging and cleanup helpers are visible
but full wrapper/stream lifetime is not recovered. No unsafe behavior is
reproduced by the detached models.

Six forward-path fixtures, eight supplied-hit search plans and four composed
routing/forwarding/BIG-prefix fixtures pass. Three policy rejections cover NUL,
non-ASCII and paths longer than the conservative 255-character diagnostic
bound; it is not an asserted engine limit. Eighteen private code/literal faults
are rejected, and every stable nested field matches a second JSON run.

Prerequisites cover thirteen available archives and 17,383 directory entries,
reading 5,390,165 metadata bytes per invocation and zero archive payload bytes.
WorldBuilder's large BIN is not dumped/decompressed. Configured MapsCampaign.big
is still missing; the Disabled-named file is not substituted or renamed.

Next: recover the search-path pair producer and complete config/startup order.
Authentic EP1 ProcessingHash, compiler dependencies/layouts and an authored mod
accepted in-game remain separate release gates. No production guard, installed
game file or registry setting was changed. Effort stays approximately
**52% complete / 48% remaining**; the 165 compiler test groups were not rerun
for this PowerShell-only milestone.
