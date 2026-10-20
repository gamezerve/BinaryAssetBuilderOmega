# RA3-compatible build log contract

<!-- Reborn: preserve the user's official SDK log as an explicit compatibility target, not a claim that these stages are already implemented. -->

## Status and reference

Requested October 20, 2026. The user supplied an official RA3 SDK Studio build
log for `Deneme`. Its presentation and build-stage vocabulary are the target
for the Uprising SDK. This document records requirements; it does not enable
missing compiler features or change the current Studio adapter.

The reference was read from attachment
`891397d0-210f-4909-946c-47874458cc9e/Yapıştırılan metin.txt`.
The large machine-specific transcript is not copied into the repository.

Implemented presentation checkpoint: the live repository driver now emits all
eleven numbered phases, phase durations, explicit skipped-stage reasons, streamed
source/diagnostic lines, output paths and failure-phase summaries. Its backend
still combines native compilation, qualification and initial archive generation
inside step 6; steps 8 and 10 describe receipt checks and staging honestly.
This is presentation compatibility, not eleven independent compiler features.
Full raw backend output is retained at `alpha/.work/last-detail.log` on success
and `alpha/.work/last-failure-detail.log` on backend failure. The four public
output files are unchanged; `last-build.log` is now the readable transcript.
`alpha/.work/last-failure.log` contains the readable current failure. A failure
before backend execution may have no raw compiler detail log.

Studio's existing host still adds its own outer step duration and total build
duration. Backend phase timings are nested within that host operation. The
saved readable transcript includes its own completion and total duration.

## Studio envelope and phase order

Studio starts with `Building Mod <name>`. The reference has these phases:

| Step | Reference label | Required behavior |
| --- | --- | --- |
| 1 | Clearing build cache | Report actual cache cleanup or an explicit skip. |
| 2 | Clearing session cache | Report actual session cleanup or an explicit skip. |
| 3 | Clearing built mod | Prepare owned staging; retain the last good public output until successful replacement. |
| 4 | Building AptUI | Compile requested UI inputs; absence and unsupported requested inputs must be distinguished. |
| 5 | Building global data | Compile requested global data or identify verified precompiled dependencies. |
| 6 | Building static data | Resolve source, definitions, inheritance and schemas, then compile mod assets. |
| 7 | Merging assets | Report actual merge work, not a placeholder success. |
| 8 | Fixing assets | Report actual EP1-specific validation/transformation and counts. |
| 9 | Building official maps | Process requested map overrides, with per-map diagnostics. |
| 10 | Creating big files | Report archive creation and actual entry counts. |
| 11 | Creating skudef file | Report SKUDef creation and final package locations. |

Each executed phase emits `Step <n>: <label>` and
`Step time taken: <TimeSpan>`. Skipped phases explain why. A requested feature
that is unsupported fails instead of being silently skipped. Do not claim
cache cleanup, AptUI compilation, map compilation or legacy postprocessing
when the alpha has not performed it. Skips retain stable phase numbering.

Successful completion uses `Build Completed` and
`Build time taken: <TimeSpan>`. A failed build uses an unambiguous failure
summary, includes elapsed time and the failing phase, returns failure to
Studio, and never presents an older package as a newly successful build.
Cancellation is distinct from success and failure.

Studio owns its envelope and timing when it supplies them. The backend must
not duplicate `Building Mod`, final completion or timing lines in that view.
Phase timing must describe the real operation even while several operations
share one backend executable.

## Compiler detail

Use elapsed-time prefixes like `[00:00:00.0012160]` for compiler events. Include:

- The actual command line and selected source/search roots, without secrets.
- `BinaryAssetBuilder started` and a truthful completion/failure event.
- Source loading and the selected mod-first or SDK fallback path.
- XML/schema errors with source path and line/column where available.
- Include, definition and inheritance diagnostics naming the source and asset.
- `Compiling <Type>:<Id> [<type hash>:<instance hash>]`, using actual identities
  and hashes, not fabricated values or stock counts.
- Reference resolution, schema/asset dependency sorting, stream generation,
  and BIN/IMP/RELO linking when those operations actually execute.
- Per-type instance counts, total processing duration, longest instance and
  average duration; total processing duration across types.

Total per-type work time need not equal wall-clock time under parallelism.
Timing scope and interleaving must remain understandable. Emit complete lines
and expose progress while work runs rather than replaying everything at exit.
Warnings/errors must continue to populate Studio's Error Log tree.
Normal output should not dump internal JSON receipts; keep receipts on disk
and log their paths. Detailed diagnostics remain available separately.

## Postprocessors and output safety

The reference names `Hash Fix`, `Mod Asset Resolver`, `Stream Sorter`,
`LoD Stream Builder`, and map `Reference Fix`, with counts such as
`Instance hashes fixed`, `Assets prepended`, `Streams built`,
`References added`, and `References deleted`.

These identify responsibilities to investigate, not permission to run RA3
binary-mutating tools on EP1 streams. EP1-native correct output may not need
the same repair pass. In that case report validation/no repair required,
rather than falsely claiming that a fix ran. Counts must come from measured
operations and final stream readback.

The supplied reference reports a missing input as treated empty and unknown
asset warnings, yet ends in `Build Completed`. Match its useful presentation,
not unsafe success semantics: required missing sources, broken inheritance,
invalid schemas and unresolved required references must fail. Optional or
verified stock/precompiled references require an explicit policy.

Keep the live SDK output contract: `alpha/builtmods/<name>/<name>.big`,
`mod_1.0.skudef`, `build.json` and `last-build.log`. No automatic CFG, install,
or game launch is introduced. Preserve previous successful output on failure.
Retain a complete current failure log separately, including phase and summary.

## Implementation and acceptance gates

1. Add shared phase/event reporting at actual compiler boundaries; keep the
   Studio single-entry workflow and cancellation/publication protections.
2. Wire frontend and asset processors to source-aware events and measured
   per-type statistics. Unsupported work stays explicit.
3. Add stream/postprocessor/archive events at the operations themselves;
   do not infer success from a child process starting or an old file existing.
4. Verify through `RebornTest`: successful build, malformed XML, missing
   include, inheritance failure, unsupported requested resource, cancellation,
   and failure preserving the previous package. User-owned `Reborn` is not a
   regression fixture.
5. Check both Studio tabs and saved logs for correct classification, complete
   lines, timing, no false completion, and no duplicated Studio envelope.

The existing Reborn inheritance failure remains a separate compiler blocker.
Formatting alone cannot make the original EA Conscript source buildable.
