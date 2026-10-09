# Post-config identity limit and startup-crash candidate

October 10, 2026. This is read-only analysis of the reviewed 1.1 image and
the exact local CRT named in the [failed inert trial](RA3EP11_INERT_CONFIG_TRIAL.md).
No new game launch, debugger, package, binary patch or system setting change.

## Concrete test-harness defect

After calling config reader `004D86B0`, startup continues at `004D98AB`.
It calls `004D5820`, which returns the basename after the final slash, copies
it into a 256-byte local buffer, and truncates at the **first underscore**.
At `004D98FD` it passes the resulting string to `004D0F10`.
That helper calls imported `strcpy_s` with destination `00CC4B6C` and capacity
**16 bytes**, including the terminating NUL.

Our historical probe basename `config-read-only.cfg` has no underscore and
is **20 ASCII bytes**. If this startup branch reaches the setter with that
name, the copy cannot fit. Whitespace-only file content did not make the
complete startup experiment safe: the original harness omitted filename
identity constraints. This is our probe-design defect, not evidence that the
SDK's authored-asset serializer or all EP1 mod loading is broken.

The replacement `probe_1.0.cfg` has the same single-LF content/SHA-256:
`01BA4719C80B6FE911B091A7C05124B64EEECE964E09C058EF8F9805DACA546B`.
Its identity before the underscore is `probe`, five bytes. The reviewed later
branch extracts `1.0` between the first underscore and final dot and copies
it into a separate six-byte destination at `00CF1868`; this chosen value fits
that copy as well. Full numeric-version semantics are not certified.
The separately authorized [replacement control](RA3EP11_SHORT_NAME_CONFIG_TRIAL.md)
subsequently reached the menu; all 34 child snapshots show the expected path.
The old fixture remains unchanged for historical/helper evidence only.

## Correlation with the recorded CRT fault

Local CRT SHA-256:
`A74E18C475E7853D6158AE668DCC3F6C30C7B9EB7BB2360A8B58D945C1E81FBB`.
Its export table binds `strcpy_s` to RVA `1455B`; the recorded fault RVA
`14584` is **41 bytes into that function**, at the return site immediately
following a call to `_invalid_parameter`. The reviewed function reaches the
same error path for null destination/source, zero capacity or an exhausted
copy capacity. The offset does not distinguish those conditions.

Thus the filename overflow is a strong, reproducible static explanation
consistent with the CRT signature, **not proof of the exact crashing caller**.
We have no crash stack, live argument snapshot at that setter or successful
follow-up runtime control. Config-read success remains unproved; even a reader
call occurring earlier in the branch would not prove a successful I/O result.

The first path join `004D69A0` detects a colon or leading slash and directly
copies such paths into a 256-byte destination. Our 156-character `C:` path
does not require appending the installation directory at this specific call.
This weakens the initial combined-path overflow hypothesis, without certifying
all downstream buffers or providers.

## Guard and validation

`Ra3Ep11ConfigNamePolicy.ps1` models ASCII basename/first-underscore identity
length and refuses a 16-byte-or-longer identity. This is one conservative
copy-boundary guard, not native parser emulation or general safety proof.
Both the launch runner and smoke planner now apply it. The runner is pinned
to `probe_1.0.cfg`; no arbitrary config-path input was added.

`Get-Ra3Ep11ConfigIdentityEvidence.ps1 -SelfTest` checks the full reviewed
image identity, three new instruction slices, bounded CRT export resolution,
the exact CRT function body, four detached name/boundary cases and three
private instruction-mutation refusals, then rechecks image hashes. Export
tables are read in existing checked 4-KiB chunks; shared guards are unchanged.
The runner's detached tests include explicit refusal of the old filename.
The complete smoke-plan repeat/whitespace/refusal check passed with the new
absolute probe path, unchanged one-LF hash and restored installation profile.
These tests do not count as new compiler test groups or successful game loads.

The same-content replacement-name control is now complete, keeping launcher,
flags and sharing unchanged. It strengthens this explanation but does not
provide the earlier crash stack or certify config I/O. Next: scope actual
config-consumption evidence separately. Do not introduce authored assets or
silently relaunch/add instrumentation.
Overall estimate remains approximately **52% / 48%**.
