# EP1 1.1 scoped provider routing and BIG registration

## Scope

This connects [BIG open selection](RA3EP11_BIG_OPEN_SELECTION.md) to the
filesystem provider router. Inputs retain the full pinned 1.1 image, baseline,
SKU and stock directory guards. All inspection is read-only; the game, native
providers and allocators are not executed. Detached routing fixtures operate
on supplied provider aliases, not actual runtime objects.

Implementation: `scripts/Get-Ra3Ep11ProviderRouting.ps1`.
Regression: `scripts/Test-Ra3Ep11ProviderRouting.ps1`.

## Independently guarded evidence

| VA | Raw offset | Complete bytes | Role |
| --- | --- | ---: | --- |
| 0096A050 | 56A050 | 372 | Path qualification and prefix-to-provider selection |
| 0096A8A0 | 56A8A0 | 98 | Append provider registration |
| 00969DA0 | 569DA0 | 70 | Append interface alias |
| 00969C60 | 569C60 | 99 | ASCII case-insensitive alias comparison |
| 00995690 | 595690 | 43 | BIG interface constructor |
| 00969F80 | 569F80 | 36 | Base interface constructor and initial alias |
| 00969DF0 | 569DF0 | 113 | Registration-node constructor |

Seven complete bodies have independent SHA-256 pins. Additional pins cover
only the explicitly scoped startup slice `004D980C..004D9836` (43 bytes) and
the first four slots of two static vtables. They are not whole startup-function
or whole-vtable coverage. The `null:` literal at `00C7F1D0` is checked; earlier
BIG-open prerequisites independently guard `big:` at `00C84EAC`.

## Provider selection differs from archive selection

`0096A050` extracts the first colon and includes that colon in the prefix
token. With a non-empty token it traverses context provider registrations
from context+00h and each registration's interface at +C0h. It traverses that
interface's alias list from +04h, comparing each alias at alias-node+04h
through `00969C60`. ASCII case differences are accepted.

The first matching provider registration wins. An unknown explicit prefix
returns null, not the context default. A registration with null interface
terminates the scan rather than skipping to later registrations. Without a
prefix token the function returns context+28h, the supplied default provider.
Thus `BIG:` may select the BIG provider at this layer, although the BIG-open
body's own prefix stripping is exact lowercase `big:`. The complete calling
path between these layers must be checked before claiming the uppercase form
opens a particular asset.

The same function has an optional output-path qualification branch before
prefix selection. It copies a context base path, handles a leading `./` or
`.\`, root-leading separators and a colon-qualified input. The detached helper
intentionally models only **already-qualified prefix selection**, not that
qualification branch or every caller's path normalization. In particular a
synthetic `D:\file` query has prefix `D:` and no default fallback if no such
alias was supplied. This is not a claim about the application's full disk
path handling.

## Registration and aliases

`0096A8A0` requests a 208-byte registration, invokes `00969DF0`, then appends
it to the context list: head +00h, tail +04h, count +08h, next link at
registration+00h. The constructor stores the interface at registration+C0h.
Append order therefore affects first-prefix-match selection, but actual
startup registration order has not been reconstructed in full.

`00969DA0` requests a 20-byte alias node, copies the supplied NUL-terminated
name at +04h and appends it to interface head +04h/tail +08h/count +0Ch.
The copy has no explicit length bound in the reviewed body. Allocation failure
safety is also not established: the registration's allocation-null path later
reaches a store through the selected node pointer. Detached diagnostics do not
emulate those unsafe writes or invoke the engine allocator.

## Scoped startup-to-BIG bridge

The pinned startup slice passes allocator `00CF2374` to BIG constructor
`00995690` at `004D9813`, overrides the interface vtable with `00BF9D78`,
stores the interface in global `00CF231C`, then registers it with flag zero
through `0096A8A0` at `004D9832`. The returned registration is subsequently
stored at `00CF2320`; the store itself is visible in surrounding disassembly,
outside this deliberately bounded 43-byte startup pin.

The BIG constructor invokes base constructor `00969F80` with the `big:` alias
literal, then installs base BIG vtable `00C84E60`. In both base and overridden
vtables, virtual slot +0Ch points to `009956D0`, the previously audited BIG
open body. This establishes a static registration/interface/open bridge, not
that startup ran successfully or that an authored package loaded.

Initialization disassembly also shows a `null:` alias associated with builtin
context setup near `0096AB24..0096AB4A`. This report checks its literal only;
it does not recover the builtin provider's complete behavior or certify all
default-provider initialization paths. The report's `BuiltinDefaultAlias`
field is static alias evidence, not a runtime default object observation.

## Tests and remaining migration work

Eight detached fixtures cover unprefixed default selection, case-insensitive
BIG prefix, a secondary alias, a later provider, unknown prefix, drive-like
prefix, null-interface early termination and a null supplied default.
Three printable-ASCII/length policy rejections and twenty private byte faults
cover every reviewed body, startup/vtable slices and the literal. Stable
nested report fields match a second JSON run.

Prerequisites still cover thirteen available stock archives and 17,383 entries;
each invocation reads 5,390,165 directory metadata bytes and zero archive
payload bytes. The large WorldBuilder BIN is untouched. Missing configured
MapsCampaign.big remains separate; its Disabled-named neighbor is not renamed
or substituted.

Next: inspect real provider-open callers and path qualification, then the
complete startup/config order rather than treating SKU inventory order as
mount precedence. Native buffer/allocation safety, global provider precedence,
authentic EP1 compiler ProcessingHash, compiler dependencies and a successful
authored in-game mod remain unresolved. No installed file, registry setting
or production compatibility guard was changed.

Effort remains approximately **52% complete / 48% remaining**. The 165 compiler
test groups were not rerun for this PowerShell-only milestone.
