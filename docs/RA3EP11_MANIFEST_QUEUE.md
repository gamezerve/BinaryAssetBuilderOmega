# EP1 1.1 config manifest queue to reader bridge

Static, read-only evidence for the pinned 1.1 image SHA-256
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
Extends [config consumption](RA3EP11_CONFIG_CONSUMER.md) and
[reader ownership](RA3EP11_READER_OWNERSHIP.md). No game is executed.

## Queue consumers

Scanning the reviewed text section for the literal address `00CF2354` yields
five raw candidates: `004D9019` (dispatcher owner), `0064D4AD` and `0064D505`
(reads in the reviewed driver), `00BD0433` and `00BD0452` (not yet interpreted).
Raw byte matches are not a complete cross-reference/call graph.

Complete driver `0064D440` (661 bytes) creates default ordinary readers for
`static.manifest` at owner+370h and `mod.manifest` at owner+35Ch if absent.
When the reader vector at owner+360h is empty under its healthy four-byte
element invariant, it iterates queue begin/end `00CF2354`/`00CF2358`, calls
ordinary factory `004AAC80` with the queued string and argument two=1,
and appends returned readers. No null-factory rejection before append or
queue drain/clear is observed in this body.

The driver selects `_L`, `_M` or empty suffix from owner+2E8h, calls
`004AA300` with argument one=1 for default and queued readers, and follows
distinct previously-initialized versus initial-load branches. It is incorrect
to claim every queue entry is unconditionally loaded on every invocation.

On the initial-load branch it probes queued readers with `004AA5B0` and,
when the probe returns true, calls `004D05E0(reader,0)` at `0064D6AF`.
The complete probe body calls `004D7840` on reader+0Ch, has version-dependent
alternate naming paths and can invoke `004AA300` with argument one=2.
Exact filename transformation/fallback and underlying source lookup remain
open, rather than assigning these operations guessed language/patch meanings.

## Third concrete wrapper caller path

Complete `004D05E0` (293 bytes) rejects null and already-member readers through
`0045F060`. It builds an owner node, links it to sentinel `00CF1584` and uses
list owner `00CF1578`. At `004D06DB` it calls `004CFEB0` with:

| Wrapper argument | Reviewed origin |
| --- | --- |
| 1 | Address of a zero-initialized local context |
| 2 | Address of stored node reader payload, node+8 |
| 3 | `00CF1578` list owner |
| 4 | Zero EBX |
| 5 | Literal 1 |
| 6 | `004D05E0` argument two |

The sixth value is loaded at `004D0668` from ESP+7Ch with 6Ch locals and two
saved registers before pushes: original helper argument two. The queue driver
passes zero; this therefore forwards zero to the added ninth loader argument
through the already-reviewed wrapper mapping. This is a third reviewed caller
path, not a claim that the remaining `004D07F8` caller or ninth consumer is solved.

This connects config-authored manifest strings to a real reader factory and
conditional wrapper invocation. Startup reachability, complete loading success,
every stream binding and BIG mounting remain unresolved. The earlier ownership
report's two-path count describes its original scope; this follow-up adds one.

## Tests and next gate

`Get-Ra3Ep11ManifestQueue.ps1` independently pins the whole driver, direct-load
wrapper, probe and stock literal block. `Test-Ra3Ep11ManifestQueue.ps1` requires
eighteen detached byte-fault rejections and stable repeat JSON. Use PowerShell 7
with explicit 1.1 `-ImagePath` and 1.0 `-BaselineImagePath`.

No native callbacks, game or managed hash command are executed. Production
readiness and mod-package loading remain false; no stock assets are modified.
Engineering estimate remains **52% / 48%**; the 165 compiler groups are not rerun.
Next: full variant setter `004AA300`, source lookup `004D7840`, and BIG mount
helper `00995160` before building a detached package preflight.
