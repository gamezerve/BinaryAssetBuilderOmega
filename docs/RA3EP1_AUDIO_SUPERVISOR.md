# Supervised audio worker and bounded result acceptance

Date: October 4, 2026. Native audio now has an opt-in supervised entry point:
`supervised-core-audio-poc <absolute-audited-audio.dll>`. The parent never loads
the audio DLL. A Windows inspector apphost child performs the fixed core/native
PoC and the parent independently validates its result before writing ACCEPTED.json.
This is diagnostic evidence acceptance, not SDK/mod publication or a security sandbox.

## Process and protocol boundary

`AudioEncoderSupervisor.Run` creates a fresh `Reborn-SupervisedAudio-<nonce>` job
under the temporary directory, with an exclusive request.json. It launches only
the current inspector apphost using argument-list parameters, no shell and no visible
window. The worker validates the nonce, version and exact owned job root before work.
It writes exclusively into the initially empty `worker` subdirectory.

The parent drains stdout/stderr concurrently, retains at most 65,536 characters per
pipe, and rejects output overflow. Capped logs are stored as worker.stdout.txt and
worker.stderr.txt, including failed jobs. Worker text is data, not an instruction
or a success signal. Default work timeout is 30 seconds; allowed range is 100 ms
through 30 seconds. A timed-out child/process tree is killed and checked for exit,
with bounded five-second termination and pipe-drain waits.

Only exit zero, no timeout/log overflow and a complete result may reach validation.
The worker emits result.json only after normal encoding, package verification,
shutdown and unload return. Failed workers may leave partial diagnostic files or
packages inside their job; these are retained but **not accepted** by the parent.
No broad cleanup or game-directory publication is performed.

Protocol version 1 limits:

- Request/result JSON: 32 KiB; null, unknown fields and duplicate keys rejected.
- Result paths: bounded canonical relative paths, no rooted/traversal/backslash
  paths, duplicate identities or reparse ancestry; parent derives the output root.
- Inventory: at most 64 files, 1 MiB per file, 4 MiB total, 16 directories and depth 4.
- Parent re-enumerates the complete bounded tree and recalculates SHA256/length,
  rather than opening arbitrary paths supplied by a worker.

`ValidateResult` reconstructs immutable RAM/streamed native/custom records, checks
canonical fixed source XML before calling the core loader, re-parses actual core
identities and checks current preparation. It verifies frozen PCM copies, the
identity sidecar and fixed event source. It independently verifies two- and
three-entry packages using the existing frozen-byte and dual-reader checks.
The mixed event's recovered fixed wire shape/dependency binding is checked; this
is not a general proof of all AudioEvent fields or source graphs.

ACCEPTED.json is written exclusively **after** these checks. It is an unsigned
time-of-validation diagnostic marker, not an immutable security certificate:
consumers must revalidate evidence if files change later. Concurrent malicious
filesystem replacement, hostile native code and OS-level sandboxing are not solved.
The native DLL remains pinned/audited by the child, and broader native errors remain
isolated/unproven rather than enabled inside a general compiler process.

## Validation

`AudioEncoderSupervisorSmokeTest` is compiler group **100**. Twelve actual managed
child-process failure cases run without loading native codecs: nonzero exit,
simulated 0xC0000005 exit code, timeout/kill, missing/invalid result, wrong nonce,
log overflow, fake zero-exit success, path escape, duplicate JSON keys, unknown
fields and oversized JSON. Each requires the intended rejection and no acceptance
marker. The crash-exit case uses Environment.Exit with that code; it is **not**
an actual access violation, and the previous unsafe DLL condition is not repeated.

`supervised-audio-tamper-test <dll>` is separately opt-in. Two actual native children
encode successfully, then mutate only owned evidence. One changes runtime bytes
after inventory capture, which the parent rejects by hash. The other corrupts a
manifest before capturing its updated inventory: hashes match, but independent
package verification rejects it. Neither receives ACCEPTED.json.

Actual successful supervised job:
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-fd51db01f058414a9377d42ad4516914`

All 28 worker evidence files match the prior validated unsupervised core PoC by
SHA256. Core hashes remain 8F8D3317/E73D3F1F and diagnostic content hashes remain
BBE648A4/E7D8D8B8; they are not substituted for each other. Payloads, package sizes
and local event relationships are unchanged. Inspector Release/x86 build, all 100
compiler groups and 33 enum checks pass. Models/marshallers remain 785/1,390 and
762/1,390. Default tests spawn managed workers but never load the audio DLL.

## Remaining work

Overall effort remains approximately **50% complete / 50% remaining**. The bounded
supervision/acceptance gate is implemented, but all native encoding still uses a
fixed owned fixture. Next: carry an explicitly bounded authored input snapshot
through this worker protocol, with checked identity/settings and output ownership,
before experimental AudioFile processor integration. General source graphs,
inheritance, codecs/WAV variants, stock producer identities, WorldBuilder and
in-game loading remain open. Production/cache admission remains disabled.
