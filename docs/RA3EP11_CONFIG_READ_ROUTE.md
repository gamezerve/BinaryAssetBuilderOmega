# Native config read route and observation plan

October 10, 2026. Read-only review of the same SHA-pinned 1.1 executable.
The short-name probe reached the menu, but actual consumption remains open.
No game launch, debugger attach, system trace or binary patch in this review.

## Why the last read need not be a kernel file read

Config reader `004D86B0` opens through `004D7E90` with flags `401h`.
After its concrete source-reader open succeeds, the opener tests bit `400h`
and calls virtual slot `38h`. Constructor `004D7B90` installs source vtable
`00BFA270`; that slot targets `004D7C40`.

The conversion allocates a 32-byte object with memory vtable `00BFA208` and
calls slot `54h`, `004D7170`, with the original reader. Population obtains
the source size, allocates storage, and calls source slot `0Ch`. It stores
the returned EAX as memory size. Negative results fail; **zero is admitted**
as success after allocation, so conversion success alone does not prove that
the one-byte probe was transferred. A successful conversion closes the original
reader; a failed conversion returns the original reader after cleanup.
Some allocation-failure paths lack a reviewed null guard; this is not a safety
certification or authority for malformed-input/native fuzz testing.

Memory slot `0Ch` targets `004D5920`. It uses data pointer +14h, cursor +18h
and size +1Ch, clamps a signed requested count to remaining bytes, optionally
copies bytes and advances the cursor, then returns the resulting count in EAX.
A null data pointer returns -1. A null destination can skip copying while
still advancing/returning a positive count. Thus a positive return alone is
insufficient: observe valid source/destination pointers and actual content.
Signed arithmetic behavior for malformed state is not modeled as safe.

## Narrow proposed evidence chain

| Site | Evidence needed | What it does not establish alone |
| --- | --- | --- |
| Reader entry `004D86B0` | Exact bounded ASCII probe path, PID/TID, caller | A successful open/read |
| Open return `004D86F2` | Non-null EAX, concrete returned vtable, same invocation | Which provider/source bytes were read |
| Population return `004D71CA` if conversion used | Source reader, non-null buffer, size 1, EAX 1, byte 0A | Final parser consumption |
| Reader read return `004D8725` | Expected concrete reader, same TID, non-null EDI, EBX 1, contract-valid EAX, byte 0A | Correct handling of all directives/assets |
| Splitter entry `004D9040` | Same buffer at ESP+4, bytes 0A 00, correlated invocation | Manifest acceptance or mod loading |

Those addresses are preferred VAs, **not automatically valid live addresses**.
Resolve and verify the loaded image base and instruction bytes before setting
any game breakpoint. Refuse unexpected reader vtables rather than interpreting
their EAX using the memory-reader contract. The fallback source-reader read
body is not yet pinned/characterized by this audit.

Do not reuse helper CDB commands unchanged: the game may have multiple threads
and concurrent opens; global scratch registers and broad one-shot return
breakpoints are not a validated game observer. Scope records to one exact
probe invocation, PID/TID/object/buffer lifetime, reject overlap or missing
events, and separate loaded-buffer proof from attributable kernel-read proof.

Before an instrumented trial, resolve the early-launch ownership route and
supervision policy. Attaching after menu arrival can miss the config read.
Debugging the launcher with automatic child debugging may include unwanted
descendants. Direct native launch differs from the proven launcher route.
None of those alternatives is silently authorized or implemented here.
Disclose temporary-memory breakpoints/timing effects, no unknown-process
termination, missing-evidence behavior and safe debugger exit/detach before
asking the user to approve a separately scoped real trial.

## Validation

`Get-Ra3Ep11ConfigReadRoute.ps1` reuses the full image/consumer identity guard,
adds six exact code/data pins and three concrete vtable bindings, and rechecks
the image hash at completion. `-SelfTest` exercises nine private mutation
refusals without calling the native code. All nine refusals and final image
identity recheck passed. This is static route recovery, not
a runnable debugger recipe or another compiler test group.

Actual kernel read, memory read, exact config consumption, effective paths,
manifest loading and authored mods remain unproved. The rejected WPR recording
route stays disabled. Overall estimate remains approximately **52% / 48%**.
