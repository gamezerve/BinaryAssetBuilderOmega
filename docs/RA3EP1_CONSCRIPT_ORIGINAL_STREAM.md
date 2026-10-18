# Actual original Conscript: scoped diagnostic linked streams

## Outcome — October 18, 2026

The complete independently qualified original Conscript native/metadata pair
now reaches fresh manifest/bin/relo/imp files and independent readback. This
is a scoped diagnostic stream, not yet a playable GameObject SDK capability.

| Proof | Result |
| --- | ---: |
| Local tokenized GameObject entries | 1 |
| Metadata prefix | 88,824 bytes |
| Original root fields / disjoint metadata owners | 126 / 21 |
| Shared strong reference occurrences | 275 |
| External metadata/runtime mappings | 2 |
| BIN / RELO / IMP payload bytes | 128,184 / 1,800 / 1,104 |
| BIN / RELO / IMP file bytes, including headers | 128,192 / 1,808 / 1,112 |
| Stream/dependency/profile refusals | 580 |

The complete [native](RA3EP1_CONSCRIPT_ORIGINAL_NATIVE.md) and
[metadata](RA3EP1_CONSCRIPT_ORIGINAL_METADATA.md) proofs run first. Only their
owned qualified output is passed to the new stream test; it is not silently
recompiled through an unqualified GameObject plugin. The one previously
bounded Script CRLF/LF equivalence remains unchanged.

## Explicit scope and identity

`source/BinaryAssetBuilder.ManifestInspector/GameObjectOriginalStreamSmokeTest.cs`
receives the qualified pair and the real static/global manifest paths.
`BoundedDiagnosticBuild.SerializeOriginalConscript` is a distinct internal
diagnostic entry point. It requires the original GameObject name/instance ID,
type 942FFF2D, type hash CEB9FE36, nonzero instance hash, exact qualified chunk
sizes and 275 validated references identical in order to Core normalization's
reference occurrences. Default `Serialize(..., tokenized: true)` still refuses
GameObject; ordinary armor/native-family routes retain their prior admission.

The source was normalized through real Core code but was not fully processed
as a production AssetDeclarationDocument. The stream test therefore creates
an explicitly diagnostic document context with source filename
`OriginalConscript.normalized.xml`, and derives an **experimental instance
hash from normalized UTF-8 XML**. This is not a claim of official EA processing
hash equivalence or a production cache receipt. The input instance is not
mutated; the test uses a separate declaration/context and owned payload copies.

The actual Core output-checksum method is invoked, then independently checked
against its 256-byte-capacity identity packing: type ID, type hash, instance
ID, experimental instance hash and reference count. Payload integrity is not
inferred from this identity checksum; complete chunk readback is separate.

## External references

Both supplied physical manifests must validate as linked EP1 v7 catalogs with
the reviewed AllTypesHash and no patch-base references. Each of the 275 strong
occurrences must resolve to exactly one actual asset across those catalogs.
Repeated occurrences remain repeated; they are never deduplicated or replaced
with captured stock import indices.

The existing OutputManager physical/runtime mapping seam serializes explicit
`data\static.manifest` and `data\global.manifest` paths. Both source manifests
are SHA-256 bracketed across the proof. This qualifies catalog identities and
serialized runtime names, **not game-loader acceptance or the external assets'
native/transitive payload closure**. No external game payload is copied.

## Readback and refusal controls

Fresh `first` and `repeat` directories contain deterministic stream bytes and
an explicit diagnostic-only notice. Existing output is never overwritten.
The independent Manifest reader and Utility reader must agree on v7/link/type
registry identity, tokenized flag, names, source name, ordered references,
all chunk sizes and each stream's byte offset. Headers, checksum and full
payload bytes are checked. A manifest must end exactly at its declared source
name buffer; the forensic reader's tolerance for trailing data is not accepted
for newly serialized output. Every import selector is checked against the
original normalized source reference sequence.

Corruption controls alter both the stored file and the expected-byte snapshot,
so independent semantic/readback checks must detect the failure. The 580
refusals comprise default tokenized-GameObject rejection, occupied output,
20 stream/header/length/fingerprint corruptions, all 275 reference rows, all
275 native import selectors, missing and ambiguous catalogs, tokenization
relabeling, an unadmitted XML identity, a truncated qualified chunk, reversed
validated references and two wrong type hashes. Every damaged file is restored
in its freshly owned directory and final readback runs again.

## Verification evidence

`gameobject-original-native-self-test` now runs the complete native -> metadata
-> diagnostic stream chain. Focused batch
`artifacts/ConscriptOriginalStream-b/batch-report.json`: **74/74 groups passed
in 409.014 seconds**, four isolated workers. The actual-original group took
**77.205 seconds**. Previous all-module and selected-root groups passed in
**39.023 / 33.842 seconds**. Eight frozen runtime hashes stayed unchanged;
the batch build had zero warnings/errors. No Full run or game/installation
changes. Timings cover overlapping diagnostic cases, not a mod-build benchmark.

The earlier `ConscriptOriginalStream-a` attempt stopped at compilation because
a refusal control tried to assign a read-only instance ID. It started no
workers and did not run a stale executable. The control now constructs a
separate XML/declaration with a different identity. The successful fresh `-b`
batch is the verification evidence, not that failed build attempt.

## Next boundary

The subsequent [authored-trial gate](RA3EP1_CONSCRIPT_AUTHORED_TRIALS.md) now
qualifies two fixed source identities, BuildTime=1 and offline BIG/config
candidates through the previously observed default-reader variant route.
The stream-only diagnostic does not itself qualify those authored changes or
packages. General GameObjects in the exported alpha, includes/inheritance/
expressions, LUA, new art and gameplay remain separate. The new identity still
needs recruitment/spawn integration; both candidates need shorter-path runtime
preparation before an actual separately controlled game test.
