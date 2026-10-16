# Batched offline validation

The workflow now uses one compiler build and a matrix of independent test
processes, instead of rebuilding and stopping after each small feature check.
`scripts/Test-Ra3Ep1Batch.ps1` collects all group results, even after a group
fails, then exits nonzero if any real group failed or compiler files changed.
Each group has a separate log and owned TEMP/TMP directory; Core static
settings/caches do not cross process boundaries. Default concurrency is two
workers, configurable from one to four. Concurrency is not multiple agents.

Current Focused profile: **58** groups by default, **62** when the external
official XML root is explicitly supplied through `-UprisingXmlRoot`. The extra
groups compile both complete Conscript scripted-draw native graphs, 288 particle
controls, animation defaults and detached/source/dependency refusals. It also
executes the existing combined animation/particle compiler fixture directly.
The Full switch still adds its two separate broad groups; Focused results do not
claim a Full run. The token group independently verifies both complete recursive
stock metadata prefixes, full-source edits, 66 malformed native and 14,405
detached-output controls. See [native evidence](RA3EP1_CONSCRIPT_SCRIPTED_DRAW_NATIVE.md)
and [metadata evidence](RA3EP1_CONSCRIPT_SCRIPTED_DRAW_TOKENS.md).
The shared-container group adds all four complete Draws on one Core occurrence
table/Tracker, independent whole-list metadata, 12 source cases and 14,682
refusals. See [shared Draws evidence](RA3EP1_CONSCRIPT_SHARED_DRAWS.md).
The selected root + complete Draws group adds 13 combined source cases on one
Core table/Tracker, up to 102 partial root fields/189 imports, and cross-container
identity/ownership/output refusals. See [root integration evidence](RA3EP1_GAMEOBJECT_DRAWS_COMPOSITION.md).

At the original milestone the focused matrix had 24 groups: layout, armor input, weapon input,
includes/definitions, definition subset, instance inheritance, expressions,
file-reference catalog, document reuse, dependency resolution, dependency hash,
checksum, authored weapon values, actual armor Core output and actual pair Core
output, general single-armor authoring, general multi-armor sets, the armor source bridge
and bounded stock/native and named-source GameObject ArmorSet projections,
plus complete stock root-name/offset inventory with explicit indirect/unmapped
native-member boundaries, selected module topology/inline AI marshal controls,
and three newly ported module payload/dispatch/combined-native-graph controls.
The new damage/motion group adds nested tracker/effect/status payloads, freefall
and actual Core-normalized modifier reference/import checks.
One group can contain many value/negative-control checks; group counts
are not a count of newly supported SDK features.

`-Full` adds the complete existing compiler suite and expanded authored armor
field/package regression (26 groups total). This mode is deliberately heavier;
use it for shared schema/native/token/output changes and release gates rather
than every documentation/adapter change. The focused matrix is the default
iteration gate. Batch scheduling does not change individual test assertions.

## Running

Use PowerShell 7 from the repository, with an existing output parent and a
fresh child path:

```powershell
# Reborn: run a parallel offline matrix with explicit stock metadata; no game launch or installation change.
& .\scripts\Test-Ra3Ep1Batch.ps1 `
  -OutputDirectory (Join-Path $PWD ('artifacts/Batch-'+[guid]::NewGuid().ToString('N'))) `
  -StaticManifest 'C:\YOUR\UNPACKED\static\data\static.manifest' `
  -GlobalManifest 'C:\YOUR\UNPACKED\global\data\global.manifest' `
  -Workers 2
```

Add `-Full` for the broader run. Stock streams/XML remain read-only. Output
must be fresh and its existing ancestry must not contain reparse points.
`batch-report.json` records per-group status, exit code, duration and log paths,
plus the hashes of eight compiler/runtime dependency files. The compiler is
built once before tests and those file hashes are checked after all tests.
Logs and diagnostic artifacts are retained for failed groups; nothing is
automatically deleted, installed or uploaded. A compiler build failure occurs
before scheduling and is recorded in compiler-build.log, not reported as a
completed matrix. Duration in the report covers workers, excluding the build.

`-SchedulerControl` requires no manifests/compiler build. Three disposable
PowerShell children return success, intentional exit 7 and success. The mode
returns success only if the expected failure and subsequent success are retained
in the aggregate report. Its two-passed/one-failed result is expected, not a
compiler regression. Serial and two-worker continuation controls are checked.

## Interpretation and next development batches

Every group carries an evidence label. Source includes, inheritance and
definitions are **DiagnosticOnly**: their passing tests do not enable these
features in the alpha's playable authoring path. Scoped armor/weapon stream and
package checks remain limited to their admitted shapes. Separate packages in a
batch are not a shared mixed-asset stream. No gameplay evidence is obtained by
this offline runner; actual game tests still need separate launch authorization.

The next implementation batch should connect reusable source semantics and
type/identity admission to actual Core output, rather than add another
hard-coded unit/field recipe. Test direct assets, new identities, multiple
assets, instance includes, inheritance, definitions and reference failures in
one matrix; keep per-capability publication gates until each is proven. Once
mixed-stream compilation works, use a gameplay package with independently
recognizable effects and controls. If a combined game test fails, split only
the failing subset, not the entire development workflow.

General GameObject, texture/model processing and generic weapon tokenization
are still separate implementation work, not unlocked by batching tests.

## Verified run — October 12, 2026

The final two-worker focused matrix passes **15/15 groups**, with no failures,
in approximately **28 seconds excluding the one compiler build**. All eight
compiler/runtime file hashes remain unchanged. Local report:
`artifacts/RebornBatchFinal-1b5408ff8e94419f8a3f70119c548460/batch-report.json`.
An earlier focused matrix also passes 15/15. Serial and two-worker scheduler
controls retain the expected failed child while completing the remaining
children; attempting to reuse the final output directory is refused without
changing its report. The broader `-Full` option was not rerun in this milestone;
it invokes the existing broad suite and authored armor regression. No new
gameplay/GUI/alpha feature support is claimed from this test-workflow change.
