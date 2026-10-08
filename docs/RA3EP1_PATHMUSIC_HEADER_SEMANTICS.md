# Reference PathMusic header semantics — October 8, 2026

## Outcome

Static IL review found the actual header reader in
`BinaryAssetBuilder.AudioCompiler.Plugin`, not in a type named PathMusicEvent.
The runtime offset-4 event word is read from a hexadecimal header definition,
**not computed from the asset name's symbol hash**. The RA3 reference compiler
retains zero and warns when its header cannot be opened or its resulting event
value is zero. It then continues runtime serialization. This is reference behavior,
not authorization to suppress the EP1 graph's missing dependency.

New read-only commands:

- `pathmusic-reference-review <reference-audio.dll>` pins the exact reviewed DLL
  and reports method tokens/RVAs/IL hashes without loading/executing the DLL.
- `pathmusic-header-review <reference-audio.dll> <header.h> <event-id>` reviews an
  explicit caller-supplied header through a conservative diagnostic literal model.
- `pathmusic-header-semantics-self-test` runs owned in-memory lexical/guard fixtures.

No authentic `RA3EPMus.h` has been recovered. No production music processor,
registry entry, automatic header, zero fallback or source dependency bypass is added.

## Pinned reference evidence

Reference path under the repository:
`Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.AudioCompiler.dll`.
Size: 1,232,896 bytes. SHA-256:
`A14D15ADDEF502C2DF6F6BA272D6098C4C131FBEA9C3ED0149C11C4556347139`.

| Method | Token | RVA | IL bytes |
| --- | --- | --- | ---: |
| ParsePathMusicHeaderLine | 06000117 | 00002AA0 | 1618 |
| ProcessPathMusicEventInstance | 06000119 | 000050C4 | 670 |

Parser body SHA-256:
`E3766FE3AA433D84D33F2ADF5FA04CE82D6DC1923740D92484B5EA3D5E3E2CDE`.
Event processor body SHA-256:
`222D3F22A5922647259C626E8C2A7877B0EC861DDC05E9C8F9B2EBF8D416AFF0`.
Different DLL snapshots refuse before method interpretation. These are RA3
reference identities, not recovered EP1 processing/type hashes.

## Line parser control flow

`ParsePathMusicHeaderLine` takes prefix, input line, event name and integer output:

| IL range | Observed behavior |
| --- | --- |
| 0202–0212 | Find `#define` anywhere in the line, not necessarily at its start |
| 029D–02AE | Find supplied prefix (event caller supplies `PATH_EVENT_`) |
| 0339–034E | Find literal ASCII space starting at that prefix |
| 03D9–040B | Extract text after prefix up to that space; require exact event-name equality |
| 04C3–04D7 | Find first lowercase `0x` anywhere in the whole line |
| 058F–059F | `strtol` from that hex position, null end pointer, radix 16; write integer |
| 0650–0651 | Return true after this conversion |

Helpers `eastl.basic_string.find` (06000030), `eastl.search` (06000050) and
`eastl.operator==` (06000035) use byte comparisons without case folding.
Consequently this is not a C preprocessor: comments can contain matching text,
tabs are not interchangeable with the required name-terminating space, uppercase
`0X` does not satisfy the literal search, an earlier hex literal can be selected,
and trailing text is not validated through an end pointer. CRT invalid/overflow,
ANSI conversion and exact getline-boundary behavior remain outside our admitted model.

## Event processor control flow and zero handling

`ProcessPathMusicEventInstance`:

- IL_00AE–00AF initializes event value to zero; IL_00B1–010B reads
  PathfinderEventHeader and unmodified XML id, converting them to ANSI strings.
- IL_010D–0129 opens the header; IL_012B–0146 warns if it cannot be opened.
- IL_014D–0198 reads with a 512-character native getline buffer, calls the line
  parser with `PATH_EVENT_`, and stops at the first successful match, including zero.
- IL_019A–01C4 closes the file and warns if the resulting value is zero.
  Missing match and an explicitly zero definition both reach this warning.
- IL_01DA–01DD copies the four-byte base record. IL_01DF–0206 copies an optional
  weak alternate through a tracker allocation rooted at runtime offset 8.
- IL_0208–0211 copies IsCacheable to runtime offset 12; IL_0212–0217 stores the
  header integer at runtime offset 4, then endian-corrects/finalizes the tracker.

This independently explains the previously observed 16-byte plain / 20-byte
relocated alternate records. It provides a possible zero-producing path, **not
proof why the four particular stock EP1 events are zero**. Authentic EP1 header
contents and EP1 processor compatibility still need verification.

## Diagnostic model boundaries

`PathMusicHeaderSemantics.ReviewLine` reproduces the reviewed lexical selection
for safe ASCII names/lines only. It distinguishes legacy shape match from a
canonical standalone definition. Supported numeric observations are 1–8 hex
digits at or below signed Int32 maximum. Unsupported CRT results leave value and
warning prediction null; they are not presented as successful non-warning values.
Comments, earlier hex and trailing suffixes retain observed candidate values but
report CanonicalLiteral=false and a review reason, rather than gaining authority.

`Scan` stops on the first lexical match, distinguishing explicit zero from absent
definition. Maximums: 1 MiB header, 8,192 inspected lines, 510 characters per
line and 256 characters per safe event identifier. The 510-character limit is
deliberately conservative relative to the reference's 512-buffer getline; it does
not claim exact edge-case equivalence. NUL/non-ASCII/unsafe names reject. Header
reads use existing reparse/bounds checks and hash post-rechecks.

These are managed diagnostic fixtures derived from static review, **not a native
reference-execution oracle or general legacy parser equivalence proof**.
All outputs retain HeaderRecovered=false and ProductionBuildReady=false.

## Validation and remaining work

Compiler group **153** covers canonical hex, case/prefix/name/space differences,
comment/first-hex/suffix hazards, unsupported invalid/overflow numbers, first-match
and explicit-zero/missing warnings, unknown warning results, reference pinning and
ASCII/size/line/name limits. Focused fixtures and all 153 compiler groups pass.
Repeated reference reports agree. Missing command arguments and an unreviewed DLL
return exit 1. Initial full-suite sandbox execution hit the existing temporary-file
move restriction; the authorized rerun passes. Release incremental build has zero
warnings/errors; the initial dependency rebuild had three existing warnings.

`tests/fixtures/PathMusicHeaderLiteralProbe.h` is explicitly synthetic and uses
arbitrary Reborn probe values, not recovered stock constants. End-to-end header
CLI reviews of nonzero, first-zero-before-duplicate and missing probes each agree
on two runs. It is not copied into an AUDIO root or used to resolve any game asset.

Coverage remains 785/1,390 models, 762/1,390 marshallers and EP1-only 48/48.
Last expanded graph remains 398 validated XML, four AUDIO path issues and 198
occurrences of one required header. No Core/schema/reference/source/registry
changes and no native DLL execution. Effort remains **52% complete / 48% remaining**.

Next: recover authentic EP1 header input, or prove an explicitly scoped stock
identity/reference contract. Before a production processor, verify header/content
cache invalidation, duplicate/missing/zero policy, signed/range semantics and exact
native output against stock; do not inherit the legacy warning-and-zero policy
implicitly. See [stock event reconciliation](RA3EP1_PATHMUSIC_STOCK_REVIEW.md).
