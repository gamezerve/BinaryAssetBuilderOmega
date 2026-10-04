# Audio encoder failure boundaries and known native crash prevention

Date: October 4, 2026. Eight controlled worker scenarios now verify completed
resource-call balance and publication rejection. A real native-path crash was
also found and must not be confused with a successfully handled native error.
Production AudioFile processing remains disabled.

## Actual crash and protective boundary

An initial attempt to obtain a native create error used a nonexistent output
parent within a fresh owned fixture directory. The Windows x86 worker exited with
**0xC0000005** (signed exit -1073741819). The stack reported AudioEncoderPoc.Encode.
The exact native instruction or create/error-reporting/cleanup subpath has not
been localized. No successful shutdown/cleanup is claimed for this crashed worker.
The dangerous native call condition was not repeated after discovery.

Crash fixture:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-fc7bc2f209f1460d970a1f13a5a2395e`

Read-only inspection found only input.wav, ram.input.wav, ram.xml and streamed.xml;
no package/staging directories. No game files were involved. The process boundary
contained the failure; no claim is made that finally blocks survived native faults.

`AudioEncoderPoc.ValidateNativeOutputPrefix` now rejects missing/relative parents,
non-simple prefixes, reparse ancestry and existing prefix/SNR/SNS artifacts before
SIMEX_create. This prevents the observed invalid-parent pathway. It does not make
all native errors safe: permission failures, disk-full conditions, invalid native
returns, access violations or other library faults remain unproven. Worker isolation
is still mandatory; the DLL is not enabled in a general production compiler.

The replacement scenario is named `missing-output-parent` and verifies **managed
preflight rejection**, not a successful native create failure. The former unsafe
`native-create-error` scenario name is no longer admitted.

## Independent cleanup

`AudioEncoderPoc.Release` detaches the nonzero handle before exactly one attempt.
`Cleanup` uses nested independent finally blocks for target, sound-info and source,
so one managed release exception cannot skip the remaining attempts. Raw return
status is preserved rather than interpreted from an undocumented success convention.

`AudioEncoderCleanupSmokeTest` uses fake handles and managed callbacks against the
same Cleanup method. It covers success and a throw from each release, then repeated
cleanup with zeroed handles. These are managed control-flow tests, not native fault
recovery. Native access violations remain outside this guarantee.

## Controlled actual worker evidence

Command: `core-audio-encoder-fault <absolute-audited-audio.dll> <scenario>`.
Each invocation is a separate explicitly launched Windows x86 inspector process.
The audited DLL fingerprint, owned fixtures and current core preparation checks
remain unchanged. Default compiler tests do not load the native library.

| Scenario | Acquired source/info/target | Result before publication |
| --- | --- | --- |
| after-open | 1/0/0 | Controlled managed exception after actual open |
| after-info | 1/1/0 | Controlled managed exception after actual info/read |
| after-create | 1/1/1 | Controlled managed exception after actual create |
| after-write | 1/1/1 | Controlled managed exception after actual write |
| missing-output-parent | 1/1/0 | Unsafe output rejected before native create |
| after-encode-wave | 1/1/1 | Original WAV changed after encoding; core identity recheck rejects |
| before-publish-wave | 2/2/2 | Both encodes complete; same-size/timestamp-preserving WAV edit rejects |
| before-publish-xml | 2/2/2 | Both encodes complete; changed compression XML rejects |

All eight safe scenarios passed twice after the guard was installed; the second
pass additionally requires the exact reached stage and intended rejection reason.
Every acquired handle has a matching completed release call; observed raw release
statuses were 1. Init/shutdown/module-unload returned exactly once per worker.
No package or staging directory was emitted. Raw partial evidence is retained.
These observations do not prove allocator leak freedom or decoded audio correctness.

## Regression and remaining work

Managed cleanup/output-prefix validation is compiler group **99**; all 99 groups
were executed successfully. Inspector Release/x86 build and 33 enum checks pass.
Models/marshallers remain 785/1,390 and 762/1,390. Current core and legacy authored-only
native success runs still pass. The 28 generated core-success files match the prior
validated run by SHA256, including packages, payloads and identity sidecar.

Post-fix success fixture:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-88685f58ea1e4db299f303795bb85109`

Approximately **50% complete / 50% remaining** overall. The bounded failure/publication
gate is improved, but the native crash reinforces the need for a supervised worker
protocol with bounded results, exit-code/timeout handling and no publication from
failed workers before any wider AudioFile processor integration. General formats,
source graphs, stock producer identity, WorldBuilder and in-game loading remain open.
