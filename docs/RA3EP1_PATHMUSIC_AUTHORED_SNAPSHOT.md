# Local authored PathMusic snapshot — October 8, 2026

## Scope and result

The isolated music serializer now has a bounded authored input profile: one local
directory containing `events.xml` and `events.h`. This profile admits 1–8 direct
PathMusicEvent owners through the reviewed EP1 schema and prepares detached native
chunks using the previously proved Win32 tracker layout. It writes no package.

This is **not** a replacement for official AUDIO inputs. Reports explicitly retain
`OfficialAudioDependenciesResolved=false` and `ProductionBuildReady=false`.
The new diagnostic fingerprint is not a Core InstanceHash, stock processing hash,
TypeHash or an authenticated production compilation identity.

## Implementation

`source/BinaryAssetBuilder.ManifestInspector/PathMusicAuthoredSnapshot.cs`:

- `Read`: bounded, reparse-checked reads of fixed local leaves; strict UTF-8 XML
  without BOM; DTD prohibited; reviewed schema validation and FileReference binding.
  Only direct empty authored events with safe IDs, exact `events.h` and supported
  attributes are admitted. Includes, inheritance, runtime roots, path aliases,
  AUDIO roots and external alternate references are outside this profile.
- Name uniqueness checks both ordinal spellings and actual SAGE instance IDs.
  Optional alternates must exactly name an admitted owner. Self references and
  cycles are not prohibited: these are weak IDs, not recursive compilation edges.
- Cache values are exact `true`, `false`, `1` or `0`; omission defaults to true.
  Whitespace-normalized boolean spellings refuse rather than being misinterpreted.
- `PathMusicRuntimeProbe.FromHeader` requires one canonical supported nonzero
  literal per owner. Missing, zero, duplicate and unsafe definitions refuse.
- `Preflight`: private raw source/header snapshots, copied immutable row records,
  complete native BIN/RELO hashes and versioned diagnostic content fingerprint.
- `VerifyCurrent`: reread/admit current inputs and compare raw bytes, not timestamps
  or file sizes. Valid edited inputs still invalidate an old snapshot.
- `Compile`: before/after current-input checks and freshly detached chunks,
  matching frozen native hashes and empty IMP. No processor or cache registration.

These checks detect ordinary edits but are not an atomic filesystem transaction.
No adversarial ABA/concurrent-write proof or production hash binding is claimed.

## Commands and reproducible fixture

From the repository root, use the x86 Release inspector:

```text
pathmusic-authored-preflight tests/fixtures/PathMusicAuthoredProbe
pathmusic-authored-self-test
compiler-self-test
```

The checked-in fixture has two explicitly synthetic values (1 and 2), a local
alternate and false/default-true cache settings. It is not stock-derived and must
not be copied into official AUDIO directories. Preflight returns JSON only;
there is no output directory argument or production build command.

## Validation and remaining gate

Compiler group 155 covers 1/3/8 owners, deterministic content identities, copied
report rows and native buffers, same-length timestamp-preserving source/header
edits, stale rejection and refreshed preparation. Negative cases cover path/root,
Include/inheritance/runtime/DTD, ID collision, local alternate, cache spelling,
owner budget, missing/zero/duplicate/commented/malformed header definitions.
Focused tests and all **155 compiler groups pass**. The checked-in two-event
CLI fixture returns exit 0 and byte-identical repeated JSON reports; missing
arguments return exit 1. Incremental x86 Release build has zero warnings/errors.

The next bounded step is isolated package framing and independent readback for
this local profile, with explicit experimental identity handling. Authentic EP1
header/source dependencies, Core processing identity, production processor/type
table admission and in-game loading remain separate required gates.

Model/marshaller coverage remains 785/1,390 and 762/1,390 (EP1-only 48/48).
The last official expanded graph remains 398 validated XML / four AUDIO path
issues / 198 header occurrences; this local profile does not bypass those issues.
Engineering estimate remains **52% complete / 48% remaining**.
