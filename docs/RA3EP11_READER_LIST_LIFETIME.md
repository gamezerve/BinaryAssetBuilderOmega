# EP1 1.1 reader list lifetime

Static, read-only evidence for image SHA-256
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
The game is not executed. This is not a working mod-loading result.

## Scoped insertion and release

The factory-dispatch insertion slice at `004CEA62` uses the list-owner's
embedded circular sentinel at `owner+0Ch`. A node has next/previous links at
0/4 and a reader payload at 8. Let S be the sentinel, F the previous first
node and N the new allocation. The observed writes are:

1. N.next=F; N.previous=F.previous.
2. F.previous.next=N; F.previous=N.
3. Reload S.next and write its reader payload.

Under the explicit healthy circular-list invariant F.previous=S, step two
updates S.next=N, so step three writes the new node. Empty lists work through
the same alias. Successive insertion gives newest-first traversal. This
does not prove every initializer, concurrent use or native corruption safety.

The source-list release/reset slice `004ABDEB` traverses node+8 reader values,
calls each reader's virtual slot 08h, then calls node cleanup `00497090` and
restores both sentinel self-links. Node cleanup also traverses nested data
at node+24h and invokes `008B2CC0`; arbitrary callback semantics remain open.

Concrete vtable `00BF8EA0` routes slot 08h to `004171C0`, a null-guarded
thunk that calls slot 3Ch with deletion flag 1. Slot 3Ch resolves to
`0049F800`: call `0049B6F0`, conditionally free the reader via `004169B0`
when argument bit zero is set, return the reader address. The full cache
and destructor helper semantics are deliberately not claimed by this audit.

Resource cleanup `004184B0` is different: release/clear reader+18h,
release linked-state+1Ch handle, free linked-state+8 and +0 buffers and the
state itself, clear reader+54h. This reviewed body does not free the reader
object. Underlying virtual calls are not proven safe for arbitrary readers.

## Reproducible evidence

`scripts/Get-Ra3Ep11ReaderListLifetime.ps1` uses the strict 1.1 ownership
chain and independently pins seven reviewed regions. The 1.0 baseline remains
separately pinned; old addresses are not accepted in place of 1.1 evidence.

`scripts/Test-Ra3Ep11ReaderListLifetime.ps1` requires four detached insertion
scenarios, two invalid-list rejections, twelve detached byte-fault rejections
and stable repeat JSON. Synthetic fixtures never dereference native pointers.
No native allocator, destructor, codec or managed hash command is executed.

Run with PowerShell 7 and explicit `-ImagePath` (1.1) and
`-BaselineImagePath` (1.0). Production readiness, full stream provenance,
all list initializers, complete cache lifetime and mod package loading remain
false. No ProcessingHash guess or production guard change is introduced.

Next: independently pin the complete `0049B6F0` destructor and inspect its
cache lookup/removal callees, alongside ordinary cache publication `006A34B0`.
Engineering estimate remains approximately **52% complete / 48% remaining**;
the 165 compiler groups are not rerun by these standalone audits.
