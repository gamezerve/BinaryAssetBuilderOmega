# Reborn: actual Core armor identities in linked diagnostic streams

Date: October 11, 2026. Offline compilation only; no game/debugger execution.

## Implemented path

`Ep1ArmorDocumentSmokeTest.WriteCoreStream` now connects the official-schema
document pipeline and `Ra3Ep1ArmorPlugin` to the existing bounded diagnostic
stream serializer. It retains the declaration's actual TypeId, TypeHash,
InstanceId and **nonzero Core InstanceHash** without inventing replacements.
The existing dependency preparation must yield zero standalone dependencies.

It calls `AssetDeclarationDocument.ComputeOutputChecksum` and independently
checks the one-asset contract: five little-endian uint32 fields in a 20-byte
row, with the reviewed MemoryStream capacity padded to 256 bytes before
FastHash. It does not substitute a payload digest or fixed test checksum.
`OutputManager.WriteLinkedStreamHeader` remains the shared header writer.

`BoundedDiagnosticBuild.Serialize`/`Verify` accept explicit per-entry tokenized
flags; existing callers still default to all false. A true serialization flag
is restricted to the characterized armor TypeId/TypeHash fingerprint. The
general diagnostic command's source/schema/type admission is not widened.
Flags derive from the actual armor plugin metadata, not a relabeled raw chunk.

Each isolated directory contains `diagnostic.manifest/bin/relo/imp` and a
diagnostic notice. ManifestReader and Utility.Manifest independently check
metadata, tokenized flags, identities, references, offsets, linked headers,
lengths and payloads. Repeat output must match byte for byte.

## Actual control

Ignored local artifact: `RebornArmorPoc-4c3e40342a4646d2bec6eb51a5d8dbbb`.
The current-source build and all five PoC runner steps passed.
The updated report contract was also checked in
`RebornArmorPoc-492195469c6240819ed1e28c53b1aab7`: all five steps passed,
`CoreIdentityStreamsValidated=true` and 24 generated stream files (four new
Core cases plus two original golden-output families). Production/game flags
remain false.

| Document case | InstanceHash | Stream checksum | BIN payload |
| --- | --- | --- | ---: |
| Empty `DocumentArmor` | `57840A21` | `0DBBF161` | 128 |
| Unchanged repeat | `57840A21` | `0DBBF161` | 128 |
| Same ID, Default changed to 50 | `B11DED86` | `989F363F` | 128 |
| Populated `Ep1ArmorTokenProbe` | `E6F463CC` | `45D90ADD` | 744 |

These are **this project's current Core/profile values**, not asserted EA
stock identities. The edit preserves InstanceId but changes InstanceHash,
checksum and payload; the generated source is restored in `finally`. The
populated bytes still match the previously golden-validated tokenizer path.
The separate Static/WorldBuilder golden comparisons also pass unchanged.

Refusal checks cover mismatched flag counts, changed armor fingerprints,
verification as non-tokenized, and a corrupt BIN checksum even when the
expected byte snapshot is also changed. That last control must reach the
independent header validator, not merely file equality. Generated damaged
bytes are restored in `finally` and readback is repeated.

Five affected regression commands passed: bounded diagnostic build (including
its Include/filter/FX/Multisound/AudioEvent controls), modifier/FX stream,
Multisound/FX stream, diagnostic AudioEvent build and checksum/cache audit.
The entire 165-group compiler suite was not rerun in this follow-up; it passed
on the preceding PoC-runner commit. No new compiler group is declared here;
the existing armor document group gains these stream assertions.

## Remaining production boundary

This closes the diagnostic fixed-checksum/zero-InstanceHash limitation for the
new `document/core-*` output family. It does **not** call production
`OutputManager.CommitManifest`, intermediate asset commit or coordinated
linking. `CanWriteProductionOutput`, cache reuse and compiled-document reuse
remain disabled in `Ra3Ep1ArmorPlugin`; the production denial tests still pass.
Its ProcessingHash remains a project-local experimental revision. Neither
nonzero Core hashes nor a correct checksum proves runtime-compatible identity.

Next is a justified narrow production/experimental publication contract using
the actual commit/link path, followed by a reversible package and separately
authorized game test. No hash-gate bypass, fabricated EA hash or stock-file
replacement follows from these diagnostic streams. Overall SDK completeness
is not increased merely because this selected offline path now writes streams.
