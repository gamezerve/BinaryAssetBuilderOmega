# EP1 launcher profile and observation boundary

## Static result

The selected installation's `RA3EP1.exe` is 9,209,808 bytes, SHA-256
`07694EBBCF21232B1A1B401C07ABC2CFB1EDB9F50FA8D0C36DA0F94820943A4A`.
It is native I386 PE32, preferred base `00400000`, with entry RVA `008B4310`
in `.bind` (entry raw offset `0089B310`). The `.bind` raw/RVA mapping is not
the `.text` mapping; a single global VA-to-file delta would be incorrect.
CodeView records `C:\CNCRA3\Production\Run\RA3EP1Launcher.pdb`, GUID
`B94DC137-FC3D-480D-9289-5291374E13FB`, age 1. No actual PDB was obtained.

| Literal | Encoding | Raw offset | Preferred VA |
| --- | --- | --- | --- |
| `set-exe ` | ASCII | `00092B74` | `00492B74` |
| `.SkuDef` | UTF16LE | `00092C34` | `00492C34` |
| `-modconfig "` | UTF16LE | `00092C44` | `00492C44` |
| `-config "` | UTF16LE | `00092C60` | `00492C60` |
| `_*.SkuDef` | UTF16LE | `00092C74` | `00492C74` |

| Imported API | DLL | IAT preferred VA |
| --- | --- | --- |
| `GetCommandLineA` | KERNEL32 | `0048F2E4` |
| `CreateProcessW` | KERNEL32 | `0048F3A0` |
| `GetCommandLineW` | KERNEL32 | `0048F444` |
| `ShellExecuteW` | SHELL32 | `0048F5E0` |

The `.text` section contains zero literal little-endian preferred-VA word
occurrences for the five strings. A bounded disassembly at `00401000` through
`00401200` did not yield a coherent launcher flow usable for these consumers.
Together with the `.bind` entry this indicates a static-analysis boundary;
it does **not** establish a particular protection implementation, that every
code region is encoded, that the strings are unused, or how runtime resolution
works. No protection layer was decoded or bypassed.

String/import existence does not prove actual command construction, default
version selection, quote handling, forwarding of user arguments, or a
`CreateProcessW` call site's parameters. These gates remain false in the
report. Native 1.1 [option recognition](RA3EP11_MODCONFIG_BASELINE.md) is a
separate proved static fact; launcher forwarding is still unproved.

## Reproducible tools

`Get-Ra3Ep11LauncherProfile.ps1` admits only the above identity, reuses the
read-only managed PE reader, validates terminated literals and checked import
name/IAT records, uses per-section mapping, and rereads the image identity.
`Test-Ra3Ep11LauncherProfile.ps1` checks five private literal mutations, one
truncation refusal, four import bindings and repeat JSON. It neither launches
the executable nor interprets random-looking bytes as recovered functions.

`Get-Ra3Ep11LaunchObservation.ps1` is a separate **read-only observer**, not a
launcher. Its `-SelfTest` exercises two accepted version paths and three
scope refusals using detached objects, without querying running processes.
Its live mode requires `-LauncherPath` and the explicit
`-LauncherProcessId` of an already-running selected launcher. It reads only
that PID and immediate children, admitting only same-installation
`Data\ra3ep1_1.0.game` / `Data\ra3ep1_1.1.game` paths. Output preserves the
raw child command line without claiming to parse Windows arguments.

The observer rereads parent/selected child PID identities and refuses races,
missing executable paths and unavailable selected child command lines. It
does not elevate permissions, start/stop processes, attach a debugger, change
registry values, dump memory or poll indefinitely. An empty result means no
eligible immediate child was captured, not that no game was launched. Short
launcher lifetimes, indirect process chains or unavailable CIM information
can prevent observation. A later process-start trace may be needed instead;
no such trace has been collected or validated in this milestone.

## Phase A experiment record: what must be observed next

Use a baseline and isolated-config run as separate experiments. Before either
run, recheck launcher/image/SKU hashes and explicitly document the absent
configured `MapsCampaign.big`; do not silently substitute its Disabled-named
neighbor. No installed config or archive needs to be overwritten.

For each run, record:

1. Launcher absolute path, creation time/PID, requested arguments and starting
   directory. The starting directory is an experiment input; this observer
   does not recover it from CIM.
2. Actual child absolute path, creation time/PID/parent PID and raw command
   line. If it selects 1.0, stop attributing the result to 1.1. If no reliable
   child can be captured, report that gap rather than infer selection from SKU.
3. For an isolated config, its workspace-only path/hash and exact requested
   argument. Verify the observed child contains the intended correctly quoted
   argument before treating the run as a forwarding experiment. The literal
   `-config` is not proof that a proposed version override will work.
4. Independently observable access/read of that config and resulting mount
   behavior. Command-line presence alone is not config consumption; a menu
   screenshot alone is not successful mod loading.

Do not immediately build an authored asset around an unverified launch path.
This milestone prepares observation but **has not run either experiment**.
The isolated config contents and their effective interaction with later
startup updates still require design and validation. Overall SDK effort
remains approximately **52% / 48%**, with zero proven game loading.
