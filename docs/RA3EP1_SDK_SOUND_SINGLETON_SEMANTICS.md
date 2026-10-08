# Sound singleton conflict characterization

## Outcome (2026-10-08)

New managed-only `sdk-sound-singleton-semantics-self-test` characterizes the
unchanged Core against the admitted, fingerprint-reviewed EP1 effective schema.
It adds compiler group 147, not a preprocessing option. No graph admission
expands: the last measured global graph remains **395 valid / 1 blocked**.
Reference XML/XSD, Core implementation and native/game execution are unchanged.

The two pairs inventoried by the preceding
[expression-stage review](RA3EP1_SDK_INSTANCE_SOUND_OFFSETS.md) are reproduced
as owned in-memory fixtures, not normalized in the user's source files:

| Singleton | First Low / High | Second Low / High | Isolated Core result |
| --- | --- | --- | --- |
| BuildingInfiltrated1 PitchShift | -10 / -5 | -1 / 1 | -1 / 1 |
| StreetLampCrush NonInterruptibleTime | 0.0s / 0.5s | 0.0s / 0.8s | 0.0s / 0.8s |

These results concern isolated explicit field pairs. They do not establish
complete owner inheritance, raw-source pipeline acceptance or engine equivalence.

## Evidence and limits

`NodeJoiner.Override` first copies the base, then overlays each authored child
in order. `SelectSame` chooses the matching singleton name; `ReplaceXmlNode`
recursively overlays the existing child, and `ReplaceNodeAttributes` replaces
individual scalar values. This is field-level overlay, not wholesale last-node
replacement. These methods are in
`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/NodeJoiner.cs`.

For both RealRange/PitchShift and TimeRange/NonInterruptibleTime, tests pin:

- The raw duplicate pair fails the reviewed schema's singleton cardinality.
- The isolated merged complete pair has exactly one child with the second pair's
  explicit attributes and passes schema validation.
- Reversing occurrences changes the result; identical pairs coalesce.
- A later High-only child retains the earlier Low, including inherited Low.
  Keeping only the last source child is therefore not a general equivalent.
- Fully specified children override inherited Low/High; partial later overlays
  retain fields from that earlier complete overlay.
- A reversed Low/High range can still pass schema validation. Valid XML alone
  neither establishes range semantics nor proves preservation of authored intent.
- Mixed PitchShift/NonInterruptibleTime overlays retain schema sequence order.
- Existing audio-tree preparation still atomically refuses the raw repeats,
  without processed hashes or partial overlay witnesses.

The fixtures use the actual reviewed EP1 schema rather than a permissive custom
range schema. They run no native codec, serializer, stream generation or game.
They do not demonstrate that EA's toolchain accepts these invalid raw occurrences,
and do not authorize generic duplicate folding, dropping the first occurrence,
source edits, maxOccurs changes or reuse for other singleton shapes/commands.

Implementation:
`source/BinaryAssetBuilder.ManifestInspector/SdkSoundSingletonSemanticsSmokeTest.cs`
(`Run`, owned `Merge`/`Valid`, exact `Check` projection), registered in
`CompilerSmokeTest.cs` and exposed by `Program.cs`.

Validation: the focused test and all 147 compiler groups pass. Coverage remains
785/1390 complex models, 762/1390 typed marshallers and 48/48 EP1-only types.
A full rebuild has zero errors and three existing warnings: CS0649 in Core's
Set.cs and CS8632 in Ra3Ep1AudioEventPlugin.cs/Marshaler.Ep1AudioEvent.cs.
The new characterization test introduces no compiler warning.

## Selected stock native corroboration

Read-only `asset-bytes` selected the two exact AudioEvent owners in
`D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest`
and read only their small instance/relocation/import slices. The manifest SHA-256
is `08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
Both entries report TypeId `844D7B9F` and TypeHash `560C2E45`.
The global.bin file is 2,768,548 bytes; full-stream dumping/hashing was not used.
An independent bounded file read confirmed these little-endian range words:

| Owner / InstanceId | Stream offset / size | Pointer slot -> local range | Exact 8 bytes | Decoded range |
| --- | --- | --- | --- | --- |
| BuildingInfiltrated1 / 143F6474 | 606696 / 184 | 84 -> 168 | 000080BF0000803F | -1 / 1 |
| StreetLampCrush / B6FDC451 | 1369296 / 216 | 124 -> 208 | 00000000CDCC4C3F | 0 / 0.8 seconds (float32) |

Both pointer slots occur in the corresponding relocation tables; imported
AudioFile references are elsewhere, not these range slots. BuildingInfiltrated1
has one AudioFile reference; StreetLampCrush has three. Instance slice SHA-256:

- BuildingInfiltrated1:
  `7ACC3B3B7D946126837FDB1E171E3BC792456C3842DBF9A174C25D87D410406F`
- StreetLampCrush:
  `877D0E0A31331D4FE8661469BBCCEB9FB7E6F353C9179647054DA58E362491DF`

These specific independently observed words corroborate the isolated Core
results; they do not establish generic EA duplicate processing. A re-read of
the authored SoundEffects hash matches the preceding expression review:
`F054D586CAAB71C22AEEA42AA2A71CF91568913DA7FC50D5CE844AD191A2073C`.
Both owners inherit `AudioEvent:BaseSoundEffect`. Their complete authored bodies
also contain other children and sound references, deliberately not normalized
or admitted by these isolated tests.

## Next gate

The selected native ranges now corroborate the isolated pair results. Next,
check complete source-local base context and every owner field/child projection.
A separate opt-in admission would still
need a full predicted owner projection, unchanged-source witnesses, stale-input
rejection and atomic final schema/dependency binding. It must retain all prior
profile refusals and reject unsupported children, commands or multiplicity.

Until then SoundEffects remains blocked. Estimated effort stays **51% complete /
49% remaining**: this review removes uncertainty, not the source, native emission,
dependency, packaging, WorldBuilder or actual game-loading gates.
