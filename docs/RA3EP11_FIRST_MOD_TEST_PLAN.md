# First Uprising mod-test plan

## Timing: a target, not a promise

We do not need a complete SDK to begin a narrow controlled loading test.
The planning target is **2–4 focused work sessions** to prepare the first
config/stock-loading smoke test. A session here means a substantive milestone
with validation, not each short continuation message. This is a manual estimate
from current evidence, not a measured completion date or commitment. Launcher
behavior, installation completeness or unexpected runtime failures may extend it.

A newly authored XML asset that demonstrably changes the game is a separate
test. No defensible date is available for that yet: the selected type's native
serialization/tokenization, dependency closure and valid runtime hash/manifest
contract still need proof. We will not wait for all 1,390 complex types, but
will not replace missing hashes with zero or relax production guards either.

## Phase A: config and unchanged-stock loading

### Keep launch selection separate from stream variants

A selected mod `.skudef`, a launcher's optional `-runver` request, SKU
`set-exe` selection and a stream `.version` suffix serve different roles.
The first EP1 plan remains pinned to the existing SKU's
`Data\ra3ep1_1.1.game` declaration and requires observing the actual child.
Do not add `-runver 1.1` merely because a RA3 launcher uses `-runver`;
EP1 forwarding and version selection must be independently verified.
Use an explicit child WorkingDirectory and disclose any child PATH changes;
do not change the observer's own working directory or saved game settings.
An application's started-process exit event does not independently describe
the final native game child's lifetime or prove config consumption.

Helper-only CDB calibration now includes separate x64 and x86 recipes. Before any
game-under-debugger trial, define the x86 argument/return observation,
ownership, cleanup and instrumentation impact separately. The WPR candidate
remains rejected. Private reference-source audits stay local; they are not
bundled into this SDK or treated as working EP1 launcher implementations.

Goal: prove the selected 1.1 launch path actually consumes the isolated config
and reaches existing valid stock assets. This is not an authored-asset test.

Remaining preparation:

1. Bounded config discovery is complete for the selected root/Data directories
   and originally 13 available configured archives: [config inventory](RA3EP11_CONFIG_INVENTORY.md)
   found four loose configs and no `filesystem.cfg`. This is not global absence
   or proof of runtime fallback. Resolve actual joined roots/additional config
   candidates and reconcile later updates with the intended test config; do not
   infer effective precedence from SKU order or create a stock replacement.
2. Verify launcher-to-1.1 argument forwarding and define a reproducible baseline
   launch plus isolated config launch. The [launcher profile and read-only
   observer](RA3EP11_LAUNCHER_PROFILE.md) now pin static literals/imports and
   prepare a scoped child-path/command-line observation. The `.bind` entry and
   unavailable reviewed consumer flow leave actual forwarding unproved; use
   runtime observation rather than infer it from strings. Keep test artifacts in a new workspace
   directory; do not patch executables, change registry settings or overwrite
   installed configs/archives.
3. The [user-authorized campaign restoration](RA3EP11_CAMPAIGN_RESTORE.md)
   restored the missing configured archive without overwrite or byte changes.
   Known loose metadata/map files remain; disclose them and resolve the baseline
   isolation decision before attributing a launch result to a new mod.
4. Choose an observable read/load signal. A main menu screenshot alone cannot
   prove `-modconfig` was consumed: require a supported log, attributable file
   access observation or another independently validated signal.

Run baseline and isolated-config cases separately and record exact executable,
SKU/config identities, arguments, observable result and failure symptoms.
Successful entry to a menu is startup evidence only. Keep rollback trivial:
the isolated configuration is not the installation's active stock config.
No game launch has been performed by the current audit sequence.

### First real observation: execution and stop policy still require review

Before a game-under-debugger run, explicitly approve that instrumentation.
Do not attach to an existing process or use the helper runner as a game runner.
A newly owned launcher/child observation must identify exact file identities,
creation time and ancestry, not merely a process name or reused PID. Keep
baseline and probe separate. Do not automatically launch both or retry failures.
No blanket child-debug flag, system-wide trace, injected library, binary patch,
registry edit, save overwrite or campaign-file rename is part of this plan.

Define a bounded deadline and log budget, plus shutdown of only owned debugger
and verified owned game process(es). If ancestry/ownership is lost, stop
automated intervention and report it rather than killing matching names.
Unexpected x86 breakpoint/return errors, asynchronous completion, ambiguous
handle reuse, non-target read correlation, missing expected child or fixture
identity changes must fail observation, not be treated as config consumption.
The helper recipe's global scratch registers and all-open logging are not a
general concurrent-game tracer; an exact-target/thread-aware design remains
necessary. Starting a game may write ordinary user preferences/logs or trigger
Steam behavior even without changing installed files; disclose this before run.

The user authorized restoration of the original Disabled-named campaign archive;
it was restored without overwrite or byte changes. Four loose `Data/mapmetadata.*`
files and 118 files under `Data/maps` remain untouched. Archive completeness
does not establish a clean stock baseline or effective override precedence.
Separately decide whether to test that modified baseline or reversibly isolate
the exact loose namespace; no blanket deletion is authorized. Restoration
does not by itself authorize a debugger-under-game run. Runtime test
timing now depends on these choices and observation implementation; the earlier
2–4-session target is historical planning, not a renewed countdown.

Prepared next experiment: [inert config-read plan](RA3EP11_CONFIG_READ_SMOKE_PLAN.md).
A single-LF external fixture and checked baseline/probe request arrays now
exist. The probe has zero directives; it targets config consumption first,
not effective mod paths or asset-package loading. Actual child/version capture,
a validated attributable file-read signal and the incomplete-stock decision
remain before runtime validation. No launcher invocation is generated or run
by the planner.

## Phase B: first narrowly authored asset

Prefer a simple already-characterized type rather than starting with music.
[ArmorTemplate tokenized PoC](RA3EP1_ARMOR_TOKEN_POC.md) is a candidate because
controlled serializer output has matched two bounded stock samples. That is
not a current game-load result or automatic authorization to publish its
experimental output as a compatible mod.

For the selected type, require:

- Fresh source/schema validation and the selected serializer/tokenizer tests.
- Stock-correlated type identity and a valid selected runtime hash/flag path;
  no fabricated compiler hash, zero-hash bypass or tokenized-flag relabeling.
- Correct manifest/BIN/RELO/IMP checksums, lengths, references and dependencies.
- A package readback independent of the writer and a deterministic, reversible
  in-game change that can distinguish this asset from unchanged stock.
- A baseline control and repeat load, with crash/load errors recorded rather
  than inferred away from static evidence.

If the selected tokenized type has a valid independently proved runtime path,
it need not wait for a universal ProcessingHash generator. Proving that path
is required; the existence of a conditional native branch alone is insufficient.
Music remains a later target because EP1 AUDIO inputs and authentic PathMusic
compiler identity are unresolved. Existing diagnostic music packages with
synthetic hashes are not playable-mod candidates.

## Reporting progress

Overall SDK effort remains approximately **52% / 48%**. These audits reduce
uncertainty in startup/loading but do not earn a successful mod-load percentage.
At the first runtime test, report Phase A independently of SDK readiness; at
the first authored change, report the selected type and exact tested scope.
The next priority is Phase A preparation, rather than indefinite broad reverse
engineering before attempting any controlled loading experiment.
