# Short-name inert modconfig control — October 10, 2026

## Result

The user explicitly authorized one follow-up start with `probe_1.0.cfg` and
confirmed **the main menu opened**. This is human UI confirmation, not an
automated screenshot/menu assertion. The launch runner's JSON therefore
retains `MainMenuReachedProven=false`; that flag describes the observer only.

The reviewed launcher, native images, SKU, campaign archive, working directory,
empty probe content, argument option and file-sharing conditions were unchanged.
The repository probe's basename changed from `config-read-only.cfg` to
`probe_1.0.cfg`. Both files contain exactly one LF byte, SHA-256
`01BA4719C80B6FE911B091A7C05124B64EEECE964E09C058EF8F9805DACA546B`.
The new filename uses identity `probe` and version text `1.0` to fit the reviewed
copies; the old name had no underscore and a 20-byte identity.

No debugger, system-wide recording, authored assets, mod package, retry,
process termination, game/CRT patch or system settings change was performed.
Preflight and detached runner tests passed. The user was told the game may
write its own settings/logs and could close it normally after observation.

## Recorded process evidence

At approximately **00:41:48 Europe/Istanbul**, launcher PID **22724** started
the reviewed installation's `Data\ra3ep1_1.1.game`, PID **21420**, creation UTC
`2026-10-09T21:41:48.900183Z`. The observer saved 34 qualifying snapshots from
`21:41:49.5015739Z` through `21:42:14.3750238Z`; all **34/34** matched the exact
visible `-modconfig` and absolute `probe_1.0.cfg` argument. The child's command
line also contained the default `-config` argument for
`RA3EP1_English_1.1.SkuDef`. It does not certify native argument parsing.

Observation completed without an observer error. The finite snapshot sequence
is not a measured total game lifetime or a recorded child exit code. The
observer never stopped the game. A read-only Application-log query in UTC
`21:41:40`–`21:42:30` found no Event 1000/1001 records. This narrow absence does
not prove that no failure occurred at any later time or in another log.

Full local ignored evidence:
`artifacts/Baseline-b390877188f646da91233187426a4875/baseline.json`.
SHA-256: `75CE07B257436614A2834E7B5B300687C2074454D25595B69BB8B9CB5FA84710`.
Snapshots are non-atomic; PID reuse is not fully excluded and indirect
descendants are not observed.

## Interpretation and next gate

Compared with the PID-matched CRT crash in the old-name trial, successful menu
arrival under the same-content replacement **strengthens the post-config
filename identity overflow explanation**. The reviewed static copy cannot fit
the old name, and the recorded CRT fault lies on `strcpy_s`'s error path.
We still have no runtime stack proving which copy caused that earlier crash.
This does not establish that all startup configurations are safe.

The normal launch and the inert short-name launch now both reach the menu.
However, the probe has no directives, so menu arrival cannot distinguish a
successfully consumed probe from an ignored/skipped file. Attributable native
read evidence or a separately reviewed observable config action is the next
gate; neither is authorized merely by this control. Do not silently add a
debugger, restart with new directives or proceed to authored asset packages.
Effective search paths, manifest acceptance, native serialization/hash
compatibility and authored-mod loading remain unproved.

README and runtime plans reflect this follow-up. Overall estimate remains
approximately **52% complete / 48% remaining**: this resolves the immediate
test-harness startup obstacle for this control, not the remaining SDK gates.
