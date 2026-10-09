# Phase A inert config-read experiment

**Latest runtime control:** [short-name probe](RA3EP11_SHORT_NAME_CONFIG_TRIAL.md)
reached the menu (user confirmed), with 34/34 matching `-modconfig` argument
snapshots. Actual config consumption is still unproved: the inert file has
no observable directive effect. No debugger or mod package was used.

**Probe-name correction:** future plans use `probe_1.0.cfg`, containing the
same one LF byte, rather than the historical `config-read-only.cfg` described
below. The [post-reader identity copy](RA3EP11_CONFIG_IDENTITY_LIMIT.md) cannot
fit that historical 20-byte basename. Both runner and planner now enforce
the identity-copy boundary. The replacement subsequently passed the menu
control linked above; this does not prove a successful file read.

**Runtime update, October 10:** the [single inert trial](RA3EP11_INERT_CONFIG_TRIAL.md)
showed the expected option/path in the 1.1 child, then crashed with a PID-matched
Windows error record. The earlier normal baseline reached the menu; this trial
did not. The probe below is not a proven successful read. These results do
not authorize an automatic rerun or game instrumentation.

**Current installation update:** [campaign restoration](RA3EP11_CAMPAIGN_RESTORE.md)
was explicitly authorized and completed without overwrite. The user then
authorized reversible isolation of the known loose metadata/map overrides.
The planner now reports configured archives present and known overrides absent,
without claiming full stock authenticity. Earlier missing-campaign statements are historical.

**Current capture gate:** [the user's real control trace](RA3EP11_READ_CONTROL_RESULT.md)
contained the negative helper and unrelated processes. The requested WPR
process-name scope is rejected and recording is disabled. The game experiment
must not use that candidate merely because its XML was accepted.

## Prepared artifact, not a run result

`fixtures/ra3ep11/phase-a/config-read-only.cfg` contains **one LF byte** (`0A`),
SHA-256 `01BA4719C80B6FE911B091A7C05124B64EEECE964E09C058EF8F9805DACA546B`.
The fixture is marked `-text` in `.gitattributes` to preserve its byte identity
on Windows. It contains no comments, search-path directives, archive mounts,
manifest queue entries, recursion or authored assets. Do not add explanatory
comments to this native config: native comment syntax is not assumed.

The [reviewed line splitter](RA3EP11_CONFIG_CONSUMER.md) skips LF before
dispatching commands. This makes a whitespace-only file a minimal config-read
probe whose intended effect is no directive action. It does not prove runtime
I/O, successful parsing, unchanged startup or mod-path persistence. Reading
this probe is a smaller milestone than loading an actual asset package.

`Get-Ra3Ep11SmokeTestPlan.ps1` prepares the following **requested** cases, not
proven launcher behavior:

| Case | Executable requested | Working directory | Argument list |
| --- | --- | --- | --- |
| baseline | Selected installation's `RA3EP1.exe` | Selected SKU directory | Empty |
| isolated-config-read | Same launcher | Same directory | `-modconfig`, absolute external probe path |

Arguments are retained as arrays, never sent to a shell or process API by the
planner. Quote the config as one Windows argument when a later launcher is
implemented. Presence of `-config` in the launcher is not proof that a proposed
version-selection override works; the plan does not invent such an override.

The required **observed** child is the selected installation's
`Data\ra3ep1_1.1.game`, SHA-256
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
The launcher and SKU identities come from their existing pinned audits.
If a baseline launches 1.0, do not compare it as though it were the reviewed
1.1 baseline. Stop and investigate selection before testing 1.1 forwarding.

## Explicit incomplete-stock profile

The current report labels the installation
`IncompleteStock-CampaignMissing-NoAlias`: 14 configured archives, 13 present,
missing `Data\MapsCampaign.big`. It does not rename or read the Disabled-named
neighbor or borrow an archive from another installation. This is a draft
startup/config-read profile, not a complete stock/campaign validation profile.
An early failure may be installation-related; it cannot be attributed to mod
compatibility merely because `-modconfig` was requested.

Before runtime execution either resolve that discrepancy with the user's
decision or explicitly accept and record this incomplete profile for a narrow
startup/config-read experiment. A missing-campaign baseline must not silently
become a campaign test or a complete-stock claim.

## Reproduction and verification

Supply absolute `-ImagePath`, `-BaselineImagePath`, `-SkuDefinitionPath`,
`-LauncherPath` and `-ConfigPath` to `Test-Ra3Ep11SmokeTestPlan.ps1` under
PowerShell 7. Use `Get-Ra3Ep11SmokeTestPlan.ps1 -AsJson` for the two requests
and identities. The checked-in single-LF probe is the reviewed test fixture;
the planner also accepts bounded whitespace-only probes for later review.

The planner reads/rechecks the probe, reruns scoped installed-config inventory,
and pins the launcher. It rejects directives, empty/oversized content,
non-ASCII or oversized paths, quote/semicolon ambiguity, mismatched launcher
installation, and placement under the installed game tree. File readers retain
their reparse-path guards. The 240-character/64-byte limits are conservative
diagnostic policy, not universal recovered game limits.

Tests cover two detached whitespace fixtures, nine detached policy refusals,
the exact single-LF identity, both request arrays, unresolved gates and repeat
JSON. No game/launcher/native codec is executed, and no registry, stock config,
archive or asset is modified. These tests are separate from the 165 compiler
test groups, which are not rerun by this milestone.

## Remaining runtime evidence

1. Capture the actual child executable/version, PID/creation identity and raw
   command line for each case. The [scoped observer](RA3EP11_LAUNCHER_PROFILE.md)
   is available, but short-lived parents or indirect children may require a
   different process-start capture. No live observation has occurred yet.
2. Validate a file-I/O observation method before interpreting the probe run.
   Require successful reads of this exact path attributable to the selected
   1.1 process; an existence check/open alone is weaker than content reading.
   Argument presence and a menu screenshot are not config-consumption proof.
   No file-I/O capture tool/profile has yet been validated. A PATH lookup found
   Windows `wpr.exe` but no Procmon executable; this is not proof that no such
   tool exists elsewhere, or that a usable scoped trace is configured. Read-only
   `wpr -profiles` / `-profiledetails FileIO` inspection found the installed
   `FileIO.Verbose.Memory` profile, including FileRead/FileOpEnd stacks and
   process/thread keywords. It also includes unrelated system/event providers
   and large buffer settings. This default is not a validated minimal scoped
   game trace; no recording session was started. Narrow capture design, event
   decoding and read/result-to-path/PID correlation remain required before
   enabling tracing; default profile availability does not close that gate.
   Follow-up: a [single-provider/process-name/five-event candidate](RA3EP11_CONFIG_TRACE_CANDIDATE.md)
   is now accepted by WPR's metadata commands and scope mutation tests. Its
   4 MiB configured buffer product replaces the broad default request, not
   runtime validation. A non-game controlled capture must verify live filtering
   and file/read/completion correlation before the game probe is attempted.
3. Record baseline and probe outcomes separately, including early failures;
   repeat a successful case before marking the config-read gate satisfied.
4. Only afterward prepare an unchanged-stock package-loading configuration.
   This probe does not exercise BIG mounting, manifest selection or authored
   XML. The ArmorTemplate authored-asset gates remain separate.

Overall SDK effort remains **52% / 48%**, zero proven game loading. Phase A
now has an inert external fixture and checked request plan, but it is not
completed and its original 2–4-session planning target is not a guaranteed
runtime-test date.
