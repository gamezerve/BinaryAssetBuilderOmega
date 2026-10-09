# Inert modconfig trial — October 10, 2026

## Outcome: forwarding observed, startup failed

The user authorized one normal launcher invocation with the fixed single-LF
`fixtures/ra3ep11/phase-a/config-read-only.cfg` as `-modconfig`. No debugger,
system-wide recording, mod package, patch, automatic retry or process stop
was used. The user reported **nothing visibly happened**; menu arrival is not
claimed. The previous no-modconfig baseline did reach the menu.

Preflight passed reviewed installation hashes, no existing Uprising processes,
absence of known loose overrides and exact single-LF content. The probe has
a 156-character ASCII absolute path. Its read-only, read-share handle remained
open during the experiment to deny ordinary writes/replacement. That sharing
is an experimental condition: incompatible native open sharing is untested.

At approximately **00:24:36 Europe/Istanbul**, launcher PID **46544** created
same-installation `Data\ra3ep1_1.1.game`, PID **21572**, creation UTC
`2026-10-09T21:24:36.591871Z`. Five snapshots were recorded from
`21:24:37.2524922Z` through `21:24:40.2305756Z`; later samples did not observe
the child. A matching Windows Application Error corroborates its crash.
`Failure=null` means sampling completed, not successful game startup. The
observer does not capture the child's exit code or verify UI state.

The visible command line includes `-modconfig <absolute probe path>` followed
by `-config "<installation root>\RA3EP1_English_1.1.SkuDef"`. Because the probe
path has no spaces, it was serialized unquoted. The original quoted-only
matcher reported zero matches. The corrected matcher admits an exact
whitespace-free unquoted path with boundaries; offline reevaluation matched
**5/5 existing snapshots** without altering the JSON or relaunching the game.
This supports forwarding in this run, not native parsing or a successful read.

## Windows error evidence

A narrow Application-log query, UTC `21:24:30`–`21:26:30`, found:

- Event 1000, `2026-10-10T00:24:38.4817926+03:00`, Application Error.
- Application `ra3ep1_1.1.game`, version `1.1.0.0`, exact installation path.
- Faulting PID `0x5444` = **21572**, matching the observed child.
- Module `MSVCR80.dll`, version `8.0.50727.9680`.
- Exception `0xc000000d`, module-relative offset `0x00014584`.
- Two later Event 1001 records with `BEX` and the same report identity.

These are recorded Windows fields, not a reconstructed call stack. They do
not identify the responsible game function or prove config access. They do
not justify replacing the CRT, changing DEP or patching the game.
The single referenced WER archive was inaccessible even outside the workspace
sandbox because of Windows ACLs. No ownership/ACL/admin-token changes were
attempted, and no dump was opened. No Uprising processes remained in the
post-trial name-scoped query; the observer did not terminate them.

## Evidence and validation

Full process observations remain local and ignored:
`artifacts/Baseline-227288c0a39742e6b1daf6b163de6380/baseline.json`.
SHA-256: `3659B3B1508AF3B2AEA54A649C1B99A2427FA7EBAB1C7EE951631BD1F14FA43B`.
Keep its historical zero match count intact; the corrected offline result is
recorded above. Snapshots are non-atomic; PID reuse is not fully excluded and
indirect descendants are not observed. The sampling loop runs approximately
30 seconds but does not impose a hard timeout on individual CIM queries.

`Invoke-Ra3Ep11Baseline.ps1 -SelfTest` passed two request positives, six request
refusals, three visible-argument positives and six negatives. Cases include
quoted spaced paths, unquoted whitespace-free paths, wrong option/path,
missing command line and token-boundary faults. No process query or game
execution occurs in these detached tests. `-ConfigProbe` without `-Run` is
preflight only; a real trial needs explicit authorization.

## Next diagnostic gate

Distinguish option/path handling, file-provider/open sharing and reader/line
splitting before introducing assets. The reviewed setter has a 256-byte
destination; this path alone does not exceed it, but downstream joins or
smaller buffers are not ruled out. A shorter-path or sharing control is a new
game trial, not authority to retry silently. Stack/consumer instrumentation
needs separate approval; helper calibration is not game evidence.

Config consumption, effective paths, manifest acceptance and authored-mod
loading remain unproved. Overall estimate stays approximately **52% complete /
48% remaining**. This is failed startup with useful forwarding/crash evidence,
not a successful Phase A loading milestone.
