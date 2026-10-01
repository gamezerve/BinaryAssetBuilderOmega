# Two-family diagnostic stream round trip

## Outcome and boundary

`modifier-shader-stream-self-test` now compiles a resolved included ShaderOverride
and its parent AttributeModifier, writes a linked v7 diagnostic stream and reads
all metadata/native slices back through ManifestReader and Utility.Manifest.
Local shader and external FX identities survive normalization, resolution,
compiler entry, serialized manifest references and native import selection.

The writer is private to a fixed synthetic self-test and uses only its own fresh
temporary directory. It is not an arbitrary input/output SDK command. It does not
call OutputManager.CommitManifest, LinkStream, production sorting, intermediate
asset/cache commits or packaging. Both mapped experimental plugins still reject
production before missing source/cache access. No type-table readiness claim is
made by the target aggregate. Generated directories include DIAGNOSTIC_ONLY.txt.

## Inputs, identities and serialized output

The parent contains one synthetic modifier with StartFX=RebornStreamFX and
Shader=ShaderOverride_ObjectsIronCurtain. An instance Include supplies the literal
IronCurtain shader from ShaderOverrideProbe.xml. External FX lookup uses a tiny
synthetic manifest; FX native compilation is not implemented or claimed.

The real document stage assigns observed EP1 type/processing metadata and source
instance hashes. The private dependency-stage seam resolves the local shader and
external FX. The explicitly selected output order is shader, then modifier; this
is a two-node fixture order, not validation of the production stable sorter.

| Output asset | BIN/RELO/IMP | Absolute linked offsets BIN/RELO/IMP |
|---|---|---|
| ShaderOverride_ObjectsIronCurtain | 180/52/0 | 8/8/8 |
| RebornStreamModifier | 60/8/12 | 188/60/8 |

Each stream begins with its four-byte magic and four-byte checksum. Total file
sizes are BIN 248, RELO 68 and IMP 20 bytes. Manifest maxima are 180/52/12, total
instance data 240, two entries and a sixteen-byte ordered asset-reference buffer.
Native Tokenized flags are zero. Name/source offsets and exact type/instance
hashes are serialized with the existing Utility.ManifestHeader and AssetEntry
writers. Runtime external reference uses ReferencedFileBuffer's normal role and
name `external.manifest`; it is a runtime-name serialization fixture, not a
packaged external stream. The physical lookup manifest resides outside each
generated first/repeat stream directory and contains no FX BIN data.

The checksum comes from the exact existing ComputeOutputChecksum helper over
the explicit ordered declarations, retaining its capacity-padded identity
algorithm. It is not a payload digest or a newly reconstructed EA type hash.
InstanceHash values come from the real focused document stage, not fixed zero
test markers; complete production cache identity compatibility is still unproven.

## Independent readback and corruption rejection

The inspector checks header/aggregate, totals, maxima, local entries, type and
instance hashes, ordered dependency identities, native flags and external runtime
reference. Utility.Manifest independently reads the same entries and dependencies
and must report identical absolute chunk offsets. Every bounded BIN/RELO/IMP
slice matches the actual compiler buffer. Decoding the modifier's one-biased
selectors against the parsed dependency table chooses the intended external FX
and the first local shader entry.

A second serialization of the same declarations is byte-for-byte identical across
all four files. Four diagnostic-owned mutations must fail verification, then
restore original bytes:

- BIN checksum word changed while its native payload stays intact.
- Shader selector changed to invalid dependency index 3 in a two-slot table.
- IMP truncated by one byte, violating its exact expected length.
- Manifest's two dependency IDs exchanged with entry sizes/counts/checksum intact.

The last case matters because the official identity checksum includes reference
count but not dependency identities. Explicit reference-table/native checks are
necessary; equal checksum alone is not a target-resolution proof. The corruption
checks are fixture readback guards, not a general production semantic stream
validator. Some failures are caught by exact baseline equality before the decoded
selector check; no checksum-only integrity claim is made.

## Actual Utility.Manifest offset bug fixed

The new readback initially failed with an explicit message: IronCurtain utility
offsets were 4/4/4 while the prefixed EP1 streams require 8/8/8. Utility.Manifest
still initialized linked offsets with a hard-coded four-byte checksum prefix.
It now initializes them as containerPrefixSize + sizeof(uint), so the observed
four-byte EP1 manifest prefix yields the required eight-byte stream prefix.

A metadata-only unprefixed v7 probe still reports 4/4/4 and second-asset offsets
184/56/4. The compiled VERSION7 reader still rejects v6 as Unsupported file
version; this work does not add legacy version support. Those probes contain no
legacy/native payload compatibility evidence. No stream format, compiler output,
processing stamp, cache format or production policy was changed by the offset fix.

## Implementation and verification

- `source/BinaryAssetBuilder.ManifestInspector/ModifierShaderStreamSmokeTest.cs`:
  Run builds the fixed resolved graph; Write uses utility/header serializers;
  Verify compares both readers/slices/selectors; CheckCorruption restores owned
  fixtures; CheckLegacyOffsets guards unprefixed-v7 behavior and v6 rejection.
- `source/BinaryAssetBuilder.Utility/BinaryAssetBuilder/Utility/Manifest.cs`:
  Load initializes linked offsets from the detected container prefix.
- CompilerSmokeTest and Program register the new fixture-only test command.

Release/x86 inspector and main builder builds pass; all 69 compiler groups,
layout tests and 33 schema enum mappings pass. Model inventory remains 785/1,390
and typed marshallers 762/1,390. Overall effort remains approximately 50%.
Original user XML/schema/game files are unchanged. No game BIN is needed for
this writer test; the separate stock native proofs remain distinct evidence.

## Next gate

Replace fixture-only serialization with a deliberately bounded diagnostic build
entry that validates its admitted root universe, dependency targets and runtime
mapping before publication, with no partial output on rejection. Before any
production release, complete target registry/AllTypesHash, production graph sorting
and link lifecycle, FX custom processing, inheritance/cache policy, SDK and
WorldBuilder packaging and actual Uprising runtime loading still require proof.
