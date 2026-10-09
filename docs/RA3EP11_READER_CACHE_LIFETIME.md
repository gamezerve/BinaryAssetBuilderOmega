# EP1 1.1 scoped reader cache lifetime

Read-only static review of `ra3ep1_1.1.game`, SHA-256
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
No game/compiler/codec invocation, no native pointer dereference and no
production hash/guard modification. This extends, but does not retroactively
rewrite, the narrower [list lifetime](RA3EP11_READER_LIST_LIFETIME.md) report.

## Destructor and removal

Deleting thunk `0049F800` invokes complete destructor body `0049B6F0`
(316 bytes), then frees the reader allocation only when argument bit zero
is set. Flags 0 and 2 do not request allocation free; 1, 3 and FFFFFFFF do.
The destructor is called for all these flags. Tests model this predicate only.

The destructor resets the concrete vtable to `00BF8EA0`, prepares reader+10h
as a lookup key for ordinary cache `00CC4ADC`, compares the returned node to
sentinel `00CC4AE0`, and calls removal `004970F0` when not sentinel. It repeats
the flow using reader+0Ch and tag-two cache/sentinel `00CC4AF8`/`00CC4AFC`.
Both lookups call `00677520`. Key-preparation helper `0047E090` is not assigned
complete path-normalization semantics in this report.

Removal decrements cache+14h, invokes `00AFA060` and `00968FC0`, conditionally
frees the node's key buffer at +10h according to the +18h span, frees the node,
and writes an iterator output. This reviewed body does not directly free the
reader payload. Complete rebalance/unlink semantics of its callees are open.

After the two cache paths the destructor calls resource cleanup `004184B0`,
frees reader fields +08h (conditional nonnull), +0Ch, +10h and +14h, invokes
member cleanup `004D1970` for +60h/+5Ch/+58h, and writes final base vtable
`00BF7EB8`. The member helper invokes `004D1480`, a virtual operation with -1,
conditionally decrements/frees storage eight bytes before its member pointer,
then invokes an import through `00BD81D0`. No locking, thread safety or complete
reference-count ownership is asserted from that sequence.

## Publication and lookup boundaries

Ordinary factory's publication helper `006A34B0` (158 bytes) has two explicit
return paths: an existing node returns node+20h; the insertion path calls
`00B0E6A0` and `006998B0`, then returns the resulting node+20h. The factory's
subsequent store writes the reader into that returned payload slot. This
resolves the scoped payload-slot question, not the full allocator/tree model.

Lookup `00677520` traverses cache+0Ch using node+0/+4 links, compares key spans
at node+10h/+14h through `004170E0`, and uses `004495D0` for its final ordering
decision. It outputs either a node or cache+4 sentinel. `004495D0` itself
compares the shared byte prefix and then signed span lengths. The precise
semantics of `004170E0`, arbitrary span validity and complete tree ordering
remain unresolved; no Unicode/case/path behavior is guessed.

## Validation

`Get-Ra3Ep11ReaderCacheLifetime.ps1` independently pins six whole bodies:
destructor, lookup, removal, publication, comparison and member cleanup.
`Test-Ra3Ep11ReaderCacheLifetime.ps1` requires five detached deletion flags,
eighteen detached code-fault rejections and stable repeat JSON; both take
explicit 1.1 `-ImagePath` and 1.0 `-BaselineImagePath` under PowerShell 7.

Full cache lifetime, complete stream provenance, production build readiness
and game package loading remain false. The 165 compiler groups are not rerun.
Overall engineering estimate remains **52% / 48%**. The next useful migration
gate is the mod-config file consumer: command-line dispatch alone does not
prove package mounting or stock dependency loading.
