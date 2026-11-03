# Production source graph scaling

<!-- Reborn: fixture quotas must never determine the maximum supported size of an authored mod. -->
The old 32-depth/512-source GraphLimit was a diagnostic profile allowance, not an
EP1 game format requirement. Studio's source path now enters a production source
transaction automatically. There is no new user-configured replacement ceiling.

`SdkSourceScale` scopes the real routing, dependency audit, source preparation and
SDK armor frontend. Diagnostic APIs used directly by isolated regression fixtures
keep their existing quota/refusal controls; these do not constrain Studio builds.
The scope is thread-local and disposed after success or failure.
Supported armor-only SDK inputs also select the common scalable mixed frontend,
rather than the older focused armor fixture bridge.

## Removed production quotas

- Source and Include counts; Include traversal depth; resource inventory counts.
- Per-source XML byte allowances and aggregate captured/prepared XML allowances.
- Definition-source, visible definition, evaluated definition and substitution counts.
- Direct-instance Include/handle counts and aggregate expanded inheritance bytes.
- Source element/asset counters and fixture fragment count/byte/depth allowances.

Source path/language validity, hash/precedence replay, DTD prohibition, XSD
validation, actual active cycles and native format checks remain in force.
An incomplete or erroneous graph is never silently accepted as a successful build.

## Traversal and semantics

Production Include capture uses explicit heap-backed frames, retaining declared
edge order and active-cycle detection. All-Include emission uses a lookup table
and iterative postorder traversal. Routing is iterative too.

Instance dependencies, local inheritance bases and imported definition tables are
prepared in dependency order so consumers reuse prepared children. Arbitrary
chain length is no longer interpreted as an invalid source graph. Actual cycles
still fail. All/instance visibility and publication roles are preserved.

An empty namespace-valid ART placeholder contributes no definitions or assets.
In particular, an all-Include of such a file is a no-op during inheritance, not a
malformed definition-only leaf. This does not invent or resolve a missing asset.

## Evidence

Developer checks are explicit maintenance work, not automatic Studio build tests.
`scripts/Test-Ra3Ep1SourceScale.ps1` uses only a fresh private SDK work directory.
The following cases passed:

| Case | Result |
| --- | --- |
| 701 distinct XML sources and 5200 Include edges | Complete preparation |
| 700 emitted GameScriptList assets | Native compilation, stream/package readback |
| Dependency preflight over that same graph | Complete; 700 mod identities |
| 600 nested Include edges | Preparation without recursive emitter stack growth |
| 2048 imported definitions | Complete table; expression resolved correctly |
| Six 6-MiB XML source files | Preparation above old file and aggregate allowances |
| 300 inherited UpgradeTemplate assets | Final value preserved |
| 132 Includes with an external base and empty all-import | Value/visibility preserved |
| Actual Include cycle | Rejected as IncludeCycle, not GraphLimit |

Evidence: `alpha/.work/source-scale-02.log`, its `wide-audit-final.json`, and
`alpha/.work/source-scale-external-01/test.log`.

The user's current SampleMod Romania map prepared 685 XML sources (757 receipts
including 72 terrain resources), producing 80 authored assets. The extra authored
Conscript comes from the user's overrides.xml. Native compilation and stream
readback passed privately; see `alpha/.work/map-unrestricted-sources-02` and
`alpha/.work/map-unrestricted-native-01.log`. No user source, Documents mod output
or game installation was modified and no game was launched.

## Actual remaining scale constraints

This change removes source fixture quotas. It does not remove physical memory
requirements or native/archive representation ranges. The current compiler is
x86, and source snapshots, DOMs and some native/package buffers remain resident
in memory. Very large builds can still encounter a genuine allocation failure.
Further scaling work is a streaming/ownership review and a compatible 64-bit host
strategy, including isolation of any required 32-bit native tools. Merely raising
a constant or changing the build platform would not provide that architecture.

The validation sizes above are measurements, not configured maximums or a claim
that every unrelated processor's unimplemented layout has been enabled.
