# EP1 1.1 scoped startup config order

## Result and evidence

The reviewed startup segment reads the supplied mod config conditionally,
then probes additional config candidates in reverse vector order, then reads
`filesystem.cfg`. Consequently, reading a mod config is not proof that its
search-path settings remain effective after later startup updates.

Implementation: `scripts/Get-Ra3Ep11StartupConfigOrder.ps1`.
Regression: `scripts/Test-Ra3Ep11StartupConfigOrder.ps1`.
Prerequisites retain [search-pair rebuilding](RA3EP11_SEARCH_PATH_PAIRS.md) and
the fixed image/baseline/SKU/stock-directory identities.

| Reviewed slice VA | Raw offset | Bytes | Scope |
| --- | --- | ---: | --- |
| 004D985E | 0D985E | 77 | Nonempty modconfig buffer and probe gate |
| 004D99E1 | 0D99E1 | 229 | Reverse additional-candidate traversal |
| 004D9AC6 | 0D9AC6 | 215 | Base config read and synthetic fallback/language commands |

These are three **scoped slices**, not a complete startup-function pin.
Their independent SHA-256 hashes and five exact strings are checked. Full
image identity is verified before and after. No game/native code is executed.

## Ordering and conditionals

The modconfig buffer at `00CF2118` is checked for nonempty contents. A joined
candidate is probed through `004D6F10`; on success the subsequent startup code
calls config reader `004D86B0` at `004D98A6`. The previously reviewed config
consumer pins this downstream call; this audit's first slice ends at the probe
branch, so it does not claim to cover all intervening language setup.

Additional candidates come from vector begin/end globals `00CF499C/00CF49A0`.
The reviewed record stride is 70h (112 bytes). Traversal starts at count minus
one and decrements. Candidate processing includes the string `\mod.skudef`,
path preparation and a probe; only a successful probe calls the config reader.
The detached model records supplied labels in reverse order. It does not
guess the string helper's complete concatenation semantics, actual candidate
paths, vector initialization or which files exist.

The next stage joins and reads `filesystem.cfg` through `004D86B0` at
`004D9AE5`. If that read fails, startup synthesizes one of these exact formats
and passes it to line dispatcher `004D9040`:

- `set-search-path big:;%s`
- `set-search-path big:;%s\lang\%s;%s`

If the read succeeds, a flag at the supplied object+04h can instead cause
`add-search-path %s\lang\%s`. The formats and branches are established, but
the flag's complete provenance/meaning and actual formatted arguments are
not recovered. The detached helper receives branch outcomes, not native files
or an inferred runtime language.

The synthetic `set-search-path` route connects to the pair rebuilder already
audited. This explains why configuration read order is a practical mod-test
gate: later updates may replace earlier search pairs. It does not prove that
every mod setting is overwritten, or that a specific authored asset wins.

## Regression and boundary

Six detached ordering fixtures cover successful/absent mod probing, reverse
candidate order (including duplicates), base read success/failure and the
language-command branch. Three ASCII/length policy rejections and nineteen
private slice/string faults pass. Every stable nested field matches a second
JSON run. No whole startup reachability, effective override, native loading,
compiler compatibility or production-ready SDK claim is made.

Prerequisites cover thirteen available archives and 17,383 entries, reading
5,390,165 directory metadata bytes per invocation and zero archive payload
bytes. WorldBuilder's large BIN is not read/decompressed. The missing configured
MapsCampaign.big and its untouched Disabled-named neighbor remain separate.

See [first mod-test plan](RA3EP11_FIRST_MOD_TEST_PLAN.md) for practical exit
criteria. Effort remains approximately **52% complete / 48% remaining**.
The 165 compiler groups were not rerun for this PowerShell-only audit.
