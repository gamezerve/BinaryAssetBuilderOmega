# Isolated PathMusic runtime serialization — October 8, 2026

## Result

An isolated Win32 serializer using the **actual Relo.Tracker** now matches all
**197 selected stock music records**, including 19 relocated alternate records
and four zero-valued event words. Full BIN/RELO hashes match, with no imports.
Selected stock data is 3,228 BIN bytes and 152 RELO bytes per observation pass;
two command reports agree. No output directory or game stream is written.

This is a runtime-layout proof. The stock comparison deliberately feeds the
observed event words into an independent tracker serializer: **it does not prove
header recovery or authentic authored-header-to-stock-value derivation**.
StockValuesReplayed=true, HeaderRecovered=false, ProductionBuildReady=false.

Owned synthetic header fixtures separately demonstrate canonical literal →
native chunk serialization against independent hand-assembled byte goldens.
No stock-derived header or production processor is created.

## Implementation and contract

`source/BinaryAssetBuilder.ManifestInspector/PathMusicRuntimeProbe.cs`:

- `Runtime`: private explicit-layout 16-byte record, base at 0, event at 4,
  optional uint pointer at 8, one-byte cache flag at 12 with zeroed padding.
  It is deliberately outside production model/marshaller inventories.
- `EncodeObserved`: Win32 ABI/offset and signed-literal-range guards; actual
  tracker root allocation and optional four-byte weak-ID allocation.
  Native output is 16/0/0 without alternate or 20/8/0 with alternate, with RELO
  words `8, FFFFFFFF`. This observed-value path allows zero only for characterization.
- `FromHeader`: clone/scan a caller-owned header, require one canonical supported
  nonzero literal, inspect the full bounded header for duplicate reference-style
  matches, serialize, post-check caller bytes, return detached chunk and SHA-256.
  Missing, zero, duplicate, commented, suffixed and unsupported numeric definitions
  refuse. This is stricter than legacy warning-and-zero/first-match behavior.
- `CompareStock`: use the existing bounded exact source/stock reconciliation,
  serialize its observed values, compare complete native/relocation hashes and
  require empty imports. It does not register processors or create packages.

Public diagnostic commands:
`pathmusic-runtime-self-test` and
`pathmusic-runtime-layout-proof <PathMusicEvents.xml> <global.manifest>`.
The latter returns layout evidence only; a successful exit is not SDK/game readiness.

## Authored versus runtime schema boundary

The unchanged reviewed EP1 schema contains `PathMusicEventRuntime` as a complex
type, but **does not list it in AssetDeclaration's authored element choice**.
`CnC3Types.xsd` instead admits `PathMusicEvent`, whose header is a required
FileReference. `AssetTypePathMusic.xsd` describes the processed runtime type.
Therefore a future processor must transform authored events into runtime data;
adding a runtime root to source XML is not a valid substitute.

Group 154 explicitly proves authored PathMusicEvent/header shape admission and
runtime-root refusal. A test-only **in-memory** element referring to the unchanged
official runtime complex type validates EventNameHash, alternate/cache fields,
default-true cache insertion and missing-required-value rejection. This wrapper
is not saved to the official schema catalog or admitted by the production pipeline.

## Validation

`PathMusicRuntimeProbeSmokeTest` is compiler group **154**. It covers actual
tracker output versus independent 16/20-byte goldens, exact padding and auxiliary
sentinels, cache true/false, boundary values, repeated determinism, detached output
and input-edit behavior, strict header and invalid alternate refusal, and schema
source/runtime separation. Focused tests and all 154 compiler groups pass.
Missing layout-proof arguments return exit 1.

Stock source:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\PathMusic\PathMusicEvents.xml`.
SHA-256: `BBE869BE8AC6D6C9C127BAA8AEF9C039BE7C7C3B7D740D21DD25EC35F514E4DD`.

Stock manifest:
`D:\Temp\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest`.
SHA-256: `08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
TypeId/TypeHash remain `9A651D89/599CDAF2`, version 7 / AllTypesHash `5454A8E9`.
The existing stock reviewer repeats selected reads and post-checks source/manifest
snapshots and stream framing. This is bounded snapshot evidence, not an atomic
filesystem transaction or full-stream integrity proof.

The expanded source graph was rerun: **398 validated XML**, four AUDIO path
issues and 198 occurrences of the required header, exit 2, no output directory.
These failures are not suppressed by runtime layout success. Model/marshaller
coverage stays 785/1,390 and 762/1,390, EP1-only 48/48.
Initial dependency rebuild: three existing warnings, zero errors; incremental
Release build: zero warnings/errors. No reference audio DLL or native codec is
executed; the existing tracker uses its normal CRT memory allocation/copy helpers.
No official source/schema/Core/registry changes.

## Next gate and effort

Runtime layout is now independently corroborated, but authentic EP1 header
values, processor processing hashes/content dependency invalidation and source
closure remain unproved. Next establish a scoped authored input/identity contract
or independently validated precompiled stock binding, then isolated package
readback before any production registration or game-load claim.

Effort remains **52% complete / 48% remaining**: one isolated native layout does
not close complete dependency, type-table, production output or game-loading gates.
See [reference header semantics](RA3EP1_PATHMUSIC_HEADER_SEMANTICS.md).
