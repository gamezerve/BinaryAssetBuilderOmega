# Actual control ETL: read workload visible, process filter rejected

## Outcome

The user ran the reviewed control in an administrator session and saved
`artifacts/RebornConfigControl-090ea57d839e41569951a56df6fab856/control.etl`.
The helper results were positive PID 2456 and negative PID 43788, each
`READ|1|10`; recording elapsed time was 1,145 ms. No game was executed.

The saved ETL is 2,752,512 bytes, SHA-256
`6D2C1ECCE053540D5A5386D65EB6324DD495A1270F66F16ACD804D893C8E9C81`.
Read-only decoding found **both** the positive and negative helper's exact
probe-path create/read records, plus Kernel-File events attributed to 24 other
PIDs. Therefore the requested `ProcessExeFilter="RebornConfigReadControl.exe"`
**did not enforce the intended scope in this recording**. This is a real
counterexample, not an inconclusive missing-event case.

The game profile uses the same requested provider/filter mechanism, so it
must not be promoted to a safe scoped game trace. The actual cause is not
established here: XML acceptance alone did not establish filter enforcement.
`Invoke-Ra3Ep11ConfigReadControl.ps1 -Record` is now explicitly disabled before
any compilation/recording, even with administrator rights. Do not rerun the
previous capture command. Preflight, detached tests and helper-only validation
remain available; the saved record can be inspected read-only.

## Narrow evidence extracted

| Property | Observed count |
| --- | ---: |
| All decoded events | 4,287 |
| Kernel-File events | 4,199 |
| Other/unselected-provider records | 88 |
| NameCreate (10) | 10 |
| NameDelete (11) | 4 |
| Create (12) | 716 |
| Read (15) | 96 |
| OperationEnd (24) | 3,373 |

| Control | Exact probe read, UTC | Requested size/offset | Next selected same-PID/Irp completion candidate |
| --- | --- | --- | --- |
| Positive / 2456 | 18:50:22.3335150, October 9 | 1 / 0 | 18:50:22.3335277; Status 0; ExtraInformation 0x1 |
| Negative / 43788 | 18:50:22.1480543, October 9 | 1 / 0 | 18:50:22.1480754; Status 0; ExtraInformation 0x1 |

Create/read versions are 1; completion version is 0. The latest observed
Create's FileObject connects each read request to the exact captured native
probe path. The decoder accepts an explicitly supplied `\Device\...` path;
it does not assert a guessed drive-letter/device alias.

Completion candidates are strongly consistent with the known helper read,
but the five-ID selection excludes other operation types and IRP values are
visibly reused. Do not claim every intervening operation is known, infer
transferred bytes from IOSize alone, or universalize ExtraInformation meaning.
Lost-event statistics and independent path-alias validation are not completed.
None of these limitations weaken the observed negative-helper counterexample.

## Reproduction and privacy

`Get-Ra3Ep11ReadControlEvidence.ps1` takes absolute `-TracePath`, explicit
`-PositiveProcessId`, `-NegativeProcessId` and `-CapturedProbePath`. It caps
ETL input at 32 MiB and decoded events at 100,000, rejects reparse inputs,
holds a read-only handle denying writers, and hashes before/after decoding.
It reports only selected control read rows and aggregate counts, not unrelated
file names, computer metadata or command lines. Windows trace-provider access
may require the product's reviewed unsandboxed read even though no recorder
or game is executed.

`Test-Ra3Ep11ReadControlEvidence.ps1` pins this exact ETL identity/counts,
five detached correlation checks and repeat decoding. This regression deliberately
expects **scope rejection**. Tests do not generate a successful-filter verdict.
The latest-create map is invalidated by a replacement create for another path;
PID/path mismatch and wrong completion IRP must not produce a match.

Raw ETL and generated binary/stdout artifacts remain local and Git-ignored.
They are not staged, published or uploaded. The result demonstrates why:
the recording contains non-target activity despite the narrow requested XML.
No further recording session was started during this analysis.

## Next decision

Design a replacement that enforces capture-time isolation, or explicitly
request user approval for a short broader local recording with post-filtering
and disclose that it collects unrelated activity. Post-filtering is not a
privacy-equivalent substitute for capture-time filtering. Do not silently
remove the filter, broaden keywords or install a driver/tool to work around it.

Until one of those routes is validated, the first 1.1 config-consumption trial
remains pending. Read workload calibration has progressed; SDK effort remains
approximately **52% / 48%**, with no proven game mod loading.
