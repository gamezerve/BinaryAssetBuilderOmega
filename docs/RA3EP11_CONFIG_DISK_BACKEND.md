# Config disk backend: Windows open/read bridge

October 10, 2026. Static follow-up to the [engine read route](RA3EP11_CONFIG_READ_ROUTE.md),
using the reviewed 1.1 image. No game, debugger, DLL loading, trace or patch.

## Concrete bridge

`0096C330` enumerates logical drive bits, allocates provider objects and installs
vtable `00C7F200` before registering them. Its slot `0Ch` is `0096C1D0`, disk
open; slot `14h` is `0096BBF0`, disk read. The request worker's virtual read
call can therefore reach this concrete disk route if its provider object uses
that vtable. This is not proof of runtime provider selection for the probe.

The source opener `004D6AF0` maps incoming engine flags. For the config's
`401h`, the scoped mapped backend flags are `20h`. Disk open keeps default
desired access `80000000h`, share mode 1, creation disposition 3 and attributes
`80h`. It converts the ASCII/UTF-8 path through `MultiByteToWideChar` to a
260-wide-character local buffer, then calls `CreateFileW`. Successful handles
are wrapped in a 16-byte object whose first word holds the Windows handle.
The initial read-only probe handle and this reviewed open route are compatible
in their scoped read/share requests; unknown providers and other flag values
are not certified. Rooted-path copying, allocation failure cleanup and full
path normalization are not modeled as universally safe.

The PE import descriptor and original-name/IAT thunks independently bind:

| Imported API | IAT preferred VA | Reviewed call |
| --- | --- | --- |
| MultiByteToWideChar | `00BD8230` | `0096C2BC` |
| CreateFileW | `00BD82B0` | `0096C2DF` |
| ReadFile | `00BD81BC` | `0096BC09` |

This is import/call evidence, not recognition of a string or unverified
pointer hit. The complete image identity is checked before offsets are used.

## Count versus success

`0096BBF0` obtains the Windows handle from the wrapper's first word and calls
`ReadFile` with a null overlapped pointer. The output-count pointer aliases
the stack argument location that previously held requested length. At return
site `0096BC0F`, **EAX still holds the Windows boolean result**; the next
instruction overwrites it with the output count. The wrapper does not branch
on that boolean, advances its internal cursor by the count and returns it.

Thus the proposed one-byte observation must capture the API boolean **before
it is overwritten**, output count 1, non-null destination and byte `0A`, and
link the same handle to an exact-path successful open. A requested count of 1
before the call is not completed I/O evidence. An engine EAX count of 1 after
the wrapper returns is not the Windows boolean result. Observing both removes
this ambiguity, but asynchronous/unexpected-provider paths still require refusal.

## Proposed scope, not an executable debugger recipe

Correlate one exact short-name probe invocation through config-reader entry,
source/memory object, request, concrete provider and handle. Capture
`CreateFileW` path/return plus close/reuse, `ReadFile` entry arguments and
`0096BC0F` return evidence, then the final engine buffer/parser entry described
in the read-route plan. Runtime addresses must be resolved and byte-verified;
the listed values are preferred VAs. Reject missing, overlapping, different
thread/object/handle or unexpected-vtable observations.

Early process ownership, safe debugger supervision/exit and live provider
selection remain to implement. Do not copy helper commands into the game,
attach to existing processes, enable automatic descendant debugging or start
a direct native game executable without separately reviewed scope/authorization.
No actual successful read or config-consumption claim is made here.

## Validation

`Get-Ra3Ep11ConfigDiskBackend.ps1 -SelfTest` passed five instruction/table pins,
two concrete slot checks, three bounded KERNEL32 import bindings and seven
private instruction/table mutation refusals. It rechecks the complete image
SHA-256 afterward. The inherited read-route guards remain unchanged.
These are standalone evidence tests, not new compiler test groups.

The concrete disk bridge is recovered; actual runtime disk-provider selection,
Windows read success, parser consumption and authored mod loading remain
unproved. Overall estimate remains approximately **52% / 48%**.
